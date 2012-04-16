namespace EVEMon.CharacterMonitoring
{
    partial class CharacterWalletJournalList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterWalletJournalList));
            this.noWalletJournalLabel = new System.Windows.Forms.Label();
            this.lvWalletJournal = new System.Windows.Forms.ListView();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // noWalletJournalLabel
            // 
            this.noWalletJournalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noWalletJournalLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.noWalletJournalLabel.Location = new System.Drawing.Point(0, 0);
            this.noWalletJournalLabel.Name = "noWalletJournalLabel";
            this.noWalletJournalLabel.Size = new System.Drawing.Size(454, 434);
            this.noWalletJournalLabel.TabIndex = 3;
            this.noWalletJournalLabel.Text = "No wallet journal is available.";
            this.noWalletJournalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvWalletJournal
            // 
            this.lvWalletJournal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvWalletJournal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvWalletJournal.FullRowSelect = true;
            this.lvWalletJournal.HideSelection = false;
            this.lvWalletJournal.Location = new System.Drawing.Point(0, 0);
            this.lvWalletJournal.Name = "lvWalletJournal";
            this.lvWalletJournal.Size = new System.Drawing.Size(454, 434);
            this.lvWalletJournal.SmallImageList = this.ilIcons;
            this.lvWalletJournal.TabIndex = 4;
            this.lvWalletJournal.UseCompatibleStateImageBehavior = false;
            this.lvWalletJournal.View = System.Windows.Forms.View.Details;
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "arrow_up.png");
            this.ilIcons.Images.SetKeyName(1, "arrow_down.png");
            this.ilIcons.Images.SetKeyName(2, "16x16Transparant.png");
            // 
            // CharacterWalletJournalList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvWalletJournal);
            this.Controls.Add(this.noWalletJournalLabel);
            this.Name = "CharacterWalletJournalList";
            this.Size = new System.Drawing.Size(454, 434);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label noWalletJournalLabel;
        private System.Windows.Forms.ListView lvWalletJournal;
        private System.Windows.Forms.ImageList ilIcons;
    }
}
