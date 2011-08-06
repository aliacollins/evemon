using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EVEMon.Common;
using EVEMon.Common.Data;
using EVEMon.Common.SettingsObjects;
using EVEMon.Controls;

namespace EVEMon
{
    /// <summary>
    /// Displays a list of market orders.
    /// </summary>
    public partial class MainWindowMarketOrdersList : UserControl, IGroupingListView
    {
        private List<MarketOrderColumnSettings> m_columns = new List<MarketOrderColumnSettings>();
        private readonly List<MarketOrder> m_list = new List<MarketOrder>();

        private MarketOrderGrouping m_grouping;
        private MarketOrderColumn m_sortCriteria;
        private IssuedFor m_showIssuedFor;

        private string m_textFilter = String.Empty;
        private bool m_sortAscending = true;

        private bool m_hideInactive;
        private bool m_numberFormat;
        private bool m_isUpdatingColumns;
        private bool m_columnsChanged;
        private bool m_init;

        // Panel info variables
        private int m_skillBasedOrders;
        private float m_baseBrokerFee, m_transactionTax;
        private int m_askRange, m_bidRange, m_modificationRange, m_remoteBidRange;
        private int m_activeOrdersIssuedForCharacter, m_activeOrdersIssuedForCorporation;
        private int m_activeSellOrdersIssuedForCharacterCount, m_activeSellOrdersIssuedForCorporationCount;
        private int m_activeBuyOrdersIssuedForCharacterCount, m_activeBuyOrdersIssuedForCorporationCount;
        private decimal m_sellOrdersIssuedForCharacterTotal, m_sellOrdersIssuedForCorporationTotal;
        private decimal m_buyOrdersIssuedForCharacterTotal, m_buyOrdersIssuedForCorporationTotal;
        private decimal m_issuedForCharacterTotalEscrow, m_issuedForCorporationTotalEscrow;
        private decimal m_issuedForCharacterEscrowAdditionalToCover, m_issuedForCorporationEscrowAdditionalToCover;


        # region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowMarketOrdersList()
        {
            InitializeComponent();
            InitializeExpandablePanelControls();

            lvOrders.Visible = false;
            lvOrders.ShowItemToolTips = true;
            lvOrders.AllowColumnReorder = true;
            lvOrders.Columns.Clear();

            noOrdersLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            marketExpPanelControl.Font = FontFactory.GetFont("Tahoma", 8.25f);
            marketExpPanelControl.Visible = false;

            ListViewHelper.EnableDoubleBuffer(lvOrders);

            lvOrders.ColumnClick += listView_ColumnClick;
            lvOrders.KeyDown += listView_KeyDown;
            lvOrders.ColumnWidthChanged += listView_ColumnWidthChanged;
            lvOrders.ColumnReordered += listView_ColumnReordered;

            Resize += MainWindowMarketOrdersList_Resize;

            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.CharacterMarketOrdersUpdated += EveMonClient_CharacterMarketOrdersUpdated;
            Disposed += OnDisposed;
        }

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
        /// Gets or sets the grouping mode.
        /// </summary>
        public Enum Grouping
        {
            get { return m_grouping; }
            set
            {
                m_grouping = (MarketOrderGrouping)value;
                if (m_init)
                    UpdateColumns();
            }
        }

