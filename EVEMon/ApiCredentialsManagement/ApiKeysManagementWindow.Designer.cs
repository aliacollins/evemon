using EVEMon.Common.Controls.MultiPanel;

namespace EVEMon.ApiCredentialsManagement
{
    partial class ApiKeysManagementWindow
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "CCP",
            "John Doe",
            "123456",
            "(none)"}, 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApiKeysManagementWindow));
            this.apiKeysLabel = new System.Windows.Forms.Label();
            this.charactersLabel = new System.Windows.Forms.Label();
            this.apiKeyListLabel = new System.Windows.Forms.Label();
            this.charactersListLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.charactersTabPage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.charactersMultiPanel = new EVEMon.Common.Controls.MultiPanel.MultiPanel();
            this.charactersListPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.charactersListView = new System.Windows.Forms.ListView();
            this.columnMonitored = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnAPIKeyID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnUri = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.noCharactersPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.charactersToolStrip = new System.Windows.Forms.ToolStrip();
            this.importCharacterMenu = new System.Windows.Forms.ToolStripButton();
            this.deleteCharacterMenu = new System.Windows.Forms.ToolStripButton();
            this.editUriMenu = new System.Windows.Forms.ToolStripButton();
            this.groupingMenu = new System.Windows.Forms.ToolStripButton();
            this.apiKeysTabPage = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.apiKeysMultiPanel = new EVEMon.Common.Controls.MultiPanel.MultiPanel();
            this.apiKeysListPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.apiKeysListBox = new EVEMon.ApiCredentialsManagement.ApiKeysListBox();
            this.noAPIKeysPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.apiKeysToolStrip = new System.Windows.Forms.ToolStrip();
            this.addAPIKeyMenu = new System.Windows.Forms.ToolStripButton();
            this.deleteAPIKeyMenu = new System.Windows.Forms.ToolStripButton();
            this.editAPIKeyMenu = new System.Windows.Forms.ToolStripButton();
            this.tabControl.SuspendLayout();
            this.charactersTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.charactersMultiPanel.SuspendLayout();
            this.charactersListPage.SuspendLayout();
            this.noCharactersPage.SuspendLayout();
            this.charactersToolStrip.SuspendLayout();
            this.apiKeysTabPage.SuspendLayout();
            this.panel2.SuspendLayout();
            this.apiKeysMultiPanel.SuspendLayout();
            this.apiKeysListPage.SuspendLayout();
            this.noAPIKeysPage.SuspendLayout();
            this.apiKeysToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // apiKeysLabel
            // 
            this.apiKeysLabel.AutoSize = true;
            this.apiKeysLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apiKeysLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.apiKeysLabel.Location = new System.Drawing.Point(6, 3);
            this.apiKeysLabel.Name = "apiKeysLabel";
            this.apiKeysLabel.Size = new System.Drawing.Size(72, 19);
            this.apiKeysLabel.TabIndex = 1;
            this.apiKeysLabel.Text = "API Keys";
            // 
            // charactersLabel
            // 
            this.charactersLabel.AutoSize = true;
            this.charactersLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.charactersLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.charactersLabel.Location = new System.Drawing.Point(6, 3);
            this.charactersLabel.Name = "charactersLabel";
            this.charactersLabel.Size = new System.Drawing.Size(83, 19);
            this.charactersLabel.TabIndex = 5;
            this.charactersLabel.Text = "Characters";
            // 
            // apiKeyListLabel
            // 
            this.apiKeyListLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apiKeyListLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apiKeyListLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.apiKeyListLabel.Location = new System.Drawing.Point(0, 0);
            this.apiKeyListLabel.Name = "apiKeyListLabel";
            this.apiKeyListLabel.Size = new System.Drawing.Size(633, 364);
            this.apiKeyListLabel.TabIndex = 0;
            this.apiKeyListLabel.Text = "First add your API key using the above buttons.";
            this.apiKeyListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // charactersListLabel
            // 
            this.charactersListLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charactersListLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.charactersListLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.charactersListLabel.Location = new System.Drawing.Point(0, 0);
            this.charactersListLabel.Name = "charactersListLabel";
            this.charactersListLabel.Size = new System.Drawing.Size(633, 364);
            this.charactersListLabel.TabIndex = 10;
            this.charactersListLabel.Text = "First add your API key at API Keys section, characters will then appear in this l" +
                "ist.\r\nYou can also import XML character sheets from files or URLs.";
            this.charactersListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.CausesValidation = false;
            this.closeButton.Location = new System.Drawing.Point(579, 464);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(90, 23);
            this.closeButton.TabIndex = 9;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.charactersTabPage);
            this.tabControl.Controls.Add(this.apiKeysTabPage);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(657, 446);
            this.tabControl.TabIndex = 12;
            // 
            // charactersTabPage
            // 
            this.charactersTabPage.Controls.Add(this.panel1);
            this.charactersTabPage.Controls.Add(this.charactersLabel);
            this.charactersTabPage.Location = new System.Drawing.Point(4, 22);
            this.charactersTabPage.Name = "charactersTabPage";
            this.charactersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.charactersTabPage.Size = new System.Drawing.Size(649, 420);
            this.charactersTabPage.TabIndex = 1;
            this.charactersTabPage.Text = "Characters";
            this.charactersTabPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.charactersMultiPanel);
            this.panel1.Controls.Add(this.charactersToolStrip);
            this.panel1.Location = new System.Drawing.Point(10, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(633, 389);
            this.panel1.TabIndex = 16;
            // 
            // charactersMultiPanel
            // 
            this.charactersMultiPanel.Controls.Add(this.charactersListPage);
            this.charactersMultiPanel.Controls.Add(this.noCharactersPage);
            this.charactersMultiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charactersMultiPanel.Location = new System.Drawing.Point(0, 25);
            this.charactersMultiPanel.Name = "charactersMultiPanel";
            this.charactersMultiPanel.SelectedPage = this.noCharactersPage;
            this.charactersMultiPanel.Size = new System.Drawing.Size(633, 364);
            this.charactersMultiPanel.TabIndex = 15;
            // 
            // charactersListPage
            // 
            this.charactersListPage.Controls.Add(this.charactersListView);
            this.charactersListPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charactersListPage.Location = new System.Drawing.Point(0, 0);
            this.charactersListPage.Name = "charactersListPage";
            this.charactersListPage.Size = new System.Drawing.Size(633, 364);
            this.charactersListPage.TabIndex = 0;
            this.charactersListPage.Text = "charactersListPage";
            // 
            // charactersListView
            // 
            this.charactersListView.CheckBoxes = true;
            this.charactersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnMonitored,
            this.columnType,
            this.columnName,
            this.columnAPIKeyID,
            this.columnUri});
            this.charactersListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charactersListView.FullRowSelect = true;
            listViewItem1.StateImageIndex = 0;
            this.charactersListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.charactersListView.Location = new System.Drawing.Point(0, 0);
            this.charactersListView.MultiSelect = false;
            this.charactersListView.Name = "charactersListView";
            this.charactersListView.Size = new System.Drawing.Size(633, 364);
            this.charactersListView.TabIndex = 12;
            this.charactersListView.UseCompatibleStateImageBehavior = false;
            this.charactersListView.View = System.Windows.Forms.View.Details;
            this.charactersListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.charactersListView_ItemChecked);
            this.charactersListView.SelectedIndexChanged += new System.EventHandler(this.charactersListView_SelectedIndexChanged);
            this.charactersListView.DoubleClick += new System.EventHandler(this.charactersListView_DoubleClick);
            this.charactersListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.charactersListView_KeyDown);
            // 
            // columnMonitored
            // 
            this.columnMonitored.Text = "";
            this.columnMonitored.Width = 36;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 49;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 117;
            // 
            // columnAPIKeyID
            // 
            this.columnAPIKeyID.Text = "Key ID";
            this.columnAPIKeyID.Width = 75;
            // 
            // columnUri
            // 
            this.columnUri.Text = "Uri";
            this.columnUri.Width = 358;
            // 
            // noCharactersPage
            // 
            this.noCharactersPage.Controls.Add(this.charactersListLabel);
            this.noCharactersPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noCharactersPage.Location = new System.Drawing.Point(0, 0);
            this.noCharactersPage.Name = "noCharactersPage";
            this.noCharactersPage.Size = new System.Drawing.Size(633, 364);
            this.noCharactersPage.TabIndex = 1;
            this.noCharactersPage.Text = "noCharactersPage";
            // 
            // charactersToolStrip
            // 
            this.charactersToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.charactersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importCharacterMenu,
            this.deleteCharacterMenu,
            this.editUriMenu,
            this.groupingMenu});
            this.charactersToolStrip.Location = new System.Drawing.Point(0, 0);
            this.charactersToolStrip.Name = "charactersToolStrip";
            this.charactersToolStrip.Size = new System.Drawing.Size(633, 25);
            this.charactersToolStrip.TabIndex = 13;
            // 
            // importCharacterMenu
            // 
            this.importCharacterMenu.Image = ((System.Drawing.Image)(resources.GetObject("importCharacterMenu.Image")));
            this.importCharacterMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importCharacterMenu.Name = "importCharacterMenu";
            this.importCharacterMenu.Size = new System.Drawing.Size(72, 22);
            this.importCharacterMenu.Text = "&Import...";
            this.importCharacterMenu.Click += new System.EventHandler(this.importCharacterMenu_Click);
            // 
            // deleteCharacterMenu
            // 
            this.deleteCharacterMenu.Enabled = false;
            this.deleteCharacterMenu.Image = ((System.Drawing.Image)(resources.GetObject("deleteCharacterMenu.Image")));
            this.deleteCharacterMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteCharacterMenu.Name = "deleteCharacterMenu";
            this.deleteCharacterMenu.Size = new System.Drawing.Size(69, 22);
            this.deleteCharacterMenu.Text = "&Delete...";
            this.deleteCharacterMenu.Click += new System.EventHandler(this.deleteCharacterMenu_Click);
            // 
            // editUriMenu
            // 
            this.editUriMenu.Enabled = false;
            this.editUriMenu.Image = ((System.Drawing.Image)(resources.GetObject("editUriMenu.Image")));
            this.editUriMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editUriMenu.Name = "editUriMenu";
            this.editUriMenu.Size = new System.Drawing.Size(74, 22);
            this.editUriMenu.Text = "&Edit Uri...";
            this.editUriMenu.Click += new System.EventHandler(this.editUriButton_Click);
            // 
            // groupingMenu
            // 
            this.groupingMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.groupingMenu.Checked = true;
            this.groupingMenu.CheckOnClick = true;
            this.groupingMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.groupingMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.groupingMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.groupingMenu.Name = "groupingMenu";
            this.groupingMenu.Size = new System.Drawing.Size(101, 22);
            this.groupingMenu.Text = "&Group characters";
            this.groupingMenu.Click += new System.EventHandler(this.groupingMenu_Click);
            // 
            // apiKeysTabPage
            // 
            this.apiKeysTabPage.Controls.Add(this.panel2);
            this.apiKeysTabPage.Controls.Add(this.apiKeysLabel);
            this.apiKeysTabPage.Location = new System.Drawing.Point(4, 22);
            this.apiKeysTabPage.Name = "apiKeysTabPage";
            this.apiKeysTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.apiKeysTabPage.Size = new System.Drawing.Size(649, 420);
            this.apiKeysTabPage.TabIndex = 0;
            this.apiKeysTabPage.Text = "API Keys";
            this.apiKeysTabPage.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.apiKeysMultiPanel);
            this.panel2.Controls.Add(this.apiKeysToolStrip);
            this.panel2.Location = new System.Drawing.Point(10, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(633, 389);
            this.panel2.TabIndex = 17;
            // 
            // apiKeysMultiPanel
            // 
            this.apiKeysMultiPanel.Controls.Add(this.apiKeysListPage);
            this.apiKeysMultiPanel.Controls.Add(this.noAPIKeysPage);
            this.apiKeysMultiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apiKeysMultiPanel.Location = new System.Drawing.Point(0, 25);
            this.apiKeysMultiPanel.Name = "apiKeysMultiPanel";
            this.apiKeysMultiPanel.SelectedPage = this.noAPIKeysPage;
            this.apiKeysMultiPanel.Size = new System.Drawing.Size(633, 364);
            this.apiKeysMultiPanel.TabIndex = 16;
            // 
            // apiKeysListPage
            // 
            this.apiKeysListPage.Controls.Add(this.apiKeysListBox);
            this.apiKeysListPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apiKeysListPage.Location = new System.Drawing.Point(0, 0);
            this.apiKeysListPage.Name = "apiKeysListPage";
            this.apiKeysListPage.Size = new System.Drawing.Size(633, 364);
            this.apiKeysListPage.TabIndex = 0;
            this.apiKeysListPage.Text = "apiKeysListPage";
            // 
            // apiKeysListBox
            // 
            this.apiKeysListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apiKeysListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.apiKeysListBox.FormattingEnabled = true;
            this.apiKeysListBox.ItemHeight = 40;
            this.apiKeysListBox.Location = new System.Drawing.Point(0, 0);
            this.apiKeysListBox.Name = "apiKeysListBox";
            this.apiKeysListBox.Size = new System.Drawing.Size(633, 364);
            this.apiKeysListBox.TabIndex = 0;
            this.apiKeysListBox.SelectedIndexChanged += new System.EventHandler(this.apiKeysListBox_SelectedIndexChanged);
            this.apiKeysListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.apiKeysListBox_MouseClick);
            this.apiKeysListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.apiKeysListBox_MouseDoubleClick);
            this.apiKeysListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.apiKeysListBox_KeyDown);
            // 
            // noAPIKeysPage
            // 
            this.noAPIKeysPage.Controls.Add(this.apiKeyListLabel);
            this.noAPIKeysPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noAPIKeysPage.Location = new System.Drawing.Point(0, 0);
            this.noAPIKeysPage.Name = "noAPIKeysPage";
            this.noAPIKeysPage.Size = new System.Drawing.Size(633, 364);
            this.noAPIKeysPage.TabIndex = 1;
            this.noAPIKeysPage.Text = "noAPIKeysPage";
            // 
            // apiKeysToolStrip
            // 
            this.apiKeysToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.apiKeysToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAPIKeyMenu,
            this.deleteAPIKeyMenu,
            this.editAPIKeyMenu});
            this.apiKeysToolStrip.Location = new System.Drawing.Point(0, 0);
            this.apiKeysToolStrip.Name = "apiKeysToolStrip";
            this.apiKeysToolStrip.Size = new System.Drawing.Size(633, 25);
            this.apiKeysToolStrip.TabIndex = 3;
            // 
            // addAPIKeyMenu
            // 
            this.addAPIKeyMenu.Image = ((System.Drawing.Image)(resources.GetObject("addAPIKeyMenu.Image")));
            this.addAPIKeyMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addAPIKeyMenu.Name = "addAPIKeyMenu";
            this.addAPIKeyMenu.Size = new System.Drawing.Size(58, 22);
            this.addAPIKeyMenu.Text = "&Add...";
            this.addAPIKeyMenu.Click += new System.EventHandler(this.addAPIKeyMenu_Click);
            // 
            // deleteAPIKeyMenu
            // 
            this.deleteAPIKeyMenu.Enabled = false;
            this.deleteAPIKeyMenu.Image = ((System.Drawing.Image)(resources.GetObject("deleteAPIKeyMenu.Image")));
            this.deleteAPIKeyMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteAPIKeyMenu.Name = "deleteAPIKeyMenu";
            this.deleteAPIKeyMenu.Size = new System.Drawing.Size(69, 22);
            this.deleteAPIKeyMenu.Text = "&Delete...";
            this.deleteAPIKeyMenu.Click += new System.EventHandler(this.deleteAPIKeyMenu_Click);
            // 
            // editAPIKeyMenu
            // 
            this.editAPIKeyMenu.Enabled = false;
            this.editAPIKeyMenu.Image = ((System.Drawing.Image)(resources.GetObject("editAPIKeyMenu.Image")));
            this.editAPIKeyMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editAPIKeyMenu.Name = "editAPIKeyMenu";
            this.editAPIKeyMenu.Size = new System.Drawing.Size(56, 22);
            this.editAPIKeyMenu.Text = "&Edit...";
            this.editAPIKeyMenu.Click += new System.EventHandler(this.editAPIKeyMenu_Click);
            // 
            // ApiKeysManagementWindow
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(681, 499);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.closeButton);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(580, 530);
            this.Name = "ApiKeysManagementWindow";
            this.Text = "API Keys Management";
            this.tabControl.ResumeLayout(false);
            this.charactersTabPage.ResumeLayout(false);
            this.charactersTabPage.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.charactersMultiPanel.ResumeLayout(false);
            this.charactersListPage.ResumeLayout(false);
            this.noCharactersPage.ResumeLayout(false);
            this.charactersToolStrip.ResumeLayout(false);
            this.charactersToolStrip.PerformLayout();
            this.apiKeysTabPage.ResumeLayout(false);
            this.apiKeysTabPage.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.apiKeysMultiPanel.ResumeLayout(false);
            this.apiKeysListPage.ResumeLayout(false);
            this.noAPIKeysPage.ResumeLayout(false);
            this.apiKeysToolStrip.ResumeLayout(false);
            this.apiKeysToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage apiKeysTabPage;
        private System.Windows.Forms.TabPage charactersTabPage;
        private System.Windows.Forms.ListView charactersListView;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnAPIKeyID;
        private System.Windows.Forms.ColumnHeader columnUri;
        private System.Windows.Forms.ColumnHeader columnMonitored;
        private MultiPanel apiKeysMultiPanel;
        private MultiPanelPage apiKeysListPage;
        private MultiPanelPage noAPIKeysPage;
        private MultiPanel charactersMultiPanel;
        private MultiPanelPage charactersListPage;
        private MultiPanelPage noCharactersPage;
        private System.Windows.Forms.ToolStrip charactersToolStrip;
        private System.Windows.Forms.ToolStripButton importCharacterMenu;
        private System.Windows.Forms.ToolStripButton deleteCharacterMenu;
        private System.Windows.Forms.ToolStripButton editUriMenu;
        private System.Windows.Forms.ToolStripButton groupingMenu;
        private System.Windows.Forms.ToolStrip apiKeysToolStrip;
        private System.Windows.Forms.ToolStripButton addAPIKeyMenu;
        private System.Windows.Forms.ToolStripButton deleteAPIKeyMenu;
        private System.Windows.Forms.ToolStripButton editAPIKeyMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label apiKeysLabel;
        private System.Windows.Forms.Label charactersLabel;
        private System.Windows.Forms.Label apiKeyListLabel;
        private System.Windows.Forms.Label charactersListLabel;
        private ApiKeysListBox apiKeysListBox;
    }
}