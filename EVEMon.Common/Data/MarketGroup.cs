using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Collections;

namespace EVEMon.Common.Data
{
    #region MarketGroup
    /// <summary>
    /// Represents an item category
    /// </summary>
    public sealed class MarketGroup
    {
        private readonly MarketGroupCollection m_subCategories;
        private readonly ItemCollection m_items;
        private readonly MarketGroup m_parent;
        private readonly string m_name;
        private readonly int m_id;

        /// <summary>
        /// Deserialization constructor for root category only
        /// </summary>
        /// <param name="src"></param>
        public MarketGroup(SerializableMarketGroup src)
        {
            m_id = src.ID;
            m_name = src.Name;
            m_subCategories = new MarketGroupCollection(this, src.SubGroups);
            m_items = new ItemCollection(this, src.Items);
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="src"></param>
        public MarketGroup(MarketGroup parent, SerializableMarketGroup src)
            : this(src)
        {
            m_parent = parent;
        }

        /// <summary>
        /// Gets the group ID.
        /// </summary>
        public int ID
        {
            get { return m_id; }
        }

        /// <summary>
        /// Gets the parent category. <c>Null</c> for the root category.
        /// </summary>
        public MarketGroup ParentGroup
        {
            get { return m_parent; }
        }

        /// <summary>
        /// Gets the sub categories
        /// </summary>
        public MarketGroupCollection SubGroups
        {
            get { return m_subCategories; }
        }

        /// <summary>
        /// Gets the items in this category
        /// </summary>
        public ItemCollection Items
        {
            get { return m_items; }
        }

        /// <summary>
        /// Gets the collection of all the items in this category and its descendants.
        /// </summary>
        public IEnumerable<Item> AllItems
        {
            get
            {
                foreach (var item in m_items)
                {
                    yield return item;
                }

                foreach (var cat in m_subCategories)
                {
                    foreach (var subItem in cat.AllItems)
                    {
                        yield return subItem;
                    }
                }

            }
        }

        /// <summary>
        /// Gets this category's name
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// gets the name of this item.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }
    }
    #endregion



    #region MarketGroupCollection
    /// <summary>
    /// Represents a read-only collection of items
    /// </summary>
    public sealed class MarketGroupCollection : ReadonlyCollection<MarketGroup>
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="src"></param>
        internal MarketGroupCollection(MarketGroup cat, SerializableMarketGroup[] src)
            : base(src == null ? 0 : src.Length)
        {
            if (src == null) return;

            foreach (var subCat in src)
            {
                m_items.Add(new MarketGroup(cat, subCat));
            }
        }
    }
    #endregion
}
