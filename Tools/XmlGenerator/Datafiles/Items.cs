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
    public static class Items
    {
        private static List<InvMarketGroups> s_injectedMarketGroups;
        private static List<InvTypes> s_nullMarketItems; 

        /// <summary>
        /// Generate the items datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating items datafile... ");

            // Create custom market groups that don't exist in EVE
            ConfigureNullMarketItems();

            Dictionary<int, SerializableMarketGroup> groups = new Dictionary<int, SerializableMarketGroup>();

            // Create the market groups
            CreateMarketGroups(groups);

            // Create the parent-children groups relations
            foreach (SerializableMarketGroup group in groups.Values)
            {
                IEnumerable<SerializableMarketGroup> children = Database.InvMarketGroupsTable.Concat(
                    s_injectedMarketGroups).Where(x => x.ParentID.GetValueOrDefault() == group.ID).Select(
                        x => groups[x.ID]).OrderBy(x => x.Name);

                group.SubGroups.AddRange(children);
            }

            // Pick the family
            SetItemFamilyByMarketGroup(groups[DBConstants.BlueprintsMarketGroupID], ItemFamily.Blueprint);
            SetItemFamilyByMarketGroup(groups[DBConstants.ShipsMarketGroupID], ItemFamily.Ship);
            SetItemFamilyByMarketGroup(groups[DBConstants.ImplantsMarketGroupID], ItemFamily.Implant);
            SetItemFamilyByMarketGroup(groups[DBConstants.DronesMarketGroupID], ItemFamily.Drone);
            SetItemFamilyByMarketGroup(groups[DBConstants.StarbaseStructuresMarketGroupID], ItemFamily.StarbaseStructure);

            // Sort groups
            IEnumerable<SerializableMarketGroup> rootGroups = Database.InvMarketGroupsTable.Concat(s_injectedMarketGroups).Where(
                x => !x.ParentID.HasValue).Select(x => groups[x.ID]).OrderBy(x => x.Name);

            // Reset the custom market groups
            s_nullMarketItems.ForEach(srcItem => srcItem.MarketGroupID = null);

            // Serialize
            ItemsDatafile datafile = new ItemsDatafile();
            datafile.MarketGroups.AddRange(rootGroups);

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}",
                                            DateTime.Now.Subtract(startTime)).TrimEnd('0'));

            Util.SerializeXML(datafile, DatafileConstants.ItemsDatafile);
        }

        /// <summary>
        /// Creates the market groups.
        /// </summary>
        /// <param name="groups">The groups.</param>
        private static void CreateMarketGroups(IDictionary<int, SerializableMarketGroup> groups)
        {
            foreach (InvMarketGroups marketGroup in Database.InvMarketGroupsTable.Concat(s_injectedMarketGroups))
            {
                SerializableMarketGroup group = new SerializableMarketGroup { ID = marketGroup.ID, Name = marketGroup.Name };
                groups[marketGroup.ID] = group;

                // Add the items in this group; excluding the implants we are adding below
                List<SerializableItem> items = new List<SerializableItem>();
                foreach (InvTypes srcItem in Database.InvTypesTable.Where(
                    item => !item.Generated && item.Published && item.MarketGroupID.GetValueOrDefault() == marketGroup.ID).Where(
                        srcItem => marketGroup.ParentID != DBConstants.RootNonMarketGroupID ||
                                   Database.InvGroupsTable[srcItem.GroupID].CategoryID != DBConstants.ImplantCategoryID ||
                                   srcItem.GroupID == DBConstants.CyberLearningImplantsGroupID))
                {
                    CreateItem(srcItem, items);
                }

                // If this is an implant group, we add the implants with no market groups in this one
                if (marketGroup.ParentID == DBConstants.SkillHardwiringImplantsMarketGroupID ||
                    marketGroup.ParentID == DBConstants.AttributeEnhancersImplantsMarketGroupID)
                {
                    AddImplant(items, marketGroup);
                }

                // Store the items
                group.Items.AddRange(items.OrderBy(x => x.Name));
            }
        }

        /// <summary>
        /// Adds an implant.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="srcGroups">The market group.</param>
        private static void AddImplant(ICollection<SerializableItem> items, InvMarketGroups srcGroups)
        {
            string slotString = srcGroups.Name.Substring("Implant Slot ".Length);
            int slot = Int32.Parse(slotString, CultureConstants.InvariantCulture);

            // Enumerate all implants without market groups
            foreach (InvTypes srcItem in Database.InvTypesTable.Where(
                item => (item.MarketGroupID == null || s_nullMarketItems.Contains(item)) &&
                        Database.InvGroupsTable[item.GroupID].CategoryID == DBConstants.ImplantCategoryID &&
                        item.GroupID != DBConstants.CyberLearningImplantsGroupID).Select(
                            srcItem =>
                            new
                                {
                                    srcItem,
                                    slotAttrib =
                                Database.DgmTypeAttributesTable.Get(srcItem.ID, DBConstants.ImplantSlotPropertyID)
                                }).Where(x => x.slotAttrib != null && x.slotAttrib.GetIntValue == slot).Select(
                                    x => x.srcItem))
            {
                CreateItem(srcItem, items);
            }
        }

        /// <summary>
        /// Configures the null market items.
        /// </summary>
        private static void ConfigureNullMarketItems()
        {
            s_injectedMarketGroups = new List<InvMarketGroups>
                                         {
                                             new InvMarketGroups
                                                 {
                                                     Name = "Rookie Ships",
                                                     Description = "Capsuleer starter ship",
                                                     ID = DBConstants.RookieShipRootGroupID,
                                                     ParentID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Amarr",
                                                     Description = "Amarr rookie ship",
                                                     ID = DBConstants.RookieShipAmarrGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Caldari",
                                                     Description = "Caldari rookie ship",
                                                     ID = DBConstants.RookieShipCaldariGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Gallente",
                                                     Description = "Gallente rookie ship",
                                                     ID = DBConstants.RookieShipGallenteGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Minmatar",
                                                     Description = "Minmatar rookie ship",
                                                     ID = DBConstants.RookieShipMinmatarGroupID,
                                                     ParentID = DBConstants.RookieShipRootGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Unique Designs",
                                                     Description = "Ships of a unique design",
                                                     ID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     ParentID = DBConstants.ShipsMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Unique Shuttles",
                                                     Description = "Fast ships of a unique design",
                                                     ID = DBConstants.UniqueDesignShuttlesNonMarketGroupID,
                                                     ParentID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Unique Battleships",
                                                     Description = "Battleships ships of a unique design",
                                                     ID = DBConstants.UniqueDesignBattleshipsNonMarketGroupID,
                                                     ParentID = DBConstants.UniqueDesignsRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownShipIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Various Non-Market",
                                                     Description = "Non-Market Items",
                                                     ID = DBConstants.RootNonMarketGroupID,
                                                     ParentID = null,
                                                     IconID = DBConstants.UnknownIconID
                                                 }
                                         };

            // Manually set some items attributes
            Database.InvTypesTable[DBConstants.CivilianGatlingPulseLaserID].Published = true;
            Database.InvTypesTable[DBConstants.CivilianGatlingAutocannonID].Published = true;
            Database.InvTypesTable[DBConstants.CivilianGatlingRailgunID].Published = true;
            Database.InvTypesTable[DBConstants.CivilianLightElectronBlasterID].Published = true;
            Database.InvTypesTable[DBConstants.CivilianDataInterfaceID].Published = true;
            Database.InvTypesTable[DBConstants.TemperatePlanetID].Published = true;
            Database.InvTypesTable[DBConstants.IcePlanetID].Published = true;
            Database.InvTypesTable[DBConstants.GasPlanetID].Published = true;
            Database.InvTypesTable[DBConstants.OceanicPlanetID].Published = true;
            Database.InvTypesTable[DBConstants.LavaPlanetID].Published = true;
            Database.InvTypesTable[DBConstants.BarrenPlanetID].Published = true;
            Database.InvTypesTable[DBConstants.StormPlanetID].Published = true;
            Database.InvTypesTable[DBConstants.PlasmaPlanetID].Published = true;
            Database.InvTypesTable[DBConstants.ShatteredPlanetID].Published = true;
            Database.InvTypesTable[DBConstants.ImpairorID].Published = true;
            Database.InvTypesTable[DBConstants.IbisID].Published = true;
            Database.InvTypesTable[DBConstants.VelatorID].Published = true;
            Database.InvTypesTable[DBConstants.ReaperID].Published = true;
            Database.InvTypesTable[DBConstants.CapsuleID].Published = true;

            // Include only items that are published (Special condition check for blueprints)
            s_nullMarketItems = Database.InvTypesTable.Where(x => x.Published && x.MarketGroupID == null).Where(
                srcItem => Database.InvBlueprintTypesTable.Where(x => x.ID == srcItem.ID).Select(
                    item => new
                                {
                                    item,
                                    productItemID = Database.InvBlueprintTypesTable[srcItem.ID].ProductTypeID
                                }).All(item => Database.InvTypesTable[item.productItemID].Published)).ToList();

            // Set some attributes to items because their MarketGroupID is NULL
            foreach (InvTypes srcItem in s_nullMarketItems)
            {
                // Set all items to market groups manually
                srcItem.MarketGroupID = DBConstants.RootNonMarketGroupID;

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
                }
            }
        }
        
        /// <summary>
        /// Add properties to an item.
        /// </summary>
        /// <param name="srcItem"></param>
        /// <param name="groupItems"></param>
        /// <returns></returns>
        private static void CreateItem(InvTypes srcItem, ICollection<SerializableItem> groupItems)
        {
            Util.UpdatePercentDone(Database.ItemsTotalCount);

            srcItem.Generated = true;

            InvGroups itemGroup = Database.InvGroupsTable[srcItem.GroupID];

            // Creates the item with base informations
            SerializableItem item = new SerializableItem
                                        {
                                            ID = srcItem.ID,
                                            Name = srcItem.Name,
                                            Description = srcItem.Description ?? String.Empty,
                                            Icon = (srcItem.IconID.HasValue
                                                        ? Database.EveIconsTable[srcItem.IconID.Value].Icon
                                                        : String.Empty),
                                            PortionSize = srcItem.PortionSize,
                                            MetaGroup = ItemMetaGroup.None,
                                            Group = itemGroup.Name,
                                            Category = Database.InvCategoriesTable[itemGroup.CategoryID].Name
                                        };

            // Add the properties and prereqs
            IEnumerable<SerializablePropertyValue> props = AddItemPropsAndPrereq(srcItem, item);

            // Metagroup
            AddMetaGroup(srcItem, item);

            // Race ID
            AddRace(srcItem, item);

            // Set race to Faction if ship has Pirate Faction property
            if (props.Any(prop => prop.ID == DBConstants.ShipBonusPirateFactionPropertyID))
                item.Race = Race.Faction;

            // Look for slots
            if (Database.DgmTypeEffectsTable.Contains(srcItem.ID, DBConstants.LowSlotEffectID))
                item.Slot = ItemSlot.Low;
            else if (Database.DgmTypeEffectsTable.Contains(srcItem.ID, DBConstants.MedSlotEffectID))
                item.Slot = ItemSlot.Medium;
            else if (Database.DgmTypeEffectsTable.Contains(srcItem.ID, DBConstants.HiSlotEffectID))
                item.Slot = ItemSlot.High;
            else
                item.Slot = ItemSlot.None; // Replace with 'ItemSlot.NoSlot' after year 2013

            // Add this item
            groupItems.Add(item);

            // If the current item isn't in a market group then we are done
            if (s_nullMarketItems.Contains(srcItem))
                return;

            // Look for variations which are not in any market group
            foreach (InvTypes srcVariationItem in Database.InvMetaTypesTable.Where(x => x.ParentItemID == srcItem.ID).Select(
                variation => Database.InvTypesTable[variation.ItemID]).Where(
                    srcVariationItem => srcVariationItem.Published && s_nullMarketItems.Contains(srcVariationItem)))
            {
                srcVariationItem.RaceID = (int)Race.Faction;
                CreateItem(srcVariationItem, groupItems);
            }
        }

        /// <summary>
        /// Adds the meta group.
        /// </summary>
        /// <param name="srcItem">The source item.</param>
        /// <param name="item">The item.</param>
        private static void AddMetaGroup(IHasID srcItem, SerializableItem item)
        {
            int itemID = srcItem.ID;

            if (Database.InvBlueprintTypesTable.Any(x => x.ID == srcItem.ID))
                itemID = Database.InvBlueprintTypesTable.First(x => x.ID == srcItem.ID).ProductTypeID;

            foreach (InvMetaTypes relation in Database.InvMetaTypesTable.Where(x => x.ItemID == itemID))
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

            if (item.MetaGroup == ItemMetaGroup.None)
                item.MetaGroup = ItemMetaGroup.T1;
        }

        /// <summary>
        /// Adds the race.
        /// </summary>
        /// <param name="srcItem">The SRC item.</param>
        /// <param name="item">The item.</param>
        private static void AddRace(InvTypes srcItem, SerializableItem item)
        {
            item.Race = (Race)Enum.ToObject(typeof(Race), (srcItem.RaceID ?? 0));

            // Set race to Faction if item race is Jovian
            if (item.Race == Race.Jove)
                item.Race = Race.Faction;

            // Set race to ORE if it is in the ORE market groups
            // within mining barges, exhumers, industrial or capital industrial ships
            if (srcItem.MarketGroupID == DBConstants.OREMiningBargesMarketGroupID
                || srcItem.MarketGroupID == DBConstants.OREExhumersMarketGroupID
                || srcItem.MarketGroupID == DBConstants.OREIndustrialsMarketGroupID
                || srcItem.MarketGroupID == DBConstants.ORECapitalIndustrialsMarketGroupID)
                item.Race = Race.Ore;
        }

        /// <summary>
        /// Adds the item properties and prerequisites.
        /// </summary>
        /// <param name="srcItem">The SRC item.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private static IEnumerable<SerializablePropertyValue> AddItemPropsAndPrereq(InvTypes srcItem, SerializableItem item)
        {
            int[] prereqSkills = new int[DBConstants.RequiredSkillPropertyIDs.Count];
            int[] prereqLevels = new int[DBConstants.RequiredSkillPropertyIDs.Count];
            List<SerializablePropertyValue> props = new List<SerializablePropertyValue>();
            const int BaseWarpSpeed = 3;
            double warpSpeedMultiplier = 1;
            foreach (DgmTypeAttributes srcProp in Database.DgmTypeAttributesTable.Where(x => x.ItemID == srcItem.ID))
            {
                int propIntValue = srcProp.GetIntValue;

                // Is it a prereq skill ?
                int prereqIndex = DBConstants.RequiredSkillPropertyIDs.IndexOf(srcProp.AttributeID);
                if (prereqIndex > -1)
                {
                    prereqSkills[prereqIndex] = propIntValue;
                    continue;
                }

                // Is it a prereq level ?
                prereqIndex = DBConstants.RequiredSkillLevelPropertyIDs.IndexOf(srcProp.AttributeID);
                if (prereqIndex > -1)
                {
                    prereqLevels[prereqIndex] = propIntValue;
                    continue;
                }

                // Launcher group ?
                int launcherIndex = DBConstants.LauncherGroupPropertyIDs.IndexOf(srcProp.AttributeID);
                if (launcherIndex > -1)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = Database.InvGroupsTable[propIntValue].Name
                                  });
                    continue;
                }

                // Charge group ?
                int chargeIndex = DBConstants.ChargeGroupPropertyIDs.IndexOf(srcProp.AttributeID);
                if (chargeIndex > -1)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = Database.InvGroupsTable[propIntValue].Name
                                  });
                    continue;
                }

                // CanFitShip group ?
                int canFitShipIndex = DBConstants.CanFitShipGroupPropertyIDs.IndexOf(srcProp.AttributeID);
                if (canFitShipIndex > -1)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = Database.InvGroupsTable[propIntValue].Name
                                  });
                    continue;
                }

                // ModuleShip group ?
                int moduleShipIndex = DBConstants.ModuleShipGroupPropertyIDs.IndexOf(srcProp.AttributeID);
                if (moduleShipIndex > -1)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = Database.InvGroupsTable[propIntValue].Name
                                  });
                    continue;
                }

                // SpecialisationAsteroid group ?
                int specialisationAsteroidIndex = DBConstants.SpecialisationAsteroidGroupPropertyIDs.IndexOf(srcProp.AttributeID);
                if (specialisationAsteroidIndex > -1)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = Database.InvGroupsTable[propIntValue].Name
                                  });
                    continue;
                }

                // Reaction group ?
                int reactionIndex = DBConstants.ReactionGroupPropertyIDs.IndexOf(srcProp.AttributeID);
                if (reactionIndex > -1)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = Database.InvGroupsTable[propIntValue].Name
                                  });
                    continue;
                }

                // PosCargobayAccept group ?
                int posCargobayAcceptIndex = DBConstants.PosCargobayAcceptGroupPropertyIDs.IndexOf(srcProp.AttributeID);
                if (posCargobayAcceptIndex > -1)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = Database.InvGroupsTable[propIntValue].Name
                                  });
                    continue;
                }

                // Get the warp speed multiplier
                if (srcProp.AttributeID == DBConstants.WarpSpeedMultiplierPropertyID && srcProp.ValueFloat != null)
                    warpSpeedMultiplier = srcProp.ValueFloat.Value;

                // We calculate and add the ships warp speed
                if (srcProp.AttributeID == DBConstants.ShipWarpSpeedPropertyID)
                {
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = srcProp.AttributeID,
                                      Value = (BaseWarpSpeed * warpSpeedMultiplier).ToString(CultureConstants.InvariantCulture)
                                  });

                    // Also add packaged volume as a prop as only ships have 'ship warp speed' attribute
                    props.Add(new SerializablePropertyValue
                                  {
                                      ID = Properties.PropPackagedVolumeID,
                                      Value = GetPackagedVolume(srcItem.GroupID).ToString(CultureConstants.InvariantCulture)
                                  });
                }

                // Other props
                props.Add(new SerializablePropertyValue { ID = srcProp.AttributeID, Value = srcProp.FormatPropertyValue() });

                AddMetaData(item, propIntValue, srcProp);
            }

            CompleteItemPropertiesAddition(srcItem, props);

            // Add properties info to item
            item.Properties.AddRange(props);

            // Prerequisites completion
            List<SerializablePrerequisiteSkill> prereqs = new List<SerializablePrerequisiteSkill>();
            for (int i = 0; i < prereqSkills.Length; i++)
            {
                if (prereqSkills[i] != 0)
                    prereqs.Add(new SerializablePrerequisiteSkill { ID = prereqSkills[i], Level = prereqLevels[i] });
            }

            // Add prerequisite skills info to item
            item.PrerequisiteSkills.AddRange(prereqs);

            return props;
        }

        /// <summary>
        /// Adds the meta data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="propIntValue">The prop int value.</param>
        /// <param name="srcProp">The SRC prop.</param>
        private static void AddMetaData(SerializableItem item, int propIntValue, DgmTypeAttributes srcProp)
        {
            // Is metalevel property ?
            if (srcProp.AttributeID == DBConstants.MetaLevelPropertyID)
                item.MetaLevel = propIntValue;

            // Is techlevel property ?
            if (srcProp.AttributeID == DBConstants.TechLevelPropertyID)
            {
                switch (propIntValue)
                {
                    default:
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
            if (srcProp.AttributeID != DBConstants.MetaGroupPropertyID)
                return;

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

        /// <summary>
        /// Completes the item properties addition.
        /// </summary>
        /// <param name="srcItem">The SRC item.</param>
        /// <param name="props">The props.</param>
        private static void CompleteItemPropertiesAddition(InvTypes srcItem, ICollection<SerializablePropertyValue> props)
        {
            // Ensures there is a mass and add it to prop
            if (Math.Abs(srcItem.Mass) > double.Epsilon)
            {
                props.Add(new SerializablePropertyValue
                              {
                                  ID = DBConstants.MassPropertyID,
                                  Value = srcItem.Mass.ToString(CultureConstants.InvariantCulture)
                              });
            }

            // Ensures there is a cargo capacity and add it to prop
            if (Math.Abs(srcItem.Capacity - 0) > double.Epsilon)
            {
                props.Add(new SerializablePropertyValue
                              {
                                  ID = DBConstants.CargoCapacityPropertyID,
                                  Value = srcItem.Capacity.ToString(CultureConstants.InvariantCulture)
                              });
            }

            // Ensures there is a volume and add it to prop
            if (Math.Abs(srcItem.Volume - 0) > double.Epsilon)
            {
                props.Add(new SerializablePropertyValue
                              {
                                  ID = DBConstants.VolumePropertyID,
                                  Value = srcItem.Volume.ToString(CultureConstants.InvariantCulture)
                              });
            }

            // Add base price as a prop
            props.Add(new SerializablePropertyValue { ID = Properties.PropBasePriceID, Value = srcItem.BasePrice.FormatDecimal() });
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
                case DBConstants.InterdictorGroupID:
                case DBConstants.StrategicCruiserGroupID:
                    return 5000;
                case DBConstants.CruiserGroupID:
                case DBConstants.HeavyAssaultShipGroupID:
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
    }
}
