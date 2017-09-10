using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.Eve
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


        [XmlIgnore]
        public string WeaponTypeName
        {
            get
            {
                if (WeaponTypeID == 0)
                    return string.Empty;

                Item weapon = StaticItems.GetItemByID(WeaponTypeID);
                return weapon?.Name ?? EveMonConstants.UnknownText;
            }
        }
    }
}