using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of a characters' research points. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIResearch
    {
        private readonly Collection<SerializableResearchListItem> m_researchPoints;

        public SerializableAPIResearch()
        {
            m_researchPoints = new Collection<SerializableResearchListItem>();
        }

        [XmlArray("research")]
        [XmlArrayItem("points")]
        public Collection<SerializableResearchListItem> ResearchPoints
        {
            get { return m_researchPoints; }
        }
    }
}