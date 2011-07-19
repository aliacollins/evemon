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
            this.fullAPIFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AccountStatusLabel = new System.Windows.Forms.Label();
            this.EVEMailMessagesLabel = new System.Windows.Forms.Label();
            this.EVENotificationsLabel = new System.Windows.Forms.Label();
            this.IndustryJobsLabel = new System.Windows.Forms.Label();
            this.MarketOrdersLabel = new System.Windows.Forms.Label();
            this.ResearchPointsLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MainFlowLayoutPanel.SuspendLayout();
            this.fullAPIFlowLayoutPanel.SuspendLayout();
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
            this.MainFlowLayoutPanel.Controls.Add(this.label1);
            this.MainFlowLayoutPanel.Controls.Add(this.FullApiLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.fullAPIFlowLayoutPanel);
            this.MainFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.MainFlowLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.MainFlowLayoutPanel.Name = "MainFlowLayoutPanel";
            this.MainFlowLayoutPanel.Size = new System.Drawing.Size(342, 162);
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
            this.SkillQueueLabel.Size = new System.Drawing.Size(232, 18);
            this.SkillQueueLabel.TabIndex = 1;
            this.SkillQueueLabel.Text = "Skill queue monitoring and skill levels planning.";
            // 
            // FullApiLabel
            // 
            this.FullApiLabel.AutoSize = true;
            this.FullApiLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FullApiLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FullApiLabel.Location = new System.Drawing.Point(3, 70);
            this.FullApiLabel.Name = "FullApiLabel";
            this.FullApiLabel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.FullApiLabel.Size = new System.Drawing.Size(75, 26);
            this.FullApiLabel.TabIndex = 2;
            this.FullApiLabel.Text = "Full API Key";
            // 
            // fullAPIFlowLayoutPanel
            // 
            this.fullAPIFlowLayoutPanel.Controls.Add(this.AccountStatusLabel);
            this.fullAPIFlowLayoutPanel.Controls.Add(this.EVEMailMessagesLabel);
            this.fullAPIFlowLayoutPanel.Controls.Add(this.EVENotificationsLabel);
            this.fullAPIFlowLayoutPanel.Controls.Add(this.IndustryJobsLabel);
            this.fullAPIFlowLayoutPanel.Controls.Add(this.MarketOrdersLabel);
            this.fullAPIFlowLayoutPanel.Controls.Add(this.ResearchPointsLabel);
            this.fullAPIFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.fullAPIFlowLayoutPanel.Location = new System.Drawing.Point(3, 99);
            this.fullAPIFlowLayoutPanel.Name = "fullAPIFlowLayoutPanel";
            this.fullAPIFlowLayoutPanel.Size = new System.Drawing.Size(339, 60);
            this.fullAPIFlowLayoutPanel.TabIndex = 10;
            // 
            // AccountStatusLabel
            // 
            this.AccountStatusLabel.AutoSize = true;
            this.AccountStatusLabel.Location = new System.Drawing.Point(3, 0);
            this.AccountStatusLabel.Name = "AccountStatusLabel";
            this.AccountStatusLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.AccountStatusLabel.Size = new System.Drawing.Size(142, 18);
            this.AccountStatusLabel.TabIndex = 9;
            this.AccountStatusLabel.Text = "Account Status monitoring.";
            // 
            // EVEMailMessagesLabel
            // 
            this.EVEMailMessagesLabel.AutoSize = true;
            this.EVEMailMessagesLabel.Location = new System.Drawing.Point(3, 18);
            this.EVEMailMessagesLabel.Name = "EVEMailMessagesLabel";
            this.EVEMailMessagesLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.EVEMailMessagesLabel.Size = new System.Drawing.Size(158, 18);
            this.EVEMailMessagesLabel.TabIndex = 7;
            this.EVEMailMessagesLabel.Text = "EVE Mail messages monitoring.";
            // 
            // EVENotificationsLabel
            // 
            this.EVENotificationsLabel.AutoSize = true;
            this.EVENotificationsLabel.Location = new System.Drawing.Point(3, 36);
            this.EVENotificationsLabel.Name = "EVENotificationsLabel";
            this.EVENotificationsLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.EVENotificationsLabel.Size = new System.Drawing.Size(149, 18);
            this.EVENotificationsLabel.TabIndex = 8;
            this.EVENotificationsLabel.Text = "EVE Notifications monitoring.";
            // 
            // IndustryJobsLabel
            // 
            this.IndustryJobsLabel.AutoSize = true;
            this.IndustryJobsLabel.Location = new System.Drawing.Point(167, 0);
            this.IndustryJobsLabel.Name = "IndustryJobsLabel";
            this.IndustryJobsLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.IndustryJobsLabel.Size = new System.Drawing.Size(133, 18);
            this.IndustryJobsLabel.TabIndex = 5;
            this.IndustryJobsLabel.Text = "Industry jobs monitoring.";
            // 
            // MarketOrdersLabel
            // 
            this.MarketOrdersLabel.AutoSize = true;
            this.MarketOrdersLabel.Location = new System.Drawing.Point(167, 18);
            this.MarketOrdersLabel.Name = "MarketOrdersLabel";
            this.MarketOrdersLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.MarketOrdersLabel.Size = new System.Drawing.Size(136, 18);
            this.MarketOrdersLabel.TabIndex = 3;
            this.MarketOrdersLabel.Text = "Market orders monitoring.";
            // 
            // ResearchPointsLabel
            // 
            this.ResearchPointsLabel.AutoSize = true;
            this.ResearchPointsLabel.Location = new System.Drawing.Point(167, 36);
            this.ResearchPointsLabel.Name = "ResearchPointsLabel";
            this.ResearchPointsLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.ResearchPointsLabel.Size = new System.Drawing.Size(146, 18);
            this.ResearchPointsLabel.TabIndex = 6;
            this.ResearchPointsLabel.Text = "Research points monitoring.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 52);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(116, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "Standings monitoring.";
            // 
            // FeaturesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(366, 186);
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
            this.fullAPIFlowLayoutPanel.ResumeLayout(false);
            this.fullAPIFlowLayoutPanel.PerformLayout();
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
        private System.Windows.Forms.Label ResearchPointsLabel;
        private System.Windows.Forms.Label EVEMailMessagesLabel;
        private System.Windows.Forms.Label EVENotificationsLabel;
        private System.Windows.Forms.FlowLayoutPanel fullAPIFlowLayoutPanel;
        private System.Windows.Forms.Label AccountStatusLabel;
        private System.Windows.Forms.Label label1;
    }
}