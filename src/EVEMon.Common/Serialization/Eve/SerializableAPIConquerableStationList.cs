using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of the conquerable stations list. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIConquerableStationList
    {
        private readonly Collection<SerializableOutpost> m_outposts;

        public SerializableAPIConquerableStationList()
        {
            m_outposts = new Collection<SerializableOutpost>();
        }

        [XmlArray("outposts")]
        [XmlArrayItem("outpost")]
        public Collection<SerializableOutpost> Outposts => m_outposts;
    }
}