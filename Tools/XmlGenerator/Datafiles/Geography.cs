using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.XmlGenerator.Datafiles
{
    public static class Geography
    {
        /// <summary>
        /// Generates the geography datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating geography datafile... ");

            // Regions
            IEnumerable<SerializableRegion> regions = Database.MapRegionsTable.Select(
                srcRegion =>
                    {
                        Util.UpdatePercentDone(Database.GeographyTotalCount);
                        SerializableRegion region = new SerializableRegion
                                                        {
                                                            ID = srcRegion.ID,
                                                            Name = srcRegion.Name
                                                        };

                        // Constellations
                        region.Constellations.AddRange(ExportConstellations(srcRegion).OrderBy(x => x.Name));
                        return region;
                    });

            // Jumps
            IEnumerable<SerializableJump> jumps = Database.MapSolarSystemJumpsTable.Where(
                srcJump => srcJump.A < srcJump.B).Select(
                    srcJump => new SerializableJump { FirstSystemID = srcJump.A, SecondSystemID = srcJump.B });

            // Serialize
            GeoDatafile datafile = new GeoDatafile();
            datafile.Regions.AddRange(regions.OrderBy(x => x.Name));
            datafile.Jumps.AddRange(jumps);

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}",
                                            DateTime.Now.Subtract(startTime)).TrimEnd('0'));

            Util.SerializeXML(datafile, DatafileConstants.GeographyDatafile);
        }

        /// <summary>
        /// Exports the constellations.
        /// </summary>
        /// <param name="srcRegion">The source region.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableConstellation> ExportConstellations(IHasID srcRegion)
        {
            return Database.MapConstellationsTable.Where(
                x => x.RegionID == srcRegion.ID).Select(
                    srcConstellation =>
                        {
                            SerializableConstellation constellation = new SerializableConstellation
                                                                          {
                                                                              ID = srcConstellation.ID,
                                                                              Name = srcConstellation.Name
                                                                          };

                            // Systems
                            constellation.Systems.AddRange(ExportSystems(srcConstellation).OrderBy(x => x.Name));
                            return constellation;
                        });
        }

        /// <summary>
        /// Exports the systems.
        /// </summary>
        /// <param name="srcConstellation">The source constellation.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableSolarSystem> ExportSystems(IHasID srcConstellation)
        {
            return Database.MapSolarSystemsTable.Where(
                x => x.ConstellationID == srcConstellation.ID).Select(
                    srcSystem =>
                        {
                            const double BaseDistance = 1.0E14;
                            SerializableSolarSystem system = new SerializableSolarSystem
                                                                 {
                                                                     ID = srcSystem.ID,
                                                                     Name = srcSystem.Name,
                                                                     X = (int)(srcSystem.X / BaseDistance),
                                                                     Y = (int)(srcSystem.Y / BaseDistance),
                                                                     Z = (int)(srcSystem.Z / BaseDistance),
                                                                     SecurityLevel = srcSystem.SecurityLevel
                                                                 };

                            // Stations
                            system.Stations.AddRange(ExportStations(srcSystem).OrderBy(x => x.Name));
                            return system;
                        });
        }

        /// <summary>
        /// Exports the stations.
        /// </summary>
        /// <param name="srcSystem">The SRC system.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableStation> ExportStations(IHasID srcSystem)
        {
            return Database.StaStationsTable.Where(x => x.SolarSystemID == srcSystem.ID).Select(
                srcStation =>
                    {
                        SerializableStation station = new SerializableStation
                                                          {
                                                              ID = srcStation.ID,
                                                              Name = srcStation.Name,
                                                              CorporationID = srcStation.CorporationID,
                                                              CorporationName = Database.InvNamesTable.First(
                                                                  x => x.ID == srcStation.CorporationID).Name,
                                                              ReprocessingEfficiency = srcStation.ReprocessingEfficiency,
                                                              ReprocessingStationsTake = srcStation.ReprocessingStationsTake
                                                          };

                        // Agents
                        station.Agents.AddRange(ExportAgents(srcStation));
                        return station;
                    });
        }

        /// <summary>
        /// Exports the agents.
        /// </summary>
        /// <param name="srcStation">The station.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableAgent> ExportAgents(IHasID srcStation)
        {
            return Database.AgtAgentsTable.Where(x => x.LocationID == srcStation.ID).Select(
                srcAgent =>
                new
                    {
                        srcAgent,
                        researchAgent = Database.AgtResearchAgentsTable.FirstOrDefault(x => x.ID == srcAgent.ID)
                    }).Select(agent =>
                              new SerializableAgent
                                  {
                                      ID = agent.srcAgent.ID,
                                      Level = agent.srcAgent.Level,
                                      Quality = agent.srcAgent.Quality,
                                      Name = Database.InvNamesTable.First(x => x.ID == agent.srcAgent.ID).Name,
                                      DivisionName = Database.CrpNPCDivisionsTable.First(
                                          x => x.ID == agent.srcAgent.DivisionID).DivisionName,
                                      AgentType = Database.AgtAgentTypesTable.First(
                                          x => x.ID == agent.srcAgent.AgentTypeID).AgentType,
                                      ResearchSkillID = agent.researchAgent != null
                                                            ? agent.researchAgent.ResearchSkillID
                                                            : 0,
                                      LocatorService = agent.srcAgent.IsLocator
                                  });
        }
    }
}
