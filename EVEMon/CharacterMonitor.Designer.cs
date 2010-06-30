namespace EVEMon
{
    partial class CharacterMonitor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterMonitor));
            this.pnlTraining = new System.Windows.Forms.Panel();
            this.tlpStatus = new System.Windows.Forms.TableLayoutPanel();
            this.flpStatusLabels = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCurrentlyTraining = new System.Windows.Forms.Label();
            this.lblSPPerHour = new System.Windows.Forms.Label();
            this.lblScheduleWarning = new System.Windows.Forms.Label();
            this.flpStatusValues = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTrainingSkill = new System.Windows.Forms.Label();
            this.lblTrainingRemain = new System.Windows.Forms.Label();
            this.lblTrainingEst = new System.Windows.Forms.Label();
            this.btnAddToCalendar = new System.Windows.Forms.Button();
            this.upperPanel = new System.Windows.Forms.Panel();
            this.Header = new EVEMon.CharacterMonitorHeader();
            this.sfdSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.ttToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.skillQueuePanel = new System.Windows.Forms.Panel();
            this.lblPaused = new System.Windows.Forms.Label();
            this.skillQueueTimePanel = new System.Windows.Forms.Panel();
            this.lblQueueCompletionTime = new System.Windows.Forms.Label();
            this.lblQueueRemaining = new System.Windows.Forms.Label();
            this.skillQueueControl = new EVEMon.SkillQueueControl();
            this.lowerPanel = new System.Windows.Forms.Panel();
            this.skillsPanel = new EVEMon.Controls.BorderPanel();
            this.corePanel = new System.Windows.Forms.Panel();
            this.multiPanel = new EVEMon.Controls.MultiPanel();
            this.skillsPage = new EVEMon.Controls.MultiPanelPage();
            this.skillsList = new EVEMon.MainWindowSkillsList();
            this.ordersPage = new EVEMon.Controls.MultiPanelPage();
            this.ordersList = new EVEMon.MainWindowMarketOrdersList();
            this.skillQueuePage = new EVEMon.Controls.MultiPanelPage();
            this.skillQueueList = new EVEMon.MainWindowSkillsQueueList();
            this.jobsPage = new EVEMon.Controls.MultiPanelPage();
            this.jobsList = new EVEMon.MainWindowIndustryJobsList();
            this.warningLabel = new System.Windows.Forms.Label();
            this.notificationList = new EVEMon.NotificationList();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.skillsIcon = new System.Windows.Forms.ToolStripButton();
            this.skillQueueIcon = new System.Windows.Forms.ToolStripButton();
            this.ordersIcon = new System.Windows.Forms.ToolStripButton();
            this.jobsIcon = new System.Windows.Forms.ToolStripButton();
            this.toggleSkillsIcon = new System.Windows.Forms.ToolStripButton();
            this.preferencesMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.columnSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsColumnSettingsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.hideInactiveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numberAbsFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsOptionsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.showOnlyCharMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyCorpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.groupMenu = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pnlTraining.SuspendLayout();
            this.tlpStatus.SuspendLayout();
            this.flpStatusLabels.SuspendLayout();
            this.flpStatusValues.SuspendLayout();
            this.upperPanel.SuspendLayout();
            this.skillQueuePanel.SuspendLayout();
            this.skillQueueTimePanel.SuspendLayout();
            this.lowerPanel.SuspendLayout();
            this.skillsPanel.SuspendLayout();
            this.corePanel.SuspendLayout();
            this.multiPanel.SuspendLayout();
            this.skillsPage.SuspendLayout();
            this.ordersPage.SuspendLayout();
            this.skillQueuePage.SuspendLayout();
            this.jobsPage.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // pnlTraining
            // 
            this.pnlTraining.AutoSize = true;
            this.pnlTraining.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlTraining.Controls.Add(this.tlpStatus);
            this.pnlTraining.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTraining.Location = new System.Drawing.Point(0, 296);
            this.pnlTraining.Name = "pnlTraining";
            this.pnlTraining.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.pnlTraining.Size = new System.Drawing.Size(574, 42);
            this.pnlTraining.TabIndex = 1;
            this.pnlTraining.Visible = false;
            // 
            // tlpStatus
            // 
            this.tlpStatus.AutoSize = true;
            this.tlpStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpStatus.ColumnCount = 3;
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStatus.Controls.Add(this.flpStatusLabels, 0, 0);
            this.tlpStatus.Controls.Add(this.flpStatusValues, 1, 0);
            this.tlpStatus.Controls.Add(this.btnAddToCalendar, 2, 0);
            this.tlpStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpStatus.Location = new System.Drawing.Point(0, 3);
            this.tlpStatus.Margin = new System.Windows.Forms.Padding(0);
            this.tlpStatus.Name = "tlpStatus";
            this.tlpStatus.RowCount = 1;
            this.tlpStatus.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpStatus.Size = new System.Drawing.Size(574, 39);
            this.tlpStatus.TabIndex = 4;
            // 
            // flpStatusLabels
            // 
            this.flpStatusLabels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpStatusLabels.AutoSize = true;
            this.flpStatusLabels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpStatusLabels.Controls.Add(this.lblCurrentlyTraining);
            this.flpStatusLabels.Controls.Add(this.lblSPPerHour);
            this.flpStatusLabels.Controls.Add(this.lblScheduleWarning);
            this.flpStatusLabels.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpStatusLabels.Location = new System.Drawing.Point(0, 0);
            this.flpStatusLabels.Margin = new System.Windows.Forms.Padding(0);
            this.flpStatusLabels.Name = "flpStatusLabels";
            this.flpStatusLabels.Size = new System.Drawing.Size(96, 39);
            this.flpStatusLabels.TabIndex = 15;
            this.flpStatusLabels.WrapContents = false;
            // 
            // lblCurrentlyTraining
            // 
            this.lblCurrentlyTraining.AutoSize = true;
            this.lblCurrentlyTraining.Location = new System.Drawing.Point(0, 0);
            this.lblCurrentlyTraining.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCurrentlyTraining.Name = "lblCurrentlyTraining";
            this.lblCurrentlyTraining.Size = new System.Drawing.Size(92, 13);
            this.lblCurrentlyTraining.TabIndex = 2;
            this.lblCurrentlyTraining.Text = "Currently Training:";
            // 
            // lblSPPerHour
            // 
            this.lblSPPerHour.AutoSize = true;
            this.lblSPPerHour.Location = new System.Drawing.Point(0, 13);
            this.lblSPPerHour.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblSPPerHour.Name = "lblSPPerHour";
            this.lblSPPerHour.Size = new System.Drawing.Size(54, 13);
            this.lblSPPerHour.TabIndex = 0;
            this.lblSPPerHour.Text = "X sp/hour";
            // 
            // lblScheduleWarning
            // 
            this.lblScheduleWarning.AutoSize = true;
            this.lblScheduleWarning.ForeColor = System.Drawing.Color.Red;
            this.lblScheduleWarning.Location = new System.Drawing.Point(0, 26);
            this.lblScheduleWarning.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblScheduleWarning.Name = "lblScheduleWarning";
            this.lblScheduleWarning.Size = new System.Drawing.Size(93, 13);
            this.lblScheduleWarning.TabIndex = 1;
            this.lblScheduleWarning.Text = "Schedule Conflict!";
            this.lblScheduleWarning.Visible = false;
            // 
            // flpStatusValues
            // 
            this.flpStatusValues.AutoSize = true;
            this.flpStatusValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpStatusValues.Controls.Add(this.lblTrainingSkill);
            this.flpStatusValues.Controls.Add(this.lblTrainingRemain);
            this.flpStatusValues.Controls.Add(this.lblTrainingEst);
            this.flpStatusValues.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpStatusValues.Location = new System.Drawing.Point(96, 0);
            this.flpStatusValues.Margin = new System.Windows.Forms.Padding(0);
            this.flpStatusValues.Name = "flpStatusValues";
            this.flpStatusValues.Size = new System.Drawing.Size(47, 39);
            this.flpStatusValues.TabIndex = 5;
            // 
            // lblTrainingSkill
            // 
            this.lblTrainingSkill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrainingSkill.AutoSize = true;
            this.lblTrainingSkill.Location = new System.Drawing.Point(0, 0);
            this.lblTrainingSkill.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblTrainingSkill.Name = "lblTrainingSkill";
            this.lblTrainingSkill.Size = new System.Drawing.Size(44, 13);
            this.lblTrainingSkill.TabIndex = 0;
            this.lblTrainingSkill.Text = "Nothing";
            // 
            // lblTrainingRemain
            // 
            this.lblTrainingRemain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrainingRemain.AutoSize = true;
            this.lblTrainingRemain.Location = new System.Drawing.Point(0, 13);
            this.lblTrainingRemain.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblTrainingRemain.Name = "lblTrainingRemain";
            this.lblTrainingRemain.Size = new System.Drawing.Size(10, 13);
            this.lblTrainingRemain.TabIndex = 1;
            this.lblTrainingRemain.Text = ".";
            // 
            // lblTrainingEst
            // 
            this.lblTrainingEst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrainingEst.AutoSize = true;
            this.lblTrainingEst.Location = new System.Drawing.Point(0, 26);
            this.lblTrainingEst.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblTrainingEst.Name = "lblTrainingEst";
            this.lblTrainingEst.Size = new System.Drawing.Size(10, 13);
            this.lblTrainingEst.TabIndex = 2;
            this.lblTrainingEst.Text = ".";
            // 
            // btnAddToCalendar
            // 
            this.btnAddToCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddToCalendar.Location = new System.Drawing.Point(462, 13);
            this.btnAddToCalendar.Name = "btnAddToCalendar";
            this.btnAddToCalendar.Size = new System.Drawing.Size(109, 23);
            this.btnAddToCalendar.TabIndex = 0;
            this.btnAddToCalendar.Text = "Update Calendar";
            this.btnAddToCalendar.UseVisualStyleBackColor = true;
            this.btnAddToCalendar.Click += new System.EventHandler(this.btnAddToCalendar_Click);
            // 
            // upperPanel
            // 
            this.upperPanel.AutoSize = true;
            this.upperPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.upperPanel.Controls.Add(this.Header);
            this.upperPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.upperPanel.Location = new System.Drawing.Point(0, 0);
            this.upperPanel.Name = "upperPanel";
            this.upperPanel.Size = new System.Drawing.Size(574, 136);
            this.upperPanel.TabIndex = 14;
            // 
            // Header
            // 
            this.Header.Character = null;
            this.Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.Header.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Header.Location = new System.Drawing.Point(0, 0);
            this.Header.Name = "Header";
            this.Header.Size = new System.Drawing.Size(574, 136);
            this.Header.TabIndex = 15;
            // 
            // ttToolTip
            // 
            this.ttToolTip.AutoPopDelay = 5000000;
            this.ttToolTip.InitialDelay = 500;
            this.ttToolTip.IsBalloon = true;
            this.ttToolTip.ReshowDelay = 100;
            // 
            // skillQueuePanel
            // 
            this.skillQueuePanel.AutoSize = true;
            this.skillQueuePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.skillQueuePanel.Controls.Add(this.lblPaused);
            this.skillQueuePanel.Controls.Add(this.skillQueueTimePanel);
            this.skillQueuePanel.Controls.Add(this.skillQueueControl);
            this.skillQueuePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.skillQueuePanel.Location = new System.Drawing.Point(0, 240);
            this.skillQueuePanel.Name = "skillQueuePanel";
            this.skillQueuePanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.skillQueuePanel.Size = new System.Drawing.Size(574, 56);
            this.skillQueuePanel.TabIndex = 13;
            // 
            // lblPaused
            // 
            this.lblPaused.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblPaused.Location = new System.Drawing.Point(0, 23);
            this.lblPaused.Name = "lblPaused";
            this.lblPaused.Size = new System.Drawing.Size(574, 17);
            this.lblPaused.TabIndex = 3;
            this.lblPaused.Text = "Paused";
            this.lblPaused.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // skillQueueTimePanel
            // 
            this.skillQueueTimePanel.Controls.Add(this.lblQueueCompletionTime);
            this.skillQueueTimePanel.Controls.Add(this.lblQueueRemaining);
            this.skillQueueTimePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.skillQueueTimePanel.Location = new System.Drawing.Point(0, 6);
            this.skillQueueTimePanel.Name = "skillQueueTimePanel";
            this.skillQueueTimePanel.Size = new System.Drawing.Size(574, 17);
            this.skillQueueTimePanel.TabIndex = 17;
            // 
            // lblQueueCompletionTime
            // 
            this.lblQueueCompletionTime.AutoSize = true;
            this.lblQueueCompletionTime.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblQueueCompletionTime.Location = new System.Drawing.Point(530, 0);
            this.lblQueueCompletionTime.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblQueueCompletionTime.Name = "lblQueueCompletionTime";
            this.lblQueueCompletionTime.Size = new System.Drawing.Size(44, 13);
            this.lblQueueCompletionTime.TabIndex = 15;
            this.lblQueueCompletionTime.Text = "Nothing";
            // 
            // lblQueueRemaining
            // 
            this.lblQueueRemaining.AutoSize = true;
            this.lblQueueRemaining.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblQueueRemaining.Location = new System.Drawing.Point(0, 0);
            this.lblQueueRemaining.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblQueueRemaining.Name = "lblQueueRemaining";
            this.lblQueueRemaining.Size = new System.Drawing.Size(44, 13);
            this.lblQueueRemaining.TabIndex = 16;
            this.lblQueueRemaining.Text = "Nothing";
            // 
            // skillQueueControl
            // 
            this.skillQueueControl.BackColor = System.Drawing.SystemColors.Control;
            this.skillQueueControl.BorderColor = System.Drawing.Color.Gray;
            this.skillQueueControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.skillQueueControl.EmptyColor = System.Drawing.Color.DimGray;
            this.skillQueueControl.FirstColor = System.Drawing.Color.LightSteelBlue;
            this.skillQueueControl.Location = new System.Drawing.Point(0, 40);
            this.skillQueueControl.Name = "skillQueueControl";
            this.skillQueueControl.SecondColor = System.Drawing.Color.LightSlateGray;
            this.skillQueueControl.Size = new System.Drawing.Size(574, 10);
            this.skillQueueControl.SkillQueue = null;
            this.skillQueueControl.TabIndex = 13;
            // 
            // lowerPanel
            // 
            this.lowerPanel.Controls.Add(this.skillsPanel);
            this.lowerPanel.Controls.Add(this.skillQueuePanel);
            this.lowerPanel.Controls.Add(this.pnlTraining);
            this.lowerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lowerPanel.Location = new System.Drawing.Point(0, 161);
            this.lowerPanel.Name = "lowerPanel";
            this.lowerPanel.Size = new System.Drawing.Size(574, 338);
            this.lowerPanel.TabIndex = 3;
            // 
            // skillsPanel
            // 
            this.skillsPanel.BackColor = System.Drawing.SystemColors.Window;
            this.skillsPanel.Controls.Add(this.corePanel);
            this.skillsPanel.Controls.Add(this.notificationList);
            this.skillsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillsPanel.ForeColor = System.Drawing.Color.Gray;
            this.skillsPanel.Location = new System.Drawing.Point(0, 0);
            this.skillsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.skillsPanel.Name = "skillsPanel";
            this.skillsPanel.Padding = new System.Windows.Forms.Padding(2, 2, 1, 2);
            this.skillsPanel.Size = new System.Drawing.Size(574, 240);
            this.skillsPanel.TabIndex = 3;
            // 
            // corePanel
            // 
            this.corePanel.Controls.Add(this.multiPanel);
            this.corePanel.Controls.Add(this.warningLabel);
            this.corePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.corePanel.Location = new System.Drawing.Point(2, 74);
            this.corePanel.Margin = new System.Windows.Forms.Padding(0);
            this.corePanel.Name = "corePanel";
            this.corePanel.Padding = new System.Windows.Forms.Padding(1, 1, 2, 0);
            this.corePanel.Size = new System.Drawing.Size(571, 164);
            this.corePanel.TabIndex = 14;
            // 
            // multiPanel
            // 
            this.multiPanel.Controls.Add(this.skillsPage);
            this.multiPanel.Controls.Add(this.ordersPage);
            this.multiPanel.Controls.Add(this.skillQueuePage);
            this.multiPanel.Controls.Add(this.jobsPage);
            this.multiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiPanel.Location = new System.Drawing.Point(1, 18);
            this.multiPanel.Name = "multiPanel";
            this.multiPanel.SelectedPage = this.skillsPage;
            this.multiPanel.Size = new System.Drawing.Size(568, 146);
            this.multiPanel.TabIndex = 14;
            // 
            // skillsPage
            // 
            this.skillsPage.Controls.Add(this.skillsList);
            this.skillsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillsPage.Location = new System.Drawing.Point(0, 0);
            this.skillsPage.Name = "skillsPage";
            this.skillsPage.Size = new System.Drawing.Size(568, 146);
            this.skillsPage.TabIndex = 0;
            this.skillsPage.Text = "skillsPage";
            // 
            // skillsList
            // 
            this.skillsList.Character = null;
            this.skillsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillsList.Location = new System.Drawing.Point(0, 0);
            this.skillsList.Margin = new System.Windows.Forms.Padding(0);
            this.skillsList.Name = "skillsList";
            this.skillsList.Size = new System.Drawing.Size(568, 146);
            this.skillsList.TabIndex = 12;
            // 
            // ordersPage
            // 
            this.ordersPage.Controls.Add(this.ordersList);
            this.ordersPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordersPage.Location = new System.Drawing.Point(0, 0);
            this.ordersPage.Name = "ordersPage";
            this.ordersPage.Size = new System.Drawing.Size(568, 141);
            this.ordersPage.TabIndex = 2;
            this.ordersPage.Text = "ordersPage";
            // 
            // ordersList
            // 
            this.ordersList.Character = null;
            this.ordersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordersList.Grouping = EVEMon.Common.SettingsObjects.MarketOrderGrouping.OrderTypeDesc;
            this.ordersList.Location = new System.Drawing.Point(0, 0);
            this.ordersList.Name = "ordersList";
            this.ordersList.ShowIssuedFor = EVEMon.Common.IssuedFor.All;
            this.ordersList.Size = new System.Drawing.Size(568, 141);
            this.ordersList.TabIndex = 13;
            this.ordersList.TextFilter = "";
            // 
            // skillQueuePage
            // 
            this.skillQueuePage.Controls.Add(this.skillQueueList);
            this.skillQueuePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillQueuePage.Location = new System.Drawing.Point(0, 0);
            this.skillQueuePage.Name = "skillQueuePage";
            this.skillQueuePage.Size = new System.Drawing.Size(568, 141);
            this.skillQueuePage.TabIndex = 1;
            this.skillQueuePage.Text = "skillQueuePage";
            // 
            // skillQueueList
            // 
            this.skillQueueList.Character = null;
            this.skillQueueList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillQueueList.Location = new System.Drawing.Point(0, 0);
            this.skillQueueList.Name = "skillQueueList";
            this.skillQueueList.Size = new System.Drawing.Size(568, 141);
            this.skillQueueList.TabIndex = 0;
            // 
            // jobsPage
            // 
            this.jobsPage.Controls.Add(this.jobsList);
            this.jobsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobsPage.Location = new System.Drawing.Point(0, 0);
            this.jobsPage.Name = "jobsPage";
            this.jobsPage.Size = new System.Drawing.Size(568, 141);
            this.jobsPage.TabIndex = 3;
            this.jobsPage.Text = "industryJobsPage";
            // 
            // jobsList
            // 
            this.jobsList.Character = null;
            this.jobsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobsList.Grouping = EVEMon.Common.SettingsObjects.IndustryJobGrouping.State;
            this.jobsList.Location = new System.Drawing.Point(0, 0);
            this.jobsList.Name = "jobsList";
            this.jobsList.ShowIssuedFor = EVEMon.Common.IssuedFor.All;
            this.jobsList.Size = new System.Drawing.Size(568, 141);
            this.jobsList.TabIndex = 0;
            this.jobsList.TextFilter = "";
            // 
            // warningLabel
            // 
            this.warningLabel.AutoEllipsis = true;
            this.warningLabel.BackColor = System.Drawing.Color.Black;
            this.warningLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.warningLabel.ForeColor = System.Drawing.Color.White;
            this.warningLabel.Image = global::EVEMon.Properties.Resources.APIKeyFull16;
            this.warningLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.warningLabel.Location = new System.Drawing.Point(1, 1);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(568, 17);
            this.warningLabel.TabIndex = 1;
            this.warningLabel.Text = "Key level warning.";
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // notificationList
            // 
            this.notificationList.Dock = System.Windows.Forms.DockStyle.Top;
            this.notificationList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notificationList.ForeColor = System.Drawing.SystemColors.ControlText;
            this.notificationList.Location = new System.Drawing.Point(2, 2);
            this.notificationList.Margin = new System.Windows.Forms.Padding(0);
            this.notificationList.Name = "notificationList";
            this.notificationList.Size = new System.Drawing.Size(571, 72);
            this.notificationList.TabIndex = 13;
            this.notificationList.Resize += new System.EventHandler(this.notificationList_Resize);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skillsIcon,
            this.skillQueueIcon,
            this.ordersIcon,
            this.jobsIcon,
            toolStripSeparator1,
            this.toggleSkillsIcon,
            this.preferencesMenu,
            this.searchTextBox,
            this.groupMenu});
            this.toolStrip.Location = new System.Drawing.Point(0, 136);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(574, 25);
            this.toolStrip.TabIndex = 13;
            // 
            // skillsIcon
            // 
            this.skillsIcon.Checked = true;
            this.skillsIcon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.skillsIcon.Image = global::EVEMon.Properties.Resources.Skills;
            this.skillsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.skillsIcon.Name = "skillsIcon";
            this.skillsIcon.Size = new System.Drawing.Size(53, 22);
            this.skillsIcon.Tag = "skillsPage";
            this.skillsIcon.Text = "Skills";
            this.skillsIcon.ToolTipText = "Display skills list";
            this.skillsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // skillQueueIcon
            // 
            this.skillQueueIcon.Image = global::EVEMon.Properties.Resources.SkillsQueue;
            this.skillQueueIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.skillQueueIcon.Name = "skillQueueIcon";
            this.skillQueueIcon.Size = new System.Drawing.Size(62, 22);
            this.skillQueueIcon.Tag = "skillQueuePage";
            this.skillQueueIcon.Text = "Queue";
            this.skillQueueIcon.ToolTipText = "Display skills in queue";
            this.skillQueueIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // ordersIcon
            // 
            this.ordersIcon.Image = global::EVEMon.Properties.Resources.Money;
            this.ordersIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ordersIcon.Name = "ordersIcon";
            this.ordersIcon.Size = new System.Drawing.Size(102, 22);
            this.ordersIcon.Tag = "ordersPage";
            this.ordersIcon.Text = "Market Orders";
            this.ordersIcon.ToolTipText = "Display market orders";
            this.ordersIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // jobsIcon
            // 
            this.jobsIcon.Image = global::EVEMon.Properties.Resources.Industry;
            this.jobsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.jobsIcon.Name = "jobsIcon";
            this.jobsIcon.Size = new System.Drawing.Size(96, 22);
            this.jobsIcon.Tag = "jobsPage";
            this.jobsIcon.Text = "Industry Jobs";
            this.jobsIcon.ToolTipText = "Display industry jobs";
            this.jobsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // toggleSkillsIcon
            // 
            this.toggleSkillsIcon.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleSkillsIcon.Image = ((System.Drawing.Image)(resources.GetObject("toggleSkillsIcon.Image")));
            this.toggleSkillsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleSkillsIcon.Name = "toggleSkillsIcon";
            this.toggleSkillsIcon.Size = new System.Drawing.Size(110, 22);
            this.toggleSkillsIcon.Text = "Toggle All Skills";
            this.toggleSkillsIcon.ToolTipText = "Toggle all skills";
            this.toggleSkillsIcon.Click += new System.EventHandler(this.toggleSkillsIcon_Click);
            // 
            // preferencesMenu
            // 
            this.preferencesMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.preferencesMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.preferencesMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnSettingsMenuItem,
            this.tsColumnSettingsSeparator,
            this.hideInactiveMenuItem,
            this.numberAbsFormatMenuItem,
            this.tsOptionsSeparator,
            this.showOnlyCharMenuItem,
            this.showOnlyCorpMenuItem});
            this.preferencesMenu.Image = global::EVEMon.Properties.Resources.Settings;
            this.preferencesMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.preferencesMenu.Name = "preferencesMenu";
            this.preferencesMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.preferencesMenu.Size = new System.Drawing.Size(29, 22);
            this.preferencesMenu.Text = "Preferences";
            this.preferencesMenu.ToolTipText = "Preferences";
            this.preferencesMenu.DropDownOpening += new System.EventHandler(this.preferencesMenu_DropDownOpening);
            // 
            // columnSettingsMenuItem
            // 
            this.columnSettingsMenuItem.Name = "columnSettingsMenuItem";
            this.columnSettingsMenuItem.Size = new System.Drawing.Size(252, 22);
            this.columnSettingsMenuItem.Text = "Column Settings";
            this.columnSettingsMenuItem.Click += new System.EventHandler(this.columnSettingsMenuItem_Click);
            // 
            // tsColumnSettingsSeparator
            // 
            this.tsColumnSettingsSeparator.Name = "tsColumnSettingsSeparator";
            this.tsColumnSettingsSeparator.Size = new System.Drawing.Size(249, 6);
            // 
            // hideInactiveMenuItem
            // 
            this.hideInactiveMenuItem.Name = "hideInactiveMenuItem";
            this.hideInactiveMenuItem.Size = new System.Drawing.Size(252, 22);
            this.hideInactiveMenuItem.Text = "Hide Inactive Orders";
            this.hideInactiveMenuItem.Click += new System.EventHandler(this.hideInactiveMenuItem_Click);
            // 
            // numberAbsFormatMenuItem
            // 
            this.numberAbsFormatMenuItem.Name = "numberAbsFormatMenuItem";
            this.numberAbsFormatMenuItem.Size = new System.Drawing.Size(252, 22);
            this.numberAbsFormatMenuItem.Text = "Number Abbreviating Format";
            this.numberAbsFormatMenuItem.Click += new System.EventHandler(this.numberAbsFormatMenuItem_Click);
            // 
            // tsOptionsSeparator
            // 
            this.tsOptionsSeparator.Name = "tsOptionsSeparator";
            this.tsOptionsSeparator.Size = new System.Drawing.Size(249, 6);
            // 
            // showOnlyCharMenuItem
            // 
            this.showOnlyCharMenuItem.CheckOnClick = true;
            this.showOnlyCharMenuItem.Name = "showOnlyCharMenuItem";
            this.showOnlyCharMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showOnlyCharMenuItem.Text = "Show Only Issued for Character";
            this.showOnlyCharMenuItem.Click += new System.EventHandler(this.showOnlyCharMenuItem_Click);
            // 
            // showOnlyCorpMenuItem
            // 
            this.showOnlyCorpMenuItem.CheckOnClick = true;
            this.showOnlyCorpMenuItem.Name = "showOnlyCorpMenuItem";
            this.showOnlyCorpMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showOnlyCorpMenuItem.Text = "Show Only Issued for Corporation";
            this.showOnlyCorpMenuItem.Click += new System.EventHandler(this.showOnlyCorpMenuItem_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.searchTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.searchTextBox.AutoSize = false;
            this.searchTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(120, 21);
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // groupMenu
            // 
            this.groupMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.groupMenu.Image = ((System.Drawing.Image)(resources.GetObject("groupMenu.Image")));
            this.groupMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.groupMenu.Name = "groupMenu";
            this.groupMenu.Size = new System.Drawing.Size(94, 20);
            this.groupMenu.Text = "Group By...";
            this.groupMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.groupMenu_DropDownItemClicked);
            this.groupMenu.DropDownOpening += new System.EventHandler(this.groupMenu_DropDownOpening);
            // 
            // CharacterMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lowerPanel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.upperPanel);
            this.Name = "CharacterMonitor";
            this.Size = new System.Drawing.Size(574, 499);
            this.pnlTraining.ResumeLayout(false);
            this.pnlTraining.PerformLayout();
            this.tlpStatus.ResumeLayout(false);
            this.tlpStatus.PerformLayout();
            this.flpStatusLabels.ResumeLayout(false);
            this.flpStatusLabels.PerformLayout();
            this.flpStatusValues.ResumeLayout(false);
            this.flpStatusValues.PerformLayout();
            this.upperPanel.ResumeLayout(false);
            this.skillQueuePanel.ResumeLayout(false);
            this.skillQueueTimePanel.ResumeLayout(false);
            this.skillQueueTimePanel.PerformLayout();
            this.lowerPanel.ResumeLayout(false);
            this.lowerPanel.PerformLayout();
            this.skillsPanel.ResumeLayout(false);
            this.corePanel.ResumeLayout(false);
            this.multiPanel.ResumeLayout(false);
            this.skillsPage.ResumeLayout(false);
            this.ordersPage.ResumeLayout(false);
            this.skillQueuePage.ResumeLayout(false);
            this.jobsPage.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private MainWindowSkillsList skillsList;
        private System.Windows.Forms.Panel pnlTraining;
        private System.Windows.Forms.Label lblTrainingEst;
        private System.Windows.Forms.Label lblTrainingRemain;
        private System.Windows.Forms.Label lblTrainingSkill;
        private System.Windows.Forms.Label lblCurrentlyTraining;
        private System.Windows.Forms.Panel upperPanel;
        private System.Windows.Forms.SaveFileDialog sfdSaveDialog;
        private System.Windows.Forms.ToolTip ttToolTip;
        private System.Windows.Forms.TableLayoutPanel tlpStatus;
        private System.Windows.Forms.FlowLayoutPanel flpStatusValues;
        private System.Windows.Forms.FlowLayoutPanel flpStatusLabels;
        private System.Windows.Forms.Label lblSPPerHour;
        private System.Windows.Forms.Label lblScheduleWarning;
        private System.Windows.Forms.Button btnAddToCalendar;
        private EVEMon.Controls.BorderPanel skillsPanel;
        private System.Windows.Forms.Panel skillQueuePanel;
        private NotificationList notificationList;
        private System.Windows.Forms.Panel corePanel;
        private System.Windows.Forms.Label lblPaused;
        private EVEMon.Controls.MultiPanel multiPanel;
        private EVEMon.Controls.MultiPanelPage skillsPage;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton skillsIcon;
        private System.Windows.Forms.ToolStripButton ordersIcon;
        private System.Windows.Forms.ToolStripButton toggleSkillsIcon;
        private System.Windows.Forms.Panel lowerPanel;
        private EVEMon.Controls.MultiPanelPage ordersPage;
        private MainWindowMarketOrdersList ordersList;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.ToolStripTextBox searchTextBox;
        private System.Windows.Forms.ToolStripDropDownButton groupMenu;
        private System.Windows.Forms.Label lblQueueRemaining;
        private System.Windows.Forms.Label lblQueueCompletionTime;
        private System.Windows.Forms.Panel skillQueueTimePanel;
        private SkillQueueControl skillQueueControl;
        private System.Windows.Forms.ToolStripDropDownButton preferencesMenu;
        private System.Windows.Forms.ToolStripMenuItem columnSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideInactiveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numberAbsFormatMenuItem;
        private EVEMon.Controls.MultiPanelPage skillQueuePage;
        private MainWindowSkillsQueueList skillQueueList;
        private System.Windows.Forms.ToolStripButton skillQueueIcon;
        private System.Windows.Forms.ToolStripSeparator tsColumnSettingsSeparator;
        private System.Windows.Forms.ToolStripSeparator tsOptionsSeparator;
        private System.Windows.Forms.ToolStripMenuItem showOnlyCharMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlyCorpMenuItem;
        private System.Windows.Forms.ToolStripButton jobsIcon;
        private EVEMon.Controls.MultiPanelPage jobsPage;
        private MainWindowIndustryJobsList jobsList;
        private CharacterMonitorHeader Header;
    }
}