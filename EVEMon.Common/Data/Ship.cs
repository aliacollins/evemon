using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a readonly ship definition.
    /// </summary>
    public class Ship : Item
    {
        # region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="src">The source.</param>
        internal Ship(MarketGroup group, SerializableItem src)
            : base(group, src)
        {
        }

        #endregion
    }
}