using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of NPC standings. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIStandings
    {
        [XmlElement("characterNPCStandings")]
        public SerializableStandings CharacterNPCStandings { get; set; }
    }
}
