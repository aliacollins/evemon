namespace EVEMon.SkillPlanner
{
    partial class PlanWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlanWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslSuggestion = new System.Windows.Forms.ToolStripStatusLabel();
            this.slblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.upperToolStrip = new System.Windows.Forms.ToolStrip();
            this.tsddbPlans = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsddbSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiAfterPlanCharacter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbDeletePlan = new System.Windows.Forms.ToolStripButton();
            this.tsbPrintPlan = new System.Windows.Forms.ToolStripButton();
            this.tsbCopyForum = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbImplantCalculator = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAttributesOptimization = new System.Windows.Forms.ToolStripButton();
            this.tsbEFTImport = new System.Windows.Forms.ToolStripButton();
            this.ttToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpPlanQueue = new System.Windows.Forms.TabPage();
            this.planEditor = new EVEMon.SkillPlanner.PlanEditorControl();
            this.tpSkillBrowser = new System.Windows.Forms.TabPage();
            this.skillBrowser = new EVEMon.SkillPlanner.SkillBrowser();
            this.tpCertificateBrowser = new System.Windows.Forms.TabPage();
            this.certBrowser = new EVEMon.SkillPlanner.CertificateBrowserControl();
            this.tpShipBrowser = new System.Windows.Forms.TabPage();
            this.shipBrowser = new EVEMon.SkillPlanner.ShipBrowserControl();
            this.tpItemBrowser = new System.Windows.Forms.TabPage();
            this.itemBrowser = new EVEMon.SkillPlanner.ItemBrowserControl();
            this.ilTabIcons = new System.Windows.Forms.ImageList(this.components);
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
            this.statusStrip1.Size = new System.Drawing.Size(944, 22);
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
            this.tslSuggestion.Size = new System.Drawing.Size(91, 17);
            this.tslSuggestion.Text = "Suggestion...";
            this.tslSuggestion.Visible = false;
            this.tslSuggestion.Click += new System.EventHandler(this.tslSuggestion_Click);
            // 
            // slblStatusText
            // 
            this.slblStatusText.Name = "slblStatusText";
            this.slblStatusText.Size = new System.Drawing.Size(88, 17);
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
            this.upperToolStrip.Size = new System.Drawing.Size(944, 25);
            this.upperToolStrip.TabIndex = 3;
            this.upperToolStrip.Text = "toolStrip1";
            // 
            // tsddbPlans
            // 
            this.tsddbPlans.Image = global::EVEMon.Properties.Resources.Plan;
            this.tsddbPlans.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsddbPlans.Name = "tsddbPlans";
            this.tsddbPlans.Size = new System.Drawing.Size(93, 22);
            this.tsddbPlans.Text = "Select Plan";
            this.tsddbPlans.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsddbPlans_DropDownItemClicked);
            this.tsddbPlans.DropDownOpening += new System.EventHandler(this.tsddbPlans_DropDownOpening);
            // 
            // tsddbSave
            // 
            this.tsddbSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPlan,
            this.tsmiAfterPlanCharacter});
            this.tsddbSave.Image = global::EVEMon.Properties.Resources.ExportArrow;
            this.tsddbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbSave.Name = "tsddbSave";
            this.tsddbSave.Size = new System.Drawing.Size(69, 22);
            this.tsddbSave.Text = "Export";
            // 
            // tsmiCharacter
            // 
            this.tsmiAfterPlanCharacter.Image = global::EVEMon.Properties.Resources.ExportCharacter;
            this.tsmiAfterPlanCharacter.Name = "tsmiAfterPlanCharacter";
            this.tsmiAfterPlanCharacter.Size = new System.Drawing.Size(152, 22);
            this.tsmiAfterPlanCharacter.Text = "After Plan Character";
            this.tsmiAfterPlanCharacter.Click += new System.EventHandler(this.tsmiAfterPlanCharacter_Click);
            // 
            // tsmiPlan
            // 
            this.tsmiPlan.Image = global::EVEMon.Properties.Resources.ExportPlan;
            this.tsmiPlan.Name = "tsmiPlan";
            this.tsmiPlan.Size = new System.Drawing.Size(152, 22);
            this.tsmiPlan.Text = "Plan";
            this.tsmiPlan.Click += new System.EventHandler(this.tsmiPlan_Click);
            // 
            // tsbDeletePlan
            // 
            this.tsbDeletePlan.Image = global::EVEMon.Properties.Resources.DeletePlan;
            this.tsbDeletePlan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeletePlan.Name = "tsbDeletePlan";
            this.tsbDeletePlan.Size = new System.Drawing.Size(69, 22);
            this.tsbDeletePlan.Text = "Delete...";
            this.tsbDeletePlan.Click += new System.EventHandler(this.tsbDeletePlan_Click);
            // 
            // tsbPrintPlan
            // 
            this.tsbPrintPlan.Image = global::EVEMon.Properties.Resources.Printer;
            this.tsbPrintPlan.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbPrintPlan.Name = "tsbPrintPlan";
            this.tsbPrintPlan.Size = new System.Drawing.Size(61, 22);
            this.tsbPrintPlan.Text = "Print...";
            this.tsbPrintPlan.ToolTipText = "Print this plan";
            this.tsbPrintPlan.Click += new System.EventHandler(this.tsbPrintPlan_Click);
            // 
            // tsbCopyForum
            // 
            this.tsbCopyForum.Image = global::EVEMon.Properties.Resources.Copy;
            this.tsbCopyForum.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopyForum.Name = "tsbCopyForum";
            this.tsbCopyForum.Size = new System.Drawing.Size(124, 22);
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
            this.tsbImplantCalculator.Image = global::EVEMon.Properties.Resources.ImplantCalculator;
            this.tsbImplantCalculator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbImplantCalculator.Name = "tsbImplantCalculator";
            this.tsbImplantCalculator.Size = new System.Drawing.Size(103, 22);
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
            this.tsbAttributesOptimization.Image = global::EVEMon.Properties.Resources.AttributeOptimize;
            this.tsbAttributesOptimization.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAttributesOptimization.Name = "tsbAttributesOptimization";
            this.tsbAttributesOptimization.Size = new System.Drawing.Size(128, 22);
            this.tsbAttributesOptimization.Text = "Optimize attributes";
            this.tsbAttributesOptimization.Click += new System.EventHandler(this.tsbAttributesOptimization_Click);
            // 
            // tsbEFTImport
            // 
            this.tsbEFTImport.Image = global::EVEMon.Properties.Resources.Ship;
            this.tsbEFTImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEFTImport.Name = "tsbEFTImport";
            this.tsbEFTImport.Size = new System.Drawing.Size(85, 22);
            this.tsbEFTImport.Text = "EFT import";
            this.tsbEFTImport.Click += new System.EventHandler(this.tsbEFTImport_Click);
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
            this.tabControl.Size = new System.Drawing.Size(944, 571);
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
            this.tpPlanQueue.Size = new System.Drawing.Size(936, 536);
            this.tpPlanQueue.TabIndex = 1;
            this.tpPlanQueue.Text = "Plan queue";
            this.tpPlanQueue.UseVisualStyleBackColor = true;
            // 
            // planEditor
            // 
            this.planEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.planEditor.Location = new System.Drawing.Point(0, 0);
            this.planEditor.Name = "planEditor";
            this.planEditor.Plan = null;
            this.planEditor.Size = new System.Drawing.Size(936, 536);
            this.planEditor.TabIndex = 2;
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
            this.tpSkillBrowser.Text = "Skill browser";
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
            // PlanWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 618);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.upperToolStrip);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(780, 353);
            this.Name = "PlanWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EVEMon Skill Planner";
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
        private PlanEditorControl planEditor;
        private System.Windows.Forms.TabPage tpShipBrowser;
        private ShipBrowserControl shipBrowser;
        private System.Windows.Forms.TabPage tpSkillBrowser;
        private SkillBrowser skillBrowser;
        private System.Windows.Forms.ToolStripDropDownButton tsddbPlans;
        private System.Windows.Forms.ToolStripButton tsbPrintPlan;
        private System.Windows.Forms.ToolStripDropDownButton tsddbSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiAfterPlanCharacter;
        private System.Windows.Forms.ToolStripMenuItem tsmiPlan;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbAttributesOptimization;
        private System.Windows.Forms.TabPage tpCertificateBrowser;
        private CertificateBrowserControl certBrowser;
        private System.Windows.Forms.ImageList ilTabIcons;
        private System.Windows.Forms.ToolStripButton tsbEFTImport;
    }
}
