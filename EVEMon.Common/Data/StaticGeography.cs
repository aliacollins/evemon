using System.Collections.Generic;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Stores all the data regarding geography.
    /// </summary>
    public static class StaticGeography
    {
        #region Fields

        private static readonly Dictionary<long, Region> s_regionsByID = new Dictionary<long, Region>();
        private static readonly Dictionary<long, Constellation> s_constellationsByID = new Dictionary<long, Constellation>();
        private static readonly Dictionary<long, SolarSystem> s_solarSystemsByID = new Dictionary<long, SolarSystem>();
        private static readonly Dictionary<long, Station> s_stationsByID = new Dictionary<long, Station>();
        private static readonly Dictionary<long, Agent> s_agentsByID = new Dictionary<long, Agent>();
        private static bool m_initialized = false;
        
        #endregion


        #region Initializer

        /// <summary>
        /// Ensures the datafile has been intialized.
        /// </summary>
        public static void EnsureInitialized()
        {
            if (m_initialized)
                return;

            GeoDatafile datafile = Util.DeserializeDatafile<GeoDatafile>(DatafileConstants.GeographyDatafile);

            // Generate the nodes
            foreach (var srcRegion in datafile.Regions)
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

                            foreach (Agent agent in station)
                            {
                                s_agentsByID[agent.ID] = agent;
                            }
                        }
                    }
                }
            }

            // Connects the systems
            foreach (var srcJump in datafile.Jumps)
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
            m_initialized = true;
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
                foreach (Region region in s_regionsByID.Values)
                {
                    yield return region;
                }
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
                foreach (Constellation constellation in s_constellationsByID.Values)
                {
                    yield return constellation;
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of all the systems in the universe.
        /// </summary>
        public static IEnumerable<SolarSystem> AllSystems
        {
            get
            {
                EnsureInitialized();
                foreach (SolarSystem system in s_solarSystemsByID.Values)
                {
                    yield return system;
                }
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
                foreach (Station station in s_stationsByID.Values)
                {
                    yield return station;
                }
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
                foreach (Agent agent in s_agentsByID.Values)
                {
                    yield return agent;
                }
            }
        }
        
        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the region with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Region GetRegionByID(long id)
        {
            EnsureInitialized();
            Region result = null;
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
            foreach (Region region in s_regionsByID.Values)
            {
                if (region.Name == name)
                    return region;
            }
            return null;
        }

        /// <summary>
        /// Gets the constellation with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Constellation GetConstellationByID(long id)
        {
            EnsureInitialized();
            Constellation result = null;
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
            foreach (Constellation constellation in s_constellationsByID.Values)
            {
                if (constellation.Name == name)
                    return constellation;
            }
            return null;
        }

        /// <summary>
        /// Gets the system with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static SolarSystem GetSolarSystemByID(long id)
        {
            EnsureInitialized();
            SolarSystem result = null;
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
            foreach (SolarSystem system in s_solarSystemsByID.Values)
            {
                if (system.Name == name)
                    return system;
            }
            return null;
        }

        /// <summary>
        /// Gets the station with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Station GetStationByID(long id)
        {
            EnsureInitialized();
            Station result = null;
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
            foreach (Station station in s_stationsByID.Values)
            {
                if (station.Name == name)
                    return station;
            }
            return null;
        }

        /// <summary>
        /// Gets the agent with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Agent GetAgentByID(long id)
        {
            EnsureInitialized();
            Agent result = null;
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
            foreach (Station station in s_stationsByID.Values)
            {
                if (station.Name == name)
                    return station;
            }
            return null;
        }

        #endregion

    }
}
