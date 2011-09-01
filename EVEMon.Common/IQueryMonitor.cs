using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public interface IQueryMonitor
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IQueryMonitor"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets the status of the query.
        /// </summary>
        QueryStatus Status { get; }

        /// <summary>
        /// Gets the API method monitored by this instance.
        /// </summary>
        APIMethods Method { get; }

        /// <summary>
        /// Gets the last time this instance was updated (UTC).
        /// </summary>
        DateTime LastUpdate { get; }

        /// <summary>
        /// Gets the next time this instance should be updated (UTC), based on both the CCP cache time and the user preferences.
        /// </summary>
        DateTime NextUpdate { get; }

        /// <summary>
        /// Gets true whether the method is curently being requeried.
        /// </summary>
        bool IsUpdating { get; }

        /// <summary>
        /// Gets the last API result.
        /// </summary>
        IAPIResult LastResult { get; }

        /// <summary>
        /// Gets true whether a full key is needed.
        /// </summary>
        bool IsFullKeyNeeded { get; }

        /// <summary>
        /// Gets true when a force update is within CCP cache time.
        /// </summary>
        bool ForceUpdateWillCauseError { get; }
    }
}