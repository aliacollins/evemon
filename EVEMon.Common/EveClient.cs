using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using EVEMon.Common.Attributes;
using EVEMon.Common.Data;
using EVEMon.Common.Net;
using EVEMon.Common.Notifications;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    /// <summary>
    /// Provides a controller layer for the application. This class manages API querying, objects lifecycle, etc. 
    /// See it as the entry point of the library and its collections as databases with stored procedures (the public ones).
    /// </summary>
    [EnforceUIThreadAffinity]
    public static partial class EveClient
    {
        private static StreamWriter s_traceStream;
        private static TextWriterTraceListener s_traceListener;
        private static readonly DateTime s_startTime = DateTime.UtcNow;

        private static readonly Object s_pathsInitializationLock = new Object();
        private static readonly Object s_initializationLock = new Object();
        private static bool s_initialized;
        private static bool s_running;
        private static GlobalDatafileCollection s_datafiles;

        #region Initialization and threading

        /// <summary>
        /// Initializes paths, static objects, check and load datafiles, etc.
        /// </summary>
        /// <remarks>May be called more than once without causing redundant operations to occur.</remarks>
        public static void Initialize()
        {
            lock (s_initializationLock)
            {
                if (s_initialized)
                    return;

                s_initialized = true;

                Trace("EveClient.Initialize() - begin");

                // File system paths
                InitializeFileSystemPaths();
                LocalXmlCache.Initialize();

                // Members instantiations
                HttpWebService = new HttpWebService();
                APIProviders = new GlobalAPIProviderCollection();
                MonitoredCharacters = new GlobalMonitoredCharacterCollection();
                CharacterIdentities = new GlobalCharacterIdentityCollection();
                Notifications = new GlobalNotificationCollection();
                Characters = new GlobalCharacterCollection();
                s_datafiles = new GlobalDatafileCollection();
                Accounts = new GlobalAccountCollection();
                EVEServer = new EveServer();

                // Load static datas (min order to follow : skills before anything else, items before certs)
                Trace("Load Datafiles - begin");
                StaticProperties.Load();
                StaticSkills.Load();
                StaticItems.Load();
                StaticCertificates.Load();
                StaticBlueprints.Load();
                Trace("Load Datafiles - done");

                // Network monitoring (connection availability changes)
                NetworkMonitor.Initialize();

                Trace("EveClient.Initialize() - done");
            }
        }

        /// <summary>
        /// Starts the event processing on a multi-threaded model, with the UI actor being the main actor.
        /// </summary>
        /// <param name="mainForm">The main form of the application</param>
        /// <remarks>May be called more than once without causing redundant operations to occur.</remarks>
        public static void Run(Form mainForm)
        {
            Trace("EveClient.Run()");

            s_running = true;
            Dispatcher.Run(new UIActor(mainForm));
            UpdateOnOneSecondTick();
        }

        /// <summary>
        /// Shutdowns the timer
        /// </summary>
        public static void Shutdown()
        {
            Closed = true;
            s_running = false;
            Dispatcher.Shutdown();
        }

        /// <summary>
        /// Gets true whether the client has been shut down.
        /// </summary>
        public static bool Closed { get; private set; }

        #endregion


        #region File paths
        private static string s_settingsFile;
        private static string s_traceFile;

        /// <summary>
        /// Creates the file system paths (settings file name, appdata directory, etc)
        /// </summary>
        /// <remarks>Already called by <see cref="Initialize"/>.</remarks>
        public static void InitializeFileSystemPaths()
        {
            lock (s_pathsInitializationLock)
            {
                // Ensure it is made once only
                if (!String.IsNullOrEmpty(s_settingsFile))
                    return;

#if DEBUG
                s_settingsFile = "settings-debug.xml";
                s_traceFile = "trace-debug.txt";
#else
                s_settingsFile = "settings.xml";
                s_traceFile = "trace.txt";
#endif

                while (true)
                {
                    try
                    {
                        InitializeEVEMonPaths();
                        InitializeDefaultEvePortraitCachePath();
                        return;
                    }
                    catch (UnauthorizedAccessException exc)
                    {
                        string msg = String.Format("An error occurred while EVEMon was looking for its data directory. " +
                        "You may have insufficient rights or a synchronization may be taking place.{0}{0}The message was :{0}{1}", 
                            Environment.NewLine, exc.Message);

                        var result = MessageBox.Show(
                            msg,
                            "Couldn't read the data directory",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Error);

                        if (result == DialogResult.Cancel)
                        {
                            Application.Exit();
                            return;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Retrieves the settings file path
        /// </summary>
        private static void InitializeEVEMonPaths()
        {
            // If settings.xml exists in the app's directory, we use this one
            EVEMonDataDir = Directory.GetCurrentDirectory();
            var settingsFile = Path.Combine(EVEMonDataDir, s_settingsFile);

            // Else, we use %APPDATA%\EVEMon
            if (!File.Exists(settingsFile))
                EVEMonDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EVEMon");
            
            // Create the directory if it does not exist already
            if (!Directory.Exists(EVEMonDataDir))
                Directory.CreateDirectory(EVEMonDataDir);
            
            // Create the cache subfolder
            if (!Directory.Exists(Path.Combine(EVEMonDataDir, "cache")))
                Directory.CreateDirectory(Path.Combine(EVEMonDataDir, "cache"));
        }

        /// <summary>
        /// Retrieves the portrait cache folder, from the game installation.
        /// </summary>
        private static void InitializeDefaultEvePortraitCachePath()
        {
            DefaultEvePortraitCacheFolders = new string[] { };
            string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            EVEApplicationDataDir = String.Format(CultureConstants.DefaultCulture, "{1}{0}CCP{0}EVE", Path.DirectorySeparatorChar, LocalApplicationData);

            // Check folder exists
            if (!Directory.Exists(EVEApplicationDataDir))
                return;

            // Create a pattern that matches anything "*_tranquility"
            // Enumerate files in the EVE cache directory
            DirectoryInfo di = new DirectoryInfo(EVEApplicationDataDir);
            DirectoryInfo[] filesInEveCache = di.GetDirectories("*_tranquility");

            if (filesInEveCache.Length == 0)
                return;

            var evePortraitCacheFolders = new List<string>();

            foreach (var eveDataPath in filesInEveCache)
            {
                string PortraitCache = eveDataPath.Name;
                string evePortraitCacheFolder = String.Format(CultureConstants.DefaultCulture, 
                                                    "{2}{0}{1}{0}cache{0}Pictures{0}Characters",
                                                    Path.DirectorySeparatorChar, 
                                                    PortraitCache,
                                                    EVEApplicationDataDir);
                evePortraitCacheFolders.Add(evePortraitCacheFolder);
            }

            EvePortraitCacheFolders = evePortraitCacheFolders.ToArray();
            DefaultEvePortraitCacheFolders = EvePortraitCacheFolders;
        }

        /// <summary>
        /// Gets or sets the EVE Online installation's default portrait cache folder.
        /// </summary>
        public static string[] DefaultEvePortraitCacheFolders { get; private set; }

        /// <summary>
        /// Gets or sets the portrait cache folder defined by the user.
        /// </summary>
        public static string[] EvePortraitCacheFolders { get; private set; }

        /// <summary>
        /// Set the EVE Online installation's portrait cache folder.
        /// </summary>
        /// <param name="path">location of the folder</param>
        internal static void SetEvePortraitCacheFolder(string[] path)
        {
            EvePortraitCacheFolders = path;
        }

        /// <summary>
        /// Gets or sets the EVE Online application data folder.
        /// </summary>
        public static string EVEApplicationDataDir { get; private set; }
        
        /// <summary>
        /// Returns the current data storage directory and initializes SettingsFile.
        /// </summary>
        public static string EVEMonDataDir { get; private set; }

        /// <summary>
        /// Gets the fully qualified path to the current settings file
        /// </summary>
        public static string SettingsFileName
        {
            get { return Path.Combine(EVEMonDataDir, s_settingsFile); }
        }

        /// <summary>
        /// Gets the fully qualified path to the trace file
        /// </summary>
        public static string TraceFileName
        {
            get { return Path.Combine(EVEMonDataDir, s_traceFile); }
        }

        #endregion


        #region Services

        /// <summary>
        /// Gets an enumeration over the datafiles checksums.
        /// </summary>
        public static GlobalDatafileCollection Datafiles
        {
            get 
            {
                s_datafiles.Refresh();
                return s_datafiles; 
            }
        }

        /// <summary>
        /// Gets the http web service we use to query web services.
        /// </summary>
        public static HttpWebService HttpWebService { get; private set; }

        /// <summary>
        /// Gets the API providers collection.
        /// </summary>
        public static GlobalAPIProviderCollection APIProviders { get; private set; }

        /// <summary>
        /// Gets the EVE server's informations
        /// </summary>
        public static EveServer EVEServer { get; private set; }
        
        /// <summary>
        /// Apply some settings changes
        /// </summary>
        private static void UpdateSettings()
        {
            HttpWebService.State.Proxy = Settings.Proxy;
        }

        #endregion


        #region Accounts management

        /// <summary>
        /// Gets the collection of all known accounts.
        /// </summary>
        public static GlobalAccountCollection Accounts { get; private set; }

        /// <summary>
        /// Gets the collection of all characters
        /// </summary>
        public static GlobalCharacterCollection Characters { get; private set; }

        /// <summary>
        /// Gets the collection of all known character identities. For monitored character sources, see <see cref="MonitoredCharacterSources"/>
        /// </summary>
        public static GlobalCharacterIdentityCollection CharacterIdentities { get; private set; }

        /// <summary>
        /// Gets the collection of all monitored characters
        /// </summary>
        public static GlobalMonitoredCharacterCollection MonitoredCharacters { get; private set; }

        /// <summary>
        /// Gets the collection of notifications.
        /// </summary>
        public static GlobalNotificationCollection Notifications { get; private set; }

        /// <summary>
        /// Everytime the API timer is clicked, we fire this to check whether we need to update the queries
        /// </summary>
        internal static void UpdateOnOneSecondTick()
        {
            if (!s_running)
                return;

            // Updates EVE server status
            EVEServer.UpdateOnOneSecondTick();

            // Updates the accounts
            foreach(var account in Accounts)
            {
                account.UpdateOnOneSecondTick();
            }

            // Updates the characters
            foreach(var character in Characters)
            {
                var ccpCharacter = character as CCPCharacter;
                if (ccpCharacter != null)
                    ccpCharacter.UpdateOnOneSecondTick();
            }

            // Fires the event for subscribers
            if (TimerTick != null)
                TimerTick(null, EventArgs.Empty);

            // Check for settings save
            Settings.UpdateOnOneSecondTick();
        }

        #endregion


        #region Events firing

        /// <summary>
        /// Occurs every second.
        /// </summary>
        public static event EventHandler TimerTick;

        /// <summary>
        /// Occurs when the scheduler entries changed.
        /// </summary>
        public static event EventHandler SchedulerChanged;

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        public static event EventHandler SettingsChanged;

        /// <summary>
        /// Occurs when the collection of accounts changed.
        /// </summary>
        public static event EventHandler AccountCollectionChanged;

        /// <summary>
        /// Occurs when the collection of characters changed.
        /// </summary>
        public static event EventHandler CharacterCollectionChanged;

        /// <summary>
        /// Occurs when the list of characters in an account changed.
        /// </summary>
        public static event EventHandler CharacterListChanged;

        /// <summary>
        /// Occurs when the collection of monitored characters changed.
        /// </summary>
        public static event EventHandler MonitoredCharacterCollectionChanged;

        /// <summary>
        /// Occurs when a character training check on an account have been updated.
        /// </summary>
        public static event EventHandler AccountCharactersSkillInTrainingUpdated;

        /// <summary>
        /// Occurs when the conquerable station list has been updated.
        /// </summary>
        public static event EventHandler ConquerableStationListUpdated;

        /// <summary>
        /// Occurs when one or many queued skills have been completed.
        /// </summary>
        public static event EventHandler<QueuedSkillsEventArgs> QueuedSkillsCompleted;

        /// <summary>
        /// Occurs when one of the character's collection of plans changed.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterPlanCollectionChanged;

        /// <summary>
        /// Occurs when a character changed.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterChanged;

        /// <summary>
        /// Occurs when a character skill queue changed.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterSkillQueueChanged;

        /// <summary>
        /// Occurs when a character's potrait changed.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterPortraitChanged;

        /// <summary>
        /// Occurs when the market orders of a character have been updated.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterMarketOrdersChanged;

        /// <summary>
        /// Occurs when the industry jobs of a character have been updated.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterIndustryJobsChanged;

        /// <summary>
        /// Occurs when the industry jobs of a character have been completed.
        /// </summary>
        public static event EventHandler<IndustryJobsEventArgs> CharacterIndustryJobsCompleted;

        /// <summary>
        /// Occurs when the research points of a character have been updated.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterResearchPointsChanged;
        
        /// <summary>
        /// Occurs when a plan's name changed.
        /// </summary>
        public static event EventHandler<PlanChangedEventArgs> PlanNameChanged;

        /// <summary>
        /// Occurs when a plan changed.
        /// </summary>
        public static event EventHandler<PlanChangedEventArgs> PlanChanged;

        /// <summary>
        /// Fired every time we ping the TQ server status (update pilots online count etc).
        /// </summary>
        public static event EventHandler<EveServerEventArgs> ServerStatusUpdated;

        /// <summary>
        /// Fired every time a notification (API errors, skill completed) is sent.
        /// </summary>
        public static event EventHandler<Notification> NotificationSent;

        /// <summary>
        /// Fired every time a notification (API errors, skill completed) is invalidated.
        /// </summary>
        public static event EventHandler<NotificationInvalidationEventArgs> NotificationInvalidated;

        /// <summary>
        /// Fires the <see cref="SettingsChanged"/> event.
        /// </summary>
        public static void OnSettingsChanged()
        {
            Trace("EveClient.OnSettingsChanged");
            Settings.Save();
            UpdateSettings();
            if (SettingsChanged != null)
                SettingsChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// Fires the <see cref="SchedulerChanged"/> event.
        /// </summary>
        public static void OnSchedulerChanged()
        {
            Trace("EveClient.OnSchedulerChanged");
            Settings.Save();
            if (SchedulerChanged != null)
                SchedulerChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void OnAccountCollectionChanged()
        {
            Trace("EveClient.OnAccountCollectionChanged");
            Settings.Save();
            if (AccountCollectionChanged != null)
                AccountCollectionChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void OnMonitoredCharactersChanged()
        {
            Trace("EveClient.OnMonitoredCharactersChanged");
            Settings.Save();
            if (MonitoredCharacterCollectionChanged != null)
                MonitoredCharacterCollectionChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void OnCharacterCollectionChanged()
        {
            Trace("EveClient.OnCharacterCollectionChanged");
            Settings.Save();
            if (CharacterCollectionChanged != null)
                CharacterCollectionChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the conquerable station list has been updated.
        /// </summary>
        internal static void OnConquerableStationListUpdated()
        {
            Trace("EveClient.OnAccountStatusUpdated");
            if (ConquerableStationListUpdated != null)
                ConquerableStationListUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        internal static void OnCharacterListChanged(Account account)
        {
            Trace("EveClient.OnCharacterListChanged - {0}", account);
            Settings.Save();
            if (CharacterListChanged != null)
                CharacterListChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterChanged(Character character)
        {
            Trace("EveClient.OnCharacterChanged - {0}", character.Name);
            Settings.Save();
            if (CharacterChanged != null)
                CharacterChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterSkillQueueChanged(Character character)
        {
            Trace("EveClient.OnSkillQueueChanged - {0}", character.Name);
            Settings.Save();
            if (CharacterSkillQueueChanged != null)
                CharacterSkillQueueChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skillsCompleted"></param>
        internal static void OnCharacterQueuedSkillsCompleted(Character character, IEnumerable<QueuedSkill> skillsCompleted)
        {
            Trace("EveClient.OnCharacterQueuedSkillsCompleted - {0}", character.Name);
            if (QueuedSkillsCompleted != null)
                QueuedSkillsCompleted(null, new QueuedSkillsEventArgs(character, skillsCompleted));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterMarketOrdersChanged(Character character)
        {
            Trace("EveClient.OnCharacterMarketOrdersChanged - {0}", character.Name);
            Settings.Save();
            if (CharacterMarketOrdersChanged != null)
                CharacterMarketOrdersChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterIndustryJobsChanged(Character character)
        {
            Trace("EveClient.OnCharacterIndustryJobsChanged - {0}", character.Name);
            Settings.Save();
            if (CharacterIndustryJobsChanged != null)
                CharacterIndustryJobsChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="jobsCompleted"></param>
        internal static void OnCharacterIndustryJobsCompleted(Character character, IEnumerable<IndustryJob> jobsCompleted)
        {
            Trace("EveClient.OnCharacterIndustryJobsCompleted - {0}", character.Name);
            if (CharacterIndustryJobsCompleted != null)
                CharacterIndustryJobsCompleted(null, new IndustryJobsEventArgs(character, jobsCompleted));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterResearchPointsChanged(Character character)
        {
            Trace("EveClient.OnCharacterResearchPointsChanged - {0}", character.Name);
            Settings.Save();
            if (CharacterResearchPointsChanged != null)
                CharacterResearchPointsChanged(null, new CharacterChangedEventArgs(character));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterPortraitChanged(Character character)
        {
            Trace("EveClient.OnCharacterPortraitChanged - {0}", character.Name);
            Settings.Save();
            if (CharacterPortraitChanged != null)
                CharacterPortraitChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterPlanCollectionChanged(Character character)
        {
            Trace("EveClient.OnCharacterPlanCollectionChanged - {0}", character.Name);
            Settings.Save();
            if (CharacterPlanCollectionChanged != null)
                CharacterPlanCollectionChanged(null, new CharacterChangedEventArgs(character));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plan"></param>
        internal static void OnPlanChanged(Plan plan)
        {
            Trace("EveClient.OnPlanChanged - {0}", plan.Name);
            Settings.Save();
            if (PlanChanged != null)
                PlanChanged(null, new PlanChangedEventArgs(plan));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plan"></param>
        internal static void OnPlanNameChanged(Plan plan)
        {
            Trace("EveClient.OnPlanNameChanged - {0}", plan.Name);
            Settings.Save();
            if (PlanNameChanged != null)
                PlanNameChanged(null, new PlanChangedEventArgs(plan));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="previousStatus"></param>
        /// <param name="status"></param>
        internal static void OnServerStatusUpdated(EveServer server, ServerStatus previousStatus, ServerStatus status)
        {
            Trace("EveClient.OnServerStatusUpdated");
            if (ServerStatusUpdated != null)
                ServerStatusUpdated(null, new EveServerEventArgs(server, previousStatus, status));
        }

        /// <summary>
        /// Called when all account characters 'skill in training' check has been updated.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="character">The character.</param>
        internal static void OnAccountCharactersSkillInTrainingUpdated(Account account)
        {
            Trace("EveClient.OnAccountCharactersSkillInTrainingUpdated - {0}", account);
            if (AccountCharactersSkillInTrainingUpdated != null)
                AccountCharactersSkillInTrainingUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        internal static void OnNotificationSent(Notification notification)
        {
            Trace("EveClient.OnNotificationSent - {0}", notification);
            if (NotificationSent != null)
                NotificationSent(null, notification);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        internal static void OnNotificationInvalidated(NotificationInvalidationEventArgs args)
        {
            Trace("EveClient.OnNotificationInvalidated");
            if (NotificationInvalidated != null)
                NotificationInvalidated(null, args);
        }

        #endregion 


        #region Diagnostics

        /// <summary>
        /// Sends a message to the trace with the prepended time since
        /// startup.
        /// </summary>
        /// <param name="message">message to trace</param>
        public static void Trace(string message)
        {
            var time = DateTime.UtcNow - s_startTime;
            string timeStr = String.Format(CultureConstants.DefaultCulture,
                "{0:#0}d {1:#0}h {2:00}m {3:00}s > ", time.Days, time.Hours, time.Minutes, time.Seconds);
            System.Diagnostics.Trace.WriteLine(timeStr + message);
        }

        /// <summary>
        /// Sends a message to the trace with the prepended time since
        /// startup, in addition to argument insertind into the format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public static void Trace(string format, params object[] args)
        {
            Trace(String.Format(format, args));
        }

        /// <summary>
        /// Sends a message to the trace with the calling method, time
        /// and the types of any arguments passed to the method.
        /// </summary>
        public static void Trace()
        {
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();
            var parameters = FormatParameters(method.GetParameters());
            var declaringType = method.DeclaringType.ToString().Replace("EVEMon.", String.Empty);

            Trace("{0}.{1}({2})", declaringType, method.Name, parameters);
        }

        /// <summary>
        /// Formats the parameters of a method into a string.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A comma seperated string of paramater types and names.</returns>
        private static string FormatParameters(ParameterInfo[] parameters)
        {
            var paramDetail = new StringBuilder();

            foreach (var param in parameters)
            {
                if (paramDetail.Length != 0)
                    paramDetail.Append(", ");

                paramDetail.AppendFormat("{0} {1}", param.GetType().Name, param.Name);
            }

            return paramDetail.ToString();
        }

        /// <summary>
        /// Starts the logging of trace messages to a file
        /// </summary>
        public static void StartTraceLogging()
        {
            System.Diagnostics.Trace.AutoFlush = true;
            s_traceStream = File.CreateText(EveClient.TraceFileName);
            s_traceListener = new TextWriterTraceListener(s_traceStream);
            System.Diagnostics.Trace.Listeners.Add(s_traceListener);
        }

        /// <summary>
        /// Stops the logging of trace messages to a file
        /// </summary>
        public static void StopTraceLogging()
        {
            System.Diagnostics.Trace.Listeners.Remove(s_traceListener);
            s_traceListener.Close();
            s_traceStream.Close();
        }

        #endregion
    }
}