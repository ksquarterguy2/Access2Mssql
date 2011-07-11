using System;
using System.Collections.Generic;
using System.Text;

namespace Access2Mssql
{
    public class AccessToMssqlException : Exception
    {
        private ErrorType m_ErrorType;
        private Exception m_Exception;

        public AccessToMssqlException(Exception inner, ErrorType err)
        {
            m_Exception = inner;
            m_ErrorType = err;
        }

        public override System.Collections.IDictionary Data
        {
            get { return m_Exception.Data; }
        }

        public override string Message
        {
            get { return m_Exception.Message; }
        }

        public override string StackTrace
        {
            get { return m_Exception.StackTrace; }
        }

        public override string ToString()
        {
            return m_Exception.ToString();
        }

        public ErrorType ErrorType
        {
            get { return m_ErrorType; }
        }
    }

}
