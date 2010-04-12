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
        private static int s_propBasePriceID = 0;
        private static int baseWarpSpeed = 3;
        private static double warpSpeedMultiplier;

        private static Bag<EveUnit> s_units;
        private static Bag<EveGraphic> s_graphics;
        private static Bag<DgmAttribute> s_attributes;
        private static Bag<DgmAttributeCategory> s_attributeCategories;
        private static Bag<MapRegion> s_regions;
        private static Bag<MapConstellation> s_constellations;
        private static Bag<MapSolarSystem> s_solarSystems;
        private static Bag<StaStation> s_stations;
        private static List<MapSolarSystemJump> s_jumps;
        private static RelationSet<DgmTypeAttribute> s_typeAttributes;
        private static Bag<InvMarketGroup> s_marketGroups;
        private static Bag<InvGroup> s_groups;
        private static Bag<InvType> s_types;
        private static RelationSet<InvMetaType> s_metaTypes;
        private static RelationSet<DgmTypeEffect> s_typeEffects;
        private static List<TypeActivityMaterial> s_activities;
        private static Bag<CrtCategories> s_crtCategories;
        private static Bag<CrtClasses> s_crtClasses;
        private static Bag<CrtCertificates> s_certificates;
        private static Bag<CrtRecommendations> s_crtRecommendations;
        private static Bag<CrtRelationships> s_crtRelationships;
        

        static void Main(string[] args)
        {
            // Setting a standard format for the generated files
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            // Data dumps are available from CCP

            Console.Write("Loading Data SQL... ");
            
            // Read Zofu's XML
            s_units = Database.Units();
            Console.Write("5%");
            s_graphics = Database.Graphics();
            Console.SetCursorPosition(Console.CursorLeft - 2, Console.CursorTop);
            Console.Write("10%");
            s_attributes = Database.Attributes();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("15%");
            s_attributeCategories = Database.AttributeCategories();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("20%");
            s_regions = Database.Regions();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("25%");
            s_constellations = Database.Constellations();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("30%");
            s_solarSystems = Database.Solarsystems();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("35%");
            s_stations = Database.Stations();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("40%");
            s_jumps = Database.Jumps();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("45%");
            s_typeAttributes = Database.TypeAttributes();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("50%");
            s_marketGroups = Database.MarketGroups();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("55%");
            s_groups = Database.Groups();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("60%");
            s_metaTypes = Database.MetaTypes();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("65%");
            s_typeEffects = Database.TypeEffects();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("70%");
            s_types = Database.Types();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("75%");
            s_activities = Database.Materials();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("80%");
            s_crtCategories = Database.CertificateCategories();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("85%");
            s_crtClasses = Database.CertificateClasses();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("90%");
            s_certificates = Database.Certificates();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("95%");
            s_crtRecommendations = Database.CertificateRecommendations();
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("100%");
            s_crtRelationships = Database.CertificateRelationships();
            Console.WriteLine();
            Console.WriteLine();
            
            // Generate datafiles
            Console.WriteLine("Datafile Generating In Progress");
            Console.WriteLine("(Please be patient as this process can take up to 5 minutes)");
            Console.WriteLine();

            GenerateProperties();
            GenerateItems(); // Requires GenerateProperties()
            GenerateSkills();
            GenerateCertificates();
            GenerateGeography();
            GenerateReprocessing(); // Requires GenerateItems()
            
            GenerateMD5Sums();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        #region Certificates Datafile
        
        /// <summary>
        /// Generate the certificates datafile.
        /// </summary>        
        private static void GenerateCertificates()
        {
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

        #region Skills Datafile

        /// <summary>
        /// Generate the skills datafile.
        /// </summary>
        private static void GenerateSkills()
        {
            // Export skill groups
            List<SerializableSkillGroup> listOfSkillGroups = new List<SerializableSkillGroup>();

            foreach (var group in s_groups.Where(x => x.CategoryID == 16 && x.ID != 505).OrderBy(x => x.Name))
            {
                // the following section is drawn manually from dgmAttributeTypes 
                int maxPreReqCount = 6;

                int[] prereqSkillAttrs = new int[maxPreReqCount];
                prereqSkillAttrs[0] = 182; // requiredSkill1
                prereqSkillAttrs[1] = 183; // requiredSkill2
                prereqSkillAttrs[2] = 184; // requiredSkill3
                prereqSkillAttrs[3] = 1285; // requiredSkill4
                prereqSkillAttrs[4] = 1289; // requiredSkill5
                prereqSkillAttrs[5] = 1290; // requiredSkill6

                int[] prereqLevelAttrs = new int[maxPreReqCount];
                prereqLevelAttrs[0] = 277; // requiredSkill1Level
                prereqLevelAttrs[1] = 278; // requiredSkill2Level
                prereqLevelAttrs[2] = 279; // requiredSkill3Level
                prereqLevelAttrs[3] = 1286; // requiredSkill4Level
                prereqLevelAttrs[4] = 1287; // requiredSkill5Level
                prereqLevelAttrs[5] = 1288; // requiredSkill6Level

                SerializableSkillGroup skillGroup = new SerializableSkillGroup
                {
                    ID = group.ID,
                    Name = group.Name,
                };

                // Export skills
                List<SerializableSkill> listOfSkillsInGroup = new List<SerializableSkill>();

                foreach (var skill in s_types.Where(x => x.GroupID == group.ID))
                {
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

                    for (var i = 0; i < maxPreReqCount; i++)
                    {
                        if (skillAttributes.ContainsKey(prereqSkillAttrs[i]) && skillAttributes.ContainsKey(prereqLevelAttrs[i]))
                        {
                            SerializableSkillPrerequisite preReq = new SerializableSkillPrerequisite
                            {
                                Level = skillAttributes[prereqLevelAttrs[i]],
                            };

                            InvType prereqSkillName = s_types.First(x => x.ID == skillAttributes[prereqSkillAttrs[i]]);

                            if (prereqSkillName != null)
                                preReq.Name = prereqSkillName.Name;

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

        #region Properties Datafile

        /// <summary>
        /// Generate the properties datafile.
        /// </summary>
        private static void GenerateProperties()
        {
            int newID = 0;

            // Change some display names and default values
            s_attributes[9].DisplayName = "Structure HP";
            s_attributes[263].DisplayName = "Shield HP";
            s_attributes[265].DisplayName = "Armor HP";

            s_attributes[38].DisplayName = "Cargo Capacity";
            s_attributes[48].DisplayName = "CPU";
            s_attributes[11].DisplayName = "Powergrid";

            // Shield
            s_attributes[271].DisplayName = "EM Resistance";
            s_attributes[272].DisplayName = "Explosive Resistance";
            s_attributes[273].DisplayName = "Kinetic Resistance";
            s_attributes[274].DisplayName = "Thermal Resistance";

            // Armor
            s_attributes[267].DisplayName = "EM Resistance";
            s_attributes[268].DisplayName = "Explosive Resistance";
            s_attributes[269].DisplayName = "Kinetic Resistance";
            s_attributes[270].DisplayName = "Thermal Resistance";

            // Hull
            s_attributes[974].DisplayName = "EM Resistance";
            s_attributes[975].DisplayName = "Explosive Resistance";
            s_attributes[976].DisplayName = "Kinetic Resistance";
            s_attributes[977].DisplayName = "Thermal Resistance";

            // Items attribute
            s_attributes[6].DisplayName = "Activation cost";
            s_attributes[30].DisplayName = "Powergrid usage";
            s_attributes[68].DisplayName = "Shield Bonus";
            s_attributes[87].DisplayName = "Shield Transfer Range";
            s_attributes[116].DisplayName = "Explosive damage";
            s_attributes[424].DisplayName = "CPU Output Bonus";
            s_attributes[1082].DisplayName = "CPU Penalty";
            
            // Changing the categoryID for some attributes 
            s_attributes[1132].CategoryID = 1; // Calibration
            s_attributes[1547].CategoryID = 1; // Rig Size
            s_attributes[908].CategoryID = 4; // Ship Maintenance Bay Capacity

            // Changing HigherIsBetter to false (CCP has this wrong?)
            s_attributes[30].HigherIsBetter = false; // CPU usage
            s_attributes[50].HigherIsBetter = false; // PG usage
            s_attributes[161].HigherIsBetter = false; // Volume
            s_attributes[4].HigherIsBetter = false; // Mass
            s_attributes[70].HigherIsBetter = false; // Inertia Modifier
            s_attributes[6].HigherIsBetter = false; // Activation Cost
            s_attributes[55].HigherIsBetter = false; // Capacitor recharge time
            s_attributes[479].HigherIsBetter = false; // Shield recharge time
            s_attributes[552].HigherIsBetter = false; // Signature radius
            s_attributes[560].HigherIsBetter = false; // Sensor Recalibration Time
            s_attributes[1082].HigherIsBetter = false; // CPU Penalty
            s_attributes[1153].HigherIsBetter = false; // Calibration cost
            s_attributes[1272].HigherIsBetter = false; // Bandwidth Needed
            s_attributes[1416].HigherIsBetter = false; // Target Switch Timer
            s_attributes[73].HigherIsBetter = false; // Activation time / duration
            s_attributes[556].HigherIsBetter = false; // Anchoring Delay
            s_attributes[676].HigherIsBetter = false; // Unanchoring Delay
            s_attributes[677].HigherIsBetter = false; // Onlining Delay
            s_attributes[780].HigherIsBetter = false; // Cycle Time bonus

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

                foreach (var srcProp in s_attributes.Where(x => x.CategoryID == srcCategory.ID))
                {
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
                        case 974:
                            properties.Insert(5, prop);
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
            
            // Serialize
            PropertiesDatafile datafile = new PropertiesDatafile();
            datafile.Categories = categories.OrderBy(x => Array.IndexOf(orderedGroupNames, String.Intern(x.Name))).ToArray();
            Util.SerializeXML(datafile, DatafileConstants.PropertiesDatafile);
        }

        #endregion

        #region Geography Datafile

        /// <summary>
        /// Generates the geo datafile.
        /// </summary>
        private static void GenerateGeography()
        {
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
                // In CCP tables, every jump is included twice, we only need one.
                if (srcJump.A < srcJump.B)
                    jumps.Add(new SerializableJump { FirstSystemID = srcJump.A, SecondSystemID = srcJump.B });
            }

            // Serialize
            GeoDatafile datafile = new GeoDatafile();
            datafile.Regions = regions.OrderBy(x => x.Name).ToArray();
            datafile.Jumps = jumps.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.GeographyDatafile);
        }

        #endregion

        #region Items Datafile

        /// <summary>
        /// Generate the items datafile.
        /// </summary>
        private static void GenerateItems()
        {
            var allItems = new List<SerializableItem>();
            var groups = new Dictionary<int, SerializableMarketGroup>();
            var injectedMarketGroups = new List<InvMarketGroup>();

            // Create some market groups that don't exist in EVE
            injectedMarketGroups.Add(
                new InvMarketGroup()
                {
                    Name = "Unique Designs",
                    Description = "Ships and modules of a unique design",
                    ID = 10001,
                    ParentID = 4,
                    GraphicID = 2703
                });

            // Inject the missing ships that are missing from the output because their MarketGroupID is NULL
            foreach (var srcItem in s_types.Where(x => x.Published && x.MarketGroupID == null))
            {
                switch (srcItem.ID)
                {
                    case 17703: // Amarr Navy Slicer
                        srcItem.MarketGroupID = 72;
                        srcItem.RaceID = 32;
                        break; 
                    case 17619: // Caldari Navy Hookbill
                        srcItem.MarketGroupID = 61;
                        srcItem.RaceID = 32;
                        break;
                    case 17841: // Gallente Navy Comet
                        srcItem.MarketGroupID = 77;
                        srcItem.RaceID = 32;
                        break;
                    case 17812: // Republic Fleet Firetail
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
                    case 17932: // Dramiel
                        srcItem.MarketGroupID = 64;
                        srcItem.RaceID = 32;
                        break;
                    case 17926: // Cruor
                        srcItem.MarketGroupID = 72;
                        srcItem.RaceID = 32;
                        break;
                    case 17924: // Succubus
                        srcItem.MarketGroupID = 72;
                        srcItem.RaceID = 32;
                        break;
                    case 17928: // Daredevil
                        srcItem.MarketGroupID = 64;
                        srcItem.RaceID = 32;
                        break;
                    case 17720: // Cynabal
                        srcItem.MarketGroupID = 73;
                        srcItem.RaceID = 32;
                        break;
                    case 17922: // Ashimmu
                        srcItem.MarketGroupID = 74;
                        srcItem.RaceID = 32;
                        break;
                    case 17718: // Phantasm
                        srcItem.MarketGroupID = 74;
                        srcItem.RaceID = 32;
                        break;
                    case 21097: // Goru's Shuttle
                        srcItem.MarketGroupID = 396;
                        srcItem.RaceID = 32;
                        break;
                    case 21628: // Guristas Shuttle
                        srcItem.MarketGroupID = 396;
                        srcItem.RaceID = 32;
                        break;
                    case 30842: // Interbus Shuttle
                        srcItem.MarketGroupID = 395;
                        srcItem.RaceID = 32;
                        break;
                    case 29266: // Apotheosis
                        srcItem.MarketGroupID = 10001;
                        srcItem.RaceID = 32;
                        break;
                    case 2078: // Zephyr
                        srcItem.MarketGroupID = 10001;
                        srcItem.RaceID = 32;
                        break;
                }
            }
            
            // Create the market groups
            foreach (var srcGroup in s_marketGroups.Concat(injectedMarketGroups))
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
                var children = s_marketGroups.Concat(injectedMarketGroups).Where(x => x.ParentID.GetValueOrDefault() == group.ID).Select(x => groups[x.ID]);
                group.SubGroups = children.OrderBy(x => x.Name).ToArray();
            }

            // Pick the family
            SetItemFamilyByMarketGroup(groups[2], ItemFamily.Bpo);
            SetItemFamilyByMarketGroup(groups[4], ItemFamily.Ship);
            SetItemFamilyByMarketGroup(groups[27], ItemFamily.Implant);
            SetItemFamilyByMarketGroup(groups[157], ItemFamily.Drone);
            SetItemFamilyByMarketGroup(groups[477], ItemFamily.StarbaseStructure);

            // Sort groups
            var rootGroups = s_marketGroups.Concat(injectedMarketGroups).Where(x => !x.ParentID.HasValue).Select(x => groups[x.ID]).OrderBy(x => x.Name);
            
            // Serialize
            ItemsDatafiles datafile = new ItemsDatafiles();
            datafile.MarketGroups = rootGroups.ToArray();
            Util.SerializeXML(datafile, DatafileConstants.ItemsDatafile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcItem"></param>
        /// <param name="groupItems"></param>
        /// <returns></returns>
        private static void CreateItem(InvType srcItem, List<SerializableItem> groupItems)
        {
            srcItem.Generated = true;

            // Creates the item with base informations.
            var item = new SerializableItem
            {
                ID = srcItem.ID,
                Name = srcItem.Name,
                Description = srcItem.Description
            };

            // Icon
            if (srcItem.GraphicID.HasValue)
                item.Icon = s_graphics[srcItem.GraphicID.Value].Icon;

            // Add the properties and prereqs
            var props = new List<SerializablePropertyValue>();
            var prereqSkills = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            var prereqLevels = new int[DBConstants.RequiredSkillPropertyIDs.Length];
            foreach (var srcProp in s_typeAttributes.Where(x => x.ItemID == srcItem.ID))
            {
                int propIntValue = (srcProp.ValueInt.HasValue ? srcProp.ValueInt.Value : (int)srcProp.ValueFloat.Value);

                // Is it a prereq ID ?
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

            //Add prop to item
            item.Properties = props.ToArray();

            // Prerequisites completion
            var prereqs = new List<SerializablePrerequisiteSkill>();
            for (int i = 0; i < prereqSkills.Length; i++)
            {
                if (prereqSkills[i] != 0)
                    prereqs.Add(new SerializablePrerequisiteSkill { Level = prereqLevels[i], ID = prereqSkills[i] });

            }

            //Add prereqs to item
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
            switch (srcItem.RaceID)
            {
                case 1:
                    item.Race = Race.Caldari;
                    break;
                case 2:
                    item.Race = Race.Minmatar;
                    break;
                case 4:
                    item.Race = Race.Amarr;
                    break;
                case 8:
                    item.Race = Race.Gallente;
                    break;
                case 16:
                    item.Race = Race.Jove;
                    break;
                case 32:
                    item.Race = Race.Faction;
                    break;
                default:
                    if (item.MetaGroup == ItemMetaGroup.Faction)
                    {
                        item.Race = Race.Faction;
                        break;
                    }

                    item.Race = Race.None;
                    break;
            }

            // Set race to ORE if it is in the ORE market
            // groups within mining barges, exhumers or capital
            // industrial ships
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

        #region Reprocessing Datafile

        /// <summary>
        /// Generates the reprocessing datafile.
        /// </summary>
        private static void GenerateReprocessing()
        {
            var types = new List<SerializableItemMaterials>();

            // Regions
            var materialTypes = s_types.Where(x => x.Generated).Select(x => x.ID);

            foreach (var typeID in s_types.Where(x => x.Generated).Select(x => x.ID))
            {
                var materials = new List<SerializableMaterialQuantity>();
                foreach(var srcMaterial in s_activities.Where(x => x.ActivityID == 6 && x.TypeID == typeID))
                {
                    materials.Add(new SerializableMaterialQuantity { ID = srcMaterial.RequiredTypeID, Quantity = srcMaterial.Quantity });
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
    }
}