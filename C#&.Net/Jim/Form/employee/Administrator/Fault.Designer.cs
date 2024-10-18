namespace Jim
{
    partial class Fault
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.customer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.developer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.record_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processing_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Processing_report = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.customer,
            this.id,
            this.developer,
            this.code,
            this.record_date,
            this.type,
            this.hist,
            this.pri,
            this.processing_date,
            this.Processing_report,
            this.status});
            this.dataGridView1.Location = new System.Drawing.Point(44, 12);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 82;
            this.dataGridView1.RowTemplate.Height = 37;
            this.dataGridView1.Size = new System.Drawing.Size(677, 335);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(70, 362);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(243, 50);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(428, 362);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(243, 50);
            this.button2.TabIndex = 2;
            this.button2.Text = "Update";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(70, 432);
            this.button3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(243, 50);
            this.button3.TabIndex = 3;
            this.button3.Text = "Delete";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(428, 432);
            this.button4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(243, 50);
            this.button4.TabIndex = 4;
            this.button4.Text = "Back";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // customer
            // 
            this.customer.HeaderText = "customer";
            this.customer.MinimumWidth = 10;
            this.customer.Name = "customer";
            this.customer.Width = 80;
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.MinimumWidth = 10;
            this.id.Name = "id";
            this.id.Width = 80;
            // 
            // developer
            // 
            this.developer.HeaderText = "developer";
            this.developer.MinimumWidth = 10;
            this.developer.Name = "developer";
            this.developer.Width = 80;
            // 
            // code
            // 
            this.code.HeaderText = "code";
            this.code.MinimumWidth = 10;
            this.code.Name = "code";
            this.code.Width = 80;
            // 
            // record_date
            // 
            this.record_date.HeaderText = "record_date";
            this.record_date.MinimumWidth = 10;
            this.record_date.Name = "record_date";
            this.record_date.Width = 80;
            // 
            // type
            // 
            this.type.HeaderText = "type";
            this.type.MinimumWidth = 10;
            this.type.Name = "type";
            this.type.Width = 80;
            // 
            // hist
            // 
            this.hist.HeaderText = "hist";
            this.hist.MinimumWidth = 10;
            this.hist.Name = "hist";
            this.hist.Width = 80;
            // 
            // pri
            // 
            this.pri.HeaderText = "pri";
            this.pri.MinimumWidth = 10;
            this.pri.Name = "pri";
            this.pri.Width = 80;
            // 
            // processing_date
            // 
            this.processing_date.HeaderText = "processing_date";
            this.processing_date.MinimumWidth = 10;
            this.processing_date.Name = "processing_date";
            this.processing_date.Width = 80;
            // 
            // Processing_report
            // 
            this.Processing_report.HeaderText = "Processing_report";
            this.Processing_report.MinimumWidth = 10;
            this.Processing_report.Name = "Processing_report";
            this.Processing_report.Width = 80;
            // 
            // status
            // 
            this.status.HeaderText = "status";
            this.status.MinimumWidth = 10;
            this.status.Name = "status";
            this.status.Width = 80;
            // 
            // Fault
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 463);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Fault";
            this.Text = "Fault";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridViewTextBoxColumn customer;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn developer;
        private System.Windows.Forms.DataGridViewTextBoxColumn code;
        private System.Windows.Forms.DataGridViewTextBoxColumn record_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn hist;
        private System.Windows.Forms.DataGridViewTextBoxColumn pri;
        private System.Windows.Forms.DataGridViewTextBoxColumn processing_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Processing_report;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
    }
}