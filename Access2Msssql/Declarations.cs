using System;

namespace Access2Mssql
{
    #region Enums

    public enum ErrorType
    {
        COULD_NOT_CONNECT_ACCESS,
        COULD_NOT_CONNECT_MSSQL,
        OPERATION_FAILED
    }

    public enum FileSizeType
    {
        KB = 0,
        MB,
        GB
    }

    #endregion

    #region Delegates

    public delegate void ConnectionExceptionEventHandler(ErrorType e,string extend_message);
    public delegate void OperationFailedExceptionEventHandler(ref string error_content);
    public delegate void OnProgressEventHandler(string status);
    public delegate void OnCompleteEventHandler(ConvertSummary summary);
    

    #endregion

    #region Structures
   
    public struct DatabaseSettings
    {
        public string   AccessDbFile;
        public string   Server;
        public string   Database;
        public bool     IsSqlAuthencation;
        public bool     TrustedConnection;
        public string   Username;
        public string   Password;

        public void CreateInstance(out DatabaseSettings Instance)
        {
            Instance = new DatabaseSettings();
            Instance.AccessDbFile = this.AccessDbFile;
            Instance.Database = this.Database;
            Instance.IsSqlAuthencation = this.IsSqlAuthencation;
            Instance.Password = this.Password;
            Instance.Server = this.Server;
            Instance.TrustedConnection = this.TrustedConnection;
            Instance.Username = this.Username;
        }
    }

    public struct ColumnSchemes
    {
        public string  ColumnName;
        public Type    ColumnType;
        public string  SqlServerTypeStr;
        public bool    IsIncremented;
        public bool    IsPrimary;
    }

    public struct AppSetting
    {
        public string           Collation;
        public string           TextType;
        public int              Size;
        public bool             NewDatabaseDoesNotExist;
        public bool             AutoFixTextPrimaryAttempt;
        public bool             TryAutoDetectStoragePath;
        public string           DatabaseStoragePath;
        public ushort           DatabaseLevel;
        public int              MdfMaxSize;
        public int              LdfMaxSize;
        public FileSizeType     MdfSizeType;
        public FileSizeType     LdfSizeType;

    }

    public struct ConvertSummary
    {
        public ulong        TotalAffectedRows;
        public int          TableCount;
        public DateTime     OperationStartTime;
        public DateTime     OperationFinishTime;
        public TimeSpan     ElapsedTime;
        public UInt64       ConvertedTotalBytes;
        public double       AvarageTransferRate;
        public int          MissedRecords;
        public int          OccurredErrors;
        public int          FixedErrors;
    }

    #endregion
}