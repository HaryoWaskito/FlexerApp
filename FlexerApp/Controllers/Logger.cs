using FlexerApp.Contexts;
using FlexerApp.Models;
using Gma.System.MouseKeyHook;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace FlexerApp.Controllers
{
    public class Logger
    {
        private const string API_URL = "http://35.186.145.215:2345";
        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const string ACTIVITY_TYPE_APPLICATION = "Application";
        private const string ACTIVITY_TYPE_URL = "URL";

        public Stopwatch stopwatch;
        public DateTime loginTime;

        private DateTime innerTime ;
               
        private int sessionId;
        private string loginToken;

        /// <summary>
        /// The timer
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// The context database
        /// </summary>
        private Context _contextDB = new Context();

        /// <summary>
        /// The m events
        /// </summary>
        private IKeyboardMouseEvents m_Events;

        /// <summary>
        /// The keyboard mouse log list
        /// </summary>
        private List<KeyboardMouseLogModel> keyboardMouseLogList = new List<KeyboardMouseLogModel>();

        /// <summary>
        /// Begins the watching.
        /// </summary>
        public void BeginWatching()
        {
            innerTime = loginTime.AddTicks(stopwatch.ElapsedTicks);

            GetSession();

            Thread oThread = new Thread(new ThreadStart(HookManager_Screenshot_Timer));

            oThread.Start();

            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        /// <summary>
        /// Subscribes the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.KeyPress += HookManager_KeyPress;
            m_Events.MouseClick += HookManager_MouseClick;
        }

        /// <summary>
        /// Unsubscribes this instance.
        /// </summary>
        private void Unsubscribe()
        {
            if (m_Events == null) return;

            m_Events.KeyPress -= HookManager_KeyPress;
            m_Events.MouseClick -= HookManager_MouseClick;
            m_Events.Dispose();
            m_Events = null;
        }

        /// <summary>
        /// Handles the KeyPress event of the HookManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Int32 hwnd = 0;
                hwnd = GetForegroundWindow();

                if (hwnd == 0)
                    return;

                string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

                if (keyboardMouseLogList.Exists(monitor => monitor.ActivityName == appProcessName))
                {
                    var monitor = keyboardMouseLogList.Where(a => a.ActivityName == appProcessName).OrderByDescending(b => b.StartTime).FirstOrDefault();
                    monitor.KeyStrokeCount++;
                    monitor.InputKey = string.Concat(monitor.InputKey, e.KeyChar.ToString());
                    monitor.EndTime = innerTime;
                }
                else
                {
                    if (keyboardMouseLogList.Count > 0)
                    {
                        AddActivity(keyboardMouseLogList, sessionId, loginToken);
                        keyboardMouseLogList = new List<KeyboardMouseLogModel>();
                    }

                    var monitor = new KeyboardMouseLogModel();
                    monitor.KeyboardMouseLogModelId = Guid.NewGuid().ToString();
                    monitor.SessionID = sessionId;
                    monitor.ActivityName = appProcessName;
                    monitor.ActivityType = ACTIVITY_TYPE_APPLICATION;
                    monitor.InputKey = e.KeyChar.ToString();
                    monitor.KeyStrokeCount = 1;
                    monitor.MouseClickCount = 0;
                    monitor.StartTime = innerTime;
                    monitor.EndTime = innerTime;
                    monitor.IsSuccessSendToServer = false;

                    keyboardMouseLogList.Add(monitor);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Handles the MouseClick event of the HookManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void HookManager_MouseClick(object sender, MouseEventArgs e)
        {
            Int32 hwnd = 0;
            hwnd = GetForegroundWindow();

            if (hwnd == 0)
                return;

            string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

            if (keyboardMouseLogList.Exists(monitor => monitor.ActivityName == appProcessName))
            {
                var monitor = keyboardMouseLogList.Where(a => a.ActivityName == appProcessName).OrderByDescending(b => b.StartTime).FirstOrDefault();
                monitor.MouseClickCount++;
                monitor.EndTime = innerTime;
            }
            else
            {
                if (keyboardMouseLogList.Count > 0)
                {
                    AddActivity(keyboardMouseLogList, sessionId, loginToken);
                    keyboardMouseLogList = new List<KeyboardMouseLogModel>();
                }

                var monitor = new KeyboardMouseLogModel();
                monitor.KeyboardMouseLogModelId = Guid.NewGuid().ToString();
                monitor.ActivityName = appProcessName;
                monitor.ActivityType = ACTIVITY_TYPE_APPLICATION;
                monitor.KeyStrokeCount = 0;
                monitor.MouseClickCount = 1;
                monitor.StartTime = innerTime;
                monitor.EndTime = innerTime;
                monitor.IsSuccessSendToServer = false;

                keyboardMouseLogList.Add(monitor);
            }
        }

        /// <summary>
        /// Hooks the manager screenshot timer.
        /// </summary>
        private void HookManager_Screenshot_Timer()
        {
            _timer = new System.Timers.Timer(10000);
            _timer.Elapsed += new ElapsedEventHandler(HookManager_ScreenShot_TimerElapsed);
            _timer.Enabled = true;
        }

        /// <summary>
        /// Handles the TimerElapsed event of the HookManager_ScreenShot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void HookManager_ScreenShot_TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            HookManager_Screenshot();
        }

        /// <summary>
        /// Hooks the manager screenshot.
        /// </summary>
        private void HookManager_Screenshot()
        {
            Int32 hwnd = 0;
            hwnd = GetForegroundWindow();

            if (hwnd == 0)
                return;

            string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

            var capture = new ScreenshotLogModel();
            capture.ScreenshotLogModelId = Guid.NewGuid().ToString();
            capture.SessionID = sessionId;
            capture.ActivityName = appProcessName;
            capture.ActivityType = "Application"; //Sementara dipantek!
            capture.Image = new ScreenCapture().CaptureScreenByteArrayString(System.Drawing.Imaging.ImageFormat.Jpeg);
            capture.CaptureScreenDate = innerTime;
            capture.IsSuccessSendToServer = false;

            _contextDB.CreateImageData(capture);

            if (IsServerAlive())
            {
                var imageLogList = _contextDB.GetScreenshotLogListSendToServer();
                SendLocalImageDataToServer(imageLogList);
            }
        }

        /// <summary>
        /// Gets the foreground window.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        /// <summary>
        /// Gets the window thread process identifier.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="lpdwProcessId">The LPDW process identifier.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);

        /// <summary>
        /// Gets the window process identifier.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <returns></returns>
        private static Int32 GetWindowProcessID(Int32 hwnd)
        {
            Int32 pid = 1;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        private void GetSession()
        {
            var sessionTemp = _contextDB.GetLoginSession();
            sessionId = sessionTemp.sessionID;
            loginToken = sessionTemp.loginToken;
        }

        /// <summary>
        /// Adds the activity.
        /// </summary>
        /// <param name="logList">The log list.</param>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="loginToken">The login token.</param>
        /// <returns></returns>
        private bool AddActivity(List<KeyboardMouseLogModel> logList, long sessionID, string loginToken)
        {
            bool isSuccessSend = false;

            isSuccessSend = InsertIntoLocalDatabase(logList, sessionID, loginToken);

            if (IsServerAlive())
                isSuccessSend = SendLocalDatabaseToServer();

            return isSuccessSend;
        }

        /// <summary>
        /// Inserts the into local database.
        /// </summary>
        /// <param name="logList">The log list.</param>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="loginToken">The login token.</param>
        /// <returns></returns>
        private bool InsertIntoLocalDatabase(List<KeyboardMouseLogModel> logList, long sessionID, string loginToken)
        {
            bool isSuccessSave = false;

            foreach (var log in logList)
                _contextDB.CreateData(log);

            return isSuccessSave;
        }

        /// <summary>
        /// Sends the local database to server.
        /// </summary>
        /// <returns></returns>
        private bool SendLocalDatabaseToServer()
        {
            bool isSuccessSend = false;

            var logList = _contextDB.GetLogListSendToServer();

            isSuccessSend = SendToServer(logList, sessionId, loginToken);

            return isSuccessSend;
        }

        /// <summary>
        /// Determines whether [is server alive].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is server alive]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsServerAlive()
        {
            var ping = new System.Net.NetworkInformation.Ping();
            var ipAddress = API_URL.Split(':')[1].Substring(2);
            var result = ping.Send(ipAddress);

            return result.Status == System.Net.NetworkInformation.IPStatus.Success;
        }

        /// <summary>
        /// Sends to server.
        /// </summary>
        /// <param name="logList">The log list.</param>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="loginToken">The login token.</param>
        /// <returns></returns>
        private bool SendToServer(List<KeyboardMouseLogModel> logList, long sessionID, string loginToken)
        {
            if (logList.Count == 0)
                return false;

            bool isSuccessSend = false;

            try
            {
                var requestBody = new StringBuilder();
                var client = new RestClient(string.Format("{0}/addActivity", API_URL));
                var request = new RestRequest(Method.POST);

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", string.Format("Bearer {0}", loginToken));

                requestBody.AppendFormat("\"clientDate\" : \"{0}\",", innerTime.ToString(DATE_TIME_FORMAT));
                requestBody.AppendFormat("\"sessionID\" : {0},", sessionID);
                requestBody.AppendFormat("\"transactions\" : ");
                requestBody.AppendFormat("[");
                long rowNo = 1;
                foreach (var detailAct in logList)
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
                    requestBody.AppendFormat("{0}", rowNo < logList.Count ? "," : string.Empty);
                    rowNo++;
                }
                requestBody.AppendFormat("]");

                request.AddParameter("application/json", string.Concat("{", requestBody.ToString(), "}"), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                isSuccessSend = response.StatusCode.ToString() == "OK" ? true : false;

                if (isSuccessSend)
                {
                    foreach (var log in logList)
                    {
                        log.IsSuccessSendToServer = true;
                        _contextDB.UpdateData(log);
                    }

                    _contextDB.DeleteDataSuccessSendToServer();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccessSend;
        }

        /// <summary>
        /// Sends the local image data to server.
        /// </summary>
        /// <param name="imageLogList">The image log list.</param>
        private void SendLocalImageDataToServer(List<ScreenshotLogModel> imageLogList)
        {
            if (imageLogList.Count == 0)
                return;

            StringBuilder requestBody;

            RestClient client = null;
            RestRequest request = null;

            foreach (var item in imageLogList)
            {
                client = new RestClient(string.Format("{0}/addActivity/screenshot", API_URL));
                request = new RestRequest(Method.POST);

                requestBody = new StringBuilder();

                request.RequestFormat = DataFormat.Json;

                request.AddHeader("authorization", string.Format("Bearer {0}", loginToken));
                request.AddHeader("content-type", "multipart/form-data");

                request.AddFileBytes("img", item.Image, string.Concat(item.CaptureScreenDate.ToString(DATE_TIME_FORMAT), "jpeg"), "image/jpeg");

                request.AddParameter("sessionID", sessionId);
                request.AddParameter("screenshotDate", item.CaptureScreenDate.ToString(DATE_TIME_FORMAT));
                request.AddParameter("activityName", item.ActivityName);
                request.AddParameter("activityType", item.ActivityType);

                //string filePath = "tempFile.jpg";
                //File.WriteAllBytes(filePath, item.Image.ToArray());
                //string parameter = string.Format("Content-Disposition: form-data; name=\"img\"; filename =\"{0}\"; Content-Type: image/jpeg; Content-Disposition: form-data; name=\"sessionID\":{1}; Content-Disposition: form-data; name=\"screenshotDate\"\r\n\r\n{2}\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"activityName\"\r\n\r\n{3}\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"activityType\"\r\n\r\n{4}\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", filePath, sessionId, item.CaptureScreenDate.ToString(DATE_TIME_FORMAT), item.ActivityName, item.ActivityType);
                //request.AddParameter("multipart/form-data", parameter, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                var content = response.Content;
                //if (File.Exists(filePath))
                //    File.Delete(filePath);

                if (response.StatusCode.ToString() == "OK")
                {
                    item.IsSuccessSendToServer = true;
                    _contextDB.UpdateImageData(item);
                }
            }
        }

        /// <summary>
        /// Sends the image lis to server.
        /// </summary>
        /// <param name="imageList">The image list.</param>
        private void SendImageLisToServer(List<ScreenshotLogModel> imageList)
        {
            //    using (var client = new HttpClient())
            //    {
            //        using (var content = new MultipartFormDataContent())
            //        {
            //            var values = new[]
            //            {
            //                new KeyValuePair<string, string>("Foo", "Bar"),
            //                new KeyValuePair<string, string>("More", "Less"),
            //};

            //            foreach (var keyValuePair in values)
            //            {
            //                content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
            //            }

            //            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(fileName));
            //            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //            {
            //                FileName = "Foo.txt"
            //            };
            //            content.Add(fileContent);

            //            var requestUri = "/api/action";
            //            var result = client.PostAsync(requestUri, content).Result;
            //        }
            //    }
        }
    }
}
