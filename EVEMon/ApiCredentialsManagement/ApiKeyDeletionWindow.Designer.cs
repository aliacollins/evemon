namespace EVEMon.ApiCredentialsManagement
{
    partial class ApiKeyDeletionWindow
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
            System.Windows.Forms.Label deletionLabel;
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("John Doe");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Mary Jane");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApiKeyDeletionWindow));
            this.cancelButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.charactersListView = new System.Windows.Forms.ListView();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.deleteWarningLabel = new System.Windows.Forms.Label();
            this.charactersListGroupBox = new System.Windows.Forms.GroupBox();
            deletionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.charactersListGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // deletionLabel
            // 
            deletionLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            deletionLabel.Location = new System.Drawing.Point(51, 13);
            deletionLabel.Name = "deletionLabel";
            deletionLabel.Size = new System.Drawing.Size(376, 32);
            deletionLabel.TabIndex = 4;
            deletionLabel.Text = "You are about to delete an API key.";
            deletionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(435, 180);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.deleteButton.Location = new System.Drawing.Point(354, 180);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // charactersListView
            // 
            this.charactersListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.charactersListView.CheckBoxes = true;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            this.charactersListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.charactersListView.Location = new System.Drawing.Point(308, 28);
            this.charactersListView.Name = "charactersListView";
            this.charactersListView.Size = new System.Drawing.Size(181, 77);
            this.charactersListView.TabIndex = 2;
            this.charactersListView.UseCompatibleStateImageBehavior = false;
            this.charactersListView.View = System.Windows.Forms.View.List;
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(13, 13);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(32, 32);
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            this.infoLabel.Location = new System.Drawing.Point(6, 28);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(296, 48);
            this.infoLabel.TabIndex = 6;
            this.infoLabel.Text = "If you choose to leave your characters, they will be displayed as cached and auto" +
                "matically reconnected once they\'re found on another one of your API keys.";
            // 
            // deleteWarningLabel
            // 
            this.deleteWarningLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteWarningLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.deleteWarningLabel.Location = new System.Drawing.Point(6, 90);
            this.deleteWarningLabel.Name = "deleteWarningLabel";
            this.deleteWarningLabel.Size = new System.Drawing.Size(304, 15);
            this.deleteWarningLabel.TabIndex = 7;
            this.deleteWarningLabel.Text = "Removing characters means losing all their data and plans!";
            // 
            // charactersListGroupBox
            // 
            this.charactersListGroupBox.Controls.Add(this.deleteWarningLabel);
            this.charactersListGroupBox.Controls.Add(this.infoLabel);
            this.charactersListGroupBox.Controls.Add(this.charactersListView);
            this.charactersListGroupBox.Location = new System.Drawing.Point(15, 60);
            this.charactersListGroupBox.Name = "charactersListGroupBox";
            this.charactersListGroupBox.Size = new System.Drawing.Size(495, 114);
            this.charactersListGroupBox.TabIndex = 8;
            this.charactersListGroupBox.TabStop = false;
            this.charactersListGroupBox.Text = "Select the characters you want to remove";
            // 
            // ApiKeyDeletionWindow
            // 
            this.AcceptButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(522, 215);
            this.Controls.Add(this.charactersListGroupBox);
            this.Controls.Add(deletionLabel);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.cancelButton);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "ApiKeyDeletionWindow";
            this.Text = "Delete an API key";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.charactersListGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ListView charactersListView;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label deleteWarningLabel;
        private System.Windows.Forms.GroupBox charactersListGroupBox;
    }
}