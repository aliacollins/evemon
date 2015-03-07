using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic.Loadout
{
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableLoadoutRace
    {
        private readonly Collection<SerializableLoadout> m_loadouts;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableLoadoutRace"/> class.
        /// </summary>
        public SerializableLoadoutRace()
        {
            m_loadouts = new Collection<SerializableLoadout>();
        }

        /// <summary>
        /// Gets the loadouts.
        /// </summary>
        /// <value>The loadouts.</value>
        [XmlArray("ship")]
        [XmlArrayItem("loadout")]
        public Collection<SerializableLoadout> Loadouts
        {
            get { return m_loadouts; }
        }
    }
}