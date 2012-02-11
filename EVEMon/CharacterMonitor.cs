using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls.MultiPanel;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.ExternalCalendar;
using EVEMon.Common.Notifications;
using EVEMon.Common.Scheduling;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    /// <summary>
    /// Implements the content of each of the character tabs.
    /// </summary>
    public sealed partial class CharacterMonitor : UserControl
    {
        private readonly List<ToolStripButton> m_advancedFeatures = new List<ToolStripButton>();
        private readonly ToolStripItem[] m_preferenceMenu;
        private readonly Character m_character;


        #region Constructor

        /// <summary>
        /// Design-time constructor
        /// </summary>
        private CharacterMonitor()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            Font = FontFactory.GetFont("Tahoma");
            lblScheduleWarning.Font = FontFactory.GetFont("Tahoma", FontStyle.Bold);

            // We make a copy of the preference menu for later use
            m_preferenceMenu = new ToolStripItem[preferencesMenu.DropDownItems.Count];
            preferencesMenu.DropDownItems.CopyTo(m_preferenceMenu, 0);

            multiPanel.SelectionChange += multiPanel_SelectionChange;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMonitor"/> class.
        /// </summary>
        /// <param name="character">The character.</param>
        public CharacterMonitor(Character character)
            : this()
        {
            m_character = character;
            Header.Character = character;
            skillsList.Character = character;
            skillQueueList.Character = character;
            employmentList.Character = character;
            standingsList.Character = character;
            ordersList.Character = character;
            contractsList.Character = character;
            jobsList.Character = character;
            researchList.Character = character;
            mailMessagesList.Character = character;
            eveNotificationsList.Character = character;
            notificationList.Notifications = null;

            // Create a list of the advanced features
            m_advancedFeatures.AddRange(new[]
                                            {
                                                standingsIcon, ordersIcon, contractsIcon, jobsIcon,
                                                researchIcon, mailMessagesIcon, eveNotificationsIcon
                                            });

            // Hide all advanced features related controls
            m_advancedFeatures.ForEach(x => x.Visible = false);
            featuresMenu.Visible = false;
            toggleSkillsIcon.Visible = tsToggleSeparator.Visible = false;
            toolStripContextual.Visible = false;
            warningLabel.Visible = false;

            CCPCharacter ccpCharacter = character as CCPCharacter;
            if (ccpCharacter != null)
                skillQueueControl.SkillQueue = ccpCharacter.SkillQueue;
            else
            {
                pnlTraining.Visible = false;
                skillQueuePanel.Visible = false;
                skillQueueIcon.Visible = false;
                employmentIcon.Visible = false;
            }

            // Subscribe events
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.SchedulerChanged += EveMonClient_SchedulerChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.CharacterSkillQueueUpdated += EveMonClient_CharacterSkillQueueUpdated;
            EveMonClient.MarketOrdersUpdated += EveMonClient_MarketOrdersUpdated;
            EveMonClient.IndustryJobsUpdated += EveMonClient_IndustryJobsUpdated;
            EveMonClient.CharacterResearchPointsUpdated += EveMonClient_CharacterResearchPointsUpdated;
            EveMonClient.CharacterEVEMailMessagesUpdated += EveMonClient_CharacterEVEMailMessagesUpdated;
            EveMonClient.CharacterEVENotificationsUpdated += EveMonClient_CharacterEVENotificationsUpdated;
            EveMonClient.NotificationSent += EveMonClient_NotificationSent;
            EveMonClient.NotificationInvalidated += EveMonClient_NotificationInvalidated;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.SchedulerChanged -= EveMonClient_SchedulerChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.CharacterSkillQueueUpdated -= EveMonClient_CharacterSkillQueueUpdated;
            EveMonClient.MarketOrdersUpdated -= EveMonClient_MarketOrdersUpdated;
            EveMonClient.IndustryJobsUpdated -= EveMonClient_IndustryJobsUpdated;
            EveMonClient.CharacterResearchPointsUpdated -= EveMonClient_CharacterResearchPointsUpdated;
            EveMonClient.CharacterEVEMailMessagesUpdated -= EveMonClient_CharacterEVEMailMessagesUpdated;
            EveMonClient.CharacterEVENotificationsUpdated -= EveMonClient_CharacterEVENotificationsUpdated;
            EveMonClient.NotificationSent -= EveMonClient_NotificationSent;
            EveMonClient.NotificationInvalidated -= EveMonClient_NotificationInvalidated;
            Disposed -= OnDisposed;
        }

        #endregion


        #region Inherited events

        /// <summary>
        /// On load, we subscribe the events, start the timers, etc...
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Updates the controls
            UpdateInfrequentControls();

            // Picks the last selected page
            multiPanel.SelectedPage = null;
            string tag = m_character.UISettings.SelectedPage;
            ToolStripItem item = null;

            // Only for CCP characters
            if (m_character is CCPCharacter)
            {
                item = toolStripFeatures.Items.Cast<ToolStripItem>().FirstOrDefault(x => tag == (string)x.Tag);

                // If it's not an advanced feature page make it visible
                if (item != null && !m_advancedFeatures.Contains(item))
                    item.Visible = true;

                // If it's an advanced feature page reset to skills page
                if (item != null && m_advancedFeatures.Contains(item))
                    item = skillsIcon;
            }

            toolbarIcon_Click((item ?? skillsIcon), EventArgs.Empty);
        }

        /// <summary>
        /// On visibility, we may need to refresh the display.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!Visible)
                return;

            UpdateFrequentControls();
            UpdateInfrequentControls();
        }

        #endregion


        #region Display update on character change

        /// <summary>
        /// Updates the controls whos content changes frequently.
        /// </summary>
        private void UpdateFrequentControls()
        {
            SuspendLayout();
            try
            {
                // Hides or shows the warning about a character with no API key
                warningLabel.Visible = (m_character.Identity.APIKeys.IsEmpty());

                // Update the training controls
                UpdateTrainingControls();

                // Update the advanced features enabled pages
                UpdateFeaturesMenu();
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        /// Updates the informations for skill training.
        /// </summary>
        private void UpdateTrainingControls()
        {
            // No need to do anything when the control is not visible
            if (!Visible)
                return;

            // Is the character in training ?
            if (m_character.IsTraining)
            {
                UpdateTrainingSkillInfo();

                UpdateSkillQueueInfo();

                skillQueuePanel.Visible = true;
                pnlTraining.Visible = true;
                lblPaused.Visible = false;
                return;
            }

            // Not in training, check for paused skill queue
            if (SkillQueueIsPaused())
                return;

            // Not training, no skill queue
            skillQueuePanel.Visible = false;
            pnlTraining.Visible = false;
            lblPaused.Visible = false;
        }

        /// <summary>
        /// Updates the training skill info.
        /// </summary>
        private void UpdateTrainingSkillInfo()
        {
            QueuedSkill training = m_character.CurrentlyTrainingSkill;
            DateTime completionTime = training.EndTime.ToLocalTime();

            lblTrainingSkill.Text = training.ToString();
            lblSPPerHour.Text = (training.Skill == null
                                     ? "???"
                                     : String.Format(CultureConstants.DefaultCulture, "{0} SP/Hour",
                                                     training.Skill.SkillPointsPerHour));
            lblTrainingEst.Text = String.Format(CultureConstants.DefaultCulture, "{0:ddd} {1:G}", completionTime, completionTime);

            // Dipslay a warning if anything scheduled is blocking us
            string conflictMessage;
            lblScheduleWarning.Visible = Scheduler.SkillIsBlockedAt(training.EndTime.ToLocalTime(), out conflictMessage);
            lblScheduleWarning.Text = conflictMessage;
        }

        /// <summary>
        /// Updates the skill queue info.
        /// </summary>
        private void UpdateSkillQueueInfo()
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            DateTime queueCompletionTime = ccpCharacter.SkillQueue.EndTime.ToLocalTime();
            lblQueueCompletionTime.Text = String.Format(CultureConstants.DefaultCulture,
                                                        "{0:ddd} {0:G}", queueCompletionTime);

            // Skill queue time panel
            skillQueueTimePanel.Visible = ccpCharacter.SkillQueue.Count > 1 || Settings.UI.MainWindow.AlwaysShowSkillQueueTime ||
                                          (ccpCharacter.SkillQueue.Count == 1 && Settings.UI.MainWindow.AlwaysShowSkillQueueTime);

            // Update the remaining training time label
            QueuedSkill training = m_character.CurrentlyTrainingSkill;
            lblTrainingRemain.Text = training.EndTime.ToRemainingTimeDescription(DateTimeKind.Utc);

            // Update the remaining queue time label
            DateTime queueEndTime = ccpCharacter.SkillQueue.EndTime;
            lblQueueRemaining.Text = queueEndTime.ToRemainingTimeDescription(DateTimeKind.Utc);
        }

        /// <summary>
        /// Updates the skill queue info if queue is paused.
        /// </summary>
        /// <returns></returns>
        private bool SkillQueueIsPaused()
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null || !ccpCharacter.SkillQueue.IsPaused)
                return false;

            QueuedSkill training = ccpCharacter.SkillQueue.CurrentlyTraining;
            lblTrainingSkill.Text = training.ToString();
            lblSPPerHour.Text = (training.Skill == null
                                     ? "???"
                                     : String.Format(CultureConstants.DefaultCulture, "{0} SP/Hour",
                                                     training.Skill.SkillPointsPerHour));

            lblTrainingRemain.Text = "Paused";
            lblTrainingEst.Text = String.Empty;
            lblScheduleWarning.Visible = false;
            skillQueueTimePanel.Visible = false;
            skillQueuePanel.Visible = true;
            pnlTraining.Visible = true;
            lblPaused.Visible = true;

            return true;
        }

        /// <summary>
        /// Updates the controls whos content changes infrequently.
        /// </summary>
        private void UpdateInfrequentControls()
        {
            // No need to do anything when the control is not visible
            if (!Visible)
                return;

            SuspendLayout();
            try
            {
                // "Update Calendar" button
                btnAddToCalendar.Visible = Settings.Calendar.Enabled;

                // Read the settings
                if (Settings.UI.SafeForWork)
                {
                    // Takes care of the features icons
                    foreach (ToolStripItem item in toolStripFeatures.Items.Cast<ToolStripItem>().Where(
                        item => item is ToolStripButton || item is ToolStripDropDownButton))
                    {
                        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    }

                    // Takes care of the special second toolstrip icons 
                    foreach (ToolStripItem item in toolStripContextual.Items)
                    {
                        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    }

                    toolStripFeatures.ContextMenuStrip = null;
                    return;
                }

                // Display image and text of the features according to user preference
                foreach (ToolStripButton item in toolStripFeatures.Items.OfType<ToolStripButton>())
                {
                    item.DisplayStyle = Settings.UI.ShowTextInToolStrip
                                            ? ToolStripItemDisplayStyle.ImageAndText
                                            : ToolStripItemDisplayStyle.Image;
                }

                featuresMenu.DisplayStyle = ToolStripItemDisplayStyle.Image;
                preferencesMenu.DisplayStyle = ToolStripItemDisplayStyle.Image;

                groupMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;

                toolStripFeatures.ContextMenuStrip = toolstripContextMenu;
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        /// Hides or shows the feature menu.
        /// </summary>
        private void UpdateFeaturesMenu()
        {
            if (EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed) || !m_character.Identity.APIKeys.Any())
                return;

            CCPCharacter ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            featuresMenu.Visible = ccpCharacter.QueryMonitors.Any();
            tsToggleSeparator.Visible = featuresMenu.Visible && toggleSkillsIcon.Visible;
            m_advancedFeatures.ForEach(SetVisibility);
            ToggleAdvancedFeaturesMonitoring();
        }

        /// <summary>
        /// Sets the button visibility.
        /// </summary>
        /// <param name="button">The button.</param>
        private void SetVisibility(ToolStripButton button)
        {
            IEnumerable<IQueryMonitor> monitors = ButtonToMonitors(button);
            bool visible = IsEnabledFeature(button.Text) && monitors.Any() && monitors.Any(monitor => monitor.HasAccess);
            button.Visible = visible;

            // Quit if the button should stay visible
            // (Do not use the buttons' 'Visible' property as condition,
            // because there is a .NET bug when returning from minimized state) 
            if (visible)
                return;

            // Buttons' related monitor lost access to data while it was enabled, so...
            // 1. Remove buttons' related page from settings
            if (m_character.UISettings.AdvancedFeaturesEnabledPages.Contains(button.Text))
                m_character.UISettings.AdvancedFeaturesEnabledPages.Remove(button.Text);

            // 2. Uncheck in dropdown menu
            if (featuresMenu.DropDownItems.Count == 0)
                return;

            foreach (ToolStripMenuItem item in featuresMenu.DropDownItems.Cast<ToolStripMenuItem>().Where(
                item => item.Text == button.Text && item.Checked))
            {
                item.Checked = !item.Checked;

                // Update the selected page
                UpdateSelectedPage();
            }
        }

        /// <summary>
        /// Hides or shows the pages controls.
        /// </summary>
        private void UpdatePageControls()
        {
            // Enables / Disables the skill page controls
            toggleSkillsIcon.Enabled = m_character.Skills.Any();

            // Exit if it's a non-CCPCharacter
            CCPCharacter ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            // Ensures the visibility of the toolstrip items
            foreach (ToolStripItem item in toolStripContextual.Items)
            {
                item.Visible = true;
            }

            // Enables / Disables the market orders page related controls
            if (multiPanel.SelectedPage == ordersPage)
                toolStripContextual.Enabled = !ccpCharacter.MarketOrders.IsEmpty();

            // Enables / Disables the contracts page related controls
            if (multiPanel.SelectedPage == contractsPage)
                toolStripContextual.Enabled = !ccpCharacter.Contracts.IsEmpty();

            // Enables / Disables the industry jobs page related controls
            if (multiPanel.SelectedPage == jobsPage)
                toolStripContextual.Enabled = !ccpCharacter.IndustryJobs.IsEmpty();

            // Enables / Disables the research points page related controls
            if (multiPanel.SelectedPage == researchPage)
            {
                toolStripContextual.Enabled = !ccpCharacter.ResearchPoints.IsEmpty();
                groupMenu.Visible = false;
            }

            // Enables / Disables the EVE mail messages page related controls
            if (multiPanel.SelectedPage == mailMessagesPage)
                toolStripContextual.Enabled = !ccpCharacter.EVEMailMessages.IsEmpty();

            // Enables / Disables the EVE notifications page related controls
            if (multiPanel.SelectedPage == eveNotificationsPage)
                toolStripContextual.Enabled = !ccpCharacter.EVENotifications.IsEmpty();
        }

        /// <summary>
        /// Toggles the advanced features monitoring.
        /// </summary>
        private void ToggleAdvancedFeaturesMonitoring()
        {
            // Quit if it's a non-CCPCharacter
            CCPCharacter ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            foreach (ToolStripButton button in m_advancedFeatures)
            {
                List<IQueryMonitor> monitors = ButtonToMonitors(button).ToList();

                if (monitors.IsEmpty())
                    continue;

                foreach (IQueryMonitor monitor in monitors)
                {
                    monitor.Enabled = IsEnabledFeature(button.Text);
                    if (monitor.QueryOnStartup && monitor.Enabled && monitor.LastResult == null)
                        ccpCharacter.QueryMonitors.Query(monitor.Method);
                }
            }
        }

        /// <summary>
        /// Updates advanced features pages selection and settings.
        /// </summary>
        private void UpdateAdvancedFeaturesPagesSettings()
        {
            UpdateSelectedPage();

            List<string> enabledAdvancedFeaturesPages =
                (featuresMenu.DropDownItems.Cast<ToolStripMenuItem>().Where(
                    menuItem => menuItem.Checked).Select(menuItem => menuItem.Text)).ToList();

            m_character.UISettings.AdvancedFeaturesEnabledPages.Clear();
            enabledAdvancedFeaturesPages.ForEach(page => m_character.UISettings.AdvancedFeaturesEnabledPages.Add(page));
        }

        /// <summary>
        /// Updates the selected page.
        /// </summary>
        private void UpdateSelectedPage()
        {
            if (m_advancedFeatures.Any(featureIcon =>
                                       multiPanel.SelectedPage.Text == (string)featureIcon.Tag && !featureIcon.Visible))
            {
                toolbarIcon_Click(skillsIcon, EventArgs.Empty);
            }
        }

        #endregion


        #region Updates on global events

        /// <summary>
        /// Occur on every second. We update the total SP, remaining time and the matching item in skill list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateFrequentControls();
        }

        /// <summary>
        /// Updates the controls on settings change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateInfrequentControls();
        }

        /// <summary>
        /// When the scheduler changed, we need to check the conflicts.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SchedulerChanged(object sender, EventArgs e)
        {
            UpdateTrainingControls();
        }

        /// <summary>
        /// Occur when the character updates. We update all the controls' content.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdateInfrequentControls();
        }

        /// <summary>
        /// Occur when the character skill queue updates.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterSkillQueueUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            skillQueueControl.Invalidate();
        }

        /// <summary>
        /// Updates the page controls on market orders change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_MarketOrdersUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdatePageControls();
        }

        /// <summary>
        /// Updates the page controls on industry jobs change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_IndustryJobsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdatePageControls();
        }

        /// <summary>
        /// Updates the page controls on research points change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterResearchPointsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdatePageControls();
        }

        /// <summary>
        /// Updates the page controls on EVE mail messages change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterEVEMailMessagesUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdatePageControls();
        }

        /// <summary>
        /// Updates the page controls on EVE notifications change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterEVENotificationsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdatePageControls();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.Notifications.NotificationInvalidationEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_NotificationInvalidated(object sender, NotificationInvalidationEventArgs e)
        {
            UpdateNotifications();
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void EveMonClient_NotificationSent(object sender, NotificationEventArgs e)
        {
            UpdateNotifications();
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
            foreach (ToolStripItem item in toolStripFeatures.Items)
            {
                // Skip tags without tag, those ones do not represent "pages switches"
                if (item.Tag == null)
                    continue;

                // Is it the item we clicked ?
                ToolStripButton button = item as ToolStripButton;
                if (button != null && item == sender)
                {
                    // Selects the proper page
                    multiPanel.SelectedPage =
                        multiPanel.Controls.Cast<MultiPanelPage>().First(x => x.Name == (string)item.Tag);

                    // Checks it
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
        /// When the selected page changes, we may have to update the related controls visibility.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MultiPanelSelectionChangeEventArgs"/> instance containing the event data.</param>
        private void multiPanel_SelectionChange(object sender, MultiPanelSelectionChangeEventArgs e)
        {
            if (e.NewPage == null)
                return;

            // Stores the setting
            m_character.UISettings.SelectedPage = e.NewPage.Text;

            // Update the buttons visibility
            toggleSkillsIcon.Visible = (e.NewPage == skillsPage);
            tsToggleSeparator.Visible = featuresMenu.Visible && toggleSkillsIcon.Visible;
            toolStripContextual.Visible = m_advancedFeatures.Any(button => (string)button.Tag != standingsPage.Text &&
                                                                           (string)button.Tag == e.NewPage.Text);

            // Update the page controls
            UpdatePageControls();
        }

        /// <summary>
        /// Toggles all the skill groups to collapse or open.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toggleSkillsIcon_Click(object sender, EventArgs e)
        {
            skillsList.ToggleAll();
        }

        /// <summary>
        /// Occurs when the user click the "Update Calendar" button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnUpdateCalendar_Click(object sender, EventArgs e)
        {
            // Ensure that we are trying to use the external calendar
            if (!Settings.Calendar.Enabled)
            {
                btnAddToCalendar.Visible = false;
                return;
            }

            if (m_character is CCPCharacter)
                ExternalCalendar.UpdateCalendar(m_character as CCPCharacter);
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


        #region Helper Methods

        /// <summary>
        /// Gets true if the page in question is enabled.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsEnabledFeature(string text)
        {
            return m_character.UISettings.AdvancedFeaturesEnabledPages.Any(x => x == text);
        }

        /// <summary>
        /// Gets the monitors related to the toolstrip button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        private IEnumerable<IQueryMonitor> ButtonToMonitors(ToolStripItem button)
        {
            MultiPanelPage page = multiPanel.Controls.Cast<MultiPanelPage>().First(x => x.Name == (string)button.Tag);
            CCPCharacter ccpCharacter = (CCPCharacter)m_character;

            List<IQueryMonitor> monitors = new List<IQueryMonitor>();
            if (Enum.IsDefined(typeof(APICharacterMethods), page.Tag))
            {
                APICharacterMethods method = (APICharacterMethods)Enum.Parse(typeof(APICharacterMethods), (string)page.Tag);
                if (ccpCharacter.QueryMonitors[method.ToString()] != null)
                    monitors.Add(ccpCharacter.QueryMonitors[method.ToString()]);
            }

            if (Enum.IsDefined(typeof(APICorporationMethods),
                               String.Format(CultureConstants.InvariantCulture, "Corporation{0}", page.Tag)))
            {
                APICorporationMethods method =
                    (APICorporationMethods)Enum.Parse(typeof(APICorporationMethods),
                                                      String.Format(CultureConstants.InvariantCulture, "Corporation{0}", page.Tag));
                if (ccpCharacter.QueryMonitors[method.ToString()] != null)
                    monitors.Add(ccpCharacter.QueryMonitors[method.ToString()]);
            }

            return monitors;
        }

        /// <summary>
        /// Update the notifications list.
        /// </summary>
        private void UpdateNotifications()
        {
            notificationList.Notifications = EveMonClient.Notifications.Where(x => x.Sender == m_character);
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

            skillsList.Dock = DockStyle.None;
            skillsList.Height = preferredHeight;
            skillsList.Update();

            Bitmap bitmap;
            using (Bitmap tempBitmap = new Bitmap(skillsList.Width, preferredHeight))
            {
                skillsList.DrawToBitmap(tempBitmap, new Rectangle(0, 0, skillsList.Width, preferredHeight));
                bitmap = (Bitmap)tempBitmap.Clone();
            }

            skillsList.Dock = DockStyle.Fill;
            skillsList.Height = cachedHeight;
            skillsList.Update();

            Invalidate();
            return bitmap;
        }

        # endregion


        # region Multi Panel Control/Component Event Handlers

        /// <summary>
        /// On menu opening we create the menu items and update their checked state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void featureMenu_DropDownOpening(object sender, EventArgs e)
        {
            featuresMenu.DropDownItems.Clear();

            // Create the menu items
            foreach (ToolStripMenuItem item in m_advancedFeatures.Select(
                button => new { button, monitor = ButtonToMonitors(button) }).Where(
                    item => item.monitor != null).Select(
                        item =>
                            {
                                ToolStripMenuItem tsi;
                                ToolStripMenuItem tempToolStripItem = null;
                                try
                                {
                                    tempToolStripItem = new ToolStripMenuItem(item.button.Text);
                                    tempToolStripItem.Checked = IsEnabledFeature(item.button.Text);
                                    tempToolStripItem.Enabled = item.monitor.Any(monitor => monitor.HasAccess);

                                    tsi = tempToolStripItem;
                                    tempToolStripItem = null;
                                }
                                finally
                                {
                                    if (tempToolStripItem != null)
                                        tempToolStripItem.Dispose();
                                }
                                return tsi;
                            }))
            {
                featuresMenu.DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// Occurs when the user click an item in the features menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void featuresMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)e.ClickedItem;
            item.Checked = !item.Checked;

            m_advancedFeatures.ForEach(featureIcon => featureIcon.Visible = (item.Text == featureIcon.Text
                                                                                 ? item.Checked
                                                                                 : featureIcon.Visible));

            UpdateAdvancedFeaturesPagesSettings();
            ToggleAdvancedFeaturesMonitoring();
        }

        /// <summary>
        /// On opening we create the menu items for "Group By..." in panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void groupMenu_DropDownOpening(object sender, EventArgs e)
        {
            groupMenu.DropDownItems.Clear();

            if (multiPanel.SelectedPage == ordersPage)
                CreateGroupMenuList<MarketOrderGrouping, Enum>(ordersList);

            if (multiPanel.SelectedPage == contractsPage)
                CreateGroupMenuList<ContractGrouping, Enum>(contractsList);

            if (multiPanel.SelectedPage == jobsPage)
                CreateGroupMenuList<IndustryJobGrouping, Enum>(jobsList);

            if (multiPanel.SelectedPage == mailMessagesPage)
                CreateGroupMenuList<EVEMailMessagesGrouping, Enum>(mailMessagesList);

            if (multiPanel.SelectedPage == eveNotificationsPage)
                CreateGroupMenuList<EVENotificationsGrouping, Enum>(eveNotificationsList);
        }

        /// <summary>
        /// Occurs when the user click an item in the "Group By..." menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ToolStripItemClickedEventArgs"/> instance containing the event data.</param>
        private void groupMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            if (multiPanel.SelectedPage == ordersPage)
                GroupMenuSetting<MarketOrderGrouping, Enum>(item, ordersList);

            if (multiPanel.SelectedPage == contractsPage)
                GroupMenuSetting<ContractGrouping, Enum>(item, contractsList);

            if (multiPanel.SelectedPage == jobsPage)
                GroupMenuSetting<IndustryJobGrouping, Enum>(item, jobsList);

            if (multiPanel.SelectedPage == mailMessagesPage)
                GroupMenuSetting<EVEMailMessagesGrouping, Enum>(item, mailMessagesList);

            if (multiPanel.SelectedPage == eveNotificationsPage)
                GroupMenuSetting<EVENotificationsGrouping, Enum>(item, eveNotificationsList);
        }

        /// <summary>
        /// Occurs when the search text changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (multiPanel.SelectedPage == ordersPage)
                ordersList.TextFilter = searchTextBox.Text;

            if (multiPanel.SelectedPage == contractsPage)
                contractsList.TextFilter = searchTextBox.Text;

            if (multiPanel.SelectedPage == jobsPage)
                jobsList.TextFilter = searchTextBox.Text;

            if (multiPanel.SelectedPage == researchPage)
                researchList.TextFilter = searchTextBox.Text;

            if (multiPanel.SelectedPage == mailMessagesPage)
                mailMessagesList.TextFilter = searchTextBox.Text;

            if (multiPanel.SelectedPage == eveNotificationsPage)
                eveNotificationsList.TextFilter = searchTextBox.Text;
        }

        /// <summary>
        /// On menu opening we update the menu items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void preferencesMenu_DropDownOpening(object sender, EventArgs e)
        {
            bool hideInactive = true;

            if (multiPanel.SelectedPage == ordersPage)
            {
                bool numberFormat = Settings.UI.MainWindow.MarketOrders.NumberAbsFormat;
                hideInactive = Settings.UI.MainWindow.MarketOrders.HideInactiveOrders;

                preferencesMenu.DropDownItems.Clear();
                foreach (ToolStripItem item in m_preferenceMenu.Where(
                    item => !item.Equals(tsReadingPaneSeparator) && !item.Equals(readingPaneMenuItem)))
                {
                    preferencesMenu.DropDownItems.Add(item);
                }

                numberAbsFormatMenuItem.Text = (numberFormat ? "Full Number Format" : "Abbreviating Number Format");
                showOnlyCharMenuItem.Checked = ordersList.ShowIssuedFor == IssuedFor.Character;
                showOnlyCorpMenuItem.Checked = ordersList.ShowIssuedFor == IssuedFor.Corporation;
            }

            if (multiPanel.SelectedPage == contractsPage)
            {
                bool numberFormat = Settings.UI.MainWindow.Contracts.NumberAbsFormat;
                hideInactive = Settings.UI.MainWindow.Contracts.HideInactiveContracts;

                preferencesMenu.DropDownItems.Clear();
                foreach (ToolStripItem item in m_preferenceMenu.Where(
                    item => !item.Equals(tsReadingPaneSeparator) && !item.Equals(readingPaneMenuItem)))
                {
                    preferencesMenu.DropDownItems.Add(item);
                }

                numberAbsFormatMenuItem.Text = (numberFormat ? "Full Number Format" : "Abbreviating Number Format");
                showOnlyCharMenuItem.Checked = contractsList.ShowIssuedFor == IssuedFor.Character;
                showOnlyCorpMenuItem.Checked = contractsList.ShowIssuedFor == IssuedFor.Corporation;
            }

            if (multiPanel.SelectedPage == jobsPage)
            {
                hideInactive = Settings.UI.MainWindow.IndustryJobs.HideInactiveJobs;

                preferencesMenu.DropDownItems.Clear();
                foreach (ToolStripItem item in m_preferenceMenu.Where(
                    item => !item.Equals(numberAbsFormatMenuItem) && !item.Equals(tsReadingPaneSeparator) &&
                            !item.Equals(readingPaneMenuItem)))
                {
                    preferencesMenu.DropDownItems.Add(item);
                }

                showOnlyCharMenuItem.Checked = jobsList.ShowIssuedFor == IssuedFor.Character;
                showOnlyCorpMenuItem.Checked = jobsList.ShowIssuedFor == IssuedFor.Corporation;
            }

            if (multiPanel.SelectedPage == researchPage)
            {
                preferencesMenu.DropDownItems.Clear();
                preferencesMenu.DropDownItems.Add(columnSettingsMenuItem);
                preferencesMenu.DropDownItems.Add(autoSizeColumnMenuItem);
                return;
            }

            if (multiPanel.SelectedPage == mailMessagesPage || multiPanel.SelectedPage == eveNotificationsPage)
            {
                preferencesMenu.DropDownItems.Clear();
                preferencesMenu.DropDownItems.Add(columnSettingsMenuItem);
                preferencesMenu.DropDownItems.Add(autoSizeColumnMenuItem);
                preferencesMenu.DropDownItems.Add(tsReadingPaneSeparator);
                preferencesMenu.DropDownItems.Add(readingPaneMenuItem);
                return;
            }

            hideInactiveMenuItem.Text = (hideInactive ? "Unhide Inactive" : "Hide Inactive");
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
                using (MarketOrdersColumnsSelectWindow f =
                    new MarketOrdersColumnsSelectWindow(ordersList.Columns.Cast<MarketOrderColumnSettings>()))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        ordersList.Columns = f.Columns;
                        Settings.UI.MainWindow.MarketOrders.Columns.Clear();
                        Settings.UI.MainWindow.MarketOrders.Columns.AddRange(ordersList.Columns.Cast<MarketOrderColumnSettings>());
                    }
                }
            }

            if (multiPanel.SelectedPage == contractsPage)
            {
                using (ContractsColumnsSelectWindow f =
                    new ContractsColumnsSelectWindow(contractsList.Columns.Cast<ContractColumnSettings>()))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        contractsList.Columns = f.Columns;
                        Settings.UI.MainWindow.Contracts.Columns.Clear();
                        Settings.UI.MainWindow.Contracts.Columns.AddRange(contractsList.Columns.Cast<ContractColumnSettings>());
                    }
                }
            }

            if (multiPanel.SelectedPage == jobsPage)
            {
                using (IndustryJobsColumnsSelectWindow f =
                    new IndustryJobsColumnsSelectWindow(jobsList.Columns.Cast<IndustryJobColumnSettings>()))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        jobsList.Columns = f.Columns;
                        Settings.UI.MainWindow.IndustryJobs.Columns.Clear();
                        Settings.UI.MainWindow.IndustryJobs.Columns.AddRange(jobsList.Columns.Cast<IndustryJobColumnSettings>());
                    }
                }
            }

            if (multiPanel.SelectedPage == researchPage)
            {
                using (ResearchColumnsSelectWindow f =
                    new ResearchColumnsSelectWindow(researchList.Columns.Cast<ResearchColumnSettings>()))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        researchList.Columns = f.Columns;
                        Settings.UI.MainWindow.Research.Columns.Clear();
                        Settings.UI.MainWindow.Research.Columns.AddRange(researchList.Columns.Cast<ResearchColumnSettings>());
                    }
                }
            }

            if (multiPanel.SelectedPage == mailMessagesPage)
            {
                using (EveMailMessagesColumnsSelectWindow f =
                    new EveMailMessagesColumnsSelectWindow(mailMessagesList.Columns.Cast<EveMailMessageColumnSettings>()))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        mailMessagesList.Columns = f.Columns;
                        Settings.UI.MainWindow.EVEMailMessages.Columns.Clear();
                        Settings.UI.MainWindow.EVEMailMessages.Columns.AddRange(
                            mailMessagesList.Columns.Cast<EveMailMessageColumnSettings>());
                    }
                }
            }

            if (multiPanel.SelectedPage == eveNotificationsPage)
            {
                using (EveNotificationsColumnsSelectWindow f =
                    new EveNotificationsColumnsSelectWindow(eveNotificationsList.Columns.Cast<EveNotificationColumnSettings>()))
                {
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        eveNotificationsList.Columns = f.Columns;
                        Settings.UI.MainWindow.EVENotifications.Columns.Clear();
                        Settings.UI.MainWindow.EVENotifications.Columns.AddRange(
                            eveNotificationsList.Columns.Cast<EveNotificationColumnSettings>());
                    }
                }
            }
        }

        /// <summary>
        /// Auto-Sizes the columns width of the page list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void autoSizeColumnMenuItem_Click(object sender, EventArgs e)
        {
            IListView list = multiPanel.SelectedPage.Controls.OfType<IListView>().FirstOrDefault();
            if (list == null)
                return;
            list.Columns.Where(column => column.Visible).ToList().ForEach(column => column.Width = -2);
            list.UpdateColumns();
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

            if (multiPanel.SelectedPage == contractsPage)
            {
                hideInactive = Settings.UI.MainWindow.Contracts.HideInactiveContracts;
                Settings.UI.MainWindow.Contracts.HideInactiveContracts = !hideInactive;
                contractsList.UpdateColumns();
            }

            if (multiPanel.SelectedPage == jobsPage)
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
            if (multiPanel.SelectedPage == ordersPage)
            {
                bool numberFormat = Settings.UI.MainWindow.MarketOrders.NumberAbsFormat;
                numberAbsFormatMenuItem.Text = (!numberFormat ? "Number Full Format" : "Number Abbreviating Format");
                Settings.UI.MainWindow.MarketOrders.NumberAbsFormat = !numberFormat;
                ordersList.UpdateColumns();
            }

            if (multiPanel.SelectedPage == contractsPage)
            {
                bool numberFormat = Settings.UI.MainWindow.Contracts.NumberAbsFormat;
                numberAbsFormatMenuItem.Text = (!numberFormat ? "Number Full Format" : "Number Abbreviating Format");
                Settings.UI.MainWindow.Contracts.NumberAbsFormat = !numberFormat;
                contractsList.UpdateColumns();
            }
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
                showOnlyCorpMenuItem.Checked = (ordersList.ShowIssuedFor == IssuedFor.Corporation);
            }

            if (multiPanel.SelectedPage == contractsPage)
            {
                contractsList.ShowIssuedFor = (showOnlyCharMenuItem.Checked ? IssuedFor.Character : IssuedFor.All);
                showOnlyCorpMenuItem.Checked = (contractsList.ShowIssuedFor == IssuedFor.Corporation);
            }

            if (multiPanel.SelectedPage == jobsPage)
            {
                jobsList.ShowIssuedFor = (showOnlyCharMenuItem.Checked ? IssuedFor.Character : IssuedFor.All);
                showOnlyCorpMenuItem.Checked = (jobsList.ShowIssuedFor == IssuedFor.Corporation);
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
                showOnlyCharMenuItem.Checked = (ordersList.ShowIssuedFor == IssuedFor.Character);
            }

            if (multiPanel.SelectedPage == contractsPage)
            {
                contractsList.ShowIssuedFor = (showOnlyCorpMenuItem.Checked ? IssuedFor.Corporation : IssuedFor.All);
                showOnlyCharMenuItem.Checked = (contractsList.ShowIssuedFor == IssuedFor.Character);
            }

            if (multiPanel.SelectedPage != jobsPage)
            {
                jobsList.ShowIssuedFor = (showOnlyCorpMenuItem.Checked ? IssuedFor.Corporation : IssuedFor.All);
                showOnlyCharMenuItem.Checked = (jobsList.ShowIssuedFor == IssuedFor.Character);
            }
        }

        /// <summary>
        /// Handles the DropDownOpening event of the readingPaneMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void readingPaneMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string paneSetting = ReadingPanePositioning.Off.ToString();

            if (multiPanel.SelectedPage == mailMessagesPage)
                paneSetting = Settings.UI.MainWindow.EVEMailMessages.ReadingPanePosition.ToString();

            if (multiPanel.SelectedPage == eveNotificationsPage)
                paneSetting = Settings.UI.MainWindow.EVENotifications.ReadingPanePosition.ToString();

            foreach (ToolStripMenuItem menuItem in readingPaneMenuItem.DropDownItems)
            {
                menuItem.Checked = ((string)menuItem.Tag == paneSetting);
            }
        }

        /// <summary>
        /// Handles the Click event of the paneRightMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void paneRightMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menuItem in readingPaneMenuItem.DropDownItems)
            {
                menuItem.Checked = false;
            }

            paneRightMenuItem.Checked = true;

            if (multiPanel.SelectedPage == mailMessagesPage)
            {
                mailMessagesList.PanePosition = ReadingPanePositioning.Right;
                Settings.UI.MainWindow.EVEMailMessages.ReadingPanePosition = mailMessagesList.PanePosition;
            }

            if (multiPanel.SelectedPage == eveNotificationsPage)
            {
                eveNotificationsList.PanePosition = ReadingPanePositioning.Right;
                Settings.UI.MainWindow.EVENotifications.ReadingPanePosition = eveNotificationsList.PanePosition;
            }
        }

        /// <summary>
        /// Handles the Click event of the paneBottomMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void paneBottomMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menuItem in readingPaneMenuItem.DropDownItems)
            {
                menuItem.Checked = false;
            }

            paneBottomMenuItem.Checked = true;

            if (multiPanel.SelectedPage == mailMessagesPage)
            {
                mailMessagesList.PanePosition = ReadingPanePositioning.Bottom;
                Settings.UI.MainWindow.EVEMailMessages.ReadingPanePosition = mailMessagesList.PanePosition;
            }

            if (multiPanel.SelectedPage == eveNotificationsPage)
            {
                eveNotificationsList.PanePosition = ReadingPanePositioning.Bottom;
                Settings.UI.MainWindow.EVENotifications.ReadingPanePosition = eveNotificationsList.PanePosition;
            }
        }

        /// <summary>
        /// Handles the Click event of the paneOffMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void paneOffMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menuItem in readingPaneMenuItem.DropDownItems)
            {
                menuItem.Checked = false;
            }

            paneOffMenuItem.Checked = true;

            if (multiPanel.SelectedPage == mailMessagesPage)
            {
                mailMessagesList.PanePosition = ReadingPanePositioning.Off;
                Settings.UI.MainWindow.EVEMailMessages.ReadingPanePosition = mailMessagesList.PanePosition;
            }

            if (multiPanel.SelectedPage == eveNotificationsPage)
            {
                eveNotificationsList.PanePosition = ReadingPanePositioning.Off;
                Settings.UI.MainWindow.EVENotifications.ReadingPanePosition = eveNotificationsList.PanePosition;
            }
        }

        /// <summary>
        /// On menu opening we update the menu items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolstripContextMenu_Opening(object sender, CancelEventArgs e)
        {
            showTextMenuItem.Checked = Settings.UI.ShowTextInToolStrip;
        }

        /// <summary>
        /// Shows or hides the text of the toolstrip items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showTextMenuItem_Click(object sender, EventArgs e)
        {
            Settings.UI.ShowTextInToolStrip = showTextMenuItem.Checked = !showTextMenuItem.Checked;
            EveMonClient_SettingsChanged(null, EventArgs.Empty);
        }

        # endregion


        #region Generic Helper Methods

        /// <summary>
        /// Creates the group menu list.
        /// </summary>
        /// <typeparam name="T">The grouping type.</typeparam>
        /// <typeparam name="T1">The grouping base type.</typeparam>
        /// <param name="list">The list.</param>
        private void CreateGroupMenuList<T, T1>(IListView list)
            where T : T1
        {
            foreach (ToolStripButton menu in EnumExtensions.GetValues<T>().Select(
                grouping => new { grouping, group = grouping as Enum }).Where(
                    menu => menu.group != null).Select(
                        menu =>
                            {
                                ToolStripButton tsb;
                                ToolStripButton tempToolStripButton = null;
                                try
                                {
                                    tempToolStripButton = new ToolStripButton(menu.group.GetHeader());
                                    tempToolStripButton.Checked = (list.Grouping.CompareTo(menu.group) == 0);
                                    tempToolStripButton.Tag = menu.grouping;

                                    tsb = tempToolStripButton;
                                    tempToolStripButton = null;
                                }
                                finally
                                {
                                    if (tempToolStripButton != null)
                                        tempToolStripButton.Dispose();
                                }
                                return tsb;
                            }))
            {
                groupMenu.DropDownItems.Add(menu);
            }
        }

        /// <summary>
        /// Sets and stores in settings the GroupBy selection.
        /// </summary>
        /// <typeparam name="T">The grouping type.</typeparam>
        /// <typeparam name="T1">The grouping base type.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        private void GroupMenuSetting<T, T1>(ToolStripItem item, IListView list)
            where T : T1
        {
            Enum grouping = item.Tag as Enum;
            if (grouping == null)
                return;

            list.Grouping = grouping;
            T obj = default(T);

            if (obj is MarketOrderGrouping)
                m_character.UISettings.OrdersGroupBy = (MarketOrderGrouping)grouping;

            if (obj is IndustryJobGrouping)
                m_character.UISettings.JobsGroupBy = (IndustryJobGrouping)grouping;

            if (obj is EVEMailMessagesGrouping)
                m_character.UISettings.EVEMailMessagesGroupBy = (EVEMailMessagesGrouping)grouping;

            if (obj is EVENotificationsGrouping)
                m_character.UISettings.EVENotificationsGroupBy = (EVENotificationsGrouping)grouping;
        }

        #endregion


        #region Testing Function

        /// <summary>
        /// Tests character's notification display in the Character Monitor.
        /// </summary>
        internal void TestCharacterNotification()
        {
            NotificationEventArgs notification = new NotificationEventArgs(m_character, NotificationCategory.TestNofitication)
                                                     {
                                                         Priority = NotificationPriority.Warning,
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Description = "Test Character Notification."
                                                     };
            EveMonClient.Notifications.Notify(notification);
        }

        #endregion
    }
}