using System;
using System.Collections;
using System.Collections.Generic;

namespace EVEMon.XmlGenerator
{
    /// <summary>
    /// A dictionary-based implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Bag<T> : IEnumerable<T>
        where T : IHasID
    {
        private readonly Dictionary<int, T> m_items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bag&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The list.</param>
        public Bag(IndexedCollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            m_items = new Dictionary<int, T>();

            foreach (T item in collection.Items)
            {
                m_items[item.ID] = item;
            }
        }

        /// <summary>
        /// Determines whether the specified id has value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// 	<c>true</c> if the specified id has value; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValue(int id)
        {
            return m_items.ContainsKey(id);
        }

        /// <summary>
        /// Gets or sets the <see cref="T"/> with the specified id.
        /// </summary>
        /// <value></value>
        public T this[int id]
        {
            get { return m_items[id]; }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)m_items.Values).GetEnumerator();
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