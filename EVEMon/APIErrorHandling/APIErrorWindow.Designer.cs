namespace EVEMon.ApiErrorHandling
{
    partial class APIErrorWindow
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
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.IconPictureBox = new System.Windows.Forms.PictureBox();
            this.ErrorTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.DetailsPanel = new EVEMon.Controls.BorderPanel();
            this.DetailsTextBox = new System.Windows.Forms.TextBox();
            this.TroubleshooterPanel = new System.Windows.Forms.Panel();
            this.CopyToClipboardLinkLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.IconPictureBox)).BeginInit();
            this.ErrorTableLayoutPanel.SuspendLayout();
            this.DetailsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.AutoSize = true;
            this.ErrorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorLabel.Location = new System.Drawing.Point(43, 0);
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(480, 39);
            this.ErrorLabel.TabIndex = 1;
            this.ErrorLabel.Text = "Error details.";
            // 
            // IconPictureBox
            // 
            this.IconPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IconPictureBox.Image = global::EVEMon.Properties.Resources.Error32;
            this.IconPictureBox.Location = new System.Drawing.Point(3, 3);
            this.IconPictureBox.Name = "IconPictureBox";
            this.IconPictureBox.Size = new System.Drawing.Size(34, 33);
            this.IconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.IconPictureBox.TabIndex = 0;
            this.IconPictureBox.TabStop = false;
            // 
            // ErrorTableLayoutPanel
            // 
            this.ErrorTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorTableLayoutPanel.AutoSize = true;
            this.ErrorTableLayoutPanel.ColumnCount = 2;
            this.ErrorTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.ErrorTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ErrorTableLayoutPanel.Controls.Add(this.ErrorLabel, 1, 0);
            this.ErrorTableLayoutPanel.Controls.Add(this.IconPictureBox, 0, 0);
            this.ErrorTableLayoutPanel.Location = new System.Drawing.Point(13, 12);
            this.ErrorTableLayoutPanel.Name = "ErrorTableLayoutPanel";
            this.ErrorTableLayoutPanel.RowCount = 1;
            this.ErrorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ErrorTableLayoutPanel.Size = new System.Drawing.Size(526, 39);
            this.ErrorTableLayoutPanel.TabIndex = 3;
            // 
            // DetailsPanel
            // 
            this.DetailsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DetailsPanel.BackColor = System.Drawing.SystemColors.Window;
            this.DetailsPanel.Controls.Add(this.DetailsTextBox);
            this.DetailsPanel.Controls.Add(this.TroubleshooterPanel);
            this.DetailsPanel.ForeColor = System.Drawing.Color.Gray;
            this.DetailsPanel.Location = new System.Drawing.Point(9, 77);
            this.DetailsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.DetailsPanel.Name = "DetailsPanel";
            this.DetailsPanel.Padding = new System.Windows.Forms.Padding(10);
            this.DetailsPanel.Size = new System.Drawing.Size(533, 380);
            this.DetailsPanel.TabIndex = 2;
            // 
            // DetailsTextBox
            // 
            this.DetailsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.DetailsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DetailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DetailsTextBox.Location = new System.Drawing.Point(10, 10);
            this.DetailsTextBox.Multiline = true;
            this.DetailsTextBox.Name = "DetailsTextBox";
            this.DetailsTextBox.ReadOnly = true;
            this.DetailsTextBox.Size = new System.Drawing.Size(513, 360);
            this.DetailsTextBox.TabIndex = 3;
            this.DetailsTextBox.Text = "This text should not be seen.";
            // 
            // TroubleshooterPanel
            // 
            this.TroubleshooterPanel.AutoSize = true;
            this.TroubleshooterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TroubleshooterPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.TroubleshooterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TroubleshooterPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.TroubleshooterPanel.Location = new System.Drawing.Point(10, 10);
            this.TroubleshooterPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 13);
            this.TroubleshooterPanel.Name = "TroubleshooterPanel";
            this.TroubleshooterPanel.Size = new System.Drawing.Size(513, 0);
            this.TroubleshooterPanel.TabIndex = 2;
            this.TroubleshooterPanel.Visible = false;
            // 
            // CopyToClipboardLinkLabel
            // 
            this.CopyToClipboardLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CopyToClipboardLinkLabel.AutoSize = true;
            this.CopyToClipboardLinkLabel.Location = new System.Drawing.Point(449, 59);
            this.CopyToClipboardLinkLabel.Name = "CopyToClipboardLinkLabel";
            this.CopyToClipboardLinkLabel.Size = new System.Drawing.Size(93, 13);
            this.CopyToClipboardLinkLabel.TabIndex = 4;
            this.CopyToClipboardLinkLabel.TabStop = true;
            this.CopyToClipboardLinkLabel.Text = "Copy to Clipboard";
            this.CopyToClipboardLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CopyToClipboardLinkLabel_LinkClicked);
            // 
            // APIErrorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(551, 466);
            this.Controls.Add(this.CopyToClipboardLinkLabel);
            this.Controls.Add(this.ErrorTableLayoutPanel);
            this.Controls.Add(this.DetailsPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "APIErrorWindow";
            this.Text = "API Error Details";
            ((System.ComponentModel.ISupportInitialize)(this.IconPictureBox)).EndInit();
            this.ErrorTableLayoutPanel.ResumeLayout(false);
            this.ErrorTableLayoutPanel.PerformLayout();
            this.DetailsPanel.ResumeLayout(false);
            this.DetailsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.PictureBox IconPictureBox;
        private EVEMon.Controls.BorderPanel DetailsPanel;
        private System.Windows.Forms.TableLayoutPanel ErrorTableLayoutPanel;
        private System.Windows.Forms.TextBox DetailsTextBox;
        private System.Windows.Forms.Panel TroubleshooterPanel;
        private System.Windows.Forms.LinkLabel CopyToClipboardLinkLabel;
    }
}