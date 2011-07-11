using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Access2Mssql
{
    class Access2MssqlLogger
    {
        private FileStream LogStream;
        private string LogFile;
        private bool IsReady;
        private bool NextStyle = true;

        public enum LogType
        {
            INFO,
            WARNING,
            ERROR
        }

        private void InternalWrite(string s)
        {
            byte[] Bytes = Encoding.Default.GetBytes(s);
            LogStream.Write(Bytes, 0, Bytes.Length);
        }

        public Access2MssqlLogger(string file)
        {
            LogFile = file;
            IsReady = false;
        }

        public void StartSession()
        {
            try
            {
                LogStream = new FileStream(LogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                IsReady = true;
            }
            catch
            {
                IsReady = false;
            }
        

            if (IsReady)
            {
                if (LogStream.Length == 0)
                {
                    InternalWrite("<html><head><title>Progress Logs</title><style type=\"text/css\">div{font-family: Verdana;font-size: 12px;height: 85px;}");
                    InternalWrite(".t{height: 15px;background-color: Blue;color: White;font-weight: bold;font-family: Verdana;font-size: 10px;}");
                    InternalWrite(".w{background-color: #F2D51C}.i{background-color: #628FC7}.e{background-color: #A81D1D}</style></head><body bgcolor=\"#b0c4dd\">");
                }
                else
                {
                    LogStream.Seek(LogStream.Length - 14, SeekOrigin.Begin);
                }
            }
        }

        public void Write(string log,LogType LType)
        {
            string s = LType == LogType.ERROR ? "e" : LType == LogType.WARNING ? "w" : "i";

            if (IsReady)
            {
                InternalWrite(string.Format("<div class=\"{0}\"><div class=\"t\">{1}</div>{2}</div>", s, DateTime.Now, log));
                NextStyle = !NextStyle;
            }
        }

        public void CloseSession()
        {
            if (IsReady)
            {
                InternalWrite("</body></html>");
                LogStream.Close();
                LogStream.Dispose();
                LogStream = null;
            }
            IsReady = false;
        }
    }
}
