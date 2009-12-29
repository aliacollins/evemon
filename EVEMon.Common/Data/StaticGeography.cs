using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Stores all the data regarding geography.
    /// </summary>
    public static class StaticGeography
    {
        private static readonly Dictionary<int, Region> s_regionsByID = new Dictionary<int, Region>();
        private static readonly Dictionary<int, Constellation> s_constellationsByID = new Dictionary<int, Constellation>();
        private static readonly Dictionary<int, SolarSystem> s_solarSystemsByID = new Dictionary<int, SolarSystem>();
        private static readonly Dictionary<int, Station> s_stationsByID = new Dictionary<int, Station>();
        private static bool m_initialized = false;

        /// <summary>
        /// Gets an enumeration of all the systems in the universe.
        /// </summary>
        public static IEnumerable<SolarSystem> AllSystems
        {
            get
            {
                EnsureInitialized();
                foreach (var system in s_solarSystemsByID.Values)
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
                foreach (var station in s_stationsByID.Values)
                {
                    yield return station;
                }
            }
        }

        /// <summary>
        /// Gets the station wtih the provided ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Station GetStation(int id)
        {
            EnsureInitialized();
            Station result = null;
            s_stationsByID.TryGetValue(id, out result);
            if (result == null)
            {
                EveClient.Trace("Could not find station id {0}", id);
                s_stationsByID.TryGetValue(60013747, out result);
                EveClient.Trace("Setting to {0}", result.Name);
            }
            return result;
        }

        /// <summary>
        /// Ensures the datafile has been intialized
        /// </summary>
        public static void EnsureInitialized()
        {
            if (m_initialized) return;
            var datafile = Util.DeserializeDatafile<GeoDatafile>(DatafileConstants.GeographyDatafile);

            // Generate the nodes
            foreach (var srcRegion in datafile.Regions)
            {
                var region = new Region(srcRegion);
                s_regionsByID[srcRegion.ID] = region;

                // Store the children into their dictionaries
                foreach (var constellation in region)
                {
                    s_constellationsByID[constellation.ID] = constellation;

                    foreach (var solarSystem in constellation)
                    {
                        s_solarSystemsByID[solarSystem.ID] = solarSystem;

                        foreach (var station in solarSystem)
                        {
                            s_stationsByID[station.ID] = station;
                        }
                    }
                }
            }

            // Connects the systems
            foreach (var srcJump in datafile.Jumps)
            {
                var a = s_solarSystemsByID[srcJump.FirstSystemID];
                var b = s_solarSystemsByID[srcJump.SecondSystemID];
                a.AddNeightbor(b);
                b.AddNeightbor(a);
            }
            foreach (var system in s_solarSystemsByID.Values)
            {
                system.TrimNeighbors();
            }

            // Mark as initialized
            m_initialized = true;

        }
    }
}
