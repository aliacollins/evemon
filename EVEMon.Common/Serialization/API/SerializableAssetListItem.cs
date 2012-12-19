using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAssetListItem
    {
        private readonly Collection<SerializableAssetListItem> m_contents;

        public SerializableAssetListItem()
        {
            m_contents = new Collection<SerializableAssetListItem>();
        }

        [XmlArray("contents")]
        [XmlArrayItem("content")]
        public Collection<SerializableAssetListItem> Contents
        {
            get { return m_contents; }
        }

        [XmlAttribute("itemID")]
        public long ItemID { get; set; }

        [XmlAttribute("locationID")]
        public long LocationID { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("quantity")]
        public long Quantity { get; set; }

        [XmlAttribute("flag")]
        public short EVEFlag { get; set; }

        [XmlAttribute("singleton")]
        public byte Singleton { get; set; }

        [XmlAttribute("rawQuantity")]
        public int RawQuantity { get; set; }
    }
}