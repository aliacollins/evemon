using System.Xml.Serialization;

using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Serialization.Importation
{
    [XmlRoot("attributes")]
    public sealed class OldAttributes
    {
        [XmlElement("intelligence")]
        public int Intelligence { get; set; }

        [XmlElement("charisma")]
        public int Charisma { get; set; }

        [XmlElement("perception")]
        public int Perception { get; set; }

        [XmlElement("memory")]
        public int Memory { get; set; }

        [XmlElement("willpower")]
        public int Willpower { get; set; }

        internal SerializableCharacterAttributes ToSerializableAttributes()
        {
            return new SerializableCharacterAttributes
                       {
                           Charisma = Charisma,
                           Intelligence = Intelligence,
                           Memory = Memory,
                           Perception = Perception,
                           Willpower = Willpower
                       };
        }
    }
}
