using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Schedule;
using System.Drawing;
using System.Collections.Specialized;
using System.Collections;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Provides a way for implant calculator and attributes optimization form to add a column showning the training time difference.
    /// </summary>
    public interface IPlanOrderPluggable
    {
        event EventHandler Disposed;
        bool UseRemappingPointsForOld { get; }
        bool UseRemappingPointsForNew { get; }
        EveAttributeScratchpad GetScratchpad(out bool isNew);
    }

    public partial class PlanOrderEditorControl : UserControl
    {
        private System.Drawing.Font m_plannedSkillFont;
        private System.Drawing.Font m_prerequisiteSkillFont;
        private System.Drawing.Color m_trainablePlanEntryColor;
        private System.Drawing.Color m_remappingBackColor;
        private System.Drawing.Color m_remappingForeColor;
        private Settings m_settings;

        public PlanOrderEditorControl()
        {
            InitializeComponent();
            m_plannedSkillFont = new System.Drawing.Font(lvSkills.Font, System.Drawing.FontStyle.Bold);
            m_prerequisiteSkillFont = new System.Drawing.Font(lvSkills.Font, System.Drawing.FontStyle.Regular);
            m_trainablePlanEntryColor = SystemColors.GrayText;
            m_remappingBackColor = SystemColors.Info;
            m_remappingForeColor = SystemColors.HotTrack;
        }


        private NewPlannerWindow m_plannerWindow;
        public NewPlannerWindow PlannerWindow
        {
            set { m_plannerWindow = value; }
        }

        private Plan m_plan;
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan != null)
                {
                    m_plan.Changed -= new EventHandler<EventArgs>(OnPlanChanged);
                    m_plan.GrandCharacterInfo.SkillChanged -= new SkillChangedHandler(OnSkillChanged);
                }
                m_plan = value;
                this.skillSelectControl.Plan = value;
                if (m_plan != null)
                {
                    m_plan.Changed += new EventHandler<EventArgs>(OnPlanChanged);
                    m_plan.GrandCharacterInfo.SkillChanged += new SkillChangedHandler(OnSkillChanged);
                }
                UpdateListColumns();
                OnPlanChanged(null, null);
            }
        }

        public CharacterInfo GrandCharacterInfo
        {
            set { this.skillSelectControl.GrandCharacterInfo = value; }
        }

        private void OnPlanChanged(object sender, EventArgs e)
        {
            tmrAutoRefresh.Enabled = false;
            if (m_plan == null)
            {
                lvSkills.Items.Clear();
            }
            else
            {
                UpdateSkillList();
            }
        }

        private void OnSkillChanged(object sender, SkillChangedEventArgs e)
        {
            UpdateListViewItems();
        }

        private void tmrAutoRefresh_Tick(object sender, EventArgs e)
        {
            UpdateListViewItems();
        }

        private void llSuggestionLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ColumnPreference pref = m_plan.ColumnPreference;
            using (PlanOrderEditorColumnSelectWindow f = new PlanOrderEditorColumnSelectWindow(pref))
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    UpdateListColumns();
                    Program.Settings.Save();
                }
            }
        }

        #region Look and Feel
        private bool m_HighlightPrerequisites = false;
        public bool HighlightPrerequisites
        {
            get { return m_HighlightPrerequisites; }
            set { m_HighlightPrerequisites = value; UpdateSkillList(); }
        }

        private bool m_HighlightPlannedSkills = false;
        public bool HighlightPlannedSkills
        {
            get { return m_HighlightPlannedSkills; }
            set { m_HighlightPlannedSkills = value; UpdateSkillList(); }
        }

        private bool m_HighlightConflicts = false;
        public bool HighlightConflicts
        {
            get { return m_HighlightConflicts; }
            set { m_HighlightConflicts = value; UpdateSkillList(); }
        }


        private bool m_DimUntrainable = false;
        public bool DimUntrainable
        {
            get { return m_DimUntrainable; }
            set { m_DimUntrainable = value; UpdateSkillList(); }
        }

        private bool m_WorksafeMode = false;
        public bool WorksafeMode
        {
            get { return m_WorksafeMode; }
            set { m_WorksafeMode = value; UpdateSkillList(); }
        }
        #endregion Look and Feel

        #region Skill List View
        private void UpdateSkillList()
        {
            if (m_plan == null) return;
            lvSkills.BeginUpdate();
            try
            {
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (var pe in m_plan.Entries)
                {
                    if (pe != null)
                    {
                        // In any case, we must insert a new item for this plan's entry at the current index
                        // Do we need to insert a remapping point ?
                        if (pe.Remapping != null)
                        {
                            ListViewItem rlv = new ListViewItem();
                            //rlv.Font = new Font(lvSkills.Font, FontStyle..Underline);
                            rlv.BackColor = m_remappingBackColor;
                            rlv.ForeColor = m_remappingForeColor;
                            rlv.UseItemStyleForSubItems = true;
                            rlv.Text = pe.Remapping.ToString();
                            rlv.Tag = pe.Remapping;
                            rlv.StateImageIndex = 3;
                            items.Add(rlv);
                        }

                        // Insert the entry
                        ListViewItem lvi = new ListViewItem();
                        lvi.Tag = pe;
                        items.Add(lvi);

                        // Highlight entries that were planned, if option enabled.
                        if (!m_WorksafeMode && m_HighlightPlannedSkills && pe.EntryType == Plan.Entry.Type.Planned)
                        {
                            lvi.Font = m_plannedSkillFont;
                        }
                        else
                        {
                            lvi.Font = m_prerequisiteSkillFont;
                        }

                        // Gray out entries that can not be trained immediately.
                        if (!pe.CanTrainNow && m_DimUntrainable)
                        {
                            lvi.ForeColor = m_trainablePlanEntryColor;

                        }

                        // If this skill is currently in training, (re)start the auto refresh timer
                        Skill gs = pe.Skill;
                        if (gs.InTraining) tmrAutoRefresh.Enabled = true;
                    }
                }

                lvSkills.Items.Clear();
                lvSkills.Items.AddRange(items.ToArray());

                UpdateListViewItems();
            }
            finally
            {
                lvSkills.EndUpdate();
            }
        }

        private void InsertRemappingPoint(int index, Plan.Entry pe)
        {
            ListViewItem rlv = new ListViewItem();
            //rlv.Font = new Font(lvSkills.Font, FontStyle..Underline);
            rlv.BackColor = m_remappingBackColor;
            rlv.ForeColor = m_remappingForeColor;
            rlv.UseItemStyleForSubItems = true;
            rlv.Text = pe.Remapping.ToString();
            rlv.Tag = pe.Remapping;
            rlv.StateImageIndex = 3;
            lvSkills.Items.Insert(index, rlv);
        }

        private const int MAX_NOTES_PREVIEW_CHARS = 60;

        private void UpdateListViewItems()
        {
            if (m_settings == null) m_settings = Settings.GetInstance();
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateListViewItems));
                return;
            }

            // Gets the starting scratchpad, asking the pluggable when relevant
            // Note that pluggable can either override the current scratchpad (Implants calc, 
            // optimizer with no remapping points) or thd old scratchpad (when using remapping points
            // for the new informations)
            bool useRemappingPointsForOld = true;
            bool useRemappingPointsForNew = true;
            EveAttributeScratchpad scratchpad = new EveAttributeScratchpad();
            EveAttributeScratchpad oldScratchpad = new EveAttributeScratchpad();
            if (m_pluggable != null)
            {
                bool isNew;
                var temp = m_pluggable.GetScratchpad(out isNew);

                if (isNew) scratchpad = temp;
                else oldScratchpad = temp;

                useRemappingPointsForOld = m_pluggable.UseRemappingPointsForOld;
                useRemappingPointsForNew = m_pluggable.UseRemappingPointsForNew;
            }

            // Start updating the list
            lvSkills.BeginUpdate();
            try
            {
                DateTime start = DateTime.Now;
                int skillPointTotal = m_plan.GrandCharacterInfo.SkillPointTotal;
                NumberFormatInfo nfi = NumberFormatInfo.CurrentInfo;

                for (int i = 0; i < lvSkills.Items.Count; i++)
                {
                    ListViewItem lvi = lvSkills.Items[i];

                    // Remapping point
                    if (lvi.Tag is Plan.RemappingPoint)
                    {
                        Plan.RemappingPoint rm = (Plan.RemappingPoint)lvi.Tag;
                        lvi.Text = (useRemappingPointsForNew ? rm.ToString() : "Remapping (ignored)");

                        if (rm.Status != Plan.RemappingPoint.PointStatus.NotComputed)
                        {
                            if (useRemappingPointsForNew)
                            {
                                scratchpad = rm.TransformSctratchpad(m_plan.GrandCharacterInfo, scratchpad);
                            }
                            if (useRemappingPointsForOld)
                            {
                                oldScratchpad = rm.TransformSctratchpad(m_plan.GrandCharacterInfo, oldScratchpad);
                            }
                        }

                    }
                    // Skill
                    else
                    {
                        Plan.Entry pe = (Plan.Entry)lvi.Tag;
                        Skill gs = pe.Skill;

                        while (lvi.SubItems.Count < lvSkills.Columns.Count + 1)
                        {
                            lvi.SubItems.Add(String.Empty);
                        }

                        // Compute the training time and misc parameters
                        TimeSpan trainTime = gs.GetTrainingTimeOfLevelOnly(pe.Level, skillPointTotal, true, scratchpad);
                        TimeSpan trainTimeNatural = gs.GetTrainingTimeOfLevelOnly(pe.Level, skillPointTotal, true, scratchpad, false);
                        TimeSpan oldTrainTime = gs.GetTrainingTimeOfLevelOnly(pe.Level, skillPointTotal, true, oldScratchpad);

                        int currentSP = gs.CurrentSkillPoints;
                        int reqToThisLevel = gs.GetPointsRequiredForLevel(pe.Level);
                        int reqBeforeThisLevel = gs.GetPointsRequiredForLevel(pe.Level - 1);
                        int spHour = gs.GetPointsForTimeSpan(new TimeSpan(1, 0, 0), scratchpad);
                        int pointsInThisLevel = Math.Max(0, currentSP - reqBeforeThisLevel);
                        double deltaPointsOfLevel = Convert.ToDouble(reqToThisLevel - reqBeforeThisLevel);
                        double pctComplete = Math.Min(1.0, pointsInThisLevel / deltaPointsOfLevel);

                        DateTime thisStart = start;
                        DateTime thisEnd = start + trainTime;
                        start = thisEnd;

                        skillPointTotal += (reqToThisLevel - reqBeforeThisLevel - pointsInThisLevel);

                        scratchpad.ApplyALevelOf(gs);
                        oldScratchpad.ApplyALevelOf(gs);

                        // Retrieve entry's plan group
                        string planGroups;
                        if (pe.PlanGroups.Count == 0)
                        {
                            planGroups = "None";
                        }
                        else if (pe.PlanGroups.Count == 1)
                        {
                            planGroups = (string)pe.PlanGroups[0];
                        }
                        else
                        {
                            planGroups = "Multiple (" + pe.PlanGroups.Count + ")";
                        }

                        // See if this entry conflicts with a schedule entry
                        string BlockingEntry = string.Empty;
                        bool isBlocked = m_settings.SkillIsBlockedAt(thisEnd, out BlockingEntry);
                        if (isBlocked && m_HighlightConflicts) lvi.ForeColor = Color.Red;

                        // Update every column
                        lvi.UseItemStyleForSubItems = (m_pluggable == null);
                        for (int x = 0; x < lvSkills.Columns.Count; x++)
                        {
                            string res = String.Empty;
                            ColumnPreference.ColumnType ct;
                            // Regular columns (not pluggable-dependent)
                            if (lvSkills.Columns[x].Tag != null)
                            {
                                lvi.SubItems[x].ForeColor = lvi.ForeColor;
                                ct = (ColumnPreference.ColumnType)lvSkills.Columns[x].Tag;
                                switch (ct)
                                {
                                    case ColumnPreference.ColumnType.SkillName:
                                        res = gs.Name + " " + Skill.GetRomanForInt(pe.Level);
                                        break;
                                    case ColumnPreference.ColumnType.PlanGroup:
                                        res = planGroups;
                                        break;
                                    case ColumnPreference.ColumnType.TrainingTime:
                                        res = Skill.TimeSpanToDescriptiveText(trainTime, DescriptiveTextOptions.IncludeCommas);
                                        break;
                                    case ColumnPreference.ColumnType.TrainingTimeNatural:
                                        res = Skill.TimeSpanToDescriptiveText(trainTimeNatural, DescriptiveTextOptions.IncludeCommas);
                                        break;
                                    case ColumnPreference.ColumnType.EarliestStart:
                                        res = thisStart.ToString("ddd ") + thisStart.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.EarliestEnd:
                                        res = thisEnd.ToString("ddd ") + thisEnd.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.PercentComplete:
                                        res = pctComplete.ToString("0%");
                                        break;
                                    case ColumnPreference.ColumnType.SkillRank:
                                        res = gs.Rank.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.PrimaryAttribute:
                                        res = gs.PrimaryAttribute.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.SecondaryAttribute:
                                        res = gs.SecondaryAttribute.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.SkillGroup:
                                        res = gs.SkillGroup.Name;
                                        break;
                                    case ColumnPreference.ColumnType.Notes:
                                        string xx;
                                        if (String.IsNullOrEmpty(pe.Notes))
                                        {
                                            res = String.Empty;
                                        }
                                        else
                                        {
                                            xx = Regex.Replace(pe.Notes, @"(\r|\n)+", " ", RegexOptions.None);
                                            if (xx.Length <= MAX_NOTES_PREVIEW_CHARS)
                                            {
                                                res = xx;
                                            }
                                            else
                                            {
                                                res = xx.Substring(0, MAX_NOTES_PREVIEW_CHARS) + "...";
                                            }
                                        }
                                        break;
                                    case ColumnPreference.ColumnType.PlanType:
                                        res = pe.EntryType.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.SPTotal:
                                        res = skillPointTotal.ToString("N00", nfi);
                                        break;
                                    case ColumnPreference.ColumnType.SPPerHour:
                                        res = spHour.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.Priority:
                                        res = pe.Priority.ToString();
                                        break;
                                    case ColumnPreference.ColumnType.Cost:
                                        if (pe.Level == 1)
                                        {
                                            if (!pe.Skill.Known)
                                            {
                                                if (pe.Skill.Owned)
                                                {
                                                    res = "Owned";
                                                }
                                                else
                                                {
                                                    res = pe.Skill.FormattedCost;
                                                }
                                            }
                                        }
                                        break;
                                    case ColumnPreference.ColumnType.Conflicts:
                                        res = BlockingEntry;
                                        break;
                                }
                            }
                            // Tag was null so this is the manufactured column to show the 
                            // difference in training time using the attributes from the
                            // implant calc
                            else
                            {
                                TimeSpan t;
                                res = "";
                                if (oldTrainTime < trainTime)
                                {
                                    res = "+";
                                    t = trainTime - oldTrainTime;
                                    lvi.SubItems[x].ForeColor = Color.DarkRed;
                                }
                                else if (oldTrainTime > trainTime)
                                {
                                    res = "-";
                                    t = oldTrainTime - trainTime;
                                    lvi.SubItems[x].ForeColor = Color.DarkGreen;
                                }
                                else
                                {
                                    t = TimeSpan.Zero;
                                    lvi.SubItems[x].ForeColor = lvi.ForeColor;
                                }
                                res += Skill.TimeSpanToDescriptiveText(t, DescriptiveTextOptions.IncludeCommas);

                                // needed so Eewec's bit below compiles...
                                ct = ColumnPreference.ColumnType.TrainingTime;
                            }

                            lvi.SubItems[x].Text = res;
                        }
                        lvi.SubItems[lvSkills.Columns.Count].Text = pe.EntryType.ToString();
                    }
                }
            }
            finally
            {
                lvSkills.EndUpdate();
            }
        }

        private double ApplyLearningLevel(int value, EveAttributeScratchpad scratchpad)
        {
            int learningLevel = m_plan.GrandCharacterInfo.SkillGroups["Learning"]["Learning"].Level;
            if (scratchpad != null)
            {
                learningLevel += scratchpad.LearningLevelBonus;
            }

            double learningAdjust = 1.0 + (0.02 * Convert.ToDouble(learningLevel));
            return Convert.ToDouble(value) * learningAdjust;
        }

        private void lvSkills_ListViewItemsDragged(object sender, EventArgs e)
        {
            RebuildPlanFromListViewOrder(lvSkills.Items);
        }

        private void lvSkills_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            // Because this occurs before the reordering happens, we have to delay the
            // Order update a bit...
            ThreadPool.QueueUserWorkItem(delegate
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    StringBuilder sb = new StringBuilder();
                    SortedDictionary<int, ColumnPreference.ColumnType> order = new SortedDictionary<int, ColumnPreference.ColumnType>();
                    for (int i = 0; i < lvSkills.Columns.Count; i++)
                    {
                        ColumnHeader ch = lvSkills.Columns[i];
                        order.Add(ch.DisplayIndex, (ColumnPreference.ColumnType)ch.Tag);
                    }
                    foreach (KeyValuePair<int, ColumnPreference.ColumnType> pair in order)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(',');
                        }
                        sb.Append(pair.Value.ToString());
                    }
                    m_plan.ColumnPreference.Order = sb.ToString();
                    Program.Settings.Save();
                }));
            });
        }

        private void lvSkills_ItemHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            ListViewItem lvi = e.Item;
            if (null != lvi)
            {
                var entry = GetPlanEntryForListViewItem(lvi);
                if (entry != null)
                {
                    Skill s = entry.Skill;
                    StringBuilder sb = new StringBuilder(s.Description);
                    lvi.ToolTipText = s.Description;
                    if (!s.Known)
                    {
                        sb.Append("\n\nYou do not know this skill - you ");
                        if (!s.Owned)
                            sb.Append("do not ");
                        sb.Append("own the skillbook");
                    }
                    lvi.ToolTipText = sb.ToString();
                }
                else if(lvi.Tag is Plan.RemappingPoint)
                {
                    var point = lvi.Tag as Plan.RemappingPoint;
                    lvi.ToolTipText = point.ToLongString();
                }
            }
        }

        private void RebuildPlanFromListViewOrder(IEnumerable items)
        {
            m_plan.SuppressEvents();
            try
            {
                m_plan.Entries.Clear();
                Plan.RemappingPoint rp = null;
                foreach (ListViewItem lvi in items)
                {
                    Plan.Entry pe = lvi.Tag as Plan.Entry;
                    if (pe != null)
                    {
                        Plan.Entry newPe = pe.Clone() as Plan.Entry;
                        m_plan.Entries.Add(newPe);
                        newPe.Remapping = rp;
                        rp = null;
                    }
                    else
                    {
                        rp = lvi.Tag as Plan.RemappingPoint;
                    }
                }

                // Enforces proper ordering too!
                m_plan.CheckForMissingPrerequisites();
            }
            finally
            {
                m_plan.ResumeEvents();
            }
        }

        public void UpdateListColumns()
        {
            if (m_plan != null)
            {
                lvSkills.BeginUpdate();
                try
                {
                    lvSkills.Columns.Clear();
                    List<ColumnPreference.ColumnType> alreadyAdded = new List<ColumnPreference.ColumnType>();

                    foreach (string ts in m_plan.ColumnPreference.Order.Split(','))
                    {
                        try
                        {
                            ColumnPreference.ColumnType ct = (ColumnPreference.ColumnType)Enum.Parse(typeof(ColumnPreference.ColumnType), ts, true);
                            if (m_plan.ColumnPreference[ct] && !alreadyAdded.Contains(ct))
                            {
                                AddColumn(ct);
                                alreadyAdded.Add(ct);
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.LogException(e, false);
                        }
                    }

                    for (int i = 0; i < ColumnPreference.ColumnCount; i++)
                    {
                        ColumnPreference.ColumnType ct = (ColumnPreference.ColumnType)i;
                        if (m_plan.ColumnPreference[i] && !alreadyAdded.Contains(ct))
                        {
                            AddColumn(ct);
                            alreadyAdded.Add(ct);
                        }
                    }
                    //}
                    //finally
                    //{
                    //    lvSkills.EndUpdate();
                    //}
                    UpdateListViewItems();
                    //lvSkills.BeginUpdate();
                    //try
                    //{
                    for (int i = 0; i < lvSkills.Columns.Count; i++)
                    {
                        ColumnHeader ch = lvSkills.Columns[i];
                        if (ch.Tag != null)
                        {
                            ColumnPreference.ColumnDisplayAttribute cda = ColumnPreference.GetAttribute((ColumnPreference.ColumnType)ch.Tag);
                            ch.Width = cda.Width;
                        }
                    }
                }
                finally
                {
                    lvSkills.EndUpdate();
                }
            }
        }

        private void AddColumn(ColumnPreference.ColumnType ct)
        {
            ColumnPreference.ColumnDisplayAttribute cda = ColumnPreference.GetAttribute(ct);
            ColumnHeader ch = new ColumnHeader();
            ch.Text = cda.Header;
            ch.Tag = ct;
            lvSkills.Columns.Add(ch);
            // add a temporary column if we're showing the implant calculator
            if (m_pluggable != null && ch.Text == "Training Time")
            {
                ch = new ColumnHeader();
                ch.Text = "Diff with Calc Atts";
                ch.Tag = null;
                ch.Width = 105;
                lvSkills.Columns.Add(ch);
            }
        }
        #endregion Skill List View

        #region Implant Calc

        private IPlanOrderPluggable m_pluggable = null;
        internal void ShowWithPluggable(IPlanOrderPluggable pluggable)
        {
            lvSkills.BeginUpdate();
            try
            {
                if (m_pluggable == null)
                {
                    m_pluggable = pluggable;
                    pluggable.Disposed += new EventHandler(pluggable_Disposed);
                    UpdateListColumns();
                }
                UpdateListViewItems();
            }
            finally
            {
                lvSkills.EndUpdate();
            }
        }

        private void pluggable_Disposed(object o, EventArgs e)
        {
            m_pluggable = null;
            UpdateListColumns();
        }

        #endregion

        #region Context Menu

        private void cmsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            //miRemoveFromPlan.Enabled = (lvSkills.SelectedItems.Count == 1);
            miRemoveFromPlan.Enabled = true;
            miChangeNote.Enabled = (lvSkills.SelectedItems.Count > 0);
            miChangePriority.Enabled = miChangeNote.Enabled;

            // When there is only one selected item, we enable/disable some items
            if (lvSkills.SelectedItems.Count == 1)
            {
                miChangeNote.Text = "View/Change Note...";
                Plan.Entry pe = lvSkills.SelectedItems[0].Tag as Plan.Entry;

                // Menu items' visibility
                foreach(ToolStripItem item in cmsContextMenu.Items)
                {
                    if (item != miRemoveFromPlan) 
                    {
                        item.Visible = (pe != null);
                    }
                }
                if (pe == null) return;

                Skill s = pe.Skill;
                if (!s.Known)
                {
                    if (s.Owned)
                    {
                        miMarkOwned.Text = "Mark as unowned";
                    }
                    else
                    {
                        miMarkOwned.Text = "Mark as owned";
                    }
                    miMarkOwned.Enabled = true;
                }
                else
                {
                    miMarkOwned.Text = "Mark as owned";
                    miMarkOwned.Enabled = false;
                }
                miChangeLevel.Enabled = SetChangeLevelMenu();
            }
            else
            {
                miChangeLevel.Enabled = false;
                miChangeNote.Text = "Change Note...";
            }

            miShowInSkillBrowser.Enabled = (lvSkills.SelectedItems.Count == 1);
            miShowInSkillExplorer.Enabled = (lvSkills.SelectedItems.Count == 1);
            miMarkOwned.Enabled = (lvSkills.SelectedItems.Count > 0);
            var entry = GetPlanEntryForListViewItem(lvSkills.SelectedItems[0]);

            // If a single item is selected, which belong to different plan groups, we display that menu
            if (lvSkills.SelectedItems.Count == 1 && entry.PlanGroups.Count > 0)
            {
                miPlanGroups.Enabled = true;
                miPlanGroups.DropDownItems.Clear();
                List<string> planGroups = new List<string>();
                foreach (string pg in GetPlanEntryForListViewItem(lvSkills.SelectedItems[0]).PlanGroups)
                {
                    planGroups.Add(pg);
                }
                planGroups.Sort();
                foreach (string pg in planGroups)
                {
                    ToolStripButton tsb = new ToolStripButton(pg);
                    tsb.Click += new EventHandler(tsb_Click);
                    miPlanGroups.DropDownItems.Add(tsb);
                }
            }
            else
            {
                miPlanGroups.Enabled = false;
            }

            // More than one item selected ? We display the "create sub plan" menu
            if (lvSkills.SelectedItems.Count >= 1)
            {
                miSubPlan.Enabled = true;
            }
            else
            {
                miSubPlan.Enabled = false;
            }

        }

        private void tsb_Click(object sender, EventArgs e)
        {
            string planGroup = ((ToolStripButton)sender).Text;
            foreach (ListViewItem lvi in lvSkills.Items)
            {
                lvi.Selected = GetPlanEntryForListViewItem(lvi).PlanGroups.Contains(planGroup);
            }
        }

        private void miShowInSkillBrowser_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = lvSkills.SelectedItems[0];
            Plan.Entry pe = lvi.Tag as Plan.Entry;
            if (pe != null)
            {
                Skill gs = pe.Skill;
                m_plannerWindow.ShowSkillInTree(gs);
            }
        }

        private void miShowInSkillExplorer_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = lvSkills.SelectedItems[0];
            Plan.Entry pe = lvi.Tag as Plan.Entry;
            if (pe != null)
            {
                Skill gs = pe.Skill;
                m_plannerWindow.ShowSkillInExplorer(gs);
            }
        }

        private void miRemoveFromPlan_Click(object sender, EventArgs e)
        {
            //Abstracted logic to function RemoveEntry for issue #369: Add use of Delete key
            if (lvSkills.SelectedItems.Count == 1)
            {
                RemoveSelectedEntry();
            }
            else RemoveSelectedEntries();
        }

        private void miChangePriority_Click(object sender, EventArgs e)
        {
            using (ChangePriorityForm f = new ChangePriorityForm())
            {
                // Get intial priority
                f.Priority = Plan.Entry.DEFAULT_PRIORITY;
                if (lvSkills.SelectedItems.Count == 1)
                {
                    Plan.Entry pe = lvSkills.SelectedItems[0].Tag as Plan.Entry;
                    if (pe != null) f.Priority = pe.Priority;
                }

                // User canceled ?
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel) return;

                // Update priorities, while performing backup for subsequent check
                Dictionary<Plan.Entry, Plan.Entry> backup = new Dictionary<Plan.Entry, Plan.Entry>();
                bool loweringPriorities = true; // if only lowering priorities
                foreach (ListViewItem lvi in lvSkills.SelectedItems)
                {
                    Plan.Entry penew = lvi.Tag as Plan.Entry;
                    if (penew != null)
                    {
                        Plan.Entry peold = (Plan.Entry)penew.Clone();
                        loweringPriorities &= (f.Priority > peold.Priority);
                        penew.Priority = f.Priority;
                        backup.Add(penew, peold);
                    }
                }

                // We need to check that prerequisites do not have a lower priority
                if (!m_plan.CheckPriorities(false))
                {
                    DialogResult drb = MessageBox.Show("This would result in a priorioty conflict. (Either pre-requisites with a lower priority, or dependant skills with a higher priority.)\nClick Yes if you wish to do this, and adjust the other skills, or No if you do not wish to change the priority", "Priority Conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (drb == DialogResult.No)
                    {
                        // cancel!
                        foreach (Plan.Entry pe in backup.Keys)
                        {
                            pe.Priority = backup[pe].Priority;
                        }
                    }
                    else
                    {
                        m_plan.CheckPriorities(true, loweringPriorities);
                    }
                }

                UpdateListViewItems();
                Program.Settings.Save();
            }
        }

        private void RemoveSelectedEntry()
        {
            if (lvSkills.SelectedItems.Count != 1)
            {
                RemoveSelectedEntries();
                return;
            }

            ListViewItem item = lvSkills.SelectedItems[0];
            if (item.Tag is Plan.RemappingPoint)
            {
                ListViewItem nextItem = lvSkills.Items[item.Index + 1];
                Plan.Entry entry = nextItem.Tag as Plan.Entry;
                entry.Remapping = null;
                lvSkills.Items.Remove(item);
                nextItem.Selected = true;
            }
            else
            {
                using (CancelChoiceWindow f = new CancelChoiceWindow())
                {
                    f.CancelMultipleSkills = false;
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.Cancel)
                    {
                        return;
                    }
                    if (dr == DialogResult.Yes)
                    {
                        RemoveFromPlan(GetPlanEntryForListViewItem(lvSkills.SelectedItems[0]), true);
                    }
                    if (dr == DialogResult.No)
                    {
                        RemoveFromPlan(GetPlanEntryForListViewItem(lvSkills.SelectedItems[0]), false);
                    }
                }
            }
        }

        // Get the dialog
        // Get the list of selected plan entries
        // while entries not removed
        // remove last entry
        private void RemoveSelectedEntries()
        {
            Stack<ListViewItem> selectedItems = new Stack<ListViewItem>();
            foreach (ListViewItem lvi in lvSkills.SelectedItems)
            {
                if (lvi.Tag is Plan.Entry) selectedItems.Push(lvi);
            }

            bool removePrereqs;

            using (CancelChoiceWindow f = new CancelChoiceWindow())
            {
                f.CancelMultipleSkills = true;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                removePrereqs = (dr == DialogResult.Yes);
            }

            while (selectedItems.Count > 0)
            {
                ListViewItem lvi = selectedItems.Pop();
                Plan.Entry pe = GetPlanEntryForListViewItem(lvi);
                if (m_plan.GetEntry(pe.SkillName,pe.Level) != null)
                {
                    // we still have the entry - remove it.
                     RemoveFromPlan(pe, removePrereqs);
                }
            }

            UpdateSkillList();
        }


        private void miChangeNote_Click(object sender, EventArgs e)
        {
            if (lvSkills.SelectedItems.Count < 0) return;

            // We get the first entry
            Plan.Entry pe = GetFirstSelectedEntry();
            if (pe == null) return;

            // We get the current skill's note and call the note editor window with this intial value
            string sn = "Selected Skills";
            string noteText = pe.Notes;
            if (lvSkills.SelectedItems.Count == 1)
            {
                sn = pe.SkillName + " " + Skill.GetRomanForInt(pe.Level);
            }

            using (EditEntryNoteWindow f = new EditEntryNoteWindow(sn))
            {
                f.NoteText = noteText;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                noteText = f.NoteText;
            }

            // We update every item
            foreach (ListViewItem lvi in lvSkills.SelectedItems)
            {
                pe = lvi.Tag as Plan.Entry;
                if (pe != null) pe.Notes = noteText;
            }
            UpdateListViewItems();
            Program.Settings.Save();
        }

        private Plan.Entry GetFirstSelectedEntry()
        {
            Plan.Entry pe = null;
            foreach (ListViewItem item in lvSkills.SelectedItems)
            {
                pe = lvSkills.SelectedItems[0].Tag as Plan.Entry;
                if (pe != null) return pe;
            }
            return null;
        }

        private void miSubPlan_Click(object sender, EventArgs e)
        {
            // Gets the correct plan-key (depends whether the char is file-based or not)
            string planKey = m_plan.GrandCharacterInfo.Name;
            foreach (CharFileInfo cfi in m_settings.CharFileList)
            {
                if (cfi.CharacterName.Equals(m_plan.GrandCharacterInfo.Name))
                {
                    planKey = cfi.Filename;
                    break;
                }
            }

            // Recursively group skills, then their prerequisites
            bool doAgain = true;
            while (doAgain)
            {
                using (NewPlanWindow npw = new NewPlanWindow())
                {
                    DialogResult dr = npw.ShowDialog();
                    if (dr == DialogResult.Cancel)
                    {
                        return;
                    }
                    string planName = npw.Result;
                    Plan newPlan = new Plan();
                    newPlan.Name = planName;
                    newPlan.GrandCharacterInfo = m_plan.GrandCharacterInfo;

                    foreach (ListViewItem lvi in lvSkills.SelectedItems)
                    {
                        Plan.Entry oldplanitem = lvi.Tag as Plan.Entry;
                        if (oldplanitem != null)
                        {
                            newPlan.PlanTo(oldplanitem.Skill, oldplanitem.Level, "Exported from " + m_plan.Name);
                        }
                    }

                    // Enforces proper ordering too!
                    newPlan.CheckForMissingPrerequisites();
                    try
                    {
                        m_settings.AddPlanFor(planKey, newPlan, planName);
                        doAgain = false;
                    }
                    catch (ApplicationException err)
                    {
                        ExceptionHandler.LogException(err, true);
                        DialogResult xdr =
                            MessageBox.Show(err.Message, "Failed to Add Plan", MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Error);
                        if (xdr == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void miMarkOwned_Click(object sender, EventArgs e)
        {
            Plan.Entry pe = lvSkills.SelectedItems[0].Tag as Plan.Entry;
            if (pe != null)
            {
                pe.Skill.Owned = !pe.Skill.Owned;
                pe.Plan.GrandCharacterInfo.UpdateOwnedSkills();
                UpdateListViewItems();
                m_plannerWindow.UpdateStatusBar();
            }
        }

        private void miChangeToN_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            string level = tsmi.Tag as string;
            Plan.Entry pe = lvSkills.SelectedItems[0].Tag as Plan.Entry;
            if (pe != null) m_plan.PlanTo(pe.Skill, Int32.Parse(level));
        }

        private bool SetChangeLevelMenu()
        {
            bool result = false;

            Plan.Entry pe = lvSkills.SelectedItems[0].Tag as Plan.Entry;
            // determine what valid levels we can change to
            int minPlan = pe.Skill.Level + 1;

            int plannedTo = 0;
            for (int i = 1; i < 6; i++)
            {
                if (m_plan.IsPlanned(pe.Skill, i))
                {
                    plannedTo = i;
                }
            }

            for (int i = 1; i < 6; i++)
            {
                if (i < minPlan || i == plannedTo)
                {
                    miChangeLevel.DropDownItems[i].Enabled = false;
                }
                else
                {
                    miChangeLevel.DropDownItems[i].Enabled = true;

                    // see if there are any skills dependant on this skill
                    foreach (ListViewItem current in lvSkills.Items)
                    {
                        Plan.Entry currentSkill = current.Tag as Plan.Entry;
                        if (currentSkill != null)
                        {
                            int neededLevel;
                            if (currentSkill.Skill.HasAsImmedPrereq(pe.Skill, out neededLevel))
                            {
                                if (currentSkill.Level == 1 && neededLevel > i)
                                {
                                    miChangeLevel.DropDownItems[i].Enabled = false;
                                    // we have a post-dependancy - disable the remove option
                                    miChangeLevel.DropDownItems[0].Enabled = false;
                                }
                            }
                        }
                    }
                }
                if (miChangeLevel.DropDownItems[i].Enabled)
                {
                    result = true;
                }
            }
            return result;
        }

        #endregion Context Menu

        #region Plan Re-Ordering
        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            lvSkills.BeginUpdate();
            var selection = StoreSelection();
            try
            {
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (ListViewItem lvi in lvSkills.Items) items.Add(lvi);

                // Then, we move the items
                bool isHead = true;
                for (int i = 0; i < items.Count; i++)
                {
                    ListViewItem item = items[i];
                    if (item.Selected && !isHead)
                    {
                        items.RemoveAt(i);
                        items.Insert(i - 1, item);
                    }
                    else
                    {
                        // As soon that a non-selected item has been encountered, the tail is over
                        isHead = false;
                    }
                }

                RebuildPlanFromListViewOrder(items);
            }
            finally
            {
                RestoreSelection(selection);
                lvSkills.EndUpdate();
            }
        }

        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            lvSkills.BeginUpdate();
            var selection = StoreSelection();
            try
            {
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (ListViewItem lvi in lvSkills.Items) items.Add(lvi);

                // Then, we move the plan's entries
                bool isTail = true;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    ListViewItem item = lvSkills.Items[i];
                    if (item.Selected && !isTail)
                    {
                        items.RemoveAt(i);
                        items.Insert(i + 1, item);
                    }
                    else
                    {
                        // As soon that a non-selected item has been encountered, the tail is over
                        isTail = false;
                    }
                }

                RebuildPlanFromListViewOrder(items);
            }
            finally
            {
                RestoreSelection(selection);
                lvSkills.EndUpdate();
            }
        }

        private void tsbSort_Click(object sender, EventArgs e)
        {
            using (PlanSortWindow f = new PlanSortWindow())
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    PlanSorter.SortPlan(m_plan, f.SortType, f.LearningFirst, f.Priority);
                    tsbMoveDown.Enabled = false;
                    tsbMoveUp.Enabled = false;
                }
            }
        }
        #endregion Plan Re-Ordering


        #region Selection and items' perma-id
        private ListViewItem GetItemById(int id)
        {
            foreach (ListViewItem lvi in lvSkills.Items)
            {
                if (GetID(lvi) == id) return lvi;
            }
            return null;
        }

        private int GetID(ListViewItem lvi)
        {
            Plan.Entry pe = lvi.Tag as Plan.Entry;
            if (pe != null) return GetID(pe);

            Plan.RemappingPoint rp = lvi.Tag as Plan.RemappingPoint;
            if (rp != null) return GetID(rp);

            return -1;
        }

        private int GetID(Plan.Entry pe)
        {
            return pe.SkillName.GetHashCode() << 3 | pe.Level;
        }

        private int GetID(Plan.RemappingPoint point)
        {
            return point.Guid.GetHashCode();
        }

        private Dictionary<int, bool> StoreSelection()
        {
            Dictionary<int, bool> c = new Dictionary<int, bool>();

            // Compute and store a string ID for every item
            foreach (ListViewItem lvi in lvSkills.SelectedItems)
            {
                // If an entry, no problem
                if (lvi.Tag is Plan.Entry)
                {
                    Plan.Entry pe = lvi.Tag as Plan.Entry;
                    int id = GetID(pe);
                    c[id] = true;
                }
                // If a remapping point, we retrieve the entry it is attached to and add "remapping" after this entry's id
                else if (lvi.Tag is Plan.RemappingPoint)
                {
                    Plan.RemappingPoint rp = lvi.Tag as Plan.RemappingPoint;
                    int id = GetID(rp);
                    c[id] = true;
                }
            }

            return c;
        }

        private void RestoreSelection(Dictionary<int, bool> c)
        {
            for (int i = this.lvSkills.Items.Count - 1; i >= 0; i--)
            {
                // Retrieve this item's id
                int id = 0;
                ListViewItem lvi = lvSkills.Items[i];
                if (lvi.Tag is Plan.Entry)
                {
                    Plan.Entry pe = lvi.Tag as Plan.Entry;
                    id = GetID(pe);
                }
                else if (lvi.Tag is Plan.RemappingPoint)
                {
                    Plan.RemappingPoint rp = lvi.Tag as Plan.RemappingPoint;
                    id = GetID(rp);
                }

                // Check whether this id must be selected
                if (c.ContainsKey(id))
                {
                    lvi.Selected = true;
                    c.Remove(id);
                }
                else
                {
                    lvi.Selected = false;
                }
            }
        }
        #endregion Selection and items' perma-id


        private Plan.Entry GetPlanEntryForListViewItem(ListViewItem lvi)
        {
            if (lvi == null)
            {
                return null;
            }
            return lvi.Tag as Plan.Entry;
        }

        private void RemoveFromPlan(Plan.Entry pe, bool includePrerequisites)
        {
            bool result = m_plan.RemoveEntry(pe.Skill, includePrerequisites, false);
            if (!result)
            {
                MessageBox.Show(this,
                                "The plan for this skill could not be cancelled because this skill is " +
                                "required for another skill you have planned.",
                                "Skill Needed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void lvSkills_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedEntry();
            }
        }

        private void PlanOrderEditorControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            if (m_settings == null)
            {
                m_settings = Settings.GetInstance();
            }
            tmrSelect.Enabled = false;
        }

        private void lvSkills_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvSkills.SelectedItems.Count == 1)
            {
                miShowInSkillBrowser_Click(sender, e);
            }
        }

        private void lvSkills_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                if (lvSkills.SelectedItems.Count > 0)
                {
                    miChangeNote_Click(sender, e);
                }
            }
            else if (e.KeyCode == Keys.F9)
            {
                this.tsbToggleRemapping_Click(null, null);
            }
        }

        private void tmrSelect_Tick(object sender, EventArgs e)
        {
            tmrSelect.Enabled = false;
            SelectedIndexChanged();
        }

        private void lvSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Calculating the summary status for all selected skills is a 
             * slow operation. If user does a shift-click to select multiple rows, 
             * then this event handler will be called for every item in the selectiom.
             * i.e. if tehre are 100 entries in the plan, and user clicks 1st item and shift-clicks
             * the last item, then this handler will be called 100 times - but we're
             * only interested in the last event from a shift-click - so we start a
             * timer that runs for 100 milliseconds before we process the event.
             * If the timer is already running, then we don't do anything.
             * Hence, we only respond to one event every 100 milliseconds.
             * Cunning!!
             */
            if (tmrSelect.Enabled)
            {
                return;
            }
            tmrSelect.Interval = 100;
            tmrSelect.Enabled = true;
            tmrSelect.Start();
        }

        private void SelectedIndexChanged()
        {
            // This is the real event handler, after we've suppressed rapid multiple events
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(SelectedIndexChanged));
                return;
            }

            if (lvSkills.SelectedIndices.Count == 0)
            {
                tsbMoveUp.Enabled = false;
                tsbMoveDown.Enabled = false;
            }
            else
            {
                tsbMoveUp.Enabled = (lvSkills.SelectedIndices[0] != 0);
                tsbMoveDown.Enabled = (lvSkills.SelectedIndices[lvSkills.SelectedIndices.Count - 1] != lvSkills.Items.Count - 1);
            }

            // Update the colour of all items
            foreach (ListViewItem current in lvSkills.Items)
            {
                bool isSameSkill = false;
                bool isPreRequisite = false;
                bool isPostRequisite = false;

                // For single selection, we also check now for pre-requisite highlighting
                // Multi-selection is dealt with later
                if (!m_WorksafeMode && m_HighlightPrerequisites && lvSkills.SelectedItems.Count == 1)
                {
                    // Single select so check for pre-requisite highlighting
                    Plan.Entry currentSkill = current.Tag as Plan.Entry;
                    Plan.Entry selectedSkill = lvSkills.SelectedItems[0].Tag as Plan.Entry;
                    if (currentSkill != null && selectedSkill != null)
                    {
                        int neededLevel;
                        if (currentSkill.Skill.HasAsImmedPrereq(selectedSkill.Skill, out neededLevel))
                        {
                            if (currentSkill.Level == 1 && neededLevel >= selectedSkill.Level)
                            {
                                isPostRequisite = true;
                            }
                        }
                        if (selectedSkill.Skill.HasAsImmedPrereq(currentSkill.Skill, out neededLevel))
                        {
                            if (currentSkill.Level == neededLevel)
                            {
                                isPreRequisite = true;
                            }
                        }
                        if (currentSkill.SkillName == selectedSkill.SkillName)
                        {
                            isSameSkill = true;
                        }
                    }
                }

                if (current.Tag is Plan.RemappingPoint) 
                {
                    current.StateImageIndex = 3;
                }
                else if (isSameSkill)
                {
                    //current.BackColor = Color.LightYellow;
                    current.StateImageIndex = 1;
                }
                else if (isPreRequisite)
                {
                    //current.BackColor = Color.LightPink;
                    current.StateImageIndex = 2;
                }
                else if (isPostRequisite)
                {
                    //current.BackColor = Color.LightGreen;
                    current.StateImageIndex = 0;
                }
                else
                {
                    //current.BackColor = lvSkills.BackColor;
                    current.StateImageIndex = -1;
                }
            }

            // Multi-selection, check for prerequisites
            if (lvSkills.SelectedItems.Count > 1)
            {
                List<Skill> countedSkills = new List<Skill>();
                TimeSpan selectedTrainTime = TimeSpan.Zero;
                TimeSpan selectedTimeWithLearning = TimeSpan.Zero;
                long cost = 0;
                int cumulativeSkillTotal = m_plan.GrandCharacterInfo.SkillPointTotal;
                int entriesCount = 0;

                // need to loop through all entries to include effect of training skills
                // in the total time of selected skills.
                EveAttributeScratchpad scratchpad = new EveAttributeScratchpad();
                for (int i = 0; i < lvSkills.Items.Count; i++)
                {
                    ListViewItem lvi = lvSkills.Items[i];
                    Plan.Entry pe = lvi.Tag as Plan.Entry;
                    if (pe != null)
                    {
                        entriesCount++;
                        Skill gs = pe.Skill;
                        TimeSpan trainTime = gs.GetTrainingTimeOfLevelOnly(pe.Level, cumulativeSkillTotal, true, scratchpad);
                        cumulativeSkillTotal += gs.GetPointsForLevelOnly(pe.Level, true);
                        if (lvSkills.SelectedItems.Contains(lvi))
                        {
                            // ensure cost is only counted once!
                            if (!countedSkills.Contains(gs))
                            {
                                countedSkills.Add(gs);
                                if (!gs.Known && !gs.Owned)
                                {
                                    cost += gs.Cost;
                                }
                            }
                            selectedTrainTime += gs.GetTrainingTimeOfLevelOnly(pe.Level, cumulativeSkillTotal, true, null);
                            selectedTimeWithLearning += trainTime;
                        }
                        scratchpad.ApplyALevelOf(gs);
                    }
                }

                String sb = String.Format("{0} Skills selected, Training time: {1}", entriesCount,
                    Skill.TimeSpanToDescriptiveText(selectedTrainTime, DescriptiveTextOptions.IncludeCommas));

                if (selectedTimeWithLearning != selectedTrainTime)
                {
                    sb += String.Format(" ({0} with preceding learning skills)", Skill.TimeSpanToDescriptiveText(selectedTimeWithLearning, DescriptiveTextOptions.IncludeCommas));
                }
                if (cost > 0)
                {
                    sb += String.Format(", cost of unknown skills is {0:0,0,0}", cost);
                }
                m_plannerWindow.UpdateStatusBarSelected(sb);
            }
            else
            {
                // reset the status bar to normal in case multiple items were previously selected
                m_plannerWindow.UpdateStatusBar();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pscPlan.Panel2Collapsed = !pscPlan.Panel2Collapsed;
            tsbToggleSkills.Checked = !pscPlan.Panel2Collapsed;
            pscPlan.SplitterDistance = pscPlan.Width - 200;
            UpdateListColumns();
        }

        private static Skill GetDraggingSkill(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode"))
            {
                return (Skill)((TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode")).Tag;
            }
            return null;
        }

        private MouseButtons m_dragButton = MouseButtons.None;

        private void SetDragMouseButton(DragEventArgs e)
        {
            if ((e.KeyState & 1) == 1)
            {
                m_dragButton = MouseButtons.Left;
            }
            else if ((e.KeyState & 2) == 2)
            {
                m_dragButton = MouseButtons.Right;
            }
        }

        private ListViewItem BuildPlanItemForSkill(Skill gs)
        {
            int newLevel = m_plan.PlannedLevel(gs) + 1;
            if (gs.Level >= newLevel)
            {
                newLevel = gs.Level + 1;
            }
            if (newLevel > 5) return new ListViewItem("ERROR");

            Plan.Entry newEntry = new Plan.Entry();
            newEntry.SkillName = gs.Name;
            newEntry.Level = newLevel;
            newEntry.EntryType = Plan.Entry.Type.Planned;

            ListViewItem newItem = new ListViewItem(gs.Name + " " + Skill.GetRomanForInt(newLevel));
            newItem.Tag = newEntry;
            return newItem;
        }

        private void lvSkills_DragDrop(object sender, DragEventArgs e)
        {
            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill != null)
            {
                Point cp = lvSkills.PointToClient(new Point(e.X, e.Y));
                ListViewItem hoverItem = lvSkills.GetItemAt(cp.X, cp.Y);
                int dragIndex = lvSkills.Items.Count;
                if (hoverItem != null)
                {
                    Rectangle hoverBounds = hoverItem.GetBounds(ItemBoundsPortion.ItemOnly);
                    dragIndex = hoverItem.Index;
                    if (cp.Y > (hoverBounds.Top + (hoverBounds.Height / 2)))
                    {
                        dragIndex++;
                    }
                }
                if (m_dragButton == MouseButtons.Left || m_dragButton == MouseButtons.Right)
                {
                    ListViewItem newItem = BuildPlanItemForSkill(dragSkill);
                    if (newItem.Text != "ERROR")
                    {
                        lvSkills.Items.Insert(dragIndex, newItem);
                    }
                }
                //FIXME: need to add a context menu that pops up here and lets you pick a higher
                // level when dragging, but at present I am unsure how to do that and want to get
                // the basic feature in :)
                //else if (m_dragButton == MouseButtons.Right)
                //{
                //    MessageBox.Show("Right Drag -> Drop");
                //    // pop context menu to pick level (check skill select context menu for specifics)
                //}
                lvSkills.ClearDropMarker();
                e.Effect = DragDropEffects.None;
                m_dragButton = MouseButtons.None;
                RebuildPlanFromListViewOrder(lvSkills.Items);
            }
        }

        private void lvSkills_DragOver(object sender, DragEventArgs e)
        {
            SetDragMouseButton(e);
            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill != null)
            {
                e.Effect = DragDropEffects.Move;
                Point cp = lvSkills.PointToClient(new Point(e.X, e.Y));
                ListViewItem hoverItem = lvSkills.GetItemAt(cp.X, cp.Y);
                if (hoverItem != null)
                {
                    Rectangle hoverBounds = hoverItem.GetBounds(ItemBoundsPortion.ItemOnly);
                    lvSkills.DrawDropMarker(hoverItem.Index, (cp.Y > (hoverBounds.Top + (hoverBounds.Height / 2))));
                    if ((e.KeyState & 8) == 8 && System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
                else
                {
                    lvSkills.ClearDropMarker();
                }
            }
        }

        private void lvSkills_DragEnter(object sender, DragEventArgs e)
        {
            SetDragMouseButton(e);
            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill != null)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void lvSkills_DragLeave(object sender, EventArgs e)
        {
            m_dragButton = MouseButtons.None;
        }

        private void tsbToggleRemapping_Click(object sender, EventArgs e)
        {
            if (this.lvSkills.SelectedIndices.Count != 0)
            {
                int focusedId = 0;
                var item = this.lvSkills.SelectedItems[0];
                var tag = item.Tag;

                lvSkills.BeginUpdate();
                try
                {

                    // Remove an existing point
                    if (tag is Plan.RemappingPoint)
                    {
                        var entryIndex = this.lvSkills.SelectedItems[0].Index + 1;
                        var entry = this.lvSkills.Items[entryIndex].Tag as Plan.Entry;
                        entry.Remapping = null;
                        this.lvSkills.SelectedIndices.Add(entryIndex);
                        focusedId = GetID(lvSkills.Items[entryIndex]);
                    }
                    // Toggle on a skill
                    else
                    {
                        var entryIndex = this.lvSkills.SelectedIndices[0];
                        var entry = tag as Plan.Entry;

                        // Add a remapping point
                        if (entry.Remapping == null)
                        {
                            entry.Remapping = new Plan.RemappingPoint();
                            focusedId = GetID(item);
                        }
                        // Remove a remapping point
                        else
                        {
                            entry.Remapping = null;
                            focusedId = GetID(item);
                        }
                    }
                }
                finally
                {
                    var selection = StoreSelection();
                    this.UpdateSkillList();
                    this.RestoreSelection(selection);
                    lvSkills.FocusedItem = GetItemById(focusedId);
                    lvSkills.EndUpdate();
                }
            }
        }
    }
}
