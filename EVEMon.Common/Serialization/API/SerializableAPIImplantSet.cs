using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents the set of attributes enhancers
    /// </summary>
    public sealed class SerializableAPIImplantSet
    {
        [XmlElement("intelligenceBonus")]
        public SerializableAPIImplant Intelligence
        {
            get;
            set;
        }

        [XmlElement("memoryBonus")]
        public SerializableAPIImplant Memory
        {
            get;
            set;
        }

        [XmlElement("willpowerBonus")]
        public SerializableAPIImplant Willpower
        {
            get;
            set;
        }

        [XmlElement("perceptionBonus")]
        public SerializableAPIImplant Perception
        {
            get;
            set;
        }

        [XmlElement("charismaBonus")]
        public SerializableAPIImplant Charisma
        {
            get;
            set;
        }
    }
}
