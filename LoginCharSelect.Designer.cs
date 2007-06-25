namespace EVEMon
{
    partial class LoginCharSelect
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
            this.btnCharSelect = new System.Windows.Forms.Button();
            this.tbCharName = new System.Windows.Forms.TextBox();
            this.tbAuthKey = new System.Windows.Forms.TextBox();
            this.tbUserId = new System.Windows.Forms.TextBox();
            this.lbLoginCharacter = new System.Windows.Forms.Label();
            this.lbAuthKey = new System.Windows.Forms.Label();
            this.lbUserId = new System.Windows.Forms.Label();
            this.enterLoginLabel = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbSpecifyXML = new System.Windows.Forms.Label();
            this.tbFilename = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lbAddCharacter = new System.Windows.Forms.Label();
            this.cbCharacterType = new System.Windows.Forms.ComboBox();
            this.gbEveLogin = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAPILink = new System.Windows.Forms.Label();
            this.lnkAPI = new System.Windows.Forms.LinkLabel();
            this.gbSavedXML = new System.Windows.Forms.GroupBox();
            this.tbFileCharName = new System.Windows.Forms.TextBox();
            this.lbSavedCharacter = new System.Windows.Forms.Label();
            this.cbMonitorFile = new System.Windows.Forms.CheckBox();
            this.ofdOpenXml = new System.Windows.Forms.OpenFileDialog();
            this.gbEveLogin.SuspendLayout();
            this.gbSavedXML.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCharSelect
            // 
            this.btnCharSelect.Location = new System.Drawing.Point(177, 131);
            this.btnCharSelect.Name = "btnCharSelect";
            this.btnCharSelect.Size = new System.Drawing.Size(30, 23);
            this.btnCharSelect.TabIndex = 2;
            this.btnCharSelect.Text = "...";
            this.btnCharSelect.UseVisualStyleBackColor = true;
            this.btnCharSelect.Click += new System.EventHandler(this.btnCharSelect_Click);
            // 
            // tbCharName
            // 
            this.tbCharName.Location = new System.Drawing.Point(71, 132);
            this.tbCharName.Name = "tbCharName";
            this.tbCharName.ReadOnly = true;
            this.tbCharName.Size = new System.Drawing.Size(100, 21);
            this.tbCharName.TabIndex = 3;
            this.tbCharName.TabStop = false;
            this.tbCharName.Text = "(None)";
            // 
            // tbAuthKey
            // 
            this.tbAuthKey.Location = new System.Drawing.Point(61, 100);
            this.tbAuthKey.Name = "tbAuthKey";
            this.tbAuthKey.Size = new System.Drawing.Size(328, 21);
            this.tbAuthKey.TabIndex = 1;
            this.tbAuthKey.TextChanged += new System.EventHandler(this.tbApiKey_TextChanged);
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(59, 76);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.Size = new System.Drawing.Size(100, 21);
            this.tbUserId.TabIndex = 0;
            this.tbUserId.TextChanged += new System.EventHandler(this.tbUsername_TextChanged);
            // 
            // lbLoginCharacter
            // 
            this.lbLoginCharacter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLoginCharacter.AutoSize = true;
            this.lbLoginCharacter.Location = new System.Drawing.Point(7, 135);
            this.lbLoginCharacter.Name = "lbLoginCharacter";
            this.lbLoginCharacter.Size = new System.Drawing.Size(59, 13);
            this.lbLoginCharacter.TabIndex = 2;
            this.lbLoginCharacter.Text = "Character:";
            this.lbLoginCharacter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbAuthKey
            // 
            this.lbAuthKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAuthKey.AutoSize = true;
            this.lbAuthKey.Location = new System.Drawing.Point(7, 103);
            this.lbAuthKey.Name = "lbAuthKey";
            this.lbAuthKey.Size = new System.Drawing.Size(49, 13);
            this.lbAuthKey.TabIndex = 1;
            this.lbAuthKey.Text = "API Key:";
            this.lbAuthKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbUserId
            // 
            this.lbUserId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbUserId.AutoSize = true;
            this.lbUserId.Location = new System.Drawing.Point(7, 79);
            this.lbUserId.Name = "lbUserId";
            this.lbUserId.Size = new System.Drawing.Size(47, 13);
            this.lbUserId.TabIndex = 0;
            this.lbUserId.Text = "User ID:";
            this.lbUserId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // enterLoginLabel
            // 
            this.enterLoginLabel.AutoSize = true;
            this.enterLoginLabel.Location = new System.Drawing.Point(6, 17);
            this.enterLoginLabel.Name = "enterLoginLabel";
            this.enterLoginLabel.Size = new System.Drawing.Size(310, 13);
            this.enterLoginLabel.TabIndex = 1;
            this.enterLoginLabel.Text = "Enter your EVE Online API Parameters and choose a character:";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(276, 234);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(357, 234);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbSpecifyXML
            // 
            this.lbSpecifyXML.AutoSize = true;
            this.lbSpecifyXML.Location = new System.Drawing.Point(15, 28);
            this.lbSpecifyXML.Name = "lbSpecifyXML";
            this.lbSpecifyXML.Size = new System.Drawing.Size(210, 13);
            this.lbSpecifyXML.TabIndex = 0;
            this.lbSpecifyXML.Text = "Specify a saved Character XML file to use:";
            // 
            // tbFilename
            // 
            this.tbFilename.Location = new System.Drawing.Point(18, 55);
            this.tbFilename.Name = "tbFilename";
            this.tbFilename.ReadOnly = true;
            this.tbFilename.Size = new System.Drawing.Size(200, 21);
            this.tbFilename.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(224, 55);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lbAddCharacter
            // 
            this.lbAddCharacter.AutoSize = true;
            this.lbAddCharacter.Location = new System.Drawing.Point(12, 15);
            this.lbAddCharacter.Name = "lbAddCharacter";
            this.lbAddCharacter.Size = new System.Drawing.Size(107, 13);
            this.lbAddCharacter.TabIndex = 5;
            this.lbAddCharacter.Text = "Add character using:";
            // 
            // cbCharacterType
            // 
            this.cbCharacterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCharacterType.FormattingEnabled = true;
            this.cbCharacterType.Items.AddRange(new object[] {
            "EVE Online API",
            "Saved Character XML"});
            this.cbCharacterType.Location = new System.Drawing.Point(125, 12);
            this.cbCharacterType.Name = "cbCharacterType";
            this.cbCharacterType.Size = new System.Drawing.Size(144, 21);
            this.cbCharacterType.TabIndex = 6;
            this.cbCharacterType.SelectedIndexChanged += new System.EventHandler(this.cbCharacterType_SelectedIndexChanged);
            // 
            // gbEveLogin
            // 
            this.gbEveLogin.Controls.Add(this.label1);
            this.gbEveLogin.Controls.Add(this.lblAPILink);
            this.gbEveLogin.Controls.Add(this.lnkAPI);
            this.gbEveLogin.Controls.Add(this.btnCharSelect);
            this.gbEveLogin.Controls.Add(this.enterLoginLabel);
            this.gbEveLogin.Controls.Add(this.tbCharName);
            this.gbEveLogin.Controls.Add(this.lbUserId);
            this.gbEveLogin.Controls.Add(this.lbLoginCharacter);
            this.gbEveLogin.Controls.Add(this.tbAuthKey);
            this.gbEveLogin.Controls.Add(this.tbUserId);
            this.gbEveLogin.Controls.Add(this.lbAuthKey);
            this.gbEveLogin.Location = new System.Drawing.Point(12, 39);
            this.gbEveLogin.Name = "gbEveLogin";
            this.gbEveLogin.Size = new System.Drawing.Size(395, 181);
            this.gbEveLogin.TabIndex = 7;
            this.gbEveLogin.TabStop = false;
            this.gbEveLogin.Text = "EVE Online API Parameters";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "and paste your User ID and API Key into the form below.";
            // 
            // lblAPILink
            // 
            this.lblAPILink.AutoSize = true;
            this.lblAPILink.Location = new System.Drawing.Point(6, 39);
            this.lblAPILink.Name = "lblAPILink";
            this.lblAPILink.Size = new System.Drawing.Size(135, 13);
            this.lblAPILink.TabIndex = 5;
            this.lblAPILink.Text = "Get your API  details from:";
            // 
            // lnkAPI
            // 
            this.lnkAPI.AutoSize = true;
            this.lnkAPI.Location = new System.Drawing.Point(147, 39);
            this.lnkAPI.Name = "lnkAPI";
            this.lnkAPI.Size = new System.Drawing.Size(225, 13);
            this.lnkAPI.TabIndex = 4;
            this.lnkAPI.TabStop = true;
            this.lnkAPI.Text = "http://myeve.eve-online.com/api/default.asp";
            this.lnkAPI.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAPI_LinkClicked);
            // 
            // gbSavedXML
            // 
            this.gbSavedXML.Controls.Add(this.tbFileCharName);
            this.gbSavedXML.Controls.Add(this.lbSavedCharacter);
            this.gbSavedXML.Controls.Add(this.cbMonitorFile);
            this.gbSavedXML.Controls.Add(this.btnBrowse);
            this.gbSavedXML.Controls.Add(this.lbSpecifyXML);
            this.gbSavedXML.Controls.Add(this.tbFilename);
            this.gbSavedXML.Location = new System.Drawing.Point(12, 39);
            this.gbSavedXML.Name = "gbSavedXML";
            this.gbSavedXML.Size = new System.Drawing.Size(318, 154);
            this.gbSavedXML.TabIndex = 8;
            this.gbSavedXML.TabStop = false;
            this.gbSavedXML.Text = "Saved Character XML";
            // 
            // tbFileCharName
            // 
            this.tbFileCharName.Location = new System.Drawing.Point(83, 109);
            this.tbFileCharName.Name = "tbFileCharName";
            this.tbFileCharName.ReadOnly = true;
            this.tbFileCharName.Size = new System.Drawing.Size(100, 21);
            this.tbFileCharName.TabIndex = 5;
            this.tbFileCharName.TabStop = false;
            this.tbFileCharName.Text = "(None)";
            // 
            // lbSavedCharacter
            // 
            this.lbSavedCharacter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSavedCharacter.AutoSize = true;
            this.lbSavedCharacter.Location = new System.Drawing.Point(18, 112);
            this.lbSavedCharacter.Name = "lbSavedCharacter";
            this.lbSavedCharacter.Size = new System.Drawing.Size(59, 13);
            this.lbSavedCharacter.TabIndex = 4;
            this.lbSavedCharacter.Text = "Character:";
            this.lbSavedCharacter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbMonitorFile
            // 
            this.cbMonitorFile.AutoSize = true;
            this.cbMonitorFile.Location = new System.Drawing.Point(18, 81);
            this.cbMonitorFile.Name = "cbMonitorFile";
            this.cbMonitorFile.Size = new System.Drawing.Size(224, 17);
            this.cbMonitorFile.TabIndex = 3;
            this.cbMonitorFile.Text = "Monitor this file for updates automatically";
            this.cbMonitorFile.UseVisualStyleBackColor = true;
            // 
            // ofdOpenXml
            // 
            this.ofdOpenXml.Filter = "Character XML File (*.xml)|*.xml";
            this.ofdOpenXml.Title = "Open Character XML";
            // 
            // LoginCharSelect
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(444, 269);
            this.ControlBox = false;
            this.Controls.Add(this.gbEveLogin);
            this.Controls.Add(this.cbCharacterType);
            this.Controls.Add(this.lbAddCharacter);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gbSavedXML);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginCharSelect";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Character";
            this.Load += new System.EventHandler(this.LoginCharSelect_Load);
            this.gbEveLogin.ResumeLayout(false);
            this.gbEveLogin.PerformLayout();
            this.gbSavedXML.ResumeLayout(false);
            this.gbSavedXML.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbUserId;
        private System.Windows.Forms.Label lbAuthKey;
        private System.Windows.Forms.Label lbLoginCharacter;
        private System.Windows.Forms.TextBox tbUserId;
        private System.Windows.Forms.TextBox tbAuthKey;
        private System.Windows.Forms.Label enterLoginLabel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbCharName;
        private System.Windows.Forms.Button btnCharSelect;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox tbFilename;
        private System.Windows.Forms.Label lbSpecifyXML;
        private System.Windows.Forms.Label lbAddCharacter;
        private System.Windows.Forms.ComboBox cbCharacterType;
        private System.Windows.Forms.GroupBox gbEveLogin;
        private System.Windows.Forms.GroupBox gbSavedXML;
        private System.Windows.Forms.CheckBox cbMonitorFile;
        private System.Windows.Forms.OpenFileDialog ofdOpenXml;
        private System.Windows.Forms.TextBox tbFileCharName;
        private System.Windows.Forms.Label lbSavedCharacter;
        private System.Windows.Forms.LinkLabel lnkAPI;
        private System.Windows.Forms.Label lblAPILink;
        private System.Windows.Forms.Label label1;
    }
}
