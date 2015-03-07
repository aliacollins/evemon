using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a remapping point.
    /// </summary>
    public sealed class SerializableRemappingPoint
    {
        [XmlAttribute("status")]
        public RemappingPointStatus Status { get; set; }

        [XmlAttribute("per")]
        public Int64 Perception { get; set; }

        [XmlAttribute("int")]
        public Int64 Intelligence { get; set; }

        [XmlAttribute("mem")]
        public Int64 Memory { get; set; }

        [XmlAttribute("wil")]
        public Int64 Willpower { get; set; }

        [XmlAttribute("cha")]
        public Int64 Charisma { get; set; }

        [XmlAttribute("description")]
        public String Description { get; set; }
    }
}