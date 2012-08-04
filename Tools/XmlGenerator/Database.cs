using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator
{
    public static class Database
    {
        private static DateTime s_startTime;

        internal const int TotalTablesCount = 34;
        internal const int CertificatesTotalCount = 4272;

        internal static int PropertiesTotalCount;
        internal static int ItemsTotalCount;
        internal static int SkillsTotalCount;
        internal static int BlueprintsTotalCount;
        internal static int GeographyTotalCount;
        internal static int ReprocessingTotalCount;


        #region Properties

        /// <summary>
        /// Gets or sets the agt agents table.
        /// </summary>
        /// <value>The agt agents table.</value>
        internal static Bag<AgtAgents> AgtAgentsTable { get; private set; }

        /// <summary>
        /// Gets or sets the agt agent types table.
        /// </summary>
        /// <value>The agt agent types table.</value>
        internal static Bag<AgtAgentTypes> AgtAgentTypesTable { get; private set; }

        /// <summary>
        /// Gets or sets the agt research agents table.
        /// </summary>
        /// <value>The agt research agents table.</value>
        internal static Bag<AgtResearchAgents> AgtResearchAgentsTable { get; private set; }

        /// <summary>
        /// Gets or sets the chr factions table.
        /// </summary>
        /// <value>The chr factions table.</value>
        internal static Bag<ChrFactions> ChrFactionsTable { get; private set; }

        /// <summary>
        /// Gets or sets the crp NPC divisions table.
        /// </summary>
        /// <value>The crp NPC divisions table.</value>
        internal static Bag<CrpNPCDivisions> CrpNPCDivisionsTable { get; private set; }

        /// <summary>
        /// Gets or sets the crt categories table.
        /// </summary>
        /// <value>The crt categories table.</value>
        internal static Bag<CrtCategories> CrtCategoriesTable { get; private set; }

        /// <summary>
        /// Gets or sets the crt classes table.
        /// </summary>
        /// <value>The crt classes table.</value>
        internal static Bag<CrtClasses> CrtClassesTable { get; private set; }

        /// <summary>
        /// Gets or sets the crt certificates table.
        /// </summary>
        /// <value>The crt certificates table.</value>
        internal static Bag<CrtCertificates> CrtCertificatesTable { get; private set; }

        /// <summary>
        /// Gets or sets the crt recommendations table.
        /// </summary>
        /// <value>The crt recommendations table.</value>
        internal static Bag<CrtRecommendations> CrtRecommendationsTable { get; private set; }

        /// <summary>
        /// Gets or sets the crt relationships table.
        /// </summary>
        /// <value>The crt relationships table.</value>
        internal static Bag<CrtRelationships> CrtRelationshipsTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv names table.
        /// </summary>
        /// <value>The inv names table.</value>
        internal static Bag<InvNames> InvNamesTable { get; private set; }

        /// <summary>
        /// Gets or sets the eve icons table.
        /// </summary>
        /// <value>The eve icons table.</value>
        internal static Bag<EveIcons> EveIconsTable { get; private set; }

        /// <summary>
        /// Gets or sets the eve units table.
        /// </summary>
        /// <value>The eve units table.</value>
        internal static Bag<EveUnits> EveUnitsTable { get; private set; }

        /// <summary>
        /// Gets or sets the dgm attribute types table.
        /// </summary>
        /// <value>The dgm attribute types table.</value>
        internal static Bag<DgmAttributeTypes> DgmAttributeTypesTable { get; private set; }

        /// <summary>
        /// Gets or sets the dgm attribute categories table.
        /// </summary>
        /// <value>The dgm attribute categories table.</value>
        internal static Bag<DgmAttributeCategories> DgmAttributeCategoriesTable { get; private set; }

        /// <summary>
        /// Gets or sets the dgm type effects table.
        /// </summary>
        /// <value>The dgm type effects table.</value>
        internal static RelationSet<DgmTypeEffects> DgmTypeEffectsTable { get; private set; }

        /// <summary>
        /// Gets or sets the dgm type attributes table.
        /// </summary>
        /// <value>The type attributes table.</value>
        internal static RelationSet<DgmTypeAttributes> DgmTypeAttributesTable { get; private set; }

        /// <summary>
        /// Gets or sets the map regions table.
        /// </summary>
        /// <value>The map regions table.</value>
        internal static Bag<MapRegions> MapRegionsTable { get; private set; }

        /// <summary>
        /// Gets or sets the map constellations table.
        /// </summary>
        /// <value>The map constellations table.</value>
        internal static Bag<MapConstellations> MapConstellationsTable { get; private set; }

        /// <summary>
        /// Gets or sets the map solar system table.
        /// </summary>
        /// <value>The map solar systems table.</value>
        internal static Bag<MapSolarSystems> MapSolarSystemsTable { get; private set; }

        /// <summary>
        /// Gets or sets the sta stations table.
        /// </summary>
        /// <value>The sta station table.</value>
        internal static Bag<StaStations> StaStationsTable { get; private set; }

        /// <summary>
        /// Gets or sets the map solar system jump table.
        /// </summary>
        /// <value>The map solar system jumps table.</value>
        internal static List<MapSolarSystemsJump> MapSolarSystemJumpsTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv blueprint types table.
        /// </summary>
        /// <value>The inv blueprint types table.</value>
        internal static Bag<InvBlueprintTypes> InvBlueprintTypesTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv categories table.
        /// </summary>
        /// <value>The inv categories table.</value>
        internal static Bag<InvCategories> InvCategoriesTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv control tower resource purposes table.
        /// </summary>
        /// <value>The inv control tower resource purposes table.</value>
        internal static Bag<InvControlTowerResourcePurposes> InvControlTowerResourcePurposesTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv control tower resources table.
        /// </summary>
        /// <value>The inv control tower resource table.</value>
        internal static List<InvControlTowerResources> InvControlTowerResourcesTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv flag table.
        /// </summary>
        /// <value>The inv flags table.</value>
        internal static Bag<InvFlags> InvFlagsTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv group table.
        /// </summary>
        /// <value>The inv groups table.</value>
        internal static Bag<InvGroups> InvGroupsTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv market group table.
        /// </summary>
        /// <value>The inv market groups table.</value>
        internal static Bag<InvMarketGroups> InvMarketGroupsTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv meta type table.
        /// </summary>
        /// <value>The inv meta types table.</value>
        internal static RelationSet<InvMetaTypes> InvMetaTypesTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv type table.
        /// </summary>
        /// <value>The inv types table.</value>
        internal static Bag<InvTypes> InvTypesTable { get; private set; }

        /// <summary>
        /// Gets or sets the inv type materials table.
        /// </summary>
        /// <value>The inv type materials table.</value>
        internal static List<InvTypeMaterials> InvTypeMaterialsTable { get; private set; }

        /// <summary>
        /// Gets the inv type reactions table.
        /// </summary>
        internal static List<InvTypeReactions> InvTypeReactionsTable { get; private set; }

        /// <summary>
        /// Gets or sets the ram type requirements table.
        /// </summary>
        /// <value>The ram type requirements table.</value>
        internal static List<RamTypeRequirements> RamTypeRequirementsTable { get; private set; }

        /// <summary>
        /// Makes the context available to all database methods without
        /// reinstantiating each time.
        /// </summary>
        private static EveStaticDataEntities Context { get; set; }

        #endregion


        #region Database Connection Methods

        /// <summary>
        /// Creates the connection to the SQL Database.
        /// </summary>
        /// <returns></returns>
        private static void CreateConnection()
        {
            const string Text = "Connecting to SQL Database... ";
            Console.Write(Text);

            // Initialize the connection string builder for the underlying provider
            SqlConnectionStringBuilder sqlBuilder =
                new SqlConnectionStringBuilder
                    {
                        // Set the properties for the data source
                        DataSource = @".\SQLEXPRESS",
                        InitialCatalog = "EveStaticData",
                        IntegratedSecurity = true,
                        MultipleActiveResultSets = true,
                        ApplicationName = "EVEMon.XmlGenerator",
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
            EntityConnection connection = GetEntityConnection(entityBuilder.ToString());

            try
            {
                connection.Open();
                Context = new EveStaticDataEntities(connection);

                Console.SetCursorPosition(Console.CursorLeft - Text.Length, Console.CursorTop);
                Console.WriteLine("Connection to SQL Database: Successful");
                Console.WriteLine();
            }
            catch (EntityException)
            {
                Console.SetCursorPosition(Console.CursorLeft - Text.Length, Console.CursorTop);
                Console.WriteLine("Connection to SQL Database: Failed");
                Console.Write("Press any key to exit.");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Gets the entity connection.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private static EntityConnection GetEntityConnection(string entity)
        {
            return new EntityConnection(entity);
        }

        #endregion


        #region Table Creation Methods

        /// <summary>
        /// Creates the tables from database.
        /// </summary>
        internal static void CreateTables()
        {
            CreateConnection();

            // Data dumps are available from CCP
            Console.Write("Loading Data from SQL Server... ");

            s_startTime = DateTime.Now;

            AgtAgentsTable = Agents();
            Util.UpdateProgress();
            AgtAgentTypesTable = AgentTypes();
            Util.UpdateProgress();
            AgtResearchAgentsTable = ResearchAgents();
            Util.UpdateProgress();
            ChrFactionsTable = Factions();
            Util.UpdateProgress();
            CrpNPCDivisionsTable = NPCDivisions();
            Util.UpdateProgress();
            CrtCategoriesTable = CertificateCategories();
            Util.UpdateProgress();
            CrtClassesTable = CertificateClasses();
            Util.UpdateProgress();
            CrtCertificatesTable = Certificates();
            Util.UpdateProgress();
            CrtRecommendationsTable = CertificateRecommendations();
            Util.UpdateProgress();
            CrtRelationshipsTable = CertificateRelationships();
            Util.UpdateProgress();
            DgmAttributeTypesTable = AttributeTypes();
            Util.UpdateProgress();
            DgmAttributeCategoriesTable = AttributeCategories();
            Util.UpdateProgress();
            EveIconsTable = Icons();
            Util.UpdateProgress();
            EveUnitsTable = Units();
            Util.UpdateProgress();
            DgmTypeAttributesTable = TypeAttributes();
            Util.UpdateProgress();
            DgmTypeEffectsTable = TypeEffects();
            Util.UpdateProgress();
            InvBlueprintTypesTable = BlueprintTypes();
            Util.UpdateProgress();
            InvCategoriesTable = Categories();
            Util.UpdateProgress();
            InvControlTowerResourcePurposesTable = ControlTowerResourcePurposes();
            Util.UpdateProgress();
            InvControlTowerResourcesTable = ControlTowerResources();
            Util.UpdateProgress();
            InvFlagsTable = Flags();
            Util.UpdateProgress();
            InvGroupsTable = Groups();
            Util.UpdateProgress();
            InvMarketGroupsTable = MarketGroups();
            Util.UpdateProgress();
            InvMetaTypesTable = MetaTypes();
            Util.UpdateProgress();
            InvNamesTable = Names();
            Util.UpdateProgress();
            InvTypeMaterialsTable = TypeMaterials();
            Util.UpdateProgress();
            InvTypeReactionsTable = TypeReactions();
            Util.UpdateProgress();
            InvTypesTable = Types();
            Util.UpdateProgress();
            MapRegionsTable = Regions();
            Util.UpdateProgress();
            MapConstellationsTable = Constellations();
            Util.UpdateProgress();
            MapSolarSystemsTable = SolarSystems();
            Util.UpdateProgress();
            MapSolarSystemJumpsTable = Jumps();
            Util.UpdateProgress();
            RamTypeRequirementsTable = TypeRequirements();
            Util.UpdateProgress();
            StaStationsTable = Stations();
            Util.UpdateProgress();

            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, " in {0}", DateTime.Now.Subtract(s_startTime)).TrimEnd('0'));
        }

        /// <summary>
        /// Agent Agents.
        /// </summary>
        /// <returns><c>Bag</c> of Agent Agents.</returns>
        private static Bag<AgtAgents> Agents()
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

                if (agent.isLocator.HasValue)
                    item.IsLocator = agent.isLocator.Value;

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Agent Agent Types.
        /// </summary>
        /// <returns><c>Bag</c> of Agent Agent Types.</returns>
        private static Bag<AgtAgentTypes> AgentTypes()
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

            return collection.ToBag();
        }

        /// <summary>
        /// Agent Research Agents.
        /// </summary>
        /// <returns><c>Bag</c> of Agent Research Agents.</returns>
        private static Bag<AgtResearchAgents> ResearchAgents()
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

            return collection.ToBag();
        }

        /// <summary>
        /// Character Factions.
        /// </summary>
        /// <returns><c>Bag</c> of Character Factions.</returns>
        private static Bag<ChrFactions> Factions()
        {
            IndexedCollection<ChrFactions> collection = new IndexedCollection<ChrFactions>();

            foreach (chrFactions faction in Context.chrFactions)
            {
                ChrFactions item = new ChrFactions
                                       {
                                           ID = faction.factionID,
                                           FactionName = faction.factionName,
                                           Description = faction.description,
                                           MilitiaCorporationID = faction.militiaCorporationID,
                                       };

                item.Description = item.Description.Clean();

                if (faction.raceIDs.HasValue)
                    item.RaceID = faction.raceIDs.Value;

                if (faction.solarSystemID.HasValue)
                    item.SolarSystemID = faction.solarSystemID.Value;

                if (faction.corporationID.HasValue)
                    item.CorporationID = faction.corporationID.Value;

                if (faction.sizeFactor.HasValue)
                    item.SizeFactor = faction.sizeFactor.Value;

                if (faction.stationCount.HasValue)
                    item.StationCount = faction.stationCount.Value;

                if (faction.stationSystemCount.HasValue)
                    item.StationSystemCount = faction.stationSystemCount.Value;

                if (faction.iconID.HasValue)
                    item.IconID = faction.iconID.Value;

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Corporation NPC Divisions.
        /// </summary>
        /// <returns><c>Bag</c> of Corporation NPC Divisions.</returns>
        private static Bag<CrpNPCDivisions> NPCDivisions()
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

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Names.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Names.</returns>
        private static Bag<InvNames> Names()
        {
            IndexedCollection<InvNames> collection = new IndexedCollection<InvNames>();

            foreach (InvNames item in Context.invNames.Select(
                name => new InvNames
                            {
                                ID = name.itemID,
                                Name = name.itemName
                            }))
            {
                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// EVE Units.
        /// </summary>
        /// <returns><c>Bag</c> of EVE Units.</returns>
        private static Bag<EveUnits> Units()
        {
            IndexedCollection<EveUnits> collection = new IndexedCollection<EveUnits>();

            foreach (EveUnits item in Context.eveUnits.Select(
                unit => new EveUnits
                            {
                                Description = unit.description,
                                DisplayName = unit.displayName,
                                ID = unit.unitID,
                                Name = unit.unitName
                            }))
            {
                item.Description = item.Description.Clean();
                item.DisplayName = item.DisplayName.Clean();

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// EVE Icons.
        /// </summary>
        /// <returns><c>Bag</c> of EVE icons.</returns>
        private static Bag<EveIcons> Icons()
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

            return collection.ToBag();
        }

        /// <summary>
        /// Dogma Attribute Types.
        /// </summary>
        /// <returns><c>Bag</c> of Dogma Attribute Types.</returns>
        private static Bag<DgmAttributeTypes> AttributeTypes()
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

                item.Description = item.Description.Clean();
                item.DisplayName = item.DisplayName.Clean();
                item.Name = item.Name.Clean();

                if (attribute.defaultValue.HasValue)
                    item.DefaultValue = attribute.defaultValue.Value.ToString(CultureInfo.InvariantCulture);

                if (attribute.published.HasValue)
                    item.Published = attribute.published.Value;

                if (attribute.highIsGood.HasValue)
                    item.HigherIsBetter = attribute.highIsGood.Value;

                collection.Items.Add(item);
            }

            // Set properties total count
            PropertiesTotalCount = collection.Items.Count;

            return collection.ToBag();
        }

        /// <summary>
        /// Dogma Attribute categories.
        /// </summary>
        /// <returns><c>Bag</c> of Dogma Attribute Categories.</returns>
        private static Bag<DgmAttributeCategories> AttributeCategories()
        {
            IndexedCollection<DgmAttributeCategories> collection = new IndexedCollection<DgmAttributeCategories>();

            foreach (DgmAttributeCategories item in Context.dgmAttributeCategories.Select(
                category => new DgmAttributeCategories
                                {
                                    ID = category.categoryID,
                                    Description = category.categoryDescription,
                                    Name = category.categoryName
                                }))
            {
                item.Description = item.Description.Clean();
                item.Name = item.Name.Clean();

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Map Regions.
        /// </summary>
        /// <returns><c>Bag</c> of Map Regions.</returns>
        /// <remarks>Regions in the EVE Universe.</remarks>
        private static Bag<MapRegions> Regions()
        {
            IndexedCollection<MapRegions> collection = new IndexedCollection<MapRegions>();

            foreach (MapRegions item in Context.mapRegions.Select(
                region => new MapRegions
                              {
                                  ID = region.regionID,
                                  Name = region.regionName,
                                  FactionID = region.factionID
                              }))
            {
                collection.Items.Add(item);
            }

            GeographyTotalCount = collection.Items.Count;

            return collection.ToBag();
        }

        /// <summary>
        /// Map Constellations.
        /// </summary>
        /// <returns><c>Bag</c> of Map Constellations.</returns>
        /// <remarks>Constallations in the EVE Universe.</remarks>
        private static Bag<MapConstellations> Constellations()
        {
            IndexedCollection<MapConstellations> collection = new IndexedCollection<MapConstellations>();

            foreach (mapConstellations constellation in Context.mapConstellations)
            {
                MapConstellations item = new MapConstellations
                                             {
                                                 ID = constellation.constellationID,
                                                 Name = constellation.constellationName,
                                             };

                if (constellation.regionID.HasValue)
                    item.RegionID = constellation.regionID.Value;

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Map Solar Systems.
        /// </summary>
        /// <returns><c>Bag</c> of Map Solar Systems.</returns>
        private static Bag<MapSolarSystems> SolarSystems()
        {
            IndexedCollection<MapSolarSystems> collection = new IndexedCollection<MapSolarSystems>();

            foreach (mapSolarSystems solarsystem in Context.mapSolarSystems)
            {
                MapSolarSystems item = new MapSolarSystems
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

            return collection.ToBag();
        }

        /// <summary>
        /// Station Stations.
        /// </summary>
        /// <returns><c>Bag</c> of Station Stations.</returns>
        /// <remarks>Stations in the EVE Universe.</remarks>
        private static Bag<StaStations> Stations()
        {
            IndexedCollection<StaStations> collection = new IndexedCollection<StaStations>();

            foreach (staStations station in Context.staStations)
            {
                StaStations item = new StaStations
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

            return collection.ToBag();
        }

        /// <summary>
        /// Map Solar Systems Jump.
        /// </summary>
        /// <returns><c>List</c> of Map Solar Systems Jump.</returns>
        /// <remarks>Jumps between two solar systems in the EVE Universe.</remarks>
        private static List<MapSolarSystemsJump> Jumps()
        {
            return Context.mapSolarSystemJumps.Select(
                jump => new MapSolarSystemsJump
                            {
                                A = jump.fromSolarSystemID,
                                B = jump.toSolarSystemID
                            }).ToList();
        }

        /// <summary>
        /// Inventory Blueprint Types.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Blueprint Types.</returns>
        private static Bag<InvBlueprintTypes> BlueprintTypes()
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

            BlueprintsTotalCount = collection.Items.Count;

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Market Groups.
        /// </summary>
        /// <returns><c>Bag</c> of Market Groups available on the market.</returns>
        private static Bag<InvMarketGroups> MarketGroups()
        {
            IndexedCollection<InvMarketGroups> collection = new IndexedCollection<InvMarketGroups>();

            foreach (InvMarketGroups item in Context.invMarketGroups.Select(
                marketGroup => new InvMarketGroups
                                   {
                                       ID = marketGroup.marketGroupID,
                                       Description = marketGroup.description,
                                       IconID = marketGroup.iconID,
                                       Name = marketGroup.marketGroupName,
                                       ParentID = marketGroup.parentGroupID
                                   }))
            {
                item.Description = item.Description.Clean();

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Categories.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Categories.</returns>
        private static Bag<InvCategories> Categories()
        {
            IndexedCollection<InvCategories> collection = new IndexedCollection<InvCategories>();

            foreach (invCategories category in Context.invCategories)
            {
                InvCategories item = new InvCategories
                                         {
                                             ID = category.categoryID,
                                             Name = category.categoryName,
                                             Description = category.description,
                                             IconID = category.iconID
                                         };

                if (category.published.HasValue)
                    item.Published = category.published.Value;

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Control Tower Resource Purposes.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Control Tower Resource Purposes.</returns>
        private static Bag<InvControlTowerResourcePurposes> ControlTowerResourcePurposes()
        {
            IndexedCollection<InvControlTowerResourcePurposes> collection =
                new IndexedCollection<InvControlTowerResourcePurposes>();

            foreach (InvControlTowerResourcePurposes item in Context.invControlTowerResourcePurposes.Select(
                resource => new InvControlTowerResourcePurposes
                                {
                                    ID = resource.purpose,
                                    PurposeName = resource.purposeText,
                                }))
            {
                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Control Tower Resources.
        /// </summary>
        /// <returns><c>List</c> of Inventory Control Tower Resources .</returns>
        private static List<InvControlTowerResources> ControlTowerResources()
        {
            List<InvControlTowerResources> list = new List<InvControlTowerResources>();

            foreach (invControlTowerResources resource in Context.invControlTowerResources)
            {
                InvControlTowerResources item = new InvControlTowerResources
                                                    {
                                                        ID = resource.controlTowerTypeID,
                                                        ResourceID = resource.resourceTypeID,
                                                        MinSecurityLevel = resource.minSecurityLevel,
                                                        FactionID = resource.factionID,
                                                    };

                if (resource.purpose.HasValue)
                    item.PurposeID = resource.purpose.Value;

                if (resource.quantity.HasValue)
                    item.Quantity = resource.quantity.Value;

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Inventory Flags.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Flags.</returns>
        private static Bag<InvFlags> Flags()
        {
            IndexedCollection<InvFlags> collection = new IndexedCollection<InvFlags>();

            foreach (invFlags flag in Context.invFlags)
            {
                InvFlags item = new InvFlags
                                    {
                                        ID = flag.flagID,
                                        Name = flag.flagName,
                                    };

                item.Text = flag.flagText.Clean();

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Groups.
        /// </summary>
        /// <returns><c>Bag</c> of Inventory Groups.</returns>
        private static Bag<InvGroups> Groups()
        {
            IndexedCollection<InvGroups> collection = new IndexedCollection<InvGroups>();

            foreach (invGroups group in Context.invGroups)
            {
                InvGroups item = new InvGroups
                                     {
                                         ID = group.groupID,
                                         Name = group.groupName
                                     };

                if (group.published.HasValue)
                    item.Published = group.published.Value;

                if (group.categoryID.HasValue)
                    item.CategoryID = group.categoryID.Value;

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Types.
        /// </summary>
        /// <returns><c>Bag</c> of items from the Inventory.</returns>
        private static Bag<InvTypes> Types()
        {
            IndexedCollection<InvTypes> collection = new IndexedCollection<InvTypes>();

            foreach (invTypes type in Context.invTypes)
            {
                InvTypes item = new InvTypes
                                    {
                                        ID = type.typeID,
                                        Description = type.description,
                                        IconID = type.iconID,
                                        MarketGroupID = type.marketGroupID,
                                        Name = type.typeName,
                                        RaceID = type.raceID
                                    };
                item.Description = item.Description.Clean();

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

            // Set items total count
            ItemsTotalCount = ReprocessingTotalCount = collection.Items.Count;

            // Set skills total count
            SkillsTotalCount = collection.Items.Count(
                item => item.GroupID != DBConstants.FakeSkillsGroupID &&
                        InvGroupsTable[item.GroupID].CategoryID == DBConstants.SkillCategoryID);

            return collection.ToBag();
        }

        /// <summary>
        /// Inventory Requirements.
        /// </summary>
        /// <returns>List of Requirements needed for a particular activity.</returns>
        /// <remarks>Used for an Activity</remarks>
        private static List<RamTypeRequirements> TypeRequirements()
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
        /// Inventory Materials.
        /// </summary>
        /// <returns>List of Materials.</returns>
        private static List<InvTypeMaterials> TypeMaterials()
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
        /// Inventory Type Reactions.
        /// </summary>
        /// <returns>List of reaction info.</returns>
        private static List<InvTypeReactions> TypeReactions()
        {
            List<InvTypeReactions> list = new List<InvTypeReactions>();

            foreach (invTypeReactions reaction in Context.invTypeReactions)
            {
                InvTypeReactions item = new InvTypeReactions
                                            {
                                                ReactionTypeID = reaction.reactionTypeID,
                                                Input = reaction.input,
                                                TypeID = reaction.typeID,
                                            };

                if (reaction.quantity.HasValue)
                    item.Quantity = reaction.quantity.Value;

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Certificate Categories.
        /// </summary>
        /// <returns><c>Bag</c> of Certificate Categories.</returns>
        private static Bag<CrtCategories> CertificateCategories()
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
                item.Description = item.Description.Clean();

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Certificate Classes.
        /// </summary>
        /// <returns><c>Bag</c> of Classes of Certificate.</returns>
        private static Bag<CrtClasses> CertificateClasses()
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
                item.Description = item.Description.Clean();

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Certificate Certificates.
        /// </summary>
        /// <returns><c>Bag</c> of Certificates.</returns>
        private static Bag<CrtCertificates> Certificates()
        {
            IndexedCollection<CrtCertificates> collection = new IndexedCollection<CrtCertificates>();

            foreach (crtCertificates certificate in Context.crtCertificates)
            {
                CrtCertificates item = new CrtCertificates
                                           {
                                               ID = certificate.certificateID,
                                               Description = certificate.description
                                           };
                item.Description = item.Description.Clean();

                if (certificate.categoryID.HasValue)
                    item.CategoryID = certificate.categoryID.Value;

                if (certificate.classID.HasValue)
                    item.ClassID = certificate.classID.Value;

                if (certificate.grade.HasValue)
                    item.Grade = certificate.grade.Value;

                collection.Items.Add(item);
            }

            return collection.ToBag();
        }

        /// <summary>
        /// Certificate Recommendations.
        /// </summary>
        /// <returns><c>Bag</c> of Certificate Recommendations.</returns>
        private static Bag<CrtRecommendations> CertificateRecommendations()
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

            return collection.ToBag();
        }

        /// <summary>
        /// Certificate Relationships.
        /// </summary>
        /// <returns><c>Bag</c> of parent-child relationships between certificates.</returns>
        private static Bag<CrtRelationships> CertificateRelationships()
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

            return collection.ToBag();
        }

        /// <summary>
        /// Dogma Type Attributes.
        /// </summary>
        /// <returns><c>RelationSet</c> of attributes for types.</returns>
        private static RelationSet<DgmTypeAttributes> TypeAttributes()
        {
            IEnumerable<DgmTypeAttributes> list = Context.dgmTypeAttributes.Select(
                typeAttribute => new DgmTypeAttributes
                                     {
                                         AttributeID = typeAttribute.attributeID,
                                         ItemID = typeAttribute.typeID,
                                         ValueFloat = typeAttribute.valueFloat,
                                         ValueInt = typeAttribute.valueInt
                                     });
            return new RelationSet<DgmTypeAttributes>(list);
        }

        /// <summary>
        /// Inventory Meta Types.
        /// </summary>
        /// <returns><c>RelationSet</c> parent-child relationships between types.</returns>
        private static RelationSet<InvMetaTypes> MetaTypes()
        {
            List<InvMetaTypes> list = new List<InvMetaTypes>();

            foreach (invMetaTypes metaType in Context.invMetaTypes)
            {
                InvMetaTypes item = new InvMetaTypes { ItemID = metaType.typeID };
                if (metaType.metaGroupID.HasValue)
                    item.MetaGroupID = Convert.ToInt32(metaType.metaGroupID, CultureInfo.InvariantCulture);

                if (metaType.parentTypeID.HasValue)
                    item.ParentItemID = Convert.ToInt32(metaType.parentTypeID, CultureInfo.InvariantCulture);
                list.Add(item);
            }
            return new RelationSet<InvMetaTypes>(list);
        }

        /// <summary>
        /// Dogma Type Effects.
        /// </summary>
        /// <returns><c>RelationSet</c> of Types and Effects.</returns>
        private static RelationSet<DgmTypeEffects> TypeEffects()
        {
            List<DgmTypeEffects> list = Context.dgmTypeEffects.Select(
                typeEffect => new DgmTypeEffects
                                  {
                                      EffectID = typeEffect.effectID,
                                      ItemID = typeEffect.typeID
                                  }).ToList();

            return new RelationSet<DgmTypeEffects>(list);
        }

        #endregion
    }
}