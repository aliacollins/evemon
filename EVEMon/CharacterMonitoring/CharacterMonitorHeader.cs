using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.ApiCredentialsManagement;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Net;

namespace EVEMon.CharacterMonitoring
{
    /// <summary>
    /// Implements the header component of the main character monitor user interface.
    /// </summary>
    public sealed partial class CharacterMonitorHeader : UserControl
    {
        #region Fields

        private Character m_character;
        private int m_spAtLastRedraw;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMonitorHeader"/> class.
        /// </summary>
        public CharacterMonitorHeader()
        {
            InitializeComponent();
        }

        #endregion

        
        #region Inherited Events

        /// <summary>
        /// Occurs when control loads.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Fonts
            Font = FontFactory.GetFont("Tahoma");
            CharacterNameLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            // Subscribe to events
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.CharacterInfoUpdated += EveMonClient_CharacterInfoUpdated;
            EveMonClient.MarketOrdersUpdated += EveMonClient_MarketOrdersUpdated;
            Disposed += OnDisposed;

            base.OnLoad(e);
        }

        /// <summary>
        /// Occurs when visibility changes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!Visible)
                return;

            UpdateFrequentControls();
            UpdateInfrequentControls();

            base.OnVisibleChanged(e);
        }

        /// <summary>
        /// Called when the control is disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.CharacterInfoUpdated -= EveMonClient_CharacterInfoUpdated;
            EveMonClient.MarketOrdersUpdated -= EveMonClient_MarketOrdersUpdated;
            Disposed -= OnDisposed;
        }

        #endregion


        #region Updating Methods

        /// <summary>
        /// Updates the controls whos content changes frequently.
        /// </summary>
        private void UpdateFrequentControls()
        {
            if (m_character == null)
                return;

            SuspendLayout();
            try
            {
                RefreshThrobber();

                // Only update the skill summary when the skill points change
                if (m_spAtLastRedraw != m_character.SkillPoints)
                    SkillSummaryLabel.Text = FormatSkillSummary();

                m_spAtLastRedraw = m_character.SkillPoints;
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        /// Updates the controls whos content changes infrequently.
        /// </summary>
        private void UpdateInfrequentControls()
        {
            if (m_character == null)
                return;

            SuspendLayout();
            try
            {
                // Safe for work implementation
                MainTableLayoutPanel.ColumnStyles[0].SizeType = Settings.UI.SafeForWork ? SizeType.Absolute : SizeType.AutoSize;
                MainTableLayoutPanel.ColumnStyles[0].Width = 0;
                CharacterPortrait.Visible = !Settings.UI.SafeForWork;

                CharacterPortrait.Character = m_character;
                CharacterNameLabel.Text = m_character.AdornedName;
                BioInfoLabel.Text = String.Format(CultureConstants.DefaultCulture, "{0} - {1} - {2} - {3}",
                                                  m_character.Gender ?? "Gender",
                                                  m_character.Race ?? "Race",
                                                  m_character.Bloodline ?? "Bloodline",
                                                  m_character.Ancestry ?? "Ancestry");
                BirthdayLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                                   "Birthday: {0}", m_character.Birthday.ToLocalTime());
                CorporationNameLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                                          "Corporation: {0}", m_character.CorporationName ?? "Unknown");

                AllianceInfoIndicationPictureBox.Visible = m_character.AllianceID != 0;

                FormatBalance();

                FormatAttributes();

                UpdateInfoControls();
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        /// Updates the info controls.
        /// </summary>
        private void UpdateInfoControls()
        {
            if (m_character == null)
                return;

            SuspendLayout();
            try
            {
                SecurityStatusLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                                         "Security Status: {0:N2}", m_character.SecurityStatus);
                ActiveShipLabel.Text = GetActiveShipText();

                APIKey apiKey = m_character.Identity.FindAPIKeyWithAccess(APICharacterMethods.CharacterInfo);
                LocationInfoIndicationPictureBox.Visible =
                    apiKey != null && !String.IsNullOrWhiteSpace(m_character.LastKnownLocation);
            }
            finally
            {
                ResumeLayout();
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Sets the character.
        /// </summary>
        /// <value>The character.</value>
        public void SetCharacter(Character character)
        {
            if (m_character == character)
                return;

            m_character = character;
        }

        /// <summary>
        /// Formats the balance.
        /// </summary>
        private void FormatBalance()
        {
            if (m_character == null)
                return;

            BalanceLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                              "Balance: {0:N} ISK", m_character.Balance);

            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            if (ccpCharacter == null)
                return;

            IQueryMonitor marketMonitor = ccpCharacter.QueryMonitors[APICharacterMethods.MarketOrders];
            if (!Settings.UI.SafeForWork && !ccpCharacter.HasSufficientBalance && marketMonitor != null && marketMonitor.Enabled)
            {
                BalanceLabel.ForeColor = Color.Orange;
                BalanceLabel.Font = FontFactory.GetFont(Font, FontStyle.Bold);
                return;
            }

            BalanceLabel.ForeColor = !Settings.UI.SafeForWork && m_character.Balance < 0 ? Color.Red : SystemColors.ControlText;
            BalanceLabel.Font = FontFactory.GetFont(Font);
        }

        /// <summary>
        /// Refreshes the throbber.
        /// </summary>
        private void RefreshThrobber()
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            if (ccpCharacter == null)
            {
                HideThrobber();
                return;
            }

            if (ccpCharacter.QueryMonitors.Any(monitor => monitor.IsUpdating))
            {
                SetThrobberUpdating();
                return;
            }

            if (!NetworkMonitor.IsNetworkAvailable)
            {
                SetThrobberStrobing("Network Unavailable");
                return;
            }

            SetThrobberStopped();
            UpdateCountdown();
        }

        /// <summary>
        /// Updates the countdown.
        /// </summary>
        private void UpdateCountdown()
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            if (ccpCharacter == null)
                return;

            IQueryMonitor nextMonitor = ccpCharacter.QueryMonitors.NextMonitorToBeUpdated;

            if (nextMonitor == null)
            {
                UpdateLabel.Text = String.Empty;
                return;
            }

            TimeSpan timeLeft = nextMonitor.NextUpdate.Subtract(DateTime.UtcNow);

            if (timeLeft <= TimeSpan.Zero)
            {
                UpdateLabel.Text = "Pending...";
                return;
            }

            if (UpdateThrobber.State == ThrobberState.Rotating)
                return;

            UpdateLabel.Text = String.Format(CultureConstants.DefaultCulture, "{0:#00}:{1:d2}:{2:d2}",
                                             Math.Floor(timeLeft.TotalHours), timeLeft.Minutes, timeLeft.Seconds);
        }

        /// <summary>
        /// Sets the throbber stopped.
        /// </summary>
        private void SetThrobberStopped()
        {
            UpdateThrobber.State = ThrobberState.Stopped;

            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            if (ccpCharacter == null)
                return;

            if (!ccpCharacter.Identity.APIKeys.Any() || ccpCharacter.QueryMonitors.Any(x => !x.CanForceUpdate))
            {
                ToolTip.SetToolTip(UpdateThrobber, String.Empty);
                return;
            }

            ToolTip.SetToolTip(UpdateThrobber, "Click to update now");
        }

        /// <summary>
        /// Sets the throbber strobing.
        /// </summary>
        /// <param name="status">The status.</param>
        private void SetThrobberStrobing(string status)
        {
            UpdateLabel.Text = String.Empty;
            UpdateThrobber.Visible = true;
            UpdateThrobber.State = ThrobberState.Strobing;
            ToolTip.SetToolTip(UpdateThrobber, status);
        }

        /// <summary>
        /// Sets the throbber updating.
        /// </summary>
        private void SetThrobberUpdating()
        {
            UpdateLabel.Text = String.Empty;
            UpdateThrobber.State = ThrobberState.Rotating;
            UpdateThrobber.Visible = true;
            ToolTip.SetToolTip(UpdateThrobber, "Retrieving data from API...");
        }

        /// <summary>
        /// Hides the throbber.
        /// </summary>
        private void HideThrobber()
        {
            UpdateLabel.Text = String.Empty;
            UpdateThrobber.Visible = false;
            UpdateThrobber.State = ThrobberState.Stopped;
        }

        /// <summary>
        /// Populates the attribute text for the attribute labels.
        /// </summary>
        private void FormatAttributes()
        {
            SetAttributeLabel(lblINTAttribute, EveAttribute.Intelligence);
            SetAttributeLabel(lblPERAttribute, EveAttribute.Perception);
            SetAttributeLabel(lblCHAAttribute, EveAttribute.Charisma);
            SetAttributeLabel(lblWILAttribute, EveAttribute.Willpower);
            SetAttributeLabel(lblMEMAttribute, EveAttribute.Memory);
        }

        /// <summary>
        /// Gets the update status.
        /// </summary>
        /// <returns>Status text to display in the tool tip.</returns>
        private string GetUpdateStatus()
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            if (ccpCharacter == null)
                return String.Empty;

            StringBuilder output = new StringBuilder();

            // Skip character's corporation monitors if they are bound with the character's personal monitor
            foreach (IQueryMonitor monitor in ccpCharacter.QueryMonitors.OrderedByUpdateTime.Where(
                monitor => monitor.Method.HasHeader() && monitor.HasAccess).Where(
                    monitor =>
                    (!m_character.Identity.CanQueryCharacterInfo || monitor.Method.GetType() != typeof(APICorporationMethods)) &&
                    (m_character.Identity.CanQueryCharacterInfo || !m_character.Identity.CanQueryCorporationInfo ||
                     monitor.Method.GetType() != typeof(APICharacterMethods))))
            {
                output.AppendLine(GetStatusForMonitor(monitor));
            }

            return output.ToString();
        }

        /// <summary>
        /// Generates text representing the time to next update.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>String describing the time until the next update.</returns>
        private static string GenerateTimeToNextUpdateText(IQueryMonitor monitor)
        {
            TimeSpan timeToNextUpdate = monitor.NextUpdate.Subtract(DateTime.UtcNow);

            if (monitor.Status == QueryStatus.Disabled)
                return "(Disabled)";

            if (timeToNextUpdate <= TimeSpan.Zero)
                return "(Pending)";

            if (monitor.NextUpdate == DateTime.MaxValue)
                return "(Never)";

            return timeToNextUpdate.TotalMinutes >= 60
                       ? String.Format(CultureConstants.DefaultCulture, "(~{0}h)", Math.Floor(timeToNextUpdate.TotalHours))
                       : String.Format(CultureConstants.DefaultCulture, "({0}m)", Math.Floor(timeToNextUpdate.TotalMinutes));
        }

        /// <summary>
        /// Gets the update status for a monitor.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>Status text for the monitor.</returns>
        private static string GetStatusForMonitor(IQueryMonitor monitor)
        {
            StringBuilder output = new StringBuilder();

            output.AppendFormat(CultureConstants.DefaultCulture, "{0}: ", monitor);

            if (monitor.Status == QueryStatus.Pending)
            {
                output.Append(GetDetailedStatusForMonitor(monitor));
                return output.ToString();
            }

            output.Append(monitor.Status.GetDescription());
            return output.ToString();
        }

        /// <summary>
        /// Gets the detailed status for monitor.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>Detailed status text for the monitor.</returns>
        private static string GetDetailedStatusForMonitor(IQueryMonitor monitor)
        {
            if (monitor.NextUpdate == DateTime.MaxValue)
                return "Never";

            TimeSpan remainingTime = monitor.NextUpdate.Subtract(DateTime.UtcNow);
            if (remainingTime.Minutes < 1)
                return "Less than a minute";

            return remainingTime.ToDescriptiveText(
                DescriptiveTextOptions.FullText |
                DescriptiveTextOptions.SpaceText |
                DescriptiveTextOptions.SpaceBetween, false);
        }

        /// <summary>
        /// Creates the new monitor toolstrip menu item.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>New menu item for a monitor.</returns>
        private static ToolStripMenuItem CreateNewMonitorToolStripMenuItem(IQueryMonitor monitor)
        {
            string menuText = String.Format(CultureConstants.DefaultCulture,
                                            "Update {0} {1}", monitor, GenerateTimeToNextUpdateText(monitor));

            ToolStripMenuItem menu;
            ToolStripMenuItem tempMenu = null;
            try
            {
                tempMenu = new ToolStripMenuItem(menuText);
                tempMenu.Tag = monitor.Method;
                tempMenu.Enabled = monitor.Enabled && monitor.HasAccess && monitor.CanForceUpdate;

                menu = tempMenu;
                tempMenu = null;
            }
            finally
            {
                if (tempMenu != null)
                    tempMenu.Dispose();
            }
            return menu;
        }

        /// <summary>
        /// Gets the attribute text for a character.
        /// </summary>
        /// <param name="label">The attribute label.</param>
        /// <param name="eveAttribute">The eve attribute.</param>
        /// <returns>Formatted string describing the attribute and its value.</returns>
        private void SetAttributeLabel(Control label, EveAttribute eveAttribute)
        {
            label.Text = m_character[eveAttribute].EffectiveValue.ToString(CultureConstants.DefaultCulture);

            label.Tag = eveAttribute;
        }

        /// <summary>
        /// Formats the characters skill summary as a multi-line string.
        /// </summary>
        /// <returns>Formatted list of information about a characters skills.</returns>
        private string FormatSkillSummary()
        {
            StringBuilder output = new StringBuilder();

            output.AppendFormat(CultureConstants.DefaultCulture, "Known Skills: {0}", m_character.KnownSkillCount).AppendLine();
            output.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level V: {0}",
                                m_character.GetSkillCountAtLevel(5)).AppendLine();
            output.AppendFormat(CultureConstants.DefaultCulture, "Total SP: {0:N0}",
                                m_character.SkillPoints).AppendLine();
            output.AppendFormat(CultureConstants.DefaultCulture, "Clone Limit: {0:N0}",
                                m_character.CloneSkillPoints).AppendLine();
            output.Append(m_character.CloneName);

            return output.ToString();
        }

        /// <summary>
        /// Gets the active ship description.
        /// </summary>
        /// <returns></returns>
        private string GetActiveShipText()
        {
            return String.Format(CultureConstants.DefaultCulture, "Active Ship: {0}",
                                 (!String.IsNullOrEmpty(m_character.ShipTypeName) && !String.IsNullOrEmpty(m_character.ShipName)
                                      ? String.Format(CultureConstants.DefaultCulture, "{0} [{1}]", m_character.ShipTypeName,
                                                      m_character.ShipName)
                                      : "Unknown"));
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            // No need to do this if control is not visible
            if (!Visible)
                return;

            UpdateFrequentControls();
        }

        /// <summary>
        /// Handles the SettingsChanged event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            // No need to do this if control is not visible
            if (!Visible)
                return;

            UpdateInfrequentControls();
        }

        /// <summary>
        /// Handles the CharacterUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            // No need to do this if control is not visible
            if (!Visible || e.Character != m_character)
                return;

            UpdateInfrequentControls();
        }

        /// <summary>
        /// Handles the CharacterInfoUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterInfoUpdated(object sender, CharacterChangedEventArgs e)
        {
            // No need to do this if control is not visible
            if (!Visible || e.Character != m_character)
                return;

            UpdateInfoControls();
        }

        /// <summary>
        /// Handles the MarketOrdersChanged event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_MarketOrdersUpdated(object sender, CharacterChangedEventArgs e)
        {
            // No need to do this if control is not visible
            if (!Visible || e.Character != m_character)
                return;

            FormatBalance();
        }

        #endregion


        #region Local Events

        /// <summary>
        /// Occurs when the user click the throbber.
        /// Query the API for or a full update when possible, or show the throbber's context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpdateThrobber_Click(object sender, MouseEventArgs e)
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            // This is not a CCP character, it can't be updated
            if (ccpCharacter == null)
                return;

            // There has been an error in the past (Authorization, Server Error, etc.)
            // or updating now will return the same data because the cache has not expired
            // or character has no associated API key
            if (UpdateThrobber.State == ThrobberState.Strobing || !ccpCharacter.Identity.APIKeys.Any() ||
                ccpCharacter.QueryMonitors.Any(x => !x.CanForceUpdate))
            {
                ThrobberContextMenu.Show(MousePosition);
                return;
            }

            if (e.Button == MouseButtons.Right)
                return;

            // All checks out query everything
            ccpCharacter.QueryMonitors.QueryEverything();
        }

        /// <summary>
        /// Handles the MouseHover event of the UpdateLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpdateLabel_MouseHover(object sender, EventArgs e)
        {
            ToolTip.SetToolTip(UpdateLabel, GetUpdateStatus());
        }

        /// <summary>
        /// Handles the Opening event of the ThrobberContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void ThrobberContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip contextMenu = (ContextMenuStrip)sender;

            // Remove all the items after the separator including the separator
            int separatorIndex = contextMenu.Items.IndexOf(ThrobberSeparator);
            while (separatorIndex > -1 && separatorIndex < contextMenu.Items.Count)
            {
                contextMenu.Items.RemoveAt(separatorIndex);
            }

            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            // Exit for non-CCP characters or no associated API key
            if (ccpCharacter == null || !ccpCharacter.Identity.APIKeys.Any() || !ccpCharacter.QueryMonitors.Any())
            {
                QueryEverythingMenuItem.Enabled = false;
                return;
            }

            // Enables / Disables the "query everything" menu item
            QueryEverythingMenuItem.Enabled = ccpCharacter.QueryMonitors.All(x => x.HasAccess && x.CanForceUpdate);

            // Add a separator before monitor items if it doesn't exist already
            if (!ThrobberContextMenu.Items.Contains(ThrobberSeparator))
                ThrobberContextMenu.Items.Add(ThrobberSeparator);

            // Add monitor items
            // Skip character's corporation monitors if they are bound with the character's personal monitor
            foreach (ToolStripMenuItem menuItem in ccpCharacter.QueryMonitors.Where(
                monitor => monitor.Method.HasHeader() && monitor.HasAccess).Where(
                    monitor => !m_character.Identity.CanQueryCharacterInfo ||
                               monitor.Method.GetType() != typeof(APICorporationMethods)).Select(
                                   CreateNewMonitorToolStripMenuItem))
            {
                ThrobberContextMenu.Items.Add(menuItem);
            }
        }

        /// <summary>
        /// Handles the ItemClicked event of the ThrobberContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ToolStripItemClickedEventArgs"/> instance containing the event data.</param>
        private void ThrobberContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            if (ccpCharacter == null)
                return;

            if (e.ClickedItem == QueryEverythingMenuItem)
            {
                SetThrobberUpdating();
                ccpCharacter.QueryMonitors.QueryEverything();
                return;
            }

            Enum method = e.ClickedItem.Tag as Enum;

            if (method == null)
                return;

            SetThrobberUpdating();

            foreach (IQueryMonitor monitor in ccpCharacter.QueryMonitors.Where(
                monitor => monitor.Method.ToString().Contains(method.ToString())))
            {
                ccpCharacter.QueryMonitors.Query(monitor.Method);
            }
        }

        /// <summary>
        /// Handles the MouseHover event of the SkillSummaryLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SkillSummaryLabel_MouseHover(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int skillLevel = 0; skillLevel <= 5; skillLevel++)
            {
                int count = m_character.GetSkillCountAtLevel(skillLevel);

                if (skillLevel > 0)
                    sb.AppendLine();

                sb.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level {0}: {1}", skillLevel,
                                count.ToString(CultureConstants.DefaultCulture).PadLeft(5));
            }

            ToolTip.SetToolTip((Label)sender, sb.ToString());
        }

        /// <summary>
        /// When the user hovers over one of the attribute label, we display a tooltip such as :
        /// 19.8 (7 base + 7 remap points + 4 implants)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttributeLabel_MouseHover(object sender, EventArgs e)
        {
            // Retrieve the attribute from the sender
            Label attributeLabel = (Label)sender;
            EveAttribute eveAttribute = (EveAttribute)attributeLabel.Tag;

            // Format the values for the tooltip
            ICharacterAttribute attribute = m_character[eveAttribute];
            string toolTip = attribute.ToString("%e (%B base + %r remap points + %i implants)");
            ToolTip.SetToolTip(attributeLabel, toolTip);
        }

        /// <summary>
        /// Handles the MouseHover event of the CorporationNameLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CorporationNameLabel_MouseHover(object sender, EventArgs e)
        {
            if (m_character.AllianceID == 0)
                return;

            string tooltipText = String.Format(CultureConstants.DefaultCulture, "Alliance member of: {0}",
                                               m_character.AllianceName);
            ToolTip.SetToolTip((Label)sender, tooltipText);
        }

        /// <summary>
        /// Handles the MouseHover event of the ActiveShipLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ActiveShipLabel_MouseHover(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(m_character.LastKnownLocation))
                return;

            // Show the tooltip on when the user provides api key
            APIKey apiKey = m_character.Identity.FindAPIKeyWithAccess(APICharacterMethods.CharacterInfo);
            if (apiKey == null)
                return;

            string location = "Lost in space";

            // Check if in an NPC station or in an outpost
            Station station = m_character.LastKnownStation;

            // Not in any station ?
            if (station == null)
            {
                // Has to be in a solar system at least
                SolarSystem system = m_character.LastKnownSolarSystem;

                // Not in a solar system ??? Then show default location
                if (system != null)
                    location = String.Format(CultureConstants.DefaultCulture, "{0} ({1:N1})", system.FullLocation,
                                             system.SecurityLevel);
            }
            else
            {
                ConquerableStation outpost = station as ConquerableStation;
                location = String.Format(CultureConstants.DefaultCulture, "{0} ({1:N1})\nDocked in {2}",
                                         station.SolarSystem.FullLocation,
                                         station.SolarSystem.SecurityLevel,
                                         (outpost != null ? outpost.FullName : station.Name));
            }

            string tooltipText = String.Format(CultureConstants.DefaultCulture, "Location: {0}", location);
            ToolTip.SetToolTip((Label)sender, tooltipText);
        }

        /// <summary>
        /// Handles the Resize event of the CharacterMonitorHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CharacterMonitorHeader_Resize(object sender, EventArgs e)
        {
            Height = MainTableLayoutPanel.Height;
            MainTableLayoutPanel.Width = Width;
        }

        /// <summary>
        /// Handles the Click event of the ChangeAPIKeyInfoMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ChangeAPIKeyInfoMenuItem_Click(object sender, EventArgs e)
        {
            // This menu should be enabled only for CCP characters
            WindowsFactory.ShowByTag<ApiKeyUpdateOrAdditionWindow, IEnumerable<APIKey>>(m_character.Identity.APIKeys);
        }

        #endregion
    }
}
