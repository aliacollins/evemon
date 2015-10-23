using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a mastery ship.
    /// </summary>
    public sealed class MasteryShip : ReadonlyCollection<Mastery>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src">The source.</param>
        internal MasteryShip(SerializableMasteryShip src)
            : base(src == null ? 0 : src.Masteries.Count)
        {
            if (src == null)
                return;

            Ship = StaticItems.GetItemByID(src.ID) as Ship;

            foreach (SerializableMastery mastery in src.Masteries)
            {
                Items.Add(new Mastery(this, mastery));
            }
        }

        #endregion
        

        #region Public Properties

        /// <summary>
        /// Gets the ship.
        /// </summary>
        public Ship Ship { get; private set; }

        #endregion

    }
}