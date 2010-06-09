//#define DEBUG_SINGLETHREAD
using System;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using EVEMon.Common;
using EVEMon.Controls;
using EVEMon.Common.Net;
using EVEMon.Accounting;
using EVEMon.SkillPlanner;
using EVEMon.ExternalCalendar;
using EVEMon.Common.Scheduling;
using EVEMon.Common.Notifications;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public partial class CharacterMonitor : UserControl
    {
        private readonly Character m_character;
        private bool m_pendingUpdate;
        private bool m_loaded;

        /// <summary>
        /// Design-time constructor
        /// </summary>
        private CharacterMonitor()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.lblCharacterName.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            this.lblCurrentlyTraining.Font = FontFactory.GetFont("Tahoma", FontStyle.Bold);
            this.lblScheduleWarning.Font = FontFactory.GetFont("Tahoma", FontStyle.Bold);
            this.lblSkillHeader.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);

            this.lblIntelligence.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);
            this.lblMemory.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);
            this.lblCharisma.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);
            this.lblWillpower.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);
            this.lblPerception.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);
            this.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);

            throbber.Click += new EventHandler(throbber_Click);
            multiPanel.SelectionChange += new MultiPanelSelectionChangeHandler(multiPanel_SelectionChange);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterMonitor(Character character)
            :this()
        {
            m_character = character;
            this.skillsList.Character = character;
            this.skillQueueList.Character = character;
            this.ordersList.Character = character;
            this.jobsList.Character = character;
            notificationList.Notifications = null;

            if (character is CCPCharacter)
            {
                var ccpCharacter = (CCPCharacter)character;
                skillQueueControl.SkillQueue = ccpCharacter.SkillQueue;
                miQueryEverything.Enabled = true;
            }
            else
            {
                pnlTraining.Visible = false;
                skillQueuePanel.Visible = false;
                skillQueueIcon.Visible = false;
                ordersIcon.Visible = false;
                jobsIcon.Visible = false;
            }

            // Subscribe events
            EveClient.TimerTick += new EventHandler(EveClient_TimerTick);
            EveClient.SettingsChanged += new EventHandler(EveClient_SettingsChanged);
            EveClient.SchedulerChanged += new EventHandler(EveClient_SchedulerChanged);
            EveClient.CharacterChanged += new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.NotificationSent += new EventHandler<Notification>(EveClient_NotificationSent);
            EveClient.NotificationInvalidated += new EventHandler<NotificationInvalidationEventArgs>(EveClient_NotificationInvalidated);
            this.Disposed += new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDisposed(object sender, EventArgs e)
        {
            EveClient.TimerTick -= new EventHandler(EveClient_TimerTick);
            EveClient.SettingsChanged -= new EventHandler(EveClient_SettingsChanged);
            EveClient.SchedulerChanged -= new EventHandler(EveClient_SchedulerChanged);
            EveClient.CharacterChanged -= new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.NotificationSent -= new EventHandler<Notification>(EveClient_NotificationSent);
            EveClient.NotificationInvalidated -= new EventHandler<NotificationInvalidationEventArgs>(EveClient_NotificationInvalidated);
            this.Disposed -= new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        public Character Character
        {
            get { return m_character; }
        }

        #region Inherited events
        /// <summary>
        /// On load, we subscribe the events, start the timers, etc...
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            m_loaded = false;
            base.OnLoad(e);

            // Picks the last selected page
            multiPanel.SelectedPage = null;
            var tag = m_character.UISettings.SelectedPage;
            var item = toolStrip.Items.Cast<ToolStripItem>().FirstOrDefault(x => tag == x.Tag as string && x.Visible);
            toolbarIcon_Click((item ?? skillsIcon), null);

            // Updates the rest of the control
            EveClient_SettingsChanged(null, null);
            warningLabel.Visible = false;
            UpdateContent();
            m_loaded = true;
        }

        /// <summary>
        /// On visibility, we may need to refresh the display.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (m_pendingUpdate)
                UpdateContent();

            base.OnVisibleChanged(e);
        }
        #endregion


        #region Display update on character change

        /// <summary>
        /// Updates all the content
        /// </summary>
        /// <remarks>Another high-complexity method for us to look at.</remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EVEMon.Common.SkillChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateContent()
        {
            if (!this.Visible)
            {
                m_pendingUpdate = true;
                return;
            }
            m_pendingUpdate = false;

            // Display the "no skills" label if there's no skills
            this.SuspendLayout();
            try
            {
                miChangeInfo.Enabled = (m_character is CCPCharacter);

                // Update the other controls
                UpdateCharacterInfo();
                UpdateSkillHeaderStats();
                UpdateErrorInfo();

                // Update the rest of the controls
                EveClient_TimerTick(null, EventArgs.Empty);
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// Creates a public callable function to update the characterinfo
        /// </summary>
        public void UpdateCharacterInfo()
        {
            // Gender, race, bloodline, corporation
            lblBioInfo.Text = String.Format(CultureConstants.DefaultCulture, "{0} {1} {2}", m_character.Gender, m_character.Race, m_character.Bloodline);
            lblCorpInfo.Text = String.Format(CultureConstants.DefaultCulture, "Corporation: {0}", m_character.CorporationName);

            // Balance
            lblBalanceAmount.Text = String.Format(CultureConstants.DefaultCulture, "{0:N}", m_character.Balance);
            var ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter != null && ccpCharacter.HasInsufficientBalance)
            {
                lblBalanceAmount.ForeColor = Color.Orange;
                lblBalanceAmount.Font = new Font(Font, FontStyle.Bold);
            }
            else
            {
                lblBalanceAmount.ForeColor = SystemColors.ControlText;
                lblBalanceAmount.Font = new Font(Font, FontStyle.Regular);
            }

            //Assigning new positions to labels as the designer fails to position them correctly
            lblBalanceAmount.Location = new Point(lblBalanceText.Width - 4);
            lblBalanceISK.Location = new Point(lblBalanceAmount.Location.X + lblBalanceAmount.Width - 4);

            // Name
            lblCharacterName.Text = m_character.AdornedName;

            // Attributes
            SetAttributeLabel(lblIntelligence, EveAttribute.Intelligence);
            SetAttributeLabel(lblPerception, EveAttribute.Perception);
            SetAttributeLabel(lblWillpower, EveAttribute.Willpower);
            SetAttributeLabel(lblCharisma, EveAttribute.Charisma);
            SetAttributeLabel(lblMemory, EveAttribute.Memory);
        }

        /// <summary>
        /// Updates skill summary info (the block just below the portrait).
        /// </summary>
        private void UpdateSkillHeaderStats()
        {
            // Update the known skills count, total SP, skills at lv5, clone limit
            StringBuilder header = new StringBuilder();

            header.AppendFormat(CultureConstants.DefaultCulture, "Known Skills: {0}{1}", m_character.KnownSkillCount, Environment.NewLine);
            header.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level V: {0}{1}", m_character.GetSkillCountAtLevel(5), Environment.NewLine);
            header.AppendFormat(CultureConstants.DefaultCulture, "Total SP: {0:#,##0}{1}", m_character.SkillPoints, Environment.NewLine);
            header.AppendFormat(CultureConstants.DefaultCulture, "Clone Limit: {0:#,##0}{1}", m_character.CloneSkillPoints, Environment.NewLine);
            header.Append(m_character.CloneName);

            lblSkillHeader.Text = header.ToString();
        }

        /// <summary>
        /// Sets the specified attribute label.
        /// </summary>
        /// <param name="lblAttrib">The LBL attrib.</param>
        /// <param name="eveAttribute">The eve attribute.</param>
        private void SetAttributeLabel(Label lblAttrib, EveAttribute eveAttribute)
        {
            lblAttrib.Text = String.Format(CultureConstants.DefaultCulture, "{0}: {1:0.00}", eveAttribute, m_character[eveAttribute].EffectiveValue);
        }

        /// <summary>
        /// Updates the informations for skill training
        /// </summary>
        private void UpdateTrainingInfo()
        {
            var ccpCharacter = m_character as CCPCharacter;

            // Is the character in training ?
            if (m_character.IsTraining)
            {
                var training = m_character.CurrentlyTrainingSkill;
                var completionTime = training.EndTime.ToLocalTime();

                lblTrainingSkill.Text = training.ToString();
                lblSPPerHour.Text = (training.Skill == null ? "???" : String.Format(CultureConstants.DefaultCulture, "{0} SP/Hour", training.Skill.SkillPointsPerHour));
                lblTrainingEst.Text = String.Format(CultureConstants.DefaultCulture, "{0} {1}", completionTime.ToString("ddd"), completionTime.ToString("G"));

                string conflictMessage;
                if (Scheduler.SkillIsBlockedAt(training.EndTime.ToLocalTime(), out conflictMessage))
                {
                    lblScheduleWarning.Text = conflictMessage;
                    lblScheduleWarning.Visible = true;
                }
                else
                {
                    lblScheduleWarning.Visible = false;
                }

                if (ccpCharacter != null)
                {
                    var queueCompletionTime = ccpCharacter.SkillQueue.EndTime.ToLocalTime();
                    lblQueueCompletionTime.Text = String.Format(CultureConstants.DefaultCulture, "{0} {1}", queueCompletionTime.ToString("ddd"), queueCompletionTime.ToString("G"));
                    if (skillQueueList.QueueHasChanged(ccpCharacter.SkillQueue.ToArray()))
                        skillQueueControl.Invalidate();
                    skillQueuePanel.Visible = true;
                    skillQueueTimePanel.Visible = ccpCharacter.SkillQueue.Count > 1 || Settings.UI.MainWindow.AlwaysShowSkillQueueTime;
                }

                pnlTraining.Visible = true;
                lblPaused.Visible = false;
                return;
            }

            // Not in training, check for paused skill queue
            if (ccpCharacter != null && ccpCharacter.SkillQueue.IsPaused)
            {
                var training = ccpCharacter.SkillQueue.CurrentlyTraining;
                lblTrainingSkill.Text = training.ToString();
                lblSPPerHour.Text = (training.Skill == null ? "???" : String.Format(CultureConstants.DefaultCulture, "{0} SP/Hour", training.Skill.SkillPointsPerHour));

                lblTrainingRemain.Text = "Paused";
                lblTrainingEst.Text = String.Empty;
                lblScheduleWarning.Visible = false;
                skillQueueTimePanel.Visible = false;
                skillQueuePanel.Visible = true;
                lblPaused.Visible = true;
                pnlTraining.Visible = true;
                return;
            }

            // Not training, no skill queue
            skillQueuePanel.Visible = false;
            pnlTraining.Visible = false;
            lblPaused.Visible = false;
        }

        /// <summary>
        /// Updates the errors feedback
        /// </summary>
        private void UpdateErrorInfo()
        {
            var ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            // TODO : Errors
        }

        /// <summary>
        /// Updates label above the throbber for the next update
        /// </summary>
        private void UpdateThrobberState()
        {
            if (m_character is UriCharacter)
            {
                throbber.State = ThrobberState.Stopped;
                throbber.Visible = false;
                lblUpdateTimer.Visible = false;
                return;
            }

            var ccpCharacter = (CCPCharacter)m_character;

            // When we're querying, the throbber must rotate.
            if (ccpCharacter.QueryMonitors.AnyUpdating)
            {
                throbber.Visible = true;
                throbber.State = ThrobberState.Rotating;
                ttToolTip.SetToolTip(throbber, "Retrieving data from EVE Online...");
                lblUpdateTimer.Visible = false;
                return;
            }

            // When an error has been encountered or we didn't updated yet, the throbber must blink.
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                throbber.Visible = true;
                throbber.State = ThrobberState.Strobing;

                ttToolTip.SetToolTip(throbber, "No network available.");
                lblUpdateTimer.Visible = false;
                return;
            }

            // Display the remaining time
            throbber.State = ThrobberState.Stopped;
            
            // Set the tooltip text
            string throbberTooltipText = (ccpCharacter.QueryMonitors.Any(x => x.ForceUpdateWillCauseError) ? String.Empty : "Click to update now");
            ttToolTip.SetToolTip(throbber, throbberTooltipText);

            // Updates the next autoupdate timer label
            var nextMonitor = ccpCharacter.QueryMonitors.NextUpdate;
            if (nextMonitor != null)
            {
                TimeSpan timeLeft = nextMonitor.NextUpdate.Subtract(DateTime.UtcNow);
                if (timeLeft < TimeSpan.Zero)
                {
                    lblUpdateTimer.Text = "Pending...";
                }
                else
                {
                    string hours = timeLeft.Hours.ToString("d2", CultureConstants.DefaultCulture);
                    string minutes = timeLeft.Minutes.ToString("d2", CultureConstants.DefaultCulture);
                    string seconds = timeLeft.Seconds.ToString("d2", CultureConstants.DefaultCulture);
                    lblUpdateTimer.Text = String.Format(CultureConstants.DefaultCulture, "{0}:{1}:{2}", hours, minutes, seconds);
                }
                lblUpdateTimer.Visible = true;
            }
            else
            {
                lblUpdateTimer.Visible = false;
            }
        }

        /// <summary>
        /// Hides or shows the warning about the insufficient key level.
        /// </summary>
        /// <param name="e"></param>
        private void UpdateWarningLabel()
        {
            var account = m_character.Identity.Account;
            if (m_loaded && account == null)
            {
                warningLabel.Text = "This character has no associated account, data won't be updated.";
                warningLabel.Visible = true;
                return;
            }

            if (multiPanel.SelectedPage == ordersPage || multiPanel.SelectedPage == jobsPage)
            {
                if (account == null)
                    return;

                switch(account.KeyLevel)
                {
                    case CredentialsLevel.Limited:
                        warningLabel.Text = "This feature requires a full API key but you only provided a limited one.";
                        warningLabel.Visible = true;
                        break;

                    case CredentialsLevel.Unknown:
                        warningLabel.Text = "The level (full or limited) of this account's key is still unknown.";
                        warningLabel.Visible = true;
                        break;

                    default:
                        warningLabel.Visible = false;
                        break;
                }
            }
            else
            {
                warningLabel.Visible = false;
            }
        }

        /// <summary>
        /// Hides or shows the pages controls.
        /// </summary>
        private void UpdatePageControls()
        {
            // Enables/Disables the skill page controls
            toggleSkillsIcon.Enabled = !m_character.Skills.IsEmpty();

            // Exit if it's a non-CCPCharacter
            var ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            // Saves any changes we've made to the market orders page columns
            // (I've tried to find a better way to deal with this but failed)
            if (multiPanel.SelectedPage == ordersPage && !ccpCharacter.MarketOrders.IsEmpty())
            {
                // Enables/Disables the market orders page controls
                groupMenu.Enabled = searchTextBox.Enabled = preferencesMenu.Enabled = !ccpCharacter.MarketOrders.IsEmpty();

                if (!ccpCharacter.MarketOrders.IsEmpty())
                {
                    Settings.UI.MainWindow.MarketOrders.Columns = ordersList.Columns.Select(x => x.Clone()).ToArray();
                    m_character.UISettings.OrdersGroupBy = ordersList.Grouping;
                }
            }

            // Saves any changes we've made to the industry jobs page columns
            if (multiPanel.SelectedPage == jobsPage && !ccpCharacter.IndustryJobs.IsEmpty())
            {
                // Enables/Disables the industry jobs page controls
                groupMenu.Enabled = searchTextBox.Enabled = preferencesMenu.Enabled = !ccpCharacter.IndustryJobs.IsEmpty();

                if (!ccpCharacter.IndustryJobs.IsEmpty())
                {
                    Settings.UI.MainWindow.IndustryJobs.Columns = jobsList.Columns.Select(x => x.Clone()).ToArray();
                    m_character.UISettings.JobsGroupBy = jobsList.Grouping;
                }
            }
        }

        #endregion


        #region Updates on global events
        /// <summary>
        /// Occur when the character changed. We update all the controls' content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_CharacterChanged(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// Updates the controls on settings change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_SettingsChanged(object sender, EventArgs e)
        {
            // Read the settings
            if (!Settings.UI.SafeForWork)
            {
                pbCharImage.Character = m_character;
                pbCharImage.Visible = true;
                skillsIcon.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                skillQueueIcon.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                ordersIcon.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                jobsIcon.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                groupMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                preferencesMenu.DisplayStyle = ToolStripItemDisplayStyle.Image;
                toggleSkillsIcon.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
            else
            {
                pbCharImage.Visible = false;
                skillsIcon.DisplayStyle = ToolStripItemDisplayStyle.Text;
                skillQueueIcon.DisplayStyle = ToolStripItemDisplayStyle.Text;
                ordersIcon.DisplayStyle = ToolStripItemDisplayStyle.Text;
                jobsIcon.DisplayStyle = ToolStripItemDisplayStyle.Text;
                groupMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
                preferencesMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
                toggleSkillsIcon.DisplayStyle = ToolStripItemDisplayStyle.Text;
            }

            // "Update Calendar" button
            btnAddToCalendar.Visible = Settings.Calendar.Enabled;

            // Skill queue time
            CCPCharacter ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter != null && ccpCharacter.SkillQueue.Count == 1)
                skillQueueTimePanel.Visible = ccpCharacter.IsTraining && Settings.UI.MainWindow.AlwaysShowSkillQueueTime;
        }

        /// <summary>
        /// Occur on every second. We update the total SP, remaining time and the matching item in skill list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_TimerTick(object sender, EventArgs e)
        {
            // No need to do anything when the control is not visible
            if (!this.Visible)
                return;

            // Update the training info
            UpdateTrainingInfo();

            // Update the throbber
            UpdateThrobberState();

            // Update the warning label
            UpdateWarningLabel();

            // Update the page controls
            UpdatePageControls();

            CCPCharacter ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;
            
            // Update character's balance info
            if (ccpCharacter.MarketOrdersUpdated)
            {
                UpdateCharacterInfo();
                ccpCharacter.MarketOrdersUpdated = false;
            }

            // Is the character in training ?
            if (ccpCharacter.IsTraining)
            {
                // Remaining training time label
                var training = m_character.CurrentlyTrainingSkill;
                lblTrainingRemain.Text = training.EndTime.ToRemainingTimeDescription();

                // Remaining queue time label
                var queueEndTime = ccpCharacter.SkillQueue.EndTime;
                lblQueueRemaining.Text = queueEndTime.ToRemainingTimeDescription();

                // Update total SP
                UpdateSkillHeaderStats();
            }
        }

        /// <summary>
        /// When the scheduler changed, we need to check the conflicts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_SchedulerChanged(object sender, EventArgs e)
        {
            UpdateTrainingInfo();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_NotificationInvalidated(object sender, NotificationInvalidationEventArgs e)
        {
            UpdateNotifications();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_NotificationSent(object sender, Notification e)
        {
            UpdateNotifications();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        void UpdateNotifications()
        {
            notificationList.Notifications = EveClient.Notifications.Where(x => x.Sender == m_character);
        }
        #endregion


        #region Control/Component Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolbarIcon_Click(object sender, EventArgs e)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                // Skip tags without tag, those ones do not represent "pages switches"
                if (item.Tag == null)
                    continue;

                // Is it the item we clicked ?
                var button = item as ToolStripButton;
                if (item == sender)
                {
                    // Selects the proper page
                    multiPanel.SelectedPage = multiPanel.Controls.Cast<MultiPanelPage>().First(x => x.Name == (string)item.Tag);

                    // Checks it.
                    button.Checked = true;
                }
                // Or another one representing another page ?
                else if (button != null)
                {
                    // Unchecks it
                    button.Checked = false;
                }
            }
        }

        /// <summary>
        /// When the selected page changes, we may have to update the warning about full key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void multiPanel_SelectionChange(object sender, MultiPanelSelectionChangeEventArgs e)
        {
            if (e.NewPage == null)
                return;

            // Hides or shows the full key warning.
            UpdateWarningLabel();

            // Stores the setting
            m_character.UISettings.SelectedPage = e.NewPage.Text;

            // Update the buttons visibility.
            toggleSkillsIcon.Visible = (e.NewPage == skillsPage);

            searchTextBox.Visible = (e.NewPage == ordersPage || e.NewPage == jobsPage);
            groupMenu.Visible = (e.NewPage == ordersPage || e.NewPage == jobsPage);
            preferencesMenu.Visible = (e.NewPage == ordersPage || e.NewPage == jobsPage);
        }

        /// <summary>
        /// When the mouse moves over the update timer, we update it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lblUpdateTimer_MouseHover(object sender, EventArgs e)
        {
            var ccpCharacter = (CCPCharacter)m_character;
            if (ccpCharacter == null)
                return;

            // Updates the autotimer label's tooltip
            var sb = new StringBuilder();
            foreach (var autoUpdate in ccpCharacter.QueryMonitors.OrderedByUpdateTime)
            {
                // Skip character's corporation market orders and industry jobs monitor,
                // cause there is no need to show them as they are bind
                // with the character's personal monitor
                if (autoUpdate.Method == APIMethods.CorporationMarketOrders
                    || autoUpdate.Method == APIMethods.CorporationIndustryJobs)
                    continue;

                var description = autoUpdate.ToString();
                sb.Append(description).Append(": ");

                // We either display the timer (when pending)...
                var status = autoUpdate.Status;
                if (status == QueryStatus.Pending)
                {
                    if (autoUpdate.NextUpdate.Year != 9999)
                    {
                        var remainingTime = autoUpdate.NextUpdate.Subtract(DateTime.UtcNow);

                        if (remainingTime.Minutes > 0)
                        {
                            sb.AppendLine(remainingTime.ToDescriptiveText(
                               DescriptiveTextOptions.FullText |
                               DescriptiveTextOptions.SpaceText |
                               DescriptiveTextOptions.SpaceBetween, false));
                        }
                        else
                        {
                            sb.AppendLine("Less than a minute");
                        }
                    }
                    else
                    {
                        sb.AppendLine("Never");
                    }
                }
                // ...or we display the status (updating, no network, full key needed, etc)
                else
                {
                    var statusDescription = status.GetDescription();
                    sb.AppendLine(statusDescription);
                }
            }

            // Sets the tooltip
            if (sb.Length != 0)
                ttToolTip.SetToolTip(lblUpdateTimer, sb.ToString());
        }

        /// <summary>
        /// Toggles all the skill groups to collapse or open.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void toggleSkillsIcon_Click(object sender, EventArgs e)
        {
            skillsList.ToggleAll();
        }

        /// <summary>
        /// Throbber's context menu > Get data from EVE Online.
        /// Request a full update for the character
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miHitEveO_Click(object sender, EventArgs e)
        {
            // This menu should not be enabled for non-ccp characters
            var ccpCharacter = (CCPCharacter)m_character;
            ccpCharacter.QueryMonitors.QueryEverything();
        }

        /// <summary>
        /// Occurs when the user click the throbber.
        /// Query the API for or a full update when possible, or show the throbber's context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void throbber_Click(object sender, EventArgs e)
        {
            if (throbber.State != ThrobberState.Strobing && m_character is CCPCharacter) 
            {
                var ccpCharacter = (CCPCharacter)m_character;
                if (!ccpCharacter.QueryMonitors.Any(x=> x.ForceUpdateWillCauseError))
                    ccpCharacter.QueryMonitors.QueryEverything();
            }
            else
            {
                throbberContextMenu.Show(MousePosition);
            }
        }

        /// <summary>
        /// When the throbber's context menu is opened, we update the "Query" menus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void throbberContextMenu_Opening(object sender, CancelEventArgs e)
        {
            // Remove all the items including the separator
            int separatorIndex = throbberContextMenu.Items.IndexOf(throbberSeparator);
            while(separatorIndex > -1 && separatorIndex < throbberContextMenu.Items.Count)
            {
                throbberContextMenu.Items.RemoveAt(separatorIndex);
            }

            // Exit for non-CCP characters or no associated account
            var ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null || ccpCharacter.Identity.Account == null)
                return;

            // Enables/disables the "query everything" menu item
            miQueryEverything.Enabled = !ccpCharacter.QueryMonitors.Any(x => x.ForceUpdateWillCauseError);

            // Add new separator before monitor items
            throbberSeparator = new ToolStripSeparator();
            throbberSeparator.Name = "throbberSeparator";
            throbberContextMenu.Items.Add(throbberSeparator);

            // Add monitor items
            foreach (var monitor in ccpCharacter.QueryMonitors)
            {
                // Skip market orders monitor if api key is a limited one
                if (monitor.IsFullKeyNeeded && ccpCharacter.Identity.Account.KeyLevel != CredentialsLevel.Full)
                    continue;

                // Skip character's corporation market orders and industry jobs monitor,
                // cause there is no need to show them as they are bind
                // with the character's personal monitor
                if (monitor.Method == APIMethods.CorporationMarketOrders
                    || monitor.Method == APIMethods.CorporationIndustryJobs)
                    continue;

                TimeSpan timeToNextUpdate = monitor.NextUpdate.Subtract(DateTime.UtcNow);
                string timeToNextUpdateText;

                if (monitor.NextUpdate.Year.Equals(9999))
                {
                    timeToNextUpdateText = "Never";
                }
                else if (timeToNextUpdate.TotalMinutes >= 60)
                {
                    timeToNextUpdateText = String.Format(CultureConstants.DefaultCulture, "{0}h", Math.Floor(timeToNextUpdate.TotalHours));
                }
                else
                {
                    timeToNextUpdateText = String.Format(CultureConstants.DefaultCulture, "{0}m", Math.Floor(timeToNextUpdate.TotalMinutes));
                }

                string menuText = String.Format(CultureConstants.DefaultCulture, "Update {0} {1}", monitor.ToString(),
                    timeToNextUpdate > TimeSpan.Zero ? String.Format(CultureConstants.DefaultCulture, "({0})", timeToNextUpdateText) : String.Empty);
                var menu = new ToolStripMenuItem(menuText);
                menu.Tag = (object)monitor.Method;
                menu.Enabled = !monitor.ForceUpdateWillCauseError;

                throbberContextMenu.Items.Add(menu);
            }
        }

        /// <summary>
        /// Throbber's context menu > "Query XXX".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void throbberContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            if (e.ClickedItem.Tag is APIMethods)
            {
                var method = (APIMethods)e.ClickedItem.Tag;
                throbber.State = ThrobberState.Rotating;
                ccpCharacter.QueryMonitors.Query(method);

                if (method == APIMethods.MarketOrders)
                    ccpCharacter.QueryMonitors.Query(APIMethods.CorporationMarketOrders);

                if (method == APIMethods.IndustryJobs)
                    ccpCharacter.QueryMonitors.Query(APIMethods.CorporationIndustryJobs);
            }

        }

        /// <summary>
        /// Throbber's context menu > Change API Key information.
        /// Prompt the window to get the new API credentials.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void miChangeInfo_Click(object sender, EventArgs e)
        {
            // This menu should be enabled only for CCP characters with non-null accounts.
            using (AccountUpdateOrAdditionWindow f = new AccountUpdateOrAdditionWindow(m_character.Identity.Account))
            {
                f.ShowDialog();
            }
        }

        /// <summary>
        /// When the user hovers over one of the attribute label, we display a tooltip such as :
        /// 19.8 [(7 base + 7 skills + 4 implants) * 1.10 from learning bonus]
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void lblAttribute_MouseHover(object sender, EventArgs e)
        {
            // retrieve the attribute from the sender
            Label lblAttrib = (Label)sender;
            EveAttribute eveAttribute = EveAttribute.None;
            switch (lblAttrib.Text.Split(':')[0])
            {
                case "Intelligence": eveAttribute = EveAttribute.Intelligence; break;
                case "Charisma": eveAttribute = EveAttribute.Charisma; break;
                case "Memory": eveAttribute = EveAttribute.Memory; break;
                case "Willpower": eveAttribute = EveAttribute.Willpower; break;
                case "Perception": eveAttribute = EveAttribute.Perception; break;
                default: break;
            }

            // Retrieve the values
            var attribute = m_character[eveAttribute];
            string str = attribute.ToString("%e [(%b base + %s skills + %i implants) * %f from learning bonus]");

            ttToolTip.SetToolTip(lblAttrib, str);
        }

        /// <summary>
        /// When the user hovers the skills states (skills count, at lv5, etc), displays a tool for the number if skills on every level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSkillHeader_MouseHover(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= 5; i++)
            {
                int count = m_character.GetSkillCountAtLevel(i);

                if (i > 1)
                    sb.Append("\n");

                sb.AppendFormat(CultureConstants.DefaultCulture, "{0} Skills at Level {1}", count, i);
            }

            ttToolTip.SetToolTip(sender as Label, sb.ToString());
        }

        /// <summary>
        /// Occurs when the user click the "add to calendar" button.
        /// We open the unique external calendar window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddToCalendar_Click(object sender, EventArgs e)
        {
            // Ensure that we are trying to use the external calendar.
            if (!Settings.Calendar.Enabled)
            {
                btnAddToCalendar.Visible = false;
                return;
            }

            WindowsFactory<ExternalCalendarWindow>.ShowByTag(m_character);
        }

        /// <summary>
        /// Notification list was resized, this may affect the skills list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notificationList_Resize(object sender, EventArgs e)
        {
            UpdateNotifications();
            skillsList.Invalidate();
        }
        #endregion


        # region Screenshot Method
        /// <summary>
        /// Takes a screeenshot of this character's monitor and returns it (used for PNG exportation)
        /// </summary>
        /// <returns></returns>
        internal Bitmap GetCharacterScreenshot()
        {
            int cachedHeight = skillsList.Height;
            int preferredHeight = skillsList.PreferredSize.Height;

            skillsList.Dock = System.Windows.Forms.DockStyle.None;
            skillsList.Height = preferredHeight;
            skillsList.Update();

            Bitmap bitmap = new Bitmap(skillsList.Width, preferredHeight);
            skillsList.DrawToBitmap(bitmap, new Rectangle(0, 0, skillsList.Width, preferredHeight));

            skillsList.Dock = System.Windows.Forms.DockStyle.Fill;
            skillsList.Height = cachedHeight;
            skillsList.Update();

            this.Invalidate();
            return bitmap;
        }
        # endregion


        # region Multi Panel Control/Component Event Handlers
        /// <summary>
        /// On opening we create the menu items for "Group By..." in panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void groupMenu_DropDownOpening(object sender, EventArgs e)
        {
            groupMenu.DropDownItems.Clear();

            if (multiPanel.SelectedPage == ordersPage)
            {
                foreach (var grouping in EnumExtensions.GetValues<MarketOrderGrouping>())
                {
                    var menu = new ToolStripButton(grouping.GetHeader());
                    menu.Checked = (ordersList.Grouping == grouping);
                    menu.Tag = (object)grouping;

                    groupMenu.DropDownItems.Add(menu);
                }
            }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                foreach (var grouping in EnumExtensions.GetValues<IndustryJobGrouping>())
                {
                    var menu = new ToolStripButton(grouping.GetHeader());
                    menu.Checked = (jobsList.Grouping == grouping);
                    menu.Tag = (object)grouping;

                    groupMenu.DropDownItems.Add(menu);
                }
            }
        }

        /// <summary>
        /// Occurs when the user click an item in the "Group By..." menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void groupMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            if (multiPanel.SelectedPage == ordersPage)
            {
                var grouping = (MarketOrderGrouping)item.Tag;
                ordersList.Grouping = grouping;
            }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                var grouping = (IndustryJobGrouping)item.Tag;
                jobsList.Grouping = grouping;
            }
        }

        /// <summary>
        /// Occurs when the search text changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (multiPanel.SelectedPage == ordersPage)
            {
                ordersList.TextFilter = searchTextBox.Text;
            }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                jobsList.TextFilter = searchTextBox.Text;
            }
        }

        /// <summary>
        /// Display the window to select columns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void columnSettingsMenuItem_Click(object sender, EventArgs e)
        {
            if (multiPanel.SelectedPage == ordersPage)
            {
                using (var f = new MarketOrdersColumnsSelectWindow(ordersList.Columns.Select(x => x.Clone())))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        ordersList.Columns = f.Columns.Cast<MarketOrderColumnSettings>();
                        ordersList.UpdateColumns();
                    }
                }
             }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                using (var f = new IndustryJobsColumnsSelectWindow(jobsList.Columns.Select(x => x.Clone())))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        jobsList.Columns = f.Columns.Cast<IndustryJobColumnSettings>();
                        jobsList.UpdateColumns();
                    }
                }
            }
       }

        /// <summary>
        /// On menu opening we update the menu items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void preferencesMenu_DropDownOpening(object sender, EventArgs e)
        {
            bool hideInactive = true;
            bool numberFormat = false;

            if (multiPanel.SelectedPage == ordersPage)
            {
                hideInactive = Settings.UI.MainWindow.MarketOrders.HideInactiveOrders;
                numberFormat = Settings.UI.MainWindow.MarketOrders.NumberAbsFormat;
                preferencesMenu.DropDownItems.Insert(3, numberAbsFormatMenuItem);
                numberAbsFormatMenuItem.Text = (numberFormat ? "Number Full Format" : "Number Abbreviating Format");
                showOnlyCharMenuItem.Checked = ordersList.ShowIssuedFor == IssuedFor.Character;
                showOnlyCorpMenuItem.Checked = ordersList.ShowIssuedFor == IssuedFor.Corporation;
            }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                hideInactive = Settings.UI.MainWindow.IndustryJobs.HideInactiveJobs;
                preferencesMenu.DropDownItems.Remove(numberAbsFormatMenuItem);
                showOnlyCharMenuItem.Checked = jobsList.ShowIssuedFor == IssuedFor.Character;
                showOnlyCorpMenuItem.Checked = jobsList.ShowIssuedFor == IssuedFor.Corporation;
            }

            hideInactiveMenuItem.Text = (hideInactive ? "Unhide Inactive" : "Hide Inactive");
        }

        /// <summary>
        /// Hide/Show the inactive entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void hideInactiveMenuItem_Click(object sender, EventArgs e)
        {
            bool hideInactive = true;
            if (multiPanel.SelectedPage == ordersPage)
            {
                hideInactive = Settings.UI.MainWindow.MarketOrders.HideInactiveOrders;
                Settings.UI.MainWindow.MarketOrders.HideInactiveOrders = !hideInactive;
                ordersList.UpdateColumns();
            }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                hideInactive = Settings.UI.MainWindow.IndustryJobs.HideInactiveJobs;
                Settings.UI.MainWindow.IndustryJobs.HideInactiveJobs = !hideInactive;
                jobsList.UpdateColumns();
            }
            hideInactiveMenuItem.Text = (!hideInactive ? "Unhide Inactive" : "Hide Inactive");
        }

        /// <summary>
        /// Switches between Abbreviating/Full number format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void numberAbsFormatMenuItem_Click(object sender, EventArgs e)
        {
            bool numberFormat = Settings.UI.MainWindow.MarketOrders.NumberAbsFormat;
            numberAbsFormatMenuItem.Text = (!numberFormat ? "Number Full Format" : "Number Abbreviating Format");
            Settings.UI.MainWindow.MarketOrders.NumberAbsFormat = !numberFormat;
            ordersList.UpdateColumns();
        }

        /// <summary>
        /// Displays only the entries issued for character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void showOnlyCharMenuItem_Click(object sender, EventArgs e)
        {
            if (multiPanel.SelectedPage == ordersPage)
            {
                ordersList.ShowIssuedFor = (showOnlyCharMenuItem.Checked ? IssuedFor.Character : IssuedFor.All);
                showOnlyCorpMenuItem.Checked = ordersList.ShowIssuedFor == IssuedFor.Corporation;
            }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                jobsList.ShowIssuedFor = (showOnlyCharMenuItem.Checked ? IssuedFor.Character : IssuedFor.All);
                showOnlyCorpMenuItem.Checked = jobsList.ShowIssuedFor == IssuedFor.Corporation;
            }
        }

        /// <summary>
        /// Displays only the entries issued for corporation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void showOnlyCorpMenuItem_Click(object sender, EventArgs e)
        {
            if (multiPanel.SelectedPage == ordersPage)
            {
                ordersList.ShowIssuedFor = (showOnlyCorpMenuItem.Checked ? IssuedFor.Corporation : IssuedFor.All);
                showOnlyCharMenuItem.Checked = ordersList.ShowIssuedFor == IssuedFor.Character;
            }
            else if (multiPanel.SelectedPage == jobsPage)
            {
                jobsList.ShowIssuedFor = (showOnlyCorpMenuItem.Checked ? IssuedFor.Corporation : IssuedFor.All);
                showOnlyCharMenuItem.Checked = jobsList.ShowIssuedFor == IssuedFor.Character;
            }
        }
        # endregion


        #region Testing Function
        /// <summary>
        /// Tests character's notification display in the Character Monitor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void TestCharacterNotification()
        {
            var notification = new Notification(NotificationCategory.TestNofitication, m_character);
            notification.Priority = NotificationPriority.Warning;
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Description = "Test Character Notification.";
            EveClient.Notifications.Notify(notification);
        }
        #endregion
    }
}