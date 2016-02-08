using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Scheduling;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Controls
{
    /// <summary>
    /// Represents an item displayed on the overview.
    /// </summary>
    public partial class OverviewItem : UserControl
    {
        #region Fields

        private readonly bool m_isTooltip;

        private Color m_settingsForeColor;
        private bool m_showConflicts;
        private bool m_showSkillInTraining;
        private bool m_showCompletionTime;
        private bool m_showRemainingTime;
        private bool m_showWalletBalance;
        private bool m_showPortrait;
        private bool m_showSkillQueueTrainingTime;
        private int m_portraitSize = 96;

        private bool m_hovered;
        private bool m_pressed;
        private int m_preferredWidth = 1;
        private int m_preferredHeight = 1;

        private bool m_hasRemainingTime;
        private bool m_hasCompletionTime;
        private bool m_hasSkillInTraining;
        private bool m_hasSkillQueueTrainingTime;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor for designer.
        /// </summary>
        private OverviewItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor used in-code.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="isTooltip">if set to <c>true</c> if this instance is used as tooltip.</param>
        internal OverviewItem(Character character, bool isTooltip = false)
            : this()
        {
            m_isTooltip = isTooltip;
            Character = character;
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Completes initialization.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Returns in design mode or when no char
            if (DesignMode || this.IsDesignModeHosted())
                return;

            DoubleBuffered = true;

            // Initializes fonts
            lblCharName.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            lblBalance.Font = FontFactory.GetFont("Tahoma", 9.75F, FontStyle.Bold);
            lblRemainingTime.Font = FontFactory.GetFont("Tahoma", 9.75F);
            lblSkillInTraining.Font = FontFactory.GetFont("Tahoma", 8.25F);
            lblCompletionTime.Font = FontFactory.GetFont("Tahoma");
            lblSkillQueueTrainingTime.Font = FontFactory.GetFont("Tahoma", 8.25F);

            // Initializes the portrait
            pbCharacterPortrait.Visible = false;
            pbCharacterPortrait.Character = Character;

            // Initialize the skill queue free room label text
            lblSkillQueueTrainingTime.Text = String.Empty;

            // Global events
            EveMonClient.CharacterSkillQueueUpdated += EveMonClient_CharacterSkillQueueUpdated;
            EveMonClient.QueuedSkillsCompleted += EveMonClient_QueuedSkillsCompleted;
            EveMonClient.MarketOrdersUpdated += EveMonClient_MarketOrdersUpdated;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.SchedulerChanged += EveMonClient_SchedulerChanged;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            Disposed += OnDisposed;

            UpdateFromSettings();
        }

        /// <summary>
        /// On dispose, unsubscribe events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.CharacterSkillQueueUpdated -= EveMonClient_CharacterSkillQueueUpdated;
            EveMonClient.QueuedSkillsCompleted -= EveMonClient_QueuedSkillsCompleted;
            EveMonClient.MarketOrdersUpdated -= EveMonClient_MarketOrdersUpdated;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.SchedulerChanged -= EveMonClient_SchedulerChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Occurs when the visibility changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            UpdateContent();
            UpdateTrainingTime();

            base.OnVisibleChanged(e);
        }

        /// <summary>
        /// Paints a button behind when hovered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (!m_hovered)
                return;

            ButtonRenderer.DrawButton(e.Graphics, DisplayRectangle, m_pressed
                ? PushButtonState.Pressed
                : PushButtonState.Hot);
            base.OnPaint(e);
        }

        /// <summary>
        /// When the mouse enters control, we need to display the back button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!Clickable)
                return;

            // Show back button
            m_hovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// When the mouse leaves the control, we need to hide the button background.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            m_hovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_pressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_pressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the character control is bound to.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// Gets or sets true whether a button should appear on hover.
        /// </summary>
        [Description("When true, a background button will appear on hover and the control will fire Click event")]
        public bool Clickable { get; set; }

        #endregion


        #region Content update

        /// <summary>
        /// Updates from settings.
        /// </summary>
        private void UpdateFromSettings()
        {
            TrayPopupSettings trayPopupSettings = Settings.UI.SystemTrayPopup;
            MainWindowSettings mainWindowSettings = Settings.UI.MainWindow;
            PortraitSizes portraitSize = m_isTooltip
                ? trayPopupSettings.PortraitSize
                : mainWindowSettings.OverviewItemSize;

            // Misc fields
            m_portraitSize = Int32.Parse(portraitSize.ToString().Substring(1), CultureConstants.InvariantCulture);
            m_showConflicts = !m_isTooltip || trayPopupSettings.HighlightConflicts;
            m_showCompletionTime = !m_isTooltip || trayPopupSettings.ShowCompletionTime;
            m_showRemainingTime = !m_isTooltip || trayPopupSettings.ShowRemainingTime;
            m_showSkillInTraining = !m_isTooltip || trayPopupSettings.ShowSkillInTraining;
            m_showWalletBalance = m_isTooltip
                ? trayPopupSettings.ShowWallet
                : mainWindowSettings.ShowOverviewWallet;
            m_showPortrait = m_isTooltip
                ? trayPopupSettings.ShowPortrait
                : mainWindowSettings.ShowOverviewPortrait;
            m_showSkillQueueTrainingTime = m_isTooltip
                ? trayPopupSettings.ShowSkillQueueTrainingTime
                : mainWindowSettings.ShowOverviewSkillQueueTrainingTime;

            // Update colors
            UpdateContrastColor();

            // Update the controls
            UpdateContent();
        }

        /// <summary>
        /// Updates the color of the contrast.
        /// </summary>
        private void UpdateContrastColor()
        {
            m_settingsForeColor = (m_isTooltip && Settings.UI.SystemTrayPopup.UseIncreasedContrast)
                                  || (!m_isTooltip && Settings.UI.MainWindow.UseIncreasedContrastOnOverview)
                ? Color.Black
                : Color.DimGray;

            lblBalance.ForeColor = m_settingsForeColor;
            lblRemainingTime.ForeColor = m_settingsForeColor;
            lblSkillInTraining.ForeColor = m_settingsForeColor;
            lblCompletionTime.ForeColor = m_settingsForeColor;
        }

        /// <summary>
        /// Update the controls.
        /// </summary>
        private void UpdateContent()
        {
            this.SuspendDrawing();

            // Update character's 'Adorned Name' and 'Portrait' in case they have changed
            lblCharName.Text = Character.AdornedName;
            pbCharacterPortrait.Character = Character;

            FormatBalance();

            // Character in training ? We have labels to fill
            if (Character.IsTraining)
            {
                // Update the skill in training label
                QueuedSkill trainingSkill = Character.CurrentlyTrainingSkill;
                lblSkillInTraining.Text = trainingSkill.ToString();
                DateTime endTime = trainingSkill.EndTime.ToLocalTime();

                // Update the completion time
                lblCompletionTime.Text = (m_portraitSize > 80
                    ? String.Format(CultureConstants.DefaultCulture, "{0:ddd} {0}", endTime)
                    : endTime.ToString(CultureConstants.DefaultCulture));

                // Changes the completion time color on scheduling block
                string blockingEntry;
                bool isAutoBlocking;
                bool isBlocking = Scheduler.SkillIsBlockedAt(endTime, out blockingEntry, out isAutoBlocking);
                CCPCharacter ccpCharacter = Character as CCPCharacter;
                lblCompletionTime.ForeColor = m_showConflicts && isBlocking &&
                                              (ccpCharacter == null || ccpCharacter.SkillQueue.Count == 1 || !isAutoBlocking)
                    ? Color.Red
                    : m_settingsForeColor;

                // Updates the time remaining label
                lblRemainingTime.Text = trainingSkill.RemainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);

                // Update the skill queue free room label
                UpdateSkillQueueTrainingTime();

                // Show the training labels
                m_hasSkillInTraining = true;
                m_hasCompletionTime = true;
                m_hasRemainingTime = true;
                m_hasSkillQueueTrainingTime = true;
            }
            else
            {
                // Hide the training labels
                m_hasSkillInTraining = false;
                m_hasCompletionTime = false;
                m_hasRemainingTime = false;
                m_hasSkillQueueTrainingTime = false;
            }

            // Adjusts all the controls layout
            PerformCustomLayout(m_isTooltip);

            this.ResumeDrawing();
        }

        /// <summary>
        /// Formats the balance.
        /// </summary>
        private void FormatBalance()
        {
            lblBalance.Text = String.Format(CultureConstants.DefaultCulture, "{0:N} ISK", Character.Balance);

            CCPCharacter ccpCharacter = Character as CCPCharacter;

            if (ccpCharacter == null)
                return;

            IQueryMonitor marketMonitor = ccpCharacter.QueryMonitors[CCPAPICharacterMethods.MarketOrders];
            if (!Settings.UI.SafeForWork && !ccpCharacter.HasSufficientBalance && marketMonitor != null && marketMonitor.Enabled)
            {
                lblBalance.ForeColor = Color.Orange;
                return;
            }

            lblBalance.ForeColor = !Settings.UI.SafeForWork && ccpCharacter.Balance < 0 ? Color.Red : m_settingsForeColor;
        }

        /// <summary>
        /// Updates the controls' visibility.
        /// </summary>
        /// <returns></returns>
        private void UpdateVisibilities()
        {
            lblRemainingTime.Visible = m_hasRemainingTime & m_showRemainingTime;
            lblCompletionTime.Visible = m_hasCompletionTime & m_showCompletionTime;
            lblSkillInTraining.Visible = m_hasSkillInTraining & m_showSkillInTraining;
            lblSkillQueueTrainingTime.Visible = m_hasSkillQueueTrainingTime & m_showSkillQueueTrainingTime;
            lblBalance.Visible = m_showWalletBalance;
        }

        /// <summary>
        /// Updates the training time.
        /// </summary>
        private void UpdateTrainingTime()
        {
            if (!Visible || !Character.IsTraining)
                return;

            TimeSpan remainingTime = Character.CurrentlyTrainingSkill.RemainingTime;
            lblRemainingTime.Text = remainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);

            UpdateSkillQueueTrainingTime();
        }

        /// <summary>
        /// Updates the skill queue training time.
        /// </summary>
        /// <returns></returns>
        private void UpdateSkillQueueTrainingTime()
        {
            CCPCharacter ccpCharacter = Character as CCPCharacter;

            // Current character isn't a CCP character, so can't have a Queue
            if (ccpCharacter == null)
                return;

            DateTime skillQueueEndTime = ccpCharacter.SkillQueue.EndTime;
            TimeSpan timeLeft = DateTime.UtcNow.AddHours(24).Subtract(skillQueueEndTime);

            // Negative time ?
            if (timeLeft < TimeSpan.Zero)
            {
                // More than one entry in queue ? Display total queue remaining time
                if (ccpCharacter.SkillQueue.Count > 1)
                {
                    lblSkillQueueTrainingTime.ForeColor = m_settingsForeColor;
                    lblSkillQueueTrainingTime.Text = String.Format(
                        CultureConstants.DefaultCulture,
                        "Queue finishes in: {0}",
                        skillQueueEndTime.ToRemainingTimeShortDescription(DateTimeKind.Utc));
                    return;
                }

                // We don't display anything
                lblSkillQueueTrainingTime.Text = String.Empty;
                return;
            }

            // Training completed ?
            if (timeLeft == TimeSpan.Zero)
            {
                // We don't display anything
                lblSkillQueueTrainingTime.Text = String.Empty;
                return;
            }

            // Skill queue is empty ?
            if (timeLeft > TimeSpan.FromDays(1))
            {
                lblSkillQueueTrainingTime.Text = @"Skill queue is empty";
                return;
            }

            // Less than one minute ? Display seconds else display time without seconds
            string timeLeftText = (timeLeft < TimeSpan.FromMinutes(1)
                ? timeLeft.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)
                : timeLeft.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, false));

            lblSkillQueueTrainingTime.ForeColor = Color.Red;
            lblSkillQueueTrainingTime.Text = String.Format(CultureConstants.DefaultCulture,
                "{0} free room in skill queue", timeLeftText);
        }

        #endregion


        #region Global Events

        /// <summary>
        /// On every second, we update the remaining time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateTrainingTime();
        }

        /// <summary>
        /// When the settings changed, update everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateFromSettings();
        }

        /// <summary>
        /// When the scheduler changed, we may have to display a warning (blocking entry).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SchedulerChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// On skill completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_QueuedSkillsCompleted(object sender, QueuedSkillsEventArgs e)
        {
            if (e.Character != Character)
                return;

            // Character still training ? Jump to next skill
            if (Character.IsTraining)
                UpdateContent();
            else
            {
                lblRemainingTime.Text = @"Completed";
                m_hasCompletionTime = false;
                UpdateVisibilities();
            }
        }

        /// <summary>
        /// On character market orders updated, update the balance format.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_MarketOrdersUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != Character)
                return;

            FormatBalance();
        }

        /// <summary>
        /// On character sheet changed, update everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != Character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// On character skill queue changed, update everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterSkillQueueUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != Character)
                return;

            UpdateContent();
        }

        #endregion


        #region Layout

        /// <summary>
        /// Adjusts all the controls layout.
        /// </summary>
        /// <param name="tooltip"></param>
        private void PerformCustomLayout(bool tooltip)
        {
            if (!Visible)
                return;

            SuspendLayout();

            UpdateVisibilities();

            bool showPortrait = (m_showPortrait && !Settings.UI.SafeForWork);
            int portraitSize = m_portraitSize;

            int margin = 10;
            if (tooltip)
            {
                if (portraitSize <= 48)
                    margin = 2;
                else if (portraitSize <= 64)
                    margin = 4;
                else if (portraitSize <= 80)
                    margin = 6;
            }

            // Label height
            int labelHeight = 18;
            int smallLabelHeight = 13;
            if (portraitSize <= 48)
                labelHeight = 13;
            else if (portraitSize <= 64)
                labelHeight = 16;

            // Label width
            int labelWidth = 0;
            if (!tooltip)
                labelWidth = (int)(250 * (Graphics.FromHwnd(Handle).DpiX / EVEMonConstants.DefaultDpi));

            // Big font size
            float bigFontSize = 11.25f;
            if (portraitSize <= 48)
                bigFontSize = 8.25f;
            else if (portraitSize <= 64)
                bigFontSize = 9.75f;

            // Medium font size
            float mediumFontSize = 9.75f;
            if (portraitSize <= 64)
                mediumFontSize = 8.25f;

            // Margin between the two labels groups
            int verticalMargin = (m_showSkillQueueTrainingTime ? 4 : 16);
            if (portraitSize <= 80)
                verticalMargin = 0;

            // Adjust portrait
            pbCharacterPortrait.Location = new Point(margin, margin);
            pbCharacterPortrait.Size = new Size(portraitSize, portraitSize);
            pbCharacterPortrait.Visible = showPortrait;

            // Adjust the top labels
            int top = margin - 2;
            int left = (showPortrait ? portraitSize + margin * 2 : margin);
            int rightPad = tooltip ? 10 : 0;

            lblCharName.Font = FontFactory.GetFont(lblCharName.Font.FontFamily, bigFontSize, lblCharName.Font.Style);
            lblCharName.Location = new Point(left, top);
            labelWidth = Math.Max(labelWidth, lblCharName.PreferredWidth + rightPad);
            labelHeight = Math.Max(labelHeight, lblCharName.Font.Height);
            lblCharName.Size = new Size(labelWidth, labelHeight);
            top += labelHeight;

            if (lblBalance.Visible)
            {
                lblBalance.Font = FontFactory.GetFont(lblBalance.Font.FontFamily, mediumFontSize, lblBalance.Font.Style);
                lblBalance.Location = new Point(left, top);
                labelWidth = Math.Max(labelWidth, lblBalance.PreferredWidth + rightPad);
                labelHeight = Math.Max(labelHeight, lblBalance.Font.Height);
                lblBalance.Size = new Size(labelWidth, labelHeight);
                top += labelHeight;
            }

            if (lblRemainingTime.Visible || lblSkillInTraining.Visible || lblCompletionTime.Visible)
                top += verticalMargin;

            if (lblRemainingTime.Visible)
            {
                lblRemainingTime.Font = FontFactory.GetFont(lblRemainingTime.Font.FontFamily, mediumFontSize,
                    lblRemainingTime.Font.Style);
                lblRemainingTime.Location = new Point(left, top);
                labelWidth = Math.Max(labelWidth, lblRemainingTime.PreferredWidth + rightPad);
                labelHeight = Math.Max(labelHeight, lblRemainingTime.Font.Height);
                lblRemainingTime.Size = new Size(labelWidth, labelHeight);
                top += labelHeight;
            }

            if (lblSkillInTraining.Visible)
            {
                lblSkillInTraining.Location = new Point(left, top);
                labelWidth = Math.Max(labelWidth, lblSkillInTraining.PreferredWidth + rightPad);
                smallLabelHeight = Math.Max(smallLabelHeight, lblSkillInTraining.Font.Height);
                lblSkillInTraining.Size = new Size(labelWidth, smallLabelHeight);
                top += smallLabelHeight;
            }

            if (lblCompletionTime.Visible)
            {
                lblCompletionTime.Location = new Point(left, top);
                labelWidth = Math.Max(labelWidth, lblCompletionTime.PreferredWidth + rightPad);
                smallLabelHeight = Math.Max(smallLabelHeight, lblCompletionTime.Font.Height);
                lblCompletionTime.Size = new Size(labelWidth, smallLabelHeight);
                top += smallLabelHeight;
            }

            if (lblSkillQueueTrainingTime.Visible)
            {
                lblSkillQueueTrainingTime.Location = new Point(left, top);
                labelWidth = Math.Max(labelWidth, lblSkillQueueTrainingTime.PreferredWidth + rightPad);
                smallLabelHeight = Math.Max(smallLabelHeight, lblSkillQueueTrainingTime.Font.Height);
                lblSkillQueueTrainingTime.Size = new Size(labelWidth, smallLabelHeight);
                top += smallLabelHeight;
            }

            Height = (pbCharacterPortrait.Visible ? Math.Max(pbCharacterPortrait.Height + 2 * margin, top + margin) : top + margin);

            Width = left + labelWidth + margin;
            m_preferredHeight = Height;
            m_preferredWidth = Width;

            ResumeLayout(false);
        }

        /// <summary>
        /// Gets the preferred size for control. Used by parents to decide which size they will grant to their children.
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size(m_preferredWidth, m_preferredHeight);
        }

        #endregion
    }
}