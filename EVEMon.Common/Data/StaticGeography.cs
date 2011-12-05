using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Stores all the data regarding geography.
    /// </summary>
    public static class StaticGeography
    {
        #region Fields

        private static readonly Dictionary<int, Region> s_regionsByID = new Dictionary<int, Region>();
        private static readonly Dictionary<int, Constellation> s_constellationsByID = new Dictionary<int, Constellation>();
        private static readonly Dictionary<int, SolarSystem> s_solarSystemsByID = new Dictionary<int, SolarSystem>();
        private static readonly Dictionary<int, Station> s_stationsByID = new Dictionary<int, Station>();
        private static readonly Dictionary<int, NPCCorporation> s_corporationsByID = new Dictionary<int, NPCCorporation>();
        private static readonly Dictionary<int, Agent> s_agentsByID = new Dictionary<int, Agent>();
        private static bool s_initialized;

        #endregion


        #region Initialization

        /// <summary>
        /// Ensures the datafile has been intialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (s_initialized)
                return;

            GeoDatafile datafile = Util.DeserializeDatafile<GeoDatafile>(DatafileConstants.GeographyDatafile);

            // Generate the nodes
            foreach (SerializableRegion srcRegion in datafile.Regions)
            {
                Region region = new Region(srcRegion);
                s_regionsByID[srcRegion.ID] = region;

                // Store the children into their dictionaries
                foreach (Constellation constellation in region)
                {
                    s_constellationsByID[constellation.ID] = constellation;

                    foreach (SolarSystem solarSystem in constellation)
                    {
                        s_solarSystemsByID[solarSystem.ID] = solarSystem;

                        foreach (Station station in solarSystem)
                        {
                            s_stationsByID[station.ID] = station;

                            s_corporationsByID[station.CorporationID] = new NPCCorporation
                                                                            {
                                                                                ID = station.CorporationID,
                                                                                Name = station.CorporationName
                                                                            };

                            foreach (Agent agent in station)
                            {
                                s_agentsByID[agent.ID] = agent;
                            }
                        }
                    }
                }
            }

            // Connects the systems
            foreach (SerializableJump srcJump in datafile.Jumps)
            {
                SolarSystem a = s_solarSystemsByID[srcJump.FirstSystemID];
                SolarSystem b = s_solarSystemsByID[srcJump.SecondSystemID];
                a.AddNeighbor(b);
                b.AddNeighbor(a);
            }

            foreach (SolarSystem system in s_solarSystemsByID.Values)
            {
                system.TrimNeighbors();
            }

            // Mark as initialized
            s_initialized = true;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets an enumeration of all the regions in the universe.
        /// </summary>
        public static IEnumerable<Region> AllRegions
        {
            get
            {
                EnsureInitialized();
                return s_regionsByID.Values;
            }
        }

        /// <summary>
        /// Gets an enumeration of all the constellations in the universe.
        /// </summary>
        public static IEnumerable<Constellation> AllConstellations
        {
            get
            {
                EnsureInitialized();
                return s_constellationsByID.Values;
            }
        }

        /// <summary>
        /// Gets an enumeration of all the solar systems in the universe.
        /// </summary>
        public static IEnumerable<SolarSystem> AllSolarSystems
        {
            get
            {
                EnsureInitialized();
                return s_solarSystemsByID.Values;
            }
        }

        /// <summary>
        /// Gets an enumeration of all the stations in the universe.
        /// </summary>
        public static IEnumerable<Station> AllStations
        {
            get
            {
                EnsureInitialized();
                return s_stationsByID.Values;
            }
        }

        /// <summary>
        /// Gets an enumeration of all the NPCCorporations in the universe.
        /// </summary>
        public static IEnumerable<NPCCorporation> AllNPCCorporations
        {
            get
            {
                EnsureInitialized();
                return s_corporationsByID.Values;
            }
        }

        /// <summary>
        /// Gets an enumeration of all the agents in the universe.
        /// </summary>
        public static IEnumerable<Agent> AllAgents
        {
            get
            {
                EnsureInitialized();
                return s_agentsByID.Values;
            }
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the region with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Region GetRegionByID(int id)
        {
            EnsureInitialized();
            Region result;
            s_regionsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the region with the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Region GetRegionByName(string name)
        {
            EnsureInitialized();
            return s_regionsByID.Values.FirstOrDefault(region => region.Name == name);
        }

        /// <summary>
        /// Gets the constellation with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Constellation GetConstellationByID(int id)
        {
            EnsureInitialized();
            Constellation result;
            s_constellationsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the constellation with the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Constellation GetConstellationByName(string name)
        {
            EnsureInitialized();
            return s_constellationsByID.Values.FirstOrDefault(constellation => constellation.Name == name);
        }

        /// <summary>
        /// Gets the system with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static SolarSystem GetSolarSystemByID(int id)
        {
            EnsureInitialized();
            SolarSystem result;
            s_solarSystemsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the system with the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static SolarSystem GetSolarSystemByName(string name)
        {
            EnsureInitialized();
            return s_solarSystemsByID.Values.FirstOrDefault(system => system.Name == name);
        }

        /// <summary>
        /// Gets the station with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Station GetStationByID(int id)
        {
            EnsureInitialized();
            Station result;
            s_stationsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the station with the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Station GetStationByName(string name)
        {
            EnsureInitialized();
            return s_stationsByID.Values.FirstOrDefault(station => station.Name == name);
        }

        /// <summary>
        /// Gets the NPC Coproration with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static NPCCorporation GetCorporationByID(int id)
        {
            EnsureInitialized();
            NPCCorporation result;
            s_corporationsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the  NPC Coproration with the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static NPCCorporation GetCorporationByName(string name)
        {
            EnsureInitialized();
            return s_corporationsByID.Values.FirstOrDefault(corporation => corporation.Name == name);
        }

        /// <summary>
        /// Gets the agent with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Agent GetAgentByID(int id)
        {
            EnsureInitialized();
            Agent result;
            s_agentsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the agent with the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Station GetAgentByName(string name)
        {
            EnsureInitialized();
            return s_stationsByID.Values.FirstOrDefault(station => station.Name == name);
        }

        #endregion


        /// <summary>
        /// The description of the range.
        /// </summary>
        public static string GetRange(int range)
        {
            switch (range)
            {
                case 0:
                    return "stations";
                case 1:
                    return "solar systems";
                case 2:
                    return "5 jumps";
                case 3:
                    return "10 jumps";
                case 4:
                    return "20 jumps";
                case 5:
                    return "regions";
                default:
                    return String.Empty;
            }
        }
    }
}