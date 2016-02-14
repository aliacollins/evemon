using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of planetary colonies. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIPlanetaryColonies
    {
        private readonly Collection<SerializablePlanetaryColony> m_colonies;

        public SerializableAPIPlanetaryColonies()
        {
            m_colonies = new Collection<SerializablePlanetaryColony>();
        }

        [XmlArray("colonies")]
        [XmlArrayItem("colony")]
        public Collection<SerializablePlanetaryColony> Colonies => m_colonies;
    }
}
