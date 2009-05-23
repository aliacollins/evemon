namespace EVEMon
{
    partial class CharactersGridItem
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
            this.pbCharacterPortrait = new System.Windows.Forms.PictureBox();
            this.backButton = new System.Windows.Forms.Button();
            this.lblCompletionTime = new EVEMon.CharactersGridLabel();
            this.lblCharName = new EVEMon.CharactersGridLabel();
            this.lblSkillInTraining = new EVEMon.CharactersGridLabel();
            this.lblTimeToCompletion = new EVEMon.CharactersGridLabel();
            this.lblBalance = new EVEMon.CharactersGridLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbCharacterPortrait)).BeginInit();
            this.SuspendLayout();
            // 
            // pbCharacterPortrait
            // 
            this.pbCharacterPortrait.Enabled = false;
            this.pbCharacterPortrait.ErrorImage = null;
            this.pbCharacterPortrait.Image = global::EVEMon.Properties.Resources.default_char_pic;
            this.pbCharacterPortrait.Location = new System.Drawing.Point(9, 11);
            this.pbCharacterPortrait.Name = "pbCharacterPortrait";
            this.pbCharacterPortrait.Size = new System.Drawing.Size(92, 92);
            this.pbCharacterPortrait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCharacterPortrait.TabIndex = 0;
            this.pbCharacterPortrait.TabStop = false;
            // 
            // backButton
            // 
            this.backButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backButton.Location = new System.Drawing.Point(0, 0);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(296, 114);
            this.backButton.TabIndex = 10;
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Visible = false;
            // 
            // lblCompletionTime
            // 
            this.lblCompletionTime.AutoEllipsis = true;
            this.lblCompletionTime.BackColor = System.Drawing.Color.Transparent;
            this.lblCompletionTime.Enabled = false;
            this.lblCompletionTime.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompletionTime.ForeColor = System.Drawing.Color.DimGray;
            this.lblCompletionTime.Location = new System.Drawing.Point(107, 90);
            this.lblCompletionTime.Name = "lblCompletionTime";
            this.lblCompletionTime.Size = new System.Drawing.Size(186, 13);
            this.lblCompletionTime.TabIndex = 11;
            this.lblCompletionTime.Text = "Monday 5, 18:32:15";
            // 
            // lblCharName
            // 
            this.lblCharName.AutoEllipsis = true;
            this.lblCharName.BackColor = System.Drawing.Color.Transparent;
            this.lblCharName.Enabled = false;
            this.lblCharName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharName.Location = new System.Drawing.Point(107, 11);
            this.lblCharName.Name = "lblCharName";
            this.lblCharName.Size = new System.Drawing.Size(186, 18);
            this.lblCharName.TabIndex = 6;
            this.lblCharName.Text = "Character Name";
            // 
            // lblSkillInTraining
            // 
            this.lblSkillInTraining.AutoEllipsis = true;
            this.lblSkillInTraining.BackColor = System.Drawing.Color.Transparent;
            this.lblSkillInTraining.Enabled = false;
            this.lblSkillInTraining.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSkillInTraining.ForeColor = System.Drawing.Color.DimGray;
            this.lblSkillInTraining.Location = new System.Drawing.Point(107, 77);
            this.lblSkillInTraining.Name = "lblSkillInTraining";
            this.lblSkillInTraining.Size = new System.Drawing.Size(186, 13);
            this.lblSkillInTraining.TabIndex = 8;
            this.lblSkillInTraining.Text = "Energy Management V";
            // 
            // lblTimeToCompletion
            // 
            this.lblTimeToCompletion.AutoEllipsis = true;
            this.lblTimeToCompletion.BackColor = System.Drawing.Color.Transparent;
            this.lblTimeToCompletion.Enabled = false;
            this.lblTimeToCompletion.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeToCompletion.ForeColor = System.Drawing.Color.DimGray;
            this.lblTimeToCompletion.Location = new System.Drawing.Point(107, 61);
            this.lblTimeToCompletion.Name = "lblTimeToCompletion";
            this.lblTimeToCompletion.Size = new System.Drawing.Size(186, 16);
            this.lblTimeToCompletion.TabIndex = 7;
            this.lblTimeToCompletion.Text = "10d 12h 35m 24s";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoEllipsis = true;
            this.lblBalance.BackColor = System.Drawing.Color.Transparent;
            this.lblBalance.Enabled = false;
            this.lblBalance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalance.ForeColor = System.Drawing.Color.DimGray;
            this.lblBalance.Location = new System.Drawing.Point(108, 30);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(185, 16);
            this.lblBalance.TabIndex = 9;
            this.lblBalance.Text = "12,534,125,453.02 ISK";
            // 
            // CharactersGridItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblCompletionTime);
            this.Controls.Add(this.lblCharName);
            this.Controls.Add(this.lblSkillInTraining);
            this.Controls.Add(this.lblTimeToCompletion);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.pbCharacterPortrait);
            this.Controls.Add(this.backButton);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "CharactersGridItem";
            this.Size = new System.Drawing.Size(296, 114);
            ((System.ComponentModel.ISupportInitialize)(this.pbCharacterPortrait)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbCharacterPortrait;
        private EVEMon.CharactersGridLabel lblCharName;
        private EVEMon.CharactersGridLabel lblSkillInTraining;
        private EVEMon.CharactersGridLabel lblTimeToCompletion;
        private EVEMon.CharactersGridLabel lblBalance;
        private System.Windows.Forms.Button backButton;
        private EVEMon.CharactersGridLabel lblCompletionTime;
    }
}
