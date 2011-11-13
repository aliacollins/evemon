using System;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a constellation of the EVE universe.
    /// </summary>
    public sealed class Constellation : ReadonlyCollection<SolarSystem>, IComparable<Constellation>
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="src">The source.</param>
        public Constellation(Region region, SerializableConstellation src)
            : base(src.Systems.Count)
        {
            ID = src.ID;
            Name = src.Name;
            Region = region;
            FullLocation = String.Format("{0} > {1}", Region.Name, Name);

            foreach (SerializableSolarSystem srcSystem in src.Systems)
            {
                Items.Add(new SolarSystem(this, srcSystem));
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets this object's id.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets this object's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the region where this constellation is located.
        /// </summary>
        public Region Region { get; private set; }

        /// <summary>
        /// Gets something like Region > Constellation.
        /// </summary>
        public string FullLocation { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Compares this system with another one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Constellation other)
        {
            return Region != other.Region ? Region.CompareTo(other.Region) : Name.CompareTo(other.Name);
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