using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of alliance list. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIAllianceList
    {
        private readonly Collection<SerializableAllianceListItem> m_alliances;

        public SerializableAPIAllianceList()
        {
            m_alliances = new Collection<SerializableAllianceListItem>();
        }

        [XmlArray("alliances")]
        [XmlArrayItem("alliance")]
        public Collection<SerializableAllianceListItem> Alliances
        {
            get { return m_alliances; }
        }
    }
}
