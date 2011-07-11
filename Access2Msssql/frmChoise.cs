using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Access2Mssql
{
    public partial class frmChoise : Form
    {
        public byte m_Stat;
        public frmChoise()
        {
            InitializeComponent();
        }

        private void btnAbort_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_Stat = 0;
            this.Close();
        }

        private void btnSuspend_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_Stat = 1;
            this.Close();
        }

        private void btnContinue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_Stat = 2;
            this.Close();
        }
    }
}
