namespace Access2Mssql
{
    partial class frmChoise
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
            this.btnAbort = new System.Windows.Forms.LinkLabel();
            this.btnSuspend = new System.Windows.Forms.LinkLabel();
            this.btnContinue = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "What do you wanna do?";
            // 
            // btnAbort
            // 
            this.btnAbort.AutoSize = true;
            this.btnAbort.Location = new System.Drawing.Point(59, 48);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(74, 13);
            this.btnAbort.TabIndex = 1;
            this.btnAbort.TabStop = true;
            this.btnAbort.Text = "Abort Transfer";
            this.btnAbort.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnAbort_LinkClicked);
            // 
            // btnSuspend
            // 
            this.btnSuspend.AutoSize = true;
            this.btnSuspend.Location = new System.Drawing.Point(59, 74);
            this.btnSuspend.Name = "btnSuspend";
            this.btnSuspend.Size = new System.Drawing.Size(91, 13);
            this.btnSuspend.TabIndex = 2;
            this.btnSuspend.TabStop = true;
            this.btnSuspend.Text = "Suspend Transfer";
            this.btnSuspend.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnSuspend_LinkClicked);
            // 
            // btnContinue
            // 
            this.btnContinue.AutoSize = true;
            this.btnContinue.Location = new System.Drawing.Point(59, 101);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(49, 13);
            this.btnContinue.TabIndex = 3;
            this.btnContinue.TabStop = true;
            this.btnContinue.Text = "Continue";
            this.btnContinue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnContinue_LinkClicked);
            // 
            // frmChoise
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 135);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnSuspend);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChoise";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Make a chooise";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel btnAbort;
        private System.Windows.Forms.LinkLabel btnSuspend;
        private System.Windows.Forms.LinkLabel btnContinue;
    }
}