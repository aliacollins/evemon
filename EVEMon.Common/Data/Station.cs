using System;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Datafiles;

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
            ID = src.StationID;
            Name = src.StationName;
            CorporationID = src.CorporationID;
            CorporationName = src.CorporationName;
            SolarSystem = StaticGeography.GetSolarSystemByID(src.SolarSystemID);
            FullLocation = String.Format("{0} > {1}", SolarSystem.FullLocation, src.StationName);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="src">The source.</param>
        public Station(SolarSystem owner, SerializableStation src)
            : base(src.Agents == null ? 0 : src.Agents.Length)
        {
            ID = src.ID;
            Name = src.Name;
            CorporationID = src.CorporationID;
            CorporationName = src.CorporationName;
            SolarSystem = owner;
            ReprocessingStationsTake = src.ReprocessingStationsTake;
            ReprocessingEfficiency = src.ReprocessingEfficiency;
            FullLocation = String.Format("{0} > {1}", owner.FullLocation, src.Name);
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
        public long ID { get; private set; }

        /// <summary>
        /// Gets this object's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets this object's corporation id.
        /// </summary>
        public int CorporationID { get; private set; }

        /// <summary>
        /// Gets this object's corporation name.
        /// </summary>
        public string CorporationName { get; private set; }

        /// <summary>
        /// Gets the solar system where this station is located.
        /// </summary>
        public SolarSystem SolarSystem { get; private set; }

        /// <summary>
        /// Gets something like Heimatar > Constellation > Pator > Pator III - Republic Military School.
        /// </summary>
        public string FullLocation { get; private set; }

        /// <summary>
        /// Gets the base reprocessing efficiency of the station.
        /// </summary>
        public float ReprocessingEfficiency { get; private set; }

        /// <summary>
        /// Gets the fraction of reprocessing products taken by the station.
        /// </summary>
        public float ReprocessingStationsTake { get; private set; }
        
        #endregion


        #region Public Methods

        /// <summary>
        /// Compares this station with another one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Station other)
        {
            return (SolarSystem != other.SolarSystem ? SolarSystem.CompareTo(other.SolarSystem) : Name.CompareTo(other.Name));
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
