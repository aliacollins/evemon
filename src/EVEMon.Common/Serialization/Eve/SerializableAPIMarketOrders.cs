using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of market orders. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIMarketOrders
    {
        private readonly Collection<SerializableOrderListItem> m_orders;

        public SerializableAPIMarketOrders()
        {
            m_orders = new Collection<SerializableOrderListItem>();
        }

        [XmlArray("orders")]
        [XmlArrayItem("order")]
        public Collection<SerializableOrderListItem> Orders => m_orders;
    }
}