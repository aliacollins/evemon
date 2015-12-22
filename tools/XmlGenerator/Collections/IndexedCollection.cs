using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.Collections
{
    /// <summary>
    /// A collection of indexed items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class IndexedCollection<T> : IEnumerable<T>
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
        /// Converts collection to a BagCollection.
        /// </summary>
        /// <returns>BagCollection of type T</returns>
        public BagCollection<T> ToBag()
        {
            return new BagCollection<T>(this);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}