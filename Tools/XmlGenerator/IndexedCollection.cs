using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator
{
    /// <summary>
    /// A collection of indexed items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlRoot("collection")]
    public sealed class IndexedCollection<T>
        where T : IHasID
    {
        private readonly Collection<T> m_items; 

        /// <summary>
        /// Constructor.
        /// </summary>
        public IndexedCollection()
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

        /// <summary>
        /// Converts collection to a Bag.
        /// </summary>
        /// <returns>Bag of type T</returns>
        public Bag<T> ToBag()
        {
            return new Bag<T>(this);
        }
    }
}