namespace EVEMon.ExternalCalendar
{
    partial class ExternalCalendar
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
            this.gbExternalCalendar = new System.Windows.Forms.GroupBox();
            this.tbReminder = new System.Windows.Forms.TextBox();
            this.cbSetReminder = new System.Windows.Forms.CheckBox();
            this.dtpLateReminder = new System.Windows.Forms.DateTimePicker();
            this.lblLateReminder = new System.Windows.Forms.Label();
            this.dtpEarlyReminder = new System.Windows.Forms.DateTimePicker();
            this.lblEarlyReminder = new System.Windows.Forms.Label();
            this.cbUseAlterateReminder = new System.Windows.Forms.CheckBox();
            this.gbGoogle = new System.Windows.Forms.GroupBox();
            this.cbGoogleReminder = new System.Windows.Forms.ComboBox();
            this.labelGoogleReminder = new System.Windows.Forms.Label();
            this.lblURI = new System.Windows.Forms.Label();
            this.tbGoogleURI = new System.Windows.Forms.TextBox();
            this.tbGooglePassword = new System.Windows.Forms.TextBox();
            this.tbGoogleEmail = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblGoogleEmail = new System.Windows.Forms.Label();
            this.rbGoogle = new System.Windows.Forms.RadioButton();
            this.rbMSOutlook = new System.Windows.Forms.RadioButton();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbExternalCalendar.SuspendLayout();
            this.gbGoogle.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbExternalCalendar
            // 
            this.gbExternalCalendar.Controls.Add(this.tbReminder);
            this.gbExternalCalendar.Controls.Add(this.cbSetReminder);
            this.gbExternalCalendar.Controls.Add(this.dtpLateReminder);
            this.gbExternalCalendar.Controls.Add(this.lblLateReminder);
            this.gbExternalCalendar.Controls.Add(this.dtpEarlyReminder);
            this.gbExternalCalendar.Controls.Add(this.lblEarlyReminder);
            this.gbExternalCalendar.Controls.Add(this.cbUseAlterateReminder);
            this.gbExternalCalendar.Controls.Add(this.gbGoogle);
            this.gbExternalCalendar.Controls.Add(this.rbGoogle);
            this.gbExternalCalendar.Controls.Add(this.rbMSOutlook);
            this.gbExternalCalendar.Location = new System.Drawing.Point(12, 12);
            this.gbExternalCalendar.Name = "gbExternalCalendar";
            this.gbExternalCalendar.Size = new System.Drawing.Size(377, 264);
            this.gbExternalCalendar.TabIndex = 3;
            this.gbExternalCalendar.TabStop = false;
            this.gbExternalCalendar.Text = "External Calendar";
            // 
            // tbReminder
            // 
            this.tbReminder.Location = new System.Drawing.Point(96, 185);
            this.tbReminder.Name = "tbReminder";
            this.tbReminder.Size = new System.Drawing.Size(35, 20);
            this.tbReminder.TabIndex = 7;
            this.tbReminder.Text = "5";
            // 
            // cbSetReminder
            // 
            this.cbSetReminder.AutoSize = true;
            this.cbSetReminder.Checked = true;
            this.cbSetReminder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSetReminder.Location = new System.Drawing.Point(6, 187);
            this.cbSetReminder.Name = "cbSetReminder";
            this.cbSetReminder.Size = new System.Drawing.Size(71, 17);
            this.cbSetReminder.TabIndex = 6;
            this.cbSetReminder.Text = "Reminder";
            this.cbSetReminder.UseVisualStyleBackColor = true;
            // 
            // dtpLateReminder
            // 
            this.dtpLateReminder.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpLateReminder.Location = new System.Drawing.Point(276, 235);
            this.dtpLateReminder.Name = "dtpLateReminder";
            this.dtpLateReminder.ShowUpDown = true;
            this.dtpLateReminder.Size = new System.Drawing.Size(72, 20);
            this.dtpLateReminder.TabIndex = 10;
            this.dtpLateReminder.Value = new System.DateTime(2007, 9, 21, 0, 0, 0, 0);
            // 
            // lblLateReminder
            // 
            this.lblLateReminder.AutoSize = true;
            this.lblLateReminder.Location = new System.Drawing.Point(190, 239);
            this.lblLateReminder.Name = "lblLateReminder";
            this.lblLateReminder.Size = new System.Drawing.Size(79, 13);
            this.lblLateReminder.TabIndex = 6;
            this.lblLateReminder.Text = "Late Reminder:";
            // 
            // dtpEarlyReminder
            // 
            this.dtpEarlyReminder.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpEarlyReminder.Location = new System.Drawing.Point(96, 235);
            this.dtpEarlyReminder.Name = "dtpEarlyReminder";
            this.dtpEarlyReminder.ShowUpDown = true;
            this.dtpEarlyReminder.Size = new System.Drawing.Size(70, 20);
            this.dtpEarlyReminder.TabIndex = 9;
            this.dtpEarlyReminder.Value = new System.DateTime(2007, 9, 21, 0, 0, 0, 0);
            // 
            // lblEarlyReminder
            // 
            this.lblEarlyReminder.AutoSize = true;
            this.lblEarlyReminder.Location = new System.Drawing.Point(5, 239);
            this.lblEarlyReminder.Name = "lblEarlyReminder";
            this.lblEarlyReminder.Size = new System.Drawing.Size(81, 13);
            this.lblEarlyReminder.TabIndex = 4;
            this.lblEarlyReminder.Text = "Early Reminder:";
            // 
            // cbUseAlterateReminder
            // 
            this.cbUseAlterateReminder.AutoSize = true;
            this.cbUseAlterateReminder.Checked = true;
            this.cbUseAlterateReminder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseAlterateReminder.Location = new System.Drawing.Point(6, 212);
            this.cbUseAlterateReminder.Name = "cbUseAlterateReminder";
            this.cbUseAlterateReminder.Size = new System.Drawing.Size(132, 17);
            this.cbUseAlterateReminder.TabIndex = 8;
            this.cbUseAlterateReminder.Text = "Use alternate reminder";
            this.cbUseAlterateReminder.UseVisualStyleBackColor = true;
            // 
            // gbGoogle
            // 
            this.gbGoogle.Controls.Add(this.cbGoogleReminder);
            this.gbGoogle.Controls.Add(this.labelGoogleReminder);
            this.gbGoogle.Controls.Add(this.lblURI);
            this.gbGoogle.Controls.Add(this.tbGoogleURI);
            this.gbGoogle.Controls.Add(this.tbGooglePassword);
            this.gbGoogle.Controls.Add(this.tbGoogleEmail);
            this.gbGoogle.Controls.Add(this.lblPassword);
            this.gbGoogle.Controls.Add(this.lblGoogleEmail);
            this.gbGoogle.Enabled = false;
            this.gbGoogle.Location = new System.Drawing.Point(8, 43);
            this.gbGoogle.Name = "gbGoogle";
            this.gbGoogle.Size = new System.Drawing.Size(359, 136);
            this.gbGoogle.TabIndex = 3;
            this.gbGoogle.TabStop = false;
            this.gbGoogle.Text = "Google Information";
            // 
            // cbGoogleReminder
            // 
            this.cbGoogleReminder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGoogleReminder.FormattingEnabled = true;
            this.cbGoogleReminder.Items.AddRange(new object[] {
            "Alert",
            "All",
            "Email",
            "None",
            "SMS",
            "Unspecified"});
            this.cbGoogleReminder.Location = new System.Drawing.Point(83, 103);
            this.cbGoogleReminder.Name = "cbGoogleReminder";
            this.cbGoogleReminder.Size = new System.Drawing.Size(263, 21);
            this.cbGoogleReminder.TabIndex = 7;
            // 
            // labelGoogleReminder
            // 
            this.labelGoogleReminder.AutoSize = true;
            this.labelGoogleReminder.Location = new System.Drawing.Point(6, 106);
            this.labelGoogleReminder.Name = "labelGoogleReminder";
            this.labelGoogleReminder.Size = new System.Drawing.Size(55, 13);
            this.labelGoogleReminder.TabIndex = 6;
            this.labelGoogleReminder.Text = "Reminder:";
            // 
            // lblURI
            // 
            this.lblURI.AutoSize = true;
            this.lblURI.Location = new System.Drawing.Point(6, 80);
            this.lblURI.Name = "lblURI";
            this.lblURI.Size = new System.Drawing.Size(29, 13);
            this.lblURI.TabIndex = 5;
            this.lblURI.Text = "URI:";
            // 
            // tbGoogleURI
            // 
            this.tbGoogleURI.Location = new System.Drawing.Point(83, 76);
            this.tbGoogleURI.Name = "tbGoogleURI";
            this.tbGoogleURI.Size = new System.Drawing.Size(263, 20);
            this.tbGoogleURI.TabIndex = 5;
            this.tbGoogleURI.Text = "http://www.google.com/calendar/feeds/default/private/full";
            // 
            // tbGooglePassword
            // 
            this.tbGooglePassword.Location = new System.Drawing.Point(83, 49);
            this.tbGooglePassword.Name = "tbGooglePassword";
            this.tbGooglePassword.PasswordChar = '*';
            this.tbGooglePassword.Size = new System.Drawing.Size(263, 20);
            this.tbGooglePassword.TabIndex = 4;
            // 
            // tbGoogleEmail
            // 
            this.tbGoogleEmail.Location = new System.Drawing.Point(83, 21);
            this.tbGoogleEmail.Name = "tbGoogleEmail";
            this.tbGoogleEmail.Size = new System.Drawing.Size(263, 20);
            this.tbGoogleEmail.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(6, 52);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "Password:";
            // 
            // lblGoogleEmail
            // 
            this.lblGoogleEmail.AutoSize = true;
            this.lblGoogleEmail.Location = new System.Drawing.Point(6, 24);
            this.lblGoogleEmail.Name = "lblGoogleEmail";
            this.lblGoogleEmail.Size = new System.Drawing.Size(72, 13);
            this.lblGoogleEmail.TabIndex = 0;
            this.lblGoogleEmail.Text = "Google Email:";
            // 
            // rbGoogle
            // 
            this.rbGoogle.AutoSize = true;
            this.rbGoogle.Location = new System.Drawing.Point(91, 19);
            this.rbGoogle.Name = "rbGoogle";
            this.rbGoogle.Size = new System.Drawing.Size(59, 17);
            this.rbGoogle.TabIndex = 2;
            this.rbGoogle.Text = "Google";
            this.rbGoogle.UseVisualStyleBackColor = true;
            this.rbGoogle.Click += new System.EventHandler(this.rbMSOutlook_Click);
            // 
            // rbMSOutlook
            // 
            this.rbMSOutlook.AutoSize = true;
            this.rbMSOutlook.Checked = true;
            this.rbMSOutlook.Location = new System.Drawing.Point(6, 19);
            this.rbMSOutlook.Name = "rbMSOutlook";
            this.rbMSOutlook.Size = new System.Drawing.Size(81, 17);
            this.rbMSOutlook.TabIndex = 1;
            this.rbMSOutlook.TabStop = true;
            this.rbMSOutlook.Text = "MS Outlook";
            this.rbMSOutlook.UseVisualStyleBackColor = true;
            this.rbMSOutlook.Click += new System.EventHandler(this.rbMSOutlook_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(12, 282);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 4;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(93, 282);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ExternalCalendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 310);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.gbExternalCalendar);
            this.Name = "ExternalCalendar";
            this.Text = "ExternalCalendar";
            this.gbExternalCalendar.ResumeLayout(false);
            this.gbExternalCalendar.PerformLayout();
            this.gbGoogle.ResumeLayout(false);
            this.gbGoogle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbExternalCalendar;
        private System.Windows.Forms.TextBox tbReminder;
        private System.Windows.Forms.CheckBox cbSetReminder;
        private System.Windows.Forms.DateTimePicker dtpLateReminder;
        private System.Windows.Forms.Label lblLateReminder;
        private System.Windows.Forms.DateTimePicker dtpEarlyReminder;
        private System.Windows.Forms.Label lblEarlyReminder;
        private System.Windows.Forms.CheckBox cbUseAlterateReminder;
        private System.Windows.Forms.GroupBox gbGoogle;
        private System.Windows.Forms.Label lblURI;
        private System.Windows.Forms.TextBox tbGoogleURI;
        private System.Windows.Forms.TextBox tbGooglePassword;
        private System.Windows.Forms.TextBox tbGoogleEmail;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblGoogleEmail;
        private System.Windows.Forms.RadioButton rbGoogle;
        private System.Windows.Forms.RadioButton rbMSOutlook;
        private System.Windows.Forms.ComboBox cbGoogleReminder;
        private System.Windows.Forms.Label labelGoogleReminder;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
    }
}
