using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvTypeReactions
    {
        [XmlElement("reactionTypeID")]
        public int ReactionTypeID { get; set; }

        [XmlElement("input")]
        public bool Input { get; set; }

        [XmlElement("typeID")]
        public int TypeID { get; set; }

        [XmlElement("quantity")]
        public long Quantity { get; set; }
    }
}