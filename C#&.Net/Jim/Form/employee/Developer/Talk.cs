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
    public partial class Talk : Form
    {
        private SQLiteDBHelper _dbHelper = new SQLiteDBHelper("database.db");

        public Talk(string code, string managerInf)
        {
            InitializeComponent();
            textBox1.Text = code;
            textBox2.Text = managerInf;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Talk_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string developerInf = textBox3.Text;
            string code = textBox1.Text;

            if (string.IsNullOrEmpty(developerInf))
            {
                MessageBox.Show("Please enter the developer information.");
                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("The code is missing. Cannot update the information.");
                return;
            }

            bool success = await _dbHelper.UpdateInformationAsync1(code, developerInf);

            if (success)
            {
                MessageBox.Show("Information updated successfully.");
            }
            else
            {
                MessageBox.Show("Failed to update the information.");
            }
        }

    }
}
