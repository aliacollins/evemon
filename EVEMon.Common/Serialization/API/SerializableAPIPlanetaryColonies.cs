using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIPlanetaryColonies
    {
        private readonly Collection<SerializablePlanetaryColony> m_colonies;

        public SerializableAPIPlanetaryColonies()
        {
            m_colonies = new Collection<SerializablePlanetaryColony>();
        }

        [XmlArray("colonies")]
        [XmlArrayItem("colony")]
        public Collection<SerializablePlanetaryColony> Colonies
        {
            get { return m_colonies; }
        }
    }
}
