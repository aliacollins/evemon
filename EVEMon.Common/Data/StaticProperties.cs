using System.Collections.Generic;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public static class StaticProperties
    {
        #region Fileds

        private static readonly Dictionary<long, EveProperty> m_propertiesByID = new Dictionary<long, EveProperty>();
        private static readonly Dictionary<string, EveProperty> m_propertiesByName = new Dictionary<string, EveProperty>();
        private static readonly Dictionary<string, EvePropertyCategory> m_categoriesByName = new Dictionary<string, EvePropertyCategory>();

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
                m_categoriesByName[category.Name] = category;

                // Store skills
                foreach (EveProperty property in category)
                {
                    m_propertiesByID[property.ID] = property;
                    m_propertiesByName[property.Name] = property;
                }
            }

            // Visibility in ships browser
            m_propertiesByName["Base Price"].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.CPUOutputPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.PGOutputPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.UpgradeCapacityPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.HiSlotsPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.MedSlotsPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.LowSlotsPropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.DroneCapacityPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.DroneBandwidthPropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.CargoCapacityPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.MassPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.VolumePropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.CapacitorCapacityPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.CapacitorRechargeRatePropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.MaxTargetRangePropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ScanResolutionPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.SignatureRadiusPropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.MaxVelocityPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ShipWarpSpeedPropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.StructureHitpointsPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ShieldHitpointsPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ArmorHitpointsPropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ShieldRechargeRatePropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.ShieldEMResistancePropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ShieldExplosiveResistancePropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ShieldKineticResistancePropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ShieldThermalResistancePropertyID].AlwaysVisibleForShips = true;

            m_propertiesByID[DBConstants.ArmorEMResistancePropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ArmorExplosiveResistancePropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ArmorKineticResistancePropertyID].AlwaysVisibleForShips = true;
            m_propertiesByID[DBConstants.ArmorThermalResistancePropertyID].AlwaysVisibleForShips = true;

            // Hide if default
            m_propertiesByID[DBConstants.LauncherSlotsLeftPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.TurretSlotsLeftPropertyID].HideIfDefault = true;

            m_propertiesByID[DBConstants.ScanRadarStrengthPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.ScanLadarStrengthPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.ScanMagnetometricStrengthPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.ScanGravimetricStrengthPropertyID].HideIfDefault = true;

            m_propertiesByID[DBConstants.HullEMResistancePropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.HullExplosiveResistancePropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.HullKineticResistancePropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.HullThermalResistancePropertyID].HideIfDefault = true;

            m_propertiesByID[DBConstants.EmDamagePropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.ExplosiveDamagePropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.KineticDamagePropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.ThermalDamagePropertyID].HideIfDefault = true;

            m_propertiesByID[DBConstants.CharismaModifierPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.IntelligenceModifierPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.MemoryModifierPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.PerceptionModifierPropertyID].HideIfDefault = true;
            m_propertiesByID[DBConstants.WillpowerModifierPropertyID].HideIfDefault = true;

            m_propertiesByID[DBConstants.MetaLevelPropertyID].HideIfDefault = true;
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
                foreach (EvePropertyCategory category in m_categoriesByName.Values)
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
                foreach (EvePropertyCategory category in m_categoriesByName.Values)
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
            m_propertiesByName.TryGetValue(name, out property);
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
            m_propertiesByID.TryGetValue(id, out property);
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
            m_categoriesByName.TryGetValue(name, out category);
            return category;
        }

        #endregion
    }
}