using System;
using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureConstants.DefaultCulture, "Level {0}", Skill.GetRomanFromInt((Level)));
        }

    }
}