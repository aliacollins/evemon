using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIMarketOrders
    {
        [XmlArray("orders")]
        [XmlArrayItem("order")]
        public List<SerializableOrderListItem> Orders
        {
            get;
            set;
        }
    }
}
