using System;
namespace EVEMon.SettingsUI
{
    partial class SettingsForm
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
            System.Windows.Forms.Panel bottomPanel;
            System.Windows.Forms.Label label31;
            System.Windows.Forms.GroupBox groupBox15;
            System.Windows.Forms.GroupBox groupBox7;
            System.Windows.Forms.Label label29;
            System.Windows.Forms.GroupBox groupBox10;
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node3");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node4");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node5");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Node6");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node7");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node8");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8});
            System.Windows.Forms.Label label16;
            System.Windows.Forms.Label label13;
            System.Windows.Forms.Label label12;
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label lblPortNumber;
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Label label19;
            System.Windows.Forms.Label label24;
            System.Windows.Forms.Label label20;
            System.Windows.Forms.Label label21;
            System.Windows.Forms.Label label22;
            System.Windows.Forms.Label lblReminder;
            System.Windows.Forms.Label lblURI;
            System.Windows.Forms.Label lblPassword;
            System.Windows.Forms.Label lblGoogleEmail;
            System.Windows.Forms.Label lblEarlyReminder;
            System.Windows.Forms.Label lblLateReminder;
            System.Windows.Forms.Label label25;
            System.Windows.Forms.Label label27;
            System.Windows.Forms.Label label14;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label30;
            System.Windows.Forms.Label label23;
            System.Windows.Forms.FlowLayoutPanel flowLayoutPanel28;
            System.Windows.Forms.Label igbHelpLabel;
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Updates", 5, 5);
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Network", 8, 8);
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Logitech Keyboards", 12, 12);
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("IGB Server", 11, 11);
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Relocation");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("General", 10, 10, new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14});
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Main Window", 6, 6);
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Skill Planner", 7, 7);
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("System Tray Icon", 9, 9);
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("External Calendar", 13, 13);
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Scheduler", 1, 1, new System.Windows.Forms.TreeNode[] {
            treeNode19});
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Skills Completion Mails", 4, 4);
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Notifications", 3, 3, new System.Windows.Forms.TreeNode[] {
            treeNode21});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.applyButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.cbColorQueuedSkills = new System.Windows.Forms.CheckBox();
            this.cbShowPrereqMetSkills = new System.Windows.Forms.CheckBox();
            this.cbColorPartialSkills = new System.Windows.Forms.CheckBox();
            this.cbAlwaysShowSkillQueueTime = new System.Windows.Forms.CheckBox();
            this.cbShowNonPublicSkills = new System.Windows.Forms.CheckBox();
            this.cbShowAllPublicSkills = new System.Windows.Forms.CheckBox();
            this.cbWindowsTitleList = new System.Windows.Forms.ComboBox();
            this.cbSkillInTitle = new System.Windows.Forms.CheckBox();
            this.cbTitleToTime = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.cbSkillIconSet = new System.Windows.Forms.ComboBox();
            this.tvlist = new System.Windows.Forms.TreeView();
            this.rbSystemTrayOptionsNever = new System.Windows.Forms.RadioButton();
            this.rbSystemTrayOptionsAlways = new System.Windows.Forms.RadioButton();
            this.rbSystemTrayOptionsMinimized = new System.Windows.Forms.RadioButton();
            this.igbUrlTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.ttToolTipCodes = new System.Windows.Forms.ToolTip(this.components);
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.leftPanel = new System.Windows.Forms.Panel();
            this.multiPanel = new EVEMon.Controls.MultiPanel();
            this.mainWindowPage = new EVEMon.Controls.MultiPanelPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.overviewPanel = new System.Windows.Forms.Panel();
            this.overviewShowSkillQueueFreeRoomCheckBox = new System.Windows.Forms.CheckBox();
            this.overviewShowWalletCheckBox = new System.Windows.Forms.CheckBox();
            this.overviewShowPortraitCheckBox = new System.Windows.Forms.CheckBox();
            this.overviewPortraitSizeComboBox = new System.Windows.Forms.ComboBox();
            this.cbShowOverViewTab = new System.Windows.Forms.CheckBox();
            this.generalPage = new EVEMon.Controls.MultiPanelPage();
            this.cbWorksafeMode = new System.Windows.Forms.CheckBox();
            this.compatibilityCombo = new System.Windows.Forms.ComboBox();
            this.runAtStartupComboBox = new System.Windows.Forms.CheckBox();
            this.skillPlannerPage = new EVEMon.Controls.MultiPanelPage();
            this.SummaryOnMultiSelectOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.cbHighlightQueuedSiklls = new System.Windows.Forms.CheckBox();
            this.cbHighlightPartialSkills = new System.Windows.Forms.CheckBox();
            this.cbHighlightConflicts = new System.Windows.Forms.CheckBox();
            this.cbHighlightPrerequisites = new System.Windows.Forms.CheckBox();
            this.cbHighlightPlannedSkills = new System.Windows.Forms.CheckBox();
            this.networkPage = new EVEMon.Controls.MultiPanelPage();
            this.ApiProxyGroupBox = new System.Windows.Forms.GroupBox();
            this.btnDeleteAPIServer = new System.Windows.Forms.Button();
            this.btnAddAPIServer = new System.Windows.Forms.Button();
            this.cbAPIServer = new System.Windows.Forms.ComboBox();
            this.btnEditAPIServer = new System.Windows.Forms.Button();
            this.ProxyServerGroupBox = new System.Windows.Forms.GroupBox();
            this.customProxyCheckBox = new System.Windows.Forms.CheckBox();
            this.customProxyPanel = new System.Windows.Forms.Panel();
            this.proxyPortTextBox = new System.Windows.Forms.TextBox();
            this.proxyAuthenticationButton = new System.Windows.Forms.Button();
            this.proxyHttpHostTextBox = new System.Windows.Forms.TextBox();
            this.emailNotificationsPage = new EVEMon.Controls.MultiPanelPage();
            this.mailNotificationPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.cbEmailUseShortFormat = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpEmailSettings = new System.Windows.Forms.TableLayoutPanel();
            this.tbMailServer = new System.Windows.Forms.TextBox();
            this.tbFromAddress = new System.Windows.Forms.TextBox();
            this.tbToAddress = new System.Windows.Forms.TextBox();
            this.cbEmailServerRequireSsl = new System.Windows.Forms.CheckBox();
            this.cbEmailAuthRequired = new System.Windows.Forms.CheckBox();
            this.tlpEmailAuthTable = new System.Windows.Forms.TableLayoutPanel();
            this.tbEmailUsername = new System.Windows.Forms.TextBox();
            this.tbEmailPassword = new System.Windows.Forms.TextBox();
            this.emailPortTextBox = new System.Windows.Forms.TextBox();
            this.btnTestEmail = new System.Windows.Forms.Button();
            this.mailNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.notificationsPage = new EVEMon.Controls.MultiPanelPage();
            this.notificationsControl = new EVEMon.SettingsUI.NotificationsControl();
            this.cbPlaySoundOnSkillComplete = new System.Windows.Forms.CheckBox();
            this.trayIconPage = new EVEMon.Controls.MultiPanelPage();
            this.WindowBehaviourGroupBox = new System.Windows.Forms.GroupBox();
            this.rbMinToTaskBar = new System.Windows.Forms.RadioButton();
            this.rbMinToTray = new System.Windows.Forms.RadioButton();
            this.rbExitEVEMon = new System.Windows.Forms.RadioButton();
            this.trayIconPopupGroupBox = new System.Windows.Forms.GroupBox();
            this.trayPopupButton = new System.Windows.Forms.Button();
            this.trayPopupRadio = new System.Windows.Forms.RadioButton();
            this.trayTooltipRadio = new System.Windows.Forms.RadioButton();
            this.trayTooltipButton = new System.Windows.Forms.Button();
            this.updatesPage = new EVEMon.Controls.MultiPanelPage();
            this.updateSettingsControl = new EVEMon.SettingsUI.UpdateSettingsControl();
            this.label18 = new System.Windows.Forms.Label();
            this.cbCheckTimeOnStartup = new System.Windows.Forms.CheckBox();
            this.cbAutomaticallySearchForNewVersions = new System.Windows.Forms.CheckBox();
            this.schedulerUIPage = new EVEMon.Controls.MultiPanelPage();
            this.panelColorText = new System.Windows.Forms.Panel();
            this.panelColorRecurring2 = new System.Windows.Forms.Panel();
            this.panelColorRecurring1 = new System.Windows.Forms.Panel();
            this.panelColorSingle2 = new System.Windows.Forms.Panel();
            this.panelColorSingle1 = new System.Windows.Forms.Panel();
            this.panelColorBlocking = new System.Windows.Forms.Panel();
            this.externalCalendarPage = new EVEMon.Controls.MultiPanelPage();
            this.externalCalendarPanel = new System.Windows.Forms.Panel();
            this.rbMSOutlook = new System.Windows.Forms.RadioButton();
            this.dtpLateReminder = new System.Windows.Forms.DateTimePicker();
            this.tbReminder = new System.Windows.Forms.TextBox();
            this.dtpEarlyReminder = new System.Windows.Forms.DateTimePicker();
            this.gbGoogle = new System.Windows.Forms.GroupBox();
            this.cbGoogleReminder = new System.Windows.Forms.ComboBox();
            this.tbGoogleURI = new System.Windows.Forms.TextBox();
            this.tbGooglePassword = new System.Windows.Forms.TextBox();
            this.tbGoogleEmail = new System.Windows.Forms.TextBox();
            this.rbGoogle = new System.Windows.Forms.RadioButton();
            this.cbUseAlterateReminder = new System.Windows.Forms.CheckBox();
            this.cbSetReminder = new System.Windows.Forms.CheckBox();
            this.externalCalendarCheckbox = new System.Windows.Forms.CheckBox();
            this.g15Page = new EVEMon.Controls.MultiPanelPage();
            this.g15CheckBox = new System.Windows.Forms.CheckBox();
            this.g15Panel = new System.Windows.Forms.Panel();
            this.cbG15ShowTime = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cbG15CycleTimes = new System.Windows.Forms.CheckBox();
            this.ACycleTimesInterval = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbG15ACycle = new System.Windows.Forms.CheckBox();
            this.ACycleInterval = new System.Windows.Forms.NumericUpDown();
            this.igbServerPage = new EVEMon.Controls.MultiPanelPage();
            this.igbCheckBox = new System.Windows.Forms.CheckBox();
            this.igbFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel27 = new System.Windows.Forms.FlowLayoutPanel();
            this.igbPortTextBox = new System.Windows.Forms.TextBox();
            this.cbIGBPublic = new System.Windows.Forms.CheckBox();
            this.relocationPage = new EVEMon.Controls.MultiPanelPage();
            this.relocationCheckEveryLabel = new System.Windows.Forms.Label();
            this.relocationSecondsLabel = new System.Windows.Forms.Label();
            this.relocationSecondsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.enableAutomaticRelocationCheckBox = new System.Windows.Forms.CheckBox();
            this.showRelocationMenuCheckbox = new System.Windows.Forms.CheckBox();
            bottomPanel = new System.Windows.Forms.Panel();
            label31 = new System.Windows.Forms.Label();
            groupBox15 = new System.Windows.Forms.GroupBox();
            groupBox7 = new System.Windows.Forms.GroupBox();
            label29 = new System.Windows.Forms.Label();
            groupBox10 = new System.Windows.Forms.GroupBox();
            label16 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            lblPortNumber = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label19 = new System.Windows.Forms.Label();
            label24 = new System.Windows.Forms.Label();
            label20 = new System.Windows.Forms.Label();
            label21 = new System.Windows.Forms.Label();
            label22 = new System.Windows.Forms.Label();
            lblReminder = new System.Windows.Forms.Label();
            lblURI = new System.Windows.Forms.Label();
            lblPassword = new System.Windows.Forms.Label();
            lblGoogleEmail = new System.Windows.Forms.Label();
            lblEarlyReminder = new System.Windows.Forms.Label();
            lblLateReminder = new System.Windows.Forms.Label();
            label25 = new System.Windows.Forms.Label();
            label27 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label30 = new System.Windows.Forms.Label();
            label23 = new System.Windows.Forms.Label();
            flowLayoutPanel28 = new System.Windows.Forms.FlowLayoutPanel();
            igbHelpLabel = new System.Windows.Forms.Label();
            bottomPanel.SuspendLayout();
            groupBox15.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox10.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            groupBox1.SuspendLayout();
            flowLayoutPanel28.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.multiPanel.SuspendLayout();
            this.mainWindowPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.overviewPanel.SuspendLayout();
            this.generalPage.SuspendLayout();
            this.skillPlannerPage.SuspendLayout();
            this.networkPage.SuspendLayout();
            this.ApiProxyGroupBox.SuspendLayout();
            this.ProxyServerGroupBox.SuspendLayout();
            this.customProxyPanel.SuspendLayout();
            this.emailNotificationsPage.SuspendLayout();
            this.mailNotificationPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tlpEmailSettings.SuspendLayout();
            this.tlpEmailAuthTable.SuspendLayout();
            this.notificationsPage.SuspendLayout();
            this.trayIconPage.SuspendLayout();
            this.WindowBehaviourGroupBox.SuspendLayout();
            this.trayIconPopupGroupBox.SuspendLayout();
            this.updatesPage.SuspendLayout();
            this.schedulerUIPage.SuspendLayout();
            this.externalCalendarPage.SuspendLayout();
            this.externalCalendarPanel.SuspendLayout();
            this.gbGoogle.SuspendLayout();
            this.g15Page.SuspendLayout();
            this.g15Panel.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ACycleTimesInterval)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ACycleInterval)).BeginInit();
            this.igbServerPage.SuspendLayout();
            this.igbFlowPanel.SuspendLayout();
            this.flowLayoutPanel27.SuspendLayout();
            this.relocationPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.relocationSecondsNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            bottomPanel.Controls.Add(this.applyButton);
            bottomPanel.Controls.Add(this.okButton);
            bottomPanel.Controls.Add(this.cancelButton);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 436);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(644, 46);
            bottomPanel.TabIndex = 8;
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(557, 11);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "&Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(395, 11);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(476, 11);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new System.Drawing.Point(20, 52);
            label31.Name = "label31";
            label31.Size = new System.Drawing.Size(26, 13);
            label31.TabIndex = 31;
            label31.Text = "Size";
            // 
            // groupBox15
            // 
            groupBox15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox15.Controls.Add(this.cbColorQueuedSkills);
            groupBox15.Controls.Add(this.cbShowPrereqMetSkills);
            groupBox15.Controls.Add(this.cbColorPartialSkills);
            groupBox15.Controls.Add(this.cbAlwaysShowSkillQueueTime);
            groupBox15.Controls.Add(this.cbShowNonPublicSkills);
            groupBox15.Controls.Add(this.cbShowAllPublicSkills);
            groupBox15.Location = new System.Drawing.Point(3, 177);
            groupBox15.Name = "groupBox15";
            groupBox15.Size = new System.Drawing.Size(426, 85);
            groupBox15.TabIndex = 7;
            groupBox15.TabStop = false;
            groupBox15.Text = "Character Monitor";
            // 
            // cbColorQueuedSkills
            // 
            this.cbColorQueuedSkills.AutoSize = true;
            this.cbColorQueuedSkills.Location = new System.Drawing.Point(188, 38);
            this.cbColorQueuedSkills.Name = "cbColorQueuedSkills";
            this.cbColorQueuedSkills.Size = new System.Drawing.Size(133, 17);
            this.cbColorQueuedSkills.TabIndex = 13;
            this.cbColorQueuedSkills.Text = "Highlight Queued Skills";
            this.cbColorQueuedSkills.UseVisualStyleBackColor = true;
            // 
            // cbShowPrereqMetSkills
            // 
            this.cbShowPrereqMetSkills.AutoSize = true;
            this.cbShowPrereqMetSkills.Location = new System.Drawing.Point(15, 57);
            this.cbShowPrereqMetSkills.Name = "cbShowPrereqMetSkills";
            this.cbShowPrereqMetSkills.Size = new System.Drawing.Size(157, 17);
            this.cbShowPrereqMetSkills.TabIndex = 12;
            this.cbShowPrereqMetSkills.Text = "Show Also Prereq-Met Skills";
            this.cbShowPrereqMetSkills.UseVisualStyleBackColor = true;
            // 
            // cbColorPartialSkills
            // 
            this.cbColorPartialSkills.AutoSize = true;
            this.cbColorPartialSkills.Location = new System.Drawing.Point(188, 20);
            this.cbColorPartialSkills.Name = "cbColorPartialSkills";
            this.cbColorPartialSkills.Size = new System.Drawing.Size(172, 17);
            this.cbColorPartialSkills.TabIndex = 11;
            this.cbColorPartialSkills.Text = "Highlight Partially Trained Skills";
            this.cbColorPartialSkills.UseVisualStyleBackColor = true;
            // 
            // cbAlwaysShowSkillQueueTime
            // 
            this.cbAlwaysShowSkillQueueTime.AutoSize = true;
            this.cbAlwaysShowSkillQueueTime.Location = new System.Drawing.Point(188, 57);
            this.cbAlwaysShowSkillQueueTime.Name = "cbAlwaysShowSkillQueueTime";
            this.cbAlwaysShowSkillQueueTime.Size = new System.Drawing.Size(215, 17);
            this.cbAlwaysShowSkillQueueTime.TabIndex = 2;
            this.cbAlwaysShowSkillQueueTime.Text = "Always show time above the skill queue";
            this.cbAlwaysShowSkillQueueTime.UseVisualStyleBackColor = true;
            // 
            // cbShowNonPublicSkills
            // 
            this.cbShowNonPublicSkills.AutoSize = true;
            this.cbShowNonPublicSkills.Enabled = false;
            this.cbShowNonPublicSkills.Location = new System.Drawing.Point(15, 38);
            this.cbShowNonPublicSkills.Name = "cbShowNonPublicSkills";
            this.cbShowNonPublicSkills.Size = new System.Drawing.Size(153, 17);
            this.cbShowNonPublicSkills.TabIndex = 1;
            this.cbShowNonPublicSkills.Text = "Show Also Non-Public Skills";
            this.cbShowNonPublicSkills.UseVisualStyleBackColor = true;
            // 
            // cbShowAllPublicSkills
            // 
            this.cbShowAllPublicSkills.AutoSize = true;
            this.cbShowAllPublicSkills.Location = new System.Drawing.Point(15, 20);
            this.cbShowAllPublicSkills.Name = "cbShowAllPublicSkills";
            this.cbShowAllPublicSkills.Size = new System.Drawing.Size(144, 17);
            this.cbShowAllPublicSkills.TabIndex = 0;
            this.cbShowAllPublicSkills.Text = "Show Also All Public Skills";
            this.cbShowAllPublicSkills.UseVisualStyleBackColor = true;
            this.cbShowAllPublicSkills.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // groupBox7
            // 
            groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox7.Controls.Add(this.cbWindowsTitleList);
            groupBox7.Controls.Add(this.cbSkillInTitle);
            groupBox7.Controls.Add(this.cbTitleToTime);
            groupBox7.Location = new System.Drawing.Point(3, 61);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new System.Drawing.Size(426, 105);
            groupBox7.TabIndex = 14;
            groupBox7.TabStop = false;
            groupBox7.Text = "Window Title";
            // 
            // cbWindowsTitleList
            // 
            this.cbWindowsTitleList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWindowsTitleList.FormattingEnabled = true;
            this.cbWindowsTitleList.Items.AddRange(new object[] {
            "Single character - finishing skill next",
            "Single character - selected character",
            "Multi character - finishing skill next first",
            "Multi character - selected character first "});
            this.cbWindowsTitleList.Location = new System.Drawing.Point(15, 66);
            this.cbWindowsTitleList.Name = "cbWindowsTitleList";
            this.cbWindowsTitleList.Size = new System.Drawing.Size(224, 21);
            this.cbWindowsTitleList.TabIndex = 1;
            // 
            // cbSkillInTitle
            // 
            this.cbSkillInTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.cbSkillInTitle.AutoSize = true;
            this.cbSkillInTitle.Location = new System.Drawing.Point(15, 43);
            this.cbSkillInTitle.Name = "cbSkillInTitle";
            this.cbSkillInTitle.Size = new System.Drawing.Size(121, 17);
            this.cbSkillInTitle.TabIndex = 7;
            this.cbSkillInTitle.Text = "Show skill in training";
            this.cbSkillInTitle.UseVisualStyleBackColor = true;
            // 
            // cbTitleToTime
            // 
            this.cbTitleToTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.cbTitleToTime.AutoSize = true;
            this.cbTitleToTime.Location = new System.Drawing.Point(15, 20);
            this.cbTitleToTime.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.cbTitleToTime.Name = "cbTitleToTime";
            this.cbTitleToTime.Size = new System.Drawing.Size(193, 17);
            this.cbTitleToTime.TabIndex = 6;
            this.cbTitleToTime.Text = "Show character info in window title";
            this.cbTitleToTime.UseVisualStyleBackColor = true;
            this.cbTitleToTime.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new System.Drawing.Point(3, 181);
            label29.Name = "label29";
            label29.Size = new System.Drawing.Size(222, 13);
            label29.TabIndex = 1;
            label29.Text = "Environment (requires restart to take effect)";
            // 
            // groupBox10
            // 
            groupBox10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox10.Controls.Add(this.tableLayoutPanel4);
            groupBox10.Location = new System.Drawing.Point(1, 126);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new System.Drawing.Size(435, 218);
            groupBox10.TabIndex = 13;
            groupBox10.TabStop = false;
            groupBox10.Text = "Skill Browser Icon Set";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.cbSkillIconSet, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tvlist, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(429, 198);
            this.tableLayoutPanel4.TabIndex = 15;
            // 
            // cbSkillIconSet
            // 
            this.cbSkillIconSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbSkillIconSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSkillIconSet.FormattingEnabled = true;
            this.cbSkillIconSet.Location = new System.Drawing.Point(3, 3);
            this.cbSkillIconSet.Name = "cbSkillIconSet";
            this.cbSkillIconSet.Size = new System.Drawing.Size(423, 21);
            this.cbSkillIconSet.TabIndex = 3;
            this.cbSkillIconSet.SelectedIndexChanged += new System.EventHandler(this.skillIconSetComboBox_SelectedIndexChanged);
            // 
            // tvlist
            // 
            this.tvlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvlist.Location = new System.Drawing.Point(3, 30);
            this.tvlist.Name = "tvlist";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            treeNode2.Name = "Node2";
            treeNode2.Text = "Node2";
            treeNode3.Name = "Node3";
            treeNode3.Text = "Node3";
            treeNode4.Name = "Node4";
            treeNode4.Text = "Node4";
            treeNode5.Name = "Node5";
            treeNode5.Text = "Node5";
            treeNode6.Name = "Node6";
            treeNode6.Text = "Node6";
            treeNode7.Name = "Node7";
            treeNode7.Text = "Node7";
            treeNode8.Name = "Node8";
            treeNode8.Text = "Node8";
            treeNode9.Name = "Node0";
            treeNode9.Text = "Node0";
            this.tvlist.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode9});
            this.tvlist.Size = new System.Drawing.Size(423, 165);
            this.tvlist.TabIndex = 9;
            // 
            // label16
            // 
            label16.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label16.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            label16.Location = new System.Drawing.Point(12, 17);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(374, 29);
            label16.TabIndex = 8;
            label16.Text = "By default, EVEMon queries CCP for the API data. You can implement your own provi" +
                "der and make EVEMon uses it.";
            // 
            // label13
            // 
            label13.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label13.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            label13.Location = new System.Drawing.Point(9, 17);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(382, 32);
            label13.TabIndex = 8;
            label13.Text = "By default, EVEMon will use the same Proxy settings than Internet Explorer (can b" +
                "e configured through the Control Panel).";
            // 
            // label12
            // 
            label12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label12.Location = new System.Drawing.Point(50, 8);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(165, 13);
            label12.TabIndex = 3;
            label12.Text = "Host/IP Address";
            label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label11
            // 
            label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label11.Location = new System.Drawing.Point(221, 8);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(39, 13);
            label11.TabIndex = 4;
            label11.Text = "Port";
            label11.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(8, 27);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(36, 13);
            label10.TabIndex = 0;
            label10.Text = "HTTP:";
            label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 27);
            label1.TabIndex = 0;
            label1.Text = "Email Server:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 162);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(76, 27);
            label2.TabIndex = 1;
            label2.Text = "From address:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(15, 189);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(64, 27);
            label3.TabIndex = 2;
            label3.Text = "To address:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(5, 27);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(57, 27);
            label5.TabIndex = 8;
            label5.Text = "Password:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(59, 27);
            label4.TabIndex = 7;
            label4.Text = "Username:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPortNumber
            // 
            lblPortNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            lblPortNumber.AutoSize = true;
            lblPortNumber.Location = new System.Drawing.Point(8, 27);
            lblPortNumber.Name = "lblPortNumber";
            lblPortNumber.Size = new System.Drawing.Size(71, 27);
            lblPortNumber.TabIndex = 10;
            lblPortNumber.Text = "Port Number:";
            lblPortNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this.rbSystemTrayOptionsNever);
            groupBox1.Controls.Add(this.rbSystemTrayOptionsAlways);
            groupBox1.Controls.Add(this.rbSystemTrayOptionsMinimized);
            groupBox1.Location = new System.Drawing.Point(9, 29);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(419, 100);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "Show System Tray Icon";
            // 
            // rbSystemTrayOptionsNever
            // 
            this.rbSystemTrayOptionsNever.AutoSize = true;
            this.rbSystemTrayOptionsNever.Location = new System.Drawing.Point(6, 20);
            this.rbSystemTrayOptionsNever.Name = "rbSystemTrayOptionsNever";
            this.rbSystemTrayOptionsNever.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.rbSystemTrayOptionsNever.Size = new System.Drawing.Size(57, 17);
            this.rbSystemTrayOptionsNever.TabIndex = 1;
            this.rbSystemTrayOptionsNever.TabStop = true;
            this.rbSystemTrayOptionsNever.Tag = "";
            this.rbSystemTrayOptionsNever.Text = "Never";
            this.rbSystemTrayOptionsNever.UseVisualStyleBackColor = true;
            this.rbSystemTrayOptionsNever.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // rbSystemTrayOptionsAlways
            // 
            this.rbSystemTrayOptionsAlways.AutoSize = true;
            this.rbSystemTrayOptionsAlways.Location = new System.Drawing.Point(6, 66);
            this.rbSystemTrayOptionsAlways.Name = "rbSystemTrayOptionsAlways";
            this.rbSystemTrayOptionsAlways.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.rbSystemTrayOptionsAlways.Size = new System.Drawing.Size(62, 17);
            this.rbSystemTrayOptionsAlways.TabIndex = 3;
            this.rbSystemTrayOptionsAlways.TabStop = true;
            this.rbSystemTrayOptionsAlways.Tag = "";
            this.rbSystemTrayOptionsAlways.Text = "Always";
            this.rbSystemTrayOptionsAlways.UseVisualStyleBackColor = true;
            // 
            // rbSystemTrayOptionsMinimized
            // 
            this.rbSystemTrayOptionsMinimized.AutoSize = true;
            this.rbSystemTrayOptionsMinimized.Location = new System.Drawing.Point(6, 43);
            this.rbSystemTrayOptionsMinimized.Name = "rbSystemTrayOptionsMinimized";
            this.rbSystemTrayOptionsMinimized.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.rbSystemTrayOptionsMinimized.Size = new System.Drawing.Size(104, 17);
            this.rbSystemTrayOptionsMinimized.TabIndex = 2;
            this.rbSystemTrayOptionsMinimized.TabStop = true;
            this.rbSystemTrayOptionsMinimized.Tag = "";
            this.rbSystemTrayOptionsMinimized.Text = "When Minimized";
            this.rbSystemTrayOptionsMinimized.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            label19.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label19.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            label19.Location = new System.Drawing.Point(6, 34);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(422, 45);
            label19.TabIndex = 6;
            label19.Text = "Select the colors used in the scheduler. Using the scheduler, EVEMon can warn you" +
                " about skill that will complete at times you will be away from your computer.";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new System.Drawing.Point(6, 109);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(33, 13);
            label24.TabIndex = 5;
            label24.Text = "Text:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new System.Drawing.Point(6, 132);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(85, 13);
            label20.TabIndex = 0;
            label20.Text = "Blocking Events:";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new System.Drawing.Point(6, 180);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(93, 13);
            label21.TabIndex = 1;
            label21.Text = "Recurring Events:";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(6, 157);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(77, 13);
            label22.TabIndex = 2;
            label22.Text = "Simple Events:";
            // 
            // lblReminder
            // 
            lblReminder.AutoSize = true;
            lblReminder.Location = new System.Drawing.Point(6, 107);
            lblReminder.Name = "lblReminder";
            lblReminder.Size = new System.Drawing.Size(56, 13);
            lblReminder.TabIndex = 6;
            lblReminder.Text = "Reminder:";
            // 
            // lblURI
            // 
            lblURI.AutoSize = true;
            lblURI.Location = new System.Drawing.Point(6, 80);
            lblURI.Name = "lblURI";
            lblURI.Size = new System.Drawing.Size(29, 13);
            lblURI.TabIndex = 5;
            lblURI.Text = "URI:";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(6, 52);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(57, 13);
            lblPassword.TabIndex = 1;
            lblPassword.Text = "Password:";
            // 
            // lblGoogleEmail
            // 
            lblGoogleEmail.AutoSize = true;
            lblGoogleEmail.Location = new System.Drawing.Point(6, 24);
            lblGoogleEmail.Name = "lblGoogleEmail";
            lblGoogleEmail.Size = new System.Drawing.Size(71, 13);
            lblGoogleEmail.TabIndex = 0;
            lblGoogleEmail.Text = "Google Email:";
            // 
            // lblEarlyReminder
            // 
            lblEarlyReminder.AutoSize = true;
            lblEarlyReminder.Location = new System.Drawing.Point(30, 228);
            lblEarlyReminder.Name = "lblEarlyReminder";
            lblEarlyReminder.Size = new System.Drawing.Size(83, 13);
            lblEarlyReminder.TabIndex = 4;
            lblEarlyReminder.Text = "Early Reminder:";
            // 
            // lblLateReminder
            // 
            lblLateReminder.AutoSize = true;
            lblLateReminder.Location = new System.Drawing.Point(195, 228);
            lblLateReminder.Name = "lblLateReminder";
            lblLateReminder.Size = new System.Drawing.Size(80, 13);
            lblLateReminder.TabIndex = 6;
            lblLateReminder.Text = "Late Reminder:";
            // 
            // label25
            // 
            label25.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label25.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            label25.Location = new System.Drawing.Point(3, 32);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(429, 35);
            label25.TabIndex = 11;
            label25.Text = "EVEMon can import scheduler entries from Outlook or Google calendars to emphasize" +
                " the skills that will complete at times you won\'t be available.";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            label27.Location = new System.Drawing.Point(3, 40);
            label27.Name = "label27";
            label27.Size = new System.Drawing.Size(339, 13);
            label27.TabIndex = 5;
            label27.Text = "EVEMon supports the LCD display of the Logitech G15/G19 keyboard.";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(273, 6);
            label14.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(46, 13);
            label14.TabIndex = 9;
            label14.Text = "seconds";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(166, 6);
            label9.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(46, 13);
            label9.TabIndex = 6;
            label9.Text = "seconds";
            // 
            // label30
            // 
            label30.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label30.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            label30.Location = new System.Drawing.Point(4, 34);
            label30.Name = "label30";
            label30.Size = new System.Drawing.Size(424, 48);
            label30.TabIndex = 5;
            label30.Text = "When this option is checked and EVEMon running, you can open the in-game browser " +
                "and type the address provided below to give a quick look at your plans and the s" +
                "killbooks you need to buy.\r\n";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Dock = System.Windows.Forms.DockStyle.Left;
            label23.Location = new System.Drawing.Point(3, 0);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(51, 27);
            label23.TabIndex = 6;
            label23.Text = "IGB Port:";
            label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel28
            // 
            flowLayoutPanel28.Controls.Add(igbHelpLabel);
            flowLayoutPanel28.Controls.Add(this.igbUrlTextBox);
            flowLayoutPanel28.Location = new System.Drawing.Point(12, 37);
            flowLayoutPanel28.Name = "flowLayoutPanel28";
            flowLayoutPanel28.Size = new System.Drawing.Size(334, 22);
            flowLayoutPanel28.TabIndex = 11;
            // 
            // igbHelpLabel
            // 
            igbHelpLabel.AutoSize = true;
            igbHelpLabel.Location = new System.Drawing.Point(3, 3);
            igbHelpLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            igbHelpLabel.Name = "igbHelpLabel";
            igbHelpLabel.Size = new System.Drawing.Size(188, 13);
            igbHelpLabel.TabIndex = 8;
            igbHelpLabel.Text = "Open the in-game browser and type :";
            // 
            // igbUrlTextBox
            // 
            this.igbUrlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.igbUrlTextBox.Location = new System.Drawing.Point(197, 3);
            this.igbUrlTextBox.Name = "igbUrlTextBox";
            this.igbUrlTextBox.ReadOnly = true;
            this.igbUrlTextBox.Size = new System.Drawing.Size(100, 14);
            this.igbUrlTextBox.TabIndex = 10;
            this.igbUrlTextBox.Text = "http://localhost:80/";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 40);
            this.label6.TabIndex = 8;
            this.label6.Text = "Server Password:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Email Server:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 26);
            this.label8.TabIndex = 1;
            this.label8.Text = "From address:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.AutoSize = true;
            this.flowLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel9.Controls.Add(this.checkBox1);
            this.flowLayoutPanel9.Controls.Add(this.checkBox2);
            this.flowLayoutPanel9.Controls.Add(this.checkBox3);
            this.flowLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel9.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel9.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Padding = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this.flowLayoutPanel9.Size = new System.Drawing.Size(356, 70);
            this.flowLayoutPanel9.TabIndex = 3;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(102, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Minimize to Tray";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(12, 26);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(171, 17);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "Set window title to training time";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(12, 49);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(264, 17);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "Run in \"safe for work\" mode (no portraits or colors)";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // chName
            // 
            this.chName.Text = "Sample";
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.ItemHeight = 20;
            this.treeView.Location = new System.Drawing.Point(6, 6);
            this.treeView.Name = "treeView";
            treeNode10.ImageIndex = 5;
            treeNode10.Name = "Node9";
            treeNode10.SelectedImageIndex = 5;
            treeNode10.Tag = "updatesPage";
            treeNode10.Text = "Updates";
            treeNode11.ImageIndex = 8;
            treeNode11.Name = "Node5";
            treeNode11.SelectedImageIndex = 8;
            treeNode11.Tag = "networkPage";
            treeNode11.Text = "Network";
            treeNode12.ImageIndex = 12;
            treeNode12.Name = "g15Node";
            treeNode12.SelectedImageIndex = 12;
            treeNode12.Tag = "g15Page";
            treeNode12.Text = "Logitech Keyboards";
            treeNode13.ImageIndex = 11;
            treeNode13.Name = "Node2";
            treeNode13.SelectedImageIndex = 11;
            treeNode13.Tag = "igbServerPage";
            treeNode13.Text = "IGB Server";
            treeNode14.Name = "relocationNode";
            treeNode14.Tag = "relocationPage";
            treeNode14.Text = "Relocation";
            treeNode15.ImageIndex = 10;
            treeNode15.Name = "generalNode";
            treeNode15.SelectedImageIndex = 10;
            treeNode15.Tag = "generalPage";
            treeNode15.Text = "General";
            treeNode16.ImageIndex = 6;
            treeNode16.Name = "Node3";
            treeNode16.SelectedImageIndex = 6;
            treeNode16.Tag = "mainWindowPage";
            treeNode16.Text = "Main Window";
            treeNode17.ImageIndex = 7;
            treeNode17.Name = "Node4";
            treeNode17.SelectedImageIndex = 7;
            treeNode17.Tag = "skillPlannerPage";
            treeNode17.Text = "Skill Planner";
            treeNode18.ImageIndex = 9;
            treeNode18.Name = "trayIconNode";
            treeNode18.SelectedImageIndex = 9;
            treeNode18.Tag = "trayIconPage";
            treeNode18.Text = "System Tray Icon";
            treeNode19.ImageIndex = 13;
            treeNode19.Name = "Node11";
            treeNode19.SelectedImageIndex = 13;
            treeNode19.Tag = "externalCalendarPage";
            treeNode19.Text = "External Calendar";
            treeNode20.ImageIndex = 1;
            treeNode20.Name = "Node10";
            treeNode20.SelectedImageIndex = 1;
            treeNode20.Tag = "schedulerUIPage";
            treeNode20.Text = "Scheduler";
            treeNode21.ImageIndex = 4;
            treeNode21.Name = "Node7";
            treeNode21.SelectedImageIndex = 4;
            treeNode21.Tag = "emailNotificationsPage";
            treeNode21.Text = "Skills Completion Mails";
            treeNode22.ImageIndex = 3;
            treeNode22.Name = "Node2";
            treeNode22.SelectedImageIndex = 3;
            treeNode22.Tag = "notificationsPage";
            treeNode22.Text = "Notifications";
            this.treeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode20,
            treeNode22});
            this.treeView.SelectedImageIndex = 0;
            this.treeView.ShowLines = false;
            this.treeView.ShowPlusMinus = false;
            this.treeView.ShowRootLines = false;
            this.treeView.Size = new System.Drawing.Size(187, 424);
            this.treeView.TabIndex = 6;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Transparent.png");
            this.imageList.Images.SetKeyName(1, "Calendar.png");
            this.imageList.Images.SetKeyName(2, "LookNFeel.png");
            this.imageList.Images.SetKeyName(3, "Notification2.png");
            this.imageList.Images.SetKeyName(4, "Email.png");
            this.imageList.Images.SetKeyName(5, "Software update.png");
            this.imageList.Images.SetKeyName(6, "MainWindow.png");
            this.imageList.Images.SetKeyName(7, "Plan.png");
            this.imageList.Images.SetKeyName(8, "Connection.png");
            this.imageList.Images.SetKeyName(9, "EVEMon16.png");
            this.imageList.Images.SetKeyName(10, "System config.png");
            this.imageList.Images.SetKeyName(11, "IGB Server.png");
            this.imageList.Images.SetKeyName(12, "Logitech Keyboard.png");
            this.imageList.Images.SetKeyName(13, "gcalendar.png");
            // 
            // leftPanel
            // 
            this.leftPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.leftPanel.Controls.Add(this.treeView);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Padding = new System.Windows.Forms.Padding(6);
            this.leftPanel.Size = new System.Drawing.Size(199, 436);
            this.leftPanel.TabIndex = 9;
            // 
            // multiPanel
            // 
            this.multiPanel.Controls.Add(this.mainWindowPage);
            this.multiPanel.Controls.Add(this.generalPage);
            this.multiPanel.Controls.Add(this.skillPlannerPage);
            this.multiPanel.Controls.Add(this.networkPage);
            this.multiPanel.Controls.Add(this.emailNotificationsPage);
            this.multiPanel.Controls.Add(this.notificationsPage);
            this.multiPanel.Controls.Add(this.trayIconPage);
            this.multiPanel.Controls.Add(this.updatesPage);
            this.multiPanel.Controls.Add(this.schedulerUIPage);
            this.multiPanel.Controls.Add(this.externalCalendarPage);
            this.multiPanel.Controls.Add(this.g15Page);
            this.multiPanel.Controls.Add(this.igbServerPage);
            this.multiPanel.Controls.Add(this.relocationPage);
            this.multiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiPanel.Location = new System.Drawing.Point(199, 0);
            this.multiPanel.Name = "multiPanel";
            this.multiPanel.Padding = new System.Windows.Forms.Padding(5);
            this.multiPanel.SelectedPage = this.skillPlannerPage;
            this.multiPanel.Size = new System.Drawing.Size(445, 436);
            this.multiPanel.TabIndex = 7;
            // 
            // mainWindowPage
            // 
            this.mainWindowPage.Controls.Add(this.groupBox2);
            this.mainWindowPage.Controls.Add(groupBox15);
            this.mainWindowPage.Controls.Add(groupBox7);
            this.mainWindowPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainWindowPage.Location = new System.Drawing.Point(5, 5);
            this.mainWindowPage.Name = "mainWindowPage";
            this.mainWindowPage.Size = new System.Drawing.Size(435, 426);
            this.mainWindowPage.TabIndex = 1;
            this.mainWindowPage.Text = "mainWindowPage";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.overviewPanel);
            this.groupBox2.Controls.Add(this.cbShowOverViewTab);
            this.groupBox2.Location = new System.Drawing.Point(1, 274);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(428, 127);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Overview";
            // 
            // overviewPanel
            // 
            this.overviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.overviewPanel.Controls.Add(this.overviewShowSkillQueueFreeRoomCheckBox);
            this.overviewPanel.Controls.Add(this.overviewShowWalletCheckBox);
            this.overviewPanel.Controls.Add(label31);
            this.overviewPanel.Controls.Add(this.overviewShowPortraitCheckBox);
            this.overviewPanel.Controls.Add(this.overviewPortraitSizeComboBox);
            this.overviewPanel.Location = new System.Drawing.Point(29, 43);
            this.overviewPanel.Name = "overviewPanel";
            this.overviewPanel.Size = new System.Drawing.Size(393, 79);
            this.overviewPanel.TabIndex = 32;
            // 
            // overviewShowSkillQueueFreeRoomCheckBox
            // 
            this.overviewShowSkillQueueFreeRoomCheckBox.AutoSize = true;
            this.overviewShowSkillQueueFreeRoomCheckBox.Location = new System.Drawing.Point(161, 3);
            this.overviewShowSkillQueueFreeRoomCheckBox.Name = "overviewShowSkillQueueFreeRoomCheckBox";
            this.overviewShowSkillQueueFreeRoomCheckBox.Size = new System.Drawing.Size(162, 17);
            this.overviewShowSkillQueueFreeRoomCheckBox.TabIndex = 32;
            this.overviewShowSkillQueueFreeRoomCheckBox.Text = "Show Skill Queue Free Room";
            this.overviewShowSkillQueueFreeRoomCheckBox.UseVisualStyleBackColor = true;
            // 
            // overviewShowWalletCheckBox
            // 
            this.overviewShowWalletCheckBox.AutoSize = true;
            this.overviewShowWalletCheckBox.Location = new System.Drawing.Point(3, 3);
            this.overviewShowWalletCheckBox.Name = "overviewShowWalletCheckBox";
            this.overviewShowWalletCheckBox.Size = new System.Drawing.Size(125, 17);
            this.overviewShowWalletCheckBox.TabIndex = 30;
            this.overviewShowWalletCheckBox.Text = "Show Wallet Balance";
            this.overviewShowWalletCheckBox.UseVisualStyleBackColor = true;
            // 
            // overviewShowPortraitCheckBox
            // 
            this.overviewShowPortraitCheckBox.AutoSize = true;
            this.overviewShowPortraitCheckBox.Location = new System.Drawing.Point(3, 26);
            this.overviewShowPortraitCheckBox.Name = "overviewShowPortraitCheckBox";
            this.overviewShowPortraitCheckBox.Size = new System.Drawing.Size(142, 17);
            this.overviewShowPortraitCheckBox.TabIndex = 26;
            this.overviewShowPortraitCheckBox.Text = "Show Character Portrait";
            this.overviewShowPortraitCheckBox.UseVisualStyleBackColor = true;
            // 
            // overviewPortraitSizeComboBox
            // 
            this.overviewPortraitSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.overviewPortraitSizeComboBox.FormattingEnabled = true;
            this.overviewPortraitSizeComboBox.Location = new System.Drawing.Point(52, 49);
            this.overviewPortraitSizeComboBox.Name = "overviewPortraitSizeComboBox";
            this.overviewPortraitSizeComboBox.Size = new System.Drawing.Size(93, 21);
            this.overviewPortraitSizeComboBox.TabIndex = 28;
            // 
            // cbShowOverViewTab
            // 
            this.cbShowOverViewTab.AutoSize = true;
            this.cbShowOverViewTab.Location = new System.Drawing.Point(14, 20);
            this.cbShowOverViewTab.Name = "cbShowOverViewTab";
            this.cbShowOverViewTab.Size = new System.Drawing.Size(128, 17);
            this.cbShowOverViewTab.TabIndex = 0;
            this.cbShowOverViewTab.Text = "Show \"Overview\" tab";
            this.cbShowOverViewTab.UseVisualStyleBackColor = true;
            this.cbShowOverViewTab.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // generalPage
            // 
            this.generalPage.Controls.Add(this.cbWorksafeMode);
            this.generalPage.Controls.Add(this.compatibilityCombo);
            this.generalPage.Controls.Add(label29);
            this.generalPage.Controls.Add(this.runAtStartupComboBox);
            this.generalPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalPage.Location = new System.Drawing.Point(5, 5);
            this.generalPage.Name = "generalPage";
            this.generalPage.Size = new System.Drawing.Size(435, 426);
            this.generalPage.TabIndex = 0;
            this.generalPage.Text = "generalPage";
            this.generalPage.Visible = false;
            // 
            // cbWorksafeMode
            // 
            this.cbWorksafeMode.AutoSize = true;
            this.cbWorksafeMode.Location = new System.Drawing.Point(3, 131);
            this.cbWorksafeMode.Name = "cbWorksafeMode";
            this.cbWorksafeMode.Size = new System.Drawing.Size(271, 17);
            this.cbWorksafeMode.TabIndex = 6;
            this.cbWorksafeMode.Text = "Run in \"safe for work\" mode (no portraits or colors)";
            this.cbWorksafeMode.UseVisualStyleBackColor = true;
            this.cbWorksafeMode.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // compatibilityCombo
            // 
            this.compatibilityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compatibilityCombo.FormattingEnabled = true;
            this.compatibilityCombo.Items.AddRange(new object[] {
            "Windows",
            "Wine"});
            this.compatibilityCombo.Location = new System.Drawing.Point(231, 178);
            this.compatibilityCombo.Name = "compatibilityCombo";
            this.compatibilityCombo.Size = new System.Drawing.Size(121, 21);
            this.compatibilityCombo.TabIndex = 0;
            // 
            // runAtStartupComboBox
            // 
            this.runAtStartupComboBox.AutoSize = true;
            this.runAtStartupComboBox.Location = new System.Drawing.Point(3, 88);
            this.runAtStartupComboBox.Name = "runAtStartupComboBox";
            this.runAtStartupComboBox.Size = new System.Drawing.Size(138, 17);
            this.runAtStartupComboBox.TabIndex = 5;
            this.runAtStartupComboBox.Text = "Run EVEMon at Startup";
            this.runAtStartupComboBox.UseVisualStyleBackColor = true;
            // 
            // skillPlannerPage
            // 
            this.skillPlannerPage.Controls.Add(this.SummaryOnMultiSelectOnlyCheckBox);
            this.skillPlannerPage.Controls.Add(this.cbHighlightQueuedSiklls);
            this.skillPlannerPage.Controls.Add(this.cbHighlightPartialSkills);
            this.skillPlannerPage.Controls.Add(this.cbHighlightConflicts);
            this.skillPlannerPage.Controls.Add(this.cbHighlightPrerequisites);
            this.skillPlannerPage.Controls.Add(this.cbHighlightPlannedSkills);
            this.skillPlannerPage.Controls.Add(groupBox10);
            this.skillPlannerPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillPlannerPage.Location = new System.Drawing.Point(5, 5);
            this.skillPlannerPage.Name = "skillPlannerPage";
            this.skillPlannerPage.Size = new System.Drawing.Size(435, 426);
            this.skillPlannerPage.TabIndex = 3;
            this.skillPlannerPage.Text = "skillPlannerPage";
            this.skillPlannerPage.Visible = false;
            // 
            // SummaryOnMultiSelectOnlyCheckBox
            // 
            this.SummaryOnMultiSelectOnlyCheckBox.AutoSize = true;
            this.SummaryOnMultiSelectOnlyCheckBox.Location = new System.Drawing.Point(7, 103);
            this.SummaryOnMultiSelectOnlyCheckBox.Name = "SummaryOnMultiSelectOnlyCheckBox";
            this.SummaryOnMultiSelectOnlyCheckBox.Size = new System.Drawing.Size(253, 17);
            this.SummaryOnMultiSelectOnlyCheckBox.TabIndex = 15;
            this.SummaryOnMultiSelectOnlyCheckBox.Text = "Only Show Selection Summary On \"Multi-Select\"";
            this.SummaryOnMultiSelectOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // cbHighlightQueuedSiklls
            // 
            this.cbHighlightQueuedSiklls.AutoSize = true;
            this.cbHighlightQueuedSiklls.Location = new System.Drawing.Point(7, 80);
            this.cbHighlightQueuedSiklls.Name = "cbHighlightQueuedSiklls";
            this.cbHighlightQueuedSiklls.Size = new System.Drawing.Size(133, 17);
            this.cbHighlightQueuedSiklls.TabIndex = 14;
            this.cbHighlightQueuedSiklls.Text = "Highlight Queued Skills";
            this.cbHighlightQueuedSiklls.UseVisualStyleBackColor = true;
            // 
            // cbHighlightPartialSkills
            // 
            this.cbHighlightPartialSkills.AutoSize = true;
            this.cbHighlightPartialSkills.Location = new System.Drawing.Point(195, 57);
            this.cbHighlightPartialSkills.Name = "cbHighlightPartialSkills";
            this.cbHighlightPartialSkills.Size = new System.Drawing.Size(172, 17);
            this.cbHighlightPartialSkills.TabIndex = 10;
            this.cbHighlightPartialSkills.Text = "Highlight Partially Trained Skills";
            this.cbHighlightPartialSkills.UseVisualStyleBackColor = true;
            // 
            // cbHighlightConflicts
            // 
            this.cbHighlightConflicts.AutoSize = true;
            this.cbHighlightConflicts.Location = new System.Drawing.Point(7, 57);
            this.cbHighlightConflicts.Name = "cbHighlightConflicts";
            this.cbHighlightConflicts.Size = new System.Drawing.Size(142, 17);
            this.cbHighlightConflicts.TabIndex = 9;
            this.cbHighlightConflicts.Text = "Show Schedule Conflicts";
            this.cbHighlightConflicts.UseVisualStyleBackColor = true;
            // 
            // cbHighlightPrerequisites
            // 
            this.cbHighlightPrerequisites.AutoSize = true;
            this.cbHighlightPrerequisites.Location = new System.Drawing.Point(195, 34);
            this.cbHighlightPrerequisites.Name = "cbHighlightPrerequisites";
            this.cbHighlightPrerequisites.Size = new System.Drawing.Size(132, 17);
            this.cbHighlightPrerequisites.TabIndex = 8;
            this.cbHighlightPrerequisites.Text = "Highlight Prerequisites";
            this.cbHighlightPrerequisites.UseVisualStyleBackColor = true;
            // 
            // cbHighlightPlannedSkills
            // 
            this.cbHighlightPlannedSkills.AutoSize = true;
            this.cbHighlightPlannedSkills.Location = new System.Drawing.Point(7, 34);
            this.cbHighlightPlannedSkills.Name = "cbHighlightPlannedSkills";
            this.cbHighlightPlannedSkills.Size = new System.Drawing.Size(142, 17);
            this.cbHighlightPlannedSkills.TabIndex = 0;
            this.cbHighlightPlannedSkills.Text = "Emphasize Planned Skills";
            this.cbHighlightPlannedSkills.UseVisualStyleBackColor = true;
            // 
            // networkPage
            // 
            this.networkPage.Controls.Add(this.ApiProxyGroupBox);
            this.networkPage.Controls.Add(this.ProxyServerGroupBox);
            this.networkPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.networkPage.Location = new System.Drawing.Point(5, 5);
            this.networkPage.Name = "networkPage";
            this.networkPage.Size = new System.Drawing.Size(435, 426);
            this.networkPage.TabIndex = 4;
            this.networkPage.Text = "networkPage";
            this.networkPage.Visible = false;
            // 
            // ApiProxyGroupBox
            // 
            this.ApiProxyGroupBox.Controls.Add(label16);
            this.ApiProxyGroupBox.Controls.Add(this.btnDeleteAPIServer);
            this.ApiProxyGroupBox.Controls.Add(this.btnAddAPIServer);
            this.ApiProxyGroupBox.Controls.Add(this.cbAPIServer);
            this.ApiProxyGroupBox.Controls.Add(this.btnEditAPIServer);
            this.ApiProxyGroupBox.Location = new System.Drawing.Point(0, 244);
            this.ApiProxyGroupBox.Name = "ApiProxyGroupBox";
            this.ApiProxyGroupBox.Size = new System.Drawing.Size(392, 122);
            this.ApiProxyGroupBox.TabIndex = 7;
            this.ApiProxyGroupBox.TabStop = false;
            this.ApiProxyGroupBox.Text = "API Provider";
            // 
            // btnDeleteAPIServer
            // 
            this.btnDeleteAPIServer.Location = new System.Drawing.Point(174, 85);
            this.btnDeleteAPIServer.Name = "btnDeleteAPIServer";
            this.btnDeleteAPIServer.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteAPIServer.TabIndex = 4;
            this.btnDeleteAPIServer.Text = "Delete";
            this.btnDeleteAPIServer.UseVisualStyleBackColor = true;
            this.btnDeleteAPIServer.Click += new System.EventHandler(this.btnDeleteAPIServer_Click);
            // 
            // btnAddAPIServer
            // 
            this.btnAddAPIServer.Location = new System.Drawing.Point(12, 85);
            this.btnAddAPIServer.Name = "btnAddAPIServer";
            this.btnAddAPIServer.Size = new System.Drawing.Size(75, 23);
            this.btnAddAPIServer.TabIndex = 2;
            this.btnAddAPIServer.Text = "Add";
            this.btnAddAPIServer.UseVisualStyleBackColor = true;
            this.btnAddAPIServer.Click += new System.EventHandler(this.btnAddAPIServer_Click);
            // 
            // cbAPIServer
            // 
            this.cbAPIServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAPIServer.FormattingEnabled = true;
            this.cbAPIServer.Location = new System.Drawing.Point(12, 58);
            this.cbAPIServer.Name = "cbAPIServer";
            this.cbAPIServer.Size = new System.Drawing.Size(233, 21);
            this.cbAPIServer.TabIndex = 0;
            this.cbAPIServer.SelectedIndexChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // btnEditAPIServer
            // 
            this.btnEditAPIServer.Location = new System.Drawing.Point(93, 85);
            this.btnEditAPIServer.Name = "btnEditAPIServer";
            this.btnEditAPIServer.Size = new System.Drawing.Size(75, 23);
            this.btnEditAPIServer.TabIndex = 3;
            this.btnEditAPIServer.Text = "Edit";
            this.btnEditAPIServer.UseVisualStyleBackColor = true;
            this.btnEditAPIServer.Click += new System.EventHandler(this.btnEditAPIServer_Click);
            // 
            // ProxyServerGroupBox
            // 
            this.ProxyServerGroupBox.Controls.Add(this.customProxyCheckBox);
            this.ProxyServerGroupBox.Controls.Add(label13);
            this.ProxyServerGroupBox.Controls.Add(this.customProxyPanel);
            this.ProxyServerGroupBox.Location = new System.Drawing.Point(3, 31);
            this.ProxyServerGroupBox.Name = "ProxyServerGroupBox";
            this.ProxyServerGroupBox.Size = new System.Drawing.Size(392, 157);
            this.ProxyServerGroupBox.TabIndex = 0;
            this.ProxyServerGroupBox.TabStop = false;
            this.ProxyServerGroupBox.Text = "Proxy Server Settings";
            // 
            // customProxyCheckBox
            // 
            this.customProxyCheckBox.AutoSize = true;
            this.customProxyCheckBox.Location = new System.Drawing.Point(9, 74);
            this.customProxyCheckBox.Name = "customProxyCheckBox";
            this.customProxyCheckBox.Size = new System.Drawing.Size(121, 17);
            this.customProxyCheckBox.TabIndex = 9;
            this.customProxyCheckBox.Text = "Use a custom proxy";
            this.customProxyCheckBox.UseVisualStyleBackColor = true;
            this.customProxyCheckBox.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // customProxyPanel
            // 
            this.customProxyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.customProxyPanel.Controls.Add(this.proxyPortTextBox);
            this.customProxyPanel.Controls.Add(label12);
            this.customProxyPanel.Controls.Add(this.proxyAuthenticationButton);
            this.customProxyPanel.Controls.Add(label11);
            this.customProxyPanel.Controls.Add(label10);
            this.customProxyPanel.Controls.Add(this.proxyHttpHostTextBox);
            this.customProxyPanel.Location = new System.Drawing.Point(17, 97);
            this.customProxyPanel.Name = "customProxyPanel";
            this.customProxyPanel.Size = new System.Drawing.Size(369, 54);
            this.customProxyPanel.TabIndex = 6;
            // 
            // proxyPortTextBox
            // 
            this.proxyPortTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyPortTextBox.Location = new System.Drawing.Point(222, 24);
            this.proxyPortTextBox.Name = "proxyPortTextBox";
            this.proxyPortTextBox.Size = new System.Drawing.Size(38, 21);
            this.proxyPortTextBox.TabIndex = 2;
            this.proxyPortTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.proxyPortTextBox_Validating);
            // 
            // proxyAuthenticationButton
            // 
            this.proxyAuthenticationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyAuthenticationButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.proxyAuthenticationButton.Location = new System.Drawing.Point(266, 22);
            this.proxyAuthenticationButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.proxyAuthenticationButton.Name = "proxyAuthenticationButton";
            this.proxyAuthenticationButton.Size = new System.Drawing.Size(97, 23);
            this.proxyAuthenticationButton.TabIndex = 5;
            this.proxyAuthenticationButton.Text = "Authentication...";
            this.proxyAuthenticationButton.UseVisualStyleBackColor = true;
            this.proxyAuthenticationButton.Click += new System.EventHandler(this.proxyAuthenticationButton_Click);
            // 
            // proxyHttpHostTextBox
            // 
            this.proxyHttpHostTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyHttpHostTextBox.Location = new System.Drawing.Point(50, 24);
            this.proxyHttpHostTextBox.Name = "proxyHttpHostTextBox";
            this.proxyHttpHostTextBox.Size = new System.Drawing.Size(165, 21);
            this.proxyHttpHostTextBox.TabIndex = 1;
            // 
            // emailNotificationsPage
            // 
            this.emailNotificationsPage.Controls.Add(this.mailNotificationPanel);
            this.emailNotificationsPage.Controls.Add(this.mailNotificationCheckBox);
            this.emailNotificationsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.emailNotificationsPage.Location = new System.Drawing.Point(5, 5);
            this.emailNotificationsPage.Name = "emailNotificationsPage";
            this.emailNotificationsPage.Size = new System.Drawing.Size(435, 426);
            this.emailNotificationsPage.TabIndex = 6;
            this.emailNotificationsPage.Text = "emailNotificationsPage";
            this.emailNotificationsPage.Visible = false;
            // 
            // mailNotificationPanel
            // 
            this.mailNotificationPanel.AutoSize = true;
            this.mailNotificationPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mailNotificationPanel.Controls.Add(this.cbEmailUseShortFormat);
            this.mailNotificationPanel.Controls.Add(this.tableLayoutPanel2);
            this.mailNotificationPanel.Controls.Add(this.btnTestEmail);
            this.mailNotificationPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mailNotificationPanel.Location = new System.Drawing.Point(3, 70);
            this.mailNotificationPanel.Name = "mailNotificationPanel";
            this.mailNotificationPanel.Padding = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this.mailNotificationPanel.Size = new System.Drawing.Size(362, 285);
            this.mailNotificationPanel.TabIndex = 1;
            this.mailNotificationPanel.WrapContents = false;
            // 
            // cbEmailUseShortFormat
            // 
            this.cbEmailUseShortFormat.AutoSize = true;
            this.cbEmailUseShortFormat.Location = new System.Drawing.Point(12, 3);
            this.cbEmailUseShortFormat.Name = "cbEmailUseShortFormat";
            this.cbEmailUseShortFormat.Size = new System.Drawing.Size(183, 17);
            this.cbEmailUseShortFormat.TabIndex = 2;
            this.cbEmailUseShortFormat.Text = "Use Short Format (SMS-Friendly)";
            this.cbEmailUseShortFormat.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tlpEmailSettings, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 26);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 227F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 227F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(338, 227);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tlpEmailSettings
            // 
            this.tlpEmailSettings.AutoSize = true;
            this.tlpEmailSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpEmailSettings.ColumnCount = 2;
            this.tlpEmailSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpEmailSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpEmailSettings.Controls.Add(label1, 0, 0);
            this.tlpEmailSettings.Controls.Add(label2, 0, 5);
            this.tlpEmailSettings.Controls.Add(label3, 0, 6);
            this.tlpEmailSettings.Controls.Add(this.tbMailServer, 1, 0);
            this.tlpEmailSettings.Controls.Add(this.tbFromAddress, 1, 5);
            this.tlpEmailSettings.Controls.Add(this.tbToAddress, 1, 6);
            this.tlpEmailSettings.Controls.Add(this.cbEmailServerRequireSsl, 1, 2);
            this.tlpEmailSettings.Controls.Add(this.cbEmailAuthRequired, 1, 3);
            this.tlpEmailSettings.Controls.Add(this.tlpEmailAuthTable, 1, 4);
            this.tlpEmailSettings.Controls.Add(this.emailPortTextBox, 1, 1);
            this.tlpEmailSettings.Controls.Add(lblPortNumber, 0, 1);
            this.tlpEmailSettings.Location = new System.Drawing.Point(22, 3);
            this.tlpEmailSettings.Name = "tlpEmailSettings";
            this.tlpEmailSettings.RowCount = 7;
            this.tlpEmailSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailSettings.Size = new System.Drawing.Size(294, 216);
            this.tlpEmailSettings.TabIndex = 2;
            // 
            // tbMailServer
            // 
            this.tbMailServer.Location = new System.Drawing.Point(85, 3);
            this.tbMailServer.Name = "tbMailServer";
            this.tbMailServer.Size = new System.Drawing.Size(152, 21);
            this.tbMailServer.TabIndex = 1;
            // 
            // tbFromAddress
            // 
            this.tbFromAddress.Location = new System.Drawing.Point(85, 165);
            this.tbFromAddress.Name = "tbFromAddress";
            this.tbFromAddress.Size = new System.Drawing.Size(206, 21);
            this.tbFromAddress.TabIndex = 1;
            // 
            // tbToAddress
            // 
            this.tbToAddress.Location = new System.Drawing.Point(85, 192);
            this.tbToAddress.Name = "tbToAddress";
            this.tbToAddress.Size = new System.Drawing.Size(206, 21);
            this.tbToAddress.TabIndex = 1;
            // 
            // cbEmailServerRequireSsl
            // 
            this.cbEmailServerRequireSsl.AutoSize = true;
            this.cbEmailServerRequireSsl.Location = new System.Drawing.Point(85, 57);
            this.cbEmailServerRequireSsl.Name = "cbEmailServerRequireSsl";
            this.cbEmailServerRequireSsl.Size = new System.Drawing.Size(114, 17);
            this.cbEmailServerRequireSsl.TabIndex = 1;
            this.cbEmailServerRequireSsl.Text = "Connect using SSL";
            this.cbEmailServerRequireSsl.UseVisualStyleBackColor = true;
            // 
            // cbEmailAuthRequired
            // 
            this.cbEmailAuthRequired.AutoSize = true;
            this.cbEmailAuthRequired.Location = new System.Drawing.Point(85, 80);
            this.cbEmailAuthRequired.Name = "cbEmailAuthRequired";
            this.cbEmailAuthRequired.Size = new System.Drawing.Size(125, 17);
            this.cbEmailAuthRequired.TabIndex = 1;
            this.cbEmailAuthRequired.Text = "Server requires login";
            this.cbEmailAuthRequired.UseVisualStyleBackColor = true;
            this.cbEmailAuthRequired.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // tlpEmailAuthTable
            // 
            this.tlpEmailAuthTable.AutoSize = true;
            this.tlpEmailAuthTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpEmailAuthTable.ColumnCount = 2;
            this.tlpEmailAuthTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpEmailAuthTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpEmailAuthTable.Controls.Add(label5, 0, 1);
            this.tlpEmailAuthTable.Controls.Add(label4, 0, 0);
            this.tlpEmailAuthTable.Controls.Add(this.tbEmailUsername, 1, 0);
            this.tlpEmailAuthTable.Controls.Add(this.tbEmailPassword, 1, 1);
            this.tlpEmailAuthTable.Location = new System.Drawing.Point(85, 103);
            this.tlpEmailAuthTable.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.tlpEmailAuthTable.Name = "tlpEmailAuthTable";
            this.tlpEmailAuthTable.RowCount = 2;
            this.tlpEmailAuthTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailAuthTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpEmailAuthTable.Size = new System.Drawing.Size(200, 54);
            this.tlpEmailAuthTable.TabIndex = 8;
            // 
            // tbEmailUsername
            // 
            this.tbEmailUsername.Location = new System.Drawing.Point(68, 3);
            this.tbEmailUsername.Name = "tbEmailUsername";
            this.tbEmailUsername.Size = new System.Drawing.Size(129, 21);
            this.tbEmailUsername.TabIndex = 1;
            // 
            // tbEmailPassword
            // 
            this.tbEmailPassword.Location = new System.Drawing.Point(68, 30);
            this.tbEmailPassword.Name = "tbEmailPassword";
            this.tbEmailPassword.PasswordChar = '*';
            this.tbEmailPassword.Size = new System.Drawing.Size(129, 21);
            this.tbEmailPassword.TabIndex = 1;
            // 
            // emailPortTextBox
            // 
            this.emailPortTextBox.Location = new System.Drawing.Point(85, 30);
            this.emailPortTextBox.Name = "emailPortTextBox";
            this.emailPortTextBox.Size = new System.Drawing.Size(152, 21);
            this.emailPortTextBox.TabIndex = 1;
            this.emailPortTextBox.Text = "25";
            // 
            // btnTestEmail
            // 
            this.btnTestEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestEmail.Location = new System.Drawing.Point(240, 259);
            this.btnTestEmail.Name = "btnTestEmail";
            this.btnTestEmail.Size = new System.Drawing.Size(110, 23);
            this.btnTestEmail.TabIndex = 10;
            this.btnTestEmail.Text = "Send Test Email";
            this.btnTestEmail.UseVisualStyleBackColor = true;
            this.btnTestEmail.Click += new System.EventHandler(this.emailTestButton_Click);
            // 
            // mailNotificationCheckBox
            // 
            this.mailNotificationCheckBox.AutoSize = true;
            this.mailNotificationCheckBox.Location = new System.Drawing.Point(3, 47);
            this.mailNotificationCheckBox.Name = "mailNotificationCheckBox";
            this.mailNotificationCheckBox.Size = new System.Drawing.Size(215, 17);
            this.mailNotificationCheckBox.TabIndex = 1;
            this.mailNotificationCheckBox.Text = "Send email when skill training completes";
            this.mailNotificationCheckBox.UseVisualStyleBackColor = true;
            this.mailNotificationCheckBox.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // notificationsPage
            // 
            this.notificationsPage.Controls.Add(this.notificationsControl);
            this.notificationsPage.Controls.Add(this.cbPlaySoundOnSkillComplete);
            this.notificationsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notificationsPage.Location = new System.Drawing.Point(5, 5);
            this.notificationsPage.Name = "notificationsPage";
            this.notificationsPage.Size = new System.Drawing.Size(435, 426);
            this.notificationsPage.TabIndex = 7;
            this.notificationsPage.Text = "notificationsPage";
            this.notificationsPage.Visible = false;
            // 
            // notificationsControl
            // 
            this.notificationsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.notificationsControl.BackColor = System.Drawing.SystemColors.Window;
            this.notificationsControl.Location = new System.Drawing.Point(3, 30);
            this.notificationsControl.Name = "notificationsControl";
            this.notificationsControl.Settings = null;
            this.notificationsControl.Size = new System.Drawing.Size(417, 340);
            this.notificationsControl.TabIndex = 4;
            // 
            // cbPlaySoundOnSkillComplete
            // 
            this.cbPlaySoundOnSkillComplete.AutoSize = true;
            this.cbPlaySoundOnSkillComplete.Location = new System.Drawing.Point(3, 380);
            this.cbPlaySoundOnSkillComplete.Name = "cbPlaySoundOnSkillComplete";
            this.cbPlaySoundOnSkillComplete.Size = new System.Drawing.Size(216, 17);
            this.cbPlaySoundOnSkillComplete.TabIndex = 3;
            this.cbPlaySoundOnSkillComplete.Text = "Play sound when skill training completes";
            this.cbPlaySoundOnSkillComplete.UseVisualStyleBackColor = true;
            // 
            // trayIconPage
            // 
            this.trayIconPage.Controls.Add(this.WindowBehaviourGroupBox);
            this.trayIconPage.Controls.Add(this.trayIconPopupGroupBox);
            this.trayIconPage.Controls.Add(groupBox1);
            this.trayIconPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trayIconPage.Location = new System.Drawing.Point(5, 5);
            this.trayIconPage.Name = "trayIconPage";
            this.trayIconPage.Size = new System.Drawing.Size(435, 426);
            this.trayIconPage.TabIndex = 8;
            this.trayIconPage.Text = "trayIconPage";
            this.trayIconPage.Visible = false;
            // 
            // WindowBehaviourGroupBox
            // 
            this.WindowBehaviourGroupBox.Controls.Add(this.rbMinToTaskBar);
            this.WindowBehaviourGroupBox.Controls.Add(this.rbMinToTray);
            this.WindowBehaviourGroupBox.Controls.Add(this.rbExitEVEMon);
            this.WindowBehaviourGroupBox.Location = new System.Drawing.Point(9, 293);
            this.WindowBehaviourGroupBox.Name = "WindowBehaviourGroupBox";
            this.WindowBehaviourGroupBox.Size = new System.Drawing.Size(419, 91);
            this.WindowBehaviourGroupBox.TabIndex = 17;
            this.WindowBehaviourGroupBox.TabStop = false;
            this.WindowBehaviourGroupBox.Text = "Main Window Close Behaviour";
            // 
            // rbMinToTaskBar
            // 
            this.rbMinToTaskBar.AutoSize = true;
            this.rbMinToTaskBar.Location = new System.Drawing.Point(12, 66);
            this.rbMinToTaskBar.Name = "rbMinToTaskBar";
            this.rbMinToTaskBar.Size = new System.Drawing.Size(288, 17);
            this.rbMinToTaskBar.TabIndex = 2;
            this.rbMinToTaskBar.TabStop = true;
            this.rbMinToTaskBar.Text = "Minimize to the taskbar (Recommended for Windows 7)";
            this.rbMinToTaskBar.UseVisualStyleBackColor = true;
            // 
            // rbMinToTray
            // 
            this.rbMinToTray.AutoSize = true;
            this.rbMinToTray.Location = new System.Drawing.Point(12, 43);
            this.rbMinToTray.Name = "rbMinToTray";
            this.rbMinToTray.Size = new System.Drawing.Size(296, 17);
            this.rbMinToTray.TabIndex = 1;
            this.rbMinToTray.TabStop = true;
            this.rbMinToTray.Text = "Minimize to the system tray (Recommended for XP/Vista)";
            this.rbMinToTray.UseVisualStyleBackColor = true;
            // 
            // rbExitEVEMon
            // 
            this.rbExitEVEMon.AutoSize = true;
            this.rbExitEVEMon.Location = new System.Drawing.Point(12, 20);
            this.rbExitEVEMon.Name = "rbExitEVEMon";
            this.rbExitEVEMon.Size = new System.Drawing.Size(84, 17);
            this.rbExitEVEMon.TabIndex = 0;
            this.rbExitEVEMon.TabStop = true;
            this.rbExitEVEMon.Text = "Exit EVEMon";
            this.rbExitEVEMon.UseVisualStyleBackColor = true;
            // 
            // trayIconPopupGroupBox
            // 
            this.trayIconPopupGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trayIconPopupGroupBox.Controls.Add(this.trayPopupButton);
            this.trayIconPopupGroupBox.Controls.Add(this.trayPopupRadio);
            this.trayIconPopupGroupBox.Controls.Add(this.trayTooltipRadio);
            this.trayIconPopupGroupBox.Controls.Add(this.trayTooltipButton);
            this.trayIconPopupGroupBox.Location = new System.Drawing.Point(9, 170);
            this.trayIconPopupGroupBox.Name = "trayIconPopupGroupBox";
            this.trayIconPopupGroupBox.Size = new System.Drawing.Size(419, 83);
            this.trayIconPopupGroupBox.TabIndex = 10;
            this.trayIconPopupGroupBox.TabStop = false;
            this.trayIconPopupGroupBox.Text = "Icon Popup Style";
            // 
            // trayPopupButton
            // 
            this.trayPopupButton.Location = new System.Drawing.Point(69, 46);
            this.trayPopupButton.Name = "trayPopupButton";
            this.trayPopupButton.Size = new System.Drawing.Size(75, 23);
            this.trayPopupButton.TabIndex = 4;
            this.trayPopupButton.Text = "Configure";
            this.trayPopupButton.UseVisualStyleBackColor = true;
            this.trayPopupButton.Click += new System.EventHandler(this.trayPopupButton_Click);
            // 
            // trayPopupRadio
            // 
            this.trayPopupRadio.AutoSize = true;
            this.trayPopupRadio.Location = new System.Drawing.Point(6, 49);
            this.trayPopupRadio.Name = "trayPopupRadio";
            this.trayPopupRadio.Size = new System.Drawing.Size(55, 17);
            this.trayPopupRadio.TabIndex = 3;
            this.trayPopupRadio.TabStop = true;
            this.trayPopupRadio.Text = "Popup";
            this.trayPopupRadio.UseVisualStyleBackColor = true;
            // 
            // trayTooltipRadio
            // 
            this.trayTooltipRadio.AutoSize = true;
            this.trayTooltipRadio.Location = new System.Drawing.Point(6, 20);
            this.trayTooltipRadio.Name = "trayTooltipRadio";
            this.trayTooltipRadio.Size = new System.Drawing.Size(57, 17);
            this.trayTooltipRadio.TabIndex = 0;
            this.trayTooltipRadio.TabStop = true;
            this.trayTooltipRadio.Text = "Tooltip";
            this.trayTooltipRadio.UseVisualStyleBackColor = true;
            // 
            // trayTooltipButton
            // 
            this.trayTooltipButton.Location = new System.Drawing.Point(69, 17);
            this.trayTooltipButton.Name = "trayTooltipButton";
            this.trayTooltipButton.Size = new System.Drawing.Size(75, 23);
            this.trayTooltipButton.TabIndex = 2;
            this.trayTooltipButton.Text = "Configure";
            this.trayTooltipButton.UseVisualStyleBackColor = true;
            this.trayTooltipButton.Click += new System.EventHandler(this.trayTooltipButton_Click);
            // 
            // updatesPage
            // 
            this.updatesPage.Controls.Add(this.updateSettingsControl);
            this.updatesPage.Controls.Add(this.label18);
            this.updatesPage.Controls.Add(this.cbCheckTimeOnStartup);
            this.updatesPage.Controls.Add(this.cbAutomaticallySearchForNewVersions);
            this.updatesPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updatesPage.Location = new System.Drawing.Point(5, 5);
            this.updatesPage.Name = "updatesPage";
            this.updatesPage.Size = new System.Drawing.Size(435, 426);
            this.updatesPage.TabIndex = 9;
            this.updatesPage.Text = "updatesPage";
            this.updatesPage.Visible = false;
            // 
            // updateSettingsControl
            // 
            this.updateSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.updateSettingsControl.Location = new System.Drawing.Point(15, 136);
            this.updateSettingsControl.Name = "updateSettingsControl";
            this.updateSettingsControl.Settings = null;
            this.updateSettingsControl.Size = new System.Drawing.Size(413, 287);
            this.updateSettingsControl.TabIndex = 10;
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label18.Location = new System.Drawing.Point(3, 29);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(429, 34);
            this.label18.TabIndex = 9;
            this.label18.Text = "The following settings help reducing the network load, especially for high-latenc" +
                "y connections and clients with many characters.";
            // 
            // cbCheckTimeOnStartup
            // 
            this.cbCheckTimeOnStartup.AutoSize = true;
            this.cbCheckTimeOnStartup.Location = new System.Drawing.Point(15, 77);
            this.cbCheckTimeOnStartup.Name = "cbCheckTimeOnStartup";
            this.cbCheckTimeOnStartup.Size = new System.Drawing.Size(201, 17);
            this.cbCheckTimeOnStartup.TabIndex = 0;
            this.cbCheckTimeOnStartup.Text = "Check the computer clock on startup";
            this.cbCheckTimeOnStartup.UseVisualStyleBackColor = true;
            // 
            // cbAutomaticallySearchForNewVersions
            // 
            this.cbAutomaticallySearchForNewVersions.AutoSize = true;
            this.cbAutomaticallySearchForNewVersions.Location = new System.Drawing.Point(15, 100);
            this.cbAutomaticallySearchForNewVersions.Name = "cbAutomaticallySearchForNewVersions";
            this.cbAutomaticallySearchForNewVersions.Size = new System.Drawing.Size(249, 17);
            this.cbAutomaticallySearchForNewVersions.TabIndex = 0;
            this.cbAutomaticallySearchForNewVersions.Text = "Automatically search for new EVEMon versions";
            this.cbAutomaticallySearchForNewVersions.UseVisualStyleBackColor = true;
            // 
            // schedulerUIPage
            // 
            this.schedulerUIPage.Controls.Add(label19);
            this.schedulerUIPage.Controls.Add(this.panelColorText);
            this.schedulerUIPage.Controls.Add(label24);
            this.schedulerUIPage.Controls.Add(this.panelColorRecurring2);
            this.schedulerUIPage.Controls.Add(label20);
            this.schedulerUIPage.Controls.Add(this.panelColorRecurring1);
            this.schedulerUIPage.Controls.Add(label21);
            this.schedulerUIPage.Controls.Add(this.panelColorSingle2);
            this.schedulerUIPage.Controls.Add(label22);
            this.schedulerUIPage.Controls.Add(this.panelColorSingle1);
            this.schedulerUIPage.Controls.Add(this.panelColorBlocking);
            this.schedulerUIPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schedulerUIPage.Location = new System.Drawing.Point(5, 5);
            this.schedulerUIPage.Name = "schedulerUIPage";
            this.schedulerUIPage.Size = new System.Drawing.Size(435, 426);
            this.schedulerUIPage.TabIndex = 10;
            this.schedulerUIPage.Text = "schedulerUIPage";
            this.schedulerUIPage.Visible = false;
            // 
            // panelColorText
            // 
            this.panelColorText.BackColor = System.Drawing.Color.White;
            this.panelColorText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorText.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelColorText.Location = new System.Drawing.Point(107, 105);
            this.panelColorText.Name = "panelColorText";
            this.panelColorText.Size = new System.Drawing.Size(43, 17);
            this.panelColorText.TabIndex = 4;
            this.panelColorText.Click += new System.EventHandler(this.colorPanel_Click);
            // 
            // panelColorRecurring2
            // 
            this.panelColorRecurring2.BackColor = System.Drawing.Color.LightGreen;
            this.panelColorRecurring2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorRecurring2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelColorRecurring2.Location = new System.Drawing.Point(156, 176);
            this.panelColorRecurring2.Name = "panelColorRecurring2";
            this.panelColorRecurring2.Size = new System.Drawing.Size(43, 17);
            this.panelColorRecurring2.TabIndex = 4;
            this.panelColorRecurring2.Click += new System.EventHandler(this.colorPanel_Click);
            // 
            // panelColorRecurring1
            // 
            this.panelColorRecurring1.BackColor = System.Drawing.Color.Green;
            this.panelColorRecurring1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorRecurring1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelColorRecurring1.Location = new System.Drawing.Point(107, 176);
            this.panelColorRecurring1.Name = "panelColorRecurring1";
            this.panelColorRecurring1.Size = new System.Drawing.Size(43, 17);
            this.panelColorRecurring1.TabIndex = 4;
            this.panelColorRecurring1.Click += new System.EventHandler(this.colorPanel_Click);
            // 
            // panelColorSingle2
            // 
            this.panelColorSingle2.BackColor = System.Drawing.Color.LightBlue;
            this.panelColorSingle2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorSingle2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelColorSingle2.Location = new System.Drawing.Point(156, 153);
            this.panelColorSingle2.Name = "panelColorSingle2";
            this.panelColorSingle2.Size = new System.Drawing.Size(43, 17);
            this.panelColorSingle2.TabIndex = 4;
            this.panelColorSingle2.Click += new System.EventHandler(this.colorPanel_Click);
            // 
            // panelColorSingle1
            // 
            this.panelColorSingle1.BackColor = System.Drawing.Color.Blue;
            this.panelColorSingle1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorSingle1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelColorSingle1.Location = new System.Drawing.Point(107, 153);
            this.panelColorSingle1.Name = "panelColorSingle1";
            this.panelColorSingle1.Size = new System.Drawing.Size(43, 17);
            this.panelColorSingle1.TabIndex = 4;
            this.panelColorSingle1.Click += new System.EventHandler(this.colorPanel_Click);
            // 
            // panelColorBlocking
            // 
            this.panelColorBlocking.BackColor = System.Drawing.Color.Red;
            this.panelColorBlocking.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorBlocking.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelColorBlocking.Location = new System.Drawing.Point(107, 128);
            this.panelColorBlocking.Name = "panelColorBlocking";
            this.panelColorBlocking.Size = new System.Drawing.Size(43, 17);
            this.panelColorBlocking.TabIndex = 3;
            this.panelColorBlocking.Click += new System.EventHandler(this.colorPanel_Click);
            // 
            // externalCalendarPage
            // 
            this.externalCalendarPage.Controls.Add(this.externalCalendarPanel);
            this.externalCalendarPage.Controls.Add(label25);
            this.externalCalendarPage.Controls.Add(this.externalCalendarCheckbox);
            this.externalCalendarPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.externalCalendarPage.Location = new System.Drawing.Point(5, 5);
            this.externalCalendarPage.Name = "externalCalendarPage";
            this.externalCalendarPage.Size = new System.Drawing.Size(435, 426);
            this.externalCalendarPage.TabIndex = 11;
            this.externalCalendarPage.Text = "externalCalendarPage";
            this.externalCalendarPage.Visible = false;
            // 
            // externalCalendarPanel
            // 
            this.externalCalendarPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.externalCalendarPanel.Controls.Add(this.rbMSOutlook);
            this.externalCalendarPanel.Controls.Add(this.dtpLateReminder);
            this.externalCalendarPanel.Controls.Add(this.tbReminder);
            this.externalCalendarPanel.Controls.Add(this.dtpEarlyReminder);
            this.externalCalendarPanel.Controls.Add(this.gbGoogle);
            this.externalCalendarPanel.Controls.Add(this.rbGoogle);
            this.externalCalendarPanel.Controls.Add(this.cbUseAlterateReminder);
            this.externalCalendarPanel.Controls.Add(lblEarlyReminder);
            this.externalCalendarPanel.Controls.Add(this.cbSetReminder);
            this.externalCalendarPanel.Controls.Add(lblLateReminder);
            this.externalCalendarPanel.Location = new System.Drawing.Point(3, 119);
            this.externalCalendarPanel.Name = "externalCalendarPanel";
            this.externalCalendarPanel.Size = new System.Drawing.Size(429, 262);
            this.externalCalendarPanel.TabIndex = 12;
            // 
            // rbMSOutlook
            // 
            this.rbMSOutlook.AutoSize = true;
            this.rbMSOutlook.Checked = true;
            this.rbMSOutlook.Location = new System.Drawing.Point(3, 5);
            this.rbMSOutlook.Name = "rbMSOutlook";
            this.rbMSOutlook.Size = new System.Drawing.Size(79, 17);
            this.rbMSOutlook.TabIndex = 1;
            this.rbMSOutlook.TabStop = true;
            this.rbMSOutlook.Text = "MS Outlook";
            this.rbMSOutlook.UseVisualStyleBackColor = true;
            this.rbMSOutlook.Click += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // dtpLateReminder
            // 
            this.dtpLateReminder.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpLateReminder.Location = new System.Drawing.Point(281, 224);
            this.dtpLateReminder.Name = "dtpLateReminder";
            this.dtpLateReminder.ShowUpDown = true;
            this.dtpLateReminder.Size = new System.Drawing.Size(72, 21);
            this.dtpLateReminder.TabIndex = 10;
            this.dtpLateReminder.Value = new System.DateTime(2007, 9, 21, 0, 0, 0, 0);
            // 
            // tbReminder
            // 
            this.tbReminder.Location = new System.Drawing.Point(88, 171);
            this.tbReminder.Name = "tbReminder";
            this.tbReminder.Size = new System.Drawing.Size(35, 21);
            this.tbReminder.TabIndex = 7;
            this.tbReminder.Text = "5";
            this.tbReminder.Validating += new System.ComponentModel.CancelEventHandler(this.tbReminder_Validating);
            // 
            // dtpEarlyReminder
            // 
            this.dtpEarlyReminder.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpEarlyReminder.Location = new System.Drawing.Point(119, 224);
            this.dtpEarlyReminder.Name = "dtpEarlyReminder";
            this.dtpEarlyReminder.ShowUpDown = true;
            this.dtpEarlyReminder.Size = new System.Drawing.Size(70, 21);
            this.dtpEarlyReminder.TabIndex = 9;
            this.dtpEarlyReminder.Value = new System.DateTime(2007, 9, 21, 0, 0, 0, 0);
            // 
            // gbGoogle
            // 
            this.gbGoogle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbGoogle.Controls.Add(this.cbGoogleReminder);
            this.gbGoogle.Controls.Add(lblReminder);
            this.gbGoogle.Controls.Add(lblURI);
            this.gbGoogle.Controls.Add(this.tbGoogleURI);
            this.gbGoogle.Controls.Add(this.tbGooglePassword);
            this.gbGoogle.Controls.Add(this.tbGoogleEmail);
            this.gbGoogle.Controls.Add(lblPassword);
            this.gbGoogle.Controls.Add(lblGoogleEmail);
            this.gbGoogle.Enabled = false;
            this.gbGoogle.Location = new System.Drawing.Point(2, 28);
            this.gbGoogle.Name = "gbGoogle";
            this.gbGoogle.Size = new System.Drawing.Size(423, 137);
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
            this.cbGoogleReminder.Location = new System.Drawing.Point(83, 104);
            this.cbGoogleReminder.Name = "cbGoogleReminder";
            this.cbGoogleReminder.Size = new System.Drawing.Size(121, 21);
            this.cbGoogleReminder.TabIndex = 7;
            // 
            // tbGoogleURI
            // 
            this.tbGoogleURI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGoogleURI.Location = new System.Drawing.Point(83, 76);
            this.tbGoogleURI.Name = "tbGoogleURI";
            this.tbGoogleURI.Size = new System.Drawing.Size(334, 21);
            this.tbGoogleURI.TabIndex = 5;
            this.tbGoogleURI.Text = "http://www.google.com/calendar/feeds/default/private/full";
            // 
            // tbGooglePassword
            // 
            this.tbGooglePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGooglePassword.Location = new System.Drawing.Point(83, 49);
            this.tbGooglePassword.Name = "tbGooglePassword";
            this.tbGooglePassword.PasswordChar = '*';
            this.tbGooglePassword.Size = new System.Drawing.Size(334, 21);
            this.tbGooglePassword.TabIndex = 4;
            // 
            // tbGoogleEmail
            // 
            this.tbGoogleEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGoogleEmail.Location = new System.Drawing.Point(83, 21);
            this.tbGoogleEmail.Name = "tbGoogleEmail";
            this.tbGoogleEmail.Size = new System.Drawing.Size(334, 21);
            this.tbGoogleEmail.TabIndex = 3;
            // 
            // rbGoogle
            // 
            this.rbGoogle.AutoSize = true;
            this.rbGoogle.Location = new System.Drawing.Point(88, 4);
            this.rbGoogle.Name = "rbGoogle";
            this.rbGoogle.Size = new System.Drawing.Size(58, 17);
            this.rbGoogle.TabIndex = 2;
            this.rbGoogle.Text = "Google";
            this.rbGoogle.UseVisualStyleBackColor = true;
            this.rbGoogle.Click += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // cbUseAlterateReminder
            // 
            this.cbUseAlterateReminder.AutoSize = true;
            this.cbUseAlterateReminder.Checked = true;
            this.cbUseAlterateReminder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseAlterateReminder.Location = new System.Drawing.Point(11, 198);
            this.cbUseAlterateReminder.Name = "cbUseAlterateReminder";
            this.cbUseAlterateReminder.Size = new System.Drawing.Size(136, 17);
            this.cbUseAlterateReminder.TabIndex = 8;
            this.cbUseAlterateReminder.Text = "Use alternate reminder";
            this.cbUseAlterateReminder.UseVisualStyleBackColor = true;
            // 
            // cbSetReminder
            // 
            this.cbSetReminder.AutoSize = true;
            this.cbSetReminder.Checked = true;
            this.cbSetReminder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSetReminder.Location = new System.Drawing.Point(11, 173);
            this.cbSetReminder.Name = "cbSetReminder";
            this.cbSetReminder.Size = new System.Drawing.Size(71, 17);
            this.cbSetReminder.TabIndex = 6;
            this.cbSetReminder.Text = "Reminder";
            this.cbSetReminder.UseVisualStyleBackColor = true;
            // 
            // externalCalendarCheckbox
            // 
            this.externalCalendarCheckbox.AutoSize = true;
            this.externalCalendarCheckbox.Location = new System.Drawing.Point(6, 98);
            this.externalCalendarCheckbox.Name = "externalCalendarCheckbox";
            this.externalCalendarCheckbox.Size = new System.Drawing.Size(176, 17);
            this.externalCalendarCheckbox.TabIndex = 0;
            this.externalCalendarCheckbox.Text = "Use External Calendar (Import)";
            this.externalCalendarCheckbox.UseVisualStyleBackColor = true;
            this.externalCalendarCheckbox.Click += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // g15Page
            // 
            this.g15Page.Controls.Add(this.g15CheckBox);
            this.g15Page.Controls.Add(label27);
            this.g15Page.Controls.Add(this.g15Panel);
            this.g15Page.Dock = System.Windows.Forms.DockStyle.Fill;
            this.g15Page.Location = new System.Drawing.Point(5, 5);
            this.g15Page.Name = "g15Page";
            this.g15Page.Size = new System.Drawing.Size(435, 426);
            this.g15Page.TabIndex = 13;
            this.g15Page.Text = "g15Page";
            this.g15Page.Visible = false;
            // 
            // g15CheckBox
            // 
            this.g15CheckBox.AutoSize = true;
            this.g15CheckBox.Location = new System.Drawing.Point(6, 102);
            this.g15CheckBox.Name = "g15CheckBox";
            this.g15CheckBox.Size = new System.Drawing.Size(126, 17);
            this.g15CheckBox.TabIndex = 0;
            this.g15CheckBox.Text = "Use G15/G19 Display";
            this.g15CheckBox.UseVisualStyleBackColor = true;
            this.g15CheckBox.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // g15Panel
            // 
            this.g15Panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.g15Panel.Controls.Add(this.cbG15ShowTime);
            this.g15Panel.Controls.Add(this.panel3);
            this.g15Panel.Controls.Add(this.panel1);
            this.g15Panel.Location = new System.Drawing.Point(6, 125);
            this.g15Panel.Margin = new System.Windows.Forms.Padding(0);
            this.g15Panel.Name = "g15Panel";
            this.g15Panel.Size = new System.Drawing.Size(399, 112);
            this.g15Panel.TabIndex = 7;
            // 
            // cbG15ShowTime
            // 
            this.cbG15ShowTime.AutoSize = true;
            this.cbG15ShowTime.Location = new System.Drawing.Point(10, 59);
            this.cbG15ShowTime.Name = "cbG15ShowTime";
            this.cbG15ShowTime.Size = new System.Drawing.Size(115, 17);
            this.cbG15ShowTime.TabIndex = 8;
            this.cbG15ShowTime.Text = "Show System Time";
            this.cbG15ShowTime.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.cbG15CycleTimes);
            this.panel3.Controls.Add(this.ACycleTimesInterval);
            this.panel3.Controls.Add(label14);
            this.panel3.Location = new System.Drawing.Point(7, 29);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(332, 28);
            this.panel3.TabIndex = 7;
            // 
            // cbG15CycleTimes
            // 
            this.cbG15CycleTimes.AutoSize = true;
            this.cbG15CycleTimes.Location = new System.Drawing.Point(3, 5);
            this.cbG15CycleTimes.Margin = new System.Windows.Forms.Padding(3, 5, 0, 3);
            this.cbG15CycleTimes.Name = "cbG15CycleTimes";
            this.cbG15CycleTimes.Size = new System.Drawing.Size(220, 17);
            this.cbG15CycleTimes.TabIndex = 7;
            this.cbG15CycleTimes.Text = "Cycle training and completion time every";
            this.cbG15CycleTimes.UseVisualStyleBackColor = true;
            this.cbG15CycleTimes.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // ACycleTimesInterval
            // 
            this.ACycleTimesInterval.AutoSize = true;
            this.ACycleTimesInterval.Location = new System.Drawing.Point(229, 4);
            this.ACycleTimesInterval.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.ACycleTimesInterval.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.ACycleTimesInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ACycleTimesInterval.Name = "ACycleTimesInterval";
            this.ACycleTimesInterval.Size = new System.Drawing.Size(45, 21);
            this.ACycleTimesInterval.TabIndex = 8;
            this.ACycleTimesInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.cbG15ACycle);
            this.panel1.Controls.Add(this.ACycleInterval);
            this.panel1.Controls.Add(label9);
            this.panel1.Location = new System.Drawing.Point(7, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 28);
            this.panel1.TabIndex = 6;
            // 
            // cbG15ACycle
            // 
            this.cbG15ACycle.AutoSize = true;
            this.cbG15ACycle.Location = new System.Drawing.Point(3, 5);
            this.cbG15ACycle.Margin = new System.Windows.Forms.Padding(3, 5, 0, 3);
            this.cbG15ACycle.Name = "cbG15ACycle";
            this.cbG15ACycle.Size = new System.Drawing.Size(114, 17);
            this.cbG15ACycle.TabIndex = 4;
            this.cbG15ACycle.Text = "Cycle Chars every";
            this.cbG15ACycle.UseVisualStyleBackColor = true;
            this.cbG15ACycle.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // ACycleInterval
            // 
            this.ACycleInterval.AutoSize = true;
            this.ACycleInterval.Location = new System.Drawing.Point(122, 4);
            this.ACycleInterval.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.ACycleInterval.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.ACycleInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ACycleInterval.Name = "ACycleInterval";
            this.ACycleInterval.Size = new System.Drawing.Size(45, 21);
            this.ACycleInterval.TabIndex = 5;
            this.ACycleInterval.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // igbServerPage
            // 
            this.igbServerPage.Controls.Add(this.igbCheckBox);
            this.igbServerPage.Controls.Add(label30);
            this.igbServerPage.Controls.Add(this.igbFlowPanel);
            this.igbServerPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.igbServerPage.Location = new System.Drawing.Point(5, 5);
            this.igbServerPage.Name = "igbServerPage";
            this.igbServerPage.Size = new System.Drawing.Size(435, 426);
            this.igbServerPage.TabIndex = 14;
            this.igbServerPage.Text = "igbServerPage";
            this.igbServerPage.Visible = false;
            // 
            // igbCheckBox
            // 
            this.igbCheckBox.AutoSize = true;
            this.igbCheckBox.Location = new System.Drawing.Point(7, 115);
            this.igbCheckBox.Name = "igbCheckBox";
            this.igbCheckBox.Size = new System.Drawing.Size(121, 17);
            this.igbCheckBox.TabIndex = 3;
            this.igbCheckBox.Text = "Run IGB Mini-server";
            this.igbCheckBox.UseVisualStyleBackColor = true;
            this.igbCheckBox.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // igbFlowPanel
            // 
            this.igbFlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.igbFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.igbFlowPanel.Controls.Add(this.flowLayoutPanel27);
            this.igbFlowPanel.Controls.Add(flowLayoutPanel28);
            this.igbFlowPanel.Controls.Add(this.cbIGBPublic);
            this.igbFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.igbFlowPanel.Location = new System.Drawing.Point(7, 138);
            this.igbFlowPanel.Name = "igbFlowPanel";
            this.igbFlowPanel.Padding = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this.igbFlowPanel.Size = new System.Drawing.Size(413, 95);
            this.igbFlowPanel.TabIndex = 4;
            // 
            // flowLayoutPanel27
            // 
            this.flowLayoutPanel27.Controls.Add(label23);
            this.flowLayoutPanel27.Controls.Add(this.igbPortTextBox);
            this.flowLayoutPanel27.Location = new System.Drawing.Point(12, 3);
            this.flowLayoutPanel27.Name = "flowLayoutPanel27";
            this.flowLayoutPanel27.Size = new System.Drawing.Size(125, 28);
            this.flowLayoutPanel27.TabIndex = 7;
            // 
            // igbPortTextBox
            // 
            this.igbPortTextBox.Location = new System.Drawing.Point(60, 3);
            this.igbPortTextBox.Name = "igbPortTextBox";
            this.igbPortTextBox.Size = new System.Drawing.Size(35, 21);
            this.igbPortTextBox.TabIndex = 8;
            this.igbPortTextBox.Text = "80";
            // 
            // cbIGBPublic
            // 
            this.cbIGBPublic.AutoSize = true;
            this.cbIGBPublic.Location = new System.Drawing.Point(12, 65);
            this.cbIGBPublic.Name = "cbIGBPublic";
            this.cbIGBPublic.Size = new System.Drawing.Size(162, 17);
            this.cbIGBPublic.TabIndex = 4;
            this.cbIGBPublic.Text = "Make IGB Mini-server public?";
            this.cbIGBPublic.UseVisualStyleBackColor = true;
            // 
            // relocationPage
            // 
            this.relocationPage.Controls.Add(this.relocationCheckEveryLabel);
            this.relocationPage.Controls.Add(this.relocationSecondsLabel);
            this.relocationPage.Controls.Add(this.relocationSecondsNumericUpDown);
            this.relocationPage.Controls.Add(this.enableAutomaticRelocationCheckBox);
            this.relocationPage.Controls.Add(this.showRelocationMenuCheckbox);
            this.relocationPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.relocationPage.Location = new System.Drawing.Point(5, 5);
            this.relocationPage.Name = "relocationPage";
            this.relocationPage.Size = new System.Drawing.Size(435, 426);
            this.relocationPage.TabIndex = 15;
            this.relocationPage.Text = "relocationPage";
            // 
            // relocationCheckEveryLabel
            // 
            this.relocationCheckEveryLabel.AutoSize = true;
            this.relocationCheckEveryLabel.Location = new System.Drawing.Point(40, 97);
            this.relocationCheckEveryLabel.Name = "relocationCheckEveryLabel";
            this.relocationCheckEveryLabel.Size = new System.Drawing.Size(67, 13);
            this.relocationCheckEveryLabel.TabIndex = 5;
            this.relocationCheckEveryLabel.Text = "Check every";
            // 
            // relocationSecondsLabel
            // 
            this.relocationSecondsLabel.AutoSize = true;
            this.relocationSecondsLabel.Location = new System.Drawing.Point(168, 97);
            this.relocationSecondsLabel.Name = "relocationSecondsLabel";
            this.relocationSecondsLabel.Size = new System.Drawing.Size(46, 13);
            this.relocationSecondsLabel.TabIndex = 4;
            this.relocationSecondsLabel.Text = "seconds";
            // 
            // relocationSecondsNumericUpDown
            // 
            this.relocationSecondsNumericUpDown.Location = new System.Drawing.Point(113, 95);
            this.relocationSecondsNumericUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.relocationSecondsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.relocationSecondsNumericUpDown.Name = "relocationSecondsNumericUpDown";
            this.relocationSecondsNumericUpDown.Size = new System.Drawing.Size(49, 21);
            this.relocationSecondsNumericUpDown.TabIndex = 3;
            this.relocationSecondsNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // enableAutomaticRelocationCheckBox
            // 
            this.enableAutomaticRelocationCheckBox.AutoSize = true;
            this.enableAutomaticRelocationCheckBox.Location = new System.Drawing.Point(22, 63);
            this.enableAutomaticRelocationCheckBox.Name = "enableAutomaticRelocationCheckBox";
            this.enableAutomaticRelocationCheckBox.Size = new System.Drawing.Size(158, 17);
            this.enableAutomaticRelocationCheckBox.TabIndex = 2;
            this.enableAutomaticRelocationCheckBox.Text = "Enable automatic relocation";
            this.enableAutomaticRelocationCheckBox.UseVisualStyleBackColor = true;
            this.enableAutomaticRelocationCheckBox.CheckedChanged += new System.EventHandler(this.OnMustEnableOrDisable);
            // 
            // showRelocationMenuCheckbox
            // 
            this.showRelocationMenuCheckbox.AutoSize = true;
            this.showRelocationMenuCheckbox.Location = new System.Drawing.Point(22, 30);
            this.showRelocationMenuCheckbox.Name = "showRelocationMenuCheckbox";
            this.showRelocationMenuCheckbox.Size = new System.Drawing.Size(131, 17);
            this.showRelocationMenuCheckbox.TabIndex = 1;
            this.showRelocationMenuCheckbox.Text = "Show relocation menu";
            this.showRelocationMenuCheckbox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(644, 482);
            this.Controls.Add(this.multiPanel);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(bottomPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EVEMon Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            bottomPanel.ResumeLayout(false);
            groupBox15.ResumeLayout(false);
            groupBox15.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            flowLayoutPanel28.ResumeLayout(false);
            flowLayoutPanel28.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel9.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.multiPanel.ResumeLayout(false);
            this.mainWindowPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.overviewPanel.ResumeLayout(false);
            this.overviewPanel.PerformLayout();
            this.generalPage.ResumeLayout(false);
            this.generalPage.PerformLayout();
            this.skillPlannerPage.ResumeLayout(false);
            this.skillPlannerPage.PerformLayout();
            this.networkPage.ResumeLayout(false);
            this.ApiProxyGroupBox.ResumeLayout(false);
            this.ProxyServerGroupBox.ResumeLayout(false);
            this.ProxyServerGroupBox.PerformLayout();
            this.customProxyPanel.ResumeLayout(false);
            this.customProxyPanel.PerformLayout();
            this.emailNotificationsPage.ResumeLayout(false);
            this.emailNotificationsPage.PerformLayout();
            this.mailNotificationPanel.ResumeLayout(false);
            this.mailNotificationPanel.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tlpEmailSettings.ResumeLayout(false);
            this.tlpEmailSettings.PerformLayout();
            this.tlpEmailAuthTable.ResumeLayout(false);
            this.tlpEmailAuthTable.PerformLayout();
            this.notificationsPage.ResumeLayout(false);
            this.notificationsPage.PerformLayout();
            this.trayIconPage.ResumeLayout(false);
            this.WindowBehaviourGroupBox.ResumeLayout(false);
            this.WindowBehaviourGroupBox.PerformLayout();
            this.trayIconPopupGroupBox.ResumeLayout(false);
            this.trayIconPopupGroupBox.PerformLayout();
            this.updatesPage.ResumeLayout(false);
            this.updatesPage.PerformLayout();
            this.schedulerUIPage.ResumeLayout(false);
            this.schedulerUIPage.PerformLayout();
            this.externalCalendarPage.ResumeLayout(false);
            this.externalCalendarPage.PerformLayout();
            this.externalCalendarPanel.ResumeLayout(false);
            this.externalCalendarPanel.PerformLayout();
            this.gbGoogle.ResumeLayout(false);
            this.gbGoogle.PerformLayout();
            this.g15Page.ResumeLayout(false);
            this.g15Page.PerformLayout();
            this.g15Panel.ResumeLayout(false);
            this.g15Panel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ACycleTimesInterval)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ACycleInterval)).EndInit();
            this.igbServerPage.ResumeLayout(false);
            this.igbServerPage.PerformLayout();
            this.igbFlowPanel.ResumeLayout(false);
            this.igbFlowPanel.PerformLayout();
            this.flowLayoutPanel27.ResumeLayout(false);
            this.flowLayoutPanel27.PerformLayout();
            this.relocationPage.ResumeLayout(false);
            this.relocationPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.relocationSecondsNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox mailNotificationCheckBox;
        private System.Windows.Forms.TableLayoutPanel tlpEmailSettings;
        private System.Windows.Forms.TextBox tbMailServer;
        private System.Windows.Forms.TextBox tbFromAddress;
        private System.Windows.Forms.TextBox tbToAddress;
        private System.Windows.Forms.Button btnTestEmail;
        private System.Windows.Forms.CheckBox cbEmailServerRequireSsl;
        private System.Windows.Forms.TextBox tbEmailPassword;
        private System.Windows.Forms.TextBox tbEmailUsername;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox cbEmailAuthRequired;
        private System.Windows.Forms.TableLayoutPanel tlpEmailAuthTable;
        private System.Windows.Forms.CheckBox cbPlaySoundOnSkillComplete;
        private System.Windows.Forms.FlowLayoutPanel mailNotificationPanel;
        private System.Windows.Forms.GroupBox ProxyServerGroupBox;
        private System.Windows.Forms.TextBox proxyPortTextBox;
        private System.Windows.Forms.TextBox proxyHttpHostTextBox;
        private System.Windows.Forms.Button proxyAuthenticationButton;
        private System.Windows.Forms.CheckBox cbAutomaticallySearchForNewVersions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox cbWorksafeMode;
        private System.Windows.Forms.CheckBox cbHighlightPlannedSkills;
        private System.Windows.Forms.CheckBox cbHighlightPrerequisites;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TreeView tvlist;
        private System.Windows.Forms.ComboBox cbSkillIconSet;
        private System.Windows.Forms.TextBox emailPortTextBox;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.CheckBox runAtStartupComboBox;
        private System.Windows.Forms.CheckBox cbTitleToTime;
        private System.Windows.Forms.ComboBox cbWindowsTitleList;
        private System.Windows.Forms.CheckBox g15CheckBox;
        private System.Windows.Forms.RadioButton rbSystemTrayOptionsNever;
        private System.Windows.Forms.RadioButton rbSystemTrayOptionsMinimized;
        private System.Windows.Forms.RadioButton rbSystemTrayOptionsAlways;
        private System.Windows.Forms.CheckBox cbEmailUseShortFormat;
        private System.Windows.Forms.FlowLayoutPanel igbFlowPanel;
        private System.Windows.Forms.CheckBox igbCheckBox;
        private System.Windows.Forms.CheckBox cbIGBPublic;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel27;
        private System.Windows.Forms.ToolTip ttToolTipCodes;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox cbSkillInTitle;
        private System.Windows.Forms.CheckBox cbHighlightConflicts;
        private System.Windows.Forms.CheckBox cbHighlightPartialSkills;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Panel panelColorBlocking;
        private System.Windows.Forms.Panel panelColorRecurring2;
        private System.Windows.Forms.Panel panelColorRecurring1;
        private System.Windows.Forms.Panel panelColorSingle2;
        private System.Windows.Forms.Panel panelColorSingle1;
        private System.Windows.Forms.Panel panelColorText;
        private System.Windows.Forms.Button trayTooltipButton;
        private System.Windows.Forms.CheckBox externalCalendarCheckbox;
        private System.Windows.Forms.TextBox tbReminder;
        private System.Windows.Forms.CheckBox cbSetReminder;
        private System.Windows.Forms.DateTimePicker dtpLateReminder;
        private System.Windows.Forms.DateTimePicker dtpEarlyReminder;
        private System.Windows.Forms.CheckBox cbUseAlterateReminder;
        private System.Windows.Forms.GroupBox gbGoogle;
        private System.Windows.Forms.ComboBox cbGoogleReminder;
        private System.Windows.Forms.TextBox tbGoogleURI;
        private System.Windows.Forms.TextBox tbGooglePassword;
        private System.Windows.Forms.TextBox tbGoogleEmail;
        private System.Windows.Forms.RadioButton rbGoogle;
        private System.Windows.Forms.RadioButton rbMSOutlook;
        private System.Windows.Forms.CheckBox cbShowAllPublicSkills;
        private System.Windows.Forms.CheckBox cbShowNonPublicSkills;
        private System.Windows.Forms.ComboBox cbAPIServer;
        private System.Windows.Forms.Button btnAddAPIServer;
        private System.Windows.Forms.Button btnEditAPIServer;
        private System.Windows.Forms.Button btnDeleteAPIServer;
        private System.Windows.Forms.CheckBox cbCheckTimeOnStartup;
        private System.Windows.Forms.GroupBox ApiProxyGroupBox;
        private System.Windows.Forms.CheckBox cbShowOverViewTab;
        private System.Windows.Forms.ComboBox compatibilityCombo;
        private System.Windows.Forms.TextBox igbUrlTextBox;
        private EVEMon.SettingsUI.NotificationsControl notificationsControl;
        private System.Windows.Forms.TreeView treeView;
        private EVEMon.Controls.MultiPanel multiPanel;
        private EVEMon.Controls.MultiPanelPage generalPage;
        private EVEMon.Controls.MultiPanelPage mainWindowPage;
        private EVEMon.Controls.MultiPanelPage skillPlannerPage;
        private EVEMon.Controls.MultiPanelPage networkPage;
        private EVEMon.Controls.MultiPanelPage emailNotificationsPage;
        private EVEMon.Controls.MultiPanelPage notificationsPage;
        private EVEMon.Controls.MultiPanelPage trayIconPage;
        private EVEMon.Controls.MultiPanelPage updatesPage;
        private EVEMon.Controls.MultiPanelPage schedulerUIPage;
        private EVEMon.Controls.MultiPanelPage externalCalendarPage;
        private System.Windows.Forms.Panel customProxyPanel;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Panel leftPanel;
        private EVEMon.Controls.MultiPanelPage g15Page;
        private EVEMon.Controls.MultiPanelPage igbServerPage;
        private System.Windows.Forms.CheckBox customProxyCheckBox;
        private System.Windows.Forms.GroupBox trayIconPopupGroupBox;
        private System.Windows.Forms.Button trayPopupButton;
        private System.Windows.Forms.RadioButton trayPopupRadio;
        private System.Windows.Forms.RadioButton trayTooltipRadio;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Panel externalCalendarPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox overviewPortraitSizeComboBox;
        private System.Windows.Forms.CheckBox overviewShowPortraitCheckBox;
        private System.Windows.Forms.CheckBox overviewShowWalletCheckBox;
        private System.Windows.Forms.Panel overviewPanel;
        private UpdateSettingsControl updateSettingsControl;
        private System.Windows.Forms.GroupBox WindowBehaviourGroupBox;
        private System.Windows.Forms.RadioButton rbMinToTaskBar;
        private System.Windows.Forms.RadioButton rbMinToTray;
        private System.Windows.Forms.RadioButton rbExitEVEMon;
        private System.Windows.Forms.CheckBox cbAlwaysShowSkillQueueTime;
        private System.Windows.Forms.CheckBox cbColorPartialSkills;
        private System.Windows.Forms.CheckBox cbShowPrereqMetSkills;
        private System.Windows.Forms.CheckBox cbColorQueuedSkills;
        private System.Windows.Forms.CheckBox cbHighlightQueuedSiklls;
        private System.Windows.Forms.CheckBox overviewShowSkillQueueFreeRoomCheckBox;
        private System.Windows.Forms.TextBox igbPortTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbG15ACycle;
        private System.Windows.Forms.NumericUpDown ACycleInterval;
        private System.Windows.Forms.Panel g15Panel;
        private System.Windows.Forms.CheckBox cbG15ShowTime;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox cbG15CycleTimes;
        private System.Windows.Forms.NumericUpDown ACycleTimesInterval;
        private EVEMon.Controls.MultiPanelPage relocationPage;
        private System.Windows.Forms.CheckBox enableAutomaticRelocationCheckBox;
        private System.Windows.Forms.CheckBox showRelocationMenuCheckbox;
        private System.Windows.Forms.Label relocationCheckEveryLabel;
        private System.Windows.Forms.Label relocationSecondsLabel;
        private System.Windows.Forms.NumericUpDown relocationSecondsNumericUpDown;
        private System.Windows.Forms.CheckBox SummaryOnMultiSelectOnlyCheckBox;
    }
}
