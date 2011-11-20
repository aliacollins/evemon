using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Scheduling;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// The main control of the plan editor window, the list of plan entries.
    /// </summary>
    public partial class PlanEditorControl : UserControl
    {
        #region Fields

        private const int ArrowUpIndex = 4;
        private const int ArrowDownIndex = 5;

        private int m_lastImplantSetIndex;
        private bool m_areRemappingPointsActive;

        private Font m_plannedSkillFont;
        private Font m_prerequisiteSkillFont;
        private Color m_nonImmedTrainablePlanEntryColor;
        private Color m_remappingBackColor;
        private Color m_remappingForeColor;

        private Character m_character;
        private RemappingPoint m_formTag;
        private AttributesOptimizationForm m_oldForm;

        private Plan m_plan;

        // The ImplantsControl or the AttributesOptimizationForm
        private IPlanOrderPluggable m_pluggable;

        // Drag and drop
        private MouseButtons m_dragButton = MouseButtons.None;

        // Columns
        private bool m_isUpdatingColumns;
        private bool m_columnsOrderChanged;
        private readonly List<PlanColumnSettings> m_columns = new List<PlanColumnSettings>();

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlanEditorControl()
        {
            InitializeComponent();
            pscPlan.RememberDistanceKey = "PlanEditor";

            ListViewHelper.EnableDoubleBuffer(lvSkills);

            lvSkills.ColumnWidthChanged += lvSkills_ColumnWidthChanged;
            lvSkills.ColumnClick += lvSkills_ColumnClick;
            tsSortPriorities.Click += tsSortPriorities_Clicked;
            cbChooseImplantSet.DropDown += cbChooseImplantSet_DropDown;

            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.SchedulerChanged += EveMonClient_SchedulerChanged;
            Disposed += OnDisposed;
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.SchedulerChanged -= EveMonClient_SchedulerChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// On load, updates the controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted())
                return;

            Font = FontFactory.GetFont("Tahoma", 8.25F);

            m_columns.Clear();
            m_columns.AddRange(Settings.UI.PlanWindow.Columns);

            m_plannedSkillFont = FontFactory.GetFont(lvSkills.Font, FontStyle.Bold);
            m_prerequisiteSkillFont = FontFactory.GetFont(lvSkills.Font);
            m_nonImmedTrainablePlanEntryColor = SystemColors.GrayText;
            m_remappingForeColor = SystemColors.HotTrack;
            m_remappingBackColor = SystemColors.Info;

            // Update the skill list
            UpdateSkillList();

            base.OnLoad(e);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the plan represented by this editor.
        /// </summary>
        [Browsable(false)]
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (value  == null || m_plan == value)
                    return;

                m_plan = value;
                m_character = (Character)m_plan.Character;
                DisplayPlan = new PlanScratchpad(m_character);
                m_lastImplantSetIndex = -1;

                // Children controls
                skillSelectControl.Plan = m_plan;

                // Build the plan
                UpdateDisplayPlan();

                // Update Implant Set control
                UpdateImplantSetList();
                cbChooseImplantSet.SelectedIndex = 0;

                // Update the columns
                UpdateListColumns();
            }
        }

        /// <summary>
        /// Gets the version of the plan as it is currently displayed.
        /// </summary>
        [Browsable(false)]
        public PlanScratchpad DisplayPlan { get; private set; }

        /// <summary>
        /// Gets the character this control is bound to.
        /// </summary>
        [Browsable(false)]
        public Character Character
        {
            get { return (Character)m_plan.Character; }
        }

        /// <summary>
        /// Gets the number of unique skills selected (two levels of same skill counts for one unique skill).
        /// </summary>
        [Browsable(false)]
        public int UniqueSkillsCount
        {
            get { return SelectedEntries.GetUniqueSkillsCount(); }
        }

        /// <summary>
        /// Gets the number of not known skills selected (two levels of same skill counts for one unique skill).
        /// </summary>
        [Browsable(false)]
        public int NotKnownSkillsCount
        {
            get { return SelectedEntries.GetNotKnownSkillsCount(); }
        }

        /// <summary>
        /// Gets the cost of known skills selected.
        /// </summary>
        [Browsable(false)]
        public long SkillBooksCost
        {
            get { return SelectedEntries.GetTotalBooksCost(); }
        }

        /// <summary>
        /// Gets the cost of not known skills selected.
        /// </summary>
        [Browsable(false)]
        public long NotKnownSkillBooksCost
        {
            get { return SelectedEntries.GetNotKnownSkillBooksCost(); }
        }

        /// <summary>
        /// Gets the skill select control.
        /// </summary>
        /// <value>The skill select control.</value>
        [Browsable(false)]
        public Control SkillSelectControl
        {
            get { return skillSelectControl; }
        }

        #endregion


        #region Global events

        /// <summary>
        /// When the character change, some entries may now be known, so we reupdate everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (!Visible || e.Character != m_character)
                return;

            UpdateDisplayPlan();
            UpdateSkillList();
        }

        /// <summary>
        /// When the plan changed, entries may have changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            UpdateDisplayPlan();
            UpdateSkillList();
            UpdateListColumns();
        }

        /// <summary>
        /// When the settings changed, implant sets, the highlights and such may be different. 
        /// Entries are still the same but we may need to update implant sets, highlights and others.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateImplantSetList();
            cbChooseImplantSet.SelectedIndex = m_lastImplantSetIndex;
            UpdateImplantSet();

            UpdateSkillList();
        }

        /// <summary>
        /// When the scheduler changed, the blocking highlights may be different. 
        /// Entries are still the same but we may need to update the blocking highlights.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SchedulerChanged(object sender, EventArgs e)
        {
            UpdateSkillList();
            UpdateListColumns();
        }

        /// <summary>
        /// Per second check if columns have been reordered.
        /// When true, column settings get saved and reloaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (!Visible)
                return;

            if (m_columnsOrderChanged)
            {
                Settings.UI.PlanWindow.Add(ExportColumnSettings().ToList());
                ImportColumnSettings(Settings.UI.PlanWindow.Columns);
            }

            m_columnsOrderChanged = false;
        }

        #endregion


        #region Content creation and refresh

        /// <summary>
        /// Update the Implant Set control list.
        /// </summary>
        private void UpdateImplantSetList()
        {
            // Populate the "choose implant set"
            cbChooseImplantSet.Items.Clear();
            cbChooseImplantSet.Items.AddRange(m_character.ImplantSets.ToArray());
        }

        /// <summary>
        /// Whenever the sorting option or the base plan changed, we update the sorted plan.
        /// </summary>
        private void UpdateDisplayPlan()
        {
            DisplayPlan.RebuildPlanFrom(m_plan, true);

            // Share the remapping points
            PlanEntry[] srcEntries = m_plan.ToArray();
            PlanEntry[] destEntries = DisplayPlan.ToArray();
            for (int i = 0; i < srcEntries.Length; i++)
            {
                destEntries[i].Remapping = srcEntries[i].Remapping;
            }

            // Apply the sort
            DisplayPlan.Sort(m_plan.SortingPreferences);
        }

        /// <summary>
        /// Rebuild the list items from the entries and their remapping points. Plan entries items are not fully initialized, 
        /// only their tags and a couple of things.
        /// Full intialization, especially the columns values and such, will be completed in <see cref="UpdateListViewItems"/>.
        /// </summary>
        /// <param name="restoreSelectionAndFocus">When false, selection and focus will be reseted.</param>
        private void UpdateSkillList(bool restoreSelectionAndFocus = true)
        {
            // Disable controls, they will be restored one the selection is updated
            tsbMoveUp.Enabled = false;
            tsbMoveDown.Enabled = false;
            tmrAutoRefresh.Stop();

            // Stores selection and focus, to restore them after the update
            Dictionary<int, bool> selection = (restoreSelectionAndFocus ? StoreSelection() : null);
            int focusedHashCode = (restoreSelectionAndFocus && lvSkills.FocusedItem != null
                                       ? lvSkills.FocusedItem.Tag.GetHashCode()
                                       : 0);

            lvSkills.BeginUpdate();
            try
            {
                // Scroll through entries and their remapping points
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (PlanEntry entry in DisplayPlan)
                {
                    // In any case, we must insert a new item for this plan's entry at the current index
                    // Do we need to insert a remapping point ?
                    if (entry.Remapping != null)
                    {
                        ListViewItem rlv = new ListViewItem
                                               {
                                                   UseItemStyleForSubItems = true,
                                                   Tag = entry.Remapping,
                                                   ImageIndex = 3
                                               };
                        items.Add(rlv);
                    }

                    // Insert the entry
                    ListViewItem lvi = new ListViewItem { Tag = entry };
                    items.Add(lvi);

                    // Is it a prerequisite or a top level entry ?
                    if (!Settings.UI.SafeForWork)
                    {
                        lvi.Font = (Settings.UI.PlanWindow.HighlightPlannedSkills && entry.Type == PlanEntryType.Planned
                                        ? m_plannedSkillFont
                                        : m_prerequisiteSkillFont);
                    }

                    // Gray out entries that cannot be trained immediately
                    if (!entry.CanTrainNow && Settings.UI.PlanWindow.DimUntrainable)
                        lvi.ForeColor = m_nonImmedTrainablePlanEntryColor;

                    // Enable refresh every 30s if a skill is in training
                    tmrAutoRefresh.Enabled = entry.CharacterSkill.IsTraining;
                }

                // We avoid clear + AddRange because it causes the sliders position to reset
                while (lvSkills.Items.Count > 1)
                {
                    lvSkills.Items.RemoveAt(lvSkills.Items.Count - 1);
                }

                bool isEmpty = (lvSkills.Items.Count == 0);
                lvSkills.Items.AddRange(items.ToArray());
                if (!isEmpty)
                    lvSkills.Items.RemoveAt(0);

                // Complete the items initialization
                UpdateListViewItems();

                // Restore selection and focus
                if (restoreSelectionAndFocus)
                {
                    RestoreSelection(selection);
                    ListViewItem focusedItem =
                        lvSkills.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Tag.GetHashCode() == focusedHashCode);
                    if (focusedItem != null)
                        focusedItem.Focused = true;

                    lvSkills.Select();
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
            CheckForPluggable();

            // Start updating the list
            lvSkills.BeginUpdate();
            try
            {
                // Scroll through entries
                for (int i = 0; i < lvSkills.Items.Count; i++)
                {
                    ListViewItem lvi = lvSkills.Items[i];
                    PlanEntry entry = lvi.Tag as PlanEntry;

                    // Add enough subitems to match the number of columns
                    while (lvi.SubItems.Count < lvSkills.Columns.Count)
                    {
                        lvi.SubItems.Add(String.Empty);
                    }

                    if (entry != null)
                        FormatEntry(entry, lvi);
                        // The item represents a remapping point
                    else
                        FormatRemappingPoint(lvi);
                }
            }
            finally
            {
                lvSkills.EndUpdate();
            }

            UpdateStatusBar();
        }

        /// <summary>
        /// Formats the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="lvi">The lvi.</param>
        private void FormatEntry(PlanEntry entry, ListViewItem lvi)
        {
            // Checks if this entry has not prereq-met
            if (!entry.CharacterSkill.IsKnown)
                lvi.ForeColor = Color.Red;

            // Checks if this entry is a non-public skill
            if (!entry.CharacterSkill.IsPublic)
                lvi.ForeColor = Color.DarkRed;

            // Checks if this entry is not known but has prereq-met
            if (!entry.CharacterSkill.IsKnown && entry.CharacterSkill.IsPublic &&
                entry.CharacterSkill.ArePrerequisitesMet)
                lvi.ForeColor = Color.LightSlateGray;

            // Checks if this entry is partially trained
            bool level = (entry.Level == entry.CharacterSkill.Level + 1);
            if (Settings.UI.PlanWindow.HighlightPartialSkills)
            {
                bool partiallyTrained = (entry.CharacterSkill.FractionCompleted > 0 &&
                                         entry.CharacterSkill.FractionCompleted < 1);
                if (level && partiallyTrained)
                    lvi.ForeColor = Color.Green;
            }

            HighlightQueuedSkills(lvi, entry);

            // Checks if this entry is currently training (even if it's paused)
            if (entry.CharacterSkill.IsTraining && level)
            {
                lvi.BackColor = Color.LightSteelBlue;
                lvi.ForeColor = Color.Black;
            }

            // Checks whether this entry will be blocked
            string blockingEntry = String.Empty;
            if (Settings.UI.PlanWindow.HighlightConflicts)
            {
                bool isBlocked = Scheduler.SkillIsBlockedAt(entry.EndTime, out blockingEntry);
                if (isBlocked)
                {
                    lvi.ForeColor = Color.Red;
                    lvi.BackColor = Color.LightGray;
                }
            }

            // Update every column
            lvi.UseItemStyleForSubItems = (m_pluggable == null);
            for (int columnIndex = 0; columnIndex < lvSkills.Columns.Count; columnIndex++)
            {
                // Regular columns (not pluggable-dependent)
                if (lvSkills.Columns[columnIndex].Tag != null)
                {
                    PlanColumnSettings columnSettings = (PlanColumnSettings)lvSkills.Columns[columnIndex].Tag;

                    lvi.SubItems[columnIndex].BackColor = lvi.BackColor;
                    lvi.SubItems[columnIndex].ForeColor = lvi.ForeColor;
                    lvi.SubItems[columnIndex].Text = GetColumnTextForItem(entry, columnSettings.Column, blockingEntry);
                }
                    // Training time differences
                else
                {
                    TimeSpan timeDifference;
                    string result = String.Empty;
                    if (entry.OldTrainingTime < entry.TrainingTime)
                    {
                        result = "+";
                        timeDifference = entry.TrainingTime - entry.OldTrainingTime;
                        lvi.SubItems[columnIndex].BackColor = lvi.BackColor;
                        lvi.SubItems[columnIndex].ForeColor = Color.DarkRed;
                    }
                    else if (entry.OldTrainingTime > entry.TrainingTime)
                    {
                        result = "-";
                        timeDifference = entry.OldTrainingTime - entry.TrainingTime;
                        lvi.SubItems[columnIndex].BackColor = lvi.BackColor;
                        lvi.SubItems[columnIndex].ForeColor = Color.DarkGreen;
                    }
                    else
                    {
                        timeDifference = TimeSpan.Zero;
                        lvi.SubItems[columnIndex].BackColor = lvi.BackColor;
                        lvi.SubItems[columnIndex].ForeColor = lvi.ForeColor;
                    }

                    result += timeDifference.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);
                    lvi.SubItems[columnIndex].Text = result;
                }
            }
        }

        /// <summary>
        /// Formats the remapping point.
        /// </summary>
        /// <param name="lvi">The lvi.</param>
        private void FormatRemappingPoint(ListViewItem lvi)
        {
            RemappingPoint point = (RemappingPoint)lvi.Tag;
            for (int columnIndex = 0; columnIndex < lvSkills.Columns.Count; columnIndex++)
            {
                PlanColumnSettings columnSettings = (PlanColumnSettings)lvSkills.Columns[columnIndex].Tag;

                lvi.SubItems[columnIndex].Text = String.Empty;
                lvi.SubItems[columnIndex].BackColor = m_remappingBackColor;
                lvi.SubItems[columnIndex].ForeColor = m_remappingForeColor;

                // We display the text in the SkillName column for better visibility
                if (columnSettings != null && columnSettings.Column == PlanColumn.SkillName)
                {
                    lvi.SubItems[columnIndex].Text = (m_areRemappingPointsActive
                                                          ? point.ToString()
                                                          : "Remapping (ignored)");
                }
            }
        }

        /// <summary>
        /// Checks for pluggable.
        /// </summary>
        private void CheckForPluggable()
        {
            // When there is a pluggable (implants calculator or attributes optimizer)
            // This one provides us the scratchpad to update training times
            m_areRemappingPointsActive = true;
            if (m_pluggable != null)
            {
                bool areRemappingPointsActive;
                m_pluggable.UpdateStatistics(DisplayPlan, out areRemappingPointsActive);
                m_areRemappingPointsActive = areRemappingPointsActive;
            }
            else
            {
                CharacterScratchpad scratchpad = new CharacterScratchpad(m_character);
                if (m_plan.ChosenImplantSet != null)
                    scratchpad = scratchpad.After(m_plan.ChosenImplantSet);

                DisplayPlan.UpdateStatistics(scratchpad, true, true);
            }
        }

        /// <summary>
        /// Applies formatting to a ListViewItem if the entry is queued.
        /// </summary>
        /// <param name="lvi">ListViewItem to be formatted.</param>
        /// <param name="entry">Entry used to identify is queued.</param>
        private void HighlightQueuedSkills(ListViewItem lvi, PlanEntry entry)
        {
            if (!Settings.UI.PlanWindow.HighlightQueuedSkills)
                return;

            CCPCharacter ccpCharacter = m_character as CCPCharacter;

            // Current character isn't a CCP character, so can't have a Queue
            if (ccpCharacter == null)
                return;

            if (ccpCharacter.SkillQueue.Any(skillLevel => skillLevel.ToString() == entry.ToString()))
                lvi.ForeColor = Color.RoyalBlue;
        }

        /// <summary>
        /// Gets the text to display in the given column for the provided entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="column"></param>
        /// <param name="blockingEntry"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        private static string GetColumnTextForItem(PlanEntry entry, PlanColumn column, string blockingEntry)
        {
            const int MaxNotesLength = 60;

            switch (column)
            {
                case PlanColumn.SkillName:
                    return entry.ToString();
                case PlanColumn.PlanGroup:
                    return entry.PlanGroupsDescription;
                case PlanColumn.TrainingTime:
                    return entry.TrainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, false);
                case PlanColumn.TrainingTimeNatural:
                    return entry.NaturalTrainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, false);
                case PlanColumn.EarliestStart:
                    return String.Format(CultureConstants.DefaultCulture, "{0:ddd} {0:G}", entry.StartTime);
                case PlanColumn.EarliestEnd:
                    return String.Format(CultureConstants.DefaultCulture, "{0:ddd} {0:G}", entry.EndTime);
                case PlanColumn.PercentComplete:
                    return entry.FractionCompleted.ToString("P0", CultureConstants.DefaultCulture);
                case PlanColumn.SkillRank:
                    return entry.Skill.Rank.ToString(CultureConstants.DefaultCulture);
                case PlanColumn.PrimaryAttribute:
                    return entry.Skill.PrimaryAttribute.ToString();
                case PlanColumn.SecondaryAttribute:
                    return entry.Skill.SecondaryAttribute.ToString();
                case PlanColumn.SkillGroup:
                    return entry.Skill.Group.Name;
                case PlanColumn.PlanType:
                    return entry.Type.ToString();
                case PlanColumn.SPTotal:
                    return entry.EstimatedTotalSkillPoints.ToString("N0", CultureConstants.DefaultCulture);
                case PlanColumn.SPPerHour:
                    return entry.SpPerHour.ToString(CultureConstants.DefaultCulture);
                case PlanColumn.Priority:
                    return entry.Priority.ToString(CultureConstants.DefaultCulture);
                case PlanColumn.Conflicts:
                    return blockingEntry;
                case PlanColumn.Notes:
                    {
                        if (String.IsNullOrEmpty(entry.Notes))
                            return String.Empty;

                        string result = Regex.Replace(entry.Notes, @"(\r|\n)+", " ", RegexOptions.None);
                        if (result.Length <= MaxNotesLength)
                            return result;

                        return result.Substring(0, MaxNotesLength) + "...";
                    }
                case PlanColumn.Cost:
                    {
                        if (entry.Level != 1 || entry.CharacterSkill.IsKnown)
                            return String.Empty;
                        return entry.CharacterSkill.IsOwned ? "Owned" : entry.Skill.FormattedCost;
                    }
                case PlanColumn.SkillPointsRequired:
                    return entry.SkillPointsRequired.ToString("N0", CultureConstants.DefaultCulture);
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
                foreach (PlanColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    // Add the column
                    ColumnHeader header = lvSkills.Columns.Add(column.Column.GetHeader(), column.Width);
                    header.Tag = column;

                    // Add a temporary column when there is a pluggable (implants calc, attributes optimizer, etc)
                    if (m_pluggable == null || column.Column != PlanColumn.TrainingTime)
                        continue;

                    header = lvSkills.Columns.Add("Diff with Calc Atts", -2);
                    header.Tag = null;
                }

                // Update the items
                UpdateListViewItems();

                // Update the sort arrows
                UpdateSortVisualFeedback();

                // Adjust the size of the columns
                AdjustColumns();
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
        private void RestoreSelection(IDictionary<int, bool> c)
        {
            for (int i = lvSkills.Items.Count - 1; i >= 0; i--)
            {
                // Retrieve this item's tag hash code
                ListViewItem lvi = lvSkills.Items[i];
                int hashCode = lvi.Tag.GetHashCode();

                // Check whether this id must be selected
                bool selected;
                if (c.TryGetValue(hashCode, out selected))
                    c.Remove(hashCode);

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
            PlanWindow window = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            if (window == null)
                return;

            UpdateListViewItems();
            window.CheckObsoleteEntries();
        }

        /// <summary>
        /// Removes all obsolete entries and rebuilds the plan.
        /// </summary>
        public void ClearObsoleteEntries(ObsoleteRemovalPolicy policy)
        {
            m_plan.CleanObsoleteEntries(policy);
            UpdateDisplayPlan();
            UpdateSkillList();
        }

        /// <summary>
        /// Updates the status bar.
        /// </summary>
        private void UpdateStatusBar()
        {
            PlanWindow window = WindowsFactory<PlanWindow>.GetByTag(m_plan);

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
            int entriesCount = SelectedEntries.Count();

            // We compute the training time
            selectedTrainTime = SelectedEntries.Aggregate(selectedTrainTime, (current, entry) => current.Add(entry.TrainingTime));

            window.UpdateSkillStatusLabel(true, entriesCount, UniqueSkillsCount);
            window.UpdateTimeStatusLabel(true, entriesCount, selectedTrainTime);
            window.UpdateCostStatusLabel(true, SkillBooksCost, NotKnownSkillBooksCost);
        }

        /// <summary>
        /// Updates the implant set.
        /// </summary>
        private void UpdateImplantSet()
        {
            m_plan.ChosenImplantSet = cbChooseImplantSet.SelectedItem as ImplantSet;
            m_lastImplantSetIndex = cbChooseImplantSet.SelectedIndex;
            DisplayPlan.ChosenImplantSet = m_plan.ChosenImplantSet;

            if (m_pluggable != null)
                m_pluggable.UpdateOnImplantSetChange();
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
                    pluggable.Disposed += pluggable_Disposed;
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
            m_pluggable.Disposed -= pluggable_Disposed;
            m_pluggable = null;
            UpdateSkillList();
            UpdateListColumns();
        }

        #endregion


        #region Generic helper methods

        /// <summary>
        /// From an entry of the display plan, retrieve the entry of the base plan.
        /// </summary>
        /// <param name="displayEntry"></param>
        /// <returns></returns>
        private PlanEntry GetOriginalEntry(ISkillLevel displayEntry)
        {
            return m_plan.GetEntry(displayEntry.Skill, displayEntry.Level);
        }

        /// <summary>
        /// Gets the plan entry attached to the given item.
        /// </summary>
        /// <param name="lvi"></param>
        /// <returns></returns>
        private static PlanEntry GetPlanEntry(ListViewItem lvi)
        {
            if (lvi == null)
                return null;

            return lvi.Tag as PlanEntry;
        }

        /// <summary>
        /// Gets the first selected item which has a plan entry as a tag.
        /// </summary>
        /// <returns></returns>
        private PlanEntry GetFirstSelectedEntry()
        {
            return lvSkills.SelectedItems[0].Tag as PlanEntry;
        }

        /// <summary>
        /// Gets an enumeration over the selected entries.
        /// </summary>
        private IEnumerable<PlanEntry> SelectedEntries
        {
            get
            {
                return lvSkills.SelectedItems.Cast<ListViewItem>().Where(
                    x => x.Tag is PlanEntry).Select(x => x.Tag as PlanEntry);
            }
        }

        #endregion


        #region List items and columns reordering/resizing

        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns)
                return;

            if (m_columns.Count <= e.ColumnIndex)
                return;

            m_columns[e.ColumnIndex].Width = lvSkills.Columns[e.ColumnIndex].Width;
            Settings.UI.PlanWindow.Add(ExportColumnSettings().ToList());
        }

        /// <summary>
        /// When the user click moves up, we move the list view items and rebuild the plan from 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            List<ListViewItem> items = lvSkills.Items.Cast<ListViewItem>().ToList();

            // Skip the head
            int index = 0;
            while (index < items.Count)
            {
                if (!items[index].Selected)
                    break;
                index++;
            }

            // Move up the following items
            while (index < items.Count)
            {
                ListViewItem item = items[index];
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
        /// When the user click moves down, we move the list view items and rebuild the plan from 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            List<ListViewItem> items = lvSkills.Items.Cast<ListViewItem>().ToList();

            // Skip the tail
            int index = items.Count - 1;
            while (index >= 0)
            {
                if (!items[index].Selected)
                    break;
                index--;
            }

            // Move up the following items
            while (index >= 0)
            {
                ListViewItem item = items[index];
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
        /// Moves the currently selected skill to the top of the plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToTopMenuItem_Click(object sender, EventArgs e)
        {
            // Extract the list and the selected item
            List<ListViewItem> items = lvSkills.Items.Cast<ListViewItem>().ToList();
            int index = items.First(x => x.Selected).Index;
            ListViewItem item = items[index];

            // Remove the item from the list, and add it at the top
            items.RemoveAt(index);
            items.Insert(0, item);

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
                    continue;
                }

                remapping = item.Tag as RemappingPoint;
            }

            // Since the list is not sorted anymore, we disable/hide the sort buttons and feedback
            m_plan.SortingPreferences.Order = ThreeStateSortOrder.None;
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
            m_columns.AddRange(columns);

            // Update the UI
            UpdateListColumns();
        }

        /// <summary>
        /// Rebuild the column settings from the currently displayed columns.
        /// </summary>
        public IEnumerable<PlanColumnSettings> ExportColumnSettings()
        {
            // Recreate the columns
            List<PlanColumnSettings> newList = new List<PlanColumnSettings>();

            // Add the visible columns at the beggining
            foreach (ColumnHeader columnHeader in lvSkills.Columns.Cast<ColumnHeader>().OrderBy(x => x.DisplayIndex))
            {
                // Retrieve the column and skip if null
                PlanColumnSettings column = columnHeader.Tag as PlanColumnSettings;
                if (column == null)
                    continue;

                if (column.Width > -1)
                    column.Width = columnHeader.Width;
                column.Visible = true;
                newList.Add(column);
            }

            // Then the non-displayed ones
            foreach (PlanColumnSettings column in m_columns.Where(x => !newList.Contains(x)))
            {
                column.Visible = false;
                newList.Add(column);
            }

            return newList;
        }

        #endregion


        #region Entries removal

        /// <summary>
        /// Remove all the selected entries when one or more get selected.
        /// </summary>
        private void RemoveSelectedEntries()
        {
            if (lvSkills.SelectedItems.Count == 0)
                return;

            IPlanOperation operation = PrepareSelectionRemoval();
            PlanHelper.SelectPerform(operation);
        }

        /// <summary>
        /// Prepares the selection removal.
        /// </summary>
        /// <returns></returns>
        private IPlanOperation PrepareSelectionRemoval()
        {
            IEnumerable<PlanEntry> entriesToRemove =
                lvSkills.SelectedItems.Cast<ListViewItem>().Where(x => x.Tag is PlanEntry).Select(x => (PlanEntry)x.Tag);
            IPlanOperation operation = m_plan.TryRemoveSet(entriesToRemove);
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
            UpdateSkillList();
        }

        /// <summary>
        /// When the user clicks a column header, we sort things up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader column = lvSkills.Columns[e.Column];
            PlanEntrySort criteria = GetPlanSort(column);

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
                    }
                }
                else
                    m_plan.SortingPreferences.Order = ThreeStateSortOrder.Ascending;
            }

            // Updates the criteria
            m_plan.SortingPreferences.Criteria = criteria;

            // Updates UI and display plan
            UpdateSortVisualFeedback();
            UpdateDisplayPlan();
            UpdateSkillList();
        }

        /// <summary>
        /// Gets a column by the given sort key. Null if not found or "none".
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private ColumnHeader GetColumn(PlanEntrySort criteria)
        {
            return criteria == PlanEntrySort.None
                       ? null
                       : lvSkills.Columns.Cast<ColumnHeader>().FirstOrDefault(header => GetPlanSort(header) == criteria);
        }

        /// <summary>
        /// Gets the sort key for the given column header.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static PlanEntrySort GetPlanSort(ColumnHeader header)
        {
            if (header.Tag == null)
                return PlanEntrySort.TimeDifference;

            PlanColumnSettings ct = (PlanColumnSettings)header.Tag;
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
                case PlanColumn.SkillPointsRequired:
                    return PlanEntrySort.SkillPointsRequired;
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
        private void UpdateSortVisualFeedback()
        {
            // Updates the menu icons on the left toolbar
            tsSortPriorities.Checked = m_plan.SortingPreferences.GroupByPriority;

            ColumnHeader columnWithSortCriteria = GetColumn(m_plan.SortingPreferences.Criteria);
            foreach (ColumnHeader columnHeader in lvSkills.Columns.Cast<ColumnHeader>())
            {
                if (columnWithSortCriteria == columnHeader)
                {
                    if (m_plan.SortingPreferences.Criteria == PlanEntrySort.None ||
                        m_plan.SortingPreferences.Order == ThreeStateSortOrder.None)
                    {
                        columnHeader.ImageIndex = 6;
                        continue;
                    }

                    columnHeader.ImageIndex = (m_plan.SortingPreferences.Order == ThreeStateSortOrder.Ascending
                                                   ? ArrowUpIndex
                                                   : ArrowDownIndex);
                }
                else
                    columnHeader.ImageIndex = 6;
            }
        }

        /// <summary>
        /// Adjusts the columns.
        /// </summary>
        private void AdjustColumns()
        {
            foreach (ColumnHeader column in lvSkills.Columns.Cast<ColumnHeader>())
            {
                if (m_columns[column.Index].Width == -1)
                    m_columns[column.Index].Width = -2;

                column.Width = m_columns[column.Index].Width;

                // Due to .NET design we need to prevent the last colummn to resize to the right end

                // Return if it's not the last column and not set to auto-resize
                if (column.Index != lvSkills.Columns.Count - 1 || m_columns[column.Index].Width != -2)
                    continue;

                const int Pad = 4;

                // Calculate column header text width with padding
                int columnHeaderWidth = TextRenderer.MeasureText(column.Text, Font).Width + Pad * 2;

                // If there is an image assigned to the header, add its width with padding
                if (column.ImageIndex > -1)
                    columnHeaderWidth += lvSkills.SmallImageList.ImageSize.Width + Pad;

                // Calculate the width of the header and the items of the column
                int columnMaxWidth = lvSkills.Columns[column.Index].ListView.Items.Cast<ListViewItem>().Select(
                    item => TextRenderer.MeasureText(item.SubItems[column.Index].Text, Font).Width).Concat(
                        new[] { columnHeaderWidth }).Max() + Pad + 1;

                // Assign the width found
                column.Width = columnMaxWidth;
            }
        }

        #endregion


        #region Context Menu

        /// <summary>
        /// When the context menu is opened, we update the status of the menus.
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

            // Nothing more to do when nothing selected
            if (lvSkills.SelectedItems.Count == 0)
                return;

            // Reset text in case of previous multiple selection
            miRemoveFromPlan.Text = "Remove from Plan";

            // When there is only one selected item...
            if (lvSkills.SelectedItems.Count == 1)
            {
                PlanEntry entry = lvSkills.SelectedItems.Count > 0 ? GetPlanEntry(lvSkills.SelectedItems[0]) : null;

                // When the selected item is a remapping, only "remove from plan" is visible
                if (entry == null)
                {
                    miRemoveFromPlan.Enabled = true;
                    return;
                }

                // Enable other items
                miCopyToNewPlan.Enabled = true;
                miChangePriority.Enabled = true;
                miShowInSkillBrowser.Enabled = true;
                miShowInSkillExplorer.Enabled = true;
                MoveToTopMenuItem.Enabled = true;

                // "Change note"
                miChangeNote.Enabled = true;
                miChangeNote.Text = "View/Change Note...";

                // "Change Planned Level"
                miChangeLevel.Enabled = SetChangeLevelMenu();

                // If "Change Planned Level" disabled, "remove from plan" is visible 
                if (!miChangeLevel.Enabled)
                    miRemoveFromPlan.Enabled = true;

                // "Plan groups"
                if (entry.PlanGroups.Count > 0)
                {
                    miPlanGroups.Enabled = true;

                    List<string> planGroups = new List<string>(entry.PlanGroups);
                    planGroups.Sort();

                    miPlanGroups.DropDownItems.Clear();
                    foreach (string pg in planGroups)
                    {
                        ToolStripButton tempToolStripButton = null;
                        try
                        {
                            tempToolStripButton = new ToolStripButton(pg);
                            tempToolStripButton.Click += planGroupMenu_Click;
                            tempToolStripButton.Width = TextRenderer.MeasureText(pg, tempToolStripButton.Font).Width;

                            ToolStripButton tsb = tempToolStripButton;
                            tempToolStripButton = null;

                            miPlanGroups.DropDownItems.Add(tsb);
                        }
                        finally
                        {
                            if (tempToolStripButton != null)
                                tempToolStripButton.Dispose();
                        }
                    }
                }
            }
                // Multiple items selected
            else
            {
                miCopyToNewPlan.Enabled = true;
                miMarkOwned.Enabled = true;
                miChangePriority.Enabled = true;
                miRemoveFromPlan.Enabled = true;
                IPlanOperation operation = PrepareSelectionRemoval();
                if (PlanHelper.RequiresWindow(operation))
                    miRemoveFromPlan.Text += "...";

                miChangeNote.Enabled = true;
                miChangeNote.Text = "Change Note...";
            }

            // "Mark as owned"
            IEnumerable<Skill> skills = lvSkills.SelectedItems.Cast<ListViewItem>()
                .Where(x => x.Tag is PlanEntry).Select(x => ((PlanEntry)x.Tag).CharacterSkill);
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
            for (int level = 0; level <= 5; level++)
            {
                PlanHelper.UpdatesRegularPlanToMenu(miChangeLevel.DropDownItems[level], m_plan, pe.CharacterSkill, level);
                result |= miChangeLevel.DropDownItems[level].Enabled;
            }

            return result;
        }

        /// <summary>
        /// Context menu > "Select entries from group..." > Groupname
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
            if (entry == null)
                return;

            WindowsFactory<PlanWindow>.GetByTag(m_plan).ShowSkillInBrowser(entry.CharacterSkill);
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
            if (entry == null)
                return;

            WindowsFactory<PlanWindow>.GetByTag(m_plan).ShowSkillInExplorer(entry.CharacterSkill);
        }

        /// <summary>
        /// Context menu > Remove from plan.
        /// Removes the seleted entry or remapping point from the plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miRemoveFromPlan_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = lvSkills.SelectedItems;

            if (items.Count == 1 && items[0].Tag is RemappingPoint)
                tsbToggleRemapping_Click(null, null);
            else
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
            IEnumerable<PlanEntry> entries = SelectedEntries;
            using (PlanPrioritiesEditorForm form = new PlanPrioritiesEditorForm())
            {
                // Gets the entry's priority (or default if more than one item selected)
                form.Priority = PlanEntry.DefaultPriority;
                if (lvSkills.SelectedItems.Count == 1)
                {
                    PlanEntry pe = GetPlanEntry(lvSkills.SelectedItems[0]);
                    if (pe != null)
                        form.Priority = pe.Priority;
                }

                // User canceled ?
                DialogResult dr = form.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                // Update priorities, while performing backup for subsequent check
                if (m_plan.TrySetPriority(DisplayPlan, entries, form.Priority))
                    return;

                ShowPriorityDialogBox(entries, form);
            }
        }

        /// <summary>
        /// Shows the priority dialog box.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="form">The form.</param>
        private void ShowPriorityDialogBox(IEnumerable<PlanEntry> entries, PlanPrioritiesEditorForm form)
        {
            bool showDialog = Settings.UI.PlanWindow.PrioritiesMsgBox.ShowDialogBox;

            // User wishes the dialog to be displayed
            if (showDialog)
            {
                string text = String.Concat("This would result in a priority conflict.",
                                            " (Either pre-requisites with a lower priority or dependant skills with a higher priority).\r\n\r\n",
                                            "Click Yes if you wish to do this and adjust the other skills\r\nor No if you do not wish to change the priority.");
                const string CaptionText = "Priority Conflict";
                const string CbOptionText = "Do not show this dialog again";

                // Shows the custom dialog box
                DialogResult dialogResult = MessageBoxCustom.Show(this, text, CaptionText, CbOptionText, MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Exclamation);
                Settings.UI.PlanWindow.PrioritiesMsgBox.ShowDialogBox = !MessageBoxCustom.CheckBoxChecked;

                // When the checkbox is checked we store the dialog result
                if (MessageBoxCustom.CheckBoxChecked)
                    Settings.UI.PlanWindow.PrioritiesMsgBox.DialogResult = dialogResult;

                if (dialogResult == DialogResult.Yes)
                    m_plan.SetPriority(DisplayPlan, entries, form.Priority);
            }
                // User wishes the dialog not to be displayed and has set the dialog result to "Yes"
            else if (Settings.UI.PlanWindow.PrioritiesMsgBox.DialogResult == DialogResult.Yes)
                m_plan.SetPriority(DisplayPlan, entries, form.Priority);
        }

        /// <summary>
        /// Context menu > Change note.
        /// Opens a box to change the plan's notes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miChangeNote_Click(object sender, EventArgs e)
        {
            IEnumerable<PlanEntry> entries = SelectedEntries;
            if (entries.IsEmpty())
                return;

            // We get the current skill's note and call the note editor window with this initial value
            string noteText = entries.First().Notes;
            string title = (entries.Count() == 1 ? entries.First().Skill.ToString() : "Selected entries");
            using (PlanNotesEditorWindow f = new PlanNotesEditorWindow(title))
            {
                f.NoteText = noteText;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                noteText = f.NoteText;
            }

            // We update every item
            foreach (PlanEntry entry in entries)
            {
                entry.Notes = noteText;
            }

            m_plan.RebuildPlanFrom(DisplayPlan, true);
        }

        /// <summary>
        /// Context > Copy to new plan...
        /// Opens a dialog to prompt the user for a name and create a plan with the selected entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miCopyToNewPlan_Click(object sender, EventArgs e)
        {
            IEnumerable<PlanEntry> entries = SelectedEntries;
            if (entries.IsEmpty())
                return;

            // Ask the user for a new name
            string planName,
                   planDescription;
            using (NewPlanWindow npw = new NewPlanWindow())
            {
                DialogResult dr = npw.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;
                planName = npw.PlanName;
                planDescription = npw.PlanDescription;
            }

            // Create a new plan
            Plan newPlan = new Plan(Character) { Name = planName, Description = planDescription };
            IPlanOperation operation = newPlan.TryAddSet(entries,
                                                         String.Format(CultureConstants.DefaultCulture,
                                                                       "Exported from {0}", m_plan.Name));
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
                foreach (PlanEntry entry in SelectedEntries)
                {
                    entry.CharacterSkill.IsOwned = unowned;
                }
            }

            // We update the skill tree
            skillSelectControl.UpdateContent();

            // Update also the skill browser
            PlanWindow pw = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            pw.UpdateSkillBrowser();
        }

        /// <summary>
        /// Context > Change planned level > Level N
        /// Change the planned level, or remove if 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miChangeToLevel_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            if (menu == null)
                return;

            IPlanOperation operation = menu.Tag as IPlanOperation;
            PlanHelper.SelectPerform(operation);
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
                // Quits if the button is never the left one nor the right one
                if (m_dragButton != MouseButtons.Left && m_dragButton != MouseButtons.Right)
                    return;

                // Checks there is a skill
                Skill dragSkill = GetDraggingSkill(e);
                if (dragSkill == null)
                    return;

                // Gets the item and returns if none created (already on lv5)
                ListViewItem newItem = CreatePlanItemForSkill(dragSkill);
                if (newItem == null)
                    return;

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
                        dragIndex++;
                }

                // Performs the insertion
                lvSkills.Items.Insert(dragIndex, newItem);
                RebuildPlanFromListViewOrder(lvSkills.Items);
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
            if (dragSkill == null)
                return;

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
                lvSkills.ClearDropMarker();
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
                e.Effect = DragDropEffects.Move;
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
                return (Skill)((TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode")).Tag;

            return null;
        }

        /// <summary>
        /// Creates a plan entry and a list view item for it, from the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        private ListViewItem CreatePlanItemForSkill(Skill skill)
        {
            // Gets the planned level of the skill.
            int newLevel = m_plan.GetPlannedLevel(skill) + 1;
            if (skill.Level >= newLevel)
                newLevel = skill.Level + 1;

            // Quits if already on lv5
            if (newLevel > 5)
                return null;

            // Creates the plan entry and list item for this level
            PlanEntry newEntry = new PlanEntry(m_plan, skill, newLevel);
            ListViewItem newItem = new ListViewItem(newEntry.ToString()) { Tag = newEntry };

            return newItem;
        }

        /// <summary>
        /// Gets the mouse button used to drag
        /// </summary>
        /// <param name="e"></param>
        private void SetDragMouseButton(DragEventArgs e)
        {
            if ((e.KeyState & (int)Keys.LButton) != 0)
                m_dragButton = MouseButtons.Left;
            else if ((e.KeyState & (int)Keys.RButton) != 0)
                m_dragButton = MouseButtons.Right;
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
            if (lvSkills.SelectedItems.Count != 1)
                return;

            // When the first entry is a skill, shows it in the skill browser.
            if (GetFirstSelectedEntry() != null)
                miShowInSkillBrowser_Click(sender, e);
            // When it is a remapping point, edit it
            else
                ShowUniqueAttributeOptimizationForm();
        }

        /// <summary>
        /// Shows a unique attribute optimization form.
        /// </summary>
        private void ShowUniqueAttributeOptimizationForm()
        {
            // Retrieves the point
            ListViewItem nextItem = lvSkills.Items[lvSkills.SelectedIndices[0] + 1];
            PlanEntry entry = GetPlanEntry(nextItem);
            RemappingPoint point = entry.Remapping;

            // Display the attributes optimization form
            // if it's not already shown
            if (point == m_formTag)
                return;

            // When we click on another point the previous form closes
            if (m_oldForm != null)
                m_oldForm.Close();

            // Creates the form and displays it
            AttributesOptimizationForm form;
            AttributesOptimizationForm tempForm = null;
            try
            {
                tempForm = new AttributesOptimizationForm(m_character, m_plan, point);
                tempForm.FormClosed += (attributesOptimizationForm, args) => m_formTag = null;
                tempForm.PlanEditor = this;
                tempForm.Show(this);

                form = tempForm;
                tempForm = null;
            }
            finally
            {
                if (tempForm != null)
                    tempForm.Dispose();
            }

            // Update variables for forms display control
            m_formTag = point;
            m_oldForm = form;
        }

        /// <summary>
        /// Handles key press.
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
                    UpdateSkillList();
                    break;
                case Keys.Delete:
                    RemoveSelectedEntries();
                    break;
                case Keys.A:
                    if (e.Control)
                        lvSkills.SelectAll();
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
            if (tmrSelect.Enabled)
                return;

            tmrSelect.Start();
        }

        /// <summary>
        /// When the selection update timer ticks, we process the changes caused by a selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrSelect_Tick(object sender, EventArgs e)
        {
            tmrSelect.Stop();
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
                ResetPrereqMarks();
            }
            else
            {
                tsbMoveUp.Enabled = (lvSkills.SelectedIndices[0] != 0);
                tsbMoveDown.Enabled = (lvSkills.SelectedIndices[lvSkills.SelectedIndices.Count - 1] != lvSkills.Items.Count - 1);
                ResetPrereqMarks();
            }

            // Creates the prerequisite indicators
            foreach (ListViewItem current in lvSkills.Items)
            {
                bool isSameSkill = false;
                bool isPreRequisite = false;
                bool isPostRequisite = false;

                // Checks whether it is a prerequisite of the currently selected entry and whether we should highlight it
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
                                isPostRequisite = true;
                        }

                        if (selectedEntry.Skill.HasAsImmediatePrereq(currentEntry.Skill, out neededLevel))
                        {
                            if (currentEntry.Level == neededLevel)
                                isPreRequisite = true;
                        }

                        if (currentEntry.Skill == selectedEntry.Skill)
                            isSameSkill = true;
                    }
                }

                // Color depends on the entry's status
                if (current.Tag is RemappingPoint)
                    current.ImageIndex = 3;
                else if (isSameSkill)
                    current.ImageIndex = 1;
                else if (isPreRequisite)
                    current.ImageIndex = 2;
                else if (isPostRequisite)
                    current.ImageIndex = 0;
                else
                    current.ImageIndex = -1;
            }

            UpdateStatusBar();
        }

        /// <summary>
        /// Resets the prerequisites marks.
        /// </summary>
        private void ResetPrereqMarks()
        {
            // We avoid using Invalidate() as it cause the whole listview to flicker 
            foreach (ListViewItem current in lvSkills.Items)
            {
                current.ImageIndex = -1;
            }
        }

        /// <summary>
        /// When the user hovers an item, we update the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSkills_ItemHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            ListViewItem lvi = e.Item;
            if (lvi == null)
                return;

            // Is it an entry ?
            if (lvi.Tag is PlanEntry)
            {
                Skill skill = GetPlanEntry(lvi).CharacterSkill;
                StringBuilder builder = new StringBuilder(skill.Description);

                if (!skill.IsKnown)
                {
                    builder.Append("\n\nYou do not know this skill - you ");
                    if (!skill.IsOwned)
                        builder.Append("do not ");
                    builder.Append("own the skillbook");
                }
                lvi.ToolTipText = builder.ToString();
            }
                // Then it is a remapping point
            else if (lvi.Tag is RemappingPoint)
            {
                RemappingPoint point = lvi.Tag as RemappingPoint;
                lvi.ToolTipText = (m_areRemappingPointsActive ? point.ToLongString() : "Remapping (ignored)");
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
        /// Display a skill list on the right.
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
            if (lvSkills.SelectedIndices.Count == 0)
                return;

            ListViewItem item = lvSkills.SelectedItems[0];
            object tag = item.Tag;

            // Remove an existing point
            if (tag is RemappingPoint)
            {
                // Selects the next item and focuses it.
                int entryIndex = lvSkills.SelectedItems[0].Index + 1;
                lvSkills.Items[entryIndex].Selected = true;
                lvSkills.Items[entryIndex].Focused = true;

                // Retrieve the original entry after this item and remove its remapping point.
                PlanEntry entry = lvSkills.Items[entryIndex].Tag as PlanEntry;
                PlanEntry originalEntry = GetOriginalEntry(entry);
                originalEntry.Remapping = null;
            }
                // Toggle on a skill
            else
            {
                // Retrieves the focused item's hash code.
                item.Focused = true;

                PlanEntry entry = tag as PlanEntry;
                PlanEntry originalEntry = GetOriginalEntry(entry);

                // Add a remapping point
                originalEntry.Remapping = originalEntry.Remapping == null ? new RemappingPoint() : null;
            }
        }

        /// <summary>
        /// Left toolbar > Toggle color key panel.
        /// Display a color key panel at the bottom.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbColorKey_Click(object sender, EventArgs e)
        {
            pFooter.Visible = !pFooter.Visible;
            tsbColorKey.Checked = pFooter.Visible;
        }

        /// <summary>
        /// When the user clicks the "Columns Settings",
        /// we display the suggestions window and save the changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void columnSettingsMenuItem_Click(object sender, EventArgs e)
        {
            // Update the settings from the current columns
            using (PlanColumnSelectWindow dialog = new PlanColumnSelectWindow(ExportColumnSettings()))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                ImportColumnSettings(dialog.Columns.Cast<PlanColumnSettings>());
                Settings.UI.PlanWindow.Add(ExportColumnSettings().ToList());
            }
        }

        /// <summary>
        /// Auto-Sizes the columns width.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void autoSizeColumnsMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ColumnHeader column in lvSkills.Columns.Cast<ColumnHeader>())
            {
                m_columns[column.Index].Width = -2;
            }
            UpdateListColumns();
        }

        /// <summary>
        /// When the implant set changes we update the plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChooseImplantSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbChooseImplantSet.SelectedIndex == m_lastImplantSetIndex)
                return;

            UpdateImplantSet();
            UpdateSkillList();
        }

        /// <summary>
        /// When the user clicks the "choose implant set", we update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChooseImplantSet_DropDown(object sender, EventArgs e)
        {
            UpdateImplantSetList();
        }

        /// <summary>
        /// When the user doesn't select a set, display the last selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChooseImplantSet_DropDownClosed(object sender, EventArgs e)
        {
            if (cbChooseImplantSet.SelectedIndex == -1)
                cbChooseImplantSet.SelectedIndex = m_lastImplantSetIndex;
        }

        #endregion
    }
}