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
    /// <summary>
    /// User control to display a single character's details in the tray icon popup
    /// </summary>
    public partial class TrayStatusPopUpChar : UserControl
    {
        #region Fields
        private CharacterMonitor m_characterMon;
        private DateTime m_estimatedCompletion;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public TrayStatusPopUpChar()
            : this(null)
        {
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="cm">A CharacterMonitor object for the character to be displayed</param>
        public TrayStatusPopUpChar(CharacterMonitor cm)
        {
            InitializeComponent();
            m_characterMon = cm;
        }
        #endregion

        /// <summary>
        /// Sets the display values for the control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (m_characterMon != null)
            {
                // Get the character info and settings
                CharacterInfo charInfo = m_characterMon.GrandCharacterInfo;
                Settings s = Settings.GetInstance();
                // Characters not in training use gray text, not black
                if (!charInfo.IsTraining) { this.ForeColor = SystemColors.GrayText; }
                // Character portrait
                if (!s.WorksafeMode && s.TrayPopupShowPortrait)
                {
                    if (m_characterMon.CharacterPortrait != null)
                    {
                        pbCharacterPortrait.Image = m_characterMon.CharacterPortrait;
                    }
                }
                else
                {
                    pbCharacterPortrait.Hide();
                }
                // Character Name
                lblCharName.Text = charInfo.Name;
                // Skill Training Info
                if (s.TrayPopupShowSkill && charInfo.IsTraining)
                {
                    Skill trainingSkill = charInfo.CurrentlyTrainingSkill;
                    if (trainingSkill != null)
                    {
                        m_estimatedCompletion = trainingSkill.EstimatedCompletion;
                        lblSkillInTraining.Text = trainingSkill.Name + " " + Skill.GetRomanForInt(trainingSkill.TrainingToLevel);
                        // Time to completion
                        if (s.TrayPopupShowSkillTime)
                        {
                            UpdateTimeRemainingLabel();
                            updateTimer.Start();
                        }
                        else
                        {
                            lblTimeToCompletion.Hide();
                        }
                        // Completion time
                        if (s.TrayPopupShowSkillEnd && m_estimatedCompletion != DateTime.MaxValue)
                        {
                            lblCompletionTime.Text = m_estimatedCompletion.ToString("ddd ") + m_estimatedCompletion.ToString();
                        }
                        else
                        {
                            lblCompletionTime.Hide();
                        }
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
                // Balance
                if (s.TrayPopupShowBalance)
                {
                    lblBalance.Text = "Balance: " + charInfo.Balance.ToString("#,##0.00") + " ISK";
                }
                else
                {
                    lblBalance.Hide();
                }
            }
        }

        /// <summary>
        /// Displays skill training time remaining
        /// </summary>
        private void UpdateTimeRemainingLabel()
        {
            DateTime now = DateTime.Now;
            if (m_estimatedCompletion != DateTime.MaxValue)
            {
                lblTimeToCompletion.Text = CharacterMonitor.TimeSpanDescriptive(m_estimatedCompletion);
            }
            else
            {
                lblTimeToCompletion.Hide();
            }
        }

        /// <summary>
        /// Timer to update skill in training completion time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeRemainingLabel();
        }
    }
}
