using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializablePropertyCategory
    {
        private Collection<SerializableProperty> m_properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializablePropertyCategory"/> class.
        /// </summary>
        public SerializablePropertyCategory()
        {
            m_properties = new Collection<SerializableProperty>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public Collection<SerializableProperty> Properties
        {
            get { return m_properties; }
        }

        /// <summary>
        /// Adds the specified properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void Add(List<SerializableProperty> properties)
        {
            m_properties = new Collection<SerializableProperty>(properties);
        }
    }
}