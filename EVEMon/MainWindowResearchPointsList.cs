using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public partial class MainWindowResearchPointsList : UserControl
    {
        #region Fields

        private readonly List<ResearchColumnSettings> m_columns = new List<ResearchColumnSettings>();
        private readonly List<ResearchPoint> m_list = new List<ResearchPoint>();

        private ResearchColumn m_sortCriteria;

        private string m_textFilter = String.Empty;
        private bool m_sortAscending = true;
        private bool m_columnsChanged;
        private bool m_isUpdatingColumns;
        private bool m_init;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowResearchPointsList()
        {
            InitializeComponent();

            lvResearchPoints.Visible = false;
            lvResearchPoints.AllowColumnReorder = true;
            lvResearchPoints.Columns.Clear();

            noResearchLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            ListViewHelper.EnableDoubleBuffer(lvResearchPoints);

            lvResearchPoints.ColumnClick += lvResearchPoints_ColumnClick;
            lvResearchPoints.ColumnWidthChanged += lvResearchPoints_ColumnWidthChanged;
            lvResearchPoints.ColumnReordered += lvResearchPoints_ColumnReordered;

            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.CharacterResearchPointsUpdated += EveMonClient_CharacterResearchPointsUpdated;
            Disposed += OnDisposed;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        public Character Character { get; set; }

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
        /// Gets or sets the enumeration of research points to display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<ResearchPoint> ResearchPoints
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
        public IEnumerable<IColumnSettings> Columns
        {
            get
            {
                // Add the visible columns; matching the display order
                List<ResearchColumnSettings> newColumns = new List<ResearchColumnSettings>();
                foreach (ColumnHeader header in lvResearchPoints.Columns.Cast<ColumnHeader>().OrderBy(x => x.DisplayIndex))
                {
                    ResearchColumnSettings columnSetting = m_columns.First(x => x.Column == (ResearchColumn)header.Tag);
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
                    m_columns.AddRange(value.Cast<ResearchColumnSettings>());

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
            EveMonClient.CharacterResearchPointsUpdated -= EveMonClient_CharacterResearchPointsUpdated;
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
            ResearchPoints = (ccpCharacter == null ? null : ccpCharacter.ResearchPoints);
            Columns = Settings.UI.MainWindow.Research.Columns;

            UpdateColumns();

            m_init = true;

            UpdateContent();
        }

        # endregion


        #region Update Methods

        /// <summary>
        /// Updates the columns.
        /// </summary>
        public void UpdateColumns()
        {
            lvResearchPoints.BeginUpdate();
            m_isUpdatingColumns = true;

            try
            {
                lvResearchPoints.Columns.Clear();

                foreach (ResearchColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    ColumnHeader header = lvResearchPoints.Columns.Add(column.Column.GetHeader(), column.Width);
                    header.Tag = column.Column;

                    switch (column.Column)
                    {
                        case ResearchColumn.CurrentRP:
                        case ResearchColumn.PointsPerDay:
                            header.TextAlign = HorizontalAlignment.Right;
                            break;
                        case ResearchColumn.Level:
                            header.TextAlign = HorizontalAlignment.Center;
                            break;
                    }
                }

                // We update the content
                UpdateContent();

                // Adjust the size of the columns
                AdjustColumns();
            }
            finally
            {
                lvResearchPoints.EndUpdate();
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
            int selectedItem = (lvResearchPoints.SelectedItems.Count > 0
                                    ? lvResearchPoints.SelectedItems[0].Tag.GetHashCode()
                                    : 0);

            lvResearchPoints.BeginUpdate();
            try
            {
                string text = m_textFilter.ToLowerInvariant();
                IEnumerable<ResearchPoint> researhPoints = m_list.Where(x => IsTextMatching(x, text));

                UpdateSort();

                lvResearchPoints.Items.Clear();

                // Add the items in every group
                foreach (ResearchPoint researchPoint in researhPoints)
                {
                    if (String.IsNullOrEmpty(researchPoint.AgentName) || String.IsNullOrEmpty(researchPoint.Field))
                        continue;

                    ListViewItem item = new ListViewItem(researchPoint.AgentName)
                                            { UseItemStyleForSubItems = false, Tag = researchPoint };

                    // Add enough subitems to match the number of columns
                    while (item.SubItems.Count < lvResearchPoints.Columns.Count + 1)
                    {
                        item.SubItems.Add(String.Empty);
                    }

                    // Creates the subitems
                    for (int i = 0; i < lvResearchPoints.Columns.Count; i++)
                    {
                        ColumnHeader header = lvResearchPoints.Columns[i];
                        ResearchColumn column = (ResearchColumn)header.Tag;
                        SetColumn(researchPoint, item.SubItems[i], column);
                    }

                    lvResearchPoints.Items.Add(item);
                }

                // Restore the selected item (if any)
                if (selectedItem > 0)
                {
                    foreach (ListViewItem lvItem in lvResearchPoints.Items.Cast<ListViewItem>().Where(
                        lvItem => lvItem.Tag.GetHashCode() == selectedItem))
                    {
                        lvItem.Selected = true;
                    }
                }

                // Display or hide the "no research points" label
                if (m_init)
                {
                    noResearchLabel.Visible = researhPoints.IsEmpty();
                    lvResearchPoints.Visible = !researhPoints.IsEmpty();
                }
            }
            finally
            {
                lvResearchPoints.EndUpdate();
            }
        }

        /// <summary>
        /// Adjusts the columns.
        /// </summary>
        private void AdjustColumns()
        {
            foreach (ColumnHeader column in lvResearchPoints.Columns.Cast<ColumnHeader>())
            {
                if (m_columns[column.Index].Width == -1)
                    m_columns[column.Index].Width = -2;

                column.Width = m_columns[column.Index].Width;

                // Due to .NET design we need to prevent the last colummn to resize to the right end

                // Return if it's not the last column and not set to auto-resize
                if (column.Index != lvResearchPoints.Columns.Count - 1 || m_columns[column.Index].Width != -2)
                    continue;

                const int Pad = 4;

                // Calculate column header text width with padding
                int columnHeaderWidth = TextRenderer.MeasureText(column.Text, Font).Width + Pad * 2;

                // If there is an image assigned to the header, add its width with padding
                if (ilIcons.ImageSize.Width > 0)
                    columnHeaderWidth += ilIcons.ImageSize.Width + Pad;

                // Calculate the width of the header and the items of the column
                int columnMaxWidth = lvResearchPoints.Columns[column.Index].ListView.Items.Cast<ListViewItem>().Select(
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
            lvResearchPoints.ListViewItemSorter = new ListViewItemComparerByTag<ResearchPoint>(
                new ResearchPointComparer(m_sortCriteria, m_sortAscending));

            UpdateSortVisualFeedback();
        }

        /// <summary>
        /// Updates the sort feedback (the arrow on the header).
        /// </summary>
        private void UpdateSortVisualFeedback()
        {
            for (int i = 0; i < lvResearchPoints.Columns.Count; i++)
            {
                ResearchColumn column = (ResearchColumn)lvResearchPoints.Columns[i].Tag;
                if (m_sortCriteria == column)
                    lvResearchPoints.Columns[i].ImageIndex = (m_sortAscending ? 0 : 1);
                else
                    lvResearchPoints.Columns[i].ImageIndex = 2;
            }
        }

        /// <summary>
        /// Updates the listview sub-item.
        /// </summary>
        /// <param name="researchPoint"></param>
        /// <param name="item"></param>
        /// <param name="column"></param>
        private static void SetColumn(ResearchPoint researchPoint, ListViewItem.ListViewSubItem item, ResearchColumn column)
        {
            switch (column)
            {
                case ResearchColumn.Agent:
                    item.Text = researchPoint.AgentName;
                    break;
                case ResearchColumn.Level:
                    item.Text = researchPoint.AgentLevel.ToString();
                    break;
                case ResearchColumn.Field:
                    item.Text = researchPoint.Field;
                    break;
                case ResearchColumn.CurrentRP:
                    item.Text = researchPoint.CurrentRP.ToString("N2");
                    break;
                case ResearchColumn.PointsPerDay:
                    item.Text = researchPoint.PointsPerDay.ToString("N2");
                    break;
                case ResearchColumn.StartDate:
                    item.Text = researchPoint.StartDate.ToLocalTime().ToString();
                    break;
                case ResearchColumn.Location:
                    item.Text = researchPoint.Station.FullLocation;
                    break;
                case ResearchColumn.Region:
                    item.Text = researchPoint.Station.SolarSystem.Constellation.Region.Name;
                    break;
                case ResearchColumn.SolarSystem:
                    item.Text = researchPoint.Station.SolarSystem.Name;
                    break;
                case ResearchColumn.Station:
                    item.Text = researchPoint.Station.Name;
                    break;
                case ResearchColumn.Quality:
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
        private static bool IsTextMatching(ResearchPoint x, string text)
        {
            return String.IsNullOrEmpty(text)
                   || x.AgentName.ToLowerInvariant().Contains(text)
                   || x.Field.ToLowerInvariant().Contains(text)
                   || x.Station.Name.ToLowerInvariant().Contains(text)
                   || x.Station.SolarSystem.Name.ToLowerInvariant().Contains(text)
                   || x.Station.SolarSystem.Constellation.Name.ToLowerInvariant().Contains(text)
                   || x.Station.SolarSystem.Constellation.Region.Name.ToLowerInvariant().Contains(text);
        }

        #endregion


        #region Local Event Handlers

        /// <summary>
        /// On column reorder we update the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvResearchPoints_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvResearchPoints_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns || m_columns.Count <= e.ColumnIndex)
                return;

            m_columns[e.ColumnIndex].Width = lvResearchPoints.Columns[e.ColumnIndex].Width;
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user clicks a column header, we update the sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvResearchPoints_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ResearchColumn column = (ResearchColumn)lvResearchPoints.Columns[e.Column].Tag;
            if (m_sortCriteria == column)
                m_sortAscending = !m_sortAscending;
            else
            {
                m_sortCriteria = column;
                m_sortAscending = true;
            }

            UpdateContent();
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
            if (m_columnsChanged)
            {
                Settings.UI.MainWindow.Research.Add(Columns.Cast<ResearchColumnSettings>().ToList());

                // Recreate the columns
                Columns = Settings.UI.MainWindow.Research.Columns;
            }

            m_columnsChanged = false;
        }

        /// <summary>
        /// When the research points change update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterResearchPointsUpdated(object sender, CharacterChangedEventArgs e)
        {
            CCPCharacter ccpCharacter = Character as CCPCharacter;
            if (ccpCharacter == null || e.Character != ccpCharacter)
                return;

            ResearchPoints = ccpCharacter.ResearchPoints;
            UpdateColumns();
        }

        # endregion
    }
}