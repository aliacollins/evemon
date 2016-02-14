using System;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a station inside the EVE universe.
    /// </summary>
    public class Station : ReadonlyCollection<Agent>, IComparable<Station>
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="src"></param>
        protected Station(SerializableOutpost src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            ID = src.StationID;
            Name = src.StationName;
            CorporationID = src.CorporationID;
            CorporationName = src.CorporationName;
            SolarSystem = StaticGeography.GetSolarSystemByID(src.SolarSystemID);
            FullLocation = GetFullLocation(SolarSystem, src.StationName);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="src">The source.</param>
        public Station(SolarSystem owner, SerializableStation src)
            : base(src != null && src.Agents != null ? src.Agents.Count : 0)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (src == null)
                throw new ArgumentNullException("src");

            ID = src.ID;
            Name = src.Name;
            CorporationID = src.CorporationID;
            CorporationName = src.CorporationName;
            SolarSystem = owner;
            ReprocessingStationsTake = src.ReprocessingStationsTake;
            ReprocessingEfficiency = src.ReprocessingEfficiency;
            FullLocation = GetFullLocation(owner, src.Name);

            if (src.Agents == null)
                return;

            foreach (SerializableAgent agent in src.Agents)
            {
                Items.Add(new Agent(this, agent));
            }
        }

        #endregion


        #region Public Poperties

        /// <summary>
        /// Gets this object's id.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets this object's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets this object's corporation id.
        /// </summary>
        public int CorporationID { get; }

        /// <summary>
        /// Gets this object's corporation name.
        /// </summary>
        public string CorporationName { get; }

        /// <summary>
        /// Gets the solar system where this station is located.
        /// </summary>
        public SolarSystem SolarSystem { get; }

        /// <summary>
        /// Gets something like Region > Constellation > Solar System > Station.
        /// </summary>
        public string FullLocation { get; }

        /// <summary>
        /// Gets the base reprocessing efficiency of the station.
        /// </summary>
        public float ReprocessingEfficiency { get; }

        /// <summary>
        /// Gets the fraction of reprocessing products taken by the station.
        /// </summary>
        public float ReprocessingStationsTake { get; }

        #endregion


        #region Public Methods

        /// <summary>
        /// Compares this station with another one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Station other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            return SolarSystem != other.SolarSystem
                       ? SolarSystem.CompareTo(other.SolarSystem)
                       : String.Compare(Name, other.Name, StringComparison.CurrentCulture);
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the station's full location.
        /// </summary>
        /// <param name="solarSystem">The solar system.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static string GetFullLocation(SolarSystem solarSystem, string name)
        {
            return solarSystem == null
                       ? String.Empty
                       : String.Format(CultureConstants.DefaultCulture, "{0} > {1}", solarSystem.FullLocation, name);
        }

        /// <summary>
        /// Gets the station by the provided ID.
        /// </summary>
        /// <param name="id">The station's id.</param>
        /// <returns></returns>
        public static Station GetByID(int id)
        {
            // Check if it's a conquerable outpost station, if not look in our data
            return ConquerableStation.GetStationByID(id) ?? StaticGeography.GetStationByID(id);
        }

        /// <summary>
        /// Gets the station by the provided name.
        /// </summary>
        /// <param name="name">The station's name.</param>
        /// <returns>The station or null</returns>
        internal static Station GetByName(string name)
        {
            // Check if it's a conquerable outpost station, if not look in our data
            return ConquerableStation.GetStationByName(name) ?? StaticGeography.GetStationByName(name);
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}