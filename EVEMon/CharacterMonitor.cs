//#define DEBUG_SINGLETHREAD
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using EVEMon.Common;
using EVEMon.Common.Notifications;
using EVEMon.Common.Scheduling;
using EVEMon.Common.SettingsObjects;
using EVEMon.Controls;
using EVEMon.ExternalCalendar;

namespace EVEMon
{
    /// <summary>
    /// Implements the content of each of the character tabs.
    /// </summary>
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

            this.lblScheduleWarning.Font = FontFactory.GetFont("Tahoma", FontStyle.Bold);
            this.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);

            multiPanel.SelectionChange += new MultiPanelSelectionChangeHandler(multiPanel_SelectionChange);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMonitor"/> class.
        /// </summary>
        /// <param name="character">The character.</param>
        public CharacterMonitor(Character character)
            : this()
        {
            m_character = character;
            this.skillsList.Character = character;
            this.skillQueueList.Character = character;
            this.ordersList.Character = character;
            this.jobsList.Character = character;
            this.Header.Character = character;
            notificationList.Notifications = null;

            if (character is CCPCharacter)
            {
                var ccpCharacter = (CCPCharacter)character;
                skillQueueControl.SkillQueue = ccpCharacter.SkillQueue;
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// <value>The character.</value>
        public Character Character
        {
            get { return m_character; }
        }

        #region Inherited events

        /// <summary>
        /// On load, we subscribe the events, start the timers, etc...
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
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
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
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
                // Update the other controls
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
        /// Hides or shows the warning about the insufficient key level.
        /// </summary>
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

                switch (account.KeyLevel)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveClient_CharacterChanged(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// Updates the controls on settings change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveClient_SettingsChanged(object sender, EventArgs e)
        {
            // Read the settings
            if (!Settings.UI.SafeForWork)
            {
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveClient_TimerTick(object sender, EventArgs e)
        {
            // No need to do anything when the control is not visible
            if (!this.Visible)
                return;

            // Update the training info
            UpdateTrainingInfo();

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
            }
        }

        /// <summary>
        /// When the scheduler changed, we need to check the conflicts
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveClient_SchedulerChanged(object sender, EventArgs e)
        {
            UpdateTrainingInfo();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.Notifications.NotificationInvalidationEventArgs"/> instance containing the event data.</param>
        private void EveClient_NotificationInvalidated(object sender, NotificationInvalidationEventArgs e)
        {
            UpdateNotifications();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void EveClient_NotificationSent(object sender, Notification e)
        {
            UpdateNotifications();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        private void UpdateNotifications()
        {
            notificationList.Notifications = EveClient.Notifications.Where(x => x.Sender == m_character);
        }

        #endregion


        #region Control/Component Event Handlers

        /// <summary>
        /// Handles the Click event of the toolbarIcon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Controls.MultiPanelSelectionChangeEventArgs"/> instance containing the event data.</param>
        private void multiPanel_SelectionChange(object sender, MultiPanelSelectionChangeEventArgs e)
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
        /// Toggles all the skill groups to collapse or open.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void toggleSkillsIcon_Click(object sender, EventArgs e)
        {
            skillsList.ToggleAll();
        }

        /// <summary>
        /// Occurs when the user click the "add to calendar" button.
        /// We open the unique external calendar window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// <returns>Screenshot of a character.</returns>
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void groupMenu_DropDownOpening(object sender, EventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ToolStripItemClickedEventArgs"/> instance containing the event data.</param>
        private void groupMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void searchTextBox_TextChanged(object sender, EventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void columnSettingsMenuItem_Click(object sender, EventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void preferencesMenu_DropDownOpening(object sender, EventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void hideInactiveMenuItem_Click(object sender, EventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void numberAbsFormatMenuItem_Click(object sender, EventArgs e)
        {
            bool numberFormat = Settings.UI.MainWindow.MarketOrders.NumberAbsFormat;
            numberAbsFormatMenuItem.Text = (!numberFormat ? "Number Full Format" : "Number Abbreviating Format");
            Settings.UI.MainWindow.MarketOrders.NumberAbsFormat = !numberFormat;
            ordersList.UpdateColumns();
        }

        /// <summary>
        /// Displays only the entries issued for character.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void showOnlyCharMenuItem_Click(object sender, EventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void showOnlyCorpMenuItem_Click(object sender, EventArgs e)
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