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

        private Label m_eveTimeLabel;
        private Label m_serverStatusLabel;
        private bool m_updatePending;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TrayPopUpWindow()
        {
            InitializeComponent();
        }


        #region Control's lifecycle management and painting

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
        /// Sets this window as topmost without activiting it.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Show the given form on topmost without activating it
            this.ShowInactiveTopmost();
        }

        /// <summary>
        /// Draws the rounded rectangle border.
        /// </summary>
        /// <param name="e"></param>
        private void DrawBorder(PaintEventArgs e)
        {
            // Create graphics object to work with
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // Define the size of the rectangle used for each of the 4 corner arcs.
            const int Radius = 4;
            Size cornerSize = new Size(Radius * 2, Radius * 2);

            // Construct a GraphicsPath for the outline
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();

            // Top left
            path.AddArc(new Rectangle(0, 0, cornerSize.Width, cornerSize.Height), 180, 90);

            // Top Right
            path.AddArc(new Rectangle(e.ClipRectangle.Width - 1 - cornerSize.Width, 0, cornerSize.Width, cornerSize.Height), 270,
                        90);

            // Bottom right
            path.AddArc(
                new Rectangle(e.ClipRectangle.Width - 1 - cornerSize.Width, e.ClipRectangle.Height - 1 - cornerSize.Height,
                              cornerSize.Width, cornerSize.Height), 0, 90);

            // Bottom Left
            path.AddArc(new Rectangle(0, e.ClipRectangle.Height - 1 - cornerSize.Height, cornerSize.Width, cornerSize.Height), 90,
                        90);
            path.CloseFigure();

            // Draw the background
            Brush fillBrush = new SolidBrush(SystemColors.ControlLightLight);
            g.FillPath(fillBrush, path);

            // Now the border
            Pen borderPen = SystemPens.WindowFrame;
            g.DrawPath(borderPen, path);
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

            IEnumerable<Character> characters = GetCharacters();

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
                long prevAPIKeyID = 0;
                foreach (Character character in characters)
                {
                    if (character.Identity.APIKey.ID != prevAPIKeyID)
                    {
                        mainPanel.Controls.Add(new OverviewItem(character, Settings.UI.SystemTrayPopup));
                        prevAPIKeyID = character.Identity.APIKey.ID;
                    }
                    else
                    {
                        FlowLayoutPanel accountGroupPanel = new FlowLayoutPanel
                                                                {
                                                                    AutoSize = true,
                                                                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                                                    FlowDirection = FlowDirection.TopDown,
                                                                    Padding = new Padding(10, 0, 0, 0)
                                                                };

                        OverviewItem charPanel = new OverviewItem(character, Settings.UI.SystemTrayPopup);
                        accountGroupPanel.Controls.Add(charPanel);
                        mainPanel.Controls.Add(accountGroupPanel);
                        prevAPIKeyID = character.Identity.APIKey.ID;
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
                if (GlobalAPIKeyCollection.HasAccountsNotTraining(out warningMessage))
                {
                    FlowLayoutPanel warningPanel = CreateAccountsNotTrainingPanel(warningMessage);
                    mainPanel.Controls.Add(warningPanel);
                }
            }

            // Server Status
            if (Settings.UI.SystemTrayPopup.ShowServerStatus)
            {
                m_serverStatusLabel = new Label { AutoSize = true };
                mainPanel.Controls.Add(m_serverStatusLabel);
                UpdateServerStatusLabel();
            }

            // EVE Time
            if (Settings.UI.SystemTrayPopup.ShowEveTime)
            {
                m_eveTimeLabel = new Label { AutoSize = true };
                mainPanel.Controls.Add(m_eveTimeLabel);
                UpdateEveTimeLabel();
            }

            // Updates the tooltip width
            CompleteLayout();
        }

        /// <summary>
        /// Gets the characters list, sorted, grouped and filter according to the user settings.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Character> GetCharacters()
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
                    newCharacters.AddRange(charactersList);
                    return newCharacters;
                case TrayPopupGrouping.Account:
                    newCharacters.AddRange(charactersList.Where(x => x.Identity.APIKey != null));
                    return newCharacters.GroupBy(x => x.Identity.APIKey).SelectMany(y => y);
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

        /// <summary>
        /// Creates a panel contains the warning message for accounts not in training.
        /// </summary>
        /// <param name="warningMessage"></param>
        /// <returns></returns>
        private FlowLayoutPanel CreateAccountsNotTrainingPanel(string warningMessage)
        {
            // Create a flowlayout to hold the content
            FlowLayoutPanel warningPanel = new FlowLayoutPanel
                                               {
                                                   AutoSize = true,
                                                   AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                                   Margin = new Padding(0, 0, 0, 2)
                                               };

            // Add a picture on the left with a warning icon
            if (!Settings.UI.SafeForWork)
            {
                PictureBox pbWarning = new PictureBox
                                           {
                                               Image = SystemIcons.Warning.ToBitmap(),
                                               SizeMode = PictureBoxSizeMode.StretchImage,
                                               Size = new Size(m_portraitSize[(int)Settings.UI.SystemTrayPopup.PortraitSize],
                                                               m_portraitSize[(int)Settings.UI.SystemTrayPopup.PortraitSize]),
                                               Margin = new Padding(2)
                                           };
                warningPanel.Controls.Add(pbWarning);
            }

            // Adds a label to hold the message
            Label lblMessage = new Label { AutoSize = true, Text = warningMessage };
            warningPanel.Controls.Add(lblMessage);
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
                if (!(control is FlowLayoutPanel))
                    continue;

                FlowLayoutPanel flowPanel = (FlowLayoutPanel)control;
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