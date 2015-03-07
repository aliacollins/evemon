using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents the character's attributes
    /// </summary>
    public sealed class SerializableCharacterAttributes
    {
        public SerializableCharacterAttributes()
        {
            Intelligence = Memory = Perception = Charisma = Willpower = 1;
        }

        [XmlElement("intelligence")]
        public Int64 Intelligence { get; set; }

        [XmlElement("memory")]
        public Int64 Memory { get; set; }

        [XmlElement("perception")]
        public Int64 Perception { get; set; }

        [XmlElement("willpower")]
        public Int64 Willpower { get; set; }

        [XmlElement("charisma")]
        public Int64 Charisma { get; set; }
    }
}