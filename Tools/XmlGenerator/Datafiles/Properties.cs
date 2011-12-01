using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator.Datafiles
{
    public static class Properties
    {
        private const int PropGenTotal = 1623;

        private static DateTime s_startTime;

        /// <summary>
        /// Gets or sets the base price property ID.
        /// </summary>
        /// <value>The base price property ID.</value>
        internal static long PropBasePriceID { get; private set; }

        /// <summary>
        /// Gets or sets the packaged volume property ID.
        /// </summary>
        /// <value>The packaged volume property ID.</value>
        internal static long PropPackagedVolumeID { get; private set; }

        /// <summary>
        /// Generate the properties datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            s_startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating properties datafile... ");

            ConfigureNullCategoryProperties();

            IEnumerable<SerializablePropertyCategory> categories = ExportAttributeCategories();

            // Sort groups
            string[] orderedGroupNames = new[]
                                             {
                                                 "General", "Fitting", "Drones", "Structure", "Armor", "Shield", "Capacitor",
                                                 "Targeting", "Propulsion", "Miscellaneous", "NULL"
                                             };

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}", DateTime.Now.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            PropertiesDatafile datafile = new PropertiesDatafile();
            datafile.Categories.AddRange(categories.OrderBy(x => orderedGroupNames.IndexOf(String.Intern(x.Name))));

            Util.SerializeXML(datafile, DatafileConstants.PropertiesDatafile);
        }

        /// <summary>
        /// Configures the null category properties.
        /// </summary>
        private static void ConfigureNullCategoryProperties()
        {
            // Set attributes with CategoryID 'NULL" to NULL category
            foreach (DgmAttributeTypes attribute in Database.DgmAttributeTypesTable.Where(x => x.CategoryID == null))
            {
                attribute.CategoryID = (attribute.Published
                                            ? DBConstants.MiscellaneousAttributeCategoryID
                                            : DBConstants.NULLAtributeCategoryID);
            }

            // Change some display names and default values
            Database.DgmAttributeTypesTable[DBConstants.StructureHitpointsPropertyID].DisplayName = "Structure HP";
            Database.DgmAttributeTypesTable[DBConstants.ShieldHitpointsPropertyID].DisplayName = "Shield HP";
            Database.DgmAttributeTypesTable[DBConstants.ArmorHitpointsPropertyID].DisplayName = "Armor HP";
            Database.DgmAttributeTypesTable[DBConstants.CargoCapacityPropertyID].DisplayName = "Cargo Capacity";
            Database.DgmAttributeTypesTable[DBConstants.CPUOutputPropertyID].DisplayName = "CPU";
            Database.DgmAttributeTypesTable[DBConstants.PGOutputPropertyID].DisplayName = "Powergrid";

            // Shield
            Database.DgmAttributeTypesTable[DBConstants.ShieldEMResistancePropertyID].DisplayName = "EM Resistance";
            Database.DgmAttributeTypesTable[DBConstants.ShieldExplosiveResistancePropertyID].DisplayName = "Explosive Resistance";
            Database.DgmAttributeTypesTable[DBConstants.ShieldKineticResistancePropertyID].DisplayName = "Kinetic Resistance";
            Database.DgmAttributeTypesTable[DBConstants.ShieldThermalResistancePropertyID].DisplayName = "Thermal Resistance";

            // Armor
            Database.DgmAttributeTypesTable[DBConstants.ArmorEMResistancePropertyID].DisplayName = "EM Resistance";
            Database.DgmAttributeTypesTable[DBConstants.ArmorExplosiveResistancePropertyID].DisplayName = "Explosive Resistance";
            Database.DgmAttributeTypesTable[DBConstants.ArmorKineticResistancePropertyID].DisplayName = "Kinetic Resistance";
            Database.DgmAttributeTypesTable[DBConstants.ArmorThermalResistancePropertyID].DisplayName = "Thermal Resistance";

            // Hull
            Database.DgmAttributeTypesTable[DBConstants.HullEMResistancePropertyID].DisplayName = "EM Resistance";
            Database.DgmAttributeTypesTable[DBConstants.HullExplosiveResistancePropertyID].DisplayName = "Explosive Resistance";
            Database.DgmAttributeTypesTable[DBConstants.HullKineticResistancePropertyID].DisplayName = "Kinetic Resistance";
            Database.DgmAttributeTypesTable[DBConstants.HullThermalResistancePropertyID].DisplayName = "Thermal Resistance";

            // Items attribute
            Database.DgmAttributeTypesTable[DBConstants.CapacitorNeedPropertyID].DisplayName = "Activation cost";
            Database.DgmAttributeTypesTable[DBConstants.PGNeedPropertyID].DisplayName = "Powergrid usage";
            Database.DgmAttributeTypesTable[DBConstants.ShieldBonusPropertyID].DisplayName = "Shield Bonus";
            Database.DgmAttributeTypesTable[DBConstants.ShieldTransferRangePropertyID].DisplayName = "Shield Transfer Range";
            Database.DgmAttributeTypesTable[DBConstants.ExplosiveDamagePropertyID].DisplayName = "Explosive damage";
            Database.DgmAttributeTypesTable[DBConstants.CPUOutputBonusPropertyID].DisplayName = "CPU Output Bonus";
            Database.DgmAttributeTypesTable[DBConstants.CPUPenaltyPercentPropertyID].DisplayName = "CPU Penalty";

            // Changing the categoryID for some attributes 
            Database.DgmAttributeTypesTable[DBConstants.UpgradeCapacityPropertyID].CategoryID =
                DBConstants.FittingAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.RigSizePropertyID].CategoryID = DBConstants.FittingAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.MaxSubSystemsPropertyID].CategoryID =
                DBConstants.FittingAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.FitsToShipTypePropertyID].CategoryID =
                DBConstants.FittingAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.ShipMaintenanceBayCapacityPropertyID].CategoryID =
                DBConstants.StructureAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.TurretHardPointModifierPropertyID].CategoryID =
                DBConstants.StructureAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.LauncherHardPointModifierPropertyID].CategoryID =
                DBConstants.StructureAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.HiSlotModifierPropertyID].CategoryID =
                DBConstants.StructureAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.MedSlotModifierPropertyID].CategoryID =
                DBConstants.StructureAtributeCategoryID;
            Database.DgmAttributeTypesTable[DBConstants.LowSlotModifierPropertyID].CategoryID =
                DBConstants.StructureAtributeCategoryID;

            // Changing HigherIsBetter to false (CCP has this wrong?)
            Database.DgmAttributeTypesTable[DBConstants.PGNeedPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.CPUNeedPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.VolumePropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.AgilityPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.MassPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.CapacitorNeedPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.CapacitorRechargeRatePropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.CapacitorRechargeRateMultiplierPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.ShieldRechargeRatePropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.SignatureRadiusPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.CloakingTargetingDelayPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.CPUPenaltyPercentPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.UpgradeCostPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.DroneBandwidthUsedPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.AITargetSwitchTimerPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.DurationPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.AnchoringDelayPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.UnanchoringDelayPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.OnliningDelayPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.IceHarvestCycleBonusPropertyID].HigherIsBetter = false;
            Database.DgmAttributeTypesTable[DBConstants.ModuleReactivationDelayPropertyID].HigherIsBetter = false;
        }

        /// <summary>
        /// Exports the attribute categories.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<SerializablePropertyCategory> ExportAttributeCategories()
        {
            List<SerializablePropertyCategory> categories = new List<SerializablePropertyCategory>();

            // Export attribute categories
            long newCategoryID = 0;
            long newPropID = 0;
            List<SerializableProperty> gProperties = new List<SerializableProperty>();
            List<SerializableProperty> pProperties = new List<SerializableProperty>();
            foreach (DgmAttributeCategory srcCategory in Database.DgmAttributeCategoriesTable)
            {
                List<SerializableProperty> properties = new List<SerializableProperty>();
                SerializablePropertyCategory category = new SerializablePropertyCategory
                                                            {
                                                                ID = srcCategory.ID,
                                                                Description = srcCategory.Description,
                                                                Name = srcCategory.Name
                                                            };
                categories.Add(category);

                // Export attributes
                foreach (DgmAttributeTypes srcProp in Database.DgmAttributeTypesTable.Where(x => x.CategoryID == srcCategory.ID))
                {
                    Util.UpdatePercentDone(PropGenTotal);

                    SerializableProperty prop = new SerializableProperty();
                    properties.Add(prop);

                    prop.ID = srcProp.ID;
                    prop.DefaultValue = srcProp.DefaultValue;
                    prop.Description = srcProp.Description;
                    prop.HigherIsBetter = srcProp.HigherIsBetter;
                    prop.Name = !String.IsNullOrEmpty(srcProp.DisplayName)
                                    ? srcProp.DisplayName
                                    : !String.IsNullOrEmpty(srcProp.Name)
                                          ? srcProp.Name
                                          : String.Empty;

                    // Unit
                    if (srcProp.UnitID == null)
                        prop.Unit = String.Empty;
                    else
                    {
                        prop.Unit = Database.EveUnitsTable[srcProp.UnitID.Value].DisplayName;
                        prop.UnitID = srcProp.UnitID.Value;
                    }

                    // Ship warp speed unit
                    if (srcProp.ID == DBConstants.ShipWarpSpeedPropertyID)
                        prop.Unit = "AU/S";

                    // Icon
                    prop.Icon = (srcProp.IconID.HasValue ? Database.EveIconsTable[srcProp.IconID.Value].Icon : String.Empty);

                    // Reordering some properties
                    ReorderProperties(pProperties, gProperties, prop, srcProp, properties);

                    // New property ID
                    newPropID = Math.Max(newPropID, srcProp.ID);
                }

                // Add EVEMon custom properties (Packaged Volume)
                if (srcCategory.ID == DBConstants.StructureAtributeCategoryID)
                {
                    SerializableProperty pvProp = new SerializableProperty
                                                      {
                                                          Name = "Packaged Volume",
                                                          Unit = "m3",
                                                          Icon = "02_09",
                                                          DefaultValue = "0",
                                                          Description = "The packaged volume of a ship.",
                                                          UnitID = 9
                                                      };
                    properties.Insert(4, pvProp);
                }

                category.Properties.AddRange(properties);

                // New category ID
                newCategoryID = Math.Max(newCategoryID, srcCategory.ID);
            }

            // Set packaged volume property ID
            PropPackagedVolumeID = ++newPropID;
            categories[(int)DBConstants.StructureAtributeCategoryID - 1].Properties[4].ID = PropPackagedVolumeID;

            // Add EVEMon custom properties (Base Price)
            PropBasePriceID = ++newPropID;
            SerializableProperty bpProp = new SerializableProperty
                                              {
                                                  ID = PropBasePriceID,
                                                  Name = "Base Price",
                                                  Unit = "ISK",
                                                  DefaultValue = "0",
                                                  Description = "The price from NPC vendors (does not mean there is any).",
                                                  UnitID = 133
                                              };
            gProperties.Insert(0, bpProp);

            // We insert custom categories
            SerializablePropertyCategory general = new SerializablePropertyCategory
                                                       {
                                                           ID = ++newCategoryID,
                                                           Name = "General",
                                                           Description = "General informations"
                                                       };
            categories.Insert(0, general);

            SerializablePropertyCategory propulsion = new SerializablePropertyCategory
                                                          {
                                                              ID = ++newCategoryID,
                                                              Name = "Propulsion",
                                                              Description = "Navigation attributes for ships"
                                                          };
            categories.Insert(0, propulsion);

            // Add properties to custom categories
            general.Properties.AddRange(gProperties);
            propulsion.Properties.AddRange(pProperties);

            return categories;
        }

        /// <summary>
        /// Reorders the properties.
        /// </summary>
        /// <param name="pProperties">The p properties.</param>
        /// <param name="gProperties">The g properties.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="srcProp">The SRC prop.</param>
        /// <param name="properties">The properties.</param>
        private static void ReorderProperties(IList<SerializableProperty> pProperties, IList<SerializableProperty> gProperties,
                                              SerializableProperty prop, IHasID srcProp, IList<SerializableProperty> properties)
        {
            int index = properties.IndexOf(prop);
            switch (srcProp.ID)
            {
                case DBConstants.StructureHitpointsPropertyID:
                    properties.Insert(0, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.MaxVelocityPropertyID:
                    pProperties.Insert(0, prop);
                    properties.RemoveAt(index);
                    break;
                case DBConstants.CargoCapacityPropertyID:
                    properties.Insert(1, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.CPUOutputPropertyID:
                    properties.Insert(0, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.AgilityPropertyID:
                    properties.Insert(3, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.VolumePropertyID:
                    properties.Insert(3, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.TechLevelPropertyID:
                    gProperties.Insert(0, prop);
                    properties.RemoveAt(index);
                    break;
                case DBConstants.ShieldRechargeRatePropertyID:
                    properties.Insert(6, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.CapacitorCapacityPropertyID:
                    properties.Insert(0, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.ScanResolutionPropertyID:
                    properties.Insert(4, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.MetaLevelPropertyID:
                    gProperties.Insert(1, prop);
                    properties.RemoveAt(index);
                    break;
                case DBConstants.HullEMResistancePropertyID:
                    properties.Insert(5, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.HullExplosiveResistancePropertyID:
                    properties.Insert(6, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.HullKineticResistancePropertyID:
                    properties.Insert(7, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.HullThermalResistancePropertyID:
                    properties.Insert(8, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.UpgradeCapacityPropertyID:
                    properties.Insert(2, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.RigSlotsPropertyID:
                    properties.Insert(10, prop);
                    properties.RemoveAt(index + 1);
                    break;
                case DBConstants.ShipWarpSpeedPropertyID:
                    pProperties.Insert(1, prop);
                    properties.RemoveAt(index);
                    break;
                case DBConstants.RigSizePropertyID:
                    properties.Insert(11, prop);
                    properties.RemoveAt(index + 1);
                    break;
            }
        }
    }
}
