namespace EVEMon.SkillPlanner
{
    partial class CertificateBrowserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CertificateBrowserControl));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tslbTextForEligibility = new System.Windows.Forms.ToolStripLabel();
            this.tslbEligible = new System.Windows.Forms.ToolStripLabel();
            this.tsPlanToMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsPlanToBasic = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlanToStandard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlanToImproved = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlanToElite = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.textboxDescription = new System.Windows.Forms.TextBox();
            this.lblLevel4Time = new System.Windows.Forms.Label();
            this.lblLevel3Time = new System.Windows.Forms.Label();
            this.lblLevel2Time = new System.Windows.Forms.Label();
            this.lblLevel1Time = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.certSelectCtl = new EVEMon.SkillPlanner.CertificateSelectControl();
            this.certDisplayCtl = new EVEMon.SkillPlanner.CertificateTreeDisplayControl();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.toolStrip1);
            this.panelHeader.Controls.Add(this.pictureBox1);
            this.panelHeader.Controls.Add(this.lblCategory);
            this.panelHeader.Controls.Add(this.textboxDescription);
            this.panelHeader.Controls.Add(this.lblLevel4Time);
            this.panelHeader.Controls.Add(this.lblLevel3Time);
            this.panelHeader.Controls.Add(this.lblLevel2Time);
            this.panelHeader.Controls.Add(this.lblLevel1Time);
            this.panelHeader.Controls.Add(this.lblName);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(588, 116);
            this.panelHeader.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslbTextForEligibility,
            this.tslbEligible,
            this.tsPlanToMenu});
            this.toolStrip1.Location = new System.Drawing.Point(0, 91);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(588, 25);
            this.toolStrip1.TabIndex = 28;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tslbTextForEligibility
            // 
            this.tslbTextForEligibility.Name = "tslbTextForEligibility";
            this.tslbTextForEligibility.Size = new System.Drawing.Size(181, 22);
            this.tslbTextForEligibility.Text = "After this plan you wil be eligible to :";
            // 
            // tslbEligible
            // 
            this.tslbEligible.Name = "tslbEligible";
            this.tslbEligible.Size = new System.Drawing.Size(31, 22);
            this.tslbEligible.Text = "none";
            // 
            // tsPlanToMenu
            // 
            this.tsPlanToMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsPlanToMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsPlanToMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsPlanToBasic,
            this.tsPlanToStandard,
            this.tsPlanToImproved,
            this.tsPlanToElite});
            this.tsPlanToMenu.Image = ((System.Drawing.Image)(resources.GetObject("tsPlanToMenu.Image")));
            this.tsPlanToMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPlanToMenu.Name = "tsPlanToMenu";
            this.tsPlanToMenu.Size = new System.Drawing.Size(65, 22);
            this.tsPlanToMenu.Text = "Plan to...";
            // 
            // tsPlanToBasic
            // 
            this.tsPlanToBasic.Name = "tsPlanToBasic";
            this.tsPlanToBasic.Size = new System.Drawing.Size(131, 22);
            this.tsPlanToBasic.Text = "&Basic";
            this.tsPlanToBasic.Click += new System.EventHandler(this.tsPlanToBasic_Click);
            // 
            // tsPlanToStandard
            // 
            this.tsPlanToStandard.Name = "tsPlanToStandard";
            this.tsPlanToStandard.Size = new System.Drawing.Size(131, 22);
            this.tsPlanToStandard.Text = "&Standard";
            this.tsPlanToStandard.Click += new System.EventHandler(this.tsPlanToStandard_Click);
            // 
            // tsPlanToImproved
            // 
            this.tsPlanToImproved.Name = "tsPlanToImproved";
            this.tsPlanToImproved.Size = new System.Drawing.Size(131, 22);
            this.tsPlanToImproved.Text = "&Improved";
            this.tsPlanToImproved.Click += new System.EventHandler(this.tsPlanToImproved_Click);
            // 
            // tsPlanToElite
            // 
            this.tsPlanToElite.Name = "tsPlanToElite";
            this.tsPlanToElite.Size = new System.Drawing.Size(131, 22);
            this.tsPlanToElite.Text = "&Elite";
            this.tsPlanToElite.Click += new System.EventHandler(this.tsPlanToElite_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EVEMon.Properties.Resources.Certificate_64;
            this.pictureBox1.Location = new System.Drawing.Point(186, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(3, 3);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(71, 13);
            this.lblCategory.TabIndex = 26;
            this.lblCategory.Text = "Skill Category";
            // 
            // textboxDescription
            // 
            this.textboxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxDescription.BackColor = System.Drawing.SystemColors.Window;
            this.textboxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textboxDescription.Location = new System.Drawing.Point(256, 3);
            this.textboxDescription.Multiline = true;
            this.textboxDescription.Name = "textboxDescription";
            this.textboxDescription.ReadOnly = true;
            this.textboxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textboxDescription.Size = new System.Drawing.Size(329, 85);
            this.textboxDescription.TabIndex = 25;
            // 
            // lblLevel4Time
            // 
            this.lblLevel4Time.AutoSize = true;
            this.lblLevel4Time.Location = new System.Drawing.Point(3, 71);
            this.lblLevel4Time.Name = "lblLevel4Time";
            this.lblLevel4Time.Size = new System.Drawing.Size(115, 13);
            this.lblLevel4Time.TabIndex = 23;
            this.lblLevel4Time.Text = "Elite : 11d, 6h, 33m, 3s";
            // 
            // lblLevel3Time
            // 
            this.lblLevel3Time.AutoSize = true;
            this.lblLevel3Time.Location = new System.Drawing.Point(3, 58);
            this.lblLevel3Time.Name = "lblLevel3Time";
            this.lblLevel3Time.Size = new System.Drawing.Size(145, 13);
            this.lblLevel3Time.TabIndex = 22;
            this.lblLevel3Time.Text = "Improved : 1d, 23h, 49m, 36s";
            // 
            // lblLevel2Time
            // 
            this.lblLevel2Time.AutoSize = true;
            this.lblLevel2Time.Location = new System.Drawing.Point(3, 45);
            this.lblLevel2Time.Name = "lblLevel2Time";
            this.lblLevel2Time.Size = new System.Drawing.Size(120, 13);
            this.lblLevel2Time.TabIndex = 21;
            this.lblLevel2Time.Text = "Standard : 8h, 27m, 17s";
            // 
            // lblLevel1Time
            // 
            this.lblLevel1Time.AutoSize = true;
            this.lblLevel1Time.Location = new System.Drawing.Point(3, 32);
            this.lblLevel1Time.Name = "lblLevel1Time";
            this.lblLevel1Time.Size = new System.Drawing.Size(103, 13);
            this.lblLevel1Time.TabIndex = 20;
            this.lblLevel1Time.Text = "Basic : 1h, 48m, 55s";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(65, 13);
            this.lblName.TabIndex = 19;
            this.lblName.Text = "Skill Name";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.certSelectCtl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelRight);
            this.splitContainer1.Size = new System.Drawing.Size(824, 550);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 3;
            // 
            // certSelectCtl
            // 
            this.certSelectCtl.Character = null;
            this.certSelectCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.certSelectCtl.Location = new System.Drawing.Point(0, 0);
            this.certSelectCtl.Name = "certSelectCtl";
            this.certSelectCtl.Plan = null;
            this.certSelectCtl.Size = new System.Drawing.Size(232, 550);
            this.certSelectCtl.TabIndex = 0;
            // 
            // certDisplayCtl
            // 
            this.certDisplayCtl.CertificateClass = null;
            this.certDisplayCtl.Character = null;
            this.certDisplayCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.certDisplayCtl.Location = new System.Drawing.Point(0, 116);
            this.certDisplayCtl.Name = "certDisplayCtl";
            this.certDisplayCtl.Plan = null;
            this.certDisplayCtl.Size = new System.Drawing.Size(588, 434);
            this.certDisplayCtl.TabIndex = 2;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.certDisplayCtl);
            this.panelRight.Controls.Add(this.panelHeader);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(588, 550);
            this.panelRight.TabIndex = 3;
            this.panelRight.Visible = false;
            // 
            // CertificateBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CertificateBrowserControl";
            this.Size = new System.Drawing.Size(824, 550);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CertificateSelectControl certSelectCtl;
        private System.Windows.Forms.Panel panelHeader;
        private CertificateTreeDisplayControl certDisplayCtl;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox textboxDescription;
        private System.Windows.Forms.Label lblLevel4Time;
        private System.Windows.Forms.Label lblLevel3Time;
        private System.Windows.Forms.Label lblLevel2Time;
        private System.Windows.Forms.Label lblLevel1Time;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel tslbTextForEligibility;
        private System.Windows.Forms.ToolStripLabel tslbEligible;
        private System.Windows.Forms.ToolStripDropDownButton tsPlanToMenu;
        private System.Windows.Forms.ToolStripMenuItem tsPlanToStandard;
        private System.Windows.Forms.ToolStripMenuItem tsPlanToImproved;
        private System.Windows.Forms.ToolStripMenuItem tsPlanToElite;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem tsPlanToBasic;
        private System.Windows.Forms.Panel panelRight;
    }
}
