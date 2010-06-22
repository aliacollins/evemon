using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using EVEMon.Common;
using EVEMon.Common.Scheduling;
using EVEMon.Common.SettingsObjects;

using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using System.Globalization;

namespace EVEMon
{
    /// <summary>
    /// Represents an item displayed on the overview.
    /// </summary>
    public partial class OverviewItem : UserControl
    {
        private readonly Color m_lightForeColor;
        private Character m_character;
        private int m_portraitSize = 96;
        private bool m_hovered;
        private bool m_pressed = false;
        private bool m_pendingUpdate;
        private bool m_loading;
        private int m_preferredWidth = 1;
        private int m_preferredHeight = 1;

        private bool m_showSkillInTraining;
        private bool m_showCompletionTime;
        private bool m_showRemainingTime;
        private bool m_showWalletBalance;
        private bool m_showPortrait;
        private bool m_showConflicts;
        private bool m_showSkillQueueFreeRoom;

        private bool m_hasRemainingTime;
        private bool m_hasCompletionTime;
        private bool m_hasSkillInTraining;
		private bool m_hasSkillQueueFreeRoom;

        private bool m_tooltip = false;

        #region Initialization, destruction

        /// <summary>
        /// Default constructor for designer.
        /// </summary>
        public OverviewItem()
        {
            InitializeComponent();

            // Initializes fonts and colors
            this.lblCharName.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            this.lblBalance.Font = FontFactory.GetFont("Tahoma", 9.75F, FontStyle.Bold);
            this.lblRemainingTime.Font = FontFactory.GetFont("Tahoma", 9.75F, FontStyle.Regular);
            this.lblSkillInTraining.Font = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Regular);
            this.lblCompletionTime.Font = FontFactory.GetFont("Tahoma", FontStyle.Regular);
            this.lblSkillQueueFreeRoom.Font = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Regular);

            // Misc fields
            m_showPortrait = true;
            m_showWalletBalance = true;
            m_showRemainingTime = true;
            m_showCompletionTime = true;
            m_showSkillInTraining = true;
            m_showConflicts = true;
            m_showSkillQueueFreeRoom = true;
            m_portraitSize = 96;
            m_lightForeColor = lblCompletionTime.ForeColor;

            // Initialize the skill queue free room label text
            lblSkillQueueFreeRoom.Text = String.Empty;

            // Global events
            EveClient.QueuedSkillsCompleted += new EventHandler<QueuedSkillsEventArgs>(EveClient_QueuedSkillsCompleted);
            EveClient.CharacterChanged += new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.SchedulerChanged += new EventHandler(EveClient_SchedulerChanged);
            EveClient.TimerTick += new EventHandler(EveClient_TimerTick);
            this.Disposed += new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Constructor used in-code
        /// </summary>
        /// <param name="character"></param>
        /// <param name="portraitSize"></param>
        private OverviewItem(Character character, PortraitSizes portraitSize)
            :this()
        {
            m_character = character;
            m_portraitSize = Int32.Parse(portraitSize.ToString().Substring(1), CultureConstants.DefaultCulture);

            pbCharacterPortrait.Visible = false;
            pbCharacterPortrait.Character = character;
        }

        /// <summary>
        /// Constructor used in-code.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="settings"></param>
        public OverviewItem(Character character, TrayPopupSettings settings)
            : this(character, settings.PortraitSize)
        {
            m_showConflicts = settings.HighlightConflicts;
            m_showCompletionTime = settings.ShowCompletionTime;
            m_showRemainingTime = settings.ShowRemainingTime;
            m_showSkillInTraining = settings.ShowSkillInTraining;
            m_showWalletBalance = settings.ShowWallet;
            m_showPortrait = settings.ShowPortrait;
            m_showSkillQueueFreeRoom = settings.ShowSkillQueueFreeRoom;
            m_tooltip = true;

            // Initializes colors
            if (!Settings.UI.SystemTrayPopup.UseIncreasedContrast)
                return;

            lblBalance.ForeColor = Color.Black;
            lblRemainingTime.ForeColor = Color.Black;
            lblSkillInTraining.ForeColor = Color.Black;
            lblCompletionTime.ForeColor = Color.Black;
            m_lightForeColor = lblCompletionTime.ForeColor;
        }

