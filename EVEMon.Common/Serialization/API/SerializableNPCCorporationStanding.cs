using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableNPCCorporationStanding : SerializableStandingsListItem
    {
        private readonly Collection<SerializableStandingsListItem> m_npcCorporationStandings;

        public SerializableNPCCorporationStanding()
        {
            m_npcCorporationStandings = new Collection<SerializableStandingsListItem>();
            GroupType = "NPC Corporations";
        }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public Collection<SerializableStandingsListItem> NPCCorporationStandings
        {
            get { return m_npcCorporationStandings; }
        }
    }
}