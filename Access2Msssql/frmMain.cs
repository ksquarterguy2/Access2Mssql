using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Access2Mssql
{
    public partial class frmMain : Form
    {
        private Thread TransferWorkerThread = null;

        public frmMain()
        {
            InitializeComponent();
        }

        private void LoadSettings()
        {
            StreamReader setting_file = null;
            char []d = new char[]{'='};
            try
            {
                setting_file = new StreamReader("setting.ini");

                object[] text_type;

                Program.GeneralAppSetting.Collation = setting_file.ReadLine().Split(d)[1];

                text_type = Access2Mssql.AccessToMssql.ParseTextType(setting_file.ReadLine().Split(d)[1]);

                Program.GeneralAppSetting.TextType = (string)text_type[0];
                Program.GeneralAppSetting.Size = (int)text_type[1];
                Program.GeneralAppSetting.AutoFixTextPrimaryAttempt = Convert.ToBoolean(setting_file.ReadLine().Split(d)[1]);
                Program.GeneralAppSetting.NewDatabaseDoesNotExist = Convert.ToBoolean(setting_file.ReadLine().Split(d)[1]);
                Program.GeneralAppSetting.DatabaseStoragePath = setting_file.ReadLine().Split(d)[1];
                Program.GeneralAppSetting.TryAutoDetectStoragePath = Convert.ToBoolean(setting_file.ReadLine().Split(d)[1]);
                Program.GeneralAppSetting.DatabaseLevel = Convert.ToUInt16(setting_file.ReadLine().Split(d)[1]);
                Program.GeneralAppSetting.MdfMaxSize = Convert.ToInt32(setting_file.ReadLine().Split(d)[1]);
                Program.GeneralAppSetting.MdfSizeType = (FileSizeType)Convert.ToInt32(setting_file.ReadLine().Split(d)[1]);
                Program.GeneralAppSetting.LdfMaxSize = Convert.ToInt32(setting_file.ReadLine().Split(d)[1]);
                Program.GeneralAppSetting.LdfSizeType = (FileSizeType)Convert.ToInt32(setting_file.ReadLine().Split(d)[1]);

                setting_file.Close();
            }
            catch
            {
                Program.GeneralAppSetting.Collation = "Turkish_CI_AS";
                Program.GeneralAppSetting.TextType = "varchar";
                Program.GeneralAppSetting.Size = 50;
                Program.GeneralAppSetting.NewDatabaseDoesNotExist = false;
                Program.GeneralAppSetting.AutoFixTextPrimaryAttempt = false;
                Program.GeneralAppSetting.DatabaseStoragePath = string.Empty;
                Program.GeneralAppSetting.TryAutoDetectStoragePath = true;
                Program.GeneralAppSetting.DatabaseLevel = 9;
                Program.GeneralAppSetting.MdfMaxSize = 0;
                Program.GeneralAppSetting.MdfSizeType = FileSizeType.GB;
                Program.GeneralAppSetting.LdfMaxSize = 1024;
                Program.GeneralAppSetting.LdfSizeType = FileSizeType.GB;
            }
        }
        
        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadSettings();
            cbAuthentication.SelectedIndex = 0;
            Program.Converter = new AccessToMssql();
            
        }

        private void SetStatus(string status)
        {
            lblStatus.Text = status;
        }

        private void ConnectionError(ErrorType type,string extend_error_msg)
        {
            string ErrorMessage = string.Empty;

            switch (type)
            {
                case ErrorType.COULD_NOT_CONNECT_ACCESS:
                    ErrorMessage = "Could not connect to access!";
                    break;
                case ErrorType.COULD_NOT_CONNECT_MSSQL:
                    ErrorMessage = "Could not connect to mssql server!";
                    break;
            }

            ErrorMessage += "\n\nExtended Message:\n";
            ErrorMessage += extend_error_msg;

            MessageBox.Show(ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Completed(ConvertSummary summary)
        {
            frmOperationSummary frmOS = new frmOperationSummary();
            Program.Converter.Disconnect();
            Program.Converter.Dispose();
            frmOS.SetSummary(ref summary);
            frmOS.ShowDialog();

            this.BeginInvoke((MethodInvoker)delegate() { this.btnStart.Text = "Start Translation"; });
            
            
        }

        private void OperationFailed(ref string msg)
        {
            
        }

        private void SetDatabaseSettingStructure(out DatabaseSettings Setting)
        {
            Setting.AccessDbFile = txtAccessSource.Text;
            Setting.Database = txtDatabase.Text;
            Setting.IsSqlAuthencation = txtUsername.Enabled;
            Setting.TrustedConnection = chkTrustedConn.Checked;
            Setting.Username = txtUsername.Text;
            Setting.Password = txtPassword.Text;
            Setting.Server = txtServer.Text;   
        }

        private void StartOperation()
        {
            DatabaseSettings setting = new DatabaseSettings();

            SetDatabaseSettingStructure(out setting);

            if (!Program.Converter.EventSet)
            {
                Program.Converter.OnProgressStatus += new OnProgressEventHandler(SetStatus);
                Program.Converter.OnOperationFailed += new OperationFailedExceptionEventHandler(OperationFailed);
                Program.Converter.OnConnectionError += new ConnectionExceptionEventHandler(ConnectionError);
                Program.Converter.OnComplete += new OnCompleteEventHandler(Completed);
            }

            if (!Program.Converter.Connect(setting))
            {
                Program.Converter.Dispose();
                this.BeginInvoke((MethodInvoker)delegate() { this.btnStart.Text = "Start Translation"; });
            }
            else
                Program.Converter.StartTransfer();
        }

        private void ActivateOperationWorkerThread()
        {
            TransferWorkerThread = null;
            TransferWorkerThread = new Thread(new ThreadStart(StartOperation));
            btnStart.Text = "Abort";
            TransferWorkerThread.Start();
        }

        private void cbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            label4.Enabled = label5.Enabled = txtUsername.Enabled = txtPassword.Enabled = chkMasking.Enabled = (cbAuthentication.SelectedIndex == 1);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog access = new OpenFileDialog();
            access.Filter = "Access Database File|*.mdb";
            access.InitialDirectory = "C:\\";
            access.ShowDialog();
            txtAccessSource.Text = access.FileName;
        }

        private void chkMasking_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = (chkMasking.Checked) ? '\0' : '*';
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (TransferWorkerThread == null || TransferWorkerThread.ThreadState == ThreadState.Stopped)
            {
                ActivateOperationWorkerThread();
                return;
            }
            
            switch (TransferWorkerThread.ThreadState)
            {
                case ThreadState.Unstarted:
                case ThreadState.Aborted:
                    {
                        ActivateOperationWorkerThread();
                    }
                    break;
                case ThreadState.Running:
                    {
                        frmChoise choise = new frmChoise();
                        choise.ShowDialog();
                        switch (choise.m_Stat)
                        {
                            case 0:
                                {
                                    lblStatus.Text = "aborting... please wait...";
                                    Program.Converter.AbortOperation();
                                    lblStatus.Text = "operation interrupt by user";
                                    btnStart.Text = "Start Translation";
                                }
                                break;
                            case 1:
                                {
                                    TransferWorkerThread.Suspend();
                                    btnStart.Text = "Resume";
                                    lblStatus.Text = "suspended";
                                }
                                break;
                        }
                    }
                    break;
                case ThreadState.Suspended:
                    {
                        TransferWorkerThread.Resume();
                        btnStart.Text = "Abort";
                    }
                    break;
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings dlgSettings = new frmSettings();
            dlgSettings.Show();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            frmAbout frmAboutProgram = new frmAbout();
            frmAboutProgram.ShowDialog();
        }

        private void backupRestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
