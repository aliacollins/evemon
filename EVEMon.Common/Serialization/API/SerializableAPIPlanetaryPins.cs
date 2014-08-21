using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIPlanetaryPins
    {
         private readonly Collection<SerializablePlanetaryPin> m_pins;

         public SerializableAPIPlanetaryPins()
        {
            m_pins = new Collection<SerializablePlanetaryPin>();
        }

        [XmlArray("pins")]
        [XmlArrayItem("pin")]
         public Collection<SerializablePlanetaryPin> Colonies
        {
            get { return m_pins; }
        }
   }
}
