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
            this.lbCurrentTimeInfo = new System.Windows.Forms.Label();
            this.lbOptimizedTimeInfo = new System.Windows.Forms.Label();
            this.lbGain = new System.Windows.Forms.Label();
            this.lbCurrentTime = new System.Windows.Forms.Label();
            this.lbOptimizedTime = new System.Windows.Forms.Label();
            this.lbWait = new System.Windows.Forms.Label();
            this.tblayoutComparePanel = new System.Windows.Forms.TableLayoutPanel();
            this.lbReminder = new System.Windows.Forms.Label();
            this.tblayoutSummary = new System.Windows.Forms.TableLayoutPanel();
            this.lbWarning = new System.Windows.Forms.Label();
            this.throbber = new EVEMon.Throbber();
            this.attributesOptimizationControl = new EVEMon.SkillPlanner.AttributesOptimizationControl();
            this.tblayoutComparePanel.SuspendLayout();
            this.tblayoutSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).BeginInit();
            this.SuspendLayout();
            // 
            // lbCurrentTimeInfo
            // 
            this.lbCurrentTimeInfo.AutoSize = true;
            this.lbCurrentTimeInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentTimeInfo.Location = new System.Drawing.Point(6, 6);
            this.lbCurrentTimeInfo.Margin = new System.Windows.Forms.Padding(6);
            this.lbCurrentTimeInfo.Name = "lbCurrentTimeInfo";
            this.lbCurrentTimeInfo.Size = new System.Drawing.Size(83, 13);
            this.lbCurrentTimeInfo.TabIndex = 15;
            this.lbCurrentTimeInfo.Text = "Current time :";
            // 
            // lbOptimizedTimeInfo
            // 
            this.lbOptimizedTimeInfo.AutoSize = true;
            this.lbOptimizedTimeInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOptimizedTimeInfo.Location = new System.Drawing.Point(6, 31);
            this.lbOptimizedTimeInfo.Margin = new System.Windows.Forms.Padding(6);
            this.lbOptimizedTimeInfo.Name = "lbOptimizedTimeInfo";
            this.lbOptimizedTimeInfo.Size = new System.Drawing.Size(97, 13);
            this.lbOptimizedTimeInfo.TabIndex = 16;
            this.lbOptimizedTimeInfo.Text = "Optimized time :";
            // 
            // lbGain
            // 
            this.lbGain.AutoSize = true;
            this.tblayoutComparePanel.SetColumnSpan(this.lbGain, 2);
            this.lbGain.Location = new System.Drawing.Point(6, 56);
            this.lbGain.Margin = new System.Windows.Forms.Padding(6);
            this.lbGain.Name = "lbGain";
            this.lbGain.Size = new System.Drawing.Size(156, 13);
            this.lbGain.TabIndex = 17;
            this.lbGain.Text = "Your skills are already optimized";
            // 
            // lbCurrentTime
            // 
            this.lbCurrentTime.AutoSize = true;
            this.lbCurrentTime.Location = new System.Drawing.Point(115, 6);
            this.lbCurrentTime.Margin = new System.Windows.Forms.Padding(6);
            this.lbCurrentTime.Name = "lbCurrentTime";
            this.lbCurrentTime.Size = new System.Drawing.Size(50, 13);
            this.lbCurrentTime.TabIndex = 19;
            this.lbCurrentTime.Text = "0h 0m 0s";
            // 
            // lbOptimizedTime
            // 
            this.lbOptimizedTime.AutoSize = true;
            this.lbOptimizedTime.Location = new System.Drawing.Point(115, 31);
            this.lbOptimizedTime.Margin = new System.Windows.Forms.Padding(6);
            this.lbOptimizedTime.Name = "lbOptimizedTime";
            this.lbOptimizedTime.Size = new System.Drawing.Size(13, 13);
            this.lbOptimizedTime.TabIndex = 20;
            this.lbOptimizedTime.Text = "?";
            // 
            // lbWait
            // 
            this.lbWait.AutoSize = true;
            this.lbWait.Location = new System.Drawing.Point(185, 114);
            this.lbWait.Name = "lbWait";
            this.lbWait.Size = new System.Drawing.Size(110, 13);
            this.lbWait.TabIndex = 23;
            this.lbWait.Text = "Optimizing attributes...";
            // 
            // tblayoutComparePanel
            // 
            this.tblayoutComparePanel.AutoSize = true;
            this.tblayoutComparePanel.ColumnCount = 2;
            this.tblayoutComparePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblayoutComparePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblayoutComparePanel.Controls.Add(this.lbGain, 0, 2);
            this.tblayoutComparePanel.Controls.Add(this.lbOptimizedTimeInfo, 0, 1);
            this.tblayoutComparePanel.Controls.Add(this.lbCurrentTimeInfo, 0, 0);
            this.tblayoutComparePanel.Controls.Add(this.lbCurrentTime, 1, 0);
            this.tblayoutComparePanel.Controls.Add(this.lbOptimizedTime, 1, 1);
            this.tblayoutComparePanel.Location = new System.Drawing.Point(3, 3);
            this.tblayoutComparePanel.Name = "tblayoutComparePanel";
            this.tblayoutComparePanel.RowCount = 3;
            this.tblayoutSummary.SetRowSpan(this.tblayoutComparePanel, 2);
            this.tblayoutComparePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblayoutComparePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblayoutComparePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblayoutComparePanel.Size = new System.Drawing.Size(171, 75);
            this.tblayoutComparePanel.TabIndex = 0;
            // 
            // lbReminder
            // 
            this.lbReminder.Location = new System.Drawing.Point(236, 4);
            this.lbReminder.Margin = new System.Windows.Forms.Padding(4);
            this.lbReminder.Name = "lbReminder";
            this.lbReminder.Size = new System.Drawing.Size(225, 29);
            this.lbReminder.TabIndex = 0;
            this.lbReminder.Text = "Remember you can only remap your attributes once every 12 months.";
            // 
            // tblayoutSummary
            // 
            this.tblayoutSummary.ColumnCount = 2;
            this.tblayoutSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblayoutSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblayoutSummary.Controls.Add(this.lbWarning, 1, 1);
            this.tblayoutSummary.Controls.Add(this.lbReminder, 1, 0);
            this.tblayoutSummary.Controls.Add(this.tblayoutComparePanel, 0, 0);
            this.tblayoutSummary.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tblayoutSummary.Location = new System.Drawing.Point(0, 224);
            this.tblayoutSummary.Name = "tblayoutSummary";
            this.tblayoutSummary.RowCount = 2;
            this.tblayoutSummary.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblayoutSummary.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblayoutSummary.Size = new System.Drawing.Size(465, 86);
            this.tblayoutSummary.TabIndex = 25;
            // 
            // lbWarning
            // 
            this.lbWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWarning.ForeColor = System.Drawing.Color.Red;
            this.lbWarning.Location = new System.Drawing.Point(236, 41);
            this.lbWarning.Margin = new System.Windows.Forms.Padding(4);
            this.lbWarning.Name = "lbWarning";
            this.lbWarning.Size = new System.Drawing.Size(225, 41);
            this.lbWarning.TabIndex = 1;
            this.lbWarning.Text = "Your current plan does not contain sufficent skills to last the entire 12 month p" +
                "eriod.";
            this.lbWarning.Visible = false;
            // 
            // throbber
            // 
            this.throbber.Location = new System.Drawing.Point(155, 109);
            this.throbber.MaximumSize = new System.Drawing.Size(24, 24);
            this.throbber.MinimumSize = new System.Drawing.Size(24, 24);
            this.throbber.Name = "throbber";
            this.throbber.Size = new System.Drawing.Size(24, 24);
            this.throbber.State = EVEMon.Throbber.ThrobberState.Stopped;
            this.throbber.TabIndex = 22;
            this.throbber.TabStop = false;
            // 
            // attributesOptimizationControl
            // 
            this.attributesOptimizationControl.Location = new System.Drawing.Point(13, 13);
            this.attributesOptimizationControl.Name = "attributesOptimizationControl";
            this.attributesOptimizationControl.Size = new System.Drawing.Size(442, 205);
            this.attributesOptimizationControl.TabIndex = 21;
            // 
            // AttributesOptimizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 310);
            this.Controls.Add(this.tblayoutSummary);
            this.Controls.Add(this.lbWait);
            this.Controls.Add(this.throbber);
            this.Controls.Add(this.attributesOptimizationControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AttributesOptimizationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attributes optimization";
            this.Load += new System.EventHandler(this.AttributesOptimizationForm_Load);
            this.tblayoutComparePanel.ResumeLayout(false);
            this.tblayoutComparePanel.PerformLayout();
            this.tblayoutSummary.ResumeLayout(false);
            this.tblayoutSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbGain;
        private System.Windows.Forms.Label lbCurrentTime;
        private System.Windows.Forms.Label lbOptimizedTime;
        private AttributesOptimizationControl attributesOptimizationControl;
        private Throbber throbber;
        private System.Windows.Forms.Label lbWait;
        private System.Windows.Forms.Label lbCurrentTimeInfo;
        private System.Windows.Forms.Label lbOptimizedTimeInfo;
        private System.Windows.Forms.TableLayoutPanel tblayoutComparePanel;
        private System.Windows.Forms.Label lbReminder;
        private System.Windows.Forms.TableLayoutPanel tblayoutSummary;
        private System.Windows.Forms.Label lbWarning;

    }
}
