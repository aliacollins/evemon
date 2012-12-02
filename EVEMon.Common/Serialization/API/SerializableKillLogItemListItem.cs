using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableKillLogItemListItem
    {
        private readonly Collection<SerializableKillLogItemListItem> m_items;

        public SerializableKillLogItemListItem()
        {
            m_items = new Collection<SerializableKillLogItemListItem>();
        }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("flag")]
        public short EVEFlag { get; set; }

        [XmlAttribute("qtyDropped")]
        public int QtyDropped { get; set; }

        [XmlAttribute("qtyDestroyed")]
        public int QtyDestroyed { get; set; }

        [XmlAttribute("singleton")]
        public byte Singleton { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public Collection<SerializableKillLogItemListItem> Items
        {
            get { return m_items; }
        }
    }
}