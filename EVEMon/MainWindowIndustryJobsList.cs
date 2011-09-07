using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public partial class MainWindowIndustryJobsList : UserControl, IGroupingListView
    {
        private readonly List<IndustryJobColumnSettings> m_columns = new List<IndustryJobColumnSettings>();
        private readonly List<IndustryJob> m_list = new List<IndustryJob>();

        private IndustryJobGrouping m_grouping;
        private IndustryJobColumn m_sortCriteria;
        private IssuedFor m_showIssuedFor;

        private string m_textFilter = String.Empty;
        private bool m_sortAscending = true;

        private bool m_hideInactive;
        private bool m_isUpdatingColumns;
        private bool m_columnsChanged;
        private bool m_init;

        private int m_displayIndexTTC;

        // Panel info variables
        private int m_skillBasedManufacturingJobs,
                    m_skillBasedResearchingJobs;

        private int m_remoteManufacturingRange,
                    m_remoteResearchingRange;

        private int m_activeManufJobsIssuedForCharacterCount,
                    m_activeManufJobsIssuedForCorporationCount;

        private int m_activeResearchJobsIssuedForCharacterCount,
                    m_activeResearchJobsIssuedForCorporationCount;


        # region Constructor

        public MainWindowIndustryJobsList()
        {
            InitializeComponent();
            InitializeExpandablePanelControls();

            lvJobs.Visible = false;
            lvJobs.AllowColumnReorder = true;
            lvJobs.Columns.Clear();

            noJobsLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            industryExpPanelControl.Font = FontFactory.GetFont("Tahoma", 8.25f);
            industryExpPanelControl.Visible = false;

            ListViewHelper.EnableDoubleBuffer(lvJobs);

            lvJobs.ColumnClick += lvJobs_ColumnClick;
            lvJobs.KeyDown += lvJobs_KeyDown;
            lvJobs.ColumnWidthChanged += lvJobs_ColumnWidthChanged;
            lvJobs.ColumnReordered += lvJobs_ColumnReordered;

            Resize += MainWindowIndustryJobsList_Resize;

            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.CharacterIndustryJobsUpdated += EveMonClient_CharacterIndustryJobsUpdated;
            EveMonClient.CharacterIndustryJobsCompleted += EveMonClient_IndustryJobsCompleted;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        public Character Character { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="lvJobs"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visibility
        {
            get { return lvJobs.Visible; }
            set { lvJobs.Visible = value; }
        }

        /// <summary>
        /// Gets or sets the text filter.
        /// </summary>
        public string TextFilter
        {
            get { return m_textFilter; }
            set
            {
                m_textFilter = value;
                if (m_init)
                    UpdateColumns();
            }
        }

        /// <summary>
        /// Gets or sets the grouping mode.
        /// </summary>
        public Enum Grouping
        {
            get { return m_grouping; }
            set
            {
                m_grouping = (IndustryJobGrouping)value;
                if (m_init)
                    UpdateColumns();
            }
        }

        /// <summary>
        /// Gets or sets which "Issued for" jobs to display.
        /// </summary>
        public IssuedFor ShowIssuedFor
        {
            get { return m_showIssuedFor; }
            set
            {
                m_showIssuedFor = value;
                if (m_init)
                    UpdateColumns();
            }
        }

        /// <summary>
        /// Gets true when character has active jobs issued for corporation.
        /// </summary>
        public bool HasActiveCorporationIssuedJobs
        {
            get
            {
                return m_list.Any(x =>
                                  x.State == JobState.Active && x.IssuedFor == IssuedFor.Corporation);
            }
        }

        /// <summary>
        /// Gets or sets the enumeration of jobs to display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<IndustryJob> Jobs
        {
            get { return m_list; }
            set
            {
                m_list.Clear();
                if (value == null)
                    return;

                m_list.AddRange(value);
            }
        }

        /// <summary>
        /// Gets or sets the settings used for columns.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<IndustryJobColumnSettings> Columns
        {
            get
            {
                // Add the visible columns; matching the display order
                List<IndustryJobColumnSettings> newColumns = new List<IndustryJobColumnSettings>();
                foreach (ColumnHeader header in lvJobs.Columns.Cast<ColumnHeader>().OrderBy(x => x.DisplayIndex))
                {
                    IndustryJobColumnSettings columnSetting = m_columns.First(x => x.Column == (IndustryJobColumn)header.Tag);
                    if (columnSetting.Width != -1)
                        columnSetting.Width = header.Width;

                    newColumns.Add(columnSetting);
                }

                // Then add the other columns
                newColumns.AddRange(m_columns.Where(x => !x.Visible));

                return newColumns;
            }
            set
            {
                m_columns.Clear();
                if (value != null)
                    m_columns.AddRange(value.Select(x => x.Clone()));

                // Whenever the columns changes, we need to
                // reset the dipslay index of the TTC column
                m_displayIndexTTC = -1;

                if (m_init)
                    UpdateColumns();
            }
        }

        #endregion


        # region Inherited Events

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.CharacterIndustryJobsUpdated -= EveMonClient_CharacterIndustryJobsUpdated;
            EveMonClient.CharacterIndustryJobsCompleted -= EveMonClient_IndustryJobsCompleted;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// When the control becomes visible again, we update the content.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted() || Character == null)
                return;

            base.OnVisibleChanged(e);

            if (!Visible)
                return;

            // Prevents the properties to call UpdateColumns() till we set all properties
            m_init = false;

            CCPCharacter ccpCharacter = Character as CCPCharacter;
            Jobs = (ccpCharacter == null ? null : ccpCharacter.IndustryJobs);
            Columns = Settings.UI.MainWindow.IndustryJobs.Columns;
            Grouping = (Character == null ? IndustryJobGrouping.State : Character.UISettings.JobsGroupBy);

            UpdateColumns();

            m_init = true;

            UpdateContent();
        }

        # endregion


        #region Updates Main Industry Window On Global Events

        /// <summary>
        /// Updates the columns.
        /// </summary>
        public void UpdateColumns()
        {
            lvJobs.BeginUpdate();
            m_isUpdatingColumns = true;

            try
            {
                lvJobs.Columns.Clear();

                foreach (IndustryJobColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    ColumnHeader header = lvJobs.Columns.Add(column.Column.GetHeader(), column.Column.GetHeader(), column.Width);
                    header.Tag = column.Column;
                }

                // We update the content
                UpdateContent();

                // Force the auto-resize of the columns with -1 width
                ColumnHeaderAutoResizeStyle resizeStyle = (lvJobs.Items.Count == 0
                                                               ? ColumnHeaderAutoResizeStyle.HeaderSize
                                                               : ColumnHeaderAutoResizeStyle.ColumnContent);

                int index = 0;
                foreach (IndustryJobColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    if (column.Width == -1)
                        lvJobs.AutoResizeColumn(index, resizeStyle);

                    index++;
                }
            }
            finally
            {
                lvJobs.EndUpdate();
                m_isUpdatingColumns = false;
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        private void UpdateContent()
        {
            // Returns if not visible
            if (!Visible)
                return;


            // Store the selected item (if any) to restore it after the update
            int selectedItem = (lvJobs.SelectedItems.Count > 0
                                    ? lvJobs.SelectedItems[0].Tag.GetHashCode()
                                    : 0);

            m_hideInactive = Settings.UI.MainWindow.IndustryJobs.HideInactiveJobs;

            lvJobs.BeginUpdate();
            try
            {
                string text = m_textFilter.ToLowerInvariant();
                IEnumerable<IndustryJob> jobs = m_list.Where(x => !x.Ignored && IsTextMatching(x, text));

                if (Character != null && m_hideInactive)
                    jobs = jobs.Where(x => x.IsActive);

                if (m_showIssuedFor != IssuedFor.All)
                    jobs = jobs.Where(x => x.IssuedFor == m_showIssuedFor);

                UpdateSort();

                switch (m_grouping)
                {
                    case IndustryJobGrouping.State:
                        IOrderedEnumerable<IGrouping<JobState, IndustryJob>> groups0 =
                            jobs.GroupBy(x => x.State).OrderBy(x => (int)x.Key);
                        UpdateContent(groups0);
                        break;
                    case IndustryJobGrouping.StateDesc:
                        IOrderedEnumerable<IGrouping<JobState, IndustryJob>> groups1 =
                            jobs.GroupBy(x => x.State).OrderByDescending(x => (int)x.Key);
                        UpdateContent(groups1);
                        break;
                    case IndustryJobGrouping.EndDate:
                        IOrderedEnumerable<IGrouping<DateTime, IndustryJob>> groups2 =
                            jobs.GroupBy(x => x.EndProductionTime.Date).OrderBy(x => x.Key);
                        UpdateContent(groups2);
                        break;
                    case IndustryJobGrouping.EndDateDesc:
                        IOrderedEnumerable<IGrouping<DateTime, IndustryJob>> groups3 =
                            jobs.GroupBy(x => x.EndProductionTime.Date).OrderByDescending(x => x.Key);
                        UpdateContent(groups3);
                        break;
                    case IndustryJobGrouping.InstalledItemType:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups4 =
                            jobs.GroupBy(x => x.InstalledItem.MarketGroup.GetCategoryPath()).OrderBy(x => x.Key);
                        UpdateContent(groups4);
                        break;
                    case IndustryJobGrouping.InstalledItemTypeDesc:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups5 =
                            jobs.GroupBy(x => x.InstalledItem.MarketGroup.GetCategoryPath()).OrderByDescending(x => x.Key);
                        UpdateContent(groups5);
                        break;
                    case IndustryJobGrouping.OutputItemType:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups6 =
                            jobs.GroupBy(x => x.OutputItem.MarketGroup.GetCategoryPath()).OrderBy(x => x.Key);
                        UpdateContent(groups6);
                        break;
                    case IndustryJobGrouping.OutputItemTypeDesc:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups7 =
                            jobs.GroupBy(x => x.OutputItem.MarketGroup.GetCategoryPath()).OrderByDescending(x => x.Key);
                        UpdateContent(groups7);
                        break;
                    case IndustryJobGrouping.Activity:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups8 =
                            jobs.GroupBy(x => x.Activity.GetDescription()).OrderBy(x => x.Key);
                        UpdateContent(groups8);
                        break;
                    case IndustryJobGrouping.ActivityDesc:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups9 =
                            jobs.GroupBy(x => x.Activity.GetDescription()).OrderByDescending(x => x.Key);
                        UpdateContent(groups9);
                        break;
                    case IndustryJobGrouping.Location:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups10 =
                            jobs.GroupBy(x => x.Installation).OrderBy(x => x.Key);
                        UpdateContent(groups10);
                        break;
                    case IndustryJobGrouping.LocationDesc:
                        IOrderedEnumerable<IGrouping<string, IndustryJob>> groups11 =
                            jobs.GroupBy(x => x.Installation).OrderByDescending(x => x.Key);
                        UpdateContent(groups11);
                        break;
                }

                // Restore the selected item (if any)
                if (selectedItem > 0)
                {
                    foreach (ListViewItem lvItem in lvJobs.Items.Cast<ListViewItem>().Where(
                        lvItem => lvItem.Tag.GetHashCode() == selectedItem))
                    {
                        lvItem.Selected = true;
                    }
                }

                // Update the expandable panel info
                UpdateExpPanelContent();

                // Display or hide the "no jobs" label
                if (m_init)
                {
                    noJobsLabel.Visible = jobs.IsEmpty();
                    lvJobs.Visible = !jobs.IsEmpty();
                    industryExpPanelControl.Visible = true;
                    industryExpPanelControl.Header.Visible = true;
                }
            }
            finally
            {
                lvJobs.EndUpdate();
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groups"></param>
        private void UpdateContent<TKey>(IEnumerable<IGrouping<TKey, IndustryJob>> groups)
        {
            lvJobs.Items.Clear();
            lvJobs.Groups.Clear();

            // Add the groups
            foreach (IGrouping<TKey, IndustryJob> group in groups)
            {
                string groupText;
                if (group.Key is JobState)
                    groupText = ((JobState)(Object)group.Key).GetHeader();
                else if (group.Key is DateTime)
                    groupText = ((DateTime)(Object)group.Key).ToShortDateString();
                else
                    groupText = group.Key.ToString();

                ListViewGroup listGroup = new ListViewGroup(groupText);
                lvJobs.Groups.Add(listGroup);

                // Add the items in every group
                foreach (IndustryJob job in group)
                {
                    if (job.InstalledItem == null)
                        continue;

                    ListViewItem item = new ListViewItem(job.InstalledItem.Name, listGroup)
                                            { UseItemStyleForSubItems = false, Tag = job };

                    // Display text as dimmed if the job is no longer available
                    if (!job.IsActive)
                        item.ForeColor = SystemColors.GrayText;

                    // Add enough subitems to match the number of columns
                    while (item.SubItems.Count < lvJobs.Columns.Count + 1)
                    {
                        item.SubItems.Add(String.Empty);
                    }

                    // Creates the subitems
                    for (int i = 0; i < lvJobs.Columns.Count; i++)
                    {
                        ColumnHeader header = lvJobs.Columns[i];
                        IndustryJobColumn column = (IndustryJobColumn)header.Tag;
                        SetColumn(job, item.SubItems[i], column);
                    }

                    lvJobs.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Updates the item sorter.
        /// </summary>
        private void UpdateSort()
        {
            lvJobs.ListViewItemSorter = new ListViewItemComparerByTag<IndustryJob>(
                new IndustryJobComparer(m_sortCriteria, m_sortAscending));

            UpdateSortVisualFeedback();
        }

        /// <summary>
        /// Updates the sort feedback (the arrow on the header).
        /// </summary>
        private void UpdateSortVisualFeedback()
        {
            for (int i = 0; i < lvJobs.Columns.Count; i++)
            {
                IndustryJobColumn column = (IndustryJobColumn)lvJobs.Columns[i].Tag;
                if (m_sortCriteria == column)
                    lvJobs.Columns[i].ImageIndex = (m_sortAscending ? 0 : 1);
                else
                    lvJobs.Columns[i].ImageIndex = 2;
            }
        }

        /// <summary>
        /// Updates the listview sub-item.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="item"></param>
        /// <param name="column"></param>
        private void SetColumn(IndustryJob job, ListViewItem.ListViewSubItem item, IndustryJobColumn column)
        {
            switch (column)
            {
                case IndustryJobColumn.State:
                    item.Text = (job.State == JobState.Active
                                     ? job.ActiveJobState.GetDescription()
                                     : job.State.ToString());
                    item.ForeColor = GetStateColor(job);
                    break;
                case IndustryJobColumn.TTC:
                    item.Text = job.TTC;
                    if (job.State == JobState.Paused)
                        item.ForeColor = Color.Red;
                    break;
                case IndustryJobColumn.InstalledItem:
                    item.Text = job.InstalledItem.Name;
                    break;
                case IndustryJobColumn.InstalledItemType:
                    item.Text = job.InstalledItem.MarketGroup.Name;
                    break;
                case IndustryJobColumn.OutputItem:
                    item.Text = String.Format("{0} Unit{1} of {2}", GetUnitCount(job),
                                              (GetUnitCount(job) > 1 ? "s" : String.Empty), job.OutputItem.Name);
                    break;
                case IndustryJobColumn.OutputItemType:
                    item.Text = job.OutputItem.MarketGroup.Name;
                    break;
                case IndustryJobColumn.Activity:
                    item.Text = job.Activity.GetDescription();
                    break;
                case IndustryJobColumn.InstallTime:
                    item.Text = job.InstalledTime.ToLocalTime().ToString();
                    break;
                case IndustryJobColumn.EndTime:
                    item.Text = job.EndProductionTime.ToLocalTime().ToString();
                    break;
                case IndustryJobColumn.OriginalOrCopy:
                    item.Text = job.BlueprintType.ToString();
                    break;
                case IndustryJobColumn.InstalledME:
                    item.Text = (job.Activity == BlueprintActivity.ResearchingMaterialProductivity
                                     ? job.InstalledME.ToString()
                                     : String.Empty);
                    break;
                case IndustryJobColumn.EndME:
                    item.Text = (job.Activity == BlueprintActivity.ResearchingMaterialProductivity
                                     ? (job.InstalledME + job.Runs).ToString()
                                     : String.Empty);
                    break;
                case IndustryJobColumn.InstalledPE:
                    item.Text = (job.Activity == BlueprintActivity.ResearchingTimeProductivity
                                     ? job.InstalledPE.ToString()
                                     : String.Empty);
                    break;
                case IndustryJobColumn.EndPE:
                    item.Text = (job.Activity == BlueprintActivity.ResearchingTimeProductivity
                                     ? (job.InstalledPE + job.Runs).ToString()
                                     : String.Empty);
                    break;
                case IndustryJobColumn.Location:
                    item.Text = job.FullLocation;
                    break;
                case IndustryJobColumn.Region:
                    item.Text = job.SolarSystem.Constellation.Region.Name;
                    break;
                case IndustryJobColumn.SolarSystem:
                    item.Text = job.SolarSystem.Name;
                    break;
                case IndustryJobColumn.Installation:
                    item.Text = job.Installation;
                    break;
                case IndustryJobColumn.IssuedFor:
                    item.Text = job.IssuedFor.ToString();
                    break;
                case IndustryJobColumn.LastStateChange:
                    item.Text = job.LastStateChange.ToLocalTime().ToString();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion


        # region Helper Methods

        /// <summary>
        /// Gets the unit count.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns></returns>
        private int GetUnitCount(IndustryJob job)
        {
            if (job.Activity != BlueprintActivity.Manufacturing)
                return 1;

            // Returns the ammount produced
            return job.Runs * job.OutputItem.PortionSize;
        }

        /// <summary>
        /// Gets the color of the state.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns></returns>
        private Color GetStateColor(IndustryJob job)
        {
            switch (job.State)
            {
                case JobState.Canceled:
                    return Color.DarkGray;
                case JobState.Failed:
                    return Color.DarkRed;
                case JobState.Paused:
                    return Color.RoyalBlue;
                case JobState.Active:
                    return GetActiveJobStateColor(job.ActiveJobState);
                default:
                    return SystemColors.GrayText;
            }
        }

        /// <summary>
        /// Gets the color of the active job state.
        /// </summary>
        /// <param name="activeJobState">State of the active job.</param>
        /// <returns></returns>
        private Color GetActiveJobStateColor(ActiveJobState activeJobState)
        {
            switch (activeJobState)
            {
                case ActiveJobState.Pending:
                    return Color.Red;
                case ActiveJobState.InProgress:
                    return Color.Orange;
                case ActiveJobState.Ready:
                    return Color.Green;
                default:
                    return SystemColors.GrayText;
            }
        }

        /// <summary>
        /// Checks the given text matches the item.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if [is text matching] [the specified x]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTextMatching(IndustryJob x, string text)
        {
            return String.IsNullOrEmpty(text)
                   || x.InstalledItem.Name.ToLowerInvariant().Contains(text)
                   || x.OutputItem.Name.ToLowerInvariant().Contains(text)
                   || x.Installation.ToLowerInvariant().Contains(text)
                   || x.SolarSystem.Name.ToLowerInvariant().Contains(text)
                   || x.SolarSystem.Constellation.Name.ToLowerInvariant().Contains(text)
                   || x.SolarSystem.Constellation.Region.Name.ToLowerInvariant().Contains(text);
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handles the MouseHover event of the lvJobs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void lvJobs_MouseHover(object sender, EventArgs e)
        {
            Focus();
        }

        /// <summary>
        /// On resize, updates the controls visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowIndustryJobsList_Resize(object sender, EventArgs e)
        {
            if (!m_init)
                return;

            UpdateContent();
        }

        /// <summary>
        /// On column reorder we update the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvJobs_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvJobs_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns || m_columns.Count <= e.ColumnIndex)
                return;

            // Don't update the columns if the TTC column width changes
            if (e.ColumnIndex == m_displayIndexTTC)
                return;

            m_columns[e.ColumnIndex].Width = lvJobs.Columns[e.ColumnIndex].Width;
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user clicks a column header, we update the sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvJobs_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            IndustryJobColumn column = (IndustryJobColumn)lvJobs.Columns[e.Column].Tag;
            if (m_sortCriteria == column)
                m_sortAscending = !m_sortAscending;
            else
            {
                m_sortCriteria = column;
                m_sortAscending = true;
            }

            UpdateContent();
        }

        /// <summary>
        /// Handles key press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvJobs_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Control)
                        lvJobs.SelectAll();
                    break;
                case Keys.Delete:
                    if (lvJobs.SelectedItems.Count == 0)
                        return;
                    // Mark as ignored
                    foreach (IndustryJob job in from ListViewItem item in lvJobs.SelectedItems select (IndustryJob)item.Tag)
                    {
                        job.Ignored = true;
                    }
                    // Updates
                    UpdateContent();
                    break;
            }
        }

        # endregion


        #region Global events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_IndustryJobsCompleted(object sender, IndustryJobsEventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// When the industry jobs are changed,
        /// update the list and the expandable panel info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterIndustryJobsUpdated(object sender, CharacterChangedEventArgs e)
        {
            CCPCharacter ccpCharacter = Character as CCPCharacter;
            if (ccpCharacter == null || e.Character != ccpCharacter)
                return;

            Jobs = ccpCharacter.IndustryJobs;
            UpdateColumns();
        }

        /// <summary>
        /// On timer tick, we update the time to completion, the active job state
        /// and the columns settings if any changes have beem made to them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            int colIndexTTC = m_columns.IndexOf(m_columns.FirstOrDefault(x => x.Column == IndustryJobColumn.TTC));

            for (int i = 0; i < lvJobs.Items.Count; i++)
            {
                IndustryJob job = (IndustryJob)lvJobs.Items[i].Tag;
                if (!job.IsActive || job.ActiveJobState == ActiveJobState.Ready)
                    continue;

                // Update the time to completion
                if (colIndexTTC != -1 && m_columns[colIndexTTC].Visible)
                {
                    if (m_displayIndexTTC == -1)
                        m_displayIndexTTC = lvJobs.Columns[IndustryJobColumn.TTC.GetHeader()].DisplayIndex;

                    lvJobs.Items[i].SubItems[m_displayIndexTTC].Text = job.TTC;

                    // Using AutoResizeColumn when TTC is the first column
                    // results to a nasty visual bug due to ListViewItem.ImageIndex placeholder
                    if (m_displayIndexTTC == 0)
                    {
                        int columnWidth = (lvJobs.Items.Cast<ListViewItem>().Select(
                            item => TextRenderer.MeasureText(item.SubItems[m_displayIndexTTC].Text, Font).Width)).Concat(new[]
                                                                                                                             { 0 })
                            .Max();
                        lvJobs.Columns[m_displayIndexTTC].Width = columnWidth + 22;
                    }
                    else
                        lvJobs.AutoResizeColumn(m_displayIndexTTC, ColumnHeaderAutoResizeStyle.ColumnContent);
                }

                // Job was pending and its time to start
                if (job.ActiveJobState == ActiveJobState.Pending && job.BeginProductionTime < DateTime.UtcNow)
                {
                    job.ActiveJobState = ActiveJobState.InProgress;
                    UpdateContent();
                }

                // Job is ready
                if (job.TTC == String.Empty)
                    job.ActiveJobState = ActiveJobState.Ready;
            }

            if (!m_columnsChanged)
                return;

            Settings.UI.MainWindow.IndustryJobs.Columns = Columns.Select(x => x.Clone()).ToArray();

            // Recreate the columns
            Columns = Settings.UI.MainWindow.IndustryJobs.Columns;
            m_columnsChanged = false;
        }

        # endregion


        #region Updates Expandable Panel On Global Events

        /// <summary>
        /// Updates the content of the expandable panel.
        /// </summary>
        private void UpdateExpPanelContent()
        {
            if (Character == null)
            {
                industryExpPanelControl.Visible = false;
                return;
            }

            // Update the Header text of the panel
            UpdateHeaderText();

            // Update the info in the panel
            UpdatePanelInfo();

            // Force to redraw
            industryExpPanelControl.Refresh();
        }

        /// <summary>
        /// Updates the header text of the panel.
        /// </summary>
        private void UpdateHeaderText()
        {
            const int BaseJobs = 1;
            int maxManufacturingJobs = BaseJobs + m_skillBasedManufacturingJobs;
            int maxResearchingJobs = BaseJobs + m_skillBasedResearchingJobs;
            int activeManufacturingJobs = m_activeManufJobsIssuedForCharacterCount + m_activeManufJobsIssuedForCorporationCount;
            int activeResearchingJobs = m_activeResearchJobsIssuedForCharacterCount +
                                        m_activeResearchJobsIssuedForCorporationCount;
            int remainingManufacturingJobs = maxManufacturingJobs - activeManufacturingJobs;
            int remainingResearchingJobs = maxResearchingJobs - activeResearchingJobs;

            string manufJobsRemainingText = String.Format(CultureConstants.DefaultCulture,
                                                          "Manufacturing Jobs Remaining: {0} out of {1} max",
                                                          remainingManufacturingJobs, maxManufacturingJobs);
            string researchJobsRemainingText = String.Format(CultureConstants.DefaultCulture,
                                                             "Researching Jobs Remaining: {0} out of {1} max",
                                                             remainingResearchingJobs, maxResearchingJobs);
            industryExpPanelControl.HeaderText = String.Format(CultureConstants.DefaultCulture, "{0}{2,5}{1}",
                                                               manufJobsRemainingText, researchJobsRemainingText, String.Empty);
        }

        /// <summary>
        /// Updates the labels text in the panel.
        /// </summary>
        private void UpdatePanelInfo()
        {
            // Calculate the related info for the panel
            CalculatePanelInfo();

            // Update text
            int activeManufacturingJobsCount = m_activeManufJobsIssuedForCharacterCount +
                                               m_activeManufJobsIssuedForCorporationCount;
            int activeResearchingJobsCount = m_activeResearchJobsIssuedForCharacterCount +
                                             m_activeResearchJobsIssuedForCorporationCount;

            string remoteManufacturingRange = StaticGeography.GetRange(m_remoteManufacturingRange);
            string remoteResearchingRange = StaticGeography.GetRange(m_remoteResearchingRange);

            // Basic label text
            m_lblActiveManufacturingJobs.Text = String.Format(
                CultureConstants.DefaultCulture, "Active Manufacturing Jobs: {0}", activeManufacturingJobsCount);
            m_lblActiveResearchingJobs.Text = String.Format(
                CultureConstants.DefaultCulture, "Active Researching Jobs: {0}", activeResearchingJobsCount);
            m_lblRemoteManufacturingRange.Text = String.Format(
                CultureConstants.DefaultCulture, "Remote Manufacturing Range: limited to {0}", remoteManufacturingRange);
            m_lblRemoteResearchingRange.Text = String.Format(
                CultureConstants.DefaultCulture, "Remote Researching Range: limited to {0}", remoteResearchingRange);

            // Supplemental label text
            m_lblActiveCharManufacturingJobs.Text = String.Format(
                CultureConstants.DefaultCulture, "Character Issued: {0}", m_activeManufJobsIssuedForCharacterCount);
            m_lblActiveCorpManufacturingJobs.Text = String.Format(
                CultureConstants.DefaultCulture, "Corporation Issued: {0}", m_activeManufJobsIssuedForCorporationCount);
            m_lblActiveCharResearchingJobs.Text = String.Format(
                CultureConstants.DefaultCulture, "Character Issued: {0}", m_activeResearchJobsIssuedForCharacterCount);
            m_lblActiveCorpResearchingJobs.Text = String.Format(
                CultureConstants.DefaultCulture, "Corporation Issued: {0}", m_activeResearchJobsIssuedForCorporationCount);

            // Update label position
            UpdatePanelControlPosition();
        }

        /// <summary>
        /// Updates expandable panel controls positions.
        /// </summary>
        private void UpdatePanelControlPosition()
        {
            industryExpPanelControl.SuspendLayout();

            const int Pad = 5;
            int height = (industryExpPanelControl.ExpandDirection == Direction.Up ? Pad : industryExpPanelControl.HeaderHeight);

            m_lblActiveManufacturingJobs.Location = new Point(5, height);
            m_lblRemoteManufacturingRange.Location = new Point(m_lblRemoteManufacturingRange.Location.X, height);
            height += m_lblActiveManufacturingJobs.Height;
            m_lblRemoteResearchingRange.Location = new Point(m_lblRemoteResearchingRange.Location.X, height);
            if (HasActiveCorporationIssuedJobs)
            {
                m_lblActiveCharManufacturingJobs.Location = new Point(15, height);
                m_lblActiveCharManufacturingJobs.Visible = true;
                height += m_lblActiveCharManufacturingJobs.Height;

                m_lblActiveCorpManufacturingJobs.Location = new Point(15, height);
                m_lblActiveCorpManufacturingJobs.Visible = true;
                height += m_lblActiveCorpManufacturingJobs.Height;
                height += Pad;
            }
            else
            {
                m_lblActiveCharManufacturingJobs.Visible = false;
                m_lblActiveCorpManufacturingJobs.Visible = false;
            }

            m_lblActiveResearchingJobs.Location = new Point(5, height);
            height += m_lblActiveResearchingJobs.Height;

            if (HasActiveCorporationIssuedJobs)
            {
                m_lblActiveCharResearchingJobs.Location = new Point(15, height);
                m_lblActiveCharResearchingJobs.Visible = true;
                height += m_lblActiveCharResearchingJobs.Height;

                m_lblActiveCorpResearchingJobs.Location = new Point(15, height);
                m_lblActiveCorpResearchingJobs.Visible = true;
                height += m_lblActiveCorpResearchingJobs.Height;
            }
            else
            {
                m_lblActiveCharResearchingJobs.Visible = false;
                m_lblActiveCorpResearchingJobs.Visible = false;
            }

            height += Pad;

            // Update panel's expanded height
            industryExpPanelControl.ExpandedHeight = height +
                                                     (industryExpPanelControl.ExpandDirection == Direction.Up
                                                          ? industryExpPanelControl.HeaderHeight
                                                          : Pad);

            industryExpPanelControl.ResumeLayout();
        }

        /// <summary>
        /// Calculates the industry jobs related info for the panel.
        /// </summary>
        private void CalculatePanelInfo()
        {
            IEnumerable<IndustryJob> activeManufJobsIssuedForCharacter = m_list.Where(x => (x.State == JobState.Active)
                                                                                           &&
                                                                                           x.Activity ==
                                                                                           BlueprintActivity.Manufacturing &&
                                                                                           x.IssuedFor == IssuedFor.Character);
            IEnumerable<IndustryJob> activeManufJobsIssuedForCorporation = m_list.Where(x => (x.State == JobState.Active)
                                                                                             &&
                                                                                             x.Activity ==
                                                                                             BlueprintActivity.Manufacturing &&
                                                                                             x.IssuedFor == IssuedFor.Corporation);
            IEnumerable<IndustryJob> activeResearchJobsIssuedForCharacter = m_list.Where(x => (x.State == JobState.Active)
                                                                                              &&
                                                                                              x.Activity !=
                                                                                              BlueprintActivity.Manufacturing &&
                                                                                              x.IssuedFor == IssuedFor.Character);
            IEnumerable<IndustryJob> activeResearchJobsIssuedForCorporation = m_list.Where(x => (x.State == JobState.Active)
                                                                                                &&
                                                                                                x.Activity !=
                                                                                                BlueprintActivity.Manufacturing &&
                                                                                                x.IssuedFor ==
                                                                                                IssuedFor.Corporation);

            // Calculate character's max manufacturing jobs
            m_skillBasedManufacturingJobs = Character.Skills[DBConstants.MassProductionSkillID].LastConfirmedLvl
                                            + Character.Skills[DBConstants.AdvancedMassProductionSkillID].LastConfirmedLvl;

            // Calculate character's max researching jobs
            m_skillBasedResearchingJobs = Character.Skills[DBConstants.LaboratoryOperationSkillID].LastConfirmedLvl
                                          + Character.Skills[DBConstants.AdvancedLaboratoryOperationSkillID].LastConfirmedLvl;

            // Calculate character's remote manufacturing range
            m_remoteManufacturingRange = Character.Skills[DBConstants.SupplyChainManagementSkillID].LastConfirmedLvl;

            // Calculate character's remote researching range
            m_remoteResearchingRange = Character.Skills[DBConstants.ScientificNetworkingSkillID].LastConfirmedLvl;

            // Calculate active manufacturing & researching jobs count (character & corporation issued separately)
            m_activeManufJobsIssuedForCharacterCount = activeManufJobsIssuedForCharacter.Count();
            m_activeManufJobsIssuedForCorporationCount = activeManufJobsIssuedForCorporation.Count();
            m_activeResearchJobsIssuedForCharacterCount = activeResearchJobsIssuedForCharacter.Count();
            m_activeResearchJobsIssuedForCorporationCount = activeResearchJobsIssuedForCorporation.Count();
        }

        # endregion


        #region Initialize Expandable Panel Controls

        // Basic labels constructor
        private readonly Label m_lblActiveManufacturingJobs = new Label();
        private readonly Label m_lblActiveResearchingJobs = new Label();
        private readonly Label m_lblRemoteManufacturingRange = new Label();
        private readonly Label m_lblRemoteResearchingRange = new Label();

        // Supplemental labels constructor
        private readonly Label m_lblActiveCharManufacturingJobs = new Label();
        private readonly Label m_lblActiveCorpManufacturingJobs = new Label();
        private readonly Label m_lblActiveCharResearchingJobs = new Label();
        private readonly Label m_lblActiveCorpResearchingJobs = new Label();

        private void InitializeExpandablePanelControls()
        {
            industryExpPanelControl.SuspendLayout();

            // Add basic labels to panel
            industryExpPanelControl.Controls.AddRange(new[]
                                                          {
                                                              m_lblActiveManufacturingJobs,
                                                              m_lblActiveResearchingJobs,
                                                              m_lblRemoteManufacturingRange,
                                                              m_lblRemoteResearchingRange
                                                          });

            // Add supplemental labels to panel
            industryExpPanelControl.Controls.AddRange(new[]
                                                          {
                                                              m_lblActiveCharManufacturingJobs,
                                                              m_lblActiveCorpManufacturingJobs,
                                                              m_lblActiveCharResearchingJobs,
                                                              m_lblActiveCorpResearchingJobs
                                                          });

            // Apply properties
            foreach (Label label in industryExpPanelControl.Controls.OfType<Label>())
            {
                label.ForeColor = SystemColors.ControlText;
                label.BackColor = Color.Transparent;
                label.AutoSize = true;
            }

            // Special properties
            m_lblRemoteManufacturingRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            m_lblRemoteResearchingRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            m_lblRemoteManufacturingRange.Location = new Point(170, 0);
            m_lblRemoteResearchingRange.Location = new Point(170, 0);

            // Subscribe events
            foreach (Label label in industryExpPanelControl.Controls.OfType<Label>())
            {
                label.MouseClick += industryExpPanelControl.expandablePanelControl_MouseClick;
            }

            industryExpPanelControl.ResumeLayout();
        }

        #endregion
    }
}