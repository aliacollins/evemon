using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class SkillBrowser : UserControl
    {
        public SkillBrowser()
        {
            InitializeComponent();
            this.splitContainer1.RememberDistanceKey = "SkillBrowserInfo";
            this.splitContainer2.RememberDistanceKey = "SkillBrowser";
        }

        public CharacterInfo GrandCharacterInfo
        {
            set
            {
                skillSelectControl.GrandCharacterInfo = value;
            }
        }

        private Plan m_plan = null;
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan != null)
                {
                    // Unsubscribe to existing Plan.Changed event
                    m_plan.Changed -= new EventHandler<EventArgs>(PlanChanged);
                }
                m_plan = value;
                if (m_plan != null)
                {
                    // Subscribe to new plan's changed event
                    m_plan.Changed += new EventHandler<EventArgs>(PlanChanged);
                }
                skillTreeDisplay.Plan = value;
                skillSelectControl.Plan = value;
            }
        }

        private void PlanChanged(object sender, EventArgs e)
        {
            UpdatePlanSelect();
        }

        public bool WorksafeMode
        {
            set
            {
                skillTreeDisplay.WorksafeMode = value;
            }
        }

        Skill m_selectedSkill;
        public Skill SelectedSkill
        {
            get { return m_selectedSkill; }
            set
            {
                m_selectedSkill = value;
                skillTreeDisplay.RootSkill = value;
                UpdatePlanControl();
            }
        }

        /// <summary>
        /// Internal helper class for the PlanTo combo box
        /// </summary>
        private class PlanToComboBoxItem
        {
            public int Level;
            public PlanToComboBoxItem(int _level)
            {
                Level = _level;
            }
            public override string ToString()
            {
                if (Level == 0) { return "Not planned"; }
                else { return "Level " + Skill.GetRomanForInt(Level); }
            }
        }

        /// <summary>
        /// Updates the Plan To combo box
        /// </summary>
        private void UpdatePlanSelect()
        {
            if (m_selectedSkill != null)
            {
                // Remove the event handler while the control is updated
                cbPlanSelect.SelectedIndexChanged -= new EventHandler(cbPlanSelect_SelectedIndexChanged);
                // Start the update
                cbPlanSelect.BeginUpdate();
                cbPlanSelect.Items.Clear();
                // Is the skill plannable (ie level < 5)
                if (m_selectedSkill.Level < 5)
                {
                    // Get current level
                    int currentLevel = m_selectedSkill.Level;
                    // Get planned level
                    int plannedTo = m_plan.PlannedLevel(m_selectedSkill);
                    int selectedIndex = 0;
                    // Add plannable items
                    cbPlanSelect.Items.Add(new PlanToComboBoxItem(0));
                    for (int i = currentLevel + 1; i <= 5; i++)
                    {
                        cbPlanSelect.Items.Add(new PlanToComboBoxItem(i));
                        if (i == plannedTo) { selectedIndex = i - currentLevel; }
                    }
                    cbPlanSelect.SelectedIndex = selectedIndex;
                    cbPlanSelect.Enabled = true;
                }
                else
                {
                    // Skill is already level 5
                    cbPlanSelect.Items.Add("Not trainable");
                    cbPlanSelect.SelectedIndex = 0;
                    cbPlanSelect.Enabled = false;
                }
                // Complete the update
                cbPlanSelect.EndUpdate();
                // Apply the event handler
                cbPlanSelect.SelectedIndexChanged += new EventHandler(cbPlanSelect_SelectedIndexChanged);
            }
        }

        // react to changes in the 'Plan To' Combobox
        private void cbPlanSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlanTo(((PlanToComboBoxItem)cbPlanSelect.SelectedItem).Level);
        }

        public void UpdatePlanControl()
        {
            /// TODO: fix the update of the entire control
            //if (planEditor.Visible)
            //{
            //    UpdateStatusBar();
            //    return;
            //}

            if (m_selectedSkill == null)
            {
                pnlPlanControl.Visible = false;
                skillTreeDisplay.Visible = false;
                btnEnables.Visible = false;
            }
            else
            {
                lblSkillClass.Text = m_selectedSkill.SkillGroup.Name;
                lblSkillName.Text = m_selectedSkill.Name + " (" + m_selectedSkill.Rank + ")  " + m_selectedSkill.FormattedCost + " ISK";
                textboxDescription.Text = m_selectedSkill.Description;
                if (!m_selectedSkill.Public)
                {
                    textboxDescription.Text += " ** THIS IS A NON-PUBLIC SKILL **";
                }
                lblAttributes.Text = String.Format("Primary: {0}, Secondary: {1}", m_selectedSkill.PrimaryAttribute.ToString(),
                                                                                   m_selectedSkill.SecondaryAttribute.ToString());

                //int plannedTo = 0;
                bool anyPlan = false;
                bool tPlan;
                tPlan = SetPlanLabel(lblLevel1Time, 1);
                //if (tPlan)
                //plannedTo = 1;
                anyPlan = anyPlan || tPlan;
                tPlan = SetPlanLabel(lblLevel2Time, 2);
                //if (tPlan)
                //plannedTo = 2;
                anyPlan = anyPlan || tPlan;
                tPlan = SetPlanLabel(lblLevel3Time, 3);
                //if (tPlan)
                //plannedTo = 3;
                anyPlan = anyPlan || tPlan;
                tPlan = SetPlanLabel(lblLevel4Time, 4);
                //if (tPlan)
                //plannedTo = 4;
                anyPlan = anyPlan || tPlan;
                tPlan = SetPlanLabel(lblLevel5Time, 5);
                //if (tPlan)
                //plannedTo = 5;
                anyPlan = anyPlan || tPlan;
                //btnCancelPlan.Enabled = anyPlan;

                if (m_selectedSkill.Known)
                {
                    cbOwned.Checked = false;
                    cbOwned.Enabled = false;
                }
                else
                {
                    cbOwned.Checked = m_selectedSkill.Owned;
                    cbOwned.Enabled = true;
                }

                UpdatePlanSelect();

                //if (plannedTo > 0)
                //{
                //    lblPlanDescription.Text = "Currently planned to level " +
                //        GrandSkill.GetRomanForInt(plannedTo);
                //}
                //else
                //{
                //    lblPlanDescription.Text = "Not currently planned.";
                //}
                pnlPlanControl.Visible = true;
                skillTreeDisplay.Visible = true;
                btnEnables.Visible = true;
            }

            /// TODO fix the update of the entire control
            //Parent.UpdateStatusBar();
        }

        private bool SetPlanLabel(Label label, int level)
        {
            bool isPlanned = m_plan.IsPlanned(m_selectedSkill, level);
            StringBuilder sb = new StringBuilder();
            sb.Append("Level ");
            sb.Append(Skill.GetRomanForInt(level));
            sb.Append(": ");
            if (m_selectedSkill.Level >= level)
            {
                sb.Append("Already known");
                //button.Enabled = false;
            }
            else
            {
                TimeSpan tts = m_selectedSkill.GetTrainingTimeOfLevelOnly(level);
                if (m_selectedSkill.Level == level - 1)
                {
                    tts = m_selectedSkill.GetTrainingTimeToLevel(level);
                }
                sb.Append(Skill.TimeSpanToDescriptiveText(tts, DescriptiveTextOptions.IncludeCommas));

                TimeSpan prts = m_selectedSkill.GetTrainingTimeToLevel(level - 1) + m_selectedSkill.GetPrerequisiteTime();
                if (prts > TimeSpan.Zero)
                {
                    sb.Append(" (plus ");
                    sb.Append(Skill.TimeSpanToDescriptiveText(prts, DescriptiveTextOptions.IncludeCommas));
                    sb.Append(")");
                }
                else
                {
                    // we're displaying the next level to train - show %complete
                    if (m_selectedSkill.Level != 5)
                    {
                        double percentDone = m_selectedSkill.GetPercentDone();
                        sb.Append(String.Format(" ({0} complete)", percentDone.ToString("P0")));
                    }
                }
                //button.Enabled = !isPlanned;
                //if (m_selectedSkill.InTraining && m_selectedSkill.TrainingToLevel == level)
                //    button.Enabled = false;
            }
            label.Text = sb.ToString();
            return isPlanned;
        }

        private void tmrSkillTick_Tick(object sender, EventArgs e)
        {
            UpdatePlanControl();
        }

        private void skillSelectControl_SelectedSkillChanged(object sender, EventArgs e)
        {
            SelectedSkill = skillSelectControl.SelectedSkill;
        }

        // React to clicks in the Skill Tree panel
        private void skillTreeDisplay_SkillClicked(object sender, SkillClickedEventArgs e)
        {
            if (e.Skill == m_selectedSkill)
            {
                if (e.Button == MouseButtons.Right)
                {
                    bool isPlanned = false;
                    isPlanned = SetMenuItemState(miPlanTo1, e.Skill, 1) || isPlanned;
                    isPlanned = SetMenuItemState(miPlanTo2, e.Skill, 2) || isPlanned;
                    isPlanned = SetMenuItemState(miPlanTo3, e.Skill, 3) || isPlanned;
                    isPlanned = SetMenuItemState(miPlanTo4, e.Skill, 4) || isPlanned;
                    isPlanned = SetMenuItemState(miPlanTo5, e.Skill, 5) || isPlanned;
                    miCancelPlanMenu.Enabled = isPlanned;
                    cmsSkillContext.Show(skillTreeDisplay, e.Location);
                }
            }
        }

        private const DescriptiveTextOptions DTO_OPTS = DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.UppercaseText;

        private bool SetMenuItemState(ToolStripMenuItem mi, Skill gs, int level)
        {
            bool isKnown = false;
            bool isPlanned = false;
            bool isTraining = false;
            if (level <= gs.Level)
            {
                isKnown = true;
            }
            if (!isKnown)
            {
                if (gs.InTraining && gs.TrainingToLevel == level)
                {
                    isTraining = true;
                }
                else
                {
                    foreach (Plan.Entry pe in m_plan.Entries)
                    {
                        if (pe.Skill == gs && pe.Level == level)
                        {
                            isPlanned = true;
                            break;
                        }
                    }
                }
            }

            if (!isKnown && !isTraining)
            {
                if (isPlanned)
                {
                    mi.Text = "Plan to Level " + Skill.GetRomanForInt(level) + " (Already Planned)";
                    mi.Enabled = false;
                    return true;
                }
                else
                {
                    TimeSpan ts = gs.GetPrerequisiteTime() + gs.GetTrainingTimeToLevel(level);
                    mi.Text = "Plan to Level " + Skill.GetRomanForInt(level) + " (" +
                              Skill.TimeSpanToDescriptiveText(ts, DTO_OPTS) + ")";
                    mi.Enabled = true;
                }
            }
            else
            {
                if (isKnown)
                {
                    mi.Text = "Plan to Level " + Skill.GetRomanForInt(level) + " (Already Known)";
                    mi.Enabled = false;
                }
                else if (isTraining)
                {
                    mi.Text = "Plan to Level " + Skill.GetRomanForInt(level) + " (Currently Training)";
                    mi.Enabled = false;
                }
            }
            return false;
        }

        private void PlanTo(int level)
        {
            m_plan.PlanTo(m_selectedSkill, level, true);
            UpdatePlanControl();
        }

        private void cbOwned_CheckedChanged(object sender, EventArgs e)
        {
            m_selectedSkill.Owned = cbOwned.Checked;
            m_plan.GrandCharacterInfo.UpdateOwnedSkills();
            UpdatePlanControl();
        }

        #region Skill Enables...
        private void btnEnables_Click(object sender, EventArgs e)
        {
            ShowSkillInExplorer(m_selectedSkill);
        }

        public void ShowSkillInExplorer(Skill s)
        {
            if (m_skillEnablesForm == null)
            {
                SkillEnablesForm f = new SkillEnablesForm(s, this);
                m_skillEnablesForm = f;
                NewPlannerWindow npw = m_plan.PlannerWindow.Target as NewPlannerWindow;
                npw.SkillExplorer = f;
            }
            else
            {
                m_skillEnablesForm.SetSkill(s);
            }
            m_skillEnablesForm.Show();
        }

        private SkillEnablesForm m_skillEnablesForm = null;
        public SkillEnablesForm EnablesForm
        {
            set
            {
                m_skillEnablesForm = value;
                NewPlannerWindow npw = m_plan.PlannerWindow.Target as NewPlannerWindow;
                if (npw != null)
                    npw.SkillExplorer = value;
            }
        }

        #endregion
    }
}
