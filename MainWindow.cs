using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Sales;
using EVEMon.Common.Schedule;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Media;


namespace EVEMon
{
    public partial class MainWindow : EVEMonForm
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Settings m_settings;
        private bool startMinimized;
        private bool updateFlag;
        private bool dataUpdateFlag;
        private bool firstAPIRun;

        public MainWindow(Settings s, bool startMinimized)
            : this()
        {
            m_settings = s;
            this.startMinimized = startMinimized;
            this.updateFlag = false;
            this.firstAPIRun = CheckForFirstAPIRun();
        }

        private bool CheckForFirstAPIRun()
        {
            if (m_settings.CharacterList.Count > 0)
            {
                foreach (CharLoginInfo cli in m_settings.CharacterList)
                {
                    if (cli.ApiKey == string.Empty)
                        return true;
                }
            }
            return false;
        }

        private IGBService.IGBServer m_igbServer;

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            this.RememberPositionKey = "MainWindow";
            Program.MainWindow = this;
            niMinimizeIcon.Text = Application.ProductName;
            niMinimizeIcon.Visible = m_settings.SystemTrayOptionsIsAlways;
            StaticSkill.LoadStaticSkills();
            G15Handler.Init();
            if (!firstAPIRun)
            {
                AddCharacters();
            }
            if (m_settings.CheckTranquilityStatus)
            {
                EveServer server = EveServer.GetInstance();
                server.ServerStatusUpdated += new EventHandler<EveServerEventArgs>(UpdateServerStatusLabel);
                if (m_settings.ShowTQBalloon)
                {
                    server.ServerStatusChanged += new EventHandler<EveServerEventArgs>(ShowServerStatusBalloon);
                }
                server.StartTQChecks();
            }
            if (startMinimized)
            {
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;
                this.Visible = true;
            }
            menubarToolStripMenuItem.Checked = mainMenuBar.Visible = m_settings.MainWindowMenuBarVisible;
            standardToolStripMenuItem.Checked = standardToolbar.Visible = m_settings.MainWindowToolBarVisible;
        }

        public void CheckAccountTraining()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(CheckAccountTraining));
            }
            Dictionary<int, int> accountsInTraining = new Dictionary<int, int>();
            foreach (CharLoginInfo cli in m_settings.CharacterList)
            {
                CharacterInfo ci = GetCharacterInfo(cli.CharacterName);
                if (ci == null) continue;
                if (ci.IsTraining)
                {
                    if (accountsInTraining.ContainsKey(cli.UserId))
                    {
                        accountsInTraining[cli.UserId]++;
                    }
                    else
                    {
                        accountsInTraining.Add(cli.UserId, 1);
                    }
                }
                else
                {
                    if (!accountsInTraining.ContainsKey(cli.UserId))
                    {
                        accountsInTraining.Add(cli.UserId, 0);
                    }
                }
            }
            bool notTraining = false;
            string notTrainingString = string.Empty;
            foreach (int key in accountsInTraining.Keys)
            {
                if (accountsInTraining[key] == 0)
                {
                    if (notTraining)
                    {
                        notTrainingString += ", ";
                    }
                    notTraining = true;
                    string sep = "(";
                    notTrainingString = notTrainingString + key.ToString(); 
                    foreach (string charName in m_settings.GetCharacterNamesForAccount(key))
                    {
                        notTrainingString += sep;
                        notTrainingString += charName;
                        sep = ", ";
                    }
                    notTrainingString += ")";
                }
            }
            if (notTraining)
            {
                lblTraining.Image = SystemIcons.Warning.ToBitmap();
                ttMainWindow.SetToolTip(statusStrip, "Accounts " + notTrainingString + " do not have any characters in training");
                ttMainWindow.IsBalloon = false;
                ttMainWindow.Active = true;
                ttMainWindow.ReshowDelay = 1000;
            }
            else
            {
                lblTraining.Image = null;
                ttMainWindow.SetToolTip(statusStrip, null);
                ttMainWindow.Active = false;
            }
        }

        private void AddCharacters()
        {
            List<Object> tabOrder = m_settings.TabOrder;

            List<CharFileInfo> invalidFiles = new List<CharFileInfo>();
            foreach (Object o in tabOrder)
            {
                // o will be a CharLoginInfo or a CharFileInfo object
                if (o.GetType() == typeof(CharLoginInfo))
                {
                    CharLoginInfo cli = (CharLoginInfo)o;
                    AddTab(cli);
                }
                else if (o.GetType() == typeof(CharFileInfo))
                {
                    CharFileInfo cfi = (CharFileInfo)o;
                    if (!AddTab(cfi))
                    {
                        invalidFiles.Add(cfi);
                    }
                }
            }

            foreach (CharFileInfo cfi in invalidFiles)
            {
                RemoveCharFileInfo(cfi);
            }
            if (invalidFiles.Count > 0)
            {
                UpdateTabOrder();
            }

        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (firstAPIRun)
            {
                AddCharacters();
            }
            CheckAccountTraining();
            if (m_settings.DisableEVEMonVersionCheck == false)
            {
                UpdateManager um = UpdateManager.GetInstance();
                um.UpdateAvailable += new UpdateAvailableHandler(um_UpdateAvailable);
                um.DataUpdateAvailable += new DataUpdateAvailableHandler(um_DataUpdateAvailable);
                um.Start();
            }

#if !DEBUG
            InstanceManager im = InstanceManager.GetInstance();
            im.Signaled += new EventHandler<EventArgs>(im_Signaled);
#endif

            m_igbServer = new EVEMon.IGBService.IGBServer();
            m_settings.RunIGBServerChanged += new EventHandler<EventArgs>(m_settings_RunIGBServerChanged);
            m_settings_RunIGBServerChanged(null, null);

            m_settings.RelocateEveWindowChanged += new EventHandler<EventArgs>(m_settings_RelocateEveWindowChanged);
            m_settings_RelocateEveWindowChanged(null, null);
            m_settings.CheckTranquilityStatusChanged += new EventHandler<EventArgs>(m_settings_CheckTranquilityStatusChanged);

            TipWindow.ShowTip("startup",
                "Getting Started",
                "To begin using EVEMon, click the File|Add Character... menu option, " +
                "enter your CCP API information " +
                "and choose a character to monitor.");
        }

        void m_settings_ShowTQBalloonChanged(object sender, EventArgs e)
        {
            EveServer server = EveServer.GetInstance();
            if (m_settings.CheckTranquilityStatus && m_settings.ShowTQBalloon)
            {
                server.ServerStatusChanged += new EventHandler<EveServerEventArgs>(ShowServerStatusBalloon);
            }
            else
            {
                server.ServerStatusChanged -= new EventHandler<EveServerEventArgs>(ShowServerStatusBalloon);
            }
        }

        void m_settings_CheckTranquilityStatusChanged(object sender, EventArgs e)
        {
            EveServer server = EveServer.GetInstance();
            if (m_settings.CheckTranquilityStatus)
            {
                server.ServerStatusUpdated += new EventHandler<EveServerEventArgs>(UpdateServerStatusLabel);
                if (m_settings.ShowTQBalloon)
                {
                    server.ServerStatusChanged += new EventHandler<EveServerEventArgs>(ShowServerStatusBalloon);
                }
                server.StartTQChecks();
            }
            else
            {
                server.StopTQChecks();
                server.ServerStatusUpdated -= new EventHandler<EveServerEventArgs>(UpdateServerStatusLabel);
                server.ServerStatusChanged -= new EventHandler<EveServerEventArgs>(ShowServerStatusBalloon);
            }
        }

        void m_settings_RelocateEveWindowChanged(object sender, EventArgs e)
        {
            Program.SetRelocatorState(m_settings.RelocateEveWindow);
        }

        private void m_settings_RunIGBServerChanged(object sender, EventArgs e)
        {
            if (m_settings.RunIGBServer)
            {
                //reset the settings, if necessary
                m_igbServer.Stop();
                m_igbServer.Reset(m_settings.IGBServerPublic, m_settings.IGBServerPort);
                m_igbServer.Start();
            }
            else
            {
                m_igbServer.Stop();
            }
        }

        void im_Signaled(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    im_Signaled(sender, e);
                }));
                return;
            }

            if (!this.Visible)
            {
                niMinimizeIcon_Click(this, new EventArgs());
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.BringToFront();
            }

            FlashWindow(this.Handle, true);
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        private bool m_updateShowing = false;

        private void um_UpdateAvailable(object sender, UpdateAvailableEventArgs e)
        {
            if (e.NewestVersion <= new Version(m_settings.IgnoreUpdateVersion))
            {
                return;
            }

            this.Invoke(new MethodInvoker(delegate
            {
                if (!m_updateShowing)
                {
                    m_updateShowing = true;
                    using (UpdateNotifyForm f = new UpdateNotifyForm(m_settings, e))
                    {
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            this.updateFlag = true;
                            m_settings.SaveImmediate();
                            this.Close();
                        }
                    }
                    m_updateShowing = false;
                }
            }));
        }


        private bool m_dataUpdateShowing = false;
        /// <summary>
        /// Event Handler for Data updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void um_DataUpdateAvailable(object sender, DataUpdateAvailableEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                if (!m_dataUpdateShowing)
                {
                    m_dataUpdateShowing = true;
                    using (DataUpdateNotifyForm f = new DataUpdateNotifyForm(m_settings, e))
                    {
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            this.dataUpdateFlag = true;
                            m_settings.SaveImmediate();
                            this.Close();
                        }
                    }
                    m_dataUpdateShowing = false;
                }
            }));
        }


        /// <summary>
        /// Add a tab to the form for a file based character
        /// </summary>
        /// <param name="cfi">The object containing the character info</param>
        /// <param name="silent">if true, prompt if the file is missing, else remove the character silently</param>
        /// <returns>true if the character was added</returns>
        private bool AddTab(CharFileInfo cfi)
        {
            SerializableCharacterSheet sci = SerializableCharacterSheet.CreateFromFile(cfi.Filename);
            if (sci == null)
            {
                MessageBox.Show("Unable to get character info from file.",
                        "Unable To Get Character Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            cfi.CharacterName = sci.CharacterSheet.Name;
            CharacterMonitor cm = new CharacterMonitor(cfi, sci);
            AddTab(cfi, "(File) " + sci.CharacterSheet.Name, cm);
            return true;
        }

        /// <summary>
        /// Adds a tab for an online based character
        /// </summary>
        /// <param name="cli">Object representing the character data</param>
        /// <returns>true if the tab was added ok</returns>
        private bool AddTab(CharLoginInfo cli)
        {
            if (cli.ApiKey == string.Empty || cli.UserId == 0)
            {
                if (firstAPIRun)
                {
                    firstAPIRun = false;
                    TipWindow.ShowTip("First run with API interface",
                        "First Run",
                        "EVEMon has detected a pre-API version of a settings file. At least one of your characters does not have an API key yet. Your original pre-api settings file is backed up as settings.xml.preapi. You will now be prompted for the API keys for each of your accounts.");
                    m_settings.BackupOldSettingsFile();

                }
                // No API info found - ask user - must be first run.

                // see if we can find an api key from an already added character

                using (ChangeLoginWindow f = new ChangeLoginWindow())
                {
                    f.ShowInvalidKey = true;
                    f.CharacterName = cli.CharacterName;
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        cli.UserId = f.UserId;
                        cli.ApiKey = f.ApiKey;
                        // now patch this API key into any other characters on the same account
                        foreach (CharLoginInfo li in m_settings.CharacterList)
                        {
                            if (cli.Username != string.Empty && li.Username == cli.Username && li.CharacterName != cli.CharacterName)
                            {
                                li.ApiKey = cli.ApiKey;
                                li.UserId = cli.UserId;
                                li.Username = String.Empty;
                            }
                        }
                        cli.Username = string.Empty;
                        m_settings.Save();

                        EveSession.GetSession(f.UserId, f.ApiKey);
                    }
                }
            }
            CharacterMonitor cm = new CharacterMonitor(cli);
            AddTab(cli, cli.CharacterName, cm);
            return true;
        }

        /// <summary>
        /// Common method used to add either file based or online based character tab
        /// This is where we actally create the tab and kick off the update thread
        /// </summary>
        /// <param name="charInfo">either a file based on online based info object</param>
        /// <param name="title">Titke for the tab  - "character name" or "(file) character name"</param>
        /// <param name="cm">The Character Monitor object to attach to this tab</param>
        private void AddTab(object charInfo, string title, CharacterMonitor cm)
        {
            TabPage tp = new TabPage(title);
            tp.UseVisualStyleBackColor = true;
            tp.Tag = charInfo;
            tp.Padding = new Padding(5);
            cm.Parent = tp;
            cm.Dock = DockStyle.Fill;
            cm.LCDDataChanged += new EventHandler(cm_ShortInfoChanged);

            cm.Start();
            tcCharacterTabs.TabPages.Add(tp);
            cm.GrandCharacterInfo.DownloadAttemptCompleted += new CharacterInfo.DownloadAttemptCompletedHandler(cm_DownloadAttemptCompleted);
            cm.GrandCharacterInfo.TrainingSkillChanged += new EventHandler(GrandCharacterInfo_TrainingSkillChanged);
            SetRemoveEnable();
        }

        private void UpdateTabOrder()
        {
            List<String> tabOrder = new List<string>();
            foreach (TabPage tp in tcCharacterTabs.TabPages)
            {
                if (tp.Tag.GetType() == typeof(CharFileInfo))
                {
                    CharFileInfo cfi = (CharFileInfo)tp.Tag;
                    tabOrder.Add(cfi.CharacterName);
                }
                else if (tp.Tag.GetType() == typeof(CharLoginInfo))
                {
                    CharLoginInfo cli = (CharLoginInfo)tp.Tag;
                    tabOrder.Add(cli.CharacterName);
                }
            }
            m_settings.TabOrderName = tabOrder;
        }

        private void cm_ShortInfoChanged(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                StringBuilder tsb = new StringBuilder();

                if (m_settings.TitleToTime)
                {
                    SortedList<TimeSpan, CharacterInfo> gcis = gcisByTimeSpan();
                    int selectedCharId = (tcCharacterTabs.SelectedTab.Controls[0] as CharacterMonitor).GrandCharacterInfo.CharacterId;

                    //there are real optimization opportunities here - this gets updated every second

                    foreach (TimeSpan ts in gcis.Keys)
                    {
                        CharacterInfo gci = gcis[ts];

                        string gciTimeSpanText = Skill.TimeSpanToDescriptiveText(ts, DescriptiveTextOptions.Default) + " " + gci.Name
                            + (m_settings.TitleToTimeSkill == true ? " (" + gci.CurrentlyTrainingSkill.Name + ")" : "");
                        switch (m_settings.TitleToTimeLayout)
                        {
                            case 1: // single Char - finishing skill next
                                if (tsb.Length == 0)
                                {
                                    tsb.Append(gciTimeSpanText);
                                }
                                break;
                            case 2: // single Char - selected char
                                if (selectedCharId == gci.CharacterId)
                                {
                                    tsb.Append(gciTimeSpanText);
                                }
                                break;
                            case 0: //this is the default
                            case 3: // multi Char - finishing skill next first
                                tsb.Append(tsb.Length > 0 ? " | " : String.Empty).Append(gciTimeSpanText);
                                break;
                            case 4: // multi Char - selected char first
                                if (selectedCharId == gci.CharacterId)
                                {
                                    tsb.Insert(0, gciTimeSpanText + (tsb.Length > 0 ? " | " : String.Empty));
                                }
                                else
                                {
                                    tsb.Append(tsb.Length > 0 ? " | " : String.Empty).Append(gciTimeSpanText);
                                }
                                break;
                        }
                    }
                }

                tsb.Append(tsb.Length > 0 ? " - " : String.Empty).Append(Application.ProductName);
                this.Text = tsb.ToString();
            }));
        }

        // Formats everything on the tooltip that isn't changed every second.
        private string FormatTooltipText(string fmt, SortedList<TimeSpan, CharacterInfo> gcis)
        {
            StringBuilder sb = new StringBuilder();

            if (String.IsNullOrEmpty(fmt) || gcis == null)
            {
                if (String.Empty.Equals(fmt))
                {
                    sb.Append("You can configure this tooltip in the options/general panel");
                }
                return sb.ToString();
            }

            foreach (CharacterInfo gci in gcis.Values)
            {
                if (sb.Length != 0) sb.Append("\n");
                sb.Append(Regex.Replace(fmt, "%([nbsdr]|[ct][ir])", new MatchEvaluator(delegate(Match m)
                {
                    string value = String.Empty;
                    char capture = m.Groups[1].Value[0];

                    if (capture == 'n')
                    {
                        value = gci.Name;
                    }
                    else if (capture == 'b')
                    {
                        value = gci.Balance.ToString("#,##0.00");
                    }
                    else if (capture == 's')
                    {
                        value = gci.CurrentlyTrainingSkill.Name;
                    }
                    else if (capture == 'd')
                    {
                        value = gci.CurrentlyTrainingSkill.EstimatedCompletion.ToString("g");
                    }
                    else if (capture == 'r')
                    {
                        value = '%' + gci.CharacterId.ToString() + 'r';
                    }
                    else
                    {
                        int level = -1;
                        if (capture == 'c')
                        {
                            level = gci.CurrentlyTrainingSkill.Level;
                        }
                        else if (capture == 't')
                        {
                            level = gci.CurrentlyTrainingSkill.TrainingToLevel;
                        }

                        if (m.Groups[1].Value.Length > 1 && level >= 0)
                        {
                            capture = m.Groups[1].Value[1];

                            if (capture == 'i')
                            {
                                value = level.ToString();
                            }
                            else if (capture == 'r')
                            {
                                value = Skill.GetRomanForInt(level);
                            }
                        }
                    }

                    return value;
                }), RegexOptions.Compiled));
            }
            return sb.ToString();
        }

        // Pulled this code out of cm_ShortInfoChanged, as I needed to use the returned List in multiple places
        private SortedList<TimeSpan, CharacterInfo> gcisByTimeSpan()
        {
            //selectedCharId = 0;

            SortedList<TimeSpan, CharacterInfo> gcis = new SortedList<TimeSpan, CharacterInfo>();
            foreach (TabPage tp in tcCharacterTabs.TabPages)
            {
                CharacterMonitor cm = tp.Controls[0] as CharacterMonitor;
                if (cm != null && cm.GrandCharacterInfo != null && cm.GrandCharacterInfo.CurrentlyTrainingSkill != null && cm.ShortTimeSpan != null)
                {
                    CharacterInfo gci = cm.GrandCharacterInfo;

                    //Use the TimeSpan from current CharacterMonitor, since it is freshly re-calculated every timer tick
                    //in 'CharacterMonitor.CalcSkillRemainText' just a few steps up the call stack.
                    TimeSpan ts = cm.ShortTimeSpan;

                    if (ts > TimeSpan.Zero)
                    {
                        while (gcis.ContainsKey(ts))
                        {
                            ts = ts + TimeSpan.FromMilliseconds(1);
                        }
                        gcis.Add(ts, gci);
                    }

                    //if (tcCharacterTabsNew.SelectedTab.Text.Equals(tp.Text))
                    //selectedCharId = gci.CharacterId;
                }
            }
            return gcis;
        }

        private List<string> m_completedSkills = new List<string>();
        private void GrandCharacterInfo_TrainingSkillChanged(object sender, EventArgs e)
        {
            CheckAccountTraining();
        }

        private void cm_DownloadAttemptCompleted(object sender, CharacterInfo.DownloadAttemptCompletedEventArgs e)
        {
            CheckAccountTraining();
            this.Invoke(new MethodInvoker(delegate
            {
                bool ShouldbeSilent = false;
                for (int i = 0; i < m_settings.Schedule.Count; i++)
                {
                    ScheduleEntry temp = m_settings.Schedule[i];
                    if (temp.GetType() == typeof(SimpleScheduleEntry))
                    {
                        SimpleScheduleEntry x = (SimpleScheduleEntry)temp;
                        if (x.Silent(DateTime.Now))
                        {
                            ShouldbeSilent = true;
                            break;
                        }
                    }
                    else if (temp.GetType() == typeof(RecurringScheduleEntry))
                    {
                        RecurringScheduleEntry x = (RecurringScheduleEntry)temp;
                        if (x.Silent(DateTime.Now))
                        {
                            ShouldbeSilent = true;
                            break;
                        }
                    }
                }

                if (e.Complete)
                {
                    CharacterInfo ci = GetCharacterInfo(e.CharacterName);
                    
                    if (m_settings.PlaySoundOnSkillComplete && !ShouldbeSilent)
                    {

                        string skilltrained = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "SkillTrained.wav";
                        using (SoundPlayer sp = new SoundPlayer(skilltrained))
                            sp.Play();
                    }

                    int skillLevel = ci.GetSkill(e.SkillName).Level;

                    if (m_settings.EnableBalloonTips)
                    {
                        string skillLevelString = Skill.GetRomanForInt(skillLevel);
                        string sa = e.CharacterName + " has finished learning " + e.SkillName + " " + skillLevelString + ".";

                        m_completedSkills.Add(sa);
                        if (m_completedSkills.Count > 1)
                        {
                            sa = m_completedSkills.Count.ToString() + " skills completed. Click for more info.";
                        }
                        ShowBalloonTip("EVEMon - Skill Training Completed", "Skill Training Completed", sa);

                        tmrAlertRefresh.Enabled = false;
                        tmrAlertRefresh.Interval = 60000;
                        tmrAlertRefresh.Enabled = true;
                    }

                    if (m_settings.EnableEmailAlert)
                    {
                        Emailer.SendAlertMail(m_settings, skillLevel, e.SkillName, ci);
                    }
                }
                else
                {
                    if (m_settings.EnableBalloonTips)
                    {
                        string sa = e.CharacterName + " has finished learning " + e.SkillName + ".";
                        if (m_completedSkills.Contains(sa))
                        {
                            m_completedSkills.Remove(sa);
                            if (m_completedSkills.Count == 0)
                            {
                                // need to disable the alert and associated stuff
                                niAlertIcon.Visible = false;
                                tmrAlertRefresh.Enabled = false;
                            }
                            if (m_completedSkills.Count == 1)
                            {
                                niAlertIcon.BalloonTipText = m_completedSkills[0];
                            }
                            else if (m_completedSkills.Count > 1)
                            {
                                niAlertIcon.BalloonTipText = m_completedSkills.Count.ToString() + " skills completed. Click for more info.";
                            }
                        }
                    }
                }
            }));
        }


        private void SetRemoveEnable()
        {
            G15Handler.CharListUpdate();
            removeCharacterToolStripMenuItem.Enabled =
                tsbRemoveChar.Enabled =
                    tsdbPlans.Enabled = (tcCharacterTabs.TabPages.Count > 0);
        }

        private void RemoveTab(TabPage tp)
        {
            CharacterMonitor cm = tp.Controls[0] as CharacterMonitor;
            if (cm != null)
            {
                cm.Stop();
            }
            string name = string.Empty;
            cm.LCDDataChanged -= new EventHandler(cm_ShortInfoChanged);
            tcCharacterTabs.TabPages.Remove(tp);
            if (tp.Tag is CharLoginInfo)
            {
                CharLoginInfo cli = tp.Tag as CharLoginInfo;
                name = cli.CharacterName;
                m_settings.CharacterList.Remove(cli);
                if (!m_settings.KeepCharacterPlans)
                {
                    m_settings.RemoveAllPlansFor(cli.CharacterName);
                }
                m_settings.RemoveCharacterCache(cli.CharacterName);
                UpdateTabOrder();
            }
            else if (tp.Tag is CharFileInfo)
            {
                CharFileInfo cfi = tp.Tag as CharFileInfo;
                name = cfi.CharacterName;
                RemoveCharFileInfo(cfi);
                UpdateTabOrder();
            }
            List<Pair<string, string>> toRemove = new List<Pair<string, string>>();
            foreach (Pair<string, string> grp in m_settings.CollapsedGroups)
            {
                if (grp.A == name)
                {
                    toRemove.Add(grp);
                }
            }
            foreach (Pair<string, string> grp in toRemove)
            {
                m_settings.CollapsedGroups.Remove(grp);
            }
            cm.GrandCharacterInfo.DownloadAttemptCompleted -= new CharacterInfo.DownloadAttemptCompletedHandler(cm_DownloadAttemptCompleted);
            SetRemoveEnable();
            CheckAccountTraining();
        }

        private void RemoveCharFileInfo(CharFileInfo cfi)
        {
            m_settings.CharFileList.Remove(cfi);
            if (!m_settings.KeepCharacterPlans)
            {
                m_settings.RemoveAllPlansFor(cfi.Filename);
            }
            m_settings.RemoveCharacterCache(cfi.CharacterName);
            m_settings.Save();
        }

        private void addCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (LoginCharSelect f = new LoginCharSelect())
            {
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    if (f.IsLogin)
                    {
                        int uid = 0;
                        try
                        {
                            uid = Int32.Parse(f.UserId);
                        }
                        catch (Exception) { }
                        foreach (string charName in f.CharsToAdd)
                        {
                            CharLoginInfo cli = new CharLoginInfo();
                            cli.UserId = uid;
                            cli.ApiKey = f.ApiKey;
                            cli.CharacterName = charName;
                            if (m_settings.AddCharacter(cli))
                            {
                                AddTab(cli);
                                UpdateTabOrder();
                            }
                        }
                    }
                    else if (f.IsFile)
                    {
                        CharFileInfo cfi = new CharFileInfo();
                        cfi.Filename = f.FileName;
                        cfi.MonitorFile = f.MonitorFile;
                        if (m_settings.AddFileCharacter(cfi))
                        {
                            AddTab(cfi);
                            UpdateTabOrder();
                        }
                    }
                }
            }
        }

        private void removeCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage activeTab = tcCharacterTabs.SelectedTab;
            string confirmMsg = "Are you sure you want to remove \"" + activeTab.Text + "\"?\n";
            if (m_settings.KeepCharacterPlans)
            {
                confirmMsg += "Your plans will be kept. If you want plans to be deleted, ";
            }
            else
            {
                confirmMsg += "Your plans will be deleted. If you want plans to be kept, ";
            }
            confirmMsg += "\nthen please change the \"Keep plans when character is deleted\" setting on the \"Updates\" settings panel.";
            DialogResult dr =
                MessageBox.Show(confirmMsg,
                "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                RemoveTab(activeTab);
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsForm sf = new SettingsForm(m_settings))
            {
                sf.ShowDialog();
            }
            niMinimizeIcon.Visible = m_settings.SystemTrayOptionsIsAlways;
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && !m_settings.SystemTrayOptionsIsNever)
            {
                niMinimizeIcon.Visible = true;
                this.Visible = false;
            }
            else
            {
                if (this.tcCharacterTabs != null && this.tcCharacterTabs.CanSelect)
                {
                    TabPage tp = this.tcCharacterTabs.SelectedTab;
                    if (tp != null)
                    {
                        CharacterMonitor current = tp.Controls[0] as CharacterMonitor;
                        if (current != null)
                        {
                            current.ForceLcdDataUpdate();
                        }
                    }
                }
            }
        }

        public void ShowBalloonTip(string iconTitle, string title, string message)
        {
            ShowBalloonTip(iconTitle, title, message, ToolTipIcon.Info);
        }

        public void ShowBalloonTip(string iconTitle, string title, string message, ToolTipIcon icon)
        {
            if (m_settings.CheckTranquilityStatus)
            {
                EveServer.GetInstance().PendingAlerts = true;
            }
            niAlertIcon.Text = iconTitle;
            niAlertIcon.BalloonTipTitle = title;
            niAlertIcon.BalloonTipText = message;
            niAlertIcon.BalloonTipIcon = icon;
            niAlertIcon.Visible = true;
            niAlertIcon.ShowBalloonTip(30000);
        }

        private void niMinimizeIcon_Click(object sender, EventArgs e)
        {
            if (((e as MouseEventArgs) == null) || ((e as MouseEventArgs) != null && (e as MouseEventArgs).Button != MouseButtons.Right))
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Activate();
                this.niMinimizeIcon.Visible = m_settings.SystemTrayOptionsIsAlways;
            }

            if (this.m_tooltipWindow != null && (this.m_tooltipWindow.IsAlive && this.m_tooltipWindow.Target != null))
            {
                this.m_tooltipWindow.Target.Visible = false;
            }
        }

        private void tmrAlertRefresh_Tick(object sender, EventArgs e)
        {
            tmrAlertRefresh.Enabled = false;
            if (m_settings.EnableBalloonTips && niAlertIcon.Visible)
            {
                niAlertIcon.ShowBalloonTip(30000);
                tmrAlertRefresh.Interval = 60000;
                tmrAlertRefresh.Enabled = true;
            }
        }

        private void niAlertIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            AlertIconClick();
        }

        private void niAlertIcon_Click(object sender, EventArgs e)
        {
            AlertIconClick();
        }

        private void niAlertIcon_MouseClick(object sender, MouseEventArgs e)
        {
            AlertIconClick();
        }

        private void AlertIconClick()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(AlertIconClick));
                return;
            }

            if (m_completedSkills.Count > 0 || EveServer.GetInstance().PendingAlerts)
            {
                niAlertIcon.Visible = false;
                tmrAlertRefresh.Enabled = false;
                //  always show the completed skils box if multiple skills have completed
                // otherwise user sees a "multiple skills completed, click for more info" message and the click does nothing!
                if ((m_completedSkills.Count > 0  && m_settings.EnableSkillCompleteDialog) || m_completedSkills.Count > 1)
                {
                    SkillCompleteDialog f = new SkillCompleteDialog(m_completedSkills);
                    f.FormClosed += delegate { f.Dispose(); };
                    f.Show();
                    f.Activate();
                }
                EveServer.GetInstance().PendingAlerts = false;
                m_completedSkills.Clear();
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_settings.SystemTrayOptionsIsNever &&                 // if system tray icon is always or display when minimised
                m_settings.CloseToTray &&                               // and EVEMon is configured to close to system tray
                this.Visible &&                                         // and main form is visable
                !this.updateFlag &&                                     // and code auto-update not currently in process   
                !this.dataUpdateFlag &&                                 // and data auto-update not currently in process
                !(e.CloseReason == CloseReason.ApplicationExitCall) &&  // and Application.Exit() was not called
                !(e.CloseReason == CloseReason.TaskManagerClosing) &&  // and the user isn't trying to shut the program down for some reason
                !(e.CloseReason == CloseReason.WindowsShutDown)  // and Windows is not shutting down
               )
            {
                e.Cancel = true; // Cancel the close operation
                niMinimizeIcon.Visible = true; // Display the minimize icon
                this.Visible = false; // hide the main form
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateManager um = UpdateManager.GetInstance();
            if (m_settings.DisableEVEMonVersionCheck == false && um.IsRunning)
            {
                um.Stop();
                um.UpdateAvailable -= new UpdateAvailableHandler(um_UpdateAvailable);
                um.DataUpdateAvailable -= new DataUpdateAvailableHandler(um_DataUpdateAvailable);
            }

            m_settings.RunIGBServerChanged -= new EventHandler<EventArgs>(m_settings_RunIGBServerChanged);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Emailer.SendAlertMail(m_settings, "Industry", "Anders Chydenius");
            using (AboutWindow f = new AboutWindow())
            {
                f.ShowDialog();
            }
        }

        public CharacterInfo GetCharacterInfo(string charName)
        {
            foreach (TabPage tp in tcCharacterTabs.TabPages)
            {
                CharacterMonitor cm = tp.Controls[0] as CharacterMonitor;
                CharacterInfo gci = cm.GrandCharacterInfo;
                if (gci != null && gci.Name == charName)
                {
                    return gci;
                }
            }
            return null;
        }

        public CharacterMonitor GetCharacterMonitor(string charName)
        {
            foreach (TabPage tp in tcCharacterTabs.TabPages)
            {
                CharacterMonitor cm = tp.Controls[0] as CharacterMonitor;
                CharacterInfo gci = cm.GrandCharacterInfo;
                if (gci != null && gci.Name == charName)
                {
                    return cm;
                }
            }
            return null;
        }

        private WeakReference<TrayTooltipWindow> m_tooltipWindow = null;

        private bool m_inMinIconMouseMove = false;

        private void niMinimizeIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_inMinIconMouseMove && trayIconToolStrip.Visible == false)
            {
                m_inMinIconMouseMove = true;
                try
                {
                    TrayTooltipWindow ttw = null;
                    if (m_tooltipWindow != null)
                    {
                        ttw = m_tooltipWindow.Target;
                    }

                    if (ttw == null)
                    {
                        // Prevent the icon's text property from showing in a default tooltip
                        niMinimizeIcon.Text = "";

                        ttw = new TrayTooltipWindow();
                        m_tooltipWindow = new WeakReference<TrayTooltipWindow>(ttw);
                        ttw.FormClosed += delegate
                        {
                            // Restore the icon's text since we need it there normally in order to allow the icon to
                            // be shown or hidden via the "Customize Notifications..." Windows setting
                            niMinimizeIcon.Text = Application.ProductName;

                            m_tooltipText = Application.ProductName;
                            m_tooltipWindow = null;
                            ttw.Dispose();
                        };

                        //Mostly format the tooltip text, using the current contents of m_tooltipText as the first line.
                        SortedList<TimeSpan, CharacterInfo> gcis = gcisByTimeSpan();
                        m_tooltipText = FormatTooltipText(m_settings.TooltipString, gcis);

                        //Splice in the estimated time till completion.
                        string tooltipText = m_tooltipText;
                        foreach (TimeSpan ts in gcis.Keys)
                        {
                            tooltipText = Regex.Replace(tooltipText, '%' + gcis[ts].CharacterId.ToString() + 'r',
                                Skill.TimeSpanToDescriptiveText(ts, DescriptiveTextOptions.IncludeCommas), RegexOptions.Compiled);
                        }

                        ttw.Text = tooltipText;
                        ttw.Show();
                    }
                    else
                    {
                        ttw.RefreshAlive();
                    }
                }
                finally
                {
                    m_inMinIconMouseMove = false;
                }
            }
        }

        private string m_tooltipText = Application.ProductName;

        private void SetMinimizedIconTooltipText(string txt)
        {
            TrayTooltipWindow ttw = null;
            if (m_tooltipWindow != null)
            {
                ttw = m_tooltipWindow.Target;
            }

            if (ttw != null)
            {
                ttw.Text = txt;
            }
            //m_tooltipText = txt;
        }

        private WeakReference<Schedule.ScheduleEditorWindow> m_scheduler;

        private void schedulerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Schedule.ScheduleEditorWindow sched = null;

            if (m_scheduler != null)
            {
                sched = m_scheduler.Target;
                if (sched != null)
                {
                    try
                    {
                        if (sched.Visible)
                            sched.BringToFront();
                        else
                            sched = null;
                    }
                    catch (ObjectDisposedException ex)
                    {
                        ExceptionHandler.LogException(ex, true);
                        sched = null;
                        m_scheduler = null;
                    }
                }
            }
            if (sched == null)
            {
                sched = new EVEMon.Schedule.ScheduleEditorWindow(m_settings);
                sched.Show();
                m_scheduler = new WeakReference<EVEMon.Schedule.ScheduleEditorWindow>(sched);
            }
        }

        private void mineralWorksheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MineralWorksheet ws = new MineralWorksheet(m_settings);
            ws.FormClosed += delegate
            {
                ws.Dispose();
            };
            ws.Show();
        }


        private void UpdateServerStatusLabel(object sender, EveServerEventArgs e)
        {
            lblServerStatus.Text = e.info;
        }

        private void ShowServerStatusBalloon(object sender, EveServerEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    ShowServerStatusBalloon(sender, e);
                }));
                return;
            }
            if (m_settings.ShowTQBalloon)
            {
                ShowBalloonTip("EVEMon - Server Status Information", "Server Status Information", e.info, e.icon);
            }
            tmrAlertRefresh.Enabled = false;
            tmrAlertRefresh.Interval = 60000;
            tmrAlertRefresh.Enabled = true;
        }

        private void UpdateStatusLabel()
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTimeFormatInfo fi = CultureInfo.CurrentCulture.DateTimeFormat;
            lblStatus.Text = "Current EVE Time: " + now.ToString(fi.ShortDatePattern + " HH:mm");
        }

        private void tmrClock_Tick(object sender, EventArgs e)
        {
            tmrTranquilityClock.Enabled = false;
            UpdateStatusLabel();
            tmrTranquilityClock.Interval = 60000;//1 minute
            tmrTranquilityClock.Enabled = true;
        }

        //private void tmrLCD_Tick(object sender, EventArgs e)
        //{
        //    if (Program.LCD != null)
        //    {
        //        Program.LCD.GetButtonState();
        //        tmrLCDClock.Enabled = true;
        //        tmrLCDClock.Interval = 50;
        //    }
        //    else
        //    {
        //        tmrLCDClock.Enabled = true;
        //        tmrLCDClock.Interval = 1000;
        //    }
        //}

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
            this.niMinimizeIcon.Visible = m_settings.SystemTrayOptionsIsAlways;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UpdateTabVisibility(object sender, EventArgs e)
        {
            foreach (TabPage p in tcCharacterTabs.TabPages)
            {
                CharacterMonitor cm = p.Controls[0] as CharacterMonitor;
                if (p == tcCharacterTabs.SelectedTab && cm != null)
                {
                    cm.CurrentlyVisible = true;
                }
                else
                {
                    cm.CurrentlyVisible = false;
                }
            }
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case FormWindowState.Minimized:
                    foreach (TabPage tab in tcCharacterTabs.TabPages)
                    {
                        CharacterMonitor cm = tab.Controls[0] as CharacterMonitor;
                        cm.CurrentlyVisible = false;
                    }
                    break;
                case FormWindowState.Normal:
                    foreach (TabPage tab in tcCharacterTabs.TabPages)
                    {
                        CharacterMonitor cm = tab.Controls[0] as CharacterMonitor;
                        if (tab == tcCharacterTabs.SelectedTab && cm != null)
                        {
                            cm.CurrentlyVisible = true;
                        }
                        else
                        {
                            cm.CurrentlyVisible = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        private void resetCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Manually delete the Settings file for any non-recoverble errors.
            DialogResult dr = MessageBox.Show("Are you sure you want to reset the cache, all settings will be lost including plans?",
                "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                // Take note of what the Keep Character Plans setting is.
                bool tempKeepPlans = m_settings.KeepCharacterPlans;
                m_settings.KeepCharacterPlans = false;
                // Run through any characters that are currently loaded.
                foreach (TabPage tab in tcCharacterTabs.TabPages)
                {
                    RemoveTab(tab);
                }
                // Reset the settings. Settings are resaved upon exiting the application, no need to
                // change this.
                m_settings.KeepCharacterPlans = tempKeepPlans;

                if (m_settings.ResetCache())
                {
                    MessageBox.Show("EVEMon cache reset successfully", "Cache Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Text = "EVEMon";
                    SetMinimizedIconTooltipText(this.Text);
                }
                else
                {
                    MessageBox.Show("Problem resetting EVEMon cache", "Cache Reset", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void trayIconToolStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            planToolStripMenuItem.DropDownItems.Clear();
            List<string> characters = new List<string>();
            foreach (CharLoginInfo cli in m_settings.CharacterList)
            {
                if (cli != null)
                {
                    characters.Add(cli.CharacterName);
                }
            }
            foreach (CharFileInfo cfi in m_settings.CharFileList)
            {
                if (cfi != null)
                {
                    characters.Add(cfi.CharacterName + "|" + cfi.Filename);
                }
            }
            characters.Sort();
            foreach (string character in characters)
            {
                char[] split = { '|' };
                string[] characterInfo = character.Split(split);
                string planLookup = characterInfo[characterInfo.Length - 1];
                ToolStripMenuItem characterItem = new ToolStripMenuItem(characterInfo[0]);
                characterItem.Tag = planLookup;
                foreach (string planName in m_settings.GetPlansForCharacter(planLookup))
                {
                    ToolStripMenuItem planItem = new ToolStripMenuItem(planName);
                    planItem.Click += new EventHandler(planItem_Click);
                    characterItem.DropDownItems.Add(planItem);
                }
                planToolStripMenuItem.DropDownItems.Add(characterItem);
            }
        }

        void planItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem planItem = (ToolStripMenuItem)sender;
            Plan plan = m_settings.GetPlanByName((string)planItem.OwnerItem.Tag, GetCharacterInfo(planItem.OwnerItem.Text), planItem.Text);
            plan.ShowEditor(m_settings, GetCharacterInfo(planItem.OwnerItem.Text));
        }

        private void UpdateTabVisibility(object sender, ControlEventArgs e)
        {
            foreach (TabPage p in tcCharacterTabs.TabPages)
            {
                CharacterMonitor cm = p.Controls[0] as CharacterMonitor;
                if (p == tcCharacterTabs.SelectedTab && cm != null)
                {
                    cm.CurrentlyVisible = true;
                }
                else
                {
                    cm.CurrentlyVisible = false;
                }
            }

        }

        // we've changed the tab order, so let's reset it
        private void tcCharacterTabs_DragDrop(object sender, DragEventArgs e)
        {
            UpdateTabOrder();
        }

        private void MainWindow_Deactivate(object sender, EventArgs e)
        {
            // Only cleanup if we're deactivating to the minimized state (e.g. systray)
            if (this.WindowState == FormWindowState.Minimized)
            {
                AutoShrink.Dirty(new TimeSpan(0, 0, 0, 0, 500)); // Clean up after 500 ms
            }
        }

        private void skillsPieChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tcCharacterTabs.SelectedTab == null) return;
            TabPage activeTab = tcCharacterTabs.SelectedTab;
            SkillsPieChart pie = new SkillsPieChart();

            if (activeTab.Tag is CharLoginInfo)
            {
                CharLoginInfo cli = activeTab.Tag as CharLoginInfo;
                pie.active_character = cli.CharacterName;
            }
            else if (activeTab.Tag is CharFileInfo)
            {
                CharFileInfo cfi = activeTab.Tag as CharFileInfo;
                pie.active_character = cfi.CharacterName;
            }

            CharacterMonitor cm = GetCharacterMonitor(pie.active_character);
            pie.character_info = cm.GrandCharacterInfo;
            pie.plan_key = cm.GetPlanKey();
            pie.Text = "Skillgroup chart for " + pie.active_character;
            pie.Show();
        }

        private string m_currntDirectory = String.Empty;
        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_currntDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToString();
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.CopySettings(saveFileDialog.FileName);
            // restore the working directory to orginal startup directory
            Directory.SetCurrentDirectory(m_currntDirectory);
        }

        private void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            while (tcCharacterTabs.TabPages.Count > 0)
            {
                RemoveTab(tcCharacterTabs.TabPages[0]);
            }
            Directory.SetCurrentDirectory(m_currntDirectory);
            m_settings = Settings.Restore(openFileDialog.FileName);
            m_settings.Save();
            AddCharacters();
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_currntDirectory = Directory.GetCurrentDirectory();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToString();
            openFileDialog.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public CharacterMonitor GetCurrentCharacter()
        {
            if (tcCharacterTabs.SelectedTab == null) return null;
            return tcCharacterTabs.SelectedTab.Controls[0] as CharacterMonitor;
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                cm.SaveCharacterXML();
            }
        }

        private void manualImplantGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                cm.ShowManualImplantGroups();
            }
        }

        private void plansToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            while (plansToolStripMenuItem.DropDownItems.Count > 3)
            {
                plansToolStripMenuItem.DropDownItems.RemoveAt(3);
            }
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm == null)
            {
                plansToolStripMenuItem.DropDownItems[0].Enabled =
                    plansToolStripMenuItem.DropDownItems[1].Enabled =
                        plansToolStripMenuItem.DropDownItems[2].Visible = false;
            }
            else
            {
                plansToolStripMenuItem.DropDownItems[0].Enabled =
                    plansToolStripMenuItem.DropDownItems[1].Enabled =
                        plansToolStripMenuItem.DropDownItems[2].Visible = true;
                String planKey = cm.GetPlanKey();
                foreach (string plan in m_settings.GetPlansForCharacter(planKey))
                {
                    ToolStripMenuItem menuPlanItem = new ToolStripMenuItem(plan);
                    menuPlanItem.Click += delegate(object o, EventArgs ev)
                    {
                        ToolStripMenuItem item = o as ToolStripMenuItem;
                        cm.ShowPlanEditor(item.Text);
                    };
                    plansToolStripMenuItem.DropDownItems.Add(menuPlanItem);
                }
            }
        }

        private void manageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                cm.ShowPlanSelectWindow();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                cm.ShowNewPlanWindow();
            }
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                inEvenetToolStripMenuItem.Enabled = true;
                inEvenetToolStripMenuItem.Checked = m_settings.GetCharacterSettings(cm.CharacterName).IneveSync;
                sendToInEveToolStripMenuItem.Enabled = inEvenetToolStripMenuItem.Checked;
            }
            else
            {
                inEvenetToolStripMenuItem.Enabled = false;
                sendToInEveToolStripMenuItem.Enabled = false;
            }
        }

        private void showOwnedSkillbooksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                cm.ShowOwnedSkillbooks();
            }
        }

        private void inEvenetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                if (!m_settings.GetCharacterSettings(cm.CharacterName).IneveSync)
                {
                    DialogResult dr =  MessageBox.Show("Warning! You are about to enable the sharing of your skill data with an external website.\nWe cannot take responsibility for what happens to that data - other people may be able to access your skill data.\nAre you sure you want to do this?", "Skill Data Sharing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == DialogResult.No)
                    {
                        inEvenetToolStripMenuItem.Checked = false;
                        return;
                    }
                }
                m_settings.GetCharacterSettings(cm.CharacterName).IneveSync = !m_settings.GetCharacterSettings(cm.CharacterName).IneveSync;
            }
        }

        private void sendToInEveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                cm.Session.UpdateIneveAsync(cm.GrandCharacterInfo);
            }
        }

        private void menubarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainMenuBar.Visible = !mainMenuBar.Visible;
            m_settings.MainWindowMenuBarVisible = mainMenuBar.Visible;
        }

        private void standardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            standardToolbar.Visible = !standardToolbar.Visible;
            m_settings.MainWindowToolBarVisible = standardToolbar.Visible;
        }

        private void tsdbPlans_DropDownOpening(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            if (cm != null)
            {
                tsdbPlans.DropDownItems.Clear();
                String planKey = cm.GetPlanKey();
                foreach (string plan in m_settings.GetPlansForCharacter(planKey))
                {
                    ToolStripMenuItem menuPlanItem = new ToolStripMenuItem(plan);
                    menuPlanItem.Click += delegate(object o, EventArgs ev)
                    {
                        //CharacterMonitor cm = tcCharacterTabs.SelectedTab.Controls[0] as CharacterMonitor;
                        ToolStripMenuItem item = o as ToolStripMenuItem;
                        cm.ShowPlanEditor(item.Text);
                    };
                    tsdbPlans.DropDownItems.Add(menuPlanItem);
                }
            }
        }

        private void toolbarContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            menubarToolStripMenuItem.Enabled = standardToolbar.Visible;
            standardToolStripMenuItem.Enabled = mainMenuBar.Visible;
        }

         private void copySkillsToClipboardBBFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterMonitor cm = GetCurrentCharacter();
            cm.CopyBBCodeToClipBoard();
        }


    }
}







