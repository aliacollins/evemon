using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Exportation
{
    /// <summary>
    /// A serialization class designed for HTML exportation.
    /// </summary>
    [XmlRoot("outputCharacter")]
    public sealed class OutputCharacter
    {
        private readonly Collection<OutputAttributeEnhancer> m_attributeEnhancers;
        private readonly Collection<OutputSkillGroup> m_skillGroups;
        private readonly Collection<OutputCertificate> m_certificates;

        public OutputCharacter()
        {
            m_attributeEnhancers = new Collection<OutputAttributeEnhancer>();
            m_skillGroups = new Collection<OutputSkillGroup>();
            m_certificates = new Collection<OutputCertificate>();
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("characterID")]
        public long CharacterID { get; set; }

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

        [XmlElement("balance")]
        public string Balance { get; set; }

        [XmlElement("birthday")]
        public string Birthday { get; set; }

        [XmlElement("intelligence")]
        public float Intelligence { get; set; }

        [XmlElement("charisma")]
        public float Charisma { get; set; }

        [XmlElement("perception")]
        public float Perception { get; set; }

        [XmlElement("memory")]
        public float Memory { get; set; }

        [XmlElement("willpower")]
        public float Willpower { get; set; }

        [XmlArray("attributeEnhancers")]
        [XmlArrayItem("implant")]
        public Collection<OutputAttributeEnhancer> AttributeEnhancers
        {
            get { return m_attributeEnhancers; }
        }

        [XmlArray("skills")]
        [XmlArrayItem("skillGroup")]
        public Collection<OutputSkillGroup> SkillGroups
        {
            get { return m_skillGroups; }
        }

        [XmlArray("certificates")]
        [XmlArrayItem("certificate")]
        public Collection<OutputCertificate> Certificates
        {
            get { return m_certificates; }
        }
    }
}