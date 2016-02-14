using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of planetary routes. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIPlanetaryRoutes
    {
        private readonly Collection<SerializablePlanetaryRoute> m_routes;

        public SerializableAPIPlanetaryRoutes()
        {
            m_routes = new Collection<SerializablePlanetaryRoute>();
        }

        [XmlArray("routes")]
        [XmlArrayItem("route")]
        public Collection<SerializablePlanetaryRoute> Routes => m_routes;
    }
}
