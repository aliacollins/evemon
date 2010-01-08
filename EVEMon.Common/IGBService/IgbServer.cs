using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using EVEMon.Common;
using EVEMon.Common.SettingsObjects;
using EVEMon.Common.Threading;

namespace EVEMon.Common.IgbService
{
    /// <summary>
    /// Incomplete HTTP 1.0 web server, to serve out simple pages to
    /// the in-game browser to allow in-game interaction with EVEMon.
    /// </summary>
    public class IgbServer
    {
        private int m_port;
        private bool m_isPublic = false;
        private bool m_running = false;
        private IgbTcpListener m_listener;
        private Dictionary<IgbTcpClient, byte[]> m_clients = new Dictionary<IgbTcpClient, byte[]>();

        #region Construction, Start, Stop and Reset
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isPublic">Is publicly available</param>
        /// <param name="port">Initial TCP/IP port</param>
        public IgbServer(bool isPublic, int port)
        {
            m_port = port;
            m_isPublic = isPublic;
            CreateListener();
        }

        /// <summary>
        /// Re-initilize the IGB web server service
        /// </summary>
        /// <param name="isPublic">Is publicly available</param>
        /// <param name="port">New TCP/IP port</param>
        public void Reset(bool isPublic, int port)
        {
            m_isPublic = isPublic;
            m_port = port;
            Stop();
            m_listener = null;

            CreateListener();
        }

        /// <summary>
        /// Creates the listener bound to an address and port, wires up the events
        /// </summary>
        private void CreateListener()
        {
            m_listener = new IgbTcpListener(new IPEndPoint(AddressToBind(), m_port));
            m_listener.ClientConnected += new EventHandler<ClientConnectedEventArgs>(OnClientConnected);
        }

        /// <summary>
        /// Cacluates the address to bind to
        /// </summary>
        /// <returns>IPAddress.Any if public, IPAddress.Loopback if not public</returns>
        private IPAddress AddressToBind()
        {
            return m_isPublic ? IPAddress.Any : IPAddress.Loopback;
        }

        /// <summary>
        /// Starts the IGB Service if not running
        /// </summary>
        public void Start()
        {
            if (!m_running)
            {
                m_running = true;
                m_listener.Start();
            }
        }

        /// <summary>
        /// Stops the IGB Service if running
        /// </summary>
        public void Stop()
        {
            if (m_running)
            {
                m_running = false;
                m_listener.Stop();
            }
        }
        #endregion

        #region Client Event Handlers
        
        /// <summary>
        /// Event triggered on client connection
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Argments</param>
        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            IgbTcpClient cli = new IgbTcpClient(e.TcpClient);
            cli.DataRead += new EventHandler<IgbClientDataReadEventArgs>(OnDataRead);
            cli.Closed += new EventHandler<EventArgs>(OnClosed);
            lock (m_clients)
            {
                m_clients.Add(cli, new byte[0]);
            }
            cli.Start();
        }

