using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Root SkillsDatafile Serialization Class
    /// </summary>
    [XmlRoot("skills")]
    public sealed class SkillsDatafile
    {
        [XmlElement("group")]
        public SerializableSkillGroup[] Groups { get; set; }
    }
}