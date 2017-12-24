using FlexerApp.Controllers;
using FlexerApp.Models;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FlexerApp
{
    public partial class FlexerApp : Form
    {
        #region Rounded Windows Form

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
             int nLeftRect,     // x-coordinate of upper-left corner
             int nTopRect,      // y-coordinate of upper-left corner
             int nRightRect,    // x-coordinate of lower-right corner
             int nBottomRect,   // y-coordinate of lower-right corner
             int nWidthEllipse, // height of ellipse
             int nHeightEllipse // width of ellipse
        );

        #endregion

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
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            EmailControl.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, EmailControl.Width, EmailControl.Height, 20, 20));
            PasswordControl.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, PasswordControl.Width, PasswordControl.Height, 20, 20));
            LoginControl.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, LoginControl.Width, LoginControl.Height, 20, 20));
            ExitControl.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, ExitControl.Width, ExitControl.Height, 20, 20));
        }

        /// <summary>
        /// Handles the Click event of the LoginControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void LoginControl_Click(object sender, EventArgs e)
        {
            ProgressCircleControl.Show();
            await System.Threading.Tasks.Task.Run(() => ProcessLogin());
            ProgressCircleControl.Hide();
        }

        /// <summary>
        /// Processes the login.
        /// </summary>
        private void ProcessLogin()
        {
            var controller = new Controller();
            var login = new LoginModel();
            var locator = new Locator();
            //var innerClock = new InnerClock();

            //EmailControl.Text = "asd";
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
            }
            else
            {
                ShowErrorMessage("Login failed!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">The text.</param>
        delegate void StringArgReturningVoidDelegate(string text);

        /// <summary>
        /// Shows the error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void ShowErrorMessage(string errorMessage)
        {            
            if (ErrorMessageControl.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(ShowErrorMessage);
                this.Invoke(d, new object[] { errorMessage });
            }
            else
            {
                ErrorMessageControl.Text = "Login failed!";
            }
                        
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
    }    
}
