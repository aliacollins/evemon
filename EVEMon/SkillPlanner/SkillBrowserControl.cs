using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.SkillPlanner
{
    public partial class SkillBrowser : UserControl
    {
        private Skill m_selectedSkill;
        private Plan m_plan;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SkillBrowser()
        {
            InitializeComponent();
            verticalSplitContainer.RememberDistanceKey = "SkillBrowser_Vertical";
        }

        /// <summary>
        /// On load.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lblSkillName.Font = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold);

            // Reposition the help text along side the treeview
            Control[] result = skillSelectControl.Controls.Find("pnlResults", true);
            if (result.Length > 0)
                lblHelp.Location = new Point(lblHelp.Location.X, result[0].Location.Y);

            skillTreeDisplay.SkillClicked += skillTreeDisplay_SkillClicked;

            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            Disposed += OnDisposed;

            //Update the controls visibility
            UpdateControlVisibility();
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            skillTreeDisplay.SkillClicked -= skillTreeDisplay_SkillClicked;
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Gets or sets the plan this control is bound to.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                m_plan = value;
                skillTreeDisplay.Plan = value;
                skillSelectControl.Plan = value;
                UpdateContent();
            }
        }

        /// <summary>
        /// Gets or sets the selected skills.
        /// </summary>
        public Skill SelectedSkill
        {
            get { return m_selectedSkill; }
            set
            {
                if (m_selectedSkill == value)
                    return;

                m_selectedSkill = value;
                skillTreeDisplay.RootSkill = value;
                skillSelectControl.SelectedSkill = value;
                UpdateContent();
            }
        }

        /// <summary>
        /// Show the given skill in the skill explorer.
        /// </summary>
        /// <param name="skill"></param>
        public void ShowSkillInExplorer(Skill skill)
        {
            PlanWindow planWindow = new WindowsFactory<PlanWindow>().GetByTag(m_plan);
            SkillExplorerWindow skillExplorer = new WindowsFactory<SkillExplorerWindow>().ShowByTag(
                planWindow, window => new SkillExplorerWindow(skill, window));
            skillExplorer.Skill = skill;
        }


        #region Content update

        /// <summary>
        /// Occurs whenever a plan is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            UpdatePlannedLevel();
        }

        /// <summary>
        /// Occurs whenever the settings changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();
        }

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            if (!Settings.UI.SafeForWork)
            {
                planToMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                showSkillExplorerMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
            else
            {
                planToMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
                showSkillExplorerMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            }
        }

        /// <summary>
        /// Updates the combo on the top right, to change the planned level.
        /// </summary>
        private void UpdatePlannedLevel()
        {
            // Not visible
            if (m_selectedSkill == null)
                return;

            // Toolbar > Planned to... dropdown menu
            bool enabled = false;
            enabled |= PlanHelper.UpdatesRegularPlanToMenu(planTo0Menu, m_plan, m_selectedSkill, 0);
            enabled |= PlanHelper.UpdatesRegularPlanToMenu(planTo1Menu, m_plan, m_selectedSkill, 1);
            enabled |= PlanHelper.UpdatesRegularPlanToMenu(planTo2Menu, m_plan, m_selectedSkill, 2);
            enabled |= PlanHelper.UpdatesRegularPlanToMenu(planTo3Menu, m_plan, m_selectedSkill, 3);
            enabled |= PlanHelper.UpdatesRegularPlanToMenu(planTo4Menu, m_plan, m_selectedSkill, 4);
            enabled |= PlanHelper.UpdatesRegularPlanToMenu(planTo5Menu, m_plan, m_selectedSkill, 5);
            planToMenu.Enabled = enabled;

            // Toolbar > "Planned to" label
            int level = m_plan.GetPlannedLevel(m_selectedSkill);

            planToMenu.Text = (level == 0
                                   ? "Plan To (none)..."
                                   : String.Format(CultureConstants.DefaultCulture, "Plan To Level {0}...",
                                                   Skill.GetRomanFromInt(level)));
        }

        /// <summary>
        /// Updates the browser's content.
        /// </summary>
        private void UpdateContent()
        {
            if (m_selectedSkill == null)
            {
                // View help message
                lblHelp.Visible = true;

                rightPanel.Visible = false;
                return;
            }

            // Hide help message
            lblHelp.Visible = false;

            // Updates controls visibility
            rightPanel.Visible = true;

            // Updates the main labels
            lblSkillClass.Text = m_selectedSkill.Group.Name;
            lblSkillName.Text = String.Format(CultureConstants.DefaultCulture, "{0} ({1})",
                                              m_selectedSkill.Name,
                                              m_selectedSkill.Rank);
            lblSkillCost.Text = String.Format(CultureConstants.DefaultCulture, "{0} ISK", m_selectedSkill.FormattedCost);
            descriptionTextBox.Text = m_selectedSkill.Description;
            if (!m_selectedSkill.IsPublic)
                descriptionTextBox.Text += " ** THIS IS A NON-PUBLIC SKILL **";

            lblAttributes.Text = String.Format(CultureConstants.DefaultCulture,
                                               "Primary: {0}, Secondary: {1} (SP/Hour: {2:#,##0})",
                                               m_selectedSkill.PrimaryAttribute.ToString(),
                                               m_selectedSkill.SecondaryAttribute.ToString(),
                                               m_selectedSkill.SkillPointsPerHour);
            // Training time per level
            UpdateLevelLabel(lblLevel1Time, 1);
            UpdateLevelLabel(lblLevel2Time, 2);
            UpdateLevelLabel(lblLevel3Time, 3);
            UpdateLevelLabel(lblLevel4Time, 4);
            UpdateLevelLabel(lblLevel5Time, 5);

            // Update "owned" checkbox
            if (m_selectedSkill.IsKnown)
            {
                ownsBookMenu.Checked = false;
                ownsBookMenu.Enabled = false;
            }
            else
            {
                ownsBookMenu.Checked = m_selectedSkill.IsOwned;
                ownsBookMenu.Enabled = true;
            }

            // Update "planned level" combo (on the top left)
            UpdatePlannedLevel();

            // Enable refresh every 30s if the selected skill is in training
            tmrTrainingSkillTick.Enabled = m_selectedSkill.IsTraining;
        }

        /// <summary>
        /// Updates the provided label with the training time to the given level.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="level"></param>
        private void UpdateLevelLabel(Label label, int level)
        {
            // "Level III :"
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(CultureConstants.DefaultCulture, "Level {0}: ", Skill.GetRomanFromInt(level));

            // Is it already trained ?
            if (m_selectedSkill.Level >= level)
            {
                label.Text = sb.Append("Already trained").ToString();
                return;
            }

            // Left training time for level only
            TimeSpan timeOfLevelOnly = m_selectedSkill.GetLeftTrainingTimeForLevelOnly(level);
            sb.Append(timeOfLevelOnly.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, false));

            // Total training time or completion percentage
            TimeSpan timeForPrereqs = m_selectedSkill.Character.GetTrainingTimeToMultipleSkills(m_selectedSkill.Prerequisites);
            TimeSpan totalPrereqTime = m_selectedSkill.GetLeftTrainingTimeToLevel(level - 1).Add(timeForPrereqs);
            if (totalPrereqTime > TimeSpan.Zero)
            {
                sb.AppendFormat(CultureConstants.DefaultCulture, " (plus {0})",
                                totalPrereqTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas));
            }
            else
            {
                // Completion percentage
                if (m_selectedSkill.Level != 5)
                {
                    float percentDone = m_selectedSkill.FractionCompleted;
                    sb.AppendFormat(CultureConstants.DefaultCulture, " ({0} complete)", percentDone.ToString("P0"));
                }
            }

            label.Text = sb.ToString();
        }

        #endregion


        #region Controls events handlers

        /// <summary>
        /// Every 30s, we update the time for the training skill.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrTrainingSkill_Tick(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// Whenever the selection changes, we update the selected skill.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skillSelectControl_SelectedSkillChanged(object sender, EventArgs e)
        {
            SelectedSkill = skillSelectControl.SelectedSkill;
        }

        /// <summary>
        /// Toolbar > Owns book.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ownsBookMenu_Click(object sender, EventArgs e)
        {
            m_selectedSkill.IsOwned = ownsBookMenu.Checked;
            skillSelectControl.UpdateContent();
        }

        /// <summary>
        /// Toolbar > Show what this skills enables...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showSkillExplorerMenu_Click(object sender, EventArgs e)
        {
            ShowSkillInExplorer(m_selectedSkill);
        }

        #endregion


        #region Skill tree's context menu

        /// <summary>
        /// Whenever the user right-click the skill tree on the left, we display the context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skillTreeDisplay_SkillClicked(object sender, SkillClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PlanHelper.UpdatesRegularPlanToMenu(miPlanTo0, m_plan, e.Skill, 0);
                PlanHelper.UpdatesRegularPlanToMenu(miPlanTo1, m_plan, e.Skill, 1);
                PlanHelper.UpdatesRegularPlanToMenu(miPlanTo2, m_plan, e.Skill, 2);
                PlanHelper.UpdatesRegularPlanToMenu(miPlanTo3, m_plan, e.Skill, 3);
                PlanHelper.UpdatesRegularPlanToMenu(miPlanTo4, m_plan, e.Skill, 4);
                PlanHelper.UpdatesRegularPlanToMenu(miPlanTo5, m_plan, e.Skill, 5);
                cmsSkillContext.Show(skillTreeDisplay, e.Location);
                return;
            }

            SelectedSkill = e.Skill;
        }

        /// <summary>
        /// Context menu > Plan to N / Remove.
        /// Toolbar > Plan to... > Level N / Remove.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void planToMenu_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = ((ToolStripMenuItem)sender).Tag as IPlanOperation;
            PlanHelper.SelectPerform(operation);
        }

        #endregion
    }
}