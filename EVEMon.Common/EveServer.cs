using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the status of a server
    /// </summary>
    public sealed class EveServer
    {
        private int m_users;
        private ServerStatus m_status;
        private readonly QueryMonitor<SerializableAPIServerStatus> m_serverStatusMonitor;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal EveServer()
        {
            m_status = ServerStatus.Online;

            m_serverStatusMonitor = new QueryMonitor<SerializableAPIServerStatus>(APIGenericMethods.ServerStatus,
                                                                                  OnServerStatusMonitorUpdated) { Enabled = true };
        }

        /// <summary>
        /// Gets the server's name.
        /// </summary>
        private static string Name
        {
            get
            {
                return (EveMonClient.APIProviders.CurrentProvider != APIProvider.TestProvider
                            ? "Tranquility"
                            : "Sinqularity");
            }
        }

        /// <summary>
        /// Gets the server status message.
        /// </summary>
        public string StatusText
        {
            get
            {
                switch (m_status)
                {
                    case ServerStatus.Online:
                        return String.Format(CultureConstants.DefaultCulture,
                                             "{0} Server Online ({1:N0} Pilots)", Name, m_users);
                    case ServerStatus.Offline:
                        return String.Format(CultureConstants.DefaultCulture, "{0} Server Offline", Name);
                    case ServerStatus.CheckDisabled:
                        return String.Format(CultureConstants.DefaultCulture, "{0} Server Status Check Disabled", Name);
                    default:
                        return String.Format(CultureConstants.DefaultCulture, "{0} Server Status Unknown", Name);
                }
            }
        }

        /// <summary>
        /// Forces an update of the server status.
        /// </summary>
        public void ForceUpdate()
        {
            m_serverStatusMonitor.ForceUpdate(false);
        }

        /// <summary>
        /// Occurs when CCP returns new data.
        /// </summary>
        /// <param name="result"></param>
        private void OnServerStatusMonitorUpdated(APIResult<SerializableAPIServerStatus> result)
        {
            ServerStatus lastStatus = m_status;

            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return;

            // Was there an error ?
            if (result.HasError)
            {
                m_status = ServerStatus.Unknown;
                EveMonClient.Notifications.NotifyServerStatusError(result);

                // Notify subscribers about update
                EveMonClient.OnServerStatusUpdated(this, lastStatus, m_status);
                return;
            }

            // Update status and users, notify no more errors
            m_users = result.Result.Players;
            m_status = (result.Result.Open ? ServerStatus.Online : ServerStatus.Offline);
            EveMonClient.Notifications.InvalidateAPIError();

            // Notify subscribers about update
            EveMonClient.OnServerStatusUpdated(this, lastStatus, m_status);

            // Send a notification
            if (lastStatus != m_status)
            {
                EveMonClient.Notifications.NotifyServerStatusChanged(Name, m_status);
                return;
            }

            EveMonClient.Notifications.InvalidateServerStatusChange();
        }
    }
}