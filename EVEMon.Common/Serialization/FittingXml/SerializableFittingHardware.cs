using System;
using System.Xml.Serialization;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.FittingXml
{
    public sealed class SerializableFittingHardware
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
                if (!String.IsNullOrEmpty(value))
                    Item = StaticItems.GetItemByName(value);
            }
        }

        [XmlIgnore]
        public Item Item { get; set; }
    }
}