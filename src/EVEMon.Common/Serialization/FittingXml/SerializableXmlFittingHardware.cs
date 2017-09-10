using System.Xml.Serialization;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.FittingXml
{
    public sealed class SerializableXmlFittingHardware
    {
        [XmlAttribute("qty")]
        public int Quantity { get; set; }

        [XmlAttribute("slot")]
        public string Slot { get; set; }

        [XmlAttribute("type")]
        public string ItemXml
        {
            get { return Item.Name; }
            set
            {
                Item = StaticItems.GetItemByName(value) ?? Item.UnknownItem;
            }
        }

        [XmlIgnore]
        public Item Item { get; set; }
    }
}