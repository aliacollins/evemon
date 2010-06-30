using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

using EVEMon.Common.Data;
using EVEMon.Common.SettingsObjects;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a conquerable station inside the EVE universe.
    /// </summary>
    public sealed class ConquerableStation : Station
    {
        private readonly static Dictionary<int, ConquerableStation> s_conqStationsByID = new Dictionary<int, ConquerableStation>();
        private readonly static string m_filename = "ConquerableStationList";

        private static bool m_loaded;
        private string m_corp;

        /// <summary>
        /// Constructor.
        /// </summary>
        private ConquerableStation(SerializableOutpost src)
            : base (src)
        {
            m_corp = src.CorporationName;
        }

        /// <summary>
        /// Gets something like OwnerName - StationName.
        /// </summary>
        public string FullName
        {
            get { return m_corp + " - " + Name; }
        }

        /// <summary>
        /// Gets something like Region > Constellation > SolarSystem > CorpName - OutpostName.
        /// </summary>
        public new string FullLocation
        {
            get { return SolarSystem.FullLocation + " > " + FullName; }
        }


        #region File Updating
        /// <summary>
        /// Downloads the conquerable station list,
        /// while doing a file up to date check.
        /// </summary>
        private static void UpdateList()
        {
            var updatePeriods = Settings.Updates.Periods;

            // Exit on market orders "Never" update setting
            if (updatePeriods.Count != 0 && updatePeriods[APIMethods.MarketOrders] == UpdatePeriod.Never)
                return;
            
            // Set the update time and period
            DateTime updateTime = DateTime.Today.AddHours(12);
            TimeSpan updatePeriod = TimeSpan.FromDays(1);

            // Check to see if file is up to date
            bool fileUpToDate = LocalXmlCache.CheckFileUpToDate(m_filename, updateTime, updatePeriod);

            // Not up to date ?
            if (!fileUpToDate)
            {
                // Update the file
                EveClient.Trace("ConquerableStationList.Update - begin");
                var result = EveClient.APIProviders.DefaultProvider.QueryConquerableStationList();
                OnUpdated(result);
            }
        }

        /// <summary>
        /// Processes the conquerable station list.
        /// </summary>
        private static void OnUpdated(APIResult<SerializableConquerableStationList> result)
        {
            // Was there an error ?
            if (result.HasError)
            {
                EveClient.Notifications.NotifyConquerableStationListError(result);
                return;
            }

            // Deserialize the list
            Import(result.Result.Outposts);
            EveClient.Notifications.InvalidateConquerableStationListError();

            // Notify about update
            EveClient.Trace("ConquerableStationList.Update - done");
        }
        #endregion

        
        #region Importation
        /// <summary>
        /// Deserialize the file and import the list.
        /// </summary>
        private static void Import()
        {
            // Exit if we have already imported the list
            if (m_loaded)
                return;

            var file = LocalXmlCache.GetFile(m_filename).FullName;

            // Abort if the file hasn't been obtained for any reason
            if (!File.Exists(file))
                return;

            var result = Util.DeserializeAPIResult<SerializableConquerableStationList>(file, APIProvider.RowsetsTransform);

            // In case the file has an error we prevent the deserialization
            if (result.HasError)
                return;

            // Deserialize the list
            Import(result.Result.Outposts);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        private static void Import(IEnumerable<SerializableOutpost> outposts)
        {
            EveClient.Trace("ConquerableStationList.Import() - begin");
            s_conqStationsByID.Clear();

            try
            {
                foreach (var outpost in outposts)
                {
                    s_conqStationsByID.Add(outpost.StationID, new ConquerableStation(outpost));
                }
            }
            catch (Exception exc)
            {
                ExceptionHandler.LogException(exc, true);
            }
            finally
            {
                m_loaded = true;
                EveClient.Trace("ConquerableStationList.Import() - done");
            }
        }
        #endregion

        /// <summary>
        /// Ensures the list has been imported.
        /// </summary>
        private static void EnsureImportation()
        {
            UpdateList();
            Import();
        }

        /// <summary>
        /// Gets the conquerable station with the provided ID.
        /// </summary>
        internal static ConquerableStation GetStation(int id)
        {
            // Ensure list importation
            EnsureImportation();

            ConquerableStation result = null;
            s_conqStationsByID.TryGetValue(id, out result);
            return result;
        }
    }
}
