using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents our properties datafile.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    [XmlRoot("propertiesDatafile")]
    public sealed class PropertiesDatafile
    {
        private Collection<SerializablePropertyCategory> m_categories;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesDatafile"/> class.
        /// </summary>
        public PropertiesDatafile()
        {
            m_categories = new Collection<SerializablePropertyCategory>();
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        [XmlElement("category")]
        public Collection<SerializablePropertyCategory> Categories
        {
            get { return m_categories; }
        }

        /// <summary>
        /// Adds the specified categories.
        /// </summary>
        /// <param name="categories">The categories.</param>
        public void Add(List<SerializablePropertyCategory> categories)
        {
            m_categories = new Collection<SerializablePropertyCategory>(categories);
        }
    }
}