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
    public partial class Usermanagement : Form
    {
        private SQLiteDBHelper dbHelper;


        public Usermanagement()
        {
            InitializeComponent();
            dbHelper = new SQLiteDBHelper("database.db");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var dbHelper = new SQLiteDBHelper("database.db");
            string searchTerm = textBox1.Text;
            DataTable result = await dbHelper.SearchCustomerByNameOrContactAsync(searchTerm);
            dataGridView1.DataSource = result;
            dataGridView1.Columns["password"].ReadOnly = true;


        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var dbHelper = new SQLiteDBHelper("database.db");


            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string name = row.Cells["name"].Value?.ToString();
                        string address = row.Cells["address"].Value?.ToString();
                        string contact = row.Cells["contact"].Value?.ToString();
                        string password = row.Cells["password"].Value?.ToString();
                        string email = row.Cells["email"].Value?.ToString();
                        string phone = row.Cells["phone"].Value?.ToString();

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contact))
                        {
                            MessageBox.Show("Name and Contact are required!");
                            return;
                        }

                        bool added = await dbHelper.AddCustomerAsync(name, address, contact, password, email, phone);
                        if (!added)
                        {
                            MessageBox.Show($"A customer with name '{name}' or contact '{contact}' already exists.");
                            return;
                        }
                    } 
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return;
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                string originalName = row.Cells["name"].Value.ToString();
                string newName = row.Cells["name"].EditedFormattedValue.ToString();
                string address = row.Cells["address"].EditedFormattedValue.ToString();
                string contact = row.Cells["contact"].EditedFormattedValue.ToString();
                string email = row.Cells["email"].EditedFormattedValue.ToString();
                string phone = row.Cells["phone"].EditedFormattedValue.ToString();

                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                await dbHelper.UpdateCustomerAsync(originalName, newName, address, contact, email, phone);
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    string username = row.Cells["name"].Value.ToString();
                    string resultMessage = await dbHelper.DeleteCustomerAsync(username);
                    MessageBox.Show(resultMessage);
                }
            }
            else
            {
                MessageBox.Show("Please select at least one row to delete.");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
