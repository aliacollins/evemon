using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public static class EveIDToName
    {
        private static readonly string s_file = LocalXmlCache.GetFile("EveIDToName").FullName;

        private static List<string> s_listOfIDs = new List<string>();
        private static List<string> s_listOfIDsToQuery = new List<string>();
        private static readonly List<string> s_listOfNames = new List<string>();
        private static readonly Dictionary<long, string> s_cacheList = new Dictionary<long, string>();

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
        private static void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateOnOneSecondTick();
        }

        /// <summary>
        /// Gets the owner name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal static string GetIDToName(long id)
        {
            return GetIDToName(id.ToString(CultureConstants.InvariantCulture));
        }

        /// <summary>
        /// Gets the owner name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal static string GetIDToName(string id)
        {
            // If there is no ID to query return an empty string
            if (String.IsNullOrEmpty(id))
                return String.Empty;

            // If it's a zero ID return "(None)"
            if (id == "0")
                return "(None)";

            List<string> list = new List<string> { id };

            List<string> name = GetIDsToNames(list);
            return name[0];
        }

        /// <summary>
        /// Gets the owner name from its ID.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        internal static List<string> GetIDsToNames(List<string> ids)
        {
            s_listOfIDs = ids;
            s_listOfNames.Clear();
            s_listOfIDsToQuery.Clear();

            EnsureCacheFileLoad();
            LookupForName();

            return s_listOfNames;
        }

        /// <summary>
        /// Ensures the cache file is loaded.
        /// </summary>
        private static void EnsureCacheFileLoad()
        {
            if (!File.Exists(s_file) || !s_cacheList.IsEmpty())
                return;

            TryDeserializeCacheFile();
        }

        /// <summary>
        /// Tries to deserialize the EveIDToName file.
        /// </summary>
        private static void TryDeserializeCacheFile()
        {
            // Deserialize the file
            SerializableEveIDToName cache = Util.DeserializeXMLFromFile<SerializableEveIDToName>(s_file);

            // Reset the cache if anything went wrong
            if (cache == null || cache.Entities.Any(x => x.ID == 0) || cache.Entities.Any(x => x.Name.Length == 0))
            {
                EveMonClient.Trace("Deserializing EveIDToName failed. File may be corrupt. Deleting file.");
                File.Delete(s_file);
                return;
            }

            // Add the data to the dictionary
            foreach (SerializableEveIDToNameListItem entity in cache.Entities)
            {
                s_cacheList.Add(entity.ID, entity.Name);
            }
        }

        /// <summary>
        /// Lookups for name.
        /// </summary>
        private static void LookupForName()
        {
            if (!s_cacheList.IsEmpty())
                QueryCacheList();
            else
                s_listOfIDsToQuery = s_listOfIDs;

            if (!s_listOfIDsToQuery.IsEmpty())
                QueryAPICharacterName();

            // In case the list is empty, add an "Unknown" entry
            if (s_listOfNames.Count == 0)
                s_listOfNames.Add("Unknown");
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
        private static void QueryAPICharacterName()
        {
            string ids = string.Join(",", s_listOfIDsToQuery);
            APIResult<SerializableAPICharacterName> result = EveMonClient.APIProviders.CurrentProvider.QueryCharacterName(ids);
            OnQueryAPICharacterNameUpdated(result);
        }

        /// <summary>
        /// Called when the query updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnQueryAPICharacterNameUpdated(APIResult<SerializableAPICharacterName> result)
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

            // We save the data to the disk
            Save();
        }

        /// <summary>
        /// Imports the data from the query result.
        /// </summary>
        /// <param name="entities">The entities.</param>
        private static void Import(IEnumerable<SerializableCharacterNameListItem> entities)
        {
            foreach (SerializableCharacterNameListItem entity in entities)
            {
                // Add the name to the list of names
                s_listOfNames.Add(entity.Name);

                // Add the query result to our cache list if it doesn't exist already
                if (!s_cacheList.ContainsKey(entity.ID))
                    s_cacheList.Add(entity.ID, entity.Name);
            }

            // In case the list is empty, add an "Unknown" entry
            if (s_listOfNames.Count == 0)
                s_listOfNames.Add("Unknown");
        }

        /// <summary>
        /// Every timer tick, checks whether we should save the list every 10s.
        /// </summary>
        private static void UpdateOnOneSecondTick()
        {
            // Is a save requested and is the last save older than 10s ?
            if (s_savePending && DateTime.UtcNow > s_lastSaveTime.AddSeconds(10))
                SaveImmediate();
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
        private static void SaveImmediate()
        {
            SerializableEveIDToName serial = Export();
            XmlSerializer xs = new XmlSerializer(typeof(SerializableEveIDToName));

            // Save in file
            FileHelper.OverwriteOrWarnTheUser(s_file, fs =>
                                                          {
                                                              xs.Serialize(fs, serial);
                                                              fs.Flush();
                                                              return true;
                                                          });
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
            SerializableEveIDToName serial = new SerializableEveIDToName();
            IEnumerable<SerializableEveIDToNameListItem> entitiesList = s_cacheList.Select(
                item => new SerializableEveIDToNameListItem
                            {
                                ID = item.Key,
                                Name = item.Value,
                            });
            serial.Entities.AddRange(entitiesList);

            return serial;
        }
    }
}