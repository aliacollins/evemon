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
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.BioFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.CharacterNameLabel = new System.Windows.Forms.Label();
            this.BioInfoLabel = new System.Windows.Forms.Label();
            this.CorporationNameLabel = new System.Windows.Forms.Label();
            this.BalanceLabel = new System.Windows.Forms.Label();
            this.SkillSummaryPanel = new System.Windows.Forms.Panel();
            this.AttributesFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AttributeInteligenceLabel = new System.Windows.Forms.Label();
            this.AttributePerceptionLabel = new System.Windows.Forms.Label();
            this.AttributeCharismaLabel = new System.Windows.Forms.Label();
            this.AttributeWillpowerLabel = new System.Windows.Forms.Label();
            this.AttributeMemoryLabel = new System.Windows.Forms.Label();
            this.SkillSummaryLabel = new System.Windows.Forms.Label();
            this.ThrobberFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ThrobberContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ChangeInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QueryEverythingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThrobberSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.UpdateLabel = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.CharacterPortrait = new EVEMon.Common.Controls.CharacterPortrait();
            this.UpdateThrobber = new EVEMon.Controls.Throbber();
            this.MainTableLayoutPanel.SuspendLayout();
            this.BioFlowLayoutPanel.SuspendLayout();
            this.SkillSummaryPanel.SuspendLayout();
            this.AttributesFlowLayoutPanel.SuspendLayout();
            this.ThrobberFlowLayoutPanel.SuspendLayout();
            this.ThrobberContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateThrobber)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.AutoSize = true;
            this.MainTableLayoutPanel.ColumnCount = 3;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.MainTableLayoutPanel.Controls.Add(this.BioFlowLayoutPanel, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.SkillSummaryPanel, 1, 1);
            this.MainTableLayoutPanel.Controls.Add(this.CharacterPortrait, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.ThrobberFlowLayoutPanel, 2, 0);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 2;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(429, 131);
            this.MainTableLayoutPanel.TabIndex = 0;
            // 
            // BioFlowLayoutPanel
            // 
            this.BioFlowLayoutPanel.AutoSize = true;
            this.BioFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BioFlowLayoutPanel.Controls.Add(this.CharacterNameLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.BioInfoLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.CorporationNameLabel);
            this.BioFlowLayoutPanel.Controls.Add(this.BalanceLabel);
            this.BioFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.BioFlowLayoutPanel.Location = new System.Drawing.Point(131, 0);
            this.BioFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BioFlowLayoutPanel.Name = "BioFlowLayoutPanel";
            this.BioFlowLayoutPanel.Size = new System.Drawing.Size(96, 52);
            this.BioFlowLayoutPanel.TabIndex = 9;
            // 
            // CharacterNameLabel
            // 
            this.CharacterNameLabel.AutoSize = true;
            this.CharacterNameLabel.Location = new System.Drawing.Point(0, 0);
            this.CharacterNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.CharacterNameLabel.Name = "CharacterNameLabel";
            this.CharacterNameLabel.Size = new System.Drawing.Size(84, 13);
            this.CharacterNameLabel.TabIndex = 4;
            this.CharacterNameLabel.Text = "Character Name";
            // 
            // BioInfoLabel
            // 
            this.BioInfoLabel.AutoSize = true;
            this.BioInfoLabel.Location = new System.Drawing.Point(0, 13);
            this.BioInfoLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.BioInfoLabel.Name = "BioInfoLabel";
            this.BioInfoLabel.Size = new System.Drawing.Size(43, 13);
            this.BioInfoLabel.TabIndex = 5;
            this.BioInfoLabel.Text = "Bio Info";
            // 
            // CorporationNameLabel
            // 
            this.CorporationNameLabel.AutoSize = true;
            this.CorporationNameLabel.Location = new System.Drawing.Point(0, 26);
            this.CorporationNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.CorporationNameLabel.Name = "CorporationNameLabel";
            this.CorporationNameLabel.Size = new System.Drawing.Size(82, 13);
            this.CorporationNameLabel.TabIndex = 6;
            this.CorporationNameLabel.Text = "Corporation Info";
            // 
            // BalanceLabel
            // 
            this.BalanceLabel.AutoSize = true;
            this.BalanceLabel.Location = new System.Drawing.Point(0, 39);
            this.BalanceLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.BalanceLabel.Name = "BalanceLabel";
            this.BalanceLabel.Size = new System.Drawing.Size(93, 13);
            this.BalanceLabel.TabIndex = 7;
            this.BalanceLabel.Text = "Balance: 0.00 ISK";
            // 
            // SkillSummaryPanel
            // 
            this.MainTableLayoutPanel.SetColumnSpan(this.SkillSummaryPanel, 2);
            this.SkillSummaryPanel.Controls.Add(this.AttributesFlowLayoutPanel);
            this.SkillSummaryPanel.Controls.Add(this.SkillSummaryLabel);
            this.SkillSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SkillSummaryPanel.Location = new System.Drawing.Point(131, 52);
            this.SkillSummaryPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SkillSummaryPanel.Name = "SkillSummaryPanel";
            this.SkillSummaryPanel.Size = new System.Drawing.Size(298, 79);
            this.SkillSummaryPanel.TabIndex = 4;
            // 
            // AttributesFlowLayoutPanel
            // 
            this.AttributesFlowLayoutPanel.AutoSize = true;
            this.AttributesFlowLayoutPanel.Controls.Add(this.AttributeInteligenceLabel);
            this.AttributesFlowLayoutPanel.Controls.Add(this.AttributePerceptionLabel);
            this.AttributesFlowLayoutPanel.Controls.Add(this.AttributeCharismaLabel);
            this.AttributesFlowLayoutPanel.Controls.Add(this.AttributeWillpowerLabel);
            this.AttributesFlowLayoutPanel.Controls.Add(this.AttributeMemoryLabel);
            this.AttributesFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.AttributesFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.AttributesFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributesFlowLayoutPanel.Name = "AttributesFlowLayoutPanel";
            this.AttributesFlowLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.AttributesFlowLayoutPanel.Size = new System.Drawing.Size(96, 81);
            this.AttributesFlowLayoutPanel.TabIndex = 2;
            // 
            // AttributeInteligenceLabel
            // 
            this.AttributeInteligenceLabel.AutoSize = true;
            this.AttributeInteligenceLabel.Location = new System.Drawing.Point(0, 5);
            this.AttributeInteligenceLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeInteligenceLabel.Name = "AttributeInteligenceLabel";
            this.AttributeInteligenceLabel.Size = new System.Drawing.Size(74, 13);
            this.AttributeInteligenceLabel.TabIndex = 0;
            this.AttributeInteligenceLabel.Text = "00 Inteligence";
            this.AttributeInteligenceLabel.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // AttributePerceptionLabel
            // 
            this.AttributePerceptionLabel.AutoSize = true;
            this.AttributePerceptionLabel.Location = new System.Drawing.Point(0, 18);
            this.AttributePerceptionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributePerceptionLabel.Name = "AttributePerceptionLabel";
            this.AttributePerceptionLabel.Size = new System.Drawing.Size(73, 13);
            this.AttributePerceptionLabel.TabIndex = 1;
            this.AttributePerceptionLabel.Text = "00 Perception";
            this.AttributePerceptionLabel.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // AttributeCharismaLabel
            // 
            this.AttributeCharismaLabel.AutoSize = true;
            this.AttributeCharismaLabel.Location = new System.Drawing.Point(0, 31);
            this.AttributeCharismaLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeCharismaLabel.Name = "AttributeCharismaLabel";
            this.AttributeCharismaLabel.Size = new System.Drawing.Size(65, 13);
            this.AttributeCharismaLabel.TabIndex = 2;
            this.AttributeCharismaLabel.Text = "00 Charisma";
            this.AttributeCharismaLabel.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // AttributeWillpowerLabel
            // 
            this.AttributeWillpowerLabel.AutoSize = true;
            this.AttributeWillpowerLabel.Location = new System.Drawing.Point(0, 44);
            this.AttributeWillpowerLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeWillpowerLabel.Name = "AttributeWillpowerLabel";
            this.AttributeWillpowerLabel.Size = new System.Drawing.Size(68, 13);
            this.AttributeWillpowerLabel.TabIndex = 3;
            this.AttributeWillpowerLabel.Text = "00 Willpower";
            this.AttributeWillpowerLabel.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // AttributeMemoryLabel
            // 
            this.AttributeMemoryLabel.AutoSize = true;
            this.AttributeMemoryLabel.Location = new System.Drawing.Point(0, 57);
            this.AttributeMemoryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AttributeMemoryLabel.Name = "AttributeMemoryLabel";
            this.AttributeMemoryLabel.Size = new System.Drawing.Size(59, 13);
            this.AttributeMemoryLabel.TabIndex = 4;
            this.AttributeMemoryLabel.Text = "00 Memory";
            this.AttributeMemoryLabel.MouseHover += new System.EventHandler(this.AttributeLabel_MouseHover);
            // 
            // SkillSummaryLabel
            // 
            this.SkillSummaryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SkillSummaryLabel.BackColor = System.Drawing.Color.Transparent;
            this.SkillSummaryLabel.Location = new System.Drawing.Point(117, 0);
            this.SkillSummaryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.SkillSummaryLabel.Name = "SkillSummaryLabel";
            this.SkillSummaryLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.SkillSummaryLabel.Size = new System.Drawing.Size(182, 76);
            this.SkillSummaryLabel.TabIndex = 1;
            this.SkillSummaryLabel.Text = "0 Known Skills\r\n0 Skills at Level V\r\n0 Total SP\r\n0 Clone Limit\r\nClone Grade Nothi" +
                "ng";
            this.SkillSummaryLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.SkillSummaryLabel.MouseHover += new System.EventHandler(this.SkillSummaryLabel_MouseHover);
            // 
            // ThrobberFlowLayoutPanel
            // 
            this.ThrobberFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ThrobberFlowLayoutPanel.Controls.Add(this.UpdateThrobber);
            this.ThrobberFlowLayoutPanel.Controls.Add(this.UpdateLabel);
            this.ThrobberFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ThrobberFlowLayoutPanel.Location = new System.Drawing.Point(373, 0);
            this.ThrobberFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ThrobberFlowLayoutPanel.Name = "ThrobberFlowLayoutPanel";
            this.ThrobberFlowLayoutPanel.Size = new System.Drawing.Size(56, 52);
            this.ThrobberFlowLayoutPanel.TabIndex = 10;
            // 
            // ThrobberContextMenu
            // 
            this.ThrobberContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChangeInfoMenuItem,
            this.QueryEverythingMenuItem,
            this.ThrobberSeparator});
            this.ThrobberContextMenu.Name = "cmsThrobberMenu";
            this.ThrobberContextMenu.Size = new System.Drawing.Size(234, 54);
            this.ThrobberContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ThrobberContextMenu_ItemClicked);
            this.ThrobberContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ThrobberContextMenu_Opening);
            // 
            // ChangeInfoMenuItem
            // 
            this.ChangeInfoMenuItem.Name = "ChangeInfoMenuItem";
            this.ChangeInfoMenuItem.Size = new System.Drawing.Size(233, 22);
            this.ChangeInfoMenuItem.Text = "Change API Key information...";
            this.ChangeInfoMenuItem.Click += new System.EventHandler(this.ChangeInfoMenuItem_Click);
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
            this.UpdateLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ThrobberFlowLayoutPanel.SetFlowBreak(this.UpdateLabel, true);
            this.UpdateLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.UpdateLabel.Location = new System.Drawing.Point(0, 30);
            this.UpdateLabel.Margin = new System.Windows.Forms.Padding(0);
            this.UpdateLabel.Name = "UpdateLabel";
            this.UpdateLabel.Size = new System.Drawing.Size(56, 22);
            this.UpdateLabel.TabIndex = 5;
            this.UpdateLabel.Text = "00:00:00";
            this.UpdateLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.UpdateLabel.MouseHover += new System.EventHandler(this.UpdateLabel_MouseHover);
            // 
            // ToolTip
            // 
            this.ToolTip.AutoPopDelay = 5000000;
            this.ToolTip.InitialDelay = 500;
            this.ToolTip.IsBalloon = true;
            this.ToolTip.ReshowDelay = 100;
            // 
            // CharacterPortrait
            // 
            this.CharacterPortrait.Character = null;
            this.CharacterPortrait.CharacterID = ((long)(-1));
            this.CharacterPortrait.Location = new System.Drawing.Point(0, 0);
            this.CharacterPortrait.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.CharacterPortrait.MinimumSize = new System.Drawing.Size(128, 128);
            this.CharacterPortrait.Name = "CharacterPortrait";
            this.MainTableLayoutPanel.SetRowSpan(this.CharacterPortrait, 2);
            this.CharacterPortrait.Size = new System.Drawing.Size(128, 128);
            this.CharacterPortrait.TabIndex = 2;
            this.CharacterPortrait.TabStop = false;
            // 
            // UpdateThrobber
            // 
            this.UpdateThrobber.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.UpdateThrobber.ContextMenuStrip = this.ThrobberContextMenu;
            this.UpdateThrobber.Location = new System.Drawing.Point(29, 3);
            this.UpdateThrobber.MaximumSize = new System.Drawing.Size(24, 24);
            this.UpdateThrobber.MinimumSize = new System.Drawing.Size(24, 24);
            this.UpdateThrobber.Name = "UpdateThrobber";
            this.UpdateThrobber.Size = new System.Drawing.Size(24, 24);
            this.UpdateThrobber.State = EVEMon.Controls.ThrobberState.Stopped;
            this.UpdateThrobber.TabIndex = 4;
            this.UpdateThrobber.TabStop = false;
            this.UpdateThrobber.Click += new System.EventHandler(this.UpdateThrobber_Click);
            // 
            // CharacterMonitorHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Name = "CharacterMonitorHeader";
            this.Size = new System.Drawing.Size(429, 133);
            this.Resize += new System.EventHandler(this.CharacterMonitorHeader_Resize);
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.MainTableLayoutPanel.PerformLayout();
            this.BioFlowLayoutPanel.ResumeLayout(false);
            this.BioFlowLayoutPanel.PerformLayout();
            this.SkillSummaryPanel.ResumeLayout(false);
            this.SkillSummaryPanel.PerformLayout();
            this.AttributesFlowLayoutPanel.ResumeLayout(false);
            this.AttributesFlowLayoutPanel.PerformLayout();
            this.ThrobberFlowLayoutPanel.ResumeLayout(false);
            this.ThrobberContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UpdateThrobber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.Panel SkillSummaryPanel;
        private EVEMon.Common.Controls.CharacterPortrait CharacterPortrait;
        private System.Windows.Forms.Label SkillSummaryLabel;
        private System.Windows.Forms.ContextMenuStrip ThrobberContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ChangeInfoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem QueryEverythingMenuItem;
        private System.Windows.Forms.ToolStripSeparator ThrobberSeparator;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.FlowLayoutPanel AttributesFlowLayoutPanel;
        private System.Windows.Forms.Label AttributeInteligenceLabel;
        private System.Windows.Forms.Label AttributePerceptionLabel;
        private System.Windows.Forms.Label AttributeCharismaLabel;
        private System.Windows.Forms.Label AttributeWillpowerLabel;
        private System.Windows.Forms.Label AttributeMemoryLabel;
        private System.Windows.Forms.FlowLayoutPanel BioFlowLayoutPanel;
        private System.Windows.Forms.Label CharacterNameLabel;
        private System.Windows.Forms.Label BioInfoLabel;
        private System.Windows.Forms.Label CorporationNameLabel;
        private System.Windows.Forms.Label BalanceLabel;
        private System.Windows.Forms.FlowLayoutPanel ThrobberFlowLayoutPanel;
        private EVEMon.Controls.Throbber UpdateThrobber;
        private System.Windows.Forms.Label UpdateLabel;
    }
}
