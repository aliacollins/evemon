using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableAPIContractBids
    {
        private readonly Collection<SerializableContractBidsListItem> m_contractBids;

        public SerializableAPIContractBids()
        {
            m_contractBids = new Collection<SerializableContractBidsListItem>();
        }

        [XmlArray("bids")]
        [XmlArrayItem("bid")]
        public Collection<SerializableContractBidsListItem> ContractBids => m_contractBids;
    }
}
