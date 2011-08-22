using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableStandings
    {
        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public List<SerializableAgentStanding> AgentStandings { get; set; }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public List<SerializableNPCCorporationStanding> NPCCorporationStandings { get; set; }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public List<SerializableFactionStanding> FactionStandings { get; set; }

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
