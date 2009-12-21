using System;
using System.Collections.Generic;
using System.Text;
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

    /// <summary>
    /// Represents an attribute enhan
    /// </summary>
    public sealed class SerializableAPIImplant
    {
        [XmlElement("augmentatorName")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("augmentatorValue")]
        public int Amount
        {
            get;
            set;
        }
    }
}