        /// <summary>
        /// Constructor used in-code.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="settings"></param>
        public OverviewItem(Character character, MainWindowSettings settings)
            : this(character, settings.OverviewItemSize)
        {
            m_showWalletBalance = settings.ShowOverviewWallet;
            m_showPortrait = settings.ShowOverviewPortrait;
            m_showSkillQueueFreeRoom = settings.ShowOverviewSkillQueueFreeRoom;

            // Initializes colors
            if (!Settings.UI.MainWindow.UseIncreasedContrastOnOverview)
                return;

            lblBalance.ForeColor = Color.Black;
            lblRemainingTime.ForeColor = Color.Black;
            lblSkillInTraining.ForeColor = Color.Black;
            lblCompletionTime.ForeColor = Color.Black;
            m_lightForeColor = lblCompletionTime.ForeColor;
        }

        /// <summary>
        /// On dispose, unsubscrive events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDisposed(object sender, EventArgs e)
        {
            EveClient.QueuedSkillsCompleted -= new EventHandler<QueuedSkillsEventArgs>(EveClient_QueuedSkillsCompleted);
            EveClient.CharacterChanged -= new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.SchedulerChanged -= new EventHandler(EveClient_SchedulerChanged);
            EveClient.TimerTick -= new EventHandler(EveClient_TimerTick);
            this.Disposed -= new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Completes initialization.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Returns in design mode or when no char
            if (this.DesignMode || this.IsDesignModeHosted()) return;

            // Character Name
            PerformCustomLayout(m_tooltip);
            UpdateContent();
            m_loading = true;

            base.OnLoad(e);
        }

        #endregion


        #region Global events and content update

        /// <summary>
        /// Gets the character this control is bound to.
        /// </summary>
        [Browsable(false)]
        public Character Character
        {
            get { return m_character; }
            set
            {
                if (m_character == value)
                    return;

                pbCharacterPortrait.Character = value;
                m_character = value;

                UpdateContent();
            }
        }

        /// <summary>
        /// Update the controls.
        /// </summary>
        private void UpdateContent()
        {
            if (!this.Visible)
            {
                m_pendingUpdate = true;
                return;
            }

            m_pendingUpdate = false;

            lblCharName.Text = m_character.Name;

            var ccpCharacter = m_character as CCPCharacter;
            if (ccpCharacter != null)
                lblBalance.ForeColor = (ccpCharacter.HasInsufficientBalance ? Color.Orange : lblBalance.ForeColor);
            
            lblBalance.Text = String.Format(CultureConstants.DefaultCulture, "{0:N} ISK", m_character.Balance);

            // Character in training ? We have labels to fill
            if (m_character.IsTraining)
            {
                // Update the skill in training label
                var trainingSkill = m_character.CurrentlyTrainingSkill;
                lblSkillInTraining.Text = trainingSkill.ToString();
                DateTime endTime = trainingSkill.EndTime.ToLocalTime();

                // Update the completion time
                if (m_portraitSize > 80)
                {
                    lblCompletionTime.Text = String.Concat(endTime.ToString("ddd ", CultureConstants.DefaultCulture),
                                                           endTime.ToString(CultureConstants.DefaultCulture));
                }
                else
                {
                    lblCompletionTime.Text = endTime.ToString(CultureConstants.DefaultCulture);
                }

                // Changes the completion time color on scheduling block.
                string blockingEntry;
                if (m_showConflicts && Scheduler.SkillIsBlockedAt(trainingSkill.EndTime, out blockingEntry))
                {
                    lblCompletionTime.ForeColor = Color.Red;
                }
                else
                {
                    lblCompletionTime.ForeColor = m_lightForeColor;
                }

                // Updates the time remaining label
                lblRemainingTime.Text = trainingSkill.RemainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);

                // Show the training labels
                m_hasSkillInTraining = true;
                m_hasCompletionTime = true;
                m_hasRemainingTime = true;

                // Updates the skill queue free room
                UpdateSkillQueueFreeRoom();
            }
            else
            {
                // Hide the training labels
                m_hasSkillInTraining = false;
                m_hasCompletionTime = false;
                m_hasRemainingTime = false;
				m_hasSkillQueueFreeRoom = false; // Yes, it's a lie!
            }

