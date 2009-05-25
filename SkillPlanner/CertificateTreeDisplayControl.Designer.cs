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
            this.tsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsmExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tsmAddToPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmBrowse = new System.Windows.Forms.ToolStripMenuItem();
            this.cmListSkills.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.ContextMenuStrip = this.cmListSkills;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.FullRowSelect = true;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Indent = 27;
            this.treeView.ItemHeight = 32;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(359, 461);
            this.treeView.TabIndex = 0;
            // 
            // cmListSkills
            // 
            this.cmListSkills.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAddToPlan,
            this.tsmBrowse,
            this.tsSeparator,
            this.tsmExpandAll,
            this.tsmCollapseAll});
            this.cmListSkills.Name = "cmListSkills";
            this.cmListSkills.Size = new System.Drawing.Size(153, 120);
            // 
            // tsSeparator
            // 
            this.tsSeparator.Name = "tsSeparator";
            this.tsSeparator.Size = new System.Drawing.Size(149, 6);
            // 
            // tsmExpandAll
            // 
            this.tsmExpandAll.Name = "tsmExpandAll";
            this.tsmExpandAll.Size = new System.Drawing.Size(152, 22);
            this.tsmExpandAll.Text = "&Expand all";
            this.tsmExpandAll.Click += new System.EventHandler(this.tsmExpandAll_Click);
            // 
            // tsmCollapseAll
            // 
            this.tsmCollapseAll.Name = "tsmCollapseAll";
            this.tsmCollapseAll.Size = new System.Drawing.Size(152, 22);
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
            // tsmAddToPlan
            // 
            this.tsmAddToPlan.Name = "tsmAddToPlan";
            this.tsmAddToPlan.Size = new System.Drawing.Size(152, 22);
            this.tsmAddToPlan.Text = "Add to plan";
            this.tsmAddToPlan.Click += new System.EventHandler(this.tsmAddToPlan_Click);
            // 
            // tsmBrowse
            // 
            this.tsmBrowse.Name = "tsmBrowse";
            this.tsmBrowse.Size = new System.Drawing.Size(152, 22);
            this.tsmBrowse.Text = "&Browse details";
            this.tsmBrowse.Click += new System.EventHandler(this.tsmBrowse_Click);
            // 
            // CertificateTreeDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Name = "CertificateTreeDisplayControl";
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
        private System.Windows.Forms.ToolStripMenuItem tsmBrowse;
    }
}
