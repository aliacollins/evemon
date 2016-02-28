using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.NotificationWindow;

namespace EVEMon.SkillPlanner
{
    public partial class SkillBrowserControl : UserControl
    {
        private Skill m_selectedSkill;
        private Plan m_plan;


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SkillBrowserControl()
        {
            InitializeComponent();
            verticalSplitContainer.RememberDistanceKey = "SkillBrowser_Vertical";
        }


        #endregion


        #region Inherited Events

        /// <summary>
        /// On load.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            base.OnLoad(e);

            lblSkillName.Font = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold);

            // Reposition the help text along side the treeview
            Control[] result = skillSelectControl.Controls.Find("pnlResults", true);
            if (result.Length > 0)
                lblHelp.Location = new Point(lblHelp.Location.X, result[0].Location.Y);

            skillTreeDisplay.SkillClicked += skillTreeDisplay_SkillClicked;

            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
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
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            Disposed -= OnDisposed;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the plan this control is bound to.
        /// </summary>
        [Browsable(false)]
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
        [Browsable(false)]
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
                SetPlanEditorSkillSelectorSelectedSkill(value);
                UpdateContent();
            }
        }

        #endregion


        #region Content update

        /// <summary>
        /// Updates the skill browser.
        /// </summary>
        internal void UpdateSkillBrowser()
        {
            skillSelectControl.UpdateContent();
            UpdateContent();
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

            planToMenu.Enabled = false;

            // Toolbar > Planned to... dropdown menu
            for (int i = 0; i <= 5; i++)
            {
                planToMenu.Enabled |= m_plan.UpdatesRegularPlanToMenu(planToMenu.DropDownItems[i], m_selectedSkill, i);
            }

            // Toolbar > "Planned to" label
            int level = m_plan.GetPlannedLevel(m_selectedSkill);
            planToMenu.Text = $"Planned To {(level == 0 ? "(none)" : $"Level {Skill.GetRomanFromInt(level)}")}...";
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
            lblSkillName.Text = $"{m_selectedSkill.Name} ({m_selectedSkill.Rank})";
            lblSkillCost.Text = $"{m_selectedSkill.FormattedCost} ISK";
            descriptionTextBox.Text = m_selectedSkill.Description;

            if (!m_selectedSkill.IsPublic)
                descriptionTextBox.Text += @" ** THIS IS A NON-PUBLIC SKILL **";

            lblAttributes.Text = $"Primary: {m_selectedSkill.PrimaryAttribute}, " +
                                 $"Secondary: {m_selectedSkill.SecondaryAttribute} " +
                                 $"(SP/Hour: {m_selectedSkill.SkillPointsPerHour:N0})";

            // Training time per level
            UpdateLevelLabel(lblLevel1Time, 1);
            UpdateLevelLabel(lblLevel2Time, 2);
            UpdateLevelLabel(lblLevel3Time, 3);
            UpdateLevelLabel(lblLevel4Time, 4);
            UpdateLevelLabel(lblLevel5Time, 5);

            // Update "owned" checkbox
            if (m_selectedSkill.IsKnown)
            {
                ownsBookToolStripButton.Checked = false;
                ownsBookToolStripButton.Enabled = false;
            }
            else
            {
                ownsBookToolStripButton.Checked = m_selectedSkill.IsOwned;
                ownsBookToolStripButton.Enabled = true;
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
        private void UpdateLevelLabel(Control label, int level)
        {
            StringBuilder sb = new StringBuilder();

            // "Level III: "
            sb.Append($"Level {Skill.GetRomanFromInt(level)}: ");

            // Is it already trained ?
            if (m_selectedSkill.Level >= level)
            {
                label.Text = sb.Append("Already trained").ToString();
                return;
            }

            // Left training time for level only
            TimeSpan timeOfLevelOnly = m_selectedSkill.GetLeftTrainingTimeForLevelOnly(level);
            sb.Append(timeOfLevelOnly.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas));

            // Total training time or completion percentage
            TimeSpan timeForPrereqs = m_selectedSkill.Character.GetTrainingTimeToMultipleSkills(m_selectedSkill.Prerequisites);
            TimeSpan totalPrereqTime = m_selectedSkill.GetLeftTrainingTimeToLevel(level - 1).Add(timeForPrereqs);
            if (totalPrereqTime > TimeSpan.Zero)
            {
                sb.Append($" (plus {totalPrereqTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)})");
            }
            else
            {
                // Completion percentage
                if (m_selectedSkill.Level != 5)
                {
                    float percentDone = m_selectedSkill.FractionCompleted;
                    sb.Append($" ({percentDone:P0} complete)");
                }
            }

            label.Text = sb.ToString();
        }

        /// <summary>
        /// Sets the plan editor's skill selector selected skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        private void SetPlanEditorSkillSelectorSelectedSkill(Skill skill)
        {
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (planWindow == null || planWindow.IsDisposed)
                return;

            planWindow.SetPlanEditorSkillSelectorSelectedSkill(skill);
        }

        /// <summary>
        /// Show the given skill in the skill explorer.
        /// </summary>
        /// <param name="skill"></param>
        public void ShowSkillInExplorer(Skill skill)
        {
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (planWindow == null || planWindow.IsDisposed)
                return;

            SkillExplorerWindow skillExplorerWindow = WindowsFactory.ShowByTag<SkillExplorerWindow, PlanWindow>(planWindow);
            skillExplorerWindow.PlanWindow = planWindow;
            skillExplorerWindow.Skill = skill;
        }

        /// <summary>
        /// Updates the owned skill book controls.
        /// </summary>
        private void UpdateOwnedSkillBookControls()
        {
            // Set button check state according to skills 'owned' property;
            // this will also trigger a check through the character's assets
            ownsBookToolStripButton.Checked = m_selectedSkill.IsOwned |
                                              (m_selectedSkill.HasBookInAssets && !m_selectedSkill.IsKnown);

            skillSelectControl.UpdateContent();

            // Update also the skill selector of the Plan Editor
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (planWindow != null && !planWindow.IsDisposed)
                planWindow.UpdatePlanEditorSkillSelection();

            // Update the Owned Skill books window if open
            OwnedSkillBooksWindow ownedSkillBooksWindow =
                WindowsFactory.GetByTag<OwnedSkillBooksWindow, Character>((Character)m_plan.Character);

            if (ownedSkillBooksWindow == null || ownedSkillBooksWindow.IsDisposed)
                return;

            ownedSkillBooksWindow.UpdateList();
        }

        #endregion


        #region Global events

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
        /// Occurs whenever the settings changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_plan.Character || m_selectedSkill == null)
                return;

            // Update the 'Owns book' indicator 
            // if the indicator is not already set
            // This prevents the update on repeated requests from IGB
            if (ownsBookToolStripButton.Checked != m_selectedSkill.IsOwned)
                UpdateOwnedSkillBookControls();
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
        private void ownsBookToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            // Set skill's 'owned' property according to button check state
            m_selectedSkill.IsOwned = ownsBookToolStripButton.Checked;

            UpdateOwnedSkillBookControls();
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
                for (int i = 0; i <= 5; i++)
                {
                    m_plan.UpdatesRegularPlanToMenu(cmsSkillContext.Items[i], e.Skill, i);
                }

                Cursor = Cursors.Default;
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
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);
        }

        #endregion
    }
}