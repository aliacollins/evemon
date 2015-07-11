using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.EVEAddicts.MarketPricer
{
    [XmlRoot("prices")]
    public sealed class SerializableEAItemPrices
    {
        private Collection<SerializableEAItemPrice> m_itemPrices;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableEAItemPrice"/> class.
        /// </summary>
        public SerializableEAItemPrices()
        {
            m_itemPrices = new Collection<SerializableEAItemPrice>();
        }

        [XmlElement("typeID")]
        public Collection<SerializableEAItemPrice> ItemPrices
        {
            get { return m_itemPrices; }
        }
    }
}
