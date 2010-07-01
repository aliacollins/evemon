namespace EVEMon.Accounting
{
    partial class FeaturesWindow
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
            this.LimitedApiLabel = new System.Windows.Forms.Label();
            this.MainFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.CharacterLabel = new System.Windows.Forms.Label();
            this.SkillQueueLabel = new System.Windows.Forms.Label();
            this.FullApiLabel = new System.Windows.Forms.Label();
            this.MarketOrdersLabel = new System.Windows.Forms.Label();
            this.IndustryJobsLabel = new System.Windows.Forms.Label();
            this.MainFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LimitedApiLabel
            // 
            this.LimitedApiLabel.AutoSize = true;
            this.LimitedApiLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LimitedApiLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.LimitedApiLabel.Location = new System.Drawing.Point(3, 0);
            this.LimitedApiLabel.Name = "LimitedApiLabel";
            this.LimitedApiLabel.Size = new System.Drawing.Size(96, 16);
            this.LimitedApiLabel.TabIndex = 0;
            this.LimitedApiLabel.Text = "Limited API Key";
            // 
            // MainFlowLayoutPanel
            // 
            this.MainFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MainFlowLayoutPanel.Controls.Add(this.LimitedApiLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.CharacterLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.SkillQueueLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.FullApiLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.MarketOrdersLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.IndustryJobsLabel);
            this.MainFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.MainFlowLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.MainFlowLayoutPanel.Name = "MainFlowLayoutPanel";
            this.MainFlowLayoutPanel.Size = new System.Drawing.Size(342, 116);
            this.MainFlowLayoutPanel.TabIndex = 1;
            // 
            // CharacterLabel
            // 
            this.CharacterLabel.AutoSize = true;
            this.CharacterLabel.Location = new System.Drawing.Point(3, 16);
            this.CharacterLabel.Name = "CharacterLabel";
            this.CharacterLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.CharacterLabel.Size = new System.Drawing.Size(268, 18);
            this.CharacterLabel.TabIndex = 4;
            this.CharacterLabel.Text = "Character monitoring: character sheet, implants, etc.";
            // 
            // SkillQueueLabel
            // 
            this.SkillQueueLabel.AutoSize = true;
            this.SkillQueueLabel.Location = new System.Drawing.Point(3, 34);
            this.SkillQueueLabel.Name = "SkillQueueLabel";
            this.SkillQueueLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.SkillQueueLabel.Size = new System.Drawing.Size(224, 18);
            this.SkillQueueLabel.TabIndex = 1;
            this.SkillQueueLabel.Text = "Skill queue monitoring and skills planification.";
            // 
            // FullApiLabel
            // 
            this.FullApiLabel.AutoSize = true;
            this.FullApiLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FullApiLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FullApiLabel.Location = new System.Drawing.Point(3, 52);
            this.FullApiLabel.Name = "FullApiLabel";
            this.FullApiLabel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.FullApiLabel.Size = new System.Drawing.Size(75, 26);
            this.FullApiLabel.TabIndex = 2;
            this.FullApiLabel.Text = "Full API Key";
            // 
            // MarketOrdersLabel
            // 
            this.MarketOrdersLabel.AutoSize = true;
            this.MarketOrdersLabel.Location = new System.Drawing.Point(3, 78);
            this.MarketOrdersLabel.Name = "MarketOrdersLabel";
            this.MarketOrdersLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.MarketOrdersLabel.Size = new System.Drawing.Size(132, 18);
            this.MarketOrdersLabel.TabIndex = 3;
            this.MarketOrdersLabel.Text = "Market orders monitoring";
            // 
            // IndustryJobsLabel
            // 
            this.IndustryJobsLabel.AutoSize = true;
            this.IndustryJobsLabel.Location = new System.Drawing.Point(3, 96);
            this.IndustryJobsLabel.Name = "IndustryJobsLabel";
            this.IndustryJobsLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.IndustryJobsLabel.Size = new System.Drawing.Size(129, 18);
            this.IndustryJobsLabel.TabIndex = 5;
            this.IndustryJobsLabel.Text = "Industry jobs monitoring";
            // 
            // FeaturesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(366, 140);
            this.Controls.Add(this.MainFlowLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeaturesWindow";
            this.RememberPositionKey = "FeaturesWindow";
            this.Text = "Which API key do you need ?";
            this.TopMost = true;
            this.MainFlowLayoutPanel.ResumeLayout(false);
            this.MainFlowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LimitedApiLabel;
        private System.Windows.Forms.FlowLayoutPanel MainFlowLayoutPanel;
        private System.Windows.Forms.Label SkillQueueLabel;
        private System.Windows.Forms.Label FullApiLabel;
        private System.Windows.Forms.Label MarketOrdersLabel;
        private System.Windows.Forms.Label CharacterLabel;
        private System.Windows.Forms.Label IndustryJobsLabel;
    }
}