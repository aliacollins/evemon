using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
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
                    SqlConnectionStringBuilder sqlBuilder =
                        new SqlConnectionStringBuilder
                            {
                                // Set the properties for the data source
                                DataSource = @".\SQLEXPRESS",
                                InitialCatalog = "EveStaticData",
                                IntegratedSecurity = true,
                                MultipleActiveResultSets = true,
                                ApplicationName = "EntityFramework",
                            };

                    // Initialize the EntityConnectionStringBuilder
                    EntityConnectionStringBuilder entityBuilder =
                        new EntityConnectionStringBuilder
                            {
                                // Set the Metadata location
                                Metadata =
                                    @"res://*/EveStaticData.csdl|res://*/EveStaticData.ssdl|res://*/EveStaticData.msl",
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
            IndexedCollection<AgtAgents> collection = new IndexedCollection<AgtAgents>();

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

                collection.Items.Add(item);
            }

            return new Bag<AgtAgents>(collection);
        }

        /// <summary>
        /// EVE Agent Types.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Agent Types.</returns>
        internal static Bag<AgtAgentTypes> AgentTypes()
        {
            IndexedCollection<AgtAgentTypes> collection = new IndexedCollection<AgtAgentTypes>();

            foreach (AgtAgentTypes item in Context.agtAgentTypes.Select(
                agentType => new AgtAgentTypes
                                 {
                                     ID = agentType.agentTypeID,
                                     AgentType = agentType.agentType
                                 }))
            {
                collection.Items.Add(item);
            }

            return new Bag<AgtAgentTypes>(collection);
        }

        /// <summary>
        /// EVE Agent Config.
        /// </summary>
        /// <returns><c>List</c> of EVE Agent Config.</returns>
        internal static List<AgtConfig> AgentConfig()
        {
            return Context.agtConfig.Select(
                agentConfig => new AgtConfig
                                   {
                                       ID = agentConfig.agentID,
                                       Key = agentConfig.k,
                                       Value = agentConfig.v
                                   }).ToList();
        }

        /// <summary>
        /// EVE Research Agents.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Research Agents.</returns>
        internal static Bag<AgtResearchAgents> ResearchAgents()
        {
            IndexedCollection<AgtResearchAgents> collection = new IndexedCollection<AgtResearchAgents>();

            foreach (AgtResearchAgents item in Context.agtResearchAgents.Select(
                researchAgent => new AgtResearchAgents
                                     {
                                         ID = researchAgent.agentID,
                                         ResearchSkillID = researchAgent.typeID
                                     }))
            {
                collection.Items.Add(item);
            }

            return new Bag<AgtResearchAgents>(collection);
        }

        /// <summary>
        /// EVE NPC Divisions.
        /// </summary>
        /// <returns><c>Bag</c> of EVE NPC Divisions.</returns>
        internal static Bag<CrpNPCDivisions> NPCDivisions()
        {
            IndexedCollection<CrpNPCDivisions> collection = new IndexedCollection<CrpNPCDivisions>();

            foreach (CrpNPCDivisions item in Context.crpNPCDivisions.Select(
                npcDivision => new CrpNPCDivisions
                                   {
                                       ID = npcDivision.divisionID,
                                       DivisionName = npcDivision.divisionName
                                   }))
            {
                collection.Items.Add(item);
            }

            return new Bag<CrpNPCDivisions>(collection);
        }

        /// <summary>
        /// EVE Names.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Names.</returns>
        internal static Bag<EveNames> Names()
        {
            IndexedCollection<EveNames> collection = new IndexedCollection<EveNames>();

            foreach (EveNames item in Context.eveNames.Select(
                name => new EveNames
                            {
                                ID = name.itemID,
                                Name = name.itemName
                            }))
            {
                collection.Items.Add(item);
            }

            return new Bag<EveNames>(collection);
        }

        /// <summary>
        /// EVE Units.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Units.</returns>
        internal static Bag<EveUnit> Units()
        {
            IndexedCollection<EveUnit> collection = new IndexedCollection<EveUnit>();

            foreach (EveUnit item in Context.eveUnits.Select(
                unit => new EveUnit
                            {
                                Description = unit.description,
                                DisplayName = unit.displayName,
                                ID = unit.unitID,
                                Name = unit.unitName
                            }))
            {
                item.Description.Clean();
                item.DisplayName.Clean();

                collection.Items.Add(item);
            }

            return new Bag<EveUnit>(collection);
        }

        /// <summary>
        /// EVE Icons.
        /// </summary>
        /// <returns><c>Bag</c> of icons.</returns>
        internal static Bag<EveIcons> Icons()
        {
            IndexedCollection<EveIcons> collection = new IndexedCollection<EveIcons>();

            foreach (EveIcons item in Context.eveIcons.Select(
                icon => new EveIcons
                            {
                                ID = icon.iconID,
                                Icon = icon.iconFile
                            }))
            {
                collection.Items.Add(item);
            }

            return new Bag<EveIcons>(collection);
        }

        /// <summary>
        /// EVE Attributes.
        /// </summary>
        /// <returns><c>Bag</c> of Attributes.</returns>
        internal static Bag<DgmAttributeTypes> Attributes()
        {
            IndexedCollection<DgmAttributeTypes> collection = new IndexedCollection<DgmAttributeTypes>();

            foreach (dgmAttributeTypes attribute in Context.dgmAttributeTypes)
            {
                DgmAttributeTypes item = new DgmAttributeTypes
                                             {
                                                 ID = attribute.attributeID,
                                                 CategoryID = attribute.categoryID,
                                                 Description = attribute.description,
                                                 DisplayName = attribute.displayName,
                                                 IconID = attribute.iconID,
                                                 Name = attribute.attributeName,
                                                 UnitID = attribute.unitID,
                                             };
                item.Description.Clean();
                item.DisplayName.Clean();
                item.Name.Clean();

                if (attribute.defaultValue.HasValue)
                    item.DefaultValue = attribute.defaultValue.Value.ToString(CultureInfo.InvariantCulture);

                if (attribute.published.HasValue)
                    item.Published = attribute.published.Value;

                if (attribute.highIsGood.HasValue)
                    item.HigherIsBetter = attribute.highIsGood.Value;

                collection.Items.Add(item);
            }

            return new Bag<DgmAttributeTypes>(collection);
        }

        /// <summary>
        /// Attribute categories.
        /// </summary>
        /// <returns><c>Bag</c> of Attribute Categories.</returns>
        internal static Bag<DgmAttributeCategory> AttributeCategories()
        {
            IndexedCollection<DgmAttributeCategory> collection = new IndexedCollection<DgmAttributeCategory>();

            foreach (DgmAttributeCategory item in Context.dgmAttributeCategories.Select(
                category => new DgmAttributeCategory
                                {
                                    ID = category.categoryID,
                                    Description = category.categoryDescription,
                                    Name = category.categoryName
                                }))
            {
                item.Description.Clean();
                item.Name.Clean();

                collection.Items.Add(item);
            }

            return new Bag<DgmAttributeCategory>(collection);
        }

        /// <summary>
        /// Regions in the EVE Universe.
        /// </summary>
        /// <returns><c>Bag</c> of all regions in EVE.</returns>
        internal static Bag<MapRegion> Regions()
        {
            IndexedCollection<MapRegion> collection = new IndexedCollection<MapRegion>();

            foreach (MapRegion item in Context.mapRegions.Select(
                region => new MapRegion
                              {
                                  ID = region.regionID,
                                  Name = region.regionName,
                                  FactionID = region.factionID
                              }))
            {
                collection.Items.Add(item);
            }

            return new Bag<MapRegion>(collection);
        }

        /// <summary>
        /// Constallations in the EVE Universe.
        /// </summary>
        /// <returns><c>Bag</c> of Constallations in EVE.</returns>
        internal static Bag<MapConstellation> Constellations()
        {
            IndexedCollection<MapConstellation> collection = new IndexedCollection<MapConstellation>();

            foreach (mapConstellations constellation in Context.mapConstellations)
            {
                MapConstellation item = new MapConstellation
                                            {
                                                ID = constellation.constellationID,
                                                Name = constellation.constellationName,
                                            };

                if (constellation.regionID.HasValue)
                    item.RegionID = constellation.regionID.Value;

                collection.Items.Add(item);
            }

            return new Bag<MapConstellation>(collection);
        }

        /// <summary>
        /// Solar Systems in EVE.
        /// </summary>
        /// <returns><c>Bag</c> of Solar Systems in the EVE.</returns>
        internal static Bag<MapSolarSystem> Solarsystems()
        {
            IndexedCollection<MapSolarSystem> collection = new IndexedCollection<MapSolarSystem>();

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
                    item.SecurityLevel = (float)solarsystem.security.Value;

                if (solarsystem.x.HasValue)
                    item.X = solarsystem.x.Value;

                if (solarsystem.y.HasValue)
                    item.Y = solarsystem.y.Value;

                if (solarsystem.z.HasValue)
                    item.Z = solarsystem.z.Value;

                collection.Items.Add(item);
            }

            return new Bag<MapSolarSystem>(collection);
        }

        /// <summary>
        /// Stations in the EVE Universe
        /// </summary>
        /// <returns><c>Bag</c> of Stations in the EVE Universe.</returns>
        internal static Bag<StaStation> Stations()
        {
            IndexedCollection<StaStation> collection = new IndexedCollection<StaStation>();

            foreach (staStations station in Context.staStations)
            {
                StaStation item = new StaStation
                                      {
                                          ID = station.stationID,
                                          Name = station.stationName,
                                      };

                if (station.reprocessingEfficiency.HasValue)
                    item.ReprocessingEfficiency = (float)station.reprocessingEfficiency.Value;

                if (station.reprocessingStationsTake.HasValue)
                    item.ReprocessingStationsTake = (float)station.reprocessingStationsTake.Value;

                if (station.security.HasValue)
                    item.SecurityLevel = station.security.Value;

                if (station.solarSystemID.HasValue)
                    item.SolarSystemID = station.solarSystemID.Value;

                if (station.corporationID.HasValue)
                    item.CorporationID = station.corporationID.Value;

                collection.Items.Add(item);
            }

            return new Bag<StaStation>(collection);
        }

        /// <summary>
        /// Jumps between two solar systems in the EVE Universe.
        /// </summary>
        /// <returns><c>List</c> of jumps between SolarSystems in EVE.</returns>
        internal static List<MapSolarSystemJump> Jumps()
        {
            return Context.mapSolarSystemJumps.Select(
                jump => new MapSolarSystemJump
                            {
                                A = jump.fromSolarSystemID,
                                B = jump.toSolarSystemID
                            }).ToList();
        }

        /// <summary>
        /// Inventory Blueprint Types.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Blueprint Types.</returns>
        internal static Bag<InvBlueprintTypes> BlueprintTypes()
        {
            IndexedCollection<InvBlueprintTypes> collection = new IndexedCollection<InvBlueprintTypes>();

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

                collection.Items.Add(item);
            }

            return new Bag<InvBlueprintTypes>(collection);
        }

        /// <summary>
        /// Inventory Item Market Groups.
        /// </summary>
        /// <returns><c>Bag</c> of Market Groups available on the market.</returns>
        internal static Bag<InvMarketGroup> MarketGroups()
        {
            IndexedCollection<InvMarketGroup> collection = new IndexedCollection<InvMarketGroup>();

            foreach (InvMarketGroup item in Context.invMarketGroups.Select(
                marketGroup => new InvMarketGroup
                                   {
                                       ID = marketGroup.marketGroupID,
                                       Description = marketGroup.description,
                                       IconID = marketGroup.iconID,
                                       Name = marketGroup.marketGroupName,
                                       ParentID = marketGroup.parentGroupID
                                   }))
            {
                item.Description.Clean();

                collection.Items.Add(item);
            }

            return new Bag<InvMarketGroup>(collection);
        }

        /// <summary>
        /// Inventory Item Groups.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Groups.</returns>
        internal static Bag<InvGroup> Groups()
        {
            IndexedCollection<InvGroup> collection = new IndexedCollection<InvGroup>();

            foreach (invGroups group in Context.invGroups)
            {
                InvGroup item = new InvGroup
                                    {
                                        ID = group.groupID,
                                        Name = group.groupName,
                                    };

                if (group.published.HasValue)
                    item.Published = group.published.Value;

                if (group.categoryID.HasValue)
                    item.CategoryID = group.categoryID.Value;

                collection.Items.Add(item);
            }

            return new Bag<InvGroup>(collection);
        }

        /// <summary>
        /// Inventory Types.
        /// </summary>
        /// <returns><c>Bag</c> of items from the Inventory.</returns>
        internal static Bag<InvType> Types()
        {
            IndexedCollection<InvType> collection = new IndexedCollection<InvType>();

            foreach (invTypes type in Context.invTypes)
            {
                InvType item = new InvType
                                   {
                                       ID = type.typeID,
                                       Description = type.description,
                                       IconID = type.iconID,
                                       MarketGroupID = type.marketGroupID,
                                       Name = type.typeName,
                                       RaceID = type.raceID
                                   };
                item.Description.Clean();

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

                collection.Items.Add(item);
            }

            return new Bag<InvType>(collection);
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
            return Context.invTypeMaterials.Select(
                material => new InvTypeMaterials
                                {
                                    TypeID = material.typeID,
                                    MaterialTypeID = material.materialTypeID,
                                    Quantity = material.quantity
                                }).ToList();
        }

        /// <summary>
        /// Certificate Categories.
        /// </summary>
        /// <returns><c>Bag</c> of Certificate Categories.</returns>
        internal static Bag<CrtCategories> CertificateCategories()
        {
            IndexedCollection<CrtCategories> collection = new IndexedCollection<CrtCategories>();

            foreach (CrtCategories item in Context.crtCategories.Select(
                category => new CrtCategories
                                {
                                    ID = category.categoryID,
                                    CategoryName = category.categoryName,
                                    Description = category.description
                                }))
            {
                item.Description.Clean();

                collection.Items.Add(item);
            }

            return new Bag<CrtCategories>(collection);
        }

        /// <summary>
        /// Certificate Classes.
        /// </summary>
        /// <returns><c>Bag</c> of Classes of Certificate.</returns>
        internal static Bag<CrtClasses> CertificateClasses()
        {
            IndexedCollection<CrtClasses> collection = new IndexedCollection<CrtClasses>();

            foreach (CrtClasses item in Context.crtClasses.Select(
                cClass => new CrtClasses
                              {
                                  ID = cClass.classID,
                                  ClassName = cClass.className,
                                  Description = cClass.description
                              }))
            {
                item.Description.Clean();

                collection.Items.Add(item);
            }

            return new Bag<CrtClasses>(collection);
        }

        /// <summary>
        /// Certificates.
        /// </summary>
        /// <returns><c>Bag</c> of Certificates.</returns>
        internal static Bag<CrtCertificates> Certificates()
        {
            IndexedCollection<CrtCertificates> collection = new IndexedCollection<CrtCertificates>();

            foreach (crtCertificates certificate in Context.crtCertificates)
            {
                CrtCertificates item = new CrtCertificates
                                           {
                                               ID = certificate.certificateID,
                                               Description = certificate.description
                                           };
                item.Description.Clean();

                if (certificate.categoryID.HasValue)
                    item.CategoryID = certificate.categoryID.Value;

                if (certificate.classID.HasValue)
                    item.ClassID = certificate.classID.Value;

                if (certificate.grade.HasValue)
                    item.Grade = certificate.grade.Value;

                collection.Items.Add(item);
            }

            return new Bag<CrtCertificates>(collection);
        }

        /// <summary>
        /// Certificate Recommendations.
        /// </summary>
        /// <returns><c>Bag</c> of Certificate Recommendations.</returns>
        internal static Bag<CrtRecommendations> CertificateRecommendations()
        {
            IndexedCollection<CrtRecommendations> collection = new IndexedCollection<CrtRecommendations>();

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

                collection.Items.Add(item);
            }

            return new Bag<CrtRecommendations>(collection);
        }

        /// <summary>
        /// Certificate Relationships.
        /// </summary>
        /// <returns><c>Bag</c> of parent-child relationships between certificates.</returns>
        internal static Bag<CrtRelationships> CertificateRelationships()
        {
            IndexedCollection<CrtRelationships> collection = new IndexedCollection<CrtRelationships>();

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

                collection.Items.Add(item);
            }

            return new Bag<CrtRelationships>(collection);
        }

        /// <summary>
        /// Type Attributes.
        /// </summary>
        /// <returns><c>RelationSet</c> of attributes for types.</returns>
        internal static RelationSet<DgmTypeAttribute> TypeAttributes()
        {
            List<DgmTypeAttribute> list = Context.dgmTypeAttributes.Select(
                typeAttribute => new DgmTypeAttribute
                                     {
                                         AttributeID = typeAttribute.attributeID,
                                         ItemID = typeAttribute.typeID,
                                         ValueFloat = typeAttribute.valueFloat,
                                         ValueInt = typeAttribute.valueInt
                                     }).ToList();

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
                InvMetaType item = new InvMetaType { ItemID = metaType.typeID };
                if (metaType.metaGroupID.HasValue)
                    item.MetaGroupID = Convert.ToInt32(metaType.metaGroupID, CultureInfo.InvariantCulture);

                if (metaType.parentTypeID.HasValue)
                    item.ParentItemID = Convert.ToInt32(metaType.parentTypeID, CultureInfo.InvariantCulture);
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
            List<DgmTypeEffect> list = Context.dgmTypeEffects.Select(
                typeEffect => new DgmTypeEffect
                                  {
                                      EffectID = typeEffect.effectID,
                                      ItemID = typeEffect.typeID
                                  }).ToList();

            return new RelationSet<DgmTypeEffect>(list);
        }
    }
}