//#define DEBUG_SINGLETHREAD
// (If setting DEBUG_SINGLE THREAD, also set it in EVESession.cs)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using EVEMon.Common;
using EVEMon.Common.Schedule;
using EVEMon.SkillPlanner;
using EVEMon.ImpGroups;
using System.Web;

namespace EVEMon
{
    public partial class CharacterMonitor : UserControl
    {
        private Settings m_settings;
        private CharLoginInfo m_cli;
        private SerializableCharacterSheet m_sci;
        private CharFileInfo m_cfi;
        private EveSession m_session;
        private int m_charId;
        private FileSystemWatcher m_characterFileWatch = null;
        private Dictionary<SkillGroup, bool> m_groupCollapsed = new Dictionary<SkillGroup, bool>();
        private DateTime m_nextScheduledUpdateAt = DateTime.MinValue;
        private string m_skillTrainingName;
        private DateTime m_estimatedCompletion;
        private CharacterInfo m_grandCharacterInfo;
        private int m_lastTickSPPaint = 0;
        private bool m_updatingPortrait = false;
        private bool m_currentlyVisible = false;

        //This is data that would be shown ON the LCD display, not data ABOUT the LCD display
        public EventHandler LCDDataChanged;
        private string m_lcdNote = String.Empty;
        private TimeSpan m_lcdTimeSpan = TimeSpan.Zero;

        //file formats for saving your character
        private enum SaveFormat
        {
            None = 0,
            Text = 1,
            Html = 2,
            Xml = 3,
            OldXml = 4,
            Png = 5,
        }

        private CharacterMonitor()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            m_settings = Settings.GetInstance();
            throbber.Click += new EventHandler(throbber_Click);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMonitor"/> class.  For eve-o based characters
        /// </summary>
        /// <param name="s">The EVEMon settings.</param>
        /// <param name="cli">The character login info.</param>
        public CharacterMonitor(CharLoginInfo cli)
            : this()
        {
            m_cli = cli;
            m_sci = null;
            m_cfi = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMonitor"/> class.  For file based characters
        /// </summary>
        /// <param name="s">The EVEMon settings.</param>
        /// <param name="cfi">The character file info.</param>
        /// <param name="sci">The serializable character, loaded from the file.</param>
        public CharacterMonitor(CharFileInfo cfi, SerializableCharacterSheet sci)
            : this()
        {
            m_cli = null;
            m_sci = sci;
            m_cfi = cfi;
        }

        public string CharacterName
        {
            get
            {
                return m_charName;
            }
        }

        private string m_charName
        {
            get
            {
                if (m_grandCharacterInfo != null && m_grandCharacterInfo.Name != null)
                {
                    return m_grandCharacterInfo.Name;
                }
                else
                {
                    return "?";
                }
            }
        }

        /// <summary>
        /// Starts the monitor.  Sets up the GrandCharacterInfo, attempts to get the image, starts all the timers and, if needed,
        /// the FileSystemWatcher
        /// <remarks>This method can probably be broken up quite a bit, for simplicity's sake</remarks>
        /// </summary>
        public void Start()
        {
            m_session = null;
            m_charId = -1;

            if (m_cli != null)
            {
                // Quick fix for when characters aren't in the accounts list
                // Needs a proper refactoring of account and character data management
                int userID = m_cli.Account == null ? m_cli.UserId : m_cli.Account.UserId;
                m_grandCharacterInfo = new CharacterInfo(userID, m_charId, m_cli.CharacterName);
            }
            else
            {
                m_charId = m_sci.CharacterSheet.CharacterId;
                m_grandCharacterInfo = new CharacterInfo(m_sci.CharacterSheet.CharacterId, m_sci.CharacterSheet.Name);
                if (m_charId > 0)
                {
                    GetCharacterImage();
                }
                if (m_cfi.MonitorFile)
                {
                    m_characterFileWatch = new FileSystemWatcher(Path.GetDirectoryName(m_cfi.Filename), Path.GetFileName(m_cfi.Filename));
                    m_characterFileWatch.Created += new FileSystemEventHandler(CharacterFileUpdatedCallback);
                    m_characterFileWatch.Changed += new FileSystemEventHandler(CharacterFileUpdatedCallback);
                    m_characterFileWatch.EnableRaisingEvents = true;
                }
            }
            m_grandCharacterInfo.BioInfoChanged += new EventHandler(BioInfoChangedCallback);
            m_grandCharacterInfo.BalanceChanged += new EventHandler(BalanceChangedCallback);
            m_grandCharacterInfo.AttributeChanged += new EventHandler(AttributeChangedCallback);
            m_grandCharacterInfo.SkillChanged += new SkillChangedHandler(CharacterSkillChangedCallback);
            m_grandCharacterInfo.TrainingSkillChanged += new EventHandler(TrainingSkillChangedCallback);
            m_settings.ScheduleEntriesChanged += new EventHandler<EventArgs>(m_settings_ScheduleEntriesChanged);

            m_settings_ScheduleEntriesChanged(null, null);

            if (m_cli != null)
            {
                SerializableCharacterSheet sci = m_settings.GetCharacterSheet(m_cli.CharacterName);
                if (sci != null)
                {
                    m_grandCharacterInfo.AssignFromSerializableCharacterSheet(sci, m_settings.ShowAllPublicSkills, m_settings.ShowNonPublicSkills, m_settings.SkillPlannerHighlightPartialSkills);
                }
                if (m_settings.DisableXMLAutoUpdate == false)
                {
#if DEBUG_SINGLETHREAD

                    tmrUpdate_Tick(this, new EventArgs());
#else
                    tmrUpdateCharacter.Interval = 10;
                    tmrUpdateCharacter.Enabled = true;
#endif
                }
                else
                {
                    tmrUpdateCharacter.Enabled = false;
                    //throbber.Visible = false;
                }
            }
            else
            {
                tmrUpdateCharacter.Enabled = false;
                throbber.Visible = false;
                m_grandCharacterInfo.AssignFromSerializableCharacterSheet(m_sci, m_settings.ShowAllPublicSkills, m_settings.ShowNonPublicSkills, m_settings.SkillPlannerHighlightPartialSkills);
                m_sci = null;
            }

            foreach (SkillGroup gsg in m_grandCharacterInfo.SkillGroups.Values)
            {
                foreach (Pair<string, string> grp in m_settings.CollapsedGroups)
                {
                    if ((grp.A == m_grandCharacterInfo.Name) && (grp.B == gsg.Name))
                    {
                        if (!m_groupCollapsed.ContainsKey(gsg))
                        {
                            m_groupCollapsed.Add(gsg, true);
                            foreach (Skill gs in gsg)
                            {
                                try
                                {
                                    lbSkills.Items.RemoveAt(lbSkills.Items.IndexOf(gs));
                                }
                                catch
                                { }
                            }

                            gsg.isCollapsed = true;
                        }
                    }
                }
            }

            tmrTick.Enabled = true;

            m_settings.WorksafeChanged += new EventHandler<EventArgs>(WorksafeChangedCallback);
            WorksafeChangedCallback(null, null);

            m_settings.UseLogitechG15DisplayChanged += new EventHandler<EventArgs>(UpdateLcdDisplayCallback);
            //m_settings.NotificationOffsetChanged += new EventHandler<EventArgs>(m_settings_NotificationOffsetChanged);
            btnAddToCalendar.Visible = m_settings.UseExternalCalendar;
        }

        /// <summary>
        /// Stop all timers, unhook all events, lie down and die.
        /// </summary>
        public void Stop()
        {
            if (m_session != null)
            {
                m_session = null;
            }
            tmrTick.Enabled = false;
            tmrUpdateCharacter.Enabled = false;
            if (m_characterFileWatch != null)
            {
                m_characterFileWatch.EnableRaisingEvents = false;
            }

            m_grandCharacterInfo.BioInfoChanged -= new EventHandler(BioInfoChangedCallback);
            m_grandCharacterInfo.BalanceChanged -= new EventHandler(BalanceChangedCallback);

            m_grandCharacterInfo.AttributeChanged -= new EventHandler(AttributeChangedCallback);
            m_grandCharacterInfo.SkillChanged -= new SkillChangedHandler(CharacterSkillChangedCallback);
            m_grandCharacterInfo.TrainingSkillChanged -= new EventHandler(TrainingSkillChangedCallback);

            m_settings.WorksafeChanged -= new EventHandler<EventArgs>(WorksafeChangedCallback);
            m_settings.UseLogitechG15DisplayChanged -= new EventHandler<EventArgs>(UpdateLcdDisplayCallback);
            m_settings.ScheduleEntriesChanged -= new EventHandler<EventArgs>(m_settings_ScheduleEntriesChanged);
            //m_settings.NotificationOffsetChanged -= new EventHandler<EventArgs>(m_settings_NotificationOffsetChanged);
        }

        void m_settings_ScheduleEntriesChanged(object sender, EventArgs e)
        {
            // Here is where we check and change the alert on schedule collision
            if (m_grandCharacterInfo.CurrentlyTrainingSkill != null && m_grandCharacterInfo.CurrentlyTrainingSkill.InTraining)
            {
                Skill gs = m_grandCharacterInfo.CurrentlyTrainingSkill;

                string conflictText = String.Empty;
                bool isBlocked = m_settings.SkillIsBlockedAt(m_estimatedCompletion, out conflictText);
                lblScheduleWarning.Text = conflictText;
                lblScheduleWarning.Visible = isBlocked;
            }
        }

        private void ReloadFromFile()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(ReloadFromFile));
                return;
            }

            SerializableCharacterSheet sci = SerializableCharacterSheet.CreateFromFile(m_cfi.Filename);
            if (sci != null)
            {
                m_grandCharacterInfo.AssignFromSerializableCharacterSheet(sci, m_settings.ShowAllPublicSkills, m_settings.ShowNonPublicSkills, m_settings.SkillPlannerHighlightPartialSkills);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the panel is currently visible
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool CurrentlyVisible
        {
            get
            {
                return m_currentlyVisible;
            }
            set
            {
                bool old_value = m_currentlyVisible;
                m_currentlyVisible = value;
                UpdateLcdisplay();
                if (m_currentlyVisible != old_value && m_currentlyVisible)
                {
                    UpdateSkillHeaderStats();
                }
            }
        }

        /// <summary>
        /// Updates skill summary info (the block just below the portrait).
        /// </summary>
        private void UpdateSkillHeaderStats()
        {
            lblSkillHeader.Text = String.Format("{0} Known Skills\n{1} Total SP\n{2} Skills at Level V",
                                                m_grandCharacterInfo.KnownSkillCount,
                                                m_grandCharacterInfo.SkillPointTotal.ToString("#,##0"),
                                                m_grandCharacterInfo.SkillCountAtLevel(5));
        }

        /// <summary>
        /// Sets the specified attribute label.
        /// </summary>
        /// <param name="lblAttrib">The LBL attrib.</param>
        /// <param name="eveAttribute">The eve attribute.</param>
        private void SetAttributeLabel(Label lblAttrib, EveAttribute eveAttribute)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(eveAttribute.ToString());
            sb.Append(": ");
            sb.Append(m_grandCharacterInfo.GetEffectiveAttribute(eveAttribute).ToString("0.00"));
            lblAttrib.Text = sb.ToString();
        }

