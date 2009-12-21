using System.Xml.Serialization;

namespace EVEMon.Common
{
    public enum EveAttribute
    {
        [XmlEnum("perception")]
        Perception,
        [XmlEnum("memory")]
        Memory,
        [XmlEnum("willpower")]
        Willpower,
        [XmlEnum("intelligence")]
        Intelligence,
        [XmlEnum("charisma")]
        Charisma,
        [XmlEnum("none")]
        None
    }
}