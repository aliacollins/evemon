using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Serialization
{
    /// <summary>
    /// Represents a serializable version of a character sheet. Used for settings.xml serialization and CCP querying
    /// </summary>
    public class SerializableCharacterBase : ISerializableCharacterIdentity
    {
        public SerializableCharacterBase()
        {
            Attributes = new SerializableAttributes();
            Skills = new List<SerializableCharacterSkill>();
            Certificates = new List<SerializableCharacterCertificate>();
        }

        [XmlElement("characterID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("race")]
        public string Race
        {
            get;
            set;
        }

        [XmlElement("bloodLine")]
        public string BloodLine
        {
            get;
            set;
        }

        [XmlElement("gender")]
        public string Gender
        {
            get;
            set;
        }

        [XmlElement("corporationName")]
        public string CorporationName
        {
            get;
            set;
        }

        [XmlElement("corporationID")]
        public int CorporationID
        {
            get;
            set;
        }

        [XmlElement("cloneName")]
        public string CloneName
        {
            get;
            set;
        }

        [XmlElement("cloneSkillPoints")]
        public int CloneSkillPoints
        {
            get;
            set;
        }

        [XmlElement("balance")]
        public Decimal Balance
        {
            get;
            set;
        }

        [XmlElement("attributes")]
        public SerializableAttributes Attributes
        {
            get;
            set;
        }

        [XmlArray("skills")]
        [XmlArrayItem("skill")]
        public List<SerializableCharacterSkill> Skills
        {
            get;
            set;
        }

        [XmlArray("certificates")]
        [XmlArrayItem("certificate")]
        public List<SerializableCharacterCertificate> Certificates
        {
            get;
            set;
        }
    }
}
