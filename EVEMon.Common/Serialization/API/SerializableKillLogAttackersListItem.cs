using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableKillLogAttackersListItem : SerializableCharacterListItem
    {
        [XmlAttribute("damageDone")]
        public int DamageDone { get; set; }

        [XmlAttribute("finalBlow")]
        public bool FinalBlow { get; set; }

        [XmlAttribute("securityStatus")]
        public double SecurityStatus { get; set; }

        [XmlAttribute("weaponTypeID")]
        public int WeaponTypeID { get; set; }
    }
}