using System;
using System.Timers;
using System.Xml;
using EVEMon.Common.Net;

namespace EVEMon.Common
{
    public class EveServer
    {
        private static EveServer m_instance = null;

        private enum Status { Offline, Online, Unknown, CheckDisabled }

        private int m_users = 0;
        private bool m_pendingAlerts;
        private bool m_firstStatusCheck; 
        private Status m_status = Status.Online;
        private Status m_lastStatus = Status.Online;
        private Timer m_tmrCheck;
        private readonly Settings m_settings;

        private EveServer(Settings settings)
        {
            m_settings = settings;
            if (m_settings.CheckTranquilityStatus)
            {
                m_status = Status.Unknown;
            }
            else
            {
                m_status = Status.CheckDisabled;
            }
        }

        /// <summary>
        /// Fired whenever the TQ server changes state (e.g. goes offline, starting up, online etc.
        /// </summary>
        public event EventHandler<EveServerEventArgs> ServerStatusChanged;

        /// <summary>
        /// Fired every time we ping the TQ server status (update pilots online count etc)
        /// </summary>
        public event EventHandler<EveServerEventArgs> ServerStatusUpdated;

        /// <summary>
        /// Gets the number of users online at the last update
        /// </summary>
        public int Users { get { return m_users; } }

        /// <summary>
        /// Gets the server status message
        /// </summary>
        public string StatusText
        {
            get
            {
                if (m_status == Status.Online)
                    return "Tranquility Server Online (" + m_users + " Pilots)";
                else if (m_status == Status.Offline)
                    return "Tranquility Server Offline";
                else if (m_status == Status.CheckDisabled)
                    return "Tranquility Server Status Check Disabled";
                else return "Tranquility Server Status Unknown";
            }
        }

        /// <summary>
        /// Is the balloon tip active?
        /// </summary>
        public bool PendingAlerts
        {
            get { return m_pendingAlerts; }
            set { m_pendingAlerts = value; }
        }

