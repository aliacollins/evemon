using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a solar system in the EVE universe.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableSolarSystem
    {
        private readonly Collection<SerializableStation> m_stations;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableSolarSystem"/> class.
        /// </summary>
        public SerializableSolarSystem()
        {
            m_stations = new Collection<SerializableStation>();
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        [XmlAttribute("x")]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        [XmlAttribute("y")]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the Z.
        /// </summary>
        /// <value>The Z.</value>
        [XmlAttribute("z")]
        public int Z { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the security level.
        /// </summary>
        /// <value>The security level.</value>
        [XmlAttribute("securityLevel")]
        public float SecurityLevel { get; set; }

        /// <summary>
        /// Gets the stations.
        /// </summary>
        /// <value>The stations.</value>
        [XmlElement("stations")]
        public Collection<SerializableStation> Stations
        {
            get { return m_stations; }
        }
    }
}