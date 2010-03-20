using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private static bool s_closed;
        private static bool s_running;

        private static HttpWebService s_httpWebService;
        private static GlobalAccountCollection s_accounts;
        private static GlobalNotificationCollection s_notifications;
        private static GlobalCharacterIdentityCollection s_identities;
        private static GlobalMonitoredCharacterCollection s_sources;
        private static GlobalAPIProviderCollection s_apiProviders;
        private static GlobalDatafileCollection s_datafiles;
        private static GlobalCharacterCollection s_characters;
        private static EveServer s_tranquilityServer;


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
                s_httpWebService = new HttpWebService();
                s_apiProviders = new GlobalAPIProviderCollection();
                s_sources = new GlobalMonitoredCharacterCollection();
                s_identities = new GlobalCharacterIdentityCollection();
                s_notifications = new GlobalNotificationCollection();
                s_characters = new GlobalCharacterCollection();
                s_datafiles = new GlobalDatafileCollection();
                s_accounts = new GlobalAccountCollection();
                s_tranquilityServer = new EveServer();

                // Load static datas (min order to follow : skills before anything else, ships before certs)
                Trace("Load Datafiles - begin");
                StaticProperties.Load();
                StaticSkills.Load();
                StaticItems.Load();
                StaticCertificates.Load();
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
            s_closed = true;
            s_running = false;
            Dispatcher.Shutdown();
        }

        /// <summary>
        /// Gets true whether the client has been shut down.
        /// </summary>
        public static bool Closed
        {
            get { return s_closed; }
        }
        #endregion


        #region File paths
        private static string s_evePortraitCacheFolder;
        private static string s_settingsFile;
        private static string s_dataDir;
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
                        InitializeEvePortraitCachePath();
                        return;
                    }
                    catch (UnauthorizedAccessException exc)
                    {
                        string msg = "An error occured while EVEMon was looking for its data directory. You may have insufficient rights or a synchronization may be taking place.";
                        msg += "\r\n\r\nThe message was :\r\n" + exc.Message;

                        var result = MessageBox.Show(msg, "Couldn't read the data directory", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

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
        /// <returns></returns>
        private static void InitializeEVEMonPaths()
        {
            // If settings.xml exists in the app's directory, we use this one
            s_dataDir = Directory.GetCurrentDirectory();
            var settingsFile = Path.Combine(s_dataDir, s_settingsFile);

            // Else, we use %APPDATA%\EVEMon
            if (!File.Exists(settingsFile))
            {
                s_dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EVEMon");
            }
            // Create the directory if it does not exist already
            if (!Directory.Exists(s_dataDir))
            {
                Directory.CreateDirectory(s_dataDir);
            }
            // Create the cache subfolder
            if (!Directory.Exists(Path.Combine(s_dataDir, "cache")))
            {
                Directory.CreateDirectory(Path.Combine(s_dataDir, "cache"));
            }
        }

        /// <summary>
        /// Retrieves the protrait cache folder, from the game installation.
        /// </summary>
        /// <returns></returns>
        private static void InitializeEvePortraitCachePath()
        {
            s_evePortraitCacheFolder = "";
            string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string EVEApplicationData = String.Format(CultureConstants.DefaultCulture, "{1}{0}CCP{0}EVE", Path.DirectorySeparatorChar, LocalApplicationData);

            // create a pattern that matches anything "*_tranquility"
            string filePattern = "*_tranquility";

            // check folder exists
            if (!Directory.Exists(EVEApplicationData))
                return;

            // enumerate files in the EVE cache directory
            DirectoryInfo di = new DirectoryInfo(EVEApplicationData);
            DirectoryInfo[] filesInEveCache = di.GetDirectories(filePattern);

            if (filesInEveCache.Length > 0)
            {
                string PortraitCache = filesInEveCache[0].Name;
                s_evePortraitCacheFolder = String.Format(CultureConstants.DefaultCulture, "{2}{0}{1}{0}cache{0}Pictures{0}Portraits",
                    Path.DirectorySeparatorChar, PortraitCache, EVEApplicationData);

                return;
            }
        }

        /// <summary>
        /// Gets the EVE Online installation's portait cache folder.
        /// </summary>
        public static string EvePortraitCacheFolder
        {
            get { return s_evePortraitCacheFolder; }
            internal set { s_evePortraitCacheFolder = value; }
        }

        /// <summary>
        /// Returns the current data storage directory and initialises SettingsFile.
        /// </summary>
        public static string EVEMonDataDir
        {
            get { return s_dataDir; }
        }

        /// <summary>
        /// Gets the fully qualified path to the current settings file
        /// </summary>
        public static string SettingsFileName
        {
            get { return Path.Combine(s_dataDir, s_settingsFile); }
        }

        /// <summary>
        /// Gets the fully qualified path to the trace file
        /// </summary>
        public static string TraceFileName
        {
            get { return Path.Combine(s_dataDir, s_traceFile); }
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
        public static HttpWebService HttpWebService
        {
            get { return s_httpWebService; }
        }

        /// <summary>
        /// Gets the API providers collection.
        /// </summary>
        public static GlobalAPIProviderCollection APIProviders
        {
            get { return s_apiProviders; }
        }

        /// <summary>
        /// Gets the tranquility server's informations
        /// </summary>
        public static EveServer TranquilityServer
        {
            get { return s_tranquilityServer; }
        }
        
        /// <summary>
        /// Apply some settings changes
        /// </summary>
        private static void UpdateSettings()
        {
            s_httpWebService.State.Proxy = Settings.Proxy;
        }

        #endregion


        #region Accounts management
        /// <summary>
        /// Gets the collection of all known accounts.
        /// </summary>
        public static GlobalAccountCollection Accounts
        {
            get { return s_accounts; }
        }

        /// <summary>
        /// Gets the collection of all characters
        /// </summary>
        public static GlobalCharacterCollection Characters
        {
            get { return s_characters; }
        }

        /// <summary>
        /// Gets the collection of all known character identities. For monitored character sources, see <see cref="MonitoredCharacterSources"/>
        /// </summary>
        public static GlobalCharacterIdentityCollection CharacterIdentities
        {
            get { return s_identities; }
        }

        /// <summary>
        /// Gets the collection of all monitored characters
        /// </summary>
        public static GlobalMonitoredCharacterCollection MonitoredCharacters
        {
            get { return s_sources; }
        }

        /// <summary>
        /// Gets the collection of notifications.
        /// </summary>
        public static GlobalNotificationCollection Notifications
        {
            get { return s_notifications; }
        }

        /// <summary>
        /// Everytime the API timer is clicked, we fire this to check whether we need to update the queries
        /// </summary>
        internal static void UpdateOnOneSecondTick()
        {
            if (!s_running) return;

            // Updates tranquility status
            s_tranquilityServer.UpdateOnOneSecondTick();

            // Updates the accounts
            foreach(var account in s_accounts)
            {
                account.UpdateOnOneSecondTick();
            }

            // Updates the characters
            foreach(var character in s_characters)
            {
                var ccpCharacter = character as CCPCharacter;
                if (ccpCharacter != null) ccpCharacter.UpdateOnOneSecondTick();
            }

            // Fires the event for subscribers
            if (TimerTick != null) TimerTick(null, EventArgs.Empty);

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
        /// Occurs when the collection of monitored characters changed.
        /// </summary>
        public static event EventHandler MonitoredCharacterCollectionChanged;

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
        /// Occurs when a character's potrait changed.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterPortraitChanged;

        /// <summary>
        /// Occurs when the market orders of a character have been updated.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CharacterMarketOrdersChanged;

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
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterChanged(Character character)
        {
            Trace("EveClient.OnCharacterChanged - " + character.Name);
            Settings.Save();
            if (CharacterChanged != null)
                CharacterChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterMarketOrdersChanged(Character character)
        {
            Trace("EveClient.OnCharacterMarketOrdersChanged - " + character.Name);
            Settings.Save();
            if (CharacterMarketOrdersChanged != null)
                CharacterMarketOrdersChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        internal static void OnCharacterPortraitChanged(Character character)
        {
            Trace("EveClient.OnCharacterPortraitChanged - " + character.Name);
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
            Trace("EveClient.OnCharacterPlanCollectionChanged - " + character.Name);
            Settings.Save();
            if (CharacterPlanCollectionChanged != null)
                CharacterPlanCollectionChanged(null, new CharacterChangedEventArgs(character));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skillsCompleted"></param>
        internal static void OnCharacterQueuedSkillsCompleted(Character character, IEnumerable<QueuedSkill> skillsCompleted)
        {
            Trace("EveClient.OnCharacterQueuedSkillsCompleted - " + character.Name);
            if (QueuedSkillsCompleted != null)
                QueuedSkillsCompleted(null, new QueuedSkillsEventArgs(character, skillsCompleted));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plan"></param>
        internal static void OnPlanChanged(Plan plan)
        {
            Trace("EveClient.OnPlanChanged - " + plan.Name);
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
            Trace("EveClient.OnPlanNameChanged - " + plan.Name);
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
        /// 
        /// </summary>
        /// <param name="notification"></param>
        internal static void OnNotificationSent(Notification notification)
        {
            Trace("EveClient.OnNotificationSent - " + notification.ToString());
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
            string timeStr = String.Format(CultureConstants.DefaultCulture, "{0:#0}d {1:#0}h {2:00}m {3:00}s > ", time.Days, time.Hours, time.Minutes, time.Seconds);
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