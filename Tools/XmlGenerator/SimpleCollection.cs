using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator
{
    /// <summary>
    /// Simple wrapper for collection of objects of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlRoot("collection")]
    public sealed class SimpleCollection<T>
    {
        private readonly Collection<T> m_items; 

        /// <summary>
        /// Constructor.
        /// </summary>
        public SimpleCollection()
        {
            m_items = new Collection<T>();
        }

        /// <summary>
        /// List of items in the collection.
        /// </summary>
        [XmlElement("item")]
        public Collection<T> Items
        {
            get { return m_items; }
        }

    }
}