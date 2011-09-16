using EVEMon.Common.Controls;
using EVEMon.Common.Controls.MultiPanel;

namespace EVEMon.ApiCredentialsManagement
{
    partial class ApiKeyUpdateOrAdditionWindow
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
            System.Windows.Forms.LinkLabel FeaturesLinkLabel;
            System.Windows.Forms.LinkLabel ApiKeysLinkLabel;
            System.Windows.Forms.Label FetchingDataLabel;
            System.Windows.Forms.Label GuideLabel;
            System.Windows.Forms.LinkLabel ActiveLinksLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApiKeyUpdateOrAdditionWindow));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Mary Jane");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Ali Baba");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("John Doe");
            this.ButtonNext = new System.Windows.Forms.Button();
            this.ButtonPrevious = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.MultiPanel = new EVEMon.Common.Controls.MultiPanel.MultiPanel();
            this.CredentialsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.VerificationCodeTextBox = new System.Windows.Forms.TextBox();
            this.IDTextBox = new System.Windows.Forms.TextBox();
            this.VerificationCodeLabel = new System.Windows.Forms.Label();
            this.IDLabel = new System.Windows.Forms.Label();
            this.WaitingPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.Throbber = new EVEMon.Common.Controls.Throbber();
            this.ResultPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.KeyTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.KeyLabel = new System.Windows.Forms.Label();
            this.KeyPicture = new System.Windows.Forms.PictureBox();
            this.CharactersGroupBox = new System.Windows.Forms.GroupBox();
            this.ResultsMultiPanel = new EVEMon.Common.Controls.MultiPanel.MultiPanel();
            this.CharactersListPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.CharactersListView = new System.Windows.Forms.ListView();
            this.WarningLabel = new System.Windows.Forms.Label();
            this.AuthenticationErrorPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.AuthenticationErrorGuideLabel = new System.Windows.Forms.Label();
            this.LoginDeniedErrorPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.LoginDeniedLinkLabel = new System.Windows.Forms.LinkLabel();
            this.GeneralErrorPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.GeneralErrorLabel = new System.Windows.Forms.Label();
            this.APIKeyExpiredErrorPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.APIKeyExpiredLinkLabel = new System.Windows.Forms.LinkLabel();
            this.APIKeyExistsErrorPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.APIKeyExistsLabel = new System.Windows.Forms.Label();
            this.CachedWarningPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.CachedWarningLabel = new System.Windows.Forms.Label();
            this.NoCorpFeaturesYetPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.NoCorpFeaturesYetLabel = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            FeaturesLinkLabel = new System.Windows.Forms.LinkLabel();
            ApiKeysLinkLabel = new System.Windows.Forms.LinkLabel();
            FetchingDataLabel = new System.Windows.Forms.Label();
            GuideLabel = new System.Windows.Forms.Label();
            ActiveLinksLabel = new System.Windows.Forms.LinkLabel();
            this.MultiPanel.SuspendLayout();
            this.CredentialsPage.SuspendLayout();
            this.WaitingPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Throbber)).BeginInit();
            this.ResultPage.SuspendLayout();
            this.KeyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.KeyPicture)).BeginInit();
            this.CharactersGroupBox.SuspendLayout();
            this.ResultsMultiPanel.SuspendLayout();
            this.CharactersListPage.SuspendLayout();
            this.AuthenticationErrorPage.SuspendLayout();
            this.LoginDeniedErrorPage.SuspendLayout();
            this.GeneralErrorPage.SuspendLayout();
            this.APIKeyExpiredErrorPage.SuspendLayout();
            this.APIKeyExistsErrorPage.SuspendLayout();
            this.CachedWarningPage.SuspendLayout();
            this.NoCorpFeaturesYetPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // FeaturesLinkLabel
            // 
            FeaturesLinkLabel.CausesValidation = false;
            FeaturesLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(75, 15);
            FeaturesLinkLabel.Location = new System.Drawing.Point(23, 28);
            FeaturesLinkLabel.Name = "FeaturesLinkLabel";
            FeaturesLinkLabel.Size = new System.Drawing.Size(476, 18);
            FeaturesLinkLabel.TabIndex = 3;
            FeaturesLinkLabel.TabStop = true;
            FeaturesLinkLabel.Text = "To see what kind of Access Mask the API key needs to be, check the list of EVEMon" +
                " features.";
            FeaturesLinkLabel.UseCompatibleTextRendering = true;
            FeaturesLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FeaturesLinkLabel_LinkClicked);
            // 
            // ApiKeysLinkLabel
            // 
            ApiKeysLinkLabel.AutoSize = true;
            ApiKeysLinkLabel.CausesValidation = false;
            ApiKeysLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(33, 33);
            ApiKeysLinkLabel.Location = new System.Drawing.Point(23, 10);
            ApiKeysLinkLabel.Name = "ApiKeysLinkLabel";
            ApiKeysLinkLabel.Size = new System.Drawing.Size(335, 18);
            ApiKeysLinkLabel.TabIndex = 2;
            ApiKeysLinkLabel.TabStop = true;
            ApiKeysLinkLabel.Text = "Your API keys are available at : https://support.eveonline.com/api";
            ApiKeysLinkLabel.UseCompatibleTextRendering = true;
            ApiKeysLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ApiCredentialsLinkLabel_LinkClicked);
            // 
            // FetchingDataLabel
            // 
            FetchingDataLabel.Location = new System.Drawing.Point(209, 71);
            FetchingDataLabel.Name = "FetchingDataLabel";
            FetchingDataLabel.Size = new System.Drawing.Size(153, 24);
            FetchingDataLabel.TabIndex = 1;
            FetchingDataLabel.Text = "Fetching data from CCP...";
            FetchingDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GuideLabel
            // 
            GuideLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            GuideLabel.Location = new System.Drawing.Point(3, 3);
            GuideLabel.Name = "GuideLabel";
            GuideLabel.Size = new System.Drawing.Size(267, 79);
            GuideLabel.TabIndex = 3;
            GuideLabel.Text = "Uncheck the characters you do not want to import.\r\n\r\nYou can also import a charac" +
                "ter and hide it through the API keys management window.";
            // 
            // ActiveLinksLabel
            // 
            ActiveLinksLabel.CausesValidation = false;
            ActiveLinksLabel.LinkArea = new System.Windows.Forms.LinkArea(100, 56);
            ActiveLinksLabel.Location = new System.Drawing.Point(23, 46);
            ActiveLinksLabel.Name = "ActiveLinksLabel";
            ActiveLinksLabel.Size = new System.Drawing.Size(476, 30);
            ActiveLinksLabel.TabIndex = 4;
            ActiveLinksLabel.TabStop = true;
            ActiveLinksLabel.Text = "If you have already an API key, you can install it directly by clicking on the [I" +
                "nstall] link at :\r\nhttps://support.eveonline.com/api/key/activeinstalllinks";
            ActiveLinksLabel.UseCompatibleTextRendering = true;
            ActiveLinksLabel.Visible = false;
            ActiveLinksLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ActiveLinksLabel_LinkClicked);
            // 
            // ButtonNext
            // 
            this.ButtonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonNext.Location = new System.Drawing.Point(318, 177);
            this.ButtonNext.Name = "ButtonNext";
            this.ButtonNext.Size = new System.Drawing.Size(75, 23);
            this.ButtonNext.TabIndex = 1;
            this.ButtonNext.Text = "&Next >";
            this.ButtonNext.UseVisualStyleBackColor = true;
            this.ButtonNext.Click += new System.EventHandler(this.ButtonNext_Click);
            // 
            // ButtonPrevious
            // 
            this.ButtonPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonPrevious.Enabled = false;
            this.ButtonPrevious.Location = new System.Drawing.Point(237, 177);
            this.ButtonPrevious.Name = "ButtonPrevious";
            this.ButtonPrevious.Size = new System.Drawing.Size(75, 23);
            this.ButtonPrevious.TabIndex = 0;
            this.ButtonPrevious.Text = "< &Previous";
            this.ButtonPrevious.UseVisualStyleBackColor = true;
            this.ButtonPrevious.Click += new System.EventHandler(this.ButtonPrevious_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.CausesValidation = false;
            this.ButtonCancel.Location = new System.Drawing.Point(416, 177);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 2;
            this.ButtonCancel.Text = "&Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // MultiPanel
            // 
            this.MultiPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MultiPanel.Controls.Add(this.CredentialsPage);
            this.MultiPanel.Controls.Add(this.WaitingPage);
            this.MultiPanel.Controls.Add(this.ResultPage);
            this.MultiPanel.Location = new System.Drawing.Point(0, 0);
            this.MultiPanel.Name = "MultiPanel";
            this.MultiPanel.SelectedPage = this.CredentialsPage;
            this.MultiPanel.Size = new System.Drawing.Size(503, 171);
            this.MultiPanel.TabIndex = 0;
            // 
            // CredentialsPage
            // 
            this.CredentialsPage.CausesValidation = false;
            this.CredentialsPage.Controls.Add(ActiveLinksLabel);
            this.CredentialsPage.Controls.Add(FeaturesLinkLabel);
            this.CredentialsPage.Controls.Add(ApiKeysLinkLabel);
            this.CredentialsPage.Controls.Add(this.VerificationCodeTextBox);
            this.CredentialsPage.Controls.Add(this.IDTextBox);
            this.CredentialsPage.Controls.Add(this.VerificationCodeLabel);
            this.CredentialsPage.Controls.Add(this.IDLabel);
            this.CredentialsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CredentialsPage.Location = new System.Drawing.Point(0, 0);
            this.CredentialsPage.Name = "CredentialsPage";
            this.CredentialsPage.Size = new System.Drawing.Size(503, 171);
            this.CredentialsPage.TabIndex = 0;
            this.CredentialsPage.Text = "credentialsPage";
            // 
            // VerificationCodeTextBox
            // 
            this.VerificationCodeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VerificationCodeTextBox.Location = new System.Drawing.Point(41, 141);
            this.VerificationCodeTextBox.Name = "VerificationCodeTextBox";
            this.VerificationCodeTextBox.Size = new System.Drawing.Size(424, 21);
            this.VerificationCodeTextBox.TabIndex = 1;
            this.VerificationCodeTextBox.TextChanged += new System.EventHandler(this.VerificationCodeTextBox_TextChanged);
            this.VerificationCodeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.VerificationCodeTextBox_Validating);
            this.VerificationCodeTextBox.Validated += new System.EventHandler(this.VerificationCodeTextBox_Validated);
            // 
            // IDTextBox
            // 
            this.IDTextBox.Location = new System.Drawing.Point(41, 99);
            this.IDTextBox.Name = "IDTextBox";
            this.IDTextBox.Size = new System.Drawing.Size(109, 21);
            this.IDTextBox.TabIndex = 0;
            this.IDTextBox.TextChanged += new System.EventHandler(this.IDTextBox_TextChanged);
            this.IDTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.IDTextBox_Validating);
            this.IDTextBox.Validated += new System.EventHandler(this.IDTextBox_Validated);
            // 
            // VerificationCodeLabel
            // 
            this.VerificationCodeLabel.AutoSize = true;
            this.VerificationCodeLabel.Location = new System.Drawing.Point(38, 125);
            this.VerificationCodeLabel.Name = "VerificationCodeLabel";
            this.VerificationCodeLabel.Size = new System.Drawing.Size(92, 13);
            this.VerificationCodeLabel.TabIndex = 1;
            this.VerificationCodeLabel.Text = "Verification Code:";
            // 
            // IDLabel
            // 
            this.IDLabel.AutoSize = true;
            this.IDLabel.Location = new System.Drawing.Point(38, 83);
            this.IDLabel.Name = "IDLabel";
            this.IDLabel.Size = new System.Drawing.Size(22, 13);
            this.IDLabel.TabIndex = 0;
            this.IDLabel.Text = "ID:";
            // 
            // WaitingPage
            // 
            this.WaitingPage.Controls.Add(FetchingDataLabel);
            this.WaitingPage.Controls.Add(this.Throbber);
            this.WaitingPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WaitingPage.Location = new System.Drawing.Point(0, 0);
            this.WaitingPage.Name = "WaitingPage";
            this.WaitingPage.Size = new System.Drawing.Size(503, 171);
            this.WaitingPage.TabIndex = 1;
            this.WaitingPage.Text = "waitingPage";
            // 
            // Throbber
            // 
            this.Throbber.Location = new System.Drawing.Point(179, 71);
            this.Throbber.MaximumSize = new System.Drawing.Size(24, 24);
            this.Throbber.MinimumSize = new System.Drawing.Size(24, 24);
            this.Throbber.Name = "Throbber";
            this.Throbber.Size = new System.Drawing.Size(24, 24);
            this.Throbber.State = EVEMon.Common.ThrobberState.Stopped;
            this.Throbber.TabIndex = 0;
            this.Throbber.TabStop = false;
            // 
            // ResultPage
            // 
            this.ResultPage.Controls.Add(this.KeyTableLayoutPanel);
            this.ResultPage.Controls.Add(this.CharactersGroupBox);
            this.ResultPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultPage.Location = new System.Drawing.Point(0, 0);
            this.ResultPage.Name = "ResultPage";
            this.ResultPage.Size = new System.Drawing.Size(503, 171);
            this.ResultPage.TabIndex = 2;
            this.ResultPage.Text = "resultPage";
            // 
            // KeyTableLayoutPanel
            // 
            this.KeyTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.KeyTableLayoutPanel.AutoSize = true;
            this.KeyTableLayoutPanel.ColumnCount = 2;
            this.KeyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.141962F));
            this.KeyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 91.85804F));
            this.KeyTableLayoutPanel.Controls.Add(this.KeyLabel, 1, 0);
            this.KeyTableLayoutPanel.Controls.Add(this.KeyPicture, 0, 0);
            this.KeyTableLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.KeyTableLayoutPanel.Name = "KeyTableLayoutPanel";
            this.KeyTableLayoutPanel.RowCount = 1;
            this.KeyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.KeyTableLayoutPanel.Size = new System.Drawing.Size(479, 38);
            this.KeyTableLayoutPanel.TabIndex = 0;
            // 
            // KeyLabel
            // 
            this.KeyLabel.AutoSize = true;
            this.KeyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KeyLabel.Location = new System.Drawing.Point(41, 0);
            this.KeyLabel.Name = "KeyLabel";
            this.KeyLabel.Size = new System.Drawing.Size(435, 38);
            this.KeyLabel.TabIndex = 1;
            this.KeyLabel.Text = "Short description on info retrieval procedure.";
            this.KeyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // KeyPicture
            // 
            this.KeyPicture.Image = ((System.Drawing.Image)(resources.GetObject("KeyPicture.Image")));
            this.KeyPicture.Location = new System.Drawing.Point(3, 3);
            this.KeyPicture.Name = "KeyPicture";
            this.KeyPicture.Size = new System.Drawing.Size(32, 32);
            this.KeyPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.KeyPicture.TabIndex = 0;
            this.KeyPicture.TabStop = false;
            // 
            // CharactersGroupBox
            // 
            this.CharactersGroupBox.Controls.Add(this.ResultsMultiPanel);
            this.CharactersGroupBox.Location = new System.Drawing.Point(12, 50);
            this.CharactersGroupBox.Name = "CharactersGroupBox";
            this.CharactersGroupBox.Size = new System.Drawing.Size(479, 118);
            this.CharactersGroupBox.TabIndex = 3;
            this.CharactersGroupBox.TabStop = false;
            this.CharactersGroupBox.Text = "Characters exposed by API key";
            // 
            // ResultsMultiPanel
            // 
            this.ResultsMultiPanel.Controls.Add(this.CharactersListPage);
            this.ResultsMultiPanel.Controls.Add(this.AuthenticationErrorPage);
            this.ResultsMultiPanel.Controls.Add(this.LoginDeniedErrorPage);
            this.ResultsMultiPanel.Controls.Add(this.GeneralErrorPage);
            this.ResultsMultiPanel.Controls.Add(this.APIKeyExpiredErrorPage);
            this.ResultsMultiPanel.Controls.Add(this.APIKeyExistsErrorPage);
            this.ResultsMultiPanel.Controls.Add(this.CachedWarningPage);
            this.ResultsMultiPanel.Controls.Add(this.NoCorpFeaturesYetPage);
            this.ResultsMultiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultsMultiPanel.Location = new System.Drawing.Point(3, 17);
            this.ResultsMultiPanel.Name = "ResultsMultiPanel";
            this.ResultsMultiPanel.SelectedPage = this.CharactersListPage;
            this.ResultsMultiPanel.Size = new System.Drawing.Size(473, 98);
            this.ResultsMultiPanel.TabIndex = 5;
            // 
            // CharactersListPage
            // 
            this.CharactersListPage.Controls.Add(this.CharactersListView);
            this.CharactersListPage.Controls.Add(GuideLabel);
            this.CharactersListPage.Controls.Add(this.WarningLabel);
            this.CharactersListPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CharactersListPage.Location = new System.Drawing.Point(0, 0);
            this.CharactersListPage.Name = "CharactersListPage";
            this.CharactersListPage.Size = new System.Drawing.Size(473, 98);
            this.CharactersListPage.TabIndex = 0;
            this.CharactersListPage.Text = "charactersListPage";
            // 
            // CharactersListView
            // 
            this.CharactersListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CharactersListView.CheckBoxes = true;
            this.CharactersListView.FullRowSelect = true;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.Checked = true;
            listViewItem3.StateImageIndex = 1;
            this.CharactersListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.CharactersListView.Location = new System.Drawing.Point(276, 3);
            this.CharactersListView.Name = "CharactersListView";
            this.CharactersListView.Size = new System.Drawing.Size(197, 76);
            this.CharactersListView.TabIndex = 2;
            this.CharactersListView.UseCompatibleStateImageBehavior = false;
            this.CharactersListView.View = System.Windows.Forms.View.List;
            // 
            // WarningLabel
            // 
            this.WarningLabel.AutoSize = true;
            this.WarningLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.WarningLabel.Location = new System.Drawing.Point(3, 82);
            this.WarningLabel.Name = "WarningLabel";
            this.WarningLabel.Size = new System.Drawing.Size(414, 13);
            this.WarningLabel.TabIndex = 4;
            this.WarningLabel.Text = "Beware! When you remove characters, all their data and plans will be definitely l" +
                "ost !";
            // 
            // AuthenticationErrorPage
            // 
            this.AuthenticationErrorPage.Controls.Add(this.AuthenticationErrorGuideLabel);
            this.AuthenticationErrorPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthenticationErrorPage.Location = new System.Drawing.Point(0, 0);
            this.AuthenticationErrorPage.Name = "AuthenticationErrorPage";
            this.AuthenticationErrorPage.Size = new System.Drawing.Size(473, 98);
            this.AuthenticationErrorPage.TabIndex = 1;
            this.AuthenticationErrorPage.Text = "authenticationErrorPage";
            // 
            // AuthenticationErrorGuideLabel
            // 
            this.AuthenticationErrorGuideLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthenticationErrorGuideLabel.Location = new System.Drawing.Point(0, 0);
            this.AuthenticationErrorGuideLabel.Name = "AuthenticationErrorGuideLabel";
            this.AuthenticationErrorGuideLabel.Padding = new System.Windows.Forms.Padding(54, 0, 0, 0);
            this.AuthenticationErrorGuideLabel.Size = new System.Drawing.Size(473, 98);
            this.AuthenticationErrorGuideLabel.TabIndex = 0;
            this.AuthenticationErrorGuideLabel.Text = resources.GetString("AuthenticationErrorGuideLabel.Text");
            this.AuthenticationErrorGuideLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LoginDeniedErrorPage
            // 
            this.LoginDeniedErrorPage.Controls.Add(this.LoginDeniedLinkLabel);
            this.LoginDeniedErrorPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoginDeniedErrorPage.Location = new System.Drawing.Point(0, 0);
            this.LoginDeniedErrorPage.Name = "LoginDeniedErrorPage";
            this.LoginDeniedErrorPage.Size = new System.Drawing.Size(473, 98);
            this.LoginDeniedErrorPage.TabIndex = 2;
            this.LoginDeniedErrorPage.Text = "loginDeniedErrorPage";
            // 
            // LoginDeniedLinkLabel
            // 
            this.LoginDeniedLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoginDeniedLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(149, 49);
            this.LoginDeniedLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.LoginDeniedLinkLabel.Name = "LoginDeniedLinkLabel";
            this.LoginDeniedLinkLabel.Padding = new System.Windows.Forms.Padding(40, 0, 0, 0);
            this.LoginDeniedLinkLabel.Size = new System.Drawing.Size(473, 98);
            this.LoginDeniedLinkLabel.TabIndex = 0;
            this.LoginDeniedLinkLabel.TabStop = true;
            this.LoginDeniedLinkLabel.Text = "The subscription of the account, this API key belongs to, has expired.\r\n\r\nIf the AP" +
                "I key belongs to one of your accounts, consider re-subscribing at :\r\nhttps://sec" +
                "ure.eveonline.com/AccountManMenu.aspx\r\n";
            this.LoginDeniedLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LoginDeniedLinkLabel.UseCompatibleTextRendering = true;
            this.LoginDeniedLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LoginDeniedLinkLabel_LinkClicked);
            // 
            // GeneralErrorPage
            // 
            this.GeneralErrorPage.Controls.Add(this.GeneralErrorLabel);
            this.GeneralErrorPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralErrorPage.Location = new System.Drawing.Point(0, 0);
            this.GeneralErrorPage.Name = "GeneralErrorPage";
            this.GeneralErrorPage.Size = new System.Drawing.Size(473, 98);
            this.GeneralErrorPage.TabIndex = 3;
            this.GeneralErrorPage.Text = "generalErrorPage";
            // 
            // GeneralErrorLabel
            // 
            this.GeneralErrorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralErrorLabel.Location = new System.Drawing.Point(0, 0);
            this.GeneralErrorLabel.Name = "GeneralErrorLabel";
            this.GeneralErrorLabel.Padding = new System.Windows.Forms.Padding(50, 0, 50, 0);
            this.GeneralErrorLabel.Size = new System.Drawing.Size(473, 98);
            this.GeneralErrorLabel.TabIndex = 0;
            this.GeneralErrorLabel.Text = "An error occurred while retrieving the information.\r\n\r\nThe error message was: {0}" +
                "";
            this.GeneralErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // APIKeyExpiredErrorPage
            // 
            this.APIKeyExpiredErrorPage.Controls.Add(this.APIKeyExpiredLinkLabel);
            this.APIKeyExpiredErrorPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.APIKeyExpiredErrorPage.Location = new System.Drawing.Point(0, 0);
            this.APIKeyExpiredErrorPage.Name = "APIKeyExpiredErrorPage";
            this.APIKeyExpiredErrorPage.Size = new System.Drawing.Size(473, 98);
            this.APIKeyExpiredErrorPage.TabIndex = 4;
            this.APIKeyExpiredErrorPage.Text = "apiKeyExpiredErrorPage";
            // 
            // APIKeyExpiredLinkLabel
            // 
            this.APIKeyExpiredLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.APIKeyExpiredLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(69, 45);
            this.APIKeyExpiredLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.APIKeyExpiredLinkLabel.Name = "APIKeyExpiredLinkLabel";
            this.APIKeyExpiredLinkLabel.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.APIKeyExpiredLinkLabel.Size = new System.Drawing.Size(473, 98);
            this.APIKeyExpiredLinkLabel.TabIndex = 0;
            this.APIKeyExpiredLinkLabel.TabStop = true;
            this.APIKeyExpiredLinkLabel.Text = "The API key has expired.\r\n\r\nIf this API key is yours, update it at : https://supp" +
                "ort.eveonline.com/api/key/update\r\notherwise contact the API key owner for access" +
                " renewal.";
            this.APIKeyExpiredLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.APIKeyExpiredLinkLabel.UseCompatibleTextRendering = true;
            this.APIKeyExpiredLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.APIKeyExpiredLinkLabel_LinkClicked);
            // 
            // APIKeyExistsErrorPage
            // 
            this.APIKeyExistsErrorPage.Controls.Add(this.APIKeyExistsLabel);
            this.APIKeyExistsErrorPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.APIKeyExistsErrorPage.Location = new System.Drawing.Point(0, 0);
            this.APIKeyExistsErrorPage.Name = "APIKeyExistsErrorPage";
            this.APIKeyExistsErrorPage.Size = new System.Drawing.Size(473, 98);
            this.APIKeyExistsErrorPage.TabIndex = 5;
            this.APIKeyExistsErrorPage.Text = "apiKeyExistsErrorPage";
            // 
            // APIKeyExistsLabel
            // 
            this.APIKeyExistsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.APIKeyExistsLabel.Location = new System.Drawing.Point(0, 0);
            this.APIKeyExistsLabel.Name = "APIKeyExistsLabel";
            this.APIKeyExistsLabel.Padding = new System.Windows.Forms.Padding(35, 0, 0, 0);
            this.APIKeyExistsLabel.Size = new System.Drawing.Size(473, 98);
            this.APIKeyExistsLabel.TabIndex = 0;
            this.APIKeyExistsLabel.Text = "This API key already exists in the API Keys list.\r\n\r\nIf you where trying to updat" +
                "e it, use \'Edit\' in \"Manage API Keys > API Keys tab\",\r\nafter you have selected t" +
                "he API key. ";
            this.APIKeyExistsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CachedWarningPage
            // 
            this.CachedWarningPage.Controls.Add(this.CachedWarningLabel);
            this.CachedWarningPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CachedWarningPage.Location = new System.Drawing.Point(0, 0);
            this.CachedWarningPage.Name = "CachedWarningPage";
            this.CachedWarningPage.Size = new System.Drawing.Size(473, 98);
            this.CachedWarningPage.TabIndex = 6;
            this.CachedWarningPage.Text = "cachedWarningPage";
            // 
            // CachedWarningLabel
            // 
            this.CachedWarningLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CachedWarningLabel.Location = new System.Drawing.Point(0, 0);
            this.CachedWarningLabel.Name = "CachedWarningLabel";
            this.CachedWarningLabel.Size = new System.Drawing.Size(473, 98);
            this.CachedWarningLabel.TabIndex = 0;
            this.CachedWarningLabel.Text = "Due to the fact that the cached timer has not yet expired,\r\nyour query attempt wi" +
                "ll result in getting the same data you already have.";
            this.CachedWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NoCorpFeaturesYetPage
            // 
            this.NoCorpFeaturesYetPage.Controls.Add(this.NoCorpFeaturesYetLabel);
            this.NoCorpFeaturesYetPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NoCorpFeaturesYetPage.Location = new System.Drawing.Point(0, 0);
            this.NoCorpFeaturesYetPage.Name = "NoCorpFeaturesYetPage";
            this.NoCorpFeaturesYetPage.Size = new System.Drawing.Size(473, 98);
            this.NoCorpFeaturesYetPage.TabIndex = 6;
            this.NoCorpFeaturesYetPage.Text = "noCorpFeaturesYetPage";
            // 
            // NoCorpFeaturesYetLabel
            // 
            this.NoCorpFeaturesYetLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NoCorpFeaturesYetLabel.Location = new System.Drawing.Point(0, 0);
            this.NoCorpFeaturesYetLabel.Name = "NoCorpFeaturesYetLabel";
            this.NoCorpFeaturesYetLabel.Size = new System.Drawing.Size(473, 98);
            this.NoCorpFeaturesYetLabel.TabIndex = 0;
            this.NoCorpFeaturesYetLabel.Text = "EVEMon does not support any Corporation feature yet.\r\nAdding such an API key has " +
                "no meaning.\r\nSupport for Corporation features may happen in the near future, so " +
                "stay tuned.";
            this.NoCorpFeaturesYetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ApiKeyUpdateOrAdditionWindow
            // 
            this.AcceptButton = this.ButtonNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(503, 212);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonPrevious);
            this.Controls.Add(this.ButtonNext);
            this.Controls.Add(this.MultiPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApiKeyUpdateOrAdditionWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "API Key Importation";
            this.TopMost = true;
            this.MultiPanel.ResumeLayout(false);
            this.CredentialsPage.ResumeLayout(false);
            this.CredentialsPage.PerformLayout();
            this.WaitingPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Throbber)).EndInit();
            this.ResultPage.ResumeLayout(false);
            this.ResultPage.PerformLayout();
            this.KeyTableLayoutPanel.ResumeLayout(false);
            this.KeyTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.KeyPicture)).EndInit();
            this.CharactersGroupBox.ResumeLayout(false);
            this.ResultsMultiPanel.ResumeLayout(false);
            this.CharactersListPage.ResumeLayout(false);
            this.CharactersListPage.PerformLayout();
            this.AuthenticationErrorPage.ResumeLayout(false);
            this.LoginDeniedErrorPage.ResumeLayout(false);
            this.GeneralErrorPage.ResumeLayout(false);
            this.APIKeyExpiredErrorPage.ResumeLayout(false);
            this.APIKeyExistsErrorPage.ResumeLayout(false);
            this.CachedWarningPage.ResumeLayout(false);
            this.NoCorpFeaturesYetPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MultiPanel MultiPanel;
        private MultiPanelPage CredentialsPage;
        private System.Windows.Forms.TextBox VerificationCodeTextBox;
        private System.Windows.Forms.TextBox IDTextBox;
        private System.Windows.Forms.Label VerificationCodeLabel;
        private System.Windows.Forms.Label IDLabel;
        private System.Windows.Forms.Button ButtonNext;
        private System.Windows.Forms.Button ButtonPrevious;
        private System.Windows.Forms.Button ButtonCancel;
        private MultiPanelPage WaitingPage;
        private Throbber Throbber;
        private MultiPanelPage ResultPage;
        private System.Windows.Forms.PictureBox KeyPicture;
        private System.Windows.Forms.GroupBox CharactersGroupBox;
        private System.Windows.Forms.ListView CharactersListView;
        private System.Windows.Forms.Label WarningLabel;
        private System.Windows.Forms.TableLayoutPanel KeyTableLayoutPanel;
        private System.Windows.Forms.Label KeyLabel;
        private MultiPanel ResultsMultiPanel;
        private MultiPanelPage CharactersListPage;
        private MultiPanelPage AuthenticationErrorPage;
        private System.Windows.Forms.Label AuthenticationErrorGuideLabel;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private MultiPanelPage LoginDeniedErrorPage;
        private System.Windows.Forms.LinkLabel LoginDeniedLinkLabel;
        private MultiPanelPage GeneralErrorPage;
        private System.Windows.Forms.Label GeneralErrorLabel;
        private MultiPanelPage APIKeyExpiredErrorPage;
        private System.Windows.Forms.LinkLabel APIKeyExpiredLinkLabel;
        private MultiPanelPage APIKeyExistsErrorPage;
        private System.Windows.Forms.Label APIKeyExistsLabel;
        private MultiPanelPage CachedWarningPage;
        private System.Windows.Forms.Label CachedWarningLabel;
        private MultiPanelPage NoCorpFeaturesYetPage;
        private System.Windows.Forms.Label NoCorpFeaturesYetLabel;
    }
}