using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
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
            ID = src.FactionID;
            Name = src.FactionName;
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

        public int ID { get; private set; }

        public string Name { get; private set; }

        public DateTime EnlistedDate { get; private set; }

        public int CurrentRank { get; private set; }

        public int HighestRank { get; private set; }

        public int KillsYesterday { get; private set; }

        public int KillsLastWeek { get; private set; }

        public int KillsTotal { get; private set; }

        public int VictoryPointsYesterday { get; private set; }

        public int VictoryPointsLastWeek { get; private set; }

        public int VictoryPointsTotal { get; private set; }

        #endregion
    }
}
