namespace EVEMon.SkillPlanner
{
    partial class SkillSelectControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkillSelectControl));
            this.cbSkillFilter = new System.Windows.Forms.ComboBox();
            this.tbSearchText = new System.Windows.Forms.TextBox();
            this.pbSearchImage = new System.Windows.Forms.PictureBox();
            this.tvItems = new System.Windows.Forms.TreeView();
            this.cmSkills = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiExpandSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiPlanTo = new System.Windows.Forms.ToolStripMenuItem();
            this.level1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbSearchList = new System.Windows.Forms.ListBox();
            this.cmListSkills = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiLvPlanTo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLevel1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLevel2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLevel3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLevel4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLevel5 = new System.Windows.Forms.ToolStripMenuItem();
            this.lbSearchTextHint = new System.Windows.Forms.Label();
            this.lbNoMatches = new System.Windows.Forms.Label();
            this.cbShowNonPublic = new System.Windows.Forms.CheckBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.cbSorting = new System.Windows.Forms.ComboBox();
            this.lblSort = new System.Windows.Forms.Label();
            this.lvSortedSkillList = new System.Windows.Forms.ListView();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.chSortKey = new System.Windows.Forms.ColumnHeader();
            this.ilSkillIcons = new System.Windows.Forms.ImageList(this.components);
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.pnlResults = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).BeginInit();
            this.cmSkills.SuspendLayout();
            this.cmListSkills.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            this.pnlResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbSkillFilter
            // 
            this.cbSkillFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSkillFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSkillFilter.FormattingEnabled = true;
            this.cbSkillFilter.Items.AddRange(new object[] {
            "All",
            "Hide Level V Skills",
            "Known",
            "Level I Ready",
            "Not Known",
            "Not Known - Owned",
            "Not Known - Trainable",
            "Not Known - Unowned",
            "Not Known - Untrainable",
            "Not Planned",
            "Not Planned - Trainable",
            "Partially Trained",
            "Planned",
            "Trainable (All)",
            "Trainable on trial account"}); 
            this.cbSkillFilter.Location = new System.Drawing.Point(31, 3);
            this.cbSkillFilter.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbSkillFilter.MaxDropDownItems = 12;
            this.cbSkillFilter.Name = "cbSkillFilter";
            this.cbSkillFilter.Size = new System.Drawing.Size(169, 21);
            this.cbSkillFilter.Sorted = true;
            this.cbSkillFilter.TabIndex = 1;
            this.cbSkillFilter.SelectedIndexChanged += new System.EventHandler(this.cbSkillFilter_SelectedIndexChanged);
            // 
            // tbSearchText
            // 
            this.tbSearchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchText.Location = new System.Drawing.Point(31, 54);
            this.tbSearchText.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(169, 21);
            this.tbSearchText.TabIndex = 36;
            this.tbSearchText.Enter += new System.EventHandler(this.tbSearch_Enter);
            this.tbSearchText.Leave += new System.EventHandler(this.tbSearch_Leave);
            this.tbSearchText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSearch_KeyPress);
            this.tbSearchText.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
            // 
            // pbSearchImage
            // 
            this.pbSearchImage.Image = ((System.Drawing.Image)(resources.GetObject("pbSearchImage.Image")));
            this.pbSearchImage.InitialImage = null;
            this.pbSearchImage.Location = new System.Drawing.Point(7, 54);
            this.pbSearchImage.Margin = new System.Windows.Forms.Padding(2);
            this.pbSearchImage.Name = "pbSearchImage";
            this.pbSearchImage.Size = new System.Drawing.Size(20, 20);
            this.pbSearchImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSearchImage.TabIndex = 19;
            this.pbSearchImage.TabStop = false;
            // 
            // tvItems
            // 
            this.tvItems.ContextMenuStrip = this.cmSkills;
            this.tvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvItems.HideSelection = false;
            this.tvItems.Location = new System.Drawing.Point(0, 0);
            this.tvItems.Margin = new System.Windows.Forms.Padding(2);
            this.tvItems.Name = "tvItems";
            this.tvItems.Size = new System.Drawing.Size(200, 313);
            this.tvItems.TabIndex = 20;
            this.tvItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSkillList_AfterSelect);
            this.tvItems.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvItems_NodeMouseClick);
            this.tvItems.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvItems_ItemDrag);
            // 
            // cmSkills
            // 
            this.cmSkills.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiExpandSelected,
            this.cmiCollapseSelected,
            this.cmiExpandAll,
            this.cmiCollapseAll,
            this.toolStripSeparator1,
            this.cmiPlanTo});
            this.cmSkills.Name = "contextMenuStrip1";
            this.cmSkills.Size = new System.Drawing.Size(170, 120);
            this.cmSkills.Opening += new System.ComponentModel.CancelEventHandler(this.cmSkills_Opening);
            // 
            // cmiExpandSelected
            // 
            this.cmiExpandSelected.Name = "cmiExpandSelected";
            this.cmiExpandSelected.Size = new System.Drawing.Size(169, 22);
            this.cmiExpandSelected.Text = "Expand Selected";
            this.cmiExpandSelected.Click += new System.EventHandler(this.cmiExpandSelected_Click);
            // 
            // cmiCollapseSelected
            // 
            this.cmiCollapseSelected.Name = "cmiCollapseSelected";
            this.cmiCollapseSelected.Size = new System.Drawing.Size(169, 22);
            this.cmiCollapseSelected.Text = "Collapse Selected";
            this.cmiCollapseSelected.Click += new System.EventHandler(this.cmiCollapseSelected_Click);
            // 
            // cmiExpandAll
            // 
            this.cmiExpandAll.Name = "cmiExpandAll";
            this.cmiExpandAll.Size = new System.Drawing.Size(169, 22);
            this.cmiExpandAll.Text = "Expand All";
            this.cmiExpandAll.Click += new System.EventHandler(this.cmiExpandAll_Click);
            // 
            // cmiCollapseAll
            // 
            this.cmiCollapseAll.Name = "cmiCollapseAll";
            this.cmiCollapseAll.Size = new System.Drawing.Size(169, 22);
            this.cmiCollapseAll.Text = "Collapse All";
            this.cmiCollapseAll.Click += new System.EventHandler(this.cmiCollapseAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
            // 
            // cmiPlanTo
            // 
            this.cmiPlanTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.level1ToolStripMenuItem,
            this.level2ToolStripMenuItem,
            this.level3ToolStripMenuItem,
            this.level4ToolStripMenuItem,
            this.level5ToolStripMenuItem});
            this.cmiPlanTo.Name = "cmiPlanTo";
            this.cmiPlanTo.Size = new System.Drawing.Size(169, 22);
            this.cmiPlanTo.Text = "&Plan to...";
            // 
            // level1ToolStripMenuItem
            // 
            this.level1ToolStripMenuItem.Name = "level1ToolStripMenuItem";
            this.level1ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.level1ToolStripMenuItem.Text = "Level &1";
            this.level1ToolStripMenuItem.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // level2ToolStripMenuItem
            // 
            this.level2ToolStripMenuItem.Name = "level2ToolStripMenuItem";
            this.level2ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.level2ToolStripMenuItem.Text = "Level &2";
            this.level2ToolStripMenuItem.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // level3ToolStripMenuItem
            // 
            this.level3ToolStripMenuItem.Name = "level3ToolStripMenuItem";
            this.level3ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.level3ToolStripMenuItem.Text = "Level &3";
            this.level3ToolStripMenuItem.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // level4ToolStripMenuItem
            // 
            this.level4ToolStripMenuItem.Name = "level4ToolStripMenuItem";
            this.level4ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.level4ToolStripMenuItem.Text = "Level &4";
            this.level4ToolStripMenuItem.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // level5ToolStripMenuItem
            // 
            this.level5ToolStripMenuItem.Name = "level5ToolStripMenuItem";
            this.level5ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.level5ToolStripMenuItem.Text = "Level &5";
            this.level5ToolStripMenuItem.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // lbSearchList
            // 
            this.lbSearchList.ContextMenuStrip = this.cmListSkills;
            this.lbSearchList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSearchList.FormattingEnabled = true;
            this.lbSearchList.IntegralHeight = false;
            this.lbSearchList.Location = new System.Drawing.Point(0, 0);
            this.lbSearchList.Margin = new System.Windows.Forms.Padding(2);
            this.lbSearchList.Name = "lbSearchList";
            this.lbSearchList.Size = new System.Drawing.Size(200, 313);
            this.lbSearchList.TabIndex = 46;
            this.lbSearchList.Visible = false;
            this.lbSearchList.SelectedIndexChanged += new System.EventHandler(this.lbSearchList_SelectedIndexChanged);
            // 
            // cmListSkills
            // 
            this.cmListSkills.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiLvPlanTo});
            this.cmListSkills.Name = "cmListSkills";
            this.cmListSkills.Size = new System.Drawing.Size(133, 26);
            this.cmListSkills.Opening += new System.ComponentModel.CancelEventHandler(this.cmListSkills_Opening);
            // 
            // cmiLvPlanTo
            // 
            this.cmiLvPlanTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmLevel1,
            this.tsmLevel2,
            this.tsmLevel3,
            this.tsmLevel4,
            this.tsmLevel5});
            this.cmiLvPlanTo.Name = "cmiLvPlanTo";
            this.cmiLvPlanTo.Size = new System.Drawing.Size(132, 22);
            this.cmiLvPlanTo.Text = "&Plan To...";
            // 
            // tsmLevel1
            // 
            this.tsmLevel1.Name = "tsmLevel1";
            this.tsmLevel1.Size = new System.Drawing.Size(119, 22);
            this.tsmLevel1.Text = "Level &1";
            this.tsmLevel1.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // tsmLevel2
            // 
            this.tsmLevel2.Name = "tsmLevel2";
            this.tsmLevel2.Size = new System.Drawing.Size(119, 22);
            this.tsmLevel2.Text = "Level &2";
            this.tsmLevel2.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // tsmLevel3
            // 
            this.tsmLevel3.Name = "tsmLevel3";
            this.tsmLevel3.Size = new System.Drawing.Size(119, 22);
            this.tsmLevel3.Text = "Level &3";
            this.tsmLevel3.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // tsmLevel4
            // 
            this.tsmLevel4.Name = "tsmLevel4";
            this.tsmLevel4.Size = new System.Drawing.Size(119, 22);
            this.tsmLevel4.Text = "Level &4";
            this.tsmLevel4.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // tsmLevel5
            // 
            this.tsmLevel5.Name = "tsmLevel5";
            this.tsmLevel5.Size = new System.Drawing.Size(119, 22);
            this.tsmLevel5.Text = "Level &5";
            this.tsmLevel5.Click += new System.EventHandler(this.levelToolStripMenuItem_Click);
            // 
            // lbSearchTextHint
            // 
            this.lbSearchTextHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSearchTextHint.BackColor = System.Drawing.SystemColors.Window;
            this.lbSearchTextHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbSearchTextHint.Location = new System.Drawing.Point(32, 56);
            this.lbSearchTextHint.Name = "lbSearchTextHint";
            this.lbSearchTextHint.Size = new System.Drawing.Size(68, 14);
            this.lbSearchTextHint.TabIndex = 22;
            this.lbSearchTextHint.Text = "Search Text";
            this.lbSearchTextHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSearchTextHint.Click += new System.EventHandler(this.lblSearchTextHint_Click);
            // 
            // lbNoMatches
            // 
            this.lbNoMatches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbNoMatches.BackColor = System.Drawing.SystemColors.Window;
            this.lbNoMatches.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbNoMatches.Location = new System.Drawing.Point(4, 23);
            this.lbNoMatches.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbNoMatches.Name = "lbNoMatches";
            this.lbNoMatches.Padding = new System.Windows.Forms.Padding(4);
            this.lbNoMatches.Size = new System.Drawing.Size(194, 22);
            this.lbNoMatches.TabIndex = 23;
            this.lbNoMatches.Text = "No skills match your search.";
            this.lbNoMatches.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbNoMatches.Visible = false;
            // 
            // cbShowNonPublic
            // 
            this.cbShowNonPublic.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbShowNonPublic.Location = new System.Drawing.Point(0, 391);
            this.cbShowNonPublic.Margin = new System.Windows.Forms.Padding(2);
            this.cbShowNonPublic.Name = "cbShowNonPublic";
            this.cbShowNonPublic.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.cbShowNonPublic.Size = new System.Drawing.Size(200, 22);
            this.cbShowNonPublic.TabIndex = 50;
            this.cbShowNonPublic.Text = "Show Non-Public Skills";
            this.cbShowNonPublic.UseVisualStyleBackColor = true;
            this.cbShowNonPublic.CheckedChanged += new System.EventHandler(this.cbShowNonPublic_CheckedChanged);
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(-1, 6);
            this.lblFilter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(35, 13);
            this.lblFilter.TabIndex = 25;
            this.lblFilter.Text = "Filter:";
            // 
            // cbSorting
            // 
            this.cbSorting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSorting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSorting.FormattingEnabled = true;
            this.cbSorting.Items.AddRange(new object[] {
            "No Sorting",
            "Time to Next Level",
            "Time to Level V",
            "Skill rank"});
            this.cbSorting.Location = new System.Drawing.Point(31, 29);
            this.cbSorting.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbSorting.Name = "cbSorting";
            this.cbSorting.Size = new System.Drawing.Size(169, 21);
            this.cbSorting.TabIndex = 26;
            this.cbSorting.SelectedIndexChanged += new System.EventHandler(this.cbSorting_SelectedIndexChanged);
            // 
            // lblSort
            // 
            this.lblSort.AutoSize = true;
            this.lblSort.Location = new System.Drawing.Point(-1, 31);
            this.lblSort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSort.Name = "lblSort";
            this.lblSort.Size = new System.Drawing.Size(31, 13);
            this.lblSort.TabIndex = 27;
            this.lblSort.Text = "Sort:";
            // 
            // lvSortedSkillList
            // 
            this.lvSortedSkillList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSortKey});
            this.lvSortedSkillList.ContextMenuStrip = this.cmListSkills;
            this.lvSortedSkillList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSortedSkillList.FullRowSelect = true;
            this.lvSortedSkillList.Location = new System.Drawing.Point(0, 0);
            this.lvSortedSkillList.Margin = new System.Windows.Forms.Padding(2);
            this.lvSortedSkillList.Name = "lvSortedSkillList";
            this.lvSortedSkillList.Size = new System.Drawing.Size(200, 313);
            this.lvSortedSkillList.TabIndex = 28;
            this.lvSortedSkillList.TileSize = new System.Drawing.Size(16, 16);
            this.lvSortedSkillList.UseCompatibleStateImageBehavior = false;
            this.lvSortedSkillList.View = System.Windows.Forms.View.Details;
            this.lvSortedSkillList.Visible = false;
            this.lvSortedSkillList.SelectedIndexChanged += new System.EventHandler(this.lvSortedSkillList_SelectedIndexChanged);
            // 
            // chName
            // 
            this.chName.Text = "Name";
            // 
            // chSortKey
            // 
            this.chSortKey.Text = "Sort";
            // 
            // ilSkillIcons
            // 
            this.ilSkillIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSkillIcons.ImageStream")));
            this.ilSkillIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSkillIcons.Images.SetKeyName(0, "book");
            this.ilSkillIcons.Images.SetKeyName(1, "PrereqsNOTMet");
            this.ilSkillIcons.Images.SetKeyName(2, "PrereqsMet");
            this.ilSkillIcons.Images.SetKeyName(3, "lvl0");
            this.ilSkillIcons.Images.SetKeyName(4, "lvl1");
            this.ilSkillIcons.Images.SetKeyName(5, "lvl2");
            this.ilSkillIcons.Images.SetKeyName(6, "lvl3");
            this.ilSkillIcons.Images.SetKeyName(7, "lvl4");
            this.ilSkillIcons.Images.SetKeyName(8, "lvl5");
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.cbSorting);
            this.pnlFilter.Controls.Add(this.cbSkillFilter);
            this.pnlFilter.Controls.Add(this.lblSort);
            this.pnlFilter.Controls.Add(this.pbSearchImage);
            this.pnlFilter.Controls.Add(this.lbSearchTextHint);
            this.pnlFilter.Controls.Add(this.lblFilter);
            this.pnlFilter.Controls.Add(this.tbSearchText);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(200, 78);
            this.pnlFilter.TabIndex = 29;
            // 
            // pnlResults
            // 
            this.pnlResults.AutoSize = true;
            this.pnlResults.Controls.Add(this.lbNoMatches);
            this.pnlResults.Controls.Add(this.lbSearchList);
            this.pnlResults.Controls.Add(this.tvItems);
            this.pnlResults.Controls.Add(this.lvSortedSkillList);
            this.pnlResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlResults.Location = new System.Drawing.Point(0, 78);
            this.pnlResults.Margin = new System.Windows.Forms.Padding(0);
            this.pnlResults.Name = "pnlResults";
            this.pnlResults.Size = new System.Drawing.Size(200, 313);
            this.pnlResults.TabIndex = 30;
            // 
            // SkillSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.pnlResults);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.cbShowNonPublic);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SkillSelectControl";
            this.Size = new System.Drawing.Size(200, 413);
            this.Load += new System.EventHandler(this.SkillSelectControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).EndInit();
            this.cmSkills.ResumeLayout(false);
            this.cmListSkills.ResumeLayout(false);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlResults.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSearchText;
        private System.Windows.Forms.PictureBox pbSearchImage;
        private System.Windows.Forms.ListBox lbSearchList;
        private System.Windows.Forms.Label lbSearchTextHint;
        private System.Windows.Forms.Label lbNoMatches;
        private System.Windows.Forms.CheckBox cbShowNonPublic;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.ComboBox cbSorting;
        private System.Windows.Forms.Label lblSort;
        private System.Windows.Forms.ListView lvSortedSkillList;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSortKey;
        private System.Windows.Forms.ImageList ilSkillIcons;
        public System.Windows.Forms.TreeView tvItems;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Panel pnlResults;
        private System.Windows.Forms.ComboBox cbSkillFilter;
        private System.Windows.Forms.ContextMenuStrip cmSkills;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandAll;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseAll;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandSelected;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseSelected;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmiPlanTo;
        private System.Windows.Forms.ToolStripMenuItem level1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level5ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmListSkills;
        private System.Windows.Forms.ToolStripMenuItem cmiLvPlanTo;
        private System.Windows.Forms.ToolStripMenuItem tsmLevel1;
        private System.Windows.Forms.ToolStripMenuItem tsmLevel2;
        private System.Windows.Forms.ToolStripMenuItem tsmLevel3;
        private System.Windows.Forms.ToolStripMenuItem tsmLevel4;
        private System.Windows.Forms.ToolStripMenuItem tsmLevel5;
    }
}
