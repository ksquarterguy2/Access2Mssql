using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Access2Mssql
{
    public partial class frmOperationSummary : Form
    {
        public frmOperationSummary()
        {
            InitializeComponent();
        }

        public void SetSummary(ref ConvertSummary summary)
        {
            lblAffectedRecords.Text = summary.TotalAffectedRows.ToString();
            lblAvRate.Text = summary.AvarageTransferRate.ToString() + " kbps";
            lblElapsedTime.Text = string.Format("{0}:{1}:{2}",
                                  summary.ElapsedTime.Hours,
                                  summary.ElapsedTime.Minutes,
                                  summary.ElapsedTime.Seconds);

            lblErrorCount.Text = summary.OccurredErrors.ToString();
            lblFinishTime.Text = summary.OperationFinishTime.ToString();
            lblMissedCount.Text = summary.MissedRecords.ToString();
            lblStartTime.Text = summary.OperationStartTime.ToString();
            lblTableCount.Text = summary.TableCount.ToString();
            lblTransTotalSize.Text = (summary.ConvertedTotalBytes / 1024).ToString() + " KB";
            lblFixedCount.Text = summary.FixedErrors.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmOperationSummary_Load(object sender, EventArgs e)
        {

        }

    }
}