        /// <summary>
        /// Event triggered on data read
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataRead(object sender, IgbClientDataReadEventArgs e)
        {
            IgbTcpClient IgbSender = (IgbTcpClient) sender;
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

        /// <summary>
        /// Process the buffer and respond to the client.
        /// </summary>
        /// <param name="client">client to respond to</param>
        /// <param name="buffer">buffer to use</param>
        /// <param name="length">length of tail</param>
        private void TryProcessBuffer(IgbTcpClient client, byte[] buffer, int length)
        {
            // make sure the request is well formed
            if (!TailHasTwoNewLine(buffer, length)) return;

            Dictionary<string, string> headers = new Dictionary<string, string>();

            string requestUrl = ExtractHeaders(buffer, headers);

            SendOutputToClient(client, headers, requestUrl);

            client.Close();
        }

        /// <summary>
        /// Process request and send output to client
        /// </summary>
        /// <param name="client">client to send output to</param>
        /// <param name="headers">dictionary of headers</param>
        /// <param name="requestUrl">url to respond to</param>
        private void SendOutputToClient(IgbTcpClient client, Dictionary<string, string> headers, string requestUrl)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            {
                ProcessRequest(requestUrl, headers, sw);

                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                client.Write("HTTP/1.1 200 OK\n");
                client.Write("Server: EVEMon/1.0\n");
                client.Write("Content-Type: text/html; charset=utf-8\n");
                if (headers.ContainsKey("eve_trusted") && headers["eve_trusted"].ToLower() == "no")
                {
                    client.Write("eve.trustme: http://" + BuildHostAndPort(headers["host"]) + "/::EVEMon needs your pilot information.\n");
                }
                client.Write("Connection: close\n");
                client.Write("Content-Length: " + ms.Length.ToString() + "\n\n");
                using (StreamReader sr = new StreamReader(ms))
                {
                    client.Write(sr.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// Extract the headers from the buffer and ad them to a dictionary
        /// </summary>
        /// <param name="buffer">the buffer to extract headers from</param>
        /// <param name="headers">dictionary to add headers to</param>
        /// <returns></returns>
        private static string ExtractHeaders(byte[] buffer, Dictionary<string, string> headers)
        {
            string headerStr = Encoding.UTF8.GetString(buffer);

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
            return requestUrl;
        }

        /// <summary>
        /// Checks to see if the header contains two New Lines as per HTTP specification
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="tailLength"></param>
        /// <returns></returns>
        private static bool TailHasTwoNewLine(byte[] buffer, int length)
        {
            bool gotOne = false;
            for (int i = 0; i < length; i++)
            {
                if (buffer[buffer.Length - i - 1] == ((byte)'\n'))
                {
                    if (gotOne)
                    {
                        return true;
                    }
                    else
                    {
                        gotOne = true;
                    }
                }
                else if (buffer[buffer.Length - i - 1] != ((byte)'\r'))
                {
                    gotOne = false;
                }
            }

            return false;
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
            // (IGB shouldnt really do this but it is!
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

        /// <summary>
        /// Process the request
        /// </summary>
        /// <param name="requestUrl">URL of the request</param>
        /// <param name="headers">dictionary of headers</param>
        /// <param name="sw">stream writer to output to</param>
        private void ProcessRequest(string requestUrl, Dictionary<string, string> headers, StreamWriter sw)
        {
            if (!headers.ContainsKey("eve_trusted"))
            {
                sw.WriteLine("Please visit this site using the in-game browser.");
                return;
            }

            if (headers["eve_trusted"].ToLower() != "yes")
            {
                sw.WriteLine("not trusted");
                return;
            }

            Character ci = EveClient.MonitoredCharacters.FirstOrDefault(x => x.Name == headers["eve_charname"]);
            if (requestUrl.StartsWith("/plan/") || requestUrl.StartsWith("/shopping/") || requestUrl.StartsWith("/owned/"))
            {
                GeneratePlanOrShoppingOutput(requestUrl, sw, ci);
            }
            else if (requestUrl.StartsWith("/skills/bytime"))
            {
                GenerateSkillsByTimeOutput(headers, sw, ci);
            }
            else
            {
                GeneratePlanListOutput(headers, sw, ci);
            }
        }

        /// <summary>
        /// Output list of plans to a stream writer
        /// </summary>
        /// <param name="headers">headers for the request</param>
        /// <param name="sw">stream writer to output to</param>
        /// <param name="ci">character to use</param>
        private static void GeneratePlanListOutput(Dictionary<string, string> headers, StreamWriter sw, Character ci)
        {
            sw.WriteLine(
                String.Format("<html><head><title>Hi</title></head><body><h1>Hello, {0}</h1>",
                              headers["eve_charname"]));

            sw.WriteLine("<h2>Your Plans:</h2>");
            foreach (var plan in ci.Plans)
            {
                sw.WriteLine(String.Format("<a href=\"/plan/{0}\">{1}</a> (<a href=\"/shopping/{0}\">shopping list</a>)<br>",
                                           HttpUtility.UrlEncode(plan.Name),
                                           HttpUtility.HtmlEncode(plan.Name)));
            }

            sw.WriteLine("<br><h2>Your Skills:</h2>");
            sw.WriteLine("<a href=\"/skills/bytime\">By training time</a>");
            sw.WriteLine("</body></html>");
        }

        /// <summary>
        /// Generate a list of skills by time
        /// </summary>
        /// <param name="headers">headers for the request</param>
        /// <param name="sw">stream writer to output to</param>
        /// <param name="ci">character to use</param>
        private static void GenerateSkillsByTimeOutput(Dictionary<string, string> headers, StreamWriter sw, Character ci)
        {
            sw.WriteLine("<html><head><title>Skills</title></head><body>");
            sw.WriteLine("<h1>Skills: By training time</h1>");
            sw.WriteLine("<hr><a href=\"/\">Back</a></hr>");
            sw.WriteLine(string.Format("<p>Skills for {0}</p>", HttpUtility.HtmlEncode(headers["eve_charname"])));

            var allskills = ci.Skills.Where(x => x.IsPublic && x.Level < 5 && x.Level > 0);
            allskills = allskills.OrderBy(x => x.GetLeftTrainingTimeToNextLevel());

            sw.WriteLine("<table>");
            sw.Write("<tr><td colspan=\"2\" width=\"265\"><b>Skill</b></td>" +
                "<td width=\"100\"><b>Next Level</b></td><td><b>Training Time</b></td></tr>");

            int index = 0;
            foreach (Skill s in allskills)
            {
                index++;
                sw.Write("<tr>");

                sw.Write("<td width=\"15\">");
                sw.Write(string.Format("<b>{0}.</b>", index.ToString()));
                sw.Write("</td>");

                sw.Write("<td width=\"250\">");
                sw.Write(string.Format("<b><a href=\"\" onclick=\"CCPEVE.showInfo({0})\">{1}</a></b>", s.ID, s.Name));
                sw.Write("</td>");

                sw.Write("<td width=\"100\">");
                sw.Write(string.Format("<b>{0} -&gt; {1}</b>", s.RomanLevel, Skill.GetRomanForInt(s.Level + 1)));
                sw.Write("</td>");

                sw.Write("<td>");
                sw.Write(Skill.TimeSpanToDescriptiveText(s.GetLeftTrainingTimeToNextLevel(),
                                                    DescriptiveTextOptions.FullText |
                                                    DescriptiveTextOptions.IncludeCommas |
                                                    DescriptiveTextOptions.SpaceText));
                sw.Write("</td>");
                sw.Write("</tr>");
            }
            sw.WriteLine("</table>");

            sw.WriteLine("<hr><a href=\"/\">Back</a></hr>");
            sw.WriteLine("</body></html>");
        }

        /// <summary>
        /// Generate the plan or shopping list
        /// </summary>
        /// <param name="headers">headers for the request</param>
        /// <param name="sw">stream writer to output to</param>
        /// <param name="ci">character to use</param>
        private static void GeneratePlanOrShoppingOutput(string requestUrl, StreamWriter sw, Character ci)
        {
            var regex = new Regex(@"\/(owned\/(?'skillId'[^\/]+)\/(?'markOwned'[^\/]+)\/)?(?'requestType'shopping|plan)\/(?'planName'[^\/]+)(.*)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            var match = regex.Match(requestUrl);

            if (match.Success)
            {
                var requestType = match.Groups["requestType"].Value;
                var shopping = requestType.Equals("shopping", StringComparison.OrdinalIgnoreCase);
                var planName = HttpUtility.UrlDecode(match.Groups["planName"].Value);

                sw.WriteLine("<html><head><title>Plan</title></head><body>");
                int skillId;
                bool setAsOwned;
                if (match.Groups["skillId"].Success &&
                    match.Groups["markOwned"].Success &&
                    Int32.TryParse(match.Groups["skillId"].Value, out skillId) &&
                    Boolean.TryParse(match.Groups["markOwned"].Value, out setAsOwned))
                {
                    var skill = ci.Skills.FirstOrDefault(x => x.ID == skillId);
                    if (skill != null)
                    {
                        sw.WriteLine("<h2>Skillbook shopping result</h2>");
                        Dispatcher.Invoke(() => skill.IsOwned = setAsOwned);
                        if (skill.IsOwned)
                        {
                            sw.WriteLine("Congratulations, <a href=\"\" onclick=\"CCPEVE.showInfo({0})\">{1}</a> is marked as owned.", skill.ID, skill.Name);
                        }
                        else
                        {
                            sw.WriteLine("Sorry, <a href=\"\" onclick=\"CCPEVE.showInfo({0})\">{1}</a> is marked as not owned.", skill.ID, skill.Name);
                        }
                        sw.WriteLine("<hr/>");
                    }
                    else
                    {
                        // maybe we should do nothing in this case?
                        sw.WriteLine("skill with id '{0}' could not be found", skillId);
                    }
                }

                sw.WriteLine("<h2>Plan: {0}</h2>", HttpUtility.HtmlEncode(planName));
                sw.WriteLine("<hr/><a href=\"/\">Back</a><br/><br/>");

                Plan p = ci.Plans[planName];
                if (p == null)
                {
                    sw.WriteLine("non-existant plan name");
                }
                else
                {
                    PlanExportSettings x = new PlanExportSettings();
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
                    sw.Write(PlanExporter.ExportAsText(p, x, (builder, entry, settings) =>
                    {
                        if (settings.Markup != MarkupType.Html)
                            return;

                        // skill is known
                        if (entry.CharacterSkill.IsKnown || entry.Level != 1)
                            return;

                        builder.AppendFormat("<a href='/owned/{0}/{1}/{3}/{4}'>{2}</a>", entry.Skill.ID, !entry.CharacterSkill.IsOwned, !entry.CharacterSkill.IsOwned ? "Mark as owned" : "Mark as not owned", requestType, planName);
                    }));
                }
            }
            else
            {
                sw.WriteLine("invalid request");
            }

            sw.WriteLine("<hr/><a href=\"/\">Back</a>");
            sw.WriteLine("</body></html>");
        }

        /// <summary>
        /// Event triggered on connection close read
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, EventArgs e)
        {
            lock (m_clients)
            {
                m_clients.Remove((IgbTcpClient) sender);
            }
        }
        #endregion
    }
}
