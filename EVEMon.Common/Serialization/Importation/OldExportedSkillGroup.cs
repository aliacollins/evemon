using System.Collections.Generic;
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
        public OldExportedSkillGroup()
        {
            Skills = new List<OldExportedSkill>();
        }

        [XmlAttribute("groupName")]
        public string Name { get; set; }

        [XmlAttribute("groupID")]
        public int Id { get; set; }

        [XmlElement("skill")]
        public List<OldExportedSkill> Skills { get; set; }
    }
}
