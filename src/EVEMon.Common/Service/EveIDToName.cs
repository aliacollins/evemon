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
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Service
{
    public static class EveIDToName
    {
        private const string Filename = "EveIDToName";

        private static readonly Dictionary<long, string> s_cacheList = new Dictionary<long, string>();

        // Providers for characters, corps, and alliances
        private static readonly IDToNameProvider s_chars = new CharIDToNameProvider(s_cacheList);
        private static readonly IDToNameProvider s_corps = new CharIDToNameProvider(s_cacheList);
        private static readonly IDToNameProvider s_alliances = new CharIDToNameProvider(s_cacheList);

        private static bool s_savePending;
        private static DateTime s_lastSaveTime;

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static EveIDToName()
        {
            EveMonClient.TimerTick += EveMonClient_TimerTick;

            // For blank corporations and alliances
            s_cacheList.Add(0L, "(None)");
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
        /// Gets the alliance name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The corporation name.</returns>
        internal static string AllianceIDToName(long id)
        {
            EveMonClient.Trace("Alliance ID lookup: {0:D}", id);
            return s_alliances.GetIDToName(id) ?? EveMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets the character name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The character name.</returns>
        internal static string CharIDToName(long id)
        {
            EveMonClient.Trace("Char ID lookup: {0:D}", id);
            return s_chars.GetIDToName(id) ?? EveMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets the character name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The character name.</returns>
        internal static IEnumerable<string> CharIDsToNames(IEnumerable<long> ids)
        {
            EveMonClient.Trace("Char ID lookup: {0:D}", string.Join(",", ids));
            return s_chars.GetAllIDToName(ids);
        }

        /// <summary>
        /// Gets the corporation name from its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The corporation name.</returns>
        internal static string CorpIDToName(long id)
        {
            EveMonClient.Trace("Corp ID lookup: {0:D}", id);
            return s_corps.GetIDToName(id) ?? EveMonConstants.UnknownText;
        }
        
        /// <summary>
        /// Initializes the cache from file.
        /// </summary>
        public static void InitializeFromFile()
        {
            // Quit if the client has been shut down
            if (EveMonClient.Closed)
                return;

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
        /// Imports the data from the query result.
        /// </summary>
        /// <param name="entities">The entities.</param>
        private static void Import(IEnumerable<SerializableCharacterNameListItem> entities)
        {
            foreach (SerializableCharacterNameListItem entity in entities)
            {
                // Add the query result to our cache list if it doesn't exist already
                if (!s_cacheList.ContainsKey(entity.ID))
                    s_cacheList.Add(entity.ID, entity.Name);
            }
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
            IEnumerable<SerializableEveIDToNameListItem> entitiesList;

            lock (s_cacheList)
            {
                entitiesList = s_cacheList.Select(item =>
                    new SerializableEveIDToNameListItem
                    {
                        ID = item.Key,
                        Name = item.Value,
                    });
            }

            var serial = new SerializableEveIDToName();
            serial.Entities.AddRange(entitiesList);

            return serial;
        }

        /// <summary>
        /// Provides character ID to name conversion.
        /// </summary>
        private class CharIDToNameProvider : IDToNameProvider
        {
            public CharIDToNameProvider(IDictionary<long, string> cacheList) : base(cacheList) { }

            protected override void FetchIDToName()
            {
                string ids;
                lock (m_pendingIDs)
                {
                    // Take them all
                    ids = string.Join(",", m_pendingIDs);
                    m_pendingIDs.Clear();
                }
                EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICharacterName>(
                    CCPAPIGenericMethods.CharacterName, ids, OnQueryAPICharacterNameUpdated);
            }

            private void OnQueryAPICharacterNameUpdated(CCPAPIResult<SerializableAPICharacterName> result)
            {
                // Bail if there is an error
                if (result.HasError)
                {
                    EveMonClient.Notifications.NotifyCharacterNameError(result);
                    m_queryPending = false;
                    return;
                }

                EveMonClient.Notifications.InvalidateAPIError();

                lock (m_cacheList)
                {
                    // Add resulting names to the cache; duplicates should not occur, but
                    // guard against them defensively
                    foreach (var namePair in result.Result.Entities)
                    {
                        long id = namePair.ID;

                        if (m_cacheList.ContainsKey(id))
                            m_cacheList[id] = namePair.Name;
                        else
                            m_cacheList.Add(id, namePair.Name);
                    }
                }
                OnIDToNameComplete();
            }
        }

        /// <summary>
        /// A class used to provide ID to name services
        /// </summary>
        private abstract class IDToNameProvider
        {
            /// <summary>
            /// Shared reference to the master ID lookup table.
            /// </summary>
            protected readonly IDictionary<long, string> m_cacheList;

            /// <summary>
            /// List of IDs awaiting query. No duplicates allowed, and in ascending order for
            /// the picky API calls that need it that way.
            /// </summary>
            protected ISet<long> m_pendingIDs;

            /// <summary>
            /// Is a query currently running?
            /// </summary>
            protected volatile bool m_queryPending;

            public IDToNameProvider(IDictionary<long, string> cacheList)
            {
                cacheList.ThrowIfNull(nameof(cacheList));

                m_pendingIDs = new SortedSet<long>();
                m_cacheList = cacheList;
                m_queryPending = false;
            }

            /// <summary>
            /// Evict as many IDs as can be handled at once from m_listOfIDsToQuery and update
            /// m_cacheList with the new mappings. Call OnIDToNameComplete in callback.
            /// </summary>
            protected abstract void FetchIDToName();

            /// <summary>
            /// Convert the ID to a name.
            /// </summary>
            /// <param name="id">The ID (type depends on implementation)</param>
            /// <returns>The name, or null if no item with this name exists (NOT EveMonConstants.UnknownText!)</returns>
            public string GetIDToName(long id)
            {
                string value;
                bool retrieved;

                // Thread safety
                lock (m_cacheList)
                {
                    retrieved = m_cacheList.TryGetValue(id, out value);
                }

                if (!retrieved && QueueID(id))
                    // No query running and a new one needs to be started
                    FetchIDToName();

                return value;
            }

            /// <summary>
            /// Convert the IDs to names.
            /// </summary>
            /// <param name="id">The ID (type depends on implementation)</param>
            /// <returns>The names, with null for each item where no name exists (NOT EveMonConstants.UnknownText!)</returns>
            public IEnumerable<string> GetAllIDToName(IEnumerable<long> ids)
            {
                string value;
                bool retrieved, start = false;
                var ret = new LinkedList<string>();

                // Thread safety
                lock (m_cacheList)
                {
                    foreach (var id in ids)
                    {
                        // Always add the value, even if it is null
                        retrieved = m_cacheList.TryGetValue(id, out value);
                        ret.AddLast(value);

                        if (!retrieved && QueueID(id))
                            start = true;
                    }
                }

                // One query for many names
                if (start)
                    FetchIDToName();

                return ret;
            }

            /// <summary>
            /// Called by subclasses when an ID conversion completes, whether entries remain
            /// or not.
            /// </summary>
            protected void OnIDToNameComplete()
            {
                bool done = false;

                // No more?
                lock (m_pendingIDs)
                {
                    done = m_pendingIDs.Count <= 0;
                    if (done)
                        m_queryPending = false;
                }

                if (done)
                    // Tell everyone we have new names
                    EveMonClient.OnEveIDToNameUpdated();
                else
                {
                    // Go again
                    FetchIDToName();
                }
            }

            /// <summary>
            /// Starts querying for an ID to name conversion with whatever is in the list.
            /// </summary>
            private bool QueueID(long id)
            {
                // Need to add to the requirements list
                bool startQuery = false;

                lock (m_pendingIDs)
                {
                    m_pendingIDs.Add(id);
                    if (!m_queryPending)
                    {
                        m_queryPending = true;
                        startQuery = true;
                    }
                }

                return startQuery;
            }
        }
    }
}
