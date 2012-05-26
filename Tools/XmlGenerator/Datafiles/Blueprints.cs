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
    public static class Blueprints
    {
        private static List<InvMarketGroups> s_injectedMarketGroups;
        private static List<InvTypes> s_nullMarketBlueprints;

        /// <summary>
        /// Generate the skills datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating blueprints datafile... ");

            // Configure blueprints with Null market group
            ConfigureNullMarketBlueprint();

            Dictionary<int, SerializableBlueprintMarketGroup> groups = new Dictionary<int, SerializableBlueprintMarketGroup>();

            // Export blueprint groups           
            CreateMarketGroups(groups);

            // Create the parent-children groups relations
            foreach (SerializableBlueprintMarketGroup group in groups.Values)
            {
                IEnumerable<SerializableBlueprintMarketGroup> children = Database.InvMarketGroupsTable.Concat(
                    s_injectedMarketGroups).Where(x => x.ParentID == group.ID).Select(
                        x => groups[x.ID]).OrderBy(x => x.Name);

                group.SubGroups.AddRange(children);
            }

            // Sort groups
            IEnumerable<SerializableBlueprintMarketGroup> blueprintGroups = Database.InvMarketGroupsTable.Concat(
                s_injectedMarketGroups).Where(x => x.ParentID == DBConstants.BlueprintsMarketGroupID).Select(
                    x => groups[x.ID]).OrderBy(x => x.Name);

            // Reset the custom market groups
            s_nullMarketBlueprints.ForEach(srcItem => srcItem.MarketGroupID = null);

            // Serialize
            BlueprintsDatafile datafile = new BlueprintsDatafile();
            datafile.MarketGroups.AddRange(blueprintGroups);

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}",
                                            DateTime.Now.Subtract(startTime)).TrimEnd('0'));

            Util.SerializeXML(datafile, DatafileConstants.BlueprintsDatafile);
        }

        /// <summary>
        /// Creates the market groups.
        /// </summary>
        /// <param name="groups">The groups.</param>
        private static void CreateMarketGroups(IDictionary<int, SerializableBlueprintMarketGroup> groups)
        {
            foreach (InvMarketGroups marketGroup in Database.InvMarketGroupsTable.Concat(s_injectedMarketGroups))
            {
                SerializableBlueprintMarketGroup group = new SerializableBlueprintMarketGroup
                                                             {
                                                                 ID = marketGroup.ID,
                                                                 Name = marketGroup.Name,
                                                             };

                groups[marketGroup.ID] = group;

                // Add the items in this group
                List<SerializableBlueprint> blueprints = new List<SerializableBlueprint>();
                foreach (InvTypes item in Database.InvTypesTable.Where(
                    item => item.Published && item.MarketGroupID.GetValueOrDefault() == marketGroup.ID).Select(
                        item => new { item, group = Database.InvGroupsTable[item.GroupID] }).Where(
                            itemGroup => itemGroup.group.CategoryID == DBConstants.BlueprintCategoryID
                                         && itemGroup.group.Published).Select(itemGroup => itemGroup.item))
                {
                    CreateBlueprint(item, blueprints);
                }

                // Store the items
                group.Blueprints.AddRange(blueprints.OrderBy(x => x.Name));
            }
        }

        /// <summary>
        /// Configures the null market blueprint.
        /// </summary>
        private static void ConfigureNullMarketBlueprint()
        {
            // Create custom market groups that don't exist in EVE
            s_injectedMarketGroups = new List<InvMarketGroups>
                                         {
                                             new InvMarketGroups
                                                 {
                                                     Name = "Various Non-Market",
                                                     Description = "Various blueprints not in EVE market",
                                                     ID = DBConstants.BlueprintRootNonMarketGroupID,
                                                     ParentID = DBConstants.BlueprintsMarketGroupID,
                                                     IconID = DBConstants.UnknownBlueprintBackdropIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Tech I",
                                                     Description = "Tech I blueprints not in EVE market",
                                                     ID = DBConstants.BlueprintTechINonMarketGroupID,
                                                     ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownBlueprintBackdropIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Tech II",
                                                     Description = "Tech II blueprints not in EVE market",
                                                     ID = DBConstants.BlueprintTechIINonMarketGroupID,
                                                     ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownBlueprintBackdropIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Storyline",
                                                     Description = "Storyline blueprints not in EVE market",
                                                     ID = DBConstants.BlueprintStorylineNonMarketGroupID,
                                                     ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownBlueprintBackdropIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Faction",
                                                     Description = "Faction blueprints not in EVE market",
                                                     ID = DBConstants.BlueprintFactionNonMarketGroupID,
                                                     ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownBlueprintBackdropIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Officer",
                                                     Description = "Officer blueprints not in EVE market",
                                                     ID = DBConstants.BlueprintOfficerNonMarketGroupID,
                                                     ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownBlueprintBackdropIconID
                                                 },
                                             new InvMarketGroups
                                                 {
                                                     Name = "Tech III",
                                                     Description = "Tech III blueprints not in EVE market",
                                                     ID = DBConstants.BlueprintTechIIINonMarketGroupID,
                                                     ParentID = DBConstants.BlueprintRootNonMarketGroupID,
                                                     IconID = DBConstants.UnknownBlueprintBackdropIconID
                                                 }
                                         };

            s_nullMarketBlueprints = Database.InvTypesTable.Where(
                item => item.MarketGroupID == null && !item.Name.Contains("TEST")).Where(
                    item => Database.InvBlueprintTypesTable.Any(blueprintType => blueprintType.ID == item.ID)).Select(
                        blueprint => new
                                         {
                                             blueprint,
                                             productedItemID = Database.InvBlueprintTypesTable[blueprint.ID].ProductTypeID,
                                         }).Where(
                                             blueprint =>
                                             Database.InvTypesTable.Any(item => item.ID == blueprint.productedItemID) &&
                                             Database.InvTypesTable[blueprint.productedItemID].Published).Select(
                                                 item =>
                                                     {
                                                         Util.UpdatePercentDone(Database.BlueprintsTotalCount);
                                                         item.blueprint.Published = true;
                                                         return item.blueprint;
                                                     }).ToList();

            // Set the market group of the blueprints with NULL MarketGroupID to custom market groups
            foreach (InvTypes item in s_nullMarketBlueprints)
            {
                // Set some blueprints to market groups manually
                SetMarketGroupManually(item);

                // Set some blueprints to custom market groups according to their metagroup
                SetMarketGroupFromMetaGroup(item);

                if (item.MarketGroupID == null)
                    item.MarketGroupID = DBConstants.BlueprintTechINonMarketGroupID;
            }
        }

        /// <summary>
        /// Sets the market group.
        /// </summary>
        /// <param name="item">The item.</param>
        private static void SetMarketGroupManually(InvTypes item)
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
            }
        }

        /// <summary>
        /// Sets the market group from meta group.
        /// </summary>
        /// <param name="item">The item.</param>
        private static void SetMarketGroupFromMetaGroup(InvTypes item)
        {
            foreach (InvMetaTypes relation in Database.InvMetaTypesTable.Where(
                x => x.ItemID == Database.InvBlueprintTypesTable[item.ID].ProductTypeID))
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
        }

        /// <summary>
        /// Add properties to a blueprint.
        /// </summary>
        /// <param name="srcBlueprint"></param>
        /// <param name="blueprintsGroup"></param>
        /// <returns></returns>
        private static void CreateBlueprint(InvTypes srcBlueprint, ICollection<SerializableBlueprint> blueprintsGroup)
        {
            Util.UpdatePercentDone(Database.BlueprintsTotalCount);

            srcBlueprint.Generated = true;

            // Creates the blueprint with base informations
            SerializableBlueprint blueprint = new SerializableBlueprint
                                                  {
                                                      ID = srcBlueprint.ID,
                                                      Name = srcBlueprint.Name,
                                                      Icon = (srcBlueprint.IconID.HasValue
                                                                  ? Database.EveIconsTable[srcBlueprint.IconID.Value].Icon
                                                                  : String.Empty),
                                                  };

            // Export attributes
            foreach (InvBlueprintTypes attribute in Database.InvBlueprintTypesTable.Where(x => x.ID == srcBlueprint.ID))
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
            SetBlueprintMetaGroup(srcBlueprint, blueprint);

            // Export item requirements
            ExportRequirements(srcBlueprint, blueprint);

            // Look for the tech 2 variations that this blueprint invents
            IEnumerable<int> listOfInventionTypeID = Database.InvMetaTypesTable.Where(
                x => x.ParentItemID == blueprint.ProduceItemID &&
                     x.MetaGroupID == DBConstants.TechIIMetaGroupID).Select(x => x.ItemID).SelectMany(
                         relationItemID => Database.InvBlueprintTypesTable.Where(
                             x => x.ProductTypeID == relationItemID).Select(x => x.ID),
                         (relationItemID, variationItemID) => Database.InvTypesTable[variationItemID].ID);

            // Add invention blueprints to item
            blueprint.InventionTypeID.AddRange(listOfInventionTypeID);

            // Add this item
            blueprintsGroup.Add(blueprint);
        }

        /// <summary>
        /// Sets the blueprint meta group.
        /// </summary>
        /// <param name="srcBlueprint">The SRC blueprint.</param>
        /// <param name="blueprint">The blueprint.</param>
        private static void SetBlueprintMetaGroup(InvTypes srcBlueprint, SerializableBlueprint blueprint)
        {
            foreach (InvMetaTypes relation in Database.InvMetaTypesTable.Where(
                x => x.ItemID == Database.InvBlueprintTypesTable[srcBlueprint.ID].ProductTypeID))
            {
                switch (relation.MetaGroupID)
                {
                    default:
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
                case DBConstants.BlueprintOfficerNonMarketGroupID:
                    blueprint.MetaGroup = ItemMetaGroup.Officer;
                    break;
                case DBConstants.BlueprintTechIIINonMarketGroupID:
                    blueprint.MetaGroup = ItemMetaGroup.T3;
                    break;
                case DBConstants.BlueprintTechIINonMarketGroupID:
                    blueprint.MetaGroup = ItemMetaGroup.T2;
                    break;
            }

            if (blueprint.MetaGroup == ItemMetaGroup.None)
                blueprint.MetaGroup = ItemMetaGroup.T1;
        }

        /// <summary>
        /// Export item requirements. 
        /// </summary>
        /// <param name="srcBlueprint"></param>
        /// <param name="blueprint"></param>
        private static void ExportRequirements(IHasID srcBlueprint, SerializableBlueprint blueprint)
        {
            List<SerializablePrereqSkill> prerequisiteSkills = new List<SerializablePrereqSkill>();
            List<SerializableRequiredMaterial> requiredMaterials = new List<SerializableRequiredMaterial>();

            // Add the required raw materials
            AddRequiredRawMaterials(blueprint.ProduceItemID, requiredMaterials);

            // Add the required extra materials
            AddRequiredExtraMaterials(srcBlueprint.ID, prerequisiteSkills, requiredMaterials);

            // Add prerequisite skills to item
            blueprint.PrereqSkill.AddRange(prerequisiteSkills.OrderBy(x => x.Activity));

            // Add required materials to item
            blueprint.ReqMaterial.AddRange(requiredMaterials.OrderBy(x => x.Activity));
        }

        /// <summary>
        /// Adds the raw materials needed to produce an item.
        /// </summary>
        /// <param name="produceItemID">The produce item ID.</param>
        /// <param name="requiredMaterials">The required materials.</param>
        private static void AddRequiredRawMaterials(int produceItemID,
                                                    ICollection<SerializableRequiredMaterial> requiredMaterials)
        {
            // Find the raw materials needed for the produced item and add them to the list
            IEnumerable<SerializableRequiredMaterial> rawMaterials = Database.InvTypeMaterialsTable.Where(
                x => x.TypeID == produceItemID).Select(
                    reprocItem => new SerializableRequiredMaterial
                                      {
                                          ID = reprocItem.MaterialTypeID,
                                          Quantity = reprocItem.Quantity,
                                          DamagePerJob = 1,
                                          Activity = (int)BlueprintActivity.Manufacturing,
                                          WasteAffected = 1
                                      });

            requiredMaterials.AddRange(rawMaterials);
        }

        /// <summary>
        /// Adds the extra materials needed to produce an item.
        /// </summary>
        /// <param name="blueprintID">The blueprint ID.</param>
        /// <param name="prerequisiteSkills">The prerequisite skills.</param>
        /// <param name="requiredMaterials">The required materials.</param>
        private static void AddRequiredExtraMaterials(int blueprintID,
                                                      ICollection<SerializablePrereqSkill> prerequisiteSkills,
                                                      ICollection<SerializableRequiredMaterial> requiredMaterials)
        {
            // Find the additional extra materials and add them to the list
            foreach (RamTypeRequirements requirement in Database.RamTypeRequirementsTable.Where(x => x.TypeID == blueprintID))
            {
                // Is it a skill ? Add it to the prerequisities skills list
                if (Database.InvTypesTable.Any(x => x.ID == requirement.RequiredTypeID &&
                                                    Database.InvGroupsTable.Any(
                                                        y => y.ID == x.GroupID &&
                                                             y.CategoryID == DBConstants.SkillCategoryID)))
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
                        foreach (InvTypeMaterials reprocItem in Database.InvTypeMaterialsTable.Where(
                            x => x.TypeID == requirement.RequiredTypeID))
                        {
                            if (requiredMaterials.All(x => x.ID != reprocItem.MaterialTypeID))
                                continue;

                            SerializableRequiredMaterial material = requiredMaterials.First(
                                x => x.ID == reprocItem.MaterialTypeID);

                            material.Quantity -= requirement.Quantity * reprocItem.Quantity;

                            if (material.Quantity < 1)
                                requiredMaterials.Remove(material);
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
                                                ICollection<SerializablePrereqSkill> prerequisiteSkills)
        {
            int[] prereqSkills = new int[DBConstants.RequiredSkillPropertyIDs.Count];
            int[] prereqLevels = new int[DBConstants.RequiredSkillPropertyIDs.Count];

            foreach (DgmTypeAttributes attribute in Database.DgmTypeAttributesTable.Where(
                x => x.ItemID == requirement.RequiredTypeID))
            {
                int attributeIntValue = attribute.GetIntValue;

                // Is it a prereq skill ?
                int prereqIndex = DBConstants.RequiredSkillPropertyIDs.IndexOf(attribute.AttributeID);
                if (prereqIndex >= 0)
                {
                    prereqSkills[prereqIndex] = attributeIntValue;
                    continue;
                }

                // Is it a prereq level ?
                prereqIndex = DBConstants.RequiredSkillLevelPropertyIDs.IndexOf(attribute.AttributeID);
                if (prereqIndex < 0)
                    continue;

                prereqLevels[prereqIndex] = attributeIntValue;
            }

            // Add the prerequisite skills
            for (int i = 0; i < prereqSkills.Length; i++)
            {
                if (prereqSkills[i] != 0)
                {
                    prerequisiteSkills.Add(new SerializablePrereqSkill
                                               {
                                                   ID = prereqSkills[i],
                                                   Level = prereqLevels[i],
                                                   Activity = requirement.ActivityID
                                               });
                }
            }
        }
    }
}
