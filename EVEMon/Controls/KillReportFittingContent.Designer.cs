namespace EVEMon.Controls
{
    partial class KillReportFittingContent
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.SaveFittingButton = new System.Windows.Forms.Button();
            this.FittingContentLabel = new System.Windows.Forms.Label();
            this.FooterPanel = new System.Windows.Forms.Panel();
            this.CostLabel = new System.Windows.Forms.Label();
            this.EstimatedTotalLossLabel = new System.Windows.Forms.Label();
            this.FittingContentListBox = new EVEMon.Common.Controls.NoFlickerListBox();
            this.tableLayoutPanel.SuspendLayout();
            this.HeaderPanel.SuspendLayout();
            this.FooterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.HeaderPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.FooterPanel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.FittingContentListBox, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(264, 142);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.AutoSize = true;
            this.HeaderPanel.Controls.Add(this.SaveFittingButton);
            this.HeaderPanel.Controls.Add(this.FittingContentLabel);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(264, 23);
            this.HeaderPanel.TabIndex = 1;
            // 
            // SaveFittingButton
            // 
            this.SaveFittingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveFittingButton.Location = new System.Drawing.Point(186, 0);
            this.SaveFittingButton.Margin = new System.Windows.Forms.Padding(0);
            this.SaveFittingButton.Name = "SaveFittingButton";
            this.SaveFittingButton.Size = new System.Drawing.Size(75, 23);
            this.SaveFittingButton.TabIndex = 1;
            this.SaveFittingButton.Text = "Save Fitting";
            this.SaveFittingButton.UseVisualStyleBackColor = true;
            // 
            // FittingContentLabel
            // 
            this.FittingContentLabel.AutoSize = true;
            this.FittingContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.FittingContentLabel.Location = new System.Drawing.Point(3, 3);
            this.FittingContentLabel.Name = "FittingContentLabel";
            this.FittingContentLabel.Size = new System.Drawing.Size(117, 16);
            this.FittingContentLabel.TabIndex = 0;
            this.FittingContentLabel.Text = "Fitting and Content";
            // 
            // FooterPanel
            // 
            this.FooterPanel.AutoSize = true;
            this.FooterPanel.Controls.Add(this.CostLabel);
            this.FooterPanel.Controls.Add(this.EstimatedTotalLossLabel);
            this.FooterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FooterPanel.Location = new System.Drawing.Point(0, 126);
            this.FooterPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FooterPanel.Name = "FooterPanel";
            this.FooterPanel.Size = new System.Drawing.Size(264, 16);
            this.FooterPanel.TabIndex = 3;
            // 
            // CostLabel
            // 
            this.CostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CostLabel.AutoSize = true;
            this.CostLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.CostLabel.ForeColor = System.Drawing.Color.Red;
            this.CostLabel.Location = new System.Drawing.Point(198, 0);
            this.CostLabel.Name = "CostLabel";
            this.CostLabel.Size = new System.Drawing.Size(63, 16);
            this.CostLabel.TabIndex = 0;
            this.CostLabel.Text = "Unknown";
            // 
            // EstimatedTotalLossLabel
            // 
            this.EstimatedTotalLossLabel.AutoSize = true;
            this.EstimatedTotalLossLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.EstimatedTotalLossLabel.Location = new System.Drawing.Point(3, 0);
            this.EstimatedTotalLossLabel.Name = "EstimatedTotalLossLabel";
            this.EstimatedTotalLossLabel.Size = new System.Drawing.Size(99, 16);
            this.EstimatedTotalLossLabel.TabIndex = 2;
            this.EstimatedTotalLossLabel.Text = "Est. Total Loss:";
            // 
            // FittingContentListBox
            // 
            this.FittingContentListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FittingContentListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FittingContentListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.FittingContentListBox.FormattingEnabled = true;
            this.FittingContentListBox.Location = new System.Drawing.Point(3, 26);
            this.FittingContentListBox.Name = "FittingContentListBox";
            this.FittingContentListBox.Size = new System.Drawing.Size(258, 97);
            this.FittingContentListBox.TabIndex = 4;
            // 
            // KillReportFittingContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "KillReportFittingContent";
            this.Size = new System.Drawing.Size(264, 142);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.HeaderPanel.ResumeLayout(false);
            this.HeaderPanel.PerformLayout();
            this.FooterPanel.ResumeLayout(false);
            this.FooterPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel HeaderPanel;
        private System.Windows.Forms.Button SaveFittingButton;
        private System.Windows.Forms.Label FittingContentLabel;
        private System.Windows.Forms.Panel FooterPanel;
        private System.Windows.Forms.Label CostLabel;
        private System.Windows.Forms.Label EstimatedTotalLossLabel;
        private Common.Controls.NoFlickerListBox FittingContentListBox;
    }
}
