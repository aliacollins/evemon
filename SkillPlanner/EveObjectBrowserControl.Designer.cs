namespace EVEMon.SkillPlanner
{
    partial class EveObjectBrowserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EveObjectBrowserControl));
            this.scObjectBrowser = new EVEMon.SkillPlanner.PersistentSplitContainer();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.pnlBrowserHeader = new System.Windows.Forms.Panel();
            this.eoImage = new EVEMon.Common.EveImage();
            this.lblEveObjCategory = new System.Windows.Forms.Label();
            this.lblEveObjName = new System.Windows.Forms.Label();
            this.lblHelp = new System.Windows.Forms.Label();
            this.scObjectBrowser.Panel2.SuspendLayout();
            this.scObjectBrowser.SuspendLayout();
            this.pnlBrowserHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // scObjectBrowser
            // 
            this.scObjectBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scObjectBrowser.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scObjectBrowser.Location = new System.Drawing.Point(0, 0);
            this.scObjectBrowser.Margin = new System.Windows.Forms.Padding(2);
            this.scObjectBrowser.Name = "scObjectBrowser";
            // 
            // scObjectBrowser.Panel2
            // 
            this.scObjectBrowser.Panel2.Controls.Add(this.pnlDetails);
            this.scObjectBrowser.Panel2.Controls.Add(this.pnlBrowserHeader);
            this.scObjectBrowser.Panel2.Controls.Add(this.lblHelp);
            this.scObjectBrowser.RememberDistanceKey = null;
            this.scObjectBrowser.Size = new System.Drawing.Size(650, 413);
            this.scObjectBrowser.SplitterDistance = 163;
            this.scObjectBrowser.SplitterWidth = 5;
            this.scObjectBrowser.TabIndex = 0;
            // 
            // pnlDetails
            // 
            this.pnlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetails.Location = new System.Drawing.Point(0, 70);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(482, 343);
            this.pnlDetails.TabIndex = 13;
            // 
            // pnlBrowserHeader
            // 
            this.pnlBrowserHeader.Controls.Add(this.eoImage);
            this.pnlBrowserHeader.Controls.Add(this.lblEveObjCategory);
            this.pnlBrowserHeader.Controls.Add(this.lblEveObjName);
            this.pnlBrowserHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBrowserHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlBrowserHeader.Name = "pnlBrowserHeader";
            this.pnlBrowserHeader.Size = new System.Drawing.Size(482, 70);
            this.pnlBrowserHeader.TabIndex = 12;
            // 
            // eoImage
            // 
            this.eoImage.EveItem = null;
            this.eoImage.ImageSize = EVEMon.Common.EveImage.EveImageSize._64_64;
            this.eoImage.Location = new System.Drawing.Point(3, 3);
            this.eoImage.Name = "eoImage";
            this.eoImage.PopUpEnabled = true;
            this.eoImage.Size = new System.Drawing.Size(64, 64);
            this.eoImage.TabIndex = 8;
            // 
            // lblEveObjCategory
            // 
            this.lblEveObjCategory.AutoSize = true;
            this.lblEveObjCategory.Location = new System.Drawing.Point(70, 3);
            this.lblEveObjCategory.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblEveObjCategory.Name = "lblEveObjCategory";
            this.lblEveObjCategory.Size = new System.Drawing.Size(102, 13);
            this.lblEveObjCategory.TabIndex = 6;
            this.lblEveObjCategory.Text = "EveObject Category";
            // 
            // lblEveObjName
            // 
            this.lblEveObjName.AutoSize = true;
            this.lblEveObjName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEveObjName.Location = new System.Drawing.Point(70, 16);
            this.lblEveObjName.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblEveObjName.Name = "lblEveObjName";
            this.lblEveObjName.Size = new System.Drawing.Size(131, 18);
            this.lblEveObjName.TabIndex = 7;
            this.lblEveObjName.Text = "EveObject Name";
            this.lblEveObjName.Click += new System.EventHandler(EveObjName_Clicked);

            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.Location = new System.Drawing.Point(3, 3);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(378, 65);
            this.lblHelp.TabIndex = 1;
            this.lblHelp.Text = resources.GetString("lblHelp.Text");
            // 
            // EveObjectBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scObjectBrowser);
            this.Name = "EveObjectBrowserControl";
            this.Size = new System.Drawing.Size(650, 413);
            this.Load += new System.EventHandler(this.EveObjectBrowserControl_Load);
            this.scObjectBrowser.Panel2.ResumeLayout(false);
            this.scObjectBrowser.Panel2.PerformLayout();
            this.scObjectBrowser.ResumeLayout(false);
            this.pnlBrowserHeader.ResumeLayout(false);
            this.pnlBrowserHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected PersistentSplitContainer scObjectBrowser;
        protected EVEMon.Common.EveImage eoImage;
        protected System.Windows.Forms.Label lblEveObjCategory;
        protected System.Windows.Forms.Label lblEveObjName;
        protected System.Windows.Forms.Panel pnlDetails;
        protected System.Windows.Forms.Panel pnlBrowserHeader;
        protected System.Windows.Forms.Label lblHelp;

    }
}
