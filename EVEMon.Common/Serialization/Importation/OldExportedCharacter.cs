using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Serialization.Importation
{
    /// <summary>
    /// Facilitates importation of file characters from versions of
    /// EVEMon prior to 1.3.0.
    /// </summary>
    /// <remarks>
    /// These changes were released early 2010, it is safe to assume
    /// that they can be removed from the project early 2012.
    /// </remarks>
    [XmlRoot("character")]
    public sealed class OldExportedCharacter
    {
        private readonly Collection<OldExportedSkillGroup> m_skillgroups;
        private readonly Collection<OldExportedCertificate> m_certificates;

        public OldExportedCharacter()
        {
            m_skillgroups = new Collection<OldExportedSkillGroup>();
            m_certificates = new Collection<OldExportedCertificate>();
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("characterID")]
        public int CharacterId { get; set; }

        [XmlElement("race")]
        public string Race { get; set; }

        [XmlElement("bloodLine")]
        public string BloodLine { get; set; }

        [XmlElement("gender")]
        public string Gender { get; set; }

        [XmlElement("corporationName")]
        public string CorpName { get; set; }

        [XmlElement("cloneName")]
        public string CloneName { get; set; }

        [XmlElement("cloneSkillPoints")]
        public int CloneSkillPoints { get; set; }

        [XmlElement("balance")]
        public Decimal Balance { get; set; }

        [XmlElement("attributes")]
        public OldAttributes Attributes { get; set; }

        [XmlElement("attributeEnhancers")]
        public OldExportedAttributeEnhancers AttributeEnhancers { get; set; }

        [XmlArray("skills")]
        [XmlArrayItem("skillGroup")]
        public Collection<OldExportedSkillGroup> SkillGroups
        {
            get { return m_skillgroups; }
        }

        [XmlArray("certificates")]
        public Collection<OldExportedCertificate> Certificates
        {
            get { return m_certificates; }
        }

        /// <summary>
        /// Toes the serializable CCP character.
        /// </summary>
        /// <returns></returns>
        public SerializableCCPCharacter ToSerializableCCPCharacter()
        {
            return new SerializableCCPCharacter
                       {
                           Name = Name,
                           ID = CharacterId,
                           Race = Race,
                           BloodLine = BloodLine,
                           Gender = Gender,
                           CorporationName = CorpName,
                           CorporationID = 0,
                           CloneName = CloneName,
                           CloneSkillPoints = CloneSkillPoints,
                           Balance = Balance,
                           Attributes = Attributes.ToSerializableAttributes(),
                           ImplantSets = { API = OldExportedAttributeEnhancers.ToSerializableImplantSet() },
                           Skills = CreateSerializableCharacterSkillList(),
                           Certificates = CreateSerializableCharacterCertificateList()
                       };
        }

        /// <summary>
        /// Creates the serializable character skill list.
        /// </summary>
        /// <returns></returns>
        private List<SerializableCharacterSkill> CreateSerializableCharacterSkillList()
        {
            return (SkillGroups.SelectMany(group => group.Skills,
                                           (group, skill) => new SerializableCharacterSkill
                                                                 {
                                                                     ID = skill.Id,
                                                                     IsKnown = true,
                                                                     Level = skill.Level,
                                                                     OwnsBook = true,
                                                                     Skillpoints = skill.SkillPoints
                                                                 })).ToList();
        }

        /// <summary>
        /// Creates the serializable character certificate list.
        /// </summary>
        /// <returns></returns>
        private List<SerializableCharacterCertificate> CreateSerializableCharacterCertificateList()
        {
            return Certificates.Select(
                certificate => new SerializableCharacterCertificate
                                   {
                                       CertificateID = certificate.CertificateID
                                   }).ToList();
        }
    }
}