using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents our EVE geography datafile.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    [XmlRoot("geographyDatafile")]
    public sealed class GeoDatafile
    {
        private Collection<SerializableRegion> m_regions;
        private Collection<SerializableJump> m_jumps;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoDatafile"/> class.
        /// </summary>
        public GeoDatafile()
        {
            m_regions = new Collection<SerializableRegion>();
            m_jumps = new Collection<SerializableJump>();
        }

        /// <summary>
        /// Gets the regions.
        /// </summary>
        /// <value>The regions.</value>
        [XmlArray("regions")]
        [XmlArrayItem("region")]
        public Collection<SerializableRegion> Regions
        {
            get { return m_regions; }
        }

        /// <summary>
        /// Gets the jumps.
        /// </summary>
        /// <value>The jumps.</value>
        [XmlArray("jumps")]
        [XmlArrayItem("jump")]
        public Collection<SerializableJump> Jumps
        {
            get { return m_jumps; }
        }

        /// <summary>
        /// Adds the specified regions.
        /// </summary>
        /// <param name="regions">The regions.</param>
        public void Add(List<SerializableRegion> regions)
        {
            m_regions = new Collection<SerializableRegion>(regions);
        }

        /// <summary>
        /// Adds the specified jumps.
        /// </summary>
        /// <param name="jumps">The jumps.</param>
        public void Add(List<SerializableJump> jumps)
        {
            m_jumps = new Collection<SerializableJump>(jumps);
        }
    }
}