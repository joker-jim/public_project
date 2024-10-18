using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jim
{
    public partial class Managerorder : Form
    {
        private SQLiteDBHelper dbHelper;
        public Managerorder()
        {
            InitializeComponent();
            dbHelper = new SQLiteDBHelper("database.db");
            SetupDataGridView();
        }
        private void SetupDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.CellBeginEdit += (s, e) =>
            {
                if (dataGridView1.Columns[e.ColumnIndex].Name != "developer")
                {
                    e.Cancel = true;
                }
            };
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string status = comboBox1.SelectedItem as string;
            string searchText = textBox1.Text;

            string query = "SELECT * FROM Fault";
            if (!string.IsNullOrEmpty(status) || !string.IsNullOrEmpty(searchText))
            {
                query += " WHERE";
                if (!string.IsNullOrEmpty(status))
                {
                    query += " status" + (status == "unsolved" ? " IS NULL" : " = 'resolved'");
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " AND";
                    }
                }
                if (!string.IsNullOrEmpty(searchText))
                {
                    query += " (developer = @searchText OR code = @searchText)";
                }
            }

            try
            {
                using (var dt = await dbHelper.GetDataAsync(query, new SQLiteParameter("@searchText", searchText)))
                {
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row in the grid.");
                return;
            }

            string developerName = textBox2.Text.Trim();
            string pri = comboBox2.SelectedItem?.ToString();
            string type = comboBox3.SelectedItem?.ToString();
            string code = dataGridView1.SelectedRows[0].Cells["code"].Value.ToString(); 

            if (string.IsNullOrEmpty(developerName) || string.IsNullOrEmpty(pri) || string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            try
            {
                if (await dbHelper.EmployeeExistsAsync(developerName, "developer"))
                {
                    await dbHelper.UpdateFaultAsync(code, developerName, pri, type);
                    await dbHelper.InsertInformationAsync(EmployeeCache.CurrentEmployee.name, developerName, code, "1");

                    MessageBox.Show("Update successful!");
                }
                else
                {
                    MessageBox.Show("Developer not found or is not a developer.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row in the grid.");
                return;
            }

            string code = dataGridView1.SelectedRows[0].Cells["code"].Value.ToString(); 
            string developer = dataGridView1.SelectedRows[0].Cells["developer"].Value.ToString(); 

            try
            {
                await dbHelper.DeleteInformationAsync(code, developer);
                await dbHelper.ClearFaultFieldsAsync(code);

                MessageBox.Show("Operation successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
