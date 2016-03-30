using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of planetary links. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIPlanetaryLinks
    {
        private readonly Collection<SerializablePlanetaryLink> m_links;

        public SerializableAPIPlanetaryLinks()
        {
            m_links = new Collection<SerializablePlanetaryLink>();
        }

        [XmlArray("links")]
        [XmlArrayItem("link")]
        public Collection<SerializablePlanetaryLink> Links => m_links;
    }
}
