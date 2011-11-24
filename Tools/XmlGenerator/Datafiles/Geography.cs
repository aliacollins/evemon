using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator.Datafiles
{
    public static class Geography
    {
        private const int GeoGenTotal = 97;

        private static DateTime s_startTime;

        /// <summary>
        /// Generates the geo datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            s_startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating geography datafile... ");

            // Regions
            List<SerializableRegion> regions = new List<SerializableRegion>();
            foreach (MapRegion srcRegion in Database.MapRegionsTable)
            {
                Util.UpdatePercentDone(GeoGenTotal);

                SerializableRegion region = new SerializableRegion
                                                {
                                                    ID = srcRegion.ID,
                                                    Name = srcRegion.Name
                                                };
                regions.Add(region);

                // Constellations
                region.Constellations.AddRange(ExportConstellations(srcRegion).OrderBy(x => x.Name));
            }

            // Jumps
            IEnumerable<SerializableJump> jumps = Database.MapSolarSystemJumpTable.Where(srcJump => srcJump.A < srcJump.B).Select(
                srcJump => new SerializableJump { FirstSystemID = srcJump.A, SecondSystemID = srcJump.B });

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}", DateTime.Now.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            GeoDatafile datafile = new GeoDatafile();
            datafile.Regions.AddRange(regions.OrderBy(x => x.Name));
            datafile.Jumps.AddRange(jumps);

            Util.SerializeXML(datafile, DatafileConstants.GeographyDatafile);
        }

        /// <summary>
        /// Exports the constellations.
        /// </summary>
        /// <param name="srcRegion">The SRC region.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableConstellation> ExportConstellations(IHasID srcRegion)
        {
            List<SerializableConstellation> constellations = new List<SerializableConstellation>();
            foreach (MapConstellation srcConstellation in Database.MapConstellationsTable.Where(x => x.RegionID == srcRegion.ID))
            {
                SerializableConstellation constellation = new SerializableConstellation
                                                              {
                                                                  ID = srcConstellation.ID,
                                                                  Name = srcConstellation.Name
                                                              };
                constellations.Add(constellation);

                // Systems
                constellation.Systems.AddRange(ExportSystems(srcConstellation).OrderBy(x => x.Name));
            }
            return constellations;
        }

        /// <summary>
        /// Exports the systems.
        /// </summary>
        /// <param name="srcConstellation">The SRC constellation.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableSolarSystem> ExportSystems(IHasID srcConstellation)
        {
            const double BaseDistance = 1.0E14;
            List<SerializableSolarSystem> systems = new List<SerializableSolarSystem>();
            foreach (MapSolarSystem srcSystem in Database.MapSolarSystemTable.Where(
                x => x.ConstellationID == srcConstellation.ID))
            {
                SerializableSolarSystem system = new SerializableSolarSystem
                                                     {
                                                         ID = srcSystem.ID,
                                                         Name = srcSystem.Name,
                                                         X = (int)(srcSystem.X / BaseDistance),
                                                         Y = (int)(srcSystem.Y / BaseDistance),
                                                         Z = (int)(srcSystem.Z / BaseDistance),
                                                         SecurityLevel = srcSystem.SecurityLevel
                                                     };
                systems.Add(system);

                // Stations
                system.Stations.AddRange(ExportStations(srcSystem).OrderBy(x => x.Name));
            }
            return systems;
        }

        /// <summary>
        /// Exports the stations.
        /// </summary>
        /// <param name="srcSystem">The SRC system.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableStation> ExportStations(IHasID srcSystem)
        {
            List<SerializableStation> stations = new List<SerializableStation>();
            foreach (StaStation srcStation in Database.StaStationTable.Where(x => x.SolarSystemID == srcSystem.ID))
            {
                SerializableStation station = new SerializableStation
                                                  {
                                                      ID = srcStation.ID,
                                                      Name = srcStation.Name,
                                                      CorporationID = srcStation.CorporationID,
                                                      CorporationName = Database.EveNamesTable.First(
                                                          x => x.ID == srcStation.CorporationID).Name,
                                                      ReprocessingEfficiency = srcStation.ReprocessingEfficiency,
                                                      ReprocessingStationsTake = srcStation.ReprocessingStationsTake
                                                  };
                stations.Add(station);

                // Agents
                station.Agents.AddRange(ExportAgents(srcStation));
            }
            return stations;
        }

        /// <summary>
        /// Exports the agents.
        /// </summary>
        /// <param name="srcStation">The SRC station.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableAgent> ExportAgents(IHasID srcStation)
        {
            return Database.AgentsTable.Where(x => x.LocationID == srcStation.ID).Select(
                srcAgent =>
                new
                    {
                        srcAgent,
                        researchAgent = Database.ResearchAgentsTable.FirstOrDefault(x => x.ID == srcAgent.ID)
                    }).Select(
                        config =>
                        new
                            {
                                config,
                                agentConfig = Database.AgentConfigTable.FirstOrDefault(x => x.ID == config.srcAgent.ID)
                            }).Select(
                                agent =>
                                new SerializableAgent
                                    {
                                        ID = agent.config.srcAgent.ID,
                                        Level = agent.config.srcAgent.Level,
                                        Quality = agent.config.srcAgent.Quality,
                                        Name = Database.EveNamesTable.First(x => x.ID == agent.config.srcAgent.ID).Name,
                                        DivisionName = Database.NPCDivisionsTable.First(
                                            x => x.ID == agent.config.srcAgent.DivisionID).DivisionName,
                                        AgentType = Database.AgentTypesTable.First(
                                            x => x.ID == agent.config.srcAgent.AgentTypeID).AgentType,
                                        ResearchSkillID = agent.config.researchAgent != null
                                                              ? agent.config.researchAgent.ResearchSkillID
                                                              : 0,
                                        LocatorService = agent.agentConfig != null &&
                                                         agent.agentConfig.Key.Contains("agent.LocateCharacterService.enabled")
                                    });
        }
    }
}
