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
    public partial class OrderAdd : Form
    {
        private SQLiteDBHelper dbHelper;
        public OrderAdd()
        {
            InitializeComponent();
            dbHelper = new SQLiteDBHelper("database.db");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string contact = textBox2.Text;
            string code = textBox3.Text;
            string hist = textBox4.Text;
            string productName = textBox5.Text;
            DateTime recordDate = DateTime.Now;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(contact) ||
                string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Please make sure all fields are filled out.");
                return;
            }

            bool customerExists = await dbHelper.CustomerExistsAsync(name, contact);
            if (!customerExists)
            {
                MessageBox.Show("The specified customer does not exist.");
                return;
            }

            bool productExists = await dbHelper.ProductExistsAsync(productName, code);
            if (!productExists)
            {
                MessageBox.Show("The specified product does not exist or the code does not match.");
                return;
            }

            bool codeIsUnique = await dbHelper.CodeIsUniqueAsync(code);
            if (!codeIsUnique)
            {
                MessageBox.Show("The specified code already exists.");
                Ordermanagement ordermanagement = new Ordermanagement();
                ordermanagement.Show();
                this.Hide();
                return;
            }

            bool recordAdded = await dbHelper.AddFaultRecordAsync(name, code, hist, recordDate);
            if (recordAdded)
            {
                MessageBox.Show("Record added successfully.");
                
            }
            else
            {
                MessageBox.Show("Failed to add the record.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
