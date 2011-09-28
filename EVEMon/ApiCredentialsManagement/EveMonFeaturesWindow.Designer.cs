namespace EVEMon.ApiCredentialsManagement
{
    partial class EVEMonFeaturesWindow
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
            this.BasicFeaturesLabel = new System.Windows.Forms.Label();
            this.MainFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.basicFeaturesflowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.CharacterMonitoringLabel = new System.Windows.Forms.Label();
            this.SkillQueueMonitoringLabel = new System.Windows.Forms.Label();
            this.CreateBasicAPIKeyLinkLabel = new System.Windows.Forms.LinkLabel();
            this.AdvancedFeaturesLabel = new System.Windows.Forms.Label();
            this.advanceFeaturesFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AccountStatusLabel = new System.Windows.Forms.Label();
            this.EVEMailMessagesLabel = new System.Windows.Forms.Label();
            this.EVENotificationsLabel = new System.Windows.Forms.Label();
            this.IndustryJobsLabel = new System.Windows.Forms.Label();
            this.MarketOrdersLabel = new System.Windows.Forms.Label();
            this.ResearchPointsLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CreateAdvancedAPIKeyLinkLabel = new System.Windows.Forms.LinkLabel();
            this.MainFlowLayoutPanel.SuspendLayout();
            this.basicFeaturesflowLayoutPanel.SuspendLayout();
            this.advanceFeaturesFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BasicFeaturesLabel
            // 
            this.BasicFeaturesLabel.AutoSize = true;
            this.BasicFeaturesLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BasicFeaturesLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.BasicFeaturesLabel.Location = new System.Drawing.Point(3, 0);
            this.BasicFeaturesLabel.Name = "BasicFeaturesLabel";
            this.BasicFeaturesLabel.Size = new System.Drawing.Size(91, 16);
            this.BasicFeaturesLabel.TabIndex = 0;
            this.BasicFeaturesLabel.Text = "Basic Features";
            // 
            // MainFlowLayoutPanel
            // 
            this.MainFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MainFlowLayoutPanel.Controls.Add(this.BasicFeaturesLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.basicFeaturesflowLayoutPanel);
            this.MainFlowLayoutPanel.Controls.Add(this.CreateBasicAPIKeyLinkLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.AdvancedFeaturesLabel);
            this.MainFlowLayoutPanel.Controls.Add(this.advanceFeaturesFlowLayoutPanel);
            this.MainFlowLayoutPanel.Controls.Add(this.CreateAdvancedAPIKeyLinkLabel);
            this.MainFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.MainFlowLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.MainFlowLayoutPanel.Name = "MainFlowLayoutPanel";
            this.MainFlowLayoutPanel.Size = new System.Drawing.Size(360, 343);
            this.MainFlowLayoutPanel.TabIndex = 1;
            // 
            // basicFeaturesflowLayoutPanel
            // 
            this.basicFeaturesflowLayoutPanel.Controls.Add(this.CharacterMonitoringLabel);
            this.basicFeaturesflowLayoutPanel.Controls.Add(this.SkillQueueMonitoringLabel);
            this.basicFeaturesflowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.basicFeaturesflowLayoutPanel.Location = new System.Drawing.Point(3, 19);
            this.basicFeaturesflowLayoutPanel.Name = "basicFeaturesflowLayoutPanel";
            this.basicFeaturesflowLayoutPanel.Size = new System.Drawing.Size(354, 60);
            this.basicFeaturesflowLayoutPanel.TabIndex = 1;
            // 
            // CharacterMonitoringLabel
            // 
            this.CharacterMonitoringLabel.AutoSize = true;
            this.CharacterMonitoringLabel.Location = new System.Drawing.Point(3, 0);
            this.CharacterMonitoringLabel.Name = "CharacterMonitoringLabel";
            this.CharacterMonitoringLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.CharacterMonitoringLabel.Size = new System.Drawing.Size(178, 57);
            this.CharacterMonitoringLabel.TabIndex = 4;
            this.CharacterMonitoringLabel.Text = "For Character monitoring\r\nand skill levels planning: \r\n- Character Sheet\r\n- Chara" +
                "cter Info (Private or Public)";
            // 
            // SkillQueueMonitoringLabel
            // 
            this.SkillQueueMonitoringLabel.AutoSize = true;
            this.SkillQueueMonitoringLabel.Location = new System.Drawing.Point(187, 0);
            this.SkillQueueMonitoringLabel.Name = "SkillQueueMonitoringLabel";
            this.SkillQueueMonitoringLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.SkillQueueMonitoringLabel.Size = new System.Drawing.Size(140, 44);
            this.SkillQueueMonitoringLabel.TabIndex = 1;
            this.SkillQueueMonitoringLabel.Text = "For Skill Queue monitoring:\r\n- Skill Queue\r\n- Skill In Training";
            // 
            // CreateBasicAPIKeyLinkLabel
            // 
            this.CreateBasicAPIKeyLinkLabel.AutoSize = true;
            this.CreateBasicAPIKeyLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(43, 55);
            this.CreateBasicAPIKeyLinkLabel.Location = new System.Drawing.Point(3, 82);
            this.CreateBasicAPIKeyLinkLabel.Name = "CreateBasicAPIKeyLinkLabel";
            this.CreateBasicAPIKeyLinkLabel.Size = new System.Drawing.Size(288, 31);
            this.CreateBasicAPIKeyLinkLabel.TabIndex = 11;
            this.CreateBasicAPIKeyLinkLabel.TabStop = true;
            this.CreateBasicAPIKeyLinkLabel.Text = "To create a basic features API key visit : https://support.eveonline.com /api/key" +
                "/createpredefined";
            this.CreateBasicAPIKeyLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CreateBasicAPIKeyLinkLabel.UseCompatibleTextRendering = true;
            this.CreateBasicAPIKeyLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CreateBasicAPIKeyLinkLabel_LinkClicked);
            // 
            // AdvancedFeaturesLabel
            // 
            this.AdvancedFeaturesLabel.AutoSize = true;
            this.AdvancedFeaturesLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdvancedFeaturesLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.AdvancedFeaturesLabel.Location = new System.Drawing.Point(3, 113);
            this.AdvancedFeaturesLabel.Name = "AdvancedFeaturesLabel";
            this.AdvancedFeaturesLabel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.AdvancedFeaturesLabel.Size = new System.Drawing.Size(117, 26);
            this.AdvancedFeaturesLabel.TabIndex = 2;
            this.AdvancedFeaturesLabel.Text = "Advanced Features";
            // 
            // advanceFeaturesFlowLayoutPanel
            // 
            this.advanceFeaturesFlowLayoutPanel.Controls.Add(this.AccountStatusLabel);
            this.advanceFeaturesFlowLayoutPanel.Controls.Add(this.EVEMailMessagesLabel);
            this.advanceFeaturesFlowLayoutPanel.Controls.Add(this.EVENotificationsLabel);
            this.advanceFeaturesFlowLayoutPanel.Controls.Add(this.IndustryJobsLabel);
            this.advanceFeaturesFlowLayoutPanel.Controls.Add(this.MarketOrdersLabel);
            this.advanceFeaturesFlowLayoutPanel.Controls.Add(this.ResearchPointsLabel);
            this.advanceFeaturesFlowLayoutPanel.Controls.Add(this.label1);
            this.advanceFeaturesFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.advanceFeaturesFlowLayoutPanel.Location = new System.Drawing.Point(3, 142);
            this.advanceFeaturesFlowLayoutPanel.Name = "advanceFeaturesFlowLayoutPanel";
            this.advanceFeaturesFlowLayoutPanel.Size = new System.Drawing.Size(354, 166);
            this.advanceFeaturesFlowLayoutPanel.TabIndex = 10;
            // 
            // AccountStatusLabel
            // 
            this.AccountStatusLabel.AutoSize = true;
            this.AccountStatusLabel.Location = new System.Drawing.Point(3, 0);
            this.AccountStatusLabel.Name = "AccountStatusLabel";
            this.AccountStatusLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.AccountStatusLabel.Size = new System.Drawing.Size(161, 31);
            this.AccountStatusLabel.TabIndex = 9;
            this.AccountStatusLabel.Text = "For Account Status monitoring:\r\n- AccountStatus";
            // 
            // EVEMailMessagesLabel
            // 
            this.EVEMailMessagesLabel.AutoSize = true;
            this.EVEMailMessagesLabel.Location = new System.Drawing.Point(3, 31);
            this.EVEMailMessagesLabel.Name = "EVEMailMessagesLabel";
            this.EVEMailMessagesLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.EVEMailMessagesLabel.Size = new System.Drawing.Size(177, 57);
            this.EVEMailMessagesLabel.TabIndex = 7;
            this.EVEMailMessagesLabel.Text = "For EVE Mail messages monitoring:\r\n- MailMessages\r\n- MailingLists\r\n- MailBodies";
            // 
            // EVENotificationsLabel
            // 
            this.EVENotificationsLabel.AutoSize = true;
            this.EVENotificationsLabel.Location = new System.Drawing.Point(3, 88);
            this.EVENotificationsLabel.Name = "EVENotificationsLabel";
            this.EVENotificationsLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.EVENotificationsLabel.Size = new System.Drawing.Size(168, 44);
            this.EVENotificationsLabel.TabIndex = 8;
            this.EVENotificationsLabel.Text = "For EVE Notifications monitoring:\r\n- Notifications\r\n- NotificationTexts";
            // 
            // IndustryJobsLabel
            // 
            this.IndustryJobsLabel.AutoSize = true;
            this.IndustryJobsLabel.Location = new System.Drawing.Point(3, 132);
            this.IndustryJobsLabel.Name = "IndustryJobsLabel";
            this.IndustryJobsLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.IndustryJobsLabel.Size = new System.Drawing.Size(152, 31);
            this.IndustryJobsLabel.TabIndex = 5;
            this.IndustryJobsLabel.Text = "For Industry jobs monitoring:\r\n- IndustryJobs";
            // 
            // MarketOrdersLabel
            // 
            this.MarketOrdersLabel.AutoSize = true;
            this.MarketOrdersLabel.Location = new System.Drawing.Point(186, 0);
            this.MarketOrdersLabel.Name = "MarketOrdersLabel";
            this.MarketOrdersLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.MarketOrdersLabel.Size = new System.Drawing.Size(155, 31);
            this.MarketOrdersLabel.TabIndex = 3;
            this.MarketOrdersLabel.Text = "For Market orders monitoring:\r\n- MarketOrders";
            // 
            // ResearchPointsLabel
            // 
            this.ResearchPointsLabel.AutoSize = true;
            this.ResearchPointsLabel.Location = new System.Drawing.Point(186, 31);
            this.ResearchPointsLabel.Name = "ResearchPointsLabel";
            this.ResearchPointsLabel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.ResearchPointsLabel.Size = new System.Drawing.Size(165, 31);
            this.ResearchPointsLabel.TabIndex = 6;
            this.ResearchPointsLabel.Text = "For Research points monitoring:\r\n- Research";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(186, 62);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(135, 31);
            this.label1.TabIndex = 11;
            this.label1.Text = "For Standings monitoring:\r\n- Standings";
            // 
            // CreateAdvancedAPIKeyLinkLabel
            // 
            this.CreateAdvancedAPIKeyLinkLabel.AutoSize = true;
            this.CreateAdvancedAPIKeyLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(56, 55);
            this.CreateAdvancedAPIKeyLinkLabel.Location = new System.Drawing.Point(3, 311);
            this.CreateAdvancedAPIKeyLinkLabel.Name = "CreateAdvancedAPIKeyLinkLabel";
            this.CreateAdvancedAPIKeyLinkLabel.Size = new System.Drawing.Size(288, 31);
            this.CreateAdvancedAPIKeyLinkLabel.TabIndex = 12;
            this.CreateAdvancedAPIKeyLinkLabel.TabStop = true;
            this.CreateAdvancedAPIKeyLinkLabel.Text = "To create a basic and advanced features API key visit : https://support.eveonline" +
                ".com /api/key/createpredefined";
            this.CreateAdvancedAPIKeyLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CreateAdvancedAPIKeyLinkLabel.UseCompatibleTextRendering = true;
            this.CreateAdvancedAPIKeyLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CreateAllFeaturesAPIKeyLinkLabel_LinkClicked);
            // 
            // EVEMonFeaturesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(384, 367);
            this.Controls.Add(this.MainFlowLayoutPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EVEMonFeaturesWindow";
            this.Text = "Which Access Mask do you need ?";
            this.TopMost = true;
            this.MainFlowLayoutPanel.ResumeLayout(false);
            this.MainFlowLayoutPanel.PerformLayout();
            this.basicFeaturesflowLayoutPanel.ResumeLayout(false);
            this.basicFeaturesflowLayoutPanel.PerformLayout();
            this.advanceFeaturesFlowLayoutPanel.ResumeLayout(false);
            this.advanceFeaturesFlowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label BasicFeaturesLabel;
        private System.Windows.Forms.FlowLayoutPanel MainFlowLayoutPanel;
        private System.Windows.Forms.Label SkillQueueMonitoringLabel;
        private System.Windows.Forms.Label AdvancedFeaturesLabel;
        private System.Windows.Forms.Label MarketOrdersLabel;
        private System.Windows.Forms.Label CharacterMonitoringLabel;
        private System.Windows.Forms.Label IndustryJobsLabel;
        private System.Windows.Forms.Label ResearchPointsLabel;
        private System.Windows.Forms.Label EVEMailMessagesLabel;
        private System.Windows.Forms.Label EVENotificationsLabel;
        private System.Windows.Forms.FlowLayoutPanel advanceFeaturesFlowLayoutPanel;
        private System.Windows.Forms.Label AccountStatusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel basicFeaturesflowLayoutPanel;
        private System.Windows.Forms.LinkLabel CreateBasicAPIKeyLinkLabel;
        private System.Windows.Forms.LinkLabel CreateAdvancedAPIKeyLinkLabel;
    }
}