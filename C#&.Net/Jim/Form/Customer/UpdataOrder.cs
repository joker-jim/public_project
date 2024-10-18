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

    public partial class UpdataOrder : Form
    {
        private SQLiteDBHelper _dbHelper;
        private async void LoadData()
        {
            DataTable faultData = await _dbHelper.GetFaultDataByUserAsync(UserCache.CurrentUser.name); // This assumes you have a method and a user cache like this.
            dataGridView1.DataSource = faultData;
        }
        public UpdataOrder()
        {
            InitializeComponent();
            _dbHelper = new SQLiteDBHelper("database.db");
        }
        public void UpdateDataSource(DataTable dataTable)
        {
           

        }

        private void UpdataOrder_Load(object sender, EventArgs e)
        {

        }
        public void ShowFaultData(DataTable faultData)
        {
            if (dataGridView1.InvokeRequired)
            {
               
                dataGridView1.Invoke(new Action(() =>
                {
                    ShowFaultData(faultData); 
                }));
            }
            else
            {
                
                dataGridView1.DataSource = faultData;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.Visible = false;
                }

              
                dataGridView1.Columns["customer"].Visible = true;
                dataGridView1.Columns["code"].Visible = true;
                dataGridView1.Columns["record_date"].Visible = true;
                dataGridView1.Columns["hist"].Visible = true;
                dataGridView1.Columns["status"].Visible = true;

                dataGridView1.Columns["customer"].ReadOnly = true;
                dataGridView1.Columns["code"].ReadOnly = true;
                dataGridView1.Columns["record_date"].ReadOnly = true;
                dataGridView1.Columns["status"].ReadOnly = true;


            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (_dbHelper == null)
            {
                MessageBox.Show("Database connection uninitialized!");
                return;
            }

            if (dataGridView1.DataSource is DataTable dataTable)
            {
                try
                {
                    await _dbHelper.UpdateDatabaseAsync(dataTable);
                    dataTable.AcceptChanges(); 
                    MessageBox.Show("Database update successful!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating the database:" + ex.Message);
                }
            }
        }


        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete the selected row?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                string code = dataGridView1.SelectedRows[0].Cells["code"].Value.ToString();

                bool deleteSuccessful = await _dbHelper.DeleteFaultRowAsync(code);
                if (deleteSuccessful)
                {
                    MessageBox.Show("Row deleted successfully.");
                   
                    LoadData(); 
                }
                else
                {
                    MessageBox.Show("Failed to delete row.");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
