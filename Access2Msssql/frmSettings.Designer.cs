namespace Access2Mssql
{
    partial class frmSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbCollation = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVarSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbTextType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkExistDb = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkFix = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStoragePath = new System.Windows.Forms.TextBox();
            this.pnlDbSettings = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.cbLdfType = new System.Windows.Forms.ComboBox();
            this.txtLdfMax = new System.Windows.Forms.TextBox();
            this.cbMdfType = new System.Windows.Forms.ComboBox();
            this.txtMdfMax = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.mnCompatibleLevel = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.chkAutoDetect = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.pnlDbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnCompatibleLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Collation:";
            // 
            // cbCollation
            // 
            this.cbCollation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCollation.FormattingEnabled = true;
            this.cbCollation.Location = new System.Drawing.Point(68, 6);
            this.cbCollation.Name = "cbCollation";
            this.cbCollation.Size = new System.Drawing.Size(254, 21);
            this.cbCollation.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtVarSize);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbTextType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(17, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 87);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Text Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "bytes.";
            // 
            // txtVarSize
            // 
            this.txtVarSize.Location = new System.Drawing.Point(87, 47);
            this.txtVarSize.Name = "txtVarSize";
            this.txtVarSize.Size = new System.Drawing.Size(58, 20);
            this.txtVarSize.TabIndex = 3;
            this.txtVarSize.Text = "256";
            this.txtVarSize.TextChanged += new System.EventHandler(this.txtVarSize_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Size:";
            // 
            // cbTextType
            // 
            this.cbTextType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextType.FormattingEnabled = true;
            this.cbTextType.Items.AddRange(new object[] {
            "char",
            "nchar",
            "nvarchar",
            "varchar",
            "text",
            "ntext"});
            this.cbTextType.Location = new System.Drawing.Point(87, 22);
            this.cbTextType.Name = "cbTextType";
            this.cbTextType.Size = new System.Drawing.Size(99, 21);
            this.cbTextType.TabIndex = 1;
            this.cbTextType.SelectedIndexChanged += new System.EventHandler(this.cbTextType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Text type as: ";
            // 
            // chkExistDb
            // 
            this.chkExistDb.AutoSize = true;
            this.chkExistDb.Location = new System.Drawing.Point(18, 162);
            this.chkExistDb.Name = "chkExistDb";
            this.chkExistDb.Size = new System.Drawing.Size(236, 17);
            this.chkExistDb.TabIndex = 3;
            this.chkExistDb.Text = "If database does not exist create a new one.";
            this.chkExistDb.UseVisualStyleBackColor = true;
            this.chkExistDb.CheckedChanged += new System.EventHandler(this.chkExistDb_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(247, 386);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkFix
            // 
            this.chkFix.AutoSize = true;
            this.chkFix.Location = new System.Drawing.Point(18, 139);
            this.chkFix.Name = "chkFix";
            this.chkFix.Size = new System.Drawing.Size(296, 17);
            this.chkFix.TabIndex = 5;
            this.chkFix.Text = "Auto fix when attempting to set primary key \'text\' or \'ntext\'.";
            this.chkFix.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "SQL Server Storage Path:";
            // 
            // txtStoragePath
            // 
            this.txtStoragePath.Location = new System.Drawing.Point(12, 27);
            this.txtStoragePath.Name = "txtStoragePath";
            this.txtStoragePath.Size = new System.Drawing.Size(287, 20);
            this.txtStoragePath.TabIndex = 7;
            // 
            // pnlDbSettings
            // 
            this.pnlDbSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnlDbSettings.Controls.Add(this.chkAutoDetect);
            this.pnlDbSettings.Controls.Add(this.label9);
            this.pnlDbSettings.Controls.Add(this.cbLdfType);
            this.pnlDbSettings.Controls.Add(this.txtLdfMax);
            this.pnlDbSettings.Controls.Add(this.cbMdfType);
            this.pnlDbSettings.Controls.Add(this.txtMdfMax);
            this.pnlDbSettings.Controls.Add(this.label8);
            this.pnlDbSettings.Controls.Add(this.label7);
            this.pnlDbSettings.Controls.Add(this.mnCompatibleLevel);
            this.pnlDbSettings.Controls.Add(this.label6);
            this.pnlDbSettings.Controls.Add(this.label5);
            this.pnlDbSettings.Controls.Add(this.txtStoragePath);
            this.pnlDbSettings.Location = new System.Drawing.Point(15, 185);
            this.pnlDbSettings.Name = "pnlDbSettings";
            this.pnlDbSettings.Size = new System.Drawing.Size(307, 195);
            this.pnlDbSettings.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label9.Location = new System.Drawing.Point(169, 164);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "(0 = UNLIMITED)";
            // 
            // cbLdfType
            // 
            this.cbLdfType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLdfType.FormattingEnabled = true;
            this.cbLdfType.Items.AddRange(new object[] {
            "KB",
            "MB",
            "GB"});
            this.cbLdfType.Location = new System.Drawing.Point(207, 128);
            this.cbLdfType.Name = "cbLdfType";
            this.cbLdfType.Size = new System.Drawing.Size(42, 21);
            this.cbLdfType.TabIndex = 15;
            // 
            // txtLdfMax
            // 
            this.txtLdfMax.Location = new System.Drawing.Point(111, 129);
            this.txtLdfMax.Name = "txtLdfMax";
            this.txtLdfMax.Size = new System.Drawing.Size(90, 20);
            this.txtLdfMax.TabIndex = 14;
            // 
            // cbMdfType
            // 
            this.cbMdfType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMdfType.FormattingEnabled = true;
            this.cbMdfType.Items.AddRange(new object[] {
            "KB",
            "MB",
            "GB"});
            this.cbMdfType.Location = new System.Drawing.Point(207, 104);
            this.cbMdfType.Name = "cbMdfType";
            this.cbMdfType.Size = new System.Drawing.Size(42, 21);
            this.cbMdfType.TabIndex = 13;
            // 
            // txtMdfMax
            // 
            this.txtMdfMax.Location = new System.Drawing.Point(111, 105);
            this.txtMdfMax.Name = "txtMdfMax";
            this.txtMdfMax.Size = new System.Drawing.Size(90, 20);
            this.txtMdfMax.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Ldf File Max Size:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Mdf File Max Size:";
            // 
            // mnCompatibleLevel
            // 
            this.mnCompatibleLevel.Location = new System.Drawing.Point(111, 79);
            this.mnCompatibleLevel.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.mnCompatibleLevel.Minimum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.mnCompatibleLevel.Name = "mnCompatibleLevel";
            this.mnCompatibleLevel.Size = new System.Drawing.Size(38, 20);
            this.mnCompatibleLevel.TabIndex = 9;
            this.mnCompatibleLevel.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Compatible Level:";
            // 
            // chkAutoDetect
            // 
            this.chkAutoDetect.AutoSize = true;
            this.chkAutoDetect.Location = new System.Drawing.Point(12, 53);
            this.chkAutoDetect.Name = "chkAutoDetect";
            this.chkAutoDetect.Size = new System.Drawing.Size(98, 17);
            this.chkAutoDetect.TabIndex = 17;
            this.chkAutoDetect.Text = "Try auto detect";
            this.chkAutoDetect.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 425);
            this.Controls.Add(this.pnlDbSettings);
            this.Controls.Add(this.chkFix);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkExistDb);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbCollation);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlDbSettings.ResumeLayout(false);
            this.pnlDbSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnCompatibleLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCollation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbTextType;
        private System.Windows.Forms.TextBox txtVarSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkExistDb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkFix;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStoragePath;
        private System.Windows.Forms.Panel pnlDbSettings;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown mnCompatibleLevel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbLdfType;
        private System.Windows.Forms.TextBox txtLdfMax;
        private System.Windows.Forms.ComboBox cbMdfType;
        private System.Windows.Forms.TextBox txtMdfMax;
        private System.Windows.Forms.CheckBox chkAutoDetect;
    }
}