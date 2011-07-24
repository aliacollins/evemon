using System.Collections.Generic;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents the list of all static blueprints.
    /// </summary>
    public static class StaticBlueprints
    {
        private static readonly Dictionary<long, Blueprint> s_blueprintsByID = new Dictionary<long, Blueprint>();
        private static readonly Dictionary<int, BlueprintActivity> s_activityByID = new Dictionary<int, BlueprintActivity>();
        
        #region Public Properties

        /// <summary>
        /// Gets the root category, containing all the top level categories
        /// </summary>
        public static BlueprintMarketGroupCollection BlueprintMarketGroups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of all the blueprints in this category and its descendants.
        /// </summary>
        public static IEnumerable<Item> AllBlueprints
        {
            get
            {
                foreach (Blueprint blueprint in s_blueprintsByID.Values)
                {
                    yield return blueprint;
                }
            }
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Recursively searches the root category and all underlying categories
        /// for the first blueprint with an Id matching the given blueprintId.
        /// </summary>
        /// <param name="itemId">The id of the blueprint to find.</param>
        /// <returns>The first blueprint which id matches blueprintId, Null if no such blueprint is found.</returns>
        public static Blueprint GetBlueprintByID(long blueprintId)
        {
            Blueprint value = null;
            s_blueprintsByID.TryGetValue(blueprintId, out value);
            return value;
        }

        /// <summary>
        /// Recursively searches the root category and all underlying categories
        /// for the first blueprint with a name matching the given blueprintId.
        /// </summary>
        /// <param name="blueprintName">The name of the blueprint to find.</param>
        /// <returns>The first blueprint which name matches blueprintName, Null if no such blueprint is found.</returns>
        public static Item GetBlueprintByName(string blueprintName)
        {
            foreach (Blueprint item in s_blueprintsByID.Values)
            {
                if (item.Name == blueprintName)
                    return item;
            }
            return null;
        }

        #endregion


        #region Initializers

        /// <summary>
        /// Initialize static blueprints.
        /// </summary>
        internal static void Load()
        {
            if (BlueprintMarketGroups != null)
                return;
            
            BlueprintsDatafile datafile = Util.DeserializeDatafile<BlueprintsDatafile>(DatafileConstants.BlueprintsDatafile);

            BlueprintMarketGroups = new BlueprintMarketGroupCollection(null, datafile.Groups);

            foreach (BlueprintMarketGroup srcGroup in BlueprintMarketGroups)
            {
                InitializeDictionaries(srcGroup);
            }
        }

        /// <summary>
        /// Recursively collect the blueprints within all groups and stores them in the dictionaries.
        /// </summary>
        /// <param name="group"></param>
        private static void InitializeDictionaries(BlueprintMarketGroup group)
        {
            foreach (Blueprint blueprint in group.Blueprints)
            {
                s_blueprintsByID[blueprint.ID] = blueprint;
            }

            foreach (BlueprintMarketGroup childGroup in group.SubGroups)
            {
                InitializeDictionaries(childGroup);
            }
        }

        #endregion

    }
}
