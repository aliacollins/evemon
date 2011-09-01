using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon
{
    /// <summary>
    /// Displays a Windows-style tooltip
    /// </summary>
    public partial class TrayTooltipWindow : Form
    {
        private readonly List<Character> m_characters = new List<Character>();
        private String m_tooltipFormat = String.Empty;
        private bool m_updatePending;


        #region Constructor

        /// <summary>
        /// Designer constructor.
        /// </summary>
        public TrayTooltipWindow()
        {
            InitializeComponent();
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// On load, update controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            Font = FontFactory.GetFont(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints);

            EveMonClient.MonitoredCharacterCollectionChanged += EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;

            UpdateCharactersList();
        }

        /// <summary>
        /// On close, stops the timer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            EveMonClient.MonitoredCharacterCollectionChanged -= EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            displayTimer.Stop();
            base.OnClosed(e);
        }

        /// <summary>
        /// When the window is shown, sets it as topmost without activation.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Equivalent to setting TopMost = true, except don't activate the window
            NativeMethods.SetWindowPos(Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
                                       NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);

            // Show the window without activating it
            NativeMethods.ShowWindow(Handle, NativeMethods.SW_SHOWNOACTIVATE);
        }

        /// <summary>
        /// On visible, checks whether an update is pending.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible && m_updatePending)
                UpdateContent();

            base.OnVisibleChanged(e);
        }

        #endregion


        #region Content creation and refresh

        /// <summary>
        /// Updates the tooltip format we use as a base for update on every second, along with the characters list.
        /// </summary>
        private void UpdateCharactersList()
        {
            // Update characters list and selects display order
            m_characters.Clear();
            m_characters.AddRange(Settings.UI.SystemTrayTooltip.DisplayOrder
                                      ? TrayPopUpWindow.GetCharacters()
                                      : EveMonClient.MonitoredCharacters.Where(x => x.IsTraining));

            // Assembles the tooltip format
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Settings.UI.SystemTrayTooltip.Format))
            {
                // Bad tooltip format
                displayTimer.Enabled = false;
                sb.Append("You can configure this tooltip in the options/general panel");
            }
            else if (m_characters.Count == 0)
            {
                // No character in training
                displayTimer.Enabled = false;
                sb.Append("No Characters in training!");
            }
            else
            {
                // Start the display timer
                displayTimer.Enabled = true;
                displayTimer.Start();

                // Assemble tooltip base format with character informations
                foreach (Character character in m_characters)
                {
                    if (sb.Length != 0)
                        sb.Append("\n");

                    sb.Append(FormatTooltipText(Settings.UI.SystemTrayTooltip.Format, character));
                }
            }
            m_tooltipFormat = sb.ToString();

            // Update the tooltip's content
            UpdateContent();
        }

        /// <summary>
        /// Updates the tooltip's content.
        /// </summary>
        private void UpdateContent()
        {
            if (!Visible)
            {
                m_updatePending = true;
                return;
            }
            m_updatePending = false;

            // Replaces the fragments like "%10546464r" (the number being the character ID) by the remaining time.
            string tooltip = m_tooltipFormat;
            foreach (Character character in m_characters)
            {
                if (character.IsTraining)
                {
                    QueuedSkill trainingSkill = character.CurrentlyTrainingSkill;
                    TimeSpan remainingTime = trainingSkill.EndTime.Subtract(DateTime.UtcNow);

                    tooltip = Regex.Replace(tooltip,
                                            '%' + character.CharacterID.ToString() + 'r',
                                            remainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas),
                                            RegexOptions.Compiled);
                }

                CCPCharacter ccpCharacter = character as CCPCharacter;
                if (ccpCharacter != null && ccpCharacter.SkillQueue.IsPaused)
                {
                    tooltip = Regex.Replace(tooltip,
                                            '%' + character.CharacterID.ToString() + 'r', "(Paused)",
                                            RegexOptions.Compiled);
                }
            }

            // Updates the tooltip and its location
            lblToolTip.Text = tooltip;
            TrayIcon.SetToolTipLocation(this);
        }

        /// <summary>
        /// Formats the tooltip text.
        /// </summary>
        /// <param name="toolTipFormat">The tool tip format.</param>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        private string FormatTooltipText(string toolTipFormat, Character character)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Regex.Replace(toolTipFormat, "%([nbsdr]|[ct][ir])",
                                    m =>
                                        {
                                            // First group
                                            switch (m.Groups[1].Value[0])
                                            {
                                                case 'n':
                                                    return character.Name;
                                                case 'b':
                                                    return character.Balance.ToString("N2");
                                            }

                                            CCPCharacter ccpCharacter = character as CCPCharacter;
                                            if (ccpCharacter != null &&
                                                (ccpCharacter.IsTraining || ccpCharacter.SkillQueue.IsPaused))
                                            {
                                                int level;
                                                switch (m.Groups[1].Value[0])
                                                {
                                                    case 'r':
                                                        return '%' + character.CharacterID.ToString() + 'r';
                                                    case 's':
                                                        return character.CurrentlyTrainingSkill.SkillName;
                                                    case 'd':
                                                        return character.CurrentlyTrainingSkill.EndTime.ToString("g");
                                                    case 'c':
                                                        level = character.CurrentlyTrainingSkill.Level - 1;
                                                        break;
                                                    case 't':
                                                        level = character.CurrentlyTrainingSkill.Level;
                                                        break;
                                                    default:
                                                        return String.Empty;
                                                }

                                                // Second group
                                                if (level >= 0 && m.Groups[1].Value.Length > 1)
                                                {
                                                    switch (m.Groups[1].Value[1])
                                                    {
                                                        case 'i':
                                                            return level.ToString();
                                                        case 'r':
                                                            return Skill.GetRomanFromInt(level);
                                                        default:
                                                            return String.Empty;
                                                    }
                                                }
                                            }

                                            return String.Empty;
                                        }, RegexOptions.Compiled));

            return sb.ToString();
        }

        #endregion


        #region Timer and global events

        /// <summary>
        /// When a character changes (skill completed, now data from CCP, etc), update the characters list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            UpdateCharactersList();
        }

        /// <summary>
        /// Whenever the monitored characters collection changes, update the characters list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_MonitoredCharacterCollectionChanged(object sender, EventArgs e)
        {
            UpdateCharactersList();
        }

        /// <summary>
        /// Every second, when characters are in training, we update the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayTimer_Tick(object sender, EventArgs e)
        {
            UpdateContent();
        }

        #endregion
    }
}