using System.Collections.Generic;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public static class StaticProperties
    {
        #region Fileds

        private static readonly Dictionary<long, EveProperty> s_propertiesByID = new Dictionary<long, EveProperty>();
        private static readonly Dictionary<string, EveProperty> s_propertiesByName = new Dictionary<string, EveProperty>();
        private static readonly Dictionary<string, EvePropertyCategory> s_categoriesByName = new Dictionary<string, EvePropertyCategory>();

        #endregion


        #region Initilizer

        /// <summary>
        /// Initialize static properties.
        /// </summary>
        internal static void Load()
        {
            PropertiesDatafile datafile = Util.DeserializeDatafile<PropertiesDatafile>(DatafileConstants.PropertiesDatafile);

            // Fetch deserialized data
            foreach (SerializablePropertyCategory srcCategory in datafile.Categories)
            {
                EvePropertyCategory category = new EvePropertyCategory(srcCategory);
                s_categoriesByName[category.Name] = category;

                // Store skills
                foreach (EveProperty property in category)
                {
                    s_propertiesByID[property.ID] = property;
                    s_propertiesByName[property.Name] = property;
                }
            }

            // Visibility in ships browser
            s_propertiesByID[DBConstants.CPUOutputPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.PGOutputPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.UpgradeCapacityPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.HiSlotsPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.MedSlotsPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.LowSlotsPropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.DroneCapacityPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.DroneBandwidthPropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.CargoCapacityPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.MassPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.VolumePropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.CapacitorCapacityPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.CapacitorRechargeRatePropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.MaxTargetRangePropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ScanResolutionPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.SignatureRadiusPropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.MaxVelocityPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ShipWarpSpeedPropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.StructureHitpointsPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ShieldHitpointsPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ArmorHitpointsPropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ShieldRechargeRatePropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.ShieldEMResistancePropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ShieldExplosiveResistancePropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ShieldKineticResistancePropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ShieldThermalResistancePropertyID].AlwaysVisibleForShips = true;

            s_propertiesByID[DBConstants.ArmorEMResistancePropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ArmorExplosiveResistancePropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ArmorKineticResistancePropertyID].AlwaysVisibleForShips = true;
            s_propertiesByID[DBConstants.ArmorThermalResistancePropertyID].AlwaysVisibleForShips = true;

            // Hide if default
            s_propertiesByID[DBConstants.LauncherSlotsLeftPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.TurretSlotsLeftPropertyID].HideIfDefault = true;

            s_propertiesByID[DBConstants.TurretHardPointModifierPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.LauncherHardPointModifierPropertyID].HideIfDefault = true;

            s_propertiesByID[DBConstants.HiSlotModifierPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.MedSlotModifierPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.LowSlotModifierPropertyID].HideIfDefault = true;

            s_propertiesByID[DBConstants.ScanRadarStrengthPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.ScanLadarStrengthPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.ScanMagnetometricStrengthPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.ScanGravimetricStrengthPropertyID].HideIfDefault = true;

            s_propertiesByID[DBConstants.HullEMResistancePropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.HullExplosiveResistancePropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.HullKineticResistancePropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.HullThermalResistancePropertyID].HideIfDefault = true;

            s_propertiesByID[DBConstants.EmDamagePropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.ExplosiveDamagePropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.KineticDamagePropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.ThermalDamagePropertyID].HideIfDefault = true;

            s_propertiesByID[DBConstants.CharismaModifierPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.IntelligenceModifierPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.MemoryModifierPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.PerceptionModifierPropertyID].HideIfDefault = true;
            s_propertiesByID[DBConstants.WillpowerModifierPropertyID].HideIfDefault = true;

            s_propertiesByID[DBConstants.MetaLevelPropertyID].HideIfDefault = true;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the list of properties categories.
        /// </summary>
        public static IEnumerable<EvePropertyCategory> AllCategories
        {
            get
            {
                foreach (EvePropertyCategory category in s_categoriesByName.Values)
                {
                    yield return category;
                }
            }
        }

        /// <summary>
        /// Gets the list of properties.
        /// </summary>
        public static IEnumerable<EveProperty> AllProperties
        {
            get
            {
                foreach (EvePropertyCategory category in s_categoriesByName.Values)
                {
                    foreach (EveProperty property in category)
                    {
                        yield return property;
                    }
                }
            }
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets a property by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EveProperty GetPropertyByName(string name)
        {
            EveProperty property = null;
            s_propertiesByName.TryGetValue(name, out property);
            return property;
        }

        /// <summary>
        /// Gets a property by its identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EveProperty GetPropertyById(int id)
        {
            EveProperty property = null;
            s_propertiesByID.TryGetValue(id, out property);
            return property;
        }

        /// <summary>
        /// Gets a group by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EvePropertyCategory GetCategoryByName(string name)
        {
            EvePropertyCategory category = null;
            s_categoriesByName.TryGetValue(name, out category);
            return category;
        }

        #endregion
    }
}