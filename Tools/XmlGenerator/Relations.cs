using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator
{
    /// <summary>
    /// Collection of related items of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlRoot("collection")]
    public sealed class Relations<T>
        where T : class, IRelation
    {
        private readonly Collection<T> m_items; 

        /// <summary>
        /// Constructor.
        /// </summary>
        public Relations()
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
        /// Converts collection to a RelationSet.
        /// </summary>
        /// <returns></returns>
        public RelationSet<T> ToSet()
        {
            return new RelationSet<T>(Items);
        }
    }
}