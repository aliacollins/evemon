namespace EVEMon
{
    partial class AboutWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            this.DevContribListLabelLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.DevContribLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.DevelopersList = new System.Windows.Forms.ListBox();
            this.LegalLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.HeaderLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.VerCopyLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.EveMonLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.HomePageLinkLabel = new System.Windows.Forms.LinkLabel();
            this.GplLabel = new System.Windows.Forms.Label();
            this.BodyLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ContribLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AuthorsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ContinuedByLabel = new System.Windows.Forms.Label();
            this.CreatedByLabel = new System.Windows.Forms.Label();
            this.DonationsLabel = new System.Windows.Forms.Label();
            this.OkButton = new System.Windows.Forms.Button();
            this.DevContribLabel = new System.Windows.Forms.Label();
            this.DevContribListLabelLayoutPanel.SuspendLayout();
            this.DevContribLayoutPanel.SuspendLayout();
            this.LegalLayoutPanel.SuspendLayout();
            this.HeaderLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.VerCopyLayoutPanel.SuspendLayout();
            this.BodyLayoutPanel.SuspendLayout();
            this.ContribLayoutPanel.SuspendLayout();
            this.AuthorsLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // DevContribListLabelLayoutPanel
            // 
            this.DevContribListLabelLayoutPanel.AutoSize = true;
            this.DevContribListLabelLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DevContribListLabelLayoutPanel.Controls.Add(this.DevContribLayoutPanel);
            this.DevContribListLabelLayoutPanel.Controls.Add(this.DevelopersList);
            this.DevContribListLabelLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DevContribListLabelLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.DevContribListLabelLayoutPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DevContribListLabelLayoutPanel.Location = new System.Drawing.Point(299, 3);
            this.DevContribListLabelLayoutPanel.Name = "DevContribListLabelLayoutPanel";
            this.DevContribListLabelLayoutPanel.Size = new System.Drawing.Size(203, 392);
            this.DevContribListLabelLayoutPanel.TabIndex = 11;
            // 
            // DevContribLayoutPanel
            // 
            this.DevContribLayoutPanel.AutoSize = true;
            this.DevContribLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DevContribLayoutPanel.Controls.Add(this.DevContribLabel);
            this.DevContribLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.DevContribLayoutPanel.Name = "DevContribLayoutPanel";
            this.DevContribLayoutPanel.Size = new System.Drawing.Size(175, 13);
            this.DevContribLayoutPanel.TabIndex = 1;
            // 
            // DevelopersList
            // 
            this.DevelopersList.BackColor = System.Drawing.SystemColors.Menu;
            this.DevelopersList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DevelopersList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DevelopersList.ForeColor = System.Drawing.SystemColors.WindowText;
            this.DevelopersList.FormattingEnabled = true;
            this.DevelopersList.Location = new System.Drawing.Point(2, 21);
            this.DevelopersList.Margin = new System.Windows.Forms.Padding(2);
            this.DevelopersList.Name = "DevelopersList";
            this.DevelopersList.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.DevelopersList.Size = new System.Drawing.Size(198, 327);
            this.DevelopersList.Sorted = true;
            this.DevelopersList.TabIndex = 2;
            this.DevelopersList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstDevelopers_DrawItem);
            // 
            // LegalLayoutPanel
            // 
            this.LegalLayoutPanel.AutoSize = true;
            this.LegalLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LegalLayoutPanel.Controls.Add(this.HeaderLayoutPanel);
            this.LegalLayoutPanel.Controls.Add(this.GplLabel);
            this.LegalLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.LegalLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.LegalLayoutPanel.Name = "LegalLayoutPanel";
            this.LegalLayoutPanel.Size = new System.Drawing.Size(290, 392);
            this.LegalLayoutPanel.TabIndex = 10;
            // 
            // HeaderLayoutPanel
            // 
            this.HeaderLayoutPanel.AutoSize = true;
            this.HeaderLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HeaderLayoutPanel.Controls.Add(this.LogoPictureBox);
            this.HeaderLayoutPanel.Controls.Add(this.VerCopyLayoutPanel);
            this.HeaderLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HeaderLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.HeaderLayoutPanel.Name = "HeaderLayoutPanel";
            this.HeaderLayoutPanel.Size = new System.Drawing.Size(294, 102);
            this.HeaderLayoutPanel.TabIndex = 0;
            this.HeaderLayoutPanel.WrapContents = false;
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("LogoPictureBox.Image")));
            this.LogoPictureBox.Location = new System.Drawing.Point(0, 0);
            this.LogoPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(100, 100);
            this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.LogoPictureBox.TabIndex = 7;
            this.LogoPictureBox.TabStop = false;
            // 
            // VerCopyLayoutPanel
            // 
            this.VerCopyLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.VerCopyLayoutPanel.AutoSize = true;
            this.VerCopyLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.VerCopyLayoutPanel.Controls.Add(this.EveMonLabel);
            this.VerCopyLayoutPanel.Controls.Add(this.VersionLabel);
            this.VerCopyLayoutPanel.Controls.Add(this.CopyrightLabel);
            this.VerCopyLayoutPanel.Controls.Add(this.HomePageLinkLabel);
            this.VerCopyLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.VerCopyLayoutPanel.Location = new System.Drawing.Point(100, 0);
            this.VerCopyLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.VerCopyLayoutPanel.Name = "VerCopyLayoutPanel";
            this.VerCopyLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 25, 0, 25);
            this.VerCopyLayoutPanel.Size = new System.Drawing.Size(170, 102);
            this.VerCopyLayoutPanel.TabIndex = 8;
            this.VerCopyLayoutPanel.WrapContents = false;
            // 
            // EveMonLabel
            // 
            this.EveMonLabel.AutoSize = true;
            this.EveMonLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EveMonLabel.Location = new System.Drawing.Point(3, 25);
            this.EveMonLabel.Name = "EveMonLabel";
            this.EveMonLabel.Size = new System.Drawing.Size(50, 13);
            this.EveMonLabel.TabIndex = 0;
            this.EveMonLabel.Text = "EVEMon";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(3, 38);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(61, 13);
            this.VersionLabel.TabIndex = 1;
            this.VersionLabel.Text = "Version {0}";
            // 
            // CopyrightLabel
            // 
            this.CopyrightLabel.AutoSize = true;
            this.CopyrightLabel.Location = new System.Drawing.Point(3, 51);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(164, 13);
            this.CopyrightLabel.TabIndex = 5;
            this.CopyrightLabel.Text = "Copyright © 2006  Timothy Fries";
            // 
            // HomePageLinkLabel
            // 
            this.HomePageLinkLabel.AutoSize = true;
            this.HomePageLinkLabel.Location = new System.Drawing.Point(3, 64);
            this.HomePageLinkLabel.Name = "HomePageLinkLabel";
            this.HomePageLinkLabel.Size = new System.Drawing.Size(158, 13);
            this.HomePageLinkLabel.TabIndex = 0;
            this.HomePageLinkLabel.TabStop = true;
            this.HomePageLinkLabel.Text = "http://evemon.battleclinic.com/";
            this.HomePageLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llHomePage_LinkClicked);
            // 
            // GplLabel
            // 
            this.GplLabel.Location = new System.Drawing.Point(3, 111);
            this.GplLabel.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.GplLabel.Name = "GplLabel";
            this.GplLabel.Size = new System.Drawing.Size(288, 281);
            this.GplLabel.TabIndex = 6;
            this.GplLabel.Text = resources.GetString("GplLabel.Text");
            // 
            // BodyLayoutPanel
            // 
            this.BodyLayoutPanel.AutoSize = true;
            this.BodyLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BodyLayoutPanel.ColumnCount = 2;
            this.BodyLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.81188F));
            this.BodyLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.18812F));
            this.BodyLayoutPanel.Controls.Add(this.LegalLayoutPanel, 0, 0);
            this.BodyLayoutPanel.Controls.Add(this.ContribLayoutPanel, 0, 1);
            this.BodyLayoutPanel.Controls.Add(this.DevContribListLabelLayoutPanel, 1, 0);
            this.BodyLayoutPanel.Controls.Add(this.OkButton, 1, 1);
            this.BodyLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BodyLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.BodyLayoutPanel.Name = "BodyLayoutPanel";
            this.BodyLayoutPanel.RowCount = 2;
            this.BodyLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.BodyLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.BodyLayoutPanel.Size = new System.Drawing.Size(505, 454);
            this.BodyLayoutPanel.TabIndex = 11;
            // 
            // ContribLayoutPanel
            // 
            this.ContribLayoutPanel.AutoSize = true;
            this.ContribLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ContribLayoutPanel.ColumnCount = 2;
            this.ContribLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ContribLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ContribLayoutPanel.Controls.Add(this.AuthorsLayoutPanel, 0, 0);
            this.ContribLayoutPanel.Location = new System.Drawing.Point(0, 407);
            this.ContribLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this.ContribLayoutPanel.Name = "ContribLayoutPanel";
            this.ContribLayoutPanel.RowCount = 1;
            this.ContribLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ContribLayoutPanel.Size = new System.Drawing.Size(296, 39);
            this.ContribLayoutPanel.TabIndex = 0;
            // 
            // AuthorsLayoutPanel
            // 
            this.AuthorsLayoutPanel.AutoSize = true;
            this.AuthorsLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AuthorsLayoutPanel.Controls.Add(this.ContinuedByLabel);
            this.AuthorsLayoutPanel.Controls.Add(this.CreatedByLabel);
            this.AuthorsLayoutPanel.Controls.Add(this.DonationsLabel);
            this.AuthorsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthorsLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.AuthorsLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.AuthorsLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.AuthorsLayoutPanel.Name = "AuthorsLayoutPanel";
            this.AuthorsLayoutPanel.Size = new System.Drawing.Size(298, 39);
            this.AuthorsLayoutPanel.TabIndex = 0;
            this.AuthorsLayoutPanel.WrapContents = false;
            // 
            // ContinuedByLabel
            // 
            this.ContinuedByLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ContinuedByLabel.AutoSize = true;
            this.ContinuedByLabel.Location = new System.Drawing.Point(3, 0);
            this.ContinuedByLabel.Name = "ContinuedByLabel";
            this.ContinuedByLabel.Size = new System.Drawing.Size(292, 13);
            this.ContinuedByLabel.TabIndex = 3;
            this.ContinuedByLabel.Text = "Continued by Anders Chydenius and the listed Contributors";
            // 
            // CreatedByLabel
            // 
            this.CreatedByLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CreatedByLabel.AutoSize = true;
            this.CreatedByLabel.Location = new System.Drawing.Point(3, 13);
            this.CreatedByLabel.Name = "CreatedByLabel";
            this.CreatedByLabel.Size = new System.Drawing.Size(214, 13);
            this.CreatedByLabel.TabIndex = 2;
            this.CreatedByLabel.Text = "Originally created by Six Anari of Goonfleet";
            // 
            // DonationsLabel
            // 
            this.DonationsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DonationsLabel.AutoSize = true;
            this.DonationsLabel.Location = new System.Drawing.Point(3, 26);
            this.DonationsLabel.Name = "DonationsLabel";
            this.DonationsLabel.Size = new System.Drawing.Size(217, 13);
            this.DonationsLabel.TabIndex = 4;
            this.DonationsLabel.Text = "Donations of ISK are always appreciated. ;)";
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.Location = new System.Drawing.Point(424, 425);
            this.OkButton.Margin = new System.Windows.Forms.Padding(6);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 0;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // DevContribLabel
            // 
            this.DevContribLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.DevContribLabel.AutoSize = true;
            this.DevContribLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DevContribLabel.Location = new System.Drawing.Point(3, 0);
            this.DevContribLabel.Name = "DevContribLabel";
            this.DevContribLabel.Size = new System.Drawing.Size(169, 13);
            this.DevContribLabel.TabIndex = 0;
            this.DevContribLabel.Text = "Developers and Contributors";
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(523, 472);
            this.Controls.Add(this.BodyLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About EVEMon";
            this.Load += new System.EventHandler(this.AboutWindow_Load);
            this.DevContribListLabelLayoutPanel.ResumeLayout(false);
            this.DevContribListLabelLayoutPanel.PerformLayout();
            this.DevContribLayoutPanel.ResumeLayout(false);
            this.DevContribLayoutPanel.PerformLayout();
            this.LegalLayoutPanel.ResumeLayout(false);
            this.LegalLayoutPanel.PerformLayout();
            this.HeaderLayoutPanel.ResumeLayout(false);
            this.HeaderLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.VerCopyLayoutPanel.ResumeLayout(false);
            this.VerCopyLayoutPanel.PerformLayout();
            this.BodyLayoutPanel.ResumeLayout(false);
            this.BodyLayoutPanel.PerformLayout();
            this.ContribLayoutPanel.ResumeLayout(false);
            this.ContribLayoutPanel.PerformLayout();
            this.AuthorsLayoutPanel.ResumeLayout(false);
            this.AuthorsLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel DevContribListLabelLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel DevContribLayoutPanel;
        private System.Windows.Forms.ListBox DevelopersList;
        private System.Windows.Forms.FlowLayoutPanel LegalLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel HeaderLayoutPanel;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.FlowLayoutPanel VerCopyLayoutPanel;
        private System.Windows.Forms.Label EveMonLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label CopyrightLabel;
        private System.Windows.Forms.LinkLabel HomePageLinkLabel;
        private System.Windows.Forms.Label GplLabel;
        private System.Windows.Forms.TableLayoutPanel BodyLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel ContribLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel AuthorsLayoutPanel;
        private System.Windows.Forms.Label ContinuedByLabel;
        private System.Windows.Forms.Label CreatedByLabel;
        private System.Windows.Forms.Label DonationsLabel;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label DevContribLabel;

    }
}
