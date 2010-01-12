namespace EVEMon.SkillPlanner
{
    partial class RequiredSkillsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequiredSkillsControl));
            this.btnAddSkills = new System.Windows.Forms.Button();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTimeRequired = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tvSkillList = new EVEMon.SkillPlanner.ReqSkillsTreeView();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.planToMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.level1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showInSkillsBrowserMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.showInSkillsExplorerMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSkills
            // 
            this.btnAddSkills.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSkills.Location = new System.Drawing.Point(153, 4);
            this.btnAddSkills.Name = "btnAddSkills";
            this.btnAddSkills.Size = new System.Drawing.Size(88, 23);
            this.btnAddSkills.TabIndex = 0;
            this.btnAddSkills.Text = "Add All To Plan";
            this.btnAddSkills.UseVisualStyleBackColor = true;
            this.btnAddSkills.Click += new System.EventHandler(this.btnAddSkills_Click);
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "Cross");
            this.ilIcons.Images.SetKeyName(1, "Tick");
            this.ilIcons.Images.SetKeyName(2, "Planned");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblTimeRequired);
            this.panel1.Controls.Add(this.btnAddSkills);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 127);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(244, 30);
            this.panel1.TabIndex = 4;
            // 
            // lblTimeRequired
            // 
            this.lblTimeRequired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTimeRequired.AutoSize = true;
            this.lblTimeRequired.Location = new System.Drawing.Point(3, 9);
            this.lblTimeRequired.Name = "lblTimeRequired";
            this.lblTimeRequired.Size = new System.Drawing.Size(71, 13);
            this.lblTimeRequired.TabIndex = 1;
            this.lblTimeRequired.Text = "Time required";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tvSkillList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(244, 127);
            this.panel2.TabIndex = 5;
            // 
            // tvSkillList
            // 
            this.tvSkillList.ContextMenuStrip = this.contextMenu;
            this.tvSkillList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSkillList.FullRowSelect = true;
            this.tvSkillList.ImageIndex = 0;
            this.tvSkillList.ImageList = this.ilIcons;
            this.tvSkillList.Location = new System.Drawing.Point(0, 0);
            this.tvSkillList.Name = "tvSkillList";
            this.tvSkillList.SelectedImageIndex = 0;
            this.tvSkillList.Size = new System.Drawing.Size(244, 127);
            this.tvSkillList.TabIndex = 0;
            this.tvSkillList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvSkillList_NodeMouseDoubleClick);
            this.tvSkillList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvSkillList_NodeMouseClick);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.planToMenu,
            this.toolStripSeparator1,
            this.showInSkillsBrowserMenu,
            this.showInSkillsExplorerMenu});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(200, 76);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // planToMenu
            // 
            this.planToMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.level1ToolStripMenuItem,
            this.level2ToolStripMenuItem,
            this.level3ToolStripMenuItem,
            this.level4ToolStripMenuItem,
            this.level5ToolStripMenuItem});
            this.planToMenu.Image = global::EVEMon.Properties.Resources.PlanEdit;
            this.planToMenu.Name = "planToMenu";
            this.planToMenu.Size = new System.Drawing.Size(199, 22);
            this.planToMenu.Text = "&Plan to";
            // 
            // level1ToolStripMenuItem
            // 
            this.level1ToolStripMenuItem.Name = "level1ToolStripMenuItem";
            this.level1ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.level1ToolStripMenuItem.Text = "Level &1";
            this.level1ToolStripMenuItem.Click += new System.EventHandler(this.planToMenuItem_Click);
            // 
            // level2ToolStripMenuItem
            // 
            this.level2ToolStripMenuItem.Name = "level2ToolStripMenuItem";
            this.level2ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.level2ToolStripMenuItem.Text = "Level &2";
            this.level2ToolStripMenuItem.Click += new System.EventHandler(this.planToMenuItem_Click);
            // 
            // level3ToolStripMenuItem
            // 
            this.level3ToolStripMenuItem.Name = "level3ToolStripMenuItem";
            this.level3ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.level3ToolStripMenuItem.Text = "Level &3";
            this.level3ToolStripMenuItem.Click += new System.EventHandler(this.planToMenuItem_Click);
            // 
            // level4ToolStripMenuItem
            // 
            this.level4ToolStripMenuItem.Name = "level4ToolStripMenuItem";
            this.level4ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.level4ToolStripMenuItem.Text = "Level &4";
            this.level4ToolStripMenuItem.Click += new System.EventHandler(this.planToMenuItem_Click);
            // 
            // level5ToolStripMenuItem
            // 
            this.level5ToolStripMenuItem.Name = "level5ToolStripMenuItem";
            this.level5ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.level5ToolStripMenuItem.Text = "Level &5";
            this.level5ToolStripMenuItem.Click += new System.EventHandler(this.planToMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // showInSkillsBrowserMenu
            // 
            this.showInSkillsBrowserMenu.Name = "showInSkillsBrowserMenu";
            this.showInSkillsBrowserMenu.Size = new System.Drawing.Size(199, 22);
            this.showInSkillsBrowserMenu.Text = "Show in Skills &Browser...";
            this.showInSkillsBrowserMenu.Click += new System.EventHandler(this.showInSkillsBrowserMenu_Click);
            // 
            // showInSkillsExplorerMenu
            // 
            this.showInSkillsExplorerMenu.Image = global::EVEMon.Properties.Resources.LeadsTo;
            this.showInSkillsExplorerMenu.Name = "showInSkillsExplorerMenu";
            this.showInSkillsExplorerMenu.Size = new System.Drawing.Size(199, 22);
            this.showInSkillsExplorerMenu.Text = "Show in Skills Explorer...";
            this.showInSkillsExplorerMenu.Click += new System.EventHandler(this.showInSkillsExplorerMenu_Click);
            // 
            // RequiredSkillsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "RequiredSkillsControl";
            this.Size = new System.Drawing.Size(244, 157);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Button btnAddSkills;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTimeRequired;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ImageList ilIcons;
        private ReqSkillsTreeView tvSkillList;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem planToMenu;
        private System.Windows.Forms.ToolStripMenuItem level1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showInSkillsBrowserMenu;
        private System.Windows.Forms.ToolStripMenuItem showInSkillsExplorerMenu;
    }
}
