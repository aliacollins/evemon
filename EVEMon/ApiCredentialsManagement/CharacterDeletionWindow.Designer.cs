namespace EVEMon.ApiCredentialsManagement
{
    partial class CharacterDeletionWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterDeletionWindow));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("123456");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("234567");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("3456789");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("123123123");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("455345");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("1234553234");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("1235435");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("12314124");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("1235454");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("45656456");
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.characterToRemoveLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.deleteAPIKeyCheckBox = new System.Windows.Forms.CheckBox();
            this.noCharactersLabel = new System.Windows.Forms.Label();
            this.apiKeyslistView = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // characterToRemoveLabel
            // 
            this.characterToRemoveLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.characterToRemoveLabel.AutoSize = true;
            this.characterToRemoveLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.characterToRemoveLabel.Location = new System.Drawing.Point(51, 22);
            this.characterToRemoveLabel.Name = "characterToRemoveLabel";
            this.characterToRemoveLabel.Size = new System.Drawing.Size(259, 13);
            this.characterToRemoveLabel.TabIndex = 1;
            this.characterToRemoveLabel.Text = "You are about to delete the character \"{0}\".";
            this.characterToRemoveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.DarkRed;
            this.label2.Location = new System.Drawing.Point(51, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "All your data and plans will be lost!";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(412, 142);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.deleteButton.Location = new System.Drawing.Point(331, 142);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 4;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // deleteAPIKeyCheckBox
            // 
            this.deleteAPIKeyCheckBox.AutoSize = true;
            this.deleteAPIKeyCheckBox.Location = new System.Drawing.Point(54, 88);
            this.deleteAPIKeyCheckBox.Name = "deleteAPIKeyCheckBox";
            this.deleteAPIKeyCheckBox.Size = new System.Drawing.Size(154, 17);
            this.deleteAPIKeyCheckBox.TabIndex = 5;
            this.deleteAPIKeyCheckBox.Text = "Delete the API key{0} also.";
            this.deleteAPIKeyCheckBox.UseVisualStyleBackColor = true;
            // 
            // noCharactersLabel
            // 
            this.noCharactersLabel.AutoSize = true;
            this.noCharactersLabel.Location = new System.Drawing.Point(51, 66);
            this.noCharactersLabel.Name = "noCharactersLabel";
            this.noCharactersLabel.Size = new System.Drawing.Size(245, 13);
            this.noCharactersLabel.TabIndex = 6;
            this.noCharactersLabel.Text = "There will be no characters left on the API key{0} :";
            // 
            // apiKeyslistView
            // 
            this.apiKeyslistView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.apiKeyslistView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10});
            this.apiKeyslistView.Location = new System.Drawing.Point(302, 66);
            this.apiKeyslistView.Name = "apiKeyslistView";
            this.apiKeyslistView.Size = new System.Drawing.Size(192, 94);
            this.apiKeyslistView.TabIndex = 7;
            this.apiKeyslistView.UseCompatibleStateImageBehavior = false;
            this.apiKeyslistView.View = System.Windows.Forms.View.List;
            // 
            // CharacterDeletionWindow
            // 
            this.AcceptButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(499, 177);
            this.Controls.Add(this.noCharactersLabel);
            this.Controls.Add(this.deleteAPIKeyCheckBox);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.characterToRemoveLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.apiKeyslistView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CharacterDeletionWindow";
            this.Text = "Delete Character";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label characterToRemoveLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.CheckBox deleteAPIKeyCheckBox;
        private System.Windows.Forms.Label noCharactersLabel;
        private System.Windows.Forms.ListView apiKeyslistView;
    }
}