namespace EVEMon
{
    partial class TrayStatusPopUpChar
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
            this.components = new System.ComponentModel.Container();
            this.containerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pbCharacterPortrait = new System.Windows.Forms.PictureBox();
            this.charDetailsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCharName = new System.Windows.Forms.Label();
            this.lblSkillInTraining = new System.Windows.Forms.Label();
            this.lblTimeToCompletion = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblCompletionTime = new System.Windows.Forms.Label();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.containerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCharacterPortrait)).BeginInit();
            this.charDetailsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // containerPanel
            // 
            this.containerPanel.AutoSize = true;
            this.containerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.containerPanel.Controls.Add(this.pbCharacterPortrait);
            this.containerPanel.Controls.Add(this.charDetailsPanel);
            this.containerPanel.Location = new System.Drawing.Point(3, 3);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(195, 76);
            this.containerPanel.TabIndex = 0;
            // 
            // pbCharacterPortrait
            // 
            this.pbCharacterPortrait.Location = new System.Drawing.Point(3, 3);
            this.pbCharacterPortrait.Name = "pbCharacterPortrait";
            this.pbCharacterPortrait.Size = new System.Drawing.Size(48, 48);
            this.pbCharacterPortrait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCharacterPortrait.TabIndex = 0;
            this.pbCharacterPortrait.TabStop = false;
            // 
            // charDetailsPanel
            // 
            this.charDetailsPanel.AutoSize = true;
            this.charDetailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.charDetailsPanel.Controls.Add(this.lblCharName);
            this.charDetailsPanel.Controls.Add(this.lblSkillInTraining);
            this.charDetailsPanel.Controls.Add(this.lblTimeToCompletion);
            this.charDetailsPanel.Controls.Add(this.lblCompletionTime);
            this.charDetailsPanel.Controls.Add(this.lblBalance);
            this.charDetailsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.charDetailsPanel.Location = new System.Drawing.Point(57, 3);
            this.charDetailsPanel.Name = "charDetailsPanel";
            this.charDetailsPanel.Size = new System.Drawing.Size(135, 70);
            this.charDetailsPanel.TabIndex = 1;
            // 
            // lblCharName
            // 
            this.lblCharName.AutoSize = true;
            this.lblCharName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharName.Location = new System.Drawing.Point(3, 0);
            this.lblCharName.Name = "lblCharName";
            this.lblCharName.Size = new System.Drawing.Size(129, 18);
            this.lblCharName.TabIndex = 1;
            this.lblCharName.Text = "Character Name";
            // 
            // lblSkillInTraining
            // 
            this.lblSkillInTraining.AutoSize = true;
            this.lblSkillInTraining.Location = new System.Drawing.Point(3, 18);
            this.lblSkillInTraining.Name = "lblSkillInTraining";
            this.lblSkillInTraining.Size = new System.Drawing.Size(72, 13);
            this.lblSkillInTraining.TabIndex = 2;
            this.lblSkillInTraining.Text = "SkillInTraining";
            // 
            // lblTimeToCompletion
            // 
            this.lblTimeToCompletion.AutoSize = true;
            this.lblTimeToCompletion.Location = new System.Drawing.Point(3, 31);
            this.lblTimeToCompletion.Name = "lblTimeToCompletion";
            this.lblTimeToCompletion.Size = new System.Drawing.Size(98, 13);
            this.lblTimeToCompletion.TabIndex = 2;
            this.lblTimeToCompletion.Text = "Time to Completion";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Location = new System.Drawing.Point(3, 57);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(44, 13);
            this.lblBalance.TabIndex = 4;
            this.lblBalance.Text = "Balance";
            // 
            // lblCompletionTime
            // 
            this.lblCompletionTime.AutoSize = true;
            this.lblCompletionTime.Location = new System.Drawing.Point(3, 44);
            this.lblCompletionTime.Name = "lblCompletionTime";
            this.lblCompletionTime.Size = new System.Drawing.Size(82, 13);
            this.lblCompletionTime.TabIndex = 5;
            this.lblCompletionTime.Text = "CompletionTime";
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // TrayStatusPopUpChar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.containerPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TrayStatusPopUpChar";
            this.Size = new System.Drawing.Size(201, 82);
            this.containerPanel.ResumeLayout(false);
            this.containerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCharacterPortrait)).EndInit();
            this.charDetailsPanel.ResumeLayout(false);
            this.charDetailsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel containerPanel;
        private System.Windows.Forms.PictureBox pbCharacterPortrait;
        private System.Windows.Forms.FlowLayoutPanel charDetailsPanel;
        private System.Windows.Forms.Label lblCharName;
        private System.Windows.Forms.Label lblSkillInTraining;
        private System.Windows.Forms.Label lblTimeToCompletion;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Label lblCompletionTime;
        private System.Windows.Forms.Timer updateTimer;
    }
}
