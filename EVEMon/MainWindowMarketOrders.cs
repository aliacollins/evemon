using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.SettingsObjects;
using EVEMon.Controls;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon
{
    /// <summary>
    /// Displays a list of market orders.
    /// </summary>
    public partial class MainWindowMarketOrdersList : UserControl
    {
        private List<MarketOrderColumnSettings> m_columns = new List<MarketOrderColumnSettings>();
        private readonly List<MarketOrder> m_list = new List<MarketOrder>();

        private MarketOrderGrouping m_grouping;
        private MarketOrderColumn m_sortCriteria;
        private Character m_character;

        private string m_textFilter = String.Empty;
        private bool m_sortAscending = true;

        private bool m_hideInactive;
        private bool m_numberFormat;
        private bool m_updatePending;
        private bool m_isUpdatingColumns;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowMarketOrdersList()
        {
            InitializeComponent();

            listView.Visible = false;
            listView.ShowItemToolTips = true;
            listView.AllowColumnReorder = true;
            listView.Columns.Clear();
            noOrdersLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            noOrdersLabel.Visible = true;

            listView.ColumnClick += new ColumnClickEventHandler(listView_ColumnClick);
            listView.KeyDown += new KeyEventHandler(listView_KeyDown);
            listView.ColumnWidthChanged += new ColumnWidthChangedEventHandler(listView_ColumnWidthChanged);

            EveClient.CharacterMarketOrdersChanged += new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterMarketOrdersChanged);
            this.Disposed += new EventHandler(OnDisposed);
        }
        
        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveClient.CharacterMarketOrdersChanged -= new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterMarketOrdersChanged);
            this.Disposed -= new EventHandler(OnDisposed);
        }
        

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        public Character Character
        {
            get { return m_character; }
            set { m_character = value; }
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
                UpdateContent();
            }
        }

        /// <summary>
        /// Gets or sets the grouping mode.
        /// </summary>
        public MarketOrderGrouping Grouping
        {
            get { return m_grouping; }
            set
            {
                m_grouping = value;
                UpdateContent();
            }
        }

        /// <summary>
        /// Gets or sets the enumeration of orders to display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<MarketOrder> Orders
        {
            get
            {
                foreach (var order in m_list)
                {
                    yield return order;
                }
            }
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
        public IEnumerable<MarketOrderColumnSettings> Columns
        {
            get 
            { 
                // Add the visible columns; matching the display order
                var newColumns = new List<MarketOrderColumnSettings>();
                foreach (var header in listView.Columns.Cast<ColumnHeader>().OrderBy(x => x.DisplayIndex))
                {
                    var columnSetting = m_columns.First(x => x.Column == (MarketOrderColumn)header.Tag);
                    if (columnSetting.Width != -1)
                        columnSetting.Width = header.Width;
                    newColumns.Add(columnSetting);
                }

                // Then add the other columns
                foreach (var column in m_columns.Where(x => !x.Visible))
                {
                    newColumns.Add(column);
                }

                return newColumns; 
            }
            set 
            {
                m_columns.Clear();
                if (value != null)
                    m_columns.AddRange(value.Select(x => x.Clone()));
                UpdateColumns();
            }
        }

        # region Inherited Events
        /// <summary>
        /// On load, we update the content
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateContent();
        }

        /// <summary>
        /// When the control becomes visible again, check for pending updates.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.DesignMode || this.IsDesignModeHosted())
                return;

            if (this.Visible)
            {
                var ccpCharacter = m_character as CCPCharacter;
                this.Orders = (ccpCharacter == null ? null : ccpCharacter.MarketOrders);
                this.Columns = Settings.UI.MainWindow.MarketOrders.Columns;
                this.Grouping = (m_character == null ? MarketOrderGrouping.State : m_character.UISettings.OrdersGroupBy);

                UpdateContent();
            }

            if (m_updatePending)
                UpdateContent();

            base.OnVisibleChanged(e);
        }
        # endregion


        #region Updates on global events
        /// <summary>
        /// <summary>
        /// Updates the columns.
        /// </summary>
        public void UpdateColumns()
        {
            listView.BeginUpdate();
            m_isUpdatingColumns = true;

            try
            {
                listView.Items.Clear();
                listView.Groups.Clear();
                listView.Columns.Clear();

                foreach (var column in m_columns.Where(x => x.Visible))
                {
                    var header = listView.Columns.Add(column.Column.GetHeader(), column.Width);
                    header.Tag = (object)column.Column;

                    switch (column.Column)
                    {
                        case MarketOrderColumn.Issued:
                        case MarketOrderColumn.LastStateChange:
                        case MarketOrderColumn.InitialVolume:
                        case MarketOrderColumn.RemainingVolume:                        
                        case MarketOrderColumn.TotalPrice:
                        case MarketOrderColumn.Escrow:
                        case MarketOrderColumn.UnitaryPrice:
                            header.TextAlign = HorizontalAlignment.Right;
                            break;
                        case MarketOrderColumn.Volume:
                            header.TextAlign = HorizontalAlignment.Center;
                            break;
                    }
                }

                // We update the content
                UpdateContent();

                // Force the auto-update of the columns with -1 width
                var resizeStyle = (listView.Items.Count == 0 ?
                    ColumnHeaderAutoResizeStyle.HeaderSize :
                    ColumnHeaderAutoResizeStyle.ColumnContent);

                int index = 0;
                foreach (var column in m_columns.Where(x => x.Visible))
                {
                    if (column.Width == -1)
                    {
                        listView.AutoResizeColumn(index, resizeStyle);
                    }
                    index++;
                }
            }
            finally
            {
                listView.EndUpdate();
                m_isUpdatingColumns = false;
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        public void UpdateContent()
        {
            // Returns if not visible
            if (!this.Visible)
            {
                m_updatePending = true;
                return;
            }
            m_updatePending = false;
 
            m_hideInactive = Settings.UI.MainWindow.MarketOrders.HideInactiveOrders;
            m_numberFormat = Settings.UI.MainWindow.MarketOrders.NumberAbsFormat;
            
            listView.BeginUpdate();
            try
            {
                var text = m_textFilter.ToLowerInvariant();
                var orders = m_list.Where(x => !x.Ignored && IsTextMatching(x, text));
                if (m_character != null && m_hideInactive)
                    orders = orders.Where(x => x.IsAvailable);
                noOrdersLabel.Visible = orders.IsEmpty();
                listView.Visible = !orders.IsEmpty();
                
                listView.Items.Clear();
                listView.Groups.Clear();

                UpdateSort();

                switch (m_grouping)
                {
                    case MarketOrderGrouping.State:
                        var groups0 = orders.GroupBy(x => x.State).OrderBy(x => (int)x.Key);
                        UpdateContent(groups0);
                        break;
                    case MarketOrderGrouping.StateDesc:
                        var groups1 = orders.GroupBy(x => x.State).OrderByDescending(x => (int)x.Key);
                        UpdateContent(groups1);
                        break;
                    case MarketOrderGrouping.Issued:
                        var groups2 = orders.GroupBy(x => x.Issued.Date).OrderBy(x => x.Key);
                        UpdateContent(groups2);
                        break;
                    case MarketOrderGrouping.IssuedDesc:
                        var groups3 = orders.GroupBy(x => x.Issued.Date).OrderByDescending(x => x.Key);
                        UpdateContent(groups3);
                        break;
                    case MarketOrderGrouping.ItemType:
                        var groups4 = orders.GroupBy(x => x.Item.MarketGroup).OrderBy(x => x.Key.Name);
                        UpdateContent(groups4);
                        break;
                    case MarketOrderGrouping.ItemTypeDesc:
                        var groups5 = orders.GroupBy(x => x.Item.MarketGroup).OrderByDescending(x => x.Key.Name);
                        UpdateContent(groups5);
                        break;
                    case MarketOrderGrouping.Location:
                        var groups6 = orders.GroupBy(x => x.Station).OrderBy(x => x.Key);
                        UpdateContent(groups6);
                        break;
                    case MarketOrderGrouping.LocationDesc:
                        var groups7 = orders.GroupBy(x => x.Station).OrderByDescending(x => x.Key);
                        UpdateContent(groups7);
                        break;
                    case MarketOrderGrouping.OrderType:
                        var groups8 = orders.GroupBy(x => x is BuyOrder ? "Buying Orders" : "Selling Orders").OrderBy(x => x.Key);
                        UpdateContent(groups8);
                        break;
                    case MarketOrderGrouping.OrderTypeDesc:
                        var groups9 = orders.GroupBy(x => x is BuyOrder ? "Buying Orders" : "Selling Orders").OrderByDescending(x => x.Key);
                        UpdateContent(groups9);
                        break;
                }
            }
            finally
            {
                listView.EndUpdate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groups"></param>
        private void UpdateContent<TKey>(IEnumerable<IGrouping<TKey, MarketOrder>> groups)
        {
            // Add the groups.
            foreach (var group in groups)
            {
                string groupText = String.Empty;
                if (group.Key is OrderState)
                    groupText = ((OrderState)(Object)group.Key).GetHeader().ToString();
                else if (group.Key is DateTime)
                    groupText = ((DateTime)(Object)group.Key).ToShortDateString();
                else groupText = group.Key.ToString();

                var listGroup = new ListViewGroup(groupText);
                listView.Groups.Add(listGroup);

                // Add the items in every group.
                foreach (var order in group)
                {
                    if (order.Item == null)
                        continue;

                    var item = new ListViewItem(order.Item.Name, listGroup);
                    item.UseItemStyleForSubItems = false;
                    item.Tag = order;

                    // Display text as dimmed if the order is no longer available.
                    if (!order.IsAvailable)
                    {
                        item.ForeColor = SystemColors.GrayText;
                    }

                    // Display text highlighted if the order is modified.
                    if (order.State == OrderState.Modified)
                    {
                        item.ForeColor = SystemColors.HotTrack;
                    }

                    // Add enough subitems to match the number of columns
                    while (item.SubItems.Count < listView.Columns.Count + 1)
                    {
                        item.SubItems.Add(String.Empty);
                    }

                    // Creates the subitems
                    for (int i = 0; i < listView.Columns.Count; i++)
                    {
                        var header = listView.Columns[i];
                        var column = (MarketOrderColumn)header.Tag;
                        SetColumn(order, item.SubItems[i], column);
                    }

                    // Tooltip
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Issued: ").AppendLine(order.Issued.ToLocalTime().ToString());
                    builder.AppendFormat("Duration: {0} Day{1}", order.Duration, (order.Duration > 1 ? "s" : String.Empty));
                    builder.AppendLine();
                    builder.Append("Solar System: ").AppendLine(order.Station.SolarSystem.FullLocation);
                    builder.Append("Station: ").AppendLine(order.Station.Name);
                    item.ToolTipText = builder.ToString();

                    listView.Items.Add(item);
                }
            }

        }

        /// <summary>
        /// Updates the item sorter.
        /// </summary>
        private void UpdateSort()
        {
            listView.ListViewItemSorter = new ListViewItemComparerByTag<MarketOrder>(
                new MarketOrderComparer(m_sortCriteria, m_sortAscending));
            
            UpdateSortVisualFeedback();
        }

        /// <summary>
        /// Updates the sort feedback (the arrow on the header).
        /// </summary>
        private void UpdateSortVisualFeedback()
        {
            for (int i = 0; i < listView.Columns.Count; i++)
            {
                var column = (MarketOrderColumn)listView.Columns[i].Tag;
                if (m_sortCriteria == column)
                {
                    listView.Columns[i].ImageIndex = (m_sortAscending ? 0 : 1);
                }
                else
                {
                    listView.Columns[i].ImageIndex = 2;
                }
            }
        }

        /// <summary>
        /// Updates the listview sub-item.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="item"></param>
        /// <param name="column"></param>
        private void SetColumn(MarketOrder order, ListViewItem.ListViewSubItem item, MarketOrderColumn column)
        {
            switch (column)
            {
                case MarketOrderColumn.Duration:
                    item.Text = String.Format(CultureConstants.DefaultCulture, "{0} Day{1}", order.Duration, (order.Duration > 1 ? "s" : String.Empty));
                    break;

                case MarketOrderColumn.Expiration:
                    item.Text = (order.IsAvailable ? TimeExtensions.ToRemainingTimeShortDescription(order.Expiration).ToUpper() : "Expired");
                    item.ForeColor = (order.IsAvailable ? Color.Black : Color.Red);
                    if (order.IsAvailable && order.Expiration < DateTime.UtcNow.AddDays(1))
                        item.ForeColor = Color.DarkOrange; 
                    break;

                case MarketOrderColumn.InitialVolume:
                    item.Text = ( m_numberFormat ?
                        MarketOrder.Format(order.InitialVolume, AbbreviationFormat.AbbreviationSymbols):
                        String.Format(CultureConstants.DefaultCulture, order.InitialVolume.ToString("#,##0")));
                    break;

                case MarketOrderColumn.Issued:
                    item.Text = order.Issued.ToLocalTime().ToShortDateString();
                    break;

                case MarketOrderColumn.Item:
                    item.Text = order.Item.ToString();
                    break;

                case MarketOrderColumn.ItemType:
                    item.Text = order.Item.MarketGroup.Name;
                    break;

                case MarketOrderColumn.Location:
                    item.Text = (order.Station is ConquerableStation ? (order.Station as ConquerableStation).FullLocation : order.Station.FullLocation);
                    break;

                case MarketOrderColumn.MinimumVolume:
                    item.Text = (m_numberFormat ?
                        MarketOrder.Format(order.MinVolume, AbbreviationFormat.AbbreviationSymbols) :
                        String.Format(CultureConstants.DefaultCulture, order.MinVolume.ToString("#,##0")));
                    break;

                case MarketOrderColumn.Region:
                    item.Text = order.Station.SolarSystem.Constellation.Region.Name;
                    break;

                case MarketOrderColumn.RemainingVolume:
                    item.Text = (m_numberFormat ?
                        MarketOrder.Format(order.RemainingVolume, AbbreviationFormat.AbbreviationSymbols) :
                        String.Format(CultureConstants.DefaultCulture, order.RemainingVolume.ToString("#,##0")));
                    break;

                case MarketOrderColumn.SolarSystem:
                    item.Text = order.Station.SolarSystem.Name;
                    break;

                case MarketOrderColumn.Station:
                    item.Text = (order.Station is ConquerableStation ? (order.Station as ConquerableStation).FullName : order.Station.Name);
                    break;

                case MarketOrderColumn.TotalPrice:
                    item.Text = (m_numberFormat ?
                        MarketOrder.Format(order.TotalPrice, AbbreviationFormat.AbbreviationSymbols) :
                        String.Format(CultureConstants.DefaultCulture, order.TotalPrice.ToString("#,##0.#0")));
                    item.ForeColor = (order is BuyOrder ? Color.DarkRed : Color.DarkGreen);
                    break;

                case MarketOrderColumn.UnitaryPrice:
                    item.Text = ( m_numberFormat ?
                        MarketOrder.Format(order.UnitaryPrice, AbbreviationFormat.AbbreviationSymbols) :
                        String.Format(CultureConstants.DefaultCulture, order.UnitaryPrice.ToString("#,##0.#0")));
                    item.ForeColor = (order is BuyOrder ? Color.DarkRed : Color.DarkGreen);
                    break;

                case MarketOrderColumn.Volume:
                    item.Text = String.Format(CultureConstants.DefaultCulture, "{0} / {1}",
                        (m_numberFormat ?
                        MarketOrder.Format(order.RemainingVolume, AbbreviationFormat.AbbreviationSymbols) :
                        String.Format(CultureConstants.DefaultCulture, order.RemainingVolume.ToString("#,##0"))),
                        (m_numberFormat ?
                        MarketOrder.Format(order.InitialVolume, AbbreviationFormat.AbbreviationSymbols) :
                        String.Format(CultureConstants.DefaultCulture, order.InitialVolume.ToString("#,##0"))));
                    break;

                case MarketOrderColumn.LastStateChange:
                    item.Text = order.LastStateChange.ToLocalTime().ToShortDateString();
                    break;

                case MarketOrderColumn.OrderRange:
                    if (order is BuyOrder)
                        item.Text = (order as BuyOrder).RangeDescription;
                    break;
                
                case MarketOrderColumn.Escrow:
                    if (order is BuyOrder)
                    {
                        item.Text = (m_numberFormat ?
                            MarketOrder.Format((order as BuyOrder).Escrow, AbbreviationFormat.AbbreviationSymbols) :
                            String.Format(CultureConstants.DefaultCulture, (order as BuyOrder).Escrow.ToString("#,##0.#0")));
                        item.ForeColor = Color.DarkBlue;
                    }
                    break;

                default:
                    //return;
                    throw new NotImplementedException();
            }
        }
        # endregion


        /// <summary>
        /// Checks the given text matches the item.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsTextMatching(MarketOrder x, string text)
        {
            if (String.IsNullOrEmpty(text)) return true;

            if (x.Item.Name.ToLowerInvariant().Contains(text)) return true;
            if (x.Item.Description.ToLowerInvariant().Contains(text)) return true;
            if (x.Station.Name.ToLowerInvariant().Contains(text)) return true;
            if (x.Station.SolarSystem.Name.ToLowerInvariant().Contains(text)) return true;
            if (x.Station.SolarSystem.Constellation.Name.ToLowerInvariant().Contains(text)) return true;
            if (x.Station.SolarSystem.Constellation.Region.Name.ToLowerInvariant().Contains(text)) return true;
            return false;
        }


        #region Event Handlers
        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns)
                return;
            if (m_columns.Count <= e.ColumnIndex)
                return;
            m_columns[e.ColumnIndex].Width = listView.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// When the user clicks a column header, we update the sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var column = (MarketOrderColumn)listView.Columns[e.Column].Tag;
            if (m_sortCriteria == column)
            {
                m_sortAscending = !m_sortAscending;
            }
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
        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (listView.SelectedItems.Count == 0)
                        return;

                    // Mark as ignored
                    foreach (ListViewItem item in listView.SelectedItems)
                    {
                        var order = (MarketOrder)item.Tag;
                        order.Ignored = true;
                    }

                    // Updates
                    UpdateContent();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// When the market orders are changed, update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_CharacterMarketOrdersChanged(object sender, CharacterChangedEventArgs e)
        {
            var ccpCharacter = m_character as CCPCharacter;
            if (e.Character != ccpCharacter)
                return;
            
            this.Orders = (ccpCharacter == null ? null : ccpCharacter.MarketOrders);
            UpdateContent();
        }
        # endregion
    }
}
