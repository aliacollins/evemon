using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a skill
    /// </summary>
    public sealed class SerializableCharacterSkill
    {
        [XmlAttribute("typeID")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("level")]
        public Int64 Level { get; set; }

        [XmlAttribute("activelevel")]
        public Int64 ActiveLevel { get; set; }

        [XmlAttribute("skillpoints")]
        public Int64 Skillpoints { get; set; }

        [XmlAttribute("ownsBook")]
        public bool OwnsBook { get; set; }

        [XmlAttribute("isKnown")]
        public bool IsKnown { get; set; }
    }
}
