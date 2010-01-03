using System;
using System.Timers;
using System.Xml;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.API;
using System.Globalization;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the status of a server
    /// </summary>
    public sealed class EveServer
    {
        private int m_users;
        private ServerStatus m_status;
        private QueryMonitor<SerializableServerStatus> m_monitor;

        /// <summary>
        /// Constructor
        /// </summary>
        internal EveServer()
        {
            m_status = ServerStatus.Online;

            m_monitor = new QueryMonitor<SerializableServerStatus>(APIMethods.ServerStatus);
            m_monitor.Updated += new QueryCallback<SerializableServerStatus>(OnMonitorUpdated);
        }

        /// <summary>
        /// Gets the number of users online at the last update
        /// </summary>
        public int Users 
        { 
            get { return m_users; } 
        }

        /// <summary>
        /// Gets the server's status
        /// </summary>
        public ServerStatus Status
        {
            get { return m_status; }
        }

        /// <summary>
        /// Gets the server status message
        /// </summary>
        public string StatusText
        {
            get
            {
                switch(m_status)
                {
                    case ServerStatus.Online:
                        return String.Format(CultureConstants.TidyInteger, "Server Online ({0:n} Pilots)", m_users);
                    case ServerStatus.Offline:
                        return "Server Offline";
                    case ServerStatus.CheckDisabled:
                        return "Server Status Check Disabled";
                    default:
                        return "Server Status Unknown";
                }
            }
        }


        /// <summary>
        /// Update on a time tick
        /// </summary>
        public void UpdateOnOneSecondTick()
        {
            m_monitor.UpdateOnOneSecondTick();
        }

        /// <summary>
        /// Occurs when CCP returns new data
        /// </summary>
        /// <param name="result"></param>
        private void OnMonitorUpdated(APIResult<SerializableServerStatus> result)
        {
            // Was there an error ?
            var lastStatus = m_status;
            if (result.HasError)
            {
                m_status = ServerStatus.Unknown;
                EveClient.Notifications.NotifyServerStatusError(result);
                return;
            }

            // Update status and users, notify no more errors
            m_users = result.Result.Players;
            m_status = (result.Result.Open ? ServerStatus.Online : ServerStatus.Offline);
            EveClient.Notifications.InvalidateServerStatusError();

            // Notify subscribers about update
            EveClient.OnServerStatusUpdated(this, lastStatus, m_status);

            // Send a notification
            if (lastStatus != m_status) EveClient.Notifications.NotifyServerStatusChange(m_status);
            else EveClient.Notifications.InvalidateServerStatusChange();
        }
    }
}
