using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of a character sheet. Used for settings.xml serialization and CCP querying
    /// </summary>
    public class SerializableCharacterSheetBase : ISerializableCharacterIdentity
    {
        private readonly Collection<SerializableCharacterSkill> m_skills;
        private readonly Collection<SerializableCharacterCertificate> m_certificates;
        private readonly Collection<SerializableEmploymentHistory> m_employmentHistory;

        protected SerializableCharacterSheetBase()
        {
            Attributes = new SerializableCharacterAttributes();
            m_skills = new Collection<SerializableCharacterSkill>();
            m_certificates = new Collection<SerializableCharacterCertificate>();
            m_employmentHistory = new Collection<SerializableEmploymentHistory>();
        }

        [XmlElement("characterID")]
        public long ID { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("DoB")]
        public string BirthdayXml
        {
            get { return Birthday.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    Birthday = value.TimeStringToDateTime();
            }
        }

        [XmlElement("race")]
        public string Race { get; set; }

        [XmlElement("bloodLine")]
        public string BloodLine { get; set; }

        [XmlElement("ancestry")]
        public string Ancestry { get; set; }

        [XmlElement("gender")]
        public string Gender { get; set; }

        [XmlElement("corporationName")]
        public string CorporationName { get; set; }

        [XmlElement("corporationID")]
        public long CorporationID { get; set; }

        [XmlElement("allianceName")]
        public string AllianceName { get; set; }

        [XmlElement("allianceID")]
        public int AllianceID { get; set; }

        [XmlElement("cloneName")]
        public string CloneName { get; set; }

        [XmlElement("cloneSkillPoints")]
        public int CloneSkillPoints { get; set; }

        [XmlElement("balance")]
        public Decimal Balance { get; set; }

        [XmlElement("shipName")]
        public string ShipName { get; set; }

        [XmlElement("shipTypeName")]
        public string ShipTypeName { get; set; }

        [XmlElement("lastKnownLocation")]
        public string LastKnownLocation { get; set; }

        [XmlElement("securityStatus")]
        public double SecurityStatus { get; set; }

        [XmlArray("employmentHistory")]
        [XmlArrayItem("record")]
        public Collection<SerializableEmploymentHistory> EmploymentHistory
        {
            get { return m_employmentHistory; }
        }

        [XmlElement("attributes")]
        public SerializableCharacterAttributes Attributes { get; set; }

        [XmlArray("skills")]
        [XmlArrayItem("skill")]
        public Collection<SerializableCharacterSkill> Skills
        {
            get { return m_skills; }
        }

        [XmlArray("certificates")]
        [XmlArrayItem("certificate")]
        public Collection<SerializableCharacterCertificate> Certificates
        {
            get { return m_certificates; }
        }

        /// <summary>
        /// The date and time the character was created.
        /// </summary>
        [XmlIgnore]
        public DateTime Birthday { get; set; }
    }
}