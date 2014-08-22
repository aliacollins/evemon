using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.CharacterMonitoring
{
    public partial class CharacterPlanetaryColoniesList : UserControl, IListView
    {
        #region Fields

        private readonly List<PlanetaryColumnSettings> m_columns = new List<PlanetaryColumnSettings>();
        private readonly List<PlanetaryColony> m_list = new List<PlanetaryColony>();

        private InfiniteDisplayToolTip m_tooltip;
        private Timer m_refreshTimer;
        private PlanetaryColoniesGrouping m_grouping;
        private PlanetaryColoniesColumn m_sortCriteria;

        private string m_textFilter = String.Empty;
        private bool m_sortAscending = true;
        private bool m_columnsChanged;
        private bool m_isUpdatingColumns;
        private bool m_init;

        private int m_columnTTCDisplayIndex;

        #endregion


        #region Constructor

        public CharacterPlanetaryColoniesList()
        {
            InitializeComponent();

            lvPlanetaryColonies.Visible = false;
            lvPlanetaryColonies.AllowColumnReorder = true;
            lvPlanetaryColonies.Columns.Clear();

            noPlanetaryColoniesLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            ListViewHelper.EnableDoubleBuffer(lvPlanetaryColonies);

            lvPlanetaryColonies.ColumnClick += lvPlanetaryColonies_ColumnClick;
            lvPlanetaryColonies.ColumnWidthChanged += lvPlanetaryColonies_ColumnWidthChanged;
            lvPlanetaryColonies.ColumnReordered += lvPlanetaryColonies_ColumnReordered;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        internal CCPCharacter Character { get; set; }

        /// <summary>
        /// Gets or sets the text filter.
        /// </summary>
        [Browsable(false)]
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
        /// Gets or sets the enumeration of planetary colonies to display.
        /// </summary>
        private IEnumerable<PlanetaryColony> PlanetaryColonies
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
        /// Gets or sets the grouping mode.
        /// </summary>
        [Browsable(false)]
        public Enum Grouping
        {
            get { return m_grouping; }
            set
            {
                m_grouping = (PlanetaryColoniesGrouping)value;
                if (m_init)
                    UpdateColumns();
            }
        }

        /// <summary>
        /// Gets or sets the settings used for columns.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<IColumnSettings> Columns
        {
            get
            {
                // Add the visible columns; matching the display order
                List<PlanetaryColumnSettings> newColumns = new List<PlanetaryColumnSettings>();
                foreach (ColumnHeader header in lvPlanetaryColonies.Columns.Cast<ColumnHeader>().OrderBy(x => x.DisplayIndex))
                {
                    PlanetaryColumnSettings columnSetting = m_columns.First(x => x.Column == (PlanetaryColoniesColumn)header.Tag);
                    if (columnSetting.Width > -1)
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
                    m_columns.AddRange(value.Cast<PlanetaryColumnSettings>());

                if (m_init)
                    UpdateColumns();
            }
        }

        #endregion


        # region Inherited Events

        /// <summary>
        /// On load subscribe the events.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            m_tooltip = new InfiniteDisplayToolTip(lvPlanetaryColonies);
            m_refreshTimer = new Timer();

            m_refreshTimer.Tick += refresh_TimerTick;
            m_refreshTimer.Interval = 1000;

            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.CharacterPlanetaryColoniesUpdated += EveMonClient_CharacterPlanetaryColoniesUpdated;
            EveMonClient.CharacterPlanetaryPinsUpdated += EveMonClient_CharacterPlanetaryPinsUpdated;
            EveMonClient.CharacterPlanetaryRoutesUpdated += EveMonClient_CharacterPlanetaryRoutesUpdated;
            EveMonClient.CharacterPlanetaryLinksUpdated += EveMonClient_CharacterPlanetaryLinksUpdated;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            m_tooltip.Dispose();
            m_refreshTimer.Dispose();

            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.CharacterPlanetaryColoniesUpdated -= EveMonClient_CharacterPlanetaryColoniesUpdated;
            EveMonClient.CharacterPlanetaryPinsUpdated -= EveMonClient_CharacterPlanetaryPinsUpdated;
            EveMonClient.CharacterPlanetaryRoutesUpdated -= EveMonClient_CharacterPlanetaryRoutesUpdated;
            EveMonClient.CharacterPlanetaryLinksUpdated -= EveMonClient_CharacterPlanetaryLinksUpdated;
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

            lvPlanetaryColonies.Visible = false;

            PlanetaryColonies = (Character == null ? null : Character.PlanetaryColonies);
            Columns = Settings.UI.MainWindow.PlanetaryColonies.Columns;
            Grouping = (Character == null ? PlanetaryColoniesGrouping.None : Character.UISettings.PlanetaryGroupBy);
            TextFilter = String.Empty;

            UpdateColumns();

            m_init = true;

            UpdateListVisibility();
        }

        # endregion


        #region Update Methods

        /// <summary>
        /// Autoresizes the columns.
        /// </summary>
        public void AutoResizeColumns()
        {
            m_columns.ForEach(column =>
            {
                if (column.Visible)
                    column.Width = -2;
            });

            UpdateColumns();
        }

        /// <summary>
        /// Updates the columns.
        /// </summary>
        private void UpdateColumns()
        {
            // Returns if not visible
            if (!Visible)
                return;

            lvPlanetaryColonies.BeginUpdate();
            m_isUpdatingColumns = true;

            try
            {
                lvPlanetaryColonies.Columns.Clear();

                foreach (PlanetaryColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    ColumnHeader header = lvPlanetaryColonies.Columns.Add(column.Column.GetHeader(), column.Width);
                    header.Tag = column.Column;

                    switch (column.Column)
                    {
                        case PlanetaryColoniesColumn.Installations:
                        case PlanetaryColoniesColumn.UpgradeLevel:
                            header.TextAlign = HorizontalAlignment.Center;
                            break;
                    }
                }

                // We update the content
                UpdateContent();
            }
            finally
            {
                lvPlanetaryColonies.EndUpdate();
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

            int scrollBarPosition = lvPlanetaryColonies.GetVerticalScrollBarPosition();

            // Store the selected item (if any) to restore it after the update
            int selectedItem = (lvPlanetaryColonies.SelectedItems.Count > 0
                                    ? lvPlanetaryColonies.SelectedItems[0].Tag.GetHashCode()
                                    : 0);

            lvPlanetaryColonies.BeginUpdate();
            try
            {
                string text = m_textFilter.ToLowerInvariant();
                IEnumerable<PlanetaryColony> colonies = m_list.Where(x => IsTextMatching(x, text));

                UpdateSort();

                UpdateContentByGroup(colonies);

                // Restore the selected item (if any)
                if (selectedItem > 0)
                {
                    foreach (ListViewItem lvItem in lvPlanetaryColonies.Items.Cast<ListViewItem>().Where(
                        lvItem => lvItem.Tag.GetHashCode() == selectedItem))
                    {
                        lvItem.Selected = true;
                    }
                }

                // Adjust the size of the columns
                AdjustColumns();

                UpdateListVisibility();
            }
            finally
            {
                lvPlanetaryColonies.EndUpdate();
                lvPlanetaryColonies.SetVerticalScrollBarPosition(scrollBarPosition);
            }
        }

        /// <summary>
        /// Updates the content by group.
        /// </summary>
        /// <param name="colonies">The colonies.</param>
        private void UpdateContentByGroup(IEnumerable<PlanetaryColony> colonies)
        {
            switch (m_grouping)
            {
                case PlanetaryColoniesGrouping.None:
                    UpdateNoGroupContent(colonies);
                    break;
                case PlanetaryColoniesGrouping.SolarSystem:
                    IOrderedEnumerable<IGrouping<string, PlanetaryColony>> groups0 =
                        colonies.GroupBy(x => x.SolarSystem.Name).OrderBy(x => x.Key);
                    UpdateContent(groups0);
                    break;
                case PlanetaryColoniesGrouping.SolarSystemDesc:
                    IOrderedEnumerable<IGrouping<string, PlanetaryColony>> groups1 =
                        colonies.GroupBy(x => x.SolarSystem.Name).OrderByDescending(x => x.Key);
                    UpdateContent(groups1);
                    break;
                case PlanetaryColoniesGrouping.PlanetType:
                    IOrderedEnumerable<IGrouping<string, PlanetaryColony>> groups2 =
                        colonies.GroupBy(x => x.PlanetTypeName).OrderBy(x => x.Key);
                    UpdateContent(groups2);
                    break;
                case PlanetaryColoniesGrouping.PlanetTypeDesc:
                    IOrderedEnumerable<IGrouping<string, PlanetaryColony>> groups3 =
                        colonies.GroupBy(x => x.PlanetTypeName).OrderByDescending(x => x.Key);
                    UpdateContent(groups3);
                    break;
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        private void UpdateNoGroupContent(IEnumerable<PlanetaryColony> colonies)
        {
            lvPlanetaryColonies.Items.Clear();
            lvPlanetaryColonies.Groups.Clear();

            // Add the items
            lvPlanetaryColonies.Items.AddRange(colonies.Select(
                colony => new
                {
                    colony,
                    item = new ListViewItem(colony.PlanetName)
                    {
                        UseItemStyleForSubItems = false,
                        Tag = colony
                    }
                }).Select(x => CreateSubItems(x.colony, x.item)).ToArray());
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groups"></param>
        private void UpdateContent<TKey>(IEnumerable<IGrouping<TKey, PlanetaryColony>> groups)
        {
            lvPlanetaryColonies.Items.Clear();
            lvPlanetaryColonies.Groups.Clear();

            // Add the groups
            foreach (IGrouping<TKey, PlanetaryColony> group in groups)
            {
                string groupText;
                if (group.Key is DateTime)
                    groupText = ((DateTime)(Object)group.Key).ToShortDateString();
                else
                    groupText = group.Key.ToString();

                ListViewGroup listGroup = new ListViewGroup(groupText);
                lvPlanetaryColonies.Groups.Add(listGroup);

                // Add the items in every group
                lvPlanetaryColonies.Items.AddRange(
                    group.Select(colony => new
                    {
                        colony,
                        item = new ListViewItem(colony.PlanetName, listGroup)
                        {
                            UseItemStyleForSubItems = false,
                            Tag = colony
                        }

                    }).Select(x => CreateSubItems(x.colony, x.item)).ToArray());
            }
        }

        /// <summary>
        /// Creates the list view sub items.
        /// </summary>
        /// <param name="colony">The colony.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private ListViewItem CreateSubItems(PlanetaryColony colony, ListViewItem item)
        {
            // Add enough subitems to match the number of columns
            while (item.SubItems.Count < lvPlanetaryColonies.Columns.Count + 1)
            {
                item.SubItems.Add(String.Empty);
            }

            // Creates the subitems
            for (int i = 0; i < lvPlanetaryColonies.Columns.Count; i++)
            {
                SetColumn(colony, item.SubItems[i], (PlanetaryColoniesColumn)lvPlanetaryColonies.Columns[i].Tag);
            }

            return item;
        }

        /// <summary>
        /// Updates the list visibility.
        /// </summary>
        private void UpdateListVisibility()
        {
            // Display or hide the "no planetary colonies" label
            if (!m_init)
                return;

            noPlanetaryColoniesLabel.Visible = lvPlanetaryColonies.Items.Count == 0;
            lvPlanetaryColonies.Visible = !noPlanetaryColoniesLabel.Visible;
            m_refreshTimer.Enabled = lvPlanetaryColonies.Visible;
        }

        /// <summary>
        /// Adjusts the columns.
        /// </summary>
        private void AdjustColumns()
        {
            foreach (ColumnHeader column in lvPlanetaryColonies.Columns)
            {
                if (m_columns[column.Index].Width == -1)
                    m_columns[column.Index].Width = -2;

                column.Width = m_columns[column.Index].Width;

                // Due to .NET design we need to prevent the last colummn to resize to the right end

                // Return if it's not the last column and not set to auto-resize
                if (column.Index != lvPlanetaryColonies.Columns.Count - 1 || m_columns[column.Index].Width != -2)
                    continue;

                const int Pad = 4;

                // Calculate column header text width with padding
                int columnHeaderWidth = TextRenderer.MeasureText(column.Text, Font).Width + Pad * 2;

                // If there is an image assigned to the header, add its width with padding
                if (lvPlanetaryColonies.SmallImageList != null && column.ImageIndex > -1)
                    columnHeaderWidth += lvPlanetaryColonies.SmallImageList.ImageSize.Width + Pad;

                // Calculate the width of the header and the items of the column
                int columnMaxWidth = column.ListView.Items.Cast<ListViewItem>().Select(
                    item => TextRenderer.MeasureText(item.SubItems[column.Index].Text, Font).Width).Concat(
                        new[] { columnHeaderWidth }).Max() + Pad + 1;

                // Assign the width found
                column.Width = columnMaxWidth;
            }
        }

        /// <summary>
        /// Updates the item sorter.
        /// </summary>
        private void UpdateSort()
        {
            lvPlanetaryColonies.ListViewItemSorter = new ListViewItemComparerByTag<PlanetaryColony>(
                new PlanetaryColonyComparer(m_sortCriteria, m_sortAscending));

            UpdateSortVisualFeedback();
        }

        /// <summary>
        /// Updates the sort feedback (the arrow on the header).
        /// </summary>
        private void UpdateSortVisualFeedback()
        {
            foreach (ColumnHeader columnHeader in lvPlanetaryColonies.Columns.Cast<ColumnHeader>())
            {
                PlanetaryColoniesColumn column = (PlanetaryColoniesColumn)columnHeader.Tag;
                if (m_sortCriteria == column)
                    columnHeader.ImageIndex = (m_sortAscending ? 0 : 1);
                else
                    columnHeader.ImageIndex = 2;
            }
        }

        /// <summary>
        /// Updates the listview sub-item.
        /// </summary>
        /// <param name="colony">The colony.</param>
        /// <param name="item">The item.</param>
        /// <param name="column">The column.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void SetColumn(PlanetaryColony colony, ListViewItem.ListViewSubItem item, PlanetaryColoniesColumn column)
        {
            switch (column)
            {
                case PlanetaryColoniesColumn.PlanetName:
                    item.Text = colony.PlanetName;
                    break;
                case PlanetaryColoniesColumn.PlanetTypeName:
                    item.Text = colony.PlanetTypeName;
                    break;
                case PlanetaryColoniesColumn.SolarSystem:
                    item.Text = colony.SolarSystem.Name;
                    item.ForeColor = colony.SolarSystem.SecurityLevelColor;
                    break;
                case PlanetaryColoniesColumn.Installations:
                    item.Text = colony.NumberOfPins.ToString();
                    break;
                case PlanetaryColoniesColumn.UpgradeLevel:
                    item.Text = colony.UpgradeLevel.ToString();
                    break;
                case PlanetaryColoniesColumn.Location:
                    item.Text = colony.FullLocation;
                    break;
                case PlanetaryColoniesColumn.Region:
                    item.Text = colony.SolarSystem.Constellation.Region.Name;
                    break;
                case PlanetaryColoniesColumn.LastUpdate:
                    item.Text = colony.LastUpdate.ToLocalTime().ToString();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Checks the given text matches the item.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if [is text matching] [the specified x]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsTextMatching(PlanetaryColony x, string text)
        {
            return String.IsNullOrEmpty(text);
        }
        /// <summary>
        /// Updates the time to completion.
        /// </summary>
        private void UpdateTimeToCompletion()
        {
            //const int Pad = 4;
            //int columnTTCIndex = m_columns.IndexOf(m_columns.FirstOrDefault(x => x.Column == PlanetaryColoniesColumn.TTC));

            //foreach (ListViewItem listViewItem in lvPlanetaryColonies.Items.Cast<ListViewItem>())
            //{
            //    IndustryJob job = (IndustryJob)listViewItem.Tag;
            //    if (!job.IsActive || job.ActiveJobState == ActiveJobState.Ready)
            //        continue;

            //    // Update the time to completion
            //    if (columnTTCIndex != -1 && m_columns[columnTTCIndex].Visible)
            //    {
            //        if (m_columnTTCDisplayIndex == -1)
            //            m_columnTTCDisplayIndex = lvPlanetaryColonies.Columns[columnTTCIndex].DisplayIndex;

            //        listViewItem.SubItems[m_columnTTCDisplayIndex].Text = job.TTC;

            //        // Using AutoResizeColumn when TTC is the first column
            //        // results to a nasty visual bug due to ListViewItem.ImageIndex placeholder
            //        if (m_columnTTCDisplayIndex == 0)
            //        {
            //            // Calculate column header text width with padding
            //            int columnHeaderWidth =
            //                TextRenderer.MeasureText(lvPlanetaryColonies.Columns[m_columnTTCDisplayIndex].Text, Font).Width + Pad * 2;

            //            // If there is an image assigned to the header, add its width with padding
            //            if (ilIcons.ImageSize.Width > 0)
            //                columnHeaderWidth += ilIcons.ImageSize.Width + Pad;

            //            int columnWidth = (lvPlanetaryColonies.Items.Cast<ListViewItem>().Select(
            //                item => TextRenderer.MeasureText(item.SubItems[m_columnTTCDisplayIndex].Text, Font).Width)).Concat(
            //                    new[] { columnHeaderWidth }).Max() + Pad + 2;
            //            lvPlanetaryColonies.Columns[m_columnTTCDisplayIndex].Width = columnWidth;
            //        }
            //        else
            //            lvPlanetaryColonies.AutoResizeColumn(m_columnTTCDisplayIndex, ColumnHeaderAutoResizeStyle.HeaderSize);
            //    }

            //    // Job was pending and its time to start
            //    if (job.ActiveJobState == ActiveJobState.Pending && job.StartDate < DateTime.UtcNow)
            //    {
            //        job.ActiveJobState = ActiveJobState.InProgress;
            //        UpdateContent();
            //    }

            //    if (job.TTC.Length != 0)
            //        continue;

            //    // Job is ready
            //    job.ActiveJobState = ActiveJobState.Ready;
            //    UpdateContent();
            //}
        }

        #endregion


        #region Local Event Handlers

        /// <summary>
        /// Exports item info to CSV format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewExporter.CreateCSV(lvPlanetaryColonies);
        }

        /// <summary>
        /// Handles the Tick event of the m_timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void refresh_TimerTick(object sender, EventArgs e)
        {
            UpdateTimeToCompletion();
        }

        /// <summary>
        /// On column reorder we update the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvPlanetaryColonies_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvPlanetaryColonies_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns || m_columns.Count <= e.ColumnIndex)
                return;

            if (m_columns[e.ColumnIndex].Width == lvPlanetaryColonies.Columns[e.ColumnIndex].Width)
                return;

            m_columns[e.ColumnIndex].Width = lvPlanetaryColonies.Columns[e.ColumnIndex].Width;
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user clicks a column header, we update the sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvPlanetaryColonies_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            PlanetaryColoniesColumn column = (PlanetaryColoniesColumn)lvPlanetaryColonies.Columns[e.Column].Tag;
            if (m_sortCriteria == column)
                m_sortAscending = !m_sortAscending;
            else
            {
                m_sortCriteria = column;
                m_sortAscending = true;
            }

            m_isUpdatingColumns = true;

            // Updates the item sorter
            UpdateSort();

            m_isUpdatingColumns = false;
        }

        # endregion


        #region Global Events

        /// <summary>
        /// On timer tick, we update the column settings if any changes have been made to them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (!Visible || !m_columnsChanged)
                return;

            Settings.UI.MainWindow.PlanetaryColonies.Columns.Clear();
            Settings.UI.MainWindow.PlanetaryColonies.Columns.AddRange(Columns.Cast<PlanetaryColumnSettings>());

            // Recreate the columns
            Columns = Settings.UI.MainWindow.PlanetaryColonies.Columns;
            m_columnsChanged = false;
        }

        /// <summary>
        /// When the planetary colonies change update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterPlanetaryColoniesUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (Character == null || e.Character != Character)
                return;

            PlanetaryColonies = Character.PlanetaryColonies;
            UpdateColumns();
        }

        /// <summary>
        /// When the planetary pins change update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterPlanetaryPinsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (Character == null || e.Character != Character)
                return;

            UpdateColumns();
        }

        /// <summary>
        /// When the planetary routes change update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterPlanetaryRoutesUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (Character == null || e.Character != Character)
                return;

            UpdateColumns();
        }

        /// <summary>
        /// When the planetary links change update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterPlanetaryLinksUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (Character == null || e.Character != Character)
                return;

            UpdateColumns();
        }

        # endregion
    }
}