        /// <summary>
        /// Gets or sets which "Issued for" orders to display.
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
        /// Gets true when character has active issued order for corporation.
        /// </summary>
        public bool HasActiveCorporationIssuedOrders
        {
            get 
            { 
                return m_list.Any(x=>
                (x.State == OrderState.Active || x.State == OrderState.Modified)
                && x.IssuedFor == IssuedFor.Corporation); 
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
                foreach (MarketOrder order in m_list)
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
                List<MarketOrderColumnSettings> newColumns = new List<MarketOrderColumnSettings>();
                foreach (var header in lvOrders.Columns.Cast<ColumnHeader>().OrderBy(x => x.DisplayIndex))
                {
                    MarketOrderColumnSettings columnSetting = m_columns.First(x => x.Column == (MarketOrderColumn)header.Tag);
                    if (columnSetting.Width != -1)
                        columnSetting.Width = header.Width;
                    newColumns.Add(columnSetting);
                }

                // Then add the other columns
                foreach (MarketOrderColumnSettings column in m_columns.Where(x => !x.Visible))
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
            EveMonClient.CharacterMarketOrdersUpdated -= EveMonClient_CharacterMarketOrdersUpdated;
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
            Orders = (ccpCharacter == null ? null : ccpCharacter.MarketOrders);
            Columns = Settings.UI.MainWindow.MarketOrders.Columns;
            Grouping = (Character == null ? MarketOrderGrouping.State : Character.UISettings.OrdersGroupBy);

            UpdateColumns();

            m_init = true;

            UpdateContent();
            UpdateExpPanelContent();
        }

        # endregion


        #region Updates Main Market Window On Global Events

        /// <summary>
        /// <summary>
        /// Updates the columns.
        /// </summary>
        public void UpdateColumns()
        {
            // Returns if not visible
            if (!Visible)
                return;

            lvOrders.BeginUpdate();
            m_isUpdatingColumns = true;

            try
            {
                lvOrders.Columns.Clear();

                foreach (MarketOrderColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    ColumnHeader header = lvOrders.Columns.Add(column.Column.GetHeader(), column.Width);
                    header.Tag = (object)column.Column;

                    switch (column.Column)
                    {
                        case MarketOrderColumn.Issued:
                        case MarketOrderColumn.IssuedFor:
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

                // Force the auto-resize of the columns with -1 width
                ColumnHeaderAutoResizeStyle resizeStyle = (lvOrders.Items.Count == 0 ?
                                                            ColumnHeaderAutoResizeStyle.HeaderSize :
                                                            ColumnHeaderAutoResizeStyle.ColumnContent);

                int index = 0;
                foreach (MarketOrderColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    if (column.Width == -1)
                        lvOrders.AutoResizeColumn(index, resizeStyle);

                    index++;
                }
            }
            finally
            {
                lvOrders.EndUpdate();
                m_isUpdatingColumns = false;
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        public void UpdateContent()
        {
            // Returns if not visible
            if (!Visible)
                return;

            // Store the selected item (if any) to restore it after the update
            int selectedItem = (lvOrders.SelectedItems.Count > 0 ?
                                lvOrders.SelectedItems[0].Tag.GetHashCode() : 0);

            m_hideInactive = Settings.UI.MainWindow.MarketOrders.HideInactiveOrders;
            m_numberFormat = Settings.UI.MainWindow.MarketOrders.NumberAbsFormat;
            
            lvOrders.BeginUpdate();
            try
            {
                string text = m_textFilter.ToLowerInvariant();
                IEnumerable<MarketOrder> orders = m_list.Where(x => !x.Ignored && IsTextMatching(x, text));
                if (Character != null && m_hideInactive)
                    orders = orders.Where(x => x.IsAvailable);

                switch (m_showIssuedFor)
                {
                    case IssuedFor.Character:
                        orders = orders.Where(x => x.IssuedFor == IssuedFor.Character);
                        break;
                    case IssuedFor.Corporation:
                        orders = orders.Where(x => x.IssuedFor == IssuedFor.Corporation);
                        break;
                }

                UpdateSort();

                switch (m_grouping)
                {
                    case MarketOrderGrouping.State:
                        IOrderedEnumerable<IGrouping<OrderState, MarketOrder>> groups0 = orders.GroupBy(x => x.State).OrderBy(x => (int)x.Key);
                        UpdateContent(groups0);
                        break;
                    case MarketOrderGrouping.StateDesc:
                        IOrderedEnumerable<IGrouping<OrderState, MarketOrder>> groups1 = orders.GroupBy(x => x.State).OrderByDescending(x => (int)x.Key);
                        UpdateContent(groups1);
                        break;
                    case MarketOrderGrouping.Issued:
                        IOrderedEnumerable<IGrouping<DateTime, MarketOrder>> groups2 = orders.GroupBy(x => x.Issued.Date).OrderBy(x => x.Key);
                        UpdateContent(groups2);
                        break;
                    case MarketOrderGrouping.IssuedDesc:
                        IOrderedEnumerable<IGrouping<DateTime, MarketOrder>> groups3 = orders.GroupBy(x => x.Issued.Date).OrderByDescending(x => x.Key);
                        UpdateContent(groups3);
                        break;
                    case MarketOrderGrouping.ItemType:
                        IOrderedEnumerable<IGrouping<MarketGroup, MarketOrder>> groups4 = orders.GroupBy(x => x.Item.MarketGroup).OrderBy(x => x.Key.Name);
                        UpdateContent(groups4);
                        break;
                    case MarketOrderGrouping.ItemTypeDesc:
                        IOrderedEnumerable<IGrouping<MarketGroup, MarketOrder>> groups5 = orders.GroupBy(x => x.Item.MarketGroup).OrderByDescending(x => x.Key.Name);
                        UpdateContent(groups5);
                        break;
                    case MarketOrderGrouping.Location:
                        IOrderedEnumerable<IGrouping<Station, MarketOrder>> groups6 = orders.GroupBy(x => x.Station).OrderBy(x => x.Key.Name);
                        UpdateContent(groups6);
                        break;
                    case MarketOrderGrouping.LocationDesc:
                        IOrderedEnumerable<IGrouping<Station, MarketOrder>> groups7 = orders.GroupBy(x => x.Station).OrderByDescending(x => x.Key.Name);
                        UpdateContent(groups7);
                        break;
                    case MarketOrderGrouping.OrderType:
                        IOrderedEnumerable<IGrouping<string, MarketOrder>> groups8 = orders.GroupBy(x => x is BuyOrder ? "Buying Orders" : "Selling Orders")
                                                                                            .OrderBy(x => x.Key);
                        UpdateContent(groups8);
                        break;
                    case MarketOrderGrouping.OrderTypeDesc:
                        IOrderedEnumerable<IGrouping<string, MarketOrder>> groups9 = orders.GroupBy(x => x is BuyOrder ? "Buying Orders" : "Selling Orders")
                                                                                            .OrderByDescending(x => x.Key);
                        UpdateContent(groups9);
                        break;
                }

                // Restore the selected item (if any)
                if (selectedItem > 0)
                {
                    foreach (ListViewItem lvItem in lvOrders.Items)
                    {
                        if (lvItem.Tag.GetHashCode() == selectedItem)
                            lvItem.Selected = true;
                    }
                }

                // Display or hide the "no orders" label
                if (m_init)
                {
                    noOrdersLabel.Visible = orders.IsEmpty();
                    lvOrders.Visible = !orders.IsEmpty();
                    marketExpPanelControl.Visible = true;
                    marketExpPanelControl.Header.Visible = true;
                }
            }
            finally
            {
                lvOrders.EndUpdate();
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groups"></param>
        private void UpdateContent<TKey>(IEnumerable<IGrouping<TKey, MarketOrder>> groups)
        {
            lvOrders.Items.Clear();
            lvOrders.Groups.Clear();

            // Add the groups
            foreach (IGrouping<TKey, MarketOrder> group in groups)
            {
                string groupText = String.Empty;
                if (group.Key is OrderState)
                {
                    groupText = ((OrderState)(Object)group.Key).GetHeader();
                }
                else if (group.Key is DateTime)
                {
                    groupText = ((DateTime)(Object)group.Key).ToShortDateString();
                }
                else
                {
                    groupText = group.Key.ToString();
                }

                ListViewGroup listGroup = new ListViewGroup(groupText);
                lvOrders.Groups.Add(listGroup);

                // Add the items in every group
                foreach (MarketOrder order in group)
                {
                    if (order.Item == null)
                        continue;

                    ListViewItem item = new ListViewItem(order.Item.Name, listGroup);
                    item.UseItemStyleForSubItems = false;
                    item.Tag = order;

                    // Display text as dimmed if the order is no longer available
                    if (!order.IsAvailable)
                        item.ForeColor = SystemColors.GrayText;

                    // Display text highlighted if the order is modified
                    if (order.State == OrderState.Modified)
                        item.ForeColor = SystemColors.HotTrack;

                    // Add enough subitems to match the number of columns
                    while (item.SubItems.Count < lvOrders.Columns.Count + 1)
                    {
                        item.SubItems.Add(String.Empty);
                    }

                    // Creates the subitems
                    for (int i = 0; i < lvOrders.Columns.Count; i++)
                    {
                        ColumnHeader header = lvOrders.Columns[i];
                        MarketOrderColumn column = (MarketOrderColumn)header.Tag;
                        SetColumn(order, item.SubItems[i], column);
                    }

                    // Tooltip
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Issued For: ").AppendLine(order.IssuedFor.ToString());
                    builder.Append("Issued: ").AppendLine(order.Issued.ToLocalTime().ToString());
                    builder.AppendFormat(CultureConstants.DefaultCulture, "Duration: {0} Day{1}", order.Duration, (order.Duration > 1 ? "s" : String.Empty));
                    builder.AppendLine();
                    builder.Append("Solar System: ").AppendLine(order.Station.SolarSystem.FullLocation);
                    builder.Append("Station: ").AppendLine(order.Station.Name);
                    item.ToolTipText = builder.ToString();

                    lvOrders.Items.Add(item);
                }
            }

        }

        /// <summary>
        /// Updates the item sorter.
        /// </summary>
        private void UpdateSort()
        {
            lvOrders.ListViewItemSorter = new ListViewItemComparerByTag<MarketOrder>(
                new MarketOrderComparer(m_sortCriteria, m_sortAscending));
            
            UpdateSortVisualFeedback();
        }

        /// <summary>
        /// Updates the sort feedback (the arrow on the header).
        /// </summary>
        private void UpdateSortVisualFeedback()
        {
            for (int i = 0; i < lvOrders.Columns.Count; i++)
            {
                MarketOrderColumn column = (MarketOrderColumn)lvOrders.Columns[i].Tag;
                if (m_sortCriteria == column)
                {
                    lvOrders.Columns[i].ImageIndex = (m_sortAscending ? 0 : 1);
                }
                else
                {
                    lvOrders.Columns[i].ImageIndex = 2;
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
                    CellFormat format = FormatExpiration(order);
                    item.Text = format.Text;
                    item.ForeColor = format.TextColor;
                    break;

                case MarketOrderColumn.InitialVolume:
                    item.Text = ( m_numberFormat ?
                        MarketOrder.Format(order.InitialVolume, AbbreviationFormat.AbbreviationSymbols):
                        String.Format(CultureConstants.DefaultCulture, order.InitialVolume.ToString("#,##0")));
                    break;

                case MarketOrderColumn.Issued:
                    item.Text = order.Issued.ToLocalTime().ToShortDateString();
                    break;

                case MarketOrderColumn.IssuedFor:
                    item.Text = order.IssuedFor.ToString();
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

        #endregion


        # region Helper Methods

        /// <summary>
        /// The description of the range.
        /// </summary>
        public string GetRange(int range)
        {
            switch (range)
            {
                case 0:
                    return "stations";
                case 1:
                    return "solar systems";
                case 2:
                    return "5 jumps";
                case 3:
                    return "10 jumps";
                case 4:
                    return "20 jumps";
                case 5:
                    return "regions";
                default:
                    return String.Empty; 
            }
        }

        /// <summary>
        /// Checks the given text matches the item.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsTextMatching(MarketOrder x, string text)
        {
            if (String.IsNullOrEmpty(text)
                || x.Item.Name.ToLowerInvariant().Contains(text)
                || x.Item.Description.ToLowerInvariant().Contains(text)
                || x.Station.Name.ToLowerInvariant().Contains(text)
                || x.Station.SolarSystem.Name.ToLowerInvariant().Contains(text)
                || x.Station.SolarSystem.Constellation.Name.ToLowerInvariant().Contains(text)
                || x.Station.SolarSystem.Constellation.Region.Name.ToLowerInvariant().Contains(text))
                return true;

            return false;
        }

        /// <summary>
        /// Gets the text and formatting for the expiration cell
        /// </summary>
        /// <param name="order">Order to generate a format for</param>
        /// <returns>CellFormat object describing the format of the cell</returns>
        private static CellFormat FormatExpiration(MarketOrder order)
        {
            // Initialize to sensible defaults
            CellFormat format = new CellFormat()
            {
                TextColor = Color.Black,
                Text = order.Expiration.ToRemainingTimeShortDescription(DateTimeKind.Utc).ToUpper(CultureConstants.DefaultCulture)
            };

            // Order is expiring soon
            if (order.IsAvailable && order.Expiration < DateTime.UtcNow.AddDays(1))
                format.TextColor = Color.DarkOrange;

            // We have all the information for formatting an available order
            if (order.IsAvailable)
                return format;

            // Order isn't available so lets format it as such
            format.Text = order.State.ToString();

            if (order.State == OrderState.Expired)
                format.TextColor = Color.Red;

            if (order.State == OrderState.Fulfilled)
                format.TextColor = Color.DarkGreen;

            return format;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handles the MouseHover event of the lvOrders control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void lvOrders_MouseHover(object sender, EventArgs e)
        {
            Focus();
        }

        /// <summary>
        /// On resize, updates the controls visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindowMarketOrdersList_Resize(object sender, EventArgs e)
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
        void listView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            m_columnsChanged = true;
        } 

        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns || m_columns.Count <= e.ColumnIndex)
                return;

            m_columns[e.ColumnIndex].Width = lvOrders.Columns[e.ColumnIndex].Width;
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user clicks a column header, we update the sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            MarketOrderColumn column = (MarketOrderColumn)lvOrders.Columns[e.Column].Tag;
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
                case Keys.A:
                    if (e.Control)
                        lvOrders.SelectAll();
                    break;
                case Keys.Delete:
                    if (lvOrders.SelectedItems.Count == 0)
                        return;

                    // Mark as ignored
                    foreach (ListViewItem item in lvOrders.SelectedItems)
                    {
                        MarketOrder order = (MarketOrder)item.Tag;
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
        /// On timer tick, we update the column settings if any changes have been made to them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (m_columnsChanged)
            {
                Settings.UI.MainWindow.MarketOrders.Columns = Columns.Select(x => x.Clone()).ToArray();

                // Recreate the columns
                Columns = Settings.UI.MainWindow.MarketOrders.Columns;
            }

            m_columnsChanged = false;
        }

        /// <summary>
        /// When the market orders are changed,
        /// update the list and the expandable panel info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveMonClient_CharacterMarketOrdersUpdated(object sender, CharacterChangedEventArgs e)
        {
            CCPCharacter ccpCharacter = Character as CCPCharacter;
            if (e.Character != ccpCharacter)
                return;
            
            Orders = ccpCharacter.MarketOrders;
            UpdateColumns();
            UpdateExpPanelContent();
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
                marketExpPanelControl.Visible = false;
                return;
            }

            if (m_init)
                marketExpPanelControl.Visible = true;

            // Update the Header text of the panel
            UpdateHeaderText();

            // Update the info in the panel
            UpdatePanelInfo();

            // Force to redraw
            marketExpPanelControl.Refresh();
        }

        /// <summary>
        /// Updates the header text of the panel.
        /// </summary>
        private void UpdateHeaderText()
        {
            int baseOrders = 5;
            int maxOrders = baseOrders + m_skillBasedOrders;
            int activeOrders = m_activeOrdersIssuedForCharacter + m_activeOrdersIssuedForCorporation;
            int remainingOrders = maxOrders - activeOrders;
            decimal activeSellOrdersTotal = m_sellOrdersIssuedForCharacterTotal + m_sellOrdersIssuedForCorporationTotal;
            decimal activeBuyOrdersTotal = m_buyOrdersIssuedForCharacterTotal + m_buyOrdersIssuedForCorporationTotal;

            string ordersRemainingText = String.Format(CultureConstants.DefaultCulture, "Orders Remaining: {0} out of {1} max", remainingOrders, maxOrders);
            string activeSellOrdersTotalText = String.Format(CultureConstants.DefaultCulture, "Sell Orders Total: {0:N} ISK", activeSellOrdersTotal);
            string activeBuyOrdersTotalText = String.Format(CultureConstants.DefaultCulture, "Buy Orders Total: {0:N} ISK", activeBuyOrdersTotal);
            marketExpPanelControl.HeaderText = String.Format(CultureConstants.DefaultCulture, "{0}{3,5}{1}{3,5}{2}",
                                                ordersRemainingText, activeSellOrdersTotalText, activeBuyOrdersTotalText, String.Empty);
        }

        /// <summary>
        /// Updates the labels text in the panel.
        /// </summary>
        private void UpdatePanelInfo()
        {
            // Calculate the related info for the panel
            CalculatePanelInfo();

            // Update each label text
            decimal totalEscrow = m_issuedForCharacterTotalEscrow + m_issuedForCorporationTotalEscrow;
            decimal escrowAdditionalToCover = m_issuedForCharacterEscrowAdditionalToCover + m_issuedForCorporationEscrowAdditionalToCover;
            int activeSellOrdersCount = m_activeSellOrdersIssuedForCharacterCount + m_activeSellOrdersIssuedForCorporationCount;
            int activeBuyOrdersCount = m_activeBuyOrdersIssuedForCharacterCount + m_activeBuyOrdersIssuedForCorporationCount;
            bool marketingFirstLevelIsTrained = (Character.Skills[DBConstants.MarketingSkillID].LastConfirmedLvl > 0);
            string askRange = GetRange(m_askRange);
            string bidRange = GetRange(m_bidRange);
            string modificationRange = GetRange(m_modificationRange);
            string remoteBidRange = GetRange(m_remoteBidRange);

            // Update the basic label text
            lblTotalEscrow.Text = String.Format(CultureConstants.DefaultCulture, "Total in Escrow: {0:N} ISK (additional {1:N} ISK to cover)", totalEscrow, escrowAdditionalToCover);
            lblBaseBrokerFee.Text = String.Format(CultureConstants.DefaultCulture, "Base Broker Fee: {0:0.0#}% of order value", m_baseBrokerFee);
            lblTransactionTax.Text = String.Format(CultureConstants.DefaultCulture, "Transaction Tax: {0:0.0#}% of sales value", m_transactionTax);
            lblActiveSellOrdersCount.Text = String.Format(CultureConstants.DefaultCulture, "Active Sell Orders: {0}", activeSellOrdersCount);
            lblActiveBuyOrdersCount.Text = String.Format(CultureConstants.DefaultCulture, "Active Buy Orders: {0}", activeBuyOrdersCount);
            lblAskRange.Text = String.Format(CultureConstants.DefaultCulture, "Ask Range: limited to {0}", askRange);
            lblBidRange.Text = String.Format(CultureConstants.DefaultCulture, "Bid Range: limited to {0}", bidRange);
            lblModificationRange.Text = String.Format(CultureConstants.DefaultCulture, "Modification Range: limited to {0}", modificationRange);
            lblRemoteBidRange.Text = (marketingFirstLevelIsTrained ? String.Format(CultureConstants.DefaultCulture, "Remote Bid Range: limited to {0}", remoteBidRange) : String.Empty);

            // Update the supplemental label text
            lblCharTotalEscrow.Text = String.Format(CultureConstants.DefaultCulture, "Character Issued: {0:N} ISK (additional {1:N} ISK to cover)", m_issuedForCharacterTotalEscrow, m_issuedForCharacterEscrowAdditionalToCover);
            lblCorpTotalEscrow.Text = String.Format(CultureConstants.DefaultCulture, "Corporation Issued: {0:N} ISK (additional {1:N} ISK to cover)", m_issuedForCorporationTotalEscrow, m_issuedForCorporationEscrowAdditionalToCover);
            lblActiveCharSellOrdersCount.Text = String.Format(CultureConstants.DefaultCulture, "Character Issued: {0}", m_activeSellOrdersIssuedForCharacterCount);
            lblActiveCorpSellOrdersCount.Text = String.Format(CultureConstants.DefaultCulture, "Corporation Issued: {0}", m_activeSellOrdersIssuedForCorporationCount);
            lblActiveCharBuyOrdersCount.Text = String.Format(CultureConstants.DefaultCulture, "Character Issued: {0}", m_activeBuyOrdersIssuedForCharacterCount);
            lblActiveCorpBuyOrdersCount.Text = String.Format(CultureConstants.DefaultCulture, "Corporation Issued: {0}", m_activeBuyOrdersIssuedForCorporationCount);
            lblActiveCharSellOrdersTotal.Text = String.Format(CultureConstants.DefaultCulture, "Total: {0:N} ISK", m_sellOrdersIssuedForCharacterTotal);
            lblActiveCorpSellOrdersTotal.Text = String.Format(CultureConstants.DefaultCulture, "Total: {0:N} ISK", m_sellOrdersIssuedForCorporationTotal);
            lblActiveCharBuyOrdersTotal.Text = String.Format(CultureConstants.DefaultCulture, "Total: {0:N} ISK", m_buyOrdersIssuedForCharacterTotal);
            lblActiveCorpBuyOrdersTotal.Text = String.Format(CultureConstants.DefaultCulture, "Total: {0:N} ISK", m_buyOrdersIssuedForCorporationTotal);

            // Update label position
            UpdatePanelControlPosition();
        }

        /// <summary>
        /// Updates expandable panel controls positions.
        /// </summary>
        private void UpdatePanelControlPosition()
        {
            marketExpPanelControl.SuspendLayout();

            int pad = 5;
            int height = (marketExpPanelControl.ExpandDirection == Direction.Up ? pad : marketExpPanelControl.HeaderHeight);

            lblTotalEscrow.Location = new Point(5, height);
            height += lblTotalEscrow.Height;
            if (HasActiveCorporationIssuedOrders)
            {
                lblCharTotalEscrow.Location = new Point(15, height);
                lblCharTotalEscrow.Visible = true;
                height += lblCharTotalEscrow.Height;

                lblCorpTotalEscrow.Location = new Point(15, height);
                lblCorpTotalEscrow.Visible = true;
                height += lblCorpTotalEscrow.Height;
            }
            else
            {
                lblCharTotalEscrow.Visible = false;
                lblCorpTotalEscrow.Visible = false;
            }

            height += pad;

            lblBaseBrokerFee.Location = new Point(5, height);
            lblAskRange.Location = new Point(lblAskRange.Location.X, height);
            height += lblBaseBrokerFee.Height;

            lblTransactionTax.Location = new Point(5, height);
            lblBidRange.Location = new Point(lblBidRange.Location.X, height);
            height += lblTransactionTax.Height;

            lblModificationRange.Location = new Point(lblModificationRange.Location.X, height);
            height += lblModificationRange.Height;

            lblActiveSellOrdersCount.Location = new Point(5, height);
            lblRemoteBidRange.Location = new Point(lblRemoteBidRange.Location.X, height);
            height += lblActiveSellOrdersCount.Height;

            if (HasActiveCorporationIssuedOrders)
            {
                lblActiveCharSellOrdersCount.Location = new Point(15, height);
                lblActiveCharSellOrdersTotal.Location = new Point(150, height);
                lblActiveCharSellOrdersCount.Visible = true;
                lblActiveCharSellOrdersTotal.Visible = true;
                height += lblCharTotalEscrow.Height;

                lblActiveCorpSellOrdersCount.Location = new Point(15, height);
                lblActiveCorpSellOrdersTotal.Location = new Point(150, height);
                lblActiveCorpSellOrdersCount.Visible = true;
                lblActiveCorpSellOrdersTotal.Visible = true;
                height += lblCorpTotalEscrow.Height + pad;
            }
            else
            {
                lblActiveCharSellOrdersCount.Visible = false;
                lblActiveCharSellOrdersTotal.Visible = false;
                lblActiveCorpSellOrdersCount.Visible = false;
                lblActiveCorpSellOrdersTotal.Visible = false;
            }

            lblActiveBuyOrdersCount.Location = new Point(5, height);
            height += lblActiveBuyOrdersCount.Height;

            if (HasActiveCorporationIssuedOrders)
            {
                lblActiveCharBuyOrdersCount.Location = new Point(15, height);
                lblActiveCharBuyOrdersTotal.Location = new Point(150, height);
                lblActiveCharBuyOrdersCount.Visible = true;
                lblActiveCharBuyOrdersTotal.Visible = true;
                height += lblCharTotalEscrow.Height;

                lblActiveCorpBuyOrdersCount.Location = new Point(15, height);
                lblActiveCorpBuyOrdersTotal.Location = new Point(150, height);
                lblActiveCorpBuyOrdersCount.Visible = true;
                lblActiveCorpBuyOrdersTotal.Visible = true;
                height += lblCorpTotalEscrow.Height;
            }
            else
            {
                lblActiveCharBuyOrdersCount.Visible = false;
                lblActiveCharBuyOrdersTotal.Visible = false;
                lblActiveCorpBuyOrdersCount.Visible = false;
                lblActiveCorpBuyOrdersTotal.Visible = false;
            }
            height += pad;

            // Update panel's expanded height
            marketExpPanelControl.ExpandedHeight = height +
                (marketExpPanelControl.ExpandDirection == Direction.Up ? marketExpPanelControl.HeaderHeight : pad);
           
            marketExpPanelControl.ResumeLayout();
        }

        /// <summary>
        /// Calculates the market orders related info for the panel.
        /// </summary>
        private void CalculatePanelInfo()
        {
            IEnumerable<MarketOrder> activeSellOrdersIssuedForCharacter = m_list
                                        .Where(x => (x.State == OrderState.Active || x.State == OrderState.Modified)
                                                    && x is SellOrder && x.IssuedFor == IssuedFor.Character);
            IEnumerable<MarketOrder> activeSellOrdersIssuedForCorporation = m_list
                                        .Where(x => (x.State == OrderState.Active || x.State == OrderState.Modified)
                                                    && x is SellOrder && x.IssuedFor == IssuedFor.Corporation);
            IEnumerable<MarketOrder> activeBuyOrdersIssuedForCharacter = m_list
                                        .Where(x => (x.State == OrderState.Active || x.State == OrderState.Modified)
                                                    && x is BuyOrder && x.IssuedFor == IssuedFor.Character);
            IEnumerable<MarketOrder> activeBuyOrdersIssuedForCorporation = m_list
                                        .Where(x => (x.State == OrderState.Active || x.State == OrderState.Modified)
                                                    && x is BuyOrder && x.IssuedFor == IssuedFor.Corporation);

            // Calculate character's max orders
            m_skillBasedOrders = Character.Skills[DBConstants.TradeSkillID].LastConfirmedLvl * 4
                + Character.Skills[DBConstants.RetailSkillID].LastConfirmedLvl * 8
                + Character.Skills[DBConstants.WholesaleSkillID].LastConfirmedLvl * 16
                + Character.Skills[DBConstants.TycconSkillID].LastConfirmedLvl * 32;

            // Calculate character's base broker fee
            m_baseBrokerFee = 1 - (Character.Skills[DBConstants.BrokerRelationsSkillID].LastConfirmedLvl * 0.05f);

            // Calculate character's transaction tax
            m_transactionTax = 1 - (Character.Skills[DBConstants.AccountingSkillID].LastConfirmedLvl * 0.1f);

            // Calculate character's ask range
            m_askRange = Character.Skills[DBConstants.MarketingSkillID].LastConfirmedLvl;

            // Calculate character's bid range
            m_bidRange = Character.Skills[DBConstants.ProcurementSkillID].LastConfirmedLvl;

            // Calculate character's modification range
            m_modificationRange = Character.Skills[DBConstants.DaytradingSkillID].LastConfirmedLvl;

            // Calculate character's remote bid range
            m_remoteBidRange = Character.Skills[DBConstants.VisibilitySkillID].LastConfirmedLvl;

            // Calculate active sell & buy orders total price (character & corporation issued separately)
            m_sellOrdersIssuedForCharacterTotal = activeSellOrdersIssuedForCharacter.Sum(x => x.TotalPrice);
            m_sellOrdersIssuedForCorporationTotal = activeSellOrdersIssuedForCorporation.Sum(x => x.TotalPrice);
            m_buyOrdersIssuedForCharacterTotal = activeBuyOrdersIssuedForCharacter.Sum(x => x.TotalPrice);
            m_buyOrdersIssuedForCorporationTotal = activeBuyOrdersIssuedForCorporation.Sum(x => x.TotalPrice);

            // Calculate active sell & buy orders count (character & corporation issued separately)
            m_activeSellOrdersIssuedForCharacterCount = activeSellOrdersIssuedForCharacter.Count();
            m_activeSellOrdersIssuedForCorporationCount = activeSellOrdersIssuedForCorporation.Count();
            m_activeBuyOrdersIssuedForCharacterCount = activeBuyOrdersIssuedForCharacter.Count();
            m_activeBuyOrdersIssuedForCorporationCount = activeBuyOrdersIssuedForCorporation.Count();

            // Calculate active orders (character & corporation issued separately)
            m_activeOrdersIssuedForCharacter = m_activeSellOrdersIssuedForCharacterCount + m_activeBuyOrdersIssuedForCharacterCount;
            m_activeOrdersIssuedForCorporation = m_activeSellOrdersIssuedForCorporationCount + m_activeBuyOrdersIssuedForCorporationCount;

            // Calculate total escrow (character & corporation issued separately)
            m_issuedForCharacterTotalEscrow = activeBuyOrdersIssuedForCharacter.Sum(x => ((BuyOrder)x).Escrow);
            m_issuedForCorporationTotalEscrow = activeBuyOrdersIssuedForCorporation.Sum(x => ((BuyOrder)x).Escrow);

            // Calculate escrow additional to cover (character & corporation issued separately)
            m_issuedForCharacterEscrowAdditionalToCover = m_buyOrdersIssuedForCharacterTotal - m_issuedForCharacterTotalEscrow;
            m_issuedForCorporationEscrowAdditionalToCover = m_buyOrdersIssuedForCorporationTotal - m_issuedForCorporationTotalEscrow;
        }

        # endregion


        #region Initialize Expandable Panel Controls

        // Basic labels constructor
        private Label lblTotalEscrow = new Label();
        private Label lblBaseBrokerFee = new Label();
        private Label lblTransactionTax = new Label();
        private Label lblActiveSellOrdersCount = new Label();
        private Label lblActiveBuyOrdersCount = new Label();
        private Label lblAskRange = new Label();
        private Label lblBidRange = new Label();
        private Label lblModificationRange = new Label();
        private Label lblRemoteBidRange = new Label();

        // Supplemental labels constructor
        private Label lblCharTotalEscrow = new Label();
        private Label lblCorpTotalEscrow = new Label();
        private Label lblActiveCharSellOrdersTotal = new Label();
        private Label lblActiveCorpSellOrdersTotal = new Label();
        private Label lblActiveCharBuyOrdersTotal = new Label();
        private Label lblActiveCorpBuyOrdersTotal = new Label();
        private Label lblActiveCharSellOrdersCount = new Label();
        private Label lblActiveCorpSellOrdersCount = new Label();
        private Label lblActiveCharBuyOrdersCount = new Label();
        private Label lblActiveCorpBuyOrdersCount = new Label();

        private void InitializeExpandablePanelControls()
        {
            marketExpPanelControl.SuspendLayout();

            // Add basic labels to panel
            marketExpPanelControl.Controls.AddRange(new Label[]
            { 
                lblTotalEscrow,
                lblBaseBrokerFee,
                lblTransactionTax,
                lblActiveSellOrdersCount,
                lblActiveBuyOrdersCount,
                lblAskRange,
                lblBidRange,
                lblModificationRange,
                lblRemoteBidRange,
            });

            // Add supplemental labels to panel
            marketExpPanelControl.Controls.AddRange(new Label[]
            { 
                lblCharTotalEscrow,
                lblCorpTotalEscrow,
                lblActiveCharSellOrdersTotal,
                lblActiveCorpSellOrdersTotal,
                lblActiveCharBuyOrdersTotal,
                lblActiveCorpBuyOrdersTotal,
                lblActiveCharSellOrdersCount,
                lblActiveCorpSellOrdersCount,
                lblActiveCharBuyOrdersCount,
                lblActiveCorpBuyOrdersCount,
            });

            // Apply properties
            foreach (Label label in marketExpPanelControl.Controls.OfType<Label>())
            {
                label.ForeColor = SystemColors.ControlText;
                label.BackColor = Color.Transparent;
                label.AutoSize = true;
            }

            // Special properties
            lblAskRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblBidRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblModificationRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblRemoteBidRange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAskRange.Location = new Point(220, 0);
            lblBidRange.Location = new Point(220, 0);
            lblModificationRange.Location = new Point(220, 0);
            lblRemoteBidRange.Location = new Point(220, 0);

            // Subscribe events
            foreach (Control control in marketExpPanelControl.Controls.OfType<Label>())
            {
                control.MouseClick += new MouseEventHandler(marketExpPanelControl.expandablePanelControl_MouseClick);
            }

            marketExpPanelControl.ResumeLayout();
        }

        #endregion


        #region Helper Classes

        private class CellFormat
        {
            public Color TextColor { get; set; }
            public string Text { get; set; }
        }

        #endregion
    }
}
