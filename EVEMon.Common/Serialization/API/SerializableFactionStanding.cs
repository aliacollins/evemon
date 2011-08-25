using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableFactionStanding : SerializableStandingsListItem
    {
        public SerializableFactionStanding()
        {
            GroupType = "Factions";
        }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public List<SerializableStandingsListItem> FactionStandings { get; set; }
    }
}
