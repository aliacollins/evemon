using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace EVEMon.Common
{
    public class EveSession
    {
        #region Static Members
        private static Dictionary<string, WeakReference<EveSession>> m_sessions =
            new Dictionary<string, WeakReference<EveSession>>();

        public static EveSession GetSession(string username, string password)
        {
            string junk;
            return GetSession(username, password, out junk);
        }

        public static EveSession GetSession(string username, string password, out string errMessage)
        {
            lock (m_sessions)
            {
                errMessage = String.Empty;

                string hkey = username + ":" + password;
                EveSession result = null;
                if (m_sessions.ContainsKey(hkey))
                {
                    result = m_sessions[hkey].Target;
                }
                if (result == null)
                {
                    try
                    {
                        EveSession s = new EveSession(username, password);
                        m_sessions[hkey] = new WeakReference<EveSession>(s);
                        result = s;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex, true);
                        errMessage = ex.Message;
                    }
                }
                return result;
            }
        }

        public static void GetCharacterImageAsync(int charId, GetImageCallback callback)
        {
            // t20 from CCP has broken the 512 sized image
            //GetImageAsync("http://img.eve.is/serv.asp?s=512&c=" + charId.ToString(), false, callback);
            GetImageAsync("http://img.eve.is/serv.asp?s=256&c=" + charId.ToString(), false, callback);
        }

        public static string ImageCacheDirectory
        {
            get
            {
                string cacheDir = Settings.EveMonDataDir + "\\cache\\images";
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
                return cacheDir;
            }
        }

        public static void GetImageAsync(string url, bool useCache, GetImageCallback callback)
        {
            if (useCache)
            {
                string cacheFileName = ImageCacheDirectory + "\\" + GetCacheName(url);
                if (File.Exists(cacheFileName))
                {
                    try
                    {
                        FileStream fs = new FileStream(cacheFileName, FileMode.Open);
                        Image i = Image.FromStream(fs, true);
                        fs.Close();
                        fs.Dispose();

                        callback(null, i);
                        return;
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.LogException(e, false);
                    }
                }
                GetImageCallback origCallback = callback;
                callback = new GetImageCallback(delegate(EveSession s, Image i)
                                                    {
                                                        if (i != null)
                                                        {
                                                            AddImageToCache(url, i);
                                                        }
                                                        origCallback(s, i);
                                                    });
            }
            HttpWebRequest wr = EVEMonWebRequest.GetWebRequest(url);
            Pair<HttpWebRequest, GetImageCallback> p =
                new Pair<HttpWebRequest, GetImageCallback>(wr, callback);
            wr.BeginGetResponse(new AsyncCallback(GotImage), p);
        }

        private static void GotImage(IAsyncResult ar)
        {
            Pair<HttpWebRequest, GetImageCallback> p = ar.AsyncState as Pair<HttpWebRequest, GetImageCallback>;
            HttpWebRequest wr = p.A;
            GetImageCallback callback = p.B;
            try
            {
                HttpWebResponse resp = (HttpWebResponse) wr.EndGetResponse(ar);
                int contentLength = Convert.ToInt32(resp.ContentLength);
                int bytesToGo = contentLength;
                byte[] data = new byte[bytesToGo];
                using (Stream s = resp.GetResponseStream())
                {
                    while (bytesToGo > 0)
                    {
                        int bytesRead = s.Read(data, contentLength - bytesToGo, bytesToGo);
                        bytesToGo -= bytesRead;
                    }
                }
                MemoryStream ms = new MemoryStream(data);
                Image i = Image.FromStream(ms, true, true);
                callback(null, i);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                callback(null, null);
            }
        }

        private static void AddImageToCache(string url, Image i)
        {
            string cacheName = GetCacheName(url);
            using (StreamWriter sw = new StreamWriter(ImageCacheDirectory + "\\file.map", true))
            {
                sw.WriteLine(String.Format("{0} {1}", cacheName, url));
                sw.Close();
                //sw.Dispose();
            }
            string fn = ImageCacheDirectory + "\\" + cacheName;
            try
            {
                FileStream fs = new FileStream(fn, FileMode.Create);
                i.Save(fs, ImageFormat.Png);
                fs.Close();
                //fs.Dispose();
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }
        }

        private static string GetCacheName(string url)
        {
            Match extensionMatch = Regex.Match(url, @"([^\.]+)$");
            string ext = String.Empty;
            if (extensionMatch.Success)
            {
                ext = "." + extensionMatch.Groups[1];
            }
            byte[] hash = MD5.Create().ComputeHash(
                Encoding.UTF8.GetBytes(url));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(String.Format("{0:x2}", hash[i]));
            }
            sb.Append(ext);
            return sb.ToString();
        }
        #endregion

        #region Nonstatic Members
        private string m_username;
        private string m_password;

        private EveSession(string username, string password)
        {
            m_username = username;
            m_password = password;
            try
            {
                ReLogin();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("ReLogin() issue: " + ex.Message + "\nPlease confirm that the \"My Character\" page is functional on the eve-online website as that is where you are trying to access!");
            }
            try
            {
                GetCharacterList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("GetCharacterList() issue: " + ex.Message);
            }
        }

        private List<Pair<string, int>> m_storedCharacterList = null;

        public List<Pair<string, int>> GetCharacterListUncached()
        {
            m_storedCharacterList = null;
            return GetCharacterList();
        }

        public List<Pair<string, int>> GetCharacterList()
        {
            if (m_storedCharacterList != null)
            {
                return m_storedCharacterList;
            }

            string stxt = GetSessionUrlText("http://myeve.eve-online.com/character/xml.asp");
            if (String.IsNullOrEmpty(stxt))
            {
                throw new ApplicationException("Could not retrieve character list.");
            }

            List<Pair<string, int>> nl = new List<Pair<string, int>>();

            // catch System.Xml.XmlException (see trac ticket 539)
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(stxt);
                foreach (XmlElement el in xdoc.SelectNodes("/charactersheet/characters/character"))
                {
                    nl.Add(new Pair<string, int>(el.GetAttribute("name"),
                                                 Convert.ToInt32(el.GetAttribute("characterID"))));
                }
            }
            catch (System.Xml.XmlException xe)
            {
                throw new System.Xml.XmlException("XML is " + stxt, xe);
            }
            m_storedCharacterList = nl;
            return m_storedCharacterList;
        }

        private string GetSessionUrlText(string url)
        {
            bool needLogin = false;
            int maxTries = 2;

            while (maxTries > 0)
            {
                maxTries--;
                if (needLogin)
                {
                    ReLogin();
                }
                needLogin = true;

                string myurl = url;
                if (!String.IsNullOrEmpty(m_requestSid))
                {
                    if (myurl.Contains("?"))
                    {
                        myurl = myurl + "&sid=" + m_requestSid;
                    }
                    else
                    {
                        myurl = myurl + "?sid=" + m_requestSid;
                    }
                    m_requestSid = null;
                }

                HttpWebResponse resp = null;
                WebRequestState wrs = new WebRequestState();
                wrs.CookieContainer = m_cookies;
                wrs.AllowRedirects = false;
                try
                {
                    string txt = EVEMonWebRequest.GetUrlString(myurl, wrs, out resp);
                    if (wrs.RequestError == RequestError.None && resp.StatusCode == HttpStatusCode.OK)
                    {
                        return txt;
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }
            return String.Empty;
        }

        public int GetCharacterId(string charName)
        {
            foreach (Pair<string, int> p in m_storedCharacterList)
            {
                if (p.A == charName)
                {
                    return p.B;
                }
            }
            return -1;
        }

        /* This is never actually used, so it's getting commented out for now.
        public void GetCharacterInfoAsync(int charId, GetCharacterInfoCallback callback)
        {
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 SerializableCharacterInfo sci = GetCharacterInfo(charId);
                                                 callback(null, sci);
                                             });
        }*/

        private SerializableSkillTrainingInfo GetTrainingSkillInfo(int charId)
        {
            try
            {
                string stxt;
                /*
                int testcharid = ;
                string testxmlfilepath = "";
                if (charId != testcharid)*/
                stxt = GetSessionUrlText("http://myeve.eve-online.com/xml/skilltraining.asp?characterID=" +
                         charId.ToString());
                /*else
                {
                    FileStream fs = new FileStream(testxmlfilepath + "test.xml", FileMode.Open);
                    StreamReader reader = new StreamReader(fs);
                    stxt = reader.ReadToEnd();
                    reader.Dispose();
                    fs.Dispose();
                }*/
                if (String.IsNullOrEmpty(stxt))
                {
                    return null;
                }
                XmlDocument sdoc = new XmlDocument();
                try
                {
                    sdoc.LoadXml(stxt);
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e, true);
                    return null;
                }
                SerializableSkillTrainingInfo ssti = null;
                if (sdoc != null)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(SerializableSkillTrainingInfo));
                    XmlElement charRoot = sdoc.DocumentElement;

                    int test = Convert.ToInt32(charRoot.GetAttribute("characterID"));

                    using (XmlNodeReader xnr = new XmlNodeReader(charRoot))
                    {
                        ssti = (SerializableSkillTrainingInfo)xs.Deserialize(xnr);
                        // This set must only be done at deserialisation when we can compare current time agaisnt machine time
                        ssti.TQOffset = ((TimeSpan)(ssti.GetDateTimeAtUpdate.ToLocalTime().Subtract(DateTime.Now))).TotalMilliseconds;
                    }
                }
                return ssti;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            }
        }

        /// <summary>
        /// Gets the character info from the myeve website and stores it in the local cache.
        /// </summary>
        /// <param name="charId">The char id.</param>
        /// <returns>The SerializableCharacterInfo, fully populated</returns>
        private SerializableCharacterInfo GetCharacterInfo(int charId)
        {
            try
            {
                string xtxt =
                    GetSessionUrlText("http://myeve.eve-online.com/character/xml.asp?characterID=" + charId.ToString());
                if (String.IsNullOrEmpty(xtxt))
                {
                    return null;
                }
                XmlDocument xdoc = new XmlDocument();
                try
                {
                    xdoc.LoadXml(xtxt);
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e, true);
                    return null;
                }
                int timeLeftInCache = DEFAULT_RETRY_INTERVAL;
                SerializableCharacterInfo sci = ProcessCharacterXml(xdoc, charId, out timeLeftInCache);
                sci.TimeLeftInCache = timeLeftInCache;

                //save the xml in the character cache
                LocalXmlCache.Instance.Save(xdoc);
                
                return sci;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            }
        }

        public void UpdateIneveAsync(CharacterInfo info)
        {
            
            if (info != null)
            {
                ThreadPool.QueueUserWorkItem(UpdateIneve, info.Name);
            }
        }

        /// <summary>
        /// Uploads the character to ineve.  Relies on the local xml cache.  Should only be called asynchronously.
        /// </summary>
        /// <param name="charName">Name of the char as an object.</param>
        public void UpdateIneve(object charName)
        {
            lock (LocalXmlCache.Instance)
            {
                string character = charName as string;
                WebClient client = new WebClient();
                byte[] bytes = null;
                try
                {
                    bytes = client.UploadFile("http://ineve.net/skills/evemon_upload.php", LocalXmlCache.Instance[character].FullName);

                }
                catch (WebException)
                {
                    //just fail and trust that we'll try again next time.
                    return;
                }
                    string response = Encoding.UTF8.GetString(bytes);
            }            
            
            /*string encoded = string.Empty;
            lock (LocalXmlCache.Instance)
            {
                FileInfo charFile = LocalXmlCache.Instance[charName as string];
                FileStream fs = new FileStream(charFile.FullName, FileMode.Open);
                StreamReader reader = new StreamReader(fs);
                string charXml = reader.ReadToEnd();
                reader.Dispose();
                fs.Dispose();
                encoded = HttpUtility.UrlEncode(charXml);
            }


            string postData = "charxml=\"" + encoded;// +HttpUtility.UrlEncode("\"&login=\"Anders Chydenius&password=\"password\"");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://ineve.net/skills/evemon_upload.php");
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "EVEMon/" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();*/
            
        }

        private SerializableCharacterInfo ProcessCharacterXml(XmlDocument xdoc, int charId, out int cacheExpires)
        {
            XmlSerializer xs = new XmlSerializer(typeof (SerializableCharacterInfo));
            XmlElement charRoot =
                xdoc.DocumentElement.SelectSingleNode("/charactersheet/characters/character[@characterID='" +
                                                      charId.ToString() + "']") as XmlElement;

            cacheExpires = Convert.ToInt32(charRoot.GetAttribute("timeLeftInCache"));

            using (XmlNodeReader xnr = new XmlNodeReader(charRoot))
            {
                return (SerializableCharacterInfo) xs.Deserialize(xnr);
            }
        }

        /// <summary>
        /// Use this function at your peril!!
        /// If we do things this way CCP WILL block that version of EVEMon!
        /// Not Only that but it uses obsolete code, so Ner!
        /// </summary>
        /// <param name="htmld"></param>
        /// <returns></returns>
        /*private SerializableSkillInTraining ProcessSkilltreeHtml(string htmld)
        {
            SerializableSkillInTraining sit = null;
            int cti = htmld.IndexOf("Currently training to: ");
            if (cti != -1)
            {
                sit = new SerializableSkillInTraining();
                string bsubstr = ReverseString(htmld.Substring(cti - 400, 400));
                string s1 = Regex.Match(bsubstr, @"knaR>i< / (.+?)>""xp11:ezis-tnof").Groups[1].Value;
                sit.SkillName = ReverseString(s1);
                string fsubstr = htmld.Substring(cti, 800);
                sit.TrainingToLevel =
                    Convert.ToInt32(
                        Regex.Match(fsubstr, @"Currently training to: <\/font><strong>level (\d) </st").Groups[1].Value);
                string timeLeft = Regex.Match(fsubstr, @"Time left: <\/font><strong>(.+?)<\/strong>").Groups[1].Value;
                sit.EstimatedCompletion = DateTime.Now + ConvertTimeStringToTimeSpan(timeLeft);
                sit.CurrentPoints =
                    Convert.ToInt32(
                        Regex.Match(fsubstr, @"SP done: </font><strong>(\d+) of \d+</strong>").Groups[1].Value);
                sit.NeededPoints =
                    Convert.ToInt32(
                        Regex.Match(fsubstr, @"SP done: </font><strong>\d+ of (\d+)</strong>").Groups[1].Value);
            }
            else
            {
                sit = null;
            }
            return sit;
        }*/

        private TimeSpan ConvertTimeStringToTimeSpan(string timeLeft)
        {
            TimeSpan result = new TimeSpan();
            if (timeLeft.Contains("second"))
            {
                result += TimeSpan.FromSeconds(
                    Convert.ToInt32(Regex.Match(timeLeft, @"(\d+) seconds?").Groups[1].Value));
            }
            if (timeLeft.Contains("minute"))
            {
                result += TimeSpan.FromMinutes(
                    Convert.ToInt32(Regex.Match(timeLeft, @"(\d+) minutes?").Groups[1].Value));
            }
            if (timeLeft.Contains("hour"))
            {
                result += TimeSpan.FromHours(
                    Convert.ToInt32(Regex.Match(timeLeft, @"(\d+) hours?").Groups[1].Value));
            }
            if (timeLeft.Contains("day"))
            {
                result += TimeSpan.FromDays(
                    Convert.ToInt32(Regex.Match(timeLeft, @"(\d+) days?").Groups[1].Value));
            }
            return result;
        }

        private string ReverseString(string p)
        {
            char[] ca = new char[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                ca[p.Length - i - 1] = p[i];
            }
            return new String(ca);
        }

        private static Thread m_mainThread = null;

        public static Thread MainThread
        {
            get { return m_mainThread; }
            set { m_mainThread = value; }
        }

        private string m_requestSid = null;
        private CookieContainer m_cookies = null;

        public void ReLogin()
        {
            if (m_cookies == null)
            {
                m_cookies = new CookieContainer();
            }
            WebRequestState wrs = new WebRequestState();
            wrs.CookieContainer = m_cookies;
            wrs.AllowRedirects = false;
            HttpWebResponse resp = null;
            string s = EVEMonWebRequest.GetUrlString(
                "https://myeve.eve-online.com/login.asp?username=" +
                HttpUtility.UrlEncode(m_username, Encoding.GetEncoding("iso-8859-1")) +
                "&password=" +
                HttpUtility.UrlEncode(m_password, Encoding.GetEncoding("iso-8859-1")) +
                "&login=Login&Check=OK&r=&t=/ingameboard.asp&remember=1", wrs, out resp);
            string loc = null;
            if (resp != null)
            {
                loc = resp.Headers[HttpResponseHeader.Location];
            }
            else
            {
                throw new ApplicationException(wrs.WebException.Message);
            }
            Match sidm = null;
            if (!String.IsNullOrEmpty(loc))
            {
                sidm = Regex.Match(loc, @"sid=(\d+)");
            }
            if (sidm != null && sidm.Success)
            {
                string sid = sidm.Groups[1].Value;
                m_requestSid = sid;

                HttpWebResponse resp1 = null;
                EVEMonWebRequest.GetUrlString(
                    "http://myeve.eve-online.com/login.asp", wrs, out resp1);

                return;
            }
            else if (s.Contains("Login credentials incorrect"))
            {
                throw new ApplicationException(
                    "The username/password supplied is invalid. Please ensure it is entered correctly.");
            }
            else
            {
                throw new ApplicationException("Failed to get login SID");
            }
        }

        private class UpdateGCIArgs
        {
            public CharacterInfo GrandCharacterInfo;
            public Control InvokeControl;
            public UpdateGrandCharacterInfoCallback UpdateGrandCharacterInfoCallback;
        }

        public void UpdateGrandCharacterInfoAsync(CharacterInfo grandCharacterInfo, Control invokeControl,
                                                  UpdateGrandCharacterInfoCallback callback)
        {
            UpdateGCIArgs xx = new UpdateGCIArgs();
            xx.GrandCharacterInfo = grandCharacterInfo;
            xx.InvokeControl = invokeControl;
            xx.UpdateGrandCharacterInfoCallback = callback;
#if DEBUG_SINGLETHREAD
            UpdateGrandCharacterInfoAsyncCaller(xx);
#else
            ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateGrandCharacterInfoAsyncCaller), xx);