            // Adjusts all the controls layout.
            PerformCustomLayout(m_tooltip);
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
			lblSkillQueueFreeRoom.Visible = m_hasSkillQueueFreeRoom & m_showSkillQueueFreeRoom;
            lblBalance.Visible = m_showWalletBalance;
        }

        /// <summary>
        /// Updates the skill queue free room.
        /// </summary>
        /// <returns></returns>
        private void UpdateSkillQueueFreeRoom()
        {
            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            // Current character isn't a CCP character, so can't have a Queue.
            if (ccpCharacter == null)
                return;

            var skillQueueEndTime = ccpCharacter.SkillQueue.EndTime;
            bool freeTime = skillQueueEndTime < DateTime.UtcNow.AddHours(24);

			if (freeTime)
			{
                TimeSpan timeLeft = DateTime.UtcNow.AddHours(24).Subtract(skillQueueEndTime);
				string timeLeftText;

				// Prevents the "(none)" text from being displayed
				if (timeLeft < TimeSpan.FromSeconds(1))
					return;

				// Less than minute ? Display seconds
				if (timeLeft < TimeSpan.FromMinutes(1))
				{
					timeLeftText = timeLeft.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);
				}
				// Display time without seconds
				else
				{
                    timeLeftText = timeLeft.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, false);
				}

				lblSkillQueueFreeRoom.Text = String.Format(CultureConstants.DefaultCulture, "{0} free room in skill queue", timeLeftText);
				m_hasSkillQueueFreeRoom = true;
			}
			else
			{
				m_hasSkillQueueFreeRoom = false;
			}
        }

        /// <summary>
        /// On every second, we update the remaining time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_TimerTick(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            if (m_character.IsTraining)
            {
                var remainingTime = m_character.CurrentlyTrainingSkill.RemainingTime;
                lblRemainingTime.Text = remainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);

                UpdateSkillQueueFreeRoom();

                if (m_loading)
                    UpdateContent();

                m_loading = false;
            }
            else
            {
                UpdateContent();
            }
        }

        /// <summary>
        /// When the scheduler changed, we may have to change the color of the trained skill.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveClient_SchedulerChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// On skill completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_QueuedSkillsCompleted(object sender, QueuedSkillsEventArgs e)
        {
            if (m_character != e.Character) return;

            // Character still training ? Jump to next skill
            if (m_character.IsTraining)
            {
                UpdateContent();
            }
            else
            {
                lblRemainingTime.Text = "Completed";
                m_hasCompletionTime = false;
                UpdateVisibilities();
            }
        }

        /// <summary>
        /// On character completion, update everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_CharacterChanged(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character) return;
            UpdateContent();
        }

        #endregion


        #region Controls events

        /// <summary>
        /// Occurs when the visibility changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Visible)
            {
                if (m_pendingUpdate) UpdateContent();
                EveClient_TimerTick(null, null);
            }
            base.OnVisibleChanged(e);
        }

        /// <summary>
        /// Paints a button behind when hovered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_hovered)
            {
                ButtonRenderer.DrawButton(e.Graphics, this.DisplayRectangle,
                    m_pressed ? PushButtonState.Pressed
                              : PushButtonState.Hot);
            }
            base.OnPaint(e);
        }

        /// <summary>
        /// When the mouse enters this control, we need to display the back button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this.Clickable) return;

            // Show back button
            m_hovered = true;
            this.Invalidate();

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// When the mouse leaves the control, we need to hide the button background.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            m_hovered = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_pressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_pressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        #endregion


        #region Layout

        /// <summary>
        /// Gets or sets true whether a button should appear on hover.
        /// </summary>
        [Browsable(true), Description("When true, a background button will appear on hover and the control will fire Click event")]
        public bool Clickable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size mode.
        /// </summary>
        [Browsable(true), Description("The size of a portrait.")]
        public PortraitSizes PortraitSize
        {
            get { return (PortraitSizes)Enum.Parse(typeof(PortraitSizes), String.Format(CultureConstants.DefaultCulture, "x{0}", m_portraitSize)); }
            set
            {
                var portraitSize = Int32.Parse(value.ToString().Substring(1), CultureConstants.DefaultCulture);
                if (m_portraitSize == portraitSize) return;
                m_portraitSize = portraitSize;

                PerformCustomLayout(m_tooltip);
            }
        }

        [Browsable(true), Description("When true, the skill currently in training will be displayed.")]
        /// <summary>
        /// Gets or sets whether the skill currently in training must be displayed.
        /// </summary>
        public bool ShowSkillInTraining
        {
            get { return m_showSkillInTraining; }
            set 
            {
                m_showSkillInTraining = value;
                PerformCustomLayout(m_tooltip);
            }
        }

        [Browsable(true), Description("When true, the remaining training time will be displayed.")]
        /// <summary>
        /// Gets or sets whether the remaining time must be displayed.
        /// </summary>
        public bool ShowRemainingTime
        {
            get { return m_showRemainingTime; }
            set 
            { 
                m_showRemainingTime = value;
                PerformCustomLayout(m_tooltip);
            }
        }

        [Browsable(true), Description("When true, the training completion time will be displayed.")]
        /// <summary>
        /// Gets or sets whether the training completion time must be displayed.
        /// </summary>
        public bool ShowCompletionTime
        {
            get { return m_showCompletionTime; }
            set 
            {
                m_showCompletionTime = value;
                PerformCustomLayout(m_tooltip);
            }
        }

        [Browsable(true), Description("When true, the wallet balance will be displayed.")]
        /// <summary>
        /// Gets or sets whether the wallet balance must be displayed.
        /// </summary>
        public bool ShowWalletBalance
        {
            get { return m_showWalletBalance; }
            set 
            {
                m_showWalletBalance = value;
                PerformCustomLayout(m_tooltip);
            }
        }

        [Browsable(true), Description("When true, the portrait will be displayed.")]
        /// <summary>
        /// Gets or sets whether the portrait must be displayed.
        /// </summary>
        public bool ShowPortrait
        {
            get { return m_showPortrait; }
            set
            {
                m_showPortrait = value;
                PerformCustomLayout(m_tooltip);
            }
        }

        [Browsable(true), Description("When true, the skill queue free room time will be displayed.")]
        /// <summary>
        /// Gets or sets whether the skill queue free room time must be displayed.
        /// </summary>
        public bool ShowSkillQueueFreeRoom
        {
            get { return m_showSkillQueueFreeRoom; }
            set
            {
                m_showSkillQueueFreeRoom = value;
                PerformCustomLayout(m_tooltip);
            }
        }

        /// <summary>
        /// Adjusts all the controls layout.
        /// </summary>
        /// <param name="portraitSize"></param>
        public void PerformCustomLayout(bool tooltip)
        {
            UpdateVisibilities();

            bool showPortrait = (m_showPortrait && !Settings.UI.SafeForWork);
            int portraitSize = m_portraitSize;

            // Retrieve margin
            int margin = 10;
            if (portraitSize <= 48) margin = 1;
            else if (portraitSize <= 64) margin = 3;
            else if (portraitSize <= 80) margin = 6;

            // Label height
            int labelHeight = 18;
            int smallLabelHeight = 13;
            if (portraitSize <= 48) labelHeight = 13;
            else if (portraitSize <= 64) labelHeight = 16;

            // Label width
            int labelWidth = 0;
            if (!tooltip) labelWidth = 215;
            
            // Big font size
            var bigFontSize = 11.25f;
            if (portraitSize <= 48) bigFontSize = 8.25f;
            else if (portraitSize <= 64) bigFontSize = 9.75f;

            // Medium font size
            var mediumFontSize = 9.75f;
            if (portraitSize <= 64) mediumFontSize = 8.25f;

            // Margin between the two labels groups
            int verticalMargin = (m_showSkillQueueFreeRoom ? 4 : 16);
            if (portraitSize <= 80) verticalMargin = 0;

            // Adjust portrait
            pbCharacterPortrait.Location = new Point(margin, margin);
            pbCharacterPortrait.Size = new Size(portraitSize, portraitSize);
            pbCharacterPortrait.Visible = showPortrait;

            // Adjust the top labels
            int top = margin - 2;
            int left = (showPortrait ? portraitSize + margin * 2 : margin);
            int right = 10;

            lblCharName.Font = FontFactory.GetFont(lblCharName.Font.FontFamily, bigFontSize, lblCharName.Font.Style);
            lblCharName.Location = new Point(left, top);
            if (lblCharName.PreferredWidth + right > labelWidth) labelWidth = lblCharName.PreferredWidth + right;
            labelHeight = Math.Max (labelHeight, lblCharName.Font.Height);
            lblCharName.Size = new Size(labelWidth, labelHeight);
            top += labelHeight;

            if (lblBalance.Visible)
            {
                lblBalance.Font = FontFactory.GetFont(lblBalance.Font.FontFamily, mediumFontSize, lblBalance.Font.Style);
                lblBalance.Location = new Point(left, top);
                if (lblBalance.PreferredWidth + right > labelWidth) labelWidth = lblBalance.PreferredWidth + right;
                labelHeight = Math.Max(labelHeight, lblBalance.Font.Height);
                lblBalance.Size = new Size(labelWidth, labelHeight);
                top += labelHeight;
            }

            if (lblRemainingTime.Visible || lblSkillInTraining.Visible || lblCompletionTime.Visible)
            {
                top += verticalMargin;
            }

            if (lblRemainingTime.Visible)
            {
                lblRemainingTime.Font = FontFactory.GetFont(lblRemainingTime.Font.FontFamily, mediumFontSize, lblRemainingTime.Font.Style);
                lblRemainingTime.Location = new Point(left, top);
                if (lblRemainingTime.PreferredWidth + right > labelWidth) labelWidth = lblRemainingTime.PreferredWidth + right;
                labelHeight = Math.Max(labelHeight, lblRemainingTime.Font.Height);
                lblRemainingTime.Size = new Size(labelWidth, labelHeight);
                top += labelHeight;
            }

            if (lblSkillInTraining.Visible)
            {
                lblSkillInTraining.Location = new Point(left, top);
                if (lblSkillInTraining.PreferredWidth + right > labelWidth) labelWidth = lblSkillInTraining.PreferredWidth + right;
                smallLabelHeight = Math.Max(smallLabelHeight, lblSkillInTraining.Font.Height);
                lblSkillInTraining.Size = new Size(labelWidth, smallLabelHeight);
                top += smallLabelHeight;
            }

            if (lblCompletionTime.Visible)
            {
                lblCompletionTime.Location = new Point(left, top);
                if (lblCompletionTime.PreferredWidth + right > labelWidth) labelWidth = lblCompletionTime.PreferredWidth + right;
                smallLabelHeight = Math.Max(smallLabelHeight, lblCompletionTime.Font.Height);
                lblCompletionTime.Size = new Size(labelWidth, smallLabelHeight);
                top += smallLabelHeight;
            }
            
            if (lblSkillQueueFreeRoom.Visible)
            {
                lblSkillQueueFreeRoom.Location = new Point(left, top);
                if (lblSkillQueueFreeRoom.PreferredWidth + right > labelWidth) labelWidth = lblSkillQueueFreeRoom.PreferredWidth + right;
                smallLabelHeight = Math.Max(smallLabelHeight, lblSkillQueueFreeRoom.Font.Height);
                lblSkillQueueFreeRoom.Size = new Size(labelWidth, smallLabelHeight);
                top += smallLabelHeight;
            }

            if (pbCharacterPortrait.Visible)
            {
                this.Height = Math.Max(pbCharacterPortrait.Height + 2 * margin, top + margin);
            }
            else
            {
                this.Height = top + margin;
            }

            this.Width = left + labelWidth + margin;
            m_preferredHeight = this.Height;
            m_preferredWidth = this.Width;
        }

        /// <summary>
        /// Gets the preferred size for this control. Used by parents to decide which size they will grant to their children.
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