
using Jim;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Jim
{
    public partial class UserLogin : Form
    {
        private const string _username = "user";
        private const string _password = "password";
        private SQLiteDBHelper _dbHelper;


        public UserLogin()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
            _dbHelper = new SQLiteDBHelper("database.db");
        }



        private async void button1_Click(object sender, EventArgs e)
        {
            string enteredUsername = textBox1.Text;
            string enteredPassword = textBox2.Text;

            if (_dbHelper == null)
            {
                MessageBox.Show("Database Helper is not initialized.");
                return;
            }

            bool isValidUser = await _dbHelper.UserLogin(enteredUsername, enteredPassword);

            if (isValidUser)
            {
                UserInfo userInfo = await _dbHelper.GetUserInfo(enteredUsername);
                UserCache.CurrentUser = userInfo;
                UserForm userForm = new UserForm();
                userForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("User name or password error, please try again.", "Login failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
