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
            this.LocalhostGroupBox = new System.Windows.Forms.GroupBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.LocalhostComboBox = new System.Windows.Forms.ComboBox();
            this.EndpointsGroupBox.SuspendLayout();
            this.EndpointsPanel.SuspendLayout();
            this.ProgressGroupBox.SuspendLayout();
            this.LocalhostGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // EndpointsGroupBox
            // 
            this.EndpointsGroupBox.Controls.Add(this.EndpointsPanel);
            this.EndpointsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.EndpointsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.EndpointsGroupBox.Name = "EndpointsGroupBox";
            this.EndpointsGroupBox.Size = new System.Drawing.Size(420, 100);
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
            this.EndpointsPanel.Size = new System.Drawing.Size(414, 81);
            this.EndpointsPanel.TabIndex = 3;
            // 
            // EndPointsCheckedListBox
            // 
            this.EndPointsCheckedListBox.CheckOnClick = true;
            this.EndPointsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EndPointsCheckedListBox.Location = new System.Drawing.Point(0, 0);
            this.EndPointsCheckedListBox.Name = "EndPointsCheckedListBox";
            this.EndPointsCheckedListBox.Size = new System.Drawing.Size(414, 81);
            this.EndPointsCheckedListBox.TabIndex = 1;
            // 
            // NoEndPointsLabel
            // 
            this.NoEndPointsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NoEndPointsLabel.Location = new System.Drawing.Point(0, 0);
            this.NoEndPointsLabel.Name = "NoEndPointsLabel";
            this.NoEndPointsLabel.Size = new System.Drawing.Size(414, 81);
            this.NoEndPointsLabel.TabIndex = 0;
            this.NoEndPointsLabel.Text = "Enable the uploader and click \"Apply\" to fetch online endpoints.";
            this.NoEndPointsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressGroupBox
            // 
            this.ProgressGroupBox.Controls.Add(this.ProgressTextBox);
            this.ProgressGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressGroupBox.Location = new System.Drawing.Point(0, 179);
            this.ProgressGroupBox.Name = "ProgressGroupBox";
            this.ProgressGroupBox.Size = new System.Drawing.Size(420, 181);
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
            this.ProgressTextBox.Size = new System.Drawing.Size(414, 162);
            this.ProgressTextBox.TabIndex = 0;
            // 
            // LocalhostGroupBox
            // 
            this.LocalhostGroupBox.Controls.Add(this.DeleteButton);
            this.LocalhostGroupBox.Controls.Add(this.EditButton);
            this.LocalhostGroupBox.Controls.Add(this.AddButton);
            this.LocalhostGroupBox.Controls.Add(this.LocalhostComboBox);
            this.LocalhostGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.LocalhostGroupBox.Location = new System.Drawing.Point(0, 100);
            this.LocalhostGroupBox.Name = "LocalhostGroupBox";
            this.LocalhostGroupBox.Size = new System.Drawing.Size(420, 79);
            this.LocalhostGroupBox.TabIndex = 2;
            this.LocalhostGroupBox.TabStop = false;
            this.LocalhostGroupBox.Text = "Localhosts";
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(171, 48);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteButton.TabIndex = 3;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // EditButton
            // 
            this.EditButton.Location = new System.Drawing.Point(89, 48);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(75, 23);
            this.EditButton.TabIndex = 2;
            this.EditButton.Text = "Edit";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(7, 48);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 1;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // LocalhostComboBox
            // 
            this.LocalhostComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LocalhostComboBox.FormattingEnabled = true;
            this.LocalhostComboBox.Location = new System.Drawing.Point(7, 20);
            this.LocalhostComboBox.Name = "LocalhostComboBox";
            this.LocalhostComboBox.Size = new System.Drawing.Size(239, 21);
            this.LocalhostComboBox.TabIndex = 0;
            // 
            // MarketUnifiedUploaderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ProgressGroupBox);
            this.Controls.Add(this.LocalhostGroupBox);
            this.Controls.Add(this.EndpointsGroupBox);
            this.Name = "MarketUnifiedUploaderControl";
            this.Size = new System.Drawing.Size(420, 360);
            this.EndpointsGroupBox.ResumeLayout(false);
            this.EndpointsPanel.ResumeLayout(false);
            this.ProgressGroupBox.ResumeLayout(false);
            this.ProgressGroupBox.PerformLayout();
            this.LocalhostGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox EndpointsGroupBox;
        private System.Windows.Forms.GroupBox ProgressGroupBox;
        private System.Windows.Forms.Panel EndpointsPanel;
        private System.Windows.Forms.Label NoEndPointsLabel;
        private System.Windows.Forms.TextBox ProgressTextBox;
        private System.Windows.Forms.CheckedListBox EndPointsCheckedListBox;
        private System.Windows.Forms.GroupBox LocalhostGroupBox;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.ComboBox LocalhostComboBox;
    }
}
