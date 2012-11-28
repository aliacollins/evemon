using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a reference to a character in the charactersList API
    /// </summary>
    public class SerializableCharacterListItem : ISerializableCharacterIdentity
    {
        [XmlAttribute("characterID")]
        public long ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("corporationID")]
        public long CorporationID { get; set; }

        [XmlAttribute("corporationName")]
        public string CorporationName { get; set; }

        [XmlAttribute("allianceID")]
        public long AllianceID { get; set; }

        [XmlAttribute("allianceName")]
        public string AllianceName { get; set; }

        [XmlAttribute("factionID")]
        public int FactionID { get; set; }

        [XmlAttribute("factionName")]
        public string FactionName { get; set; }

        [XmlAttribute("shipTypeID")]
        public int ShipTypeID { get; set; }
    }
}