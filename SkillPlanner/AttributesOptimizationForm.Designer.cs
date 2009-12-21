namespace EVEMon.SkillPlanner
{
    partial class AttributesOptimizationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributesOptimizationForm));
            this.labelDescription = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.lvPoints = new System.Windows.Forms.ListView();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.panelWait = new System.Windows.Forms.Panel();
            this.throbber = new EVEMon.Throbber();
            this.lbWait = new System.Windows.Forms.Label();
            this.panelNoResult = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.panelWait.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).BeginInit();
            this.panelNoResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelDescription.Location = new System.Drawing.Point(0, 0);
            this.labelDescription.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Padding = new System.Windows.Forms.Padding(8);
            this.labelDescription.Size = new System.Drawing.Size(98, 29);
            this.labelDescription.TabIndex = 26;
            this.labelDescription.Text = "labelDescription";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSummary);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(0, 29);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(518, 404);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 27;
            this.tabControl.Visible = false;
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.lvPoints);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(510, 378);
            this.tabSummary.TabIndex = 1;
            this.tabSummary.Text = "Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // lvPoints
            // 
            this.lvPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
            this.lvPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPoints.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lvPoints.LabelWrap = false;
            this.lvPoints.Location = new System.Drawing.Point(3, 3);
            this.lvPoints.Name = "lvPoints";
            this.lvPoints.Size = new System.Drawing.Size(504, 372);
            this.lvPoints.TabIndex = 0;
            this.lvPoints.UseCompatibleStateImageBehavior = false;
            this.lvPoints.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "";
            this.columnHeader.Width = 474;
            // 
            // panelWait
            // 
            this.panelWait.Controls.Add(this.throbber);
            this.panelWait.Controls.Add(this.lbWait);
            this.panelWait.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelWait.Location = new System.Drawing.Point(0, 29);
            this.panelWait.Name = "panelWait";
            this.panelWait.Size = new System.Drawing.Size(518, 404);
            this.panelWait.TabIndex = 28;
            // 
            // throbber
            // 
            this.throbber.Location = new System.Drawing.Point(184, 169);
            this.throbber.MaximumSize = new System.Drawing.Size(24, 24);
            this.throbber.MinimumSize = new System.Drawing.Size(24, 24);
            this.throbber.Name = "throbber";
            this.throbber.Size = new System.Drawing.Size(24, 24);
            this.throbber.State = EVEMon.Throbber.ThrobberState.Stopped;
            this.throbber.TabIndex = 24;
            this.throbber.TabStop = false;
            // 
            // lbWait
            // 
            this.lbWait.AutoSize = true;
            this.lbWait.Location = new System.Drawing.Point(224, 175);
            this.lbWait.Name = "lbWait";
            this.lbWait.Size = new System.Drawing.Size(110, 13);
            this.lbWait.TabIndex = 25;
            this.lbWait.Text = "Optimizing attributes...";
            // 
            // panelNoResult
            // 
            this.panelNoResult.Controls.Add(this.label2);
            this.panelNoResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNoResult.Location = new System.Drawing.Point(0, 29);
            this.panelNoResult.Name = "panelNoResult";
            this.panelNoResult.Size = new System.Drawing.Size(518, 404);
            this.panelNoResult.TabIndex = 29;
            this.panelNoResult.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(362, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "You have not defined any remapping point.\r\nUse the \"toggle remapping\" button on t" +
                "he left sidebar of your plan\'s window";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AttributesOptimizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(518, 433);
            this.Controls.Add(this.panelNoResult);
            this.Controls.Add(this.panelWait);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.labelDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttributesOptimizationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attributes optimization";
            this.Load += new System.EventHandler(this.AttributesOptimizationForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabSummary.ResumeLayout(false);
            this.panelWait.ResumeLayout(false);
            this.panelWait.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).EndInit();
            this.panelNoResult.ResumeLayout(false);
            this.panelNoResult.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabSummary;
		private System.Windows.Forms.ListView lvPoints;
        private System.Windows.Forms.ColumnHeader columnHeader;
		private System.Windows.Forms.Panel panelWait;
		private Throbber throbber;
		private System.Windows.Forms.Label lbWait;
		private System.Windows.Forms.Panel panelNoResult;
		private System.Windows.Forms.Label label2;

    }
}
