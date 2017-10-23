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
            this.LoginControl = new System.Windows.Forms.Button();
            this.ExitControl = new System.Windows.Forms.Button();
            this.PasswordControl = new System.Windows.Forms.TextBox();
            this.EmailControl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.notifyIconControl = new System.Windows.Forms.NotifyIcon(this.components);
            this.EmailCaptionControl = new System.Windows.Forms.Label();
            this.PasswordCaptionControl = new System.Windows.Forms.Label();
            this.ErrorMessageControl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LoginControl
            // 
            this.LoginControl.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.LoginControl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.LoginControl.Location = new System.Drawing.Point(12, 216);
            this.LoginControl.Name = "LoginControl";
            this.LoginControl.Size = new System.Drawing.Size(130, 40);
            this.LoginControl.TabIndex = 2;
            this.LoginControl.Text = "Login";
            this.LoginControl.UseVisualStyleBackColor = true;
            this.LoginControl.Click += new System.EventHandler(this.LoginControl_Click);
            // 
            // ExitControl
            // 
            this.ExitControl.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.ExitControl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.ExitControl.Location = new System.Drawing.Point(176, 216);
            this.ExitControl.Name = "ExitControl";
            this.ExitControl.Size = new System.Drawing.Size(130, 40);
            this.ExitControl.TabIndex = 3;
            this.ExitControl.Text = "Exit";
            this.ExitControl.UseVisualStyleBackColor = true;
            this.ExitControl.Click += new System.EventHandler(this.ExitControl_Click);
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
            this.EmailCaptionControl.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailCaptionControl.ForeColor = System.Drawing.Color.SeaShell;
            this.EmailCaptionControl.Location = new System.Drawing.Point(14, 80);
            this.EmailCaptionControl.Name = "EmailCaptionControl";
            this.EmailCaptionControl.Size = new System.Drawing.Size(57, 19);
            this.EmailCaptionControl.TabIndex = 7;
            this.EmailCaptionControl.Text = "Email:";
            // 
            // PasswordCaptionControl
            // 
            this.PasswordCaptionControl.AutoSize = true;
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
            // FlexerApp
            // 
            this.AcceptButton = this.LoginControl;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Navy;
            this.ClientSize = new System.Drawing.Size(318, 268);
            this.ControlBox = false;
            this.Controls.Add(this.ErrorMessageControl);
            this.Controls.Add(this.PasswordCaptionControl);
            this.Controls.Add(this.EmailCaptionControl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EmailControl);
            this.Controls.Add(this.PasswordControl);
            this.Controls.Add(this.ExitControl);
            this.Controls.Add(this.LoginControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Name = "FlexerApp";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FlexerApp_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoginControl;
        private System.Windows.Forms.Button ExitControl;
        private System.Windows.Forms.TextBox PasswordControl;
        private System.Windows.Forms.TextBox EmailControl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NotifyIcon notifyIconControl;
        private System.Windows.Forms.Label EmailCaptionControl;
        private System.Windows.Forms.Label PasswordCaptionControl;
        private System.Windows.Forms.Label ErrorMessageControl;
    }
}

