using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using EVEMon.Common;
using EVEMon.Common.Net;
using EVEMon.Common.Schedule;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Reflection;

namespace EVEMon.Common
{
    [XmlRoot("logindata2")]
    public class Settings
    {
        // *** READ ME FIRST ***
        //
        // 1. Some updates to settings class currently need 
        //    lock(mutexLock), if you are not sure post on the forums
        //
        // 2. put new settings in the right region 
        //   (use outlining and collapse all to see where they go)

        private static Object mutexLock = new Object();

        public Settings() {}

        private Settings(string version)
        {
            VersionNumber = version;
        }

        #region Version Information

        private string m_versionNumber = string.Empty;

        /// <summary>
        /// Identifies the application version associated with the settings file. This is verified against
        /// the current version number when we load the settings file so we can prompt the user to back it up
        /// when we push out a release, this way if we change the structure of Settings the user can revert to a previous
        /// release without losing data.
        /// </summary>
        public string VersionNumber
        {
            get { return m_versionNumber; }
            set
            {
                lock (mutexLock)
                {
                    m_versionNumber = value;
                }
            }
        }

        private void CheckSettingsVersion(bool notifyUser)
        {
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if ((VersionNumber != currentVersion) && notifyUser)
                PromptUserToBackupSettings();
            VersionNumber = currentVersion;
        }

