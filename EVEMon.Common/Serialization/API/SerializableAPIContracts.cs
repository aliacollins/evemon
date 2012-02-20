using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIContracts
    {
        private readonly Collection<SerializableContractListItem> m_contracts;

        public SerializableAPIContracts()
        {
            m_contracts = new Collection<SerializableContractListItem>();
        }

        [XmlArray("contracts")]
        [XmlArrayItem("contract")]
        public Collection<SerializableContractListItem> Contracts
        {
            get { return m_contracts; }
        }
    }
}
