using System;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Net;

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
        public event EventHandler<EveServerEventArgs> ServerStatusChanged;
        public event EventHandler<EveServerEventArgs> ServerStatusUpdated;

        /*
        public static bool Online { get { return (m_status == Status.Online); } }
        public static bool Offline { get { return (m_status == Status.Offline); } }
        public static bool Starting { get { return (m_status == Status.Starting); } }
         */

        public string Motd { get { return m_motd; } }

        public int Users { get { return m_users; } }

        public int Countdown { get { return m_countdown; } }

        public bool PendingAlerts
        {
            get { return m_pendingAlerts; }
            set { m_pendingAlerts = value; }
        }

        public static EveServer GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new EveServer();
                m_instance.Init();
            }
            return m_instance;
        }

        public void Init()
        {
            m_settings = Settings.GetInstance();
            m_tmrCheck = new System.Timers.Timer(m_settings.StatusUpdateInterval * 60000);
            m_tmrCheck.Elapsed += new ElapsedEventHandler(checkServerStatus);
            m_tmrCheck.Enabled = true;
            m_tmrCountdown = new System.Timers.Timer(1000);
            m_tmrCountdown.Elapsed += new ElapsedEventHandler(tmrCountdown);
            m_tmrCountdown.Enabled = false;
            checkServerStatus(m_instance, new EventArgs());
            //            fetchMotd();
        }

        private void fetchMotd()
        {
            try
            {
                m_motd = EVEMonWebRequest.GetUrlString("http://www.eve-online.com/motd.asp?server=" + m_settings.CustomTQAddress);
            }
            catch (EVEMonNetworkException ne)
            {
                ExceptionHandler.LogException(ne, true);
            }
        }

        private void tmrCountdown(object sender, EventArgs e)
        {
            m_tmrCountdown.Enabled = false;

            if (m_status == Status.Starting)
            {
                if (m_countdown == -1)
                {
                    m_countdown = 0;
                    m_tmrCountdown.Enabled = false;
                    checkServerStatus(m_instance, new EventArgs());
                    return;
                }
                // counting down ...
                setInformationText();
                ServerStatusUpdated(m_instance, new EveServerEventArgs(m_statusText));
                m_countdown -= 1;
            }
            m_tmrCountdown.Enabled = true;
        }

        // Semaphore to flag whether we are in the middle of an async server test
        bool m_checkingServer = false;
        private void ConnectCallback(IAsyncResult ar)
        {
            TcpClient conn = (TcpClient)ar.AsyncState;
            if (ar.IsCompleted && conn.Connected)
            {
                m_status = Status.Online;
                NetworkStream stream = conn.GetStream();
                byte[] data = {0x23, 0x00, 0x00, 0x00, 0x7E, 0x00, 0x00, 0x00,
                        0x00, 0x14, 0x06, 0x04, 0xE8, 0x99, 0x02, 0x00,
                        0x05, 0x8B, 0x00, 0x08, 0x0A, 0xCD, 0xCC, 0xCC,
                        0xCC, 0xCC, 0xCC, 0x00, 0x40, 0x05, 0x49, 0x0F,
                        0x10, 0x05, 0x42, 0x6C, 0x6F, 0x6F, 0x64};
                stream.Write(data, 0, data.Length);
                byte[] response = new byte[256];
                int bytes = stream.Read(response, 0, 256);
                if (bytes > 21)
                {
                    m_users = response[21] * 256 + response[20];

                }
                else
                {
                    m_users = 0;
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

            // check that we have a network connection
            if (!InternetCS.IsConnectedToInternet())
            {
                // oops, we've lost the network - reset timer to 30 seconds
                m_tmrCheck.Interval = 30000;
                m_statusText = "// Server Status Unknown";
                if (ServerStatusUpdated != null)
                {
                    ServerStatusUpdated(m_instance, new EveServerEventArgs(m_statusText));
                }
                return;
            }

            // check to see if we are recovering from loss of connection (timer was set to 30 seconds)
            if (m_tmrCheck.Interval == 30000)
            {
                // network is back - set timer to correct value.
                m_tmrCheck.Interval = m_settings.StatusUpdateInterval * 60000;
            }

            // switch on the semaphore
            m_checkingServer = true;

            if (m_settings.CheckTranquilityStatus)
            {
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
            else
            {
                m_status = Status.Offline;
                m_users = 0;
            }
        }

        public void setInformationText()
        {
            if (m_status == Status.Online)
            {
                m_statusText = "// Tranquility Server Online (" + m_users.ToString() + " Pilots)";
                m_balloonText = "The Tranquility Server is now online!";
                m_balloonIcon = System.Windows.Forms.ToolTipIcon.Info;
            }
            else if (m_status == Status.Starting)
            {
                m_statusText = "// Tranquility Server Starting (" + m_countdown.ToString() + " Seconds)";
                m_balloonText = "The Tranquility Server is starting up!";
                m_balloonIcon = System.Windows.Forms.ToolTipIcon.Warning;
            }
            else if (m_status == Status.Offline)
            {
                m_statusText = "// Tranquility Server Offline";
                m_balloonText = "The Tranquility Server is offline!";
                m_balloonIcon = System.Windows.Forms.ToolTipIcon.Error;
            }
        }
    }

    public class InternetCS
    {
        /// <summary>
        /// Performs a HTTP request to http://google.com
        /// </summary>
        /// <returns>true if internet connection is working properly</returns>
        public static bool IsConnectedToInternet()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://google.com");
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
