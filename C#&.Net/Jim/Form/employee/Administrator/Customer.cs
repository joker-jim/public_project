using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jim
{

    public partial class Customer : Form
    {
        public Customer()
        {
            InitializeComponent();
            dataGridView1.Columns["id"].DataPropertyName = "id";
            dataGridView1.Columns["name"].DataPropertyName = "name";
            dataGridView1.Columns["address"].DataPropertyName = "address";
            dataGridView1.Columns["contact"].DataPropertyName = "contact";
            dataGridView1.Columns["password"].DataPropertyName = "password";
            dataGridView1.Columns["email"].DataPropertyName = "email";
            dataGridView1.Columns["phone"].DataPropertyName = "phone";
            dataGridView1.MultiSelect = true;
            dataGridView1.ScrollBars = ScrollBars.Both;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

        }
        public async void LoadCustomers()
        {
            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
            DataTable data = await dbHelper.GetCustomersAsync();
            dataGridView1.DataSource = data;

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");

            bool atLeastOneRowAdded = false;

            foreach (DataGridViewRow row in dataGridView1.SelectedRows) 
            {
                if (!row.IsNewRow)
                {
                    try
                    {
                        string name = row.Cells["name"].Value?.ToString();
                        string address = row.Cells["address"].Value?.ToString();
                        string contact = row.Cells["contact"].Value?.ToString();
                        string password = row.Cells["password"].Value?.ToString();
                        string email = row.Cells["email"].Value?.ToString();
                        string phone = row.Cells["phone"].Value?.ToString();

                      
                        await dbHelper.InsertCustomerAsync(name, address, contact, password, email, phone);
                        atLeastOneRowAdded = true;
                    }
                    catch (ArgumentException ex)
                    {
                        
                        MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; 
                    }
                }
            }

           
            if (atLeastOneRowAdded)
            {
                LoadCustomers(); 
                MessageBox.Show("Selected customers have been successfully added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }




        private async void button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    int id = Convert.ToInt32(row.Cells["id"].Value);
                    string name = row.Cells["name"].Value?.ToString();
                    string address = row.Cells["address"].Value?.ToString();
                    string contact = row.Cells["contact"].Value?.ToString();
                    string email = row.Cells["email"].Value?.ToString();
                    string phone = row.Cells["phone"].Value?.ToString();
                   

                    SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                   
                    await dbHelper.UpdateCustomerAsync(id, name, address, contact, email, phone);
                }
            }

            LoadCustomers();
        }


        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the selected customer(s)?", "Delete Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");

                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        int id = Convert.ToInt32(row.Cells["id"].Value);
                        bool success = await dbHelper.DeleteCustomerAsync(id);
                        if (success)
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

       

    }
}
