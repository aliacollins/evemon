using System;
using System.Collections;
using System.Collections.Generic;

namespace EVEMon.XmlGenerator
{
    public sealed class RelationSet<T> : IEnumerable<T>
        where T : class, IRelation
    {
        private readonly Dictionary<long, T> m_dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationSet&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public RelationSet(IEnumerable<T> src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            m_dictionary = new Dictionary<long, T>();
            foreach (T item in src)
            {
                m_dictionary[GetKey(item)] = item;
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified left].
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified left]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(long left, long right)
        {
            return m_dictionary.ContainsKey(GetKey(left, right));
        }

        /// <summary>
        /// Gets the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public T Get(long left, long right)
        {
            T value;
            m_dictionary.TryGetValue(GetKey(left, right), out value);
            return value;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="relation">The relation.</param>
        /// <returns></returns>
        private static long GetKey(IRelation relation)
        {
            return GetKey(relation.Left, relation.Right);
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        private static long GetKey(long left, long right)
        {
            return (left << 32) | (uint)right;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)m_dictionary.Values).GetEnumerator();
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