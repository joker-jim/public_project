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
    public partial class Manager : Form
    {
        private SQLiteDBHelper dbHelper;
        public Manager()
        {
            InitializeComponent();
            dbHelper = new SQLiteDBHelper("database.db");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Managerorder managerorder = new Managerorder();
            managerorder.FormClosed += Managerorder_FormClosed;
            managerorder.Show();
            this.Hide();
        }
        private void OpenMtalkForm()
        {
            Mtalk mtalkForm = new Mtalk();
            mtalkForm.FormClosed += MtalkForm_FormClosed; 
            mtalkForm.Show();
            this.Hide(); 
        }

       
        private void MtalkForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show(); 
        }

        private void Managerorder_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show(); 
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var managerName = EmployeeCache.CurrentEmployee.name;
                var data = await dbHelper.GetInformationByManagerName(managerName);
                var managertalkForm = new Managertalk(data);
                managertalkForm.Show();
                managertalkForm.FormClosed += ManagertalkForm_FormClosed; 
                managertalkForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void ManagertalkForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show(); 
        }


        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
