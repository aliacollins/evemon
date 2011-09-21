using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a blueprint group in our datafile.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableBlueprintMarketGroup
    {
        private Collection<SerializableBlueprint> m_blueprints;
        private Collection<SerializableBlueprintMarketGroup> m_subGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableBlueprintMarketGroup"/> class.
        /// </summary>
        public SerializableBlueprintMarketGroup()
        {
            m_blueprints = new Collection<SerializableBlueprint>();
            m_subGroups = new Collection<SerializableBlueprintMarketGroup>();
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the blueprints.
        /// </summary>
        /// <value>The blueprints.</value>
        [XmlElement("blueprint")]
        public Collection<SerializableBlueprint> Blueprints
        {
            get { return m_blueprints; }
        }

        /// <summary>
        /// Gets the sub groups.
        /// </summary>
        /// <value>The sub groups.</value>
        [XmlArray("subGroups")]
        [XmlArrayItem("subGroup")]
        public Collection<SerializableBlueprintMarketGroup> SubGroups
        {
            get { return m_subGroups; }
        }

        /// <summary>
        /// Adds the specified blueprints.
        /// </summary>
        /// <param name="blueprints">The blueprints.</param>
        public void Add(List<SerializableBlueprint> blueprints)
        {
            m_blueprints = new Collection<SerializableBlueprint>(blueprints);
        }

        /// <summary>
        /// Adds the specified sub groups.
        /// </summary>
        /// <param name="subGroups">The sub groups.</param>
        public void Add(List<SerializableBlueprintMarketGroup> subGroups)
        {
            m_subGroups = new Collection<SerializableBlueprintMarketGroup>(subGroups);
        }
    }
}