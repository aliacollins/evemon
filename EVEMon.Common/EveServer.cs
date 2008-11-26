using System;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Net;
using EVEMon.Common.Net;

namespace EVEMon.Common
{
    public class EveServerEventArgs : EventArgs
    {
        public EveServerEventArgs(string info, System.Windows.Forms.ToolTipIcon icon)
        {
            this.info = info;
            this.icon = icon;
        }

        public EveServerEventArgs(string info)
        {
            this.info = info;
        }

        public string info;
        public System.Windows.Forms.ToolTipIcon icon;
    }

    public class EveServer
    {
        private static EveServer m_instance = null;

        private enum Status { Offline, Starting, Online }

        private Regex m_re = new Regex(@"Starting\ up\.\.\.\((\d+)\ sec\.\)");
        private string m_motd = "";
        private int m_users = 0;
        private int m_countdown = 0;
        private string m_statusText;
        private string m_balloonText;
        private bool m_pendingAlerts;
        private System.Windows.Forms.ToolTipIcon m_balloonIcon;
        private Status m_status = Status.Online;
        private Status m_lastStatus = Status.Online;
        private Timer m_tmrCheck;
        private Timer m_tmrCountdown;
        private Settings m_settings;

        /// <summary>
        /// Fired whenever the TQ server changes state (e.g. goes offline, starting up, online etc.
        /// </summary>
        public event EventHandler<EveServerEventArgs> ServerStatusChanged;
        

        /// <summary>
        /// Fired every time we ping the TQ server status (update pilots online count etc)
        /// </summary>
        public event EventHandler<EveServerEventArgs> ServerStatusUpdated;

        /*
        public static bool Online { get { return (m_status == Status.Online); } }
        public static bool Offline { get { return (m_status == Status.Offline); } }
        public static bool Starting { get { return (m_status == Status.Starting); } }
         */

        /// <summary>
        /// Gets the TQ Message (not currently used)
        /// </summary>
        public string Motd { get { return m_motd; } }

        /// <summary>
        /// Gets the number of users online at the last update
        /// </summary>
        public int Users { get { return m_users; } }


        /// <summary>
        /// Gets the number of seconds till TQ comes up (when server is in "Startup" state)
        /// </summary>
        public int Countdown { get { return m_countdown; } }

