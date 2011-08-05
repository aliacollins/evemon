using System.Collections.Generic;
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
        internal static void EnsureInitialized()
        {
            if (s_initialized)
                return;

            ReprocessingDatafile datafile = Util.DeserializeDatafile<ReprocessingDatafile>(DatafileConstants.ReprocessingDatafile);

            foreach (SerializableItemMaterials item in datafile.Items)
            {
                List<Material> listOfMaterials = new List<Material>();
                foreach (SerializableMaterialQuantity itemMaterial in item.Materials)
                {
                     listOfMaterials.Add(new Material(itemMaterial));
                }
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
                foreach (List<Material> materials in s_itemMaterialsByID.Values)
                {
                    yield return materials;
                }
            }
        }


        /// <summary>
        /// Gets the materials for the provided itemID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static List<Material> GetItemMaterialsByID(long id)
        {
            EnsureInitialized();
            List<Material> result = null;
            s_itemMaterialsByID.TryGetValue(id, out result);
            return result;
        }
    }
}
