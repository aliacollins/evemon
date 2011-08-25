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
    public sealed class OldExportedAttributeBonus
    {
        [XmlElement("augmentatorName")]
        public string Name { get; set; }

        [XmlElement("augmentatorValue")]
        public int Amount { get; set; }

        [XmlAttribute]
        public bool Manual { get; set; }
    }
}
