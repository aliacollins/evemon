using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents our reprocessing datafile.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    [XmlRoot("reprocessingDatafile")]
    public sealed class ReprocessingDatafile
    {
        private Collection<SerializableItemMaterials> m_items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReprocessingDatafile"/> class.
        /// </summary>
        public ReprocessingDatafile()
        {
            m_items = new Collection<SerializableItemMaterials>();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("item")]
        public Collection<SerializableItemMaterials> Items
        {
            get { return m_items; }
        }

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Add(List<SerializableItemMaterials> items)
        {
            m_items = new Collection<SerializableItemMaterials>(items);
        }
    }
}