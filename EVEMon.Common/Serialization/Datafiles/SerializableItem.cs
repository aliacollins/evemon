using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Base serializable class for items, ships and implants.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableItem
    {
        private readonly Collection<SerializablePrerequisiteSkill> m_prerequisiteSkills;
        private readonly Collection<SerializablePropertyValue> m_properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableItem"/> class.
        /// </summary>
        public SerializableItem()
        {
            m_prerequisiteSkills = new Collection<SerializablePrerequisiteSkill>();
            m_properties = new Collection<SerializablePropertyValue>();
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
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlAttribute("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        [XmlAttribute("category")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        [XmlAttribute("group")]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        [XmlAttribute("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the race.
        /// </summary>
        /// <value>The race.</value>
        [XmlAttribute("race")]
        public Race Race { get; set; }

        /// <summary>
        /// Gets or sets the meta level.
        /// </summary>
        /// <value>The meta level.</value>
        [XmlAttribute("metaLevel")]
        public int MetaLevel { get; set; }

        /// <summary>
        /// Gets or sets the meta group.
        /// </summary>
        /// <value>The meta group.</value>
        [XmlAttribute("metaGroup")]
        public ItemMetaGroup MetaGroup { get; set; }

        /// <summary>
        /// Gets or sets the slot.
        /// </summary>
        /// <value>The slot.</value>
        [XmlAttribute("slot")]
        public ItemSlot Slot { get; set; }

        /// <summary>
        /// Gets or sets the family.
        /// </summary>
        /// <value>The family.</value>
        [XmlAttribute("family")]
        public ItemFamily Family { get; set; }

        /// <summary>
        /// Gets or sets the size of the portion.
        /// </summary>
        /// <value>The size of the portion.</value>
        [XmlAttribute("portionSize")]
        public int PortionSize { get; set; }

        /// <summary>
        /// Gets the prerequisite skills.
        /// </summary>
        /// <value>The prerequisite skills.</value>
        [XmlElement("s")]
        public Collection<SerializablePrerequisiteSkill> PrerequisiteSkills
        {
            get { return m_prerequisiteSkills; }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [XmlElement("p")]
        public Collection<SerializablePropertyValue> Properties
        {
            get { return m_properties; }
        }
    }
}