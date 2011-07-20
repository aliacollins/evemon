using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Describes a type of prerequisite certificate. May be a certificate or a skill.
    /// </summary>
    public enum SerializableCertificatePrerequisiteKind
    {
        [XmlEnum("skill")]
        Skill,
        [XmlEnum("cert")]
        Certificate
    }
}
