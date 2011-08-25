using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAgentStanding : SerializableStandingsListItem
    {
        public SerializableAgentStanding()
        {
            GroupType = "Agents";
        }

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public List<SerializableStandingsListItem> AgentStandings { get; set; }
    }
}
