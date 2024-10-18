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
    public partial class Administrator : Form
    {
        public Administrator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Customer customer = new Customer();
            customer.FormClosed += (s, args) => this.Show();
            customer.Show();
            this.Hide();
            customer.LoadCustomers();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Employee employee = new Employee();
            employee.FormClosed += (s, args) => this.Show();
            employee.Show();
            this.Hide();
            employee.LoadEmployees();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Product product = new Product();
            product.FormClosed += (s, args) => this.Show();
            product.Show();
            this.Hide();
            product.LoadProducts();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Fault fault = new Fault();
            fault.FormClosed += (s, args) => this.Show();
            fault.Show();
            this.Hide();
            fault.LoadFaults();
        }

    }
}