        /// <summary>
        /// Updates the cached copy.
        /// </summary>
        /// <remarks>This makes sure the settings file only has you down with one "last skill trained", then saves the settings file</remarks>
        private void UpdateCachedCopy()
        {
            SerializableCharacterSheet sci = m_grandCharacterInfo.ExportSerializableCharacterSheet();
            m_settings.SetCharacterCache(sci);
            if (m_grandCharacterInfo.OldSerialSIT != null && m_grandCharacterInfo.AllSkillsByTypeID.ContainsKey(m_grandCharacterInfo.OldSerialSIT.TrainingSkillWithTypeID))
            {
                bool toremove = false;
                bool add = true;
                if (m_settings.OldSAPIkillsDict.ContainsKey(m_charName))
                {
                    SerializableSkillTrainingInfo x = m_settings.OldSAPIkillsDict[m_charName];
                    if (x.TrainingSkillWithTypeID == m_grandCharacterInfo.OldSerialSIT.TrainingSkillWithTypeID && x.TrainingSkillToLevel == m_grandCharacterInfo.OldSerialSIT.TrainingSkillToLevel)
                    {
                        add = false;
                        m_settings.OldSAPIkillsDict[m_charName] = (SerializableSkillTrainingInfo)m_grandCharacterInfo.OldSerialSIT.Clone();
                        if (!x.AlertRaisedAlready)
                        {
                            toremove = true;
                        }
                    }
                    else
                    {
                        toremove = true;
                    }
                    if (toremove)
                    {
                        m_settings.OldSAPIkillsDict.Remove(m_charName);
                    }
                }
                if (add && m_grandCharacterInfo.OldSerialSIT.AlertRaisedAlready)
                {
                    m_settings.OldSAPIkillsDict[m_charName] = (SerializableSkillTrainingInfo)m_grandCharacterInfo.OldSerialSIT.Clone();
                }
            }
            m_settings.Save();
        }

        private void DownloadCharacter()
        {

#if DEBUG_SINGLETHREAD
            GetCharIdAndUpdateInternal(null);
#else
            ThreadPool.QueueUserWorkItem(new WaitCallback(GetCharIdAndUpdateInternal));
#endif
        }

        /// <summary>
        /// Actually download and update the character
        /// </summary>
        /// <param name="state">Does nothing, provided for compliance with WaitCallback</param>
        private void GetCharIdAndUpdateInternal(object state)
        {
            //get the character ID
            int charId = -1;
            try
            {
                m_session = EveSession.GetSession(m_cli.Account.UserId, m_cli.Account.ApiKey);
                charId = m_session.GetCharacterId(m_cli.CharacterName);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }
            if (charId < 0)
            {
                this.Invoke(new MethodInvoker(CharacterDownloadFailedCallback));
                return;
            }

            m_grandCharacterInfo.CharacterId = charId;
            GetCharacterImage();

            //actually perform the update.  This is then happening asynchronously, is this a problem?
            UpdateGrandCharacterInfo();

            if (m_settings.OldSAPIkillsDict.ContainsKey(m_charName))
            {
                m_grandCharacterInfo.OldSerialSIT = (SerializableSkillTrainingInfo)m_settings.OldSAPIkillsDict[m_charName].Clone();
            }

            UpdateTrainingSkillInfo();
        }

        private void UpdateGrandCharacterInfo()
        {
            m_session.UpdateGrandCharacterInfoAsync(m_grandCharacterInfo, Program.MainWindow,
                                                    new UpdateGrandCharacterInfoCallback(GrandCharacterUpdatedCallback));
        }


#if DEBUG_SINGLETHREAD
        private void GrandCharacterUpdatedCallback(EveSession s, int timeLeftInCache)
        {

        }
#else

        /// <summary>
        /// Updates the throbber timer when finished updating
        /// </summary>
        /// <param name="s">The eve session - not used.</param>
        /// <param name="timeLeftInCache">The time left in cache - that is, the time before the next update to the sheet on eve-o.</param>
        private void GrandCharacterUpdatedCallback(EveSession s, int timeLeftInCache)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                if (m_session.ApiErrorCode == 0)
                {
                    m_nextScheduledUpdateAt = DateTime.Now + TimeSpan.FromMilliseconds(timeLeftInCache);
                    ttToolTip.SetToolTip(throbber, "Click to update now.");
                    ttToolTip.IsBalloon = true;
                    //timeLeftInCache == 0 is the same as timeLeftInCache == 60 minutes.
                    tmrUpdateCharacter.Interval = timeLeftInCache == 0 ? 3600000 : timeLeftInCache;
                    if (m_settings.DisableXMLAutoUpdate == false)
                    {
                        tmrUpdateCharacter.Enabled = true;
                    }

                    if (tmrMinTrainingSkillRetry.Enabled == false)
                    {
                        miHitTrainingSkill.Enabled = true;
                        m_canUpdateSkills = true;
                    }

                    StopThrobber();
                }

                else
                {
                    StopThrobber();
                    CharacterDownloadFailedCallback();
                }

            }));
        }
