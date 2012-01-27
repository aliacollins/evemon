using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.XmlGenerator.Datafiles
{
    public static class Reprocessing
    {
        private const int ReprocessGenTotal = 11462;

        private static DateTime s_startTime;

        /// <summary>
        /// Generates the reprocessing datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            s_startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating reprocessing datafile... ");

            List<SerializableItemMaterials> types = new List<SerializableItemMaterials>();

            foreach (int typeID in Database.InvTypeTable.Where(x => x.Generated).Select(x => x.ID))
            {
                Util.UpdatePercentDone(ReprocessGenTotal);

                IEnumerable<SerializableMaterialQuantity> materials = Database.InvTypeMaterialsTable.Where(
                    x => x.TypeID == typeID).Select(
                        srcMaterial => new SerializableMaterialQuantity
                                           {
                                               ID = srcMaterial.MaterialTypeID,
                                               Quantity = srcMaterial.Quantity
                                           });

                if (!materials.Any())
                    continue;

                SerializableItemMaterials itemMaterials = new SerializableItemMaterials { ID = typeID };
                itemMaterials.Materials.AddRange(materials.OrderBy(x => x.ID));
                types.Add(itemMaterials);
            }

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}", DateTime.Now.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            ReprocessingDatafile datafile = new ReprocessingDatafile();
            datafile.Items.AddRange(types);
            Util.SerializeXML(datafile, DatafileConstants.ReprocessingDatafile);
        }
    }
}
