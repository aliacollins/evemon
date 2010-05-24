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
    class Program
    {
        private static string m_text = String.Empty;
        private static double m_counter;
        private static double m_tablesCount = 0;
        private static double m_totalTablesCount = 23;
        
        private static int m_percentOld;
        private static int m_propGenTotal = 1433;
        private static int m_itemGenTotal = 8282;
        private static int m_skillGenTotal = 464;
        private static int m_certGenTotal = 3828;
        private static int m_blueprintGenTotal = 3814;
        private static int m_geoGen = 5219;
        private static int m_geoGenTotal = 19501;
        private static int m_reprocessGenTotal = 9585; 

        private static DateTime m_startTime;
        private static DateTime m_endTime;

        private static int s_propBasePriceID;
        private static List<InvMarketGroup> m_injectedMarketGroups;

        private static Bag<EveUnit> s_units;
        private static Bag<EveGraphic> s_graphics;
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


        #region Constructor

        static void Main(string[] args)
        {
            // Setting a standard format for the generated files
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            // Data dumps are available from CCP
            Console.Write("Loading Data from SQL Server... ");

            #region Read Tables From Database

            // Read tables from database
            s_units = Database.Units();
            UpdateProgress();
            s_graphics = Database.Graphics();
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
            Console.WriteLine("Datafile Generating In Progress");
            Console.WriteLine();

            GenerateProperties();
            GenerateItems(); // Requires GenerateProperties()
            GenerateSkills();
            GenerateCertificates();
            GenerateBlueprints();
            GenerateGeography();
            GenerateReprocessing(); // Requires GenerateItems()
            
            GenerateMD5Sums();

            Console.WriteLine("Done");
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
            Console.Write("Generated properties datafile... ");

            m_counter = 0;
            m_percentOld = 0;
            m_text = String.Empty;
            m_startTime = DateTime.Now;
            
            int newID = 0;

            // Change some display names and default values
            s_attributeTypes[9].DisplayName = "Structure HP";
            s_attributeTypes[263].DisplayName = "Shield HP";
            s_attributeTypes[265].DisplayName = "Armor HP";

            s_attributeTypes[38].DisplayName = "Cargo Capacity";
            s_attributeTypes[48].DisplayName = "CPU";
            s_attributeTypes[11].DisplayName = "Powergrid";

            // Shield
            s_attributeTypes[271].DisplayName = "EM Resistance";
            s_attributeTypes[272].DisplayName = "Explosive Resistance";
            s_attributeTypes[273].DisplayName = "Kinetic Resistance";
            s_attributeTypes[274].DisplayName = "Thermal Resistance";

            // Armor
            s_attributeTypes[267].DisplayName = "EM Resistance";
            s_attributeTypes[268].DisplayName = "Explosive Resistance";
            s_attributeTypes[269].DisplayName = "Kinetic Resistance";
            s_attributeTypes[270].DisplayName = "Thermal Resistance";

            // Hull
            s_attributeTypes[974].DisplayName = "EM Resistance";
            s_attributeTypes[975].DisplayName = "Explosive Resistance";
            s_attributeTypes[976].DisplayName = "Kinetic Resistance";
            s_attributeTypes[977].DisplayName = "Thermal Resistance";

            // Items attribute
            s_attributeTypes[6].DisplayName = "Activation cost";
            s_attributeTypes[30].DisplayName = "Powergrid usage";
            s_attributeTypes[68].DisplayName = "Shield Bonus";
            s_attributeTypes[87].DisplayName = "Shield Transfer Range";
            s_attributeTypes[116].DisplayName = "Explosive damage";
            s_attributeTypes[424].DisplayName = "CPU Output Bonus";
            s_attributeTypes[1082].DisplayName = "CPU Penalty";
            
            // Changing the categoryID for some attributes 
            s_attributeTypes[1132].CategoryID = 1; // Calibration
            s_attributeTypes[1547].CategoryID = 1; // Rig Size
            s_attributeTypes[908].CategoryID = 4; // Ship Maintenance Bay Capacity

            // Changing HigherIsBetter to false (CCP has this wrong?)
            s_attributeTypes[30].HigherIsBetter = false; // CPU usage
            s_attributeTypes[50].HigherIsBetter = false; // PG usage
            s_attributeTypes[161].HigherIsBetter = false; // Volume
            s_attributeTypes[70].HigherIsBetter = false; // Inertia Modifier
            s_attributeTypes[4].HigherIsBetter = false; // Mass
            s_attributeTypes[6].HigherIsBetter = false; // Activation Cost
            s_attributeTypes[55].HigherIsBetter = false; // Capacitor recharge time
            s_attributeTypes[479].HigherIsBetter = false; // Shield recharge time
            s_attributeTypes[552].HigherIsBetter = false; // Signature radius
            s_attributeTypes[560].HigherIsBetter = false; // Sensor Recalibration Time
            s_attributeTypes[1082].HigherIsBetter = false; // CPU Penalty
            s_attributeTypes[1153].HigherIsBetter = false; // Calibration cost
            s_attributeTypes[1272].HigherIsBetter = false; // Bandwidth Needed
            s_attributeTypes[1416].HigherIsBetter = false; // Target Switch Timer
            s_attributeTypes[73].HigherIsBetter = false; // Activation time / duration
            s_attributeTypes[556].HigherIsBetter = false; // Anchoring Delay
            s_attributeTypes[676].HigherIsBetter = false; // Unanchoring Delay
            s_attributeTypes[677].HigherIsBetter = false; // Onlining Delay
            s_attributeTypes[780].HigherIsBetter = false; // Cycle Time bonus

            // Export attribute categories
            var categories = new List<SerializablePropertyCategory>();

            // We insert custom categories
            var general = new SerializablePropertyCategory { Name = "General", Description = "General informations" };
            var propulsion = new SerializablePropertyCategory { Name = "Propulsion", Description = "Navigation attributes for ships" };
            var g_properties = new List<SerializableProperty>();
            var p_properties = new List<SerializableProperty>();
            categories.Insert(0, general);
            categories.Insert(0, propulsion);

            foreach (var srcCategory in s_attributeCategories)
            {
                var category = new SerializablePropertyCategory();
                categories.Add(category);

                category.Description = srcCategory.Description;
                category.Name = srcCategory.Name;

                // Export attributes
                var properties = new List<SerializableProperty>();

                foreach (var srcProp in s_attributeTypes.Where(x => x.CategoryID == srcCategory.ID))
                {
                    UpdatePercentDone(m_propGenTotal);

                    var prop = new SerializableProperty();
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
                    prop.Icon = (srcProp.GraphicID == null ? String.Empty : s_graphics[srcProp.GraphicID.Value].Icon);

                    // Reordering some properties
                    int index = properties.IndexOf(prop);
                    switch (srcProp.ID)
                    {
                        case 9: 
                            properties.Insert(0, prop); 
                            properties.RemoveAt(index + 1); 
                            break;
                        case 37:
                            p_properties.Insert(0, prop); 
                            properties.RemoveAt(index); 
                            break;
                        case 38: 
                            properties.Insert(1, prop); 
                            properties.RemoveAt(index + 1); 
                            break;
                        case 48: 
                            properties.Insert(0, prop); 
                            properties.RemoveAt(index + 1);
                            break;
                        case 70: 
                            properties.Insert(3, prop); 
                            properties.RemoveAt(index + 1); 
                            break;
                        case 161: 
                            properties.Insert(3, prop);
                            properties.RemoveAt(index + 1);
                            break;
                        case 422: 
                            g_properties.Insert(0, prop);
                            properties.RemoveAt(index);
                            break;
                        case 479: 
                            properties.Insert(6, prop); 
                            properties.RemoveAt(index + 1); 
                            break;
                        case 482: 
                            properties.Insert(0, prop); 
                            properties.RemoveAt(index + 1);
                            break;
                        case 564: 
                            properties.Insert(4, prop);
                            properties.RemoveAt(index + 1);
                            break;
                        case 633: 
                            g_properties.Insert(1, prop); 
                            properties.RemoveAt(index);
                            break;
                        case 974: properties.Insert(5, prop);
                            properties.RemoveAt(index + 1);
                            break;
                        case 975:
                            properties.Insert(6, prop);
                            properties.RemoveAt(index + 1);
                            break;
                        case 976:
                            properties.Insert(7, prop);
                            properties.RemoveAt(index + 1);
                            break;
                        case 977:
                            properties.Insert(8, prop);
                            properties.RemoveAt(index + 1); 
                            break;
                        case 1132:
                            properties.Insert(2, prop);
                            properties.RemoveAt(index + 1); 
                            break;
                        case 1137:
                            properties.Insert(10, prop);
                            properties.RemoveAt(index + 1); 
                            break;
                        case 1281:
                            p_properties.Insert(1, prop);
                            properties.RemoveAt(index);
                            break;
                        case 1547:
                            properties.Insert(11, prop); 
                            properties.RemoveAt(index + 1); 
                            break;
                        default:
                            break;
                    }

                    // New ID
                    newID = Math.Max(newID, srcProp.ID);
                }
                category.Properties = properties.ToArray();
            }

            // Add EVEMon custom properties (Base Price)
            var gprop = new SerializableProperty();
            s_propBasePriceID = newID + 1;
            g_properties.Insert(0, gprop);
            gprop.ID = s_propBasePriceID;
            gprop.Name = "Base Price";
            gprop.Unit = "ISK";
            gprop.Description = "The price from NPC vendors (does not mean there is any).";

            // Add properties to custom categories
            general.Properties = g_properties.ToArray();
            propulsion.Properties = p_properties.ToArray();

            // Sort groups
            string[] orderedGroupNames = { "General", "Fitting", "Drones", "Structure", "Armor", "Shield", "Capacitor", "Targeting", "Propulsion", "Miscellaneous", "NULL" };

            m_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", m_endTime.Subtract(m_startTime)).TrimEnd('0'));

            // Serialize
            PropertiesDatafile datafile = new PropertiesDatafile();
            datafile.Categories = categories.OrderBy(x => Array.IndexOf(orderedGroupNames, String.Intern(x.Name))).ToArray();
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
            Console.Write("Generated items datafile... ");

            m_counter = 0;
            m_percentOld = 0;
            m_text = String.Empty;
            m_startTime = DateTime.Now;

            m_injectedMarketGroups = new List<InvMarketGroup>();

            // Create custom market groups that don't exist in EVE
            m_injectedMarketGroups.Add(
                new InvMarketGroup()
                {
                    Name = "Unique Designs",
                    Description = "Ships of a unique design",
                    ID = 10000,
                    ParentID = 4,
                    GraphicID = 1443
                });

            m_injectedMarketGroups.Add(
                new InvMarketGroup()
                {
                    Name = "Unique Shuttles",
                    Description = "Fast ships of a unique design",
                    ID = 10001,
                    ParentID = 10000,
                    GraphicID = null
                });

            m_injectedMarketGroups.Add(
                new InvMarketGroup()
                {
                    Name = "Various Non-Market",
                    Description = "Non-Market Items",
                    ID = 10002,
                    ParentID = null,
                    GraphicID = null
                });

            // Manually set some items attributes
            s_types[27029].Published = true;
            s_types[32077].Published = true;

            // Set some attributes to items because their MarketGroupID is NULL
            foreach (var srcItem in s_types.Where(x => x.Published && x.MarketGroupID == null))
            {
                // Set some ships market group and race
                switch (srcItem.ID)
                {
                    case 17703: // Imperial Navy Slicer
                    case 17926: // Cruor
                    case 17924: // Succubus
                        srcItem.MarketGroupID = 72;
                        srcItem.RaceID = 32;
                        break; 
                    case 17619: // Caldari Navy Hookbill
                        srcItem.MarketGroupID = 61;
                        srcItem.RaceID = 32;
                        break;
                    case 17841: // Federation Navy Comet
                        srcItem.MarketGroupID = 77;
                        srcItem.RaceID = 32;
                        break;
                    case 17812: // Republic Fleet Firetail
                    case 17932: // Dramiel
                    case 17928: // Daredevil
                        srcItem.MarketGroupID = 64;
                        srcItem.RaceID = 32;
                        break;
                    case 17736: // Nightmare
                        srcItem.MarketGroupID = 79;
                        srcItem.RaceID = 32;
                        break;
                    case 17738: // Machariel
                        srcItem.MarketGroupID = 78;
                        srcItem.RaceID = 32;
                        break;
                    case 17720: // Cynabal
                        srcItem.MarketGroupID = 73;
                        srcItem.RaceID = 32;
                        break;
                    case 17922: // Ashimmu
                    case 17718: // Phantasm
                        srcItem.MarketGroupID = 74;
                        srcItem.RaceID = 32;
                        break;
                    case 21097: // Goru's Shuttle
                    case 21628: // Guristas Shuttle
                        srcItem.MarketGroupID = 396;
                        srcItem.RaceID = 32;
                        break;
                    case 30842: // Interbus Shuttle
                        srcItem.MarketGroupID = 395;
                        srcItem.RaceID = 32;
                        break;
                    case 29266: // Apotheosis
                    case 2078: // Zephyr
                        srcItem.MarketGroupID = 10001;
                        srcItem.RaceID = 32;
                        break;

                    // Set some items market group to support blueprints
                    case 21815:
                    case 23882:
                    case 24289:
                    case 27029:
                    case 27038:
                    case 29203:
                    case 29226:
                    case 10260:
                    case 19758:
                    case 10257:
                    case 10258:
                    case 32077:
                    case 30424:
                    case 30839:
                    case 30422:
                    case 30342:
                    case 30420:
                    case 30328:
                    case 21054:
                    case 17761:
                    case 23883:
                    case 29202:
                    case 21510:
                    case 22335:
                    case 22333:
                    case 22337:
                    case 22339:
                    case 22923:
                    case 22329:
                    case 22327:
                    case 22325:
                    case 22331:
                        srcItem.MarketGroupID = 10002;
                        break;
                }

                // Set some items market group to support blueprints
                if (srcItem.ID >28685 && srcItem.ID < 28694)
                    srcItem.MarketGroupID = 10002;
            }

            var groups = new Dictionary<int, SerializableMarketGroup>();

            // Create the market groups
            foreach (var srcGroup in s_marketGroups.Concat(m_injectedMarketGroups))
            {
                var group = new SerializableMarketGroup { ID = srcGroup.ID, Name = srcGroup.Name };
                groups[srcGroup.ID] = group;

                // Add the items in this group
                var items = new List<SerializableItem>();
                foreach (var srcItem in s_types.Where(x => x.Published && (x.MarketGroupID.GetValueOrDefault() == srcGroup.ID)))
                {
                    CreateItem(srcItem, items);
                }

                // If this is an implant group, we add the implants with no market groups in this one.
                if (srcGroup.ParentID == 531 || srcGroup.ParentID == 532) 
                {
                    var slotString = srcGroup.Name.Substring("Implant Slot ".Length);
                    int slot = Int32.Parse(slotString);

                    // Enumerate all implants without market groups
                    foreach (var srcItem in s_types.Where(x => x.MarketGroupID == null && s_groups[x.GroupID].CategoryID == 20 && x.GroupID != 745))
                    {
                        // Check the slot matches
                        var slotAttrib = s_typeAttributes.Get(srcItem.ID, DBConstants.ImplantSlotPropertyID);
                        if (slotAttrib != null && slotAttrib.GetIntValue() == slot)
                            CreateItem(srcItem, items);
                    }
                }

                // Store the items
                group.Items = items.OrderBy(x => x.Name).ToArray();
            }

            // Create the parent-children groups relations
            foreach (var group in groups.Values)
            {
                var children = s_marketGroups.Concat(m_injectedMarketGroups).Where(x => x.ParentID.GetValueOrDefault() == group.ID).Select(x => groups[x.ID]);
                group.SubGroups = children.OrderBy(x => x.Name).ToArray();
            }

            // Pick the family
            SetItemFamilyByMarketGroup(groups[2], ItemFamily.Bpo);
            SetItemFamilyByMarketGroup(groups[4], ItemFamily.Ship);
            SetItemFamilyByMarketGroup(groups[27], ItemFamily.Implant);
            SetItemFamilyByMarketGroup(groups[157], ItemFamily.Drone);
            SetItemFamilyByMarketGroup(groups[477], ItemFamily.StarbaseStructure);

            // Sort groups
            var rootGroups = s_marketGroups.Concat(m_injectedMarketGroups).Where(x => !x.ParentID.HasValue).Select(x => groups[x.ID]).OrderBy(x => x.Name);

            m_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", m_endTime.Subtract(m_startTime)).TrimEnd('0'));

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
            UpdatePercentDone(m_itemGenTotal);

            srcItem.Generated = true;

            // Creates the item with base informations
            var item = new SerializableItem
            {
                ID = srcItem.ID,
                Name = srcItem.Name,
                Description = srcItem.Description
            };

            // Icon
            item.Icon = (srcItem.GraphicID.HasValue ? s_graphics[srcItem.GraphicID.Value].Icon : String.Empty);

            // Add the properties and prereqs
            int baseWarpSpeed = 3;
            double warpSpeedMultiplier = 1;
            var props = new List<SerializablePropertyValue>();
            var prereqSkills = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            var prereqLevels = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            foreach (var srcProp in s_typeAttributes.Where(x => x.ItemID == srcItem.ID))
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
                var launcherIndex = Array.IndexOf(DBConstants.LauncherGroupIDs, srcProp.AttributeID);
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
                var chargeIndex = Array.IndexOf(DBConstants.ChargeGroupIDs, srcProp.AttributeID);
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
                var canFitShipIndex = Array.IndexOf(DBConstants.CanFitShipGroupIDs, srcProp.AttributeID);
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
                var moduleShipIndex = Array.IndexOf(DBConstants.ModuleShipGroupIDs, srcProp.AttributeID);
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
                var specialisationAsteroidIndex = Array.IndexOf(DBConstants.SpecialisationAsteroidGroupIDs, srcProp.AttributeID);
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
                var reactionIndex = Array.IndexOf(DBConstants.ReactionGroupIDs, srcProp.AttributeID);
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
                var posCargobayAcceptIndex = Array.IndexOf(DBConstants.PosCargobayAcceptGroupIDs, srcProp.AttributeID);
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

                // We calculate the Ships Warp Speed
                if (srcProp.AttributeID == DBConstants.ShipWarpSpeedPropertyID)
                    props.Add(new SerializablePropertyValue { ID = srcProp.AttributeID, Value = (baseWarpSpeed * warpSpeedMultiplier).ToString() });
                
                // Other props
                props.Add(new SerializablePropertyValue { ID = srcProp.AttributeID, Value = srcProp.FormatPropertyValue() });

                // Is metalevel property ?
                if (srcProp.AttributeID == DBConstants.MetaLevelPropertyID)
                    item.MetaLevel = propIntValue;
            }

            // Ensures there is a mass and add it to prop
            if (srcItem.Mass != 0)
                props.Add(new SerializablePropertyValue { ID = DBConstants.MassPropertyID, Value = srcItem.Mass.ToString() });

            // Ensures there is a cargo capacity and add it to prop
            if (srcItem.Capacity != 0)
                props.Add(new SerializablePropertyValue { ID = DBConstants.CargoCapacityPropertyID, Value = srcItem.Capacity.ToString() });

            // Ensures there is a volume and add it to prop
            if (srcItem.Volume != 0)
                props.Add(new SerializablePropertyValue { ID = DBConstants.VolumePropertyID, Value = srcItem.Volume.ToString() });

            // Add base price as a prop
            props.Add(new SerializablePropertyValue { ID = s_propBasePriceID, Value = srcItem.BasePrice.FormatDecimal() });

            // Add properties info to item
            item.Properties = props.ToArray();

            // Prerequisites completion
            var prereqs = new List<SerializablePrerequisiteSkill>();
            for (int i = 0; i < prereqSkills.Length; i++)
            {
                if (prereqSkills[i] != 0)
                    prereqs.Add(new SerializablePrerequisiteSkill { ID = prereqSkills[i], Level = prereqLevels[i] });
            }

            // Add prerequisite skills info to item
            item.Prereqs = prereqs.ToArray();

            // Metagroup
            item.MetaGroup = ItemMetaGroup.Empty;
            foreach (var relation in s_metaTypes.Where(x => x.ItemID == srcItem.ID))
            {
                switch (relation.MetaGroupID)
                {
                    case 1:
                        item.MetaGroup |= ItemMetaGroup.T1;
                        break;
                    case 2:
                        item.MetaGroup |= ItemMetaGroup.T2;
                        break;
                    case 3:
                        item.MetaGroup |= ItemMetaGroup.Storyline;
                        break;
                    case 4:
                        item.MetaGroup |= ItemMetaGroup.Faction;
                        break;
                    case 5:
                        item.MetaGroup |= ItemMetaGroup.Officer;
                        break;
                    case 6:
                        item.MetaGroup |= ItemMetaGroup.Deadspace;
                        break;
                    case 14:
                        item.MetaGroup |= ItemMetaGroup.T3;
                        break;
                    default:
                        item.MetaGroup |= ItemMetaGroup.Other;
                        break;
                }
            }

            if (item.MetaGroup == ItemMetaGroup.Empty)
                item.MetaGroup = ItemMetaGroup.T1;

            // Race ID
            item.Race = (Race)Enum.ToObject(typeof(Race), (srcItem.RaceID == null ? 0 : srcItem.RaceID));
            if (item.MetaGroup == ItemMetaGroup.Faction)
                item.Race = Race.Faction;

            // Set race to ORE if it is in the ORE market groups
            // within mining barges, exhumers or capital industrial ships
            if (srcItem.MarketGroupID == DBConstants.MiningBargesGroupID
                || srcItem.MarketGroupID == DBConstants.ExhumersGroupID
                || srcItem.MarketGroupID == DBConstants.CapitalIndustrialsGroupID)
                item.Race = Race.Ore;

            // Set race to Faction if ship has Pirate Faction property
            foreach (var prop in props)
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
            foreach (var variation in s_metaTypes.Where(x => x.ParentItemID == srcItem.ID))
            {
                var srcVariationItem = s_types[variation.ItemID];
                if (srcVariationItem.Published && srcVariationItem.MarketGroupID == null)
                {
                    srcVariationItem.RaceID = 32;
                    CreateItem(srcVariationItem, groupItems);
                }
            }
        }

        /// <summary>
        /// Sets the item family according to its market group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="itemFamily"></param>
        private static void SetItemFamilyByMarketGroup(SerializableMarketGroup group, ItemFamily itemFamily)
        {
            foreach (var item in group.Items)
            {
                item.Family = itemFamily;
            }

            foreach (var childGroup in group.SubGroups)
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
            Console.Write("Generated skills datafile... ");

            m_counter = 0;
            m_percentOld = 0;
            m_text = String.Empty;
            m_startTime = DateTime.Now;

            // Export skill groups
            List<SerializableSkillGroup> listOfSkillGroups = new List<SerializableSkillGroup>();

            foreach (var group in s_groups.Where(x => x.CategoryID == 16 && x.ID != 505).OrderBy(x => x.Name))
            {
                SerializableSkillGroup skillGroup = new SerializableSkillGroup
                {
                    ID = group.ID,
                    Name = group.Name,
                };

                // Export skills
                List<SerializableSkill> listOfSkillsInGroup = new List<SerializableSkill>();

                foreach (var skill in s_types.Where(x => x.GroupID == group.ID))
                {
                    UpdatePercentDone(m_skillGenTotal);

                    SerializableSkill singleSkill = new SerializableSkill
                    {
                        ID = skill.ID,
                        Name = skill.Name,
                        Description = skill.Description,
                        Public = skill.Published,
                        Cost = (long)skill.BasePrice,
                    };

                    // Export skill atributes
                    Dictionary<int, int> skillAttributes = new Dictionary<int, int>();

                    foreach (var attribute in s_typeAttributes.Where(x => x.ItemID == skill.ID))
                    {
                        skillAttributes.Add(attribute.AttributeID, attribute.GetIntValue());
                    }

                    if (skillAttributes.ContainsKey(275) && skillAttributes[275] > 0)
                    {
                        singleSkill.Rank = skillAttributes[275];
                    }
                    else
                    {
                        singleSkill.Rank = 1;
                    }
                    
                    singleSkill.PrimaryAttribute = skillAttributes.ContainsKey(180) ? IntToEveAttribute(skillAttributes[180]) : EveAttribute.None;
                    singleSkill.SecondaryAttribute = skillAttributes.ContainsKey(181) ? IntToEveAttribute(skillAttributes[181]) : EveAttribute.None;
                    singleSkill.CanTrainOnTrial = skillAttributes.ContainsKey(1047) && skillAttributes[1047] == 0 ? true : false;

                    // Export prerequesities
                    List<SerializableSkillPrerequisite> listOfPrerequsites = new List<SerializableSkillPrerequisite>();

                    for (var i = 0; i < DBConstants.RequiredSkillPropertyIDs.Length; i++)
                    {
                        if (skillAttributes.ContainsKey(DBConstants.RequiredSkillPropertyIDs[i]) && skillAttributes.ContainsKey(DBConstants.RequiredSkillLevelPropertyIDs[i]))
                        {
                            InvType prereqSkill = s_types.First(x => x.ID == skillAttributes[DBConstants.RequiredSkillPropertyIDs[i]]);

                            SerializableSkillPrerequisite preReq = new SerializableSkillPrerequisite
                            {
                                ID = prereqSkill.ID,
                                Level = skillAttributes[DBConstants.RequiredSkillLevelPropertyIDs[i]],
                            };

                            if (prereqSkill != null)
                                preReq.Name = prereqSkill.Name;

                            // Add prerequesities
                            listOfPrerequsites.Add(preReq);
                        }
                    }

                    // Add prerequesities to skill
                    singleSkill.Prereqs = listOfPrerequsites.ToArray();
                    
                    // Add skill
                    listOfSkillsInGroup.Add(singleSkill);
                }

                // Add skills in skill group
                skillGroup.Skills = listOfSkillsInGroup.OrderBy(x => x.Name).ToArray();
                
                // Add skill group
                listOfSkillGroups.Add(skillGroup);
            }

            m_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", m_endTime.Subtract(m_startTime)).TrimEnd('0'));

            // Serialize
            SkillsDatafile datafile = new SkillsDatafile();
            datafile.Groups = listOfSkillGroups.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.SkillsDatafile);
        }

        private static EveAttribute IntToEveAttribute(int attributeValue)
        {
            switch (attributeValue)
            {
                case 164: 
                    return EveAttribute.Charisma;
                case 165: 
                    return EveAttribute.Intelligence;
                case 166: 
                    return EveAttribute.Memory;
                case 167: 
                    return EveAttribute.Perception;
                case 168: 
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
            Console.Write("Generated certificates datafile... ");

            m_counter = 0;
            m_percentOld = 0;
            m_text = String.Empty;
            m_startTime = DateTime.Now;

            // Export certificates categories
            List<SerializableCertificateCategory> listOfCertCategories = new List<SerializableCertificateCategory>();

            foreach (var category in s_crtCategories.Where(x => x.ID != 17).OrderBy(x => x.CategoryName))
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
                foreach (var certClass in s_crtClasses.Where(x => x.ID <= 117))
                {
                    // Exclude unused classes
                    int id = certClass.ID;
                    if (id == 104 || id == 106 || id == 111)
                        continue;
                    
                    SerializableCertificateClass crtClasses = new SerializableCertificateClass
                    {
                        ID = certClass.ID,
                        Name = certClass.ClassName,
                        Description = certClass.Description
                    };

                    // Export certificates
                    List<SerializableCertificate> listOfCertificates = new List<SerializableCertificate>();

                    foreach (var certificate in s_certificates.Where(x=> x.CategoryID != 17 && x.ClassID == certClass.ID))
                    {
                        UpdatePercentDone(m_certGenTotal);

                        SerializableCertificate crtCertificates = new SerializableCertificate
                        {
                            ID = certificate.ID,
                            Grade = GetGrade(certificate.Grade),
                            Description = certificate.Description
                        };

                        // Export prerequesities
                        List<SerializableCertificatePrerequisite> listOfPrereq = new List<SerializableCertificatePrerequisite>();

                        foreach (var relationship in s_crtRelationships.Where(x=> x.ChildID == certificate.ID))
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

                        foreach(var recommendation in s_crtRecommendations.Where(x=> x.CertificateID == certificate.ID))
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

            m_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", m_endTime.Subtract(m_startTime)).TrimEnd('0'));

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
                case 1: 
                    return CertificateGrade.Basic;
                case 2: 
                    return CertificateGrade.Standard;
                case 3: 
                    return CertificateGrade.Improved;
                case 5: 
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
            Console.Write("Generated blueprints datafile... ");

            m_counter = 0;
            m_percentOld = 0;
            m_text = String.Empty;
            m_startTime = DateTime.Now;

            // Configure blueprints with Null market group
            ConfigureNullMarketBlueprint();

            var groups = new Dictionary<int, SerializableBlueprintGroup>();

            // Export blueprint groups           
            foreach (var marketGroup in s_marketGroups.Concat(m_injectedMarketGroups))
            {
                SerializableBlueprintGroup group = new SerializableBlueprintGroup
                {
                    ID = marketGroup.ID,
                    Name = marketGroup.Name,
                };

                groups[marketGroup.ID] = group;
                
                // Add the items in this group
                List<SerializableBlueprint> blueprints = new List<SerializableBlueprint>();
                foreach (var item in s_types.Where(x => x.MarketGroupID == marketGroup.ID && s_groups[x.GroupID].CategoryID == 9 && s_groups[x.GroupID].Published))
                {
                    CreateBlueprint(item, blueprints);
                }

                // Store the items
                group.Blueprints = blueprints.OrderBy(x => x.Name).ToArray();
            }

            // Create the parent-children groups relations
            foreach (var group in groups.Values)
            {
                var children = s_marketGroups.Concat(m_injectedMarketGroups).Where(x => x.ParentID == group.ID).Select(x => groups[x.ID]);
                group.SubGroups = children.OrderBy(x => x.Name).ToArray();
            }

            // Sort groups
            var blueprintGroups = s_marketGroups.Concat(m_injectedMarketGroups).Where(x => x.ParentID == DBConstants.BlueprintsGroupID).Select(x => groups[x.ID]).OrderBy(x => x.Name);

            m_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", m_endTime.Subtract(m_startTime)).TrimEnd('0'));

            // Serialize
            BlueprintsDatafile datafile = new BlueprintsDatafile();
            datafile.Groups = blueprintGroups.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.BlueprintsDatafile);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ConfigureNullMarketBlueprint()
        {
            m_injectedMarketGroups = new List<InvMarketGroup>();

            // Create custom market groups that don't exist in EVE
            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Unique Designs",
                Description = "Blueprints of unique design ships",
                ID = 20000,
                ParentID = 204,
                GraphicID = 2703
            });

            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Unique Shuttles",
                Description = "Blueprints of unique shuttles",
                ID = 20001,
                ParentID = 20000,
                GraphicID = 2703
            });

            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Various Non-Market",
                Description = "Various blueprints not in EVE market",
                ID = DBConstants.BlueprintRootNonMarketGroupID,
                ParentID = 2,
                GraphicID = 2703
            });

            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Storyline",
                Description = "Storyline blueprints not in EVE market",
                ID = 20003,
                ParentID = 20002,
                GraphicID = 2703
            });

            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Faction",
                Description = "Faction blueprints not in EVE market",
                ID = 20004,
                ParentID = 20002,
                GraphicID = 2703
            });

            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Tech III",
                Description = "Tech III blueprints not in EVE market",
                ID = 20005,
                ParentID = 20002,
                GraphicID = 2703
            });

            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Tech II",
                Description = "Tech II blueprints not in EVE market",
                ID = 20006,
                ParentID = 20002,
                GraphicID = 2703
            });

            m_injectedMarketGroups.Add(new InvMarketGroup()
            {
                Name = "Tech I",
                Description = "Tech I blueprints not in EVE market",
                ID = 20007,
                ParentID = 20002,
                GraphicID = 2703
            });

            // Set some blueprints to market groups manually
            foreach (var item in s_types.Where(x => x.MarketGroupID == null))
            {
                switch (item.ID)
                {
                    case 29267: // Apotheosis Blueprint
                        item.MarketGroupID = 20001;
                        break;
                    case 22924: // 'Wild' Miner I Blueprint
                        item.MarketGroupID = 20003;
                        break;
                    case 17704: // Imperial Navy Slicer Blueprint
                    case 17620: // Caldari Navy Hookbill Blueprint
                    case 17842: // Federation Navy Comet Blueprint
                    case 17813: // Republic Fleet Firetail Blueprint
                    case 17737: // Nightmare Blueprint
                    case 17739: // Machariel Blueprint
                    case 17933: // Dramiel Blueprint
                    case 17927: // Cruor Blueprint
                    case 17925: // Succubus Blueprint
                    case 17929: // Daredevil Blueprint
                    case 17721: // Cynabal Blueprint
                    case 17923: // Ashimmu Blueprint
                    case 17719: // Phantasm Blueprint
                    case 21098: // Goru's Shuttle Blueprint
                    case 21629: // Guristas Shuttle Blueprint
                    case 21842: // Gallente Mining Laser Blueprint
                    case 30843: // Interbus Shuttle Blueprint
                        item.MarketGroupID = 20004;
                        break;
                    case 29987: // Legion Blueprint
                    case 30227: // Legion Defensive - Adaptive Augmenter Blueprint
                    case 30037: // Legion Electronics - Energy Parasitic Complex Blueprint
                    case 30170: // Legion Engineering - Power Core Multiplier Blueprint
                    case 30392: // Legion Offensive - Drone Synthesis Projector Blueprint
                    case 30077: // Legion Propulsion - Chassis Optimization Blueprint
                    case 29991: // Loki Blueprint
                    case 30242: // Loki Defensive - Adaptive Shielding Blueprint
                    case 30067: // Loki Electronics - Immobility Drivers Blueprint
                    case 30160: // Loki Engineering - Power Core Multiplier Blueprint
                    case 30407: // Loki Offensive - Turret Concurrence Registry Blueprint
                    case 30107: // Loki Propulsion - Chassis Optimization Blueprint
                    case 29989: // Proteus Blueprint
                    case 30237: // Proteus Defensive - Adaptive Augmenter Blueprint
                    case 30057: // Proteus Electronics - Friction Extension Processor Blueprint
                    case 30150: // Proteus Engineering - Power Core Multiplier Blueprint
                    case 30402: // Proteus Offensive - Dissonic Encoding Platform Blueprint
                    case 30097: // Proteus Propulsion - Wake Limiter Blueprint
                    case 29985: // Tengu Blueprint
                    case 30232: // Tengu Defensive - Adaptive Shielding Blueprint
                    case 30047: // Tengu Electronics - Obfuscation Manifold Blueprint
                    case 30140: // Tengu Engineering - Power Core Multiplier Blueprint
                    case 30397: // Tengu Offensive - Accelerated Ejection Bay Blueprint
                    case 30087: // Tengu Propulsion - Intercalated Nanofibers Blueprint
                        item.MarketGroupID = 20005;
                        break;
                    case 32078: // Small EW Drone Range Augmentor II Blueprint
                        item.MarketGroupID = 20006;
                        break;
                }
            }

            // Set the market group of the blueprints with NULL MarketGroupID to custom market groups
            foreach (var item in s_types.Where(x => x.MarketGroupID == null && !x.Name.Contains("TEST")
                && s_blueprintTypes.Any(y => y.ID == x.ID)
                && s_types[s_blueprintTypes[x.ID].ProductTypeID].Published))
            {
                UpdatePercentDone(m_blueprintGenTotal);

                foreach (var relation in s_metaTypes.Where(x => x.ItemID == s_blueprintTypes[item.ID].ProductTypeID))
                {
                    switch (relation.MetaGroupID)
                    {
                        case 2:
                            item.MarketGroupID = 20006;
                            break;
                        case 3:
                            item.MarketGroupID = 20003;
                            break;
                        case 4:
                            item.MarketGroupID = 20004;
                            break;
                        case 14:
                            item.MarketGroupID = 20005;
                            break;
                    }
                }

                if (item.MarketGroupID == null)
                    item.MarketGroupID = 20007;
            }
        }

        /// <summary>
        /// Add properties to a blueprint.
        /// </summary>
        /// <param name="srcItem"></param>
        /// <param name="groupItems"></param>
        /// <returns></returns>
        private static void CreateBlueprint(InvType srcBlueprint, List<SerializableBlueprint> blueprintsGroup)
        {
            UpdatePercentDone(m_blueprintGenTotal);

            srcBlueprint.Generated = true;

            // Creates the blueprint with base informations
            var item = new SerializableBlueprint
            {
                ID = srcBlueprint.ID,
                Name = srcBlueprint.Name,
            };

            // Icon
            item.Icon = (srcBlueprint.GraphicID.HasValue ? s_graphics[srcBlueprint.GraphicID.Value].Icon : String.Empty);

            // Export attributes
            foreach (var attribute in s_blueprintTypes.Where(x=> x.ID == srcBlueprint.ID))
            {
                item.ProduceItemID = attribute.ProductTypeID;
                item.ProductionTime = attribute.ProductionTime;
                item.TechLevel = attribute.TechLevel;
                item.ResearchProductivityTime = attribute.ResearchProductivityTime;
                item.ResearchMaterialTime = attribute.ResearchMaterialTime;
                item.ResearchCopyTime = attribute.ResearchCopyTime;
                item.ResearchTechTime = attribute.ResearchTechTime;
                item.ProductivityModifier = attribute.ProductivityModifier;
                item.WasteFactor = attribute.WasteFactor;
                item.MaxProductionLimit = attribute.MaxProductionLimit;
            }
            
            // Metagroup
            item.MetaGroup = ItemMetaGroup.T1;
            foreach (var relation in s_metaTypes.Where(x => x.ItemID == s_blueprintTypes[srcBlueprint.ID].ProductTypeID && x.MetaGroupID == 2))
            {
                item.MetaGroup = ItemMetaGroup.T2;
            }

            // Metagroup for the custom market groups
            switch (srcBlueprint.MarketGroupID)
            {
                case 20003:
                    item.MetaGroup = ItemMetaGroup.Storyline;
                    break;
                case 20004:
                    item.MetaGroup = ItemMetaGroup.Faction;
                    break;
                case 20005:
                    item.MetaGroup = ItemMetaGroup.T3;
                    break;
                case 20006:
                    item.MetaGroup = ItemMetaGroup.T2;
                    break;
            }

            // Export item requirements
            ExportRequirements(srcBlueprint, item);

            // Look for the tech 2 variations that this blueprint invents
            var inventionBlueprint = new List<int>();
            foreach (var relationItemID in s_metaTypes.Where(x => x.ParentItemID == item.ProduceItemID && x.MetaGroupID == 2).Select(x => x.ItemID))
            {
                // Look for a blueprint that produces the related item
                foreach (var variationItemID in s_blueprintTypes.Where(x => x.ProductTypeID == relationItemID).Select(x => x.ID))
                {
                    // Add the variation blueprint
                    inventionBlueprint.Add(s_types[variationItemID].ID);
                }
            }

            // Add invention blueprints to item
            item.InventionTypeID = inventionBlueprint.ToArray();

            // Add this item
            blueprintsGroup.Add(item);
        }

        /// <summary>
        /// Export item requirements. 
        /// </summary>
        /// <param name="srcBlueprint"></param>
        /// <param name="item"></param>
        private static void ExportRequirements(InvType srcBlueprint, SerializableBlueprint item)
        {
            var prerequisiteSkills = new List<SerializablePrereqSkill>();
            var requiredMaterials = new List<SerializableRequiredMaterial>();
            foreach (var requirement in s_typeRequirements.Where(x => x.TypeID == srcBlueprint.ID))
            {
                // Is it a skill ?
                if (s_types.Any(x => x.ID == requirement.RequiredTypeID && s_groups.Any(y => y.ID == x.GroupID && y.CategoryID == 16)))
                {
                    prerequisiteSkills.Add(new SerializablePrereqSkill
                    {
                        ID = requirement.RequiredTypeID,
                        Level = requirement.Quantity,
                        Activity = requirement.ActivityID
                    });
                }
                else // It is an item
                {
                    requiredMaterials.Add(new SerializableRequiredMaterial
                    {
                        ID = requirement.RequiredTypeID,
                        Quantity = requirement.Quantity,
                        DamagePerJob = requirement.DamagePerJob,
                        Activity = requirement.ActivityID
                    });

                    // If activity is invention, add the prerequisite skill
                    // of the required material as it's not included in this table
                    if (requirement.ActivityID == (int)BlueprintActivity.Invention)
                    {
                        // Add the prerequisite skills for a material used in invention activity.
                        MaterialPrereqSkill(requirement, prerequisiteSkills);
                    }
                }
            }

            // Add additional materials that are in the typeMaterials table
            AdditionalRequiredMaterials(srcBlueprint, item, requiredMaterials);

            // Add prerequisite skills to item
            item.PrereqSkill = prerequisiteSkills.OrderBy(x => x.Activity).ToArray();

            // Add required materials to item
            item.ReqMaterial = requiredMaterials.OrderBy(x => x.Activity).ToArray();
        }

        /// <summary>
        /// Add the prerequisite skills for a material used in invention activity.
        /// </summary>
        private static void MaterialPrereqSkill(RamTypeRequirements requirement, List<SerializablePrereqSkill> prerequisiteSkills)
        {
            var prereqSkills = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            var prereqLevels = new int[DBConstants.RequiredSkillPropertyIDs.Length];

            foreach (var attribute in s_typeAttributes.Where(x => x.ItemID == requirement.RequiredTypeID))
            {
                int attributeIntValue = (attribute.ValueInt.HasValue ? attribute.ValueInt.Value : (int)attribute.ValueFloat.Value);

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

        /// <summary>
        /// Add additional materials needed for manufacturing.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="requiredMaterials"></param>
        private static void AdditionalRequiredMaterials(InvType src, SerializableBlueprint item, List<SerializableRequiredMaterial> requiredMaterials)
        {
            // Find the materials needed for the item and add them (exclude compression blueprints)
            foreach (var reprocItem in s_typeMaterials.Where(x => x.TypeID == item.ProduceItemID
                && (Array.IndexOf(DBConstants.CompressionBlueprintsGroupIDs, src.MarketGroupID) == -1)))
            {
                // List already contains this material ?
                // Add the material quantity to the existing one
                if (requiredMaterials.Any(x => x.ID == reprocItem.MaterialTypeID))
                {
                    var material = requiredMaterials.First(x => x.ID == reprocItem.MaterialTypeID);
                    material.Quantity += reprocItem.Quantity;
                }
                // Add it to the list
                else
                {
                    requiredMaterials.Add(new SerializableRequiredMaterial
                    {
                        ID = reprocItem.MaterialTypeID,
                        Quantity = reprocItem.Quantity,
                        DamagePerJob = 1,
                        Activity = (int)BlueprintActivity.Manufacturing
                    });
                }

                // Look if the produced item has a lower tech item and it's a ship 
                var techItem = s_metaTypes.FirstOrDefault(x => x.ItemID == item.ProduceItemID && x.MetaGroupID == 2 &&
                    s_types.Any(y => y.ID == item.ProduceItemID && s_groups.Any(z => z.ID == y.GroupID && z.CategoryID == 6)));
                
                if (techItem == null)
                    continue;

                // Find this material quantity needed for the lower tech ship and substract it
                foreach (var reprocLowerTechItem in s_typeMaterials.Where(x => x.TypeID == techItem.ParentItemID && x.MaterialTypeID == reprocItem.MaterialTypeID))
                {
                    // Substract the material quantity from the existing one
                    var material = requiredMaterials.First(x => x.ID == reprocLowerTechItem.MaterialTypeID);
                    material.Quantity -= reprocLowerTechItem.Quantity;

                    if (material.Quantity <= 0)
                        requiredMaterials.Remove(material);
                }
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
            Console.Write("Generated geography datafile... ");

            m_counter = 0;
            m_percentOld = 0;
            m_text = String.Empty;
            m_startTime = DateTime.Now;

            var allSystems = new List<SerializableSolarSystem>();
            var regions = new List<SerializableRegion>();

            // Regions
            foreach (var srcRegion in s_regions)
            {
                var region = new SerializableRegion 
                { 
                    ID = srcRegion.ID, 
                    Name = srcRegion.Name
                };
                regions.Add(region);

                // Constellations
                var constellations = new List<SerializableConstellation>();
                foreach (var srcConstellation in s_constellations.Where(x => x.RegionID == srcRegion.ID))
                {
                    var constellation = new SerializableConstellation 
                    { 
                        ID = srcConstellation.ID, 
                        Name = srcConstellation.Name 
                    };
                    constellations.Add(constellation);

                    // Systems
                    const double baseDistance = 1.0E14;
                    var systems = new List<SerializableSolarSystem>();
                    foreach (var srcSystem in s_solarSystems.Where(x => x.ConstellationID == srcConstellation.ID))
                    {
                        var system = new SerializableSolarSystem 
                        { 
                            ID = srcSystem.ID, 
                            Name = srcSystem.Name, 
                            X = (int)(srcSystem.X / baseDistance),
                            Y = (int)(srcSystem.Y / baseDistance),
                            Z = (int)(srcSystem.Z / baseDistance),
                            SecurityLevel = srcSystem.SecurityLevel
                        };
                        systems.Add(system);

                        // Stations
                        var stations = new List<SerializableStation>();
                        foreach (var srcStation in s_stations.Where(x => x.SolarSystemID == srcSystem.ID))
                        {
                            UpdatePercentDone(m_geoGen);

                            var station = new SerializableStation
                            {
                                ID = srcStation.ID,
                                Name = srcStation.Name,
                                CorporationID = srcStation.CorporationID,
                                ReprocessingEfficiency = srcStation.ReprocessingEfficiency,
                                ReprocessingStationsTake = srcStation.ReprocessingStationsTake
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
            var jumps = new List<SerializableJump>();
            foreach (var srcJump in s_jumps)
            {
                UpdatePercentDone(m_geoGenTotal);

                // In CCP tables, every jump is included twice, we only need one.
                if (srcJump.A < srcJump.B)
                    jumps.Add(new SerializableJump { FirstSystemID = srcJump.A, SecondSystemID = srcJump.B });
            }

            m_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", m_endTime.Subtract(m_startTime)).TrimEnd('0'));

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
            Console.Write("Generated reprocessing datafile... ");

            m_counter = 0;
            m_percentOld = 0;
            m_text = String.Empty;
            m_startTime = DateTime.Now;

            var types = new List<SerializableItemMaterials>();

            foreach (var typeID in s_types.Where(x => x.Generated).Select(x => x.ID))
            {
                UpdatePercentDone(m_reprocessGenTotal);

                var materials = new List<SerializableMaterialQuantity>();
                foreach (var srcMaterial in s_typeMaterials.Where(x => x.TypeID == typeID))
                {
                    materials.Add(new SerializableMaterialQuantity { ID = srcMaterial.MaterialTypeID, Quantity = srcMaterial.Quantity });
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

            m_endTime = DateTime.Now;
            Console.WriteLine(String.Format(" in {0}", m_endTime.Subtract(m_startTime)).TrimEnd('0'));

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
            m_counter++;
            int percent = (int)((m_counter / total) * 100);

            if (m_counter == 1 || m_percentOld < percent)
            {
                Console.SetCursorPosition(Console.CursorLeft - m_text.Length, Console.CursorTop);
                m_text = String.Format("{0}%", percent.ToString());
                Console.Write(m_text);
                m_percentOld = percent;
            }
        }

        /// <summary>
        /// Updates the progress of data loaded from SQL server.
        /// </summary>
        private static void UpdateProgress()
        {
            Console.SetCursorPosition(Console.CursorLeft - m_text.Length, Console.CursorTop);
            m_tablesCount++;
            m_text = String.Format("{0}%", (int)((m_tablesCount / m_totalTablesCount) * 100));
            Console.Write(m_text);
        }
        #endregion
    }
}