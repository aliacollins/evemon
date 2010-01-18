using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Scheduling;
using System.Drawing;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.InteropServices;
using EVEMon.Common.SettingsObjects;
using EVEMon.Common.Threading;
using EVEMon.Common.Data;
using EVEMon.Common.Controls;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Provides a way for implant calculator and attributes optimization form to add a column showing the training time difference.
    /// </summary>
    public interface IPlanOrderPluggable
    {
        event EventHandler Disposed;
        void UpdateStatistics(BasePlan plan, out bool areRemappingPointsActive);
    }

    /// <summary>
    /// The main control of the plan editor window, the list of plan entries.
    /// </summary>
    public partial class PlanEditorControl : UserControl
    {
        private const int ArrowUpIndex = 4;
        private const int ArrowDownIndex = 5;

        private Font m_plannedSkillFont;
        private Font m_prerequisiteSkillFont;
        private Color m_nonImmedTrainablePlanEntryColor;
        private Color m_remappingBackColor;
        private Color m_remappingForeColor;

        private Character m_character;

        /// <summary>
        /// To enable the three-state columns, we need to persist the base plan, whose order is unchanged, and the sorted plan, which represents the plan the way it is displayed.
        /// </summary>
        private PlanScratchpad m_displayPlan;
        private Plan m_plan;

        // The ImplantsControl or the AttributesOptimizationForm
        private IPlanOrderPluggable m_pluggable = null;

        // Sort key are identified 
        private PlanEntrySort m_columnWithSortFeedback = PlanEntrySort.None;

        // Drag'n drop
        private MouseButtons m_dragButton = MouseButtons.None;

        // Columns
        private bool m_isUpdatingColumns;
        private bool m_columnsOrderChanged;
        private readonly List<PlanColumnSettings> m_columns = new List<PlanColumnSettings>();


        /// <summary>
        /// Constructor
        /// </summary>
        public PlanEditorControl()
        {
            InitializeComponent();
            this.pscPlan.RememberDistanceKey = "PlanEditor";

            m_columns.AddRange(Settings.UI.PlanWindow.Columns.Select(x => x.Clone()));

            m_plannedSkillFont = FontFactory.GetFont(lvSkills.Font, System.Drawing.FontStyle.Bold);
            m_prerequisiteSkillFont = FontFactory.GetFont(lvSkills.Font, System.Drawing.FontStyle.Regular);
            m_nonImmedTrainablePlanEntryColor = SystemColors.GrayText;
            m_remappingForeColor = SystemColors.HotTrack;
            m_remappingBackColor = SystemColors.Info;

            lvSkills.ColumnWidthChanged += new ColumnWidthChangedEventHandler(lvSkills_ColumnWidthChanged);
            lvSkills.ColumnClick += new ColumnClickEventHandler(lvSkills_ColumnClick);
            tsSortLearning.Click += new EventHandler(tsSortLearning_Clicked);
            tsSortPriorities.Click += new EventHandler(tsSortPriorities_Clicked);

            EveClient.CharacterChanged += new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.PlanChanged += new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            EveClient.SettingsChanged += new EventHandler(EveClient_SettingsChanged);
            EveClient.TimerTick += new EventHandler(EveClient_TimerTick);
            this.Disposed += new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDisposed(object sender, EventArgs e)
        {
            EveClient.CharacterChanged -= new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.PlanChanged -= new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            EveClient.SettingsChanged -= new EventHandler(EveClient_SettingsChanged);
            EveClient.TimerTick -= new EventHandler(EveClient_TimerTick);
            this.Disposed -= new EventHandler(OnDisposed);
        }

        /// <summary>
        /// On load, updates the controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (this.DesignMode || this.IsDesignModeHosted()) return;
            base.OnLoad(e);
        }

        /// <summary>
        /// Gets or sets the plan represented by this editor.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan == value) return;
                m_plan = value;
                m_character = (Character)value.Character;
                m_displayPlan = new PlanScratchpad(m_character);
                UpdateDisplayPlan();

                // Children controls
                this.skillSelectControl.Plan = value;

                // Update the list
                lvSkills.BeginUpdate();
                try
                {
                    UpdateListColumns();
                    UpdateSkillList(false);
                    UpdateSortVisualFeedback();
                }
                finally
                {
                    lvSkills.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Gets the version of the plan as it is currently displayed.
        /// </summary>
        public PlanScratchpad DisplayPlan
        {
            get { return m_displayPlan; }
        }

        /// <summary>
        /// Gets the character this control is bound to.
        /// </summary>
        public Character Character
        {
            get { return (Character)m_plan.Character; }
        }

        /// <summary>
        /// Gets the number of unique skills selected (two levels of same skill counts for one unique skill).
        /// </summary>
        public int UniqueSkillsCount
        {
            get
            {
                int count = 0;
                bool[] counted = new bool[StaticSkills.ArrayIndicesCount];

                // Scroll through selection
                foreach ( var entry in SelectedEntries)
                {
                    int index = entry.Skill.ArrayIndex;
                    if (!counted[index])
                    {
                        counted[index] = true;
                        count++;
                    }
                }

                // Return the count
                return count;
            }
        }
        
        /// <summary>
        /// Gets the number of not known skills selected (two levels of same skill counts for one unique skill).
        /// </summary>
        public int NotKnownSkillsCount
        {
            get
            {
                int count = 0;
                bool[] counted = new bool[StaticSkills.ArrayIndicesCount];

                // Scroll through selection
                foreach (var entry in SelectedEntries)
                {
                    int index = entry.Skill.ArrayIndex;
                    if (!counted[index] && !entry.CharacterSkill.IsKnown && !entry.CharacterSkill.IsOwned)
                    {
                        counted[index] = true;
                        count++;
                    }
                }

                // Return the count
                return count;
            }
        }
        
        /// <summary>
        /// Gets the cost of known skills selected
        /// </summary>
        public long SkillBooksCost
        {
            get
            {
                long cost = 0;
                bool[] counted = new bool[StaticSkills.ArrayIndicesCount];
                
                // Scroll through selection
                foreach (var entry in SelectedEntries)
                {
                    int index = entry.Skill.ArrayIndex;
                    if (!counted[index])
                    {
                        counted[index] = true;
                        cost += entry.Skill.Cost;
                    }
                }

                // Return the cost
                return cost;
            }
        }
        
        /// <summary>
        /// Gets the cost of not known skills selected
        /// </summary>
        public long NotKnownSkillBooksCost
        {
            get
            {
                long cost = 0;
                bool[] counted = new bool[StaticSkills.ArrayIndicesCount];
                
                // Scroll through selection
                foreach (var entry in SelectedEntries)
                {
                    int index = entry.Skill.ArrayIndex;
                    if (!counted[index] && !entry.CharacterSkill.IsKnown && !entry.CharacterSkill.IsOwned)
                    {
                        counted[index] = true;
                        cost += entry.Skill.Cost;
                    }
                }

                // Return the cost
                return cost;
            }
        }

        #region Global events
        /// <summary>
        /// When the character change, some entries may now be known, so we reupdate everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveClient_CharacterChanged(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character) return;
            UpdateDisplayPlan();
            UpdateSkillList(true);
        }

        /// <summary>
        /// When the plan changed, entries may have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            UpdateDisplayPlan();
            UpdateSkillList(true);
            UpdateListColumns();
        }

        /// <summary>
        /// When the settings changed, the highlights and such may be different. 
        /// Entries are still the same but we may need to update highlights and others
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateSkillList(true);
        }

        /// <summary>
        /// When the columns changed in one of the window, we update all the windows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveClient_PlanColumnsChanged(object sender, EventArgs e)
        {
            UpdateListColumns();
        }

        /// <summary>
        /// Per second check if columns have been reordered.
        /// When true, column settings get saved and reloaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveClient_TimerTick(object sender, EventArgs e)
        {
            if (m_columnsOrderChanged)
            {
                Settings.UI.PlanWindow.Columns = ExportColumnSettings().ToArray();
                m_columns.Clear();
                m_columns.AddRange(Settings.UI.PlanWindow.Columns.Select(x => x.Clone()));
                UpdateListColumns();
            }
            m_columnsOrderChanged = false;
        }
        #endregion


        #region Content creation and refresh
        /// <summary>
        /// Whenever the sorting option or the base plan changed, we update the sorted plan.
        /// </summary>
        private void UpdateDisplayPlan()
        {
            m_displayPlan.RebuildPlanFrom(m_plan, true);

            // Share the remapping points
            var srcEntries = m_plan.ToArray();
            var destEntries = m_displayPlan.ToArray();
            for (int i = 0; i < srcEntries.Length; i++)
            {
                destEntries[i].Remapping = srcEntries[i].Remapping;
            }

            // Apply the sort
            m_displayPlan.Sort(m_plan.SortingPreferences);
        }

        /// <summary>
        /// Rebuild the list items from the entries and their remapping points. Plan entries items are not fully initialized, 
        /// only their tags and a couple of things.
        /// Full intialization, especially the columns values and such, will be completed in <see cref="UpdateListViewItems"/>.
        /// </summary>
        /// <param name="restoreSelectionAndFocus">When false, selection and focus will be reseted.</param>
        private void UpdateSkillList(bool restoreSelectionAndFocus)
        {
            // Disable controls, they will be restored one the selection is updated
            tsbMoveUp.Enabled = false;
            tsbMoveDown.Enabled = false;
            tmrAutoRefresh.Enabled = false;

            // Stores selection and focus, to restore them after the update
            var selection = (restoreSelectionAndFocus ? StoreSelection() : null);
            int focusedHashCode = (restoreSelectionAndFocus && lvSkills.FocusedItem != null ? lvSkills.FocusedItem.Tag.GetHashCode() : 0);

            lvSkills.BeginUpdate();
            try
            {
                // Scroll through entries and their remapping points.
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (var entry in m_displayPlan)
                {
                    // In any case, we must insert a new item for this plan's entry at the current index
                    // Do we need to insert a remapping point ?
                    if (entry.Remapping != null)
                    {
                        ListViewItem rlv = new ListViewItem();
                        rlv.BackColor = m_remappingBackColor;
                        rlv.ForeColor = m_remappingForeColor;
                        rlv.UseItemStyleForSubItems = true;
                        rlv.Text = entry.Remapping.ToString();
                        rlv.Tag = entry.Remapping;
                        rlv.ImageIndex = 3;
                        items.Add(rlv);
                    }

                    // Insert the entry
                    ListViewItem lvi = new ListViewItem();
                    lvi.Tag = entry;
                    items.Add(lvi);

                    // Is it a prerequisite or a top level entry ?
                    if (!Settings.UI.SafeForWork && Settings.UI.PlanWindow.HighlightPlannedSkills && entry.Type == PlanEntryType.Planned)
                    {
                        lvi.Font = m_plannedSkillFont;
                    }
                    else
                    {
                        lvi.Font = m_prerequisiteSkillFont;
                    }

                    // Gray out entries that cannot be trained immediately.
                    if (!entry.CanTrainNow && Settings.UI.PlanWindow.DimUntrainable)
                    {
                        lvi.ForeColor = m_nonImmedTrainablePlanEntryColor;
                    }

                    // Enable refresh every 30s if a skill is in training.
                    Skill skill = entry.CharacterSkill;
                    if (skill.IsTraining) tmrAutoRefresh.Enabled = true;
                }

                // We avoid clear + AddRange because it causes the sliders's position to reset
                while (lvSkills.Items.Count > 1) lvSkills.Items.RemoveAt(lvSkills.Items.Count - 1);
                bool isEmpty = (lvSkills.Items.Count == 0);
                lvSkills.Items.AddRange(items.ToArray());
                if (!isEmpty) lvSkills.Items.RemoveAt(0);

                // Complete the items initialization
                UpdateListViewItems();

                // Restore selection and focus
                if (restoreSelectionAndFocus)
                {
                    RestoreSelection(selection);
                    var focusedItem = lvSkills.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Tag.GetHashCode() == focusedHashCode);
                    if (focusedItem != null) focusedItem.Focused = true;
                }
            }
            finally
            {
                lvSkills.EndUpdate();
            }
        }

        /// <summary>
        /// Completes the items initialization or refresh them. Especially their columns values.
        /// </summary>
        private void UpdateListViewItems()
        {
            // When there is a pluggable (implants calculator or attributes optimizer)
            // This one provides us the scratchpad to update training times.
            bool areRemappingPointsActive = true;
            if (m_pluggable != null)
            {
                m_pluggable.UpdateStatistics(m_displayPlan, out areRemappingPointsActive);
            }
            else
            {
                var scratchpad = new CharacterScratchpad(m_character);
                m_displayPlan.UpdateStatistics(scratchpad, true, true);
            }

            // Start updating the list
            lvSkills.BeginUpdate();
            try
            {
                NumberFormatInfo nfi = NumberFormatInfo.CurrentInfo;

                // Scroll through entries
                for (int i = 0; i < lvSkills.Items.Count; i++)
                {
                    ListViewItem lvi = lvSkills.Items[i];
                    PlanEntry entry = lvi.Tag as PlanEntry;
                    if (entry != null)
                    {
                        // Add enough subitems to match the number of columns
                        while (lvi.SubItems.Count < lvSkills.Columns.Count)
                        {
                            lvi.SubItems.Add(String.Empty);
                        }

                        // Checks if this entry has not prereq-met
                        if (!entry.CharacterSkill.IsKnown) lvi.ForeColor = Color.Red;

                        // Checks if this entry is a non-public skill
                        if (!entry.CharacterSkill.IsPublic) lvi.ForeColor = Color.DarkRed;

                        // Checks if this entry is not known but has prereq-met
                        if (!entry.CharacterSkill.IsKnown && entry.CharacterSkill.IsPublic && entry.CharacterSkill.ArePrerequisitesMet)
                        {
                            lvi.ForeColor = Color.LightSlateGray;
                        }
                        
                        // Checks if this entry is partially trained
                        bool s_level = (entry.Level == entry.CharacterSkill.Level + 1);
                        if (Settings.UI.PlanWindow.HighlightPartialSkills)
                        {
                            bool s_partial = (entry.CharacterSkill.FractionCompleted > 0 && entry.CharacterSkill.FractionCompleted < 1);
                            bool isPartiallyTrained = (s_level && s_partial);
                            if (isPartiallyTrained) lvi.ForeColor = Color.Green;
                        }

                        // Checks if this entry is queued
                        if (Settings.UI.PlanWindow.HighlightQueuedSkills)
                        {
                            SkillQueue s_skill = new SkillQueue(m_character as CCPCharacter);
                            var queuedskill = s_skill.m_character.SkillQueue;
                            string e_skill = entry.ToString();
                            foreach (var skill in queuedskill)
                            {   
                                string q_skill = skill.ToString();
                                if (e_skill == q_skill) lvi.ForeColor = Color.RoyalBlue;
                            }
                        }

                        // Checks if this entry is currently training (even if it's paused)
                        if (entry.CharacterSkill.IsTraining && s_level) 
                        { 
                            lvi.BackColor = Color.LightSteelBlue; lvi.ForeColor = Color.Black; 
                        }

                        // Checks whether this entry will be blocked.
                        string blockingEntry = string.Empty;
                        if (Settings.UI.PlanWindow.HighlightConflicts)
                        {
                            bool isBlocked = Scheduler.SkillIsBlockedAt(entry.EndTime, out blockingEntry);
                            if (isBlocked) {lvi.ForeColor = Color.Red; lvi.BackColor = Color.LightGray; }
                        }

                        // Update every column
                        lvi.UseItemStyleForSubItems = (m_pluggable == null);
                        for (int columnIndex = 0; columnIndex < lvSkills.Columns.Count; columnIndex++)
                        {
                            // Regular columns (not pluggable-dependent)
                            if (lvSkills.Columns[columnIndex].Tag != null)
                            {
                                var columnSettings = (PlanColumnSettings)lvSkills.Columns[columnIndex].Tag;

                                lvi.SubItems[columnIndex].ForeColor = lvi.ForeColor;
                                lvi.SubItems[columnIndex].Text = GetColumnTextForItem(entry, columnSettings.Column, blockingEntry, nfi);
                            }
                            // Training time differences
                            else
                            {
                                TimeSpan timeDifference;
                                string result = "";
                                if (entry.OldTrainingTime < entry.TrainingTime)
                                {
                                    result = "+";
                                    timeDifference = entry.TrainingTime - entry.OldTrainingTime;
                                    lvi.SubItems[columnIndex].ForeColor = Color.DarkRed;
                                }
                                else if (entry.OldTrainingTime > entry.TrainingTime)
                                {
                                    result = "-";
                                    timeDifference = entry.OldTrainingTime - entry.TrainingTime;
                                    lvi.SubItems[columnIndex].ForeColor = Color.DarkGreen;
                                }
                                else
                                {
                                    timeDifference = TimeSpan.Zero;
                                    lvi.SubItems[columnIndex].ForeColor = lvi.ForeColor;
                                }
                                result += Skill.TimeSpanToDescriptiveText(timeDifference, DescriptiveTextOptions.IncludeCommas);
                                lvi.SubItems[columnIndex].Text = result;
                            }
                        }
                    }
                    // The item represents a remapping point
                    else
                    {
                        var point = (RemappingPoint)lvi.Tag;
                        lvi.Text = (areRemappingPointsActive ? point.ToString() : "Remapping (ignored)");

                        // When remapping points are ignored, display their value in the "diff" column
                        if (!areRemappingPointsActive)
                        {
                            // Add enough subitems to match the number of columns
                            while (lvi.SubItems.Count < lvSkills.Columns.Count)
                            {
                                var subItem = lvi.SubItems.Add(String.Empty);
                                if (lvSkills.Columns[lvi.SubItems.Count - 1].Tag == null)
                                {
                                    subItem.Text = point.ToShortString();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                lvSkills.EndUpdate();
                UpdateStatusBar();
            }
        }

        /// <summary>
        /// Gets the text to display in the given column for the provided entry
        /// </summary>
        /// <param name="pe"></param>
        /// <param name="column"></param>
        /// <param name="blockingEntry"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private string GetColumnTextForItem(PlanEntry entry, PlanColumn column, string blockingEntry, NumberFormatInfo format)
        {
            const int MaxNotesLength = 60;

            switch (column)
            {
                case PlanColumn.SkillName:
                    return entry.ToString();

                case PlanColumn.PlanGroup:
                    return entry.PlanGroupsDescription;

                case PlanColumn.TrainingTime:
                    return Skill.TimeSpanToDescriptiveText(entry.TrainingTime, DescriptiveTextOptions.IncludeCommas, false);

                case PlanColumn.TrainingTimeNatural:
                    return Skill.TimeSpanToDescriptiveText(entry.NaturalTrainingTime, DescriptiveTextOptions.IncludeCommas, false);

                case PlanColumn.EarliestStart:
                    return entry.StartTime.ToString("ddd ") + entry.StartTime.ToString();

                case PlanColumn.EarliestEnd:
                    return entry.EndTime.ToString("ddd ") + entry.EndTime.ToString();

                case PlanColumn.PercentComplete:
                    return entry.FractionCompleted.ToString("0%");

                case PlanColumn.SkillRank:
                    return entry.Skill.Rank.ToString();

                case PlanColumn.PrimaryAttribute:
                    return entry.Skill.PrimaryAttribute.ToString();

                case PlanColumn.SecondaryAttribute:
                    return entry.Skill.SecondaryAttribute.ToString();

                case PlanColumn.SkillGroup:
                    return entry.Skill.Group.Name;

                case PlanColumn.PlanType:
                    return entry.Type.ToString();

                case PlanColumn.SPTotal:
                    return entry.EstimatedTotalSkillPoints.ToString("N00", format);

                case PlanColumn.SPPerHour:
                    return entry.SpPerHour.ToString();

                case PlanColumn.Priority:
                    return entry.Priority.ToString();

                case PlanColumn.Conflicts:
                    return blockingEntry;

                case PlanColumn.Notes:
                    if (String.IsNullOrEmpty(entry.Notes)) return "";

                    string result = Regex.Replace(entry.Notes, @"(\r|\n)+", " ", RegexOptions.None);
                    if (result.Length <= MaxNotesLength) return result;

                    return result.Substring(0, MaxNotesLength) + "...";

                case PlanColumn.Cost:
                    if (entry.Level != 1) return "";
                    if (entry.CharacterSkill.IsKnown) return "";
                    if (entry.CharacterSkill.IsOwned) return "Owned";
                    return entry.Skill.FormattedCost;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Update the columns list according to the settings.
        /// </summary>
        public void UpdateListColumns()
        {
            lvSkills.BeginUpdate();
            m_isUpdatingColumns = true;
            try
            {
                // Clear and add the columns
                lvSkills.Columns.Clear();
                foreach (var column in m_columns.Where(x => x.Visible))
                {
                    // Add the column
                    ColumnHeader header = new ColumnHeader();
                    header.Text = column.Column.GetHeader();
                    header.Tag = column;
                    header.Width = column.Width;
                    lvSkills.Columns.Add(header);

                    // Add a temporary column when there is a pluggable (implants calc attributes optimizer, erc)
                    if (m_pluggable != null && column.Column == PlanColumn.TrainingTime)
                    {
                        header = new ColumnHeader();
                        header.Tag = null;
                        header.Text = "Diff with Calc Atts";
                        header.Width = column.Width;
                        lvSkills.Columns.Add(header);
                    }
                }

                // Update the items
                UpdateListViewItems();

                // Update the sort arrows
                UpdateSortVisualFeedback();

                // Force the auto-update of the columns with -1 width
                var resizeStyle = (lvSkills.Items.Count == 0 ? 
                    ColumnHeaderAutoResizeStyle.HeaderSize : 
                    ColumnHeaderAutoResizeStyle.ColumnContent);

                int index = 0;
                foreach (var column in m_columns.Where(x => x.Visible))
                {
                    if (column.Width == -1)
                    {
                        lvSkills.AutoResizeColumn(index, resizeStyle);
                    }
                    index++;
                }
            }
            finally
            {
                lvSkills.EndUpdate();
                m_isUpdatingColumns = false;
            }
        }

        /// <summary>
        /// Stores the selection to a dictionary and returns it. Dictionary keys are the tags' hash codes.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, bool> StoreSelection()
        {
            Dictionary<int, bool> c = new Dictionary<int, bool>();

            // Compute and store a string ID for every item
            foreach (ListViewItem lvi in lvSkills.SelectedItems)
            {
                c[lvi.Tag.GetHashCode()] = true;
            }

            return c;
        }

        /// <summary>
        /// Restores the selection from a dictionary where keys are tags' hash codes.
        /// </summary>
        /// <param name="c"></param>
        private void RestoreSelection(Dictionary<int, bool> c)
        {
            for (int i = this.lvSkills.Items.Count - 1; i >= 0; i--)
            {
                // Retrieve this item's tag hash code
                ListViewItem lvi = lvSkills.Items[i];
                int hashCode = lvi.Tag.GetHashCode();

                // Check whether this id must be selected
                bool selected = false;
                if (c.TryGetValue(hashCode, out selected))
                {
                    c.Remove(hashCode);
                }
                lvi.Selected = selected;
            }
        }

        /// <summary>
        /// Every 30s a timer ticks and causes the list to refresh.
        /// If there are obsolete entries user gets asked how to handle them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrAutoRefresh_Tick(object sender, EventArgs e)
        {
            var window = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            if (window == null)
                return;

            window.CheckObsoleteEntries();
            UpdateListViewItems();
        }

        /// <summary>
        /// Removes all obsolete entries and rebuilds the plan 
        /// </summary>
        public void ClearObsoleteEntries()
        {
            m_plan.CleanObsoleteEntries();
            UpdateDisplayPlan();
            UpdateSkillList(true);
        }

        /// <summary>
        /// Updates the status bar
        /// </summary>
        private void UpdateStatusBar()
        {
            var window = WindowsFactory<PlanWindow>.GetByTag(m_plan);

            if (window == null)
                return;

            // 1 or fewer items are selected and status bar only updates on multi-select
            if (lvSkills.SelectedItems.Count < 2 && Settings.UI.PlanWindow.OnlyShowSelectionSummaryOnMultiSelect)
            {
                window.UpdateStatusBar();
                return;
            }
            
            // 0 items selected
            if (lvSkills.SelectedItems.Count < 1)
            {
                window.UpdateStatusBar();
                return;
            }

            // Multi-selection
            TimeSpan selectedTrainTime = TimeSpan.Zero;
            TimeSpan selectedTimeWithLearning = TimeSpan.Zero;
            int entriesCount = 0;
            long sbcost = SkillBooksCost;
            long nksbcost = NotKnownSkillBooksCost;

            // We compute the training time
            foreach (var entry in SelectedEntries)
            {
                selectedTrainTime += entry.CharacterSkill.GetLeftTrainingTimeForLevelOnly(entry.Level);
                selectedTimeWithLearning += entry.TrainingTime;
                entriesCount++;
            }

            // Appends the string to display in the status bar.
            String sb = String.Format("{0} Skill{1} selected ({2} Unique Skill{3}). Training time: {4}. ", 
                    entriesCount,
                    entriesCount == 1 ? "" : "s",
                    UniqueSkillsCount, 
                    UniqueSkillsCount == 1 ? "" : "s",
                    Skill.TimeSpanToDescriptiveText(selectedTrainTime, DescriptiveTextOptions.IncludeCommas));

            if (selectedTimeWithLearning != selectedTrainTime)
            {
                sb += String.Format("({0} with preceding learning skills). ",
                    Skill.TimeSpanToDescriptiveText(selectedTimeWithLearning, DescriptiveTextOptions.IncludeCommas));
            }
            
            if (sbcost > 0)
            {
                sb += String.Format("Skill book{0} cost : {1:0,0,0} ISK. ", UniqueSkillsCount == 1 ? "" : "s", sbcost);
            }

            if (entriesCount > 1 && nksbcost > 0)
            {
                sb += String.Format("Not known skill book{0} cost : {1:0,0,0} ISK. ", NotKnownSkillsCount == 1 ? "" : "s", nksbcost);
            }
            
            window.UpdateStatusBarSelected(sb);
        }
        #endregion


        #region Pluggable management
        /// <summary>
        /// Connects a window implementing <see cref="IPlanOrderPluggable"/> to this window to enable displaying the training time differences.
        /// </summary>
        /// <param name="pluggable"></param>
        internal void ShowWithPluggable(IPlanOrderPluggable pluggable)
        {
            lvSkills.BeginUpdate();
            try
            {
                // Update columns when a new pluggable is inserted
                if (m_pluggable == null)
                {
                    m_pluggable = pluggable;
                    pluggable.Disposed += new EventHandler(pluggable_Disposed);
                    UpdateListColumns();
                }

                // Updates the list view
                UpdateListViewItems();
            }
            finally
            {
                lvSkills.EndUpdate();
            }
        }

        /// <summary>
        /// Once the pluggable window is disposed, we hide the training time difference again.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void pluggable_Disposed(object o, EventArgs e)
        {
            m_pluggable.Disposed -= new EventHandler(pluggable_Disposed);
            m_pluggable = null;
            UpdateListColumns();
        }
        #endregion


        #region Generic helper methods
        /// <summary>
        /// From an entry of the display plan, retrieve the entry of the base plan.
        /// </summary>
        /// <param name="lvi"></param>
        /// <returns></returns>
        private PlanEntry GetOriginalEntry(PlanEntry displayEntry)
        {
            return m_plan.GetEntry(displayEntry.Skill, displayEntry.Level);
        }

        /// <summary>
        /// Gets the plan entry attached to the given item.
        /// </summary>
        /// <param name="lvi"></param>
        /// <returns></returns>
        private PlanEntry GetPlanEntry(ListViewItem lvi)
        {
            if (lvi == null) return null;
            return lvi.Tag as PlanEntry;
        }

        /// <summary>
        /// gets the first selected item which has a plan entry as a tag.
        /// </summary>
        /// <returns></returns>
        private PlanEntry GetFirstSelectedEntry()
        {
            PlanEntry pe = null;
            foreach (ListViewItem item in lvSkills.SelectedItems)
            {
                pe = lvSkills.SelectedItems[0].Tag as PlanEntry;
                if (pe != null) return pe;
            }
            return null;
        }

        /// <summary>
        /// Gets an item from its tag's hash code.
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns></returns>
        private ListViewItem GetItemByHashCode(int hashCode)
        {
            foreach (ListViewItem lvi in lvSkills.Items)
            {
                if (lvi.Tag.GetHashCode() == hashCode) return lvi;
            }
            return null;
        }

        /// <summary>
        /// Gets an enumeration over the selected entries.
        /// </summary>
        public IEnumerable<PlanEntry> SelectedEntries
        {
            get
            {
                return lvSkills.SelectedItems.Cast<ListViewItem>().Where(x => x.Tag is PlanEntry).Select(x => x.Tag as PlanEntry);
            }
        }
        #endregion


        #region List items and columns reordering/resizing
        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lvSkills_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns) return;
            if (m_columns.Count <= e.ColumnIndex) return;
            m_columns[e.ColumnIndex].Width = lvSkills.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// When the user click moves up, we move the list view items and rebuild the plan from this.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            var items = lvSkills.Items.Cast<ListViewItem>().ToList();

            // Skip the head
            int index = 0;
            while (index < items.Count)
            {
                if (!items[index].Selected) break;
                index++;
            }

            // Move up the following items
            while (index < items.Count)
            {
                var item = items[index];
                if (item.Selected)
                {
                    items.RemoveAt(index);
                    items.Insert(index - 1, item);
                }
                index++;
            }

            // Rebuild the plan
            RebuildPlanFromListViewOrder(items);
        }

        /// <summary>
        /// When the user click moves down, we move the list view items and rebuild the plan from this.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            var items = lvSkills.Items.Cast<ListViewItem>().ToList();

            // Skip the tail
            int index = items.Count - 1;
            while (index >= 0)
            {
                if (!items[index].Selected) break;
                index--;
            }

            // Move up the following items
            while (index >= 0)
            {
                var item = items[index];
                if (item.Selected)
                {
                    items.RemoveAt(index);
                    items.Insert(index + 1, item);
                }
                index--;
            }

            // Rebuild the plan
            RebuildPlanFromListViewOrder(items);
        }

        /// <summary>
        /// When an item is moved acrosss the listview, we rebuild the entire plan from the listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_ListViewItemsDragged(object sender, EventArgs e)
        {
            RebuildPlanFromListViewOrder(lvSkills.Items);
        }

        /// <summary>
        /// Rebuild the plan from the list view items.
        /// </summary>
        /// <param name="items"></param>
        private void RebuildPlanFromListViewOrder(IEnumerable items)
        {
            // Create the new entries
            List<PlanEntry> entries = new List<PlanEntry>();

            RemappingPoint remapping = null;
            foreach (ListViewItem item in items)
            {
                PlanEntry entry = item.Tag as PlanEntry;
                if (entry != null)
                {
                    entry.Remapping = remapping;
                    entries.Add(entry);
                    remapping = null;
                }
                else
                {
                    remapping = item.Tag as RemappingPoint;
                }
            }

            // Since the list is not sorted anymore, we disable/hide the sort buttons and feedback.
            m_plan.SortingPreferences.Order = ThreeStateSortOrder.None;
            m_plan.SortingPreferences.OptimizeLearning = false;
            m_plan.SortingPreferences.GroupByPriority = false;
            UpdateSortVisualFeedback();

            // Fetch them to the plan
            m_plan.RebuildPlanFrom(entries);
        }

        /// <summary>
        /// Rebuild the column settings from the currently displayed columns.
        /// </summary>
        public void ImportColumnSettings(IEnumerable<PlanColumnSettings> columns)
        {
            // Recreate the columns
            m_columns.Clear();
            foreach (var column in columns)
            {
                m_columns.Add(column.Clone());
            }

            // Update the UI
            UpdateListColumns();
        }

        /// <summary>
        /// Rebuild the column settings from the currently displayed columns.
        /// </summary>
        public List<PlanColumnSettings> ExportColumnSettings()
        {
            // Recreate the columns
            var newList = new List<PlanColumnSettings>();

            // Add the visible columns at the beggining
            foreach (ColumnHeader columnHeader in lvSkills.Columns.Cast<ColumnHeader>().ToArray().OrderBy(x => x.DisplayIndex))
            {
                // Retrieve the column and skip if null
                var column = columnHeader.Tag as PlanColumnSettings;
                if (column == null) continue;

                if (column.Width != -1) column.Width = columnHeader.Width;
                column.Visible = true;
                newList.Add(column);
            }

            // Then the non-displayed ones
            foreach (var column in m_columns.Where(x => !newList.Contains(x)))
            {
                column.Visible = false;
                newList.Add(column);
            }

            return newList;
        }
        #endregion


        #region Entries removal
        /// <summary>
        /// Remove all the selected entries when one or more getselected.
        /// </summary>
        private void RemoveSelectedEntries()
        {
            if (lvSkills.SelectedItems.Count == 0) return;

            var operation = PrepareSelectionRemoval();
            PlanHelper.PerformSilently(operation);
        }

        private IPlanOperation PrepareSelectionRemoval()
        {
            var entriesToRemove = lvSkills.SelectedItems.Cast<ListViewItem>().Where(x => x.Tag is PlanEntry).Select(x => (PlanEntry)x.Tag);
            var operation = m_plan.TryRemoveSet(entriesToRemove);
            return operation;
        }
        #endregion


        #region Sorting
        /// <summary>
        /// The user toggled the "group priorities" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsSortPriorities_Clicked(object sender, EventArgs e)
        {
            m_plan.SortingPreferences.GroupByPriority = tsSortPriorities.Checked;
            UpdateDisplayPlan();
            UpdateSkillList(true);
        }

        /// <summary>
        /// The user toggled the "sort learning" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsSortLearning_Clicked(object sender, EventArgs e)
        {
            m_plan.SortingPreferences.OptimizeLearning = tsSortLearning.Checked;
            UpdateDisplayPlan();
            UpdateSkillList(true);
        }

        /// <summary>
        /// When the user clicks a column header, we sort things up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var column = lvSkills.Columns[e.Column];
            var criteria = GetPlanSort(column);

            // Update sort order
            if (criteria != PlanEntrySort.None)
            {
                if (m_plan.SortingPreferences.Criteria == criteria)
                {
                    switch (m_plan.SortingPreferences.Order)
                    {
                        case ThreeStateSortOrder.None:
                            m_plan.SortingPreferences.Order = ThreeStateSortOrder.Ascending;
                            break;
                        case ThreeStateSortOrder.Ascending:
                            m_plan.SortingPreferences.Order = ThreeStateSortOrder.Descending;
                            break;
                        case ThreeStateSortOrder.Descending:
                            m_plan.SortingPreferences.Order = ThreeStateSortOrder.None;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    m_plan.SortingPreferences.Order = ThreeStateSortOrder.Ascending;
                }
            }

            // Updates the criteria
            m_plan.SortingPreferences.Criteria = criteria;

            // Updates UI and display plan
            UpdateSortVisualFeedback();
            UpdateDisplayPlan();
            UpdateSkillList(true);
        }

        /// <summary>
        /// Gets a column by the given sort key. Null if not found or "none".
        /// </summary>
        /// <param name="sortKey"></param>
        /// <returns></returns>
        private ColumnHeader GetColumn(PlanEntrySort criteria)
        {
            if (criteria == PlanEntrySort.None) return null;

            foreach (ColumnHeader header in this.lvSkills.Columns)
            {
                if (GetPlanSort(header) == criteria) return header;
            }
            return null;
        }

        /// <summary>
        /// Gets the sort key for the given column header.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private PlanEntrySort GetPlanSort(ColumnHeader header)
        {
            if (header.Tag == null) return PlanEntrySort.TimeDifference;

            var ct = (PlanColumnSettings)header.Tag;
            switch (ct.Column)
            {
                case PlanColumn.SkillName:
                    return PlanEntrySort.Name;
                case PlanColumn.Cost:
                    return PlanEntrySort.Cost;
                case PlanColumn.TrainingTime:
                    return PlanEntrySort.TrainingTime;
                case PlanColumn.TrainingTimeNatural:
                    return PlanEntrySort.TrainingTimeNatural;
                case PlanColumn.PrimaryAttribute:
                    return PlanEntrySort.PrimaryAttribute;
                case PlanColumn.SecondaryAttribute:
                    return PlanEntrySort.SecondaryAttribute;
                case PlanColumn.Priority:
                    return PlanEntrySort.Priority;
                case PlanColumn.SkillGroup:
                    return PlanEntrySort.SkillGroupDuration;
                case PlanColumn.PlanGroup:
                    return PlanEntrySort.PlanGroup;
                case PlanColumn.PercentComplete:
                    return PlanEntrySort.PercentCompleted;
                case PlanColumn.SkillRank:
                    return PlanEntrySort.Rank;
                case PlanColumn.SPPerHour:
                    return PlanEntrySort.SPPerHour;
                case PlanColumn.Notes:
                    return PlanEntrySort.Notes;
                case PlanColumn.PlanType:
                    return PlanEntrySort.PlanType;
                default:
                    return PlanEntrySort.None;
            }
        }

        /// <summary>
        /// Updates the sort visual feedback for the specified column.
        /// </summary>
        /// <remarks>
        /// The ColumnHeader.ImageIndex has a bug under Vista that
        /// causes the value to be set to 0 if you set it to -1,
        /// resulting in the wrong icon being selected for the sort:
        /// https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=395739
        /// </remarks>
        /// <param name="column">The column, may be null when no sort.</param>
        /// <param name="reversed">if set to <c>true</c> [reversed].</param>
        private void UpdateSortVisualFeedback()
        {
            // Updates the menu icons on the left toolbar
            this.tsSortLearning.Checked = m_plan.SortingPreferences.OptimizeLearning;
            this.tsSortPriorities.Checked = m_plan.SortingPreferences.GroupByPriority;


            // Removes the icon from the old column
            var lastColumn = GetColumn(m_columnWithSortFeedback);
            if (lastColumn != null) lastColumn.ImageIndex = 6; // see xml comments
            m_columnWithSortFeedback = m_plan.SortingPreferences.Criteria;


            // Adds the icon on the new column
            if (m_plan.SortingPreferences.Criteria != PlanEntrySort.None && m_plan.SortingPreferences.Order != ThreeStateSortOrder.None)
            {
                var column = GetColumn(m_plan.SortingPreferences.Criteria);
                var order = m_plan.SortingPreferences.Order;

                if (column != null)
                {
                    column.ImageIndex = (m_plan.SortingPreferences.Order == ThreeStateSortOrder.Ascending ? ArrowUpIndex : ArrowDownIndex);
                }
            }
        }
        #endregion


        #region Context Menu
        /// <summary>
        /// When the context menu is opened, we upddate the status of the menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            // By default, all visible and disabled
            foreach (ToolStripItem item in cmsContextMenu.Items)
            {
                item.Visible = true;
                item.Enabled = false;
            }
            // Reset text in case of previous multiple selection
            miRemoveFromPlan.Text = "Remove from Plan";

            // Nothing more to do when nothing selected
            if (lvSkills.SelectedItems.Count == 0) return;

            // When there is only one selected item...
            if (lvSkills.SelectedItems.Count == 1)
            {
                var entry = lvSkills.SelectedItems.Count > 0 ? GetPlanEntry(lvSkills.SelectedItems[0]) : null;

                // When the selected item is a remapping, only "remove from plan" is visible
                if (entry == null)
                {
                    miRemoveFromPlan.Enabled = true;
                    return;
                }

                // Enable other items
                miSubPlan.Enabled = true;
                miChangePriority.Enabled = true;
                miShowInSkillBrowser.Enabled = true;
                miShowInSkillExplorer.Enabled = true;

                // "Change note"
                miChangeNote.Enabled = true;
                miChangeNote.Text = "View/Change Note...";

                // "Change level"
                miChangeLevel.Enabled = SetChangeLevelMenu();

                // If skill is level 5 "remove from plan" is visible
                if (entry.Level == 5) miRemoveFromPlan.Enabled = true;

                // "Plan groups"
                if (entry.PlanGroups.Count > 0)
                {
                    miPlanGroups.Enabled = true;

                    List<string> planGroups = new List<string>(entry.PlanGroups);
                    planGroups.Sort();

                    miPlanGroups.DropDownItems.Clear();
                    foreach (string pg in planGroups)
                    {
                        ToolStripButton tsb = new ToolStripButton(pg);
                        tsb.Click += new EventHandler(planGroupMenu_Click);
                        tsb.Width = TextRenderer.MeasureText(pg, tsb.Font).Width;
                        miPlanGroups.DropDownItems.Add(tsb);
                    }
                }

            }
            // Multiple items selected
            else
            {
                miSubPlan.Enabled = true;
                miMarkOwned.Enabled = true;
                miChangePriority.Enabled = true;
                miRemoveFromPlan.Enabled = true;
                var operation = PrepareSelectionRemoval();
                if (PlanHelper.RequiresWindow(operation)) miRemoveFromPlan.Text += "...";

                miChangeNote.Enabled = true;
                miChangeNote.Text = "Change Note...";
            }

            // "Mark as owned"
            var skills = lvSkills.SelectedItems.Cast<ListViewItem>().Where(x => x.Tag is PlanEntry).Select(x => ((PlanEntry)x.Tag).CharacterSkill);
            if (skills.Any(x => !x.IsKnown))
            {
                miMarkOwned.Text = (skills.Any(x => !x.IsOwned) ? "Mark as owned" : "Mark as unowned");
                miMarkOwned.Enabled = true;
            }
            else
            {
                miMarkOwned.Text = "Mark as owned";
                miMarkOwned.Enabled = false;
            }
        }

        /// <summary>
        /// Update the status of the "Plan to level N" menu entries.
        /// </summary>
        /// <returns>True if at least one of the entries could be set.</returns>
        private bool SetChangeLevelMenu()
        {
            PlanEntry pe = GetFirstSelectedEntry();

            // Scroll through levels (and menus, one per level)
            bool result = false;
            for (int i = 0; i <= 5; i++)
            {
                PlanHelper.UpdatesRegularPlanToMenu(miChangeLevel.DropDownItems[i], m_plan, pe.CharacterSkill, i);
                result |= miChangeLevel.DropDownItems[i].Enabled;
            }
            return result;
        }

        /// <summary>
        /// Context menu > "Select items from group..." > Groupname
        /// Selects all the items which belong to the same group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void planGroupMenu_Click(object sender, EventArgs e)
        {
            string planGroup = ((ToolStripButton)sender).Text;
            foreach (ListViewItem item in lvSkills.Items)
            {
                item.Selected = GetPlanEntry(item).PlanGroups.Contains(planGroup);
            }
        }

        /// <summary>
        /// Context menu > Show skill in browser.
        /// Displays the selected entry's skill in the skill browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miShowInSkillBrowser_Click(object sender, EventArgs e)
        {
            ListViewItem item = lvSkills.SelectedItems[0];
            PlanEntry entry = item.Tag as PlanEntry;
            if (entry != null)
            {
                Skill skill = entry.CharacterSkill;
                WindowsFactory<PlanWindow>.GetByTag(m_plan).ShowSkillInBrowser(skill);
            }
        }

        /// <summary>
        /// Context menu > Show skill in explorer.
        /// Displays the selected entry's skill in the skill explorer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miShowInSkillExplorer_Click(object sender, EventArgs e)
        {
            ListViewItem item = lvSkills.SelectedItems[0];
            PlanEntry entry = item.Tag as PlanEntry;
            if (entry != null)
            {
                Skill skill = entry.CharacterSkill;
                WindowsFactory<PlanWindow>.GetByTag(m_plan).ShowSkillInExplorer(skill);
            }
        }

        /// <summary>
        /// Context menu > Remove from plan.
        /// Removes the seleted entry or remapping point from the plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miRemoveFromPlan_Click(object sender, EventArgs e)
        {
            RemoveSelectedEntries();
        }

        /// <summary>
        /// Context menu > Change priority.
        /// Opens a dialog box to edit the priorities. Check for concflicts and asks the user when needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miChangePriority_Click(object sender, EventArgs e)
        {
            var entries = SelectedEntries;

            using (PlanPrioritiesEditorForm form = new PlanPrioritiesEditorForm())
            {
                // Gets the entry's priority (or default if more than one item selected)
                form.Priority = PlanEntry.DefaultPriority;
                if (lvSkills.SelectedItems.Count == 1)
                {
                    PlanEntry pe = GetPlanEntry(lvSkills.SelectedItems[0]);
                    if (pe != null) form.Priority = pe.Priority;
                }

                // User canceled ?
                DialogResult dr = form.ShowDialog();
                if (dr == DialogResult.Cancel) return;

                // Update priorities, while performing backup for subsequent check
                if (!m_plan.TrySetPriority(m_displayPlan, entries, form.Priority))
                {
                    bool showDialog = Settings.UI.PlanWindow.ShowMsgBoxCustom;
                    
                    if (showDialog)
                    {
                        string text = String.Concat("This would result in a priority conflict.",
                               " (Either pre-requisites with a lower priority or dependant skills with a higher priority).\r\n\r\n",
                               "Click Yes if you wish to do this and adjust the other skills\r\nor No if you do not wish to change the priority."),
                        captionText = "Priority Conflict",
                        cbOptionText = "Do not show this dialog again";

                        MessageBoxCustom MsgBoxCustom = new MessageBoxCustom();
                        DialogResult drb = MsgBoxCustom.Show(this, text, captionText, cbOptionText, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (drb == DialogResult.Yes)
                        {
                            m_plan.SetPriority(m_displayPlan, entries, form.Priority);
                        }
                        Settings.UI.PlanWindow.ShowMsgBoxCustom = MsgBoxCustom.cbUnchecked;
                    }
                    else m_plan.SetPriority(m_displayPlan, entries, form.Priority);
                }
            }
        }

        /// <summary>
        /// Context menu > Change note.
        /// Opens a box to change the plan's notes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miChangeNote_Click(object sender, EventArgs e)
        {
            var entries = SelectedEntries;
            if (entries.IsEmpty()) return;

            // We get the current skill's note and call the note editor window with this initial value
            string noteText = entries.First().Notes;
            string title = (entries.Count() == 1 ? entries.First().Skill.ToString() : "Selected entries");
            using (PlanNotesEditorWindow f = new PlanNotesEditorWindow(title))
            {
                f.NoteText = noteText;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel) return;

                noteText = f.NoteText;
            }

            // We update every item
            foreach (var entry in entries) entry.Notes = noteText;
            m_plan.RebuildPlanFrom(m_displayPlan, true);
        }

        /// <summary>
        /// Context > Create sub-plan...
        /// Opens a dialog to prompt the user for a name and create a plan with the selected entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miSubPlan_Click(object sender, EventArgs e)
        {
            var entries = SelectedEntries;
            if (entries.IsEmpty()) return;

            // Ask the user for a new name
            string planName;
            using (NewPlanWindow npw = new NewPlanWindow())
            {
                DialogResult dr = npw.ShowDialog();
                if (dr == DialogResult.Cancel) return;
                planName = npw.Result;
            }

            // Create a new plan
            Plan newPlan = new Plan(Character);
            newPlan.Name = planName;
            var operation = newPlan.TryAddSet(entries, "Exported from " + m_plan.Name);
            operation.Perform();

            // Add plan and save
            Character.Plans.Add(newPlan);
        }

        /// <summary>
        /// Context > Mark as owned/unowned.
        /// Toggle the owned flag for the selected skills. 
        /// When multiple entries are selected and have different flags, we mark them as all owned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miMarkOwned_Click(object sender, EventArgs e)
        {
            bool unowned = SelectedEntries.All(x => !x.CharacterSkill.IsOwned);

            using (m_plan.SuspendingEvents())
            {
                foreach (var entry in SelectedEntries)
                {
                    entry.CharacterSkill.IsOwned = unowned;
                }
            }

            // We update the skill tree
            skillSelectControl.UpdateContent();
        }

        /// <summary>
        /// Context > Change planned level > Level N
        /// Change the planned level, or remove if 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miChangeToLevel_Click(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            var operation = menu.Tag as IPlanOperation;
            PlanHelper.PerformSilently(operation);
        }
        #endregion


        #region Drag'n drop from outer controls (inner drag'n drop is in reordering region)
        /// <summary>
        /// When the user drop a skill on the list, we plans it to the next unplanned level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // Quits if the button is never the left one nor the right one.
                if (m_dragButton != MouseButtons.Left && m_dragButton != MouseButtons.Right) return;

                // Checks there is a skill
                Skill dragSkill = GetDraggingSkill(e);
                if (dragSkill == null) return;

                // Gets the item and returns if none created (already on lv5)
                ListViewItem newItem = CreatePlanItemForSkill(dragSkill);
                if (newItem == null) return;



                // By default, drop index is at the end of the list
                int dragIndex = lvSkills.Items.Count;

                // If the user is dropping on an item, infere the drag index from this item's index
                Point cp = lvSkills.PointToClient(new Point(e.X, e.Y));
                ListViewItem hoverItem = lvSkills.GetItemAt(cp.X, cp.Y);
                if (hoverItem != null)
                {
                    dragIndex = hoverItem.Index;
                    Rectangle hoverBounds = hoverItem.GetBounds(ItemBoundsPortion.ItemOnly);

                    // If the user is dropping on the lower half of the item, increase the dragging index
                    if (cp.Y > (hoverBounds.Top + (hoverBounds.Height / 2)))
                    {
                        dragIndex++;
                    }
                }


                // Performs the insertion
                m_plan.PlanTo(dragSkill, dragSkill.Level + 1);

            }
            finally
            {
                // Clean up our mess
                lvSkills.ClearDropMarker();
                e.Effect = DragDropEffects.None;
                m_dragButton = MouseButtons.None;
            }
        }

        /// <summary>
        /// When the user drags over the skill list, updates the drop marker and dragging button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_DragOver(object sender, DragEventArgs e)
        {
            // Checks there is a dragged skill
            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill == null) return;

            // Updates the dragging button
            SetDragMouseButton(e);

            // Gets the hovered item
            e.Effect = DragDropEffects.Move;
            Point cp = lvSkills.PointToClient(new Point(e.X, e.Y));
            ListViewItem hoverItem = lvSkills.GetItemAt(cp.X, cp.Y);

            // Updates the drop marker below the hovered item.
            if (hoverItem != null)
            {
                Rectangle hoverBounds = hoverItem.GetBounds(ItemBoundsPortion.ItemOnly);
                lvSkills.DrawDropMarker(hoverItem.Index, (cp.Y > (hoverBounds.Top + (hoverBounds.Height / 2))));
            }
            else
            {
                lvSkills.ClearDropMarker();
            }
        }

        /// <summary>
        /// When the user begins a drag/drop operation, updates the drag/drop button and cursor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_DragEnter(object sender, DragEventArgs e)
        {
            // Sets up the drag button
            SetDragMouseButton(e);

            // Gets the dragging skill set up by the source control
            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill != null)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// When the user leaves a drag/drop operation, clear the drop marker and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_DragLeave(object sender, EventArgs e)
        {
            m_dragButton = MouseButtons.None;
            lvSkills.ClearDropMarker();
        }

        /// <summary>
        /// Looks for a <see cref="Skill"/> in the data of the provided event arguments.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static Skill GetDraggingSkill(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode"))
            {
                return (Skill)((TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode")).Tag;
            }
            return null;
        }

        /// <summary>
        /// Creates a plan entry and a list view item for it, from the given skill
        /// </summary>
        /// <param name="gs"></param>
        /// <returns></returns>
        private ListViewItem CreatePlanItemForSkill(Skill skill)
        {
            // Gets the planned level of the skill.
            int newLevel = m_plan.GetPlannedLevel(skill) + 1;
            if (skill.Level >= newLevel) newLevel = skill.Level + 1;

            // Quits if already on lv5
            if (newLevel > 5) return null;

            // Creates the plan entry and list item for this level
            PlanEntry newEntry = new PlanEntry(m_plan, skill, newLevel);
            ListViewItem newItem = new ListViewItem(newEntry.ToString());
            newItem.Tag = newEntry;

            return newItem;
        }

        /// <summary>
        /// Gets the mouse button used to drag
        /// </summary>
        /// <param name="e"></param>
        private void SetDragMouseButton(DragEventArgs e)
        {
            if ((e.KeyState & (int)Keys.LButton) != 0)
            {
                m_dragButton = MouseButtons.Left;
            }
            else if ((e.KeyState & (int)Keys.RButton) != 0)
            {
                m_dragButton = MouseButtons.Right;
            }
        }
        #endregion


        #region Other list events : keyboard, click, hovering, selection change
        /// <summary>
        /// On a doube-click on one of the list items, we open the skill browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvSkills.SelectedItems.Count == 1)
            {
                // When the first entry is a skill, shows it in the skill browser.
                if (GetFirstSelectedEntry() != null)
                {
                    miShowInSkillBrowser_Click(sender, e);
                }
                // When it is a remapping point, edits it
                else
                {
                    // Retrieves the point
                    var nextItem = lvSkills.Items[lvSkills.SelectedIndices[0] + 1];
                    var entry = GetPlanEntry(nextItem);
                    var point = entry.Remapping;

                    // Display the attributes optimization form
                    var form = new AttributesOptimizationForm(m_character, m_displayPlan, point, m_plan.Name);
                    form.PlanEditor = this;
                    form.Show();
                }
            }
        }

        /// <summary>
        /// Handles key press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    miChangeNote_Click(sender, e);
                    break;
                case Keys.F9:
                    tsbToggleRemapping_Click(null, null);
                    break;
                case Keys.F5:
                    UpdateDisplayPlan();
                    UpdateSkillList(true);
                    break;
                case Keys.Delete:
                    RemoveSelectedEntries();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// When the user selects another entry, we do not immediately process the change but rather delay it through a timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tmrSelect.Enabled) return;
            tmrSelect.Interval = 100;
            tmrSelect.Enabled = true;
            tmrSelect.Start();
        }

        /// <summary>
        /// When the selection update timer ticks, we process the changes caused by a selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrSelect_Tick(object sender, EventArgs e)
        {
            tmrSelect.Enabled = false;
            OnSelectionChanged();
        }

        /// <summary>
        /// Handles the selection change (delayed every 100ms through a timer).
        /// </summary>
        private void OnSelectionChanged()
        {
            if (lvSkills.SelectedIndices.Count == 0)
            {
                tsbMoveUp.Enabled = false;
                tsbMoveDown.Enabled = false;
                UpdateListViewItems();
            }
            else
            {
                tsbMoveUp.Enabled = (lvSkills.SelectedIndices[0] != 0);
                tsbMoveDown.Enabled = (lvSkills.SelectedIndices[lvSkills.SelectedIndices.Count - 1] != lvSkills.Items.Count - 1);
            }

            // Creates the prerequisite indicators
            foreach (ListViewItem current in lvSkills.Items)
            {
                bool isSameSkill = false;
                bool isPreRequisite = false;
                bool isPostRequisite = false;

                // Checks whether it is a prerequisite of the currently selected entry and whether we should highlight it.
                if (!Settings.UI.SafeForWork && Settings.UI.PlanWindow.HighlightPrerequisites && SelectedEntries.Count() == 1)
                {
                    PlanEntry currentEntry = current.Tag as PlanEntry;
                    PlanEntry selectedEntry = lvSkills.SelectedItems[0].Tag as PlanEntry;
                    if (currentEntry != null && selectedEntry != null)
                    {
                        int neededLevel;
                        if (currentEntry.Skill.HasAsImmediatePrereq(selectedEntry.Skill, out neededLevel))
                        {
                            if (currentEntry.Level == 1 && neededLevel >= selectedEntry.Level)
                            {
                                isPostRequisite = true;
                            }
                        }
                        if (selectedEntry.Skill.HasAsImmediatePrereq(currentEntry.Skill, out neededLevel))
                        {
                            if (currentEntry.Level == neededLevel)
                            {
                                isPreRequisite = true;
                            }
                        }
                        if (currentEntry.Skill == selectedEntry.Skill)
                        {
                            isSameSkill = true;
                        }
                    }
                }

                // Color depends on the entry's status
                if (current.Tag is RemappingPoint)  current.ImageIndex = 3;
                else if (isSameSkill) current.ImageIndex = 1;
                else if (isPreRequisite) current.ImageIndex = 2;
                else if (isPostRequisite) current.ImageIndex = 0;
                else current.ImageIndex = -1;
            }

            UpdateStatusBar();
        }

        /// <summary>
        /// When the user hovers an item, we update the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_ItemHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            ListViewItem lvi = e.Item;
            if (lvi == null) return;

            // Is it an entry ?
            var entry = GetPlanEntry(lvi);
            if (entry != null)
            {
                Skill skill = entry.CharacterSkill;
                StringBuilder builder = new StringBuilder(skill.Description);

                if (!skill.IsKnown)
                {
                    builder.Append("\n\nYou do not know this skill - you ");
                    if (!skill.IsOwned) builder.Append("do not ");
                    builder.Append("own the skillbook");
                }
                lvi.ToolTipText = builder.ToString();
            }
            // Then it is a remapping point
            else if (lvi.Tag is RemappingPoint)
            {
                var point = lvi.Tag as RemappingPoint;
                lvi.ToolTipText = point.ToLongString();
            }
        }

        /// <summary>
        /// Upon column reordering we force a column settings update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            m_columnsOrderChanged = true;
        }
        #endregion


        #region Other controls' handlers
        /// <summary>
        /// Left toolbar > Toggle skills panel.
        /// Display a skill list on the riht
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleSkillsPanelButton_Click(object sender, EventArgs e)
        {
            pscPlan.Panel2Collapsed = !pscPlan.Panel2Collapsed;
            tsbToggleSkills.Checked = !pscPlan.Panel2Collapsed;
            pscPlan.SplitterDistance = pscPlan.Width - 200;
        }

        /// <summary>
        /// Left toolbar > Toggle remapping point.
        /// Adds or remove a remapping point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbToggleRemapping_Click(object sender, EventArgs e)
        {
            if (this.lvSkills.SelectedIndices.Count == 0) return;

            var item = this.lvSkills.SelectedItems[0];
            var tag = item.Tag;

            // Remove an existing point
            if (tag is RemappingPoint)
            {
                // Selects the next item and focuses it.
                var entryIndex = this.lvSkills.SelectedItems[0].Index + 1;
                this.lvSkills.Items[entryIndex].Selected = true;
                this.lvSkills.Items[entryIndex].Focused = true;

                // Retrieve the original entry after this item and remove its remaping point.
                var entry = this.lvSkills.Items[entryIndex].Tag as PlanEntry;
                var originalEntry = GetOriginalEntry(entry);
                originalEntry.Remapping = null;
            }
            // Toggle on a skill
            else
            {
                // Retrieves the focused item's hash code.
                item.Focused = true;

                // Retrieves the original entry
                var entryIndex = this.lvSkills.SelectedIndices[0];
                var entry = tag as PlanEntry;
                var originalEntry = GetOriginalEntry(entry);

                // Add a remapping point
                if (originalEntry.Remapping == null) originalEntry.Remapping = new RemappingPoint();
                else originalEntry.Remapping = null;
            }
        }

        /// <summary>
        /// When the user clicks the "select columns" link, we display the suggestions window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnsLink_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Update the settings from the current columns
            ExportColumnSettings();

            var columns = ExportColumnSettings();
            using (PlanColumnSelectWindow f = new PlanColumnSelectWindow(columns))
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    ImportColumnSettings(f.Columns.Cast<PlanColumnSettings>());
                }
            }
        }
        #endregion
    }
}