        /// <summary>
        /// Gets the singleton instance of the TQ Server class.
        /// </summary>
        /// <returns></returns>
        public static EveServer GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new EveServer(Settings.GetInstance());
            }
            return m_instance;
        }

        /// <summary>
        /// Kick off the TQ status check timer - should be called at startup
        /// and whenever user  enables Settings.CheckTranquilityStatus
        /// </summary>
        public void StartTQChecks()
        {
            m_firstStatusCheck = true;

            if (m_tmrCheck == null)
            {
                //In order to prevent blocking on slow connections set the first check to be in .5 sec
                m_tmrCheck = new System.Timers.Timer(500);

                m_tmrCheck.Elapsed += new ElapsedEventHandler(checkServerStatus);

                m_status = Status.Unknown;
            }
            else
            {
                //If TQChecks are restarted using the options dialog set the first check timer
                m_tmrCheck.Interval = 500;

                m_status = Status.Unknown;

                OnServerStatusChanged();
            }
            
            m_tmrCheck.Enabled = true;
        }

        /// <summary>
        /// Stops the TQ status check - should be called whenever user disables
        /// Settings.CheckTranquilityStatus
        /// </summary>
        public void StopTQChecks()
        {
            if (m_tmrCheck != null)
            {
                m_tmrCheck.Enabled = false;
            }
            m_status = Status.Offline;
            m_users = 0;
            OnServerStatusUpdated();
        }

        // Semaphore to flag whether we are in the middle of an async server test
        bool m_checkingServer = false;

        private void checkServerStatus(object source, EventArgs e)
        {
            // check to see if this is the first status check and reset
            // the interval to the user setting
            if (m_firstStatusCheck == true)
            {
                m_firstStatusCheck = false;
                m_tmrCheck.Interval = m_settings.StatusUpdateInterval * 60000;
            }
            
            // Check the semaphore to see if we're mid check
            if (m_checkingServer)
            {
                return;
            }

            // check to see if we are recovering from loss of connection (timer was set to 30 seconds)
            if (m_tmrCheck.Interval == 30000)
            {
                // network is back - set timer to correct value.
                m_tmrCheck.Interval = m_settings.StatusUpdateInterval * 60000;
            }

            // check that we have a network connection
            if (!InternetCS.IsConnectedToInternet(m_settings.ConnectivityURL))
            {
                // switch on the semaphore
                m_checkingServer = true;

                // oops, we've lost the network - reset timer to 30 seconds
                m_tmrCheck.Interval = 30000;
                m_status = Status.Unknown;
                OnServerStatusUpdated();
                // switch off the semaphore
                m_checkingServer = false;
                return;
            }

            try
            {
                APIState apiState = Singleton.Instance<APIState>();
                APIConfiguration configuration = apiState.CurrentConfiguration;
                XmlDocument xdoc = CommonContext.HttpWebService.DownloadXml(configuration.MethodUrl(APIMethods.ServerStatus));
                XmlNode errorNode = xdoc.SelectSingleNode("descendant::error");
                if (errorNode == null)
                {
                    ServerStatusInfo status = ServerStatusInfo.LoadFromXml(xdoc);
                    m_status = status.ServerStatusResult.ServerOpen ? Status.Online : Status.Offline;
                    m_users = status.ServerStatusResult.OnlinePlayers;
                }
                else
                {
                    m_status = Status.Unknown;
                }
                // Everything checked, lets see if we need to update something ...

            }
            catch(HttpWebServiceException)
            {
                m_status = Status.Unknown;
            }
            finally
            {
                if (m_status != m_lastStatus && (m_status == Status.Online || m_status == Status.Offline))
                {
                    OnServerStatusChanged();
                    m_lastStatus = m_status;
                }
                OnServerStatusUpdated();
                m_checkingServer = false;
            }
        }

        private void OnServerStatusChanged()
        {
            if (ServerStatusChanged != null)
            {
                if (m_status == Status.Online)
                {
                    ServerStatusChanged(this, new EveServerEventArgs("The Tranquility Server is now online!", System.Windows.Forms.ToolTipIcon.Info));
                }
                else if (m_status == Status.Offline)
                {
                    ServerStatusChanged(this, new EveServerEventArgs("The Tranquility Server is offline!", System.Windows.Forms.ToolTipIcon.Error));
                }
            }
        }

        private void OnServerStatusUpdated()
        {
            if (ServerStatusUpdated != null)
            {
                ServerStatusUpdated(this, new EveServerEventArgs(StatusText));
            }
        }

        private class ServerStatusInfo
        {
            private DateTime m_curTime = DateTime.MinValue;

            public string CurrentTime
            {
                get { return TimeUtil.ConvertDateTimeToCCPTimeString(m_curTime); }
                set
                {
                    m_curTime = TimeUtil.ConvertCCPTimeStringToDateTime(value);
                }

            }

            private ServerStatusResult _serverStatusResult;

            public ServerStatusResult ServerStatusResult
            {
                get { return _serverStatusResult; }
                set { _serverStatusResult = value; }
            }

            private DateTime m_cachedUntilTime = DateTime.MinValue;

            public string CachedUntilTime
            {
                get { return TimeUtil.ConvertDateTimeToCCPTimeString(m_cachedUntilTime); }
                set
                {
                    m_cachedUntilTime = TimeUtil.ConvertCCPTimeStringToDateTime(value);
                }
            }

            public static ServerStatusInfo LoadFromXml(XmlDocument xdoc)
            {
                ServerStatusInfo serverStatus = new ServerStatusInfo();
                serverStatus.CurrentTime = xdoc.SelectSingleNode("/eveapi/currentTime").InnerText;
                serverStatus.CachedUntilTime = xdoc.SelectSingleNode("/eveapi/cachedUntil").InnerText;
                serverStatus.ServerStatusResult = new ServerStatusResult();
                serverStatus.ServerStatusResult.ServerOpen = bool.Parse(xdoc.SelectSingleNode("/eveapi/result/serverOpen").InnerText);
                serverStatus.ServerStatusResult.OnlinePlayers = int.Parse(xdoc.SelectSingleNode("/eveapi/result/onlinePlayers").InnerText);
                return serverStatus;
            }
        }

        private class ServerStatusResult
        {
            private bool _serverOpen;

            public bool ServerOpen
            {
                get { return _serverOpen;}
                set { _serverOpen = value;}
            }

            private int _onlinePLayers;

            public int OnlinePlayers
            {
                get { return _onlinePLayers; }
                set { _onlinePLayers = value;}
            }

        }
    }
}
