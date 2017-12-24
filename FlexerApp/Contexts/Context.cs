using FlexerApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace FlexerApp.Contexts
{
    public class Context
    {
        private const string CONST_DATABASENAME = "TempDB.sqlite";
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.sss";

        /// <summary>
        /// Privates the connection.
        /// </summary>
        /// <returns></returns>
        private SQLiteConnection PrivateConnection()
        {
            return new SQLiteConnection(string.Format("Data Source={0};Version=3;datetimeformat=CurrentCulture", CONST_DATABASENAME));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        public Context()
        {
            if (!File.Exists(CONST_DATABASENAME))
                SQLiteConnection.CreateFile(CONST_DATABASENAME);

            CreateTable();
        }

        #region Keyboard Mouse Log Function

        /// <summary>
        /// Creates the data.
        /// </summary>
        /// <param name="item">The item.</param>
        public void CreateData(KeyboardMouseLogModel item)
        {
            string insertQuery = string.Empty;

            insertQuery = string.Format("INSERT INTO KeyboardMouseLogModel ( SessionId, ActivityName, ActivityType, KeyStrokeCount, MouseClickCount, StartTime, EndTime ) " +
                                        "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                                         item.SessionId, item.ActivityName, item.ActivityType, item.KeyStrokeCount, item.MouseClickCount, item.StartTime, item.EndTime);

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = insertQuery;
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdateData(KeyboardMouseLogModel item)
        {
            string updateQuery = string.Empty;

            updateQuery = string.Format("UPDATE KeyboardMouseLogModel SET KeyStrokeCount = '{0}', MouseClickCount = '{1}', StartTime = '{2}', EndTime = '{3}', IsSuccessSendToServer = '{4}' " +
                                        "WHERE KeyboardMouseLogModelId = '{5}'", item.KeyStrokeCount, item.MouseClickCount, item.StartTime, item.EndTime, item.IsSuccessSendToServer, item.KeyboardMouseLogModelId);

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var updateCommand = connection.CreateCommand();
                    updateCommand.Transaction = transaction;
                    updateCommand.CommandText = updateQuery;
                    updateCommand.CommandType = System.Data.CommandType.Text;
                    updateCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes the data success send to server.
        /// </summary>
        public void DeleteDataSuccessSendToServer()
        {
            string deleteQuery = "DELETE FROM KeyboardMouseLogModel WHERE IsSuccessSendToServer = 'true'";

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var deleteCommand = connection.CreateCommand();
                    deleteCommand.Transaction = transaction;
                    deleteCommand.CommandText = deleteQuery;
                    deleteCommand.CommandType = System.Data.CommandType.Text;
                    deleteCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Retrieves the data.
        /// </summary>
        /// <param name="KeyboardMouseLogModelId">The keyboard mouse log model identifier.</param>
        /// <returns></returns>
        public KeyboardMouseLogModel RetrieveData(string KeyboardMouseLogModelId)
        {
            KeyboardMouseLogModel result = new KeyboardMouseLogModel();
            string retrieveQuery = String.Format("SELECT * FROM KeyboardMouseLogModel WHERE KeyboardMouseLogModelId = '{0}'", KeyboardMouseLogModelId);
            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    DateTime dateTimeResult = new DateTime(1900, 1, 1);
                    KeyboardMouseLogModel recordData = new KeyboardMouseLogModel();
                    var sqlCommand = connection.CreateCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.CommandText = retrieveQuery;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            recordData.KeyboardMouseLogModelId = reader["KeyboardMouseLogModelId"] != DBNull.Value ? Convert.ToInt32(reader["KeyboardMouseLogModelId"]) : 0;
                            recordData.SessionId = reader["SessionId"] != DBNull.Value ? Convert.ToInt32(reader["SessionId"]) : 0;
                            recordData.ActivityName = reader["ActivityName"] != DBNull.Value ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != DBNull.Value ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.KeyStrokeCount = reader["KeyStrokeCount"] != DBNull.Value ? Convert.ToInt64(reader["KeyStrokeCount"]) : 0;
                            recordData.MouseClickCount = reader["MouseClickCount"] != DBNull.Value ? Convert.ToInt64(reader["MouseClickCount"]) : 0;
                            if (reader["StartTime"] != DBNull.Value)//&& DateTime.TryParseExact(reader["StartTime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.StartTime = Convert.ToDateTime(reader["StartTime"]);// dateTimeResult;
                            if (reader["EndTime"] != DBNull.Value)//&& DateTime.TryParseExact(reader["EndTime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.EndTime = Convert.ToDateTime(reader["EndTime"]);// dateTimeResult;
                            recordData.IsSuccessSendToServer = reader["IsSuccessSendToServer"] != DBNull.Value ? Convert.ToBoolean(reader["IsSuccessSendToServer"]) : false;
                            if (reader["CreatedDate"] != DBNull.Value)//&& DateTime.TryParseExact(reader["EndTime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);// dateTimeResult;
                            recordData.RowStatus = reader["RowStatus"] != DBNull.Value ? Convert.ToInt32(reader["RowStatus"]) : 0;
                        }
                    }
                    transaction.Commit();
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the log list send to server.
        /// </summary>
        /// <returns></returns>
        public List<KeyboardMouseLogModel> GetLogListSendToServer()
        {
            List<KeyboardMouseLogModel> result = new List<KeyboardMouseLogModel>();

            string retrieveQuery = "SELECT * FROM KeyboardMouseLogModel WHERE IsSuccessSendToServer = 'False'";

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    DateTime dateTimeResult = new DateTime(1900, 1, 1);
                    KeyboardMouseLogModel recordData = new KeyboardMouseLogModel();
                    var sqlCommand = connection.CreateCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.CommandText = retrieveQuery;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recordData.KeyboardMouseLogModelId = reader["KeyboardMouseLogModelId"] != DBNull.Value ? Convert.ToInt32(reader["KeyboardMouseLogModelId"]) : 0;
                            recordData.SessionId = reader["SessionId"] != DBNull.Value ? int.Parse(reader["SessionId"].ToString()) : 0;
                            recordData.ActivityName = reader["ActivityName"] != DBNull.Value ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != DBNull.Value ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.KeyStrokeCount = reader["KeyStrokeCount"] != DBNull.Value ? long.Parse(reader["KeyStrokeCount"].ToString()) : 0;
                            recordData.MouseClickCount = reader["MouseClickCount"] != DBNull.Value ? long.Parse(reader["MouseClickCount"].ToString()) : 0;
                            if (reader["CreatedDate"] != DBNull.Value)//&& DateTime.TryParseExact(reader["EndTime"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                            if (reader["StartTime"] != DBNull.Value)//&& DateTime.TryParseExact(reader["StartTime"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.StartTime = Convert.ToDateTime(reader["StartTime"]);//dateTimeResult;
                            if (reader["EndTime"] != DBNull.Value)// && DateTime.TryParseExact(reader["EndTime"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.EndTime = Convert.ToDateTime(reader["EndTime"]); //dateTimeResult;
                            recordData.IsSuccessSendToServer = reader["IsSuccessSendToServer"] != DBNull.Value ? Convert.ToBoolean(reader["IsSuccessSendToServer"]) : false;

                            recordData.RowStatus = reader["RowStatus"] != DBNull.Value ? Convert.ToInt32(reader["RowStatus"]) : 0;

                            result.Add(recordData);
                        }
                    }
                    transaction.Commit();
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the last active data.
        /// </summary>
        /// <returns></returns>
        public KeyboardMouseLogModel GetLastActiveData()
        {
            KeyboardMouseLogModel result = new KeyboardMouseLogModel();
            string retrieveQuery = String.Format("SELECT * FROM KeyboardMouseLogModel ORDER BY StartTime DESC LIMIT 1");
            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    DateTime dateTimeResult = new DateTime(1900, 1, 1);
                    KeyboardMouseLogModel recordData = new KeyboardMouseLogModel();
                    var sqlCommand = connection.CreateCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.CommandText = retrieveQuery;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            recordData.KeyboardMouseLogModelId = reader["KeyboardMouseLogModelId"] != DBNull.Value ? Convert.ToInt32(reader["KeyboardMouseLogModelId"]) : 0;
                            recordData.SessionId = reader["SessionId"] != DBNull.Value ? Convert.ToInt32(reader["SessionId"]) : 0;
                            recordData.ActivityName = reader["ActivityName"] != DBNull.Value ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != DBNull.Value ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.KeyStrokeCount = reader["KeyStrokeCount"] != DBNull.Value ? Convert.ToInt64(reader["KeyStrokeCount"].ToString()) : 0;
                            recordData.MouseClickCount = reader["MouseClickCount"] != DBNull.Value ? Convert.ToInt64(reader["MouseClickCount"].ToString()) : 0;
                            if (reader["StartTime"] != DBNull.Value)// && DateTime.TryParseExact(reader["StartTime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.StartTime = Convert.ToDateTime(reader["StartTime"]);// dateTimeResult;
                            if (reader["EndTime"] != DBNull.Value)// && DateTime.TryParseExact(reader["EndTime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.EndTime = Convert.ToDateTime(reader["EndTime"]);// dateTimeResult;
                            recordData.IsSuccessSendToServer = reader["IsSuccessSendToServer"] != DBNull.Value ? Convert.ToBoolean(reader["IsSuccessSendToServer"]) : false;
                            if (reader["CreatedDate"] != DBNull.Value && DateTime.TryParseExact(reader["EndTime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);// dateTimeResult;
                            recordData.RowStatus = reader["RowStatus"] != DBNull.Value ? Convert.ToInt32(reader["RowStatus"]) : 0;
                        }
                    }
                    transaction.Commit();
                }
            }
            return result;
        }

        #endregion

        #region Image Capture Function

        /// <summary>
        /// Creates the image data.
        /// </summary>
        /// <param name="item">The item.</param>
        public void CreateImageData(ScreenshotLogModel item)
        {
            string insertQuery = string.Empty;

            insertQuery = string.Format("INSERT INTO ScreenshotLogModel ( ScreenshotLogModelId, SessionId, ActivityName, ActivityType, Image, CaptureScreenDate, IsSuccessSendToServer, CreatedDate, RowStatus ) " +
                                        "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                        item.ScreenshotLogModelId, item.SessionId, item.ActivityName, item.ActivityType, Convert.ToBase64String(item.Image), item.CaptureScreenDate, item.IsSuccessSendToServer, DateTime.Now.ToString(DATE_FORMAT), 0);

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = insertQuery;
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Updates the image data.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdateImageData(ScreenshotLogModel item)
        {
            string updateQuery = string.Empty;

            updateQuery = string.Format("UPDATE ScreenshotLogModel SET IsSuccessSendToServer = '{0}' " +
                                        "WHERE ScreenshotLogModelId = '{1}'", item.IsSuccessSendToServer, item.ScreenshotLogModelId);

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var updateCommand = connection.CreateCommand();
                    updateCommand.Transaction = transaction;
                    updateCommand.CommandText = updateQuery;
                    updateCommand.CommandType = System.Data.CommandType.Text;
                    updateCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes the image success send to server.
        /// </summary>
        public void DeleteImageSuccessSendToServer()
        {
            string deleteQuery = "DELETE FROM ScreenshotLogModel WHERE IsSuccessSendToServer = 'True'";

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var deleteCommand = connection.CreateCommand();
                    deleteCommand.Transaction = transaction;
                    deleteCommand.CommandText = deleteQuery;
                    deleteCommand.CommandType = System.Data.CommandType.Text;
                    deleteCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Gets the screenshot log list send to server.
        /// </summary>
        /// <returns></returns>
        public List<ScreenshotLogModel> GetScreenshotLogListSendToServer()
        {
            List<ScreenshotLogModel> result = new List<ScreenshotLogModel>();
            string retrieveQuery = "SELECT * FROM ScreenshotLogModel WHERE IsSuccessSendToServer = 'False'";
            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    DateTime dateTimeResult = new DateTime(1900, 1, 1);
                    ScreenshotLogModel recordData = new ScreenshotLogModel();
                    var sqlCommand = connection.CreateCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.CommandText = retrieveQuery;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recordData.ScreenshotLogModelId = reader["ScreenshotLogModelId"] != DBNull.Value ? Convert.ToInt32(reader["ScreenshotLogModelId"]) : 0;
                            recordData.SessionId = reader["SessionId"] != DBNull.Value ? Convert.ToInt32(reader["SessionId"]) : 0;
                            recordData.ActivityName = reader["ActivityName"] != DBNull.Value ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != DBNull.Value ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.Image = reader["Image"] != DBNull.Value ? Convert.FromBase64String(reader["Image"].ToString()) : new byte[0];
                            if (reader["CaptureScreenDate"] != DBNull.Value)// &&(DateTime.TryParseExact(reader["CaptureScreenDate"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult)))
                                recordData.CaptureScreenDate = Convert.ToDateTime(reader["CaptureScreenDate"]);// dateTimeResult;
                            recordData.IsSuccessSendToServer = reader["IsSuccessSendToServer"] != DBNull.Value ? Convert.ToBoolean(reader["IsSuccessSendToServer"].ToString()) : false;
                            if (reader["CreatedDate"] != DBNull.Value)// && DateTime.TryParseExact(reader["EndTime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);// dateTimeResult;
                            recordData.RowStatus = reader["RowStatus"] != DBNull.Value ? Convert.ToInt32(reader["RowStatus"]) : 0;

                            result.Add(recordData);
                        }
                    }
                    transaction.Commit();
                }
            }
            return result;
        }

        #endregion

        #region Login Function

        /// <summary>
        /// Creates the login session.
        /// </summary>
        /// <param name="item">The item.</param>
        public void CreateLoginSession(LoginModel item)
        {
            string insertQuery = string.Empty;

            insertQuery = string.Format("INSERT INTO LoginModel ( Email, Password, LocationType, IPAddress, City, Lat, Long, SessionId, loginToken) " +
                                        "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                        item.Email, item.Password, item.LocationType, item.IPAddress, item.City, item.Lat, item.Long, item.SessionId, item.LoginToken);

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = insertQuery;
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Gets the login session.
        /// </summary>
        /// <returns></returns>
        public LoginModel GetLoginSession()
        {
            LoginModel result = new LoginModel();
            string retrieveQuery = String.Format("SELECT * FROM LoginModel ORDER BY CreatedDate DESC LIMIT 1");
            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    DateTime dateTimeResult = new DateTime(1900, 1, 1);

                    var sqlCommand = connection.CreateCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.CommandText = retrieveQuery;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.LoginModelId = reader["LoginModelId"] != DBNull.Value ? Convert.ToInt32(reader["LoginModelId"]) : 0;
                            result.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty;
                            result.Password = reader["Password"] != DBNull.Value ? reader["Password"].ToString() : string.Empty;
                            result.LocationType = reader["LocationType"] != DBNull.Value ? reader["LocationType"].ToString() : string.Empty;
                            result.IPAddress = reader["IPAddress"] != DBNull.Value ? reader["IPAddress"].ToString() : string.Empty;
                            result.City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty;
                            result.Lat = reader["Lat"] != DBNull.Value ? Convert.ToDecimal(reader["Lat"]) : 0;
                            result.Long = reader["Long"] != DBNull.Value ? Convert.ToDecimal(reader["Long"]) : 0;
                            result.SessionId = reader["SessionId"] != DBNull.Value ? Convert.ToInt32(reader["SessionId"]) : 0;
                            result.LoginToken = reader["loginToken"] != DBNull.Value ? reader["loginToken"].ToString() : string.Empty;
                            if (reader["CreatedDate"] != DBNull.Value)// && DateTime.TryParseExact(reader["CreatedDate"].ToString(), DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                result.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);// dateTimeResult;
                            result.RowStatus = reader["RowStatus"] != DBNull.Value ? Convert.ToInt32(reader["RowStatus"]) : 0;
                        }
                    }
                    transaction.Commit();
                }
            }
            return result;
        }

        #endregion

        #region User Task Function

        public void CreateTaskData(UserTaskModel item)
        {
            string insertQuery = string.Empty;

            insertQuery = string.Format("INSERT INTO UserTaskModel ( SessionId, TaskId, TaskStatus, TaskDate ) " +
                                        "VALUES ('{0}','{1}','{2}','{3}','{4}')",
                                        item.SessionId, item.TaskId, item.TaskStatus, item.TaskDate);

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = insertQuery;
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        #endregion

        #region Table Function

        /// <summary>
        /// Checks the is table exist.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private bool CheckIsTableExist(string tableName)
        {
            bool tableIsExist = false;

            string checkScript = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}' LIMIT 1", tableName);

            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    DateTime dateTimeResult = new DateTime(1900, 1, 1);
                    KeyboardMouseLogModel recordData = new KeyboardMouseLogModel();
                    var sqlCommand = connection.CreateCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.CommandText = checkScript;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        tableIsExist = reader.Read();
                        transaction.Commit();
                    }
                }
            }
            return tableIsExist;
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        private void CreateTable()
        {
            string createScript = string.Empty;

            if (!CheckIsTableExist("LoginModel"))
            {
                createScript = @" CREATE TABLE LoginModel
                                    ( 
                                        LoginModelId    INTEGER  PRIMARY KEY AUTOINCREMENT,
                                        Email           TEXT,
                                        Password        TEXT,
                                        LocationType    TEXT,
                                        IPAddress       TEXT,
                                        City            TEXT,
                                        Lat             REAL,
                                        Long            REAL,
                                        SessionId       INT,
                                        LoginToken      TEXT,
                                        CreatedDate     DATETIME DEFAULT (CURRENT_TIMESTAMP),
                                        RowStatus       INTEGER  DEFAULT (0) 
                                  )";
                ExecuteNonQueryScript(createScript);
            }

            if (!CheckIsTableExist("KeyboardMouseLogModel"))
            {
                createScript = @"CREATE TABLE KeyboardMouseLogModel 
                                    (
                                        KeyboardMouseLogModelId INTEGER  PRIMARY KEY AUTOINCREMENT,
                                        SessionId               INTEGER,
                                        ActivityName            TEXT,
                                        ActivityType            TEXT,
                                        KeyStrokeCount          BIGINT,
                                        MouseClickCount         BIGINT,
                                        StartTime               DATETIME,
                                        EndTime                 DATETIME,
                                        IsSuccessSendToServer   BOOLEAN  DEFAULT False,
                                        CreatedDate             DATETIME DEFAULT (CURRENT_TIMESTAMP),
                                        RowStatus               INTEGER  DEFAULT (0) 
                                    );";
                ExecuteNonQueryScript(createScript);
            }

            if (!CheckIsTableExist("ScreenshotLogModel"))
            {
                createScript = @"CREATE TABLE ScreenshotLogModel 
                                    (
                                        ScreenshotLogId       INTEGER  PRIMARY KEY AUTOINCREMENT,
                                        SessionId             INTEGER,
                                        ActivityName          TEXT,
                                        ActivityType          TEXT,
                                        Image                 TEXT,
                                        CaptureScreenDate     DATETIME,
                                        IsSuccessSendToServer BOOLEAN  DEFAULT False,
                                        CreatedDate           DATETIME DEFAULT (CURRENT_TIMESTAMP),
                                        RowStatus             INTEGER  DEFAULT (0) 
                                    )";
                ExecuteNonQueryScript(createScript);
            }

            if (!CheckIsTableExist("UserTaskModel"))
            {
                createScript = @" CREATE TABLE UserTaskModel 
                                    (
                                        UserTaskModelId INTEGER  PRIMARY KEY AUTOINCREMENT,
                                        SessionId       INTEGER,
                                        TaskId          INTEGER,
                                        TaskStatus      TEXT,
                                        TaskDate        DATETIME,
                                        CreatedDate     DATETIME DEFAULT (CURRENT_TIMESTAMP),
                                        RowStatus       INTEGER  DEFAULT (0) 
                                    )";
                ExecuteNonQueryScript(createScript);
            }
        }

        /// <summary>
        /// Executes the non query script.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private bool ExecuteNonQueryScript(string query)
        {
            bool isSuccess = false;
            using (var connection = PrivateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    DateTime dateTimeResult = new DateTime(1900, 1, 1);
                    var sqlCommand = connection.CreateCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    isSuccess = Convert.ToBoolean(sqlCommand.ExecuteNonQuery());
                    transaction.Commit();
                }
            }
            return isSuccess;
        }

        #endregion
    }
}