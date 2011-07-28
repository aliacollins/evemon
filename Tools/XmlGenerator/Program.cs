using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

using EVEMon.Common;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator
{
    internal class Program
    {
        private static string s_text = String.Empty;
        private static double s_counter;
        private static double s_tablesCount;
        private static double s_totalTablesCount = 29;

        private static int s_percentOld;
        private static int s_propGenTotal = 1550;
        private static int s_itemGenTotal = 8751;
        private static int s_skillGenTotal = 454;
        private static int s_certGenTotal = 4272;
        private static int s_blueprintGenTotal = 3953;
        private static int s_geoGen = 5219;
        private static int s_geoGenTotal = 19501;
        private static int s_reprocessGenTotal = 10074;

        private static DateTime s_startTime;
        private static DateTime s_endTime;

        private static int s_propBasePriceID;
        private static int s_propPackagedVolumeID;
        private static List<InvMarketGroup> s_injectedMarketGroups;

        private static Bag<AgtAgents> s_agents;
        private static Bag<AgtAgentTypes> s_agentTypes;
        private static List<AgtConfig> s_agentConfig;
        private static Bag<AgtResearchAgents> s_researchAgents;
        private static Bag<CrpNPCDivisions> s_npcDivisions;
        private static Bag<EveUnit> s_units;
        private static Bag<EveNames> s_names;
        private static Bag<EveIcons> s_icons;
        private static Bag<DgmAttributeTypes> s_attributeTypes;
        private static Bag<DgmAttributeCategory> s_attributeCategories;
        private static RelationSet<DgmTypeEffect> s_typeEffects;
        private static RelationSet<DgmTypeAttribute> s_typeAttributes;
        private static Bag<MapRegion> s_regions;
        private static Bag<MapConstellation> s_constellations;
        private static Bag<MapSolarSystem> s_solarSystems;
        private static Bag<StaStation> s_stations;
        private static List<MapSolarSystemJump> s_jumps;
        private static Bag<InvBlueprintTypes> s_blueprintTypes;
        private static Bag<InvMarketGroup> s_marketGroups;
        private static Bag<InvGroup> s_groups;
        private static Bag<InvType> s_types;
        private static RelationSet<InvMetaType> s_metaTypes;
        private static List<InvTypeMaterials> s_typeMaterials;
        private static List<RamTypeRequirements> s_typeRequirements;
        private static Bag<CrtCategories> s_crtCategories;
        private static Bag<CrtClasses> s_crtClasses;
        private static Bag<CrtCertificates> s_certificates;
        private static Bag<CrtRecommendations> s_crtRecommendations;
        private static Bag<CrtRelationships> s_crtRelationships;

        #region Initilization

        private static void Main(string[] args)
        {
            // Setting a standard format for the generated files
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            // Data dumps are available from CCP
            Console.Write(@"Loading Data from SQL Server... ");

            #region Read Tables From Database

            // Read tables from database
            s_agents = Database.Agents();
            UpdateProgress();
            s_agentTypes = Database.AgentTypes();
            UpdateProgress();
            s_agentConfig = Database.AgentConfig();
            UpdateProgress();
            s_researchAgents = Database.ResearchAgents();
            UpdateProgress();
            s_npcDivisions = Database.NPCDivisions();
            UpdateProgress();
            s_units = Database.Units();
            UpdateProgress();
            s_names = Database.Names();
            UpdateProgress();
            s_icons = Database.Icons();
            UpdateProgress();
            s_attributeTypes = Database.Attributes();
            UpdateProgress();
            s_attributeCategories = Database.AttributeCategories();
            UpdateProgress();
            s_regions = Database.Regions();
            UpdateProgress();
            s_constellations = Database.Constellations();
            UpdateProgress();
            s_solarSystems = Database.Solarsystems();
            UpdateProgress();
            s_stations = Database.Stations();
            UpdateProgress();
            s_jumps = Database.Jumps();
            UpdateProgress();
            s_typeAttributes = Database.TypeAttributes();
            UpdateProgress();
            s_blueprintTypes = Database.BlueprintTypes();
            UpdateProgress();
            s_marketGroups = Database.MarketGroups();
            UpdateProgress();
            s_groups = Database.Groups();
            UpdateProgress();
            s_metaTypes = Database.MetaTypes();
            UpdateProgress();
            s_typeEffects = Database.TypeEffects();
            UpdateProgress();
            s_types = Database.Types();
            UpdateProgress();
            s_typeMaterials = Database.TypeMaterials();
            UpdateProgress();
            s_typeRequirements = Database.TypeRequirements();
            UpdateProgress();
            s_crtCategories = Database.CertificateCategories();
            UpdateProgress();
            s_crtClasses = Database.CertificateClasses();
            UpdateProgress();
            s_certificates = Database.Certificates();
            UpdateProgress();
            s_crtRecommendations = Database.CertificateRecommendations();
            UpdateProgress();
            s_crtRelationships = Database.CertificateRelationships();
            UpdateProgress();

            Console.WriteLine();
            Console.WriteLine();

            #endregion

            // Generate datafiles
            Console.WriteLine(@"Datafile Generating In Progress");
            Console.WriteLine();

            GenerateProperties();
            GenerateItems(); // Requires GenerateProperties()
            GenerateSkills();
            GenerateCertificates();
            GenerateBlueprints();
            GenerateGeography();
            GenerateReprocessing(); // Requires GenerateItems()

            GenerateMD5Sums();

            Console.WriteLine(@"Done");
            Console.ReadLine();
        }

        #endregion

        #region Properties Datafile

        /// <summary>
        /// Generate the properties datafile.
        /// </summary>
        private static void GenerateProperties()
        {
            Console.WriteLine();
            Console.Write(@"Generated properties datafile... ");

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
            s_startTime = DateTime.Now;

            int newID = 0;

            // Change some display names and default values
            s_attributeTypes[DBConstants.StructureHitpointsPropertyID].DisplayName = "Structure HP";
            s_attributeTypes[DBConstants.ShieldHitpointsPropertyID].DisplayName = "Shield HP";
            s_attributeTypes[DBConstants.ArmorHitpointsPropertyID].DisplayName = "Armor HP";

            s_attributeTypes[DBConstants.CargoCapacityPropertyID].DisplayName = "Cargo Capacity";
            s_attributeTypes[DBConstants.CPUOutputPropertyID].DisplayName = "CPU";
            s_attributeTypes[DBConstants.PGOutputPropertyID].DisplayName = "Powergrid";

            // Shield
            s_attributeTypes[DBConstants.ShieldEMResistancePropertyID].DisplayName = "EM Resistance";
            s_attributeTypes[DBConstants.ShieldExplosiveResistancePropertyID].DisplayName = "Explosive Resistance";
            s_attributeTypes[DBConstants.ShieldKineticResistancePropertyID].DisplayName = "Kinetic Resistance";
            s_attributeTypes[DBConstants.ShieldThermalResistancePropertyID].DisplayName = "Thermal Resistance";

            // Armor
            s_attributeTypes[DBConstants.ArmorEMResistancePropertyID].DisplayName = "EM Resistance";
            s_attributeTypes[DBConstants.ArmorExplosiveResistancePropertyID].DisplayName = "Explosive Resistance";
            s_attributeTypes[DBConstants.ArmorKineticResistancePropertyID].DisplayName = "Kinetic Resistance";
            s_attributeTypes[DBConstants.ArmorThermalResistancePropertyID].DisplayName = "Thermal Resistance";

            // Hull
            s_attributeTypes[DBConstants.HullEMResistancePropertyID].DisplayName = "EM Resistance";
            s_attributeTypes[DBConstants.HullExplosiveResistancePropertyID].DisplayName = "Explosive Resistance";
            s_attributeTypes[DBConstants.HullKineticResistancePropertyID].DisplayName = "Kinetic Resistance";
            s_attributeTypes[DBConstants.HullThermalResistancePropertyID].DisplayName = "Thermal Resistance";

            // Items attribute
            s_attributeTypes[DBConstants.CapacitorNeedPropertyID].DisplayName = "Activation cost";
            s_attributeTypes[DBConstants.PGNeedPropertyID].DisplayName = "Powergrid usage";
            s_attributeTypes[DBConstants.ShieldBonusPropertyID].DisplayName = "Shield Bonus";
            s_attributeTypes[DBConstants.ShieldTransferRangePropertyID].DisplayName = "Shield Transfer Range";
            s_attributeTypes[DBConstants.ExplosiveDamagePropertyID].DisplayName = "Explosive damage";
            s_attributeTypes[DBConstants.CPUOutputBonusPropertyID].DisplayName = "CPU Output Bonus";
            s_attributeTypes[DBConstants.CPUPenaltyPercentPropertyID].DisplayName = "CPU Penalty";

            // Changing the categoryID for some attributes 
            s_attributeTypes[DBConstants.UpgradeCapacityPropertyID].CategoryID = 1;
            s_attributeTypes[DBConstants.RigSizePropertyID].CategoryID = 1;
            s_attributeTypes[DBConstants.ShipMaintenanceBayCapacityPropertyID].CategoryID = 4;
            s_attributeTypes[DBConstants.MetaGroupPropertyID].CategoryID = 9;

            // Changing HigherIsBetter to false (CCP has this wrong?)
            s_attributeTypes[DBConstants.PGNeedPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.CPUNeedPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.VolumePropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.AgilityPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.MassPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.CapacitorNeedPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.CapacitorRechargeRatePropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.CapacitorRechargeRateMultiplierPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.ShieldRechargeRatePropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.SignatureRadiusPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.CloakingTargetingDelayPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.CPUPenaltyPercentPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.UpgradeCostPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.DroneBandwidthUsedPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.AITargetSwitchTimerPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.DurationPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.AnchoringDelayPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.UnanchoringDelayPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.OnliningDelayPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.IceHarvestCycleBonusPropertyID].HigherIsBetter = false;
            s_attributeTypes[DBConstants.ModuleReactivationDelayPropertyID].HigherIsBetter = false;


            // Export attribute categories
            List<SerializablePropertyCategory> categories = new List<SerializablePropertyCategory>();

            // We insert custom categories
            SerializablePropertyCategory general = new SerializablePropertyCategory
            {
                Name = "General",
                Description = "General informations"
            };
            SerializablePropertyCategory propulsion = new SerializablePropertyCategory
            {
                Name = "Propulsion",
                Description = "Navigation attributes for ships"
            };
            List<SerializableProperty> gProperties = new List<SerializableProperty>();
            List<SerializableProperty> pProperties = new List<SerializableProperty>();
            categories.Insert(0, general);
            categories.Insert(0, propulsion);

            foreach (DgmAttributeCategory srcCategory in s_attributeCategories)
            {
                SerializablePropertyCategory category = new SerializablePropertyCategory();
                categories.Add(category);

                category.Description = srcCategory.Description;
                category.Name = srcCategory.Name;

                // Export attributes
                List<SerializableProperty> properties = new List<SerializableProperty>();

                foreach (DgmAttributeTypes srcProp in s_attributeTypes.Where(x => x.CategoryID == srcCategory.ID))
                {
                    UpdatePercentDone(s_propGenTotal);

                    SerializableProperty prop = new SerializableProperty();
                    properties.Add(prop);

                    prop.DefaultValue = srcProp.DefaultValue;
                    prop.Description = srcProp.Description;
                    prop.HigherIsBetter = srcProp.HigherIsBetter;
                    prop.Name = (String.IsNullOrEmpty(srcProp.DisplayName) ? srcProp.Name : srcProp.DisplayName);
                    prop.ID = srcProp.ID;

                    // Unit
                    if (srcProp.UnitID == null)
                    {
                        prop.Unit = String.Empty;
                    }
                    else
                    {
                        prop.Unit = s_units[srcProp.UnitID.Value].DisplayName;
                        prop.UnitID = srcProp.UnitID.Value;
                    }

                    // Ship warp speed unit
                    if (srcProp.ID == DBConstants.ShipWarpSpeedPropertyID)
                        prop.Unit = "AU/S";

                    // Icon
                    prop.Icon = (srcProp.IconID.HasValue ? s_icons[srcProp.IconID.Value].Icon : String.Empty);

                    // Reordering some properties
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
                        default:
                            break;
                    }

                    // New ID
                    newID = Math.Max(newID, srcProp.ID);
                }

                // Add EVEMon custom properties (Packaged Volume)
                if (srcCategory.ID == DBConstants.StructureAtributeCategoryID)
                {
                    SerializableProperty pvProp = new SerializableProperty();
                    properties.Insert(4, pvProp);
                    pvProp.Name = "Packaged Volume";
                    pvProp.Unit = "m3";
                    pvProp.Icon = "02_09";
                    pvProp.DefaultValue = "0";
                    pvProp.Description = "The packaged volume of a ship.";
                    pvProp.UnitID = 9;
                }

                category.Properties = properties.ToArray();
            }

            // Set packaged volume property ID
            s_propPackagedVolumeID = ++newID;
            categories[5].Properties[4].ID = s_propPackagedVolumeID;

            // Add EVEMon custom properties (Base Price)
            SerializableProperty bpProp = new SerializableProperty();
            s_propBasePriceID = ++newID;
            gProperties.Insert(0, bpProp);
            bpProp.ID = s_propBasePriceID;
            bpProp.Name = "Base Price";
            bpProp.Unit = "ISK";
            bpProp.DefaultValue = "0";
            bpProp.Description = "The price from NPC vendors (does not mean there is any).";
            bpProp.UnitID = 133;

            // Add properties to custom categories
            general.Properties = gProperties.ToArray();
            propulsion.Properties = pProperties.ToArray();

            // Sort groups
            string[] orderedGroupNames = {"General", "Fitting", "Drones", "Structure", "Armor", "Shield", "Capacitor", 
                                             "Targeting", "Propulsion", "Miscellaneous", "NULL"};

            s_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", s_endTime.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            PropertiesDatafile datafile = new PropertiesDatafile();
            datafile.Categories =
                categories.OrderBy(x => Array.IndexOf(orderedGroupNames, String.Intern(x.Name))).ToArray();
            Util.SerializeXML(datafile, DatafileConstants.PropertiesDatafile);
        }

        #endregion

        #region Items Datafile

        /// <summary>
        /// Generate the items datafile.
        /// </summary>
        private static void GenerateItems()
        {
            Console.WriteLine();
            Console.Write(@"Generated items datafile... ");

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
            s_startTime = DateTime.Now;


            // Create custom market groups that don't exist in EVE
            s_injectedMarketGroups = new List<InvMarketGroup>
                                         {
                                             new InvMarketGroup
                                                 {
                                                     Name = "Rookie Ships",
                                                     Description = "Capsuleer starter ship",
                                                     ID = DBConstants.RookieShipRootGroupID,
                                                     ParentID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Amarr",
                                                     Description = "Amarr rookie ship",
                                                     ID = DBConstants.RookieShipAmarrGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Caldari",
                                                     Description = "Caldari rookie ship",
                                                     ID = DBConstants.RookieShipCaldariGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Gallente",
                                                     Description = "Gallente rookie ship",
                                                     ID = DBConstants.RookieShipGallenteGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Minmatar",
                                                     Description = "Minmatar rookie ship",
                                                     ID = DBConstants.RookieShipMinmatarGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Various Non-Market",
                                                     Description = "Non-Market Items",
                                                     ID = DBConstants.RootNonMarketGroupID,
                                                     ParentID = null,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Unique Designs",
                                                     Description = "Ships of a unique design",
                                                     ID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     ParentID = DBConstants.ShipsMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Unique Shuttles",
                                                     Description = "Fast ships of a unique design",
                                                     ID = DBConstants.UniqueDesignShuttlesNonMarketGroupID,
                                                     ParentID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroup
                                                 {
                                                     Name = "Unique Battleships",
                                                     Description = "Battleships ships of a unique design",
                                                     ID = DBConstants.UniqueDesignBattleshipsNonMarketGroupID,
                                                     ParentID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },

                                         };

            // Manually set some items attributes
            s_types[DBConstants.TemperatePlanetID].Published = true;
            s_types[DBConstants.IcePlanetID].Published = true;
            s_types[DBConstants.GasPlanetID].Published = true;
            s_types[DBConstants.OceanicPlanetID].Published = true;
            s_types[DBConstants.LavaPlanetID].Published = true;
            s_types[DBConstants.BarrenPlanetID].Published = true;
            s_types[DBConstants.StormPlanetID].Published = true;
            s_types[DBConstants.PlasmaPlanetID].Published = true;
            s_types[DBConstants.ShatteredPlanetID].Published = true;
            s_types[DBConstants.ChalcopyriteID].Published = true;
            s_types[DBConstants.SmallEWDroneRangeAugmentorIIID].Published = true;
            s_types[DBConstants.ImpairorID].Published = true;
            s_types[DBConstants.IbisID].Published = true;
            s_types[DBConstants.VelatorID].Published = true;
            s_types[DBConstants.ReaperID].Published = true;
            s_types[DBConstants.CapsuleID].Published = true;
            
            // Set some attributes to items because their MarketGroupID is NULL
            foreach (InvType srcItem in s_types.Where(x => x.Published && x.MarketGroupID == null))
            {
                // Set some ships market group and race
                switch (srcItem.ID)
                {
                    case DBConstants.ImpairorID:
                        srcItem.MarketGroupID = DBConstants.RookieShipAmarrGroupID;
                        break;
                    case DBConstants.IbisID:
                        srcItem.MarketGroupID = DBConstants.RookieShipCaldariGroupID;
                        break;
                    case DBConstants.VelatorID:
                        srcItem.MarketGroupID = DBConstants.RookieShipGallenteGroupID;
                        break;
                    case DBConstants.ReaperID:
                        srcItem.MarketGroupID = DBConstants.RookieShipMinmatarGroupID;
                        break;
                    case DBConstants.CapsuleID:
                        srcItem.MarketGroupID = DBConstants.UniqueDesignsRootNonMarketGroupID;
                        srcItem.RaceID = (int)Race.All;
                        break;
                    case DBConstants.MegathronFederateIssueID:
                    case DBConstants.RavenStateIssueID:
                    case DBConstants.TempestTribalIssueID:
                        srcItem.MarketGroupID = DBConstants.UniqueDesignBattleshipsNonMarketGroupID;
                        srcItem.RaceID = (int)Race.Faction;
                        break;
                    case DBConstants.GorusShuttleID:
                    case DBConstants.GuristasShuttleID:
                    case DBConstants.InterbusShuttleID:
                        srcItem.MarketGroupID = DBConstants.UniqueDesignShuttlesNonMarketGroupID;
                        srcItem.RaceID = (int)Race.Faction;
                        break;

                        // Set some items market group to support blueprints
                    case DBConstants.EliteDroneAIID:
                    case DBConstants.StandardDecodingDeviceID:
                    case DBConstants.EncodingMatrixComponentID:
                    case DBConstants.ChalcopyriteID:
                    case DBConstants.ClayPigeonID:
                    case DBConstants.MinmatarDNAID:
                    case DBConstants.BasicRoboticsID:
                    case DBConstants.AmarrFactoryOutpostPlatformID:
                    case DBConstants.CaldariResearchOutpostPlatformID:
                    case DBConstants.GallenteAdministrativeOutpostPlatformID:
                    case DBConstants.MinmatarServiceOutpostPlatformID:
                    case DBConstants.SmallEWDroneRangeAugmentorIIID:
                    case DBConstants.CivilianBallisticDeflectionFieldID:
                    case DBConstants.CivilianDamageControlID:
                    case DBConstants.CivilianExplosionDampeningFieldID:
                    case DBConstants.CivilianHeatDissipationFieldID:
                    case DBConstants.CivilianPhotonScatteringFieldID:
                    case DBConstants.CivilianStasisWebifierID:
                    case DBConstants.CrudeSculptureID:
                    case DBConstants.GoldSculptureID:
                    case DBConstants.MethrosEnhancedDecodingDeviceID:
                    case DBConstants.ModifiedAugumeneAntidoteID:
                    case DBConstants.ProcessInterruptiveWarpDisruptorID:
                    case DBConstants.SleeperDataAnalyzerIID:
                    case DBConstants.TalocanDataAnalyzerIID:
                    case DBConstants.TerranDataAnalyzerIID:
                    case DBConstants.TetrimonDataAnalyzerIID:
                    case DBConstants.WildMinerIID:
                    case DBConstants.AlphaCodebreakerIID:
                    case DBConstants.CodexCodebreakerIID:
                    case DBConstants.DaemonCodebreakerIID:
                    case DBConstants.LibramCodebreakerIID:
                        srcItem.MarketGroupID = DBConstants.RootNonMarketGroupID;
                        break;
                }

                // Set some items market group to support blueprints
                if (srcItem.ID > DBConstants.SynthSoothSayerBoosterBlueprintID &&
                    srcItem.ID < DBConstants.AmberMykoserocinID)
                    srcItem.MarketGroupID = DBConstants.RootNonMarketGroupID;

                // Adding planets to support attribute browsing for command centers
                if (srcItem.GroupID == DBConstants.PlanetGroupID)
                    srcItem.MarketGroupID = DBConstants.RootNonMarketGroupID;
            }

            Dictionary<int, SerializableMarketGroup> groups = new Dictionary<int, SerializableMarketGroup>();

            // Create the market groups
            foreach (InvMarketGroup srcGroup in s_marketGroups.Concat(s_injectedMarketGroups))
            {
                SerializableMarketGroup group = new SerializableMarketGroup { ID = srcGroup.ID, Name = srcGroup.Name };
                groups[srcGroup.ID] = group;

                // Add the items in this group
                List<SerializableItem> items = new List<SerializableItem>();
                foreach (InvType srcItem in s_types
                    .Where(x => x.Published && (x.MarketGroupID.GetValueOrDefault() == srcGroup.ID)))
                {
                    CreateItem(srcItem, items);
                }

                // If this is an implant group, we add the implants with no market groups in this one.
                if (srcGroup.ParentID == DBConstants.SkillHardwiringImplantsMarketGroupID || srcGroup.ParentID == DBConstants.AttributeEnhancersImplantsMarketGroupID)
                {
                    string slotString = srcGroup.Name.Substring("Implant Slot ".Length);
                    int slot = Int32.Parse(slotString);

                    // Enumerate all implants without market groups
                    foreach (InvType srcItem in s_types
                        .Where(x => x.MarketGroupID == null 
                        && s_groups[x.GroupID].CategoryID == DBConstants.ImplantCategoryID 
                        && x.GroupID != DBConstants.CyberLearningImplantsGroupID)
                        )
                    {
                        // Check the slot matches
                        DgmTypeAttribute slotAttrib = s_typeAttributes.Get(srcItem.ID, DBConstants.ImplantSlotPropertyID);
                        if (slotAttrib != null && slotAttrib.GetIntValue() == slot)
                            CreateItem(srcItem, items);
                    }
                }

                // Store the items
                group.Items = items.OrderBy(x => x.Name).ToArray();
            }

            // Create the parent-children groups relations
            foreach (SerializableMarketGroup group in groups.Values)
            {
                IEnumerable<SerializableMarketGroup> children =
                    s_marketGroups.Concat(s_injectedMarketGroups).Where(x => x.ParentID.GetValueOrDefault() == group.ID)
                        .Select(x => groups[x.ID]);
                group.SubGroups = children.OrderBy(x => x.Name).ToArray();
            }

            // Pick the family
            SetItemFamilyByMarketGroup(groups[DBConstants.BlueprintsMarketGroupID], ItemFamily.Bpo);
            SetItemFamilyByMarketGroup(groups[DBConstants.ShipsMarketGroupID], ItemFamily.Ship);
            SetItemFamilyByMarketGroup(groups[DBConstants.ImplantsMarketGroupID], ItemFamily.Implant);
            SetItemFamilyByMarketGroup(groups[DBConstants.DronesMarketGroupID], ItemFamily.Drone);
            SetItemFamilyByMarketGroup(groups[DBConstants.StarbaseStructuresMarketGroupID], ItemFamily.StarbaseStructure);

            // Sort groups
            IOrderedEnumerable<SerializableMarketGroup> rootGroups =
                s_marketGroups.Concat(s_injectedMarketGroups).Where(x => !x.ParentID.HasValue).Select(x => groups[x.ID])
                    .OrderBy(x => x.Name);

            s_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", s_endTime.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            ItemsDatafile datafile = new ItemsDatafile();
            datafile.MarketGroups = rootGroups.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.ItemsDatafile);
        }

        /// <summary>
        /// Add properties to an item.
        /// </summary>
        /// <param name="srcItem"></param>
        /// <param name="groupItems"></param>
        /// <returns></returns>
        private static void CreateItem(InvType srcItem, List<SerializableItem> groupItems)
        {
            UpdatePercentDone(s_itemGenTotal);

            srcItem.Generated = true;

            // Creates the item with base informations
            SerializableItem item = new SerializableItem
                           {
                               ID = srcItem.ID,
                               Name = srcItem.Name,
                               Description = srcItem.Description
                           };

            // Icon
            item.Icon = (srcItem.IconID.HasValue ? s_icons[srcItem.IconID.Value].Icon : String.Empty);

            // Portion Size (the batch)
            item.PortionSize = srcItem.PortionSize;
            
            // Initialize item metagroup
            item.MetaGroup = ItemMetaGroup.Empty;

            // Add the properties and prereqs
            int baseWarpSpeed = 3;
            double warpSpeedMultiplier = 1;
            List<SerializablePropertyValue> props = new List<SerializablePropertyValue>();
            int[] prereqSkills = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            int[] prereqLevels = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            foreach (DgmTypeAttribute srcProp in s_typeAttributes.Where(x => x.ItemID == srcItem.ID))
            {
                int propIntValue = (srcProp.ValueInt.HasValue ? srcProp.ValueInt.Value : (int)srcProp.ValueFloat.Value);

                // Is it a prereq skill ?
                int prereqIndex = Array.IndexOf(DBConstants.RequiredSkillPropertyIDs, srcProp.AttributeID);
                if (prereqIndex >= 0)
                {
                    prereqSkills[prereqIndex] = propIntValue;
                    continue;
                }

                // Is it a prereq level ?
                prereqIndex = Array.IndexOf(DBConstants.RequiredSkillLevelPropertyIDs, srcProp.AttributeID);
                if (prereqIndex >= 0)
                {
                    prereqLevels[prereqIndex] = propIntValue;
                    continue;
                }

                // Launcher group ?
                int launcherIndex = Array.IndexOf(DBConstants.LauncherGroupIDs, srcProp.AttributeID);
                if (launcherIndex >= 0)
                {
                    props.Add(new SerializablePropertyValue
                                    {
                                        ID = srcProp.AttributeID,
                                        Value = s_groups[propIntValue].Name
                                    });
                    continue;
                }

                // Charge group ?
                int chargeIndex = Array.IndexOf(DBConstants.ChargeGroupIDs, srcProp.AttributeID);
                if (chargeIndex >= 0)
                {
                    props.Add(new SerializablePropertyValue
                                    {
                                        ID = srcProp.AttributeID,
                                        Value = s_groups[propIntValue].Name
                                    });
                    continue;
                }

                // CanFitShip group ?
                int canFitShipIndex = Array.IndexOf(DBConstants.CanFitShipGroupIDs, srcProp.AttributeID);
                if (canFitShipIndex >= 0)
                {
                    props.Add(new SerializablePropertyValue
                                    {
                                        ID = srcProp.AttributeID,
                                        Value = s_groups[propIntValue].Name
                                    });
                    continue;
                }

                // ModuleShip group ?
                int moduleShipIndex = Array.IndexOf(DBConstants.ModuleShipGroupIDs, srcProp.AttributeID);
                if (moduleShipIndex >= 0)
                {
                    props.Add(new SerializablePropertyValue
                                    {
                                        ID = srcProp.AttributeID,
                                        Value = s_groups[propIntValue].Name
                                    });
                    continue;
                }

                // SpecialisationAsteroid group ?
                int specialisationAsteroidIndex = Array.IndexOf(DBConstants.SpecialisationAsteroidGroupIDs,
                                                                srcProp.AttributeID);
                if (specialisationAsteroidIndex >= 0)
                {
                    props.Add(new SerializablePropertyValue
                                    {
                                        ID = srcProp.AttributeID,
                                        Value = s_groups[propIntValue].Name
                                    });
                    continue;
                }

                // Reaction group ?
                int reactionIndex = Array.IndexOf(DBConstants.ReactionGroupIDs, srcProp.AttributeID);
                if (reactionIndex >= 0)
                {
                    props.Add(new SerializablePropertyValue
                                    {
                                        ID = srcProp.AttributeID,
                                        Value = s_groups[propIntValue].Name
                                    });
                    continue;
                }

                // PosCargobayAccept group ?
                int posCargobayAcceptIndex = Array.IndexOf(DBConstants.PosCargobayAcceptGroupIDs, srcProp.AttributeID);
                if (posCargobayAcceptIndex >= 0)
                {
                    props.Add(new SerializablePropertyValue
                                    {
                                        ID = srcProp.AttributeID,
                                        Value = s_groups[propIntValue].Name
                                    });
                    continue;
                }

                // Get the warp speed multiplier
                if (srcProp.AttributeID == DBConstants.WarpSpeedMultiplierPropertyID)
                    warpSpeedMultiplier = srcProp.ValueFloat.Value;

                // We calculate and add the ships warp speed
                if (srcProp.AttributeID == DBConstants.ShipWarpSpeedPropertyID)
                {
                    props.Add(new SerializablePropertyValue
                                    { ID = srcProp.AttributeID, Value = (baseWarpSpeed * warpSpeedMultiplier).ToString() });

                    // Also add packaged volume as a prop as only ships have 'ship warp speed' attribute
                    props.Add(new SerializablePropertyValue
                                    { ID = s_propPackagedVolumeID, Value = GetPackagedVolume(srcItem.GroupID).ToString() });
                }

                // Other props
                props.Add(new SerializablePropertyValue
                                {ID = srcProp.AttributeID, Value = srcProp.FormatPropertyValue()});

                // Is metalevel property ?
                if (srcProp.AttributeID == DBConstants.MetaLevelPropertyID)
                    item.MetaLevel = propIntValue;
                
                // Is techlevel property ?
                if (srcProp.AttributeID == DBConstants.TechLevelPropertyID)
                {
                    switch (propIntValue)
                    {
                        default:
                        case DBConstants.TechLevelI:
                            item.MetaGroup = ItemMetaGroup.T1;
                            break;
                        case DBConstants.TechLevelII:
                            item.MetaGroup = ItemMetaGroup.T2;
                            break;
                        case DBConstants.TechLevelIII:
                            item.MetaGroup = ItemMetaGroup.T3;
                            break;
                    }
                }

                // Is metagroup property ?
                if (srcProp.AttributeID == DBConstants.MetaGroupPropertyID)
                {
                    switch (propIntValue)
                    {
                        case DBConstants.StorylineMetaGroupID:
                            item.MetaGroup = ItemMetaGroup.Storyline;
                            break;
                        case DBConstants.FactionMetaGroupID:
                            item.MetaGroup = ItemMetaGroup.Faction;
                            break;
                        case DBConstants.OfficerMetaGroupID:
                            item.MetaGroup = ItemMetaGroup.Officer;
                            break;
                        case DBConstants.DeadspaceMetaGroupID:
                            item.MetaGroup = ItemMetaGroup.Deadspace;
                            break;
                        default:
                            item.MetaGroup = ItemMetaGroup.None;
                            break;
                    }
                }
            }

            // Ensures there is a mass and add it to prop
            if (srcItem.Mass != 0)
                props.Add(new SerializablePropertyValue
                            { ID = DBConstants.MassPropertyID, Value = srcItem.Mass.ToString() });

            // Ensures there is a cargo capacity and add it to prop
            if (srcItem.Capacity != 0)
                props.Add(new SerializablePropertyValue
                            { ID = DBConstants.CargoCapacityPropertyID, Value = srcItem.Capacity.ToString() });

            // Ensures there is a volume and add it to prop
            if (srcItem.Volume != 0)
                props.Add(new SerializablePropertyValue
                            { ID = DBConstants.VolumePropertyID, Value = srcItem.Volume.ToString() });

            // Add base price as a prop
            props.Add(new SerializablePropertyValue
                        { ID = s_propBasePriceID, Value = srcItem.BasePrice.FormatDecimal()});

            // Add properties info to item
            item.Properties = props.ToArray();

            // Prerequisites completion
            List<SerializablePrerequisiteSkill> prereqs = new List<SerializablePrerequisiteSkill>();
            for (int i = 0; i < prereqSkills.Length; i++)
            {
                if (prereqSkills[i] != 0)
                    prereqs.Add(new SerializablePrerequisiteSkill { ID = prereqSkills[i], Level = prereqLevels[i] });
            }

            // Add prerequisite skills info to item
            item.Prereqs = prereqs.ToArray();

            // Metagroup
            foreach (InvMetaType relation in s_metaTypes
                .Where(x => x.ItemID == srcItem.ID && item.MetaGroup == ItemMetaGroup.Empty))
            {
                switch (relation.MetaGroupID)
                {
                    case DBConstants.TechIMetaGroupID:
                        item.MetaGroup = ItemMetaGroup.T1;
                        break;
                    case DBConstants.TechIIMetaGroupID:
                        item.MetaGroup = ItemMetaGroup.T2;
                        break;
                    case DBConstants.StorylineMetaGroupID:
                        item.MetaGroup = ItemMetaGroup.Storyline;
                        break;
                    case DBConstants.FactionMetaGroupID:
                        item.MetaGroup = ItemMetaGroup.Faction;
                        break;
                    case DBConstants.OfficerMetaGroupID:
                        item.MetaGroup = ItemMetaGroup.Officer;
                        break;
                    case DBConstants.DeadspaceMetaGroupID:
                        item.MetaGroup = ItemMetaGroup.Deadspace;
                        break;
                    case DBConstants.TechIIIMetaGroupID:
                        item.MetaGroup = ItemMetaGroup.T3;
                        break;
                    default:
                        item.MetaGroup = ItemMetaGroup.None;
                        break;
                }
            }

            if (item.MetaGroup == ItemMetaGroup.Empty)
                item.MetaGroup = ItemMetaGroup.T1;

            // Race ID
            item.Race = (Race) Enum.ToObject(typeof (Race), (srcItem.RaceID == null ? 0 : srcItem.RaceID));
            
            // Set race to Faction if item race is Jovian
            if (item.Race == Race.Jove)
                item.Race = Race.Faction;

            // Set race to ORE if it is in the ORE market groups
            // within mining barges, exhumers, industrial or capital industrial ships
            if (srcItem.MarketGroupID == DBConstants.MiningBargesMarketGroupID
                || srcItem.MarketGroupID == DBConstants.ExhumersMarketGroupID
                || srcItem.MarketGroupID == DBConstants.IndustrialsMarketGroupID
                || srcItem.MarketGroupID == DBConstants.CapitalIndustrialsMarketGroupID)
                item.Race = Race.Ore;

            // Set race to Faction if ship has Pirate Faction property
            foreach (SerializablePropertyValue prop in props)
            {
                if (prop.ID == DBConstants.ShipBonusPirateFactionPropertyID)
                    item.Race = Race.Faction;
            }

            // Look for slots
            if (s_typeEffects.Contains(srcItem.ID, DBConstants.LowSlotEffectID))
            {
                item.Slot = ItemSlot.Low;
            }
            else if (s_typeEffects.Contains(srcItem.ID, DBConstants.MedSlotEffectID))
            {
                item.Slot = ItemSlot.Medium;
            }
            else if (s_typeEffects.Contains(srcItem.ID, DBConstants.HiSlotEffectID))
            {
                item.Slot = ItemSlot.High;
            }
            else
            {
                item.Slot = ItemSlot.None;
            }

            // Add this item
            groupItems.Add(item);

            // If the current item isn't in a market group then we are done
            if (srcItem.MarketGroupID == null)
                return;

            // Look for variations which are not in any market group
            foreach (InvMetaType variation in s_metaTypes.Where(x => x.ParentItemID == srcItem.ID))
            {
                InvType srcVariationItem = s_types[variation.ItemID];
                if (srcVariationItem.Published && srcVariationItem.MarketGroupID == null)
                {
                    srcVariationItem.RaceID = (int)Race.Faction;
                    CreateItem(srcVariationItem, groupItems);
                }
            }
        }

        /// <summary>
        /// Gets the packaged volume of a ship.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <returns></returns>
        private static int GetPackagedVolume(int groupID)
        {
            switch (groupID)
            {
                default:
                case DBConstants.CapsuleGroupID:
                case DBConstants.ShuttleGroupID:
                case DBConstants.PrototypeExplorationShipGroupID:
                    return 500;
                case DBConstants.FrigateGroupID:
                case DBConstants.RookieShipGroupID:
                case DBConstants.AssaultShipGroupID:
                case DBConstants.CovertOpsGroupID:
                case DBConstants.InterceptorGroupID:
                case DBConstants.StealthBomberGroupID:
                case DBConstants.ElectronicAttackShipGroupID:
                    return 2500;
                case DBConstants.MiningBargeGroupID:
                case DBConstants.ExhumerGroupID:
                    return 3750;
                case DBConstants.DestroyerGroupID:
                case DBConstants.StrategicCruiserGroupID:
                    return 5000;
                case DBConstants.CruiserGroupID:
                case DBConstants.HeavyAssaultShipGroupID:
                case DBConstants.InterdictorGroupID:
                case DBConstants.LogisticsGroupID:
                case DBConstants.ForceReconShipGroupID:
                case DBConstants.HeavyInterdictorGroupID:
                case DBConstants.CombatReconShipGroupID:
                    return 10000;
                case DBConstants.BattlecruiserGroupID:
                case DBConstants.CommandShipGroupID:
                    return 15000;
                case DBConstants.IndustrialGroupID:
                case DBConstants.TransportShipGroupID:
                    return 20000;
                case DBConstants.BattleshipGroupID:
                case DBConstants.EliteBattleshipGroupID:
                case DBConstants.BlackOpsGroupID:
                case DBConstants.MarauderGroupID:
                    return 50000;
                case DBConstants.IndustrialCommandShipGroupID:
                    return 500000;
                case DBConstants.DreadnoughtGroupID:
                case DBConstants.FreighterGroupID:
                case DBConstants.CarrierGroupID:
                case DBConstants.SupercarrierGroupID:
                case DBConstants.CapitalIndustrialShipGroupID:
                case DBConstants.JumpFreighterGroupID:
                    return 1000000;
                case DBConstants.TitanGroupID:
                    return 10000000;
            }
        }

        /// <summary>
        /// Sets the item family according to its market group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="itemFamily"></param>
        private static void SetItemFamilyByMarketGroup(SerializableMarketGroup group, ItemFamily itemFamily)
        {
            foreach (SerializableItem item in group.Items)
            {
                item.Family = itemFamily;
            }

            foreach (SerializableMarketGroup childGroup in group.SubGroups)
            {
                SetItemFamilyByMarketGroup(childGroup, itemFamily);
            }
        }

        #endregion

        #region Skills Datafile

        /// <summary>
        /// Generate the skills datafile.
        /// </summary>
        private static void GenerateSkills()
        {
            Console.WriteLine();
            Console.Write(@"Generated skills datafile... ");

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
            s_startTime = DateTime.Now;

            // Export skill groups
            List<SerializableSkillGroup> listOfSkillGroups = new List<SerializableSkillGroup>();

            foreach (InvGroup group in s_groups
                    .Where(x => x.CategoryID == DBConstants.SkillCategoryID && x.ID != DBConstants.FakeSkillsGroupID)
                    .OrderBy(x => x.Name))
            {
                SerializableSkillGroup skillGroup = new SerializableSkillGroup
                                                         {
                                                             ID = group.ID,
                                                             Name = group.Name,
                                                         };

                // Export skills
                List<SerializableSkill> listOfSkillsInGroup = new List<SerializableSkill>();

                foreach (InvType skill in s_types.Where(x => x.GroupID == group.ID))
                {
                    UpdatePercentDone(s_skillGenTotal);

                    SerializableSkill singleSkill = new SerializableSkill
                                                          {
                                                              ID = skill.ID,
                                                              Name = skill.Name,
                                                              Description = skill.Description,
                                                              Public = skill.Published,
                                                              Cost = (long) skill.BasePrice,
                                                          };

                    // Export skill atributes
                    Dictionary<int, int> skillAttributes = new Dictionary<int, int>();

                    foreach (DgmTypeAttribute attribute in s_typeAttributes.Where(x => x.ItemID == skill.ID))
                    {
                        skillAttributes.Add(attribute.AttributeID, attribute.GetIntValue());
                    }

                    if (skillAttributes.ContainsKey(DBConstants.SkillTimeConstantPropertyID) &&
                        skillAttributes[DBConstants.SkillTimeConstantPropertyID] > 0)
                    {
                        singleSkill.Rank = skillAttributes[DBConstants.SkillTimeConstantPropertyID];
                    }
                    else
                    {
                        singleSkill.Rank = 1;
                    }

                    singleSkill.PrimaryAttribute = skillAttributes.ContainsKey(DBConstants.PrimaryAttributePropertyID)
                                                       ? IntToEveAttribute(skillAttributes[DBConstants.PrimaryAttributePropertyID])
                                                       : EveAttribute.None;
                    singleSkill.SecondaryAttribute = skillAttributes.ContainsKey(DBConstants.SecondaryAttributePropertyID)
                                                         ? IntToEveAttribute(skillAttributes[DBConstants.SecondaryAttributePropertyID])
                                                         : EveAttribute.None;
                    singleSkill.CanTrainOnTrial = skillAttributes.ContainsKey(DBConstants.CanNotBeTrainedOnTrialPropertyID) &&
                                                    skillAttributes[DBConstants.CanNotBeTrainedOnTrialPropertyID] == 0 ? true : false;

                    // Export prerequesities
                    List<SerializableSkillPrerequisite> listOfPrerequisites = new List<SerializableSkillPrerequisite>();

                    for (int i = 0; i < DBConstants.RequiredSkillPropertyIDs.Length; i++)
                    {
                        if (skillAttributes.ContainsKey(DBConstants.RequiredSkillPropertyIDs[i]) &&
                            skillAttributes.ContainsKey(DBConstants.RequiredSkillLevelPropertyIDs[i]))
                        {
                            int j = i;
                            InvType prereqSkill =
                                s_types.First(x => x.ID == skillAttributes[DBConstants.RequiredSkillPropertyIDs[j]]);

                            SerializableSkillPrerequisite preReq = new SerializableSkillPrerequisite
                                                                {
                                                                    ID = prereqSkill.ID,
                                                                    Level = skillAttributes[DBConstants.RequiredSkillLevelPropertyIDs[i]],
                                                                };

                            if (prereqSkill != null)
                                preReq.Name = prereqSkill.Name;

                            // Add prerequisites
                            listOfPrerequisites.Add(preReq);
                        }
                    }

                    // Add prerequesites to skill
                    singleSkill.Prereqs = listOfPrerequisites.ToArray();

                    // Add skill
                    listOfSkillsInGroup.Add(singleSkill);
                }

                // Add skills in skill group
                skillGroup.Skills = listOfSkillsInGroup.OrderBy(x => x.Name).ToArray();

                // Add skill group
                listOfSkillGroups.Add(skillGroup);
            }

            s_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", s_endTime.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            SkillsDatafile datafile = new SkillsDatafile();
            datafile.Groups = listOfSkillGroups.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.SkillsDatafile);
        }

        /// <summary>
        /// Gets the Eve attribute.
        /// </summary>        
        private static EveAttribute IntToEveAttribute(int attributeValue)
        {
            switch (attributeValue)
            {
                case DBConstants.CharismaPropertyID:
                    return EveAttribute.Charisma;
                case DBConstants.IntelligencePropertyID:
                    return EveAttribute.Intelligence;
                case DBConstants.MemoryPropertyID:
                    return EveAttribute.Memory;
                case DBConstants.PerceptionPropertyID:
                    return EveAttribute.Perception;
                case DBConstants.WillpowerPropertyID:
                    return EveAttribute.Willpower;
                default:
                    return EveAttribute.None;
            }
        }

        #endregion

        #region Certificates Datafile

        /// <summary>
        /// Generate the certificates datafile.
        /// </summary>        
        private static void GenerateCertificates()
        {
            Console.WriteLine();
            Console.Write(@"Generated certificates datafile... ");

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
            s_startTime = DateTime.Now;

            // Export certificates categories
            List<SerializableCertificateCategory> listOfCertCategories = new List<SerializableCertificateCategory>();

            foreach (CrtCategories category in s_crtCategories.OrderBy(x => x.CategoryName))
            {
                SerializableCertificateCategory crtCategory = new SerializableCertificateCategory
                                                                  {
                                                                      ID = category.ID,
                                                                      Name = category.CategoryName,
                                                                      Description = category.Description
                                                                  };

                // Export certificates classes
                List<SerializableCertificateClass> listOfCertClasses = new List<SerializableCertificateClass>();

                int categoryID = 0;
                foreach (CrtClasses certClass in s_crtClasses)
                {
                    // Exclude unused classes
                    int id = certClass.ID;
                    if (id == DBConstants.IndustrialHarvestingID ||
                        id == DBConstants.AutomatedMiningID ||
                        id == DBConstants.ProductionInternID)
                        continue;

                    SerializableCertificateClass crtClasses = new SerializableCertificateClass
                                                                    {
                                                                    ID = certClass.ID,
                                                                    Name = certClass.ClassName,
                                                                    Description = certClass.Description
                                                                    };

                    // Export certificates
                    List<SerializableCertificate> listOfCertificates = new List<SerializableCertificate>();

                    foreach (CrtCertificates certificate in s_certificates.Where(x => x.ClassID == certClass.ID))
                    {
                        UpdatePercentDone(s_certGenTotal);

                        SerializableCertificate crtCertificates = new SerializableCertificate
                                                                      {
                                                                          ID = certificate.ID,
                                                                          Grade = GetGrade(certificate.Grade),
                                                                          Description = certificate.Description
                                                                      };

                        // Export prerequesities
                        List<SerializableCertificatePrerequisite> listOfPrereq = new List<SerializableCertificatePrerequisite>();

                        foreach (CrtRelationships relationship in s_crtRelationships
                            .Where(x => x.ChildID == certificate.ID))
                        {
                            SerializableCertificatePrerequisite crtPrerequisites = new SerializableCertificatePrerequisite
                                                                                       {
                                                                                           ID = relationship.ID,
                                                                                       };


                            if (relationship.ParentTypeID != null) // prereq is a skill
                            {
                                InvType skill = s_types.First(x => x.ID == relationship.ParentTypeID);
                                crtPrerequisites.Kind = SerializableCertificatePrerequisiteKind.Skill;
                                crtPrerequisites.Name = skill.Name;
                                crtPrerequisites.Level = relationship.ParentLevel.ToString();
                            }
                            else // prereq is a certificate
                            {
                                CrtCertificates cert = s_certificates.First(x => x.ID == relationship.ParentID);
                                CrtClasses crtClass = s_crtClasses.First(x => x.ID == cert.ClassID);
                                crtPrerequisites.Kind = SerializableCertificatePrerequisiteKind.Certificate;
                                crtPrerequisites.Name = crtClass.ClassName;
                                crtPrerequisites.Level = GetGrade(cert.Grade).ToString();
                            }

                            // Add prerequisite
                            listOfPrereq.Add(crtPrerequisites);
                        }

                        // Export recommendations
                        List<SerializableCertificateRecommendation> listOfRecom = new List<SerializableCertificateRecommendation>();

                        foreach (CrtRecommendations recommendation in s_crtRecommendations
                            .Where(x => x.CertificateID == certificate.ID))
                        {
                            // Finds the ships name
                            InvType shipName = s_types.First(x => x.ID == recommendation.ShipTypeID);

                            SerializableCertificateRecommendation crtRecommendations = new SerializableCertificateRecommendation
                                                                                            {
                                                                                                ID = recommendation.ID,
                                                                                                Ship = shipName.Name,
                                                                                                Level = recommendation.Level
                                                                                            };

                            // Add recommendation
                            listOfRecom.Add(crtRecommendations);
                        }


                        //Add prerequisites to certificate
                        crtCertificates.Prerequisites = listOfPrereq.ToArray();

                        // Add recommendations to certificate
                        crtCertificates.Recommendations = listOfRecom.ToArray();

                        // Add certificate
                        listOfCertificates.Add(crtCertificates);

                        // Storing the certificate categoryID for use in classes
                        categoryID = certificate.CategoryID;
                    }

                    // Grouping certificates according to their classes
                    if (categoryID == category.ID)
                    {
                        // Add certificates to classes
                        crtClasses.Certificates = listOfCertificates.OrderBy(x => x.Grade).ToArray();

                        // Add certificate class
                        listOfCertClasses.Add(crtClasses);
                    }
                }

                // Add classes to categories
                crtCategory.Classes = listOfCertClasses.ToArray();

                // Add category
                listOfCertCategories.Add(crtCategory);
            }

            s_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", s_endTime.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            CertificatesDatafile datafile = new CertificatesDatafile();
            datafile.Categories = listOfCertCategories.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.CertificatesDatafile);
        }

        /// <summary>
        /// Gets the certificates Grade.
        /// </summary>        
        private static CertificateGrade GetGrade(int gradeValue)
        {
            switch (gradeValue)
            {
                case DBConstants.BasicID:
                    return CertificateGrade.Basic;
                case DBConstants.StandardID:
                    return CertificateGrade.Standard;
                case DBConstants.ImprovedID:
                    return CertificateGrade.Improved;
                case DBConstants.EliteID:
                    return CertificateGrade.Elite;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Blueprints Datafile

        /// <summary>
        /// Generate the skills datafile.
        /// </summary>
        private static void GenerateBlueprints()
        {
            Console.WriteLine();
            Console.Write(@"Generated blueprints datafile... ");

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
            s_startTime = DateTime.Now;

            // Configure blueprints with Null market group
            ConfigureNullMarketBlueprint();

            Dictionary<int, SerializableBlueprintMarketGroup> groups = new Dictionary<int, SerializableBlueprintMarketGroup>();

            // Export blueprint groups           
            foreach (InvMarketGroup marketGroup in s_marketGroups.Concat(s_injectedMarketGroups))
            {
                SerializableBlueprintMarketGroup group = new SerializableBlueprintMarketGroup
                                                        {
                                                            ID = marketGroup.ID,
                                                            Name = marketGroup.Name,
                                                        };

                groups[marketGroup.ID] = group;

                // Add the items in this group
                List<SerializableBlueprint> blueprints = new List<SerializableBlueprint>();
                foreach (InvType item in s_types.Where(x => x.MarketGroupID == marketGroup.ID
                                       && s_groups[x.GroupID].CategoryID == DBConstants.BlueprintCategoryID
                                       && s_groups[x.GroupID].Published))
                {
                    CreateBlueprint(item, blueprints);
                }

                // Store the items
                group.Blueprints = blueprints.OrderBy(x => x.Name).ToArray();
            }

            // Create the parent-children groups relations
            foreach (SerializableBlueprintMarketGroup group in groups.Values)
            {
                IEnumerable<SerializableBlueprintMarketGroup> children = s_marketGroups.Concat(
                    s_injectedMarketGroups).Where(x => x.ParentID == group.ID).Select(x => groups[x.ID]);

                group.SubGroups = children.OrderBy(x => x.Name).ToArray();
            }

            // Sort groups
            IOrderedEnumerable<SerializableBlueprintMarketGroup> blueprintGroups = s_marketGroups.Concat(
                s_injectedMarketGroups).Where(x => x.ParentID == DBConstants.BlueprintsMarketGroupID)
                .Select(x => groups[x.ID]).OrderBy(x => x.Name);

            s_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", s_endTime.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            BlueprintsDatafile datafile = new BlueprintsDatafile();
            datafile.MarketGroups = blueprintGroups.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.BlueprintsDatafile);
        }

        /// <summary>
        /// Configures the null market blueprint.
        /// </summary>
        private static void ConfigureNullMarketBlueprint()
        {
            s_injectedMarketGroups = new List<InvMarketGroup>();

            // Create custom market groups that don't exist in EVE
            s_injectedMarketGroups.Add(new InvMarketGroup
                                           {
                                               Name = "Various Non-Market",
                                               Description = "Various blueprints not in EVE market",
                                               ID = DBConstants.BlueprintRootNonMarketGroupID,
                                               ParentID = DBConstants.BlueprintsMarketGroupID,
                                               IconID = DBConstants.UnknownBlueprintBackdropIconID
                                           });

            s_injectedMarketGroups.Add(new InvMarketGroup
                                           {
                                               Name = "Tech I",
                                               Description = "Tech I blueprints not in EVE market",
                                               ID = DBConstants.BlueprintTechINonMarketGroupID,
                                               ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                               IconID = DBConstants.UnknownBlueprintBackdropIconID
                                           });

            s_injectedMarketGroups.Add(new InvMarketGroup
                                           {
                                               Name = "Tech II",
                                               Description = "Tech II blueprints not in EVE market",
                                               ID = DBConstants.BlueprintTechIINonMarketGroupID,
                                               ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                               IconID = DBConstants.UnknownBlueprintBackdropIconID
                                           });

            s_injectedMarketGroups.Add(new InvMarketGroup
                                           {
                                               Name = "Storyline",
                                               Description = "Storyline blueprints not in EVE market",
                                               ID = DBConstants.BlueprintStorylineNonMarketGroupID,
                                               ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                               IconID = DBConstants.UnknownBlueprintBackdropIconID
                                           });

            s_injectedMarketGroups.Add(new InvMarketGroup
                                           {
                                               Name = "Faction",
                                               Description = "Faction blueprints not in EVE market",
                                               ID = DBConstants.BlueprintFactionNonMarketGroupID,
                                               ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                               IconID = DBConstants.UnknownBlueprintBackdropIconID
                                           });

            s_injectedMarketGroups.Add(new InvMarketGroup
                                           {
                                               Name = "Officer",
                                               Description = "Officer blueprints not in EVE market",
                                               ID = DBConstants.BlueprintOfficerNonMarketGroupID,
                                               ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                               IconID = DBConstants.UnknownBlueprintBackdropIconID
                                           });

            s_injectedMarketGroups.Add(new InvMarketGroup
                                           {
                                               Name = "Tech III",
                                               Description = "Tech III blueprints not in EVE market",
                                               ID = DBConstants.BlueprintTechIIINonMarketGroupID,
                                               ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                               IconID = DBConstants.UnknownBlueprintBackdropIconID
                                           });

            // Set some blueprints to market groups manually
            foreach (InvType item in s_types.Where(x => x.MarketGroupID == null))
            {
                switch (item.ID)
                {
                    case DBConstants.WildMinerIBlueprintID:
                        item.MarketGroupID = DBConstants.BlueprintStorylineNonMarketGroupID;
                        break;
                    case DBConstants.AdrestiaBlueprintID:
                    case DBConstants.EchelonBlueprintID:
                    case DBConstants.ImperialNavySlicerBlueprintID:
                    case DBConstants.CaldariNavyHookbillBlueprintID:
                    case DBConstants.FederationNavyCometBlueprintID:
                    case DBConstants.RepublicFleetFiretailBlueprintID:
                    case DBConstants.NightmareBlueprintID: 
                    case DBConstants.MacharielBlueprintID:
                    case DBConstants.DramielBlueprintID:
                    case DBConstants.CruorBlueprintID:
                    case DBConstants.SuccubusBlueprintID:
                    case DBConstants.DaredevilBlueprintID:
                    case DBConstants.CynabalBlueprintID:
                    case DBConstants.AshimmuBlueprintID:
                    case DBConstants.PhantasmBlueprintID:
                    case DBConstants.GorusShuttleBlueprintID:
                    case DBConstants.GuristasShuttleBlueprintID:
                    case DBConstants.GallenteMiningLaserBlueprintID:
                    case DBConstants.InterbusShuttleBlueprintID:
                    case DBConstants.FrekiBlueprintID:
                    case DBConstants.MimirBlueprintID:
                        item.MarketGroupID = DBConstants.BlueprintFactionNonMarketGroupID;
                        break;
                    case DBConstants.LegionBlueprintID:
                    case DBConstants.LegionDefensiveAdaptiveAugmenterBlueprintID:
                    case DBConstants.LegionElectronicsEnergyParasiticComplexBlueprintID:
                    case DBConstants.LegionEngineeringPowerCoreMultiplierBlueprintID:
                    case DBConstants.LegionOffensiveDroneSynthesisProjectorBlueprintID:
                    case DBConstants.LegionPropulsionChassisOptimizationBlueprintID:
                    case DBConstants.LokiBlueprintID:
                    case DBConstants.LokiDefensiveAdaptiveShieldingBlueprintID:
                    case DBConstants.LokiElectronicsImmobilityDriversBlueprintID:
                    case DBConstants.LokiEngineeringPowerCoreMultiplierBlueprintID:
                    case DBConstants.LokiOffensiveTurretConcurrenceRegistryBlueprintID:
                    case DBConstants.LokiPropulsionChassisOptimizationBlueprintID:
                    case DBConstants.ProteusBlueprintID:
                    case DBConstants.ProteusDefensiveAdaptiveAugmenterBlueprintID:
                    case DBConstants.ProteusElectronicsFrictionExtensionProcessorBlueprintID:
                    case DBConstants.ProteusEngineeringPowerCoreMultiplierBlueprintID:
                    case DBConstants.ProteusOffensiveDissonicEncodingPlatformBlueprintID:
                    case DBConstants.ProteusPropulsionWakeLimiterBlueprintID:
                    case DBConstants.TenguBlueprintID:
                    case DBConstants.TenguDefensiveAdaptiveShieldingBlueprintID:
                    case DBConstants.TenguElectronicsObfuscationManifoldBlueprintID:
                    case DBConstants.TenguEngineeringPowerCoreMultiplierBlueprintID:
                    case DBConstants.TenguOffensiveAcceleratedEjectionBayBlueprintID:
                    case DBConstants.TenguPropulsionIntercalatedNanofibersBlueprintID:
                        item.MarketGroupID = DBConstants.BlueprintTechIIINonMarketGroupID;
                        break;
                    case DBConstants.SmallEWDroneRangeAugmentorIIBlueprintID:
                        item.MarketGroupID = DBConstants.BlueprintTechIINonMarketGroupID;
                        break;
                }
            }

            // Set the market group of the blueprints with NULL MarketGroupID to custom market groups
            foreach (InvType item in s_types.Where(x => x.MarketGroupID == null && !x.Name.Contains("TEST")
                                                        && s_blueprintTypes.Any(y => y.ID == x.ID) &&
                                                        s_types.Any(z => z.ID == s_blueprintTypes[x.ID].ProductTypeID)
                                                        && s_types[s_blueprintTypes[x.ID].ProductTypeID].Published))
            {
                UpdatePercentDone(s_blueprintGenTotal);

                foreach (InvMetaType relation in s_metaTypes
                    .Where(x => x.ItemID == s_blueprintTypes[item.ID].ProductTypeID))
                {
                    switch (relation.MetaGroupID)
                    {
                        case DBConstants.TechIIMetaGroupID:
                            item.MarketGroupID = DBConstants.BlueprintTechIINonMarketGroupID;
                            break;
                        case DBConstants.StorylineMetaGroupID:
                            item.MarketGroupID = DBConstants.BlueprintStorylineNonMarketGroupID;
                            break;
                        case DBConstants.FactionMetaGroupID:
                            item.MarketGroupID = DBConstants.BlueprintFactionNonMarketGroupID;
                            break;
                        case DBConstants.OfficerMetaGroupID:
                            item.MarketGroupID = DBConstants.BlueprintOfficerNonMarketGroupID;
                            break;
                        case DBConstants.TechIIIMetaGroupID:
                            item.MarketGroupID = DBConstants.BlueprintTechIIINonMarketGroupID;
                            break;
                    }
                }

                if (item.MarketGroupID == null)
                    item.MarketGroupID = DBConstants.BlueprintTechINonMarketGroupID;
            }
        }

        /// <summary>
        /// Add properties to a blueprint.
        /// </summary>
        /// <param name="srcBlueprint"></param>
        /// <param name="blueprintsGroup"></param>
        /// <returns></returns>
        private static void CreateBlueprint(InvType srcBlueprint, List<SerializableBlueprint> blueprintsGroup)
        {
            UpdatePercentDone(s_blueprintGenTotal);

            srcBlueprint.Generated = true;

            // Creates the blueprint with base informations
            SerializableBlueprint blueprint = new SerializableBlueprint
                                                   {
                                                       ID = srcBlueprint.ID,
                                                       Name = srcBlueprint.Name,
                                                   };

            // Icon
            blueprint.Icon = (srcBlueprint.IconID.HasValue ? s_icons[srcBlueprint.IconID.Value].Icon : String.Empty);

            // Export attributes
            foreach (InvBlueprintTypes attribute in s_blueprintTypes.Where(x => x.ID == srcBlueprint.ID))
            {
                blueprint.ProduceItemID = attribute.ProductTypeID;
                blueprint.ProductionTime = attribute.ProductionTime;
                blueprint.TechLevel = attribute.TechLevel;
                blueprint.ResearchProductivityTime = attribute.ResearchProductivityTime;
                blueprint.ResearchMaterialTime = attribute.ResearchMaterialTime;
                blueprint.ResearchCopyTime = attribute.ResearchCopyTime;
                blueprint.ResearchTechTime = attribute.ResearchTechTime;
                blueprint.ProductivityModifier = attribute.ProductivityModifier;
                blueprint.WasteFactor = attribute.WasteFactor;
                blueprint.MaxProductionLimit = attribute.MaxProductionLimit;
            }

            // Metagroup
            foreach (InvMetaType relation in s_metaTypes
                .Where(x => x.ItemID == s_blueprintTypes[srcBlueprint.ID].ProductTypeID))
            {
                switch (relation.MetaGroupID)
                {
                    default:
                    case DBConstants.TechIMetaGroupID:
                        blueprint.MetaGroup = ItemMetaGroup.T1;
                        break;
                    case DBConstants.TechIIMetaGroupID:
                        blueprint.MetaGroup = ItemMetaGroup.T2;
                        break;
                    case DBConstants.StorylineMetaGroupID:
                        blueprint.MetaGroup = ItemMetaGroup.Storyline;
                        break;
                    case DBConstants.FactionMetaGroupID:
                        blueprint.MetaGroup = ItemMetaGroup.Faction;
                        break;
                    case DBConstants.OfficerMetaGroupID:
                        blueprint.MetaGroup = ItemMetaGroup.Officer;
                        break;
                    case DBConstants.DeadspaceMetaGroupID:
                        blueprint.MetaGroup = ItemMetaGroup.Deadspace;
                        break;
                    case DBConstants.TechIIIMetaGroupID:
                        blueprint.MetaGroup = ItemMetaGroup.T3;
                        break;
                }
            }

            // Metagroup for the custom market groups
            switch (srcBlueprint.MarketGroupID)
            {
                case DBConstants.BlueprintStorylineNonMarketGroupID:
                    blueprint.MetaGroup = ItemMetaGroup.Storyline;
                    break;
                case DBConstants.BlueprintFactionNonMarketGroupID:
                    blueprint.MetaGroup = ItemMetaGroup.Faction;
                    break;
                case DBConstants.BlueprintTechIIINonMarketGroupID:
                    blueprint.MetaGroup = ItemMetaGroup.T3;
                    break;
                case DBConstants.BlueprintTechIINonMarketGroupID:
                    blueprint.MetaGroup = ItemMetaGroup.T2;
                    break;
            }

            if (blueprint.MetaGroup == ItemMetaGroup.Empty)
                blueprint.MetaGroup = ItemMetaGroup.T1;

            // Export item requirements
            ExportRequirements(srcBlueprint, blueprint);

            // Look for the tech 2 variations that this blueprint invents
            List<int> inventionBlueprint = new List<int>();
            foreach (int relationItemID in s_metaTypes
                .Where(x => x.ParentItemID == blueprint.ProduceItemID && x.MetaGroupID == DBConstants.TechIIMetaGroupID)
                .Select(x => x.ItemID))
            {
                // Look for a blueprint that produces the related item
                foreach (int variationItemID in s_blueprintTypes
                    .Where(x => x.ProductTypeID == relationItemID).Select(x => x.ID))
                {
                    // Add the variation blueprint
                    inventionBlueprint.Add(s_types[variationItemID].ID);
                }
            }

            // Add invention blueprints to item
            blueprint.InventionTypeID = inventionBlueprint.ToArray();

            // Add this item
            blueprintsGroup.Add(blueprint);
        }

        /// <summary>
        /// Export item requirements. 
        /// </summary>
        /// <param name="srcBlueprint"></param>
        /// <param name="blueprint"></param>
        private static void ExportRequirements(InvType srcBlueprint, SerializableBlueprint blueprint)
        {
            List<SerializablePrereqSkill> prerequisiteSkills = new List<SerializablePrereqSkill>();
            List<SerializableRequiredMaterial> requiredMaterials = new List<SerializableRequiredMaterial>();

            // Add the required raw materials
            AddRequiredRawMaterials(blueprint.ProduceItemID, requiredMaterials);

            // Add the required extra materials
            AddRequiredExtraMaterials(srcBlueprint.ID, prerequisiteSkills, requiredMaterials);

            // Add prerequisite skills to item
            blueprint.PrereqSkill = prerequisiteSkills.OrderBy(x => x.Activity).ToArray();

            // Add required materials to item
            blueprint.ReqMaterial = requiredMaterials.OrderBy(x => x.Activity).ToArray();
        }

        /// <summary>
        /// Adds the raw materials needed to produce an item.
        /// </summary>
        /// <param name="blueprint"></param>
        /// <param name="requiredMaterials"></param>
        private static void AddRequiredRawMaterials(int produceItemID,
                                                    List<SerializableRequiredMaterial> requiredMaterials)
        {
            // Find the raw materials needed for the produced item and add them to the list
            foreach (InvTypeMaterials reprocItem in s_typeMaterials.Where(x => x.TypeID == produceItemID))
            {
                requiredMaterials.Add(new SerializableRequiredMaterial
                                          {
                                              ID = reprocItem.MaterialTypeID,
                                              Quantity = reprocItem.Quantity,
                                              DamagePerJob = 1,
                                              Activity = (int) BlueprintActivity.Manufacturing,
                                              WasteAffected = 1
                                          });
            }
        }

        /// <summary>
        /// Adds the extra materials needed to produce an item.
        /// </summary>
        /// <param name="srcBlueprint"></param>
        /// <param name="prerequisiteSkills"></param>
        /// <param name="requiredMaterials"></param>
        private static void AddRequiredExtraMaterials(int blueprintID,
                                                      List<SerializablePrereqSkill> prerequisiteSkills,
                                                      List<SerializableRequiredMaterial> requiredMaterials)
        {
            // Find the additional extra materials and add them to the list
            foreach (RamTypeRequirements requirement in s_typeRequirements.Where(x => x.TypeID == blueprintID))
            {
                // Is it a skill ? Add it to the prerequisities skills list
                if (s_types.Any(x => x.ID == requirement.RequiredTypeID
                                    && s_groups.Any(y => y.ID == x.GroupID
                                    && y.CategoryID == DBConstants.SkillCategoryID)))
                {
                    prerequisiteSkills.Add(new SerializablePrereqSkill
                                               {
                                                   ID = requirement.RequiredTypeID,
                                                   Level = requirement.Quantity,
                                                   Activity = requirement.ActivityID
                                               });
                }
                else // It is an item (extra material)
                {
                    requiredMaterials.Add(new SerializableRequiredMaterial
                                              {
                                                  ID = requirement.RequiredTypeID,
                                                  Quantity = requirement.Quantity,
                                                  DamagePerJob = requirement.DamagePerJob,
                                                  Activity = requirement.ActivityID,
                                                  WasteAffected = 0
                                              });

                    // If the item is recyclable, we need to find the materials produced by reprocessing it
                    // and substracted them from the related materials of the requiredMaterials list
                    if (requirement.Recyclable)
                    {
                        foreach (InvTypeMaterials reprocItem in s_typeMaterials
                                .Where(x => x.TypeID == requirement.RequiredTypeID))
                        {
                            if (requiredMaterials.Any(x => x.ID == reprocItem.MaterialTypeID))
                            {
                                SerializableRequiredMaterial material = requiredMaterials
                                    .First(x => x.ID == reprocItem.MaterialTypeID);

                                material.Quantity -= requirement.Quantity*reprocItem.Quantity;

                                if (material.Quantity < 1)
                                    requiredMaterials.Remove(material);
                            }
                        }
                    }

                    // If activity is invention, add the prerequisite skill
                    // of the required material as it's not included in this table
                    if (requirement.ActivityID == (int)BlueprintActivity.Invention)
                    {
                        // Add the prerequisite skills for a material used in invention activity.
                        MaterialPrereqSkill(requirement, prerequisiteSkills);
                    }
                }
            }
        }

        /// <summary>
        /// Add the prerequisite skills for a material used in invention activity.
        /// </summary>
        private static void MaterialPrereqSkill(RamTypeRequirements requirement,
                                                List<SerializablePrereqSkill> prerequisiteSkills)
        {
            int[] prereqSkills = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            int[] prereqLevels = new int[DBConstants.RequiredSkillPropertyIDs.Length];

            foreach (DgmTypeAttribute attribute in s_typeAttributes
                    .Where(x => x.ItemID == requirement.RequiredTypeID))
            {
                int attributeIntValue = (attribute.ValueInt.HasValue
                                                ? attribute.ValueInt.Value
                                                : (int)attribute.ValueFloat.Value);

                // Is it a prereq skill ?
                int prereqIndex = Array.IndexOf(DBConstants.RequiredSkillPropertyIDs, attribute.AttributeID);
                if (prereqIndex >= 0)
                {
                    prereqSkills[prereqIndex] = attributeIntValue;
                    continue;
                }

                // Is it a prereq level ?
                prereqIndex = Array.IndexOf(DBConstants.RequiredSkillLevelPropertyIDs, attribute.AttributeID);
                if (prereqIndex >= 0)
                {
                    prereqLevels[prereqIndex] = attributeIntValue;
                    continue;
                }
            }

            // Add the prerequisite skills
            for (int i = 0; i < prereqSkills.Length; i++)
            {
                if (prereqSkills[i] != 0)
                    prerequisiteSkills.Add(new SerializablePrereqSkill
                                               {
                                                   ID = prereqSkills[i],
                                                   Level = prereqLevels[i],
                                                   Activity = requirement.ActivityID
                                               });
            }
        }

        #endregion

        #region Geography Datafile

        /// <summary>
        /// Generates the geo datafile.
        /// </summary>
        private static void GenerateGeography()
        {
            Console.WriteLine();
            Console.Write(@"Generated geography datafile... ");

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
            s_startTime = DateTime.Now;

            List<SerializableSolarSystem> allSystems = new List<SerializableSolarSystem>();
            List<SerializableRegion> regions = new List<SerializableRegion>();

            // Regions
            foreach (MapRegion srcRegion in s_regions)
            {
                SerializableRegion region = new SerializableRegion
                                                 {
                                                     ID = srcRegion.ID,
                                                     Name = srcRegion.Name
                                                 };
                regions.Add(region);

                // Constellations
                List<SerializableConstellation> constellations = new List<SerializableConstellation>();
                foreach (MapConstellation srcConstellation in s_constellations.Where(x => x.RegionID == srcRegion.ID))
                {
                    SerializableConstellation constellation = new SerializableConstellation
                                                                    {
                                                                        ID = srcConstellation.ID,
                                                                        Name = srcConstellation.Name
                                                                    };
                    constellations.Add(constellation);

                    // Systems
                    const double baseDistance = 1.0E14;
                    List<SerializableSolarSystem> systems = new List<SerializableSolarSystem>();
                    foreach (
                        MapSolarSystem srcSystem in s_solarSystems.Where(x => x.ConstellationID == srcConstellation.ID))
                    {
                        SerializableSolarSystem system = new SerializableSolarSystem
                                                             {
                                                                 ID = srcSystem.ID,
                                                                 Name = srcSystem.Name,
                                                                 X = (int) (srcSystem.X/baseDistance),
                                                                 Y = (int) (srcSystem.Y/baseDistance),
                                                                 Z = (int) (srcSystem.Z/baseDistance),
                                                                 SecurityLevel = srcSystem.SecurityLevel
                                                             };
                        systems.Add(system);

                        // Stations
                        List<SerializableStation> stations = new List<SerializableStation>();
                        foreach (StaStation srcStation in s_stations.Where(x => x.SolarSystemID == srcSystem.ID))
                        {
                            UpdatePercentDone(s_geoGen);

                            // Agents
                            List<SerializableAgent> stationAgents = new List<SerializableAgent>();
                            foreach (AgtAgents srcAgent in s_agents.Where(x => x.LocationID == srcStation.ID))
                            {
                                AgtResearchAgents researchAgent = s_researchAgents.FirstOrDefault(x => x.ID == srcAgent.ID);
                                AgtConfig agentConfig = s_agentConfig.FirstOrDefault(x => x.ID == srcAgent.ID);
                                SerializableAgent agent = new SerializableAgent
                                                                {
                                                                    ID = srcAgent.ID,
                                                                    Level = srcAgent.Level,
                                                                    Quality = srcAgent.Quality,
                                                                    Name = s_names.FirstOrDefault(x => x.ID == srcAgent.ID).Name,
                                                                    DivisionName = s_npcDivisions.FirstOrDefault(x => x.ID == srcAgent.DivisionID).DivisionName,
                                                                    AgentType = s_agentTypes.FirstOrDefault(x => x.ID == srcAgent.AgentTypeID).AgentType,
                                                                    ResearchSkillID = (researchAgent != null ? researchAgent.ResearchSkillID : 0),
                                                                    LocatorService = (agentConfig != null ?
                                                                                        agentConfig.Key.Contains("agent.LocateCharacterService.enabled") :
                                                                                        false)
                                                                };
                                stationAgents.Add(agent);
                            }

                            SerializableStation station = new SerializableStation
                                                              {
                                                                  ID = srcStation.ID,
                                                                  Name = srcStation.Name,
                                                                  CorporationID = srcStation.CorporationID,
                                                                  CorporationName = s_names.FirstOrDefault(x => x.ID == srcStation.CorporationID).Name,
                                                                  ReprocessingEfficiency = srcStation.ReprocessingEfficiency,
                                                                  ReprocessingStationsTake = srcStation.ReprocessingStationsTake,
                                                                  Agents = stationAgents.ToArray()
                                                              };
                            stations.Add(station);
                        }
                        system.Stations = stations.OrderBy(x => x.Name).ToArray();
                    }
                    constellation.Systems = systems.OrderBy(x => x.Name).ToArray();
                }
                region.Constellations = constellations.OrderBy(x => x.Name).ToArray();
            }

            // Jumps
            List<SerializableJump> jumps = new List<SerializableJump>();
            foreach (MapSolarSystemJump srcJump in s_jumps)
            {
                UpdatePercentDone(s_geoGenTotal);

                // In CCP tables, every jump is included twice, we only need one.
                if (srcJump.A < srcJump.B)
                    jumps.Add(new SerializableJump {FirstSystemID = srcJump.A, SecondSystemID = srcJump.B});
            }

            s_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", s_endTime.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            GeoDatafile datafile = new GeoDatafile();
            datafile.Regions = regions.OrderBy(x => x.Name).ToArray();
            datafile.Jumps = jumps.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.GeographyDatafile);
        }

        #endregion

        #region Reprocessing Datafile

        /// <summary>
        /// Generates the reprocessing datafile.
        /// </summary>
        private static void GenerateReprocessing()
        {
            Console.WriteLine();
            Console.Write(@"Generated reprocessing datafile... ");

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
            s_startTime = DateTime.Now;

            List<SerializableItemMaterials> types = new List<SerializableItemMaterials>();

            foreach (int typeID in s_types.Where(x => x.Generated).Select(x => x.ID))
            {
                UpdatePercentDone(s_reprocessGenTotal);

                List<SerializableMaterialQuantity> materials = new List<SerializableMaterialQuantity>();
                foreach (InvTypeMaterials srcMaterial in s_typeMaterials.Where(x => x.TypeID == typeID))
                {
                    materials.Add(new SerializableMaterialQuantity
                                        { 
                                            ID = srcMaterial.MaterialTypeID,
                                            Quantity = srcMaterial.Quantity
                                        });
                }

                if (materials.Count != 0)
                {
                    types.Add(new SerializableItemMaterials
                                  {
                                      ID = typeID,
                                      Materials = materials.OrderBy(x => x.ID).ToArray()
                                  });
                }
            }

            s_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", s_endTime.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            ReprocessingDatafile datafile = new ReprocessingDatafile();
            datafile.Items = types.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.ReprocessingDatafile);
        }

        #endregion

        #region MD5Sums

        /// <summary>
        /// Generates the MD5Sums file.
        /// </summary>
        private static void GenerateMD5Sums()
        {
            Util.CreateMD5SumsFile("MD5Sums.txt");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates the percantage done of the datafile generating procedure.
        /// </summary>
        /// <param name="total"></param>
        private static void UpdatePercentDone(double total)
        {
            s_counter++;
            int percent = (int)((s_counter / total) * 100);

            if (s_counter == 1 || s_percentOld < percent)
            {
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                s_text = String.Format("{0}%", percent);
                Console.Write(s_text);
                s_percentOld = percent;
            }
        }

        /// <summary>
        /// Updates the progress of data loaded from SQL server.
        /// </summary>
        private static void UpdateProgress()
        {
            Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
            s_tablesCount++;
            s_text = String.Format("{0}%", (int) ((s_tablesCount/s_totalTablesCount)*100));
            Console.Write(s_text);
        }

        #endregion
    }
}