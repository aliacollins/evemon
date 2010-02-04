using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MarketOrderSettings
    {
        public MarketOrderSettings()
        {
            // Add default columns
            var defaultColumns = new MarketOrderColumn[] { 
                MarketOrderColumn.Item, 
                MarketOrderColumn.SolarSystem, 
                MarketOrderColumn.UnitaryPrice, 
                MarketOrderColumn.Volume};

            Columns = EnumExtensions.GetValues<MarketOrderColumn>().Where(x => x != MarketOrderColumn.None).Select(x =>
                new MarketOrderColumnSettings { Column = x, Visible = defaultColumns.Contains(x), Width = -1 }).ToArray();

            HideInactiveOrders = true;
        }

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public MarketOrderColumnSettings[] Columns
        {
            get;
            set;
        }

        [XmlElement("hideInactiveOrders")]
        public bool HideInactiveOrders
        {
            get;
            set;
        }

        [XmlElement("numberAbsFormat")]
        public bool NumberAbsFormat
        {
            get;
            set;
        }

        public MarketOrderSettings Clone()
        {
            MarketOrderSettings clone = (MarketOrderSettings)MemberwiseClone();
            clone.Columns = this.Columns.Select(x => x.Clone()).ToArray();
            clone.HideInactiveOrders = this.HideInactiveOrders;
            clone.NumberAbsFormat = this.NumberAbsFormat;
            return clone;
        }
    }

    public enum MarketOrderGrouping
    {
        [Header("Group by order status")]
        State = 0,
        [Header("Group by order status (Desc)")]
        StateDesc = 1,
        [Header("Group by buying/selling")]
        OrderType = 2,
        [Header("Group by buying/selling (Desc)")]
        OrderTypeDesc = 3,
        [Header("Group by issue day")]
        Issued = 4,
        [Header("Group by issue day (Desc)")]
        IssuedDesc = 5,
        [Header("Group by item type")]
        ItemType = 6,
        [Header("Group by item type (Desc)")]
        ItemTypeDesc = 7,
        [Header("Group by station")]
        Location = 8,
        [Header("Group by station (Desc)")]
        LocationDesc = 9
    }
}
