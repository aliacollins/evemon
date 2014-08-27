using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of planetary pins. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIPlanetaryPins
    {
         private readonly Collection<SerializablePlanetaryPin> m_pins;

         public SerializableAPIPlanetaryPins()
        {
            m_pins = new Collection<SerializablePlanetaryPin>();
        }

        [XmlArray("pins")]
        [XmlArrayItem("pin")]
         public Collection<SerializablePlanetaryPin> Pins
        {
            get { return m_pins; }
        }
   }
}
