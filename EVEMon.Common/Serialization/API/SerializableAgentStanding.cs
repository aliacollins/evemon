using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAgentStanding : SerializableStandingsListItem
    {
        private readonly Collection<SerializableStandingsListItem> m_agentStandings;

        public SerializableAgentStanding()
        {
            m_agentStandings = new Collection<SerializableStandingsListItem>();
            GroupType = "Agents";
        }

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public Collection<SerializableStandingsListItem> AgentStandings
        {
            get { return m_agentStandings; }
        }
    }
}