using System;
using System.Collections.Generic;
using System.Drawing;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Helpers;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a solar system of the EVE universe.
    /// </summary>
    public sealed class SolarSystem : ReadonlyCollection<Station>, IComparable<SolarSystem>
    {
        // Do not set this as readonly !
        private FastList<SolarSystem> m_jumps;

        private readonly int m_x;
        private readonly int m_y;
        private readonly int m_z;


        # region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolarSystem(Constellation owner, SerializableSolarSystem src)
            : base(src != null && src.Stations != null ? src.Stations.Count : 0)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (src == null)
                throw new ArgumentNullException("src");

            ID = src.ID;
            Constellation = owner;
            Name = src.Name;
            SecurityLevel = src.SecurityLevel;
            FullLocation = String.Format(CultureConstants.DefaultCulture, "{0} > {1}", owner.FullLocation, src.Name);
            m_jumps = new FastList<SolarSystem>(0);

            m_x = src.X;
            m_y = src.Y;
            m_z = src.Z;

            if (src.Stations == null)
                return;

            foreach (SerializableStation srcStation in src.Stations)
            {
                Items.Add(new Station(this, srcStation));
            }
        }

        #endregion


        # region Public Properties

        /// <summary>
        /// Gets this object's id.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets this object's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the real security level, between -1.0 and +1.0
        /// </summary>
        public float SecurityLevel { get; private set; }

        /// <summary>
        /// Gets the constellation this solar system is located.
        /// </summary>
        public Constellation Constellation { get; private set; }

        /// <summary>
        /// Gets something like Region > Constellation > Solar System.
        /// </summary>
        public string FullLocation { get; private set; }

        /// <summary>
        /// Gets or sets the color of the security level.
        /// </summary>
        /// <value>The color of the security level.</value>
        public Color SecurityLevelColor
        {
            get
            {
                if (IsNullSec)
                    return Color.Red;

                return IsLowSec ? Color.DarkOrange : Color.Green;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this solar system is in high sec.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this solar system is in high sec; otherwise, <c>false</c>.
        /// </value>
        public bool IsHighSec
        {
            get { return Math.Round(SecurityLevel, 1) >= 0.5; }
        }

        /// <summary>
        /// Gets a value indicating whether this solar system is in low sec.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this solar system is in low sec; otherwise, <c>false</c>.
        /// </value>
        public bool IsLowSec
        {
            get
            {
                double secLevel = Math.Round(SecurityLevel, 1);
                return secLevel > 0 && secLevel < 0.5;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this solar system is in null sec.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this solar system is in null sec; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullSec
        {
            get { return Math.Round(SecurityLevel, 1) <= 0; }
        }

        #endregion


        # region Public Methods

        /// <summary>
        /// Gets the square distance with the given system.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int GetSquareDistanceWith(SolarSystem other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            int dx = m_x - other.m_x;
            int dy = m_y - other.m_y;
            int dz = m_z - other.m_z;

            return dx * dx + dy * dy + dz * dz;
        }

        /// <summary>
        /// Gets the solar systems within the given range.
        /// </summary>
        /// <param name="maxInclusiveNumberOfJumps">The maximum, inclusive, number of jumps from this system.</param>
        /// <returns></returns>
        public IEnumerable<SolarSystemRange> GetSystemsWithinRange(int maxInclusiveNumberOfJumps)
        {
            return SolarSystemRange.GetSystemRangesFrom(this, maxInclusiveNumberOfJumps);
        }

        /// <summary>
        /// Find the guessed shortest path using a A* (heuristic) algorithm.
        /// </summary>
        /// <param name="target">The target system.</param>
        /// <param name="criteria">The path searching criteria.</param>
        /// <param name="minSecurityLevel">The minimum, inclusive, real security level. Systems have levels between -1 and +1.</param>
        /// <param name="maxSecurityLevel">The maximum, inclusive, real security level. Systems have levels between -1 and +1.</param>
        /// <returns>
        /// The list of systems, beginning with this one and ending with the provided target.
        /// </returns>
        public IEnumerable<SolarSystem> GetFastestPathTo(SolarSystem target, PathSearchCriteria criteria,
                                                         float minSecurityLevel = -1.0f, float maxSecurityLevel = 1.0f)
        {
            return PathFinder.FindBestPath(this, target, criteria, minSecurityLevel, maxSecurityLevel);
        }

        /// <summary>
        /// Gets the systems which have a jumpgate connection with his one.
        /// </summary>
        public IEnumerable<SolarSystem> Neighbors
        {
            get { return m_jumps; }
        }

        #endregion


        # region Internal Methods

        /// <summary>
        /// Adds a neighbor with a jumpgate connection to this system.
        /// </summary>
        /// <param name="system"></param>
        internal void AddNeighbor(SolarSystem system)
        {
            m_jumps.Add(system);
        }

        /// <summary>
        /// Trims the neighbors list.
        /// </summary>
        internal void TrimNeighbors()
        {
            if (m_jumps.Capacity > m_jumps.Count)
                m_jumps.Trim();
        }

        #endregion


        # region Overridden Methods

        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets the ID of the object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ID;
        }

        #endregion


        # region Comparer Method

        /// <summary>
        /// Compares this system with another one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SolarSystem other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            return Constellation != other.Constellation
                       ? Constellation.CompareTo(other.Constellation)
                       : String.Compare(Name, other.Name, StringComparison.CurrentCulture);
        }

        #endregion

    }
}