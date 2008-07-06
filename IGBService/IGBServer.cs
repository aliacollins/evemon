using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using EVEMon.Common;

namespace EVEMon.IGBService
{
    public class IGBServer
    {
        public IGBServer()
        {
            m_listener.ClientConnected += new EventHandler<ClientConnectedEventArgs>(m_listener_ClientConnected);
        }

        public void Reset(bool isPublic, int port)
        {
            m_public = isPublic;
            m_port = port;
            Stop();
            m_listener = null;

            m_listener = new IGBTcpListener(new IPEndPoint(m_public ? IPAddress.Any : IPAddress.Loopback, m_port));
            m_listener.ClientConnected += new EventHandler<ClientConnectedEventArgs>(m_listener_ClientConnected);

        }

        private bool m_public = false;
        private int m_port = 80;
        private bool m_running = false;

        public void Start()
        {
            if (!m_running)
            {
                m_running = true;
                m_listener.Start();
            }
        }

        public void Stop()
        {
            if (m_running)
            {
                m_running = false;
                m_listener.Stop();
            }
        }

        private IGBTcpListener m_listener = new IGBTcpListener(new IPEndPoint(IPAddress.Loopback, 80));
        private Dictionary<IGBTcpClient, byte[]> m_clients = new Dictionary<IGBTcpClient, byte[]>();

        private void m_listener_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            IGBTcpClient cli = new IGBTcpClient(e.TcpClient);
            cli.DataRead += new EventHandler<IGBClientDataReadEventArgs>(cli_DataRead);
            cli.Closed += new EventHandler<EventArgs>(cli_Closed);
            lock (m_clients)
            {
                m_clients.Add(cli, new byte[0]);
            }
            cli.Start();
        }

        private void cli_DataRead(object sender, IGBClientDataReadEventArgs e)
        {
            IGBTcpClient IgbSender = (IGBTcpClient) sender;
            byte[] newBuf;
            lock (m_clients)
            {
                byte[] existingBuf = m_clients[IgbSender];
                newBuf = new byte[existingBuf.Length + e.Count];

                Array.Copy(existingBuf, newBuf, existingBuf.Length);
                Array.Copy(e.Buffer, 0, newBuf, existingBuf.Length, e.Count);

                m_clients[IgbSender] = newBuf;
            }

            TryProcessBuffer(IgbSender, newBuf, Math.Min(e.Count + 1, newBuf.Length));
        }