#endif
        }

        private void UpdateGrandCharacterInfoAsyncCaller(object state)
        {
            UpdateGCIArgs args = (UpdateGCIArgs) state;
            GC.Collect();
            int timeLeftInCache = this.UpdateGrandCharacterInfo(args.GrandCharacterInfo, args.InvokeControl);
            args.UpdateGrandCharacterInfoCallback(null, timeLeftInCache);
        }

        private class UpdateSTArgs
        {
            public CharacterInfo GrandCharacterInfo;
            public Control InvokeControl;
            public UpdateTrainingSkillInfoCallback UpdateTrainingSkillInfoCallback;
        }

        public void UpdateSkillTrainingInfoAsync(CharacterInfo grandCharacterInfo, Control invokeControl,
                                                  UpdateTrainingSkillInfoCallback callback)
        {
            UpdateSTArgs xx = new UpdateSTArgs();
            xx.GrandCharacterInfo = grandCharacterInfo;
            xx.InvokeControl = invokeControl;
            xx.UpdateTrainingSkillInfoCallback = callback;
#if DEBUG_SINGLETHREAD
            UpdateSkillTrainingInfoAsyncCaller(xx);
#else
            ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateSkillTrainingInfoAsyncCaller), xx);
#endif
        }

        private void UpdateSkillTrainingInfoAsyncCaller(object state)
        {
            UpdateSTArgs args = (UpdateSTArgs)state;
            GC.Collect();
            int timeToNextUpdate = this.UpdateSkillTrainingInfo(args.GrandCharacterInfo, args.InvokeControl);
            args.UpdateTrainingSkillInfoCallback(null, timeToNextUpdate);
        }

        // Now default is set to 30 minutes instead of 5 minutes for the retry interval
        // to be more polite to the server.
        private const int DEFAULT_RETRY_INTERVAL = 30*60*1000;
        private Random autoRand;
        private Object mutexLock = new Object();

        public int UpdateGrandCharacterInfo(CharacterInfo grandCharacterInfo, Control invokeControl)
        {
            lock (mutexLock)
            {
                SerializableSkillTrainingInfo temp = grandCharacterInfo.SerialSIT;
                SerializableCharacterInfo sci = GetCharacterInfo(grandCharacterInfo.CharacterId);

                if (sci == null)
                {
                    autoRand = new Random();
                    grandCharacterInfo.DownloadFailed += 1;
                    double offset = (DEFAULT_RETRY_INTERVAL * grandCharacterInfo.DownloadFailed) + (DEFAULT_RETRY_INTERVAL * autoRand.NextDouble());
                    return (int)offset;
                }
                else
                {
                    grandCharacterInfo.DownloadFailed = 0;
                    sci.TrainingSkillInfo = temp;
                }
                sci.XMLExpires = DateTime.Now.Add(TimeSpan.FromMilliseconds(sci.TimeLeftInCache));

                if (((TimeSpan)(sci.XMLExpires.Subtract(grandCharacterInfo.XMLExpires))).Duration() > new TimeSpan(0, 3, 30))
                {
                    invokeControl.Invoke(new MethodInvoker(delegate
                                                               {
                                                                   grandCharacterInfo.AssignFromSerializableCharacterInfo(sci);
                                                               }));
                }
                return sci.TimeLeftInCache;
            }
        }

        public int UpdateSkillTrainingInfo(CharacterInfo grandCharacterInfo, Control invokeControl)
        {
            lock (mutexLock)
            {
                SerializableSkillTrainingInfo ssti = GetTrainingSkillInfo(grandCharacterInfo.CharacterId);
                if (ssti == null)
                {
                    return 0;
                }
                else
                {
                    string error = ssti.Error;
                    if (error == "characterID does not belong to you.")
                        throw new Exception(error); // really should throw an exception here... so now we do!
                    if (error == "You are trying too fast.")
                    {
                        invokeControl.Invoke(new MethodInvoker(delegate
                                                                   {
                                                                       grandCharacterInfo.AssignFromSerializableSkillTrainingInfo(grandCharacterInfo.SerialSIT);
                                                                   }));
                        return (1000 * ssti.TimerToNextUpdate); // should be setting a timer to retry here.... and now we do thanks to where this function is called.
                    }
                }

                invokeControl.Invoke(new MethodInvoker(delegate
                                                           {
                                                               grandCharacterInfo.AssignFromSerializableSkillTrainingInfo(ssti);
                                                           }));
                return (1000 * ssti.TimerToNextUpdate);
            }
        }

        #endregion
    }
    [XmlRoot("pair")]
    public class Pair<TypeA, TypeB>
    {
        private TypeA m_a;
        private TypeB m_b;

        public TypeA A
        {
            get { return m_a; }
            set { m_a = value; }
        }

        public TypeB B
        {
            get { return m_b; }
            set { m_b = value; }
        }

        public Pair()
        {
        }

        public Pair(TypeA a, TypeB b)
        {
            m_a = a;
            m_b = b;
        }
    }

    public delegate void UpdateGrandCharacterInfoCallback(EveSession sender, int timeLeftInCache);

    public delegate void UpdateTrainingSkillInfoCallback(EveSession sender, int timeRetryIn);

    public delegate void GetImageCallback(EveSession sender, Image i);

    // this is never actually used, so I'm commenting it out for now.
    //public delegate void GetCharacterInfoCallback(EveSession sender, SerializableCharacterInfo ci);

    [XmlRoot("attributes")]
    public class EveAttributes
    {
        private SerializableCharacterInfo m_owner;

        internal void SetOwner(SerializableCharacterInfo ci)
        {
            m_owner = ci;
        }

        private int[] m_values = new int[5] { 0, 0, 0, 0, 0 };

        [XmlElement("intelligence")]
        public int BaseIntelligence
        {
            get { return m_values[(int)EveAttribute.Intelligence]; }
            set { m_values[(int)EveAttribute.Intelligence] = value; }
        }

        [XmlElement("charisma")]
        public int BaseCharisma
        {
            get { return m_values[(int)EveAttribute.Charisma]; }
            set { m_values[(int)EveAttribute.Charisma] = value; }
        }

        [XmlElement("perception")]
        public int BasePerception
        {
            get { return m_values[(int)EveAttribute.Perception]; }
            set { m_values[(int)EveAttribute.Perception] = value; }
        }

        [XmlElement("memory")]
        public int BaseMemory
        {
            get { return m_values[(int)EveAttribute.Memory]; }
            set { m_values[(int)EveAttribute.Memory] = value; }
        }

        [XmlElement("willpower")]
        public int BaseWillpower
        {
            get { return m_values[(int)EveAttribute.Willpower]; }
            set { m_values[(int)EveAttribute.Willpower] = value; }
        }

        [XmlIgnore]
        public double AdjustedIntelligence
        {
            get { return GetAdjustedAttribute(EveAttribute.Intelligence); }
        }

        [XmlIgnore]
        public double AdjustedCharisma
        {
            get { return GetAdjustedAttribute(EveAttribute.Charisma); }
        }

        [XmlIgnore]
        public double AdjustedPerception
        {
            get { return GetAdjustedAttribute(EveAttribute.Perception); }
        }

        [XmlIgnore]
        public double AdjustedMemory
        {
            get { return GetAdjustedAttribute(EveAttribute.Memory); }
        }

        [XmlIgnore]
        public double AdjustedWillpower
        {
            get { return GetAdjustedAttribute(EveAttribute.Willpower); }
        }

        [XmlElement("adjustedIntelligence")]
        public string _adjustedIntelligence
        {
            get { return this.AdjustedIntelligence.ToString("#.00"); }
        }

        [XmlElement("adjustedCharisma")]
        public string _adjustedCharisma
        {
            get { return this.AdjustedCharisma.ToString("#.00"); }
        }

        [XmlElement("adjustedPerception")]
        public string _adjustedPerception
        {
            get { return this.AdjustedPerception.ToString("#.00"); }
        }

        [XmlElement("adjustedMemory")]
        public string _adjustedMemory
        {
            get { return this.AdjustedMemory.ToString("#.00"); }
        }

        [XmlElement("adjustedWillpower")]
        public string _adjustedWillpower
        {
            get { return this.AdjustedWillpower.ToString("#.00"); }
        }

        public double GetAttributeAdjustment(EveAttribute eveAttribute, SerializableEveAttributeAdjustment adjustment)
        {
            double result = 0.0;
            double learningBonus = 1.0;
            if ((adjustment & SerializableEveAttributeAdjustment.Base) != 0)
            {
                result += m_values[(int)eveAttribute];
            }
            if ((adjustment & SerializableEveAttributeAdjustment.Implants) != 0)
            {
                foreach (SerializableEveAttributeBonus eab in m_owner.AttributeBonuses.Bonuses)
                {
                    if (eab.EveAttribute == eveAttribute)
                    {
                        result += eab.Amount;
                    }
                }
            }
            if (((adjustment & SerializableEveAttributeAdjustment.Skills) != 0) ||
                ((adjustment & SerializableEveAttributeAdjustment.Learning) != 0))
            {
                foreach (SerializableSkillGroup sg in m_owner.SkillGroups)
                {
                    if (sg.Name == "Learning")
                    {
                        foreach (SerializableSkill s in sg.Skills)
                        {
                            if ((adjustment & SerializableEveAttributeAdjustment.Skills) != 0)
                            {
                                switch (eveAttribute)
                                {
                                    case EveAttribute.Intelligence:
                                        if (s.Name == "Analytical Mind" || s.Name == "Logic")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Charisma:
                                        if (s.Name == "Empathy" || s.Name == "Presence")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Memory:
                                        if (s.Name == "Instant Recall" || s.Name == "Eidetic Memory")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Willpower:
                                        if (s.Name == "Iron Will" || s.Name == "Focus")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Perception:
                                        if (s.Name == "Spatial Awareness" || s.Name == "Clarity")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                }
                            }
                            if (s.Name == "Learning")
                            {
                                learningBonus = 1.0 + (0.02 * s.Level);
                            }
                        }
                    }
                }
            }
            if ((adjustment & SerializableEveAttributeAdjustment.Learning) != 0)
            {
                result = result * learningBonus;
            }
            return result;
        }

        private double GetAdjustedAttribute(EveAttribute eveAttribute)
        {
            return GetAttributeAdjustment(eveAttribute, SerializableEveAttributeAdjustment.AllWithLearning);
        }
    }

    public enum NetworkLogEventType
    {
        BeginGetUrl,
        Redirected,
        ParsedRedirect,
        GotUrlSuccess,
        GotUrlFailure
    }

    public class NetworkLogEventArgs : EventArgs
    {
        internal NetworkLogEventArgs()
        {
        }

        private NetworkLogEventType m_type;

        public NetworkLogEventType NetworkLogEventType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private string m_url;

        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }

        private string m_referer;

        public string Referer
        {
            get { return m_referer; }
            set { m_referer = value; }
        }

        private CookieCollection m_cookies;

        [XmlIgnore]
        public CookieCollection Cookies
        {
            get { return m_cookies; }
            set { m_cookies = value; }
        }

        public List<string> CookieList
        {
            get
            {
                if (m_cookies == null)
                {
                    return null;
                }
                List<string> cook = new List<string>();
                foreach (Cookie c in m_cookies)
                {
                    cook.Add(c.ToString());
                }
                return cook;
            }
        }

        private Exception m_exception;

        [XmlIgnore]
        public Exception Exception
        {
            get { return m_exception; }
            set { m_exception = value; }
        }

        public string ExceptionText
        {
            get
            {
                if (m_exception != null)
                {
                    return m_exception.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        private string m_redirectTo;

        public string RedirectTo
        {
            get { return m_redirectTo; }
            set { m_redirectTo = value; }
        }

        private HttpStatusCode m_statusCode = HttpStatusCode.OK;

        public HttpStatusCode StatusCode
        {
            get { return m_statusCode; }
            set { m_statusCode = value; }
        }

        private string m_data;

        public string Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
    }

    public enum EveAttribute
    {
        [XmlEnum("perception")]
        Perception,
        [XmlEnum("memory")]
        Memory,
        [XmlEnum("willpower")]
        Willpower,
        [XmlEnum("intelligence")]
        Intelligence,
        [XmlEnum("charisma")]
        Charisma,
        [XmlEnum("none")]
        None
    }
}
