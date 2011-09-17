using System.Collections.Generic;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Serialization.Exportation
{
    [XmlRoot("plans")]
    public sealed class OutputPlans
    {
        public OutputPlans()
        {
            Plans = new List<SerializablePlan>();
        }

        [XmlAttribute("revision")]
        public int Revision { get; set; }

        [XmlElement("plan")]
        public List<SerializablePlan> Plans { get; set; }

    }
}