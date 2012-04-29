namespace EVEMon.SettingsUI
{
    partial class MarketUnifiedUploaderControl
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
            this.EndpointsGroupBox = new System.Windows.Forms.GroupBox();
            this.EndpointsPanel = new System.Windows.Forms.Panel();
            this.EndPointsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.NoEndPointsLabel = new System.Windows.Forms.Label();
            this.ProgressGroupBox = new System.Windows.Forms.GroupBox();
            this.ProgressTextBox = new System.Windows.Forms.TextBox();
            this.EndpointsGroupBox.SuspendLayout();
            this.EndpointsPanel.SuspendLayout();
            this.ProgressGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // EndpointsGroupBox
            // 
            this.EndpointsGroupBox.Controls.Add(this.EndpointsPanel);
            this.EndpointsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.EndpointsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.EndpointsGroupBox.Name = "EndpointsGroupBox";
            this.EndpointsGroupBox.Size = new System.Drawing.Size(420, 113);
            this.EndpointsGroupBox.TabIndex = 0;
            this.EndpointsGroupBox.TabStop = false;
            this.EndpointsGroupBox.Text = "EndPoints";
            // 
            // EndpointsPanel
            // 
            this.EndpointsPanel.Controls.Add(this.EndPointsCheckedListBox);
            this.EndpointsPanel.Controls.Add(this.NoEndPointsLabel);
            this.EndpointsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EndpointsPanel.Location = new System.Drawing.Point(3, 16);
            this.EndpointsPanel.Name = "EndpointsPanel";
            this.EndpointsPanel.Size = new System.Drawing.Size(414, 94);
            this.EndpointsPanel.TabIndex = 3;
            // 
            // EndPointsCheckedListBox
            // 
            this.EndPointsCheckedListBox.CheckOnClick = true;
            this.EndPointsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EndPointsCheckedListBox.Location = new System.Drawing.Point(0, 0);
            this.EndPointsCheckedListBox.Name = "EndPointsCheckedListBox";
            this.EndPointsCheckedListBox.Size = new System.Drawing.Size(414, 94);
            this.EndPointsCheckedListBox.TabIndex = 1;
            // 
            // NoEndPointsLabel
            // 
            this.NoEndPointsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NoEndPointsLabel.Location = new System.Drawing.Point(0, 0);
            this.NoEndPointsLabel.Name = "NoEndPointsLabel";
            this.NoEndPointsLabel.Size = new System.Drawing.Size(414, 94);
            this.NoEndPointsLabel.TabIndex = 0;
            this.NoEndPointsLabel.Text = "Enable the uploader and click \"Apply\" to fetch online endpoints.";
            this.NoEndPointsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressGroupBox
            // 
            this.ProgressGroupBox.Controls.Add(this.ProgressTextBox);
            this.ProgressGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressGroupBox.Location = new System.Drawing.Point(0, 113);
            this.ProgressGroupBox.Name = "ProgressGroupBox";
            this.ProgressGroupBox.Size = new System.Drawing.Size(420, 247);
            this.ProgressGroupBox.TabIndex = 1;
            this.ProgressGroupBox.TabStop = false;
            this.ProgressGroupBox.Text = "Progress";
            // 
            // ProgressTextBox
            // 
            this.ProgressTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressTextBox.Location = new System.Drawing.Point(3, 16);
            this.ProgressTextBox.Multiline = true;
            this.ProgressTextBox.Name = "ProgressTextBox";
            this.ProgressTextBox.ReadOnly = true;
            this.ProgressTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ProgressTextBox.Size = new System.Drawing.Size(414, 228);
            this.ProgressTextBox.TabIndex = 0;
            // 
            // MarketUnifiedUploaderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ProgressGroupBox);
            this.Controls.Add(this.EndpointsGroupBox);
            this.Name = "MarketUnifiedUploaderControl";
            this.Size = new System.Drawing.Size(420, 360);
            this.EndpointsGroupBox.ResumeLayout(false);
            this.EndpointsPanel.ResumeLayout(false);
            this.ProgressGroupBox.ResumeLayout(false);
            this.ProgressGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox EndpointsGroupBox;
        private System.Windows.Forms.GroupBox ProgressGroupBox;
        private System.Windows.Forms.Panel EndpointsPanel;
        private System.Windows.Forms.Label NoEndPointsLabel;
        private System.Windows.Forms.TextBox ProgressTextBox;
        private System.Windows.Forms.CheckedListBox EndPointsCheckedListBox;
    }
}
