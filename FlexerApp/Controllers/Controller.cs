﻿using FlexerApp.Contexts;
using FlexerApp.Models;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FlexerApp.Controllers
{
    public class Controller
    {
        private const string ACTIVITY_TYPE_APPLICATION = "Application";
        private const string ACTIVITY_TYPE_URL = "URL";

        private const int CaptureImageInterval = 10000;
        private const int UploadDataInterval = 10000;

        public Stopwatch stopwatch;
        public DateTime loginTime;

        static readonly object captureImageLock = new object();
        static readonly object uploadDataLock = new object();

        private Context _contextDB = new Context();
        private IKeyboardMouseEvents m_Events;
        private List<KeyboardMouseLogModel> keyboardMouseLogList = new List<KeyboardMouseLogModel>();

        /// <summary>
        /// Begins the watching.
        /// </summary>
        public void StartMainProcess()
        {
            //new Thread(new ThreadStart(CaptureImage)).Start();
            new Thread(new ThreadStart(UploadData)).Start();

            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        /// <summary>
        /// Captures the image.
        /// </summary>
        private void CaptureImage()
        {
            while (true)
            {
                lock (captureImageLock)
                {
                    HookManager_Screenshot();
                    Thread.Sleep(CaptureImageInterval);
                }
            }
        }

        /// <summary>
        /// Sends the data to server timer.
        /// </summary>
        private void UploadData()
        {
            var connector = new Connector();

            while (true)
            {
                lock (uploadDataLock)
                {
                    connector.SendKeyLogDataToServer();
                    connector.SendLocalImageDataToServer();

                    Thread.Sleep(UploadDataInterval);
                }
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

            var sessionID = _contextDB.GetLoginSession().SessionId;

            if (hwnd == 0)
                return;

            string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

            var capture = new ScreenshotLogModel
            {
                SessionId = sessionID,
                ActivityName = appProcessName,
                ActivityType = "Application", //Sementara dipantek!
                Image = new ScreenCapture().CaptureScreenByteArrayString(System.Drawing.Imaging.ImageFormat.Jpeg),
                CaptureScreenDate = loginTime.AddTicks(stopwatch.ElapsedTicks)
            };

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

                var sessionID = _contextDB.GetLoginSession().SessionId;

                if (hwnd == 0)
                    return;

                string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

                if (keyboardMouseLogList.Exists(monitor => monitor.ActivityName == appProcessName))
                {
                    var monitor = keyboardMouseLogList.Where(a => a.ActivityName == appProcessName).OrderByDescending(b => b.StartTime).FirstOrDefault();
                    monitor.KeyStrokeCount++;
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

                    var monitor = new KeyboardMouseLogModel
                    {
                        SessionId = sessionID,
                        ActivityName = appProcessName,
                        ActivityType = ACTIVITY_TYPE_APPLICATION,
                        KeyStrokeCount = 1,
                        MouseClickCount = 0,
                        StartTime = loginTime.AddTicks(stopwatch.ElapsedTicks),
                        EndTime = loginTime.AddTicks(stopwatch.ElapsedTicks),
                        IsSuccessSendToServer = false
                    };

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

            var sessionID = _contextDB.GetLoginSession().SessionId;

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

                var monitor = new KeyboardMouseLogModel
                {
                    SessionId = sessionID,
                    ActivityName = appProcessName,
                    ActivityType = ACTIVITY_TYPE_APPLICATION,
                    KeyStrokeCount = 0,
                    MouseClickCount = 1,
                    StartTime = loginTime.AddTicks(stopwatch.ElapsedTicks),
                    EndTime = loginTime.AddTicks(stopwatch.ElapsedTicks),
                    IsSuccessSendToServer = false
                };

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
