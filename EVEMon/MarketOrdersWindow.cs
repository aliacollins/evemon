using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;
using EVEMon.Common;

namespace EVEMon
{
    public partial class MarketOrdersWindow : EVEMonForm
    {
        public MarketOrdersWindow()
        {
            InitializeComponent();
            this.RememberPositionKey = "MarketOrdersWindow";
        }


        /// <summary>
        /// Gets or sets the grouping mode.
        /// </summary>
        public MarketOrderGrouping Grouping
        {
            get { return ordersList.Grouping; }
            set { ordersList.Grouping = value; }
        }

        /// <summary>
        /// Gets or sets the enumeration of orders to display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<MarketOrder> Orders
        {
            get { return ordersList.Orders; }
            set { ordersList.Orders = value; }
        }

        /// <summary>
        /// Gets or sets the enumeration of displayed columns.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<MarketOrderColumnSettings> Columns
        {
            get { return ordersList.Columns; }
            set { ordersList.Columns = value; }
        }
    }
}
