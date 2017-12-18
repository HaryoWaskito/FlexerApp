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
        private SQLiteConnection privateConnection()
        {
            return new SQLiteConnection(string.Format("Data Source={0};Version=3;", CONST_DATABASENAME));
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

            insertQuery = string.Format("INSERT INTO KeyboardMouseLogModel ( KeyboardMouseLogModelId,  SessionID, ActivityName, ActivityType, InputKey, KeyStrokeCount, MouseClickCount, StartTime, EndTime, IsSuccessSendToServer ) " +
                                        "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                        item.KeyboardMouseLogModelId, item.SessionID, item.ActivityName, item.ActivityType, item.InputKey, item.KeyStrokeCount, item.MouseClickCount, item.StartTime, item.EndTime, item.IsSuccessSendToServer);

            using (var connection = privateConnection())
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

            using (var connection = privateConnection())
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
            string deleteQuery = "DELETE FROM KeyboardMouseLogModel WHERE IsSuccessSendToServer = 'True'";

            using (var connection = privateConnection())
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
            using (var connection = privateConnection())
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
                            recordData.KeyboardMouseLogModelId = reader["KeyboardMouseLogModelId"] != null ? reader["KeyboardMouseLogModelId"].ToString() : string.Empty;
                            recordData.SessionID = reader["SessionID"] != null ? int.Parse(reader["SessionID"].ToString()) : 0;
                            recordData.ActivityName = reader["ActivityName"] != null ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != null ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.KeyStrokeCount = reader["KeyStrokeCount"] != null ? long.Parse(reader["KeyStrokeCount"].ToString()) : 0;
                            recordData.MouseClickCount = reader["MouseClickCount"] != null ? long.Parse(reader["MouseClickCount"].ToString()) : 0;
                            if (reader["StartTime"] != null && DateTime.TryParseExact(reader["StartTime"].ToString(), "yyyymmdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.StartTime = dateTimeResult;
                            if (reader["EndTime"] != null && DateTime.TryParseExact(reader["EndTime"].ToString(), "yyyymmdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.EndTime = dateTimeResult;
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

            string retrieveQuery = String.Format("SELECT * FROM KeyboardMouseLogModel WHERE IsSuccessSendToServer = '{0}' ", false);

            using (var connection = privateConnection())
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
                            recordData.KeyboardMouseLogModelId = reader["KeyboardMouseLogModelId"] != null ? reader["KeyboardMouseLogModelId"].ToString() : string.Empty;
                            recordData.SessionID = reader["SessionID"] != null ? int.Parse(reader["SessionID"].ToString()) : 0;
                            recordData.ActivityName = reader["ActivityName"] != null ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != null ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.KeyStrokeCount = reader["KeyStrokeCount"] != null ? long.Parse(reader["KeyStrokeCount"].ToString()) : 0;
                            recordData.MouseClickCount = reader["MouseClickCount"] != null ? long.Parse(reader["MouseClickCount"].ToString()) : 0;
                            if (reader["StartTime"] != null && DateTime.TryParseExact(reader["StartTime"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.StartTime = dateTimeResult;
                            if (reader["EndTime"] != null && DateTime.TryParseExact(reader["EndTime"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.EndTime = dateTimeResult;

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
            using (var connection = privateConnection())
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
                            recordData.KeyboardMouseLogModelId = reader["KeyboardMouseLogModelId"] != null ? reader["KeyboardMouseLogModelId"].ToString() : string.Empty;
                            recordData.SessionID = reader["SessionID"] != null ? int.Parse(reader["SessionID"].ToString()) : 0;
                            recordData.ActivityName = reader["ActivityName"] != null ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != null ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.InputKey = reader["InputKey"] != null ? reader["InputKey"].ToString() : string.Empty;
                            recordData.KeyStrokeCount = reader["KeyStrokeCount"] != null ? long.Parse(reader["KeyStrokeCount"].ToString()) : 0;
                            recordData.MouseClickCount = reader["MouseClickCount"] != null ? long.Parse(reader["MouseClickCount"].ToString()) : 0;
                            if (reader["StartTime"] != null && DateTime.TryParseExact(reader["StartTime"].ToString(), "yyyymmdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.StartTime = dateTimeResult;
                            if (reader["EndTime"] != null && DateTime.TryParseExact(reader["EndTime"].ToString(), "yyyymmdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                recordData.EndTime = dateTimeResult;
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

            insertQuery = string.Format("INSERT INTO ScreenshotLogModel ( ScreenshotLogModelId, SessionID, ActivityName, ActivityType, Image, CaptureScreenDate, IsSuccessSendToServer ) " +
                                        "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                                        item.ScreenshotLogModelId, item.SessionID, item.ActivityName, item.ActivityType, Convert.ToBase64String(item.Image), item.CaptureScreenDate, item.IsSuccessSendToServer);

            using (var connection = privateConnection())
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

            using (var connection = privateConnection())
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

            using (var connection = privateConnection())
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
            string retrieveQuery = String.Format("SELECT * FROM ScreenshotLogModel WHERE IsSuccessSendToServer = '{0}'", false);
            using (var connection = privateConnection())
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
                            recordData.ScreenshotLogModelId = reader["ScreenshotLogModelId"] != null ? reader["ScreenshotLogModelId"].ToString() : string.Empty;
                            recordData.SessionID = reader["SessionID"] != null ? int.Parse(reader["SessionID"].ToString()) : 0;
                            recordData.ActivityName = reader["ActivityName"] != null ? reader["ActivityName"].ToString() : string.Empty;
                            recordData.ActivityType = reader["ActivityType"] != null ? reader["ActivityType"].ToString() : string.Empty;
                            recordData.Image = reader["Image"] != null ? Convert.FromBase64String(reader["Image"].ToString()) : new byte[0];
                            if (reader["CaptureScreenDate"] != null &&
                                (DateTime.TryParseExact(reader["CaptureScreenDate"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult) ||
                                  DateTime.TryParseExact(reader["CaptureScreenDate"].ToString(), "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult)))
                                recordData.CaptureScreenDate = dateTimeResult;
                            recordData.IsSuccessSendToServer = reader["IsSuccessSendToServer"] != null ? Convert.ToBoolean(reader["IsSuccessSendToServer"].ToString()) : false;

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

            insertQuery = string.Format("INSERT INTO LoginModel ( LoginModelId, Email, Password, LocationType, IPAddress, City, Lat, Long, sessionID, loginToken, CreatedDate ) " +
                                        "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                                        item.LoginModelId, item.Email, item.Password, item.LocationType, item.IPAddress, item.City, item.Lat, item.Long, item.sessionID, item.loginToken, DateTime.Now.ToString(DATE_FORMAT));

            using (var connection = privateConnection())
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
            using (var connection = privateConnection())
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
                            result.LoginModelId = reader["LoginModelId"] != null ? reader["LoginModelId"].ToString() : string.Empty;
                            result.Email = reader["Email"] != null ? reader["Email"].ToString() : string.Empty;
                            result.Password = reader["Password"] != null ? reader["Password"].ToString() : string.Empty;
                            result.LocationType = reader["LocationType"] != null ? reader["LocationType"].ToString() : string.Empty;
                            result.IPAddress = reader["IPAddress"] != null ? reader["IPAddress"].ToString() : string.Empty;
                            result.City = reader["City"] != null ? reader["City"].ToString() : string.Empty;
                            result.Lat = reader["Lat"] != null ? decimal.Parse(reader["Lat"].ToString()) : 0;
                            result.Long = reader["Long"] != null ? decimal.Parse(reader["Long"].ToString()) : 0;
                            result.sessionID = reader["sessionID"] != null ? int.Parse(reader["sessionID"].ToString()) : 0;
                            result.loginToken = reader["loginToken"] != null ? reader["loginToken"].ToString() : string.Empty;
                            if (reader["CreatedDate"] != null && DateTime.TryParseExact(reader["CreatedDate"].ToString(), DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                                result.CreatedDate = dateTimeResult;
                        }
                    }
                    transaction.Commit();
                }
            }
            return result;
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

            using (var connection = privateConnection())
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
                                  ( LoginModelId TEXT PRIMARY KEY,
                                    Email TEXT,
                                    Password TEXT,
                                    LocationType TEXT,
                                    IPAddress TEXT,
                                    City TEXT,
                                    Lat REAL,
                                    Long REAL,
                                    sessionID INT,
                                    loginToken TEXT,
                                    CreatedDate TEXT
                                  )";
                ExecuteNonQueryScript(createScript);
            }

            if (!CheckIsTableExist("KeyboardMouseLogModel"))
            {
                createScript = @" CREATE TABLE KeyboardMouseLogModel
                                  ( KeyboardMouseLogModelId TEXT PRIMARY KEY,
                                    SessionID BIGINT,
                                    ActivityName TEXT,
                                    ActivityType TEXT,
                                    InputKey TEXT,
                                    KeyStrokeCount BIGINT,
                                    MouseClickCount BIGINT,
                                    StartTime TEXT,
                                    EndTime TEXT,
                                    IsSuccessSendToServer INT
                                  )";

                ExecuteNonQueryScript(createScript);
            }

            if (!CheckIsTableExist("ScreenshotLogModel"))
            {
                createScript = @" CREATE TABLE ScreenshotLogModel
                                  ( ScreenshotLogModelId TEXT PRIMARY KEY,
                                    SessionID INT,
                                    ActivityName TEXT,
                                    ActivityType TEXT,
                                    Image TEXT,
                                    CaptureScreenDate TEXT,
                                    IsSuccessSendToServer TEXT                                    
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
            using (var connection = privateConnection())
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