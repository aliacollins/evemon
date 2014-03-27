using System;
using System.Xml.Serialization;
using EVEMon.Common.Data;

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
        public string CorporationNameXml
        {
            get { return CorporationName; }
            set { CorporationName = String.IsNullOrEmpty(value) ? "Unknown" : value.HtmlDecode(); }
        }

        [XmlAttribute("allianceID")]
        public long AllianceID { get; set; }

        [XmlAttribute("allianceName")]
        public string AllianceNameXml
        {
            get { return AllianceName; }
            set { AllianceName = String.IsNullOrEmpty(value) ? "Unknown" : value.HtmlDecode(); }
        }

        [XmlAttribute("factionID")]
        public int FactionID { get; set; }

        [XmlAttribute("factionName")]
        public string FactionNameXml
        {
            get { return FactionName; }
            set { FactionName = String.IsNullOrEmpty(value) ? "Unknown" : value; }
        }

        [XmlAttribute("shipTypeID")]
        public int ShipTypeID { get; set; }

        [XmlIgnore]
        public string CorporationName { get; set; }

        [XmlIgnore]
        public string AllianceName { get; set; }

        [XmlIgnore]
        public string FactionName { get; set; }

        [XmlIgnore]
        public string ShipTypeName
        {
            get
            {
                Item ship = StaticItems.GetItemByID(ShipTypeID);
                return ship == null || ShipTypeID == 0 ? "Unknown" : ship.Name;
            }
        }
    }
}