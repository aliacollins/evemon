namespace EVEMon
{
    partial class CharacterMonitorHeader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterMonitorHeader));
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CharacterPortrait = new EVEMon.Common.Controls.CharacterPortrait();
            this.ThrobberFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.UpdateThrobber = new EVEMon.Common.Controls.Throbber();
            this.ThrobberContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ChangeAPIKeyInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QueryEverythingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThrobberSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.UpdateLabel = new System.Windows.Forms.Label();
            this.BioFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.CharacterNameLabel = new System.Windows.Forms.Label();
            this.BioInfoLabel = new System.Windows.Forms.Label();
            this.BalanceLabel = new System.Windows.Forms.Label();
            this.BirthdayLabel = new System.Windows.Forms.Label();
            this.CorporationInfoFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.CorporationNameLabel = new System.Windows.Forms.Label();
            this.AllianceInfoIndicationPictureBox = new System.Windows.Forms.PictureBox();
            this.SecurityStatusLabel = new System.Windows.Forms.Label();
            this.ActiveShipFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ActiveShipLabel = new System.Windows.Forms.Label();
            this.LocationInfoIndicationPictureBox = new System.Windows.Forms.PictureBox();
            this.SkillSummaryPanel = new System.Windows.Forms.Panel();
            this.tlpAttributes = new System.Windows.Forms.TableLayoutPanel();
            this.lblMEMAttribute = new System.Windows.Forms.Label();
            this.lblWILAttribute = new System.Windows.Forms.Label();
            this.lblCHAAttribute = new System.Windows.Forms.Label();
            this.lblPERAttribute = new System.Windows.Forms.Label();
            this.AttributeCharismaLabel = new System.Windows.Forms.Label();
            this.AttributePerceptionLabel = new System.Windows.Forms.Label();
            this.AttributeIntelligenceLabel = new System.Windows.Forms.Label();
            this.AttributeMemoryLabel = new System.Windows.Forms.Label();
            this.AttributeWillpowerLabel = new System.Windows.Forms.Label();
            this.lblINTAttribute = new System.Windows.Forms.Label();
            this.SkillSummaryLabel = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.MainTableLayoutPanel.SuspendLayout();
            this.ThrobberFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateThrobber)).BeginInit();
            this.ThrobberContextMenu.SuspendLayout();
            this.BioFlowLayoutPanel.SuspendLayout();
            this.CorporationInfoFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AllianceInfoIndicationPictureBox)).BeginInit();
            this.ActiveShipFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LocationInfoIndicationPictureBox)).BeginInit();
            this.SkillSummaryPanel.SuspendLayout();
            this.tlpAttributes.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.AutoSize = true;
            this.MainTableLayoutPanel.ColumnCount = 3;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.MainTableLayoutPanel.Controls.Add(this.CharacterPortrait, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.ThrobberFlowLayoutPanel, 2, 0);
            this.MainTableLayoutPanel.Controls.Add(this.BioFlowLayoutPanel, 1, 0);
            this.MainTableLayoutPanel.Controls.Add(this.SkillSummaryPanel, 1, 1);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 2;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(572, 216);
            this.MainTableLayoutPanel.TabIndex = 0;
            // 
            // CharacterPortrait
            // 
            this.CharacterPortrait.Character = null;
            this.CharacterPortrait.CharacterID = ((long)(-1));
            this.CharacterPortrait.Location = new System.Drawing.Point(0, 9);
            this.CharacterPortrait.Margin = new System.Windows.Forms.Padding(0, 9, 4, 4);
            this.CharacterPortrait.MinimumSize = new System.Drawing.Size(171, 158);
            this.CharacterPortrait.Name = "CharacterPortrait";
            this.MainTableLayoutPanel.SetRowSpan(this.CharacterPortrait, 2);
            this.CharacterPortrait.Size = new System.Drawing.Size(171, 158);
            this.CharacterPortrait.TabIndex = 2;
            this.CharacterPortrait.TabStop = false;
            // 
            // ThrobberFlowLayoutPanel
            // 
            this.ThrobberFlowLayoutPanel.Controls.Add(this.UpdateThrobber);
            this.ThrobberFlowLayoutPanel.Controls.Add(this.UpdateLabel);
            this.ThrobberFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ThrobberFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ThrobberFlowLayoutPanel.Location = new System.Drawing.Point(492, 0);
            this.ThrobberFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ThrobberFlowLayoutPanel.Name = "ThrobberFlowLayoutPanel";
            this.ThrobberFlowLayoutPanel.Size = new System.Drawing.Size(80, 119);
            this.ThrobberFlowLayoutPanel.TabIndex = 10;
            // 
            // UpdateThrobber
            // 
            this.UpdateThrobber.ContextMenuStrip = this.ThrobberContextMenu;
            this.UpdateThrobber.Dock = System.Windows.Forms.DockStyle.Right;
            this.UpdateThrobber.Location = new System.Drawing.Point(44, 4);
            this.UpdateThrobber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UpdateThrobber.MaximumSize = new System.Drawing.Size(32, 32);
            this.UpdateThrobber.MinimumSize = new System.Drawing.Size(32, 32);
            this.UpdateThrobber.Name = "UpdateThrobber";
            this.UpdateThrobber.Size = new System.Drawing.Size(32, 32);
            this.UpdateThrobber.State = EVEMon.Common.ThrobberState.Stopped;
            this.UpdateThrobber.TabIndex = 4;
            this.UpdateThrobber.TabStop = false;
            this.UpdateThrobber.Click += new System.EventHandler(this.UpdateThrobber_Click);
            // 
            // ThrobberContextMenu
            // 
            this.ThrobberContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChangeAPIKeyInfoMenuItem,
            this.QueryEverythingMenuItem,
            this.ThrobberSeparator});
            this.ThrobberContextMenu.Name = "cmsThrobberMenu";
            this.ThrobberContextMenu.Size = new System.Drawing.Size(234, 54);
            this.ThrobberContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ThrobberContextMenu_Opening);
            this.ThrobberContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ThrobberContextMenu_ItemClicked);
            // 
            // ChangeAPIKeyInfoMenuItem
            // 
            this.ChangeAPIKeyInfoMenuItem.Name = "ChangeAPIKeyInfoMenuItem";
            this.ChangeAPIKeyInfoMenuItem.Size = new System.Drawing.Size(233, 22);
            this.ChangeAPIKeyInfoMenuItem.Text = "Change API Key Information...";
            this.ChangeAPIKeyInfoMenuItem.Click += new System.EventHandler(this.ChangeAPIKeyInfoMenuItem_Click);
            // 
            // QueryEverythingMenuItem
            // 
            this.QueryEverythingMenuItem.Name = "QueryEverythingMenuItem";
            this.QueryEverythingMenuItem.Size = new System.Drawing.Size(233, 22);
            this.QueryEverythingMenuItem.Text = "Update Everything";
            // 
            // ThrobberSeparator
            // 
            this.ThrobberSeparator.Name = "ThrobberSeparator";
            this.ThrobberSeparator.Size = new System.Drawing.Size(230, 6);
            // 
            // UpdateLabel
            // 
            this.UpdateLabel.AutoSize = true;
            this.UpdateLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ThrobberFlowLayoutPanel.SetFlowBreak(this.UpdateLabel, true);
            this.UpdateLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.UpdateLabel.Location = new System.Drawing.Point(16, 40);
            this.UpdateLabel.Margin = new System.Windows.Forms.Padding(0);
            this.UpdateLabel.Name = "UpdateLabel";
            this.UpdateLabel.Size = new System.Drawing.Size(64, 17);
            this.UpdateLabel.TabIndex = 5;
            this.UpdateLabel.Text = "00:00:00";
            this.UpdateLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.UpdateLabel.MouseHover += new System.EventHandler(this.UpdateLabel_MouseHover);
            // 
            // BioFlowLayoutPanel
            // 
            this.BioFlowLayoutPanel.AutoSize = true;
            this.BioFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BioFlowLayoutPanel.Controls.Add(this.CharacterNameLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.BioInfoLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.BalanceLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.BirthdayLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.CorporationInfoFlowLayoutPanel);
            this.BioFlowLayoutPanel.Controls.Add(this.SecurityStatusLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.ActiveShipFlowLayoutPanel);
            this.BioFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BioFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.BioFlowLayoutPanel.Location = new System.Drawing.Point(175, 0);
            this.BioFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BioFlowLayoutPanel.Name = "BioFlowLayoutPanel";
            this.BioFlowLayoutPanel.Size = new System.Drawing.Size(317, 119);
            this.BioFlowLayoutPanel.TabIndex = 9;
            // 
            // CharacterNameLabel
            // 
            this.CharacterNameLabel.AutoSize = true;
            this.CharacterNameLabel.Location = new System.Drawing.Point(0, 0);
            this.CharacterNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.CharacterNameLabel.Name = "CharacterNameLabel";
            this.CharacterNameLabel.Size = new System.Drawing.Size(111, 17);
            this.CharacterNameLabel.TabIndex = 4;
            this.CharacterNameLabel.Text = "Character Name";
            // 
            // BioInfoLabel
            // 
            this.BioInfoLabel.AutoSize = true;
            this.BioInfoLabel.Location = new System.Drawing.Point(0, 17);
            this.BioInfoLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.BioInfoLabel.Name = "BioInfoLabel";
            this.BioInfoLabel.Size = new System.Drawing.Size(55, 17);
            this.BioInfoLabel.TabIndex = 5;
            this.BioInfoLabel.Text = "Bio Info";
            // 
            // BalanceLabel
            // 
            this.BalanceLabel.AutoSize = true;
            this.BalanceLabel.Location = new System.Drawing.Point(0, 34);
            this.BalanceLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.BalanceLabel.Name = "BalanceLabel";
            this.BalanceLabel.Size = new System.Drawing.Size(120, 17);
            this.BalanceLabel.TabIndex = 7;
            this.BalanceLabel.Text = "Balance: 0.00 ISK";
            // 
            // BirthdayLabel
            // 
            this.BirthdayLabel.AutoSize = true;
            this.BirthdayLabel.Location = new System.Drawing.Point(0, 51);
            this.BirthdayLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.BirthdayLabel.Name = "BirthdayLabel";
            this.BirthdayLabel.Size = new System.Drawing.Size(60, 17);
            this.BirthdayLabel.TabIndex = 8;
            this.BirthdayLabel.Text = "Birthday";
            // 
            // CorporationInfoFlowLayoutPanel
            // 
            this.CorporationInfoFlowLayoutPanel.AutoSize = true;
            this.CorporationInfoFlowLayoutPanel.Controls.Add(this.CorporationNameLabel);
            this.CorporationInfoFlowLayoutPanel.Controls.Add(this.AllianceInfoIndicationPictureBox);
            this.CorporationInfoFlowLayoutPanel.Location = new System.Drawing.Point(0, 68);
            this.CorporationInfoFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.CorporationInfoFlowLayoutPanel.Name = "CorporationInfoFlowLayoutPanel";
            this.CorporationInfoFlowLayoutPanel.Size = new System.Drawing.Size(134, 17);
            this.CorporationInfoFlowLayoutPanel.TabIndex = 13;
            // 
            // CorporationNameLabel
            // 
            this.CorporationNameLabel.AutoSize = true;
            this.CorporationNameLabel.Location = new System.Drawing.Point(0, 0);
            this.CorporationNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.CorporationNameLabel.Name = "CorporationNameLabel";
            this.CorporationNameLabel.Size = new System.Drawing.Size(109, 17);
            this.CorporationNameLabel.TabIndex = 6;
            this.CorporationNameLabel.Text = "Corporation Info";
            this.CorporationNameLabel.MouseHover += new System.EventHandler(this.CorporationNameLabel_MouseHover);
            // 
            // AllianceInfoIndicationPictureBox
            // 
            this.AllianceInfoIndicationPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("AllianceInfoIndicationPictureBox.Image")));
            this.AllianceInfoIndicationPictureBox.Location = new System.Drawing.Point(113, 0);
            this.AllianceInfoIndicationPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.AllianceInfoIndicationPictureBox.Name = "AllianceInfoIndicationPictureBox";
            this.AllianceInfoIndicationPictureBox.Size = new System.Drawing.Size(21, 16);
            this.AllianceInfoIndicationPictureBox.TabIndex = 6;
            this.AllianceInfoIndicationPictureBox.TabStop = false;
            // 
            // SecurityStatusLabel
            // 
            this.SecurityStatusLabel.AutoSize = true;
            this.SecurityStatusLabel.Location = new System.Drawing.Point(0, 85);
            this.SecurityStatusLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.SecurityStatusLabel.Name = "SecurityStatusLabel";
            this.SecurityStatusLabel.Size = new System.Drawing.Size(130, 17);
            this.SecurityStatusLabel.TabIndex = 9;
            this.SecurityStatusLabel.Text = "Security Status Info";
            // 
            // ActiveShipFlowLayoutPanel
            // 
            this.ActiveShipFlowLayoutPanel.AutoSize = true;
            this.ActiveShipFlowLayoutPanel.Controls.Add(this.ActiveShipLabel);
            this.ActiveShipFlowLayoutPanel.Controls.Add(this.LocationInfoIndicationPictureBox);
            this.ActiveShipFlowLayoutPanel.Location = new System.Drawing.Point(0, 102);
            this.ActiveShipFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.ActiveShipFlowLayoutPanel.Name = "ActiveShipFlowLayoutPanel";
            this.ActiveShipFlowLayoutPanel.Size = new System.Drawing.Size(130, 17);
            this.ActiveShipFlowLayoutPanel.TabIndex = 12;
            // 
            // ActiveShipLabel
            // 
            this.ActiveShipLabel.AutoSize = true;
            this.ActiveShipLabel.Location = new System.Drawing.Point(0, 0);
            this.ActiveShipLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.ActiveShipLabel.Name = "ActiveShipLabel";
            this.ActiveShipLabel.Size = new System.Drawing.Size(105, 17);
            this.ActiveShipLabel.TabIndex = 10;
            this.ActiveShipLabel.Text = "Active Ship Info";
            this.ActiveShipLabel.MouseHover += new System.EventHandler(this.ActiveShipLabel_MouseHover);
            // 
            // LocationInfoIndicationPictureBox
            // 
            this.LocationInfoIndicationPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("LocationInfoIndicationPictureBox.Image")));
            this.LocationInfoIndicationPictureBox.Location = new System.Drawing.Point(109, 0);
            this.LocationInfoIndicationPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.LocationInfoIndicationPictureBox.Name = "LocationInfoIndicationPictureBox";
            this.LocationInfoIndicationPictureBox.Size = new System.Drawing.Size(21, 16);
            this.LocationInfoIndicationPictureBox.TabIndex = 11;
            this.LocationInfoIndicationPictureBox.TabStop = false;
            // 
            // SkillSummaryPanel
            // 
            this.MainTableLayoutPanel.SetColumnSpan(this.SkillSummaryPanel, 2);
            this.SkillSummaryPanel.Controls.Add(this.tlpAttributes);
            this.SkillSummaryPanel.Controls.Add(this.SkillSummaryLabel);
            this.SkillSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SkillSummaryPanel.Location = new System.Drawing.Point(175, 119);
            this.SkillSummaryPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SkillSummaryPanel.Name = "SkillSummaryPanel";
            this.SkillSummaryPanel.Size = new System.Drawing.Size(397, 97);
            this.SkillSummaryPanel.TabIndex = 4;
            // 
            // tlpAttributes
            // 
            this.tlpAttributes.AutoSize = true;
            this.tlpAttributes.ColumnCount = 2;
            this.tlpAttributes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAttributes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAttributes.Controls.Add(this.lblMEMAttribute, 1, 4);
            this.tlpAttributes.Controls.Add(this.lblWILAttribute, 1, 3);
            this.tlpAttributes.Controls.Add(this.lblCHAAttribute, 1, 2);
            this.tlpAttributes.Controls.Add(this.lblPERAttribute, 1, 1);
            this.tlpAttributes.Controls.Add(this.AttributeCharismaLabel, 0, 2);
            this.tlpAttributes.Controls.Add(this.AttributePerceptionLabel, 0, 1);
            this.tlpAttributes.Controls.Add(this.AttributeIntelligenceLabel, 0, 0);
            this.tlpAttributes.Controls.Add(this.AttributeMemoryLabel, 0, 4);
            this.tlpAttributes.Controls.Add(this.AttributeWillpowerLabel, 0, 3);
            this.tlpAttributes.Controls.Add(this.lblINTAttribute, 1, 0);
            this.tlpAttributes.Dock = System.Windows.Forms.DockStyle.Left;
            this.tlpAttributes.Location = new System.Drawing.Point(0, 0);
            this.tlpAttributes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tlpAttributes.Name = "tlpAttributes";
            this.tlpAttributes.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tlpAttributes.RowCount = 5;
            this.tlpAttributes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAttributes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAttributes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAttributes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAttributes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAttributes.Size = new System.Drawing.Size(115, 97);
            this.tlpAttributes.TabIndex = 5;
            // 
            // lblMEMAttribute
            // 
            this.lblMEMAttribute.AutoSize = true;
            this.lblMEMAttribute.Location = new System.Drawing.Point(87, 74);
            this.lblMEMAttribute.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMEMAttribute.Name = "lblMEMAttribute";
            this.lblMEMAttribute.Size = new System.Drawing.Size(24, 17);
            this.lblMEMAttribute.TabIndex = 9;
            this.lblMEMAttribute.Text = "17";
            this.lblMEMAttribute.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // lblWILAttribute
            // 
            this.lblWILAttribute.AutoSize = true;
            this.lblWILAttribute.Location = new System.Drawing.Point(87, 57);
            this.lblWILAttribute.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWILAttribute.Name = "lblWILAttribute";
            this.lblWILAttribute.Size = new System.Drawing.Size(24, 17);
            this.lblWILAttribute.TabIndex = 8;
            this.lblWILAttribute.Text = "17";
            this.lblWILAttribute.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // lblCHAAttribute
            // 
            this.lblCHAAttribute.AutoSize = true;
            this.lblCHAAttribute.Location = new System.Drawing.Point(87, 40);
            this.lblCHAAttribute.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCHAAttribute.Name = "lblCHAAttribute";
            this.lblCHAAttribute.Size = new System.Drawing.Size(24, 17);
            this.lblCHAAttribute.TabIndex = 7;
            this.lblCHAAttribute.Text = "17";
            this.lblCHAAttribute.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // lblPERAttribute
            // 
            this.lblPERAttribute.AutoSize = true;
            this.lblPERAttribute.Location = new System.Drawing.Point(87, 23);
            this.lblPERAttribute.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPERAttribute.Name = "lblPERAttribute";
            this.lblPERAttribute.Size = new System.Drawing.Size(24, 17);
            this.lblPERAttribute.TabIndex = 6;
            this.lblPERAttribute.Text = "17";
            this.lblPERAttribute.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // AttributeCharismaLabel
            // 
            this.AttributeCharismaLabel.AutoSize = true;
            this.AttributeCharismaLabel.Location = new System.Drawing.Point(0, 40);
            this.AttributeCharismaLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeCharismaLabel.Name = "AttributeCharismaLabel";
            this.AttributeCharismaLabel.Size = new System.Drawing.Size(71, 17);
            this.AttributeCharismaLabel.TabIndex = 2;
            this.AttributeCharismaLabel.Text = "Charisma:";
            // 
            // AttributePerceptionLabel
            // 
            this.AttributePerceptionLabel.AutoSize = true;
            this.AttributePerceptionLabel.Location = new System.Drawing.Point(0, 23);
            this.AttributePerceptionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributePerceptionLabel.Name = "AttributePerceptionLabel";
            this.AttributePerceptionLabel.Size = new System.Drawing.Size(80, 17);
            this.AttributePerceptionLabel.TabIndex = 1;
            this.AttributePerceptionLabel.Text = "Perception:";
            // 
            // AttributeIntelligenceLabel
            // 
            this.AttributeIntelligenceLabel.AutoSize = true;
            this.AttributeIntelligenceLabel.Location = new System.Drawing.Point(0, 6);
            this.AttributeIntelligenceLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeIntelligenceLabel.Name = "AttributeIntelligenceLabel";
            this.AttributeIntelligenceLabel.Size = new System.Drawing.Size(83, 17);
            this.AttributeIntelligenceLabel.TabIndex = 0;
            this.AttributeIntelligenceLabel.Text = "Intelligence:";
            // 
            // AttributeMemoryLabel
            // 
            this.AttributeMemoryLabel.AutoSize = true;
            this.AttributeMemoryLabel.Location = new System.Drawing.Point(0, 74);
            this.AttributeMemoryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeMemoryLabel.Name = "AttributeMemoryLabel";
            this.AttributeMemoryLabel.Size = new System.Drawing.Size(62, 17);
            this.AttributeMemoryLabel.TabIndex = 4;
            this.AttributeMemoryLabel.Text = "Memory:";
            // 
            // AttributeWillpowerLabel
            // 
            this.AttributeWillpowerLabel.AutoSize = true;
            this.AttributeWillpowerLabel.Location = new System.Drawing.Point(0, 57);
            this.AttributeWillpowerLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeWillpowerLabel.Name = "AttributeWillpowerLabel";
            this.AttributeWillpowerLabel.Size = new System.Drawing.Size(72, 17);
            this.AttributeWillpowerLabel.TabIndex = 3;
            this.AttributeWillpowerLabel.Text = "Willpower:";
            // 
            // lblINTAttribute
            // 
            this.lblINTAttribute.AutoSize = true;
            this.lblINTAttribute.Location = new System.Drawing.Point(87, 6);
            this.lblINTAttribute.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblINTAttribute.Name = "lblINTAttribute";
            this.lblINTAttribute.Size = new System.Drawing.Size(24, 17);
            this.lblINTAttribute.TabIndex = 5;
            this.lblINTAttribute.Text = "17";
            this.lblINTAttribute.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // SkillSummaryLabel
            // 
            this.SkillSummaryLabel.AutoSize = true;
            this.SkillSummaryLabel.BackColor = System.Drawing.Color.Transparent;
            this.SkillSummaryLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.SkillSummaryLabel.Location = new System.Drawing.Point(256, 0);
            this.SkillSummaryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.SkillSummaryLabel.Name = "SkillSummaryLabel";
            this.SkillSummaryLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.SkillSummaryLabel.Size = new System.Drawing.Size(141, 91);
            this.SkillSummaryLabel.TabIndex = 1;
            this.SkillSummaryLabel.Text = "0 Known Skills\r\n0 Skills at Level V\r\n0 Total SP\r\n0 Clone Limit\r\nClone Grade Nothi" +
                "ng";
            this.SkillSummaryLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.SkillSummaryLabel.MouseHover += new System.EventHandler(this.SkillSummaryLabel_MouseHover);
            // 
            // ToolTip
            // 
            this.ToolTip.AutoPopDelay = 5000000;
            this.ToolTip.InitialDelay = 500;
            this.ToolTip.IsBalloon = true;
            this.ToolTip.ReshowDelay = 100;
            // 
            // CharacterMonitorHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CharacterMonitorHeader";
            this.Size = new System.Drawing.Size(572, 218);
            this.Resize += new System.EventHandler(this.CharacterMonitorHeader_Resize);
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.MainTableLayoutPanel.PerformLayout();
            this.ThrobberFlowLayoutPanel.ResumeLayout(false);
            this.ThrobberFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateThrobber)).EndInit();
            this.ThrobberContextMenu.ResumeLayout(false);
            this.BioFlowLayoutPanel.ResumeLayout(false);
            this.BioFlowLayoutPanel.PerformLayout();
            this.CorporationInfoFlowLayoutPanel.ResumeLayout(false);
            this.CorporationInfoFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AllianceInfoIndicationPictureBox)).EndInit();
            this.ActiveShipFlowLayoutPanel.ResumeLayout(false);
            this.ActiveShipFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LocationInfoIndicationPictureBox)).EndInit();
            this.SkillSummaryPanel.ResumeLayout(false);
            this.SkillSummaryPanel.PerformLayout();
            this.tlpAttributes.ResumeLayout(false);
            this.tlpAttributes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.Panel SkillSummaryPanel;
        private EVEMon.Common.Controls.CharacterPortrait CharacterPortrait;
        private System.Windows.Forms.Label SkillSummaryLabel;
        private System.Windows.Forms.ContextMenuStrip ThrobberContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ChangeAPIKeyInfoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem QueryEverythingMenuItem;
        private System.Windows.Forms.ToolStripSeparator ThrobberSeparator;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.Label AttributeIntelligenceLabel;
        private System.Windows.Forms.Label AttributePerceptionLabel;
        private System.Windows.Forms.Label AttributeCharismaLabel;
        private System.Windows.Forms.Label AttributeWillpowerLabel;
        private System.Windows.Forms.Label AttributeMemoryLabel;
        private System.Windows.Forms.FlowLayoutPanel BioFlowLayoutPanel;
        private System.Windows.Forms.Label CharacterNameLabel;
        private System.Windows.Forms.Label BioInfoLabel;
        private System.Windows.Forms.Label BalanceLabel;
        private System.Windows.Forms.Label BirthdayLabel;
        private System.Windows.Forms.FlowLayoutPanel CorporationInfoFlowLayoutPanel;
        private System.Windows.Forms.Label CorporationNameLabel;
        private System.Windows.Forms.PictureBox AllianceInfoIndicationPictureBox;
        private System.Windows.Forms.Label SecurityStatusLabel;
        private System.Windows.Forms.FlowLayoutPanel ActiveShipFlowLayoutPanel;
        private System.Windows.Forms.Label ActiveShipLabel;
        private System.Windows.Forms.PictureBox LocationInfoIndicationPictureBox;
        private System.Windows.Forms.FlowLayoutPanel ThrobberFlowLayoutPanel;
        private EVEMon.Common.Controls.Throbber UpdateThrobber;
        private System.Windows.Forms.Label UpdateLabel;
        private System.Windows.Forms.TableLayoutPanel tlpAttributes;
        private System.Windows.Forms.Label lblMEMAttribute;
        private System.Windows.Forms.Label lblWILAttribute;
        private System.Windows.Forms.Label lblCHAAttribute;
        private System.Windows.Forms.Label lblPERAttribute;
        private System.Windows.Forms.Label lblINTAttribute;
    }
}
