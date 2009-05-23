using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class CharactersGridItem : UserControl
    {
        /// <summary>
        /// Holde the controls displayed over the back button.
        /// </summary>
        /// <remarks>
        /// For a label to be transparent, it has to be right above its parent. 
        /// When the backbutton is hidden, the other controls must belong to this grid item (they would not be visible if they were children of backbutton).
        /// However, when the backbutton is visible, the other controls must belong to it to be transparent.
        /// </remarks>
        private readonly List<Control> m_overlayControls;

        private readonly Color m_lightForeColor;
        private readonly CharacterMonitor m_characterMon;
        private readonly CharacterInfo m_character;

        private Settings m_settings;
        private DateTime m_estimatedCompletion;
        private bool m_highlightConflict;
        private bool m_update;

        /// <summary>
        /// Default constructor for designer
        /// </summary>
        public CharactersGridItem()
            : this(null)
        {
        }

        /// <summary>
        /// Constructor used by <see cref="CharactersGrid"/>
        /// </summary>
        /// <param name="characterMon"></param>
        public CharactersGridItem(CharacterMonitor characterMon)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            this.lblCharName.Font = FontHelper.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            this.lblBalance.Font = FontHelper.GetFont("Tahoma", 9.75F, FontStyle.Bold);
            this.lblSkillInTraining.Font = FontHelper.GetFont("Tahoma", 8.25F, FontStyle.Regular);
            this.lblTimeToCompletion.Font = FontHelper.GetFont("Tahoma", 9.75F, FontStyle.Regular);
            this.lblCompletionTime.Font = FontHelper.GetFont("Tahoma", FontStyle.Regular);

            // Retrieves data
            m_characterMon = characterMon;
            if (characterMon != null) m_character = characterMon.GrandCharacterInfo;

            // Retrieve overlay controls, see comments on m_overlayControls
            m_overlayControls = new List<Control>();
            foreach (Control child in this.Controls)
            {
                if (child != this.backButton) m_overlayControls.Add(child);
            }

            // Subscribe events
            this.backButton.Click += new EventHandler(backButton_Click);
            this.backButton.MouseLeave += new EventHandler(backButton_MouseLeave);

            // Misc fields
            m_estimatedCompletion = DateTime.MaxValue;
            m_lightForeColor = lblCompletionTime.ForeColor;
        }

        /// <summary>
        /// Gets the character monitor this control is bound to
        /// </summary>
        public CharacterMonitor CharacterMonitor
        {
            get { return m_characterMon; }
        }

        /// <summary>
        /// Completes initialization
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Returns in design mode or when no char
            if (this.DesignMode || m_characterMon == null) return;

            // Retrieves data
            m_settings = Settings.GetInstance();

            // Character portrait (not in safe mode)
            if (!m_settings.WorksafeMode)
            {
                if (m_characterMon.CharacterPortrait != null)
                {
                    pbCharacterPortrait.Image = m_characterMon.CharacterPortrait;
                }
                else
                {
                    m_characterMon.PortraitChanged += new EventHandler(characterMon_portraitChanged);
                }
            }

            // Character Name
            lblCharName.Text = m_character.Name;

            // Skill Training Info
            UpdateSkillTraiingInfo();

            // Balance
            UpdateWalletInfo();

            // Starts when necessary
            if (m_character.IsTraining && m_character.CurrentlyTrainingSkill != null)
            {
                Start();
            } 
            
            base.OnLoad(e);
        }


        /// <summary>
        /// Updates the wallet's info, anytime the char's infos are redownloaded from CCP.
        /// </summary>
        /// <param name="charInfo"></param>
        /// <param name="config"></param>
        private void UpdateWalletInfo()
        {
            lblBalance.Text = m_character.Balance.ToString("#,##0.00") + " ISK";
        }

        /// <summary>
        /// Updates the skill training's infos, anytime the char's infos are redownloaded from CCP or EVEMon guess a skills has finished
        /// </summary>
        private void UpdateSkillTraiingInfo()
        {
            // Not loaded yet
            if (m_settings == null) return;

            // Character in training ? We have labels to fill
            if (m_character.IsTraining)
            {
                Skill trainingSkill = m_character.CurrentlyTrainingSkill;

                if (trainingSkill != null)
                {
                    // Updates the completion's date
                    m_estimatedCompletion = trainingSkill.EstimatedCompletion;

                    // Updates the conflict highlight trigger
                    string blockingEntry = string.Empty;
                    m_highlightConflict = m_settings.SkillIsBlockedAt(m_estimatedCompletion, out blockingEntry);

                    // Updates the time remaining label
                    UpdateTimeRemainingLabel();

                    // Updates the skill in training label
                    lblSkillInTraining.Show();
                    lblSkillInTraining.Text = trainingSkill.Name + " " + Skill.GetRomanForInt(trainingSkill.TrainingToLevel);

                    if (m_highlightConflict) lblSkillInTraining.ForeColor = Color.Red;
                    else lblSkillInTraining.ForeColor = m_lightForeColor;

                    // Updates the completion time label
                    lblCompletionTime.Show();
                    lblCompletionTime.Text = m_estimatedCompletion.ToString("ddd ") + m_estimatedCompletion.ToString();

                    if (m_highlightConflict) lblCompletionTime.ForeColor = Color.Red;
                    else lblCompletionTime.ForeColor = m_lightForeColor;
                }
                else
                {
                    lblSkillInTraining.Text = "Skill training info not available";
                    lblTimeToCompletion.Hide();
                    lblCompletionTime.Hide();
                }
            }
            else
            {
                lblSkillInTraining.Hide();
                lblTimeToCompletion.Hide();
                lblCompletionTime.Hide();
            }
        }

        /// <summary>
        /// Updates the time remaining on every second
        /// </summary>
        private void UpdateTimeRemainingLabel()
        {
            if (m_estimatedCompletion != DateTime.MaxValue)
            {
                var timeLeft = m_estimatedCompletion - DateTime.Now;

                lblTimeToCompletion.Show();
                lblTimeToCompletion.Text = Skill.TimeSpanToDescriptiveText(timeLeft, DescriptiveTextOptions.IncludeCommas);

                if (m_highlightConflict) lblTimeToCompletion.ForeColor = Color.Red;
                else lblTimeToCompletion.ForeColor = m_lightForeColor;
            }
            else
            {
                lblTimeToCompletion.Hide();
            }
        }

        /// <summary>
        /// Updates the portrait when this the bound monitor completed its loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void characterMon_portraitChanged(object sender, EventArgs e)
        {
            pbCharacterPortrait.Image = m_characterMon.CharacterPortrait;
        }

        /// <summary>
        /// Subscribe events and will accept further per-second updates
        /// </summary>
        public void Start()
        {
            m_update = true;

            if (m_characterMon != null)
            {
                m_character.TrainingSkillChanged += new EventHandler(Character_TrainingSkillChanged);
                m_character.BalanceChanged += new EventHandler(GrandCharacterInfo_BalanceChanged);
            }

            UpdateSkillTraiingInfo();
            UpdateWalletInfo();
        }

        /// <summary>
        /// Unsubscribe events and will reject further per-second updates
        /// </summary>
        public void Stop()
        {
            m_update = false;

            if (m_characterMon != null)
            {
                m_character.TrainingSkillChanged -= new EventHandler(Character_TrainingSkillChanged);
                m_character.BalanceChanged -= new EventHandler(GrandCharacterInfo_BalanceChanged);
            }
        }

        /// <summary>
        /// Anytime the training skill changes (redownload, training complete, etc), we update some infos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Character_TrainingSkillChanged(object sender, EventArgs e)
        {
            if (!m_update) return;
            UpdateSkillTraiingInfo();
        }

        /// <summary>
        /// Anytime the balance is changed, we update the wallet's infos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GrandCharacterInfo_BalanceChanged(object sender, EventArgs e)
        {
            if (!m_update) return;
            UpdateWalletInfo();
        }

        /// <summary>
        /// This method is called every second by <see cref="CharactersGrid"/> to update the time remaining.
        /// </summary>
        public void UpdateOnTimerTick()
        {
            if (!m_update) return;
            UpdateTimeRemainingLabel();
        }

        /// <summary>
        /// When the mouse enters this control, we need to display the back button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            this.backButton.Show();
            
            // For explanations, see comments on m_overlayControls;
            foreach (Control child in m_overlayControls) child.Parent = this.backButton;

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// When the mouse leaves the back button, we need to hide it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void backButton_MouseLeave(object sender, EventArgs e)
        {
            if (this.backButton.GetChildAtPoint(this.backButton.PointToClient(MousePosition)) == null)
            {
                // For explanations, see comments on m_overlayControls;
                foreach (Control child in m_overlayControls) child.Parent = this;

                this.backButton.Hide();
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// When the button is clicked we consider the control is clicked (<see cref="CharactersGrid"/> being its subscriber, reacting by selecting the bound character's tab)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void backButton_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
    }
}
