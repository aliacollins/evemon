using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public class SerializableKillLogVictim : SerializableCharacterListItem
    {
        [XmlAttribute("allianceID")]
        public long AllianceID { get; set; }

        [XmlAttribute("allianceName")]
        public string AllianceName { get; set; }

        [XmlAttribute("factionID")]
        public int FactionID { get; set; }

        [XmlAttribute("factionName")]
        public string FactionName { get; set; }

        [XmlAttribute("damageTaken")]
        public int DamageTaken { get; set; }

        [XmlAttribute("shipTypeID")]
        public int ShipTypeID { get; set; }
    }
}