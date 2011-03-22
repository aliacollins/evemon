namespace EVEMon
{
    partial class EveMailReadingPane
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
            this.previewPanePanel = new System.Windows.Forms.Panel();
            this.wbMailBody = new System.Windows.Forms.WebBrowser();
            this.flPanelMailHeader = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMailHeader = new System.Windows.Forms.Label();
            this.lblSender = new System.Windows.Forms.Label();
            this.lblSendDate = new System.Windows.Forms.Label();
            this.lblRecipient = new System.Windows.Forms.Label();
            this.previewPanePanel.SuspendLayout();
            this.flPanelMailHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // previewPanePanel
            // 
            this.previewPanePanel.BackColor = System.Drawing.SystemColors.Control;
            this.previewPanePanel.Controls.Add(this.wbMailBody);
            this.previewPanePanel.Controls.Add(this.flPanelMailHeader);
            this.previewPanePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewPanePanel.Location = new System.Drawing.Point(0, 0);
            this.previewPanePanel.Name = "previewPanePanel";
            this.previewPanePanel.Padding = new System.Windows.Forms.Padding(5);
            this.previewPanePanel.Size = new System.Drawing.Size(267, 160);
            this.previewPanePanel.TabIndex = 2;
            // 
            // wbMailBody
            // 
            this.wbMailBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbMailBody.IsWebBrowserContextMenuEnabled = false;
            this.wbMailBody.Location = new System.Drawing.Point(5, 80);
            this.wbMailBody.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbMailBody.Name = "wbMailBody";
            this.wbMailBody.Size = new System.Drawing.Size(257, 75);
            this.wbMailBody.TabIndex = 2;
            this.wbMailBody.WebBrowserShortcutsEnabled = false;
            this.wbMailBody.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbMailBody_Navigating);
            // 
            // flPanelMailHeader
            // 
            this.flPanelMailHeader.BackColor = System.Drawing.SystemColors.Window;
            this.flPanelMailHeader.Controls.Add(this.lblMailHeader);
            this.flPanelMailHeader.Controls.Add(this.lblSender);
            this.flPanelMailHeader.Controls.Add(this.lblSendDate);
            this.flPanelMailHeader.Controls.Add(this.lblRecipient);
            this.flPanelMailHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.flPanelMailHeader.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flPanelMailHeader.Location = new System.Drawing.Point(5, 5);
            this.flPanelMailHeader.MinimumSize = new System.Drawing.Size(0, 75);
            this.flPanelMailHeader.Name = "flPanelMailHeader";
            this.flPanelMailHeader.Size = new System.Drawing.Size(257, 75);
            this.flPanelMailHeader.TabIndex = 1;
            this.flPanelMailHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.flPanelMailHeader_Paint);
            // 
            // lblMailHeader
            // 
            this.lblMailHeader.AutoSize = true;
            this.lblMailHeader.Location = new System.Drawing.Point(3, 0);
            this.lblMailHeader.Name = "lblMailHeader";
            this.lblMailHeader.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblMailHeader.Size = new System.Drawing.Size(64, 19);
            this.lblMailHeader.TabIndex = 4;
            this.lblMailHeader.Text = "Mail Header";
            // 
            // lblSender
            // 
            this.lblSender.AutoSize = true;
            this.lblSender.Location = new System.Drawing.Point(3, 19);
            this.lblSender.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblSender.Name = "lblSender";
            this.lblSender.Size = new System.Drawing.Size(41, 13);
            this.lblSender.TabIndex = 1;
            this.lblSender.Text = "Sender";
            // 
            // lblSendDate
            // 
            this.lblSendDate.AutoSize = true;
            this.lblSendDate.Location = new System.Drawing.Point(3, 35);
            this.lblSendDate.Name = "lblSendDate";
            this.lblSendDate.Size = new System.Drawing.Size(32, 13);
            this.lblSendDate.TabIndex = 2;
            this.lblSendDate.Text = "Sent:";
            // 
            // lblRecipient
            // 
            this.lblRecipient.AutoSize = true;
            this.lblRecipient.Location = new System.Drawing.Point(3, 48);
            this.lblRecipient.Name = "lblRecipient";
            this.lblRecipient.Size = new System.Drawing.Size(23, 13);
            this.lblRecipient.TabIndex = 3;
            this.lblRecipient.Text = "To:";
            // 
            // EveMailReadingPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.previewPanePanel);
            this.Name = "EveMailReadingPane";
            this.Size = new System.Drawing.Size(267, 160);
            this.previewPanePanel.ResumeLayout(false);
            this.flPanelMailHeader.ResumeLayout(false);
            this.flPanelMailHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel previewPanePanel;
        private System.Windows.Forms.Label lblMailHeader;
        private System.Windows.Forms.Label lblSender;
        private System.Windows.Forms.Label lblSendDate;
        private System.Windows.Forms.Label lblRecipient;
        private System.Windows.Forms.WebBrowser wbMailBody;
        private System.Windows.Forms.FlowLayoutPanel flPanelMailHeader;

    }
}
