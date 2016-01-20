using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml.Xsl;
using EVEMon.Common.Attributes;
using EVEMon.Common.CloudStorageServices;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models.Extended;
using EVEMon.Common.Notifications;
using EVEMon.Common.Scheduling;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
    /// <summary>
    /// The settings class is bound 
    /// </summary>
    [EnforceUIThreadAffinity]
    public static class Settings
    {
        /// <summary>
        /// Flag to indicate if a save is pending but not committed.
        /// </summary>
        private static bool s_savePending;

        /// <summary>
        /// The last time the settings were saved.
        /// </summary>
        private static DateTime s_lastSaveTime;

        /// <summary>
        /// The settings transform
        /// </summary>
        private static XslCompiledTransform s_settingsTransform;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Settings()
        {
            UI = new UISettings();
            G15 = new G15Settings();
            IGB = new IGBSettings();
            Proxy = new ProxySettings();
            Updates = new UpdateSettings();
            Calendar = new CalendarSettings();
            Exportation = new ExportationSettings();
            MarketPricer = new MarketPricerSettings();
            Notifications = new NotificationSettings();
            LoadoutsProvider = new LoadoutsProviderSettings();
            PortableEveInstallations = new PortableEveInstallationsSettings();
            CloudStorageServiceProvider = new CloudStorageServiceProviderSettings();

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateOnOneSecondTick();
        }

        /// <summary>
        /// Gets true if we're currently restoring the settings.
        /// </summary>
        public static bool IsRestoringSettings { get; private set; }


        #region The very settings

        /// <summary>
        /// Gets or sets the compatibility mode.
        /// </summary>
        public static CompatibilityMode Compatibility { get; private set; }

        /// <summary>
        /// Gets the settings for updates.
        /// </summary>
        public static UpdateSettings Updates { get; private set; }

        /// <summary>
        /// Gets the settings for UI (look'n feel)
        /// </summary>
        public static UISettings UI { get; private set; }

        /// <summary>
        /// Gets the settings for the G15 keyboard.
        /// </summary>
        public static G15Settings G15 { get; private set; }

        /// <summary>
        /// Gets the settings for the notifications (alerts).
        /// </summary>
        public static NotificationSettings Notifications { get; private set; }

        /// <summary>
        /// Gets the settings for the portable EVE installations.
        /// </summary>
        public static PortableEveInstallationsSettings PortableEveInstallations { get; private set; }

        /// <summary>
        /// Gets the settings for the network.
        /// </summary>
        public static IGBSettings IGB { get; private set; }

        /// <summary>
        /// Gets the calendar settings.
        /// </summary>
        public static CalendarSettings Calendar { get; private set; }

        /// <summary>
        /// Gets or sets the exportation settings.
        /// </summary>
        public static ExportationSettings Exportation { get; private set; }

        /// <summary>
        /// Gets or sets the custom proxy settings.
        /// </summary>
        public static ProxySettings Proxy { get; private set; }

        /// <summary>
        /// Gets the market pricer settings.
        /// </summary>
        public static MarketPricerSettings MarketPricer { get; private set; }

        /// <summary>
        /// Gets the loadouts provider settings.
        /// </summary>
        public static LoadoutsProviderSettings LoadoutsProvider { get; private set; }

        /// <summary>
        /// Gets the cloud storage service provider settings.
        /// </summary>
        public static CloudStorageServiceProviderSettings CloudStorageServiceProvider { get; private set; }

        #endregion


        #region Serialization - Core - Methods to update to add a property

        /// <summary>
        /// Creates new empty Settings file, overwriting the existing file.
        /// </summary>
        public static void Reset()
        {
            // Append new properties here
            Import(new SerializableSettings());

            // Notifies the client and save
            SaveImmediate();
        }

        /// <summary>
        /// Updates from the provided serialization object
        /// </summary>
        /// <param name="serial">The serializable version of the new settings. May be null (acts as a reset)</param>
        /// <param name="preferencesOnly">When true, only the user preferences will be reimported, not plans, characters, accounts and such.</param>
        public static void Import(SerializableSettings serial, bool preferencesOnly = false)
        {
            // When null, we just reset
            if (serial == null)
            {
                Reset();
                return;
            }

            IsRestoringSettings = true;
            try
            {
                EveMonClient.Trace("Settings.Import - begin");

                // Global settings
                Compatibility = serial.Compatibility;

                // API providers
                EveMonClient.APIProviders.Import(serial.APIProviders);

                // Scheduler
                Scheduler.Import(serial.Scheduler);

                // User settings
                UI = serial.UI;
                G15 = serial.G15;
                IGB = serial.IGB;
                Proxy = serial.Proxy;
                Updates = serial.Updates;
                Calendar = serial.Calendar;
                Exportation = serial.Exportation;
                Notifications = serial.Notifications;
                MarketPricer = serial.MarketPricer;
                LoadoutsProvider = serial.LoadoutsProvider;
                PortableEveInstallations = serial.PortableEveInstallations;
                CloudStorageServiceProvider = serial.CloudStorageServiceProvider;

                // Import the characters, API keys and plans
                if (!preferencesOnly)
                {
                    // The above check prevents the settings form to trigger a 
                    // characters updates since the last queried infos would be lost
                    EveMonClient.ResetCollections();
                    EveMonClient.Characters.Import(serial.Characters);
                    EveMonClient.Characters.ImportPlans(serial.Plans);
                    EveMonClient.MonitoredCharacters.Import(serial.MonitoredCharacters);
                    EveMonClient.APIKeys.Import(serial.APIKeys);
                }

                // Trim the data
                OnImportCompleted();

                // Save
                SaveImmediate();

                // Notify the subscribers
                EveMonClient.OnSettingsChanged();

                EveMonClient.Trace("Settings.Import - done");
            }
            finally
            {
                IsRestoringSettings = false;
            }
        }

        /// <summary>
        /// Corrects the imported data and add missing stuff.
        /// </summary>
        private static void OnImportCompleted()
        {
            // Add missing notification behaviours
            foreach (NotificationCategory category in EnumExtensions.GetValues<NotificationCategory>().Where(
                category => !Notifications.Categories.ContainsKey(category) && category.HasHeader()))
            {
                Notifications.Categories[category] = new NotificationCategorySettings();
            }

            // Add missing API methods update periods
            foreach (Enum method in APIMethods.Methods.Where(method => method.GetUpdatePeriod() != null).Where(
                method => !Updates.Periods.ContainsKey(method.ToString())))
            {
                Updates.Periods.Add(method.ToString(), method.GetUpdatePeriod().DefaultPeriod);

                // Bind the APIKeyInfo and CharacterList update period
                if (method.Equals(CCPAPIGenericMethods.APIKeyInfo) &&
                    Updates.Periods[CCPAPIGenericMethods.CharacterList.ToString()] != Updates.Periods[method.ToString()])
                    Updates.Periods[method.ToString()] = Updates.Periods[CCPAPIGenericMethods.CharacterList.ToString()];
            }

            // Initialize or add missing columns
            InitializeOrAddMissingColumns();

            // Removes redundant notification behaviours
            List<KeyValuePair<NotificationCategory, NotificationCategorySettings>> notifications =
                Notifications.Categories.ToList();
            foreach (KeyValuePair<NotificationCategory, NotificationCategorySettings> notification in notifications.Where(
                notification => !notification.Key.HasHeader()))
            {
                Notifications.Categories.Remove(notification.Key);
            }

            // Removes redundant windows locations
            List<KeyValuePair<string, WindowLocationSettings>> locations = UI.WindowLocations.ToList();
            foreach (KeyValuePair<string, WindowLocationSettings> windowLocation in locations.Where(
                windowLocation => windowLocation.Key == "FeaturesWindow"))
            {
                UI.WindowLocations.Remove(windowLocation.Key);
            }

            // Removes redundant splitters
            List<KeyValuePair<string, int>> splitters = UI.Splitters.ToList();
            foreach (KeyValuePair<string, int> splitter in splitters.Where(
                splitter => splitter.Key == "EFTLoadoutImportationForm"))
            {
                UI.Splitters.Remove(splitter.Key);
            }
        }

        /// <summary>
        /// Initializes or adds missing columns.
        /// </summary>
        private static void InitializeOrAddMissingColumns()
        {
            // Initializes the plan columns or adds missing ones
            UI.PlanWindow.Columns.AddRange(UI.PlanWindow.DefaultColumns);

            // Initializes the asset columns or adds missing ones
            UI.MainWindow.Assets.Columns.AddRange(UI.MainWindow.Assets.DefaultColumns);

            // Initializes the market order columns or adds missing ones
            UI.MainWindow.MarketOrders.Columns.AddRange(UI.MainWindow.MarketOrders.DefaultColumns);

            // Initializes the contracts columns or adds missing ones
            UI.MainWindow.Contracts.Columns.AddRange(UI.MainWindow.Contracts.DefaultColumns);

            // Initializes the wallet journal columns or adds missing ones
            UI.MainWindow.WalletJournal.Columns.AddRange(UI.MainWindow.WalletJournal.DefaultColumns);

            // Initializes the wallet transactions columns or adds missing ones
            UI.MainWindow.WalletTransactions.Columns.AddRange(UI.MainWindow.WalletTransactions.DefaultColumns);

            // Initializes the industry jobs columns or adds missing ones
            UI.MainWindow.IndustryJobs.Columns.AddRange(UI.MainWindow.IndustryJobs.DefaultColumns);

            // Initializes the planetary colonies columns or adds missing ones
            UI.MainWindow.Planetary.Columns.AddRange(UI.MainWindow.Planetary.DefaultColumns);

            // Initializes the research points columns or adds missing ones
            UI.MainWindow.Research.Columns.AddRange(UI.MainWindow.Research.DefaultColumns);

            // Initializes the EVE mail messages columns or adds missing ones
            UI.MainWindow.EVEMailMessages.Columns.AddRange(UI.MainWindow.EVEMailMessages.DefaultColumns);

            // Initializes the EVE notifications columns or adds missing ones
            UI.MainWindow.EVENotifications.Columns.AddRange(UI.MainWindow.EVENotifications.DefaultColumns);
        }

        /// <summary>
        /// Creates a serializable version of the settings.
        /// </summary>
        /// <returns></returns>
        public static SerializableSettings Export()
        {
            SerializableSettings serial = new SerializableSettings
            {
                Revision = Revision,
                Compatibility = Compatibility,
                APIProviders = EveMonClient.APIProviders.Export(),
                Scheduler = Scheduler.Export(),
                Calendar = Calendar,
                CloudStorageServiceProvider = CloudStorageServiceProvider,
                PortableEveInstallations = PortableEveInstallations,
                LoadoutsProvider = LoadoutsProvider,
                MarketPricer = MarketPricer,
                Notifications = Notifications,
                Exportation = Exportation,
                Updates = Updates,
                Proxy = Proxy,
                IGB = IGB,
                G15 = G15,
                UI = UI
            };

            serial.Characters.AddRange(EveMonClient.Characters.Export());
            serial.APIKeys.AddRange(EveMonClient.APIKeys.Export());
            serial.Plans.AddRange(EveMonClient.Characters.ExportPlans());
            serial.MonitoredCharacters.AddRange(EveMonClient.MonitoredCharacters.Export());

            return serial;
        }

        #endregion


        #region Initialization and loading

        /// <summary>
        /// Gets the current assembly's revision, which is also used for files versioning.
        /// </summary>
        internal static int Revision
        {
            get { return Version.Parse(EveMonClient.FileVersionInfo.FileVersion).Revision; }
        }

        /// <summary>
        /// Initialization for the EVEMon client settings.
        /// </summary>
        /// <remarks>
        /// Will attempt to fetch and initialize settings from a storage server, if user has specified so.
        /// Otherwise attempts to initialize from a locally stored file.
        /// </remarks>
        public static void Initialize()
        {
            CloudStorageServiceAPIFile settingsFile = CloudStorageServiceProvider.Provider?.DownloadSettingsFile();
            SerializableSettings settings = settingsFile != null
                ? TryDeserializeFromFileContent(settingsFile.FileContent)
                : TryDeserializeFromFile();

            // Loading settings failed, we create settings from scratch
            if (settings == null)
                Reset();
            else
                Import(settings);
        }

        /// <summary>
        /// Try to deserialize the settings from a storage server file, prompting the user for errors.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        /// <returns>
        ///   <c>Null</c> if we have been unable to deserialize anything, the generated settings otherwise
        /// </returns>
        private static SerializableSettings TryDeserializeFromFileContent(string fileContent)
        {
            if (String.IsNullOrWhiteSpace(fileContent))
                return null;

            EveMonClient.Trace("Settings.TryDeserializeFromFileContent - begin");

            // Gets the revision number of the assembly which generated this file
            int revision = Util.GetRevisionNumber(fileContent);

            // Try to load from a file (when no revision found then it's a pre 1.3.0 version file)
            SerializableSettings settings = revision == 0
                ? (SerializableSettings)UIHelper.ShowNoSupportMessage()
                : Util.DeserializeXmlFromString<SerializableSettings>(fileContent,
                    SettingsTransform);

            if (settings != null)
            {
                EveMonClient.Trace("Settings.TryDeserializeFromFileContent - done");
                return settings;
            }

            const string Caption = "Corrupt Settings";

            DialogResult dr = MessageBox.Show("Loading settings from the storage server file failed.\n" +
                                              @"Do you want to use the local settings file?",
                Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (dr != DialogResult.No)
                return TryDeserializeFromFile();

            MessageBox.Show("A new settings file will be created.\n"
                            + "You may wish then to restore a saved copy of the file.", Caption,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            return null;
        }

        /// <summary>
        /// Loads a settings file from a specified filepath.
        /// </summary>
        /// <param name="filename">The fully qualified filename of the settings file to load</param>
        /// <returns>The Settings object loaded</returns>
        public static void Restore(string filename)
        {
            // Try deserialize
            SerializableSettings settings = TryDeserializeFromBackupFile(filename, false);

            // Loading from file failed, we abort and keep our current settings
            if (settings == null)
                return;

            // Updates and save
            Import(settings);
        }

        /// <summary>
        /// Try to deserialize the settings from a file, prompting the user for errors.
        /// </summary>
        /// <returns><c>Null</c> if we have been unable to load anything from files, the generated settings otherwise</returns>
        private static SerializableSettings TryDeserializeFromFile()
        {
            string settingsFile = EveMonClient.SettingsFileNameFullPath;
            string backupFile = settingsFile + ".bak";

            // If settings file doesn't exists
            // try to recover from the backup
            if (!File.Exists(settingsFile))
                return TryDeserializeFromBackupFile(backupFile);

            EveMonClient.Trace("Settings.TryDeserializeFromFile - begin");

            // Check settings file length
            FileInfo settingsInfo = new FileInfo(settingsFile);
            if (settingsInfo.Length == 0)
                return TryDeserializeFromBackupFile(backupFile);

            // Get the revision number of the assembly which generated this file
            // Try to load from a file (when no revision found then it's a pre 1.3.0 version file)
            SerializableSettings settings = Util.GetRevisionNumber(settingsFile) == 0
                ? (SerializableSettings)UIHelper.ShowNoSupportMessage()
                : Util.DeserializeXmlFromFile<SerializableSettings>(settingsFile,
                    SettingsTransform);

            // If the settings loaded OK, make a backup as 'last good settings' and return
            if (settings == null)
                return TryDeserializeFromBackupFile(backupFile);

            CheckSettingsVersion(settings);
            FileHelper.OverwriteOrWarnTheUser(settingsFile, backupFile);
            EveMonClient.Trace("Settings.TryDeserializeFromFile - done");
            return settings;
        }

        /// <summary>
        /// Try to deserialize from the backup file.
        /// </summary>
        /// <param name="backupFile">The backup file.</param>
        /// <param name="recover">if set to <c>true</c> do a settings recover attempt.</param>
        /// <returns>
        /// 	<c>Null</c> if we have been unable to load anything from files, the generated settings otherwise
        /// </returns>
        private static SerializableSettings TryDeserializeFromBackupFile(string backupFile, bool recover = true)
        {
            // Backup file doesn't exist
            if (!File.Exists(backupFile))
                return null;

            EveMonClient.Trace("Settings.TryDeserializeFromBackupFile - begin");

            // Check backup settings file length
            FileInfo backupInfo = new FileInfo(backupFile);
            if (backupInfo.Length == 0)
                return null;

            string settingsFile = EveMonClient.SettingsFileNameFullPath;

            const string Caption = "Corrupt Settings";
            if (recover)
            {
                // Prompts the user to use the backup
                string fileDate = String.Format(CultureConstants.DefaultCulture, "{0} at {1}",
                    backupInfo.LastWriteTime.ToLocalTime().ToShortDateString(),
                    backupInfo.LastWriteTime.ToLocalTime().ToShortTimeString());
                DialogResult dialogResult = MessageBox.Show(
                    String.Format(CultureConstants.DefaultCulture,
                        "The settings file is missing or corrupt. There is a backup available from {0}.\n" +
                        "Do you want to use the backup file?",
                        fileDate), Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("A new settings file will be created.\n"
                                    + "You may wish then to restore a saved copy of the file.", Caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    // Save a copy of the corrupt file just in case
                    FileHelper.OverwriteOrWarnTheUser(backupFile, settingsFile + ".corrupt");

                    return null;
                }
            }

            // Get the revision number of the assembly which generated this file
            // Try to load from a file (when no revision found then it's a pre 1.3.0 version file)
            SerializableSettings settings = Util.GetRevisionNumber(backupFile) == 0
                ? (SerializableSettings)UIHelper.ShowNoSupportMessage()
                : Util.DeserializeXmlFromFile<SerializableSettings>(backupFile,
                    SettingsTransform);

            // If the settings loaded OK, copy to the main settings file, then copy back to stamp date
            if (settings != null)
            {
                CheckSettingsVersion(settings);
                FileHelper.OverwriteOrWarnTheUser(backupFile, settingsFile);
                FileHelper.OverwriteOrWarnTheUser(settingsFile, backupFile);
                EveMonClient.Trace("Settings.TryDeserializeFromBackupFile - done");
                return settings;
            }

            if (recover)
            {
                // Backup failed too, notify the user we have a problem
                MessageBox.Show("Loading from backup failed.\nA new settings file will be created.\n"
                                + "You may wish then to restore a saved copy of the file.",
                    Caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Save a copy of the corrupt file just in case
                FileHelper.OverwriteOrWarnTheUser(backupFile, settingsFile + ".corrupt");
            }
            else
            {
                // Restoring from file failed
                MessageBox.Show(String.Format(CultureConstants.DefaultCulture,
                    "Restoring settings from {0} failed, the file is corrupted.", backupFile),
                    Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return null;
        }

        /// <summary>
        /// Compare the settings version with this version and, when different, update and prompt the user for a backup.
        /// </summary>
        /// <param name="settings"></param>
        private static void CheckSettingsVersion(SerializableSettings settings)
        {
            if (EveMonClient.IsDebugBuild)
                return;

            if (Revision == settings.Revision)
                return;

            DialogResult backupSettings =
                MessageBox.Show("The current EVEMon settings file is from a previous version.\n" +
                                "Backup the current file before proceeding (recommended)?",
                    "EVEMon version changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

            if (backupSettings != DialogResult.Yes)
                return;

            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Title = @"Settings file backup";
                fileDialog.Filter = @"Settings Backup Files (*.bak)|*.bak";
                fileDialog.FileName = String.Format(CultureConstants.DefaultCulture,
                    "EVEMon_Settings_{0}.xml.bak", settings.Revision);
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                if (fileDialog.ShowDialog() != DialogResult.OK)
                    return;

                FileHelper.OverwriteOrWarnTheUser(EveMonClient.SettingsFileNameFullPath, fileDialog.FileName);
            }
        }

        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by <see cref="XmlSerializer"/>
        /// </summary>
        private static XslCompiledTransform SettingsTransform
        {
            get { return s_settingsTransform ?? (s_settingsTransform = Util.LoadXslt(Properties.Resources.SettingsXSLT)); }
        }

        #endregion


        #region Save

        /// <summary>
        /// Every timer tick, checks whether we should save the settings every 10s.
        /// </summary>
        private static void UpdateOnOneSecondTick()
        {
            // Is a save requested and is the last save older than 10s ?
            if (s_savePending && DateTime.UtcNow > s_lastSaveTime.AddSeconds(10))
                SaveImmediate();
        }

        /// <summary>
        /// Saves settings to disk.
        /// </summary>
        /// <remarks>
        /// Saves will be cached for 10 seconds to avoid thrashing the disk when this method is called very rapidly
        /// such as at startup. If a save is currently pending, no action is needed. 
        /// </remarks>
        public static void Save()
        {
            s_savePending = true;
        }

        /// <summary>
        /// Saves settings immediately.
        /// </summary>
        public static void SaveImmediate()
        {
            SerializableSettings settings = Export();

            // Save in settings file
            FileHelper.OverwriteOrWarnTheUser(EveMonClient.SettingsFileNameFullPath,
                fs =>
                {
                    XmlSerializer xs = new XmlSerializer(typeof(SerializableSettings));
                    xs.Serialize(fs, settings);
                    fs.Flush();
                    return true;
                });

            // Reset savePending flag
            s_lastSaveTime = DateTime.UtcNow;
            s_savePending = false;
        }

        /// <summary>
        /// Copies the current Settings file to the specified location.
        /// </summary>
        /// <param name="copyFileName">The fully qualified filename of the destination file</param>
        public static void CopySettings(string copyFileName)
        {
            // Ensure we have the latest settings saved
            SaveImmediate();
            FileHelper.OverwriteOrWarnTheUser(EveMonClient.SettingsFileNameFullPath, copyFileName);
        }

        #endregion
    }
}