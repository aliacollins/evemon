namespace EVEMon.SkillPlanner
{
    partial class EveObjectSelectControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EveObjectSelectControl));
            this.tbSearchText = new System.Windows.Forms.TextBox();
            this.lbSearchTextHint = new System.Windows.Forms.Label();
            this.lbNoMatches = new System.Windows.Forms.Label();
            this.lbSearchList = new System.Windows.Forms.ListBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiExpandSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbSkillFilter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbSearchImage = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tvItems = new CodersLab.Windows.Controls.TreeView();
            this.contextMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSearchText
            // 
            this.tbSearchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchText.Location = new System.Drawing.Point(31, 29);
            this.tbSearchText.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(152, 21);
            this.tbSearchText.TabIndex = 21;
            this.tbSearchText.TextChanged += new System.EventHandler(this.tbSearchText_TextChanged);
            this.tbSearchText.Leave += new System.EventHandler(this.tbSearchText_Leave);
            this.tbSearchText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSearchText_KeyPress);
            this.tbSearchText.Enter += new System.EventHandler(this.tbSearchText_Enter);
            // 
            // lbSearchTextHint
            // 
            this.lbSearchTextHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSearchTextHint.BackColor = System.Drawing.SystemColors.Window;
            this.lbSearchTextHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbSearchTextHint.Location = new System.Drawing.Point(32, 31);
            this.lbSearchTextHint.Name = "lbSearchTextHint";
            this.lbSearchTextHint.Size = new System.Drawing.Size(68, 14);
            this.lbSearchTextHint.TabIndex = 23;
            this.lbSearchTextHint.Text = "Search Text";
            this.lbSearchTextHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSearchTextHint.Click += new System.EventHandler(this.lbSearchTextHint_Click);
            // 
            // lbNoMatches
            // 
            this.lbNoMatches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbNoMatches.BackColor = System.Drawing.SystemColors.Window;
            this.lbNoMatches.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbNoMatches.Location = new System.Drawing.Point(4, 2);
            this.lbNoMatches.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbNoMatches.Name = "lbNoMatches";
            this.lbNoMatches.Size = new System.Drawing.Size(168, 40);
            this.lbNoMatches.TabIndex = 24;
            this.lbNoMatches.Text = "No ships match your search.";
            this.lbNoMatches.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbNoMatches.Visible = false;
            // 
            // lbSearchList
            // 
            this.lbSearchList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSearchList.FormattingEnabled = true;
            this.lbSearchList.IntegralHeight = false;
            this.lbSearchList.Location = new System.Drawing.Point(0, 0);
            this.lbSearchList.Margin = new System.Windows.Forms.Padding(2);
            this.lbSearchList.Name = "lbSearchList";
            this.lbSearchList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSearchList.Size = new System.Drawing.Size(185, 344);
            this.lbSearchList.TabIndex = 25;
            this.lbSearchList.Visible = false;
            this.lbSearchList.SelectedIndexChanged += new System.EventHandler(this.lbSearchList_SelectedIndexChanged);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiExpandSelected,
            this.cmiCollapseSelected,
            this.cmiExpandAll,
            this.cmiCollapseAll});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(170, 92);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.cbSkillFilter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lbSearchTextHint);
            this.panel1.Controls.Add(this.tbSearchText);
            this.panel1.Controls.Add(this.pbSearchImage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(185, 54);
            this.panel1.TabIndex = 26;
            // 
            // cbSkillFilter
            // 
            this.cbSkillFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSkillFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSkillFilter.FormattingEnabled = true;
            this.cbSkillFilter.Items.AddRange(new object[] {
            "All Ships",
            "Ships I Can Fly",
            "Ships I Can NOT Fly"});
            this.cbSkillFilter.Location = new System.Drawing.Point(31, 3);
            this.cbSkillFilter.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbSkillFilter.Name = "cbSkillFilter";
            this.cbSkillFilter.Size = new System.Drawing.Size(152, 21);
            this.cbSkillFilter.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Filter:";
            // 
            // pbSearchImage
            // 
            this.pbSearchImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbSearchImage.Image = ((System.Drawing.Image)(resources.GetObject("pbSearchImage.Image")));
            this.pbSearchImage.InitialImage = null;
            this.pbSearchImage.Location = new System.Drawing.Point(7, 29);
            this.pbSearchImage.Margin = new System.Windows.Forms.Padding(2);
            this.pbSearchImage.Name = "pbSearchImage";
            this.pbSearchImage.Size = new System.Drawing.Size(20, 20);
            this.pbSearchImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSearchImage.TabIndex = 20;
            this.pbSearchImage.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.lbNoMatches);
            this.panel2.Controls.Add(this.lbSearchList);
            this.panel2.Controls.Add(this.tvItems);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 54);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(185, 344);
            this.panel2.TabIndex = 27;
            // 
            // tvItems
            // 
            this.tvItems.ContextMenuStrip = this.contextMenu;
            this.tvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvItems.Location = new System.Drawing.Point(0, 0);
            this.tvItems.Margin = new System.Windows.Forms.Padding(2);
            this.tvItems.Name = "tvItems";
            this.tvItems.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.tvItems.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;
            this.tvItems.Size = new System.Drawing.Size(185, 344);
            this.tvItems.TabIndex = 22;
            this.tvItems.SelectionsChanged += new System.EventHandler(this.tvItems_SelectionsChanged);
            // 
            // EveObjectSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "EveObjectSelectControl";
            this.Size = new System.Drawing.Size(185, 398);
            this.Load += new System.EventHandler(this.EveObjectSelectControl_Load);
            this.contextMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ComboBox cbSkillFilter;
        protected System.Windows.Forms.TextBox tbSearchText;
        protected CodersLab.Windows.Controls.TreeView tvItems;
        protected System.Windows.Forms.Label lbNoMatches;
        protected System.Windows.Forms.ListBox lbSearchList;
        protected System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Panel panel2;
        protected System.Windows.Forms.Label lbSearchTextHint;
        protected System.Windows.Forms.PictureBox pbSearchImage;
        protected System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandAll;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseAll;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandSelected;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseSelected;
    }
}
