namespace EVEMon.APITester
{
    partial class APITesterWindow
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
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.tbCharID = new System.Windows.Forms.TextBox();
            this.lblCharID = new System.Windows.Forms.Label();
            this.rbExternal = new System.Windows.Forms.RadioButton();
            this.rbInternal = new System.Windows.Forms.RadioButton();
            this.tbVCode = new System.Windows.Forms.TextBox();
            this.tbKeyID = new System.Windows.Forms.TextBox();
            this.lblVCode = new System.Windows.Forms.Label();
            this.lblKeyID = new System.Windows.Forms.Label();
            this.tbIDOrName = new System.Windows.Forms.TextBox();
            this.lblIDOrName = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cbCharacter = new System.Windows.Forms.ComboBox();
            this.lblCharacter = new System.Windows.Forms.Label();
            this.cbAPIMethod = new System.Windows.Forms.ComboBox();
            this.lblAPIMethod = new System.Windows.Forms.Label();
            this.BodyPanel = new System.Windows.Forms.Panel();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.HeaderPanel.SuspendLayout();
            this.BodyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.Controls.Add(this.tbCharID);
            this.HeaderPanel.Controls.Add(this.lblCharID);
            this.HeaderPanel.Controls.Add(this.rbExternal);
            this.HeaderPanel.Controls.Add(this.rbInternal);
            this.HeaderPanel.Controls.Add(this.tbVCode);
            this.HeaderPanel.Controls.Add(this.tbKeyID);
            this.HeaderPanel.Controls.Add(this.lblVCode);
            this.HeaderPanel.Controls.Add(this.lblKeyID);
            this.HeaderPanel.Controls.Add(this.tbIDOrName);
            this.HeaderPanel.Controls.Add(this.lblIDOrName);
            this.HeaderPanel.Controls.Add(this.saveButton);
            this.HeaderPanel.Controls.Add(this.cbCharacter);
            this.HeaderPanel.Controls.Add(this.lblCharacter);
            this.HeaderPanel.Controls.Add(this.cbAPIMethod);
            this.HeaderPanel.Controls.Add(this.lblAPIMethod);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(574, 166);
            this.HeaderPanel.TabIndex = 0;
            // 
            // tbCharID
            // 
            this.tbCharID.Location = new System.Drawing.Point(382, 95);
            this.tbCharID.MaxLength = 12;
            this.tbCharID.Name = "tbCharID";
            this.tbCharID.Size = new System.Drawing.Size(100, 20);
            this.tbCharID.TabIndex = 9;
            this.tbCharID.Visible = false;
            this.tbCharID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbCharID_KeyUp);
            this.tbCharID.Validating += new System.ComponentModel.CancelEventHandler(this.IDRequiredTextBox_Validating);
            this.tbCharID.Validated += new System.EventHandler(this.IDRequiredTextBox_Validated);
            // 
            // lblCharID
            // 
            this.lblCharID.AutoSize = true;
            this.lblCharID.Location = new System.Drawing.Point(382, 79);
            this.lblCharID.Name = "lblCharID";
            this.lblCharID.Size = new System.Drawing.Size(70, 13);
            this.lblCharID.TabIndex = 11;
            this.lblCharID.Text = "Character ID:";
            this.lblCharID.Visible = false;
            // 
            // rbExternal
            // 
            this.rbExternal.AutoSize = true;
            this.rbExternal.CausesValidation = false;
            this.rbExternal.Location = new System.Drawing.Point(203, 54);
            this.rbExternal.Name = "rbExternal";
            this.rbExternal.Size = new System.Drawing.Size(106, 17);
            this.rbExternal.TabIndex = 4;
            this.rbExternal.Text = "Use External Info";
            this.rbExternal.UseVisualStyleBackColor = true;
            this.rbExternal.CheckedChanged += new System.EventHandler(this.rbExternal_CheckedChanged);
            // 
            // rbInternal
            // 
            this.rbInternal.AutoSize = true;
            this.rbInternal.CausesValidation = false;
            this.rbInternal.Checked = true;
            this.rbInternal.Location = new System.Drawing.Point(25, 54);
            this.rbInternal.Name = "rbInternal";
            this.rbInternal.Size = new System.Drawing.Size(103, 17);
            this.rbInternal.TabIndex = 3;
            this.rbInternal.TabStop = true;
            this.rbInternal.Text = "Use Internal Info";
            this.rbInternal.UseVisualStyleBackColor = true;
            this.rbInternal.CheckedChanged += new System.EventHandler(this.rbInternal_CheckedChanged);
            // 
            // tbVCode
            // 
            this.tbVCode.Location = new System.Drawing.Point(203, 134);
            this.tbVCode.MaxLength = 32;
            this.tbVCode.Name = "tbVCode";
            this.tbVCode.Size = new System.Drawing.Size(342, 20);
            this.tbVCode.TabIndex = 8;
            this.tbVCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbVCode_KeyUp);
            this.tbVCode.Validating += new System.ComponentModel.CancelEventHandler(this.tbVCode_Validating);
            this.tbVCode.Validated += new System.EventHandler(this.tbVCode_Validated);
            // 
            // tbKeyID
            // 
            this.tbKeyID.Location = new System.Drawing.Point(203, 95);
            this.tbKeyID.MaxLength = 12;
            this.tbKeyID.Name = "tbKeyID";
            this.tbKeyID.Size = new System.Drawing.Size(100, 20);
            this.tbKeyID.TabIndex = 7;
            this.tbKeyID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbKeyID_KeyUp);
            this.tbKeyID.Validating += new System.ComponentModel.CancelEventHandler(this.IDRequiredTextBox_Validating);
            this.tbKeyID.Validated += new System.EventHandler(this.IDRequiredTextBox_Validated);
            // 
            // lblVCode
            // 
            this.lblVCode.AutoSize = true;
            this.lblVCode.Location = new System.Drawing.Point(203, 118);
            this.lblVCode.Name = "lblVCode";
            this.lblVCode.Size = new System.Drawing.Size(90, 13);
            this.lblVCode.TabIndex = 6;
            this.lblVCode.Text = "Verification Code:";
            // 
            // lblKeyID
            // 
            this.lblKeyID.AutoSize = true;
            this.lblKeyID.Location = new System.Drawing.Point(203, 79);
            this.lblKeyID.Name = "lblKeyID";
            this.lblKeyID.Size = new System.Drawing.Size(42, 13);
            this.lblKeyID.TabIndex = 5;
            this.lblKeyID.Text = "Key ID:";
            // 
            // tbIDOrName
            // 
            this.tbIDOrName.Location = new System.Drawing.Point(25, 134);
            this.tbIDOrName.MaxLength = 32;
            this.tbIDOrName.Name = "tbIDOrName";
            this.tbIDOrName.Size = new System.Drawing.Size(155, 20);
            this.tbIDOrName.TabIndex = 6;
            this.tbIDOrName.Visible = false;
            this.tbIDOrName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbIDOrName_KeyUp);
            this.tbIDOrName.Validating += new System.ComponentModel.CancelEventHandler(this.IDRequiredTextBox_Validating);
            this.tbIDOrName.Validated += new System.EventHandler(this.IDRequiredTextBox_Validated);
            // 
            // lblIDOrName
            // 
            this.lblIDOrName.AutoSize = true;
            this.lblIDOrName.Location = new System.Drawing.Point(22, 118);
            this.lblIDOrName.Name = "lblIDOrName";
            this.lblIDOrName.Size = new System.Drawing.Size(21, 13);
            this.lblIDOrName.TabIndex = 3;
            this.lblIDOrName.Text = "ID:";
            this.lblIDOrName.Visible = false;
            // 
            // saveButton
            // 
            this.saveButton.CausesValidation = false;
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(268, 16);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cbCharacter
            // 
            this.cbCharacter.CausesValidation = false;
            this.cbCharacter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCharacter.FormattingEnabled = true;
            this.cbCharacter.Location = new System.Drawing.Point(25, 95);
            this.cbCharacter.Name = "cbCharacter";
            this.cbCharacter.Size = new System.Drawing.Size(155, 21);
            this.cbCharacter.TabIndex = 5;
            this.cbCharacter.SelectedIndexChanged += new System.EventHandler(this.cbCharacter_SelectedIndexChanged);
            // 
            // lblCharacter
            // 
            this.lblCharacter.AutoSize = true;
            this.lblCharacter.Location = new System.Drawing.Point(22, 79);
            this.lblCharacter.Name = "lblCharacter";
            this.lblCharacter.Size = new System.Drawing.Size(56, 13);
            this.lblCharacter.TabIndex = 2;
            this.lblCharacter.Text = "Character:";
            // 
            // cbAPIMethod
            // 
            this.cbAPIMethod.CausesValidation = false;
            this.cbAPIMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAPIMethod.FormattingEnabled = true;
            this.cbAPIMethod.Location = new System.Drawing.Point(84, 17);
            this.cbAPIMethod.Name = "cbAPIMethod";
            this.cbAPIMethod.Size = new System.Drawing.Size(155, 21);
            this.cbAPIMethod.TabIndex = 2;
            this.cbAPIMethod.SelectedIndexChanged += new System.EventHandler(this.cbAPIMethod_SelectedIndexChanged);
            // 
            // lblAPIMethod
            // 
            this.lblAPIMethod.AutoSize = true;
            this.lblAPIMethod.Location = new System.Drawing.Point(12, 22);
            this.lblAPIMethod.Name = "lblAPIMethod";
            this.lblAPIMethod.Size = new System.Drawing.Size(66, 13);
            this.lblAPIMethod.TabIndex = 1;
            this.lblAPIMethod.Text = "API Method:";
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.webBrowser);
            this.BodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BodyPanel.Location = new System.Drawing.Point(0, 166);
            this.BodyPanel.Name = "BodyPanel";
            this.BodyPanel.Padding = new System.Windows.Forms.Padding(3);
            this.BodyPanel.Size = new System.Drawing.Size(574, 229);
            this.BodyPanel.TabIndex = 1;
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(3, 3);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(568, 223);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.TabStop = false;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.webBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.webBrowser_PreviewKeyDown);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // APITesterWindow
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 395);
            this.Controls.Add(this.BodyPanel);
            this.Controls.Add(this.HeaderPanel);
            this.KeyPreview = true;
            this.Name = "APITesterWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "API Tester";
            this.Load += new System.EventHandler(this.APITesterWindow_Load);
            this.HeaderPanel.ResumeLayout(false);
            this.HeaderPanel.PerformLayout();
            this.BodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel HeaderPanel;
        private System.Windows.Forms.Panel BodyPanel;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ComboBox cbCharacter;
        private System.Windows.Forms.Label lblCharacter;
        private System.Windows.Forms.ComboBox cbAPIMethod;
        private System.Windows.Forms.Label lblAPIMethod;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox tbIDOrName;
        private System.Windows.Forms.Label lblIDOrName;
        private System.Windows.Forms.Label lblVCode;
        private System.Windows.Forms.Label lblKeyID;
        private System.Windows.Forms.TextBox tbVCode;
        private System.Windows.Forms.TextBox tbKeyID;
        private System.Windows.Forms.RadioButton rbExternal;
        private System.Windows.Forms.RadioButton rbInternal;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TextBox tbCharID;
        private System.Windows.Forms.Label lblCharID;
    }
}