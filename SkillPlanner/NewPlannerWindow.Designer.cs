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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsddbPlans = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsbSaveAs = new System.Windows.Forms.ToolStripButton();
            this.tsbDeletePlan = new System.Windows.Forms.ToolStripButton();
            this.tsbPrintPlan = new System.Windows.Forms.ToolStripButton();
            this.tsbCopyForum = new System.Windows.Forms.ToolStripButton();
            this.tsbExportToXml = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbImplantCalculator = new System.Windows.Forms.ToolStripButton();
            this.ttToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpPlanQueue = new System.Windows.Forms.TabPage();
            this.planEditor = new EVEMon.SkillPlanner.PlanOrderEditorControl();
            this.tpSkillBrowser = new System.Windows.Forms.TabPage();
            this.skillBrowser = new EVEMon.SkillPlanner.SkillBrowser();
            this.tpShipBrowser = new System.Windows.Forms.TabPage();
            this.shipBrowser = new EVEMon.SkillPlanner.ShipBrowserControl();
            this.tpItemBrowser = new System.Windows.Forms.TabPage();
            this.itemBrowser = new EVEMon.SkillPlanner.ItemBrowserControl();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tpPlanQueue.SuspendLayout();
            this.tpSkillBrowser.SuspendLayout();
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
            this.tslSuggestion.Size = new System.Drawing.Size(86, 17);
            this.tslSuggestion.Text = "Suggestion...";
            this.tslSuggestion.Visible = false;
            this.tslSuggestion.Click += new System.EventHandler(this.tslSuggestion_Click);
            // 
            // slblStatusText
            // 
            this.slblStatusText.Name = "slblStatusText";
            this.slblStatusText.Size = new System.Drawing.Size(81, 17);
            this.slblStatusText.Text = "0 Skills Planned";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbPlans,
            this.tsbSaveAs,
            this.tsbDeletePlan,
            this.tsbPrintPlan,
            this.tsbCopyForum,
            this.tsbExportToXml,
            this.toolStripSeparator2,
            this.tsbImplantCalculator});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(718, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsddbPlans
            // 
            this.tsddbPlans.Image = ((System.Drawing.Image)(resources.GetObject("tsddbPlans.Image")));
            this.tsddbPlans.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsddbPlans.Name = "tsddbPlans";
            this.tsddbPlans.Size = new System.Drawing.Size(89, 22);
            this.tsddbPlans.Text = "Select Plan";
            this.tsddbPlans.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsddbPlans_MouseDown);
            this.tsddbPlans.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsddbPlans_DropDownItemClicked);
            // 
            // tsbSaveAs
            // 
            this.tsbSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("tsbSaveAs.Image")));
            this.tsbSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveAs.Name = "tsbSaveAs";
            this.tsbSaveAs.Size = new System.Drawing.Size(61, 22);
            this.tsbSaveAs.Text = "Save...";
            this.tsbSaveAs.Click += new System.EventHandler(this.tsbSaveAs_Click);
            // 
            // tsbDeletePlan
            // 
            this.tsbDeletePlan.Image = ((System.Drawing.Image)(resources.GetObject("tsbDeletePlan.Image")));
            this.tsbDeletePlan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeletePlan.Name = "tsbDeletePlan";
            this.tsbDeletePlan.Size = new System.Drawing.Size(66, 22);
            this.tsbDeletePlan.Text = "Delete...";
            this.tsbDeletePlan.Click += new System.EventHandler(this.tsbDeletePlan_Click);
            // 
            // tsbPrintPlan
            // 
            this.tsbPrintPlan.Image = ((System.Drawing.Image)(resources.GetObject("tsbPrintPlan.Image")));
            this.tsbPrintPlan.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbPrintPlan.Name = "tsbPrintPlan";
            this.tsbPrintPlan.Size = new System.Drawing.Size(57, 22);
            this.tsbPrintPlan.Text = "Print...";
            this.tsbPrintPlan.ToolTipText = "Print this plan.";
            this.tsbPrintPlan.Click += new System.EventHandler(this.tsbPrintPlan_Click);
            // 
            // tsbCopyForum
            // 
            this.tsbCopyForum.Image = ((System.Drawing.Image)(resources.GetObject("tsbCopyForum.Image")));
            this.tsbCopyForum.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopyForum.Name = "tsbCopyForum";
            this.tsbCopyForum.Size = new System.Drawing.Size(112, 22);
            this.tsbCopyForum.Text = "Copy to Clipboard";
            this.tsbCopyForum.Click += new System.EventHandler(this.tsbCopyForum_Click);
            // 
            // tsbExportToXml
            // 
            this.tsbExportToXml.Image = ((System.Drawing.Image)(resources.GetObject("tsbExportToXml.Image")));
            this.tsbExportToXml.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExportToXml.Name = "tsbExportToXml";
            this.tsbExportToXml.Size = new System.Drawing.Size(91, 22);
            this.tsbExportToXml.Text = "Export XML...";
            this.tsbExportToXml.ToolTipText = "Export planned character to XML...";
            this.tsbExportToXml.Click += new System.EventHandler(this.tsbExportToXml_Click);
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
            this.tsbImplantCalculator.Size = new System.Drawing.Size(93, 22);
            this.tsbImplantCalculator.Text = "Implant Calc...";
            this.tsbImplantCalculator.Click += new System.EventHandler(this.tsbImplantCalculator_Click);
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
            this.tabControl.Controls.Add(this.tpShipBrowser);
            this.tabControl.Controls.Add(this.tpItemBrowser);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
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
            this.tpPlanQueue.Location = new System.Drawing.Point(4, 22);
            this.tpPlanQueue.Name = "tpPlanQueue";
            this.tpPlanQueue.Padding = new System.Windows.Forms.Padding(3);
            this.tpPlanQueue.Size = new System.Drawing.Size(710, 545);
            this.tpPlanQueue.TabIndex = 1;
            this.tpPlanQueue.Text = "Plan Queue";
            this.tpPlanQueue.UseVisualStyleBackColor = true;
            // 
            // planEditor
            // 
            this.planEditor.DimUntrainable = false;
            this.planEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.planEditor.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.planEditor.HighlightConflicts = false;
            this.planEditor.HighlightPlannedSkills = false;
            this.planEditor.HighlightPrerequisites = false;
            this.planEditor.Location = new System.Drawing.Point(3, 3);
            this.planEditor.Name = "planEditor";
            this.planEditor.Plan = null;
            this.planEditor.Size = new System.Drawing.Size(704, 539);
            this.planEditor.TabIndex = 2;
            this.planEditor.WorksafeMode = false;
            this.planEditor.TabIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tpSkillBrowser
            // 
            this.tpSkillBrowser.Controls.Add(this.skillBrowser);
            this.tpSkillBrowser.Location = new System.Drawing.Point(4, 22);
            this.tpSkillBrowser.Name = "tpSkillBrowser";
            this.tpSkillBrowser.Size = new System.Drawing.Size(710, 545);
            this.tpSkillBrowser.TabIndex = 0;
            this.tpSkillBrowser.Text = "Skill Browser";
            this.tpSkillBrowser.UseVisualStyleBackColor = true;
            // 
            // skillBrowser
            // 
            this.skillBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillBrowser.Location = new System.Drawing.Point(0, 0);
            this.skillBrowser.Name = "skillBrowser";
            this.skillBrowser.Plan = null;
            this.skillBrowser.SelectedSkill = null;
            this.skillBrowser.Size = new System.Drawing.Size(710, 545);
            this.skillBrowser.TabIndex = 0;
            // 
            // tpShipBrowser
            // 
            this.tpShipBrowser.Controls.Add(this.shipBrowser);
            this.tpShipBrowser.Location = new System.Drawing.Point(4, 22);
            this.tpShipBrowser.Name = "tpShipBrowser";
            this.tpShipBrowser.Size = new System.Drawing.Size(710, 545);
            this.tpShipBrowser.TabIndex = 2;
            this.tpShipBrowser.Text = "Ship Browser";
            this.tpShipBrowser.UseVisualStyleBackColor = true;
            // 
            // shipBrowser
            // 
            this.shipBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shipBrowser.Location = new System.Drawing.Point(0, 0);
            this.shipBrowser.Name = "shipBrowser";
            this.shipBrowser.Plan = null;
            this.shipBrowser.SelectedObject = null;
            this.shipBrowser.Size = new System.Drawing.Size(710, 545);
            this.shipBrowser.TabIndex = 0;
            // 
            // tpItemBrowser
            // 
            this.tpItemBrowser.Controls.Add(this.itemBrowser);
            this.tpItemBrowser.Location = new System.Drawing.Point(4, 22);
            this.tpItemBrowser.Name = "tpItemBrowser";
            this.tpItemBrowser.Size = new System.Drawing.Size(710, 545);
            this.tpItemBrowser.TabIndex = 3;
            this.tpItemBrowser.Text = "Item Browser";
            this.tpItemBrowser.UseVisualStyleBackColor = true;
            // 
            // itemBrowser
            // 
            this.itemBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemBrowser.Location = new System.Drawing.Point(0, 0);
            this.itemBrowser.Name = "itemBrowser";
            this.itemBrowser.Plan = null;
            this.itemBrowser.SelectedObject = null;
            this.itemBrowser.Size = new System.Drawing.Size(710, 545);
            this.itemBrowser.TabIndex = 0;
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
            // NewPlannerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 618);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip1);
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
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tpPlanQueue.ResumeLayout(false);
            this.tpSkillBrowser.ResumeLayout(false);
            this.tpShipBrowser.ResumeLayout(false);
            this.tpItemBrowser.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel slblStatusText;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbDeletePlan;
        private System.Windows.Forms.ToolStripStatusLabel tslSuggestion;
        private System.Windows.Forms.ToolTip ttToolTip;
        private System.Windows.Forms.ToolStripButton tsbSaveAs;
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
        private System.Windows.Forms.ToolStripButton tsbExportToXml;
        private System.Windows.Forms.ToolStripButton tsbPrintPlan;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
    }
}