        /// <summary>
        /// Gets the server status message
        /// </summary>
        public string StatusText { get { return m_statusText; } }


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
        /// ALSO STARTS THE TIMER IF THE REQUIRED
        /// </summary>
        /// <returns></returns>
        public static EveServer GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new EveServer();
                m_instance.m_settings = Settings.GetInstance();
                if (m_instance.m_settings.CheckTranquilityStatus)
                {
                    m_instance.m_statusText = "Tranquility Server Status Unknown";
                }
                else
                {
                    m_instance.m_statusText = "Tranquility Server Status Check Disabled";
                }
            }
            return m_instance;
        }

        /// <summary>
        /// Kick off the TQ status check timer - should be called at startup
        /// and whenever user  enables Settings.CheckTranquilityStatus
        /// </summary>
        public void StartTQChecks()
        {
            if (m_tmrCheck == null)
            {
                m_tmrCheck = new System.Timers.Timer(m_settings.StatusUpdateInterval * 60000);
                m_tmrCheck.Elapsed += new ElapsedEventHandler(checkServerStatus);
            }
            m_tmrCheck.Enabled = true;
            if (m_tmrCountdown == null)
            {
                m_tmrCountdown = new System.Timers.Timer(1000);
                m_tmrCountdown.Elapsed += new ElapsedEventHandler(tmrCountdown);
            }
            m_tmrCountdown.Enabled = false;
            checkServerStatus(this, new EventArgs());
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
            if (m_tmrCountdown != null)
            {
                m_tmrCountdown.Enabled = false;
            }

            m_status = Status.Offline;
            m_users = 0;

            m_statusText = "Tranquility Server Status Check Disabled";
            if (ServerStatusUpdated != null)
            {
                ServerStatusUpdated(this, new EveServerEventArgs(m_statusText));
            }

        }

        /// <summary>
        /// Gets the TQ Message of the day (currently unused)
        /// </summary>
        private void fetchMotd()
        {
            try
            {
                m_motd = CommonContext.HttpWebService.DownloadString(
                    "http://www.eve-online.com/motd.asp?server=" + m_settings.CustomTQAddress);
            }
            catch (HttpWebServiceException ex)
            {
                ExceptionHandler.LogException(ex, true);
            }
        }

        /// <summary>
        /// When TQ is in "Start up" state, track the countdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrCountdown(object sender, EventArgs e)
        {
            m_tmrCountdown.Enabled = false;

            if (m_status == Status.Starting)
            {
                // is countdown finished?
                if (m_countdown == -1)
                {
                    m_countdown = 0;
                    m_tmrCountdown.Enabled = false;
                    checkServerStatus(m_instance, new EventArgs());
                    return;
                }
                // no - still counting down ...
                setInformationText();
                if (ServerStatusUpdated != null)
                {
                    ServerStatusUpdated(m_instance, new EveServerEventArgs(m_statusText));
                }
                m_countdown -= 1;
            }
            m_tmrCountdown.Enabled = true;
        }

        // Semaphore to flag whether we are in the middle of an async server test
        bool m_checkingServer = false;

        /// <summary>
        /// Called when we have data from TQ 
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            TcpClient conn = (TcpClient)ar.AsyncState;
            if (ar.IsCompleted && conn.Connected)
            {
                m_status = Status.Online;
                try
                {
                    NetworkStream stream = conn.GetStream();
                    byte[] data = {0x23, 0x00, 0x00, 0x00, 0x7E, 0x00, 0x00, 0x00,
                        0x00, 0x14, 0x06, 0x04, 0xE8, 0x99, 0x02, 0x00,
                        0x05, 0x8B, 0x00, 0x08, 0x0A, 0xCD, 0xCC, 0xCC,
                        0xCC, 0xCC, 0xCC, 0x00, 0x40, 0x05, 0x49, 0x0F,
                        0x10, 0x05, 0x42, 0x6C, 0x6F, 0x6F, 0x64};
                    stream.Write(data, 0, data.Length);
                    byte[] response = new byte[256];
                    int bytes = stream.Read(response, 0, 256);

                    m_users = 0;
                    if (bytes > 21)
                    {
                        int usersBytes = 0;
                        // Amended usercount checks, info from clef on iRC
                        // [16:01] <clef> BradStone: for the moment, take that byte[19] ... if it is 1, 8 or 9, the usercount is 0.
                        // [16:01] <clef> BradStone: if it is 4, the next 32bit are the usercount. 5 -> 16bit. 6 -> 8bit.
                        switch (response[19])
                        {
                            case 4:
                                usersBytes = 4;
                                break;
                            case 5:
                                usersBytes = 2;
                                break;
                            case 6:
                                usersBytes= 1;
                                break;
                        }
                        if (usersBytes > 0 && usersBytes < 5)
                        {
                            int multiplyer = 1;
                            for (int i=0;i<usersBytes;i++)
                            {
                                m_users += response[20+i] * multiplyer;
                                multiplyer *= 256;
                            }
                        }
                    }

                    string str = new System.Text.ASCIIEncoding().GetString(response);

                    Match m = m_re.Match(str);
                    if (m.Success)
                    {
                        m_status = Status.Starting;
                        m_countdown = Convert.ToInt32(m.Groups[1].Value) - 1;
                        m_tmrCountdown.Enabled = true;
                    }
                    conn.EndConnect(ar);
                }
                catch (Exception)
                {
                    m_status = Status.Offline;
                    m_users = 0;
                }
            }
            else
            {
                m_status = Status.Offline;
                m_users = 0;
            }

            // Close the connection
            conn.Close();

            // Everything checked, lets see if we need to update something ...
            setInformationText();
            if (m_status != m_lastStatus)
            {
                if (ServerStatusChanged != null)
                {
                    ServerStatusChanged(m_instance, new EveServerEventArgs(m_balloonText, m_balloonIcon));
                }
                m_lastStatus = m_status;
            }
            if (ServerStatusUpdated != null)
            {
                ServerStatusUpdated(m_instance, new EveServerEventArgs(m_statusText));
            }

            // switch off the semaphore
            m_checkingServer = false;
        }

        private void checkServerStatus(object source, EventArgs e)
        {
            // Check the semaphore to see if we're mid check
            if (m_checkingServer == true)
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
                m_statusText = "Tranquility Server Status Unknown";
                if (ServerStatusUpdated != null)
                {
                    ServerStatusUpdated(m_instance, new EveServerEventArgs(m_statusText));
                }
                // switch off the semaphore
                m_checkingServer = false;
                return;
            }

            TcpClient conn = new TcpClient();
            try
            {
                // Set default port and ip - also perform final validation
                int serverPort = 26000;
                System.Net.IPAddress serverAddress = System.Net.IPAddress.Parse("87.237.38.200");

                // If the user selected port is valid use that one, maybe they hand edited the xml file and bypassed input validation
                if (int.TryParse(m_settings.CustomTQPort, out serverPort) && System.Net.IPAddress.TryParse(m_settings.CustomTQAddress, out serverAddress))
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.WriteLine("DEBUG: TQ check connecting to [" + serverAddress.ToString() + ":" + serverPort.ToString() + "]");
                    }

                    conn.BeginConnect(serverAddress.ToString(), serverPort, ConnectCallback, conn);
                }
                else
                    throw new Exception("Invalid TQ server IP or port"); // Shouldn't ever get here
            }
            catch (Exception)
            {
                conn.Close();
                m_status = Status.Offline;
                m_users = 0;
                // switch off the semaphore - the check failed
                m_checkingServer = false;
            }
        }

        /// <summary>
        /// Set the message for TQ status
        /// </summary>
        private void setInformationText()
        {
            if (m_status == Status.Online)
            {
                m_statusText = "Tranquility Server Online (" + m_users.ToString() + " Pilots)";
                m_balloonText = "The Tranquility Server is now online!";
                m_balloonIcon = System.Windows.Forms.ToolTipIcon.Info;
            }
            else if (m_status == Status.Starting)
            {
                m_statusText = "Tranquility Server Starting (" + m_countdown.ToString() + " Seconds)";
                m_balloonText = "The Tranquility Server is starting up!";
                m_balloonIcon = System.Windows.Forms.ToolTipIcon.Warning;
            }
            else if (m_status == Status.Offline)
            {
                m_statusText = "Tranquility Server Offline";
                m_balloonText = "The Tranquility Server is offline!";
                m_balloonIcon = System.Windows.Forms.ToolTipIcon.Error;
            }
        }
    }

    /// <summary>
    /// Class to determine if we have an internet connection
    /// We use a different URL to eve-online in case eve-online is down
    /// The server needs to be a big public server that is up 24/7/365 such as google.com
    /// </summary>
    public class InternetCS
    {
        /// <summary>
        /// Performs a HTTP request to a url (default is http://google.com)
        /// </summary>
        /// <returns>true if internet connection is working properly</returns>
        public static bool IsConnectedToInternet(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine("Connection to google.com failed: " + e.Message);
                }
            }
            return false;
        }
    }
}