#endif

        private void UpdateTrainingSkillInfo()
        {
            m_session.UpdateSkillTrainingInfoAsync(m_grandCharacterInfo, Program.MainWindow,
                                                    new UpdateTrainingSkillInfoCallback(TrainingSkillUpdatedCallback));
        }

        /// <summary>
        /// Updates the Training Skill timer when finished updating
        /// </summary>
        /// <param name="s">The eve session - not used.</param>
        /// <param name="timeLeftInCache">The time left in cache - that is, the minimum time before the xml shows what skill you are training.</param>
        private void TrainingSkillUpdatedCallback(EveSession s, int timeToNextUpdate)
        {
#if DEBUG_SINGLETHREAD
            m_canUpdateSkills = false;
#else

            this.Invoke(new MethodInvoker(delegate
            {
                m_canUpdateSkills = false;
                miHitTrainingSkill.Enabled = false;
                if (timeToNextUpdate != -1) // Only reason the timer would = -1 would be if the Character login details were correct, but didn't have this char on that account.
                {
                    tmrMinTrainingSkillRetry.Interval = timeToNextUpdate <= 0 ? 900000 : timeToNextUpdate;
                    tmrMinTrainingSkillRetry.Enabled = true;
                    DateTime nextUpdate = DateTime.Now + new TimeSpan(0, 0, (timeToNextUpdate / 1000));
                    miHitTrainingSkill.ToolTipText = "This is activated through a Timer. (Next update at " + nextUpdate.ToLongTimeString() + ")";
                }
            }));
#endif
        }

        private bool m_canUpdateSkills = false;

        private void tmrMTSRTick(object sender, EventArgs e)
        {
            tmrMinTrainingSkillRetry.Enabled = false;
            if (m_grandCharacterInfo.CharacterId < 0)
            {
                tmrMinTrainingSkillRetry.Interval = 300000;
                tmrMinTrainingSkillRetry.Enabled = true;
                miHitTrainingSkill.ToolTipText = "You had an issue with connecting.\nPlease wait 5 minutes and try again.";
                return;
            }
            else
            {
                miHitTrainingSkill.Enabled = true;
                m_canUpdateSkills = true;
                miHitTrainingSkill.ToolTipText = "Timer has expired.\nPlease click to attempt download of training info.";
            }
        }

        private void miHitTrainingSkill_Click(object sender, EventArgs e)
        {
            // Update the currently Training Skill Info here, if the timer allows.
            if (m_canUpdateSkills)
            {
                UpdateTrainingSkillInfo();
            }
        }

        /// <summary>
        /// Creates a public callable function to update the characterinfo
        /// </summary>
        public void UpdateCharacterInfo()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateCharacterInfo));
                return;
            }
            try
            {
                if (throbber.State != Throbber.ThrobberState.Strobing)
                {
                    tmrUpdate_Tick(null, null);
                    UpdateThrobberLabel();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Updates the LCD Display if G15 support is on
        /// </summary>
        private void UpdateLcdisplay()
        {
            // all moved into G15Handler
        }

        /// <summary>
        /// Calculate the skill text and time remaining for LCD Display
        /// </summary>
        private void CalculateLcdData()
        {
            DateTime now = DateTime.Now;
            if (m_estimatedCompletion != DateTime.MaxValue)
            {
                if (m_estimatedCompletion > now)
                {
                    SetLcdData(m_charName + ": " + TimeSpanDescriptiveShort(m_estimatedCompletion), 
                        m_estimatedCompletion.ToUniversalTime() - now.ToUniversalTime());
                }
                else
                {
                    SetLcdData(m_charName + ": Done", TimeSpan.Zero);
                }
            }
            else
            {
                SetLcdData(String.Empty, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Update labels in the bottom panel for time remaining
        /// </summary>
        private void UpdateTimeRemainingLabels()
        {
            DateTime now = DateTime.Now;
            if (m_estimatedCompletion != DateTime.MaxValue)
            {
                lblTrainingRemain.Text = TimeSpanDescriptive(m_estimatedCompletion);
                if (m_estimatedCompletion > now)
                {
                    lblTrainingEst.Text = m_estimatedCompletion.ToString("ddd ") + m_estimatedCompletion.ToString();
                }
                else
                {
                    lblTrainingEst.Text = String.Empty;
                }
            }
            else
            {
                lblTrainingRemain.Text = String.Empty;
                lblTrainingEst.Text = String.Empty;
            }
        }

        /// <summary>
        /// Sets the note and the timespan used by the LCDdisplay functions
        /// </summary>
        /// <param name="newShortText">The new short text.</param>
        /// <param name="timeSpan">The time span.</param>
        private void SetLcdData(string newShortText, TimeSpan timeSpan)
        {
            bool fireEvent = false;
            if (newShortText != m_lcdNote || timeSpan != m_lcdTimeSpan)
            {
                fireEvent = true;
            }
            m_lcdNote = newShortText;
            m_lcdTimeSpan = timeSpan;
            if (fireEvent && LCDDataChanged != null)
            {
                LCDDataChanged(this, new EventArgs());
            }
            UpdateLcdisplay();
        }

        /// <summary>
        /// Returns a short timespan string between now and the DateTime given... which is in the future.
        /// If someone comes up with a clean way to whack this with string.format, go for it... as it is, it's a custom format, and not one
        /// that we can throw out, so this needs to stay.  It's not SO bad.
        /// Changes to this should also touch TimeSpanDescriptive.
        /// </summary>
        /// <param name="t">The time (in the future) for which you want a span.</param>
        /// <returns>Format:XdXh0Xm0Xs</returns>
        public static string TimeSpanDescriptiveShort(DateTime t)
        {
            StringBuilder sb = new StringBuilder();
            if (t > DateTime.Now)
            {
                TimeSpan ts = t.ToUniversalTime() - DateTime.Now.ToUniversalTime();
                if (ts.Days > 0)
                {
                    sb.Append(ts.Days.ToString());
                    sb.Append("d");
                }
                ts -= TimeSpan.FromDays(ts.Days);
                if (ts.Hours > 0)
                {
                    sb.Append(ts.Hours.ToString());
                    sb.Append("h");
                }
                ts -= TimeSpan.FromHours(ts.Hours);
                if (ts.Minutes > 0)
                {
                    sb.Append(ts.Minutes.ToString());
                    sb.Append("m");
                }
                ts -= TimeSpan.FromMinutes(ts.Minutes);
                if (ts.Seconds > 0)
                {
                    sb.Append(ts.Seconds.ToString());
                    sb.Append("s");
                }
                return sb.ToString();
            }
            else
            {
                return "Done";
            }
        }

        /// <summary>
        /// Returns a long format timespan string between now and the DateTime given, which should be in the future.
        /// If someone comes up with a clean way to whack this with string.format, go for it... as it is, it's a custom format, and not one
        /// that we can throw out, so this needs to stay.  It's not SO bad.
        /// Changes to this should also touch TimeSpanDescriptiveShort.
        /// </summary>
        /// <param name="t">The time (in the future) for which you want a span</param>
        /// <returns>Format: X day(x), X hour(s), X minute(s), X second(s)</returns>
        public static string TimeSpanDescriptive(DateTime t)
        {
            StringBuilder sb = new StringBuilder();
            if (t > DateTime.Now)
            {
                TimeSpan ts = t.ToUniversalTime() - DateTime.Now.ToUniversalTime();
                if (ts.Days > 0)
                {
                    sb.Append(ts.Days.ToString());
                    sb.Append(" day");
                    if (ts.Days > 1)
                    {
                        sb.Append("s");
                    }
                }
                ts -= TimeSpan.FromDays(ts.Days);
                if (ts.Hours > 0)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ts.Hours.ToString());
                    sb.Append(" hour");
                    if (ts.Hours > 1)
                    {
                        sb.Append("s");
                    }
                }
                ts -= TimeSpan.FromHours(ts.Hours);
                if (ts.Minutes > 0)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ts.Minutes.ToString());
                    sb.Append(" minute");
                    if (ts.Minutes > 1)
                    {
                        sb.Append("s");
                    }
                }
                ts -= TimeSpan.FromMinutes(ts.Minutes);
                if (ts.Seconds > 0)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ts.Seconds.ToString());
                    sb.Append(" second");
                    if (ts.Seconds > 1)
                    {
                        sb.Append("s");
                    }
                }
                return sb.ToString();
            }
            else
            {
                return "Completed.";
            }
        }

        /// <summary>
        /// Updates label above the throbber for the next update
        /// </summary>
        private void UpdateThrobberLabel()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateThrobberLabel));
                return;
            }

            if (throbber.State == Throbber.ThrobberState.Rotating)
            {
                lblUpdateTimer.Visible = false;
                return;
            }

            TimeSpan ts = m_nextScheduledUpdateAt - DateTime.Now;
            if (ts < TimeSpan.Zero || ts > TimeSpan.FromHours(12))
            {
                lblUpdateTimer.Visible = false;
            }
            else
            {
                lblUpdateTimer.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", ts.Hours, ts.Minutes, ts.Seconds);
                lblUpdateTimer.Visible = true;
            }
        }

        /// <summary>
        /// Gets the character image.  First attempt to get it from cache, then attempt to get it from eve-o.  If you can't get it either place, fail quietly.
        /// </summary>
        private void GetCharacterImage()
        {
            if (pbCharImage.Image == null)
            {
                string cacheFileName = String.Format(
                    "{1}{0}cache{0}{2}.png", Path.DirectorySeparatorChar,
                    Settings.EveMonDataDir, this.GrandCharacterInfo.CharacterId.ToString());
                if (File.Exists(cacheFileName))
                {
                    pbCharImage.Image = PortraitFromCache(cacheFileName);
                    m_updatingPortrait = false;
                }
                if (pbCharImage.Image == null)
                {
                    UpdateCharacterImageRemote();
                }
            }
        }

        private void UpdateCharacterImageRemote()
        {
            pbCharImage.Image = null;
            ImageService.GetCharacterImageAsync(this.GrandCharacterInfo.CharacterId, new GetImageCallback(GotCharacterImage));
        }

        /// <summary>
        /// Scan the specified EVE Folder for character portraits and, if possible, copy one of them to EVEMon's cache for use, then update the portrait display.
        /// </summary>
        private void UpdateCharacterImageLocal()
        {
            pbCharImage.Image = null;
            Image i = null;

            // test to see if an EVE Folder has been selected
            if (this.GrandCharacterInfo.PortraitFolder == "")
            {
                if (LocalFileSystem.PortraitCacheFolder == null)
                {
                    // if the folder has not been selected and can not
                    // be detected, ask the user to select it
                    RequestEVEFolder();
                }
                else
                {
                    this.GrandCharacterInfo.PortraitFolder = LocalFileSystem.PortraitCacheFolder;
                }
            }

            try
            {
                // retreve path of portrait folder
                string eveCacheFolder = this.GrandCharacterInfo.PortraitFolder;

                // generate path of cache
                string cacheFileName = String.Format(
                    "{1}{0}cache{0}{2}.png", Path.DirectorySeparatorChar,
                    Settings.EveMonDataDir, this.GrandCharacterInfo.CharacterId.ToString());
                int charIDLength = this.GrandCharacterInfo.CharacterId.ToString().Length;

                // create a pattern that matches anything "<characterId>*.png"
                string filePattern = this.GrandCharacterInfo.CharacterId.ToString() + "*.png";

                // enumerate files in the EVE cache directory
                DirectoryInfo di = new DirectoryInfo(eveCacheFolder);
                FileInfo[] filesInEveCache = di.GetFiles(filePattern);

                if (filesInEveCache.Length > 0)
                {
                    // add each file to a sorted list
                    SortedList<int, string> sl = new SortedList<int, string>();

                    foreach (FileInfo file in filesInEveCache)
                    {
                        int sizeLength = (file.Name.Length - (file.Extension.Length + 1)) - charIDLength;
                        int imageSize = int.Parse(file.Name.Substring(charIDLength + 1, sizeLength));
                        sl.Add(imageSize, file.FullName);
                    }

                    // pick the largest image
                    string eveCacheFileName = sl.Values[sl.Count - 1];

                    FileStream fs1 = new FileStream(eveCacheFileName, FileMode.Open);
                    FileStream fs2 = new FileStream(cacheFileName, FileMode.Create);
                    i = Image.FromStream(fs1, true);
                    i.Save(fs2, ImageFormat.Png);
                    fs1.Close();
                    fs1.Dispose();
                    fs2.Close();
                    fs2.Dispose();
                }
                else
                {
                    String message;

                    message = "No portraits for your character were found in the folder you selected." + Environment.NewLine + Environment.NewLine;
                    message += "Ensure that you have checked the following" + Environment.NewLine;
                    message += " - You have selected a valid EVE Folder (i.e. C:\\Program Files\\CCP\\EVE\\)" + Environment.NewLine;
                    message += " - You have logged into EVE using this folder";

                    MessageBox.Show(message, "Portrait Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }

            pbCharImage.Image = i;
            m_updatingPortrait = false;
        }

        /// <summary>
        /// Pops up a window for the user to select their EVE folder.
        /// </summary>
        private void RequestEVEFolder()
        {
            using (EVEFolderWindow f = new EVEFolderWindow())
            {
                f.EVEFolder = m_grandCharacterInfo.PortraitFolder;
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    m_grandCharacterInfo.PortraitFolder = f.EVEFolder;
                    UpdateCachedCopy();
                }
            }
        }

        /// <summary>
        /// Toggles the expansion or collapsing of a single group
        /// </summary>
        /// <param name="gsg">The group to expand or collapse.</param>
        private void ToggleGroupExpandCollapse(SkillGroup gsg)
        {
            bool toCollapse;
            if (gsg.isCollapsed)
            {
                toCollapse = false;
            }
            else
            {
                toCollapse = true;
            }
            m_groupCollapsed[gsg] = toCollapse;
            gsg.isCollapsed = m_groupCollapsed[gsg];
            if (toCollapse)
            {
                // Remove the skills in the group from the list
                foreach (Skill gs in gsg)
                {
                    // Because they may have toggled the ShowAll settings during this session, we have to 
                    // cater for any invalid indexes when removing the skills.
                    try
                    {
                        lbSkills.Items.RemoveAt(lbSkills.Items.IndexOf(gs));
                    }
                    catch
                    { }
                }

                Pair<string, string> grp = new Pair<string, string>(m_grandCharacterInfo.Name, gsg.Name);
                m_settings.CollapsedGroups.Add(grp);
            }
            else
            {
                List<Skill> skillList = new List<Skill>();
                foreach (Skill gs in gsg)
                {
                    if (m_settings.ShowAllPublicSkills)
                    {
                        if (!gs.Public)
                            if (!m_settings.ShowNonPublicSkills)
                                continue;
                    }
                    skillList.Add(gs);
                }
                SkillChangedEventArgs args = new SkillChangedEventArgs(skillList.ToArray());
                CharacterSkillChangedCallback(this, args);
                foreach (Pair<string, string> grp in m_settings.CollapsedGroups)
                {
                    if ((grp.A == m_grandCharacterInfo.Name) && (grp.B == gsg.Name))
                    {
                        m_settings.CollapsedGroups.Remove(grp);
                        break;
                    }
                }
                //void m_grandCharacterInfo_SkillChanged(object sender, SkillChangedEventArgs e)
            }
            lbSkills.Invalidate(lbSkills.GetItemRectangle(lbSkills.Items.IndexOf(gsg)));
        }

        /// <summary>
        /// Handles the LinkClicked event of the llToggleAll control.  Toggles all the skill groups to collapse or open.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void llToggleAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            List<SkillGroup> toggles = new List<SkillGroup>();
            bool? setCollapsed = null;
            foreach (object o in lbSkills.Items)
            {
                if (o is SkillGroup)
                {
                    SkillGroup gsg = (SkillGroup)o;
                    bool isCollapsed = (m_groupCollapsed.ContainsKey(gsg) && m_groupCollapsed[gsg]);
                    if (setCollapsed == null)
                    {
                        setCollapsed = !isCollapsed;
                    }
                    if (isCollapsed != setCollapsed)
                    {
                        toggles.Add(gsg);
                    }
                }
            }
            lbSkills.BeginUpdate();
            try
            {
                foreach (SkillGroup toggroup in toggles)
                {
                    ToggleGroupExpandCollapse(toggroup);
                }
            }
            finally
            {
                lbSkills.EndUpdate();
            }
        }

        /// <summary>
        /// Forces the LCD data to be recalculated.
        /// </summary>
        public void ForceLcdDataUpdate()
        {
            CalculateLcdData();
        }

        #region External Callbacks

        /// <summary>
        /// Updates the LCD display.  Used to allow external agents (ie the settings object) to force this monitor to update its LCD display data in the event that
        /// something that would influence LCD data changes.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpdateLcdDisplayCallback(object sender, EventArgs e)
        {
            UpdateLcdisplay();
        }

        /// <summary>
        /// Called when the character file has been updated on the disc.  Used only by xml-backed characters
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        private void CharacterFileUpdatedCallback(object sender, FileSystemEventArgs e)
        {
            ReloadFromFile();
        }

        /// <summary>
        ///Changes worksafe mode - all the lifting is done right in this method.  Allows the settings object to notify monitors when they need to hide from the man.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void WorksafeChangedCallback(object sender, EventArgs e)
        {
            pbCharImage.Visible = !m_settings.WorksafeMode;
            if (m_settings.WorksafeMode)
            {
                tlpInfo.SetColumnSpan(lblSkillHeader, 1);
                tlpInfo.SetColumn(lblSkillHeader, 1);
            }
            else
            {
                tlpInfo.SetColumn(lblSkillHeader, 0);
                tlpInfo.SetColumnSpan(lblSkillHeader, 2);
            }
        }

        /// <summary>
        /// The currently training skill has changed.  Update ineve.
        /// This could probably take over some of the work currently done in CharacterSkillChangedCallback.
        /// </summary>
        /// <param name="sender">The sender - ignored.</param>
        /// <param name="?">The <see cref="System.EventArgs"/> instance containing the event data - ignored.</param>
        private void TrainingSkillChangedCallback(object sender, EventArgs args)
        {
            //will need this if we ever do UI things here, which we probably will.
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      TrainingSkillChangedCallback(sender, args);
                                                  }));
                return;
            }

            if (m_grandCharacterInfo.SerialSIT != null && m_grandCharacterInfo.SerialSIT.isSkillInTraining)
            {
                SerializableSkillTrainingInfo SIT = m_grandCharacterInfo.SerialSIT;
                Skill SSIT = m_grandCharacterInfo.AllSkillsByTypeID[SIT.TrainingSkillWithTypeID];
                m_skillTrainingName = SSIT.Name + " " + Skill.GetRomanForInt(SIT.TrainingSkillToLevel);
                lblTrainingSkill.Text = m_skillTrainingName;
                double spPerHour = Math.Round(SIT.SpPerMinute * 60);
                lblSPPerHour.Text = Convert.ToInt32(Math.Round(spPerHour)).ToString() + " SP/Hour";
                //   m_estimatedCompletion = ((DateTime)SIT.getTrainingEndTime.Subtract(TimeSpan.FromMilliseconds(SIT.TQOffset))).ToLocalTime();
                m_estimatedCompletion = SIT.getTrainingEndTime.ToLocalTime();

                m_settings_ScheduleEntriesChanged(null, null);

                CalculateLcdData();

                pnlTraining.Visible = true;
            }
            else if (m_grandCharacterInfo.SerialSIT == null || (m_grandCharacterInfo.SerialSIT != null && !m_grandCharacterInfo.SerialSIT.isSkillInTraining))
            {
                m_skillTrainingName = String.Empty;
                m_estimatedCompletion = DateTime.MaxValue;
                pnlTraining.Visible = false;
            }

            if (m_session != null && m_settings.GetCharacterSettings(m_charName).IneveSync)
                InEveNetUpdater.UpdateIneveAsync(m_grandCharacterInfo.Name);

        }

        /*void m_settings_NotificationOffsetChanged(object sender, EventArgs e)
        {
            if (m_grandCharacterInfo.SerialSIT != null && m_grandCharacterInfo.SerialSIT.isSkillInTraining)
                m_estimatedCompletion = m_grandCharacterInfo.SerialSIT.getTrainingEndTime.AddSeconds(-m_settings.NotificationOffset);
        }*/

        /// <summary>
        /// Handles everything when the character's training skill has changed.  Called by GrandCharacterInfo.
        /// </summary>
        /// <remarks>Another high-complexity method for us to look at.</remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EVEMon.Common.SkillChangedEventArgs"/> instance containing the event data.</param>
        private void CharacterSkillChangedCallback(object sender, SkillChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      CharacterSkillChangedCallback(sender, e);
                                                  }));
                return;
            }

            lbSkills.BeginUpdate();
            try
            {
                {
                    // Build the list builder into a loop that runs through all skills to ensure that
                    // even skill groups that don't have any learned skills in them will be added to
                    // the list.
                    if (!m_settings.ShowAllPublicSkills)
                    {
                        foreach (Skill gs in e.SkillList)
                        {
                            SkillGroup gsg = gs.SkillGroup;

                            if (gs.Known)
                            {
                                // Find the existing listbox item... if the group isn't collapsed
                                if (!m_groupCollapsed.ContainsKey(gsg) || m_groupCollapsed[gsg] == false)
                                {
                                    int lbIndex = -1;
                                    int shouldInsertAt = -1;
                                    bool shouldInsertSkillGroup = true;
                                    bool inMySkillGroup = false;
                                    bool found = false;
                                    for (int i = 0; i < lbSkills.Items.Count; i++)
                                    {
                                        object o = lbSkills.Items[i];
                                        if (o == gs)
                                        {
                                            shouldInsertSkillGroup = false;
                                            lbIndex = i;
                                            found = true;
                                            break;
                                        }
                                        else if (o == gsg)
                                        {
                                            inMySkillGroup = true;
                                            shouldInsertSkillGroup = false;
                                        }
                                        else if (o is SkillGroup && ((SkillGroup)o).Name.CompareTo(gsg.Name) > 0)
                                        {
                                            shouldInsertAt = i;
                                            shouldInsertSkillGroup = (!inMySkillGroup);
                                            break;
                                        }
                                        else if (inMySkillGroup && o is Skill &&
                                                 ((Skill)o).Name.CompareTo(gs.Name) > 0)
                                        {
                                            shouldInsertAt = i;
                                            shouldInsertSkillGroup = false;
                                            break;
                                        }
                                    }

                                    if (shouldInsertSkillGroup)
                                    {
                                        if (shouldInsertAt >= 0)
                                        {
                                            lbSkills.Items.Insert(shouldInsertAt, gsg);
                                            shouldInsertAt++;
                                        }
                                        else
                                        {
                                            lbSkills.Items.Add(gsg);
                                            shouldInsertAt = -1;
                                        }
                                    }
                                    if (!found)
                                    {
                                        if (shouldInsertAt >= 0)
                                        {
                                            lbSkills.Items.Insert(shouldInsertAt, gs);
                                            lbIndex = shouldInsertAt;
                                        }
                                        else
                                        {
                                            lbSkills.Items.Add(gs);
                                            lbIndex = lbSkills.Items.Count - 1;
                                        }
                                    }
                                    lbSkills.Invalidate(lbSkills.GetItemRectangle(lbIndex));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (SkillGroup skillGroup in m_grandCharacterInfo.SkillGroups.Values)
                        {
                            bool skillFound = false;
                            foreach (Skill skill in skillGroup)
                            {
                                foreach (Skill gs in e.SkillList)
                                {
                                    if (gs.Name == skill.Name)
                                        skillFound = true;
                                    SkillGroup gsg = gs.SkillGroup;

                                    // Find the existing listbox item... if the group isn't collapsed
                                    if (!m_groupCollapsed.ContainsKey(gsg) || m_groupCollapsed[gsg] == false)
                                    {
                                        int lbIndex = -1;
                                        int shouldInsertAt = -1;
                                        bool shouldInsertSkillGroup = true;
                                        bool inMySkillGroup = false;
                                        bool found = false;
                                        for (int i = 0; i < lbSkills.Items.Count; i++)
                                        {
                                            object o = lbSkills.Items[i];
                                            if (o == gs)
                                            {
                                                shouldInsertSkillGroup = false;
                                                lbIndex = i;
                                                found = true;
                                                break;
                                            }
                                            else if (o == gsg)
                                            {
                                                inMySkillGroup = true;
                                                shouldInsertSkillGroup = false;
                                            }
                                            else if (o is SkillGroup && ((SkillGroup)o).Name.CompareTo(gsg.Name) > 0)
                                            {
                                                shouldInsertAt = i;
                                                shouldInsertSkillGroup = (!inMySkillGroup);
                                                break;
                                            }
                                            else if (inMySkillGroup && o is Skill &&
                                                     ((Skill)o).Name.CompareTo(gs.Name) > 0)
                                            {
                                                shouldInsertAt = i;
                                                shouldInsertSkillGroup = false;
                                                break;
                                            }
                                        }

                                        if (shouldInsertSkillGroup)
                                        {
                                            if (shouldInsertAt >= 0)
                                            {
                                                lbSkills.Items.Insert(shouldInsertAt, gsg);
                                                shouldInsertAt++;
                                            }
                                            else
                                            {
                                                lbSkills.Items.Add(gsg);
                                                shouldInsertAt = -1;
                                            }
                                        }
                                        if (!found)
                                        {
                                            if (shouldInsertAt >= 0)
                                            {
                                                lbSkills.Items.Insert(shouldInsertAt, gs);
                                                lbIndex = shouldInsertAt;
                                            }
                                            else
                                            {
                                                lbSkills.Items.Add(gs);
                                                lbIndex = lbSkills.Items.Count - 1;
                                            }
                                        }
                                        lbSkills.Invalidate(lbSkills.GetItemRectangle(lbIndex));
                                    }
                                }
                                if (!skillFound)
                                {
                                    if (!skill.Public)
                                    {
                                        if (!m_settings.ShowNonPublicSkills)
                                            continue;
                                    }
                                    // Find the existing listbox item... if the group isn't collapsed
                                    if (!m_groupCollapsed.ContainsKey(skillGroup) || m_groupCollapsed[skillGroup] == false)
                                    {
                                        int lbIndex = -1;
                                        int shouldInsertAt = -1;
                                        bool shouldInsertSkillGroup = true;
                                        bool inMySkillGroup = false;
                                        bool found = false;
                                        for (int i = 0; i < lbSkills.Items.Count; i++)
                                        {
                                            object o = lbSkills.Items[i];
                                            if (o == skill)
                                            {
                                                shouldInsertSkillGroup = false;
                                                lbIndex = i;
                                                found = true;
                                                break;
                                            }
                                            else if (o == skillGroup)
                                            {
                                                inMySkillGroup = true;
                                                shouldInsertSkillGroup = false;
                                            }
                                            else if (o is SkillGroup && ((SkillGroup)o).Name.CompareTo(skillGroup.Name) > 0)
                                            {
                                                shouldInsertAt = i;
                                                shouldInsertSkillGroup = (!inMySkillGroup);
                                                break;
                                            }
                                            else if (inMySkillGroup && o is Skill &&
                                                     ((Skill)o).Name.CompareTo(skill.Name) > 0)
                                            {
                                                shouldInsertAt = i;
                                                shouldInsertSkillGroup = false;
                                                break;
                                            }
                                        }

                                        if (shouldInsertSkillGroup)
                                        {
                                            if (shouldInsertAt >= 0)
                                            {
                                                lbSkills.Items.Insert(shouldInsertAt, skillGroup);
                                                shouldInsertAt++;
                                            }
                                            else
                                            {
                                                lbSkills.Items.Add(skillGroup);
                                                shouldInsertAt = -1;
                                            }
                                        }
                                        if (!found)
                                        {
                                            if (shouldInsertAt >= 0)
                                            {
                                                lbSkills.Items.Insert(shouldInsertAt, skill);
                                                lbIndex = shouldInsertAt;
                                            }
                                            else
                                            {
                                                lbSkills.Items.Add(skill);
                                                lbIndex = lbSkills.Items.Count - 1;
                                            }
                                        }
                                        lbSkills.Invalidate(lbSkills.GetItemRectangle(lbIndex));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                lbSkills.EndUpdate();
            }

            UpdateSkillHeaderStats();
            UpdateCachedCopy();
        }

        /// <summary>
        /// Handle the event when a character's attributes change.  Called by GrandCharacterInfo.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttributeChangedCallback(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      AttributeChangedCallback(sender, e);
                                                  }));
                return;
            }

            SetAttributeLabel(lblIntelligence, EveAttribute.Intelligence);
            SetAttributeLabel(lblCharisma, EveAttribute.Charisma);
            SetAttributeLabel(lblPerception, EveAttribute.Perception);
            SetAttributeLabel(lblMemory, EveAttribute.Memory);
            SetAttributeLabel(lblWillpower, EveAttribute.Willpower);
            UpdateCachedCopy();
        }

        /// <summary>
        /// Handles the character's wallet changing, hopefully for the better.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BalanceChangedCallback(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      BalanceChangedCallback(sender, e);
                                                  }));
                return;
            }

            lblBalance.Text = "Balance: " + m_grandCharacterInfo.Balance.ToString("#,##0.00") + " ISK";

            UpdateCachedCopy();
        }

        /// <summary>
        /// Invoked when the character's bio info changes.  Used by GrandCharacterInfo
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BioInfoChangedCallback(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      BioInfoChangedCallback(sender, e);
                                                  }));
                return;
            }

            if (m_grandCharacterInfo.IsCached)
            {
                lblCharacterName.Text = m_grandCharacterInfo.Name + " (cached)";
            }
            else
            {
                lblCharacterName.Text = m_grandCharacterInfo.Name;
            }

            lblBioInfo.Text = m_grandCharacterInfo.Gender + " " +
                              m_grandCharacterInfo.Race + " " +
                              m_grandCharacterInfo.Bloodline;
            lblCorpInfo.Text = "Corporation: " + m_grandCharacterInfo.CorporationName;

            UpdateCachedCopy();
        }

        /// <summary>
        /// Update the character image in the event that the remote character image was (probably) retrieved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="i">The retrieved image.</param>
        private void GotCharacterImage(EveSession sender, Image newImage)
        {
            string cacheFileName = String.Format(
                    "{1}{0}cache{0}{2}.png",
                    Path.DirectorySeparatorChar,
                    Settings.EveMonDataDir, this.GrandCharacterInfo.CharacterId.ToString());

            //the image was not retrieved - go to plan B
            if (newImage == null)
            {
                //Can we get it from the cache?
                if (File.Exists(cacheFileName))
                {
                    newImage = PortraitFromCache(cacheFileName);
                }
                //the image is not in the cache
                if (newImage == null)
                {
                    newImage = pbCharImage.InitialImage;
                    SavePortraitToCache(newImage, cacheFileName);
                }
            }
            else
            {
                SavePortraitToCache(newImage, cacheFileName);
            }
            //whatever happened, show a portrait
            pbCharImage.Image = newImage;
            m_updatingPortrait = false;
        }

        /// <summary>
        /// Open the character portrait from the EVEMon cache
        /// </summary>
        /// <returns>The character portrait as an Image object</returns>
        private static Image PortraitFromCache(string cacheFile)
        {
            Image newImage;
            try
            {
                FileStream fs = new FileStream(cacheFile, FileMode.Open);
                newImage = Image.FromStream(fs, true);
                fs.Close();
                fs.Dispose();
                return newImage;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
                return null;
            }
        }

        /// <summary>
        /// Save the specified image to the EVEMon cache as this character's portrait
        /// </summary>
        /// <param name="newImage">The new portrait image.</param>
        private static void SavePortraitToCache(Image newImage, string cacheFile)
        {
            try
            {
                //make their character a "!" in the cache
                FileStream fs = new FileStream(cacheFile, FileMode.Create);
                newImage.Save(fs, ImageFormat.Png);
                fs.Close();
                fs.Dispose();
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }
        }

        /// <summary>
        /// Callback for the event where a character update failed.  Should be used via Invoke.
        /// </summary>
        private void CharacterDownloadFailedCallback()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(CharacterDownloadFailedCallback));
                return;
            }
            StringBuilder throbberTip = new StringBuilder();
            // Stop an exception on first run using new api interface
            try
            {
                if (m_session.ApiErrorMessage != null)
                {
                    throbberTip.Append(String.Format("Error {0}\n", m_session.ApiErrorMessage));
                }
            }
            catch (Exception) { }

            throbberTip.Append("Could not get character data!\nClick to try again.");

            ttToolTip.SetToolTip(throbber, throbberTip.ToString());
            ttToolTip.IsBalloon = true;
            if (m_settings.DisableXMLAutoUpdate == false)
            {
                tmrUpdateCharacter.Interval = 1000 * 60 * 30;
                tmrUpdateCharacter.Enabled = true;
            }
            SetErrorThrobber();
            this.m_grandCharacterInfo.checkOldSkill();
        }
        #endregion

        #region Control/Component Event Handlers

        /// <summary>
        /// Handles the Click event of the pbThrobber control.  If we're in an OK state, hit eve-o, otherwise, show the throbber menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void throbber_Click(object sender, EventArgs e)
        {
            if (throbber.State != Throbber.ThrobberState.Strobing)
            {
                tmrUpdate_Tick(null, null);
                UpdateThrobberLabel();
            }
            else
            {
                cmsThrobberMenu.Show(MousePosition);
            }
        }

        private void lbSkills_MouseEnter(object sender, EventArgs e)
        {
            ttToolTip.Active = false;
        }

        private void lbSkills_MouseLeave(object sender, EventArgs e)
        {
            ttToolTip.Active = false;
        }

        private void pbThrobber_MouseEnter(object sender, EventArgs e)
        {
            ttToolTip.IsBalloon = true;
            ttToolTip.Active = true;
        }

        private void miHitEveO_Click(object sender, EventArgs e)
        {
            tmrUpdate_Tick(this, new EventArgs());
        }

        private void tsbShowBooks_Click(object sender, EventArgs e)
        {
            ShowOwnedSkillbooks();
        }

        public void ShowOwnedSkillbooks()
        {
            // A quick MessageBox to show the skillbooks we own
            StringBuilder sb = new StringBuilder();
            SortedList<string, bool> sortedSkills = new SortedList<string, bool>();
            foreach (string skillName in m_settings.GetOwnedBooksForCharacter(GrandCharacterInfo.Name))
            {
                Skill skill = GrandCharacterInfo.GetSkill(skillName);
                if (skill.Known) continue;
                sortedSkills.Add(skillName, skill.PrerequisitesMet);
            }
            bool firstSkill = true;
            foreach (string skillName in sortedSkills.Keys)
            {
                if (!firstSkill) sb.Append("\n");
                firstSkill = false;
                sb.Append(skillName);
                if (sortedSkills[skillName])
                {
                    sb.Append(" (prereqs met)");
                }
                else
                {
                    sb.Append(" (prereqs not met)");
                }
            }
            if (firstSkill) sb.Append("You don't have any skills marked as Owned");
            MessageBox.Show(sb.ToString(), String.Format("Skills owned by {0}", GrandCharacterInfo.Name), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles the Click event of the miChangeInfo control.  Displays a new login window and gets API Credentials from the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void miChangeInfo_Click(object sender, EventArgs e)
        {
            using (ChangeLoginWindow f = new ChangeLoginWindow())
            {
                if (m_cli.Account != null)
                {
                    f.UserId = m_cli.Account.UserId;
                }
                f.ShowInvalidKey = false;
                f.CharacterName = m_grandCharacterInfo.Name;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    if (m_cli.Account == null)
                    {
                        // find teh account from the accounts list, we didn't have it at start up but now we do
                        m_cli.Account = m_settings.FindAccount(m_cli.UserId);
                    }
                    m_session = EveSession.GetSession(f.UserId, f.ApiKey);
                    m_cli.Account.UserId = f.UserId;
                    m_cli.Account.ApiKey = f.ApiKey;

                    m_charId = -1;
                    tmrUpdate_Tick(this, new EventArgs());
                    m_settings.Save();
                }
            }
        }

        /// <summary>
        /// Handles the DrawItem event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void lbSkills_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            object item = lbSkills.Items[e.Index];

            if (item is SkillGroup)
            {
                ((SkillGroup)item).Draw(e);
            }
            else if (item is Skill)
            {
                ((Skill)item).Draw(e);
            }
        }

        /// <summary>
        /// Handles the MeasureItem event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MeasureItemEventArgs"/> instance containing the event data.</param>
        private void lbSkills_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            object item = lbSkills.Items[e.Index];
            if (item is SkillGroup)
            {
                e.ItemHeight = SkillGroup.Height;
            }
            else if (item is Skill)
            {
                e.ItemHeight = Skill.Height;
            }
        }

        /// <summary>
        /// Handles the MouseWheel event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbSkills_MouseWheel(object sender, MouseEventArgs e)
        {
            // Update the drawing based upon the mouse wheel scrolling.
            int numberOfItemLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            int Lines = numberOfItemLinesToMove;

            // Prevent a divide by zero exception here, we've had one report of this but can't figure out why.
            if (Lines == 0) return;

            int direction = Lines / Math.Abs(Lines);
            int[] numberOfPixelsToMove = new int[Lines * direction];
            for (int i = 1; i <= Math.Abs(Lines); i++)
            {
                object item = null;
                if (direction == Math.Abs(direction))
                {
                    // Going up
                    if (lbSkills.TopIndex - i >= 0)
                    {
                        item = lbSkills.Items[lbSkills.TopIndex - i];
                    }
                }
                else
                {
                    // Going down
                    int h = 0; // height of items from current topindex inclusive
                    for (int j = lbSkills.TopIndex + i - 1; j < lbSkills.Items.Count; j++)
                    {
                        if (lbSkills.Items[j] is SkillGroup)
                        {
                            h += SkillGroup.Height;
                        }
                        else if (lbSkills.Items[j] is Skill)
                        {
                            h += Skill.Height;
                        }
                    }
                    if (h > lbSkills.ClientSize.Height)
                    {
                        item = lbSkills.Items[lbSkills.TopIndex + i - 1];
                    }
                }
                if (item != null)
                {
                    if (item is SkillGroup)
                    {
                        numberOfPixelsToMove[i - 1] = SkillGroup.Height * direction;
                    }
                    else if (item is Skill)
                    {
                        numberOfPixelsToMove[i - 1] = Skill.Height * direction;
                    }
                }
                else
                {
                    Lines -= direction;
                }
            }
            if (Lines != 0)
            {
                // The Array 'numberOfPixelsToMove' contains the number of pixels the
                // list box 'lbSkills' needs to scroll for each line... the question
                // is, how to tell it that and get it to do so smoothly.
                /*
                // This doesn't work...
                for (int i = 0; i < Math.Abs(Lines); i++)
                {
                    System.Drawing.Drawing2D.Matrix translateMatrix = new System.Drawing.Drawing2D.Matrix();
                    translateMatrix.Translate(0, numberOfPixelsToMove[i]);
                    mousePath.Transform(translateMatrix);
                }
                 */
                lbSkills.Invalidate();
                // invalidate is a temporary fix that does give limited functionality for purpose.
            }
        }

        public void ShowPlanSelectWindow()
        {
            Plan p = null;
            using (PlanSelectWindow psw = new PlanSelectWindow(m_settings, m_grandCharacterInfo))
            {
                if (m_cfi != null)
                {
                    psw.CharKey = m_cfi.Filename;
                }
                DialogResult dr = psw.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                p = psw.ResultPlan;
            }

            // Check for a merged plan
            if (p == null || p.Name == null)
            {
                // OK, User has requested to merge plans so ask for a name
                bool doAgain = true;
                // keep going till we get a valid name or they cancel..
                while (doAgain)
                {
                    // Ask for a plan name
                    using (NewPlanWindow npw = new NewPlanWindow())
                    {
                        DialogResult dr = npw.ShowDialog();
                        if (dr == DialogResult.Cancel)
                        {
                            // They changed their mind
                            return;
                        }
                        string planName = npw.Result;

                        if (p == null)
                        {
                            p = new Plan();
                        }
                        try
                        {
                            // we hae a name, check it's not already defined
                            if (m_cfi == null)
                            {
                                m_settings.AddPlanFor(m_grandCharacterInfo.Name, p, planName);
                            }
                            else
                            {
                                m_settings.AddPlanFor(m_cfi.Filename, p, planName);
                            }
                            // No exception, we're good to go.
                            doAgain = false;
                        }
                        catch (ApplicationException err)
                        {
                            // Plan name already exists. Go round again...
                            ExceptionHandler.LogException(err, true);
                            DialogResult xdr = MessageBox.Show(err.Message, "Failed to Add Plan", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                            if (xdr == DialogResult.Cancel)
                            {
                                // unless they changed their mind...
                                return;
                            }
                        }
                    }
                }

            } // end of merge plans processing.

            p.ShowEditor(m_settings, m_grandCharacterInfo);
        }

        public void ShowNewPlanWindow()
        {
            Plan p = null;
            bool doAgain = true;
            while (doAgain)
            {
                using (NewPlanWindow npw = new NewPlanWindow())
                {
                    DialogResult dr = npw.ShowDialog();
                    if (dr == DialogResult.Cancel)
                    {
                        return;
                    }
                    string planName = npw.Result;

                    if (p == null)
                    {
                        p = new Plan();
                    }
                    try
                    {
                        if (m_cfi == null)
                        {
                            m_settings.AddPlanFor(m_grandCharacterInfo.Name, p, planName);
                        }
                        else
                        {
                            m_settings.AddPlanFor(m_cfi.Filename, p, planName);
                        }
                        doAgain = false;
                    }
                    catch (ApplicationException err)
                    {
                        ExceptionHandler.LogException(err, true);
                        DialogResult xdr = MessageBox.Show(err.Message, "Failed to Add Plan", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        if (xdr == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
            }

            p.ShowEditor(m_settings, m_grandCharacterInfo);
        }

        public void ShowPlanEditor(string planName)
        {
            String planKey = (m_cfi == null) ? m_grandCharacterInfo.Name : m_cfi.Filename;
            m_settings.GetPlanByName(planKey, m_grandCharacterInfo, planName).ShowEditor(m_settings, m_grandCharacterInfo);
        }

        private void lbSkills_MouseDown(object sender, MouseEventArgs e)
        {
            lbSkills_MouseClick(sender, e);
        }

        /// <summary>
        /// Handles the MouseClick event of the lbSkills control.
        /// </summary>
        /// <remarks>TODO:  There are opportunities for improvement here.</remarks>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbSkills_MouseClick(object sender, MouseEventArgs e)
        {
            int index = lbSkills.IndexFromPoint(e.X, e.Y);
            object item;
            if (index < 0 || index >= lbSkills.Items.Count)
            {
                item = null;
            }
            else
            {
                item = lbSkills.Items[index];
                if (index == lbSkills.Items.Count - 1)
                {
                    // handle a click in the whitespace below the last item.
                    // IndexFromPoint() returns the last item if you click in white space below it.
                    // I consider this to be a bug. Here's a workaround...
                    Rectangle itemRect = lbSkills.GetItemRectangle(lbSkills.Items.IndexOf(item));
                    if (!itemRect.Contains(e.Location))
                    {
                        item = null;
                    }
                }
            }

            ttToolTip.IsBalloon = true;
            ttToolTip.UseAnimation = true;
            ttToolTip.UseFading = true;
            ttToolTip.AutoPopDelay = 10000;

            if (item is SkillGroup)
            {
                SkillGroup sg = (SkillGroup)item;
                if (e.Button == MouseButtons.Left)
                {
                    ToggleGroupExpandCollapse(sg);
                    return;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Rectangle itemRect = lbSkills.GetItemRectangle(lbSkills.Items.IndexOf(item));
                    Rectangle buttonRect = sg.GetButtonRectangle(itemRect);
                    if (buttonRect.Contains(e.Location))
                    {
                        ToggleGroupExpandCollapse(sg);
                        return;
                    }

                    int TotalPoints = 0;
                    double percentDonePoints = 0.0;
                    double percentDoneSkills = 0.0;

                    foreach (Skill s in sg)
                    {
                        TotalPoints += s.GetPointsRequiredForLevel(5);
                    }

                    if (sg.GetTotalPoints() < TotalPoints)
                    {
                        percentDonePoints = (Convert.ToDouble(sg.GetTotalPoints()) / Convert.ToDouble(TotalPoints) * 100);
                        percentDoneSkills = Convert.ToDouble(sg.KnownCount) / Convert.ToDouble(sg.Count);

                        string SkillGroupStats =
                            String.Format("Points Completed: {0}/{1} ({2}%)\nSkills Known: {3}/{4} ({5})",
                                          sg.GetTotalPoints().ToString("#,##0"), TotalPoints.ToString("#,##0"),
                                          percentDonePoints.ToString("N3"), sg.KnownCount.ToString("#"),
                                          sg.Count.ToString("#"), percentDoneSkills.ToString("P0"));

                        ttToolTip.SetToolTip(lbSkills, SkillGroupStats);
                        ttToolTip.Active = true;
                    }
                    else // we must have learned all the skills in this group to level 5
                    {
                        //I wish I could test this :)
                        // - save xml, edit to make all skills in a group l5, import?
                        string Done = String.Format("Skill Group completed: {0}/{1} (100%)\nSkills: {2}/{3} (100%)",
                                                    sg.GetTotalPoints().ToString("#,##0"), TotalPoints.ToString("#,##0"),
                                                    sg.KnownCount.ToString("#"), sg.Count.ToString("#"));
                        ttToolTip.Active = true;
                        ttToolTip.SetToolTip(lbSkills, Done);
                    }
                }
            }
            else if (item is Skill)
            {
                Skill s = (Skill)item;
                double percentDone = 0.0;
                int NextLevel = 0;
                int CurrentSP = s.CurrentSkillPoints;
                int reqToThisLevel = s.GetPointsRequiredForLevel(s.Level);
                int PointsRemain = 0;
                double spPerHour = 60 * (m_grandCharacterInfo.GetEffectiveAttribute(s.PrimaryAttribute) +
                                       (m_grandCharacterInfo.GetEffectiveAttribute(s.SecondaryAttribute) / 2));
                string SPPerHour = " (" + Convert.ToInt32(Math.Round(spPerHour)).ToString() + " SP/Hour)";

                if (e.Button == MouseButtons.Right)
                {
                    percentDone = s.GetPercentDone();
                    bool doPopup = false;
                    NextLevel = s.Level;
                    // They haven't completed the level...
                    if (percentDone < 100.0)
                    {
                        // They may have finished the current level, but not started the next level...
                        if (s.CurrentSkillPoints >= s.GetPointsRequiredForLevel(s.Level))
                        {
                            NextLevel += 1;
                            doPopup = true;
                        }
                    }
                    else
                    {
                        // They haven't completed Level V yet...
                        if (s.Level < 5)
                        {
                            NextLevel += 1;
                            doPopup = true;
                        }
                    }
                    // Check whether we do popup or not. If not drop through and treat the right-click the same as the left.
                    if (doPopup)
                    {
                        // Reset the menu.
                        contextMenuStripPlanPopup.Items.Clear();
                        ToolStripMenuItem tm = new ToolStripMenuItem(String.Format("Add {0}", s.Name));
                        contextMenuStripPlanPopup.Items.Add(tm);

                        String planKey = this.GetPlanKey();
                        // Build the level options.
                        for (int level = NextLevel; level < 6; level++)
                        {
                            ToolStripMenuItem menuLevel = new ToolStripMenuItem(string.Format("Level {0} to", Skill.GetRomanForInt(level)));
                            tm.DropDownItems.Add(menuLevel);
                            foreach (string plan in m_settings.GetPlansForCharacter(planKey))
                            {
                                ToolStripMenuItem menuPlanItem = new ToolStripMenuItem(plan);
                                Dictionary<Skill, int> skillAndLevel = new Dictionary<Skill, int>();
                                skillAndLevel.Add(s, level);
                                menuPlanItem.Tag = skillAndLevel;
                                menuPlanItem.Click += new EventHandler(menuPlanItem_Click);
                                menuLevel.DropDownItems.Add(menuPlanItem);
                            }
                        }
                        contextMenuStripPlanPopup.Show((Control)sender, new Point(e.X, e.Y));
                        return;
                    }
                }

                if (CurrentSP > s.GetPointsRequiredForLevel(s.Level))
                {
                    //We must have completed some, but not all, of level II, III or IV
                    NextLevel = s.Level + 1;

                    percentDone = s.GetPercentDone();
                    PointsRemain = s.GetPointsRequiredForLevel(NextLevel) - s.CurrentSkillPoints;
                    string CurrentlyDone =
                        String.Format("Partially Completed lvl {0}: {1}/{2} ({3})",
                                      Skill.GetRomanForInt(NextLevel),
                                      s.CurrentSkillPoints.ToString("#,##0"),
                                      s.GetPointsRequiredForLevel(NextLevel).ToString("#,##0"),
                                      percentDone.ToString("P0"));
                    string ToNextLevel =
                        String.Format("To Level {0}: {1} Skill Points remaining",
                                      Skill.GetRomanForInt(NextLevel), PointsRemain.ToString("#,##0"));
                    ttToolTip.Active = true;
                    ttToolTip.SetToolTip(lbSkills,
                                         CurrentlyDone + "\n" + ToNextLevel + "\nTraining Time remaining: " +
                                         Skill.TimeSpanToDescriptiveText(
                                             s.GetTrainingTimeToLevel(NextLevel),
                                             DescriptiveTextOptions.IncludeCommas |
                                             DescriptiveTextOptions.UppercaseText) + "\n" +
                                         s.DescriptionNl.ToString() + "\nPrimary: " +
                                         s.PrimaryAttribute.ToString() + ", Secondary: " +
                                         s.SecondaryAttribute.ToString() + SPPerHour);
                }
                else if (CurrentSP == s.GetPointsRequiredForLevel(s.Level))
                // We've completed all the skill points for the current level
                {
                    if (s.Level != 5)
                    {
                        NextLevel = s.Level + 1;
                        percentDone = s.GetPercentDone();
                        PointsRemain = s.GetPointsRequiredForLevel(NextLevel) - s.CurrentSkillPoints;
                        string CurrentlyDone =
                            String.Format("Completed lvl {0}: {1}/{2}",
                                          Skill.GetRomanForInt(s.Level),
                                          s.CurrentSkillPoints.ToString("#,##0"),
                                          s.GetPointsRequiredForLevel(s.Level).ToString("#,##0"));
                        string ToNextLevel =
                            String.Format("To Level {0}: {1} Skill Points required",
                                          Skill.GetRomanForInt(NextLevel),
                                          PointsRemain.ToString("#,##0"));
                        ttToolTip.Active = true;
                        ttToolTip.SetToolTip(lbSkills,
                                             CurrentlyDone + "\n" + ToNextLevel + "\nTraining Time: " +
                                             Skill.TimeSpanToDescriptiveText(
                                                 s.GetTrainingTimeToLevel(NextLevel),
                                                 DescriptiveTextOptions.IncludeCommas |
                                                 DescriptiveTextOptions.UppercaseText) + "\n" +
                                             s.DescriptionNl.ToString() + "\nPrimary: " +
                                             s.PrimaryAttribute.ToString() + ", Secondary: " +
                                             s.SecondaryAttribute.ToString() + SPPerHour);
                    }
                    else // training completed
                    {
                        ttToolTip.Active = true;
                        ttToolTip.SetToolTip(lbSkills,
                                             String.Format(
                                                 "Level V Complete: {0}/{1}\nNo further training required\n{2}\nPrimary: {3}, Secondary: {4} {5}",
                                                 s.CurrentSkillPoints.ToString("#,##0"),
                                                 s.GetPointsRequiredForLevel(5).ToString("#,##0"),
                                                 s.DescriptionNl.ToString(), s.PrimaryAttribute.ToString(),
                                                 s.SecondaryAttribute.ToString(), SPPerHour));
                    }
                }
                else // training hasn't got past level 1 yet
                {
                    NextLevel = s.Level + 1; //this should always be 1

                    percentDone = Convert.ToDouble(CurrentSP) /
                                  Convert.ToDouble(s.GetPointsRequiredForLevel(NextLevel));
                    PointsRemain = s.GetPointsRequiredForLevel(NextLevel) - s.CurrentSkillPoints;
                    string CurrentlyDone =
                        String.Format("Partially Completed lvl {0}: {1}/{2} ({3})",
                                      Skill.GetRomanForInt(NextLevel),
                                      s.CurrentSkillPoints.ToString("#,##0"),
                                      s.GetPointsRequiredForLevel(NextLevel).ToString("#,##0"),
                                      percentDone.ToString("P0"));
                    string ToNextLevel =
                        String.Format("To Level {0}: {1} Skill Points remaining",
                                      Skill.GetRomanForInt(NextLevel), PointsRemain.ToString("#,##0"));
                    ttToolTip.Active = true;
                    ttToolTip.SetToolTip(lbSkills,
                                         CurrentlyDone + "\n" + ToNextLevel + "\nTraining Time remaining: " +
                                         Skill.TimeSpanToDescriptiveText(
                                             s.GetTrainingTimeToLevel(NextLevel),
                                             DescriptiveTextOptions.IncludeCommas |
                                             DescriptiveTextOptions.UppercaseText) + "\n" +
                                         s.DescriptionNl.ToString() + "\nPrimary: " +
                                         s.PrimaryAttribute.ToString() + ", Secondary: " +
                                         s.SecondaryAttribute.ToString() + SPPerHour);
                }
            }
        }

        void menuPlanItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem planItem = (ToolStripMenuItem)sender;
            Plan plan = m_settings.GetPlanByName(m_charName, m_grandCharacterInfo, planItem.Text);
            Dictionary<Skill, int> skillAndLevel = planItem.Tag as Dictionary<Skill, int>;
            foreach (KeyValuePair<Skill, int> kvp in skillAndLevel)
            {
                plan.PlanTo(kvp.Key, kvp.Value);
            }
        }

        private void miUpdatePicture_Click(object sender, EventArgs e)
        {
            UpdateCharacterImageRemote();
        }

        private void miUpdatePictureFromEVECache_Click(object sender, EventArgs e)
        {
            UpdateCharacterImageLocal();
        }

        private void miSetEVEFolder_Click(object sender, EventArgs e)
        {
            RequestEVEFolder();
        }

        private void pbCharImage_Click(object sender, EventArgs e)
        {
            cmsPictureOptions.Show(MousePosition);
        }

        /// <summary>
        /// Handles the MouseHover event of the lblAttribute control.  Pops up a little tooltip containing everything the person could possibly want to know
        /// about their character's attributes.
        /// </summary>
        /// <remarks>Recalculates everything every time the label pops - so it's inefficient, but responsive enough for this purpose.</remarks>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void lblAttribute_MouseHover(object sender, EventArgs e)
        {
            Label lblAttrib = (Label)sender;
            EveAttribute eveAttribute = EveAttribute.None;
            switch (lblAttrib.Name)
            {
                case "lblIntelligence": eveAttribute = EveAttribute.Intelligence; break;
                case "lblCharisma": eveAttribute = EveAttribute.Charisma; break;
                case "lblMemory": eveAttribute = EveAttribute.Memory; break;
                case "lblWillpower": eveAttribute = EveAttribute.Willpower; break;
                case "lblPerception": eveAttribute = EveAttribute.Perception; break;
                default: break;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(eveAttribute.ToString());

            sb.Append(": ");
            sb.Append(m_grandCharacterInfo.GetEffectiveAttribute(eveAttribute).ToString("0.00"));

            int baseAtt = m_grandCharacterInfo.GetBaseAttribute(eveAttribute);
            double fromSkills = m_grandCharacterInfo.GetEffectiveAttribute(eveAttribute, null, false, false) - baseAtt;
            double learning = m_grandCharacterInfo.LearningBonus;
            double implant = m_grandCharacterInfo.getImplantValue(eveAttribute);

            sb.Append(" [");
            if (learning > 0.0) sb.Append("(");
            sb.Append(baseAtt.ToString("0"));
            sb.Append(" base + ");
            sb.Append(fromSkills.ToString("0"));
            sb.Append(" skills + ");
            sb.Append(implant.ToString("0"));
            if (learning > 0.0)
            {
                sb.Append(" implants) * ");
                sb.Append(learning.ToString("0.00"));
                sb.Append(" from learning bonus");
            }
            sb.Append("]");

            ttToolTip.SetToolTip(lblAttrib, sb.ToString());
            ttToolTip.IsBalloon = false;
            ttToolTip.Active = true;

        }

        /// <summary>
        /// Show a breakdown of the number of skills at each level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSkillHeader_MouseHover(object sender, EventArgs e)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < 6; i++)
            {
                if (i > 1)
                {
                    sb.Append("\n");
                }
                int count = m_grandCharacterInfo.SkillCountAtLevel(i);
                sb.Append(String.Format("{0} Skills at Level {1}", count, i));
            }

            ttToolTip.SetToolTip(sender as Label, sb.ToString());
            ttToolTip.IsBalloon = false;
            ttToolTip.Active = true;
        }

        public void SaveCharacterXML()
        {
            sfdSaveDialog.FileName = m_grandCharacterInfo.Name;
            sfdSaveDialog.FilterIndex = (int)SaveFormat.Xml;
            DialogResult dr = sfdSaveDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SaveFile((SaveFormat)sfdSaveDialog.FilterIndex, sfdSaveDialog.FileName);
            }
        }

        /// <summary>
        /// Handles the Tick event of the tmrUpdate control.  This is the one that updates the character itself.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    tmrUpdate_Tick(sender, e);
                }));
                return;
            }

            if (throbber.State == Throbber.ThrobberState.Rotating)
            {
                return;
            }

            tmrUpdateCharacter.Enabled = false;
            DateTime lastCacheTime = m_grandCharacterInfo.XMLExpires;


