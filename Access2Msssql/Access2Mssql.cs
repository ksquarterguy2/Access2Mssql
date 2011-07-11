using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace Access2Mssql
{
    class AccessToMssql : IDisposable
    {
        #region Private Constans

        private const int INTER_DATABASE_NOT_FOUND = 0xfdc;

        #endregion

        #region Unsafe WinApi Declarations

        [DllImport("psapi.dll", SetLastError = true)]
        internal static extern bool EmptyWorkingSet(IntPtr hProcess);

        #endregion

        #region Private Members

        private SqlConnection           SqlServerConnection;
        
        private OleDbConnection         AccessDbConnection;
        
        private DatabaseSettings        GeneralDatabaseSettings;
        
        private StringBuilder           TableGeneratorScript;
        
        private List<ColumnSchemes []>  ColumnSchemeOfTables;
        
        private Access2MssqlLogger      OperationLogger;
        
        private string[]                TableNames;
        
        private ConvertSummary          ConvertingOperationSummary = new ConvertSummary();
        
        private bool                    IsReadyToTransfer = false;
        private bool                    OperationAbortSignal = false;
        private bool                    IsInternalCall;
        private bool                    IsEventSet = false;


        #endregion

        #region Events

        public event ConnectionExceptionEventHandler OnConnectionError;
        public event OperationFailedExceptionEventHandler OnOperationFailed;
        public event OnProgressEventHandler OnProgressStatus;
        public event OnCompleteEventHandler OnComplete;

        #endregion

        #region Internal Static Method
        
        internal static object[] ParseTextType(string TypeText)
        {
            object[] Result = new object[2];
            int Pos = TypeText.IndexOf('(');
            
            Result[0] = TypeText.Substring(0, Pos);

            try
            {
                Result[1] = Convert.ToInt32(TypeText.Substring(Pos + 1, TypeText.IndexOf(')') - Pos - 1));
            }
            catch
            {
                Result[1] = 50;
            }

            return Result;
        }

        #endregion

        #region Static Methods

        public static string GenerateMssqlConnectionString(DatabaseSettings Setting)
        {
            StringBuilder MssqlConnStr = new StringBuilder();
            MssqlConnStr.Append("Server=");
            MssqlConnStr.Append(Setting.Server + ";");

            MssqlConnStr.Append("Database=");
            MssqlConnStr.Append((Setting.Database == string.Empty) ? "master" : Setting.Database + ";");

            if (Setting.IsSqlAuthencation)
            {
                MssqlConnStr.Append("Integrated Security=SSPI;");

                MssqlConnStr.Append("User ID=");
                MssqlConnStr.Append(Setting.Username + ";");

                MssqlConnStr.Append("Password=");
                MssqlConnStr.Append(Setting.Password + ";");
            }

            MssqlConnStr.AppendFormat("Trusted_Connection={0};", (Setting.TrustedConnection) ? "True" : "False");

            return MssqlConnStr.ToString();
        }

        #endregion

        #region Private Methods

        private void GetTablesFromAccessDb()
        {
            TableNames = null;

            SetStatusText("gathering tables from access...");

            System.Data.DataTable TableSchemes = AccessDbConnection.GetSchema("Tables", new string[] { null, null, null, "Table" });

            if (TableSchemes.Rows.Count == 0)
                return;

            TableNames = new string[TableSchemes.Rows.Count];

            for (int i = 0; i < TableSchemes.Rows.Count; i++)
                TableNames[i] = TableSchemes.Rows[i][2].ToString();

            ConvertingOperationSummary.TableCount = TableNames.Length;
        }

        private bool IsContainColumn(ref System.Data.DataColumn[] Columns, string Id)
        {
            if (Columns == null)
                return false;

            for (int i = 0; i < Columns.Length; i++)
                if (Columns[i].ColumnName == Id)
                    return true;
            return false;
        }

        private bool GetTableColumnSchemes()
        {
            ColumnSchemes[] Schemes = null;
            System.Data.DataColumn[] ColumnArrayObject = null;
            System.Data.DataSet SchemeDataset = null;
            OleDbDataAdapter OleDbAdapter = null;
            
            ColumnSchemeOfTables = new List<ColumnSchemes[]>(TableNames.Length);

            SetStatusText("getting schemas...");

            for (int i = 0; i < TableNames.Length; i++)
            {
                SchemeDataset = new System.Data.DataSet();
                try
                {
                    OleDbAdapter = new OleDbDataAdapter("Select * From [" + TableNames[i] + "];", AccessDbConnection);
                    OleDbAdapter.FillSchema(SchemeDataset, System.Data.SchemaType.Mapped);
                    OleDbAdapter.Dispose();
                }
                catch (Exception ex)
                {
                    ConvertingOperationSummary.OccurredErrors++;
                    OperationLogger.Write(ex.Message, Access2MssqlLogger.LogType.ERROR);
                    continue;
                }

                if (SchemeDataset.Tables[0].Columns.Count == 0)
                    return false;

                Schemes = new ColumnSchemes[SchemeDataset.Tables[0].Columns.Count];

                ColumnArrayObject = SchemeDataset.Tables[0].PrimaryKey;

                for (int j = 0; j < Schemes.Length; j++)
                {
                    Schemes[j].ColumnName = SchemeDataset.Tables[0].Columns[j].ColumnName;
                    Schemes[j].ColumnType = SchemeDataset.Tables[0].Columns[j].DataType;
                    Schemes[j].IsIncremented = SchemeDataset.Tables[0].Columns[j].AutoIncrement;
                    Schemes[j].IsPrimary = IsContainColumn(ref ColumnArrayObject, Schemes[j].ColumnName);

                    switch (Type.GetTypeCode(Schemes[j].ColumnType))
                    {
                        case TypeCode.Double:
                            Schemes[j].SqlServerTypeStr = "float";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                            Schemes[j].SqlServerTypeStr = "int";
                            break;
                        case TypeCode.String:
                            {
                                if (Program.GeneralAppSetting.TextType.IndexOf("text") != -1)
                                    Schemes[j].SqlServerTypeStr = Program.GeneralAppSetting.TextType;
                                else
                                    Schemes[j].SqlServerTypeStr = string.Format("{0}({1})", Program.GeneralAppSetting.TextType, Program.GeneralAppSetting.Size);
                            }
                            break;
                        case TypeCode.DateTime:
                            Schemes[j].SqlServerTypeStr = "datetime";
                            break;
                        case TypeCode.Decimal:
                            Schemes[j].SqlServerTypeStr = "money";
                            break;
                        case TypeCode.Boolean:
                            Schemes[j].SqlServerTypeStr = "bit";
                            break;
                        case TypeCode.Object:
                            Schemes[j].SqlServerTypeStr = "varbinary(MAX)";
                            break;
                    }
                }

                ColumnSchemeOfTables.Add(Schemes);

                Schemes = null;
                SchemeDataset.Dispose();
                SchemeDataset = null;
            }
            return true;
        }

        private int FindPrimaryKey(ColumnSchemes[] c)
        {
            if (c == null)
                return -1;
            for (int i = 0; i < c.Length; i++)
                if (c[i].IsPrimary)
                    return i;
            return -1;
        }

        private bool IsIndentity(int Index)
        {
            if (ColumnSchemeOfTables == null)
                return false;
            for (int i = 0; i < ColumnSchemeOfTables[Index].Length; i++)
            {
                if (ColumnSchemeOfTables[Index][i].IsIncremented)
                    return true;
            }
            return false;
        }

        private void GenerateMssqlTableScripts()
        {
            
            for (int i = 0; i < ColumnSchemeOfTables.Count; i++)
            {
                SetStatusText("generating table creation script for " + TableNames[i]);

                TableGeneratorScript = new StringBuilder();

                TableGeneratorScript.Append("Create Table [");
                TableGeneratorScript.Append(TableNames[i] + "](");

                for (int j = 0; j < ColumnSchemeOfTables[i].Length; j++)
                {
                    TableGeneratorScript.Append(ColumnSchemeOfTables[i][j].ColumnName + " ");
                    
                    if (ColumnSchemeOfTables[i][j].SqlServerTypeStr.IndexOf("text") != -1
                        &&
                        ColumnSchemeOfTables[i][j].IsPrimary)
                    {
                        if (Program.GeneralAppSetting.AutoFixTextPrimaryAttempt)
                        {
                            TableGeneratorScript.Append("nvarchar(128) ");
                            
                            
                        }
                        else
                        {
                            ConvertingOperationSummary.OccurredErrors++;
                            OperationLogger.Write(ColumnSchemeOfTables[i][j].ColumnName + " should not set to primary. Because the column is text or ntext.",Access2MssqlLogger.LogType.ERROR);
                        }
                    }
                    else
                        TableGeneratorScript.Append(ColumnSchemeOfTables[i][j].SqlServerTypeStr + " ");


                    if (ColumnSchemeOfTables[i][j].IsIncremented)
                        TableGeneratorScript.Append("Identity(1,1) NOT ");

                    if (ColumnSchemeOfTables[i][j].ColumnType.Name == "String")
                        TableGeneratorScript.Append("Collate " + Program.GeneralAppSetting.Collation + " ");

                    if (ColumnSchemeOfTables[i][j].IsPrimary &&
                        !ColumnSchemeOfTables[i][j].IsIncremented)
                        TableGeneratorScript.Append("NOT ");

                    
                    TableGeneratorScript.Append("NULL, ");
                }

                int PrimaryIndex=0;

                if ((PrimaryIndex = FindPrimaryKey(ColumnSchemeOfTables[i])) != -1)
                {
                    TableGeneratorScript.Append("Constraint [PK_" + TableNames[i] + "] Primary Key Clustered");
                    TableGeneratorScript.Append("( " + ColumnSchemeOfTables[i][PrimaryIndex].ColumnName + " Asc )");
                    TableGeneratorScript.Append("With (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) On [PRIMARY]");
                    TableGeneratorScript.Append(") On [PRIMARY]");
                }
                else
                {
                    TableGeneratorScript.Remove(TableGeneratorScript.Length - 2, 2);
                    TableGeneratorScript.Append(") On [PRIMARY]");
                }

                try
                {
                    SqlCommand cmd = new SqlCommand(TableGeneratorScript.ToString(), SqlServerConnection);
                    SetStatusText("executing script...");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                }
                catch(Exception ex)
                {
                    ConvertingOperationSummary.OccurredErrors++;
                    OperationLogger.Write(ex.Message,Access2MssqlLogger.LogType.ERROR);
                }

                TableGeneratorScript.Remove(0, TableGeneratorScript.Length);
                TableGeneratorScript = null;
                
            }
        }

        private void SetIdentityInsert(string TableName,bool ModeOn)
        {
            string CommandQuery = "Set Identity_Insert {0} {1};";
            CommandQuery = string.Format(CommandQuery, TableName, (ModeOn) ? "On" : "Off");
            SqlCommand CmdObj = new SqlCommand(CommandQuery, SqlServerConnection);
            try
            {
                CmdObj.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                CmdObj.Dispose();
                CmdObj = null;
            }
        }

        private object ConvertToSQLDate(DateTime Date)
        {
            string TimeStr = Date.Hour.ToString() + ":" + Date.Minute.ToString() + ":" + Date.Second.ToString();
            return string.Format("{0}/{1}/{2} {3}", Date.Month, Date.Day, Date.Year, TimeStr);
        }

        private string ConvertOLETypeToSQLBinary(ref byte[] OleData)
        {
            StringBuilder BinaryData = new StringBuilder();
            BinaryData.Append("0x");

            for (int i=0;i<OleData.Length;i++)
                BinaryData.AppendFormat("{0:X}",OleData[i]);
            return BinaryData.ToString();
        }

        private void SetStatusText(string Text)
        {
            if (OnProgressStatus != null)
                OnProgressStatus(Text);

            if (OperationLogger != null)
                OperationLogger.Write(Text,Access2MssqlLogger.LogType.INFO);

        }

        private bool CreateDatabase()
        {
            StringBuilder DatabaseCreatorScript = new StringBuilder();
            SqlCommand CmdObject;
            DatabaseSettings DbSettingsInstance;
            GeneralDatabaseSettings.CreateInstance(out DbSettingsInstance);
            
            DbSettingsInstance.Database = "master";

            IsInternalCall = true;

            SetStatusText("connecting to sql server's master database...");
            
            if (Connect(DbSettingsInstance))
            {

                SetStatusText("generating database creator script...");

                string MdfFileMaxSize = (Program.GeneralAppSetting.MdfMaxSize == 0) ? "UNLIMITED" : Program.GeneralAppSetting.MdfMaxSize.ToString() + Program.GeneralAppSetting.MdfSizeType.ToString();
                string LdfFileMaxSize = (Program.GeneralAppSetting.LdfMaxSize == 0) ? "UNLIMITED" : Program.GeneralAppSetting.LdfMaxSize.ToString() + Program.GeneralAppSetting.LdfSizeType.ToString();


                if (string.IsNullOrEmpty(Program.GeneralAppSetting.DatabaseStoragePath) &&
                    Program.GeneralAppSetting.TryAutoDetectStoragePath)
                {
                    SetStatusText("default storage path is not set. trying detect...");
                    Program.GeneralAppSetting.DatabaseStoragePath = DetectMssqlStoragePath();

                    if (Program.GeneralAppSetting.DatabaseStoragePath == string.Empty)
                    {
                        Disconnect();
                        return false;
                    }

                }
           


                DatabaseCreatorScript.AppendFormat("Create Database [{0}] On Primary", GeneralDatabaseSettings.Database);
                DatabaseCreatorScript.AppendFormat("( Name=N'{0}', Filename=N'{1}\\{2}.mdf', Size = 34816KB, Maxsize = {3}, Filegrowth = 1024KB )",
                                    GeneralDatabaseSettings.Database,
                                    Program.GeneralAppSetting.DatabaseStoragePath,
                                    GeneralDatabaseSettings.Database,
                                    MdfFileMaxSize);



                DatabaseCreatorScript.Append(" Log On ");
                DatabaseCreatorScript.AppendFormat("( Name=N'{0}_log', Filename=N'{1}\\{2}_log.ldf', Size = 1024KB, Maxsize = {3}, Filegrowth = 10% )",
                                    GeneralDatabaseSettings.Database,
                                    Program.GeneralAppSetting.DatabaseStoragePath,
                                    GeneralDatabaseSettings.Database,
                                    LdfFileMaxSize);

                DatabaseCreatorScript.AppendFormat("Collate {0}; ", Program.GeneralAppSetting.Collation);

                DatabaseCreatorScript.AppendFormat("Exec dbo.sp_dbcmptlevel @dbname=N'{0}', @new_cmptlevel={1}0;",
                                    GeneralDatabaseSettings.Database,
                                    Program.GeneralAppSetting.DatabaseLevel);
                
                try
                {
                    SetStatusText("executing database creator script...");
                    CmdObject = new SqlCommand(DatabaseCreatorScript.ToString(), SqlServerConnection);
                    CmdObject.ExecuteNonQuery();
                    CmdObject.Dispose();

                    SetStatusText("database created. changing default database to '" + GeneralDatabaseSettings.Database + "'...");

                    CmdObject.CommandText = "use " + GeneralDatabaseSettings.Database;
                    CmdObject.Connection = SqlServerConnection;
                    CmdObject.ExecuteNonQuery();
                    CmdObject.Dispose();

                    CmdObject = null;

                }
                catch
                {
                    IsInternalCall = false;
                    return false;
                }

                return true;
            }
            IsInternalCall = false;
            return false;
        }

        private string DetectMssqlStoragePath()
        {
            SqlCommand CmdObject = null;
            SqlDataReader Result;
            string Path = string.Empty;

            CmdObject = new SqlCommand("use master; Select filename From sys.sysfiles;",
                                        SqlServerConnection);

            SetStatusText("detecting to sql server storage path...");

            try
            {
                Result = CmdObject.ExecuteReader();
                if (Result.HasRows)
                {
                    Result.Read();
                    Path = Result.GetValue(0).ToString();
                    Path = Path.Substring(0, Path.LastIndexOf('\\'));
                    Result.Close();
                    CmdObject.Dispose();
                    SetStatusText("sql server storage path has been detected.");
                }
            }
            catch
            {
                SetStatusText("could not detect storage path. setting default path...");
                Path = "c:\\Program Files\\Microsoft SQL Server\\MSSQL.1\\MSSQL\\DATA";
            }

            return Path;
        }

        private void CheckEventSet()
        {
            IsEventSet = false;

            if (OnProgressStatus != null ||
                OnComplete != null ||
                OnOperationFailed != null ||
                OnConnectionError != null)
                IsEventSet = true;
        }

        #endregion

        #region Properties

        public bool EventSet
        {
            get { CheckEventSet();  return IsEventSet; }
        }

        #endregion

        #region Public Methods

        public AccessToMssql()
        {
            OperationLogger = new Access2MssqlLogger(System.Windows.Forms.Application.StartupPath + "\\Log.html");
            OperationLogger.StartSession();
        }

        public bool Connect(DatabaseSettings Setting)
        {
            string OleDbConnStr = string.Empty;

            CheckEventSet();

            if (!IsInternalCall)
                GeneralDatabaseSettings = Setting;
            else
                IsInternalCall = false;
            
            SetStatusText("connecting to access database...");

            try
            {
                if (AccessDbConnection == null)
                {
                    OleDbConnStr = "Provider=Microsoft.JET.OLEDB.4.0; Data Source=";
                    OleDbConnStr += Setting.AccessDbFile;

                    AccessDbConnection = new OleDbConnection(OleDbConnStr);
                    AccessDbConnection.Open();
                }
            }
            catch (Exception e)
            {
                OperationLogger.Write(e.Message,Access2MssqlLogger.LogType.ERROR);
                ConvertingOperationSummary.OccurredErrors++;

                if (OnConnectionError != null)
                    OnConnectionError(ErrorType.COULD_NOT_CONNECT_ACCESS,e.Message);
                else
                    throw new AccessToMssqlException(e, ErrorType.COULD_NOT_CONNECT_ACCESS);

                SetStatusText("could not connect to access database");

                return false;
            }

            SetStatusText("connection to ms sql server...");

            SqlServerConnection = new SqlConnection(GenerateMssqlConnectionString(Setting));

            try
            {
                SqlServerConnection.Open();
            }
            catch (SqlException ex)
            {
                string extend_error_msg = ex.Message;
                OperationLogger.Write(ex.Message,Access2MssqlLogger.LogType.ERROR);
                ConvertingOperationSummary.OccurredErrors++;

                if (ex.Number == INTER_DATABASE_NOT_FOUND && Program.GeneralAppSetting.NewDatabaseDoesNotExist)
                {
                    System.Windows.Forms.MessageBox.Show("Database not found. Will be created automatically..", "Information",
                                                         System.Windows.Forms.MessageBoxButtons.OK,
                                                         System.Windows.Forms.MessageBoxIcon.Information);

                    if (CreateDatabase())
                        return true;
                    else
                    {
                        SetStatusText("could not create database. operation aborted.");
                        return false;
                    }
                }

                if (OnConnectionError != null)
                    OnConnectionError(ErrorType.COULD_NOT_CONNECT_MSSQL,extend_error_msg);
                else
                    throw new AccessToMssqlException(ex, ErrorType.COULD_NOT_CONNECT_MSSQL);



                SetStatusText("could not connect to ms sql server");

                return false;
            }

            SetStatusText("connections are ok");

            return (IsReadyToTransfer = true);
        }

        public void Disconnect()
        {
            if (AccessDbConnection != null)
                AccessDbConnection.Close();

            if (SqlServerConnection != null)
                SqlServerConnection.Close();

            IsReadyToTransfer = false;
        }

        public void StartTransfer()
        {
            OleDbCommand cmd = null;
            OleDbDataReader reader = null;
            bool IsIdentityContain = false;
            int CurrentTable = -1;

            ConvertingOperationSummary.TotalAffectedRows = 0;
            ConvertingOperationSummary.OperationStartTime = DateTime.Now;

            string InsertQuery = string.Empty, DatabaseValues = string.Empty;
            if (!IsReadyToTransfer)
                return;

            OperationAbortSignal = false;

            
            GetTablesFromAccessDb();

            if (TableNames == null)
                return;

            if (GetTableColumnSchemes())
            {
                GenerateMssqlTableScripts();

                for (int TableIndex = 0; TableIndex < TableNames.Length; TableIndex++)
                {
                    try
                    {
                        InsertQuery = string.Format("Insert Into [{0}](", TableNames[TableIndex]);

                        for (int j = 0; j < ColumnSchemeOfTables[TableIndex].Length; j++)
                        {
                            InsertQuery += ColumnSchemeOfTables[TableIndex][j].ColumnName;
                            if (j != ColumnSchemeOfTables[TableIndex].Length - 1)
                                InsertQuery += ",";
                        }

                        InsertQuery += ") Values({0});";

                        
                        cmd = new OleDbCommand("Select * From [" + TableNames[TableIndex] + "];", AccessDbConnection);
                        reader = cmd.ExecuteReader();


                        IsIdentityContain = IsIndentity(TableIndex);
                        if (IsIdentityContain)
                            SetIdentityInsert(TableNames[TableIndex], true);

                        SetStatusText("converting access types to ms sql types...");

                        while (reader.Read())
                        {
                            if (OperationAbortSignal)
                            {
                                Disconnect();
                                reader.Close();
                                reader.Dispose();
                                Dispose();
                                return;
                            }

                            for (int ColumnIndex = 0; ColumnIndex < ColumnSchemeOfTables[TableIndex].Length; ColumnIndex++)
                            {

                                object o = reader[ColumnSchemeOfTables[TableIndex][ColumnIndex].ColumnName];

                               
                                TypeCode type = Type.GetTypeCode(ColumnSchemeOfTables[TableIndex][ColumnIndex].ColumnType);

                                switch (type)
                                {
                                    case TypeCode.Boolean:
                                        {
                                            DatabaseValues += (o.ToString().ToLower() == "true") ? "1" : "0";
                                            ConvertingOperationSummary.ConvertedTotalBytes += 1;
                                        }
                                        break;
                                    case TypeCode.Object:
                                        {
                                            if (ColumnSchemeOfTables[TableIndex][ColumnIndex].ColumnType.Name == "Byte[]")
                                            {
                                                byte[] OleData = (byte[])o;
                                                ConvertingOperationSummary.ConvertedTotalBytes += (ulong)OleData.Length;

                                                DatabaseValues += ConvertOLETypeToSQLBinary(ref OleData);
                                                OleData = null;
                                            }
                                        }
                                        break;
                                    case TypeCode.String:
                                    case TypeCode.DateTime:
                                        {
                                            if (type == TypeCode.String)
                                            {
                                                o = o.ToString().Replace("\'", "\'\'");
                                                ConvertingOperationSummary.ConvertedTotalBytes += (ulong)o.ToString().Length;
                                                if (Program.GeneralAppSetting.Size < o.ToString().Length
                                                    && Program.GeneralAppSetting.TextType.IndexOf("text") == -1)
                                                {
                                                    string ErrorMessage = "Input size overflow. Allocated size is " +
                                                                          Program.GeneralAppSetting.Size.ToString() +
                                                                          ", input size is " + o.ToString().Length.ToString();
                                                    OperationLogger.Write(ErrorMessage,Access2MssqlLogger.LogType.WARNING);
                                                }
                                            }
                                            else
                                            {
                                                ConvertingOperationSummary.ConvertedTotalBytes += 8;
                                                try
                                                {
                                                    o = ConvertToSQLDate(Convert.ToDateTime(o));
                                                }
                                                catch
                                                { o = "null"; }


                                            }
                                            if (o.ToString() != "null")
                                                DatabaseValues += string.Format("\'{0}\'", o.ToString());
                                            else
                                                DatabaseValues += o.ToString();

                                        }
                                        break;
                                    case TypeCode.Int32:
                                        {
                                            ConvertingOperationSummary.ConvertedTotalBytes += 4;
                                            DatabaseValues += (o.ToString() == string.Empty) ? "null" : o.ToString();
                                        }
                                        break;
                                    case TypeCode.Decimal:
                                        {
                                            ConvertingOperationSummary.ConvertedTotalBytes += 8;
                                            DatabaseValues += (o.ToString() == string.Empty) ? "null" : o.ToString();
                                        }
                                        break;
                                    default:
                                        DatabaseValues += (o.ToString() == string.Empty) ? "null" : o.ToString();
                                        break;
                                }
                                
                         
                                if (ColumnIndex != ColumnSchemeOfTables[TableIndex].Length - 1)
                                    DatabaseValues += ",";

                            }

                            string sql_query = string.Format(InsertQuery, DatabaseValues);
                            SqlCommand sqlcmd = null;

                            try
                            {
                                if (OperationAbortSignal)
                                {
                                    Disconnect();
                                    reader.Close();
                                    reader.Dispose();
                                    Dispose();
                                    return;
                                }

                                sqlcmd = new SqlCommand(sql_query, SqlServerConnection);

                                if (CurrentTable != TableIndex)
                                {
                                    SetStatusText("transferring values to " + TableNames[TableIndex]);
                                    CurrentTable = TableIndex;
                                }
                                sqlcmd.ExecuteNonQuery();
                                ConvertingOperationSummary.TotalAffectedRows++;

                                sqlcmd.Dispose();
                                sqlcmd = null;
                            }
                            catch (Exception ex)
                            {
                                sqlcmd.Dispose();
                                sqlcmd = null;

                                if (ex.Message.IndexOf("Cannot insert duplicate key in object") != -1)
                                {
                                    SqlCommand fixcmd = new SqlCommand(string.Format("ALTER TABLE dbo.{0} DROP CONSTRAINT PK_{1};", TableNames[TableIndex], TableNames[TableIndex]), SqlServerConnection);

                                    try
                                    {
                                        fixcmd.ExecuteNonQuery();
                                        fixcmd = new SqlCommand(sql_query, SqlServerConnection);
                                        fixcmd.ExecuteNonQuery();
                                        ConvertingOperationSummary.TotalAffectedRows++;
                                        fixcmd.Dispose();

                                        ConvertingOperationSummary.FixedErrors++;

                                        OperationLogger.Write("Duplicate key fixed successfuly for " + TableNames[TableIndex], Access2MssqlLogger.LogType.INFO);



                                    }
                                    catch { OperationLogger.Write("Duplicate key fixing error!!!!!", Access2MssqlLogger.LogType.ERROR); }

                                }
                                else
                                {

                                    OperationLogger.Write(ex.Message, Access2MssqlLogger.LogType.ERROR);
                                    string s = ex.Message;

                                    if (OnOperationFailed != null)
                                        OnOperationFailed(ref s);
                                    ConvertingOperationSummary.MissedRecords++;
                                    ConvertingOperationSummary.OccurredErrors++;
                                }
                            }
                            finally
                            {
                                DatabaseValues = string.Empty;
                            }
                        }

                        if (IsIdentityContain)
                            SetIdentityInsert(TableNames[TableIndex], false);

                        IsIdentityContain = false;

                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                    catch (Exception ex)
                    {
                        string ErrorMessage = "Operation failed during progress...\n\n";
                        ErrorMessage += ex.Message;

                        ConvertingOperationSummary.OccurredErrors++;
                        OperationLogger.Write(ex.Message,Access2MssqlLogger.LogType.ERROR);

                        if (OnOperationFailed != null)
                            OnOperationFailed(ref ErrorMessage);
                    }
                    finally
                    {
                        DatabaseValues = string.Empty;
                        if (IsIdentityContain && !OperationAbortSignal)
                            SetIdentityInsert(TableNames[TableIndex], false);
                    }
                }

                ConvertingOperationSummary.OperationFinishTime = DateTime.Now;
                
                ConvertingOperationSummary.ElapsedTime = ConvertingOperationSummary.OperationFinishTime.Subtract(ConvertingOperationSummary.OperationStartTime);

                ConvertingOperationSummary.AvarageTransferRate = (int)((ConvertingOperationSummary.ConvertedTotalBytes / 1024) / ConvertingOperationSummary.ElapsedTime.TotalSeconds);

                SetStatusText("finishing the progress...");

            }

            OperationLogger.CloseSession();

            SetStatusText("operation completed");
            if (OnComplete != null)
                OnComplete(ConvertingOperationSummary);
            
        }

        public void AbortOperation()
        {
            OperationLogger.CloseSession();
            OperationAbortSignal = true;
        }

        public void Dispose()
        {
            SqlServerConnection = null;
            AccessDbConnection = null;
            IsReadyToTransfer = false;
            
            TableGeneratorScript = null;

            if (ColumnSchemeOfTables != null)
            {
                for (int i = 0; i < ColumnSchemeOfTables.Count; i++)
                    ColumnSchemeOfTables[i] = null;

                ColumnSchemeOfTables.Clear();
                ColumnSchemeOfTables = null;
                
            }

            ConvertingOperationSummary.AvarageTransferRate = 0;
            ConvertingOperationSummary.ConvertedTotalBytes = 0;
            ConvertingOperationSummary.ElapsedTime = TimeSpan.MinValue;
            ConvertingOperationSummary.MissedRecords = 0;
            ConvertingOperationSummary.OccurredErrors = 0;
            ConvertingOperationSummary.OperationFinishTime = DateTime.MinValue;
            ConvertingOperationSummary.OperationStartTime = DateTime.MinValue;
            ConvertingOperationSummary.TableCount = 0;
            ConvertingOperationSummary.TotalAffectedRows = 0;

            TableNames = null;

            EmptyWorkingSet(System.Diagnostics.Process.GetCurrentProcess().Handle);
        }

        #endregion

    }
}
