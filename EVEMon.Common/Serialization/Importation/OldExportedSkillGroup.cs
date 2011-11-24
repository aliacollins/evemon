using System.Collections.ObjectModel;
using System.Xml.Serialization;

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
    [XmlRoot("skillGroup")]
    public sealed class OldExportedSkillGroup
    {
        private readonly Collection<OldExportedSkill> m_skills;

        public OldExportedSkillGroup()
        {
            m_skills = new Collection<OldExportedSkill>();
        }

        [XmlAttribute("groupName")]
        public string Name { get; set; }

        [XmlAttribute("groupID")]
        public int Id { get; set; }

        [XmlElement("skill")]
        public Collection<OldExportedSkill> Skills
        {
            get { return m_skills; }
        }
    }
}