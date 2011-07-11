using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Access2Mssql
{
    public partial class frmSettings : Form
    {
        private string[] DefaultCollations = new string[] 
        {
            "Turkish_BIN",
            "Turkish_BIN2",
            "Turkish_CI_AI",
            "Turkish_CI_AI_WS",
            "Turkish_CI_AI_KS",
            "Turkish_CI_AI_KS_WS",
            "Turkish_CI_AS",
            "Turkish_CI_AS_WS",
            "Turkish_CI_AS_KS",
            "Turkish_CI_AS_KS_WS",
            "Turkish_CS_AI",
            "Turkish_CS_AI_WS",
            "Turkish_CS_AI_KS",
            "Turkish_CS_AI_KS_WS",
            "Turkish_CS_AS",
            "Turkish_CS_AS_WS",
            "Turkish_CS_AS_KS",
            "Turkish_CS_AS_KS_WS"
        };

        public frmSettings()
        {
            InitializeComponent();
        }

        private void PrepateExtendedSettings()
        {

            pnlDbSettings.Enabled = chkExistDb.Checked;
            pnlDbSettings.BackColor = (chkExistDb.Checked) ? Color.DarkSeaGreen : Color.DarkRed;
        }

        private void cbTextType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtVarSize.Enabled = !(cbTextType.SelectedIndex > 3);
        }

        private void txtVarSize_TextChanged(object sender, EventArgs e)
        {
            int size=0;
            try
            {
                size = Convert.ToInt32(txtVarSize.Text);
            }
            catch
            {
                txtVarSize.Text = "0";
                size=0;
            }

            if (size < 0 || size > 65536)
            {
                txtVarSize.Text = "0";
                size = 0;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StreamWriter setting_file = new StreamWriter("setting.ini");

            try
            {
                setting_file.WriteLine("collation={0}", cbCollation.Text);
                setting_file.WriteLine("text_type_def={0}", cbTextType.Text + "(" + txtVarSize.Text + ")");
                setting_file.WriteLine("auto_fix={0}", chkFix.Checked.ToString().ToLower());
                setting_file.WriteLine("new_database={0}", chkExistDb.Checked.ToString().ToLower());
                setting_file.WriteLine("sql_server_storage_path={0}", txtStoragePath.Text);
                setting_file.WriteLine("auto_detect_storage_path={0}", chkAutoDetect.Checked.ToString().ToLower());
                setting_file.WriteLine("compatible_level={0}", mnCompatibleLevel.Value);
                setting_file.WriteLine("mdf_max_size={0}", txtMdfMax.Text);
                setting_file.WriteLine("mdf_type={0}", cbMdfType.SelectedIndex);
                setting_file.WriteLine("ldf_max_size={0}", txtLdfMax.Text);
                setting_file.WriteLine("ldf_type={0}", cbLdfType.SelectedIndex);


                Program.GeneralAppSetting.Collation = cbCollation.Text;
                Program.GeneralAppSetting.TextType = cbTextType.Text;
                Program.GeneralAppSetting.Size = Convert.ToInt32(txtVarSize.Text);
                Program.GeneralAppSetting.NewDatabaseDoesNotExist = chkExistDb.Checked;
                Program.GeneralAppSetting.AutoFixTextPrimaryAttempt = chkFix.Checked;
                Program.GeneralAppSetting.DatabaseStoragePath = txtStoragePath.Text;
                Program.GeneralAppSetting.DatabaseLevel = (ushort)mnCompatibleLevel.Value;
                Program.GeneralAppSetting.MdfMaxSize = Convert.ToInt32(txtMdfMax.Text);
                Program.GeneralAppSetting.MdfSizeType = (FileSizeType)cbLdfType.SelectedIndex;
                Program.GeneralAppSetting.LdfMaxSize = Convert.ToInt32(txtLdfMax.Text);
                Program.GeneralAppSetting.LdfSizeType = (FileSizeType)cbLdfType.SelectedIndex;

            }
            catch
            {
                MessageBox.Show("Settings file corrupted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            setting_file.Close();
            this.Close();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            StreamReader collations;
            try
            {
                
                collations = new StreamReader(Application.StartupPath + "\\collations.cdb");
                do
                {
                    cbCollation.Items.Add(collations.ReadLine());
                } while (collations.EndOfStream != true);

                cbCollation.SelectedIndex = 0;

                collations.Close();
                collations.Dispose();

            }
            catch
            {
                MessageBox.Show("collations.cdb not found!");
                for (int i = 0; i < DefaultCollations.Length; i++)
                    cbCollation.Items.Add(DefaultCollations[i]);
                cbCollation.SelectedIndex = 0;
            
            }

            
            cbCollation.Text = Program.GeneralAppSetting.Collation;
            cbTextType.SelectedIndex = cbTextType.FindString(Program.GeneralAppSetting.TextType);
            txtVarSize.Text = Program.GeneralAppSetting.Size.ToString();
            chkExistDb.Checked = Program.GeneralAppSetting.NewDatabaseDoesNotExist;
            chkFix.Checked = Program.GeneralAppSetting.AutoFixTextPrimaryAttempt;

            txtStoragePath.Text = Program.GeneralAppSetting.DatabaseStoragePath;
            chkAutoDetect.Checked = Program.GeneralAppSetting.TryAutoDetectStoragePath;
            mnCompatibleLevel.Value = Program.GeneralAppSetting.DatabaseLevel;
            txtMdfMax.Text = Program.GeneralAppSetting.MdfMaxSize.ToString();
            cbMdfType.SelectedIndex = (int)Program.GeneralAppSetting.MdfSizeType;
            txtLdfMax.Text = Program.GeneralAppSetting.LdfMaxSize.ToString();
            cbLdfType.SelectedIndex = (int)Program.GeneralAppSetting.LdfSizeType;

            PrepateExtendedSettings();
        }


        private void chkExistDb_CheckedChanged(object sender, EventArgs e)
        {
            PrepateExtendedSettings();
        }
    }
}
