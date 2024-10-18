using Jim.Code;
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
    public partial class Managertalk : Form
    {
        private SQLiteDBHelper _dbHelper = new SQLiteDBHelper("database.db");

        public Managertalk(List<InformationRecord> data)
        {
            InitializeComponent();

            
            dataGridView1.Columns.Add("Code", "Code");
            dataGridView1.Columns.Add("DeveloperName", "Developer Name");

            var sortedData = data.OrderBy(d => d.Type1 == 1 ? 0 : 1).ThenBy(d => d.Code).ToList();
            foreach (var record in sortedData)
            {
                var rowIndex = dataGridView1.Rows.Add(record.Code, record.DeveloperName);
                var row = dataGridView1.Rows[rowIndex];
                if (record.Type1 == 1)
                {
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var record = row.DataBoundItem as InformationRecord;
                if (record != null && record.Type1 == 1)
                {
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var code = dataGridView1.SelectedRows[0].Cells["code"].Value.ToString();

                var dbHelper = new SQLiteDBHelper("database.db");

                var updateSuccess = await dbHelper.UpdateInformationTypeAsync(code);
                if (updateSuccess)
                {
                    var (developer_inf, _) = await dbHelper.GetInformationAsync(code);

                    var mtalkForm = new Mtalk();
                    mtalkForm.textBox1.Text = developer_inf;
                    mtalkForm.textBox3.Text = code;
                    mtalkForm.Show(); 
                }
                else
                {
                    MessageBox.Show("Update failed");
                }
            }
            else
            {
                MessageBox.Show("Please select a row");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
