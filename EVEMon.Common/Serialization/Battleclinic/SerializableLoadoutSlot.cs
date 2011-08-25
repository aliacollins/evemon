using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    public sealed class SerializableLoadoutSlot
    {
        [XmlAttribute("type")]
        public string SlotType { get; set; }

        [XmlAttribute("position")]
        public string SlotPosition { get; set; }

        [XmlText]
        public int ItemID { get; set; }
    }
}