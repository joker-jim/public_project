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
    public partial class EmployeeLogin : Form
    {
        private const string _username = "user";
        private const string _password = "password";
        private SQLiteDBHelper _dbHelper;
        public EmployeeLogin()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
            _dbHelper = new SQLiteDBHelper("database.db");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string enteredUsername = textBox1.Text;
            string enteredPassword = textBox2.Text; 

            (bool success, string userType) = await _dbHelper.EmployeeLogin(enteredUsername, enteredPassword);

            if (success)
            {
                MessageBox.Show($"Login success! The user type is: {userType}");
                EmployeeInfo employeeInfo = await _dbHelper.GetEmployInfo(enteredUsername);
                EmployeeCache.CurrentEmployee = employeeInfo;
                


                switch (userType)
                {
                    case "administrator":
                        Administrator administrator = new Administrator();
                        administrator.Show();
                        this.Hide();  
                        break;
                    case "manager":
                        Manager manager = new Manager();
                        manager.Show();
                        this.Hide();  
                        break;
                    case "developer":
                        DataTable faults = await _dbHelper.GetDeveloperFaultsAsync(EmployeeCache.CurrentEmployee.name);
                        Developer developer = new Developer(faults);
                        developer.Show();
                        this.Hide();
                        break;
                    case "helpDeskStaff":
                        HelpDeskStaff helpDeskStaff = new HelpDeskStaff();
                        helpDeskStaff.Show();
                        this.Hide();  
                        break;
                    default:
                        MessageBox.Show("erro");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Login failed! Please check your username and password.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
