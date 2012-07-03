using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of a kill log. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIKillLog
    {
        private readonly Collection<SerializableKillLogListItem> m_kills;

        public SerializableAPIKillLog()
        {
            m_kills = new Collection<SerializableKillLogListItem>();
        }

        [XmlArray("kills")]
        [XmlArrayItem("kill")]
        public Collection<SerializableKillLogListItem> Kills
        {
            get { return m_kills; }
        }
    }
}
