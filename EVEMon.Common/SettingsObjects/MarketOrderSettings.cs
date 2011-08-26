using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MarketOrderSettings
    {
        public MarketOrderSettings()
        {
            // Add default columns
            MarketOrderColumn[] defaultColumns = new[]
                                                     {
                                                         MarketOrderColumn.Item,
                                                         MarketOrderColumn.SolarSystem,
                                                         MarketOrderColumn.UnitaryPrice,
                                                         MarketOrderColumn.Volume
                                                     };

            Columns = EnumExtensions.GetValues<MarketOrderColumn>().Where(
                x => x != MarketOrderColumn.None).Select(x =>
                                                         new MarketOrderColumnSettings
                                                             {Column = x, Visible = defaultColumns.Contains(x), Width = -1}).
                ToArray();

            HideInactiveOrders = true;
        }

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public MarketOrderColumnSettings[] Columns { get; set; }

        [XmlElement("hideInactiveOrders")]
        public bool HideInactiveOrders { get; set; }

        [XmlElement("numberAbsFormat")]
        public bool NumberAbsFormat { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public MarketOrderSettings Clone()
        {
            MarketOrderSettings clone = (MarketOrderSettings)MemberwiseClone();
            clone.Columns = Columns.Select(x => x.Clone()).ToArray();
            return clone;
        }
    }
}
