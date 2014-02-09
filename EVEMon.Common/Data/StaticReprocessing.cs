using System.Collections.Generic;
using System.IO;
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
        private static readonly Dictionary<int, MaterialCollection> s_itemMaterialsByID = new Dictionary<int, MaterialCollection>();

        /// <summary>
        /// Ensures the reprocessing informations have been intialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (s_initialized)
                return;

            if (!File.Exists(Datafile.GetFullPath(DatafileConstants.ReprocessingDatafile)))
                return;

            ReprocessingDatafile datafile = Util.DeserializeDatafile<ReprocessingDatafile>(DatafileConstants.ReprocessingDatafile);

            foreach (SerializableItemMaterials item in datafile.Items)
            {
                MaterialCollection materials = new MaterialCollection(item.Materials.Select(itemMaterial => new Material(itemMaterial)));
                s_itemMaterialsByID[item.ID] = materials;
            }

            // Mark as initialized
            s_initialized = true;
        }

        /// <summary>
        /// Gets an enumeration of all the reprocessing materials.
        /// </summary>
        public static IEnumerable<MaterialCollection> AllReprocessingMaterials
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
        public static IEnumerable<Material> GetItemMaterialsByID(int id)
        {
            EnsureInitialized();
            MaterialCollection result;
            s_itemMaterialsByID.TryGetValue(id, out result);
            return result;
        }
    }
}