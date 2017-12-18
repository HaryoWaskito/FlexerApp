using FlexerApp.Controllers;
using FlexerApp.Models;
using System;
using System.Diagnostics;

using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace FlexerApp
{
    public partial class FlexerApp : Form
    {
        #region Drag Form Function

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="Msg">The MSG.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Releases the capture.
        /// </summary>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// Handles the MouseDown event of the FlexerApp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void FlexerApp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexerApp"/> class.
        /// </summary>
        public FlexerApp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the LoginControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LoginControl_Click(object sender, EventArgs e)
        {
            //bgWorker.RunWorkerAsync();

            var controller = new Controller();
            var login = new LoginModel();
            var locator = new Locator();
            var innerClock = new InnerClock();

            login.LoginModelId = Guid.NewGuid().ToString();

            EmailControl.Text = "asd";
            if (EmailControl.Text == "asd")
            {
                login.Email = "hwk@aab.com";
                login.Password = "Password95";
            }
            else
            {
                login.Email = EmailControl.Text;
                login.Password = PasswordControl.Text;
            }

            login.LocationType = "GPS";// locator.LocationType;
            login.IPAddress = string.Empty;// locator.PrivateIP;
            login.City = "Jakarta";// locator.City;
            login.Lat = locator.Latitude;
            login.Long = locator.Longitude;
            login.GMTDiff = locator.GMT;
            login.LoginDate = DateTime.Now;// innerClock.NetworkDate;

            var stopWatch = new Stopwatch();//Create InnerClock
            stopWatch.Start();

            if (new Connector().LoginToServer(login))
            {
                SetAppToSytemTray();
                controller.stopwatch = stopWatch;
                controller.loginTime = login.LoginDate;
                controller.StartMainProcess();
                ProgressCircleControl.Hide();
            }
            else
            {
                ProgressCircleControl.Hide();
                ErrorMessageControl.Text = "Login failed!";
                var t = new Timer
                {
                    Interval = 3000 // it will Tick in 3 seconds
                };

                t.Tick += (s, timeEventArg) =>
                {
                    ErrorMessageControl.Hide();
                    t.Stop();
                };
                t.Start();
            }
        }

        /// <summary>
        /// Handles the Click event of the ExitControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ExitControl_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Sets the application to sytem tray.
        /// </summary>
        private void SetAppToSytemTray()
        {
            var contextMenu = new ContextMenu();
            var menuItemLogout = new MenuItem();
            var menuItemExit = new MenuItem();

            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItemLogout, menuItemExit });
            menuItemLogout.Index = 0;
            menuItemLogout.Text = "Logout";
            menuItemLogout.Click += new System.EventHandler(this.MenuItemLogout_Click);

            menuItemExit.Index = 1;
            menuItemExit.Text = "Exit";
            menuItemExit.Click += new System.EventHandler(this.MenuItemExit_Click);

            notifyIconControl.Visible = true;
            notifyIconControl.BalloonTipIcon = ToolTipIcon.Info;
            notifyIconControl.BalloonTipText = "All set! ready to go!";
            notifyIconControl.BalloonTipTitle = "FlexerApp";
            notifyIconControl.ContextMenu = contextMenu;
            notifyIconControl.ShowBalloonTip(500);
            this.Hide();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemLogout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MenuItemLogout_Click(object sender, EventArgs e)
        {
            notifyIconControl.Visible = false;
            this.Show();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void bgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        //{
        //    ProgressCircleControl.Visible = true;
        //}
    }
}