        private void TryProcessBuffer(IGBTcpClient client, byte[] newBuf, int tailLength)
        {
            bool gotOne = false;
            bool gotTwo = false;
            for (int i = 0; i < tailLength; i++)
            {
                if (newBuf[newBuf.Length - i - 1] == ((byte) '\n'))
                {
                    if (gotOne)
                    {
                        gotTwo = true;
                        break;
                    }
                    else
                    {
                        gotOne = true;
                    }
                }
                else if (newBuf[newBuf.Length - i - 1] != ((byte) '\r'))
                {
                    gotOne = false;
                }
            }
            if (!gotTwo)
            {
                return;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string headerStr = Encoding.UTF8.GetString(newBuf);
            if (headerStr.IndexOf('\r') != -1)
            {
                headerStr = headerStr.Replace("\r", "");
            }

            bool first = true;
            string requestUrl = String.Empty;
            foreach (string tline in headerStr.Split('\n'))
            {
                if (first)
                {
                    Match m = Regex.Match(tline, @"^(GET|POST) (.+) HTTP/(.*)$");
                    if (m.Success)
                    {
                        requestUrl = m.Groups[2].Value;
                    }
                    first = false;
                }
                else
                {
                    Match m = Regex.Match(tline, "^(.*?): (.*)$");
                    if (m.Success)
                    {
                        headers[m.Groups[1].Value.ToLower()] = m.Groups[2].Value;
                    }
                }
            }

            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            {
                ProcessRequest(requestUrl, headers, sw);
                //sw.WriteLine("<html><head><title>here</title></head><body>hello world");
                //sw.WriteLine("verb={0}, url={1}</body></html>", requestVerb, requestUrl);

                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                client.Write("HTTP/1.1 200 OK\n");
                client.Write("Server: EVEMon/1.0\n");
                client.Write("Content-Type: text/html; charset=utf-8\n");
                if (headers.ContainsKey("eve.trusted") && headers["eve.trusted"].ToLower() == "no")
                {
                    client.Write("eve.trustme: http://" +  BuildHostAndPort(headers["host"]) + "/::EVEMon needs your pilot information.\n");
                }
                client.Write("Connection: close\n");
                client.Write("Content-Length: " + ms.Length.ToString() + "\n\n");
                using (StreamReader sr = new StreamReader(ms))
                {
                    client.Write(sr.ReadToEnd());
                }
            }

            //client.Write("HTTP/1.0 200 OK\n");
            //client.Write("Content-type: text/plain\n\n");
            //client.Write("hello world\n");
            //client.Write(String.Format("verb={0}, url={1}", requestVerb, requestUrl));
            client.Close();
        }

        /// <summary>
        /// Create the host:port string for the trustme request
        /// </summary>
        /// <param name="host">The host header from the IGB</param>
        /// <returns>hostname:port number</returns>
        private String BuildHostAndPort(String host)
        {
            // Currently IGB returns host:port as the host header, it shouldn't
            // really do this

            String hostPort = host;
            // if the host string already contains a port then do nothing
            // (IOGB shouldnt really do this but it is!
            if (!host.Contains(":")) 
            {
                // now cater for when/if CCP fix the IGB to not send port as part of the host header
                if (m_port != 80)
                {
                    // non-standard port - let's add it
                    hostPort = String.Format("{0}:{1}",host,m_port);
                }
            }
            return hostPort;
        }


        private void ProcessRequest(string requestUrl, Dictionary<string, string> headers, StreamWriter sw)
        {
            if (!headers.ContainsKey("eve.trusted"))
            {
                sw.WriteLine("Please visit this site using the in-game browser.");
                return;
            }
            if (headers["eve.trusted"].ToLower() != "yes")
            {
                sw.WriteLine("not trusted");
                return;
            }
            if (requestUrl.StartsWith("/plan/") || requestUrl.StartsWith("/shopping/"))
            {
                // strip off the bad trailing / added by IGB (Trac ticket 425)
                if (requestUrl.EndsWith("/"))
                {
                    requestUrl=requestUrl.Substring(0,requestUrl.Length-1);
                }
                bool shopping = requestUrl.StartsWith("/shopping/");
                string planName = HttpUtility.UrlDecode(requestUrl.Substring(
                        shopping ? "/shopping/".Length : "/plan/".Length
                    ));
                sw.WriteLine("<html><head><title>Plan</title></head><body>");
                sw.WriteLine(String.Format("<h1>Plan: {0}</h1>",
                                           HttpUtility.HtmlEncode(planName)));

                CharacterInfo ci = Program.MainWindow.GetCharacterInfo(headers["eve.charname"]);

                Plan p = Program.Settings.GetPlanByName(headers["eve.charname"], ci, planName);
                if (p == null)
                {
                    sw.WriteLine("non-existant plan name");
                }
                else
                {
                    if (p.GrandCharacterInfo == null)
                    {
                        p.GrandCharacterInfo = ci;
                    }
                    if (p.GrandCharacterInfo == null)
                    {
                        sw.Write("Could not get character info");
                    }
                    else
                    {
                        PlanTextOptions x = new PlanTextOptions();
                        x.EntryTrainingTimes = !shopping; // only if not shopping
                        x.EntryStartDate = !shopping; // only if not shopping
                        x.EntryFinishDate = !shopping; // only if not shopping
                        x.FooterTotalTime = !shopping; // only if not shopping
                        x.FooterCount = true;
                        x.FooterDate = !shopping; // only if not shopping
                        x.ShoppingList = shopping;
                        x.EntryCost = true;
                        x.FooterCost = true;
                        x.Markup = MarkupType.Html;
                        p.SaveAsText(sw, x);
                    }
                }

                sw.WriteLine("<hr><a href=\"/\">Back</a>");
                sw.WriteLine("</body></html>");
            }
            else if (requestUrl.StartsWith("/skills/bytime"))
            {
                sw.WriteLine("<html><head><title>Skills</title></head><body>");
                sw.WriteLine("<h1>Skills: By training time</h1>");
                sw.WriteLine(string.Format("<p>Skills for {0}</p>", HttpUtility.HtmlEncode(headers["eve.charname"])));            

                CharacterInfo ci = Program.MainWindow.GetCharacterInfo(headers["eve.charname"]);
                List<Skill> allskills = new List<Skill>();
                SerializableCharacterSheet charSheet = Program.Settings.GetCharacterSheet(headers["eve.charname"]);

                foreach (SerializableSkillGroup ssg in charSheet.SkillGroups)
                    foreach (SerializableSkill ss in ssg.Skills)
                    {
                        Skill s = ci.SkillGroups[ssg.Name][ss.Name];
                        if (s.Level < 5)
                            allskills.Add(s);
                    }

                allskills.Sort(delegate(Skill x, Skill y)
                {
                    return x.GetTrainingTimeToNextLevel().CompareTo(y.GetTrainingTimeToNextLevel());
                });

                sw.WriteLine("<table>");

                sw.Write("<tr><td colspan=\"2\" width=\"265\"><b>Skill</b></td>" +
                    "<td width=\"100\"><b>Next Level</b><\td><td><b>Training Time</b></td></tr>");

                foreach (Skill s in allskills)
                {
                    sw.Write("<tr>");

                    sw.Write("<td width=\"15\">");
                    sw.Write(string.Format("<b>{0}.</b>", allskills.IndexOf(s)+1));
                    sw.Write("</td>");

                    sw.Write("<td width=\"250\">");
                    sw.Write(string.Format("<b><a href=\"showinfo:{0}\">{1}</a></b>", s.Id, s.Name));
                    sw.Write("</td>");

                    sw.Write("<td width=\"100\">");
                    sw.Write(string.Format("<b>{0} -&gt; {1}</b>", s.RomanLevel, Skill.GetRomanForInt(s.Level+1)));
                    sw.Write("</td>");

                    sw.Write("<td>");
                    sw.Write(Skill.TimeSpanToDescriptiveText(s.GetTrainingTimeToNextLevel(), 
                                                        DescriptiveTextOptions.FullText | 
                                                        DescriptiveTextOptions.IncludeCommas | 
                                                        DescriptiveTextOptions.SpaceText));
                    sw.Write("</td>");
                    sw.Write("</tr>");
                }
                sw.WriteLine("</table>");

                sw.WriteLine("<hr><a href=\"/\">Back</a>");
                sw.WriteLine("</body></html>");
            }
            else
            {
                sw.WriteLine(
                    String.Format("<html><head><title>Hi</title></head><body><h1>Hello, {0}</h1>",
                                  headers["eve.charname"]));

                sw.WriteLine("<h2>Your Plans:</h2>");
                foreach (string s in Program.Settings.GetPlansForCharacter(headers["eve.charname"]))
                {
                    sw.WriteLine(String.Format("<a href=\"/plan/{0}\">{1}</a> (<a href=\"/shopping/{0}\">shopping list</a>)<br>",
                                               HttpUtility.UrlEncode(s),
                                               HttpUtility.HtmlEncode(s)));
                }

                sw.WriteLine("<br><h2>Your Skills:</h2>");
                sw.WriteLine("<a href=\"/skills/bytime\">By training time</a>");
                sw.WriteLine("</body></html>");
            }
        }

        private void cli_Closed(object sender, EventArgs e)
        {
            lock (m_clients)
            {
                m_clients.Remove((IGBTcpClient) sender);
            }
        }
    }

