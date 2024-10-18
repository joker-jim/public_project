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
    public partial class HelpDeskStaff : Form
    {
        public HelpDeskStaff()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Usermanagement usermanagement = new Usermanagement();
            usermanagement.FormClosed += Usermanagement_FormClosed;
            usermanagement.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ordermanagement ordermanagement = new Ordermanagement();

            ordermanagement.FormClosed += Ordermanagement_FormClosed;

            ordermanagement.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Ordermanagement_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }
        private void Usermanagement_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }
    }
}
