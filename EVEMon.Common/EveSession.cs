//#define DEBUG_SINGLETHREAD
//#define USE_LOCALHOST
// (If setting DEBUG_SINGLE THREAD, also set it in CharacterMonitor.cs)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common.Net;
using System.Globalization;

namespace EVEMon.Common
{
    public class EveSession
    {
        private static readonly Dictionary<string, WeakReference<EveSession>> m_sessions =
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
        private string m_apiErrorMessage;
        private int m_apiErrorCode;

        public static string ApiKeyUrl
        {
            get { return "http://myeve.eve-online.com/api/default.asp"; }
        }

        private static XmlDocument GetCharList(string userId, string apiKey, out string errorMessage)
        {
            errorMessage = string.Empty;
            APIState apiState = Singleton.Instance<APIState>();
            APIConfiguration configuration = apiState.CurrentConfiguration;
            HttpPostData postData = new HttpPostData("userID=" + userId + "&apiKey=" + apiKey);
            return
                CommonContext.HttpWebService.DownloadXml(configuration.MethodUrl(APIMethods.CharacterList),
                                                                  postData);
        }

        private XmlDocument GetTrainingSkill(int charId)
        {
            APIState apiState = Singleton.Instance<APIState>();
            APIConfiguration configuration = apiState.CurrentConfiguration;
            HttpPostData postData = new HttpPostData("userID=" + m_userId + "&apiKey=" + m_apiKey + "&characterID=" +
                                    Convert.ToString(charId));
            return
                CommonContext.HttpWebService.DownloadXml(configuration.MethodUrl(APIMethods.SkillInTraining),
                                                                  postData);
        }

        private XmlDocument GetCharacterSheet(int charId)
        {
            APIState apiState = Singleton.Instance<APIState>();
            APIConfiguration configuration = apiState.CurrentConfiguration;
            HttpPostData postData = new HttpPostData("userID=" + m_userId + "&apiKey=" + m_apiKey + "&characterID=" +
                                    Convert.ToString(charId));
            return
                CommonContext.HttpWebService.DownloadXml(configuration.MethodUrl(APIMethods.CharacterSheet),
                                                                  postData);
        }

        private void SetAPIError(XmlElement errorNode)
        {
            string text = errorNode.GetAttribute("code");
            if (!String.IsNullOrEmpty(text))
            {
                m_apiErrorCode = Int32.Parse(text);
            }
            m_apiErrorMessage = errorNode.InnerText;
        }

        private XmlDocument m_xmlDoc;

        /// <summary>
        /// Gets the XmlDocument returned by CCP
        /// </summary>
        public XmlDocument XmlDocument
        {
            get { return m_xmlDoc; }
        }

        /// <summary>
        /// Gets true whether the session encountered an error from CCP
        /// </summary>
        public bool HasError
        {
            get { return m_apiErrorCode != 0 || !String.IsNullOrEmpty(m_apiErrorMessage); }
        }

        /// <summary>
        /// The error code return by CCP, may be 0 even when there is actually an error (obsolete ?).
        /// </summary>
        public int ApiErrorCode
        {
            get { return m_apiErrorCode; }
            set { m_apiErrorCode = value; }
        }

        /// <summary>
        /// A formatted string composed of the error message and code returned by CCP
        /// </summary>
        public string ApiErrorMessage
        {
            get
            {
                if (String.IsNullOrEmpty(m_apiErrorMessage))
                {
                    if (m_apiErrorCode == 0) return "";
                    else return "Error code was " + m_apiErrorCode.ToString();
                }
                else
                {
                    if (m_apiErrorCode == 0) return m_apiErrorMessage;
                    else return m_apiErrorMessage + " (error code was " + m_apiErrorCode.ToString() + ")";
                }
            }
        }

        /// <summary>
        /// Compute the "cached until" time for the user's machine from the currentTime and cachedUntil nodes 
        /// in a CCP API message.
        /// </summary>
        /// <param name="xdoc">The xml message after validating that there is actual content!</param>
        /// <returns>a DateTime object in UTC time for when the message can be retrieved again - adjusted to compensate for the user's clock</returns>
        private static DateTime GetCacheExpiryUTC(XmlDocument xdoc)
        {
            // Firstly, extract the currentTime form the message - in case things go wrong, assume currentTine is "now"
            DateTime CCPCurrent = DateTime.Now.ToUniversalTime();
            try
            {
                XmlNode currentTimeNode = xdoc.SelectSingleNode("/eveapi/currentTime");
                CCPCurrent = TimeUtil.ConvertCCPTimeStringToDateTime(currentTimeNode.InnerText);
            }
            catch (Exception)
            {
                // do  nothing - default to "now";
            }

            // Now suck out the cachedUntil time - assume 1 hour from now in case the parse fails
            DateTime cacheExpires = DateTime.Now + new TimeSpan(1, 0, 0);
            try
            {
                XmlNode cachedTimeNode = xdoc.SelectSingleNode("/eveapi/cachedUntil");
                cacheExpires = TimeUtil.ConvertCCPTimeStringToDateTime(cachedTimeNode.InnerText);
            }
            catch (Exception)
            {
                // do  nothing - default to "now";
            }


            // Work out the cache period from the message and calculate the expiry time according to user's pc clock...
            return  DateTime.Now.ToUniversalTime() + (cacheExpires - CCPCurrent);
        }

        #endregion

        #region Images

        #endregion

        #region Nonstatic Members
        private readonly int m_userId;
        private readonly string m_apiKey;


