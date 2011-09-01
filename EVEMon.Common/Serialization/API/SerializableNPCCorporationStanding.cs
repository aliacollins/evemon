using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableNPCCorporationStanding : SerializableStandingsListItem
    {
        public SerializableNPCCorporationStanding()
        {
            GroupType = "NPC Corporations";
        }

        [XmlArray("NPCCorporations")]
        [XmlArrayItem("NPCCorporation")]
        public List<SerializableNPCCorporationStanding> NPCCorporationStandings { get; set; }
    }
}