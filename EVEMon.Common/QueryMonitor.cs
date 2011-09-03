using System;
using EVEMon.Common.Attributes;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
    /// <summary>
    /// This class monitors a querying process. It provides services for autoupdating, update notification, etc.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [EnforceUIThreadAffinity]
    public class QueryMonitor<T> : IQueryMonitorEx, INetworkChangeSubscriber
    {
        public event QueryCallback<T> Updated;

        private readonly string m_methodHeader;

        private bool m_forceUpdate;
        private bool m_retryOnForceUpdateError;
        private bool m_isCanceled;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="method"></param>
        internal QueryMonitor(APIMethods method)
        {
            LastUpdate = DateTime.MinValue;
            IsFullKeyNeeded = method.HasAttribute<FullKeyAttribute>();
            m_methodHeader = (method.HasHeader() ? method.GetHeader() : String.Empty);
            m_forceUpdate = true;
            Method = method;
            Enabled = true;

            NetworkMonitor.Register(this);
        }

        /// <summary>
        /// Gets true if the query is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the API method monitored by this instance.
        /// </summary>
        public APIMethods Method { get; private set; }

        /// <summary>
        /// Gets the last time this instance was updated (UTC).
        /// </summary>
        public DateTime LastUpdate { get; private set; }

        /// <summary>
        /// Gets the status of the query.
        /// </summary>
        public QueryStatus Status { get; private set; }

        /// <summary>
        /// Gets true when the API provider is CCP and a force update is triggered before the next available update.
        /// </summary>
        public bool ForceUpdateWillCauseError
        {
            get
            {
                if (EveMonClient.APIProviders.CurrentProvider != APIProvider.DefaultProvider &&
                    EveMonClient.APIProviders.CurrentProvider != APIProvider.TestProvider)
                    return false;

                DateTime cachedTime = (LastResult == null ? NextUpdate : LastResult.CachedUntil);

                return DateTime.UtcNow < cachedTime;
            }
        }

        /// <summary>
        /// Gets the next time this instance should be updated (UTC), based on both the CCP cache time and the user preferences.
        /// </summary>
        public DateTime NextUpdate
        {
            get
            {
                // If there was an error on last try, we use the cached time
                // (we exclude the corporation roles error for characters corporation issued queries)
                // The 'return' condition have been placed to prevent 'CCP screw up' with the cachedUntil timer
                // as they have done in Incarna 1.0.1 expansion
                if (LastResult != null
                    && LastResult.HasError
                    && LastResult.CCPError != null
                    && !LastResult.CCPError.IsOrdersRelatedCorpRolesError
                    && !LastResult.CCPError.IsJobsRelatedCorpRolesError)
                {
                    return (LastResult.CachedUntil > LastResult.CurrentTime
                                ? LastResult.CachedUntil
                                : LastResult.CachedUntil.AddMinutes(30));
                }

                // No error ? Then we compute the next update according to the settings.
                UpdatePeriod period = Settings.Updates.Periods[Method];
                if (period == UpdatePeriod.Never)
                    return DateTime.MaxValue;

                DateTime nextUpdate = LastUpdate.Add(period.ToDuration());

                // If CCP "cached until" is greater than what we computed, return CCP cached time.
                if (LastResult != null && LastResult.CachedUntil > nextUpdate)
                    return LastResult.CachedUntil;

                return nextUpdate;
            }
        }

        /// <summary>
        /// Gets the last result queried from the API provider.
        /// </summary>
        public APIResult<T> LastResult { get; private set; }

        /// <summary>
        /// Gets true whether the method is curently being requeried.
        /// </summary>
        public bool IsUpdating { get; private set; }

        /// <summary>
        /// Gets true whether a full key is needed.
        /// </summary>
        public bool IsFullKeyNeeded { get; private set; }

        /// <summary>
        /// Manually updates this monitor with the provided data, like if it has just been updated from CCP.
        /// </summary>
        /// <remarks>
        /// This method does not fire the <see cref="Updated"/> event.
        /// </remarks>
        /// <param name="result"></param>
        internal void UpdateWith(APIResult<T> result)
        {
            LastResult = result;
            LastUpdate = DateTime.UtcNow;
        }

        /// <summary>
        /// Forces an update.
        /// </summary>
        internal void ForceUpdate(bool retryOnError)
        {
            m_forceUpdate = true;
            m_retryOnForceUpdateError |= retryOnError;
            UpdateOnOneSecondTick();
        }

        /// <summary>
        /// Updates on every second.
        /// </summary>
        internal void UpdateOnOneSecondTick()
        {
            // Are we already updating ?
            if (IsUpdating)
                return;

            m_isCanceled = false;

            // Is it enabled ?
            if (!Enabled)
            {
                Status = QueryStatus.Disabled;
                return;
            }

            // Do we have a network ?
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                Status = QueryStatus.NoNetwork;
                return;
            }

            // Check for an account
            if (!CheckAccount())
            {
                Status = QueryStatus.NoAccount;
                return;
            }

            // Check for a full key
            if (IsFullKeyNeeded && !HasFullKey())
            {
                Status = QueryStatus.NoFullKey;
                return;
            }

            // Is it an auto-update test ?
            if (!m_forceUpdate)
            {
                // If not due time yet, quits
                DateTime nextUpdate = NextUpdate;
                if (nextUpdate > DateTime.UtcNow)
                {
                    Status = QueryStatus.Pending;
                    return;
                }
            }

            // Starts the update
            IsUpdating = true;
            Status = QueryStatus.Updating;
            QueryAsyncCore(EveMonClient.APIProviders.CurrentProvider, OnQueried);
        }

        /// <summary>
        /// Check all the required account informations are known.
        /// </summary>
        /// <returns>False if an account was required and not found.</returns>
        protected virtual bool CheckAccount()
        {
            return true;
        }

        /// <summary>
        /// Check whether there is a full key.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasFullKey()
        {
            return false;
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        protected virtual void QueryAsyncCore(APIProvider provider, QueryCallback<T> callback)
        {
            provider.QueryMethodAsync(Method, callback);
        }

        /// <summary>
        /// Occurs when a new result has been queried.
        /// </summary>
        /// <param name="result">The downloaded result</param>
        private void OnQueried(APIResult<T> result)
        {
            IsUpdating = false;
            Status = QueryStatus.Pending;

            // Do we need to retry the force update ?
            m_forceUpdate = (m_retryOnForceUpdateError && result.HasError);

            // Was it canceled ?
            if (m_isCanceled)
                return;

            // Updates the stored data
            m_retryOnForceUpdateError = false;
            LastUpdate = DateTime.UtcNow;
            LastResult = result;

            // Notify subscribers
            if (Updated != null)
                Updated(result);
        }

        /// <summary>
        /// Resets the monitor with the given last update time.
        /// </summary>
        /// <param name="lastUpdate">The UTC time of the last update.</param>
        private void Reset(DateTime lastUpdate)
        {
            Cancel();
            LastUpdate = lastUpdate;
            LastResult = null;
        }

        /// <summary>
        /// Cancels the running update.
        /// </summary>
        private void Cancel()
        {
            m_isCanceled = true;
            m_forceUpdate = false;
        }

        /// <summary>
        /// Gets the bound method header.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_methodHeader;
        }

        /// <summary>
        /// Notifies the network availability changed.
        /// </summary>
        /// <param name="isAvailable"></param>
        private void SetNetworkStatus(bool isAvailable)
        {
        }


        #region Interfaces implementations.

        void INetworkChangeSubscriber.SetNetworkStatus(bool isAvailable)
        {
            SetNetworkStatus(isAvailable);
        }

        void IQueryMonitorEx.Reset(DateTime lastUpdate)
        {
            Reset(lastUpdate);
        }

        void IQueryMonitorEx.UpdateOnOneSecondTick()
        {
            UpdateOnOneSecondTick();
        }

        void IQueryMonitorEx.ForceUpdate(bool retryOnError)
        {
            ForceUpdate(retryOnError);
        }

        IAPIResult IQueryMonitor.LastResult
        {
            get { return LastResult; }
        }

        #endregion
    }
}