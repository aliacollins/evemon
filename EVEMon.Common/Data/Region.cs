using System;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a region of the EVE universe.
    /// </summary>
    public sealed class Region : ReadonlyCollection<Constellation>, IComparable<Region>
    { 
        # region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="src"></param>
        internal Region(SerializableRegion src)
            : base(src.Constellations.Length)
        {
            ID = src.ID;
            Name = src.Name;

            foreach (SerializableConstellation srcConstellation in src.Constellations)
            {
                Items.Add(new Constellation(this, srcConstellation));
            }
        }
                #endregion


        # region Public Properties

        /// <summary>
        /// Gets this object's id.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets this object's name.
        /// </summary>
        public string Name { get; private set; }

        #endregion


        # region Public Methods

        /// <summary>
        /// Compare two regions by their names.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Region other)
        {
            return Name.CompareTo(other.Name);
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

        #endregion

    }
}