    public class IGBTcpListener
    {
        public IGBTcpListener(IPEndPoint ep)
        {
            m_listenEndpoint = ep;
        }

        private bool m_running = false;
        private IPEndPoint m_listenEndpoint;
        private TcpListener m_listener;

        public void Start()
        {
            lock (this)
            {
                if (!m_running)
                {
                    m_running = true;
                    m_listener = new TcpListener(m_listenEndpoint);
                    try
                    {
                        m_listener.Start();
                        BeginAcceptTcpClient(false);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.LogException(e, true);
                        m_listener = null;
                    }
                }
            }
        }

        private void BeginAcceptTcpClient(bool acquireLock)
        {
            if (acquireLock)
            {
                Monitor.Enter(this);
            }
            try
            {
                IAsyncResult ar;
                do
                {
                    ar = null;
                    if (m_running)
                    {
                        ar = m_listener.BeginAcceptTcpClient(new AsyncCallback(EndAcceptTcpClient), null);
                    }
                } while (ar != null && ar.CompletedSynchronously);
            }
            finally
            {
                if (acquireLock)
                {
                    Monitor.Exit(this);
                }
            }
        }

        private void EndAcceptTcpClient(IAsyncResult ar)
        {
            try
            {
                bool inLock = ar.CompletedSynchronously;

                TcpClient newClient = m_listener.EndAcceptTcpClient(ar);
                OnClientConnected(newClient, !inLock);
                if (!ar.CompletedSynchronously)
                {
                    BeginAcceptTcpClient(true);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }
        }

        private void OnClientConnected(TcpClient newClient, bool acquireLock)
        {
            if (acquireLock)
            {
                Monitor.Enter(this);
            }
            try
            {
                if (m_running && ClientConnected != null)
                {
                    ClientConnected(this, new ClientConnectedEventArgs(newClient));
                }
                else
                {
                    try
                    {
                        newClient.Close();
                    }
                    catch (SocketException e)
                    {
                        ExceptionHandler.LogException(e, false);
                    }
                }
            }
            finally
            {
                if (acquireLock)
                {
                    Monitor.Exit(this);
                }
            }
        }

        public event EventHandler<ClientConnectedEventArgs> ClientConnected;

        public void Stop()
        {
            lock (this)
            {
                if (m_running)
                {
                    m_running = false;
                    if (m_listener != null)
                    {
                        m_listener.Stop();
                        m_listener = null;
                    }
                }
            }
        }
    }

