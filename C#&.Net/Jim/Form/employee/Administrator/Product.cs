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
    public partial class Product : Form
    {
        public async void LoadProducts()
        {
            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
            DataTable data = await dbHelper.GetProductsAsync();
            dataGridView1.DataSource = data;
        }

        public Product()
        {
            InitializeComponent();
            dataGridView1.Columns["id"].DataPropertyName = "id";
            dataGridView1.Columns["name"].DataPropertyName = "name";
            dataGridView1.Columns["code"].DataPropertyName = "code";
            dataGridView1.Columns["dateReleased"].DataPropertyName = "dateReleased";
            dataGridView1.Columns["release"].DataPropertyName = "release";
            dataGridView1.ScrollBars = ScrollBars.Both;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                string name = selectedRow.Cells["name"].Value.ToString();
                string code = selectedRow.Cells["code"].Value.ToString();
                DateTime dateReleased = Convert.ToDateTime(selectedRow.Cells["dateReleased"].Value);
                string release = selectedRow.Cells["release"].Value.ToString();

                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                bool added = await dbHelper.AddProductAsync(name, code, dateReleased, release);

                if (added)
                {
                    MessageBox.Show("Product added successfully!");
                    LoadProducts(); 
                }
                else
                {
                    MessageBox.Show("Error adding the product.");
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
                string name = selectedRow.Cells["name"].Value.ToString();
                string code = selectedRow.Cells["code"].Value.ToString();
                DateTime dateReleased = Convert.ToDateTime(selectedRow.Cells["dateReleased"].Value);
                string release = selectedRow.Cells["release"].Value.ToString();
                dataGridView1.ScrollBars = ScrollBars.Both;

                SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
                bool updated = await dbHelper.UpdateProductAsync(id, name, code, dateReleased, release);

                if (updated)
                {
                    MessageBox.Show("Product updated successfully!");
                    LoadProducts(); 
                }
                else
                {
                    MessageBox.Show("Error updating the product.");
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
                bool deleted = await dbHelper.DeleteProductAsync(id);

                if (deleted)
                {
                    MessageBox.Show("Product deleted successfully!");
                    LoadProducts(); 
                }
                else
                {
                    MessageBox.Show("Error deleting the product.");
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
