using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Serialization.Exportation
{
    [XmlRoot("plans")]
    public sealed class OutputPlans
    {
        private readonly Collection<SerializablePlan> m_plans;

        public OutputPlans()
        {
            m_plans = new Collection<SerializablePlan>();
        }

        [XmlAttribute("revision")]
        public int Revision { get; set; }

        [XmlElement("plan")]
        public Collection<SerializablePlan> Plans
        {
            get { return m_plans; }
        }

    }
}