using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class PlanSortWindow : EVEMonForm
    {
        public PlanSortWindow()
        {
            InitializeComponent();
        }

        private void PlanSortWindow_Load(object sender, EventArgs e)
        {
            cbSortType.BeginUpdate();
            try
            {
                cbSortType.Items.Clear();
                int count = Enum.GetValues(typeof (PlanSortType)).Length;
                for (int i = 0; i < count; i++)
                {
                    string txt = String.Empty;
                    PlanSortDescriptionAttribute descAttr =
                        EnumAttributeReader<PlanSortType, PlanSortDescriptionAttribute>.GetAttribute((PlanSortType) i);
                    if (descAttr != null)
                    {
                        txt = descAttr.DisplayText;
                    }
                    else
                    {
                        txt = ((PlanSortType) i).ToString();
                    }
                    cbSortType.Items.Add(txt);
                }
            }
            finally
            {
                cbSortType.EndUpdate();
            }
            cbSortType.SelectedIndex = (int) PlanSortType.FastestFirst;
        }

        private void cbSortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlanSortDescriptionAttribute descAttr =
                EnumAttributeReader<PlanSortType, PlanSortDescriptionAttribute>.GetAttribute(
                    (PlanSortType) cbSortType.SelectedIndex);
            if (descAttr != null)
            {
                String txt = String.Empty;
                lblDescription.Text = txt + descAttr.Description;
            }
            else
            {
                lblDescription.Text = String.Empty;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private PlanSortType m_resultSortType;
        private bool m_resultLearningFirst;
        private bool m_resultUsePriority;

        public PlanSortType SortType
        {
            get { return m_resultSortType; }
        }

        public bool LearningFirst
        {
            get { return m_resultLearningFirst; }
        }

        public bool Priority
        {
            get { return m_resultUsePriority; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_resultSortType = (PlanSortType) cbSortType.SelectedIndex;
            m_resultLearningFirst = cbArrangeLearning.Checked;
            m_resultUsePriority = cbUsePriority.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public enum PlanSortType
    {
        [PlanSortDescription("No Sorting", "Do not sort skills in the plan.")] NoChange,
        [
            PlanSortDescription("Fastest Skills First",
                "Skills with the shortest remaining training time will be ordered first.")] FastestFirst,
        [
            PlanSortDescription("Fastest Group First",
                "Skill groups with the shortest remaining training time will be ordered first.")] FastestGroupFirst,
        [PlanSortDescription("Slowest Skills First",
                "Skills with the longest remaining training time will be ordered first. "
                + "This minimizes user interaction during the beginning of the plan.")] SlowestFirst
    }

    public class PlanSortDescriptionAttribute : Attribute
    {
        private string m_displayText = String.Empty;
        private string m_description = String.Empty;

        public string DisplayText
        {
            get { return m_displayText; }
            set { m_displayText = value; }
        }

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public PlanSortDescriptionAttribute(string displayText, string description)
        {
            m_displayText = displayText;
            m_description = description;
        }
    }

    public delegate bool CompareDelegate<T>(T arg1, T arg2);


    public class PlanSorter
    {
        private static Pair<string, int>[] sm_idealLearningSkillOrder;

        static PlanSorter()
        {
            // Order borrowed with slight modification from
            // http://myeve.eve-online.com/ingameboard.asp?a=topic&threadID=242786&page=1#2

            // ... and corrected to reflect Revelation

            List<Pair<string, int>> idealBuilder = new List<Pair<string, int>>();
            for (int i = 0; i < 3; i++)
            {
                idealBuilder.Add(new Pair<string, int>("Instant Recall", i + 1));
                idealBuilder.Add(new Pair<string, int>("Analytical Mind", i + 1));
                idealBuilder.Add(new Pair<string, int>("Learning", i + 1));
            }
            idealBuilder.Add(new Pair<string, int>("Instant Recall", 4));
            for (int i = 0; i < 3; i++)
            {
                idealBuilder.Add(new Pair<string, int>("Eidetic Memory", i + 1));
            }
            idealBuilder.Add(new Pair<string, int>("Analytical Mind", 4));
            for (int i = 0; i < 3; i++)
            {
                idealBuilder.Add(new Pair<string, int>("Logic", i + 1));
            }
            idealBuilder.Add(new Pair<string, int>("Learning", 4));

            idealBuilder.Add(new Pair<string, int>("Eidetic Memory", 4));
            idealBuilder.Add(new Pair<string, int>("Logic", 4));
            idealBuilder.Add(new Pair<string, int>("Instant Recall", 5));
            idealBuilder.Add(new Pair<string, int>("Analytical Mind", 5));
            idealBuilder.Add(new Pair<string, int>("Learning", 5));
            idealBuilder.Add(new Pair<string, int>("Eidetic Memory", 5));
            idealBuilder.Add(new Pair<string, int>("Logic", 5));


            for (int i = 0; i < 5; i++)
            {
                idealBuilder.Add(new Pair<string, int>("Spatial Awareness", i + 1));
                idealBuilder.Add(new Pair<string, int>("Iron Will", i + 1));
                idealBuilder.Add(new Pair<string, int>("Empathy", i + 1));
            }
            for (int i = 0; i < 5; i++)
            {
                idealBuilder.Add(new Pair<string, int>("Focus", i + 1));
                idealBuilder.Add(new Pair<string, int>("Clarity", i + 1));
                idealBuilder.Add(new Pair<string, int>("Presence", i + 1));
            }
            sm_idealLearningSkillOrder = idealBuilder.ToArray();
        }

        public static void SortPlan(Plan p, PlanSortType sortType, bool learningFirst, bool usePriority)
        {
            PlanSorter ps = new PlanSorter(p, sortType, learningFirst,usePriority);
            ps.Sort();
        }

        private Plan m_plan;
        private PlanSortType m_sortType;
        private bool m_learningFirst;
        private bool m_usePriority;

        private PlanSorter(Plan p, PlanSortType sortType, bool learningFirst, bool usePriority)
        {
            m_plan = p;
            m_sortType = sortType;
            m_learningFirst = learningFirst;
            m_usePriority = usePriority;
        }

        private List<Plan.Entry> m_originalOrder = new List<Plan.Entry>();
        private List<Plan.Entry> m_skillsToInsert = new List<Plan.Entry>();

        private void Sort()
        {
            foreach (Plan.Entry pe in m_plan.Entries)
            {
                Plan.Entry xpe = (Plan.Entry) pe.Clone();
                m_originalOrder.Add(xpe);
                m_skillsToInsert.Add(xpe);
            }
            m_plan.SuppressEvents();
            try
            {
                m_plan.Entries.Clear();
                if (m_learningFirst)
                {
                    ArrangeLearningSkills();
                }
                m_trainedLevels = new Dictionary<string, int>();
                if (m_usePriority)
                {
                    List<Plan.Entry> skillsLeft = new List<Plan.Entry>();
                    foreach (Plan.Entry pe in m_skillsToInsert)
                    {
                        skillsLeft.Add(pe);
                    }
                    while (skillsLeft.Count > 0)
                    {
                        // Find the next highest priority left to insert
                        int highestPriority = Int32.MaxValue;
                        foreach (Plan.Entry pe1 in skillsLeft)
                        {
                            if (pe1.Priority < highestPriority)
                            {
                                highestPriority = pe1.Priority;
                            }
                        }
                        // and build a list of those skills at that priority
                        m_skillsToInsert.Clear();
                        for (int i = 0; i < skillsLeft.Count; i++ )
                        {
                            if (skillsLeft[i].Priority == highestPriority)
                            {
                                m_skillsToInsert.Add(skillsLeft[i]);
                            }
                        }
                        // and sort this priority group
                        ArrangeInSortTypeOrder();

                        // Finally, remove any entries that were just added to the plan
                        foreach (Plan.Entry npe in m_plan.Entries)
                        {
                            for (int i = 0; i < skillsLeft.Count; i++)
                            {
                                if (skillsLeft[i].SkillName == npe.SkillName && skillsLeft[i].Level == npe.Level)
                                {
                                    skillsLeft.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                }
                else  // not sorting by priority
                {
                    ArrangeInSortTypeOrder();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                m_plan.Entries.Clear();
                foreach (Plan.Entry xpe in m_originalOrder)
                {
                    Plan.Entry pe = (Plan.Entry) xpe.Clone();
                    m_plan.Entries.Add(pe);
                }
                MessageBox.Show("Sorting failed: " + ex.Message, "Sorting Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                m_plan.ResumeEvents();
            }
        }

        private void ArrangeInSortTypeOrder()
        {
            switch (m_sortType)
            {
                case PlanSortType.NoChange:
                    while (m_skillsToInsert.Count > 0)
                    {
                        Plan.Entry pe = (Plan.Entry) m_skillsToInsert[0].Clone();
                        m_plan.Entries.Add(pe);
                        m_skillsToInsert.RemoveAt(0);
                    }
                    break;
                case PlanSortType.FastestFirst:
                    ArrangeFastestFirst(
                        delegate(TimeSpan arg1, TimeSpan arg2) { return arg1 < arg2; },
                        TimeSpan.MaxValue);
                    break;
                case PlanSortType.FastestGroupFirst:
                    ArrangeFastestGroupFirst();
                    break;
                case PlanSortType.SlowestFirst:
                    ArrangeFastestFirst(
                        delegate(TimeSpan arg1, TimeSpan arg2) { return arg1 > arg2; },
                        TimeSpan.MinValue);
                    break;
            }
        }

        Dictionary<string, int> m_trainedLevels = new Dictionary<string, int>();

        private void ArrangeFastestFirst(
            CompareDelegate<TimeSpan> comparer,
            TimeSpan initialValue)
        {
            int cumulativeSkillTotal = m_plan.GrandCharacterInfo.SkillPointTotal;

            while (m_skillsToInsert.Count > 0)
            {
                TimeSpan fastestSpan = initialValue;
                Plan.Entry fastestPe = null;
                // find the fastest skill that already has it's prereqs added to the plan
                for (int i = 0; i < m_skillsToInsert.Count; i++)
                {
                    Plan.Entry thisPe = m_skillsToInsert[i];
                    Skill thisSkill = m_plan.GrandCharacterInfo.GetSkill(thisPe.SkillName);
                    TimeSpan thisSpan = thisSkill.GetTrainingTimeOfLevelOnly(thisPe.Level, cumulativeSkillTotal, true);
                    cumulativeSkillTotal += thisSkill.GetPointsForLevelOnly(thisPe.Level, true);
                    if (comparer(thisSpan, fastestSpan))
                    {
                        // this is potentially the fastest skill...
                        bool canInsert =
                            thisPe.Level == 1 // this is a new skill
                            || thisPe.Level <= thisSkill.Level + 1 // previous level is trained
                            || m_trainedLevels.ContainsKey(thisPe.SkillName) // previous level is already planned
                                && thisPe.Level <= m_trainedLevels[thisPe.SkillName] + 1;

                        if (!canInsert)
                            continue; //sanity check failed - try the next one

                        // but do we have the prereqs already added to the sorted plan?
                        foreach (Skill.Prereq pp in thisSkill.Prereqs)
                        {
                            if (pp.Skill.Level < pp.Level)
                            {
                                if (!m_trainedLevels.ContainsKey(pp.Name) ||
                                    m_trainedLevels[pp.Name] < pp.Level)
                                {
                                    // prereq not yet added to sorted plan so this isn't a candidate
                                    canInsert = false;
                                    break;
                                }
                            }
                        }
                        // We have the prereqs inserted for the sorted plan so this is a candidate
                        if (canInsert)
                        {
                            fastestSpan = thisSpan;
                            fastestPe = thisPe;
                        }
                    }
                }
                if (fastestPe == null)
                {
                    throw new ApplicationException("no suitable skill -- should never happen! (Possibly bad: " + m_skillsToInsert[0].SkillName + ")");
                }

                m_plan.Entries.Add((Plan.Entry) fastestPe.Clone());
                m_skillsToInsert.Remove(fastestPe);
                m_trainedLevels[fastestPe.SkillName] = fastestPe.Level;
            }
        }

        private void ArrangeFastestGroupFirst()
        {
            Dictionary<string, Plan> planGroups = new Dictionary<string, Plan>();

            foreach (Plan.Entry pe in m_skillsToInsert)
            {
                foreach (string pg in pe.PlanGroups)
                {
                    if (planGroups.ContainsKey(pg))
                    {
                        planGroups[pg].Entries.Add((Plan.Entry)pe.Clone());
                    }
                    else
                    {
                        Plan plan = new Plan();
                        plan.GrandCharacterInfo = m_plan.GrandCharacterInfo;
                        plan.Entries.Add((Plan.Entry)pe.Clone());
                        planGroups.Add(pg, plan);
                    }
                }
            }

            while (planGroups.Count > 0)
            {
                string shortestPlanGroup = "";
                TimeSpan shortestTimeSpan = TimeSpan.MaxValue;

                foreach (string pg in planGroups.Keys)
                {
                    if (planGroups[pg].TotalTrainingTime < shortestTimeSpan)
                    {
                        shortestTimeSpan = planGroups[pg].TotalTrainingTime;
                        shortestPlanGroup = pg;
                    }
                }

                foreach (Plan.Entry pe in planGroups[shortestPlanGroup].Entries)
                {
                    Plan.Entry _pe = null;

                    foreach (Plan p in planGroups.Values)
                    {
                        if (p != planGroups[shortestPlanGroup] &&
                            (_pe = p.GetEntry(pe.SkillName, pe.Level)) != null)
                        {
                            p.RemoveEntry(_pe);
                        }
                    }

                    if (m_plan.GetEntry(pe.SkillName, pe.Level) == null)
                    {
                        m_plan.Entries.Add((Plan.Entry)pe.Clone());
                    }
                }
                planGroups.Remove(shortestPlanGroup);
            }

            foreach (Plan.Entry pe in m_skillsToInsert)
            {
                if (m_plan.GetEntry(pe.SkillName, pe.Level) == null)
                {
                    m_plan.Entries.Add((Plan.Entry)pe.Clone());
                }
            }
        }

        private void ArrangeLearningSkills()
        {
            for (int i = 0; i < sm_idealLearningSkillOrder.Length; i++)
            {
                string skillname = sm_idealLearningSkillOrder[i].A;
                int level = sm_idealLearningSkillOrder[i].B;
                for (int z = 0; z < m_skillsToInsert.Count; z++)
                {
                    if (m_skillsToInsert[z].SkillName == skillname &&
                        m_skillsToInsert[z].Level == level)
                    {
                        Plan.Entry pe = (Plan.Entry) m_skillsToInsert[z].Clone();
                        m_plan.Entries.Add(pe);
                        m_skillsToInsert.RemoveAt(z);
                        break;
                    }
                }
            }
        }
    }
}
