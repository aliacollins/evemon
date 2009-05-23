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
    public partial class TrayPopUpChar : UserControl
    {
        #region Fields
        private CharacterMonitor m_characterMon;
        private DateTime m_estimatedCompletion;
        private bool m_highlightConflict;
        private int[] m_portraitSize = { 16, 24, 32, 40, 48, 56, 64 };
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public TrayPopUpChar()
            : this(null)
        {
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="cm">A CharacterMonitor object for the character to be displayed</param>
        public TrayPopUpChar(CharacterMonitor cm)
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
                TrayPopupConfig config = s.TrayPopupConfig;
                // Form level look and feel
                this.ForeColor = charInfo.IsTraining ? SystemColors.ControlText : SystemColors.GrayText;
                this.Font = FontHelper.GetFont(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
                // Character portrait
                if (!s.WorksafeMode && config.ShowPortrait)
                {
                    if (m_characterMon.CharacterPortrait != null)
                    {
                        pbCharacterPortrait.Image = m_characterMon.CharacterPortrait;
                        pbCharacterPortrait.Size = new Size(m_portraitSize[(int)config.PortraitSize], m_portraitSize[(int)config.PortraitSize]);
                    }
                }
                else
                {
                    pbCharacterPortrait.Hide();
                }
                // Character Name
                lblCharName.Text = charInfo.Name;
                lblCharName.Font = FontHelper.GetFont(lblCharName.Font.Name, SystemFonts.MessageBoxFont.SizeInPoints * 11 / 9, FontStyle.Regular, GraphicsUnit.Point);
                // Skill Training Info
                if (charInfo.IsTraining)
                {
                    Skill trainingSkill = charInfo.CurrentlyTrainingSkill;
                    if (trainingSkill != null)
                    {
                        m_estimatedCompletion = trainingSkill.EstimatedCompletion;
                        // See if the end time conflicts with a schedule entry
                        string blockingEntry = string.Empty;
                        bool isBlocked = s.SkillIsBlockedAt(m_estimatedCompletion, out blockingEntry);
                        m_highlightConflict = isBlocked && config.HighlightConflicts;

                        if (config.ShowSkill)
                        {
                            lblSkillInTraining.Text = trainingSkill.Name + " " + Skill.GetRomanForInt(trainingSkill.TrainingToLevel);
                            if (m_highlightConflict) lblSkillInTraining.ForeColor = Color.Red;
                        }
                        else
                        {
                            lblSkillInTraining.Hide();
                        }
                        if (config.ShowTimeToCompletion)
                        {
                            UpdateTimeRemainingLabel();
                            updateTimer.Start();
                        }
                        else
                        {
                            lblTimeToCompletion.Hide();
                        }
                        if (config.ShowCompletionTime)
                        {
                            lblCompletionTime.Text = m_estimatedCompletion.ToString("ddd ") + m_estimatedCompletion.ToString();
                            if (m_highlightConflict) lblCompletionTime.ForeColor = Color.Red;
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
                if (config.ShowWallet)
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
                if (m_highlightConflict) lblTimeToCompletion.ForeColor = Color.Red;
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
