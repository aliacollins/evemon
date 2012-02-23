using EVEMon.Common.Controls;

namespace EVEMon.SkillPlanner
{
    partial class SkillExplorerWindow
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
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node0");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node0");
            this.grpPlanName = new System.Windows.Forms.GroupBox();
            this.lblSkillInfo = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lowerPanel = new System.Windows.Forms.Panel();
            this.lblRedFont = new System.Windows.Forms.Label();
            this.lblDimmedFont = new System.Windows.Forms.Label();
            this.lblRedFontInfo = new System.Windows.Forms.Label();
            this.lblDimmedFontInfo = new System.Windows.Forms.Label();
            this.lblNormalFont = new System.Windows.Forms.Label();
            this.middlePanel = new System.Windows.Forms.Panel();
            this.splitContainer = new EVEMon.Common.Controls.PersistentSplitContainer();
            this.tvSkills = new System.Windows.Forms.TreeView();
            this.pnlSkillHeader = new System.Windows.Forms.Panel();
            this.lblSkills = new System.Windows.Forms.Label();
            this.tvEntity = new System.Windows.Forms.TreeView();
            this.pnlItemHeader = new System.Windows.Forms.Panel();
            this.lblItems = new System.Windows.Forms.Label();
            this.cmSkills = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsAddPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAddL1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAddL2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAddL3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAddL4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAddL5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSwitch = new System.Windows.Forms.ToolStripMenuItem();
            this.tsShowInBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.tsShowSkillPrereqs = new System.Windows.Forms.ToolStripMenuItem();
            this.cmEntity = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsShowObjectInBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAddObjectToPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsShowObjectPrereqs = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbHistory = new System.Windows.Forms.ComboBox();
            this.cbShowBaseOnly = new System.Windows.Forms.CheckBox();
            this.rbShowAlpha = new System.Windows.Forms.RadioButton();
            this.rbShowTree = new System.Windows.Forms.RadioButton();
            this.tmrAutoUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.grpPlanName.SuspendLayout();
            this.lowerPanel.SuspendLayout();
            this.middlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlSkillHeader.SuspendLayout();
            this.pnlItemHeader.SuspendLayout();
            this.cmSkills.SuspendLayout();
            this.cmEntity.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPlanName
            // 
            this.grpPlanName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPlanName.Controls.Add(this.lblSkillInfo);
            this.grpPlanName.Location = new System.Drawing.Point(12, 12);
            this.grpPlanName.Name = "grpPlanName";
            this.grpPlanName.Size = new System.Drawing.Size(553, 40);
            this.grpPlanName.TabIndex = 0;
            this.grpPlanName.TabStop = false;
            this.grpPlanName.Text = "Selected Skill Details";
            // 
            // lblSkillInfo
            // 
            this.lblSkillInfo.AutoSize = true;
            this.lblSkillInfo.Location = new System.Drawing.Point(12, 16);
            this.lblSkillInfo.Name = "lblSkillInfo";
            this.lblSkillInfo.Size = new System.Drawing.Size(47, 13);
            this.lblSkillInfo.TabIndex = 0;
            this.lblSkillInfo.Text = "Skill Info";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(475, 17);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lowerPanel
            // 
            this.lowerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lowerPanel.Controls.Add(this.lblRedFont);
            this.lowerPanel.Controls.Add(this.lblDimmedFont);
            this.lowerPanel.Controls.Add(this.lblRedFontInfo);
            this.lowerPanel.Controls.Add(this.lblDimmedFontInfo);
            this.lowerPanel.Controls.Add(this.lblNormalFont);
            this.lowerPanel.Controls.Add(this.btnClose);
            this.lowerPanel.Location = new System.Drawing.Point(12, 423);
            this.lowerPanel.Name = "lowerPanel";
            this.lowerPanel.Size = new System.Drawing.Size(550, 40);
            this.lowerPanel.TabIndex = 4;
            // 
            // lblRedFont
            // 
            this.lblRedFont.AutoSize = true;
            this.lblRedFont.ForeColor = System.Drawing.Color.Red;
            this.lblRedFont.Location = new System.Drawing.Point(6, 22);
            this.lblRedFont.Name = "lblRedFont";
            this.lblRedFont.Size = new System.Drawing.Size(22, 13);
            this.lblRedFont.TabIndex = 8;
            this.lblRedFont.Text = "red";
            // 
            // lblDimmedFont
            // 
            this.lblDimmedFont.AutoSize = true;
            this.lblDimmedFont.ForeColor = System.Drawing.Color.DimGray;
            this.lblDimmedFont.Location = new System.Drawing.Point(164, 9);
            this.lblDimmedFont.Name = "lblDimmedFont";
            this.lblDimmedFont.Size = new System.Drawing.Size(43, 13);
            this.lblDimmedFont.TabIndex = 6;
            this.lblDimmedFont.Text = "dimmed";
            // 
            // lblRedFontInfo
            // 
            this.lblRedFontInfo.AutoSize = true;
            this.lblRedFontInfo.Location = new System.Drawing.Point(26, 22);
            this.lblRedFontInfo.Name = "lblRedFontInfo";
            this.lblRedFontInfo.Size = new System.Drawing.Size(154, 13);
            this.lblRedFontInfo.TabIndex = 9;
            this.lblRedFontInfo.Text = "= other untrained skills needed.";
            // 
            // lblDimmedFontInfo
            // 
            this.lblDimmedFontInfo.AutoSize = true;
            this.lblDimmedFontInfo.Location = new System.Drawing.Point(204, 9);
            this.lblDimmedFontInfo.Name = "lblDimmedFontInfo";
            this.lblDimmedFontInfo.Size = new System.Drawing.Size(141, 13);
            this.lblDimmedFontInfo.TabIndex = 7;
            this.lblDimmedFontInfo.Text = "= unlocked by this skill level,";
            // 
            // lblNormalFont
            // 
            this.lblNormalFont.AutoSize = true;
            this.lblNormalFont.Location = new System.Drawing.Point(6, 9);
            this.lblNormalFont.Name = "lblNormalFont";
            this.lblNormalFont.Size = new System.Drawing.Size(145, 13);
            this.lblNormalFont.TabIndex = 5;
            this.lblNormalFont.Text = "Normal font = already trained,";
            // 
            // middlePanel
            // 
            this.middlePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.middlePanel.Controls.Add(this.splitContainer);
            this.middlePanel.Location = new System.Drawing.Point(12, 115);
            this.middlePanel.Name = "middlePanel";
            this.middlePanel.Size = new System.Drawing.Size(553, 302);
            this.middlePanel.TabIndex = 5;
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tvSkills);
            this.splitContainer.Panel1.Controls.Add(this.pnlSkillHeader);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tvEntity);
            this.splitContainer.Panel2.Controls.Add(this.pnlItemHeader);
            this.splitContainer.RememberDistanceKey = null;
            this.splitContainer.Size = new System.Drawing.Size(553, 302);
            this.splitContainer.SplitterDistance = 207;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 0;
            // 
            // tvSkills
            // 
            this.tvSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSkills.Location = new System.Drawing.Point(0, 18);
            this.tvSkills.Name = "tvSkills";
            treeNode3.Name = "Node0";
            treeNode3.Text = "Node0";
            this.tvSkills.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.tvSkills.ShowNodeToolTips = true;
            this.tvSkills.Size = new System.Drawing.Size(203, 280);
            this.tvSkills.TabIndex = 0;
            this.tvSkills.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvSkills_NodeMouseClick);
            this.tvSkills.DoubleClick += new System.EventHandler(this.tvSkills_DoubleClick);
            // 
            // pnlSkillHeader
            // 
            this.pnlSkillHeader.BackColor = System.Drawing.Color.Cornsilk;
            this.pnlSkillHeader.Controls.Add(this.lblSkills);
            this.pnlSkillHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSkillHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pnlSkillHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlSkillHeader.Name = "pnlSkillHeader";
            this.pnlSkillHeader.Size = new System.Drawing.Size(203, 18);
            this.pnlSkillHeader.TabIndex = 1;
            // 
            // lblSkills
            // 
            this.lblSkills.AutoSize = true;
            this.lblSkills.Location = new System.Drawing.Point(1, 2);
            this.lblSkills.Name = "lblSkills";
            this.lblSkills.Size = new System.Drawing.Size(73, 13);
            this.lblSkills.TabIndex = 0;
            this.lblSkills.Text = "Enabled Skills";
            // 
            // tvEntity
            // 
            this.tvEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvEntity.Location = new System.Drawing.Point(0, 18);
            this.tvEntity.Name = "tvEntity";
            treeNode4.Name = "Node0";
            treeNode4.Text = "Node0";
            this.tvEntity.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.tvEntity.ShowNodeToolTips = true;
            this.tvEntity.Size = new System.Drawing.Size(339, 280);
            this.tvEntity.TabIndex = 0;
            this.tvEntity.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvEntity_NodeMouseClick);
            this.tvEntity.DoubleClick += new System.EventHandler(this.tvEntity_DoubleClick);
            // 
            // pnlItemHeader
            // 
            this.pnlItemHeader.BackColor = System.Drawing.Color.LightCyan;
            this.pnlItemHeader.Controls.Add(this.lblItems);
            this.pnlItemHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlItemHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlItemHeader.Name = "pnlItemHeader";
            this.pnlItemHeader.Size = new System.Drawing.Size(339, 18);
            this.pnlItemHeader.TabIndex = 0;
            // 
            // lblItems
            // 
            this.lblItems.AutoSize = true;
            this.lblItems.Location = new System.Drawing.Point(1, 2);
            this.lblItems.Name = "lblItems";
            this.lblItems.Size = new System.Drawing.Size(74, 13);
            this.lblItems.TabIndex = 0;
            this.lblItems.Text = "Enabled Items";
            // 
            // cmSkills
            // 
            this.cmSkills.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsAddPlan,
            this.tsSwitch,
            this.tsShowInBrowser,
            this.tsShowSkillPrereqs});
            this.cmSkills.Name = "cmSkills";
            this.cmSkills.Size = new System.Drawing.Size(242, 92);
            // 
            // tsAddPlan
            // 
            this.tsAddPlan.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsAddL1,
            this.tsAddL2,
            this.tsAddL3,
            this.tsAddL4,
            this.tsAddL5});
            this.tsAddPlan.Name = "tsAddPlan";
            this.tsAddPlan.Size = new System.Drawing.Size(241, 22);
            this.tsAddPlan.Text = "Plan To...";
            // 
            // tsAddL1
            // 
            this.tsAddL1.Name = "tsAddL1";
            this.tsAddL1.Size = new System.Drawing.Size(110, 22);
            this.tsAddL1.Text = "Level 1";
            this.tsAddL1.Click += new System.EventHandler(this.tsAddLevel_Click);
            // 
            // tsAddL2
            // 
            this.tsAddL2.Name = "tsAddL2";
            this.tsAddL2.Size = new System.Drawing.Size(110, 22);
            this.tsAddL2.Text = "Level 2";
            this.tsAddL2.Click += new System.EventHandler(this.tsAddLevel_Click);
            // 
            // tsAddL3
            // 
            this.tsAddL3.Name = "tsAddL3";
            this.tsAddL3.Size = new System.Drawing.Size(110, 22);
            this.tsAddL3.Text = "Level 3";
            this.tsAddL3.Click += new System.EventHandler(this.tsAddLevel_Click);
            // 
            // tsAddL4
            // 
            this.tsAddL4.Name = "tsAddL4";
            this.tsAddL4.Size = new System.Drawing.Size(110, 22);
            this.tsAddL4.Text = "Level 4";
            this.tsAddL4.Click += new System.EventHandler(this.tsAddLevel_Click);
            // 
            // tsAddL5
            // 
            this.tsAddL5.Name = "tsAddL5";
            this.tsAddL5.Size = new System.Drawing.Size(110, 22);
            this.tsAddL5.Text = "Level 5";
            this.tsAddL5.Click += new System.EventHandler(this.tsAddLevel_Click);
            // 
            // tsSwitch
            // 
            this.tsSwitch.Name = "tsSwitch";
            this.tsSwitch.Size = new System.Drawing.Size(241, 22);
            this.tsSwitch.Text = "Show me what this skill unlocks";
            this.tsSwitch.Click += new System.EventHandler(this.tsSwitch_Click);
            // 
            // tsShowInBrowser
            // 
            this.tsShowInBrowser.Name = "tsShowInBrowser";
            this.tsShowInBrowser.Size = new System.Drawing.Size(241, 22);
            this.tsShowInBrowser.Text = "Show Skill In Browser";
            this.tsShowInBrowser.Click += new System.EventHandler(this.tsShowInBrowser_Click);
            // 
            // tsShowSkillPrereqs
            // 
            this.tsShowSkillPrereqs.Name = "tsShowSkillPrereqs";
            this.tsShowSkillPrereqs.Size = new System.Drawing.Size(241, 22);
            this.tsShowSkillPrereqs.Text = "Show Untrained Preqresites";
            this.tsShowSkillPrereqs.Click += new System.EventHandler(this.tsShowSkillPrereqs_Click);
            // 
            // cmEntity
            // 
            this.cmEntity.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsShowObjectInBrowser,
            this.tsAddObjectToPlan,
            this.tsShowObjectPrereqs});
            this.cmEntity.Name = "cmShips";
            this.cmEntity.Size = new System.Drawing.Size(229, 70);
            // 
            // tsShowObjectInBrowser
            // 
            this.tsShowObjectInBrowser.Name = "tsShowObjectInBrowser";
            this.tsShowObjectInBrowser.Size = new System.Drawing.Size(228, 22);
            this.tsShowObjectInBrowser.Text = "Show In Browser";
            this.tsShowObjectInBrowser.Click += new System.EventHandler(this.tvEntity_DoubleClick);
            // 
            // tsAddObjectToPlan
            // 
            this.tsAddObjectToPlan.Name = "tsAddObjectToPlan";
            this.tsAddObjectToPlan.Size = new System.Drawing.Size(228, 22);
            this.tsAddObjectToPlan.Text = "Add To Plan...";
            this.tsAddObjectToPlan.Click += new System.EventHandler(this.tsAddEntityToPlan_Click);
            // 
            // tsShowObjectPrereqs
            // 
            this.tsShowObjectPrereqs.Name = "tsShowObjectPrereqs";
            this.tsShowObjectPrereqs.Size = new System.Drawing.Size(228, 22);
            this.tsShowObjectPrereqs.Text = "Show Untrained Prerequisites";
            this.tsShowObjectPrereqs.Click += new System.EventHandler(this.tsShowItemPrereqs_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbHistory);
            this.groupBox2.Controls.Add(this.cbShowBaseOnly);
            this.groupBox2.Controls.Add(this.rbShowAlpha);
            this.groupBox2.Controls.Add(this.rbShowTree);
            this.groupBox2.Location = new System.Drawing.Point(12, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(553, 51);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // cbHistory
            // 
            this.cbHistory.DisplayMember = "Name";
            this.cbHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHistory.FormattingEnabled = true;
            this.cbHistory.Location = new System.Drawing.Point(5, 19);
            this.cbHistory.Name = "cbHistory";
            this.cbHistory.Size = new System.Drawing.Size(137, 21);
            this.cbHistory.TabIndex = 3;
            this.cbHistory.SelectedIndexChanged += new System.EventHandler(this.cbHistory_SelectedIndexChanged);
            // 
            // cbShowBaseOnly
            // 
            this.cbShowBaseOnly.AutoSize = true;
            this.cbShowBaseOnly.Location = new System.Drawing.Point(411, 21);
            this.cbShowBaseOnly.Name = "cbShowBaseOnly";
            this.cbShowBaseOnly.Size = new System.Drawing.Size(109, 17);
            this.cbShowBaseOnly.TabIndex = 2;
            this.cbShowBaseOnly.Text = "Only show T1/T2";
            this.cbShowBaseOnly.UseVisualStyleBackColor = true;
            this.cbShowBaseOnly.CheckedChanged += new System.EventHandler(this.cbShowBaseOnly_CheckedChanged);
            // 
            // rbShowAlpha
            // 
            this.rbShowAlpha.AutoSize = true;
            this.rbShowAlpha.Checked = true;
            this.rbShowAlpha.Location = new System.Drawing.Point(148, 20);
            this.rbShowAlpha.Name = "rbShowAlpha";
            this.rbShowAlpha.Size = new System.Drawing.Size(124, 17);
            this.rbShowAlpha.TabIndex = 1;
            this.rbShowAlpha.TabStop = true;
            this.rbShowAlpha.Text = "Show Alphabetic List";
            this.rbShowAlpha.UseVisualStyleBackColor = true;
            this.rbShowAlpha.CheckedChanged += new System.EventHandler(this.rbShowAlpha_CheckedChanged);
            // 
            // rbShowTree
            // 
            this.rbShowTree.AutoSize = true;
            this.rbShowTree.Location = new System.Drawing.Point(278, 20);
            this.rbShowTree.Name = "rbShowTree";
            this.rbShowTree.Size = new System.Drawing.Size(127, 17);
            this.rbShowTree.TabIndex = 0;
            this.rbShowTree.Text = "Show Category Trees";
            this.rbShowTree.UseVisualStyleBackColor = true;
            // 
            // tmrAutoUpdate
            // 
            this.tmrAutoUpdate.Interval = 30000;
            this.tmrAutoUpdate.Tick += new System.EventHandler(this.tmrAutoUpdate_Tick);
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            // 
            // SkillExplorerWindow
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(577, 475);
            this.Controls.Add(this.grpPlanName);
            this.Controls.Add(this.middlePanel);
            this.Controls.Add(this.lowerPanel);
            this.Controls.Add(this.groupBox2);
            this.MinimumSize = new System.Drawing.Size(585, 480);
            this.Name = "SkillExplorerWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Skill Explorer - What Is This Skill Used For?";
            this.grpPlanName.ResumeLayout(false);
            this.grpPlanName.PerformLayout();
            this.lowerPanel.ResumeLayout(false);
            this.lowerPanel.PerformLayout();
            this.middlePanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlSkillHeader.ResumeLayout(false);
            this.pnlSkillHeader.PerformLayout();
            this.pnlItemHeader.ResumeLayout(false);
            this.pnlItemHeader.PerformLayout();
            this.cmSkills.ResumeLayout(false);
            this.cmEntity.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.GroupBox grpPlanName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel lowerPanel;
        private System.Windows.Forms.Panel middlePanel;
        private PersistentSplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbShowBaseOnly;
        private System.Windows.Forms.RadioButton rbShowAlpha;
        private System.Windows.Forms.RadioButton rbShowTree;
        private System.Windows.Forms.Label lblSkillInfo;
        private System.Windows.Forms.TreeView tvSkills;
        private System.Windows.Forms.ContextMenuStrip cmSkills;
        private System.Windows.Forms.ToolStripMenuItem tsAddPlan;
        private System.Windows.Forms.ToolStripMenuItem tsSwitch;
        private System.Windows.Forms.ToolStripMenuItem tsShowInBrowser;
        private System.Windows.Forms.ToolStripMenuItem tsShowSkillPrereqs;
        private System.Windows.Forms.ToolStripMenuItem tsAddL1;
        private System.Windows.Forms.ToolStripMenuItem tsAddL2;
        private System.Windows.Forms.ToolStripMenuItem tsAddL3;
        private System.Windows.Forms.ToolStripMenuItem tsAddL4;
        private System.Windows.Forms.ToolStripMenuItem tsAddL5;
        private System.Windows.Forms.Panel pnlSkillHeader;
        private System.Windows.Forms.Label lblSkills;
        private System.Windows.Forms.TreeView tvEntity;
        private System.Windows.Forms.Panel pnlItemHeader;
        private System.Windows.Forms.Label lblItems;
        private System.Windows.Forms.ContextMenuStrip cmEntity;
        private System.Windows.Forms.ToolStripMenuItem tsAddObjectToPlan;
        private System.Windows.Forms.ToolStripMenuItem tsShowObjectPrereqs;
        private System.Windows.Forms.ToolStripMenuItem tsShowObjectInBrowser;
        private System.Windows.Forms.Label lblDimmedFont;
        private System.Windows.Forms.Label lblNormalFont;
        private System.Windows.Forms.Label lblRedFontInfo;
        private System.Windows.Forms.Label lblRedFont;
        private System.Windows.Forms.Label lblDimmedFontInfo;
        private System.Windows.Forms.Timer tmrAutoUpdate;
        private System.Windows.Forms.ComboBox cbHistory;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
