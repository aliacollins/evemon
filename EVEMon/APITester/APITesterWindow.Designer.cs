namespace EVEMon.ApiTester
{
    partial class ApiTesterWindow
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
            this.UrlLabel = new System.Windows.Forms.Label();
            this.CharIDTextBox = new System.Windows.Forms.TextBox();
            this.CharIDLabel = new System.Windows.Forms.Label();
            this.ExternalInfoRadioButton = new System.Windows.Forms.RadioButton();
            this.InternalInfoRadioButton = new System.Windows.Forms.RadioButton();
            this.VCodeTextBox = new System.Windows.Forms.TextBox();
            this.KeyIDTextBox = new System.Windows.Forms.TextBox();
            this.VCodeLabel = new System.Windows.Forms.Label();
            this.KeyIDLabel = new System.Windows.Forms.Label();
            this.IDOrNameTextBox = new System.Windows.Forms.TextBox();
            this.IDOrNameLabel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CharacterComboBox = new System.Windows.Forms.ComboBox();
            this.CharacterLabel = new System.Windows.Forms.Label();
            this.APIMethodComboBox = new System.Windows.Forms.ComboBox();
            this.APIMethodLabel = new System.Windows.Forms.Label();
            this.BodyPanel = new System.Windows.Forms.Panel();
            this.WebBrowser = new System.Windows.Forms.WebBrowser();
            this.ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.HeaderPanel.SuspendLayout();
            this.BodyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.Controls.Add(this.UrlLabel);
            this.HeaderPanel.Controls.Add(this.CharIDTextBox);
            this.HeaderPanel.Controls.Add(this.CharIDLabel);
            this.HeaderPanel.Controls.Add(this.ExternalInfoRadioButton);
            this.HeaderPanel.Controls.Add(this.InternalInfoRadioButton);
            this.HeaderPanel.Controls.Add(this.VCodeTextBox);
            this.HeaderPanel.Controls.Add(this.KeyIDTextBox);
            this.HeaderPanel.Controls.Add(this.VCodeLabel);
            this.HeaderPanel.Controls.Add(this.KeyIDLabel);
            this.HeaderPanel.Controls.Add(this.IDOrNameTextBox);
            this.HeaderPanel.Controls.Add(this.IDOrNameLabel);
            this.HeaderPanel.Controls.Add(this.SaveButton);
            this.HeaderPanel.Controls.Add(this.CharacterComboBox);
            this.HeaderPanel.Controls.Add(this.CharacterLabel);
            this.HeaderPanel.Controls.Add(this.APIMethodComboBox);
            this.HeaderPanel.Controls.Add(this.APIMethodLabel);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(687, 174);
            this.HeaderPanel.TabIndex = 0;
            // 
            // APIUrlLabel
            // 
            this.UrlLabel.AutoSize = true;
            this.UrlLabel.Location = new System.Drawing.Point(22, 153);
            this.UrlLabel.Name = "UrlLabel";
            this.UrlLabel.Size = new System.Drawing.Size(120, 13);
            this.UrlLabel.TabIndex = 12;
            this.UrlLabel.Text = "The URL of the API call";
            this.UrlLabel.UseMnemonic = false;
            // 
            // CharIDTextBox
            // 
            this.CharIDTextBox.Location = new System.Drawing.Point(382, 87);
            this.CharIDTextBox.MaxLength = 16;
            this.CharIDTextBox.Name = "CharIDTextBox";
            this.CharIDTextBox.Size = new System.Drawing.Size(102, 20);
            this.CharIDTextBox.TabIndex = 9;
            this.CharIDTextBox.Visible = false;
            this.CharIDTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CharIDTextBox_KeyUp);
            this.CharIDTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.RequiredIDTextBox_Validating);
            this.CharIDTextBox.Validated += new System.EventHandler(this.RequiredIDTextBox_Validated);
            // 
            // CharIDLabel
            // 
            this.CharIDLabel.AutoSize = true;
            this.CharIDLabel.Location = new System.Drawing.Point(382, 71);
            this.CharIDLabel.Name = "CharIDLabel";
            this.CharIDLabel.Size = new System.Drawing.Size(70, 13);
            this.CharIDLabel.TabIndex = 11;
            this.CharIDLabel.Text = "Character ID:";
            this.CharIDLabel.Visible = false;
            // 
            // ExternalInfoRadioButton
            // 
            this.ExternalInfoRadioButton.AutoSize = true;
            this.ExternalInfoRadioButton.CausesValidation = false;
            this.ExternalInfoRadioButton.Location = new System.Drawing.Point(203, 46);
            this.ExternalInfoRadioButton.Name = "ExternalInfoRadioButton";
            this.ExternalInfoRadioButton.Size = new System.Drawing.Size(106, 17);
            this.ExternalInfoRadioButton.TabIndex = 4;
            this.ExternalInfoRadioButton.Text = "Use External Info";
            this.ExternalInfoRadioButton.UseVisualStyleBackColor = true;
            this.ExternalInfoRadioButton.CheckedChanged += new System.EventHandler(this.ExternalInfoRadioButton_CheckedChanged);
            // 
            // InternalInfoRadioButton
            // 
            this.InternalInfoRadioButton.AutoSize = true;
            this.InternalInfoRadioButton.CausesValidation = false;
            this.InternalInfoRadioButton.Checked = true;
            this.InternalInfoRadioButton.Location = new System.Drawing.Point(25, 46);
            this.InternalInfoRadioButton.Name = "InternalInfoRadioButton";
            this.InternalInfoRadioButton.Size = new System.Drawing.Size(103, 17);
            this.InternalInfoRadioButton.TabIndex = 3;
            this.InternalInfoRadioButton.TabStop = true;
            this.InternalInfoRadioButton.Text = "Use Internal Info";
            this.InternalInfoRadioButton.UseVisualStyleBackColor = true;
            this.InternalInfoRadioButton.CheckedChanged += new System.EventHandler(this.InternalInfoRadioButton_CheckedChanged);
            // 
            // VCodeTextBox
            // 
            this.VCodeTextBox.Location = new System.Drawing.Point(203, 126);
            this.VCodeTextBox.MaxLength = 64;
            this.VCodeTextBox.Name = "VCodeTextBox";
            this.VCodeTextBox.Size = new System.Drawing.Size(456, 20);
            this.VCodeTextBox.TabIndex = 8;
            this.VCodeTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VCodeTextBox_KeyUp);
            this.VCodeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.VCodeTextBox_Validating);
            this.VCodeTextBox.Validated += new System.EventHandler(this.VCodeTextBox_Validated);
            // 
            // KeyIDTextBox
            // 
            this.KeyIDTextBox.Location = new System.Drawing.Point(203, 87);
            this.KeyIDTextBox.MaxLength = 16;
            this.KeyIDTextBox.Name = "KeyIDTextBox";
            this.KeyIDTextBox.Size = new System.Drawing.Size(102, 20);
            this.KeyIDTextBox.TabIndex = 7;
            this.KeyIDTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyIDTextBox_KeyUp);
            this.KeyIDTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.RequiredIDTextBox_Validating);
            this.KeyIDTextBox.Validated += new System.EventHandler(this.RequiredIDTextBox_Validated);
            // 
            // VCodeLabel
            // 
            this.VCodeLabel.AutoSize = true;
            this.VCodeLabel.Location = new System.Drawing.Point(203, 110);
            this.VCodeLabel.Name = "VCodeLabel";
            this.VCodeLabel.Size = new System.Drawing.Size(90, 13);
            this.VCodeLabel.TabIndex = 6;
            this.VCodeLabel.Text = "Verification Code:";
            // 
            // KeyIDLabel
            // 
            this.KeyIDLabel.AutoSize = true;
            this.KeyIDLabel.Location = new System.Drawing.Point(203, 71);
            this.KeyIDLabel.Name = "KeyIDLabel";
            this.KeyIDLabel.Size = new System.Drawing.Size(42, 13);
            this.KeyIDLabel.TabIndex = 5;
            this.KeyIDLabel.Text = "Key ID:";
            // 
            // IDOrNameTextBox
            // 
            this.IDOrNameTextBox.Location = new System.Drawing.Point(25, 126);
            this.IDOrNameTextBox.Name = "IDOrNameTextBox";
            this.IDOrNameTextBox.Size = new System.Drawing.Size(155, 20);
            this.IDOrNameTextBox.TabIndex = 6;
            this.IDOrNameTextBox.Visible = false;
            this.IDOrNameTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.IDOrNameTextBox_KeyUp);
            this.IDOrNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.RequiredIDTextBox_Validating);
            this.IDOrNameTextBox.Validated += new System.EventHandler(this.RequiredIDTextBox_Validated);
            // 
            // IDOrNameLabel
            // 
            this.IDOrNameLabel.AutoSize = true;
            this.IDOrNameLabel.Location = new System.Drawing.Point(22, 110);
            this.IDOrNameLabel.Name = "IDOrNameLabel";
            this.IDOrNameLabel.Size = new System.Drawing.Size(21, 13);
            this.IDOrNameLabel.TabIndex = 3;
            this.IDOrNameLabel.Text = "ID:";
            this.IDOrNameLabel.Visible = false;
            // 
            // SaveButton
            // 
            this.SaveButton.CausesValidation = false;
            this.SaveButton.Enabled = false;
            this.SaveButton.Location = new System.Drawing.Point(330, 16);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CharacterComboBox
            // 
            this.CharacterComboBox.CausesValidation = false;
            this.CharacterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CharacterComboBox.FormattingEnabled = true;
            this.CharacterComboBox.Location = new System.Drawing.Point(25, 87);
            this.CharacterComboBox.Name = "CharacterComboBox";
            this.CharacterComboBox.Size = new System.Drawing.Size(155, 21);
            this.CharacterComboBox.TabIndex = 5;
            this.CharacterComboBox.SelectedIndexChanged += new System.EventHandler(this.CharacterComboBox_SelectedIndexChanged);
            // 
            // CharacterLabel
            // 
            this.CharacterLabel.AutoSize = true;
            this.CharacterLabel.Location = new System.Drawing.Point(22, 71);
            this.CharacterLabel.Name = "CharacterLabel";
            this.CharacterLabel.Size = new System.Drawing.Size(56, 13);
            this.CharacterLabel.TabIndex = 2;
            this.CharacterLabel.Text = "Character:";
            // 
            // APIMethodComboBox
            // 
            this.APIMethodComboBox.CausesValidation = false;
            this.APIMethodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.APIMethodComboBox.FormattingEnabled = true;
            this.APIMethodComboBox.Location = new System.Drawing.Point(84, 17);
            this.APIMethodComboBox.Name = "APIMethodComboBox";
            this.APIMethodComboBox.Size = new System.Drawing.Size(220, 21);
            this.APIMethodComboBox.TabIndex = 2;
            this.APIMethodComboBox.SelectedIndexChanged += new System.EventHandler(this.APIMethodComboBox_SelectedIndexChanged);
            // 
            // APIMethodLabel
            // 
            this.APIMethodLabel.AutoSize = true;
            this.APIMethodLabel.Location = new System.Drawing.Point(12, 22);
            this.APIMethodLabel.Name = "APIMethodLabel";
            this.APIMethodLabel.Size = new System.Drawing.Size(66, 13);
            this.APIMethodLabel.TabIndex = 1;
            this.APIMethodLabel.Text = "API Method:";
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.WebBrowser);
            this.BodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BodyPanel.Location = new System.Drawing.Point(0, 174);
            this.BodyPanel.Name = "BodyPanel";
            this.BodyPanel.Padding = new System.Windows.Forms.Padding(3);
            this.BodyPanel.Size = new System.Drawing.Size(687, 228);
            this.BodyPanel.TabIndex = 1;
            // 
            // WebBrowser
            // 
            this.WebBrowser.AllowWebBrowserDrop = false;
            this.WebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.WebBrowser.Location = new System.Drawing.Point(3, 3);
            this.WebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser.Name = "WebBrowser";
            this.WebBrowser.Size = new System.Drawing.Size(681, 222);
            this.WebBrowser.TabIndex = 0;
            this.WebBrowser.TabStop = false;
            this.WebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowser_DocumentCompleted);
            this.WebBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowser_Navigating);
            this.WebBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.WebBrowser_PreviewKeyDown);
            // 
            // ErrorProvider
            // 
            this.ErrorProvider.ContainerControl = this;
            // 
            // ApiTesterWindow
            // 
            this.AcceptButton = this.SaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 402);
            this.Controls.Add(this.BodyPanel);
            this.Controls.Add(this.HeaderPanel);
            this.KeyPreview = true;
            this.Name = "ApiTesterWindow";
            this.Text = "API Tester";
            this.Load += new System.EventHandler(this.ApiTesterWindow_Load);
            this.HeaderPanel.ResumeLayout(false);
            this.HeaderPanel.PerformLayout();
            this.BodyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel HeaderPanel;
        private System.Windows.Forms.Panel BodyPanel;
        private System.Windows.Forms.WebBrowser WebBrowser;
        private System.Windows.Forms.ComboBox CharacterComboBox;
        private System.Windows.Forms.Label CharacterLabel;
        private System.Windows.Forms.ComboBox APIMethodComboBox;
        private System.Windows.Forms.Label APIMethodLabel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TextBox IDOrNameTextBox;
        private System.Windows.Forms.Label IDOrNameLabel;
        private System.Windows.Forms.Label VCodeLabel;
        private System.Windows.Forms.Label KeyIDLabel;
        private System.Windows.Forms.TextBox VCodeTextBox;
        private System.Windows.Forms.TextBox KeyIDTextBox;
        private System.Windows.Forms.RadioButton ExternalInfoRadioButton;
        private System.Windows.Forms.RadioButton InternalInfoRadioButton;
        private System.Windows.Forms.ErrorProvider ErrorProvider;
        private System.Windows.Forms.TextBox CharIDTextBox;
        private System.Windows.Forms.Label CharIDLabel;
        private System.Windows.Forms.Label UrlLabel;
    }
}