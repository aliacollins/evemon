using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
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
        public Collection<SerializableOutpost> Outposts
        {
            get { return m_outposts; }
        }
    }
}