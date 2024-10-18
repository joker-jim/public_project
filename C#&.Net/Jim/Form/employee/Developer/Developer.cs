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
    public partial class Developer : Form
    {
        private SQLiteDBHelper _dbHelper;
        public Developer(DataTable data)
        {
            _dbHelper = new SQLiteDBHelper("database.db");
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Customer",
                DataPropertyName = "customer"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Code",
                HeaderText = "Code",
                DataPropertyName = "code"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Record Date",
                DataPropertyName = "record_date"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Priority",
                DataPropertyName = "pri"
            });
            dataGridView1.DataSource = data;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];

                if (selectedRow.Cells["Code"] != null)
                {
                    string code = selectedRow.Cells["Code"].Value.ToString();

                    (string type, string hist) = await _dbHelper.GetFaultDetailsAsync(code);

                    if (type != null && hist != null)
                    {
                        Order orderForm = new Order(code, type, hist);
                        orderForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Fault details not found.");
                    }
                }
                else
                {
                    MessageBox.Show("The 'Code' column does not exist.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row first.");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                var codeValue = selectedRow.Cells["code"].Value.ToString();

                var information = await _dbHelper.RetrieveInformationAsync(codeValue);
                if (information != null)
                {
                    Talk talkForm = new Talk(information.Code, information.ManagerInf);
                    talkForm.Show();
                }
                else
                {
                    MessageBox.Show("No information found for the selected code.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row first.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
