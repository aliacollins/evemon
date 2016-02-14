using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Helpers;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Service
{
    public static class EveIDToName
    {
        private const string Filename = "EveIDToName";

        private static readonly Dictionary<long, string> s_cacheList = new Dictionary<long, string>();
        private static readonly List<string> s_listOfNames = new List<string>();
        private static readonly List<string> s_queriedIDs = new List<string>();

        private static List<string> s_listOfIDs = new List<string>();
        private static List<string> s_listOfIDsToQuery = new List<string>();

        private static bool s_savePending;
        private static DateTime s_lastSaveTime;

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static EveIDToName()
        {
            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static async void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            await UpdateOnOneSecondTickAsync();
        }

        /// <summary>
        /// Gets the owner name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal static string GetIDToName(long id) => GetIDToName(id.ToString(CultureConstants.InvariantCulture));

        /// <summary>
        /// Gets the owner name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static string GetIDToName(string id)
        {
            // If there is no ID to query return an empty string
            if (String.IsNullOrEmpty(id))
                return String.Empty;

            // If it's a zero ID return "(None)"
            if (id == "0")
                return "(None)";

            List<string> list = new List<string> { id };

            return GetIDsToNames(list).First();
        }

        /// <summary>
        /// Gets the owner name from its ID.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        internal static IEnumerable<string> GetIDsToNames(IEnumerable<string> ids)
        {
            s_listOfIDs = ids.ToList();
            s_listOfNames.Clear();
            s_listOfIDsToQuery.Clear();

            LookupForName();

            return s_listOfNames;
        }

        /// <summary>
        /// Initializes the cache from file.
        /// </summary>
        public static void InitializeFromFile()
        {
            string file = LocalXmlCache.GetFileInfo(Filename).FullName;

            if (!File.Exists(file) || s_cacheList.Any())
                return;

            // Deserialize the file
            SerializableEveIDToName cache = Util.DeserializeXmlFromFile<SerializableEveIDToName>(file);

            // Reset the cache if anything went wrong
            if (cache == null || cache.Entities.Any(x => x.ID == 0) || cache.Entities.Any(x => x.Name.Length == 0))
            {
                EveMonClient.Trace("Deserializing failed. File may be corrupt. Deleting file.");
                FileHelper.DeleteFile(file);
                return;
            }

            // Add the data to the cache
            Import(cache.Entities.Select(entity => new SerializableCharacterNameListItem { ID = entity.ID, Name = entity.Name }));
        }

        /// <summary>
        /// Lookups for name.
        /// </summary>
        private static void LookupForName()
        {
            if (s_cacheList.Any())
                QueryCacheList();
            else
                s_listOfIDsToQuery = s_listOfIDs;

            // Avoid querying an already querying id
            IList<string> idsToQuery = s_listOfIDsToQuery.Where(id => !s_queriedIDs.Contains(id)).ToList();
            if (idsToQuery.Any())
                QueryAPICharacterName(idsToQuery);

            // Add an "Unknown" entry for every id we query
            s_listOfIDsToQuery.ForEach(id => s_listOfNames.Add(EVEMonConstants.UnknownText));
        }

        /// <summary>
        /// Queries the cache list.
        /// </summary>
        private static void QueryCacheList()
        {
            foreach (string id in s_listOfIDs)
            {
                string name = s_cacheList.FirstOrDefault(x => x.Key.ToString(CultureConstants.InvariantCulture) == id).Value;

                if (name == null)
                    s_listOfIDsToQuery.Add(id);
                else
                    s_listOfNames.Add(name);
            }
        }

        /// <summary>
        /// Queries the API Character Name.
        /// </summary>
        /// <param name="idsToQuery">The ids to query.</param>
        private static void QueryAPICharacterName(IList<string> idsToQuery)
        {
            string ids = string.Join(",", idsToQuery);

            if (String.IsNullOrWhiteSpace(ids))
                return;

            // Add the ids to the queried list
            s_queriedIDs.AddRange(idsToQuery);

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICharacterName>(
                CCPAPIGenericMethods.CharacterName, ids, OnQueryAPICharacterNameUpdated);
        }

        /// <summary>
        /// Called when the query updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnQueryAPICharacterNameUpdated(CCPAPIResult<SerializableAPICharacterName> result)
        {
            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return;

            if (result.HasError)
            {
                EveMonClient.Notifications.NotifyCharacterNameError(result);
                return;
            }

            EveMonClient.Notifications.InvalidateAPIError();

            // Deserialize the result
            Import(result.Result.Entities);

            // Notify the subscribers
            EveMonClient.OnEveIDToNameUpdated();

            // We save the data to the disk
            Save();
        }

        /// <summary>
        /// Imports the data from the query result.
        /// </summary>
        /// <param name="entities">The entities.</param>
        private static void Import(IEnumerable<SerializableCharacterNameListItem> entities)
        {
            EveMonClient.Trace("begin");

            foreach (SerializableCharacterNameListItem entity in entities)
            {
                // Remove the queried id from the queried list
                if (s_queriedIDs.Contains(entity.ID.ToString(CultureConstants.InvariantCulture)))
                    s_queriedIDs.Remove(entity.ID.ToString(CultureConstants.InvariantCulture));

                // Add the query result to our cache list if it doesn't exist already
                if (!s_cacheList.ContainsKey(entity.ID))
                    s_cacheList.Add(entity.ID, entity.Name);
            }

            EveMonClient.Trace("done");
        }

        /// <summary>
        /// Every timer tick, checks whether we should save the list every 10s.
        /// </summary>
        private static Task UpdateOnOneSecondTickAsync()
        {
            // Is a save requested and is the last save older than 10s ?
            if (s_savePending && DateTime.UtcNow > s_lastSaveTime.AddSeconds(10))
                return SaveImmediateAsync();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Saves the list to disk.
        /// </summary>
        /// <remarks>
        /// Saves will be cached for 10 seconds to avoid thrashing the disk when this method is called very rapidly.
        /// If a save is currently pending, no action is needed. 
        /// </remarks>
        private static void Save()
        {
            s_savePending = true;
        }

        /// <summary>
        /// Saves this cache list to a file.
        /// </summary>
        public static async Task SaveImmediateAsync()
        {
            // Save in file
            await LocalXmlCache.SaveAsync(Filename, Util.SerializeToXmlDocument(Export()));

            // Reset savePending flag
            s_lastSaveTime = DateTime.UtcNow;
            s_savePending = false;
        }

        /// <summary>
        /// Exports the cache list to a serializable object.
        /// </summary>
        /// <returns></returns>
        private static SerializableEveIDToName Export()
        {
            IEnumerable<SerializableEveIDToNameListItem> entitiesList = s_cacheList
                .Select(
                    item =>
                        new SerializableEveIDToNameListItem
                        {
                            ID = item.Key,
                            Name = item.Value,
                        });

            SerializableEveIDToName serial = new SerializableEveIDToName();
            serial.Entities.AddRange(entitiesList);

            return serial;
        }
    }
}