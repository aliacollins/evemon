using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic.MarketPricer
{
    [XmlRoot("itemValues")]
    public sealed class SerializableBCItemPrices
    {
        private Collection<SerializableBCItemPrice> m_itemPrices;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableBCItemPrices"/> class.
        /// </summary>
        public SerializableBCItemPrices()
        {
            m_itemPrices = new Collection<SerializableBCItemPrice>();
        }

        [XmlElement("cachedUntil")]
        public DateTime CachedUntil { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("type")]
        public Collection<SerializableBCItemPrice> ItemPrices
        {
            get { return m_itemPrices; }
        }
    }
}