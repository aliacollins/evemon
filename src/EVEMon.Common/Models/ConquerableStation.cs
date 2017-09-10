using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Helpers;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a conquerable station inside the EVE universe.
    /// </summary>
    public sealed class ConquerableStation : Station
    {
        private static readonly Dictionary<long, ConquerableStation> s_conqStationsByID =
            new Dictionary<long, ConquerableStation>();

        private const string Filename = "ConquerableStationList";

        private static bool s_loaded;
        private static bool s_queryPending;
        private static bool s_isImporting;

        private static DateTime s_nextCheckTime;


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ConquerableStation(SerializableOutpost src)
            : base(src)
        {
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets an enumeration of all the conquerable stations in the universe.
        /// </summary>
        internal static IEnumerable<ConquerableStation> AllConquerableStations
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting
                    ? Enumerable.Empty<ConquerableStation>()
                    : s_conqStationsByID.Values;
            }
        }

        /// <summary>
        /// Gets something like OwnerName - StationName.
        /// </summary>
        public string FullName => $"{CorporationName} - {Name}";

        /// <summary>
        /// Gets something like Region > Constellation > SolarSystem > OwnerName - StationName.
        /// </summary>
        public new string FullLocation => $"{SolarSystem.FullLocation} > {FullName}";

        #endregion


        #region File Updating

        /// <summary>
        /// Downloads the conquerable station list,
        /// while doing a file up to date check.
        /// </summary>
        private static void UpdateList()
        {
            // Quit if we already checked a minute ago or query is pending
            if (s_nextCheckTime > DateTime.UtcNow || s_queryPending)
                return;

            // Set the update time and period
            DateTime updateTime = DateTime.Today
                .AddHours(EveConstants.DowntimeHour).AddMinutes(EveConstants.DowntimeDuration);
            TimeSpan updatePeriod = TimeSpan.FromDays(1);

            // Check to see if file is up to date
            bool fileUpToDate = LocalXmlCache.CheckFileUpToDate(Filename, updateTime, updatePeriod);

            s_nextCheckTime = DateTime.UtcNow.AddMinutes(1);

            // Quit if file is up to date
            if (fileUpToDate)
                return;

            s_queryPending = true;

            EveMonClient.APIProviders.CurrentProvider
                .QueryMethodAsync<SerializableAPIConquerableStationList>(CCPAPIGenericMethods.ConquerableStationList, OnUpdated);
        }

        /// <summary>
        /// Processes the conquerable station list.
        /// </summary>
        private static void OnUpdated(CCPAPIResult<SerializableAPIConquerableStationList> result)
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

                EveMonClient.Notifications.NotifyConquerableStationListError(result);
                return;
            }

            EveMonClient.Notifications.InvalidateAPIError();

            // Import the list
            Import(result.Result.Outposts);

            // Reset query pending flag
            s_queryPending = false;

            // Notify the subscribers
            EveMonClient.OnConquerableStationListUpdated();

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
        /// Deserialize the file and import the list.
        /// </summary>
        private static void Import()
        {
            // Exit if we have already imported or are in the process of importing the list
            if (s_loaded || s_queryPending || s_isImporting)
                return;

            string filename = LocalXmlCache.GetFileInfo(Filename).FullName;

            // Abort if the file hasn't been obtained for any reason
            if (!File.Exists(filename))
                return;

            CCPAPIResult<SerializableAPIConquerableStationList> result =
                Util.DeserializeAPIResultFromFile<SerializableAPIConquerableStationList>(filename, APIProvider.RowsetsTransform);

            // In case the file has an error we prevent the importation
            if (result.HasError)
            {
                FileHelper.DeleteFile(filename);

                s_nextCheckTime = DateTime.UtcNow;

                return;
            }

            // Deserialize the list
            Import(result.Result.Outposts);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        private static void Import(IEnumerable<SerializableOutpost> outposts)
        {
            s_isImporting = true;

            EveMonClient.Trace("begin");

            s_conqStationsByID.Clear();
            foreach (SerializableOutpost outpost in outposts)
            {
                s_conqStationsByID[outpost.StationID] = new ConquerableStation(outpost);
            }

            s_loaded = true;
            s_isImporting = false;

            EveMonClient.Trace("done");
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the conquerable station with the provided ID.
        /// </summary>
        public static ConquerableStation GetStationByID(long id)
        {
            // Ensure list importation
            EnsureImportation();

            if (s_isImporting)
                return null;

            ConquerableStation result;
            s_conqStationsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the conquerable station with the provided name.
        /// </summary>
        public static ConquerableStation GetStationByName(string name)
        {
            // Ensure list importation
            EnsureImportation();

            return s_isImporting
                ? null
                : s_conqStationsByID.Values.FirstOrDefault(station => station.Name == name);
        }

        #endregion
    }
}