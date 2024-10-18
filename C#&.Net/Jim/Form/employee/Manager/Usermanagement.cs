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
            string searchTerm = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                try
                {
                    // Fetch data from the Customer table
                    DataTable result = await dbHelper.SearchCustomerAsync(searchTerm);
                    // Set the fetched data as the data source for dataGridView1
                    dataGridView1.DataSource = result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a name or contact to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void Usermanagement_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