        private void PromptUserToBackupSettings()
        {
            DialogResult backupSettings =
                MessageBox.Show("The current EVEMon settings file is from a previous version of EVEMon. Backup the current file before proceeding (recommended)?",
                    "EVEMon version changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (backupSettings == DialogResult.Yes)
            {
                using (SaveFileDialog fileDialog = new SaveFileDialog())
                {
                    fileDialog.Title = "Settings file backup";
                    fileDialog.Filter = "Settings Backup Files (*.bak) | *.bak";
                    fileDialog.FileName = string.Format("EVEMon_Settings_{0}.xml.bak", VersionNumber);
                    fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    DialogResult saveFile = fileDialog.ShowDialog();
                    if (saveFile == DialogResult.OK)
                        File.Copy(SettingsFileName, fileDialog.FileName, true);
                }
            }
        }

        #endregion

        #region Character Lists

        private List<AccountDetails> m_accountList = new List<AccountDetails>();
        public List<AccountDetails> Accounts
        {
            get { return m_accountList; }
            set
            {
                lock (mutexLock)
                {
                    m_accountList = value;
                }
            }
        }

        public AccountDetails FindAccount(int userID)
        {
            AccountDetails acc = null;
            if (m_accountList != null)
            {
                foreach (AccountDetails acd in m_accountList)
                {
                    if (acd.UserId == userID)
                    {
                        acc = acd;
                        break;
                    }
                }
            }
            return acc;
        }

        public AccountDetails FindAccount(string charName)
        {
            AccountDetails acc = null;
            if (m_accountList != null)
            {
                foreach (AccountDetails acd in m_accountList)
                {
                    if (acd.HasCharacter(charName))
                    {
                        acc = acd;
                        break;
                    }
                }
            }
            return acc;
        }

        private List<CharLoginInfo> m_characterList = new List<CharLoginInfo>();
        public List<CharLoginInfo> CharacterList
        {
            get { return m_characterList; }
            set
            {
                lock (mutexLock)
                {
                    m_characterList = value;
                }
            }
        }

        public bool LogonCharExists(string charName)
        {
            foreach (CharLoginInfo cli in CharacterList)
            {
                if (cli.CharacterName == charName) return true;
            }
            return false;
        }

        private List<CharFileInfo> m_charFileList = new List<CharFileInfo>();
        public List<CharFileInfo> CharFileList
        {
            get { return m_charFileList; }
            set
            {
                lock (mutexLock)
                {
                    m_charFileList = value;
                }
            }
        }

        #endregion

        #region Character Cache

        private List<SerializableCharacterSheet> m_cachedCharacterInfo = new List<SerializableCharacterSheet>();

        public List<SerializableCharacterSheet> CachedCharacterInfo
        {
            get { return m_cachedCharacterInfo; }
        }

        public SerializableCharacterSheet GetCharacterSheet(string charName)
        {
            foreach (SerializableCharacterSheet sci in m_cachedCharacterInfo)
            {
                if (sci.CharacterSheet.Name == charName)
                {
                    return sci;
                }
            }
            return null;
        }

        public void RemoveCharacterCache(string charName)
        {
            lock (mutexLock)
            {
                for (int i = 0; i < m_cachedCharacterInfo.Count; i++)
                {
                    if (m_cachedCharacterInfo[i].CharacterSheet.Name == charName)
                    {
                        m_cachedCharacterInfo.RemoveAt(i);
                    }
                }
            }
        }

        public void SetCharacterCache(SerializableCharacterSheet sci)
        {
            lock (mutexLock)
            {
                RemoveCharacterCache(sci.CharacterSheet.Name);
                m_cachedCharacterInfo.Add(sci);
            }
        }
        #endregion

        #region Old Skills
        private SerializableDictionary<string, SerializableSkillTrainingInfo> m_oldSkillsDict = new SerializableDictionary<string, SerializableSkillTrainingInfo>();

        public SerializableDictionary<string, SerializableSkillTrainingInfo> OldSAPIkillsDict
        {
            get { return m_oldSkillsDict; }
            set
            {
                if (value != null)
                {
                    lock (mutexLock)
                    {
                        m_oldSkillsDict = value;
                    }
                }
            }
        }
        #endregion

        #region Owned Skills

        private List<Pair<string, string>> m_ownedbooks = new List<Pair<string, string>>();

        public List<Pair<string, string>> OwnedBooks
        {
            get { return m_ownedbooks; }
        }

        public IEnumerable<string> GetOwnedBooksForCharacter(string charName)
        {
            foreach (Pair<string, string> x in m_ownedbooks)
            {
                if (x.A == charName)
                {
                    yield return x.B;
                }
            }
        }

        public void SetOwnedBooks(string characterName, List<String> ownedBooks)
        {
            lock (mutexLock)
            {
                List<Pair<string, string>> newList = new List<Pair<string, string>>();
                bool added = false;
                foreach (Pair<string, string> x in m_ownedbooks)
                {
                    if (x.A == characterName)
                    {
                        if (!added)
                        {
                            foreach (string book in ownedBooks)
                            {
                                newList.Add(new Pair<string, string>(characterName, book));
                            }
                            added = true;
                        }
                    }
                    else
                    {
                        newList.Add(x);
                    }
                }
                if (!added)
                {
                    foreach (string book in ownedBooks)
                    {
                        newList.Add(new Pair<string, string>(characterName, book));
                    }
                }
                m_ownedbooks = newList;
            }
        }

        #endregion

        #region Settings Form - General Tab

        public bool SystemTrayOptionsIsNever
        {
			get { return SystemTrayOptions == SystemTrayDisplayOptions.Never; }
        }

        public bool SystemTrayOptionsIsMinimized
        {
			get { return SystemTrayOptions == SystemTrayDisplayOptions.Minimized; }
        }

        public bool SystemTrayOptionsIsAlways
        {
			get { return SystemTrayOptions == SystemTrayDisplayOptions.Always; }
        }

		private SystemTrayDisplayOptions m_systemTrayOptions
			= Environment.OSVersion.Platform == PlatformID.Unix
			 ? SystemTrayDisplayOptions.Never
			 : SystemTrayDisplayOptions.Minimized;
        public SystemTrayDisplayOptions SystemTrayOptions
        {
            get { return m_systemTrayOptions; }
            set
            {
                lock (mutexLock)
                {
                    m_systemTrayOptions = value;
                }
            }
        }

        private bool m_closeToTray = false;
        public bool CloseToTray
        {
            get { return m_closeToTray; }
            set
            {
                lock (mutexLock)
                {
                    m_closeToTray = value;
                }
            }
        }

        private bool m_relocateEveWindow = false;
        public bool RelocateEveWindow
        {
            get { return m_relocateEveWindow; }
            set
            {
                lock (mutexLock)
                {
                    m_relocateEveWindow = value;
                    OnRelocateEveWindowChanged();
                }
            }
        }
        private int m_relocateTargetScreen = 0;
        public int RelocateTargetScreen
        {
            get { return m_relocateTargetScreen; }
            set
            {
                lock (mutexLock)
                {
                    if (m_relocateTargetScreen != value)
                    {
                        m_relocateTargetScreen = value;
                        OnRelocateEveWindowChanged();
                    }
                }
            }
        }

        private bool m_useLogitechG15Display = false;
        public bool UseLogitechG15Display
        {
            get { return m_useLogitechG15Display; }
            set
            {
                lock (mutexLock)
                {
                    m_useLogitechG15Display = value;
                    OnUseLogitechG15DisplayChanged();
                }
            }
        }

        private bool m_g15acycle = false;
        public bool G15ACycle
        {
            get { return m_g15acycle; }
            set
            {
                lock (mutexLock)
                {
                    m_g15acycle = value;
                }
            }
        }

        private int m_g15acycleint = 20;
        public int G15ACycleint
        {
            get { return m_g15acycleint; }
            set
            {
                lock (mutexLock)
                {
                    m_g15acycleint = value;
                }
            }
        }

        private bool m_g15showtime = false;
        public bool G15ShowTime
        {
            get { return m_g15showtime; }
            set
            {
                lock (mutexLock)
                {
                    m_g15showtime = value;
                }
            }
        }

        private bool m_g15cycletimes = false;
        public bool G15ACycleTimes
        {
            get { return m_g15cycletimes; }
            set
            {
                lock (mutexLock)
                {
                    m_g15cycletimes = value;
                }
            }
        }

        private int m_g15cycletimesint = 10;
        public int G15ACycleTimesint
        {
            get { return m_g15cycletimesint; }
            set
            {
                lock (mutexLock)
                {
                    m_g15cycletimesint = value;
                }
            }
        }

        private bool m_showAllPublicSkills = false;
        public bool ShowAllPublicSkills
        {
            get { return m_showAllPublicSkills; }
            set
            {
                lock (mutexLock)
                {
                    m_showAllPublicSkills = value;
                }
            }
        }

        private bool m_showNonPublicSkills = false;
        public bool ShowNonPublicSkills
        {
            get { return m_showNonPublicSkills; }
            set
            {
                lock (mutexLock)
                {
                    m_showNonPublicSkills = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets true when the user wants to hide the overview tab on the main page, false otherwise.
        /// </summary>
        public bool HideOverviewTab
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the running platform, so that EVEMon can apply miscellaneous tweaks at runtime
        /// </summary>
        public CompatibilityMode Compatibility
        {
            get;
            set;
        }
        #endregion

        #region Settings Form - Look and Feel

        private bool m_worksafeMode = false;
        public bool WorksafeMode
        {
            get { return m_worksafeMode; }
            set
            {
                lock (mutexLock)
                {
                    m_worksafeMode = value; OnWorksafeChanged();
                }
            }
        }

        private bool m_showCharacterInfoInWindowTitle = true;

        public bool ShowCharacterInfoInWindowTitle
        {
            get { return m_showCharacterInfoInWindowTitle; }
            set
            {
                lock (mutexLock)
                {
                    m_showCharacterInfoInWindowTitle = value;
                }
            }
        }

        private int m_titleToTimeLayout = 0;
        public int TitleToTimeLayout
        {
            get { return m_titleToTimeLayout; }
            set
            {
                lock (mutexLock)
                {
                    m_titleToTimeLayout = value;
                }
            }
        }

        private bool m_titleToTimeSkill = false;
        public bool TitleToTimeSkill
        {
            get { return m_titleToTimeSkill; }
            set
            {
                lock (mutexLock)
                {
                    m_titleToTimeSkill = value;
                }
            }
        }

        private TrayPopupStyles m_trayPopupStyle = TrayPopupStyles.PopupForm;
        public TrayPopupStyles TrayPopupStyle
        {
            get { return m_trayPopupStyle; }
            set
            {
                lock (mutexLock)
                {
                    m_trayPopupStyle = value;
                }
            }
        }

        private TrayPopupConfig m_trayPopupConfig = new TrayPopupConfig();
        public TrayPopupConfig TrayPopupConfig
        {
            get { return m_trayPopupConfig; }
            set
            {
                lock (mutexLock)
                {
                    m_trayPopupConfig = value;
                }
            }
        }

        private string m_tooltipString = "%n - %s %tr - %r";
        public string TooltipString
        {
            get { return m_tooltipString; }
            set
            {
                lock (mutexLock)
                {
                    m_tooltipString = value;
                }
            }
        }

        private bool m_HighlightPlannedSkills;
        public bool SkillPlannerHighlightPlannedSkills
        {
            get { return m_HighlightPlannedSkills; }
            set
            {
                lock (mutexLock)
                {
                    m_HighlightPlannedSkills = value; OnHighlightPlannedSkillsChanged();
                }
            }
        }

        private bool m_HighlightPrerequisites;
        public bool SkillPlannerHighlightPrerequisites
        {
            get { return m_HighlightPrerequisites; }
            set
            {
                lock (mutexLock)
                {
                    m_HighlightPrerequisites = value; OnHighlightPrerequisitesChanged();
                }
            }
        }

        private bool m_DimUntrainable = true;
        public bool SkillPlannerDimUntrainable
        {
            get { return m_DimUntrainable; }
            set
            {
                lock (mutexLock)
                {
                    m_DimUntrainable = value; OnDimUntrainableChanged();
                }
            }
        }

        private bool m_HighlightConflicts = true;
        public bool SkillPlannerHighlightConflicts
        {
            get { return m_HighlightConflicts; }
            set
            {
                lock (mutexLock)
                {
                    m_HighlightConflicts = value; OnHighlightConflictsChanged();
                }
            }
        }

        private bool m_HighlightPartialSkills = false;
        public bool SkillPlannerHighlightPartialSkills
        {
            get { return m_HighlightPartialSkills; }
            set
            {
                lock (mutexLock)
                {
                    m_HighlightPartialSkills = value; OnHighlightPartialSkillsChanged();
                }
            }
        }


        private int m_skillIconGroup = 0;
        public int SkillIconGroup
        {
            get { return m_skillIconGroup; }
            set
            {
                lock (mutexLock)
                {
                    m_skillIconGroup = value;
                }
            }
        }


        #endregion

        #region Settings Form - Network Tab

        private bool m_useCustomProxySettings = false;
        public bool UseCustomProxySettings
        {
            get { return m_useCustomProxySettings; }
            set
            {
                lock (mutexLock)
                {
                    m_useCustomProxySettings = value;
                }
                CommonContext.HttpWebService.State.UseCustomProxy = value;
            }
        }

        private ProxySetting m_httpProxy = new ProxySetting();
        public ProxySetting HttpProxy
        {
            get { return m_httpProxy; }
            set
            {
                lock (mutexLock)
                {
                    m_httpProxy = value;
                }
                CommonContext.HttpWebService.State.Proxy = value;
            }
        }

        private bool m_disableRequestsOnAuthenticationFailure = false;
        public bool DisableRequestsOnAuthenticationFailure
        {
            get { return m_disableRequestsOnAuthenticationFailure; }
            set
            {
                lock (mutexLock)
                {
                    m_disableRequestsOnAuthenticationFailure = value;
                }
                CommonContext.HttpWebService.State.DisableOnProxyAuthenticationFailure = value;
            }
        }

        private bool m_runIgbServer = true;
        private bool m_igbServerPublic = false;
        private int m_igbPort = 80;

        public bool IGBServerPublic
        {
            get { return m_igbServerPublic; }
            set
            {
                lock (mutexLock)
                {
                    m_igbServerPublic = value;
                    OnRunIGBServerChanged();
                }
            }
        }

        public int IGBServerPort
        {
            get
            {
                return m_igbPort;
            }
            set
            {
                lock (mutexLock)
                {
                    m_igbPort = value;
                    OnRunIGBServerChanged();
                }
            }
        }

        public bool RunIGBServer
        {
            get { return m_runIgbServer; }
            set
            {
                lock (mutexLock)
                {
                    m_runIgbServer = value; OnRunIGBServerChanged();
                }
            }
        }

        private bool m_useEveProxy = false;

        public bool UseEveProxy
        {
            get { return m_useEveProxy; }
            set
            {
                lock (mutexLock)
                {
                    m_useEveProxy = value;
                }
            }
        }

        private string m_eveProxyURL = "";

        public string EveProxyURL
        {
            get
            {
                if (m_useEveProxy || m_eveProxyURL != String.Empty)
                {
                    return m_eveProxyURL;
                }
                else
                {
                    return APIConstants.Server;
                }
            }
            set
            {
                lock (mutexLock)
                {
                    m_eveProxyURL = value;
                }
            }
        }

#endregion
       
        #region Settings Form - Alerts Tab

        private bool m_enableBalloonTips = true;
        public bool EnableBalloonTips
        {
            get { return m_enableBalloonTips; }
            set
            {
                lock (mutexLock)
                {
                    m_enableBalloonTips = value;
                }
            }
        }

        private bool m_playSoundOnSkillComplete = true;
        public bool PlaySoundOnSkillComplete
        {
            get { return m_playSoundOnSkillComplete; }
            set
            {
                lock (mutexLock)
                {
                    m_playSoundOnSkillComplete = value;
                }
            }
        }

        private int m_notificationOffset = 0;
        public int NotificationOffset
        {
            get { return m_notificationOffset; }
            set
            {
                lock (mutexLock)
                {
                    m_notificationOffset = value;
                    if (NotificationOffsetChanged != null)
                    {
                        NotificationOffsetChanged(this, new EventArgs());
                    }
                }
            }
        }

        private bool m_enableEmailAlert = false;
        public bool EnableEmailAlert
        {
            get { return m_enableEmailAlert; }
            set
            {
                lock (mutexLock)
                {
                    m_enableEmailAlert = value;
                }
            }
        }
        private string m_emailServer;
        public string EmailServer
        {
            get { return m_emailServer; }
            set
            {
                lock (mutexLock)
                {
                    m_emailServer = value;
                }
            }
        }

        private bool m_emailServerRequiresSsl = false;
        public bool EmailServerRequiresSsl
        {
            get { return m_emailServerRequiresSsl; }
            set
            {
                lock (mutexLock)
                {
                    m_emailServerRequiresSsl = value;
                }
            }
        }

        private bool m_emailAuthRequired = false;
        public bool EmailAuthRequired
        {
            get { return m_emailAuthRequired; }
            set
            {
                lock (mutexLock)
                {
                    m_emailAuthRequired = value;
                }
            }
        }

        private string m_emailUsername;
        private string m_emailPassword;

        public string EmailAuthUsername
        {
            get { return m_emailUsername; }
            set
            {
                lock (mutexLock)
                {
                    m_emailUsername = value;
                }
            }
        }
        
        public string EmailAuthPassword
        {
            get { return m_emailPassword; }
            set
            {
                lock (mutexLock)
                {
                    m_emailPassword = value;
                }
            }
        }

        private string m_emailFromAddress;
        public string EmailFromAddress
        {
            get { return m_emailFromAddress; }
            set
            {
                lock (mutexLock)
                {
                    m_emailFromAddress = value;
                }
            }
        }

        private string m_emailToAddress;
        public string EmailToAddress
        {
            get { return m_emailToAddress; }
            set
            {
                lock (mutexLock)
                {
                    m_emailToAddress = value;
                }
            }
        }

        private int m_portNumber;
        public int PortNumber
        {
            get { return m_portNumber; }
            set
            {
                lock (mutexLock)
                {
                    m_portNumber = value;
                }
            }
        }

        private bool m_emailUseShortFormat = false;
        public bool EmailUseShortFormat
        {
            get { return m_emailUseShortFormat; }
            set
            {
                lock (mutexLock)
                {
                    m_emailUseShortFormat = value;
                }
            }
        }
        #endregion

        #region Settings Form - Updates Tab

        private bool m_DisableEVEMonVersionCheck;
        public bool DisableEVEMonVersionCheck
        {
            get { return m_DisableEVEMonVersionCheck; }
            set
            {
                lock (mutexLock)
                {
                    m_DisableEVEMonVersionCheck = value;
                }
            }
        }

        private bool m_CheckTimeOnStartup = true;
        public bool CheckTimeOnStartup
        {
            get { return m_CheckTimeOnStartup; }
            set
            {
                lock(mutexLock)
                {
                    m_CheckTimeOnStartup = value;
                }
            }
        }

        private bool m_DisableXMLAutoUpdate;
        public bool DisableXMLAutoUpdate
        {
            get { return m_DisableXMLAutoUpdate; }
            set
            {
                lock (mutexLock)
                {
                    m_DisableXMLAutoUpdate = value;
                }
            }
        }

        private bool m_KeepCharacterPlans;
        public bool KeepCharacterPlans
        {
            get { return m_KeepCharacterPlans; }
            set
            {
                lock (mutexLock)
                {
                    m_KeepCharacterPlans = value;
                }
            }
        }

        public bool ResetCache()
        {
            bool resetOK = true;
            lock (mutexLock)
            {
                if (File.Exists(SettingsFileName))
                {
                    try
                    {
                        File.Delete(SettingsFileName);
                    }
                    catch (Exception)
                    {
                        resetOK = false;
                    }
                }
            }
            return resetOK;
        }

        /// <summary>
        /// The number of seconds to wait before retrying the download of new API data 
        /// when no character is training
        /// </summary>
        private int m_apiUpdateDelay = 600;

        public int APIUpdateDelay
        {
            get { return m_apiUpdateDelay; }
            set { m_apiUpdateDelay = value; }
        }
	
        private bool m_EnableSkillCompleteDialog;

        public bool EnableSkillCompleteDialog
        {
            get { return m_EnableSkillCompleteDialog; }
            set
            {
                lock (mutexLock)
                {
                    m_EnableSkillCompleteDialog = value;
                }
            }
        }

        private string m_customTQAddress = "87.237.38.200";
        public string CustomTQAddress
        {
            get { return m_customTQAddress == null ? "" : m_customTQAddress; }
            set
            {
                lock (mutexLock)
                {
                    m_customTQAddress = value;
                }
            }
        }

        private string m_customTQPort = "26000";
        public string CustomTQPort
        {
            get { return m_customTQPort == null ? "" : m_customTQPort; }
            set
            {
                lock (mutexLock)
                {
                    m_customTQPort = value;
                }
            }
        }

        private bool m_useCustomTQCheckSettings = false;
        public bool UseCustomTQCheckSettings
        {
            get { return m_useCustomTQCheckSettings; }
            set
            {
                lock (mutexLock)
                {
                    m_useCustomTQCheckSettings = value;
                }
            }
        }

        private bool m_checkTranquilityStatus = true;
        public bool CheckTranquilityStatus
        {
            get { return m_checkTranquilityStatus; }
            set
            {
                lock (mutexLock)
                {
                    if (m_checkTranquilityStatus != value)
                    {
                        m_checkTranquilityStatus = value;
                        OnCheckTranquilityStatusChanged();
                    }
                }
            }
        }

        private bool m_showTQBalloon = true;
        public bool ShowTQBalloon
        {
            get { return m_showTQBalloon; }
            set 
            {
                lock (mutexLock)
                {
                    m_showTQBalloon = value;
                }
            }
        }
        
        private int m_statusUpdateInterval = 5;
        public int StatusUpdateInterval
        {
            get { return m_statusUpdateInterval; }
            set
            {
                lock (mutexLock)
                {
                    m_statusUpdateInterval = value;
                }
            }
        }
        private string m_connectivityURL = "http://www.google.com";

        public string ConnectivityURL
        {
            get { return m_connectivityURL; }
            set
            {
                lock (mutexLock)
                {
                    m_connectivityURL = value;
                }
            }
        }
	
        #endregion

        #region Settings Form - Events and Event Handlers
        public event EventHandler<EventArgs> NotificationOffsetChanged;
        public event EventHandler<EventArgs> UseLogitechG15DisplayChanged;
        public event EventHandler<EventArgs> HighlightPlannedSkillsChanged;
        public event EventHandler<EventArgs> HighlightPrerequisitesChanged;
        public event EventHandler<EventArgs> DimUntrainableChanged;
        public event EventHandler<EventArgs> HighlightConflictsChanged;
        public event EventHandler<EventArgs> HighlightPartialSkillsChanged;
        public event EventHandler<EventArgs> WorksafeChanged;
        public event EventHandler<EventArgs> RunIGBServerChanged;
        public event EventHandler<EventArgs> RelocateEveWindowChanged;
        public event EventHandler<EventArgs> ShowTQBalloonChanged;
        public event EventHandler<EventArgs> CheckTranquilityStatusChanged;

        private void OnUseLogitechG15DisplayChanged()
        {
            if (UseLogitechG15DisplayChanged != null)
            {
                UseLogitechG15DisplayChanged(this, new EventArgs());
            }
        }

        private void OnHighlightPlannedSkillsChanged()
        {
            if (HighlightPlannedSkillsChanged != null)
            {
                HighlightPlannedSkillsChanged(this, new EventArgs());
            }
        }
   
        private void OnHighlightPrerequisitesChanged()
        {
            if (HighlightPrerequisitesChanged != null)
            {
                HighlightPrerequisitesChanged(this, new EventArgs());
            }
        }

        private void OnDimUntrainableChanged()
        {
            if (DimUntrainableChanged != null)
            {
                DimUntrainableChanged(this, new EventArgs());
            }
        }

        private void OnHighlightConflictsChanged()
        {
            if (HighlightConflictsChanged != null)
            {
                HighlightConflictsChanged(this, new EventArgs());
            }
        }

        private void OnHighlightPartialSkillsChanged()
        {
            if (HighlightPartialSkillsChanged != null)
            {
                HighlightPartialSkillsChanged(this, new EventArgs());
            }
        }
        private void OnWorksafeChanged()
        {
            if (WorksafeChanged != null)
            {
                WorksafeChanged(this, new EventArgs());
            }
        }
        
        private void OnRunIGBServerChanged()
        {
            if (RunIGBServerChanged != null)
            {
                RunIGBServerChanged(this, new EventArgs());
            }
        }

        private void OnRelocateEveWindowChanged()
        {
            if (RelocateEveWindowChanged != null)
            {
                RelocateEveWindowChanged(this, new EventArgs());
            }
        }

        private void OnShowTQBalloonChanged()
        {
            if (ShowTQBalloonChanged != null)
            {
                ShowTQBalloonChanged(this, new EventArgs());
            }
        }

        private void OnCheckTranquilityStatusChanged()
        {
            if (CheckTranquilityStatusChanged != null)
            {
                CheckTranquilityStatusChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Settings Form - Calendar

        public enum ColorFormat
        {
            NamedColor,
            ARGBColor
        }

        public string SerializeColor(Color color)
        {
            if (color.IsNamedColor)
                return string.Format("{0}:{1}",
                    ColorFormat.NamedColor, color.Name);
            else
                return string.Format("{0}:{1}:{2}:{3}:{4}",
                    ColorFormat.ARGBColor,
                    color.A, color.R, color.G, color.B);
        }

        public Color DeserializeColor(string color)
        {
            byte a, r, g, b;

            string[] pieces = color.Split(new char[] { ':' });

            ColorFormat colorType = (ColorFormat)Enum.Parse(typeof(ColorFormat), pieces[0], true);

            switch (colorType)
            {
                case ColorFormat.NamedColor:
                    return Color.FromName(pieces[1]);

                case ColorFormat.ARGBColor:
                    a = byte.Parse(pieces[1]);
                    r = byte.Parse(pieces[2]);
                    g = byte.Parse(pieces[3]);
                    b = byte.Parse(pieces[4]);

                    return Color.FromArgb(a, r, g, b);
            }
            return Color.Empty;
        }

        private Color m_calendar_text = Color.White;

        [XmlIgnore]
        public Color CalendarTextColor
        {
            get { return m_calendar_text; }
            set
            {
                lock (mutexLock)
                {
                    m_calendar_text = value;
                }
            }
        }

        [XmlElement("CalendarTextColor")]
        public string XmlCalendarTextColor
        {
            get
            {
                return SerializeColor(m_calendar_text);
            }
            set
            {
                m_calendar_text = DeserializeColor(value);
            }
        }

        private Color m_calendar_blocking = Color.Red;

        [XmlIgnore]
        public Color CalendarBlockingColor
        {
            get { return m_calendar_blocking; }
            set
            {
                lock (mutexLock)
                {
                    m_calendar_blocking = value;
                }
            }
        }

        [XmlElement("CalendarBlockingColor")]
        public string XmlCalendarBlockingColor
        {
            get
            {
                return SerializeColor(m_calendar_blocking);
            }
            set
            {
                m_calendar_blocking = DeserializeColor(value);
            }
        }

        private Color m_calendar_single1 = Color.Blue;

        [XmlIgnore]
        public Color CalendarSingle1
        {
            get { return m_calendar_single1; }
            set
            {
                lock (mutexLock)
                {
                    m_calendar_single1 = value;
                }
            }
        }

        [XmlElement("CalendarSingle1")]
        public string XmlCalendarSingle1
        {
            get
            {
                return SerializeColor(m_calendar_single1);
            }
            set
            {
                m_calendar_single1 = DeserializeColor(value);
            }
        }

        private Color m_calendar_single2 = Color.LightBlue;

        [XmlIgnore]
        public Color CalendarSingle2
        {
            get { return m_calendar_single2; }
            set
            {
                lock (mutexLock)
                {
                    m_calendar_single2 = value;
                }
            }
        }

        [XmlElement("CalendarCalendarSingle2")]
        public string XmlCalendarSingle2
        {
            get
            {
                return SerializeColor(m_calendar_single2);
            }
            set
            {
                m_calendar_single2 = DeserializeColor(value);
            }
        }

        private Color m_calendar_recurring1 = Color.Green;

        [XmlIgnore]
        public Color CalendarRecurring1
        {
            get { return m_calendar_recurring1; }
            set
            {
                lock (mutexLock)
                {
                    m_calendar_recurring1 = value;
                }
            }
        }

        [XmlElement("CalendarRecurring1")]
        public string XmlCalendarRecurring1
        {
            get
            {
                return SerializeColor(m_calendar_recurring1);
            }
            set
            {
                m_calendar_recurring1 = DeserializeColor(value);
            }
        }

        private Color m_calendar_recurring2 = Color.LightGreen;

        [XmlIgnore]
        public Color CalendarRecurring2
        {
            get { return m_calendar_recurring2; }
            set
            {
                lock (mutexLock)
                {
                    m_calendar_recurring2 = value;
                }
            }
        }

        [XmlElement("CalendarRecurring2")]
        public string XmlCalendarRecurring2
        {
            get
            {
                return SerializeColor(m_calendar_recurring2);
            }
            set
            {
                m_calendar_recurring2 = DeserializeColor(value);
            }
        }

// #735 - Start
        // External Calendar Options.
        private bool m_useExternalCalendar = false;
        public bool UseExternalCalendar
        {
            get { return m_useExternalCalendar; }
            set
            {
                lock (mutexLock)
                {
                    m_useExternalCalendar = value;
                }
            }
        }
        // Future-proof in case other calendar's are added.
        private int m_calendarOption = 0;
        public int CalendarOption
        {
            get { return m_calendarOption; }
            set
            {
                lock (mutexLock)
                {
                    m_calendarOption = value;
                }
            }
        }
        private string m_googleEmail = string.Empty;
        public string GoogleEmail
        {
            get { return m_googleEmail; }
            set
            {
                lock (mutexLock)
                {
                    m_googleEmail = value;
                }
            }
        }
        private string m_googlePassword = string.Empty;
        public string GooglePassword
        {
            get { return m_googlePassword; }
            set
            {
                lock (mutexLock)
                {
                    m_googlePassword = value;
                }
            }
        }
        private string m_googleURI = "http://www.google.com/calendar/feeds/default/private/full";
        public string GoogleURI
        {
            get { return m_googleURI; }
            set
            {
                lock (mutexLock)
                {
                    m_googleURI = value;
                }
            }
        }
        private int m_googleReminder = 2;
        public int GoogleReminder
        {
            get { return m_googleReminder; }
            set
            {
                lock (mutexLock)
                {
                    m_googleReminder = value;
                }
            }
        }
        private bool m_setReminder = true;
        public bool SetReminder
        {
            get { return m_setReminder; }
            set
            {
                lock (mutexLock)
                {
                    m_setReminder = value;
                }
            }
        }
        private int m_reminderMinutes = 10;
        public int ReminderMinutes
        {
            get { return m_reminderMinutes; }
            set
            {
                lock (mutexLock)
                {
                    m_reminderMinutes = value;
                }
            }
        }
        private bool m_useAlternateReminder = true;
        public bool UseAlternateReminder
        {
            get { return m_useAlternateReminder; }
            set
            {
                lock (mutexLock)
                {
                    m_useAlternateReminder = value;
                }
            }
        }
        private DateTime m_earlyReminder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
        public DateTime EarlyReminder
        {
            get { return m_earlyReminder; }
            set
            {
                lock (mutexLock)
                {
                    m_earlyReminder = value;
                }
            }
        }
        private DateTime m_lateReminder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0);
        public DateTime LateReminder
        {
            get { return m_lateReminder; }
            set
            {
                lock (mutexLock)
                {
                    m_lateReminder = value;
                }
            }
        }
// #735 - End

        #endregion

        #region Plans

        // needs to be before plans.

        ColumnPreference m_columnPreferences = new ColumnPreference();

        public ColumnPreference ColumnPreferences
        {
            get { return m_columnPreferences; }
            set
            {
                lock (mutexLock)
                {
                    m_columnPreferences = value;
                }
            }
        }

        private List<Pair<string, Plan>> m_plans = new List<Pair<string, Plan>>();

        public List<Pair<string, Plan>> Plans
        {
            get { return m_plans; }
        }

        private const string PLAN_DEFAULT = "Default Plan";

        // WARNING!! Plans are NOT indexed by character name.
        // If the character is logon based, then they are.. BUT
        // if the character is file based, then they are index by the filename

        /// <summary>
        /// Count the plans for a character.
        /// </summary>
        /// <param name="charName">The character whose plans we wish to count.</param>
        /// <returns>The number of plans the character has.</returns>
        public int GetPlanCountForCharacter(string planKeyName)
        {
            int count = 0;
            foreach (Pair<string, Plan> x in m_plans)
            {
                if (x.A.StartsWith(planKeyName + "::"))
                {
                    count++;
                }
            }
            return count;
        }

        public IEnumerable<string> GetPlansForCharacter(string planKeyName)
        {
            foreach (Pair<string, Plan> x in m_plans)
            {
                if (x.A == planKeyName)
                {
                    yield return PLAN_DEFAULT;
                }
                else if (x.A.StartsWith(planKeyName + "::"))
                {
                    yield return x.A.Substring(planKeyName.Length + 2);
                }
            }
        }

        /// <summary>
        /// Finds a plan by name. 
        /// 
        /// This creates a new character sheet! 
        /// You should use the overload that takes a CharacterInfo parameter whereever possible
        /// USE THIS VERY CAREFULLY AS IT WILL FIRE OF LOTS OF CHARACTER SHEET EVENTS 
        /// AND CALL CheckTraining()
        /// 
        /// Currently NOTHING uses this - and it should remain ever thus.
        /// </summary>
        /// <param name="charName">The Plan Key (either char name or filename) for the character whose plans we're going to search.</param>
        /// <param name="planName">The plan name to look for.</param>
        /// <returns>The plan if found; null if not.</returns>
        
        [Obsolete]
        public Plan GetPlanByName(string planKeyName, string planName)
        {
            Plan p = FindPlan(planName, planKeyName);
            if (p == null)
            {
                return p;
            }

            SerializableCharacterSheet sci = this.GetCharacterSheet(planKeyName);
            if (sci != null)
            {
                CharacterInfo gci = new CharacterInfo(sci.CharacterSheet.CharacterId, planKeyName);
                // AssignFromSerializableCharacterSheet fires lots of events and calls the CheckTRainingSkill processing
                gci.AssignFromSerializableCharacterSheet(GetCharacterSheet(planKeyName), this.ShowAllPublicSkills, this.ShowNonPublicSkills, this.SkillPlannerHighlightPartialSkills);

                p.GrandCharacterInfo = gci;
            }
            else
            {
                p.GrandCharacterInfo = null;
            }
            return p;
        }


        /// <summary>
        /// Finds a plan by characterInfo.
        /// </summary>
        /// <param name="charInfo">The character whose plans we're going to search.</param>
        /// <param name="planName">The plan name to look for.</param>
        /// <returns>The plan if found; null if not.</returns>
        /// 

        public Plan GetPlanByName(string planKeyName, CharacterInfo  charInfo, string planName)
        {

            Plan p = FindPlan(planName, planKeyName);
            if (p != null)
            {
                p.GrandCharacterInfo = charInfo;
            }
            return p;
        }

        private Plan FindPlan(string planName, string planKeyName)
        {
            Plan p = null;
            foreach (Pair<string, Plan> x in m_plans)
            {
                if (planName == PLAN_DEFAULT && x.A == planKeyName)
                {
                    x.B.Name = PLAN_DEFAULT;
                    p = x.B;
                    break;
                }
                else if (x.A == planKeyName + "::" + planName)
                {
                    x.B.Name = planName;
                    p = x.B;
                    break;
                }
            }
            return p;
        }

        /// <summary>
        /// Adds a plan.
        /// </summary>
        /// <param name="charName">The Plan Key (either char name or filename) for the character to add a plan for.</param>
        /// <param name="plan">The plan to add.</param>
        /// <param name="planName">The name to assign to it.</param>
        public void AddPlanFor(string planKeyName, Plan plan, string planName)
        {
            if (FindPlan(planName,planKeyName) != null)
            {
                throw new ApplicationException("That plan already exists.");
            }

            Pair<string, Plan> p = new Pair<string, Plan>();
            if (planName == PLAN_DEFAULT)
            {
                p.A = planKeyName;
            }
            else
            {
                p.A = planKeyName + "::" + planName;
            }
            p.B = plan;
            lock (mutexLock)
            {
                m_plans.Add(p);
                plan.Name = planName;
            }
            this.Save();
        }

        /// <summary>
        /// Removes a plan.
        /// </summary>
        /// <param name="charName">The Plan Key (either char name or filename) for the character whose plan is to be removed.</param>
        /// <param name="planName">The plan name to remove.</param>
        public void RemovePlanFor(string planKeyName, string planName)
        {
            lock (mutexLock)
            {
                for (int i = 0; i < m_plans.Count; i++)
                {
                    if (planName == PLAN_DEFAULT && m_plans[i].A == planKeyName)
                    {
                        Plan p = m_plans[i].B;
                        p.CloseEditor();
                        m_plans.RemoveAt(i);
                        break;
                    }
                    else if (m_plans[i].A == planKeyName + "::" + planName)
                    {
                        Plan p = m_plans[i].B;
                        p.CloseEditor();
                        m_plans.RemoveAt(i);
                        break;
                    }
                }
            }
            this.Save();
        }

        /// <summary>
        /// Renames a plan.
        /// </summary>
        /// <param name="charName">The Plan Key (either char name or filename) for the character whose plan is to be renamed.</param>
        /// <param name="planName">The current plan name.</param>
        /// <param name="newName">The new plan name.</param>
        /// <returns>True if the plan was found and renamed, false if not.</returns>
        public bool RenamePlanFor(string planKeyName, string planName, string newName)
        {
            if (FindPlan(newName,planKeyName) != null)
            {
                return false;
            }

            bool found = false;
            lock (mutexLock)
            {
                for (int i = 0; i < m_plans.Count; i++)
                {
                    if (planName == PLAN_DEFAULT && m_plans[i].A == planKeyName)
                    {
                        m_plans[i].A = planKeyName + "::" + newName;
                        found = true;
                        break;
                    }
                    else if (m_plans[i].A == planKeyName + "::" + planName)
                    {
                        if (newName != PLAN_DEFAULT)
                        {
                            m_plans[i].A = planKeyName + "::" + newName;
                        }
                        else
                        {
                            m_plans[i].A = planKeyName;
                        }
                        found = true;
                        break;
                    }
                }
            }
            this.Save();
            return found;
        }

        /// <summary>
        /// Remove all plans for a character.
        /// </summary>
        /// <param name="charName">The Plan Key (either char name or filename) for the character whose plans are to be removed.</param>
        public void RemoveAllPlansFor(string planKeyName)
        {
            lock (mutexLock)
            {
                for (int i = m_plans.Count - 1; i >= 0; --i)
                {
                    if (m_plans[i].A.StartsWith(planKeyName + "::") || m_plans[i].A == planKeyName)
                    {
                        Plan p = m_plans[i].B;
                        p.CloseEditor();
                        m_plans.RemoveAt(i);
                    }
                }
            }
            this.Save();
        }

        /// <summary>
        /// Rearranges the order of m_plans to match the order of the strings in newOrder
        /// </summary>
        /// <param name="charName">The Plan Key (either char name or filename) for the character whose plans we are to reorder.</param>
        /// <param name="newOrder">The new plan order.</param>
        public void RearrangePlansFor(string planKeyName, List<string> newOrder)
        {
            lock (mutexLock)
            {
                List<Pair<string, Plan>> plans = new List<Pair<string, Plan>>();

                foreach (string planName in newOrder)
                {
                    int index = -1;
                    // Look for plan matching planName
                    for (int i = 0; i < m_plans.Count; i++)
                    {
                        if (m_plans[i].A.StartsWith(planKeyName + "::") || m_plans[i].A == planKeyName)
                        {
                            Pair<string, Plan> tp = m_plans[i];
                            string tPlanName = (tp.A == planKeyName ? PLAN_DEFAULT : tp.A.Substring(tp.A.IndexOf("::") + 2));
                            index = (tPlanName == planName ? i : index);
                        }
                    }
                    // If the plan was found, move it from old list to new
                    if (index != -1)
                    {
                        plans.Add(m_plans[index]);
                        m_plans.RemoveAt(index);
                    }
                }
                m_plans.AddRange(plans);
            }
            this.Save();
        }

        #endregion Plan Settings

        #region Plan Options

        private PlanTextOptions m_defaultCopyOptions = new PlanTextOptions();
        private PlanTextOptions m_defaultSaveOptions = new PlanTextOptions();

        public PlanTextOptions DefaultCopyOptions
        {
            get { return m_defaultCopyOptions; }
            set
            {
                lock (mutexLock)
                {
                    m_defaultCopyOptions = value;
                }
            }
        }

        public PlanTextOptions DefaultSaveOptions
        {
            get { return m_defaultSaveOptions; }
            set
            {
                lock (mutexLock)
                {
                    m_defaultSaveOptions = value;
                }
            }
        }

 #endregion

        #region Pie Chart

        private List<SerializableColor> m_skillPieChartColors = new List<SerializableColor>();

        public List<SerializableColor> SkillPieChartColors
        {
            get { return m_skillPieChartColors; }
            set
            {
                lock (mutexLock)
                {
                    m_skillPieChartColors = value;
                }
            }
        }

        private float m_skillPieChartInitialAngle = -30F;

        public float SkillPieChartInitialAngle
        {
            get { return m_skillPieChartInitialAngle; }
            set
            {
                lock (mutexLock)
                {
                    m_skillPieChartInitialAngle = value;
                }
            }
        }

        private float m_skillPieChartSliceRelativeHeight = 0.15F;

        public float SkillPieChartSliceRelativeHeight
        {
            get { return m_skillPieChartSliceRelativeHeight; }
            set
            {
                lock (mutexLock)
                {
                    m_skillPieChartSliceRelativeHeight = value;
                }
            }
        }

        private bool m_skillPieChartSortBySize = false;

        public bool SkillPieChartSortBySize
        {
            get { return m_skillPieChartSortBySize; }
            set
            {
                lock (mutexLock)
                {
                    m_skillPieChartSortBySize = value;
                }
            }
        }

        private bool m_skillPieChartMergeMinorGroups = false;

        public bool SkillPieChartMergeMinorGroups
        {
            get { return m_skillPieChartMergeMinorGroups; }
            set
            {
                lock (mutexLock)
                {
                    m_skillPieChartMergeMinorGroups = value;
                }
            }
        }

        #endregion

        #region Ignored Updates
        private string m_ignoreUpdateVersion = "0.0.0.0";
        public string IgnoreUpdateVersion
        {
            get { return m_ignoreUpdateVersion; }
            set
            {
                lock (mutexLock)
                {
                    Version v = new Version("0.0.0.0");
                    try
                    {
                        v = new Version(value);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.LogException(e, false);
                    }
                    m_ignoreUpdateVersion = v.ToString();
                }
            }
        }

        #endregion

        #region Schedule Entries

        private List<ScheduleEntry> m_schedule = new List<ScheduleEntry>();

        [XmlArrayItem("simple", typeof(SimpleScheduleEntry))]
        [XmlArrayItem("recur", typeof(RecurringScheduleEntry))]
        public List<ScheduleEntry> Schedule
        {
            get { return m_schedule; }
            set
            {
                lock (mutexLock)
                {
                    m_schedule = value;
                    OnScheduleEntriesChanged();
                }
            }
        }

        public void ScheduleAdd(ScheduleEntry x)
        {
            lock (mutexLock)
            {
                m_schedule.Add(x);
                OnScheduleEntriesChanged();
            }
        }

        public void ScheduleRemoveAt(int i)
        {
            lock (mutexLock)
            {
                m_schedule.RemoveAt(i);
                OnScheduleEntriesChanged();
            }
        }

        private void OnScheduleEntriesChanged()
        {
            lock (mutexLock)
            {
                if (ScheduleEntriesChanged != null)
                {
                    ScheduleEntriesChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler<EventArgs> ScheduleEntriesChanged;

        public bool SkillIsBlockedAt(DateTime time, out string blockingEntry)
        {
            blockingEntry = string.Empty;

            // check if it will be in downtime
            bool isBlocked = (time.ToUniversalTime().Hour == 11);
            if (isBlocked)
            {
                blockingEntry = "DOWNTIME";
                return true;
            }

            // check schedule entries to see if they overlap the imput time
            for (int j = 0; j < Schedule.Count; j++)
            {
                ScheduleEntry temp = Schedule[j];
                if (temp.GetType() == typeof(SimpleScheduleEntry))
                {
                    SimpleScheduleEntry y = (SimpleScheduleEntry)temp;
                    if (y.Blocking(time))
                    {
                        blockingEntry = y.Title;
                        return true;
                    }
                }
                else if (temp.GetType() == typeof(RecurringScheduleEntry))
                {
                    RecurringScheduleEntry y = (RecurringScheduleEntry)temp;
                    if (y.Blocking(time))
                    {
                        blockingEntry = y.Title;
                        return true;
                    }
                }
            }

            return false;
        }


        #endregion

        #region Settings File Management
        #region Properties and Fields
        /// <summary>
        /// Private member to hold currently loaded Settings instance
        /// </summary>
        /// <remarks>
        /// m_instance is only set by either Load() or Restore() and should not be set elsewhere
        /// </remarks>
        private static Settings m_instance = null;

        /// <summary>
        /// Field to hold data directory. Value is initialised by the first read of EveMonDataDir property
        /// </summary>
        private static string m_DataDir = String.Empty;

        /// <summary>
        /// Field to hold the name of the settings file in use. Value initialised by the first read of EveMonDataDir property.
        /// </summary>
        private static string m_SettingsFile = null;

        /// <summary>
        /// Returns the current data storage directory and initialises m_SettingsFile.
        /// </summary>
        public static string EveMonDataDir
        {
            get
            {
                lock (mutexLock)
                {
                    if (m_DataDir == String.Empty)
                    {
                        m_SettingsFile = Path.DirectorySeparatorChar + "settings.xml";
#if DEBUG
                        m_SettingsFile = Path.DirectorySeparatorChar + "settings-debug.xml";
#endif

                        m_DataDir = Directory.GetCurrentDirectory();

                        string fn = m_DataDir + m_SettingsFile;

                        if (!File.Exists(fn))
                        {
                            m_DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EVEMon");
                        }
                        if (!Directory.Exists(m_DataDir))
                        {
                            Directory.CreateDirectory(m_DataDir);
                        }
                        if (!Directory.Exists(Path.Combine(m_DataDir, "cache")))
                        {
                            Directory.CreateDirectory(Path.Combine(m_DataDir, "cache"));
                        }
                    }
                    return m_DataDir;
                }
            }
        }

        /// <summary>
        /// Returns the fully qualified path to the current settings file
        /// </summary>
        public static string SettingsFileName
        {
            get
            {
                return EveMonDataDir + m_SettingsFile;
            }
        }

        /// <summary>
        /// Boolean to prevent temporary Settings objects being saved.
        /// </summary>
        private bool m_neverSave = false;

        /// <summary>
        /// Flag to indicate if a save is pending but not comitted
        /// </summary>
        private bool m_savePending = false;

        /// <summary>
        /// Thread timer used to cache saves for
        /// </summary>
        private System.Threading.Timer m_saveTimer = null;

        #endregion

        #region Public Methods
        /// <summary>
        /// Helper method to return current settings instance. If no instance exists, one is loaded
        /// </summary>
        /// <returns>A Settings object for the currently loaded instance</returns>
        public static Settings GetInstance()
        {
            lock (mutexLock)
            {
                if (m_instance == null)
                    m_instance = Load();
                return m_instance;
            }
        }

        /// <summary>
        /// Loads a settings file from a specified filepath and sets m_instance
        /// </summary>
        /// <param name="filename">The fully qualified filename of the settings file to load</param>
        /// <returns>The Settings object loaded</returns>
        public static Settings Restore(string filename)
        {
            lock (mutexLock)
            {
                m_instance = LoadFromFile(filename);
                m_instance.CheckSettingsVersion(false);
                return m_instance;
            }
        }

        /// <summary>
        /// Copies the current Settings file to the specified location
        /// </summary>
        /// <param name="copyFileName">The fully qualified filename of the destination file</param>
        public static void CopySettings(string copyFileName)
        {
            lock (mutexLock)
            {
                // ensure we have the latest settings saved
                Settings.GetInstance().SaveImmediate();
                File.Copy(Settings.SettingsFileName, copyFileName, true);
            }
        }

        /// <summary>
        /// Saves settings to disk
        /// </summary>
        /// <remarks>
        /// Saves will be cached for 10 seconds to avoid thrashing the disk when this method is called very rapidly
        /// such as at startup. If a save is currently pending, no action is needed. 
        /// </remarks>
        public void Save()
        {
            lock (mutexLock)
            {
                if (!m_savePending)
                {
                    // Set savePending flag
                    m_savePending = true;
                    // Initialise a run-once timer to delay the save by 10 sec
                    m_saveTimer = new System.Threading.Timer(new System.Threading.TimerCallback(SaveCallback), null, 10000, System.Threading.Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Saves settings without the 10 second cache delay
        /// </summary>
        public void SaveImmediate()
        {
            lock (mutexLock)
            {
                // Set save pending flag
                m_savePending = true;
                // Now save to disk
                SaveCallback(null);
            }
        }

        /// <summary>
        /// Creates a backup of a pre-API settings file
        /// </summary>
        public void BackupPreAPISettingsFile()
        {
            try
            {
                if (File.Exists(SettingsFileName))
                {
                    // save the preapi settings file, but only the very very first time.
                    File.Copy(SettingsFileName, SettingsFileName + ".preapi", false);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
            }
        }

        /// <summary>
        /// Sets m_neverSave to true to prevent a file save when using a temporary Settings object
        /// </summary>
        public void NeverSave()
        {
            lock (mutexLock)
            {
                m_neverSave = true;
            }
        }

        /// <summary>
        /// Creates new empty Settings file, overwriting the existing file
        /// </summary>
        public static void Reset()
        {
            Settings s = new Settings();
            s.Save();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads settings from file.
        /// </summary>
        /// <remarks>
        /// This tries to be resilient - if the settings file is 0 length or fails to load, then look for a backup
        /// copy and ask if that is to be used.
        /// Once a settings file is loaded, a backup is taken as a 'last good settings file'
        /// </remarks>
        /// <returns>A Settings object loaded from file</returns>
        private static Settings Load()
        {

            Settings s = null;
            String settingsFile = SettingsFileName;
            String backupFile = settingsFile + ".bak";

            // Check that a settings file or backup exists
            if (File.Exists(settingsFile) || File.Exists(backupFile))
            {
                // Check settings file length
                FileInfo settingsInfo = new FileInfo(settingsFile);
                if (File.Exists(settingsFile) && settingsInfo.Length > 0)
                {
                    // Try to load from this file
                    try
                    {
                        s = LoadFromFile(settingsFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.LogException(e, true);
                    }
                    // If the settings loaded OK, make a backup as 'last good settings'
                    if (s != null)
                    {
                        File.Copy(settingsFile, backupFile, true);
                    }
                }
                if (s == null)
                {
                    // Load failed, so check for backup
                    FileInfo backupInfo = new FileInfo(backupFile);
                    if (File.Exists(backupFile) && backupInfo.Length > 0)
                    {
                        String fileDate = backupInfo.LastWriteTime.ToLocalTime().ToShortDateString() + " at " + backupInfo.LastWriteTime.ToLocalTime().ToShortTimeString();
                        DialogResult dr = MessageBox.Show(String.Format("Your settings file is missing or corrupt. There is a backup available from {0}. Do you want to use the backup file?", fileDate), "Corrupt Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (dr == DialogResult.Yes)
                        {
                            // Try to load from backup file
                            try
                            {
                                s = LoadFromFile(backupFile);
                            }
                            catch (Exception e)
                            {
                                ExceptionHandler.LogException(e, true);
                            }
                            // If the settings loaded OK, copy to the main settings file, then copy back to stamp date
                            if (s != null)
                            {
                                File.Copy(backupFile, settingsFile, true);
                                File.Copy(settingsFile, backupFile, true);
                            }
                            else
                            // Notify user we have a problem
                            {
                                MessageBox.Show("Load from backup failed. A new settings file will be created. You may wish to close down EVEMon and restore a saved copy of your file", "Corrupt Settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                    else
                    // Notify user we have a problem
                    {
                        MessageBox.Show("Your settings file is corrupt, and no backup is available. A new settings file will be created. You may wish to close down EVEMon and restore a saved copy of your file", "Corrupt Settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            // If we don't have a settings by now, either its a fresh install or all file copies are corrupt, so create new settings
            if (s == null)
            {
                s = new Settings(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
            // Make sure the version number is good
            s.CheckSettingsVersion(true);
            s.CalculateChecksums();
            return s;
        }

        /// <summary>
        /// Loads settings from the specified filepath
        /// </summary>
        /// <param name="fileName">The fully qualified filename of the settings file to load</param>
        /// <returns>The Settings object loaded</returns>
        private static Settings LoadFromFile(string fileName)
        {
            Settings result = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Settings));
                result = (Settings)xs.Deserialize(fs);
            }
            return result;
        }


        /// <summary>
        /// Performs that actual save process
        /// </summary>
        /// <remarks>
        /// Saves are only comitted if m_neverSave is false and m_savePending is true
        /// </remarks>
        /// <param name="state">Ignored. Required by the timer callback</param>
        private void SaveCallback(Object state)
        {
            lock (mutexLock)
            {
                if (!m_neverSave && m_savePending)
                {
                    // If the cache timer is running, dispose and reset as its no longer needed
                    if (m_saveTimer != null)
                    {
                        m_saveTimer.Dispose();
                        m_saveTimer = null;
                    }

                    // Save in temp file
                    string tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    //using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain())
                    //using (IsolatedStorageFileStream s = new IsolatedStorageFileStream(StoreFileName(m_key), FileMode.Create, store))
                    using (FileStream s = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
                    {
                        SaveTo(s);
                        s.Flush();
                    }

                    // Overwrite settings file
                    string fn = SettingsFileName;
                    if (File.Exists(fn)) File.Delete(fn);
                    File.Move(tempFileName, fn);

                    // Delete temp file
                    File.Delete(tempFileName);

                    // Reset savePending flag
                    m_savePending = false;
                }
            }
        }

        /// <summary>
        /// Saves settings to the specified stream
        /// </summary>
        /// <param name="s"></param>
        private void SaveTo(Stream s)
        {
            lock (mutexLock)
            {
                XmlSerializer xs = new XmlSerializer(typeof(Settings));
                xs.Serialize(s, Settings.GetInstance());
            }
        }

        #endregion


        #endregion

        #region Datafile checksums

        private List<Pair<string, string>> m_checksums = new List<Pair<string, string>>();

        public List<Pair<string, string>> DatafileChecksums
        {
            get { return m_checksums; }
        }

        /// <summary>
        /// Attempts to find a datafile  - firstly, look in current folder (in case user is roaming on a usb device)
        /// then look in the Application data folder
        /// // If not there, this could be a first run so copy from resources folder in installation directory
        /// </summary>
        public static string FindDatafile(string datafile)
        {
            string filepath = String.Format(
                "{0}{1}{2}",
                Settings.EveMonDataDir,
                Path.DirectorySeparatorChar,
                datafile);

            if (!File.Exists(filepath))
            {
                // file isn;t in current folder, maybe this is a first run so copy from resources folder
                string baseFile = String.Format(
                    "{1}Resources{0}{2}",
                    Path.DirectorySeparatorChar,
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    datafile);

                if (!File.Exists(baseFile))
                {
                    throw new ApplicationException(baseFile + " not found!");
                }

                try
                {
                    File.Copy(baseFile, filepath);
                }
                catch (Exception)
                {
                    throw new ApplicationException("Unable to copy " + baseFile);
                }

            }
            return filepath;
        }


        public void CalculateChecksums()
        {
            try
            {
                m_checksums.Clear();
                m_checksums.Add(GenerateMD5("eve-implants2.xml.gz"));
                m_checksums.Add(GenerateMD5("eve-items2.xml.gz"));
                m_checksums.Add(GenerateMD5("eve-ships2.xml.gz"));
                m_checksums.Add(GenerateMD5("eve-skills2.xml.gz"));
            }
            // don't worry if we cant create MD5  maybe they have FIPS enforced.
            catch (Exception) { }
        }

        private static Pair<string, string> GenerateMD5(string datafile)
        {
            // datafiles will either be in the current folder (if "roaming" from a USB device)
            // or in the same folder as Settings.xml.
            // If they are not there, then this is a first run and they need copying from the installation folder
            // (to allow Vista to update the files)
            

            string filename =  Settings.FindDatafile(datafile);
            
            StringBuilder sb = new StringBuilder();
            if (File.Exists(filename))
            {
                MD5 md5 = MD5.Create();
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    foreach (byte b in md5.ComputeHash(fs))
                        sb.Append(b.ToString("x2").ToLower());
                }
            }
            return new Pair<string, string>(datafile, sb.ToString());
        }

        #endregion

        #region Character Methods

        public bool AddFileCharacter(CharFileInfo cfi)
        {
            lock (mutexLock)
            {
                foreach (CharFileInfo tx in m_charFileList)
                {
                    if (cfi.Filename == tx.Filename)
                    {
                        return false;
                    }
                }
                m_charFileList.Add(cfi);
            }
            this.Save();
            return true;
        }

        public bool AddCharacter(CharLoginInfo cli)
        {
            lock (mutexLock)
            {
                foreach (CharLoginInfo tx in m_characterList)
                {
                    if (cli.CharacterName == tx.CharacterName)
                    {
                        return false;
                    }
                }
                m_characterList.Add(cli);
            }
            this.Save();
            return true;
        }

        public ICharacterSettings GetCharacterSettings(string userName)
        {
            foreach (ICharacterSettings guy in m_characterList)
            {
                if (guy.CharacterName.Equals(userName))
                {
                    return guy;
                }
            }
            foreach (ICharacterSettings guy in m_charFileList)
            {
                if (guy.CharacterName.Equals(userName))
                {
                    return guy;
                }
            }
            return null;
        }

        #endregion

        #region Browser Defaults

        private bool m_ShowT1Items = true;

        public bool ShowT1Items
        {
            get { return m_ShowT1Items; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowT1Items = value;
                }
            }
        }

        private bool m_ShowT2Items = true;

        public bool ShowT2Items
        {
            get { return m_ShowT2Items; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowT2Items = value;
                }
            }
        }

        private bool m_ShowT3Items = true;

        public bool ShowT3Items
        {
            get { return m_ShowT3Items; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowT3Items = value;
                }
            }
        }

        private bool m_ShowNamedItems = true;

        public bool ShowNamedItems
        {
            get { return m_ShowNamedItems; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowNamedItems = value;
                }
            }
        }

        private bool m_ShowOfficerItems = false;

        public bool ShowOfficerItems
        {
            get { return m_ShowOfficerItems; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowOfficerItems = value;
                }
            }
        }

        private bool m_ShowFactionItems = false;

        public bool ShowFactionItems
        {
            get { return m_ShowFactionItems; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowFactionItems = value;
                }
            }
        }

        private bool m_ShowDeadspaceItems = false;

        public bool ShowDeadspaceItems
        {
            get { return m_ShowDeadspaceItems; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowDeadspaceItems = value;
                }
            }
        }

        private bool m_ShowAmarrShips = true;

        public bool ShowAmarrShips
        {
            get { return m_ShowAmarrShips; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowAmarrShips = value;
                }
            }
        }

        private bool m_ShowCaldariShips = true;

        public bool ShowCaldariShips
        {
            get { return m_ShowCaldariShips; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowCaldariShips = value;
                }
            }
        }

        private bool m_ShowGalenteShips = true;

        public bool ShowGallenteShips
        {
            get { return m_ShowGalenteShips; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowGalenteShips = value;
                }
            }
        }

        private bool m_ShowMinmatarShips = true;

        public bool ShowMinmatarShips
        {
            get { return m_ShowMinmatarShips; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowMinmatarShips = value;
                }
            }
        }

        private bool m_ShowFactionShips = true;

        public bool ShowFactionShips
        {
            get { return m_ShowFactionShips; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowFactionShips = value;
                }
            }
        }

        private bool m_ShowOreShips = true;

        public bool ShowOreShips
        {
            get { return m_ShowOreShips; }
            set
            {
                lock (mutexLock)
                {
                    m_ShowOreShips = value;
                }
            }
        }

        private int m_itemSkillFilter = 0;

        public int ItemSkillFilter
        {
            get { return m_itemSkillFilter; }
            set
            {
                lock (mutexLock)
                {
                    m_itemSkillFilter = value;
                }
            }
        }

        private int m_itemSlotFilter = 0;

        public int ItemSlotFilter
        {
            get { return m_itemSlotFilter; }
            set
            {
                lock (mutexLock)
                {
                    m_itemSlotFilter = value;
                }
            }
        }

        private String m_itemBrowserSearch = String.Empty;

        public String ItemBrowserSearch
        {
            get { return m_itemBrowserSearch; }
            set
            {
                lock (mutexLock)
                {
                    m_itemBrowserSearch = value;
                }
            }
        }

        private int m_plannerTab = 0;

        public int PlannerTab
        {
            get { return m_plannerTab; }
            set
            {
                lock (mutexLock)
                {
                    m_plannerTab = value;
                }
            }
        }

        private int m_skillBrowserFilter = 0;

        public int SkillBrowserFilter
        {
            get { return m_skillBrowserFilter; }
            set
            {
                lock (mutexLock)
                {
                    m_skillBrowserFilter = value;
                }
            }
        }

        private bool m_storeBrowserFilters = false;

        public bool StoreBrowserFilters
        {
            get { return m_storeBrowserFilters; }
            set
            {
                lock (mutexLock)
                {
                    m_storeBrowserFilters = value;
                }
            }
        }

        private int m_skillBrowserSort = 0;

        public int SkillBrowserSort
        {
            get { return m_skillBrowserSort; }
            set
            {
                lock (mutexLock)
                {
                    m_skillBrowserSort = value;
                }
            }
        }

        private String m_skillBrowserSearch = String.Empty;

        public String SkillBrowserSearch
        {
            get { return m_skillBrowserSearch; }
            set
            {
                lock (mutexLock)
                {
                    m_skillBrowserSearch = value;
                }
            }
        }

        private bool m_showPrivateSkills = false;

        public bool ShowPrivateSkills
        {
            get { return m_showPrivateSkills; }
            set
            {
                lock (mutexLock)
                {
                    m_showPrivateSkills = value;
                }
            }
        }

        private int m_shipBrowserFilter = 0;

        public int ShipBrowserFilter
        {
            get { return m_shipBrowserFilter; }
            set
            {
                lock (mutexLock)
                {
                    m_shipBrowserFilter = value;
                }
            }
        }

        private String m_shipBrowserSearch = String.Empty;

        public String ShipBrowserSearch
        {
            get { return m_shipBrowserSearch; }
            set
            {
                lock (mutexLock)
                {
                    m_shipBrowserSearch = value;
                }
            }
        }

        #endregion

        #region Display States

        private bool m_mainWindowMenuBarVisible = true;

        public bool MainWindowMenuBarVisible
        {
            get { return m_mainWindowMenuBarVisible; }
            set { m_mainWindowMenuBarVisible = value; }
        }

        private bool m_mainWindowToolBarVisible = false;

        public bool MainWindowToolBarVisible
        {
            get { return m_mainWindowToolBarVisible; }
            set { m_mainWindowToolBarVisible = value; }
        }

        private List<string> m_tabOrderName = new List<string>();

        // List of either CharLoginInfo or CharFileInfo objects in the order
        // we want them displayed
        private List<Object> m_tabOrder = new List<object>();

        // this is what we serialize, but access is via the TabOrder property
        public List<String> TabOrderName
        {
            get { return m_tabOrderName; }
            set
            {
                lock (mutexLock)
                {
                    m_tabOrderName = value;
                }
            }
        }

        [XmlIgnore]
        public List<Object> TabOrder
        {
            get
            {
                lock (mutexLock)
                {
                    // Build list of charFileInfo  and CharLoginInfo objects in the requried tab order
                    foreach (String name in m_tabOrderName)
                    {
                        bool found = false;
                        foreach (CharLoginInfo ci in m_characterList)
                        {
                            if (ci.CharacterName == name)
                            {
                                m_tabOrder.Add(ci);
                                found = true;
                                break;
                            }
                        }
                        if (found) { continue; }

                        foreach (CharFileInfo cfi in m_charFileList)
                        {
                            if (cfi.CharacterName == name)
                            {
                                m_tabOrder.Add(cfi);
                                found = true;
                                break;
                            }
                        }
                    }

                    // Now we know that the tab order contains known characters...
                    // Check if we're missing any
                    foreach (CharLoginInfo cli in m_characterList)
                    {
                        if (!m_tabOrder.Contains(cli))
                        {
                            m_tabOrder.Add(cli);
                        }
                    }
                    foreach (CharFileInfo cfi in m_charFileList)
                    {
                        if (!m_tabOrder.Contains(cfi))
                        {
                            m_tabOrder.Add(cfi);
                        }
                    }

                    // Now reset the tab order name list from the TabOrder list, which will remove
                    // any unknown characetrs from the list
                    SetTabOrderName();
                    return m_tabOrder;
                }
            }
            set
            {
                lock (mutexLock)
                {
                    m_tabOrder = value;
                    SetTabOrderName();
                }
            }
        }

        //  helper method sets the serializable list of tab order names
        //  from the list of charXXInfo tab order
        private void SetTabOrderName()
        {
            lock (mutexLock)
            {
                m_tabOrderName.Clear();
                foreach (Object o in m_tabOrder)
                {
                    CharFileInfo cfi = o as CharFileInfo;
                    CharLoginInfo cli = o as CharLoginInfo;
                    if (cli != null) m_tabOrderName.Add(cli.CharacterName);
                    if (cfi != null) m_tabOrderName.Add(cfi.CharacterName);
                }
            }
        }

        private List<Pair<string, string>> m_collapsedGroups = new List<Pair<string, string>>();

        public List<Pair<string, string>> CollapsedGroups
        {
            get { return m_collapsedGroups; }
        }

        private SerializableDictionary<string, int> m_savedSplitterDistances = new SerializableDictionary<string, int>();

        public SerializableDictionary<string, int> SavedSplitterDistances
        {
            get { return m_savedSplitterDistances; }
            set
            {
                lock (mutexLock)
                {
                    m_savedSplitterDistances = value;
                }
            }
        }

        private SerializableDictionary<string, Rectangle> m_savedWindowLocations = new SerializableDictionary<string, Rectangle>();

        public SerializableDictionary<string, Rectangle> SavedWindowLocations
        {
            get { return m_savedWindowLocations; }
            set
            {
                lock (mutexLock)
                {
                    m_savedWindowLocations = value;
                }
            }
        }

        private int m_loadoutSortColumn = 3;

        public int LoadoutSortColumn
        {
            get { return m_loadoutSortColumn; }
            set { m_loadoutSortColumn = value; }
        }

        private bool m_loadSortAscending = false;

        public bool LoadoutSortAscending
        {
            get { return m_loadSortAscending; }
            set { m_loadSortAscending = value; }
        }
	

	
        #endregion

        #region EVEMon Tips settings
        private List<string> m_confirmedTips = new List<string>();

        public List<string> ConfirmedTips
        {
            get { return m_confirmedTips; }
        }
        #endregion

        #region API Configuration

        private string m_customAPIConfiguration = null;

        public string CustomAPIConfiguration
        {
            get { return m_customAPIConfiguration; }
            set { m_customAPIConfiguration = value; }
        }

        private List<APIConfiguration> m_apiConfigurations = new List<APIConfiguration>();

        public List<APIConfiguration> APIConfigurations
        {
            get
            {
                return m_apiConfigurations;
            }
            set
            {
                lock(mutexLock)
                {
                    m_apiConfigurations = value;
                }
            }
        }

        #endregion
    }

    public enum SystemTrayDisplayOptions
    {
        Never = 1,
        Minimized = 2,
        Always = 3
    }

    [XmlRoot]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSer = new XmlSerializer(typeof(TValue));

            reader.Read();
            reader.ReadStartElement("dictionary");
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            if (this.Count != 0)
            {
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSer = new XmlSerializer(typeof(TValue));

            writer.WriteStartElement("dictionary");
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                valueSer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }

    /// <summary>
    /// Configuration settings for the tray icon popup window
    /// </summary>
    public class TrayPopupConfig
    {
        public bool HideCharNotTraining = true;
        public CharacterGroupings GroupBy = CharacterGroupings.None;
        public SortOrders SortOrder1 = SortOrders.EarliestAtBottom;
        public SortOrders SortOrder2 = SortOrders.EarliestAtBottom;
        public bool ShowSkill = true;
        public bool ShowTimeToCompletion = true;
        public bool ShowCompletionTime = false;
        public bool HighlightConflicts = true;
        public bool ShowWallet = false;
        public bool ShowPortrait = true;
        public PortraitSizes PortraitSize = PortraitSizes.x48;
        public bool ShowWarning = true;
        public bool ShowTQStatus = true;
        public bool ShowEveTime = true;

        public enum CharacterGroupings
        {
            None = 0,
            TrainingAtTop = 1,
            TrainingAtBottom = 2,
            Account = 3
        }

        public enum SortOrders
        {
            EarliestAtBottom = 0,
            EarliestAtTop = 1,
            AlphaFirstAtTop = 2,
            AlphaFirstAtBottom = 3
        }

        public enum PortraitSizes
        {
            x16 = 0,
            x24 = 1,
            x32 = 2,
            x40 = 3,
            x48 = 4,
            x56 = 5,
            x64 = 6
        }

        public TrayPopupConfig Copy()
        {
            return this.MemberwiseClone() as TrayPopupConfig;
        }
    }

    public enum TrayPopupStyles
    {
        PopupForm = 0,
        WindowsTooltip = 1
    }

    // A conversion class to pull the implants out of a settings file created in 2.1 or earlier
    [XmlRoot("logindata2")]
    public class OldSettings
    {
        private List<SerializableCharacterInfo> m_cachedCharacterInfo = new List<SerializableCharacterInfo>();

        public List<SerializableCharacterInfo> CachedCharacterInfo
        {
            get { return m_cachedCharacterInfo; }
        }

        public SerializableCharacterInfo GetCharacterInfo(string charName)
        {
            foreach (SerializableCharacterInfo sci in m_cachedCharacterInfo)
            {
                if (sci.Name == charName)
                {
                    return sci;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Describes the target platform to allow EVEMon to apply different tweaks at runtime
    /// </summary>
    public enum CompatibilityMode
    {
        /// <summary>
        /// Windows and Linux + Wine
        /// </summary>
        Default = 0,
        /// <summary>
        /// Mac OS + Wine
        /// </summary>
        MacOSWine = 1
    }
}

