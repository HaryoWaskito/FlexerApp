using FlexerApp.Contexts;
using FlexerApp.Models;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace FlexerApp.Controllers
{
    /// <summary>
    /// Connector Class to connect and processing data to API Server 
    /// </summary>
    public class Connector
    {
        private const string API_URL = "http://flexerapi.southeastasia.cloudapp.azure.com";
        private const int API_PORT = 2345;
        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// The inner time
        /// </summary>
        private DateTime innerTime = DateTime.Today;

        /// <summary>
        /// The context database
        /// </summary>
        private Context _contextDB = new Context();

        /// <summary>
        /// Sends the key log data to server.
        /// </summary>
        public void SendKeyLogDataToServer()
        {
            var loginToken = _contextDB.GetLoginSession().LoginToken;

            var logList = _contextDB.GetLogListSendToServer();

            var sessionList = logList.Select(x => x.SessionId).Distinct();
            //WriteText("SessionList", string.Format("{0} -> {1}", sessionList.Count().ToString(), sessionList.ToArray().ToString()));
            try
            {
                foreach (var sessionID in sessionList)
                {
                    var requestBody = new StringBuilder();
                    var client = new RestClient(string.Format("{0}:{1}/addActivity", API_URL, API_PORT));
                    var request = new RestRequest(Method.POST);

                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Authorization", string.Format("Bearer {0}", loginToken));

                    requestBody.AppendFormat("\"clientDate\" : \"{0}\",", innerTime.ToString(DATE_TIME_FORMAT));
                    requestBody.AppendFormat("\"sessionID\" : {0},", sessionID);
                    requestBody.AppendFormat("\"transactions\" : ");
                    requestBody.AppendFormat("[");

                    long rowNo = 1;
                    //WriteText("logList", string.Format("{0} -> {1}", logList.Count().ToString(), logList.Select(x => x.KeyboardMouseLogModelId).ToString()));
                    var simpleList = logList.Distinct().Where(x => x.SessionId == sessionID);
                    foreach (var detailAct in simpleList)
                    {
                        requestBody.Append("{");
                        requestBody.AppendFormat("\"transactionID\" : \"{0}\",", detailAct.KeyboardMouseLogModelId);
                        requestBody.AppendFormat("\"activityName\" : \"{0}\",", detailAct.ActivityName);
                        requestBody.AppendFormat("\"activityType\" : \"{0}\",", detailAct.ActivityType);
                        requestBody.AppendFormat("\"mouseClick\" : {0},", detailAct.MouseClickCount);
                        requestBody.AppendFormat("\"keystroke\" : {0},", detailAct.KeyStrokeCount);
                        requestBody.AppendFormat("\"startDate\" : \"{0}\",", detailAct.StartTime.ToString(DATE_TIME_FORMAT));
                        requestBody.AppendFormat("\"endDate\" : \"{0}\"", detailAct.EndTime.ToString(DATE_TIME_FORMAT));
                        requestBody.Append("}");
                        requestBody.AppendFormat("{0}", rowNo < simpleList.Count() ? "," : string.Empty);
                        rowNo++;
                    }
                    requestBody.AppendFormat("]");

                    //WriteText("RequestBody", requestBody.ToString());

                    request.AddParameter("application/json", string.Concat("{", requestBody.ToString(), "}"), ParameterType.RequestBody);

                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode.ToString() == "OK")
                    {
                        foreach (var log in logList.Where(x => x.SessionId == sessionID))
                        {
                            log.IsSuccessSendToServer = true;
                            _contextDB.UpdateData(log);
                        }
                    }
                }
                _contextDB.DeleteDataSuccessSendToServer();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sends the local image data to server.
        /// </summary>
        public void SendLocalImageDataToServer()
        {
            var imageLogList = _contextDB.GetScreenshotLogListSendToServer();

            var loginToken = _contextDB.GetLoginSession().LoginToken;

            var sessionList = imageLogList.Select(x => x.SessionId).Distinct();

            foreach (var sessionID in sessionList)
            {
                StringBuilder requestBody;

                RestClient client = null;
                RestRequest request = null;

                var simpleList = imageLogList.Distinct().Where(x => x.SessionId == sessionID);

                foreach (var item in simpleList)
                {
                    client = new RestClient(string.Format("{0}:{1}/addActivity/screenshot", API_URL, API_PORT));
                    request = new RestRequest(Method.POST);

                    requestBody = new StringBuilder();

                    request.RequestFormat = DataFormat.Json;

                    request.AddHeader("authorization", string.Format("Bearer {0}", loginToken));
                    request.AddHeader("content-type", "multipart/form-data");

                    request.AddFileBytes("img", item.Image, string.Concat(item.CaptureScreenDate.ToString(DATE_TIME_FORMAT), "jpeg"), "image/jpeg");

                    request.AddParameter("sessionID", sessionID);
                    request.AddParameter("screenshotDate", item.CaptureScreenDate.ToString(DATE_TIME_FORMAT));
                    request.AddParameter("activityName", item.ActivityName);
                    request.AddParameter("activityType", item.ActivityType);

                    IRestResponse response = client.Execute(request);
                    var content = response.Content;

                    if (response.StatusCode.ToString() == "OK")
                    {
                        item.IsSuccessSendToServer = true;
                        _contextDB.UpdateImageData(item);
                    }
                }
            }
            _contextDB.DeleteImageSuccessSendToServer();
        }

        /// <summary>
        /// Logins to server.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public bool LoginToServer(LoginModel login)
        {
            bool isSuccessLogin = false;
            try
            {
                var requestBody = new StringBuilder();
                var client = new RestClient(string.Format("{0}:{1}/login", API_URL, API_PORT));
                var request = new RestRequest(Method.POST);

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                requestBody.AppendFormat("\"Email\" : \"{0}\",", login.Email);
                requestBody.AppendFormat("\"Password\" : \"{0}\",", login.Password);
                requestBody.AppendFormat("\"LocationType\" : \"{0}\",", login.LocationType);
                requestBody.AppendFormat("\"IPAddress\" : \"{0}\",", login.IPAddress);
                requestBody.AppendFormat("\"City\" : \"{0}\",", login.City);
                requestBody.AppendFormat("\"Lat\" : {0},", login.Lat.ToString().Replace(",", "."));
                requestBody.AppendFormat("\"Long\" : {0},", login.Long.ToString().Replace(",", "."));
                requestBody.AppendFormat("\"gmtDiff\" : {0},", login.GMTDiff.ToString().Replace(",", "."));
                requestBody.AppendFormat("\"clientTime\" : \"{0}\"", login.LoginDate.ToString("yyyy-MM-dd HH:mm:ss"));

                request.AddParameter("application/json", string.Concat("{", requestBody.ToString(), "}"), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                isSuccessLogin = response.StatusCode.ToString() == "OK" ? true : false;

                if (isSuccessLogin)
                {
                    var responseList = response.Content.Split(',').ToList();
                    login.SessionId = Convert.ToInt32(responseList[1].Split(':')[1]);
                    login.LoginToken = responseList[3].Split(':')[1].Substring(1, responseList[3].Split(':')[1].Length - 3);

                    _contextDB.CreateLoginSession(login);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccessLogin;
        }

        /// <summary>
        /// Logouts from server.
        /// </summary>
        public void LogoutFromServer()
        {
            var sessionID = _contextDB.GetLoginSession().SessionId;
            bool isSuccessLogout = false;
            try
            {
                var requestBody = new StringBuilder();
                var client = new RestClient(string.Format("{0}:{1}/logout", API_URL, API_PORT));
                var request = new RestRequest(Method.POST);

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                requestBody.AppendFormat("\"SessionID\" : \"{0}\",", sessionID);

                request.AddParameter("application/json", string.Concat("{", requestBody.ToString(), "}"), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                isSuccessLogout = response.StatusCode.ToString() == "OK" ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the task from server.
        /// </summary>
        public void GetTaskFromServer()
        {
            
        }

        /// <summary>
        /// Proposes the task to server.
        /// </summary>
        public void ProposeTaskToServer()
        {

        }

        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="resultText">The result text.</param>
        public static void WriteText(string fileName, string resultText)
        {
            string path = string.Format(@"C:\Users\Haryo Waskito\Desktop\{0}.txt", fileName);
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(resultText);
                }
            }

            else
                // This text is always added, making the file longer over time
                // if it is not deleted.
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(resultText);
                }
        }
    }
}
