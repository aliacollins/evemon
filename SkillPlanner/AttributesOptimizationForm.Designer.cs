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
            this.lbWait = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabWait = new System.Windows.Forms.TabPage();
            this.throbber = new EVEMon.Throbber();
            this.tabNoResult = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.lvPoints = new System.Windows.Forms.ListView();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.tabControl.SuspendLayout();
            this.tabWait.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).BeginInit();
            this.tabNoResult.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbWait
            // 
            this.lbWait.AutoSize = true;
            this.lbWait.Location = new System.Drawing.Point(202, 149);
            this.lbWait.Name = "lbWait";
            this.lbWait.Size = new System.Drawing.Size(110, 13);
            this.lbWait.TabIndex = 23;
            this.lbWait.Text = "Optimizing attributes...";
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
            this.tabControl.Controls.Add(this.tabWait);
            this.tabControl.Controls.Add(this.tabNoResult);
            this.tabControl.Controls.Add(this.tabSummary);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(0, 29);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(497, 391);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 27;
            // 
            // tabWait
            // 
            this.tabWait.Controls.Add(this.throbber);
            this.tabWait.Controls.Add(this.lbWait);
            this.tabWait.Location = new System.Drawing.Point(4, 22);
            this.tabWait.Name = "tabWait";
            this.tabWait.Padding = new System.Windows.Forms.Padding(3);
            this.tabWait.Size = new System.Drawing.Size(489, 365);
            this.tabWait.TabIndex = 2;
            this.tabWait.Text = "Running...";
            this.tabWait.UseVisualStyleBackColor = true;
            // 
            // throbber
            // 
            this.throbber.Location = new System.Drawing.Point(162, 144);
            this.throbber.MaximumSize = new System.Drawing.Size(24, 24);
            this.throbber.MinimumSize = new System.Drawing.Size(24, 24);
            this.throbber.Name = "throbber";
            this.throbber.Size = new System.Drawing.Size(24, 24);
            this.throbber.State = EVEMon.Throbber.ThrobberState.Stopped;
            this.throbber.TabIndex = 22;
            this.throbber.TabStop = false;
            // 
            // tabNoResult
            // 
            this.tabNoResult.Controls.Add(this.label1);
            this.tabNoResult.Location = new System.Drawing.Point(4, 22);
            this.tabNoResult.Name = "tabNoResult";
            this.tabNoResult.Padding = new System.Windows.Forms.Padding(3);
            this.tabNoResult.Size = new System.Drawing.Size(489, 365);
            this.tabNoResult.TabIndex = 0;
            this.tabNoResult.Text = "Result";
            this.tabNoResult.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(483, 359);
            this.label1.TabIndex = 0;
            this.label1.Text = "You have not defined any remapping point.\r\nUse the \"toggle remapping\" button on t" +
                "he left sidebar of your plan\'s window\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.lvPoints);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(489, 365);
            this.tabSummary.TabIndex = 1;
            this.tabSummary.Text = "Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // lvPoints
            // 
            this.lvPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
            this.lvPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPoints.LabelWrap = false;
            this.lvPoints.Location = new System.Drawing.Point(3, 3);
            this.lvPoints.Name = "lvPoints";
            this.lvPoints.Size = new System.Drawing.Size(483, 359);
            this.lvPoints.TabIndex = 0;
            this.lvPoints.UseCompatibleStateImageBehavior = false;
            this.lvPoints.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "";
            this.columnHeader.Width = 474;
            // 
            // AttributesOptimizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(497, 420);
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
            this.tabWait.ResumeLayout(false);
            this.tabWait.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).EndInit();
            this.tabNoResult.ResumeLayout(false);
            this.tabSummary.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Throbber throbber;
        private System.Windows.Forms.Label lbWait;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabNoResult;
        private System.Windows.Forms.TabPage tabSummary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvPoints;
        private System.Windows.Forms.TabPage tabWait;
        private System.Windows.Forms.ColumnHeader columnHeader;

    }
}
