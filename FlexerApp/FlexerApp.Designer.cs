namespace FlexerApp
{
    partial class FlexerApp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlexerApp));
            this.PasswordControl = new System.Windows.Forms.TextBox();
            this.EmailControl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.notifyIconControl = new System.Windows.Forms.NotifyIcon(this.components);
            this.EmailCaptionControl = new System.Windows.Forms.Label();
            this.PasswordCaptionControl = new System.Windows.Forms.Label();
            this.ErrorMessageControl = new System.Windows.Forms.Label();
            this.LoginControl = new System.Windows.Forms.Button();
            this.ExitControl = new System.Windows.Forms.Button();
            this.ProgressCircleControl = new CircularProgressBar.CircularProgressBar();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // PasswordControl
            // 
            this.PasswordControl.Font = new System.Drawing.Font("Arial", 12F);
            this.PasswordControl.Location = new System.Drawing.Point(12, 164);
            this.PasswordControl.Name = "PasswordControl";
            this.PasswordControl.Size = new System.Drawing.Size(294, 26);
            this.PasswordControl.TabIndex = 1;
            this.PasswordControl.UseSystemPasswordChar = true;
            // 
            // EmailControl
            // 
            this.EmailControl.Font = new System.Drawing.Font("Arial", 12F);
            this.EmailControl.Location = new System.Drawing.Point(12, 101);
            this.EmailControl.Name = "EmailControl";
            this.EmailControl.Size = new System.Drawing.Size(294, 26);
            this.EmailControl.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Copperplate Gothic Bold", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.GhostWhite;
            this.label3.Location = new System.Drawing.Point(199, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Flexer App";
            // 
            // notifyIconControl
            // 
            this.notifyIconControl.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconControl.Icon")));
            this.notifyIconControl.Text = "FlexerApp";
            // 
            // EmailCaptionControl
            // 
            this.EmailCaptionControl.AutoSize = true;
            this.EmailCaptionControl.BackColor = System.Drawing.Color.Transparent;
            this.EmailCaptionControl.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailCaptionControl.ForeColor = System.Drawing.Color.Transparent;
            this.EmailCaptionControl.Location = new System.Drawing.Point(14, 80);
            this.EmailCaptionControl.Name = "EmailCaptionControl";
            this.EmailCaptionControl.Size = new System.Drawing.Size(57, 19);
            this.EmailCaptionControl.TabIndex = 7;
            this.EmailCaptionControl.Text = "Email:";
            // 
            // PasswordCaptionControl
            // 
            this.PasswordCaptionControl.AutoSize = true;
            this.PasswordCaptionControl.BackColor = System.Drawing.Color.Transparent;
            this.PasswordCaptionControl.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.PasswordCaptionControl.ForeColor = System.Drawing.Color.SeaShell;
            this.PasswordCaptionControl.Location = new System.Drawing.Point(14, 143);
            this.PasswordCaptionControl.Name = "PasswordCaptionControl";
            this.PasswordCaptionControl.Size = new System.Drawing.Size(92, 19);
            this.PasswordCaptionControl.TabIndex = 8;
            this.PasswordCaptionControl.Text = "Password:";
            // 
            // ErrorMessageControl
            // 
            this.ErrorMessageControl.AutoSize = true;
            this.ErrorMessageControl.ForeColor = System.Drawing.Color.Red;
            this.ErrorMessageControl.Location = new System.Drawing.Point(156, 55);
            this.ErrorMessageControl.Name = "ErrorMessageControl";
            this.ErrorMessageControl.Size = new System.Drawing.Size(0, 13);
            this.ErrorMessageControl.TabIndex = 9;
            // 
            // LoginControl
            // 
            this.LoginControl.BackColor = System.Drawing.Color.Thistle;
            this.LoginControl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.LoginControl.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginControl.Location = new System.Drawing.Point(12, 213);
            this.LoginControl.Name = "LoginControl";
            this.LoginControl.Size = new System.Drawing.Size(110, 45);
            this.LoginControl.TabIndex = 10;
            this.LoginControl.Text = "Login";
            this.LoginControl.UseVisualStyleBackColor = false;
            this.LoginControl.Click += new System.EventHandler(this.LoginControl_Click);
            // 
            // ExitControl
            // 
            this.ExitControl.BackColor = System.Drawing.Color.Thistle;
            this.ExitControl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ExitControl.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitControl.Location = new System.Drawing.Point(184, 213);
            this.ExitControl.Name = "ExitControl";
            this.ExitControl.Size = new System.Drawing.Size(110, 45);
            this.ExitControl.TabIndex = 11;
            this.ExitControl.Text = "Exit";
            this.ExitControl.UseVisualStyleBackColor = false;
            this.ExitControl.Click += new System.EventHandler(this.ExitControl_Click);
            // 
            // ProgressCircleControl
            // 
            this.ProgressCircleControl.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.ProgressCircleControl.AnimationSpeed = 500;
            this.ProgressCircleControl.BackColor = System.Drawing.Color.Transparent;
            this.ProgressCircleControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold);
            this.ProgressCircleControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ProgressCircleControl.InnerColor = System.Drawing.Color.Transparent;
            this.ProgressCircleControl.InnerMargin = 2;
            this.ProgressCircleControl.InnerWidth = -1;
            this.ProgressCircleControl.Location = new System.Drawing.Point(128, 208);
            this.ProgressCircleControl.MarqueeAnimationSpeed = 2000;
            this.ProgressCircleControl.Name = "ProgressCircleControl";
            this.ProgressCircleControl.OuterColor = System.Drawing.Color.Gray;
            this.ProgressCircleControl.OuterMargin = -25;
            this.ProgressCircleControl.OuterWidth = 26;
            this.ProgressCircleControl.ProgressColor = System.Drawing.Color.MediumSlateBlue;
            this.ProgressCircleControl.ProgressWidth = 2;
            this.ProgressCircleControl.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.ProgressCircleControl.Size = new System.Drawing.Size(50, 50);
            this.ProgressCircleControl.StartAngle = 270;
            this.ProgressCircleControl.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgressCircleControl.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.ProgressCircleControl.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.ProgressCircleControl.SubscriptText = ".23";
            this.ProgressCircleControl.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.ProgressCircleControl.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.ProgressCircleControl.SuperscriptText = "";
            this.ProgressCircleControl.TabIndex = 12;
            this.ProgressCircleControl.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.ProgressCircleControl.Value = 68;
            this.ProgressCircleControl.Visible = false;

            // 
            // FlexerApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BackgroundImage = global::FlexerApp.Properties.Resources.Setan_Blue;
            this.ClientSize = new System.Drawing.Size(318, 268);
            this.ControlBox = false;
            this.Controls.Add(this.ProgressCircleControl);
            this.Controls.Add(this.ExitControl);
            this.Controls.Add(this.LoginControl);
            this.Controls.Add(this.ErrorMessageControl);
            this.Controls.Add(this.PasswordCaptionControl);
            this.Controls.Add(this.EmailCaptionControl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EmailControl);
            this.Controls.Add(this.PasswordControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Name = "FlexerApp";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FlexerApp_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox PasswordControl;
        private System.Windows.Forms.TextBox EmailControl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NotifyIcon notifyIconControl;
        private System.Windows.Forms.Label EmailCaptionControl;
        private System.Windows.Forms.Label PasswordCaptionControl;
        private System.Windows.Forms.Label ErrorMessageControl;
        private System.Windows.Forms.Button LoginControl;
        private System.Windows.Forms.Button ExitControl;
        private CircularProgressBar.CircularProgressBar ProgressCircleControl;
        private System.ComponentModel.BackgroundWorker bgWorker;
    }
}

