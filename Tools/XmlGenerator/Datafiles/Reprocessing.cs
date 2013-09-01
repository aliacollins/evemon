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
        /// <summary>
        /// Generates the reprocessing datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating reprocessing datafile... ");

            List<SerializableItemMaterials> types = new List<SerializableItemMaterials>();

            foreach (int typeID in Database.InvTypesTable.Where(x => x.Generated).Select(x => x.ID))
            {
                Util.UpdatePercentDone(Database.ReprocessingTotalCount);

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

            // Serialize
            ReprocessingDatafile datafile = new ReprocessingDatafile();
            datafile.Items.AddRange(types);

            Util.DisplayEndTime(startTime);

            Util.SerializeXML(datafile, DatafileConstants.ReprocessingDatafile);
        }
    }
}
