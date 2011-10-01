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

            m_serverStatusMonitor = new QueryMonitor<SerializableAPIServerStatus>(APIMethods.ServerStatus);
            m_serverStatusMonitor.Updated += OnServerStatusMonitorUpdated;
            m_serverStatusMonitor.Enabled = true;
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
                        return String.Format("{0} Server Offline", Name);
                    case ServerStatus.CheckDisabled:
                        return "Server Status Check Disabled";
                    default:
                        return String.Format("{0} Server Status Unknown", Name);
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

            // Was there an error ?
            if (result.HasError)
            {
                // Checks if EVE Backend Database is temporarily disabled
                if (result.EVEBackendDatabaseDisabled)
                    return;

                m_status = ServerStatus.Unknown;
                EveMonClient.Notifications.NotifyServerStatusError(result);

                // Notify subscribers about update
                EveMonClient.OnServerStatusUpdated(this, lastStatus, m_status);
                return;
            }

            // Update status and users, notify no more errors
            m_users = result.Result.Players;
            m_status = (result.Result.Open ? ServerStatus.Online : ServerStatus.Offline);
            EveMonClient.Notifications.InvalidateServerStatusError();

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