#if DEBUG_SINGLETHREAD
            // don't start the throbber in single thread debug mode
#else
            StartThrobber();
#endif

            // m_charId is only not -1 when it's a file based char... thus never going to use this function
            // the value we *should* be using is m_grandCharacterInfo.CharacterId BUT
            // that means we don't refresh the session with ccp first after the initial login.
            // What does this mean... it means we don't change this code until ccp get permanent logins working
            // Thus we ALWAYS call DownloadCharacter(), unless something miraculous has happend
            if (m_charId < 0)
            {
                DownloadCharacter();
            }
            else
            {
                UpdateGrandCharacterInfo();
            }
        }

        /// <summary>
        /// Things that happen every second:
        /// Update LCD Data, Update time remaining and throbber labels, check your skills, so on, so forth
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tmrTick_Tick(object sender, EventArgs e)
        {
            CalculateLcdData();

            if (m_currentlyVisible)
            {
                UpdateTimeRemainingLabels();
                UpdateThrobberLabel();

                Skill trainingSkill = m_grandCharacterInfo.CurrentlyTrainingSkill;
                if (trainingSkill != null)
                {
                    if (trainingSkill.CurrentSkillPoints != m_lastTickSPPaint)
                    {
                        m_lastTickSPPaint = trainingSkill.CurrentSkillPoints;
                        int idx = lbSkills.Items.IndexOf(trainingSkill);
                        if (idx >= 0)
                        {
                            lbSkills.Invalidate(lbSkills.GetItemRectangle(idx));
                        }
                        int sgidx = lbSkills.Items.IndexOf(trainingSkill.SkillGroup);
                        if (sgidx >= 0)
                        {
                            lbSkills.Invalidate(lbSkills.GetItemRectangle(sgidx));
                        }
                        UpdateSkillHeaderStats();
                    }
                }
            }
            if (m_grandCharacterInfo.DLComplete &&
                m_grandCharacterInfo.SerialSIT != null &&
                m_grandCharacterInfo.CurrentlyTrainingSkill != null &&
                m_grandCharacterInfo.SerialSIT.isSkillInTraining &&
                !m_grandCharacterInfo.SerialSIT.AlertRaisedAlready &&
                m_grandCharacterInfo.SerialSIT.TrainingSkillWithTypeID == m_grandCharacterInfo.CurrentlyTrainingSkill.Id)
            {
                // Trigger event on skill completion
                // The 'event' for a skill completion is in CharacterInfo.cs, depending on what is wanted
                // it should be triggered from there as all skill manipulation is done in that file.
                DateTime unadjustedLocalEndtime = m_grandCharacterInfo.SerialSIT.getTrainingEndTime.ToLocalTime();
                if (!m_grandCharacterInfo.SerialSIT.PreWarningGiven && m_settings.NotificationOffset != 0 && unadjustedLocalEndtime.AddSeconds(-m_settings.NotificationOffset) < DateTime.Now)
                {
                    m_grandCharacterInfo.SerialSIT.PreWarningGiven = true;
                    // here we raise a msg about skill about to complete and to get your butt into game and prepare the next skill.
                    if (m_settings.EnableBalloonTips)
                    {
                        string skillLevelString = Skill.GetRomanForInt(m_grandCharacterInfo.CurrentlyTrainingSkill.Level + 1);

                        string timeLeft = "";
                        int min = (int)Math.Floor((double)(m_settings.NotificationOffset / 60));
                        int sec = m_settings.NotificationOffset % 60;

                        if (min > 0)
                        {
                            timeLeft += String.Format("{0}m", min);
                        }
                        if (min > 0 && sec > 0)
                        {
                            timeLeft += ", ";
                        }
                        if (sec > 0)
                        {
                            timeLeft += String.Format("{0}s", sec);
                        }

                        string sa = String.Format("{0} will finish learning {1} {2} in: {3}.",
                                                    m_charName,
                                                    m_grandCharacterInfo.AllSkillsByTypeID[m_grandCharacterInfo.SerialSIT.TrainingSkillWithTypeID].Name,
                                                    skillLevelString,
                                                    timeLeft);
                        Program.MainWindow.ShowBalloonTip("EVEMon - Skill Completion Imminent", "Skill Completion Imminent", sa);
                    }
                }
                if (!m_grandCharacterInfo.SerialSIT.AlertRaisedAlready && unadjustedLocalEndtime < DateTime.Now)
                {
                    m_grandCharacterInfo.SerialSIT.AlertRaisedAlready = true;
                    // The following line triggers the required code in GrandCharacterInfo.cs for that character
                    m_grandCharacterInfo.triggerSkillComplete(m_charName);
                    UpdateSkillHeaderStats();
                }
            }
            if (m_updatingPortrait == false && pbCharImage.Image == null)
            {
                m_updatingPortrait = true;
                GetCharacterImage();
            }
        }
        #endregion

        #region Simple Properties
        /// <summary>
        /// Gets the grand character info.
        /// </summary>
        /// <value>The grand character info.</value>
        public CharacterInfo GrandCharacterInfo
        {
            get { return m_grandCharacterInfo; }
        }

        public TimeSpan ShortTimeSpan { get { return m_lcdTimeSpan; } }

        public Image CharacterPortrait { get { return pbCharImage.Image; } }
        #endregion

        #region Saving the Character

        /// <summary>
        /// Saves the character skill list as a PNG screenshot
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void SavePNGScreenshot(string fileName)
        {
            int cachedHeight = lbSkills.Height;
            lbSkills.Dock = System.Windows.Forms.DockStyle.None;
            lbSkills.Height = lbSkills.PreferredHeight;
            lbSkills.Update();

            Bitmap bitmap = new Bitmap(lbSkills.Width, lbSkills.PreferredHeight);
            lbSkills.DrawToBitmap(bitmap, new Rectangle(0, 0, lbSkills.Width, lbSkills.PreferredHeight));
            bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

            lbSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            lbSkills.Height = cachedHeight;
            lbSkills.Update();

            this.Invalidate();
        }

        private String getSkillLevelImageURL(int aSkillLevel)
        {
            String result = @"[img]http://myeve.eve-online.com/bitmaps/character/level" + aSkillLevel.ToString() + ".gif[/img]";
            return result;
        }

        public void CopyBBCodeToClipBoard()
        {
            SerializableCharacterSheet ci = m_grandCharacterInfo.ExportSerializableCharacterSheet();

            StringBuilder result = new StringBuilder();

            result.AppendLine("[b]" + ci.CharacterSheet.Name + "[/b]");
            result.AppendLine("");
            result.AppendLine("[b]Attributes[/b]");
            result.AppendLine("Intelligence: " + ci.CharacterSheet.Attributes.AdjustedIntelligence.ToString("#0.00").PadLeft(5));
            result.AppendLine("Perception: " + ci.CharacterSheet.Attributes.AdjustedPerception.ToString("#0.00").PadLeft(5));
            result.AppendLine("Charisma: " + ci.CharacterSheet.Attributes.AdjustedCharisma.ToString("#0.00").PadLeft(5));
            result.AppendLine("Willpower: " + ci.CharacterSheet.Attributes.AdjustedWillpower.ToString("#0.00").PadLeft(5));
            result.AppendLine("Memory: " + ci.CharacterSheet.Attributes.AdjustedMemory.ToString("#0.00").PadLeft(5));

            foreach (SerializableSkillGroup sg in ci.SkillGroups)
            {
                result.AppendLine("");
                result.AppendLine("[b]" + sg.Name + "[/b]");

                foreach (SerializableSkill s in sg.Skills)
                {
                    result.AppendLine(getSkillLevelImageURL(s.LastConfirmedLevel) + " " + s.Name);
                }

                result.AppendLine("Total Skillpoints in Group: " + sg.GetTotalPoints().ToString("#,##0"));
            }

            result.AppendLine("");
            result.AppendLine("Total Skillpoints: " + m_grandCharacterInfo.SkillPointTotal.ToString("#,##0"));
            result.AppendLine("Total Number of Skills: " + m_grandCharacterInfo.KnownSkillCount.ToString());
            result.AppendLine("");
            result.AppendLine("Skills at Level 1: " + m_grandCharacterInfo.SkillCountAtLevel(1).ToString());
            result.AppendLine("Skills at Level 2: " + m_grandCharacterInfo.SkillCountAtLevel(2).ToString());
            result.AppendLine("Skills at Level 3: " + m_grandCharacterInfo.SkillCountAtLevel(3).ToString());
            result.AppendLine("Skills at Level 4: " + m_grandCharacterInfo.SkillCountAtLevel(4).ToString());
            result.AppendLine("Skills at Level 5: " + m_grandCharacterInfo.SkillCountAtLevel(5).ToString());

            Clipboard.SetText(result.ToString());
        }

        /// <summary>
        /// Saves character data as a HTML file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void SaveHTMLFile(string fileName)
        {
            SerializableCharacterSheet scs = m_grandCharacterInfo.ExportSerializableCharacterSheet();
            SerializableCharacterInfo sci = scs.CreateSerializableCharacterInfo();

            try
            {
                // Part one - Generate XML
                Stream xmlStream = new MemoryStream(32767);
                XPathDocument xpdoc;
                try
                {
                    using (XmlTextWriter xtw = new XmlTextWriter(xmlStream, Encoding.UTF8))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(SerializableCharacterInfo));
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        xs.Serialize(xtw, sci, ns);
                        xtw.Flush();

                        // Part Two - Add Adjusted Attributes to XML with some stream magic
                        MemoryStream ms = (MemoryStream)xmlStream;
                        ms.Position = 0;
                        XmlDocument doc = new XmlDocument();
                        doc.Load(new StreamReader(ms));

                        XmlNode n = doc.SelectSingleNode("/character/attributes");
                        if (n == null)
                        {
                            throw new Exception("Unable to insert adjusted attributes. Node not found.");
                        }

                        XmlElement childnode = doc.CreateElement("adjustedIntelligence");
                        childnode.InnerText = sci.Attributes.AdjustedIntelligence.ToString();
                        n.AppendChild(childnode);

                        childnode = doc.CreateElement("adjustedCharisma");
                        childnode.InnerText = sci.Attributes.AdjustedCharisma.ToString();
                        n.AppendChild(childnode);

                        childnode = doc.CreateElement("adjustedMemory");
                        childnode.InnerText = sci.Attributes.AdjustedMemory.ToString();
                        n.AppendChild(childnode);

                        childnode = doc.CreateElement("adjustedPerception");
                        childnode.InnerText = sci.Attributes.AdjustedPerception.ToString();
                        n.AppendChild(childnode);

                        childnode = doc.CreateElement("adjustedWillpower");
                        childnode.InnerText = sci.Attributes.AdjustedWillpower.ToString();
                        n.AppendChild(childnode);

                        // Reset Memory Stream Position and Save XmlDocument
                        ms.Position = 0;
                        doc.Save(ms);

                        // Reset Memory Stream Position again for XPathDocument
                        ms.Position = 0;
                        using (StreamReader tr = new StreamReader(ms))
                        {
                            xpdoc = new XPathDocument(tr);
                        }

                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    return;
                }
                finally
                {
                    xmlStream.Dispose();
                }

                // Part Three - Transform to HTML

                XslCompiledTransform xstDoc2 = new XslCompiledTransform();
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("EVEMon.output-html.xsl"))
                {
                    using (XmlTextReader xtr = new XmlTextReader(s))
                    {
                        xstDoc2.Load(xtr);
                    }
                }

                using (StreamWriter sw = new StreamWriter(fileName, false))
                using (XmlTextWriter xtw = new XmlTextWriter(sw))
                {
                    xtw.Indentation = 1;
                    xtw.IndentChar = '\t';
                    xtw.Formatting = Formatting.Indented;
                    xstDoc2.Transform(xpdoc, null, xtw);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                MessageBox.Show("Failed to save:\n" + ex.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves character to a file in one of a variety of formats.  Does clever things with xsl to avoid having a seperate xml and html method.
        /// </summary>
        /// <param name="saveFormat">The save format.</param>
        /// <param name="fileName">Name of the file.</param>
        private void SaveFile(SaveFormat saveFormat, string fileName)
        {
            if (saveFormat == SaveFormat.Text)
            {
                SerializableCharacterSheet ci = m_grandCharacterInfo.ExportSerializableCharacterSheet();
                ci.SaveTextFile(fileName);
                return;
            }
            else if (saveFormat == SaveFormat.Png)
            {
                SavePNGScreenshot(fileName);
                return;
            }
            else if (saveFormat == SaveFormat.Xml)
            {
                Stream outerStream;
                outerStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                try
                {
                    using (XmlTextWriter xtw = new XmlTextWriter(outerStream, Encoding.UTF8))
                    {
                        xtw.Indentation = 1;
                        xtw.IndentChar = '\t';
                        xtw.Formatting = Formatting.Indented;
                        XmlSerializer xs = new XmlSerializer(typeof(SerializableCharacterSheet));
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        xs.Serialize(xtw, m_grandCharacterInfo.ExportSerializableCharacterSheet(), ns);
                        xtw.Flush();
                        return;
                    }
                }
                finally
                {
                    outerStream.Dispose();
                }
            }
            else if (saveFormat == SaveFormat.Html)
            {
                SaveHTMLFile(fileName);
                return;
            }

            // we're here - must be old style xml
            try
            {
                Stream xmlStream;
                XPathDocument xpdoc;
                if (saveFormat == SaveFormat.OldXml)
                {
                    xmlStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                }
                else
                {
                    xmlStream = new MemoryStream(32767);
                }
                try
                {
                    using (XmlTextWriter xtw = new XmlTextWriter(xmlStream, Encoding.UTF8))
                    {
                        if (saveFormat == SaveFormat.OldXml)
                        {
                            xtw.Indentation = 1;
                            xtw.IndentChar = '\t';
                            xtw.Formatting = Formatting.Indented;
                        }
                        XmlSerializer xs = new XmlSerializer(typeof(SerializableCharacterInfo));
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        SerializableCharacterSheet scs = m_grandCharacterInfo.ExportSerializableCharacterSheet();
                        SerializableCharacterInfo sci = scs.CreateSerializableCharacterInfo();
                        xs.Serialize(xtw, sci, ns);
                        xtw.Flush();

                        if (saveFormat == SaveFormat.OldXml)
                        {
                            return;
                        }

                        MemoryStream ms = (MemoryStream)xmlStream;
                        ms.Position = 0;
                        using (StreamReader tr = new StreamReader(ms))
                        {
                            xpdoc = new XPathDocument(tr);
                        }
                    }
                }
                finally
                {
                    xmlStream.Dispose();
                }

                XslCompiledTransform xstDoc2 = new XslCompiledTransform();
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("EVEMon.output-" + saveFormat.ToString().ToLower() + ".xsl"))
                {
                    using (XmlTextReader xtr = new XmlTextReader(s))
                    {
                        xstDoc2.Load(xtr);
                    }
                }

                using (StreamWriter sw = new StreamWriter(fileName, false))
                using (XmlTextWriter xtw = new XmlTextWriter(sw))
                {
                    xtw.Indentation = 1;
                    xtw.IndentChar = '\t';
                    xtw.Formatting = Formatting.Indented;
                    xstDoc2.Transform(xpdoc, null, xtw);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                MessageBox.Show("Failed to save:\n" + ex.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Throbber Management

        private void StartThrobber()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(StartThrobber));
                return;
            }

            throbber.State = Throbber.ThrobberState.Rotating;
            ttToolTip.SetToolTip(throbber, "Retrieving data from EVE Online...");
            ttToolTip.IsBalloon = true;
        }

        private void StopThrobber()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(StopThrobber));
                return;
            }
            throbber.State = Throbber.ThrobberState.Stopped;
        }

        private void SetErrorThrobber()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(SetErrorThrobber));
                return;
            }
            throbber.State = Throbber.ThrobberState.Strobing;
        }
        #endregion

        /// <summary>
        /// Deals with the implant changes
        /// </summary>
        private void manualImplantGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowManualImplantGroups();
        }

        public void ShowManualImplantGroups()
        {
            using (ImpGroups.ImpGroups f = new ImpGroups.ImpGroups(m_grandCharacterInfo))
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    m_grandCharacterInfo.SuppressEvents();
                    try
                    {
                        foreach (string s in f.ResultBonuses.Keys)
                        {
                            if (s != "Auto")
                            {
                                m_grandCharacterInfo.implantSets[s] = new ImplantSet(f.ResultBonuses[s].Array);
                            }
                        }
                        List<string> removesets = new List<string>();
                        foreach (string s in m_grandCharacterInfo.implantSets.Keys)
                        {
                            if (!f.ResultBonuses.ContainsKey(s) && s != "Auto")
                            {
                                removesets.Add(s);
                            }
                        }
                        foreach (string s in removesets)
                        {
                            m_grandCharacterInfo.implantSets.Remove(s);
                        }
                        m_grandCharacterInfo.ImplantBonuses.Clear();
                        if (m_grandCharacterInfo.implantSets.ContainsKey("Auto"))
                        {
                            if (!m_grandCharacterInfo.implantSets.ContainsKey("Current"))
                            {
                                for (int i = 0; i < m_grandCharacterInfo.implantSets["Auto"].Array.GetLength(0); i++)
                                {
                                    UserImplant x = m_grandCharacterInfo.implantSets["Auto"].Array[i];
                                    if (x != null)
                                    {
                                        m_grandCharacterInfo.ImplantBonuses.Add(x);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < Math.Max(m_grandCharacterInfo.implantSets["Auto"].Array.GetLength(0), m_grandCharacterInfo.implantSets["Current"].Array.GetLength(0)); i++)
                                {
                                    UserImplant x = null;
                                    if (i < m_grandCharacterInfo.implantSets["Auto"].Array.GetLength(0))
                                    {
                                        x = m_grandCharacterInfo.implantSets["Auto"].Array[i];
                                    }
                                    UserImplant y = null;
                                    if (i < m_grandCharacterInfo.implantSets["Current"].Array.GetLength(0))
                                    {
                                        y = m_grandCharacterInfo.implantSets["Current"].Array[i];
                                    }
                                    if (y != null)
                                    {
                                        m_grandCharacterInfo.ImplantBonuses.Add(y);
                                    }
                                    else if (x != null)
                                    {
                                        m_grandCharacterInfo.ImplantBonuses.Add(x);
                                    }
                                }
                            }
                        }
                        else if (m_grandCharacterInfo.implantSets.ContainsKey("Current"))
                        {
                            for (int i = 0; i < m_grandCharacterInfo.implantSets["Current"].Array.GetLength(0); i++)
                            {
                                UserImplant x = m_grandCharacterInfo.implantSets["Current"].Array[i];
                                if (x != null)
                                {
                                    m_grandCharacterInfo.ImplantBonuses.Add(x);
                                }
                            }
                        }
                    }
                    finally
                    {
                        m_grandCharacterInfo.ResumeEvents();
                    }
                }
            }
        }

        public String GetPlanKey()
        {
            return (m_cfi == null) ? m_grandCharacterInfo.Name : m_cfi.Filename;
        }

        public EveSession Session
        {
            get { return m_session; }
        }

        private void btnAddToCalendar_Click(object sender, EventArgs e)
        {
            // Ensure that we are trying to use the external calendar.
            if (!m_settings.UseExternalCalendar)
            {
                btnAddToCalendar.Visible = false;
                return;
            }

            // Call the confirmation dialog. This will allow you to check the default settings.
            // Also much easier to change the reminder settings on the fly, because it may not
            // always be convenient to use the Alternate Reminders.
            ExternalCalendar.ExternalCalendar externalCalendar = new EVEMon.ExternalCalendar.ExternalCalendar();
            externalCalendar.GrandCharacterInfo = m_grandCharacterInfo;
            externalCalendar.ApplicationSettings = m_settings;
            externalCalendar.SkillTrainingName = m_skillTrainingName;
            externalCalendar.EstimatedCompletion = m_estimatedCompletion;
            externalCalendar.DoAppointment();
        }
    }
}
