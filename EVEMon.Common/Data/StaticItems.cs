using System.Collections.Generic;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents all the items (not ships or implants, see <see cref="StaticShips"/> and <see cref="StaticImplants"/> for that) loaded from the datafiles. 
    /// Not that all items are present, only the ones you can use for your ship. 
    /// </summary>
    public static class StaticItems
    {
        private static readonly Dictionary<long, MarketGroup> s_marketGroupsByID = new Dictionary<long, MarketGroup>();
        private static readonly Dictionary<long, Item> s_itemsByID = new Dictionary<long, Item>();
        private static readonly ImplantCollection[] s_implantSlots = new ImplantCollection[10];

        #region Initialization

        /// <summary>
        /// Initialize static items.
        /// </summary>
        internal static void Load()
        {
            if (MarketGroups != null)
                return;

            // Create the implants slots
            for (int i = 0; i < s_implantSlots.Length; i++)
            {
                s_implantSlots[i] = new ImplantCollection((ImplantSlots)i);
                s_implantSlots[i].Add(new Implant());
            }

            // Deserialize the items datafile
            ItemsDatafile datafile = Util.DeserializeDatafile<ItemsDatafile>(DatafileConstants.ItemsDatafile);
            MarketGroups = new MarketGroupCollection(null, datafile.MarketGroups);

            // Gather the items into a by-ID dictionary
            foreach (MarketGroup marketGroup in MarketGroups)
            {
                InitializeDictionaries(marketGroup);
            }
        }

        /// <summary>
        /// Recursively collect the items within all groups and stores them in the dictionaries.
        /// </summary>
        /// <param name="marketGroup"></param>
        private static void InitializeDictionaries(MarketGroup marketGroup)
        {
            // Special groups
            if (marketGroup.ID == DBConstants.ShipsMarketGroupID)
                ShipsMarketGroup = marketGroup;

            s_marketGroupsByID[marketGroup.ID] = marketGroup;

            foreach (Item item in marketGroup.Items)
            {
                s_itemsByID[item.ID] = item;
            }

            foreach (MarketGroup childGroup in marketGroup.SubGroups)
            {
                InitializeDictionaries(childGroup);
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the root category, containing all the top level categories.
        /// </summary>
        public static MarketGroupCollection MarketGroups { get; private set; }

        /// <summary>
        /// Gets the market group for ships.
        /// </summary>
        public static MarketGroup ShipsMarketGroup { get; private set; }

        /// <summary>
        /// Gets the collection of all the market groups in this category and its descendants.
        /// </summary>
        public static IEnumerable<MarketGroup> AllGroups
        {
            get
            {
                foreach (MarketGroup group in s_marketGroupsByID.Values)
                {
                    yield return group;
                }
            }
        }

        /// <summary>
        /// Gets the collection of all the items in this category and its descendants.
        /// </summary>
        public static IEnumerable<Item> AllItems
        {
            get
            {
                foreach (Item item in s_itemsByID.Values)
                {
                    yield return item;
                }
            }
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the collection of implants for the given slot.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static ImplantCollection GetImplants(ImplantSlots slot)
        {
            return s_implantSlots[(int)slot];
        }

        /// <summary>
        /// Recursively searches the root category and all underlying categories
        /// for the first item with an Id matching the given itemId.
        /// </summary>
        /// <param name="itemId">The id of the item to find.</param>
        /// <returns>The first item which id matches itemId, Null if no such item is found.</returns>
        public static Item GetItemByID(long itemId)
        {
            Item value = null;
            s_itemsByID.TryGetValue(itemId, out value);
            return value;
        }

        /// <summary>
        /// Recursively searches the root category and all underlying categories for the first item with a 
        /// name that exactly matches the given itemName.
        /// </summary>
        /// <param name="itemName">The name of the item to find.</param>
        /// <returns>The first item which name matches itemName, Null if no such item is found.</returns>
        public static Item GetItemByName(string itemName)
        {
            foreach (Item item in s_itemsByID.Values)
            {
                if (item.Name == itemName)
                    return item;
            }
            return null;
        }

        #endregion

    }
}