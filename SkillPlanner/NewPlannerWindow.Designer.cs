namespace EVEMon.SkillPlanner
{
    partial class NewPlannerWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPlannerWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslSuggestion = new System.Windows.Forms.ToolStripStatusLabel();
            this.slblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.upperToolStrip = new System.Windows.Forms.ToolStrip();
            this.tsddbPlans = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsddbSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiCharacter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbDeletePlan = new System.Windows.Forms.ToolStripButton();
            this.tsbPrintPlan = new System.Windows.Forms.ToolStripButton();
            this.tsbCopyForum = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbImplantCalculator = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAttributesOptimization = new System.Windows.Forms.ToolStripButton();
            this.ttToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpPlanQueue = new System.Windows.Forms.TabPage();
            this.planEditor = new EVEMon.SkillPlanner.PlanOrderEditorControl();
            this.tpSkillBrowser = new System.Windows.Forms.TabPage();
            this.skillBrowser = new EVEMon.SkillPlanner.SkillBrowser();
            this.tpCertificateBrowser = new System.Windows.Forms.TabPage();
            this.certBrowser = new EVEMon.SkillPlanner.CertificateBrowserControl();
            this.tpShipBrowser = new System.Windows.Forms.TabPage();
            this.shipBrowser = new EVEMon.SkillPlanner.ShipBrowserControl();
            this.tpItemBrowser = new System.Windows.Forms.TabPage();
            this.itemBrowser = new EVEMon.SkillPlanner.ItemBrowserControl();
            this.ilTabIcons = new System.Windows.Forms.ImageList(this.components);
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.tsbEFTImport = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1.SuspendLayout();
            this.upperToolStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tpPlanQueue.SuspendLayout();
            this.tpSkillBrowser.SuspendLayout();
            this.tpCertificateBrowser.SuspendLayout();
            this.tpShipBrowser.SuspendLayout();
            this.tpItemBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSuggestion,
            this.slblStatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 596);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(718, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tslSuggestion
            // 
            this.tslSuggestion.Image = ((System.Drawing.Image)(resources.GetObject("tslSuggestion.Image")));
            this.tslSuggestion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tslSuggestion.IsLink = true;
            this.tslSuggestion.Margin = new System.Windows.Forms.Padding(10, 3, 0, 2);
            this.tslSuggestion.Name = "tslSuggestion";
            this.tslSuggestion.Size = new System.Drawing.Size(88, 17);
            this.tslSuggestion.Text = "Suggestion...";
            this.tslSuggestion.Visible = false;
            this.tslSuggestion.Click += new System.EventHandler(this.tslSuggestion_Click);
            // 
            // slblStatusText
            // 
            this.slblStatusText.Name = "slblStatusText";
            this.slblStatusText.Size = new System.Drawing.Size(79, 17);
            this.slblStatusText.Text = "0 Skills Planned";
            // 
            // upperToolStrip
            // 
            this.upperToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbPlans,
            this.tsddbSave,
            this.tsbDeletePlan,
            this.tsbPrintPlan,
            this.tsbCopyForum,
            this.toolStripSeparator2,
            this.tsbImplantCalculator,
            this.toolStripSeparator1,
            this.tsbAttributesOptimization,
            this.tsbEFTImport});
            this.upperToolStrip.Location = new System.Drawing.Point(0, 0);
            this.upperToolStrip.Name = "upperToolStrip";
            this.upperToolStrip.Size = new System.Drawing.Size(718, 25);
            this.upperToolStrip.TabIndex = 3;
            this.upperToolStrip.Text = "toolStrip1";
            // 
            // tsddbPlans
            // 
            this.tsddbPlans.Image = ((System.Drawing.Image)(resources.GetObject("tsddbPlans.Image")));
            this.tsddbPlans.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsddbPlans.Name = "tsddbPlans";
            this.tsddbPlans.Size = new System.Drawing.Size(88, 22);
            this.tsddbPlans.Text = "Select Plan";
            this.tsddbPlans.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsddbPlans_MouseDown);
            this.tsddbPlans.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsddbPlans_DropDownItemClicked);
            // 
            // tsddbSave
            // 
            this.tsddbSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCharacter,
            this.tsmiPlan});
            this.tsddbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsddbSave.Image")));
            this.tsddbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbSave.Name = "tsddbSave";
            this.tsddbSave.Size = new System.Drawing.Size(68, 22);
            this.tsddbSave.Text = "Export";
            // 
            // tsmiCharacter
            // 
            this.tsmiCharacter.Name = "tsmiCharacter";
            this.tsmiCharacter.Size = new System.Drawing.Size(133, 22);
            this.tsmiCharacter.Text = "Character";
            this.tsmiCharacter.Click += new System.EventHandler(this.tsmiCharacter_Click);
            // 
            // tsmiPlan
            // 
            this.tsmiPlan.Name = "tsmiPlan";
            this.tsmiPlan.Size = new System.Drawing.Size(133, 22);
            this.tsmiPlan.Text = "Plan";
            this.tsmiPlan.Click += new System.EventHandler(this.tsmiPlan_Click);
            // 
            // tsbDeletePlan
            // 
            this.tsbDeletePlan.Image = ((System.Drawing.Image)(resources.GetObject("tsbDeletePlan.Image")));
            this.tsbDeletePlan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeletePlan.Name = "tsbDeletePlan";
            this.tsbDeletePlan.Size = new System.Drawing.Size(70, 22);
            this.tsbDeletePlan.Text = "Delete...";
            this.tsbDeletePlan.Click += new System.EventHandler(this.tsbDeletePlan_Click);
            // 
            // tsbPrintPlan
            // 
            this.tsbPrintPlan.Image = ((System.Drawing.Image)(resources.GetObject("tsbPrintPlan.Image")));
            this.tsbPrintPlan.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbPrintPlan.Name = "tsbPrintPlan";
            this.tsbPrintPlan.Size = new System.Drawing.Size(61, 22);
            this.tsbPrintPlan.Text = "Print...";
            this.tsbPrintPlan.ToolTipText = "Print this plan.";
            this.tsbPrintPlan.Click += new System.EventHandler(this.tsbPrintPlan_Click);
            // 
            // tsbCopyForum
            // 
            this.tsbCopyForum.Image = ((System.Drawing.Image)(resources.GetObject("tsbCopyForum.Image")));
            this.tsbCopyForum.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopyForum.Name = "tsbCopyForum";
            this.tsbCopyForum.Size = new System.Drawing.Size(113, 22);
            this.tsbCopyForum.Text = "Copy to Clipboard";
            this.tsbCopyForum.Click += new System.EventHandler(this.tsbCopyForum_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbImplantCalculator
            // 
            this.tsbImplantCalculator.Image = ((System.Drawing.Image)(resources.GetObject("tsbImplantCalculator.Image")));
            this.tsbImplantCalculator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbImplantCalculator.Name = "tsbImplantCalculator";
            this.tsbImplantCalculator.Size = new System.Drawing.Size(98, 22);
            this.tsbImplantCalculator.Text = "Implant Calc...";
            this.tsbImplantCalculator.Click += new System.EventHandler(this.tsbImplantCalculator_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbAttributesOptimization
            // 
            this.tsbAttributesOptimization.Image = ((System.Drawing.Image)(resources.GetObject("tsbAttributesOptimization.Image")));
            this.tsbAttributesOptimization.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAttributesOptimization.Name = "tsbAttributesOptimization";
            this.tsbAttributesOptimization.Size = new System.Drawing.Size(118, 22);
            this.tsbAttributesOptimization.Text = "Optimize attributes";
            this.tsbAttributesOptimization.Click += new System.EventHandler(this.tsbAttributesOptimization_Click);
            // 
            // ttToolTip
            // 
            this.ttToolTip.AutoPopDelay = 5000000;
            this.ttToolTip.InitialDelay = 500;
            this.ttToolTip.ReshowDelay = 100;
            // 
            // sfdSave
            // 
            this.sfdSave.Filter = "EVEMon Plan Format (*.emp)|*.emp|XML  Format (*.xml)|*.xml|Text Format (*.txt)|*." +
                "txt";
            this.sfdSave.Title = "Save to File";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tpPlanQueue);
            this.tabControl.Controls.Add(this.tpSkillBrowser);
            this.tabControl.Controls.Add(this.tpCertificateBrowser);
            this.tabControl.Controls.Add(this.tpShipBrowser);
            this.tabControl.Controls.Add(this.tpItemBrowser);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ImageList = this.ilTabIcons;
            this.tabControl.Location = new System.Drawing.Point(0, 25);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(718, 571);
            this.tabControl.TabIndex = 4;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tpPlanQueue
            // 
            this.tpPlanQueue.Controls.Add(this.planEditor);
            this.tpPlanQueue.ImageIndex = 0;
            this.tpPlanQueue.Location = new System.Drawing.Point(4, 31);
            this.tpPlanQueue.Margin = new System.Windows.Forms.Padding(0);
            this.tpPlanQueue.Name = "tpPlanQueue";
            this.tpPlanQueue.Size = new System.Drawing.Size(710, 536);
            this.tpPlanQueue.TabIndex = 1;
            this.tpPlanQueue.Text = "Plan queue";
            this.tpPlanQueue.UseVisualStyleBackColor = true;
            // 
            // planEditor
            // 
            this.planEditor.DimUntrainable = false;
            this.planEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.planEditor.HighlightConflicts = false;
            this.planEditor.HighlightPlannedSkills = false;
            this.planEditor.HighlightPrerequisites = false;
            this.planEditor.Location = new System.Drawing.Point(0, 0);
            this.planEditor.Name = "planEditor";
            this.planEditor.Plan = null;
            this.planEditor.Size = new System.Drawing.Size(710, 536);
            this.planEditor.TabIndex = 2;
            this.planEditor.WorksafeMode = false;
            this.planEditor.TabIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tpSkillBrowser
            // 
            this.tpSkillBrowser.Controls.Add(this.skillBrowser);
            this.tpSkillBrowser.ImageIndex = 1;
            this.tpSkillBrowser.Location = new System.Drawing.Point(4, 31);
            this.tpSkillBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.tpSkillBrowser.Name = "tpSkillBrowser";
            this.tpSkillBrowser.Size = new System.Drawing.Size(710, 536);
            this.tpSkillBrowser.TabIndex = 0;
            this.tpSkillBrowser.Text = "Skills browser";
            this.tpSkillBrowser.UseVisualStyleBackColor = true;
            // 
            // skillBrowser
            // 
            this.skillBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillBrowser.Location = new System.Drawing.Point(0, 0);
            this.skillBrowser.Name = "skillBrowser";
            this.skillBrowser.Plan = null;
            this.skillBrowser.SelectedSkill = null;
            this.skillBrowser.Size = new System.Drawing.Size(710, 536);
            this.skillBrowser.TabIndex = 0;
            // 
            // tpCertificateBrowser
            // 
            this.tpCertificateBrowser.Controls.Add(this.certBrowser);
            this.tpCertificateBrowser.ImageIndex = 2;
            this.tpCertificateBrowser.Location = new System.Drawing.Point(4, 31);
            this.tpCertificateBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.tpCertificateBrowser.Name = "tpCertificateBrowser";
            this.tpCertificateBrowser.Size = new System.Drawing.Size(710, 536);
            this.tpCertificateBrowser.TabIndex = 4;
            this.tpCertificateBrowser.Text = "Certificates";
            this.tpCertificateBrowser.UseVisualStyleBackColor = true;
            // 
            // certBrowser
            // 
            this.certBrowser.Character = null;
            this.certBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.certBrowser.Location = new System.Drawing.Point(0, 0);
            this.certBrowser.Name = "certBrowser";
            this.certBrowser.Plan = null;
            this.certBrowser.SelectedCertificateClass = null;
            this.certBrowser.Size = new System.Drawing.Size(710, 536);
            this.certBrowser.TabIndex = 0;
            // 
            // tpShipBrowser
            // 
            this.tpShipBrowser.Controls.Add(this.shipBrowser);
            this.tpShipBrowser.ImageIndex = 3;
            this.tpShipBrowser.Location = new System.Drawing.Point(4, 31);
            this.tpShipBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.tpShipBrowser.Name = "tpShipBrowser";
            this.tpShipBrowser.Size = new System.Drawing.Size(710, 536);
            this.tpShipBrowser.TabIndex = 2;
            this.tpShipBrowser.Text = "Ships browser";
            this.tpShipBrowser.UseVisualStyleBackColor = true;
            // 
            // shipBrowser
            // 
            this.shipBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shipBrowser.Location = new System.Drawing.Point(0, 0);
            this.shipBrowser.Name = "shipBrowser";
            this.shipBrowser.Plan = null;
            this.shipBrowser.SelectedObject = null;
            this.shipBrowser.Size = new System.Drawing.Size(710, 536);
            this.shipBrowser.TabIndex = 0;
            // 
            // tpItemBrowser
            // 
            this.tpItemBrowser.Controls.Add(this.itemBrowser);
            this.tpItemBrowser.ImageIndex = 4;
            this.tpItemBrowser.Location = new System.Drawing.Point(4, 31);
            this.tpItemBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.tpItemBrowser.Name = "tpItemBrowser";
            this.tpItemBrowser.Size = new System.Drawing.Size(710, 536);
            this.tpItemBrowser.TabIndex = 3;
            this.tpItemBrowser.Text = "Items browser";
            this.tpItemBrowser.UseVisualStyleBackColor = true;
            // 
            // itemBrowser
            // 
            this.itemBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemBrowser.Location = new System.Drawing.Point(0, 0);
            this.itemBrowser.Name = "itemBrowser";
            this.itemBrowser.Plan = null;
            this.itemBrowser.SelectedObject = null;
            this.itemBrowser.Size = new System.Drawing.Size(710, 536);
            this.itemBrowser.TabIndex = 0;
            // 
            // ilTabIcons
            // 
            this.ilTabIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTabIcons.ImageStream")));
            this.ilTabIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilTabIcons.Images.SetKeyName(0, "Plan.png");
            this.ilTabIcons.Images.SetKeyName(1, "Skills.png");
            this.ilTabIcons.Images.SetKeyName(2, "Certificate-32.png");
            this.ilTabIcons.Images.SetKeyName(3, "Ships.png");
            this.ilTabIcons.Images.SetKeyName(4, "Items.png");
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            this.printDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument1_BeginPrint);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // tsbEFTImport
            // 
            this.tsbEFTImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbEFTImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEFTImport.Name = "tsbEFTImport";
            this.tsbEFTImport.Size = new System.Drawing.Size(69, 19);
            this.tsbEFTImport.Text = "EFT import";
            this.tsbEFTImport.Click += new System.EventHandler(this.tsbEFTImport_Click);
            // 
            // NewPlannerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 618);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.upperToolStrip);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(666, 353);
            this.Name = "NewPlannerWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EVEMon Skill Planner";
            this.Load += new System.EventHandler(this.NewPlannerWindow_Load);
            this.Shown += new System.EventHandler(this.NewPlannerWindow_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewPlannerWindow_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewPlannerWindow_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.upperToolStrip.ResumeLayout(false);
            this.upperToolStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tpPlanQueue.ResumeLayout(false);
            this.tpSkillBrowser.ResumeLayout(false);
            this.tpCertificateBrowser.ResumeLayout(false);
            this.tpShipBrowser.ResumeLayout(false);
            this.tpItemBrowser.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel slblStatusText;
        private System.Windows.Forms.ToolStrip upperToolStrip;
        private System.Windows.Forms.ToolStripButton tsbDeletePlan;
        private System.Windows.Forms.ToolStripStatusLabel tslSuggestion;
        private System.Windows.Forms.ToolTip ttToolTip;
        private System.Windows.Forms.ToolStripButton tsbCopyForum;
        private System.Windows.Forms.SaveFileDialog sfdSave;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbImplantCalculator;
        private System.Windows.Forms.TabPage tpItemBrowser;
        private ItemBrowserControl itemBrowser;
        private System.Windows.Forms.TabPage tpPlanQueue;
        private PlanOrderEditorControl planEditor;
        private System.Windows.Forms.TabPage tpShipBrowser;
        private ShipBrowserControl shipBrowser;
        private System.Windows.Forms.TabPage tpSkillBrowser;
        private SkillBrowser skillBrowser;
        private System.Windows.Forms.ToolStripDropDownButton tsddbPlans;
        private System.Windows.Forms.ToolStripButton tsbPrintPlan;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.ToolStripDropDownButton tsddbSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiCharacter;
        private System.Windows.Forms.ToolStripMenuItem tsmiPlan;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbAttributesOptimization;
        private System.Windows.Forms.TabPage tpCertificateBrowser;
        private CertificateBrowserControl certBrowser;
        private System.Windows.Forms.ImageList ilTabIcons;
        private System.Windows.Forms.ToolStripButton tsbEFTImport;
    }
}
