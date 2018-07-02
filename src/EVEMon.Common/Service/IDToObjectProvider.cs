using EVEMon.Common.Extensions;
using System.Collections.Generic;

namespace EVEMon.Common.Service
{
    /// <summary>
    /// A class used to provide ID lookup services.
    /// </summary>
    internal abstract class IDToObjectProvider<T, X> where T : class where X : class
    {
        /// <summary>
        /// Reference to the master cache list.
        /// </summary>
        protected readonly IDictionary<long, T> m_cache;

        /// <summary>
        /// List of IDs awaiting query. No duplicates allowed, and in ascending order for
        /// the picky API calls that need it that way.
        /// </summary>
        protected readonly SortedDictionary<long, X> m_pendingIDs;

        /// <summary>
        /// Is a query currently running?
        /// </summary>
        protected volatile bool m_queryPending;

        // IDs refreshed during this session
        protected readonly ISet<long> m_requested;

        protected IDToObjectProvider(IDictionary<long, T> cache)
        {
            cache.ThrowIfNull(nameof(cache));

            m_cache = cache;
            m_pendingIDs = new SortedDictionary<long, X>();
            m_requested = new HashSet<long>();
            m_queryPending = false;
        }

        /// <summary>
        /// Evict as many IDs as can be handled at once from m_pendingIDs and update
        /// m_cache with the new mappings. Call OnLookupComplete in callback.
        /// </summary>
        protected abstract void FetchIDs();

        /// <summary>
        /// Convert the ID to an object.
        /// </summary>
        /// <param name="id">The ID (type depends on implementation)</param>
        /// <param name="bypass">true to bypass the Prefetch filter, or false (default) to
        /// use it (recommended in most cases)</param>
        /// <returns>The object, or null if no item with this ID exists</returns>
        public T LookupID(long id, bool bypass = false)
        {
            return LookupID(id, bypass, null);
        }

        /// <summary>
        /// Convert the ID to an object. This is the raw version of LookupID which accepts the
        /// value of X provided by a subclass that uses the parameter.
        /// </summary>
        /// <param name="id">The ID (type depends on implementation)</param>
        /// <param name="bypass">true to bypass the Prefetch filter, or false to use it
        /// (recommended in most cases)</param>
        /// <param name="extra">The extra data to associate with this request</param>
        /// <returns>The object, or null if no item with this ID exists</returns>
        protected T LookupID(long id, bool bypass, X extra)
        {
            T value;
            bool needsUpdate = false;

            // Thread safety
            lock (m_cache)
            {
                if (bypass || (value = Prefetch(id)) == default(T))
                {
                    m_cache.TryGetValue(id, out value);
                    needsUpdate = !m_requested.Contains(id) && QueueID(id, extra);
                }
            }

            if (needsUpdate)
                // No query running and a new one needs to be started; note that new queries
                // will be started even for IDs in the cache if they need to be updated
                FetchIDs();
            return value;
        }

        /// <summary>
        /// Convert the IDs to objects.
        /// </summary>
        /// <param name="id">The ID (type depends on implementation)</param>
        /// <returns>The objects, with null for each item where no match exists</returns>
        public IEnumerable<T> LookupAllID(IEnumerable<long> ids)
        {
            T value;
            bool start = false;
            var ret = new LinkedList<T>();

            // Thread safety
            lock (m_cache)
            {
                foreach (var id in ids)
                {
                    if ((value = Prefetch(id)) == default(T))
                    {
                        // Always add the value, even if it is null
                        m_cache.TryGetValue(id, out value);
                        ret.AddLast(value);
                        if (!m_requested.Contains(id) && QueueID(id))
                            start = true;
                    }
                }
            }

            // One query for many IDs
            if (start)
                FetchIDs();

            return ret;
        }

        /// <summary>
        /// Called by subclasses when an ID lookup completes, whether entries remain or not.
        /// </summary>
        protected void OnLookupComplete()
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
                // Tell everyone we have updates
                TriggerEvent();
            else
                // Go again
                FetchIDs();
        }

        /// <summary>
        /// Called before any item is added to the queue; if the item can be resolved locally
        /// with no cache lookup cheaply, this method should do it.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The value that should be returned for this ID, or null if no local match
        /// is found.</returns>
        protected virtual T Prefetch(long id)
        {
            return default(T);
        }

        /// <summary>
        /// Adds a predefined ID and object to this provider. It will add the entry to the
        /// cache and mark it as requested so that this ID will not be sent upstream.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="value">The value that should be returned for this ID.</param>
        public void Prefill(long id, T value) {
            // Overwrite if it exists
            if (m_cache.ContainsKey(id))
                m_cache[id] = value;
            else
                m_cache.Add(id, value);

            m_requested.Add(id);
        }

        /// <summary>
        /// Starts querying for an ID lookup with whatever is in the list.
        /// </summary>
        private bool QueueID(long id, X extra = null)
        {
            // Need to add to the requirements list
            bool startQuery = false;

            lock (m_pendingIDs)
            {
                if (m_pendingIDs.ContainsKey(id))
                {
                    // If there is an entry without a value, add the extra value
                    if (extra != null && m_pendingIDs[id] == null)
                        m_pendingIDs[id] = extra;
                }
                else
                    m_pendingIDs.Add(id, extra);

                if (!m_queryPending)
                {
                    m_queryPending = true;
                    startQuery = true;
                }
            }

            return startQuery;
        }

        /// <summary>
        /// Triggers the proper EVEMon event when updates are completed.
        /// </summary>
        protected abstract void TriggerEvent();
    }
}
