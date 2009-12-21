using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class SuggestionWindow : EVEMonForm
    {
        public SuggestionWindow()
        {
            InitializeComponent();
        }

        public SuggestionWindow(Plan p)
            : this()
        {
            m_plan = p;
        }

        private Plan m_plan;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SuggestionWindow_Load(object sender, EventArgs e)
        {
            IEnumerable<Plan.Entry> entries = m_plan.GetSuggestions();

            CharacterInfo gci = m_plan.GrandCharacterInfo;
            EveAttributeScratchpad scratchpad = new EveAttributeScratchpad();
            TimeSpan postTime = TimeSpan.Zero;

            lbSkills.Items.Clear();

            // calculate the most optimal time for the learning
            // suggestion by copying the plan and sorting it with learning skills
            // first - This method gives an accurate savings time.
            Plan optimalPlan = new Plan();
            optimalPlan.GrandCharacterInfo = gci;
            foreach (Plan.Entry pe in m_plan.Entries)
            {
                optimalPlan.Entries.Add(pe.Clone() as Plan.Entry);
            }


            foreach (Plan.Entry pe in entries)
            {
                optimalPlan.Entries.Add(pe);
                lbSkills.Items.Add(pe.SkillName + " " +
                                   Skill.GetRomanForInt(pe.Level));
            }

            // Put the learning skills on top
            PlanSorter.PutOrderedLearningSkillsAhead(optimalPlan, m_plan.SortWithPrioritiesGrouping);
            // And get the time for the plan with learning skills.
            postTime = optimalPlan.TotalTrainingTime;

            // and throw away the test plan
            optimalPlan = null;
            
            TimeSpan preTime = m_plan.GetTotalTime(null, true);
            TimeSpan diff = preTime - postTime;

            lblBeforeTime.Text = Skill.TimeSpanToDescriptiveText(preTime, DTO_OPTS);
            lblAfterTime.Text = Skill.TimeSpanToDescriptiveText(postTime, DTO_OPTS);

            lblDiffTime.Text = Skill.TimeSpanToDescriptiveText(diff, DTO_OPTS);
        }

        private const DescriptiveTextOptions DTO_OPTS = DescriptiveTextOptions.IncludeCommas;
    }
}