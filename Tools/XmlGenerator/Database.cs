using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.SqlClient;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator
{
    public static class Database
    {
        private static EveStaticDataEntities s_entities;

        /// <summary>
        /// Makes the context available to all database methods without
        /// reinstantiating each time.
        /// </summary>
        private static EveStaticDataEntities Context
        {
            get
            {
                if (s_entities == null)
                {
                    // Initialize the connection string builder for the underlying provider
                    SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder()
                    {
                        // Set the properties for the data source
                        DataSource = @".\SQLEXPRESS",
                        InitialCatalog = "EveStaticData",
                        IntegratedSecurity = true,
                        MultipleActiveResultSets = true,
                        ApplicationName = "EntityFramework",
                    };

                    // Initialize the EntityConnectionStringBuilder
                    EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder()
                        {
                            // Set the Metadata location
                            Metadata = @"res://*/EveStaticData.csdl|res://*/EveStaticData.ssdl|res://*/EveStaticData.msl",

                            //Set the provider name
                            Provider = "System.Data.SqlClient",

                            // Set the provider-specific connection string
                            ProviderConnectionString = sqlBuilder.ToString(),
                        };

                    // Initialize the EntityConnection
                    EntityConnection connection = new EntityConnection(entityBuilder.ToString());

                    s_entities = new EveStaticDataEntities(connection);
                }
                return s_entities;
            }
        }

        /// <summary>
        /// EVE Agents.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Agents.</returns>
        internal static Bag<AgtAgents> Agents()
        {
            IndexedList<AgtAgents> list = new IndexedList<AgtAgents>();

            foreach (agtAgents agent in Context.agtAgents)
            {
                AgtAgents item = new AgtAgents
                                {
                                    ID = agent.agentID,
                                };

                if (agent.divisionID.HasValue)
                    item.DivisionID = agent.divisionID.Value;

                if (agent.locationID.HasValue)
                    item.LocationID = agent.locationID.Value;

                if (agent.level.HasValue)
                    item.Level = agent.level.Value;

                if (agent.quality.HasValue)
                    item.Quality = agent.quality.Value;

                if (agent.agentTypeID.HasValue)
                    item.AgentTypeID = agent.agentTypeID.Value;

                list.Items.Add(item);
            }

            return new Bag<AgtAgents>(list);
        }

        /// <summary>
        /// EVE Agent Types.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Agent Types.</returns>
        internal static Bag<AgtAgentTypes> AgentTypes()
        {
            IndexedList<AgtAgentTypes> list = new IndexedList<AgtAgentTypes>();

            foreach (agtAgentTypes agentType in Context.agtAgentTypes)
            {
                AgtAgentTypes item = new AgtAgentTypes
                                {
                                    ID = agentType.agentTypeID,
                                    AgentType = agentType.agentType
                                };

                list.Items.Add(item);
            }

            return new Bag<AgtAgentTypes>(list);
        }

        /// <summary>
        /// EVE Agent Config.
        /// </summary>
        /// <returns><c>List</c> of EVE Agent Config.</returns>
        internal static List<AgtConfig> AgentConfig()
        {
            List<AgtConfig> list = new List<AgtConfig>();

            foreach (agtConfig agentConfig in Context.agtConfig)
            {
                AgtConfig item = new AgtConfig
                                {
                                    ID = agentConfig.agentID,
                                    Key = agentConfig.k,
                                    Value = Convert.ToInt32(agentConfig.v)
                                };

                list.Add(item);
            }

            return list;
        }
        /// <summary>
        /// EVE Research Agents.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Research Agents.</returns>
        internal static Bag<AgtResearchAgents> ResearchAgents()
        {
            IndexedList<AgtResearchAgents> list = new IndexedList<AgtResearchAgents>();

            foreach (agtResearchAgents researchAgent in Context.agtResearchAgents)
            {
                AgtResearchAgents item = new AgtResearchAgents
                                {
                                    ID = researchAgent.agentID,
                                    ResearchSkillID = researchAgent.typeID
                                };

                list.Items.Add(item);
            }

            return new Bag<AgtResearchAgents>(list);
        }

        /// <summary>
        /// EVE NPC Divisions.
        /// </summary>
        /// <returns><c>Bag</c> of EVE NPC Divisions.</returns>
        internal static Bag<CrpNPCDivisions> NPCDivisions()
        {
            IndexedList<CrpNPCDivisions> list = new IndexedList<CrpNPCDivisions>();

            foreach (crpNPCDivisions npcDivision in Context.crpNPCDivisions)
            {
                CrpNPCDivisions item = new CrpNPCDivisions
                                {
                                    ID = npcDivision.divisionID,
                                    DivisionName = npcDivision.divisionName
                                };

                list.Items.Add(item);
            }

            return new Bag<CrpNPCDivisions>(list);
        }

        /// <summary>
        /// EVE Names.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Names.</returns>
        internal static Bag<EveNames> Names()
        {
            IndexedList<EveNames> list = new IndexedList<EveNames>();

            foreach (eveNames name in Context.eveNames)
            {
                EveNames item = new EveNames
                               {
                                   ID = name.itemID,
                                   Name = name.itemName
                               };

                list.Items.Add(item);
            }

            return new Bag<EveNames>(list);
        }

        /// <summary>
        /// EVE Units.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Units.</returns>
        internal static Bag<EveUnit> Units()
        {
            IndexedList<EveUnit> list = new IndexedList<EveUnit>();

            foreach (eveUnits unit in Context.eveUnits)
            {
                EveUnit item = new EveUnit
                               {
                                   Description = unit.description.Clean(),
                                   DisplayName = unit.displayName.Clean(),
                                   ID = unit.unitID,
                                   Name = unit.unitName
                               };

                list.Items.Add(item);
            }

            return new Bag<EveUnit>(list);
        }

        /// <summary>
        /// EVE Icons.
        /// </summary>
        /// <returns><c>Bag</c> of icons.</returns>
        internal static Bag<EveIcons> Icons()
        {
            IndexedList<EveIcons> list = new IndexedList<EveIcons>();

            foreach (eveIcons icon in Context.eveIcons)
            {
                EveIcons item = new EveIcons
                               {
                                   ID = icon.iconID,
                                   Icon = icon.iconFile
                               };

                list.Items.Add(item);
            }

            return new Bag<EveIcons>(list);
        }

        /// <summary>
        /// EVE Attributes.
        /// </summary>
        /// <returns><c>Bag</c> of Attributes.</returns>
        internal static Bag<DgmAttributeTypes> Attributes()
        {
            IndexedList<DgmAttributeTypes> list = new IndexedList<DgmAttributeTypes>();

            foreach (dgmAttributeTypes attribute in Context.dgmAttributeTypes)
            {
                DgmAttributeTypes item = new DgmAttributeTypes
                               {
                                   ID = attribute.attributeID,
                                   CategoryID = attribute.categoryID,
                                   Description = attribute.description.Clean(),
                                   DisplayName = attribute.displayName.Clean(),
                                   IconID = attribute.iconID,
                                   Name = attribute.attributeName.Clean(),
                                   UnitID = attribute.unitID,
                               };

                if (attribute.defaultValue.HasValue)
                    item.DefaultValue = attribute.defaultValue.Value.ToString();

                if (attribute.published.HasValue)
                    item.Published = attribute.published.Value;

                if (attribute.highIsGood.HasValue)
                    item.HigherIsBetter = attribute.highIsGood.Value;

                list.Items.Add(item);
            }

            return new Bag<DgmAttributeTypes>(list);
        }

        /// <summary>
        /// Attribute categories.
        /// </summary>
        /// <returns><c>Bag</c> of Attribute Categories.</returns>
        internal static Bag<DgmAttributeCategory> AttributeCategories()
        {
            IndexedList<DgmAttributeCategory> list = new IndexedList<DgmAttributeCategory>();

            foreach (dgmAttributeCategories category in Context.dgmAttributeCategories)
            {
                DgmAttributeCategory item = new DgmAttributeCategory
                               {
                                   ID = category.categoryID,
                                   Description = category.categoryDescription.Clean(),
                                   Name = category.categoryName.Clean()
                               };

                list.Items.Add(item);
            }

            return new Bag<DgmAttributeCategory>(list);
        }

        /// <summary>
        /// Regions in the EVE Universe.
        /// </summary>
        /// <returns><c>Bag</c> of all regions in EVE.</returns>
        internal static Bag<MapRegion> Regions()
        {
            IndexedList<MapRegion> list = new IndexedList<MapRegion>();

            foreach (mapRegions region in Context.mapRegions)
            {
                MapRegion item = new MapRegion
                               {
                                   ID = region.regionID,
                                   Name = region.regionName,
                                   FactionID = region.factionID
                               };

                list.Items.Add(item);
            }

            return new Bag<MapRegion>(list);
        }

        /// <summary>
        /// Constallations in the EVE Universe.
        /// </summary>
        /// <returns><c>Bag</c> of Constallations in EVE.</returns>
        internal static Bag<MapConstellation> Constellations()
        {
            IndexedList<MapConstellation> list = new IndexedList<MapConstellation>();

            foreach (mapConstellations constellation in Context.mapConstellations)
            {
                MapConstellation item = new MapConstellation
                               {
                                   ID = constellation.constellationID,
                                   Name = constellation.constellationName,
                               };

                if (constellation.regionID.HasValue)
                    item.RegionID = constellation.regionID.Value;

                list.Items.Add(item);
            }

            return new Bag<MapConstellation>(list);
        }

        /// <summary>
        /// Solar Systems in EVE.
        /// </summary>
        /// <returns><c>Bag</c> of Solar Systems in the EVE.</returns>
        internal static Bag<MapSolarSystem> Solarsystems()
        {
            IndexedList<MapSolarSystem> list = new IndexedList<MapSolarSystem>();

            foreach (mapSolarSystems solarsystem in Context.mapSolarSystems)
            {
                MapSolarSystem item = new MapSolarSystem
                               {
                                   ID = solarsystem.solarSystemID,
                                   Name = solarsystem.solarSystemName
                               };

                if (solarsystem.constellationID.HasValue)
                    item.ConstellationID = solarsystem.constellationID.Value;

                if (solarsystem.security.HasValue)
                    item.SecurityLevel = (float) solarsystem.security.Value;

                if (solarsystem.x.HasValue)
                    item.X = solarsystem.x.Value;

                if (solarsystem.y.HasValue)
                    item.Y = solarsystem.y.Value;

                if (solarsystem.z.HasValue)
                    item.Z = solarsystem.z.Value;

                list.Items.Add(item);
            }

            return new Bag<MapSolarSystem>(list);
        }

        /// <summary>
        /// Stations in the EVE Universe
        /// </summary>
        /// <returns><c>Bag</c> of Stations in the EVE Universe.</returns>
        internal static Bag<StaStation> Stations()
        {
            IndexedList<StaStation> list = new IndexedList<StaStation>();

            foreach (staStations station in Context.staStations)
            {
                StaStation item = new StaStation
                               {
                                   ID = station.stationID,
                                   Name = station.stationName,
                               };

                if (station.reprocessingEfficiency.HasValue)
                    item.ReprocessingEfficiency = (float) station.reprocessingEfficiency.Value;

                if (station.reprocessingStationsTake.HasValue)
                    item.ReprocessingStationsTake = (float) station.reprocessingStationsTake.Value;

                if (station.security.HasValue)
                    item.SecurityLevel = station.security.Value;

                if (station.solarSystemID.HasValue)
                    item.SolarSystemID = station.solarSystemID.Value;

                if (station.corporationID.HasValue)
                    item.CorporationID = station.corporationID.Value;

                list.Items.Add(item);
            }

            return new Bag<StaStation>(list);
        }

        /// <summary>
        /// Jumps between two solar systems in the EVE Universe.
        /// </summary>
        /// <returns><c>List</c> of jumps between SolarSystems in EVE.</returns>
        internal static List<MapSolarSystemJump> Jumps()
        {
            List<MapSolarSystemJump> list = new List<MapSolarSystemJump>();

            foreach (mapSolarSystemJumps jump in Context.mapSolarSystemJumps)
            {
                MapSolarSystemJump item = new MapSolarSystemJump
                               {
                                   A = jump.fromSolarSystemID,
                                   B = jump.toSolarSystemID
                               };

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Inventory Blueprint Types.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Blueprint Types.</returns>
        internal static Bag<InvBlueprintTypes> BlueprintTypes()
        {
            IndexedList<InvBlueprintTypes> list = new IndexedList<InvBlueprintTypes>();

            foreach (invBlueprintTypes blueprint in Context.invBlueprintTypes)
            {
                InvBlueprintTypes item = new InvBlueprintTypes
                               {
                                   ID = blueprint.blueprintTypeID,
                                   ParentID = blueprint.parentBlueprintTypeID,
                               };

                if (blueprint.productTypeID.HasValue)
                    item.ProductTypeID = blueprint.productTypeID.Value;

                if (blueprint.productionTime.HasValue)
                    item.ProductionTime = blueprint.productionTime.Value;

                if (blueprint.techLevel.HasValue)
                    item.TechLevel = blueprint.techLevel.Value;

                if (blueprint.researchProductivityTime.HasValue)
                    item.ResearchProductivityTime = blueprint.researchProductivityTime.Value;

                if (blueprint.researchMaterialTime.HasValue)
                    item.ResearchMaterialTime = blueprint.researchMaterialTime.Value;

                if (blueprint.researchCopyTime.HasValue)
                    item.ResearchCopyTime = blueprint.researchCopyTime.Value;

                if (blueprint.researchTechTime.HasValue)
                    item.ResearchTechTime = blueprint.researchTechTime.Value;

                if (blueprint.productivityModifier.HasValue)
                    item.ProductivityModifier = blueprint.productivityModifier.Value;

                if (blueprint.wasteFactor.HasValue)
                    item.WasteFactor = blueprint.wasteFactor.Value;

                if (blueprint.maxProductionLimit.HasValue)
                    item.MaxProductionLimit = blueprint.maxProductionLimit.Value;

                list.Items.Add(item);
            }

            return new Bag<InvBlueprintTypes>(list);
        }

        /// <summary>
        /// Inventory Item Market Groups.
        /// </summary>
        /// <returns><c>Bag</c> of Market Groups available on the market.</returns>
        internal static Bag<InvMarketGroup> MarketGroups()
        {
            IndexedList<InvMarketGroup> list = new IndexedList<InvMarketGroup>();

            foreach (invMarketGroups marketGroup in Context.invMarketGroups)
            {
                InvMarketGroup item = new InvMarketGroup
                               {
                                   ID = marketGroup.marketGroupID,
                                   Description = marketGroup.description.Clean(),
                                   IconID = marketGroup.iconID,
                                   Name = marketGroup.marketGroupName,
                                   ParentID = marketGroup.parentGroupID
                               };

                list.Items.Add(item);
            }

            return new Bag<InvMarketGroup>(list);
        }

        /// <summary>
        /// Inventory Item Groups.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Groups.</returns>
        internal static Bag<InvGroup> Groups()
        {
            IndexedList<InvGroup> list = new IndexedList<InvGroup>();

            foreach (invGroups group in Context.invGroups)
            {
                InvGroup item = new InvGroup
                               {
                                   ID = group.groupID,
                                   Name = group.groupName,
                                   Published = group.published.Value
                               };

                if (group.categoryID.HasValue)
                    item.CategoryID = group.categoryID.Value;

                list.Items.Add(item);
            }

            return new Bag<InvGroup>(list);
        }

        /// <summary>
        /// Inventory Types.
        /// </summary>
        /// <returns><c>Bag</c> of items from the Inventory.</returns>
        internal static Bag<InvType> Types()
        {
            IndexedList<InvType> list = new IndexedList<InvType>();

            foreach (invTypes type in Context.invTypes)
            {
                InvType item = new InvType
                               {
                                   ID = type.typeID,
                                   Description = type.description.Clean(),
                                   IconID = type.iconID,
                                   MarketGroupID = type.marketGroupID,
                                   Name = type.typeName,
                                   RaceID = type.raceID
                               };

                if (type.basePrice.HasValue)
                    item.BasePrice = type.basePrice.Value;

                if (type.capacity.HasValue)
                    item.Capacity = type.capacity.Value;

                if (type.groupID.HasValue)
                    item.GroupID = type.groupID.Value;

                if (type.mass.HasValue)
                    item.Mass = type.mass.Value;

                if (type.published.HasValue)
                    item.Published = type.published.Value;

                if (type.volume.HasValue)
                    item.Volume = type.volume.Value;

                if (type.portionSize.HasValue)
                    item.PortionSize = type.portionSize.Value;

                list.Items.Add(item);
            }

            return new Bag<InvType>(list);
        }

        /// <summary>
        /// Requirements used for an Activity.
        /// </summary>
        /// <returns>List of Requirements needed for a particular activity.</returns>
        internal static List<RamTypeRequirements> TypeRequirements()
        {
            List<RamTypeRequirements> list = new List<RamTypeRequirements>();

            foreach (ramTypeRequirements requirement in Context.ramTypeRequirements)
            {
                RamTypeRequirements item = new RamTypeRequirements
                               {
                                   TypeID = requirement.typeID,
                                   ActivityID = requirement.activityID,
                                   RequiredTypeID = requirement.requiredTypeID
                               };

                if (requirement.quantity.HasValue)
                    item.Quantity = requirement.quantity.Value;

                if (requirement.damagePerJob.HasValue)
                    item.DamagePerJob = requirement.damagePerJob.Value;

                if (requirement.recycle.HasValue)
                    item.Recyclable = requirement.recycle.Value;

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Materials.
        /// </summary>
        /// <returns>List of Materials.</returns>
        internal static List<InvTypeMaterials> TypeMaterials()
        {
            List<InvTypeMaterials> list = new List<InvTypeMaterials>();

            foreach (invTypeMaterials material in Context.invTypeMaterials)
            {
                InvTypeMaterials item = new InvTypeMaterials
                               {
                                   TypeID = material.typeID,
                                   MaterialTypeID = material.materialTypeID,
                                   Quantity = material.quantity
                               };

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Certificate Categories.
        /// </summary>
        /// <returns><c>Bag</c> of Certificate Categories.</returns>
        internal static Bag<CrtCategories> CertificateCategories()
        {
            IndexedList<CrtCategories> list = new IndexedList<CrtCategories>();

            foreach (crtCategories category in Context.crtCategories)
            {
                CrtCategories item = new CrtCategories
                               {
                                   ID = category.categoryID,
                                   CategoryName = category.categoryName,
                                   Description = category.description.Clean()
                               };

                list.Items.Add(item);
            }

            return new Bag<CrtCategories>(list);
        }

        /// <summary>
        /// Certificate Classes.
        /// </summary>
        /// <returns><c>Bag</c> of Classes of Certificate.</returns>
        internal static Bag<CrtClasses> CertificateClasses()
        {
            IndexedList<CrtClasses> list = new IndexedList<CrtClasses>();

            foreach (crtClasses cClass in Context.crtClasses)
            {
                CrtClasses item = new CrtClasses
                               {
                                   ID = cClass.classID,
                                   ClassName = cClass.className,
                                   Description = cClass.description.Clean()
                               };

                list.Items.Add(item);
            }

            return new Bag<CrtClasses>(list);
        }

        /// <summary>
        /// Certificates.
        /// </summary>
        /// <returns><c>Bag</c> of Certificates.</returns>
        internal static Bag<CrtCertificates> Certificates()
        {
            IndexedList<CrtCertificates> list = new IndexedList<CrtCertificates>();

            foreach (crtCertificates certificate in Context.crtCertificates)
            {
                CrtCertificates item = new CrtCertificates
                               {
                                   ID = certificate.certificateID,
                                   Description = certificate.description.Clean()
                               };

                if (certificate.categoryID.HasValue)
                    item.CategoryID = certificate.categoryID.Value;

                if (certificate.classID.HasValue)
                    item.ClassID = certificate.classID.Value;

                if (certificate.grade.HasValue)
                    item.Grade = certificate.grade.Value;

                list.Items.Add(item);
            }

            return new Bag<CrtCertificates>(list);
        }

        /// <summary>
        /// Certificate Recommendations.
        /// </summary>
        /// <returns><c>Bag</c> of Certificate Recommendations.</returns>
        internal static Bag<CrtRecommendations> CertificateRecommendations()
        {
            IndexedList<CrtRecommendations> list = new IndexedList<CrtRecommendations>();

            foreach (crtRecommendations recommendation in Context.crtRecommendations)
            {
                CrtRecommendations item = new CrtRecommendations
                               {
                                   ID = recommendation.recommendationID,
                                   Level = recommendation.recommendationLevel,
                               };

                if (recommendation.certificateID.HasValue)
                    item.CertificateID = recommendation.certificateID.Value;

                if (recommendation.shipTypeID.HasValue)
                    item.ShipTypeID = recommendation.shipTypeID.Value;

                list.Items.Add(item);
            }

            return new Bag<CrtRecommendations>(list);
        }

        /// <summary>
        /// Certificate Relationships.
        /// </summary>
        /// <returns><c>Bag</c> of parent-child relationships between certificates.</returns>
        internal static Bag<CrtRelationships> CertificateRelationships()
        {
            IndexedList<CrtRelationships> list = new IndexedList<CrtRelationships>();

            foreach (crtRelationships relationship in Context.crtRelationships)
            {
                CrtRelationships item = new CrtRelationships
                               {
                                   ID = relationship.relationshipID,
                                   ParentID = relationship.parentID,
                                   ParentLevel = relationship.parentLevel,
                               };

                if (relationship.parentTypeID != 0)
                    item.ParentTypeID = relationship.parentTypeID;

                if (relationship.childID.HasValue)
                    item.ChildID = relationship.childID.Value;

                list.Items.Add(item);
            }

            return new Bag<CrtRelationships>(list);
        }

        /// <summary>
        /// Type Attributes.
        /// </summary>
        /// <returns><c>RelationSet</c> of attributes for types.</returns>
        internal static RelationSet<DgmTypeAttribute> TypeAttributes()
        {
            List<DgmTypeAttribute> list = new List<DgmTypeAttribute>();

            foreach (dgmTypeAttributes typeAttribute in Context.dgmTypeAttributes)
            {
                DgmTypeAttribute item = new DgmTypeAttribute
                               {
                                   AttributeID = typeAttribute.attributeID,
                                   ItemID = typeAttribute.typeID,
                                   ValueFloat = typeAttribute.valueFloat,
                                   ValueInt = typeAttribute.valueInt
                               };

                list.Add(item);
            }

            return new RelationSet<DgmTypeAttribute>(list);
        }

        /// <summary>
        /// Meta Types.
        /// </summary>
        /// <returns><c>RelationSet</c> parent-child relationships between types.</returns>
        internal static RelationSet<InvMetaType> MetaTypes()
        {
            List<InvMetaType> list = new List<InvMetaType>();

            foreach (invMetaTypes metaType in Context.invMetaTypes)
            {
                InvMetaType item = new InvMetaType
                               {
                                   ItemID = metaType.typeID,
                                   MetaGroupID = Convert.ToInt32(metaType.metaGroupID),
                                   ParentItemID = Convert.ToInt32(metaType.parentTypeID)
                               };

                list.Add(item);
            }

            return new RelationSet<InvMetaType>(list);
        }

        /// <summary>
        /// Effects of various types.
        /// </summary>
        /// <returns><c>RelationSet</c> of Types and Effects.</returns>
        internal static RelationSet<DgmTypeEffect> TypeEffects()
        {
            List<DgmTypeEffect> list = new List<DgmTypeEffect>();

            foreach (dgmTypeEffects typeEffect in Context.dgmTypeEffects)
            {
                DgmTypeEffect item = new DgmTypeEffect
                               {
                                   EffectID = typeEffect.effectID,
                                   ItemID = typeEffect.typeID
                               };

                list.Add(item);
            }

            return new RelationSet<DgmTypeEffect>(list);
        }
    }
}