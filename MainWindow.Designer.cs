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
            this.lblTraining = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrTranquilityClock = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainMenuBar = new System.Windows.Forms.MenuStrip();
            this.toolbarContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menubarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.standardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ttMainWindow = new System.Windows.Forms.ToolTip(this.components);
            this.standardToolbar = new System.Windows.Forms.ToolStrip();
            this.tsbAddChar = new System.Windows.Forms.ToolStripButton();
            this.tsbRemoveChar = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsdbSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsSaveSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLoadSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbMineralSheet = new System.Windows.Forms.ToolStripButton();
            this.skillGroupPieChartButton = new System.Windows.Forms.ToolStripButton();
            this.tsbSchedule = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbManagePlans = new System.Windows.Forms.ToolStripButton();
            this.tsdbPlans = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbOptions = new System.Windows.Forms.ToolStripButton();
            this.tsbAbout = new System.Windows.Forms.ToolStripButton();
            this.tcCharacterTabs = new EVEMon.DraggableTabControl();
            this.trayIconToolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mainMenuBar.SuspendLayout();
            this.toolbarContext.SuspendLayout();
            this.standardToolbar.SuspendLayout();
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
            this.trayIconToolStrip.Size = new System.Drawing.Size(113, 76);
            this.trayIconToolStrip.Opening += new System.ComponentModel.CancelEventHandler(this.trayIconToolStrip_Opening);
            // 
            // planToolStripMenuItem
            // 
            this.planToolStripMenuItem.Name = "planToolStripMenuItem";
            this.planToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.planToolStripMenuItem.Text = "Plans";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(109, 6);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
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
            this.lblServerStatus,
            this.lblTraining});
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
            // lblTraining
            // 
            this.lblTraining.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lblTraining.Name = "lblTraining";
            this.lblTraining.Size = new System.Drawing.Size(0, 17);
            this.lblTraining.Text = "toolStripStatusLabel1";
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
            this.mainMenuBar.ContextMenuStrip = this.toolbarContext;
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
            // toolbarContext
            // 
            this.toolbarContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menubarToolStripMenuItem,
            this.standardToolStripMenuItem});
            this.toolbarContext.Name = "toolbarContext";
            this.toolbarContext.Size = new System.Drawing.Size(153, 70);
            this.toolbarContext.Opening += new System.ComponentModel.CancelEventHandler(this.toolbarContext_Opening);
            // 
            // menubarToolStripMenuItem
            // 
            this.menubarToolStripMenuItem.Checked = true;
            this.menubarToolStripMenuItem.CheckOnClick = true;
            this.menubarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menubarToolStripMenuItem.Name = "menubarToolStripMenuItem";
            this.menubarToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.menubarToolStripMenuItem.Text = "&Menubar";
            this.menubarToolStripMenuItem.Click += new System.EventHandler(this.menubarToolStripMenuItem_Click);
            // 
            // standardToolStripMenuItem
            // 
            this.standardToolStripMenuItem.CheckOnClick = true;
            this.standardToolStripMenuItem.Name = "standardToolStripMenuItem";
            this.standardToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.standardToolStripMenuItem.Text = "&Standard";
            this.standardToolStripMenuItem.Click += new System.EventHandler(this.standardToolStripMenuItem_Click);
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
            this.addCharacterToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.addCharacterToolStripMenuItem.Text = "&Add Character...";
            this.addCharacterToolStripMenuItem.Click += new System.EventHandler(this.addCharacterToolStripMenuItem_Click);
            // 
            // removeCharacterToolStripMenuItem
            // 
            this.removeCharacterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeCharacterToolStripMenuItem.Image")));
            this.removeCharacterToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeCharacterToolStripMenuItem.Name = "removeCharacterToolStripMenuItem";
            this.removeCharacterToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.removeCharacterToolStripMenuItem.Text = "&Remove Character...";
            this.removeCharacterToolStripMenuItem.Click += new System.EventHandler(this.removeCharacterToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(181, 6);
            // 
            // saveXMLToolStripMenuItem
            // 
            this.saveXMLToolStripMenuItem.Name = "saveXMLToolStripMenuItem";
            this.saveXMLToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveXMLToolStripMenuItem.Text = "&Save Character Info...";
            this.saveXMLToolStripMenuItem.Click += new System.EventHandler(this.saveXMLToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(181, 6);
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveSettingsToolStripMenuItem.Image")));
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveSettingsToolStripMenuItem.Text = "Sa&ve Settings...";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loadSettingsToolStripMenuItem.Image")));
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.loadSettingsToolStripMenuItem.Text = "R&estore Settings...";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(181, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
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
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "&New…";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // manageToolStripMenuItem
            // 
            this.manageToolStripMenuItem.Name = "manageToolStripMenuItem";
            this.manageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.manageToolStripMenuItem.Text = "&Manage...";
            this.manageToolStripMenuItem.Click += new System.EventHandler(this.manageToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(149, 6);
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
            this.mineralWorksheetToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.mineralWorksheetToolStripMenuItem.Text = "&Mineral Worksheet...";
            this.mineralWorksheetToolStripMenuItem.Click += new System.EventHandler(this.mineralWorksheetToolStripMenuItem_Click);
            // 
            // skillsPieChartToolStripMenuItem
            // 
            this.skillsPieChartToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("skillsPieChartToolStripMenuItem.Image")));
            this.skillsPieChartToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.skillsPieChartToolStripMenuItem.Name = "skillsPieChartToolStripMenuItem";
            this.skillsPieChartToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.skillsPieChartToolStripMenuItem.Text = "Skills &Pie Chart...";
            this.skillsPieChartToolStripMenuItem.Click += new System.EventHandler(this.skillsPieChartToolStripMenuItem_Click);
            // 
            // schedulerToolStripMenuItem
            // 
            this.schedulerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("schedulerToolStripMenuItem.Image")));
            this.schedulerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.schedulerToolStripMenuItem.Name = "schedulerToolStripMenuItem";
            this.schedulerToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.schedulerToolStripMenuItem.Text = "&Scheduler...";
            this.schedulerToolStripMenuItem.Click += new System.EventHandler(this.schedulerToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(204, 6);
            // 
            // manualImplantGroupsToolStripMenuItem
            // 
            this.manualImplantGroupsToolStripMenuItem.Name = "manualImplantGroupsToolStripMenuItem";
            this.manualImplantGroupsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.manualImplantGroupsToolStripMenuItem.Text = "Manual &Implant Groups...";
            this.manualImplantGroupsToolStripMenuItem.Click += new System.EventHandler(this.manualImplantGroupsToolStripMenuItem_Click);
            // 
            // inEvenetToolStripMenuItem
            // 
            this.inEvenetToolStripMenuItem.Name = "inEvenetToolStripMenuItem";
            this.inEvenetToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.inEvenetToolStripMenuItem.Text = "Synchronize with inEve.net ";
            this.inEvenetToolStripMenuItem.Click += new System.EventHandler(this.inEvenetToolStripMenuItem_Click);
            // 
            // sendToInEveToolStripMenuItem
            // 
            this.sendToInEveToolStripMenuItem.Name = "sendToInEveToolStripMenuItem";
            this.sendToInEveToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.sendToInEveToolStripMenuItem.Text = "Send Data To InEve";
            this.sendToInEveToolStripMenuItem.Click += new System.EventHandler(this.sendToInEveToolStripMenuItem_Click);
            // 
            // showOwnedSkillbooksToolStripMenuItem
            // 
            this.showOwnedSkillbooksToolStripMenuItem.Name = "showOwnedSkillbooksToolStripMenuItem";
            this.showOwnedSkillbooksToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.showOwnedSkillbooksToolStripMenuItem.Text = "Show Owned Skillbooks...";
            this.showOwnedSkillbooksToolStripMenuItem.Click += new System.EventHandler(this.showOwnedSkillbooksToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(204, 6);
            // 
            // resetCacheToolStripMenuItem
            // 
            this.resetCacheToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("resetCacheToolStripMenuItem.Image")));
            this.resetCacheToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetCacheToolStripMenuItem.Name = "resetCacheToolStripMenuItem";
            this.resetCacheToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.resetCacheToolStripMenuItem.Text = "&Reset Cache";
            this.resetCacheToolStripMenuItem.Click += new System.EventHandler(this.resetCacheToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(204, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripMenuItem.Image")));
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // standardToolbar
            // 
            this.standardToolbar.ContextMenuStrip = this.toolbarContext;
            this.standardToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.standardToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddChar,
            this.tsbRemoveChar,
            this.toolStripSeparator9,
            this.tsdbSettings,
            this.toolStripSeparator10,
            this.tsbMineralSheet,
            this.skillGroupPieChartButton,
            this.tsbSchedule,
            this.toolStripSeparator11,
            this.tsbManagePlans,
            this.tsdbPlans,
            this.toolStripSeparator12,
            this.tsbOptions,
            this.tsbAbout});
            this.standardToolbar.Location = new System.Drawing.Point(0, 24);
            this.standardToolbar.Name = "standardToolbar";
            this.standardToolbar.Size = new System.Drawing.Size(417, 25);
            this.standardToolbar.TabIndex = 5;
            this.standardToolbar.Text = "toolStrip1";
            this.standardToolbar.Visible = false;
            // 
            // tsbAddChar
            // 
            this.tsbAddChar.Image = ((System.Drawing.Image)(resources.GetObject("tsbAddChar.Image")));
            this.tsbAddChar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddChar.Name = "tsbAddChar";
            this.tsbAddChar.Size = new System.Drawing.Size(23, 22);
            this.tsbAddChar.Click += new System.EventHandler(this.addCharacterToolStripMenuItem_Click);
            // 
            // tsbRemoveChar
            // 
            this.tsbRemoveChar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemoveChar.Enabled = false;
            this.tsbRemoveChar.Image = ((System.Drawing.Image)(resources.GetObject("tsbRemoveChar.Image")));
            this.tsbRemoveChar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemoveChar.Name = "tsbRemoveChar";
            this.tsbRemoveChar.Size = new System.Drawing.Size(23, 22);
            this.tsbRemoveChar.Text = "Remove Character";
            this.tsbRemoveChar.Click += new System.EventHandler(this.removeCharacterToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // tsdbSettings
            // 
            this.tsdbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsdbSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsSaveSettings,
            this.tsLoadSettings});
            this.tsdbSettings.Image = ((System.Drawing.Image)(resources.GetObject("tsdbSettings.Image")));
            this.tsdbSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdbSettings.Name = "tsdbSettings";
            this.tsdbSettings.Size = new System.Drawing.Size(29, 22);
            this.tsdbSettings.Text = "toolStripDropDownButton1";
            this.tsdbSettings.ToolTipText = "Save/Restore Settings";
            // 
            // tsSaveSettings
            // 
            this.tsSaveSettings.Image = ((System.Drawing.Image)(resources.GetObject("tsSaveSettings.Image")));
            this.tsSaveSettings.Name = "tsSaveSettings";
            this.tsSaveSettings.Size = new System.Drawing.Size(166, 22);
            this.tsSaveSettings.Text = "Save Settings...";
            this.tsSaveSettings.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // tsLoadSettings
            // 
            this.tsLoadSettings.Image = ((System.Drawing.Image)(resources.GetObject("tsLoadSettings.Image")));
            this.tsLoadSettings.Name = "tsLoadSettings";
            this.tsLoadSettings.Size = new System.Drawing.Size(166, 22);
            this.tsLoadSettings.Text = "Restore Settings...";
            this.tsLoadSettings.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbMineralSheet
            // 
            this.tsbMineralSheet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMineralSheet.Image = ((System.Drawing.Image)(resources.GetObject("tsbMineralSheet.Image")));
            this.tsbMineralSheet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMineralSheet.Name = "tsbMineralSheet";
            this.tsbMineralSheet.Size = new System.Drawing.Size(23, 22);
            this.tsbMineralSheet.Text = "Mineral Worksheet";
            this.tsbMineralSheet.Click += new System.EventHandler(this.mineralWorksheetToolStripMenuItem_Click);
            // 
            // skillGroupPieChartButton
            // 
            this.skillGroupPieChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.skillGroupPieChartButton.Image = ((System.Drawing.Image)(resources.GetObject("skillGroupPieChartButton.Image")));
            this.skillGroupPieChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.skillGroupPieChartButton.Name = "skillGroupPieChartButton";
            this.skillGroupPieChartButton.Size = new System.Drawing.Size(23, 22);
            this.skillGroupPieChartButton.Text = "Skill Group Pie Chart";
            this.skillGroupPieChartButton.Click += new System.EventHandler(this.skillsPieChartToolStripMenuItem_Click);
            // 
            // tsbSchedule
            // 
            this.tsbSchedule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSchedule.Image = ((System.Drawing.Image)(resources.GetObject("tsbSchedule.Image")));
            this.tsbSchedule.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSchedule.Name = "tsbSchedule";
            this.tsbSchedule.Size = new System.Drawing.Size(23, 22);
            this.tsbSchedule.Text = "Schedule";
            this.tsbSchedule.Click += new System.EventHandler(this.schedulerToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbManagePlans
            // 
            this.tsbManagePlans.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbManagePlans.Image = ((System.Drawing.Image)(resources.GetObject("tsbManagePlans.Image")));
            this.tsbManagePlans.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbManagePlans.Name = "tsbManagePlans";
            this.tsbManagePlans.Size = new System.Drawing.Size(23, 22);
            this.tsbManagePlans.Text = "Manage Plans";
            this.tsbManagePlans.Click += new System.EventHandler(this.manageToolStripMenuItem_Click);
            // 
            // tsdbPlans
            // 
            this.tsdbPlans.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdbPlans.Image = ((System.Drawing.Image)(resources.GetObject("tsdbPlans.Image")));
            this.tsdbPlans.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdbPlans.Name = "tsdbPlans";
            this.tsdbPlans.Size = new System.Drawing.Size(57, 22);
            this.tsdbPlans.Text = "Plans...";
            this.tsdbPlans.DropDownOpening += new System.EventHandler(this.tsdbPlans_DropDownOpening);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbOptions
            // 
            this.tsbOptions.Image = ((System.Drawing.Image)(resources.GetObject("tsbOptions.Image")));
            this.tsbOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOptions.Name = "tsbOptions";
            this.tsbOptions.Size = new System.Drawing.Size(76, 22);
            this.tsbOptions.Text = "Options...";
            this.tsbOptions.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // tsbAbout
            // 
            this.tsbAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAbout.Image = ((System.Drawing.Image)(resources.GetObject("tsbAbout.Image")));
            this.tsbAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAbout.Name = "tsbAbout";
            this.tsbAbout.Size = new System.Drawing.Size(23, 22);
            this.tsbAbout.Text = "About...";
            this.tsbAbout.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 436);
            this.Controls.Add(this.tcCharacterTabs);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.standardToolbar);
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
            this.toolbarContext.ResumeLayout(false);
            this.standardToolbar.ResumeLayout(false);
            this.standardToolbar.PerformLayout();
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
        private System.Windows.Forms.ToolStripStatusLabel lblTraining;
        private System.Windows.Forms.ToolTip ttMainWindow;
        private System.Windows.Forms.ContextMenuStrip toolbarContext;
        private System.Windows.Forms.ToolStripMenuItem menubarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem standardToolStripMenuItem;
        private System.Windows.Forms.ToolStrip standardToolbar;
        private System.Windows.Forms.ToolStripButton tsbAddChar;
        private System.Windows.Forms.ToolStripButton tsbRemoveChar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripDropDownButton tsdbSettings;
        private System.Windows.Forms.ToolStripMenuItem tsSaveSettings;
        private System.Windows.Forms.ToolStripMenuItem tsLoadSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripButton tsbMineralSheet;
        private System.Windows.Forms.ToolStripButton skillGroupPieChartButton;
        private System.Windows.Forms.ToolStripButton tsbSchedule;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton tsbOptions;
        private System.Windows.Forms.ToolStripButton tsbAbout;
        private System.Windows.Forms.ToolStripButton tsbManagePlans;
        private System.Windows.Forms.ToolStripDropDownButton tsdbPlans;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
    }
}
