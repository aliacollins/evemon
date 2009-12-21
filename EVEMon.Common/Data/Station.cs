using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a station inside the EVE universe.
    /// </summary>
    public sealed class Station : IComparable<Station>
    {
        private readonly int m_id;
        private readonly string m_name;
        private readonly SolarSystem m_owner;
        private readonly float m_reprocessingTake;
        private readonly float m_reprocessingEfficiency;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="src"></param>
        public Station(SolarSystem owner, SerializableStation src)
        {
            m_id = src.ID;
            m_name = src.Name;
            m_owner = owner;
            m_reprocessingTake = src.ReprocessingStationsTake;
            m_reprocessingEfficiency = src.ReprocessingEfficiency;
        }

        /// <summary>
        /// Gets this object's id.
        /// </summary>
        public int ID
        {
            get { return m_id; }
        }

        /// <summary>
        /// Gets this object's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the solar system where this station is located.
        /// </summary>
        public SolarSystem SolarSystem
        {
            get { return m_owner; }
        }

        /// <summary>
        /// Gets something like Heimatar > Constellation > Pator > Pator III - Republic Military School.
        /// </summary>
        public string FullLocation
        {
            get { return m_owner.FullLocation + " > " + m_name; }
        }

        /// <summary>
        /// Gets the base reprocessing efficiency of the station.
        /// </summary>
        public float ReprocessingEfficiency
        {
            get { return m_reprocessingEfficiency; }
        }

        /// <summary>
        /// Gets the fraction of reprocessing products taken by the station.
        /// </summary>
        public float ReprocessingStationsTake
        {
            get { return m_reprocessingTake; }
        }

        /// <summary>
        /// Compares this station with another one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Station other)
        {
            if (this.SolarSystem != other.SolarSystem) return this.SolarSystem.CompareTo(other.SolarSystem);
            return m_name.CompareTo(other.m_name);
        }

        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }
    }
}
