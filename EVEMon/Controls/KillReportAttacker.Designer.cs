namespace EVEMon.Controls
{
    partial class KillReportAttacker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KillReportAttacker));
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CharacterPictureBox = new System.Windows.Forms.PictureBox();
            this.ShipPictureBox = new System.Windows.Forms.PictureBox();
            this.WeaponPictureBox = new System.Windows.Forms.PictureBox();
            this.AttackerInfoFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.CharacterNameLabel = new System.Windows.Forms.Label();
            this.CorpNameLabel = new System.Windows.Forms.Label();
            this.AllianceNameLabel = new System.Windows.Forms.Label();
            this.DamageDoneInfoFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.DamageDoneLabel = new System.Windows.Forms.Label();
            this.MainTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShipPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeaponPictureBox)).BeginInit();
            this.AttackerInfoFlowLayoutPanel.SuspendLayout();
            this.DamageDoneInfoFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.AutoSize = true;
            this.MainTableLayoutPanel.ColumnCount = 3;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.Controls.Add(this.CharacterPictureBox, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.ShipPictureBox, 1, 0);
            this.MainTableLayoutPanel.Controls.Add(this.WeaponPictureBox, 1, 1);
            this.MainTableLayoutPanel.Controls.Add(this.AttackerInfoFlowLayoutPanel, 2, 0);
            this.MainTableLayoutPanel.Controls.Add(this.DamageDoneInfoFlowLayoutPanel, 2, 1);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 2;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(250, 71);
            this.MainTableLayoutPanel.TabIndex = 0;
            // 
            // CharacterPictureBox
            // 
            this.CharacterPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("CharacterPictureBox.Image")));
            this.CharacterPictureBox.Location = new System.Drawing.Point(3, 3);
            this.CharacterPictureBox.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.CharacterPictureBox.Name = "CharacterPictureBox";
            this.MainTableLayoutPanel.SetRowSpan(this.CharacterPictureBox, 2);
            this.CharacterPictureBox.Size = new System.Drawing.Size(64, 64);
            this.CharacterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CharacterPictureBox.TabIndex = 0;
            this.CharacterPictureBox.TabStop = false;
            // 
            // ShipPictureBox
            // 
            this.ShipPictureBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ShipPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ShipPictureBox.Image")));
            this.ShipPictureBox.Location = new System.Drawing.Point(67, 3);
            this.ShipPictureBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ShipPictureBox.Name = "ShipPictureBox";
            this.ShipPictureBox.Size = new System.Drawing.Size(32, 32);
            this.ShipPictureBox.TabIndex = 1;
            this.ShipPictureBox.TabStop = false;
            // 
            // WeaponPictureBox
            // 
            this.WeaponPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.WeaponPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("WeaponPictureBox.Image")));
            this.WeaponPictureBox.Location = new System.Drawing.Point(67, 35);
            this.WeaponPictureBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.WeaponPictureBox.Name = "WeaponPictureBox";
            this.WeaponPictureBox.Size = new System.Drawing.Size(32, 32);
            this.WeaponPictureBox.TabIndex = 2;
            this.WeaponPictureBox.TabStop = false;
            // 
            // AttackerInfoFlowLayoutPanel
            // 
            this.AttackerInfoFlowLayoutPanel.AutoSize = true;
            this.AttackerInfoFlowLayoutPanel.Controls.Add(this.CharacterNameLabel);
            this.AttackerInfoFlowLayoutPanel.Controls.Add(this.CorpNameLabel);
            this.AttackerInfoFlowLayoutPanel.Controls.Add(this.AllianceNameLabel);
            this.AttackerInfoFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AttackerInfoFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.AttackerInfoFlowLayoutPanel.Location = new System.Drawing.Point(99, 0);
            this.AttackerInfoFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.AttackerInfoFlowLayoutPanel.Name = "AttackerInfoFlowLayoutPanel";
            this.AttackerInfoFlowLayoutPanel.Size = new System.Drawing.Size(151, 35);
            this.AttackerInfoFlowLayoutPanel.TabIndex = 3;
            // 
            // CharacterNameLabel
            // 
            this.CharacterNameLabel.AutoSize = true;
            this.CharacterNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.CharacterNameLabel.Location = new System.Drawing.Point(3, 3);
            this.CharacterNameLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.CharacterNameLabel.Name = "CharacterNameLabel";
            this.CharacterNameLabel.Size = new System.Drawing.Size(76, 9);
            this.CharacterNameLabel.TabIndex = 0;
            this.CharacterNameLabel.Text = "Character Name";
            // 
            // CorpNameLabel
            // 
            this.CorpNameLabel.AutoSize = true;
            this.CorpNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.CorpNameLabel.Location = new System.Drawing.Point(3, 14);
            this.CorpNameLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.CorpNameLabel.Name = "CorpNameLabel";
            this.CorpNameLabel.Size = new System.Drawing.Size(84, 9);
            this.CorpNameLabel.TabIndex = 1;
            this.CorpNameLabel.Text = "Corporation Name";
            // 
            // AllianceNameLabel
            // 
            this.AllianceNameLabel.AutoSize = true;
            this.AllianceNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.AllianceNameLabel.Location = new System.Drawing.Point(3, 25);
            this.AllianceNameLabel.Name = "AllianceNameLabel";
            this.AllianceNameLabel.Size = new System.Drawing.Size(68, 9);
            this.AllianceNameLabel.TabIndex = 2;
            this.AllianceNameLabel.Text = "Alliance Name";
            // 
            // DamageDoneInfoFlowLayoutPanel
            // 
            this.DamageDoneInfoFlowLayoutPanel.AutoSize = true;
            this.DamageDoneInfoFlowLayoutPanel.Controls.Add(this.DamageDoneLabel);
            this.DamageDoneInfoFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DamageDoneInfoFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.DamageDoneInfoFlowLayoutPanel.Location = new System.Drawing.Point(99, 35);
            this.DamageDoneInfoFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.DamageDoneInfoFlowLayoutPanel.Name = "DamageDoneInfoFlowLayoutPanel";
            this.DamageDoneInfoFlowLayoutPanel.Size = new System.Drawing.Size(151, 36);
            this.DamageDoneInfoFlowLayoutPanel.TabIndex = 4;
            // 
            // DamageDoneLabel
            // 
            this.DamageDoneLabel.AutoSize = true;
            this.DamageDoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.DamageDoneLabel.Location = new System.Drawing.Point(3, 17);
            this.DamageDoneLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.DamageDoneLabel.Name = "DamageDoneLabel";
            this.DamageDoneLabel.Size = new System.Drawing.Size(56, 13);
            this.DamageDoneLabel.TabIndex = 0;
            this.DamageDoneLabel.Text = "{0} ({1:P1})";
            // 
            // KillReportAttacker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Name = "KillReportAttacker";
            this.Size = new System.Drawing.Size(250, 71);
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.MainTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShipPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeaponPictureBox)).EndInit();
            this.AttackerInfoFlowLayoutPanel.ResumeLayout(false);
            this.AttackerInfoFlowLayoutPanel.PerformLayout();
            this.DamageDoneInfoFlowLayoutPanel.ResumeLayout(false);
            this.DamageDoneInfoFlowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.PictureBox CharacterPictureBox;
        private System.Windows.Forms.PictureBox ShipPictureBox;
        private System.Windows.Forms.PictureBox WeaponPictureBox;
        private System.Windows.Forms.FlowLayoutPanel AttackerInfoFlowLayoutPanel;
        private System.Windows.Forms.Label CharacterNameLabel;
        private System.Windows.Forms.Label CorpNameLabel;
        private System.Windows.Forms.Label AllianceNameLabel;
        private System.Windows.Forms.FlowLayoutPanel DamageDoneInfoFlowLayoutPanel;
        private System.Windows.Forms.Label DamageDoneLabel;
    }
}
