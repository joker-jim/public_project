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
    public partial class Mtalk : Form
    {


        public Mtalk()
        {
            
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var code = textBox3.Text;
            var manager_inf = textBox2.Text;

            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(manager_inf))
            {
                var dbHelper = new SQLiteDBHelper("database.db");

                var updateSuccess = await dbHelper.UpdateInformationAsync(code, manager_inf);
                if (updateSuccess)
                {
                    MessageBox.Show("Update successful");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Update failed");
                }
            }
            else
            {
                MessageBox.Show("Make sure both code and manager_inf are filled in");
            }
        }

        private void Mtalk_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
