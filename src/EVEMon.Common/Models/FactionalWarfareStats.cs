using System;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Models
{
    public sealed class FactionalWarfareStats
    {
        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal FactionalWarfareStats(SerializableAPIFactionalWarfareStats src)
        {
            FactionID = src.FactionID;
            FactionName = src.FactionName;
            EnlistedDate = src.EnlistedDate;
            CurrentRank = src.CurrentRank;
            HighestRank = src.HighestRank;
            KillsYesterday = src.KillsYesterday;
            KillsLastWeek = src.KillsLastWeek;
            KillsTotal = src.KillsTotal;
            VictoryPointsYesterday = src.VictoryPointsYesterday;
            VictoryPointsLastWeek = src.VictoryPointsLastWeek;
            VictoryPointsTotal = src.VictoryPointsTotal;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the faction ID.
        /// </summary>
        public int FactionID { get; }

        /// <summary>
        /// Gets the name of the faction.
        /// </summary>
        public string FactionName { get; }

        /// <summary>
        /// Gets the enlisted date.
        /// </summary>
        public DateTime EnlistedDate { get; }

        /// <summary>
        /// Gets the current rank.
        /// </summary>
        public int CurrentRank { get; }

        /// <summary>
        /// Gets the highest rank.
        /// </summary>
        public int HighestRank { get; }

        /// <summary>
        /// Gets the kills yesterday.
        /// </summary>
        public int KillsYesterday { get; }

        /// <summary>
        /// Gets the kills last week.
        /// </summary>
        public int KillsLastWeek { get; }

        /// <summary>
        /// Gets the kills total.
        /// </summary>
        public int KillsTotal { get; }

        /// <summary>
        /// Gets the victory points yesterday.
        /// </summary>
        public int VictoryPointsYesterday { get; }

        /// <summary>
        /// Gets the victory points last week.
        /// </summary>
        public int VictoryPointsLastWeek { get; }

        /// <summary>
        /// Gets the victory points total.
        /// </summary>
        public int VictoryPointsTotal { get; }

        #endregion
    }
}
