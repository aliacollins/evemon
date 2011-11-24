using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableStandings
    {
        private readonly Collection<SerializableAgentStanding> m_agentStandings;
        private readonly Collection<SerializableNPCCorporationStanding> m_npcCorporationStandings;
        private readonly Collection<SerializableFactionStanding> m_factionStandings;

        public SerializableStandings()
        {
            m_agentStandings = new Collection<SerializableAgentStanding>();
            m_npcCorporationStandings = new Collection<SerializableNPCCorporationStanding>();
            m_factionStandings = new Collection<SerializableFactionStanding>();
        }

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public Collection<SerializableAgentStanding> AgentStandings
        {
            get { return m_agentStandings; }
        }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public Collection<SerializableNPCCorporationStanding> NPCCorporationStandings
        {
            get { return m_npcCorporationStandings; }
        }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public Collection<SerializableFactionStanding> FactionStandings
        {
            get { return m_factionStandings; }
        }

        [XmlIgnore]
        public IEnumerable<SerializableStandingsListItem> All
        {
            get
            {
                List<SerializableStandingsListItem> standings = new List<SerializableStandingsListItem>();
                standings.AddRange(AgentStandings);
                standings.AddRange(NPCCorporationStandings);
                standings.AddRange(FactionStandings);
                return standings;
            }
        }
    }
}