        private EveSession(int userId, string apiKey)
        {
            m_userId = userId;
            m_apiKey = apiKey;
            Settings m_settings = Settings.GetInstance();
            AccountDetails acc = m_settings.FindAccount(m_userId);
            if (acc != null)
            {
                m_storedCharacterList = acc.StoredCharacterList;
                m_characterListCachedUntil = acc.CachedUntil;
            }
            try
            {
                GetCharacterListUncached();
            }
            catch (HttpWebServiceException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private string m_characterListError = string.Empty;
        private List<Pair<string, int>> m_storedCharacterList = null;
        private DateTime m_characterListCachedUntil = DateTime.MinValue;

        public List<Pair<string, int>> GetCharacterListUncached()
        {
            if (m_characterListCachedUntil < DateTime.Now)
            {
                m_storedCharacterList = null;
            }
            return GetCharacterList();
        }

        private List<Pair<string, int>> GetCharacterList()
        {
            if (m_storedCharacterList != null)
            {
                return m_storedCharacterList;
            }

            List<Pair<string, int>> charList = new List<Pair<string, int>>();
            XmlDocument xdoc = GetCharList(Convert.ToString(m_userId), m_apiKey, out m_characterListError);
            XmlNode error = xdoc.DocumentElement.SelectSingleNode("descendant::error");
            if (error != null)
            {
                m_characterListError = error.InnerText;
                throw new InvalidDataException(m_characterListError);
            }
            else
            {
                m_characterListCachedUntil = GetCacheExpiryUTC(xdoc);
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

            Settings m_settings = Settings.GetInstance();
            AccountDetails account = m_settings.FindAccount(m_userId);
            bool newAccount = false;
            if (account == null)
            {
                account = new AccountDetails();
                account.ApiKey = m_apiKey;
                account.UserId = m_userId;
                newAccount = true;
            }
            account.StoredCharacterList = m_storedCharacterList;
            account.CachedUntil = m_characterListCachedUntil;
            account.CheckForTransfer();
            if (newAccount)
            {
                Settings.GetInstance().Accounts.Add(account);
            }
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
            XmlDocument sdoc;
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
                try
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
                catch (Exception xmlException)
                {
                    ExceptionHandler.LogException(xmlException, true);
                    return null;
                }
            }
            return ssti;
        }

        /// <summary>
        /// Gets the character info from the myeve website using the API and stores it in the local cache.
        /// </summary>
        /// <param name="charId">The char id.</param>
        /// <returns>The SerializableCharacterSheet, fully populated</returns>
        private SerializableCharacterSheet GetCharacterInfo(int charId)
        {
            m_apiErrorMessage = string.Empty;
            m_apiErrorCode = 0;

            try
            {
                m_xmlDoc = GetCharacterSheet(charId);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            }

            SerializableCharacterSheet scs;
            try
            {
                // Check for an API Error...
                XmlElement errorNode;
                errorNode = m_xmlDoc.DocumentElement.SelectSingleNode("//error") as XmlElement;
                if (errorNode != null)
                {
                    SetAPIError(errorNode);
                    return null;
                }

                scs = SerializableCharacterSheet.CreateFromXmlDocument(m_xmlDoc);
                scs.CharacterSheet.CreateSkillGroups();
                scs.FromCCP = true;
                //save the xml in the character cache
                LocalXmlCache.Instance.Save(m_xmlDoc);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            }

            return scs;
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
            int timeLeftInCache = UpdateGrandCharacterInfo(args.GrandCharacterInfo, args.InvokeControl);
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
            int timeToNextUpdate = UpdateSkillTrainingInfo(args.GrandCharacterInfo, args.InvokeControl);
            args.UpdateTrainingSkillInfoCallback(null, timeToNextUpdate);
        }

        // Now default is set to 30 minutes instead of 5 minutes for the retry interval
        // to be more polite to the server.
        private const int DEFAULT_RETRY_INTERVAL = 30 * 60 * 1000;
        private Random autoRand;
        private readonly Object mutexLock = new Object();

        private int UpdateGrandCharacterInfo(CharacterInfo grandCharacterInfo, Control invokeControl)
        {

            lock (mutexLock)
            {
                Settings m_settings = Settings.GetInstance();
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
                grandCharacterInfo.AssignFromSerializableCharacterSheet(sci, m_settings.ShowAllPublicSkills, m_settings.ShowAllPublicSkills);
#else
                invokeControl.Invoke(new MethodInvoker(delegate
                {
                    grandCharacterInfo.AssignFromSerializableCharacterSheet(sci, m_settings.ShowAllPublicSkills, m_settings.ShowAllPublicSkills, m_settings.SkillPlannerHighlightPartialSkills);
                }));
#endif
                return sci.TimeLeftInCache;
            }
        }

        private int UpdateSkillTrainingInfo(CharacterInfo grandCharacterInfo, Control invokeControl)
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
                try
                {
                    invokeControl.Invoke(new MethodInvoker(delegate
                    {
                        grandCharacterInfo.AssignFromSerializableSkillTrainingInfo(ssti);
                    }));
                }
                catch (Exception) { }
                return (1000 * ssti.TimerToNextUpdate);
            }
        }

        #endregion
    }

    public delegate void UpdateGrandCharacterInfoCallback(EveSession sender, int timeLeftInCache);

    public delegate void UpdateTrainingSkillInfoCallback(EveSession sender, int timeRetryIn);

    public delegate void GetImageCallback(EveSession sender, Image i);
}
