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
    public partial class Order : Form

    {
        private SQLiteDBHelper dbHelper;
        public Order(string code, string type, string hist)
        {
            InitializeComponent();
            dbHelper = new SQLiteDBHelper("database.db");
            textBox3.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox1.ReadOnly = true;
            textBox1.Text = code;
            textBox2.Text = type;
            textBox3.Text = hist;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Order_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text;
            string processingReport = textBox4.Text;

            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
            await dbHelper.UpdateFaultAsync(code, processingReport);

            MessageBox.Show("Record updated successfully!");
        }

    }
}
