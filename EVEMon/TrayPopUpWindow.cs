using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    /// <summary>
    /// Popup form displayed when the user hovers over the tray icon
    /// </summary>
    /// <remarks>
    /// Display contents are governed by Settings.TrayPopupConfig<br/>
    /// Popup location is determined using mouse location, screen and screen bounds (see SetPosition()).<br/>
    /// </remarks>
    public partial class TrayPopUpWindow : Form
    {
        private readonly int[] m_portraitSize = { 16, 24, 32, 40, 48, 56, 64 };

        private readonly Label m_eveTimeLabel = new Label();
        private readonly Label m_serverStatusLabel = new Label();
        private bool m_updatePending;


        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TrayPopUpWindow()
        {
            InitializeComponent();
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Adds the character panes to the form, gets the TQ status message and sets the popup position
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Look'n feel
            Font = FontFactory.GetFont(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints);
            mainPanel.BackColor = SystemColors.ControlLightLight;

            // Client events
            EveMonClient.MonitoredCharacterCollectionChanged += EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.QueuedSkillsCompleted += EveMonClient_QueuedSkillsCompleted;
            EveMonClient.ServerStatusUpdated += EveMonClient_ServerStatusUpdated;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.TimerTick += EveMonClient_TimerTick;

            // Character Details
            UpdateContent();
        }

        /// <summary>
        /// Unregister events.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            EveMonClient.MonitoredCharacterCollectionChanged -= EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.QueuedSkillsCompleted -= EveMonClient_QueuedSkillsCompleted;
            EveMonClient.ServerStatusUpdated -= EveMonClient_ServerStatusUpdated;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
        }

        /// <summary>
        /// When the window becomes visible again, checks whether an update is pending.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible && m_updatePending)
                UpdateContent();

            base.OnVisibleChanged(e);
        }

        /// <summary>
        /// Draws the rounded rectangle border and background.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw the border and background
            DrawBorder(e);
        }

        /// <summary>
        /// Sets this window as topmost without activating it.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Show the given form on topmost without activating it
            this.ShowInactiveTopmost(NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
        }

        /// <summary>
        /// Draws the rounded rectangle border.
        /// </summary>
        /// <param name="e"></param>
        private static void DrawBorder(PaintEventArgs e)
        {
            // Create graphics object to work with
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // Define the size of the rectangle used for each of the 4 corner arcs.
            const int Radius = 4;
            Size cornerSize = new Size(Radius * 2, Radius * 2);

            // Construct a GraphicsPath for the outline
            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();

                // Top left
                path.AddArc(new Rectangle(0, 0, cornerSize.Width, cornerSize.Height), 180, 90);

                // Top Right
                path.AddArc(new Rectangle(e.ClipRectangle.Width - 1 - cornerSize.Width, 0, cornerSize.Width, cornerSize.Height),
                            270, 90);

                // Bottom right
                path.AddArc(new Rectangle(e.ClipRectangle.Width - 1 - cornerSize.Width,
                                          e.ClipRectangle.Height - 1 - cornerSize.Height, cornerSize.Width, cornerSize.Height),
                            0, 90);

                // Bottom Left
                path.AddArc(new Rectangle(0, e.ClipRectangle.Height - 1 - cornerSize.Height,
                                          cornerSize.Width, cornerSize.Height), 90, 90);
                path.CloseFigure();

                // Draw the background
                using (Brush fillBrush = new SolidBrush(SystemColors.ControlLightLight))
                {
                    g.FillPath(fillBrush, path);
                }

                // Now the border
                g.DrawPath(SystemPens.WindowFrame, path);
            }
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Occurs when the monitored characters collection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_MonitoredCharacterCollectionChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// Updates the TQ status message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_ServerStatusUpdated(object sender, EveServerEventArgs e)
        {
            UpdateServerStatusLabel();
        }

        /// <summary>
        /// Once per second, we update the eve time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateEveTimeLabel();
        }

        /// <summary>
        /// Occur when characters completed skills. We refresh the controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_QueuedSkillsCompleted(object sender, QueuedSkillsEventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// When the settings changed, some may affect this popup, so we recreate content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        #endregion


        #region Content management : add the controls to the panel, update them, etc

        /// <summary>
        /// Gets the characters list, sorted, grouped and filter according to the user settings.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Character> GetCharacters
        {
            get
            {
                IEnumerable<Character> characters = EveMonClient.MonitoredCharacters;

                // Filter characters not in training ?
                if (!Settings.UI.SystemTrayPopup.ShowCharNotTraining)
                    characters = characters.Where(x => x.IsTraining);

                // Sort
                List<Character> charactersList = characters.ToList();
                charactersList.StableSort(new CharacterComparer(Settings.UI.SystemTrayPopup.SecondarySortOrder));
                charactersList.StableSort(new CharacterComparer(Settings.UI.SystemTrayPopup.PrimarySortOrder));

                // Grouping
                List<Character> newCharacters = new List<Character>();
                switch (Settings.UI.SystemTrayPopup.GroupBy)
                {
                    case TrayPopupGrouping.None:
                        return charactersList;
                    case TrayPopupGrouping.Account:
                        newCharacters.AddRange(charactersList.Where(x => !x.Identity.APIKeys.IsEmpty()));
                        return newCharacters.GroupBy(AccountAPIKeyOrDefault).SelectMany(y => y);
                    case TrayPopupGrouping.TrainingAtTop:
                        newCharacters.AddRange(charactersList.Where(x => x.IsTraining));
                        newCharacters.AddRange(charactersList.Where(x => !x.IsTraining));
                        return newCharacters;
                    case TrayPopupGrouping.TrainingAtBottom:
                        newCharacters.AddRange(charactersList.Where(x => !x.IsTraining));
                        newCharacters.AddRange(charactersList.Where(x => x.IsTraining));
                        return newCharacters;
                    default:
                        return characters;
                }
            }
        }

        /// <summary>
        /// Returns the API key for characters in the same account or the default one.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>The API key for characters in the same account; otherwise the default API key of the character</returns>
        private static APIKey AccountAPIKeyOrDefault(Character character)
        {
            return character.Identity.APIKeys.First(
                apiKey => EveMonClient.MonitoredCharacters.Any(
                    monitoredCharacter => monitoredCharacter.Identity.APIKeys.Contains(apiKey)));
        }

        /// <summary>
        /// Recreates the controls for character and warning.
        /// </summary>
        private void UpdateContent()
        {
            if (!Visible)
            {
                m_updatePending = true;
                return;
            }
            m_updatePending = false;

            IEnumerable<Character> characters = GetCharacters;

            // Remove controls and dispose them
            IEnumerable<Control> oldControls = mainPanel.Controls.Cast<Control>();
            mainPanel.Controls.Clear();
            foreach (Control ctl in oldControls)
            {
                ctl.Dispose();
            }

            // Add controls for characters
            if (Settings.UI.SystemTrayPopup.GroupBy == TrayPopupGrouping.Account &&
                Settings.UI.SystemTrayPopup.IndentGroupedAccounts)
            {
                List<APIKey> prevAPIKeys = new List<APIKey>();
                foreach (Character character in characters)
                {
                    List<APIKey> apiKeys = character.Identity.APIKeys.ToList();
                    if (!apiKeys.Exists(apiKey => prevAPIKeys.Contains(apiKey)))
                    {
                        mainPanel.Controls.Add(new OverviewItem(character, Settings.UI.SystemTrayPopup));
                        prevAPIKeys = apiKeys;
                    }
                    else
                    {
                        FlowLayoutPanel tempAccountGroupPanel = null;
                        try
                        {
                            tempAccountGroupPanel = new FlowLayoutPanel();
                            OverviewItem charPanel = new OverviewItem(character, Settings.UI.SystemTrayPopup);
                            tempAccountGroupPanel.AutoSize = true;
                            tempAccountGroupPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                            tempAccountGroupPanel.FlowDirection = FlowDirection.TopDown;
                            tempAccountGroupPanel.Padding = new Padding(10, 0, 0, 0);
                            tempAccountGroupPanel.Controls.Add(charPanel);

                            FlowLayoutPanel accountGroupPanel = tempAccountGroupPanel;
                            tempAccountGroupPanel = null;

                            mainPanel.Controls.Add(accountGroupPanel);
                        }
                        finally
                        {
                            if (tempAccountGroupPanel != null)
                                tempAccountGroupPanel.Dispose();
                        }

                        prevAPIKeys = apiKeys;
                    }
                }
            }
            else
                mainPanel.Controls.AddRange(characters.Select(x => new OverviewItem(x, Settings.UI.SystemTrayPopup)).ToArray());

            // Skip if the user do not want to be warned about accounts not in training
            if (Settings.UI.SystemTrayPopup.ShowWarning)
            {
                // Creates the warning for accounts not in training
                string warningMessage;
                if (APIKey.HasCharactersNotTraining(out warningMessage))
                {
                    FlowLayoutPanel warningPanel = CreateAccountsNotTrainingPanel(warningMessage);
                    mainPanel.Controls.Add(warningPanel);
                }
            }

            // Server Status
            if (Settings.UI.SystemTrayPopup.ShowServerStatus)
            {
                m_serverStatusLabel.AutoSize = true;
                mainPanel.Controls.Add(m_serverStatusLabel);
                UpdateServerStatusLabel();
            }

            // EVE Time
            if (Settings.UI.SystemTrayPopup.ShowEveTime)
            {
                m_eveTimeLabel.AutoSize = true;
                mainPanel.Controls.Add(m_eveTimeLabel);
                UpdateEveTimeLabel();
            }

            // Updates the tooltip width
            CompleteLayout();
        }

        /// <summary>
        /// Creates a panel contains the warning message for accounts not in training.
        /// </summary>
        /// <param name="warningMessage"></param>
        /// <returns></returns>
        private FlowLayoutPanel CreateAccountsNotTrainingPanel(string warningMessage)
        {
            // Create a flowlayout to hold the content
            FlowLayoutPanel warningPanel;
            FlowLayoutPanel tempWarningPanel = null;
            try
            {
                tempWarningPanel = new FlowLayoutPanel();
                tempWarningPanel.AutoSize = true;
                tempWarningPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                tempWarningPanel.Margin = new Padding(0, 0, 0, 2);

                // Add a picture on the left with a warning icon
                if (!Settings.UI.SafeForWork)
                {
                    PictureBox tempPictureBoxWarning = null;
                    try
                    {
                        tempPictureBoxWarning = new PictureBox();
                        tempPictureBoxWarning.Image = SystemIcons.Warning.ToBitmap();
                        tempPictureBoxWarning.SizeMode = PictureBoxSizeMode.StretchImage;
                        tempPictureBoxWarning.Size = new Size(m_portraitSize[(int)Settings.UI.SystemTrayPopup.PortraitSize],
                                                              m_portraitSize[(int)Settings.UI.SystemTrayPopup.PortraitSize]);
                        tempPictureBoxWarning.Margin = new Padding(2);

                        PictureBox pbWarning = tempPictureBoxWarning;
                        tempPictureBoxWarning = null;

                        tempWarningPanel.Controls.Add(pbWarning);
                    }
                    finally
                    {
                        if (tempPictureBoxWarning != null)
                            tempPictureBoxWarning.Dispose();
                    }
                }

                // Adds a label to hold the message
                Label tempLabelMessage = null;
                try
                {
                    tempLabelMessage = new Label();
                    tempLabelMessage.AutoSize = true;
                    tempLabelMessage.Text = warningMessage;

                    Label lblMessage = tempLabelMessage;
                    tempLabelMessage = null;

                    tempWarningPanel.Controls.Add(lblMessage);
                }
                finally
                {
                    if (tempLabelMessage != null)
                        tempLabelMessage.Dispose();
                }

                warningPanel = tempWarningPanel;
                tempWarningPanel = null;
            }
            finally
            {
                if (tempWarningPanel != null)
                    tempWarningPanel.Dispose();
            }
            return warningPanel;
        }

        /// <summary>
        /// Completes the layout after new controls have been added.
        /// </summary>
        private void CompleteLayout()
        {
            // Fix the panel widths to the largest
            // We let the framework determine the appropriate widths, then fix them so that
            // updates to training time remaining don't cause the form to resize
            int pnlWidth = (mainPanel.Controls.Cast<Control>().Select(control => control.Width)).Concat(new[] { 0 }).Max();

            foreach (Control control in mainPanel.Controls)
            {
                FlowLayoutPanel flowPanel = control as FlowLayoutPanel;
                if (flowPanel == null)
                    continue;

                int pnlHeight = flowPanel.Height;
                flowPanel.AutoSize = false;
                flowPanel.Width = pnlWidth;
                flowPanel.Height = pnlHeight;
            }

            // Position Popup
            TrayIcon.SetToolTipLocation(this);
        }

        /// <summary>
        /// Updates the EVE time label.
        /// </summary>
        private void UpdateEveTimeLabel()
        {
            if (Settings.UI.SystemTrayPopup.ShowEveTime)
                m_eveTimeLabel.Text = String.Format(CultureConstants.DefaultCulture, "EVE Time: {0:HH:mm}", DateTime.UtcNow);
        }

        /// <summary>
        /// Updates the server status label.
        /// </summary>
        private void UpdateServerStatusLabel()
        {
            if (m_serverStatusLabel == null)
                return;

            if (Settings.UI.SystemTrayPopup.ShowServerStatus)
                m_serverStatusLabel.Text = EveMonClient.EVEServer.StatusText;
        }

        #endregion
    }
}