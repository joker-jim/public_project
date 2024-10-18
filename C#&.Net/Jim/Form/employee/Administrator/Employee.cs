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
    public partial class Employee : Form
    {
        public async void LoadEmployees()
        {
            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
            DataTable data = await dbHelper.GetEmployeesAsync();
            dataGridView1.DataSource = data;
        }
        public Employee()
        {
            InitializeComponent();
            dataGridView1.Columns["id"].DataPropertyName = "id";
            dataGridView1.Columns["name"].DataPropertyName = "name";
            dataGridView1.Columns["address"].DataPropertyName = "address";
            dataGridView1.Columns["type"].DataPropertyName = "type";
            dataGridView1.Columns["password"].DataPropertyName = "password";
            dataGridView1.ScrollBars = ScrollBars.Both;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

        }

        private void Employee_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");

            if (dataGridView1.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select exactly one row.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            if (!selectedRow.IsNewRow)
            {
                try
                {
                    string name = selectedRow.Cells["name"].Value?.ToString();
                    string address = selectedRow.Cells["address"].Value?.ToString();
                    string type = selectedRow.Cells["type"].Value?.ToString();
                    string password = selectedRow.Cells["password"].Value?.ToString();

                   
                    bool employeeAdded = await dbHelper.AddEmployeeAsync(name, address, type, password);

                    if (employeeAdded)
                    {
                        LoadEmployees();
                        MessageBox.Show("Employee has been successfully added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to add the employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                int id = Convert.ToInt32(selectedRow.Cells["id"].Value);
                string name = selectedRow.Cells["name"].Value.ToString();
                string address = selectedRow.Cells["address"].Value.ToString();
                string type = selectedRow.Cells["type"].Value.ToString();
                string password = selectedRow.Cells["password"].Value.ToString();
               

                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                bool updated = await dbHelper.UpdateEmployeeAsync(id, name, address, type, password);

                if (updated)
                {
                    MessageBox.Show("Employee updated successfully!");
                    LoadEmployees(); // Refresh the data grid
                }
                else
                {
                    MessageBox.Show("Error updating the employee.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                int id = Convert.ToInt32(selectedRow.Cells["id"].Value);

                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                bool deleted = await dbHelper.DeleteEmployeeAsync(id);

                if (deleted)
                {
                    MessageBox.Show("Employee deleted successfully!");
                    LoadEmployees(); // Refresh the data grid
                }
                else
                {
                    MessageBox.Show("Error deleting the employee.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
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
