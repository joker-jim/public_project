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
    public partial class Ordermanagement : Form
    {
        private SQLiteDBHelper dbHelper;
      

        public Ordermanagement()
        {
            InitializeComponent();
            dbHelper = new SQLiteDBHelper("database.db");
         
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a customer or code.");
                return;
            }

            DataTable result = await dbHelper.SearchFaultsByCustomerOrCodeAsync(searchTerm);
            dataGridView1.DataSource = result;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.Name != "hist")
                {
                    column.ReadOnly = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OrderAdd orderAddForm = new OrderAdd();
            orderAddForm.FormClosed += OrderAddForm_FormClosed;
            orderAddForm.Show();
            this.Hide();
        }
        private void OrderAddForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show(); 
        }


        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to update.");
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string code = selectedRow.Cells["code"].Value.ToString();
            string newHistValue = selectedRow.Cells["hist"].Value.ToString();

            if (await dbHelper.CanUpdateHist(code))
            {
                await dbHelper.UpdateHist(code, newHistValue);
                MessageBox.Show("Hist updated successfully!");
            }
            else
            {
                MessageBox.Show("Cannot update hist. The Fault's status, developer, type, or pri is not empty.");
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string code = selectedRow.Cells["code"].Value.ToString();

            if (await dbHelper.CanDeleteRow(code))
            {
                await dbHelper.DeleteRow(code);
                MessageBox.Show("Row deleted successfully!");
            }
            else
            {
                MessageBox.Show("Cannot delete the row. The Fault's status, developer, type, or pri is not empty.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
          
            this.Close();

        }
    }
}
