using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Access2Mssql
{
    static class Program
    {
        public static AppSetting GeneralAppSetting;
        public static AccessToMssql Converter;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
