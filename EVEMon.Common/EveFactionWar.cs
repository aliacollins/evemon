using System;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public class EveFactionWar
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EveFactionWar"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        internal EveFactionWar(SerializableEveFactionWarsListItem src)
        {
            FactionID = src.FactionID;
            FactionName = src.FactionName;
            AgainstID = src.AgainstID;
            AgainstName = src.AgainstName;

            SetPrimeEnemy();
        }

        #endregion


        #region Public Properties

        public int PrimeAgainstID { get; set; }

        /// <summary>
        /// Gets or sets the faction ID.
        /// </summary>
        public int FactionID { get; set; }

        /// <summary>
        /// Gets or sets the name of the faction.
        /// </summary>
        public string FactionName { get; set; }

        /// <summary>
        /// Gets or sets the against ID.
        /// </summary>
        public int AgainstID { get; set; }

        /// <summary>
        /// Gets or sets the name of the against.
        /// </summary>
        public string AgainstName { get; set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Sets the prime enemy of a faction.
        /// </summary>
        private void SetPrimeEnemy()
        {
            switch (FactionID)
            {
                case DBConstants.AmarrFactionID:
                    PrimeAgainstID = DBConstants.MinmatarFactionID;
                    break;
                case DBConstants.CaldariFactionID:
                    PrimeAgainstID = DBConstants.GallenteFactionID;
                    break;
                case DBConstants.GallenteFactionID:
                    PrimeAgainstID = DBConstants.CaldariFactionID;
                    break;
                case DBConstants.MinmatarFactionID:
                    PrimeAgainstID = DBConstants.AmarrFactionID;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}