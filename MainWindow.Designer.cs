namespace EVEMon
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.niMinimizeIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconToolStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.planToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.niAlertIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tmrAlertRefresh = new System.Windows.Forms.Timer(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblServerStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrTranquilityClock = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainMenuBar = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.saveXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plansToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mineralWorksheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skillsPieChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.schedulerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.manualImplantGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inEvenetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToInEveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOwnedSkillbooksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resetCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tcCharacterTabs = new EVEMon.DraggableTabControl();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIconToolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mainMenuBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // niMinimizeIcon
            // 
            this.niMinimizeIcon.ContextMenuStrip = this.trayIconToolStrip;
            this.niMinimizeIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("niMinimizeIcon.Icon")));
            this.niMinimizeIcon.Click += new System.EventHandler(this.niMinimizeIcon_Click);
            this.niMinimizeIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.niMinimizeIcon_MouseMove);
            // 
            // trayIconToolStrip
            // 
            this.trayIconToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.planToolStripMenuItem,
            this.toolStripSeparator3,
            this.restoreToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.trayIconToolStrip.Name = "trayIconToolStrip";
            this.trayIconToolStrip.Size = new System.Drawing.Size(124, 76);
            this.trayIconToolStrip.Opening += new System.ComponentModel.CancelEventHandler(this.trayIconToolStrip_Opening);
            // 
            // planToolStripMenuItem
            // 
            this.planToolStripMenuItem.Name = "planToolStripMenuItem";
            this.planToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.planToolStripMenuItem.Text = "Plans";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(120, 6);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // niAlertIcon
            // 
            this.niAlertIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("niAlertIcon.Icon")));
            this.niAlertIcon.Text = "notifyIcon1";
            this.niAlertIcon.Click += new System.EventHandler(this.niAlertIcon_Click);
            this.niAlertIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.niAlertIcon_MouseClick);
            this.niAlertIcon.BalloonTipClicked += new System.EventHandler(this.niAlertIcon_BalloonTipClicked);
            // 
            // tmrAlertRefresh
            // 
            this.tmrAlertRefresh.Tick += new System.EventHandler(this.tmrAlertRefresh_Tick);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblServerStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 414);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(417, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(94, 17);
            this.lblStatus.Text = "Current EVE Time:";
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(131, 17);
            this.lblServerStatus.Text = "// Server Status Unknown";
            // 
            // tmrTranquilityClock
            // 
            this.tmrTranquilityClock.Enabled = true;
            this.tmrTranquilityClock.Interval = 5000;
            this.tmrTranquilityClock.Tick += new System.EventHandler(this.tmrClock_Tick);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.FileName = "settings.xml.bak";
            this.saveFileDialog.Filter = "Settings Backup Files (*.bak) | *.bak";
            this.saveFileDialog.Title = "Backup Your Settings File";
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.FileName = "settings.xml.bak";
            this.openFileDialog.Filter = "Settings Backup Files (*.bak) | *.bak";
            this.openFileDialog.ShowHelp = true;
            this.openFileDialog.Title = "Restore your settings file";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // mainMenuBar
            // 
            this.mainMenuBar.BackColor = System.Drawing.SystemColors.Control;
            this.mainMenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.plansToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuBar.Location = new System.Drawing.Point(0, 0);
            this.mainMenuBar.Name = "mainMenuBar";
            this.mainMenuBar.Size = new System.Drawing.Size(417, 24);
            this.mainMenuBar.TabIndex = 3;
            this.mainMenuBar.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCharacterToolStripMenuItem,
            this.removeCharacterToolStripMenuItem,
            this.toolStripSeparator5,
            this.saveXMLToolStripMenuItem,
            this.toolStripSeparator4,
            this.saveSettingsToolStripMenuItem,
            this.loadSettingsToolStripMenuItem,
            this.toolStripSeparator6,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // addCharacterToolStripMenuItem
            // 
            this.addCharacterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addCharacterToolStripMenuItem.Image")));
            this.addCharacterToolStripMenuItem.Name = "addCharacterToolStripMenuItem";
            this.addCharacterToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.addCharacterToolStripMenuItem.Text = "&Add Character...";
            this.addCharacterToolStripMenuItem.Click += new System.EventHandler(this.addCharacterToolStripMenuItem_Click);
            // 
            // removeCharacterToolStripMenuItem
            // 
            this.removeCharacterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeCharacterToolStripMenuItem.Image")));
            this.removeCharacterToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeCharacterToolStripMenuItem.Name = "removeCharacterToolStripMenuItem";
            this.removeCharacterToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.removeCharacterToolStripMenuItem.Text = "&Remove Character...";
            this.removeCharacterToolStripMenuItem.Click += new System.EventHandler(this.removeCharacterToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(184, 6);
            // 
            // saveXMLToolStripMenuItem
            // 
            this.saveXMLToolStripMenuItem.Name = "saveXMLToolStripMenuItem";
            this.saveXMLToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveXMLToolStripMenuItem.Text = "&Save Character Info...";
            this.saveXMLToolStripMenuItem.Click += new System.EventHandler(this.saveXMLToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(184, 6);
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveSettingsToolStripMenuItem.Image")));
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveSettingsToolStripMenuItem.Text = "Sa&ve Settings...";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loadSettingsToolStripMenuItem.Image")));
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadSettingsToolStripMenuItem.Text = "R&estore Settings...";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(184, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // plansToolStripMenuItem
            // 
            this.plansToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.manageToolStripMenuItem,
            this.toolStripSeparator7});
            this.plansToolStripMenuItem.Name = "plansToolStripMenuItem";
            this.plansToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.plansToolStripMenuItem.Text = "&Plans";
            this.plansToolStripMenuItem.DropDownOpening += new System.EventHandler(this.plansToolStripMenuItem_DropDownOpening);
            // 
            // manageToolStripMenuItem
            // 
            this.manageToolStripMenuItem.Name = "manageToolStripMenuItem";
            this.manageToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.manageToolStripMenuItem.Text = "&Manage...";
            this.manageToolStripMenuItem.Click += new System.EventHandler(this.manageToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(132, 6);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mineralWorksheetToolStripMenuItem,
            this.skillsPieChartToolStripMenuItem,
            this.schedulerToolStripMenuItem,
            this.toolStripSeparator8,
            this.manualImplantGroupsToolStripMenuItem,
            this.inEvenetToolStripMenuItem,
            this.sendToInEveToolStripMenuItem,
            this.showOwnedSkillbooksToolStripMenuItem,
            this.toolStripSeparator1,
            this.resetCacheToolStripMenuItem,
            this.toolStripSeparator2,
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.toolsToolStripMenuItem_DropDownOpening);
            // 
            // mineralWorksheetToolStripMenuItem
            // 
            this.mineralWorksheetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mineralWorksheetToolStripMenuItem.Image")));
            this.mineralWorksheetToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mineralWorksheetToolStripMenuItem.Name = "mineralWorksheetToolStripMenuItem";
            this.mineralWorksheetToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.mineralWorksheetToolStripMenuItem.Text = "&Mineral Worksheet...";
            this.mineralWorksheetToolStripMenuItem.Click += new System.EventHandler(this.mineralWorksheetToolStripMenuItem_Click);
            // 
            // skillsPieChartToolStripMenuItem
            // 
            this.skillsPieChartToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("skillsPieChartToolStripMenuItem.Image")));
            this.skillsPieChartToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.skillsPieChartToolStripMenuItem.Name = "skillsPieChartToolStripMenuItem";
            this.skillsPieChartToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.skillsPieChartToolStripMenuItem.Text = "Skills &Pie Chart...";
            this.skillsPieChartToolStripMenuItem.Click += new System.EventHandler(this.skillsPieChartToolStripMenuItem_Click);
            // 
            // schedulerToolStripMenuItem
            // 
            this.schedulerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("schedulerToolStripMenuItem.Image")));
            this.schedulerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.schedulerToolStripMenuItem.Name = "schedulerToolStripMenuItem";
            this.schedulerToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.schedulerToolStripMenuItem.Text = "&Scheduler...";
            this.schedulerToolStripMenuItem.Click += new System.EventHandler(this.schedulerToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(215, 6);
            // 
            // manualImplantGroupsToolStripMenuItem
            // 
            this.manualImplantGroupsToolStripMenuItem.Name = "manualImplantGroupsToolStripMenuItem";
            this.manualImplantGroupsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.manualImplantGroupsToolStripMenuItem.Text = "Manual &Implant Groups...";
            this.manualImplantGroupsToolStripMenuItem.Click += new System.EventHandler(this.manualImplantGroupsToolStripMenuItem_Click);
            // 
            // inEvenetToolStripMenuItem
            // 
            this.inEvenetToolStripMenuItem.Name = "inEvenetToolStripMenuItem";
            this.inEvenetToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.inEvenetToolStripMenuItem.Text = "Synchronize with inEve.net ";
            this.inEvenetToolStripMenuItem.Click += new System.EventHandler(this.inEvenetToolStripMenuItem_Click);
            // 
            // sendToInEveToolStripMenuItem
            // 
            this.sendToInEveToolStripMenuItem.Name = "sendToInEveToolStripMenuItem";
            this.sendToInEveToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.sendToInEveToolStripMenuItem.Text = "Send Data To InEve";
            this.sendToInEveToolStripMenuItem.Click += new System.EventHandler(this.sendToInEveToolStripMenuItem_Click);
            // 
            // showOwnedSkillbooksToolStripMenuItem
            // 
            this.showOwnedSkillbooksToolStripMenuItem.Name = "showOwnedSkillbooksToolStripMenuItem";
            this.showOwnedSkillbooksToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.showOwnedSkillbooksToolStripMenuItem.Text = "Show Owned Skillbooks...";
            this.showOwnedSkillbooksToolStripMenuItem.Click += new System.EventHandler(this.showOwnedSkillbooksToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(215, 6);
            // 
            // resetCacheToolStripMenuItem
            // 
            this.resetCacheToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("resetCacheToolStripMenuItem.Image")));
            this.resetCacheToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetCacheToolStripMenuItem.Name = "resetCacheToolStripMenuItem";
            this.resetCacheToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.resetCacheToolStripMenuItem.Text = "&Reset Cache";
            this.resetCacheToolStripMenuItem.Click += new System.EventHandler(this.resetCacheToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(215, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripMenuItem.Image")));
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.optionsToolStripMenuItem.Text = "&Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tcCharacterTabs
            // 
            this.tcCharacterTabs.AllowDrop = true;
            this.tcCharacterTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcCharacterTabs.Location = new System.Drawing.Point(0, 24);
            this.tcCharacterTabs.Name = "tcCharacterTabs";
            this.tcCharacterTabs.SelectedIndex = 0;
            this.tcCharacterTabs.ShowToolTips = true;
            this.tcCharacterTabs.Size = new System.Drawing.Size(417, 390);
            this.tcCharacterTabs.TabIndex = 0;
            this.tcCharacterTabs.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.UpdateTabVisibility);
            this.tcCharacterTabs.DragDrop += new System.Windows.Forms.DragEventHandler(this.tcCharacterTabs_DragDrop);
            this.tcCharacterTabs.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.UpdateTabVisibility);
            this.tcCharacterTabs.SelectedIndexChanged += new System.EventHandler(this.UpdateTabVisibility);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.newToolStripMenuItem.Text = "&New…";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 436);
            this.Controls.Add(this.tcCharacterTabs);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainMenuBar);
            this.MainMenuStrip = this.mainMenuBar;
            this.MinimumSize = new System.Drawing.Size(425, 350);
            this.Name = "MainWindow";
            this.Deactivate += new System.EventHandler(this.MainWindow_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.Resize += new System.EventHandler(this.MainWindow_Resize);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.trayIconToolStrip.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainMenuBar.ResumeLayout(false);
            this.mainMenuBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon niMinimizeIcon;
        private System.Windows.Forms.NotifyIcon niAlertIcon;
        private System.Windows.Forms.Timer tmrAlertRefresh;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Timer tmrTranquilityClock;
        private System.Windows.Forms.ContextMenuStrip trayIconToolStrip;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem planToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private EVEMon.DraggableTabControl tcCharacterTabs;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripStatusLabel lblServerStatus;
        private System.Windows.Forms.MenuStrip mainMenuBar;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mineralWorksheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skillsPieChartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem schedulerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem resetCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem manualImplantGroupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plansToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem inEvenetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOwnedSkillbooksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendToInEveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
    }
}
