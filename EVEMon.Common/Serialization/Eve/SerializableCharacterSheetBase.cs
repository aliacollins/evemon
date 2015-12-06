using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of a character sheet. Used for settings.xml serialization and CCP querying
    /// </summary>
    public class SerializableCharacterSheetBase : ISerializableCharacterIdentity
    {
        private readonly Collection<SerializableCharacterSkill> m_skills;
        private readonly Collection<SerializableEmploymentHistory> m_employmentHistory;
        private readonly Collection<SerializableCharacterCertificate> m_certificates;

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
        public string NameXml
        {
            get { return Name; }
            set { Name = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("homeStationID")]
        public long HomeStationID { get; set; }

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
        public string CorporationNameXml
        {
            get { return CorporationName; }
            set { CorporationName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("corporationID")]
        public long CorporationID { get; set; }

        [XmlElement("allianceName")]
        public string AllianceNameXml
        {
            get { return AllianceName; }
            set { AllianceName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("allianceID")]
        public long AllianceID { get; set; }

        [XmlElement("factionName")]
        public string FactionName { get; set; }

        [XmlElement("factionID")]
        public int FactionID { get; set; }

        [XmlElement("freeSkillPoints")]
        public int FreeSkillPoints { get; set; }

        [XmlElement("freeRespecs")]
        public short FreeRespecs { get; set; }

        [XmlElement("cloneJumpDate")]
        public string CloneJumpDateXml
        {
            get { return CloneJumpDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    CloneJumpDate = value.TimeStringToDateTime();
            }
        }

        [XmlElement("lastRespecDate")]
        public string LastRespecDateXml
        {
            get { return LastRespecDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    LastRespecDate = value.TimeStringToDateTime();
            }
        }

        [XmlElement("lastTimedRespec")]
        public string LastTimedRespecXml
        {
            get { return LastTimedRespec.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    LastTimedRespec = value.TimeStringToDateTime();
            }
        }

        [XmlElement("remoteStationDate")]
        public string RemoteStationDateXml
        {
            get { return RemoteStationDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    RemoteStationDate = value.TimeStringToDateTime();
            }
        }

        [XmlElement("jumpActivation")]
        public string JumpActivationDateXml
        {
            get { return JumpActivationDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    JumpActivationDate = value.TimeStringToDateTime();
            }
        }

        [XmlElement("jumpFatigue")]
        public string JumpFatigueDateXml
        {
            get { return JumpFatigueDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    JumpFatigueDate = value.TimeStringToDateTime();
            }
        }

        [XmlElement("jumpLastUpdate")]
        public string JumpLastUpdateDateXml
        {
            get { return JumpLastUpdateDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    JumpLastUpdateDate = value.TimeStringToDateTime();
            }
        }

        [XmlArray("certificates")]
        [XmlArrayItem("certificate")]
        public Collection<SerializableCharacterCertificate> Certificates
        {
            get { return m_certificates; }
        }

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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlIgnore]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the corporation.
        /// </summary>
        [XmlIgnore]
        public string CorporationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the alliance.
        /// </summary>
        [XmlIgnore]
        public string AllianceName { get; set; }

        /// <summary>
        /// The date and time the character was created.
        /// </summary>
        [XmlIgnore]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// The date and time the jump clone was created.
        /// </summary>
        [XmlIgnore]
        public DateTime CloneJumpDate { get; set; }

        /// <summary>
        /// The date and time of the last remap.
        /// </summary>
        [XmlIgnore]
        public DateTime LastRespecDate { get; set; }

        /// <summary>
        /// The date and time of the last remap.
        /// </summary>
        [XmlIgnore]
        public DateTime LastTimedRespec { get; set; }

        /// <summary>
        /// The date and time of the last remap.
        /// </summary>
        [XmlIgnore]
        public DateTime RemoteStationDate { get; set; }

        /// <summary>
        /// Gets or sets the jump activation date.
        /// </summary>
        [XmlIgnore]
        public DateTime JumpActivationDate { get; set; }

        /// <summary>
        /// Gets or sets the jump fatigue date.
        /// </summary>
        [XmlIgnore]
        public DateTime JumpFatigueDate { get; set; }

        /// <summary>
        /// Gets or sets the jump last update date.
        /// </summary>
        [XmlIgnore]
        public DateTime JumpLastUpdateDate { get; set; }
    }
}