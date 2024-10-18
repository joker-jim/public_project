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
    public partial class UserForm : Form
    {
        private UpdataOrder updataOrderForm;
        public UserForm()
        {
            InitializeComponent();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string username = UserCache.CurrentUser.name;
            SQLiteDBHelper dbHelper = new SQLiteDBHelper("database.db");
            DataTable faultData = await dbHelper.GetFaultDataByUserAsync(username);

            UpdataOrder updateOrderForm = new UpdataOrder();
            updateOrderForm.ShowFaultData(faultData);
            updateOrderForm.Show();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            AddOrder form = new AddOrder();
            form.FormClosed += (s, args) => this.Show();
            form.ShowDialog();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
