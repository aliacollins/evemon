using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIContractItems
    {
        private readonly Collection<SerializableContractItemsListItem> m_contractItems;
        public SerializableAPIContractItems()
        {
            m_contractItems = new Collection<SerializableContractItemsListItem>();
        }

        [XmlArray("contractItems")]
        [XmlArrayItem("contractItem")]
        public Collection<SerializableContractItemsListItem> ContractItems
        {
            get { return m_contractItems; }
        }
    }
}
