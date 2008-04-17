namespace EVEMon
{
    partial class CharacterMonitor
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
            this.components = new System.ComponentModel.Container();
            this.pbCharImage = new System.Windows.Forms.PictureBox();
            this.lblCharacterName = new System.Windows.Forms.Label();
            this.lblBioInfo = new System.Windows.Forms.Label();
            this.lblCorpInfo = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblWillpower = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.lblPerception = new System.Windows.Forms.Label();
            this.lblCharisma = new System.Windows.Forms.Label();
            this.lblIntelligence = new System.Windows.Forms.Label();
            this.pnlTraining = new System.Windows.Forms.Panel();
            this.tlpStatus = new System.Windows.Forms.TableLayoutPanel();
            this.flpStatusLabels = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCurrentlyTraining = new System.Windows.Forms.Label();
            this.lblSPPerHour = new System.Windows.Forms.Label();
            this.lblScheduleWarning = new System.Windows.Forms.Label();
            this.flpStatusValues = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTrainingSkill = new System.Windows.Forms.Label();
            this.lblTrainingRemain = new System.Windows.Forms.Label();
            this.lblTrainingEst = new System.Windows.Forms.Label();
            this.tmrUpdateCharacter = new System.Windows.Forms.Timer(this.components);
            this.pnlCharData = new System.Windows.Forms.Panel();
            this.tlpInfo = new System.Windows.Forms.TableLayoutPanel();
            this.flpThrobber = new System.Windows.Forms.FlowLayoutPanel();
            this.throbber = new EVEMon.Throbber();
            this.cmsThrobberMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miHitEveO = new System.Windows.Forms.ToolStripMenuItem();
            this.miHitTrainingSkill = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangeInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.lblUpdateTimer = new System.Windows.Forms.Label();
            this.flpAttributes = new System.Windows.Forms.FlowLayoutPanel();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.llToggleAll = new System.Windows.Forms.LinkLabel();
            this.flpCharacterInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSkillHeader = new System.Windows.Forms.Label();
            this.tmrTick = new System.Windows.Forms.Timer(this.components);
            this.sfdSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.ttToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cmsPictureOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updatePicture = new System.Windows.Forms.ToolStripMenuItem();
            this.updatePictureFromEVECache = new System.Windows.Forms.ToolStripMenuItem();
            this.setEVEFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrMinTrainingSkillRetry = new System.Windows.Forms.Timer(this.components);
            this.lbSkills = new EVEMon.NoFlickerListBox();
            this.btnAddToCalendar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbCharImage)).BeginInit();
            this.pnlTraining.SuspendLayout();
            this.tlpStatus.SuspendLayout();
            this.flpStatusLabels.SuspendLayout();
            this.flpStatusValues.SuspendLayout();
            this.pnlCharData.SuspendLayout();
            this.tlpInfo.SuspendLayout();
            this.flpThrobber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).BeginInit();
            this.cmsThrobberMenu.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.flpCharacterInfo.SuspendLayout();
            this.cmsPictureOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbCharImage
            // 
            this.pbCharImage.InitialImage = global::EVEMon.Properties.Resources.default_char_pic;
            this.pbCharImage.Location = new System.Drawing.Point(0, 0);
            this.pbCharImage.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.pbCharImage.MinimumSize = new System.Drawing.Size(128, 128);
            this.pbCharImage.Name = "pbCharImage";
            this.pbCharImage.Size = new System.Drawing.Size(128, 128);
            this.pbCharImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCharImage.TabIndex = 0;
            this.pbCharImage.TabStop = false;
            this.pbCharImage.Click += new System.EventHandler(this.pbCharImage_Click);
            // 
            // lblCharacterName
            // 
            this.lblCharacterName.AutoSize = true;
            this.lblCharacterName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterName.Location = new System.Drawing.Point(0, 0);
            this.lblCharacterName.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCharacterName.Name = "lblCharacterName";
            this.lblCharacterName.Size = new System.Drawing.Size(129, 18);
            this.lblCharacterName.TabIndex = 1;
            this.lblCharacterName.Text = "Character Name";
            // 
            // lblBioInfo
            // 
            this.lblBioInfo.AutoSize = true;
            this.lblBioInfo.Location = new System.Drawing.Point(0, 18);
            this.lblBioInfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblBioInfo.Name = "lblBioInfo";
            this.lblBioInfo.Size = new System.Drawing.Size(44, 13);
            this.lblBioInfo.TabIndex = 3;
            this.lblBioInfo.Text = "Bio Info";
            // 
            // lblCorpInfo
            // 
            this.lblCorpInfo.AutoSize = true;
            this.lblCorpInfo.Location = new System.Drawing.Point(0, 31);
            this.lblCorpInfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCorpInfo.Name = "lblCorpInfo";
            this.lblCorpInfo.Size = new System.Drawing.Size(87, 13);
            this.lblCorpInfo.TabIndex = 4;
            this.lblCorpInfo.Text = "Corporation Info";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Location = new System.Drawing.Point(0, 44);
            this.lblBalance.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(92, 13);
            this.lblBalance.TabIndex = 5;
            this.lblBalance.Text = "Balance: 0.00 ISK";
            // 
            // lblWillpower
            // 
            this.lblWillpower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWillpower.AutoSize = true;
            this.lblWillpower.Location = new System.Drawing.Point(0, 112);
            this.lblWillpower.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblWillpower.Name = "lblWillpower";
            this.lblWillpower.Size = new System.Drawing.Size(62, 13);
            this.lblWillpower.TabIndex = 7;
            this.lblWillpower.Text = "0 Willpower";
            this.lblWillpower.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblMemory
            // 
            this.lblMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMemory.AutoSize = true;
            this.lblMemory.Location = new System.Drawing.Point(0, 99);
            this.lblMemory.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(54, 13);
            this.lblMemory.TabIndex = 8;
            this.lblMemory.Text = "0 Memory";
            this.lblMemory.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblPerception
            // 
            this.lblPerception.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPerception.AutoSize = true;
            this.lblPerception.Location = new System.Drawing.Point(0, 86);
            this.lblPerception.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblPerception.Name = "lblPerception";
            this.lblPerception.Size = new System.Drawing.Size(67, 13);
            this.lblPerception.TabIndex = 9;
            this.lblPerception.Text = "0 Perception";
            this.lblPerception.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblCharisma
            // 
            this.lblCharisma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCharisma.AutoSize = true;
            this.lblCharisma.Location = new System.Drawing.Point(0, 60);
            this.lblCharisma.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCharisma.Name = "lblCharisma";
            this.lblCharisma.Size = new System.Drawing.Size(60, 13);
            this.lblCharisma.TabIndex = 10;
            this.lblCharisma.Text = "0 Charisma";
            this.lblCharisma.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // lblIntelligence
            // 
            this.lblIntelligence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIntelligence.AutoSize = true;
            this.lblIntelligence.Location = new System.Drawing.Point(0, 73);
            this.lblIntelligence.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblIntelligence.Name = "lblIntelligence";
            this.lblIntelligence.Size = new System.Drawing.Size(71, 13);
            this.lblIntelligence.TabIndex = 11;
            this.lblIntelligence.Text = "0 Intelligence";
            this.lblIntelligence.MouseHover += new System.EventHandler(this.lblAttribute_MouseHover);
            // 
            // pnlTraining
            // 
            this.pnlTraining.AutoSize = true;
            this.pnlTraining.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlTraining.Controls.Add(this.tlpStatus);
            this.pnlTraining.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTraining.Location = new System.Drawing.Point(0, 457);
            this.pnlTraining.Name = "pnlTraining";
            this.pnlTraining.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.pnlTraining.Size = new System.Drawing.Size(392, 42);
            this.pnlTraining.TabIndex = 13;
            this.pnlTraining.Visible = false;
            // 
            // tlpStatus
            // 
            this.tlpStatus.AutoSize = true;
            this.tlpStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpStatus.ColumnCount = 3;
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStatus.Controls.Add(this.flpStatusLabels, 0, 0);
            this.tlpStatus.Controls.Add(this.flpStatusValues, 1, 0);
            this.tlpStatus.Controls.Add(this.btnAddToCalendar, 2, 0);
            this.tlpStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpStatus.Location = new System.Drawing.Point(0, 3);
            this.tlpStatus.Margin = new System.Windows.Forms.Padding(0);
            this.tlpStatus.Name = "tlpStatus";
            this.tlpStatus.RowCount = 1;
            this.tlpStatus.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpStatus.Size = new System.Drawing.Size(392, 39);
            this.tlpStatus.TabIndex = 4;
            // 
            // flpStatusLabels
            // 
            this.flpStatusLabels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpStatusLabels.AutoSize = true;
            this.flpStatusLabels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpStatusLabels.Controls.Add(this.lblCurrentlyTraining);
            this.flpStatusLabels.Controls.Add(this.lblSPPerHour);
            this.flpStatusLabels.Controls.Add(this.lblScheduleWarning);
            this.flpStatusLabels.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpStatusLabels.Location = new System.Drawing.Point(0, 0);
            this.flpStatusLabels.Margin = new System.Windows.Forms.Padding(0);
            this.flpStatusLabels.Name = "flpStatusLabels";
            this.flpStatusLabels.Size = new System.Drawing.Size(115, 39);
            this.flpStatusLabels.TabIndex = 15;
            this.flpStatusLabels.WrapContents = false;
            // 
            // lblCurrentlyTraining
            // 
            this.lblCurrentlyTraining.AutoSize = true;
            this.lblCurrentlyTraining.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentlyTraining.Location = new System.Drawing.Point(0, 0);
            this.lblCurrentlyTraining.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblCurrentlyTraining.Name = "lblCurrentlyTraining";
            this.lblCurrentlyTraining.Size = new System.Drawing.Size(112, 13);
            this.lblCurrentlyTraining.TabIndex = 0;
            this.lblCurrentlyTraining.Text = "Currently Training:";
            // 
            // lblSPPerHour
            // 
            this.lblSPPerHour.AutoSize = true;
            this.lblSPPerHour.Location = new System.Drawing.Point(0, 13);
            this.lblSPPerHour.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblSPPerHour.Name = "lblSPPerHour";
            this.lblSPPerHour.Size = new System.Drawing.Size(53, 13);
            this.lblSPPerHour.TabIndex = 1;
            this.lblSPPerHour.Text = "X sp/hour";
            // 
            // lblScheduleWarning
            // 
            this.lblScheduleWarning.AutoSize = true;
            this.lblScheduleWarning.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScheduleWarning.ForeColor = System.Drawing.Color.Red;
            this.lblScheduleWarning.Location = new System.Drawing.Point(0, 26);
            this.lblScheduleWarning.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblScheduleWarning.Name = "lblScheduleWarning";
            this.lblScheduleWarning.Size = new System.Drawing.Size(106, 13);
            this.lblScheduleWarning.TabIndex = 2;
            this.lblScheduleWarning.Text = "Schedule Conflict!";
            this.lblScheduleWarning.Visible = false;
            // 
            // flpStatusValues
            // 
            this.flpStatusValues.AutoSize = true;
            this.flpStatusValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpStatusValues.Controls.Add(this.lblTrainingSkill);
            this.flpStatusValues.Controls.Add(this.lblTrainingRemain);
            this.flpStatusValues.Controls.Add(this.lblTrainingEst);
            this.flpStatusValues.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpStatusValues.Location = new System.Drawing.Point(115, 0);
            this.flpStatusValues.Margin = new System.Windows.Forms.Padding(0);
            this.flpStatusValues.Name = "flpStatusValues";
            this.flpStatusValues.Size = new System.Drawing.Size(47, 39);
            this.flpStatusValues.TabIndex = 5;
            // 
            // lblTrainingSkill
            // 
            this.lblTrainingSkill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrainingSkill.AutoSize = true;
            this.lblTrainingSkill.Location = new System.Drawing.Point(0, 0);
            this.lblTrainingSkill.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblTrainingSkill.Name = "lblTrainingSkill";
            this.lblTrainingSkill.Size = new System.Drawing.Size(44, 13);
            this.lblTrainingSkill.TabIndex = 1;
            this.lblTrainingSkill.Text = "Nothing";
            // 
            // lblTrainingRemain
            // 
            this.lblTrainingRemain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrainingRemain.AutoSize = true;
            this.lblTrainingRemain.Location = new System.Drawing.Point(0, 13);
            this.lblTrainingRemain.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblTrainingRemain.Name = "lblTrainingRemain";
            this.lblTrainingRemain.Size = new System.Drawing.Size(11, 13);
            this.lblTrainingRemain.TabIndex = 2;
            this.lblTrainingRemain.Text = ".";
            // 
            // lblTrainingEst
            // 
            this.lblTrainingEst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrainingEst.AutoSize = true;
            this.lblTrainingEst.Location = new System.Drawing.Point(0, 26);
            this.lblTrainingEst.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblTrainingEst.Name = "lblTrainingEst";
            this.lblTrainingEst.Size = new System.Drawing.Size(11, 13);
            this.lblTrainingEst.TabIndex = 3;
            this.lblTrainingEst.Text = ".";
            // 
            // tmrUpdateCharacter
            // 
            this.tmrUpdateCharacter.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // pnlCharData
            // 
            this.pnlCharData.AutoSize = true;
            this.pnlCharData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlCharData.Controls.Add(this.tlpInfo);
            this.pnlCharData.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCharData.Location = new System.Drawing.Point(0, 0);
            this.pnlCharData.Name = "pnlCharData";
            this.pnlCharData.Size = new System.Drawing.Size(392, 184);
            this.pnlCharData.TabIndex = 14;
            // 
            // tlpInfo
            // 
            this.tlpInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpInfo.AutoSize = true;
            this.tlpInfo.ColumnCount = 3;
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpInfo.Controls.Add(this.flpThrobber, 2, 0);
            this.tlpInfo.Controls.Add(this.flpAttributes, 1, 1);
            this.tlpInfo.Controls.Add(this.flpButtons, 2, 2);
            this.tlpInfo.Controls.Add(this.flpCharacterInfo, 1, 0);
            this.tlpInfo.Controls.Add(this.pbCharImage, 0, 0);
            this.tlpInfo.Controls.Add(this.lblSkillHeader, 0, 2);
            this.tlpInfo.Location = new System.Drawing.Point(0, 0);
            this.tlpInfo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.tlpInfo.Name = "tlpInfo";
            this.tlpInfo.RowCount = 3;
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.Size = new System.Drawing.Size(392, 181);
            this.tlpInfo.TabIndex = 19;
            // 
            // flpThrobber
            // 
            this.flpThrobber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpThrobber.AutoSize = true;
            this.flpThrobber.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpThrobber.Controls.Add(this.throbber);
            this.flpThrobber.Controls.Add(this.lblUpdateTimer);
            this.flpThrobber.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpThrobber.Location = new System.Drawing.Point(361, 0);
            this.flpThrobber.Margin = new System.Windows.Forms.Padding(0);
            this.flpThrobber.Name = "flpThrobber";
            this.tlpInfo.SetRowSpan(this.flpThrobber, 2);
            this.flpThrobber.Size = new System.Drawing.Size(31, 37);
            this.flpThrobber.TabIndex = 15;
            this.flpThrobber.WrapContents = false;
            // 
            // throbber
            // 
            this.throbber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.throbber.BackColor = System.Drawing.Color.Transparent;
            this.throbber.ContextMenuStrip = this.cmsThrobberMenu;
            this.throbber.Location = new System.Drawing.Point(7, 0);
            this.throbber.Margin = new System.Windows.Forms.Padding(0);
            this.throbber.MaximumSize = new System.Drawing.Size(24, 24);
            this.throbber.MinimumSize = new System.Drawing.Size(24, 24);
            this.throbber.Name = "throbber";
            this.throbber.Size = new System.Drawing.Size(24, 24);
            this.throbber.State = EVEMon.Throbber.ThrobberState.Stopped;
            this.throbber.TabIndex = 18;
            this.throbber.TabStop = false;
            this.throbber.Text = "throbber1";
            this.ttToolTip.SetToolTip(this.throbber, "Click to update now.");
            // 
            // cmsThrobberMenu
            // 
            this.cmsThrobberMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHitEveO,
            this.miHitTrainingSkill,
            this.miChangeInfo});
            this.cmsThrobberMenu.Name = "cmsThrobberMenu";
            this.cmsThrobberMenu.Size = new System.Drawing.Size(233, 70);
            // 
            // miHitEveO
            // 
            this.miHitEveO.Name = "miHitEveO";
            this.miHitEveO.Size = new System.Drawing.Size(232, 22);
            this.miHitEveO.Text = "Get data from EVE Online";
            this.miHitEveO.Click += new System.EventHandler(this.miHitEveO_Click);
            // 
            // miHitTrainingSkill
            // 
            this.miHitTrainingSkill.Enabled = false;
            this.miHitTrainingSkill.Name = "miHitTrainingSkill";
            this.miHitTrainingSkill.Size = new System.Drawing.Size(232, 22);
            this.miHitTrainingSkill.Text = "Update Skill Training Info";
            this.miHitTrainingSkill.ToolTipText = "This is activated through a Timer.";
            this.miHitTrainingSkill.Click += new System.EventHandler(this.miHitTrainingSkill_Click);
            // 
            // miChangeInfo
            // 
            this.miChangeInfo.Name = "miChangeInfo";
            this.miChangeInfo.Size = new System.Drawing.Size(232, 22);
            this.miChangeInfo.Text = "Change API Key information...";
            this.miChangeInfo.Click += new System.EventHandler(this.miChangeInfo_Click);
            // 
            // lblUpdateTimer
            // 
            this.lblUpdateTimer.AutoSize = true;
            this.lblUpdateTimer.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblUpdateTimer.Location = new System.Drawing.Point(0, 24);
            this.lblUpdateTimer.Margin = new System.Windows.Forms.Padding(0);
            this.lblUpdateTimer.Name = "lblUpdateTimer";
            this.lblUpdateTimer.Size = new System.Drawing.Size(31, 13);
            this.lblUpdateTimer.TabIndex = 17;
            this.lblUpdateTimer.Text = "timer";
            this.lblUpdateTimer.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblUpdateTimer.Visible = false;
            // 
            // flpAttributes
            // 
            this.flpAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flpAttributes.AutoSize = true;
            this.flpAttributes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpAttributes.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flpAttributes.Location = new System.Drawing.Point(131, 132);
            this.flpAttributes.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.flpAttributes.Name = "flpAttributes";
            this.flpAttributes.Size = new System.Drawing.Size(0, 0);
            this.flpAttributes.TabIndex = 19;
            this.flpAttributes.WrapContents = false;
            // 
            // flpButtons
            // 
            this.flpButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flpButtons.AutoSize = true;
            this.flpButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpButtons.Controls.Add(this.llToggleAll);
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flpButtons.Location = new System.Drawing.Point(336, 162);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(56, 16);
            this.flpButtons.TabIndex = 20;
            // 
            // llToggleAll
            // 
            this.llToggleAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llToggleAll.AutoSize = true;
            this.llToggleAll.Location = new System.Drawing.Point(3, 3);
            this.llToggleAll.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.llToggleAll.Name = "llToggleAll";
            this.llToggleAll.Size = new System.Drawing.Size(53, 13);
            this.llToggleAll.TabIndex = 17;
            this.llToggleAll.TabStop = true;
            this.llToggleAll.Text = "Toggle All";
            this.llToggleAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llToggleAll_LinkClicked);
            // 
            // flpCharacterInfo
            // 
            this.flpCharacterInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.flpCharacterInfo.AutoSize = true;
            this.flpCharacterInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpCharacterInfo.Controls.Add(this.lblCharacterName);
            this.flpCharacterInfo.Controls.Add(this.lblBioInfo);
            this.flpCharacterInfo.Controls.Add(this.lblCorpInfo);
            this.flpCharacterInfo.Controls.Add(this.lblBalance);
            this.flpCharacterInfo.Controls.Add(this.lblCharisma);
            this.flpCharacterInfo.Controls.Add(this.lblIntelligence);
            this.flpCharacterInfo.Controls.Add(this.lblPerception);
            this.flpCharacterInfo.Controls.Add(this.lblMemory);
            this.flpCharacterInfo.Controls.Add(this.lblWillpower);
            this.flpCharacterInfo.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpCharacterInfo.Location = new System.Drawing.Point(131, 0);
            this.flpCharacterInfo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.flpCharacterInfo.Name = "flpCharacterInfo";
            this.flpCharacterInfo.Size = new System.Drawing.Size(132, 128);
            this.flpCharacterInfo.TabIndex = 18;
            this.flpCharacterInfo.WrapContents = false;
            // 
            // lblSkillHeader
            // 
            this.lblSkillHeader.AutoSize = true;
            this.tlpInfo.SetColumnSpan(this.lblSkillHeader, 2);
            this.lblSkillHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSkillHeader.Location = new System.Drawing.Point(0, 134);
            this.lblSkillHeader.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.lblSkillHeader.Name = "lblSkillHeader";
            this.lblSkillHeader.Size = new System.Drawing.Size(104, 39);
            this.lblSkillHeader.TabIndex = 14;
            this.lblSkillHeader.Text = "0 Known Skills\r\n0 Total SP\r\n0 Skills at Level V";
            this.lblSkillHeader.MouseHover += new System.EventHandler(this.lblSkillHeader_MouseHover);
            // 
            // tmrTick
            // 
            this.tmrTick.Interval = 1000;
            this.tmrTick.Tick += new System.EventHandler(this.tmrTick_Tick);
            // 
            // sfdSaveDialog
            // 
            this.sfdSaveDialog.Filter = "Text Format|*.txt|HTML Format|*.html|XML (Short) Format|*.xml|XML (Long) Format|*" +
                ".xml|PNG Image|*.png";
            this.sfdSaveDialog.Title = "Save Character Info";
            // 
            // ttToolTip
            // 
            this.ttToolTip.AutoPopDelay = 5000000;
            this.ttToolTip.InitialDelay = 500;
            this.ttToolTip.IsBalloon = true;
            this.ttToolTip.ReshowDelay = 100;
            // 
            // cmsPictureOptions
            // 
            this.cmsPictureOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updatePicture,
            this.updatePictureFromEVECache,
            this.setEVEFolder});
            this.cmsPictureOptions.Name = "contextMenuStrip1";
            this.cmsPictureOptions.Size = new System.Drawing.Size(241, 70);
            // 
            // updatePicture
            // 
            this.updatePicture.Name = "updatePicture";
            this.updatePicture.Size = new System.Drawing.Size(240, 22);
            this.updatePicture.Text = "Update Portrait From The Web";
            this.updatePicture.Click += new System.EventHandler(this.miUpdatePicture_Click);
            // 
            // updatePictureFromEVECache
            // 
            this.updatePictureFromEVECache.Name = "updatePictureFromEVECache";
            this.updatePictureFromEVECache.Size = new System.Drawing.Size(240, 22);
            this.updatePictureFromEVECache.Text = "Update Portrait From EVE Cache";
            this.updatePictureFromEVECache.Click += new System.EventHandler(this.miUpdatePictureFromEVECache_Click);
            // 
            // setEVEFolder
            // 
            this.setEVEFolder.Name = "setEVEFolder";
            this.setEVEFolder.Size = new System.Drawing.Size(240, 22);
            this.setEVEFolder.Text = "Set Portrait Folder";
            this.setEVEFolder.Click += new System.EventHandler(this.miSetEVEFolder_Click);
            // 
            // tmrMinTrainingSkillRetry
            // 
            this.tmrMinTrainingSkillRetry.Interval = 1000;
            this.tmrMinTrainingSkillRetry.Tick += new System.EventHandler(this.tmrMTSRTick);
            // 
            // lbSkills
            // 
            this.lbSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSkills.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbSkills.FormattingEnabled = true;
            this.lbSkills.IntegralHeight = false;
            this.lbSkills.ItemHeight = 15;
            this.lbSkills.Location = new System.Drawing.Point(0, 184);
            this.lbSkills.Name = "lbSkills";
            this.lbSkills.Size = new System.Drawing.Size(392, 273);
            this.lbSkills.TabIndex = 12;
            this.lbSkills.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.lbSkills_MouseWheel);
            this.lbSkills.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbSkills_DrawItem);
            this.lbSkills.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lbSkills_MeasureItem);
            this.lbSkills.MouseEnter += new System.EventHandler(this.lbSkills_MouseEnter);
            this.lbSkills.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbSkills_MouseDown);
            this.lbSkills.MouseLeave += new System.EventHandler(this.lbSkills_MouseLeave);
            // 
            // btnAddToCalendar
            // 
            this.btnAddToCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddToCalendar.Location = new System.Drawing.Point(280, 13);
            this.btnAddToCalendar.Name = "btnAddToCalendar";
            this.btnAddToCalendar.Size = new System.Drawing.Size(109, 23);
            this.btnAddToCalendar.TabIndex = 16;
            this.btnAddToCalendar.Text = "Update Calendar";
            this.btnAddToCalendar.UseVisualStyleBackColor = true;
            this.btnAddToCalendar.Click += new System.EventHandler(this.btnAddToCalendar_Click);
            // 
            // CharacterMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lbSkills);
            this.Controls.Add(this.pnlCharData);
            this.Controls.Add(this.pnlTraining);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CharacterMonitor";
            this.Size = new System.Drawing.Size(392, 499);
            ((System.ComponentModel.ISupportInitialize)(this.pbCharImage)).EndInit();
            this.pnlTraining.ResumeLayout(false);
            this.pnlTraining.PerformLayout();
            this.tlpStatus.ResumeLayout(false);
            this.tlpStatus.PerformLayout();
            this.flpStatusLabels.ResumeLayout(false);
            this.flpStatusLabels.PerformLayout();
            this.flpStatusValues.ResumeLayout(false);
            this.flpStatusValues.PerformLayout();
            this.pnlCharData.ResumeLayout(false);
            this.pnlCharData.PerformLayout();
            this.tlpInfo.ResumeLayout(false);
            this.tlpInfo.PerformLayout();
            this.flpThrobber.ResumeLayout(false);
            this.flpThrobber.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.throbber)).EndInit();
            this.cmsThrobberMenu.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.flpCharacterInfo.ResumeLayout(false);
            this.flpCharacterInfo.PerformLayout();
            this.cmsPictureOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbCharImage;
        private System.Windows.Forms.Label lblCharacterName;
        private System.Windows.Forms.Label lblBioInfo;
        private System.Windows.Forms.Label lblCorpInfo;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Label lblWillpower;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.Label lblPerception;
        private System.Windows.Forms.Label lblCharisma;
        private System.Windows.Forms.Label lblIntelligence;
        private EVEMon.NoFlickerListBox lbSkills;
        private System.Windows.Forms.Panel pnlTraining;
        private System.Windows.Forms.Label lblTrainingEst;
        private System.Windows.Forms.Label lblTrainingRemain;
        private System.Windows.Forms.Label lblTrainingSkill;
        private System.Windows.Forms.Label lblCurrentlyTraining;
        private System.Windows.Forms.Timer tmrUpdateCharacter;
        private System.Windows.Forms.Panel pnlCharData;
        private System.Windows.Forms.Timer tmrTick;
        private System.Windows.Forms.SaveFileDialog sfdSaveDialog;
        private System.Windows.Forms.Label lblSkillHeader;
        private System.Windows.Forms.ToolTip ttToolTip;
        private System.Windows.Forms.LinkLabel llToggleAll;
        private System.Windows.Forms.FlowLayoutPanel flpCharacterInfo;
        private System.Windows.Forms.TableLayoutPanel tlpInfo;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.TableLayoutPanel tlpStatus;
        private System.Windows.Forms.FlowLayoutPanel flpStatusValues;
        private System.Windows.Forms.FlowLayoutPanel flpAttributes;
        private System.Windows.Forms.FlowLayoutPanel flpThrobber;
        private System.Windows.Forms.Label lblUpdateTimer;
        private System.Windows.Forms.ContextMenuStrip cmsThrobberMenu;
        private System.Windows.Forms.ToolStripMenuItem miHitEveO;
        private System.Windows.Forms.ToolStripMenuItem miChangeInfo;
        private System.Windows.Forms.FlowLayoutPanel flpStatusLabels;
        private System.Windows.Forms.Label lblSPPerHour;
        private System.Windows.Forms.ToolStripMenuItem updatePicture;
        private System.Windows.Forms.ToolStripMenuItem updatePictureFromEVECache;
        private System.Windows.Forms.ToolStripMenuItem setEVEFolder;
        private System.Windows.Forms.ContextMenuStrip cmsPictureOptions;
        private System.Windows.Forms.Label lblScheduleWarning;
        private Throbber throbber;
        private System.Windows.Forms.Timer tmrMinTrainingSkillRetry;
        private System.Windows.Forms.ToolStripMenuItem miHitTrainingSkill;
        private System.Windows.Forms.Button btnAddToCalendar;
    }
}
