using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a mastery level.
    /// </summary>
    public sealed class Mastery : ReadonlyCollection<MasteryCertificate>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="masteryShip">The mastery ship.</param>
        /// <param name="src">The source.</param>
        internal Mastery(MasteryShip masteryShip, SerializableMastery src)
            : base(src == null ? 0 : src.Certificates.Count)
        {
            if (src == null)
                return;

            MasteryShip = masteryShip;

            Level = src.Grade;

            foreach (SerializableMasteryCertificate certificate in src.Certificates)
            {
                Items.Add(new MasteryCertificate(this, certificate));
            }
        }

        #endregion
        

        #region Public Properties

        /// <summary>
        /// Gets the mastery ship.
        /// </summary>
        public MasteryShip MasteryShip { get; private set; }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level { get; private set; }

        #endregion

    }
}