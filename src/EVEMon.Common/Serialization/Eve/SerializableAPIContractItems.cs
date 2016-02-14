using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
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
        public Collection<SerializableContractItemsListItem> ContractItems => m_contractItems;
    }
}
