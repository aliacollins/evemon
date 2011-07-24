using System.Collections.Generic;
using System.Text;

using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents an item category
    /// </summary>
    public class MarketGroup
    {
        #region Constructors

        /// <summary>
        /// Deserialization constructor for root category only
        /// </summary>
        /// <param name="src">Source Serializable Market Group</param>
        public MarketGroup(SerializableMarketGroup src)
        {
            ID = src.ID;
            Name = src.Name;
            SubGroups = new MarketGroupCollection(this, src.SubGroups);
            Items = new ItemCollection(this, src.Items);
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="parent">The Market Group this Market Group is contained within</param>
        /// <param name="src">Source Serializable Market Group</param>
        public MarketGroup(MarketGroup parent, SerializableMarketGroup src)
            : this(src)
        {
            ParentGroup = parent;
        }

        /// <summary>
        /// Deserialization constructor for root category only
        /// </summary>
        /// <param name="src"></param>
        public MarketGroup(SerializableBlueprintGroup src)
        {
            ID = src.ID;
            Name = src.Name;
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="parent">The Market Group this Market Group is contained within</param>
        /// <param name="src">Source Blueprint Group</param>
        public MarketGroup(MarketGroup parent, SerializableBlueprintGroup src)
            :this (src)
        {
            ParentGroup = parent;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the group ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the parent category. <c>Null</c> for the root category.
        /// </summary>
        public MarketGroup ParentGroup { get; private set; }

        /// <summary>
        /// Gets the sub categories.
        /// </summary>
        public MarketGroupCollection SubGroups { get; private set; }

        /// <summary>
        /// Gets the items in this category.
        /// </summary>
        public ItemCollection Items { get; private set; }

        /// <summary>
        /// Gets this category's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the collection of all the items in this category and its descendants.
        /// </summary>
        public IEnumerable<Item> AllItems
        {
            get
            {
                foreach (Item item in Items)
                {
                    yield return item;
                }

                foreach (MarketGroup cat in SubGroups)
                {
                    foreach (Item subItem in cat.AllItems)
                    {
                        yield return subItem;
                    }
                }
            }
        }


        #endregion


        #region Public Methods

        /// <summary>
        /// Gets this category full category name.
        /// </summary>
        public string GetCategoryPath()
        {
            StringBuilder fullCategoryPath = new StringBuilder();
            MarketGroup group = this;

            while (group != null)
            {
                fullCategoryPath.Insert(0, group.Name);
                group = group.ParentGroup;
                if (group != null)
                    fullCategoryPath.Insert(0, " > ");
            }

            return fullCategoryPath.ToString();
        }

        #endregion


        #region Overidden Methods

        /// <summary>
        /// Gets the name of this item.
        /// </summary>
        /// <returns>Name of the Market Group.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

    }
}
