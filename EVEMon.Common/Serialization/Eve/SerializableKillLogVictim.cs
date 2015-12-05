using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public class SerializableKillLogVictim : SerializableCharacterListItem
    {
        [XmlAttribute("damageTaken")]
        public int DamageTaken { get; set; }
    }
}