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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CharacterPictureBox = new System.Windows.Forms.PictureBox();
            this.ShipPictureBox = new System.Windows.Forms.PictureBox();
            this.WeaponPictureBox = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.CharacterNameLabel = new System.Windows.Forms.Label();
            this.CorpNameLabel = new System.Windows.Forms.Label();
            this.AllianceNameLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.DamageDoneLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShipPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeaponPictureBox)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.CharacterPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.ShipPictureBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.WeaponPictureBox, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanel1, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanel2, 2, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(250, 71);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // CharacterPictureBox
            // 
            this.CharacterPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("CharacterPictureBox.Image")));
            this.CharacterPictureBox.Location = new System.Drawing.Point(3, 4);
            this.CharacterPictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.CharacterPictureBox.Name = "CharacterPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.CharacterPictureBox, 2);
            this.CharacterPictureBox.Size = new System.Drawing.Size(64, 64);
            this.CharacterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CharacterPictureBox.TabIndex = 0;
            this.CharacterPictureBox.TabStop = false;
            // 
            // ShipPictureBox
            // 
            this.ShipPictureBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ShipPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ShipPictureBox.Image")));
            this.ShipPictureBox.Location = new System.Drawing.Point(70, 4);
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
            this.WeaponPictureBox.Location = new System.Drawing.Point(70, 36);
            this.WeaponPictureBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.WeaponPictureBox.Name = "WeaponPictureBox";
            this.WeaponPictureBox.Size = new System.Drawing.Size(32, 32);
            this.WeaponPictureBox.TabIndex = 2;
            this.WeaponPictureBox.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.CharacterNameLabel);
            this.flowLayoutPanel1.Controls.Add(this.CorpNameLabel);
            this.flowLayoutPanel1.Controls.Add(this.AllianceNameLabel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(102, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(148, 36);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // CharacterNameLabel
            // 
            this.CharacterNameLabel.AutoSize = true;
            this.CharacterNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.CharacterNameLabel.Location = new System.Drawing.Point(3, 0);
            this.CharacterNameLabel.Name = "CharacterNameLabel";
            this.CharacterNameLabel.Size = new System.Drawing.Size(87, 12);
            this.CharacterNameLabel.TabIndex = 0;
            this.CharacterNameLabel.Text = "Character Name";
            // 
            // CorpNameLabel
            // 
            this.CorpNameLabel.AutoSize = true;
            this.CorpNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.CorpNameLabel.Location = new System.Drawing.Point(3, 12);
            this.CorpNameLabel.Name = "CorpNameLabel";
            this.CorpNameLabel.Size = new System.Drawing.Size(96, 12);
            this.CorpNameLabel.TabIndex = 1;
            this.CorpNameLabel.Text = "Corporation Name";
            // 
            // AllianceNameLabel
            // 
            this.AllianceNameLabel.AutoSize = true;
            this.AllianceNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.AllianceNameLabel.Location = new System.Drawing.Point(3, 24);
            this.AllianceNameLabel.Name = "AllianceNameLabel";
            this.AllianceNameLabel.Size = new System.Drawing.Size(78, 12);
            this.AllianceNameLabel.TabIndex = 2;
            this.AllianceNameLabel.Text = "Alliance Name";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.DamageDoneLabel);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(102, 36);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(148, 35);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // DamageDoneLabel
            // 
            this.DamageDoneLabel.AutoSize = true;
            this.DamageDoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.DamageDoneLabel.Location = new System.Drawing.Point(3, 19);
            this.DamageDoneLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.DamageDoneLabel.Name = "DamageDoneLabel";
            this.DamageDoneLabel.Size = new System.Drawing.Size(56, 13);
            this.DamageDoneLabel.TabIndex = 0;
            this.DamageDoneLabel.Text = "{0} ({1:P1})";
            // 
            // KillReportAttacker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "KillReportAttacker";
            this.Size = new System.Drawing.Size(250, 71);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShipPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeaponPictureBox)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox CharacterPictureBox;
        private System.Windows.Forms.PictureBox ShipPictureBox;
        private System.Windows.Forms.PictureBox WeaponPictureBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label CharacterNameLabel;
        private System.Windows.Forms.Label CorpNameLabel;
        private System.Windows.Forms.Label AllianceNameLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label DamageDoneLabel;
    }
}
