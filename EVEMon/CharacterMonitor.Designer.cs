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
            this.lblCharacterName = new System.Windows.Forms.Label();
            this.lblBioInfo = new System.Windows.Forms.Label();
            this.lblCorpInfo = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
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
            this.tlpInfo = new System.Windows.Forms.TableLayoutPanel();
            this.upperTable = new System.Windows.Forms.TableLayoutPanel();
            this.flpCharacterInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.flpThrobber = new System.Windows.Forms.FlowLayoutPanel();
            this.throbberContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miChangeInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.miQueryEverything = new System.Windows.Forms.ToolStripMenuItem();
            this.throbberSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.lblUpdateTimer = new System.Windows.Forms.Label();
            this.pbCharImage = new EVEMon.Common.Controls.CharacterPortrait();
            this.lowerTable = new System.Windows.Forms.TableLayoutPanel();
            this.lblSkillHeader = new System.Windows.Forms.Label();
            this.attributesFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblIntelligence = new System.Windows.Forms.Label();
            this.lblPerception = new System.Windows.Forms.Label();
            this.lblCharisma = new System.Windows.Forms.Label();
            this.lblWillpower = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.sfdSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.ttToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.skillQueuePanel = new System.Windows.Forms.Panel();
            this.skillQueueTimePanel = new System.Windows.Forms.Panel();
            this.lblQueueCompletionTime = new System.Windows.Forms.Label();
            this.lblQueueRemaining = new System.Windows.Forms.Label();
            this.lblPaused = new System.Windows.Forms.Label();
            this.lowerPanel = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.skillsIcon = new System.Windows.Forms.ToolStripButton();
            this.skillQueueIcon = new System.Windows.Forms.ToolStripButton();
            this.ordersIcon = new System.Windows.Forms.ToolStripButton();
            this.toggleSkillsIcon = new System.Windows.Forms.ToolStripButton();
            this.preferencesMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.columnSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideInactiveOrdersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numberAbsFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.ordersGroupMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.skillsPanel = new EVEMon.Controls.BorderPanel();
            this.corePanel = new System.Windows.Forms.Panel();
            this.multiPanel = new EVEMon.Controls.MultiPanel();
            this.skillsPage = new EVEMon.Controls.MultiPanelPage();
            this.skillsList = new EVEMon.MainWindowSkillsList();
            this.ordersPage = new EVEMon.Controls.MultiPanelPage();
            this.ordersList = new EVEMon.MainWindowMarketOrdersList();
            this.skillQueuePage = new EVEMon.Controls.MultiPanelPage();
            this.skillQueueList = new EVEMon.MainWindowSkillsQueueList();
            this.warningLabel = new System.Windows.Forms.Label();
            this.notificationList = new EVEMon.NotificationList();
            this.skillQueueControl = new EVEMon.SkillQueueControl();
            this.throbber = new EVEMon.Controls.Throbber();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pnlTraining.SuspendLayout();
            this.tlpStatus.SuspendLayout();
            this.flpStatusLabels.SuspendLayout();
            this.flpStatusValues.SuspendLayout();
            this.upperPanel.SuspendLayout();
            this.tlpInfo.SuspendLayout();
            this.upperTable.SuspendLayout();
            this.flpCharacterInfo.SuspendLayout();
            this.flpThrobber.SuspendLayout();
            this.throbberContextMenu.SuspendLayout();
            this.lowerTable.SuspendLayout();
            this.attributesFlowPanel.SuspendLayout();
            this.skillQueuePanel.SuspendLayout();
            this.skillQueueTimePanel.SuspendLayout();
            this.lowerPanel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.skillsPanel.SuspendLayout();
            this.corePanel.SuspendLayout();
            this.multiPanel.SuspendLayout();
            this.skillsPage.SuspendLayout();
            this.ordersPage.SuspendLayout();
            this.skillQueuePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // lblCharacterName
            // 
            this.lblCharacterName.AutoSize = true;
            this.lblCharacterName.Location = new System.Drawing.Point(0, 0);
            this.lblCharacterName.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCharacterName.Name = "lblCharacterName";
            this.lblCharacterName.Size = new System.Drawing.Size(84, 13);
            this.lblCharacterName.TabIndex = 0;
            this.lblCharacterName.Text = "Character Name";
            // 
            // lblBioInfo
            // 
            this.lblBioInfo.AutoSize = true;
            this.lblBioInfo.Location = new System.Drawing.Point(0, 13);
            this.lblBioInfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblBioInfo.Name = "lblBioInfo";
            this.lblBioInfo.Size = new System.Drawing.Size(43, 13);
            this.lblBioInfo.TabIndex = 1;
            this.lblBioInfo.Text = "Bio Info";
            // 
            // lblCorpInfo
            // 
            this.lblCorpInfo.AutoSize = true;
            this.lblCorpInfo.Location = new System.Drawing.Point(0, 26);
            this.lblCorpInfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCorpInfo.Name = "lblCorpInfo";
            this.lblCorpInfo.Size = new System.Drawing.Size(82, 13);
            this.lblCorpInfo.TabIndex = 2;
            this.lblCorpInfo.Text = "Corporation Info";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Location = new System.Drawing.Point(0, 39);
            this.lblBalance.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(93, 13);
            this.lblBalance.TabIndex = 3;
            this.lblBalance.Text = "Balance: 0.00 ISK";
            // 
            // pnlTraining
            // 
            this.pnlTraining.AutoSize = true;
            this.pnlTraining.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlTraining.Controls.Add(this.tlpStatus);
            this.pnlTraining.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTraining.Location = new System.Drawing.Point(0, 291);
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
            this.upperPanel.Controls.Add(this.tlpInfo);
            this.upperPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.upperPanel.Location = new System.Drawing.Point(0, 0);
            this.upperPanel.Name = "upperPanel";
            this.upperPanel.Size = new System.Drawing.Size(574, 141);
            this.upperPanel.TabIndex = 14;
            // 
            // tlpInfo
            // 
            this.tlpInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpInfo.AutoSize = true;
            this.tlpInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpInfo.ColumnCount = 2;
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInfo.Controls.Add(this.upperTable, 1, 0);
            this.tlpInfo.Controls.Add(this.pbCharImage, 0, 0);
            this.tlpInfo.Controls.Add(this.lowerTable, 1, 1);
            this.tlpInfo.Location = new System.Drawing.Point(0, 0);
            this.tlpInfo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.tlpInfo.Name = "tlpInfo";
            this.tlpInfo.RowCount = 2;
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.Size = new System.Drawing.Size(574, 131);
            this.tlpInfo.TabIndex = 0;
            // 
            // upperTable
            // 
            this.upperTable.AutoSize = true;
            this.upperTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.upperTable.ColumnCount = 2;
            this.upperTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.upperTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.upperTable.Controls.Add(this.flpCharacterInfo, 0, 0);
            this.upperTable.Controls.Add(this.flpThrobber, 1, 0);
            this.upperTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.upperTable.Location = new System.Drawing.Point(136, 0);
            this.upperTable.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.upperTable.Name = "upperTable";
            this.upperTable.RowCount = 1;
            this.upperTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.upperTable.Size = new System.Drawing.Size(438, 55);
            this.upperTable.TabIndex = 13;
            // 
            // flpCharacterInfo
            // 
            this.flpCharacterInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCharacterInfo.AutoSize = true;
            this.flpCharacterInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpCharacterInfo.Controls.Add(this.lblCharacterName);
            this.flpCharacterInfo.Controls.Add(this.lblBioInfo);
            this.flpCharacterInfo.Controls.Add(this.lblCorpInfo);
            this.flpCharacterInfo.Controls.Add(this.lblBalance);
            this.flpCharacterInfo.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpCharacterInfo.Location = new System.Drawing.Point(0, 0);
            this.flpCharacterInfo.Margin = new System.Windows.Forms.Padding(0);
            this.flpCharacterInfo.Name = "flpCharacterInfo";
            this.flpCharacterInfo.Size = new System.Drawing.Size(409, 55);
            this.flpCharacterInfo.TabIndex = 18;
            this.flpCharacterInfo.WrapContents = false;
            // 
            // flpThrobber
            // 
            this.flpThrobber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpThrobber.AutoSize = true;
            this.flpThrobber.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpThrobber.Controls.Add(this.throbber);
            this.flpThrobber.Controls.Add(this.lblUpdateTimer);
            this.flpThrobber.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpThrobber.Location = new System.Drawing.Point(409, 0);
            this.flpThrobber.Margin = new System.Windows.Forms.Padding(0);
            this.flpThrobber.Name = "flpThrobber";
            this.flpThrobber.Size = new System.Drawing.Size(29, 37);
            this.flpThrobber.TabIndex = 0;
            this.flpThrobber.WrapContents = false;
            // 
            // throbberContextMenu
            // 
            this.throbberContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miChangeInfo,
            this.miQueryEverything,
            this.throbberSeparator});
            this.throbberContextMenu.Name = "cmsThrobberMenu";
            this.throbberContextMenu.Size = new System.Drawing.Size(234, 54);
            this.throbberContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.throbberContextMenu_ItemClicked);
            this.throbberContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.throbberContextMenu_Opening);
            // 
            // miChangeInfo
            // 
            this.miChangeInfo.Name = "miChangeInfo";
            this.miChangeInfo.Size = new System.Drawing.Size(233, 22);
            this.miChangeInfo.Text = "Change API Key information...";
            this.miChangeInfo.Click += new System.EventHandler(this.miChangeInfo_Click);
            // 
            // miQueryEverything
            // 
            this.miQueryEverything.Name = "miQueryEverything";
            this.miQueryEverything.Size = new System.Drawing.Size(233, 22);
            this.miQueryEverything.Text = "Update Everything";
            this.miQueryEverything.Click += new System.EventHandler(this.miHitEveO_Click);
            // 
            // throbberSeparator
            // 
            this.throbberSeparator.Name = "throbberSeparator";
            this.throbberSeparator.Size = new System.Drawing.Size(230, 6);
            // 
            // lblUpdateTimer
            // 
            this.lblUpdateTimer.AutoSize = true;
            this.lblUpdateTimer.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblUpdateTimer.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblUpdateTimer.Location = new System.Drawing.Point(0, 24);
            this.lblUpdateTimer.Margin = new System.Windows.Forms.Padding(0);
            this.lblUpdateTimer.Name = "lblUpdateTimer";
            this.lblUpdateTimer.Size = new System.Drawing.Size(29, 13);
            this.lblUpdateTimer.TabIndex = 0;
            this.lblUpdateTimer.Text = "timer";
            this.lblUpdateTimer.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblUpdateTimer.Visible = false;
            this.lblUpdateTimer.MouseHover += new System.EventHandler(this.lblUpdateTimer_MouseHover);
            // 
            // pbCharImage
            // 
            this.pbCharImage.Character = null;
            this.pbCharImage.CharacterID = ((long)(-1));
            this.pbCharImage.Location = new System.Drawing.Point(0, 0);
            this.pbCharImage.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.pbCharImage.MinimumSize = new System.Drawing.Size(128, 128);
            this.pbCharImage.Name = "pbCharImage";
            this.tlpInfo.SetRowSpan(this.pbCharImage, 2);
            this.pbCharImage.Size = new System.Drawing.Size(128, 128);
            this.pbCharImage.TabIndex = 0;
            this.pbCharImage.TabStop = false;
            // 
            // lowerTable
            // 
            this.lowerTable.AutoSize = true;
            this.lowerTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.lowerTable.ColumnCount = 2;
            this.lowerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.lowerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lowerTable.Controls.Add(this.lblSkillHeader, 0, 0);
            this.lowerTable.Controls.Add(this.attributesFlowPanel, 0, 0);
            this.lowerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lowerTable.Location = new System.Drawing.Point(136, 60);
            this.lowerTable.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.lowerTable.Name = "lowerTable";
            this.lowerTable.RowCount = 1;
            this.lowerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lowerTable.Size = new System.Drawing.Size(438, 71);
            this.lowerTable.TabIndex = 15;
            // 
            // lblSkillHeader
            // 
            this.lblSkillHeader.AutoSize = true;
            this.lblSkillHeader.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblSkillHeader.Location = new System.Drawing.Point(347, 0);
            this.lblSkillHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblSkillHeader.Name = "lblSkillHeader";
            this.lblSkillHeader.Size = new System.Drawing.Size(91, 71);
            this.lblSkillHeader.TabIndex = 2;
            this.lblSkillHeader.Text = "0 Known Skills\r\n0 Skills at Level V\r\n0 Total SP\r\n0 Clone Limit";
            this.lblSkillHeader.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblSkillHeader.MouseHover += new System.EventHandler(this.lblSkillHeader_MouseHover);
            // 
            // attributesFlowPanel
            // 
            this.attributesFlowPanel.AutoSize = true;
            this.attributesFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.attributesFlowPanel.Controls.Add(this.lblIntelligence);
            this.attributesFlowPanel.Controls.Add(this.lblPerception);
            this.attributesFlowPanel.Controls.Add(this.lblCharisma);
            this.attributesFlowPanel.Controls.Add(this.lblWillpower);
            this.attributesFlowPanel.Controls.Add(this.lblMemory);
            this.attributesFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.attributesFlowPanel.Location = new System.Drawing.Point(0, 0);
            this.attributesFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.attributesFlowPanel.Name = "attributesFlowPanel";
            this.attributesFlowPanel.Size = new System.Drawing.Size(73, 65);
            this.attributesFlowPanel.TabIndex = 3;
            // 
            // lblIntelligence
            // 
            this.lblIntelligence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIntelligence.AutoSize = true;
            this.lblIntelligence.Location = new System.Drawing.Point(0, 0);
            this.lblIntelligence.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblIntelligence.Name = "lblIntelligence";
            this.lblIntelligence.Size = new System.Drawing.Size(70, 13);
            this.lblIntelligence.TabIndex = 5;
            this.lblIntelligence.Text = "0 Intelligence";
            this.lblIntelligence.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblPerception
            // 
            this.lblPerception.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPerception.AutoSize = true;
            this.lblPerception.Location = new System.Drawing.Point(0, 13);
            this.lblPerception.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblPerception.Name = "lblPerception";
            this.lblPerception.Size = new System.Drawing.Size(67, 13);
            this.lblPerception.TabIndex = 6;
            this.lblPerception.Text = "0 Perception";
            this.lblPerception.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblCharisma
            // 
            this.lblCharisma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCharisma.AutoSize = true;
            this.lblCharisma.Location = new System.Drawing.Point(0, 26);
            this.lblCharisma.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCharisma.Name = "lblCharisma";
            this.lblCharisma.Size = new System.Drawing.Size(59, 13);
            this.lblCharisma.TabIndex = 4;
            this.lblCharisma.Text = "0 Charisma";
            this.lblCharisma.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblWillpower
            // 
            this.lblWillpower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWillpower.AutoSize = true;
            this.lblWillpower.Location = new System.Drawing.Point(0, 39);
            this.lblWillpower.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblWillpower.Name = "lblWillpower";
            this.lblWillpower.Size = new System.Drawing.Size(62, 13);
            this.lblWillpower.TabIndex = 8;
            this.lblWillpower.Text = "0 Willpower";
            this.lblWillpower.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblMemory
            // 
            this.lblMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMemory.AutoSize = true;
            this.lblMemory.Location = new System.Drawing.Point(0, 52);
            this.lblMemory.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(53, 13);
            this.lblMemory.TabIndex = 7;
            this.lblMemory.Text = "0 Memory";
            this.lblMemory.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
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
            this.skillQueuePanel.Location = new System.Drawing.Point(0, 235);
            this.skillQueuePanel.Name = "skillQueuePanel";
            this.skillQueuePanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.skillQueuePanel.Size = new System.Drawing.Size(574, 56);
            this.skillQueuePanel.TabIndex = 13;
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
            // lowerPanel
            // 
            this.lowerPanel.Controls.Add(this.skillsPanel);
            this.lowerPanel.Controls.Add(this.skillQueuePanel);
            this.lowerPanel.Controls.Add(this.pnlTraining);
            this.lowerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lowerPanel.Location = new System.Drawing.Point(0, 166);
            this.lowerPanel.Name = "lowerPanel";
            this.lowerPanel.Size = new System.Drawing.Size(574, 333);
            this.lowerPanel.TabIndex = 3;
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skillsIcon,
            this.skillQueueIcon,
            this.ordersIcon,
            toolStripSeparator1,
            this.toggleSkillsIcon,
            this.preferencesMenu,
            this.searchTextBox,
            this.ordersGroupMenu});
            this.toolStrip.Location = new System.Drawing.Point(0, 141);
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
            this.hideInactiveOrdersMenuItem,
            this.numberAbsFormatMenuItem});
            this.preferencesMenu.Image = global::EVEMon.Properties.Resources.Settings;
            this.preferencesMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.preferencesMenu.Name = "preferencesMenu";
            this.preferencesMenu.Size = new System.Drawing.Size(29, 22);
            this.preferencesMenu.Text = "Preferences";
            this.preferencesMenu.ToolTipText = "Preferences";
            this.preferencesMenu.DropDownOpening += new System.EventHandler(this.preferencesMenu_DropDownOpening);
            // 
            // columnSettingsMenuItem
            // 
            this.columnSettingsMenuItem.Name = "columnSettingsMenuItem";
            this.columnSettingsMenuItem.Size = new System.Drawing.Size(230, 22);
            this.columnSettingsMenuItem.Text = "Column Settings";
            this.columnSettingsMenuItem.Click += new System.EventHandler(this.columnSettingsMenuItem_Click);
            // 
            // hideInactiveOrdersMenuItem
            // 
            this.hideInactiveOrdersMenuItem.Name = "hideInactiveOrdersMenuItem";
            this.hideInactiveOrdersMenuItem.Size = new System.Drawing.Size(230, 22);
            this.hideInactiveOrdersMenuItem.Text = "Hide Inactive Orders";
            this.hideInactiveOrdersMenuItem.Click += new System.EventHandler(this.hideInactiveOrdersMenuItem_Click);
            // 
            // numberAbsFormatMenuItem
            // 
            this.numberAbsFormatMenuItem.Name = "numberAbsFormatMenuItem";
            this.numberAbsFormatMenuItem.Size = new System.Drawing.Size(230, 22);
            this.numberAbsFormatMenuItem.Text = "Number Abbreviating Format";
            this.numberAbsFormatMenuItem.Click += new System.EventHandler(this.numberAbsFormatMenuItem_Click);
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
            // ordersGroupMenu
            // 
            this.ordersGroupMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ordersGroupMenu.Image = ((System.Drawing.Image)(resources.GetObject("ordersGroupMenu.Image")));
            this.ordersGroupMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ordersGroupMenu.Name = "ordersGroupMenu";
            this.ordersGroupMenu.Size = new System.Drawing.Size(94, 20);
            this.ordersGroupMenu.Text = "Group By...";
            this.ordersGroupMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ordersGroupMenu_DropDownItemClicked);
            this.ordersGroupMenu.DropDownOpening += new System.EventHandler(this.ordersGroupMenu_DropDownOpening);
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
            this.skillsPanel.Size = new System.Drawing.Size(574, 235);
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
            this.corePanel.Size = new System.Drawing.Size(571, 159);
            this.corePanel.TabIndex = 14;
            // 
            // multiPanel
            // 
            this.multiPanel.Controls.Add(this.skillsPage);
            this.multiPanel.Controls.Add(this.ordersPage);
            this.multiPanel.Controls.Add(this.skillQueuePage);
            this.multiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiPanel.Location = new System.Drawing.Point(1, 18);
            this.multiPanel.Name = "multiPanel";
            this.multiPanel.SelectedPage = this.skillsPage;
            this.multiPanel.Size = new System.Drawing.Size(568, 141);
            this.multiPanel.TabIndex = 14;
            // 
            // skillsPage
            // 
            this.skillsPage.Controls.Add(this.skillsList);
            this.skillsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillsPage.Location = new System.Drawing.Point(0, 0);
            this.skillsPage.Name = "skillsPage";
            this.skillsPage.Size = new System.Drawing.Size(568, 141);
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
            this.skillsList.Size = new System.Drawing.Size(568, 141);
            this.skillsList.TabIndex = 12;
            // 
            // ordersPage
            // 
            this.ordersPage.Controls.Add(this.ordersList);
            this.ordersPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordersPage.Location = new System.Drawing.Point(0, 0);
            this.ordersPage.Name = "ordersPage";
            this.ordersPage.Size = new System.Drawing.Size(568, 142);
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
            this.ordersList.Size = new System.Drawing.Size(568, 142);
            this.ordersList.TabIndex = 13;
            this.ordersList.TextFilter = "";
            // 
            // skillQueuePage
            // 
            this.skillQueuePage.Controls.Add(this.skillQueueList);
            this.skillQueuePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillQueuePage.Location = new System.Drawing.Point(0, 0);
            this.skillQueuePage.Name = "skillQueuePage";
            this.skillQueuePage.Size = new System.Drawing.Size(568, 142);
            this.skillQueuePage.TabIndex = 1;
            this.skillQueuePage.Text = "skillQueuePage";
            // 
            // skillQueueList
            // 
            this.skillQueueList.Character = null;
            this.skillQueueList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillQueueList.Location = new System.Drawing.Point(0, 0);
            this.skillQueueList.Name = "skillQueueList";
            this.skillQueueList.Size = new System.Drawing.Size(568, 142);
            this.skillQueueList.TabIndex = 0;
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
            // throbber
            // 
            this.throbber.BackColor = System.Drawing.Color.Transparent;
            this.throbber.ContextMenuStrip = this.throbberContextMenu;
            this.throbber.Dock = System.Windows.Forms.DockStyle.Right;
            this.throbber.Location = new System.Drawing.Point(5, 0);
            this.throbber.Margin = new System.Windows.Forms.Padding(0);
            this.throbber.MaximumSize = new System.Drawing.Size(24, 24);
            this.throbber.MinimumSize = new System.Drawing.Size(24, 24);
            this.throbber.Name = "throbber";
            this.throbber.Size = new System.Drawing.Size(24, 24);
            this.throbber.State = EVEMon.Controls.ThrobberState.Stopped;
            this.throbber.TabIndex = 18;
            this.throbber.TabStop = false;
            this.throbber.Text = "throbber1";
            this.ttToolTip.SetToolTip(this.throbber, "Click to update everything now.");
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
            this.upperPanel.PerformLayout();
            this.tlpInfo.ResumeLayout(false);
            this.tlpInfo.PerformLayout();
            this.upperTable.ResumeLayout(false);
            this.upperTable.PerformLayout();
            this.flpCharacterInfo.ResumeLayout(false);
            this.flpCharacterInfo.PerformLayout();
            this.flpThrobber.ResumeLayout(false);
            this.flpThrobber.PerformLayout();
            this.throbberContextMenu.ResumeLayout(false);
            this.lowerTable.ResumeLayout(false);
            this.lowerTable.PerformLayout();
            this.attributesFlowPanel.ResumeLayout(false);
            this.attributesFlowPanel.PerformLayout();
            this.skillQueuePanel.ResumeLayout(false);
            this.skillQueueTimePanel.ResumeLayout(false);
            this.skillQueueTimePanel.PerformLayout();
            this.lowerPanel.ResumeLayout(false);
            this.lowerPanel.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.skillsPanel.ResumeLayout(false);
            this.corePanel.ResumeLayout(false);
            this.multiPanel.ResumeLayout(false);
            this.skillsPage.ResumeLayout(false);
            this.ordersPage.ResumeLayout(false);
            this.skillQueuePage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private EVEMon.Common.Controls.CharacterPortrait pbCharImage;
        private System.Windows.Forms.Label lblCharacterName;
        private System.Windows.Forms.Label lblBioInfo;
        private System.Windows.Forms.Label lblCorpInfo;
        private System.Windows.Forms.Label lblBalance;
        private MainWindowSkillsList skillsList;
        private System.Windows.Forms.Panel pnlTraining;
        private System.Windows.Forms.Label lblTrainingEst;
        private System.Windows.Forms.Label lblTrainingRemain;
        private System.Windows.Forms.Label lblTrainingSkill;
        private System.Windows.Forms.Label lblCurrentlyTraining;
        private System.Windows.Forms.Panel upperPanel;
        private System.Windows.Forms.SaveFileDialog sfdSaveDialog;
        private System.Windows.Forms.ToolTip ttToolTip;
        private System.Windows.Forms.FlowLayoutPanel flpCharacterInfo;
        private System.Windows.Forms.TableLayoutPanel tlpInfo;
        private System.Windows.Forms.TableLayoutPanel tlpStatus;
        private System.Windows.Forms.FlowLayoutPanel flpStatusValues;
        private System.Windows.Forms.FlowLayoutPanel flpThrobber;
        private System.Windows.Forms.Label lblUpdateTimer;
        private System.Windows.Forms.ContextMenuStrip throbberContextMenu;
        private System.Windows.Forms.ToolStripMenuItem miQueryEverything;
        private System.Windows.Forms.ToolStripMenuItem miChangeInfo;
        private System.Windows.Forms.FlowLayoutPanel flpStatusLabels;
        private System.Windows.Forms.Label lblSPPerHour;
        private System.Windows.Forms.Label lblScheduleWarning;
        private EVEMon.Controls.Throbber throbber;
        private System.Windows.Forms.Button btnAddToCalendar;
        private EVEMon.Controls.BorderPanel skillsPanel;
        private System.Windows.Forms.Panel skillQueuePanel;
        private NotificationList notificationList;
        private System.Windows.Forms.Panel corePanel;
        private System.Windows.Forms.Label lblPaused;
        private System.Windows.Forms.ToolStripSeparator throbberSeparator;
        private EVEMon.Controls.MultiPanel multiPanel;
        private EVEMon.Controls.MultiPanelPage skillsPage;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton skillsIcon;
        private System.Windows.Forms.ToolStripButton ordersIcon;
        private System.Windows.Forms.ToolStripButton toggleSkillsIcon;
        private System.Windows.Forms.Panel lowerPanel;
        private System.Windows.Forms.TableLayoutPanel upperTable;
        private System.Windows.Forms.TableLayoutPanel lowerTable;
        private System.Windows.Forms.Label lblSkillHeader;
        private System.Windows.Forms.FlowLayoutPanel attributesFlowPanel;
        private System.Windows.Forms.Label lblCharisma;
        private System.Windows.Forms.Label lblIntelligence;
        private System.Windows.Forms.Label lblPerception;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.Label lblWillpower;
        private EVEMon.Controls.MultiPanelPage ordersPage;
        private MainWindowMarketOrdersList ordersList;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.ToolStripTextBox searchTextBox;
        private System.Windows.Forms.ToolStripDropDownButton ordersGroupMenu;
		private System.Windows.Forms.Label lblQueueRemaining;
		private System.Windows.Forms.Label lblQueueCompletionTime;
        private System.Windows.Forms.Panel skillQueueTimePanel;
        private SkillQueueControl skillQueueControl;
        private System.Windows.Forms.ToolStripDropDownButton preferencesMenu;
        private System.Windows.Forms.ToolStripMenuItem columnSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideInactiveOrdersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numberAbsFormatMenuItem;
        private EVEMon.Controls.MultiPanelPage skillQueuePage;
        private MainWindowSkillsQueueList skillQueueList;
        private System.Windows.Forms.ToolStripButton skillQueueIcon;
    }
}