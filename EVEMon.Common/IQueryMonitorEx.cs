using System;

namespace EVEMon.Common
{
    /// <summary>
    /// Adds the internal methods for a query monitor.
    /// </summary>
    internal interface IQueryMonitorEx : IQueryMonitor
    {
        /// <summary>
        /// Resets the monitor with the given last update time.
        /// </summary>
        /// <param name="lastUpdate">The UTC time of the last update.</param>
        void Reset(DateTime lastUpdate);

        /// <summary>
        /// Forces an update.
        /// </summary>
        /// <param name="retryOnError">When true, the update will be reattempted until succesful.</param>
        void ForceUpdate(bool retryOnError = false);
    }
}