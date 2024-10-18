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
    public partial class Fault : Form
    {

        public async void LoadFaults()
        {
            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
            DataTable data = await dbHelper.GetFaultsAsync();
            dataGridView1.DataSource = data;
        }

        public Fault()
        {
            InitializeComponent();
            dataGridView1.Columns["id"].DataPropertyName = "id";
            dataGridView1.Columns["customer"].DataPropertyName = "customer";
            dataGridView1.Columns["developer"].DataPropertyName = "developer";
            dataGridView1.Columns["code"].DataPropertyName = "code";
            dataGridView1.Columns["record_date"].DataPropertyName = "record_date";
            dataGridView1.Columns["type"].DataPropertyName = "type";
            dataGridView1.Columns["hist"].DataPropertyName = "hist";
            dataGridView1.Columns["pri"].DataPropertyName = "pri";
            dataGridView1.Columns["processing_date"].DataPropertyName = "processing_date";
            dataGridView1.Columns["Processing_report"].DataPropertyName = "Processing_report";
            dataGridView1.Columns["status"].DataPropertyName = "status";
            dataGridView1.ScrollBars = ScrollBars.Both;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                string customer = selectedRow.Cells["customer"].Value.ToString();
                string developer = selectedRow.Cells["developer"].Value.ToString();
                string code = selectedRow.Cells["code"].Value.ToString();
                DateTime recordDate = Convert.ToDateTime(selectedRow.Cells["record_date"].Value);
                string type = selectedRow.Cells["type"].Value.ToString();
                string hist = selectedRow.Cells["hist"].Value.ToString();
                string pri= selectedRow.Cells["pri"].Value.ToString();               
                DateTime processingDate = Convert.ToDateTime(selectedRow.Cells["processing_date"].Value);
                string processingReport = selectedRow.Cells["Processing_report"].Value.ToString();
                string status = selectedRow.Cells["status"].Value.ToString();

                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                bool added = await dbHelper.AddFaultAsync(customer, developer, code, recordDate, type, hist, pri, processingDate, processingReport, status);

                if (added)
                {
                    MessageBox.Show("Fault added successfully!");
                    LoadFaults(); // Refresh the data grid
                }
                else
                {
                    MessageBox.Show("Error adding the fault.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to add.");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                int id = Convert.ToInt32(selectedRow.Cells["id"].Value);
                string customer = selectedRow.Cells["customer"].Value.ToString();
                string developer = selectedRow.Cells["developer"].Value.ToString();
                string code = selectedRow.Cells["code"].Value.ToString();
                DateTime recordDate = Convert.ToDateTime(selectedRow.Cells["record_date"].Value);
                string type = selectedRow.Cells["type"].Value.ToString();
                string hist = selectedRow.Cells["hist"].Value.ToString();
                string pri = selectedRow.Cells["pri"].Value.ToString();
                DateTime processingDate = Convert.ToDateTime(selectedRow.Cells["processing_date"].Value);
                string processingReport = selectedRow.Cells["Processing_report"].Value.ToString();
                string status = selectedRow.Cells["status"].Value.ToString();
                dataGridView1.ScrollBars = ScrollBars.Both;

                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                bool updated = await dbHelper.UpdateFaultAsync(id, customer, developer, code, recordDate, type, hist, pri, processingDate, processingReport, status);

                if (updated)
                {
                    MessageBox.Show("Fault updated successfully!");
                    LoadFaults(); 
                }
                else
                {
                    MessageBox.Show("Error updating the fault.");
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
                bool deleted = await dbHelper.DeleteFaultAsync(id);

                if (deleted)
                {
                    MessageBox.Show("Fault deleted successfully!");
                    LoadFaults(); 
                }
                else
                {
                    MessageBox.Show("Error deleting the fault.");
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
