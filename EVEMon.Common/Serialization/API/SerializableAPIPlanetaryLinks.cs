using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIPlanetaryLinks
    {
        private readonly Collection<SerializablePlanetaryLink> m_links;

        public SerializableAPIPlanetaryLinks()
        {
            m_links = new Collection<SerializablePlanetaryLink>();
        }

        [XmlArray("links")]
        [XmlArrayItem("link")]
        public Collection<SerializablePlanetaryLink> Links
        {
            get { return m_links; }
        }
    }
}
