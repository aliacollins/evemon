namespace EVEMon.SkillPlanner
{
    partial class CertificateTreeDisplayControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CertificateTreeDisplayControl));
            this.treeView = new System.Windows.Forms.TreeView();
            this.cmListSkills = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmAddToPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.showInMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.showInBrowserMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.showInExplorerMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsmExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.cmListSkills.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.ContextMenuStrip = this.cmListSkills;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView.FullRowSelect = true;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Indent = 27;
            this.treeView.ItemHeight = 32;
            this.treeView.Location = new System.Drawing.Point(0, 10);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(359, 441);
            this.treeView.TabIndex = 0;
            // 
            // cmListSkills
            // 
            this.cmListSkills.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAddToPlan,
            this.showInMenuSeparator,
            this.showInBrowserMenu,
            this.showInExplorerMenu,
            this.tsSeparator,
            this.tsmExpandAll,
            this.tsmCollapseAll});
            this.cmListSkills.Name = "cmListSkills";
            this.cmListSkills.Size = new System.Drawing.Size(185, 126);
            // 
            // tsmAddToPlan
            // 
            this.tsmAddToPlan.Name = "tsmAddToPlan";
            this.tsmAddToPlan.Size = new System.Drawing.Size(184, 22);
            this.tsmAddToPlan.Text = "Add to plan";
            this.tsmAddToPlan.Click += new System.EventHandler(this.tsmAddToPlan_Click);
            // 
            // showInMenuSeparator
            // 
            this.showInMenuSeparator.Name = "showInMenuSeparator";
            this.showInMenuSeparator.Size = new System.Drawing.Size(181, 6);
            // 
            // showInBrowserMenu
            // 
            this.showInBrowserMenu.Name = "showInBrowserMenu";
            this.showInBrowserMenu.Size = new System.Drawing.Size(184, 22);
            this.showInBrowserMenu.Text = "&Show in skill browser";
            this.showInBrowserMenu.Click += new System.EventHandler(this.showInBrowserMenu_Click);
            // 
            // showInExplorerMenu
            // 
            this.showInExplorerMenu.Name = "showInExplorerMenu";
            this.showInExplorerMenu.Size = new System.Drawing.Size(184, 22);
            this.showInExplorerMenu.Text = "Show in skill explorer";
            this.showInExplorerMenu.Click += new System.EventHandler(this.showInExplorerMenu_Click);
            // 
            // tsSeparator
            // 
            this.tsSeparator.Name = "tsSeparator";
            this.tsSeparator.Size = new System.Drawing.Size(181, 6);
            // 
            // tsmExpandAll
            // 
            this.tsmExpandAll.Name = "tsmExpandAll";
            this.tsmExpandAll.Size = new System.Drawing.Size(184, 22);
            this.tsmExpandAll.Text = "&Expand all";
            this.tsmExpandAll.Click += new System.EventHandler(this.tsmExpandAll_Click);
            // 
            // tsmCollapseAll
            // 
            this.tsmCollapseAll.Name = "tsmCollapseAll";
            this.tsmCollapseAll.Size = new System.Drawing.Size(184, 22);
            this.tsmCollapseAll.Text = "&Collapse all";
            this.tsmCollapseAll.Click += new System.EventHandler(this.tsmCollapseAll_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Certificate - Granted.png");
            this.imageList.Images.SetKeyName(1, "Certificate - Claimable.png");
            this.imageList.Images.SetKeyName(2, "Certificate - Trainable.png");
            this.imageList.Images.SetKeyName(3, "Certificate - Untrainable.png");
            this.imageList.Images.SetKeyName(4, "Certificate.png");
            this.imageList.Images.SetKeyName(5, "Skillbook.png");
            // 
            // CertificateTreeDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.treeView);
            this.Name = "CertificateTreeDisplayControl";
            this.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.Size = new System.Drawing.Size(359, 461);
            this.cmListSkills.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ContextMenuStrip cmListSkills;
        private System.Windows.Forms.ToolStripSeparator tsSeparator;
        private System.Windows.Forms.ToolStripMenuItem tsmExpandAll;
        private System.Windows.Forms.ToolStripMenuItem tsmCollapseAll;
        private System.Windows.Forms.ToolStripMenuItem tsmAddToPlan;
        private System.Windows.Forms.ToolStripMenuItem showInBrowserMenu;
        private System.Windows.Forms.ToolStripSeparator showInMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerMenu;
    }
}
