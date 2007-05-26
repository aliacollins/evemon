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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiExpandSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.lbSearchList = new System.Windows.Forms.ListBox();
            this.lbSearchTextHint = new System.Windows.Forms.Label();
            this.lbNoMatches = new System.Windows.Forms.Label();
            this.cbShowNonPublic = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSorting = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lvSortedSkillList = new System.Windows.Forms.ListView();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.chSortKey = new System.Windows.Forms.ColumnHeader();
            this.ilSkillIcons1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            "Known",
            "Level I Ready",
            "Not Known",
            "Not Known - Owned",
            "Not Known - Trainable",
            "Not Known - Unowned",
            "Not Planned",
            "Not Planned - Trainable",
            "Partially Trained",
            "Planned",
            "Trainable (All)"});
            this.cbSkillFilter.Location = new System.Drawing.Point(31, 3);
            this.cbSkillFilter.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbSkillFilter.MaxDropDownItems = 12;
            this.cbSkillFilter.Name = "cbSkillFilter";
            this.cbSkillFilter.Size = new System.Drawing.Size(231, 21);
            this.cbSkillFilter.Sorted = true;
            this.cbSkillFilter.TabIndex = 0;
            this.cbSkillFilter.SelectedIndexChanged += new System.EventHandler(this.cbSkillFilter_SelectedIndexChanged);
            // 
            // tbSearchText
            // 
            this.tbSearchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchText.Location = new System.Drawing.Point(31, 54);
            this.tbSearchText.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(231, 21);
            this.tbSearchText.TabIndex = 1;
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
            this.tvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvItems.ContextMenuStrip = this.contextMenuStrip1;
            this.tvItems.HideSelection = false;
            this.tvItems.Location = new System.Drawing.Point(0, 0);
            this.tvItems.Margin = new System.Windows.Forms.Padding(2);
            this.tvItems.Name = "tvItems";
            this.tvItems.Size = new System.Drawing.Size(262, 313);
            this.tvItems.TabIndex = 20;
            this.tvItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSkillList_AfterSelect);
            this.tvItems.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvItems_NodeMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiExpandSelected,
            this.cmiCollapseSelected,
            this.cmiExpandAll,
            this.cmiCollapseAll});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(170, 114);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
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
            // lbSearchList
            // 
            this.lbSearchList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSearchList.FormattingEnabled = true;
            this.lbSearchList.IntegralHeight = false;
            this.lbSearchList.Location = new System.Drawing.Point(0, 0);
            this.lbSearchList.Margin = new System.Windows.Forms.Padding(2);
            this.lbSearchList.Name = "lbSearchList";
            this.lbSearchList.Size = new System.Drawing.Size(262, 313);
            this.lbSearchList.TabIndex = 21;
            this.lbSearchList.Visible = false;
            this.lbSearchList.SelectedIndexChanged += new System.EventHandler(this.lbSearchList_SelectedIndexChanged);
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
            this.lbNoMatches.Size = new System.Drawing.Size(256, 22);
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
            this.cbShowNonPublic.Size = new System.Drawing.Size(262, 22);
            this.cbShowNonPublic.TabIndex = 24;
            this.cbShowNonPublic.Text = "Show Non-Public Skills";
            this.cbShowNonPublic.UseVisualStyleBackColor = true;
            this.cbShowNonPublic.CheckedChanged += new System.EventHandler(this.cbShowNonPublic_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Filter:";
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
            "Time to Level V"});
            this.cbSorting.Location = new System.Drawing.Point(31, 29);
            this.cbSorting.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbSorting.Name = "cbSorting";
            this.cbSorting.Size = new System.Drawing.Size(231, 21);
            this.cbSorting.TabIndex = 26;
            this.cbSorting.SelectedIndexChanged += new System.EventHandler(this.cbSorting_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-1, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Sort:";
            // 
            // lvSortedSkillList
            // 
            this.lvSortedSkillList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSortKey});
            this.lvSortedSkillList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSortedSkillList.FullRowSelect = true;
            this.lvSortedSkillList.Location = new System.Drawing.Point(0, 0);
            this.lvSortedSkillList.Margin = new System.Windows.Forms.Padding(2);
            this.lvSortedSkillList.Name = "lvSortedSkillList";
            this.lvSortedSkillList.Size = new System.Drawing.Size(262, 313);
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
            // ilSkillIcons1
            // 
            this.ilSkillIcons1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSkillIcons1.ImageStream")));
            this.ilSkillIcons1.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSkillIcons1.Images.SetKeyName(0, "book");
            this.ilSkillIcons1.Images.SetKeyName(1, "PrereqsNOTMet");
            this.ilSkillIcons1.Images.SetKeyName(2, "PrereqsMet");
            this.ilSkillIcons1.Images.SetKeyName(3, "lvl0");
            this.ilSkillIcons1.Images.SetKeyName(4, "lvl1");
            this.ilSkillIcons1.Images.SetKeyName(5, "lvl2");
            this.ilSkillIcons1.Images.SetKeyName(6, "lvl3");
            this.ilSkillIcons1.Images.SetKeyName(7, "lvl4");
            this.ilSkillIcons1.Images.SetKeyName(8, "lvl5");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbSorting);
            this.panel1.Controls.Add(this.cbSkillFilter);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.pbSearchImage);
            this.panel1.Controls.Add(this.lbSearchTextHint);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbSearchText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(262, 78);
            this.panel1.TabIndex = 29;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.lbNoMatches);
            this.panel2.Controls.Add(this.lbSearchList);
            this.panel2.Controls.Add(this.tvItems);
            this.panel2.Controls.Add(this.lvSortedSkillList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 78);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(262, 313);
            this.panel2.TabIndex = 30;
            // 
            // SkillSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbShowNonPublic);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SkillSelectControl";
            this.Size = new System.Drawing.Size(262, 413);
            this.Load += new System.EventHandler(this.SkillSelectControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbSorting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvSortedSkillList;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSortKey;
        private System.Windows.Forms.ImageList ilSkillIcons1;
        public System.Windows.Forms.TreeView tvItems;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cbSkillFilter;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandAll;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseAll;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandSelected;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseSelected;
    }
}
