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
            this.errorLabel = new System.Windows.Forms.Label();
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.errorTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.detailsPanel = new EVEMon.Controls.BorderPanel();
            this.detailsTextBox = new System.Windows.Forms.TextBox();
            this.TroubleshooterPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            this.errorTableLayoutPanel.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorLabel.Location = new System.Drawing.Point(42, 0);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(481, 39);
            this.errorLabel.TabIndex = 1;
            this.errorLabel.Text = "Error details.";
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconPictureBox.Image = global::EVEMon.Properties.Resources.Error32;
            this.iconPictureBox.Location = new System.Drawing.Point(3, 3);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(33, 33);
            this.iconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconPictureBox.TabIndex = 0;
            this.iconPictureBox.TabStop = false;
            // 
            // errorTableLayoutPanel
            // 
            this.errorTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.errorTableLayoutPanel.AutoSize = true;
            this.errorTableLayoutPanel.ColumnCount = 2;
            this.errorTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.414449F));
            this.errorTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 92.58555F));
            this.errorTableLayoutPanel.Controls.Add(this.errorLabel, 1, 0);
            this.errorTableLayoutPanel.Controls.Add(this.iconPictureBox, 0, 0);
            this.errorTableLayoutPanel.Location = new System.Drawing.Point(13, 12);
            this.errorTableLayoutPanel.Name = "errorTableLayoutPanel";
            this.errorTableLayoutPanel.RowCount = 1;
            this.errorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.errorTableLayoutPanel.Size = new System.Drawing.Size(526, 39);
            this.errorTableLayoutPanel.TabIndex = 3;
            // 
            // detailsPanel
            // 
            this.detailsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsPanel.BackColor = System.Drawing.SystemColors.Window;
            this.detailsPanel.Controls.Add(this.detailsTextBox);
            this.detailsPanel.Controls.Add(this.TroubleshooterPanel);
            this.detailsPanel.ForeColor = System.Drawing.Color.Gray;
            this.detailsPanel.Location = new System.Drawing.Point(13, 66);
            this.detailsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new System.Windows.Forms.Padding(3);
            this.detailsPanel.Size = new System.Drawing.Size(526, 366);
            this.detailsPanel.TabIndex = 2;
            // 
            // detailsTextBox
            // 
            this.detailsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.detailsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.detailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsTextBox.Location = new System.Drawing.Point(3, 3);
            this.detailsTextBox.Multiline = true;
            this.detailsTextBox.Name = "detailsTextBox";
            this.detailsTextBox.ReadOnly = true;
            this.detailsTextBox.Size = new System.Drawing.Size(520, 360);
            this.detailsTextBox.TabIndex = 3;
            // 
            // TroubleshooterPanel
            // 
            this.TroubleshooterPanel.AutoSize = true;
            this.TroubleshooterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TroubleshooterPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.TroubleshooterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TroubleshooterPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.TroubleshooterPanel.Location = new System.Drawing.Point(3, 3);
            this.TroubleshooterPanel.Name = "TroubleshooterPanel";
            this.TroubleshooterPanel.Size = new System.Drawing.Size(520, 0);
            this.TroubleshooterPanel.TabIndex = 2;
            // 
            // APIErrorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(551, 441);
            this.Controls.Add(this.errorTableLayoutPanel);
            this.Controls.Add(this.detailsPanel);
            this.Name = "APIErrorWindow";
            this.Text = "API Error Details";
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            this.errorTableLayoutPanel.ResumeLayout(false);
            this.errorTableLayoutPanel.PerformLayout();
            this.detailsPanel.ResumeLayout(false);
            this.detailsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.PictureBox iconPictureBox;
        private EVEMon.Controls.BorderPanel detailsPanel;
        private System.Windows.Forms.TableLayoutPanel errorTableLayoutPanel;
        private System.Windows.Forms.TextBox detailsTextBox;
        private System.Windows.Forms.Panel TroubleshooterPanel;
    }
}