    public class ClientConnectedEventArgs : EventArgs
    {
        private TcpClient m_client;

        public ClientConnectedEventArgs(TcpClient client)
        {
            m_client = client;
        }

        public TcpClient TcpClient
        {
            get { return m_client; }
        }
    }

    public class IGBTcpClient
    {
        public IGBTcpClient(TcpClient client)
        {
            m_client = client;
        }

        public void Start()
        {
            lock (this)
            {
                m_running = true;
                m_stream = m_client.GetStream();
                m_buffer = new byte[m_bufferSize];
                BeginRead(false);
            }
        }

        private int m_bufferSize = 4096;

        public int BufferSize
        {
            get { return m_bufferSize; }
            set { m_bufferSize = value; }
        }

        private byte[] m_buffer;

        private void BeginRead(bool acquireLock)
        {
            if (acquireLock)
            {
                Monitor.Enter(this);
            }
            try
            {
                IAsyncResult ar;
                do
                {
                    ar = null;
                    if (m_running)
                    {
                        ar = m_stream.BeginRead(m_buffer, 0, m_buffer.Length, new AsyncCallback(EndRead), null);
                    }
                } while (ar != null && ar.CompletedSynchronously);
            }
            finally
            {
                if (acquireLock)
                {
                    Monitor.Exit(this);
                }
            }
        }

        private void EndRead(IAsyncResult ar)
        {
            try
            {
                int bytesRead = m_stream.EndRead(ar);
                if (bytesRead <= 0)
                {
                    Close();
                }
                else
                {
                    OnDataRead(m_buffer, 0, bytesRead);
                    if (!ar.CompletedSynchronously)
                    {
                        BeginRead(true);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                Close();
            }
        }

        private void OnDataRead(byte[] buffer, int offset, int count)
        {
            if (DataRead != null)
            {
                DataRead(this, new IGBClientDataReadEventArgs(buffer, offset, count));
            }
        }

        public void Close()
        {
            if (m_running)
            {
                m_running = false;
                try
                {
                    m_client.Close();
                }
                catch (SocketException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
                OnClosed();
            }
        }

        private void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, new EventArgs());
            }
        }

        public event EventHandler<IGBClientDataReadEventArgs> DataRead;
        public event EventHandler<EventArgs> Closed;

        private bool m_running = false;
        private TcpClient m_client;
        private NetworkStream m_stream;

        public void Write(string str)
        {
            byte[] outbuf = Encoding.UTF8.GetBytes(str);
            m_stream.Write(outbuf, 0, outbuf.Length);
        }
    }

    public class IGBClientDataReadEventArgs : EventArgs
    {
        private byte[] m_buffer;
        private int m_offset;
        private int m_count;

        public IGBClientDataReadEventArgs(byte[] buffer, int offset, int count)
        {
            m_buffer = buffer;
            m_offset = offset;
            m_count = count;
        }

        public byte[] Buffer
        {
            get { return m_buffer; }
        }

        public int Offset
        {
            get { return m_offset; }
        }

        public int Count
        {
            get { return m_count; }
        }
    }
}
