using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Stores all the data regarding reprocessing.
    /// </summary>
    public static class StaticReprocessing
    {
        private static bool s_initialized;
        private static readonly Dictionary<long, List<Material>> s_itemMaterialsByID = new Dictionary<long, List<Material>>();


        /// <summary>
        /// Ensures the reprocessing informations have been intialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (s_initialized)
                return;

            ReprocessingDatafile datafile = Util.DeserializeDatafile<ReprocessingDatafile>(DatafileConstants.ReprocessingDatafile);

            foreach (SerializableItemMaterials item in datafile.Items)
            {
                List<Material> listOfMaterials = item.Materials.Select(itemMaterial => new Material(itemMaterial)).ToList();
                s_itemMaterialsByID[item.ID] = listOfMaterials;
            }

            // Mark as initialized
            s_initialized = true;
        }

        /// <summary>
        /// Gets an enumeration of all the reprocessing materials.
        /// </summary>
        public static IEnumerable<List<Material>> AllReprocessingMaterials
        {
            get
            {
                EnsureInitialized();
                return s_itemMaterialsByID.Values;
            }
        }


        /// <summary>
        /// Gets the materials for the provided itemID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static IEnumerable<Material> GetItemMaterialsByID(long id)
        {
            EnsureInitialized();
            List<Material> result;
            s_itemMaterialsByID.TryGetValue(id, out result);
            return result;
        }
    }
}
