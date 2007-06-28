//#define DEBUG_SINGLETHREAD
//#define USE_LOCALHOST
// (If setting DEBUG_SINGLE THREAD, also set it in CharacterMonitor.cs)
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
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Reflection;

namespace EVEMon.Common
{
    public class EveSession
    {
        private static Dictionary<string, WeakReference<EveSession>> m_sessions =
            new Dictionary<string, WeakReference<EveSession>>();

        public static EveSession GetSession(int userId, string apiKey)
        {
            string junk;
            return GetSession(userId, apiKey, out junk);
        }

        public static EveSession GetSession(int userId, string apiKey, out string errMessage)
        {
            lock (m_sessions)
            {
                errMessage = String.Empty;

                string hkey = userId + ":" + apiKey;
                EveSession result = null;
                if (m_sessions.ContainsKey(hkey))
                {
                    result = m_sessions[hkey].Target;
                }
                if (result == null)
                {
                    try
                    {
                        EveSession s = new EveSession(userId, apiKey);
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

        #region API stuff
#if USE_LOCALHOST
        private static string APIBASE = "http://localhost/eveapi";
        private static string m_ApiCharListUrl = "/account/Characters.xml";
        private static string m_ApiTrainingSkill = "/char/SkillIntraining.xml";
        private static string m_ApiCharacterSheet = "/char/CharacterSheet.xml";

#else
        private static string APIBASE = "http://api.eve-online.com";
        private static string m_ApiCharListUrl = "/account/Characters.xml.aspx";
        private static string m_ApiTrainingSkill = "/char/SkillIntraining.xml.aspx";
        private static string m_ApiCharacterSheet = "/char/CharacterSheet.xml.aspx";
#endif
        private string m_apiErrorMessage;
        private int m_apiErrorCode;

        public static string ApiKeyUrl
        {
            get { return "http://myeve.eve-online.com/api/default.asp"; }
        }

        private static XmlDocument GetCharList(string userId, string apiKey, out string errorMessage)
        {
            WebRequestState wrs = new WebRequestState();
            wrs.SetPost("userid=" + userId + "&apiKey=" + apiKey);
            errorMessage = string.Empty;
            return EVEMonWebRequest.LoadXml(APIBASE + m_ApiCharListUrl, wrs);
        }

        private XmlDocument GetTrainingSkill(int charId)
        {
            WebRequestState wrs = new WebRequestState();
            wrs.SetPost("userid=" + m_userId + "&apiKey=" + m_apiKey + "&characterID=" + Convert.ToString(charId));
            return EVEMonWebRequest.LoadXml(APIBASE + m_ApiTrainingSkill, wrs);
        }

        private XmlDocument GetCharacterSheet(int charId)
        {
            WebRequestState wrs = new WebRequestState();
            wrs.SetPost("userid=" + m_userId + "&apiKey=" + m_apiKey + "&characterID=" + Convert.ToString(charId));
            return EVEMonWebRequest.LoadXml(APIBASE + m_ApiCharacterSheet, wrs);
        }

        private void SetAPIError(XmlElement errorNode)
        {
            string text = errorNode.GetAttribute("code");
            if (text != null)
            {
                m_apiErrorCode = Int32.Parse(text);
            }
            m_apiErrorMessage = errorNode.InnerText;
        }

        public int ApiErrorCode
        {
            get { return m_apiErrorCode; }
            set { m_apiErrorCode = value; }
        }

        public string ApiErrorMessage
        {
            get
            {
                if (m_apiErrorCode > 0)
                {
                    return String.Format("{0}: {1}", m_apiErrorCode, m_apiErrorMessage);
                }
                else return null;
            }
        }
        #endregion

        #region Images
        public static void GetCharacterImageAsync(int charId, GetImageCallback callback)
        {
            GetImageAsync("http://img.eve.is/serv.asp?s=256&c=" + charId.ToString(), false, callback);
        }

        public static string ImageCacheDirectory
        {
            get
            {
                string cacheDir = String.Format("{1}{0}cache{0}images", Path.DirectorySeparatorChar, Settings.EveMonDataDir);
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
                string cacheFileName = Path.Combine(ImageCacheDirectory, GetCacheName(url));
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
            Pair<HttpWebRequest, GetImageCallback> p = new Pair<HttpWebRequest, GetImageCallback>(wr, callback);
            wr.BeginGetResponse(new AsyncCallback(GotImage), p);
        }

        private static void GotImage(IAsyncResult ar)
        {
            Pair<HttpWebRequest, GetImageCallback> p = ar.AsyncState as Pair<HttpWebRequest, GetImageCallback>;
            HttpWebRequest wr = p.A;
            GetImageCallback callback = p.B;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)wr.EndGetResponse(ar);
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
            using (StreamWriter sw = new StreamWriter(Path.Combine(ImageCacheDirectory, "file.map"), true))
            {
                sw.WriteLine(String.Format("{0} {1}", cacheName, url));
                sw.Close();
            }
            string fn = Path.Combine(ImageCacheDirectory, cacheName);
            try
            {
                FileStream fs = new FileStream(fn, FileMode.Create);
                i.Save(fs, ImageFormat.Png);
                fs.Close();
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
            byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(url));
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
        private int m_userId;
        private string m_apiKey;


        private EveSession(int userId, string apiKey)
        {
            m_userId = userId;
            m_apiKey = apiKey;
            try
            {
                GetCharacterList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("GetCharacterList() issue: " + ex.Message);
            }
        }

        private string m_characterListError = string.Empty;
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

            List<Pair<string, int>> charList = new List<Pair<string, int>>();
            XmlDocument xdoc = EveSession.GetCharList(Convert.ToString(m_userId), m_apiKey, out m_characterListError);
            XmlNode error = xdoc.DocumentElement.SelectSingleNode("descendant::error");
            if (error != null)
            {
                m_characterListError = error.InnerText;
                throw new InvalidDataException(m_characterListError);
            }
            else
            {
                XmlNodeList characters = xdoc.DocumentElement.SelectNodes("descendant::rowset/row");
                foreach (XmlNode charNode in characters)
                {
                    XmlAttributeCollection atts = charNode.Attributes;
                    string name = atts["name"].InnerText;
                    int id = Int32.Parse(atts["characterID"].InnerText);
                    charList.Add(new Pair<string, int>(name, id));
                }
            }
            m_storedCharacterList = charList;
            return m_storedCharacterList;
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

        private SerializableSkillTrainingInfo GetTrainingSkillInfo(int charId)
        {
            // Get the date and time before we send the request to compensate as much as possible for lag in receiving
            // and processing the response.

            DateTime requestTime = DateTime.Now;
            XmlDocument sdoc = null;
            try
            {

                sdoc = GetTrainingSkill(charId);
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
                using (XmlNodeReader xnr = new XmlNodeReader(charRoot))
                {
                    ssti = (SerializableSkillTrainingInfo)xs.Deserialize(xnr);
                    // This set must only be done at deserialisation when we can compare current time agaisnt machine time
                    // TQ is now synched with NNTP but the webserver isn't (checked with Garthagk at ccp)
                    double CCPOffset = ssti.GetDateTimeAtUpdate.ToLocalTime().Subtract(requestTime).TotalMilliseconds;
                    ssti.FixServerTimes(CCPOffset);
                }
            }
            return ssti;
        }

        /// <summary>
        /// Gets the character info from the myeve website using the API and stores it in the local cache.
        /// </summary>
        /// <param name="charId">The char id.</param>
        /// <returns>The SerializableCharacterSheet, fully populated</returns>
        public SerializableCharacterSheet GetCharacterInfo(int charId)
        {
            m_apiErrorMessage = string.Empty;
            m_apiErrorCode = 0;

            XmlDocument xdoc = null;
            try
            {
                xdoc = GetCharacterSheet(charId);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            }

            SerializableCharacterSheet scs = null;
            try
            {
                // Check for an API Error...
                XmlElement errorNode = null;
                errorNode = xdoc.DocumentElement.SelectSingleNode("//error") as XmlElement;
                if (errorNode != null)
                {
                    SetAPIError(errorNode);
                    return null;
                }

                scs = ProcessCharacterXml(xdoc, charId);
                scs.CharacterSheet.CreateSkillGroups();
                scs.FromCCP = true;
                //save the xml in the character cache
                LocalXmlCache.Instance.Save(xdoc);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            }

            return scs;
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
                    //bytes = client.UploadString( ("http://ineve.net/skills/evemon_upload.php", "");
                }
                catch (WebException)
                {
                    //just fail and trust that we'll try again next time.
                    return;
                }
                string response = Encoding.UTF8.GetString(bytes);
            }
        }

        private SerializableCharacterSheet ProcessCharacterXml(XmlDocument xdoc, int charId)
        {
            XmlSerializer xs = new XmlSerializer(typeof(SerializableCharacterSheet));
            XmlElement charRoot = xdoc.DocumentElement;
            using (XmlNodeReader xnr = new XmlNodeReader(charRoot))
            {
                return (SerializableCharacterSheet)xs.Deserialize(xnr);
            }
        }

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
            UpdateGCIArgs args = (UpdateGCIArgs)state;
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
        private const int DEFAULT_RETRY_INTERVAL = 30 * 60 * 1000;
        private Random autoRand;
        private Object mutexLock = new Object();

        public int UpdateGrandCharacterInfo(CharacterInfo grandCharacterInfo, Control invokeControl)
        {
            lock (mutexLock)
            {
                SerializableSkillTrainingInfo temp = grandCharacterInfo.SerialSIT;
                SerializableCharacterSheet sci = GetCharacterInfo(grandCharacterInfo.CharacterId);
                if (sci == null)
                {
                    autoRand = new Random();
                    grandCharacterInfo.DownloadFailed += 1;
                    grandCharacterInfo.DownloadError = ApiErrorMessage;
                    double offset = (DEFAULT_RETRY_INTERVAL * grandCharacterInfo.DownloadFailed) + (DEFAULT_RETRY_INTERVAL * autoRand.NextDouble());
                    return (int)offset;
                }
                else
                {
                    grandCharacterInfo.DownloadFailed = 0;
                    sci.TrainingSkillInfo = temp;
                }

#if DEBUG_SINGLETHREAD
                grandCharacterInfo.AssignFromSerializableCharacterSheet(sci);
#else
                invokeControl.Invoke(new MethodInvoker(delegate
                {
                    grandCharacterInfo.AssignFromSerializableCharacterSheet(sci);
                }));
#endif
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
                    string error = ssti.APIError.ErrorMessage;
                    if (error == "characterID does not belong to you.")
                        throw new Exception(error); // really should throw an exception here... so now we do!
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

    [XmlRoot("attributes")]
    public class EveAttributes
    {

        // m_owner is used for the adjusted Attribute method, which in turn is only
        // ever used for the "save as... text" method
        private CharacterSheetResult m_owner;

        internal void SetOwner(CharacterSheetResult cs)
        {
            m_owner = cs;
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

        // AdjustedXXX are only used by the "save as text" method.
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

        // only used by the "save as... text" method
        private double GetAttributeAdjustment(EveAttribute eveAttribute, SerializableEveAttributeAdjustment adjustment)
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

        // only used by "save as... text file"
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
