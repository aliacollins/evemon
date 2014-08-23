using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIPlanetaryRoutes
    {
        private readonly Collection<SerializablePlanetaryRoute> m_routes;

        public SerializableAPIPlanetaryRoutes()
        {
            m_routes = new Collection<SerializablePlanetaryRoute>();
        }

        [XmlArray("routes")]
        [XmlArrayItem("route")]
        public Collection<SerializablePlanetaryRoute> Routes
        {
            get { return m_routes; }
        }
    }
}
