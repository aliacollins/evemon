using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Models.Collections;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents the factional warfare statistics of the EVE universe.
    /// </summary>
    public static class EveFactionalWarfareStats
    {
        #region Fields

        private static readonly EveFactionWarfareStatsCollection s_eveFactionalWarfareStats =
            new EveFactionWarfareStatsCollection();

        private static readonly EveFactionWarsCollection s_eveFactionWars = new EveFactionWarsCollection();
        private const string Filename = "FacWarStats";

        private static bool s_loaded;
        private static bool s_queryPending;

        private static int s_totalsKillsYesterday;
        private static int s_totalsKillsLastWeek;
        private static int s_totalsKillsTotal;
        private static int s_totalsVictoryPointsYesterday;
        private static int s_totalsVictoryPointsLastWeek;
        private static int s_totalsVictoryPointsTotal;
        private static DateTime s_checkTime;

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the totals kills yesterday.
        /// </summary>
        public static int TotalsKillsYesterday
        {
            get
            {
                EnsureImportation();
                return s_totalsKillsYesterday;
            }
        }

        /// <summary>
        /// Gets the totals kills last week.
        /// </summary>
        public static int TotalsKillsLastWeek
        {
            get
            {
                EnsureImportation();
                return s_totalsKillsLastWeek;
            }
        }

        /// <summary>
        /// Gets the totals kills total.
        /// </summary>
        public static int TotalsKillsTotal
        {
            get
            {
                EnsureImportation();
                return s_totalsKillsTotal;
            }
        }

        /// <summary>
        /// Gets the totals victory points yesterday.
        /// </summary>
        public static int TotalsVictoryPointsYesterday
        {
            get
            {
                EnsureImportation();
                return s_totalsVictoryPointsYesterday;
            }
        }

        /// <summary>
        /// Gets the totals victory points last week.
        /// </summary>
        public static int TotalsVictoryPointsLastWeek
        {
            get
            {
                EnsureImportation();
                return s_totalsVictoryPointsLastWeek;
            }
        }

        /// <summary>
        /// Gets the totals victory points total.
        /// </summary>
        public static int TotalsVictoryPointsTotal
        {
            get
            {
                EnsureImportation();
                return s_totalsVictoryPointsTotal;
            }
        }

        /// <summary>
        /// Gets the factional warfare stats.
        /// </summary>
        public static IEnumerable<EveFactionWarfareStats> FactionalWarfareStats
        {
            get
            {
                EnsureImportation();
                return s_eveFactionalWarfareStats;
            }
        }

        #endregion


        #region File Updating

        /// <summary>
        /// Downloads the conquerable station list,
        /// while doing a file up to date check.
        /// </summary>
        private static void UpdateList()
        {
            // Quit if we already checked a minute ago or query is pending
            if (s_checkTime.AddMinutes(1) > DateTime.UtcNow || s_queryPending)
                return;

            // Set the update time and period
            DateTime updateTime = DateTime.Today.AddHours(EveConstants.DowntimeHour).AddMinutes(EveConstants.DowntimeDuration);
            TimeSpan updatePeriod = TimeSpan.FromDays(1);

            // Check to see if file is up to date
            bool fileUpToDate = LocalXmlCache.CheckFileUpToDate(Filename, updateTime, updatePeriod);

            s_checkTime = DateTime.UtcNow;

            // Quit if file is up to date
            if (fileUpToDate)
                return;

            s_queryPending = true;

            EveMonClient.APIProviders.CurrentProvider
                .QueryMethodAsync<SerializableAPIEveFactionalWarfareStats>(CCPAPIGenericMethods.EVEFactionalWarfareStats, OnUpdated);
        }

        /// <summary>
        /// Processes the conquerable station list.
        /// </summary>
        private static void OnUpdated(CCPAPIResult<SerializableAPIEveFactionalWarfareStats> result)
        {
            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
            {
                // Reset query pending flag
                s_queryPending = false;
                return;
            }

            // Was there an error ?
            if (result.HasError)
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Notifications.NotifyEveFactionalWarfareStatsError(result);
                return;
            }

            EveMonClient.Notifications.InvalidateAPIError();

            // Deserialize the result
            Import(result.Result);

            // Reset query pending flag
            s_queryPending = false;

            // Notify the subscribers
            EveMonClient.OnEveFactionalWarfareStatsUpdated();

            // Save the file to our cache
            LocalXmlCache.SaveAsync(Filename, result.XmlDocument).ConfigureAwait(false);
        }

        #endregion


        #region Importation

        /// <summary>
        /// Ensures the list has been imported.
        /// </summary>
        private static void EnsureImportation()
        {
            UpdateList();
            Import();
        }

        /// <summary>
        /// Deserialize the file and import the stats.
        /// </summary>
        private static void Import()
        {
            // Exit if we have already imported the stats
            if (s_loaded)
                return;

            string filename = LocalXmlCache.GetFileInfo(Filename).FullName;

            // Abort if the file hasn't been obtained for any reason
            if (!File.Exists(filename))
                return;

            CCPAPIResult<SerializableAPIEveFactionalWarfareStats> result =
                Util.DeserializeAPIResultFromFile<SerializableAPIEveFactionalWarfareStats>(filename, APIProvider.RowsetsTransform);

            // In case the file has an error we prevent the deserialization
            if (result.HasError)
                return;

            // Deserialize the result
            Import(result.Result);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        private static void Import(SerializableAPIEveFactionalWarfareStats src)
        {
            EveMonClient.Trace("begin");

            s_totalsKillsYesterday = src.Totals.KillsYesterday;
            s_totalsKillsLastWeek = src.Totals.KillsLastWeek;
            s_totalsKillsTotal = src.Totals.KillsTotal;
            s_totalsVictoryPointsYesterday = src.Totals.VictoryPointsYesterday;
            s_totalsVictoryPointsLastWeek = src.Totals.VictoryPointsLastWeek;
            s_totalsVictoryPointsTotal = src.Totals.VictoryPointsTotal;

            s_eveFactionalWarfareStats.Import(src.FactionalWarfareStats);
            s_eveFactionWars.Import(src.FactionWars);

            s_loaded = true;
            EveMonClient.Trace("done");
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the against faction IDs.
        /// </summary>
        /// <param name="factionID">The faction ID.</param>
        /// <returns></returns>
        public static IEnumerable<int> GetAgainstFactionIDs(long factionID)
        {
            EnsureImportation();

            List<int> againstIDs = new List<int>();
            foreach (EveFactionWar factionWar in s_eveFactionWars.Where(faction => faction.FactionID == factionID))
            {
                if (factionWar.AgainstID == factionWar.PrimeAgainstID)
                {
                    againstIDs.Insert(0, factionWar.AgainstID);
                    continue;
                }
                againstIDs.Add(factionWar.AgainstID);
            }
            return againstIDs;
        }

        /// <summary>
        /// Gets the factional warfare stats for faction.
        /// </summary>
        /// <param name="factionID">The faction ID.</param>
        /// <returns></returns>
        public static EveFactionWarfareStats GetFactionalWarfareStatsForFaction(int factionID)
        {
            EnsureImportation();
            return s_eveFactionalWarfareStats.FirstOrDefault(factionStats => factionStats.FactionID == factionID);
        }

        #endregion

    }
}
