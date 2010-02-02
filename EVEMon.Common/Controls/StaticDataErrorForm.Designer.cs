namespace EVEMon.Common.Controls
{
    partial class StaticDataErrorForm
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
            this.warningIcon = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.errorMessageTextBox = new System.Windows.Forms.TextBox();
            this.copyToClipboardLinkLabel = new System.Windows.Forms.LinkLabel();
            this.autoupdateDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.enableAutoUpdateButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.warningIcon)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.autoupdateDetailsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // warningIcon
            // 
            this.warningIcon.Location = new System.Drawing.Point(3, 5);
            this.warningIcon.Name = "warningIcon";
            this.warningIcon.Size = new System.Drawing.Size(32, 32);
            this.warningIcon.TabIndex = 2;
            this.warningIcon.TabStop = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.panel2);
            this.flowLayoutPanel2.Controls.Add(this.groupBox1);
            this.flowLayoutPanel2.Controls.Add(this.autoupdateDetailsGroupBox);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(352, 269);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.warningIcon);
            this.panel2.Location = new System.Drawing.Point(3, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(343, 42);
            this.panel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(41, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(273, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "EVEMon has encountered an internal data error";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.errorMessageTextBox);
            this.groupBox1.Controls.Add(this.copyToClipboardLinkLabel);
            this.groupBox1.Location = new System.Drawing.Point(6, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 132);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // errorMessageTextBox
            // 
            this.errorMessageTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.errorMessageTextBox.Location = new System.Drawing.Point(9, 20);
            this.errorMessageTextBox.Multiline = true;
            this.errorMessageTextBox.Name = "errorMessageTextBox";
            this.errorMessageTextBox.ReadOnly = true;
            this.errorMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorMessageTextBox.Size = new System.Drawing.Size(325, 89);
            this.errorMessageTextBox.TabIndex = 1;
            // 
            // copyToClipboardLinkLabel
            // 
            this.copyToClipboardLinkLabel.AutoSize = true;
            this.copyToClipboardLinkLabel.Location = new System.Drawing.Point(241, 112);
            this.copyToClipboardLinkLabel.Name = "copyToClipboardLinkLabel";
            this.copyToClipboardLinkLabel.Size = new System.Drawing.Size(93, 13);
            this.copyToClipboardLinkLabel.TabIndex = 2;
            this.copyToClipboardLinkLabel.TabStop = true;
            this.copyToClipboardLinkLabel.Text = "Copy to Clipboard";
            this.copyToClipboardLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.copyToClipboardLinkLabel_LinkClicked);
            // 
            // autoupdateDetailsGroupBox
            // 
            this.autoupdateDetailsGroupBox.Controls.Add(this.label2);
            this.autoupdateDetailsGroupBox.Controls.Add(this.enableAutoUpdateButton);
            this.autoupdateDetailsGroupBox.Location = new System.Drawing.Point(6, 183);
            this.autoupdateDetailsGroupBox.Name = "autoupdateDetailsGroupBox";
            this.autoupdateDetailsGroupBox.Size = new System.Drawing.Size(340, 83);
            this.autoupdateDetailsGroupBox.TabIndex = 2;
            this.autoupdateDetailsGroupBox.TabStop = false;
            this.autoupdateDetailsGroupBox.Text = "Possible Cause";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(316, 33);
            this.label2.TabIndex = 1;
            this.label2.Text = "The AutoUpdate feature is currently disabled. EVEMon datafiles may be out of date" +
                ".";
            // 
            // enableAutoUpdateButton
            // 
            this.enableAutoUpdateButton.AutoSize = true;
            this.enableAutoUpdateButton.Location = new System.Drawing.Point(9, 53);
            this.enableAutoUpdateButton.Name = "enableAutoUpdateButton";
            this.enableAutoUpdateButton.Size = new System.Drawing.Size(188, 23);
            this.enableAutoUpdateButton.TabIndex = 2;
            this.enableAutoUpdateButton.Text = "Enable AutoUpdate (recommended)";
            this.enableAutoUpdateButton.UseVisualStyleBackColor = true;
            this.enableAutoUpdateButton.Click += new System.EventHandler(this.enableAutoUpdateButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(268, 278);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // StaticDataErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(355, 309);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.flowLayoutPanel2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StaticDataErrorForm";
            this.Text = "EVEMon Data Error";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.warningIcon)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.autoupdateDetailsGroupBox.ResumeLayout(false);
            this.autoupdateDetailsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox warningIcon;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox errorMessageTextBox;
        private System.Windows.Forms.LinkLabel copyToClipboardLinkLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox autoupdateDetailsGroupBox;
        private System.Windows.Forms.Button enableAutoUpdateButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button closeButton;
    }
}