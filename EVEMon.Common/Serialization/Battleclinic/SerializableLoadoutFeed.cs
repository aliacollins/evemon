using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    [XmlRoot("loadouts")]
    public sealed class SerializableLoadoutFeed
    {
        [XmlElement("race")]
        public SerializableLoadoutRace Race { get; set; }
    }

    public sealed class SerializableLoadoutRace
    {
        [XmlArray("ship")]
        [XmlArrayItem("loadout")]
        public SerializableLoadout[] Loadouts { get; set; }
    }
}
