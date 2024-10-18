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
    public partial class AddOrder : Form
    {
        private SQLiteDBHelper _dbHelper;
       
        public AddOrder()
        {
            InitializeComponent();
            _dbHelper = new SQLiteDBHelper("database.db");

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text.Trim();
            string name = textBox2.Text.Trim();
            string hist = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Both the code and name fields are required.");
                return;
            }

            bool result = await _dbHelper.AddToFaultAsync(code,hist,name);
            if (result)
            {
                MessageBox.Show("Add successfully。");
                UserForm userForm = new UserForm();
                userForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Add failure。");
            }

        }

    

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
