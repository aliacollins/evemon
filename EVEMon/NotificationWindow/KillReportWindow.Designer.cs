namespace EVEMon.NotificationWindow
{
    partial class KillReportWindow
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
            this.killReportFittingContent1 = new EVEMon.Controls.KillReportFittingContent();
            this.killReportInvolvedParties = new EVEMon.Controls.KillReportInvolvedParties();
            this.killReportHeader1 = new EVEMon.Controls.KillReportVictim();
            this.SuspendLayout();
            // 
            // killReportFittingContent1
            // 
            this.killReportFittingContent1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.killReportFittingContent1.Location = new System.Drawing.Point(262, 142);
            this.killReportFittingContent1.Name = "killReportFittingContent1";
            this.killReportFittingContent1.Size = new System.Drawing.Size(322, 440);
            this.killReportFittingContent1.TabIndex = 2;
            // 
            // killReportInvolvedParties
            // 
            this.killReportInvolvedParties.Dock = System.Windows.Forms.DockStyle.Left;
            this.killReportInvolvedParties.Location = new System.Drawing.Point(0, 142);
            this.killReportInvolvedParties.Name = "killReportInvolvedParties";
            this.killReportInvolvedParties.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.killReportInvolvedParties.Size = new System.Drawing.Size(262, 440);
            this.killReportInvolvedParties.TabIndex = 1;
            // 
            // killReportHeader1
            // 
            this.killReportHeader1.AutoSize = true;
            this.killReportHeader1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.killReportHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.killReportHeader1.Location = new System.Drawing.Point(0, 0);
            this.killReportHeader1.Name = "killReportHeader1";
            this.killReportHeader1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.killReportHeader1.Size = new System.Drawing.Size(584, 142);
            this.killReportHeader1.TabIndex = 0;
            // 
            // KillReportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 582);
            this.Controls.Add(this.killReportFittingContent1);
            this.Controls.Add(this.killReportInvolvedParties);
            this.Controls.Add(this.killReportHeader1);
            this.MinimumSize = new System.Drawing.Size(600, 620);
            this.Name = "KillReportWindow";
            this.Text = "Kill Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.KillReportVictim killReportHeader1;
        private Controls.KillReportInvolvedParties killReportInvolvedParties;
        private Controls.KillReportFittingContent killReportFittingContent1;

    }
}