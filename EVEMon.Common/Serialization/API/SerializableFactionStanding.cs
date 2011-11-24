using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableFactionStanding : SerializableStandingsListItem
    {
        private readonly Collection<SerializableStandingsListItem> m_factionStandings;

        public SerializableFactionStanding()
        {
            m_factionStandings = new Collection<SerializableStandingsListItem>();
            GroupType = "Factions";
        }

        [XmlArray("factions")]
        [XmlArrayItem("faction")]
        public Collection<SerializableStandingsListItem> FactionStandings
        {
            get { return m_factionStandings; }
        }
    }
}