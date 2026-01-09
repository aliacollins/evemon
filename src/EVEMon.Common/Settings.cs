using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml.Xsl;
using EVEMon.Common.Attributes;
using EVEMon.Common.CloudStorageServices;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Enumerations.UISettings;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models.Extended;
using EVEMon.Common.Notifications;
using EVEMon.Common.Scheduling;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;
using Newtonsoft.Json;

namespace EVEMon.Common
{
    /// <summary>
    /// Stores EVEMon's current settings and writes them to the settings file when necessary.
    /// </summary>
    [EnforceUIThreadAffinity]
    public static class Settings
    {
        private static bool s_savePending;
        private static DateTime s_nextSaveTime;
        private static XslCompiledTransform s_settingsTransform;
        private static SerializableSettings s_settings;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Settings()
        {
            // ESI credentials loaded from esi-credentials.json (gitignored)
            // Create your own at https://developers.eveonline.com/
            LoadESICredentials();
            UI = new UISettings();
            G15 = new G15Settings();
            Proxy = new ProxySettings();
            Updates = new UpdateSettings();
            Calendar = new CalendarSettings();
            Exportation = new ExportationSettings();
            MarketPricer = new MarketPricerSettings();
            Notifications = new NotificationSettings();
            LoadoutsProvider = new LoadoutsProviderSettings();
            PortableEveInstallations = new PortableEveInstallationsSettings();
            CloudStorageServiceProvider = new CloudStorageServiceProviderSettings();

            // Use ThirtySecondTick instead of TimerTick for save checks
            // Saves are already delayed by 10 seconds, so checking every 30s is sufficient
            EveMonClient.ThirtySecondTick += EveMonClient_TimerTick;
        }

        // Default ESI credentials for EVEMon - registered by Alia Collins
        // Users can override via esi-credentials.json in app directory
        private const string DefaultClientID = "e87550c5642e4de0bac3b124d110ca7a";
        private const string DefaultClientSecret = "eat_qpDb4LCQRKRcGWKNfoLhcrRlqQo75Aes_3fgYhF";

        /// <summary>
        /// Gets whether a migration from another EVEMon fork was detected on startup.
        /// If true, ESI tokens were cleared and users need to re-add their characters.
        /// </summary>
        public static bool MigrationFromOtherForkDetected { get; private set; }

        // Our fork identifier - used to detect settings from other EVEMon forks
        private const string OurForkId = "aliacollins";

        /// <summary>
        /// Result of fork migration detection.
        /// </summary>
        private class MigrationDetectionResult
        {
            public bool MigrationDetected { get; set; }
            public bool NeedsForkIdAdded { get; set; }
            public bool HasEsiKeys { get; set; }
            public string DetectedForkId { get; set; }
            public int DetectedRevision { get; set; }
        }

        // Revision threshold for detecting peterhaneve's fork
        // peterhaneve uses auto-incrementing build numbers (e.g., 4986)
        // Our fork uses 0 for stable, 1-N for betas
        private const int PeterhaneveRevisionThreshold = 1000;

        /// <summary>
        /// Detects if the settings file is from another EVEMon fork.
        /// Uses forkId and revision number to distinguish between:
        /// - Our users (forkId matches OR forkId missing with low revision)
        /// - peterhaneve users (forkId missing with high revision > 1000)
        /// - Other fork users (forkId present but different)
        /// This method only detects - it does NOT show any UI or modify files.
        /// </summary>
        /// <param name="fileContent">The raw settings.xml content.</param>
        /// <returns>Detection result.</returns>
        private static MigrationDetectionResult DetectForkMigration(string fileContent)
        {
            var result = new MigrationDetectionResult();

            // Check for forkId attribute in the Settings root element
            var forkIdMatch = Regex.Match(fileContent, @"<Settings[^>]*\sforkId=""([^""]+)""",
                RegexOptions.IgnoreCase);
            string forkId = forkIdMatch.Success ? forkIdMatch.Groups[1].Value : null;
            result.DetectedForkId = forkId;

            // Get revision number for distinguishing forks when forkId is missing
            // peterhaneve uses high revision numbers (e.g., 4986)
            // Our fork uses 0 for stable, 1-N for betas
            int revision = Util.GetRevisionNumber(fileContent);
            result.DetectedRevision = revision;

            // Check if there are any ESI keys with refresh tokens
            var hasEsiKeys = Regex.IsMatch(fileContent, @"<esikey[^>]+refreshToken=""[^""]+""",
                RegexOptions.IgnoreCase);
            result.HasEsiKeys = hasEsiKeys;

            EveMonClient.Trace($"DetectForkMigration: forkId='{forkId ?? "(none)"}', revision={revision}, hasEsiKeys={hasEsiKeys}");

            // Detection logic:
            // 1. forkId == "aliacollins" → Our user, no migration
            // 2. forkId present AND different → Migration from that fork
            // 3. forkId missing:
            //    - revision > 1000 → peterhaneve user (they use high build numbers) → Migration
            //    - revision <= 1000 → Our existing user (pre-forkId) → Just add forkId silently

            if (forkId == OurForkId)
            {
                // Case 1: Our fork with forkId - no migration needed
                EveMonClient.Trace("DetectForkMigration: forkId matches ours, no migration");
                result.MigrationDetected = false;
            }
            else if (forkId != null && forkId != OurForkId)
            {
                // Case 2: Different forkId explicitly set - definite migration from another fork
                EveMonClient.Trace($"DetectForkMigration: Different forkId '{forkId}' detected");
                if (hasEsiKeys)
                {
                    EveMonClient.Trace("DetectForkMigration: MIGRATION DETECTED - different forkId with ESI keys");
                    result.MigrationDetected = true;
                }
                else
                {
                    // Different fork but no ESI keys - just update forkId
                    EveMonClient.Trace("DetectForkMigration: Different forkId but no ESI keys, just need to update forkId");
                    result.MigrationDetected = false;
                    result.NeedsForkIdAdded = true;
                }
            }
            else if (forkId == null)
            {
                // Case 3: forkId missing - use revision to distinguish
                if (revision > PeterhaneveRevisionThreshold && hasEsiKeys)
                {
                    // High revision (peterhaneve uses ~4986) + ESI keys = peterhaneve user
                    EveMonClient.Trace($"DetectForkMigration: MIGRATION DETECTED - high revision ({revision}) indicates peterhaneve fork");
                    result.MigrationDetected = true;
                }
                else
                {
                    // Low revision (our fork uses 0-N) = our existing user pre-forkId
                    // Just need to add forkId silently, no migration message
                    EveMonClient.Trace($"DetectForkMigration: Low revision ({revision}), assuming our existing user");
                    result.MigrationDetected = false;
                    result.NeedsForkIdAdded = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the settings file: clears ESI keys and adds our forkId/forkVersion.
        /// Called after migration is detected.
        /// </summary>
        /// <param name="fileContent">The original file content.</param>
        /// <param name="filePath">The path to the settings file.</param>
        /// <returns>The modified content.</returns>
        private static string UpdateSettingsFileForMigration(string fileContent, string filePath)
        {
            string forkVersion = EveMonClient.FileVersionInfo?.FileVersion ?? "5.1.0";

            // Clear ESI keys
            string modifiedContent = Regex.Replace(fileContent,
                @"<esiKeys>.*?</esiKeys>",
                "<esiKeys></esiKeys>",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Add or update forkId attribute on the Settings element
            if (Regex.IsMatch(modifiedContent, @"<Settings[^>]*\sforkId=""[^""]*""", RegexOptions.IgnoreCase))
            {
                // Update existing forkId
                modifiedContent = Regex.Replace(modifiedContent,
                    @"(<Settings[^>]*\s)forkId=""[^""]*""",
                    $"$1forkId=\"{OurForkId}\"",
                    RegexOptions.IgnoreCase);
            }
            else
            {
                // Add forkId attribute
                modifiedContent = Regex.Replace(modifiedContent,
                    @"<Settings\s",
                    $"<Settings forkId=\"{OurForkId}\" ",
                    RegexOptions.IgnoreCase);
            }

            // Add or update forkVersion attribute
            if (Regex.IsMatch(modifiedContent, @"<Settings[^>]*\sforkVersion=""[^""]*""", RegexOptions.IgnoreCase))
            {
                // Update existing forkVersion
                modifiedContent = Regex.Replace(modifiedContent,
                    @"(<Settings[^>]*\s)forkVersion=""[^""]*""",
                    $"$1forkVersion=\"{forkVersion}\"",
                    RegexOptions.IgnoreCase);
            }
            else
            {
                // Add forkVersion attribute after forkId
                modifiedContent = Regex.Replace(modifiedContent,
                    @"(<Settings[^>]*forkId=""[^""]*"")",
                    $"$1 forkVersion=\"{forkVersion}\"",
                    RegexOptions.IgnoreCase);
            }

            try
            {
                File.WriteAllText(filePath, modifiedContent);
                EveMonClient.Trace($"UpdateSettingsFileForMigration: Cleared ESI keys and set forkId={OurForkId}, forkVersion={forkVersion}");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"UpdateSettingsFileForMigration: Failed to update settings file: {ex.Message}");
            }

            return modifiedContent;
        }

        /// <summary>
        /// Adds our forkId and forkVersion to a settings file that doesn't have them.
        /// Called for fresh installs or when forkId is missing but no migration needed.
        /// </summary>
        /// <param name="fileContent">The original file content.</param>
        /// <param name="filePath">The path to the settings file.</param>
        /// <returns>The modified content.</returns>
        private static string AddForkIdToSettingsFile(string fileContent, string filePath)
        {
            string forkVersion = EveMonClient.FileVersionInfo?.FileVersion ?? "5.1.0";
            string modifiedContent = fileContent;
            bool modified = false;

            // Add forkId if missing
            if (!Regex.IsMatch(modifiedContent, @"<Settings[^>]*\sforkId=""[^""]*""", RegexOptions.IgnoreCase))
            {
                modifiedContent = Regex.Replace(modifiedContent,
                    @"<Settings\s",
                    $"<Settings forkId=\"{OurForkId}\" ",
                    RegexOptions.IgnoreCase);
                modified = true;
            }

            // Add forkVersion if missing
            if (!Regex.IsMatch(modifiedContent, @"<Settings[^>]*\sforkVersion=""[^""]*""", RegexOptions.IgnoreCase))
            {
                // Add forkVersion after forkId
                modifiedContent = Regex.Replace(modifiedContent,
                    @"(<Settings[^>]*forkId=""[^""]*"")",
                    $"$1 forkVersion=\"{forkVersion}\"",
                    RegexOptions.IgnoreCase);
                modified = true;
            }

            if (!modified)
            {
                return fileContent; // Nothing to add
            }

            try
            {
                File.WriteAllText(filePath, modifiedContent);
                EveMonClient.Trace($"AddForkIdToSettingsFile: Added forkId={OurForkId}, forkVersion={forkVersion}");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"AddForkIdToSettingsFile: Failed to update settings file: {ex.Message}");
            }

            return modifiedContent;
        }

        /// <summary>
        /// Shows the appropriate migration message based on detection results and whether settings can be preserved.
        /// </summary>
        /// <param name="migration">The migration detection result.</param>
        /// <param name="settingsCanBePreserved">Whether the settings were successfully loaded and can be preserved.</param>
        private static void ShowMigrationMessage(MigrationDetectionResult migration, bool settingsCanBePreserved)
        {
            string message;
            string title = "Welcome to EVEMon 5.1";

            if (settingsCanBePreserved)
            {
                // Settings format is compatible - plans and settings will be preserved
                message = @"Welcome to EVEMon 5.1!

It looks like you're coming from a different version of EVEMon.

Due to how EVE's login system works, your characters need to be re-added. This is a one-time thing - ESI authentication tokens are tied to the specific EVEMon version that created them and cannot be transferred.

Your skill plans and other settings have been preserved.

To re-add your characters:
  1. Go to File > Add Character
  2. Log in with your EVE account
  3. Repeat for each character

Click OK to continue.";
            }
            else
            {
                // Settings format is incompatible - starting fresh
                message = @"Welcome to EVEMon 5.1!

It looks like you're coming from a different version of EVEMon.

Unfortunately, your settings file format is too old to migrate. EVEMon will start with fresh settings.

You'll need to add your characters:
  1. Go to File > Add Character
  2. Log in with your EVE account
  3. Repeat for each character

Click OK to continue.";
            }

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            MigrationFromOtherForkDetected = true;
        }

        /// <summary>
        /// Loads ESI credentials - uses embedded defaults, can be overridden via esi-credentials.json.
        /// </summary>
        private static void LoadESICredentials()
        {
            // Start with embedded defaults
            SSOClientID = DefaultClientID;
            SSOClientSecret = DefaultClientSecret;

            // Look for override file in application directory
            string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "esi-credentials.json");

            if (!File.Exists(credentialsPath))
            {
                // Also check parent directories for development scenarios
                string devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "esi-credentials.json");
                if (File.Exists(devPath))
                    credentialsPath = devPath;
            }

            // Override with file credentials if present
            if (File.Exists(credentialsPath))
            {
                try
                {
                    string json = File.ReadAllText(credentialsPath);
                    var credentials = JsonConvert.DeserializeAnonymousType(json, new { ClientID = "", ClientSecret = "" });
                    if (!string.IsNullOrEmpty(credentials?.ClientID))
                        SSOClientID = credentials.ClientID;
                    if (!string.IsNullOrEmpty(credentials?.ClientSecret))
                        SSOClientSecret = credentials.ClientSecret;
                }
                catch
                {
                    // Failed to load override, continue with defaults
                }
            }
        }

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static async void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            await UpdateOnOneSecondTickAsync();
        }

        /// <summary>
        /// Gets true if we're currently restoring the settings.
        /// </summary>
        public static bool IsRestoring { get; private set; }


        #region The very settings

        /// <summary>
        /// Gets or sets the SSO client ID.
        /// </summary>
        public static string SSOClientID { get; private set; }

        /// <summary>
        /// Gets or sets the SSO secret key.
        /// </summary>
        public static string SSOClientSecret { get; private set; }

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
        public static async Task ResetAsync()
        {
            // Clear JSON files (they'll be recreated empty on next save)
            SettingsFileManager.ClearAllJsonFiles();

            s_settings = new SerializableSettings();

            IsRestoring = true;
            Import();
            await ImportDataAsync();
            IsRestoring = false;
        }

        /// <summary>
        /// Asynchronously imports the settings.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <param name="saveImmediate">if set to <c>true</c> [save immediate].</param>
        /// <returns></returns>
        public static async Task ImportAsync(SerializableSettings serial, bool saveImmediate = false)
        {
            s_settings = serial;

            Import();
            IsRestoring = true;
            if (saveImmediate)
                await SaveImmediateAsync();
            IsRestoring = false;
        }

        /// <summary>
        /// Imports the provided serialization object.
        /// </summary>
        private static void Import()
        {
            EveMonClient.Trace("begin");

            // When null, we just reset
            if (s_settings == null)
                s_settings = new SerializableSettings();

            try
            {
                // API settings
                SSOClientID = s_settings.SSOClientID ?? string.Empty;
                SSOClientSecret = s_settings.SSOClientSecret ?? string.Empty;

                // User settings
                UI = s_settings.UI;
                G15 = s_settings.G15;
                Proxy = s_settings.Proxy;
                Updates = s_settings.Updates;
                Calendar = s_settings.Calendar;
                Exportation = s_settings.Exportation;
                MarketPricer = s_settings.MarketPricer;
                Notifications = s_settings.Notifications;
                Compatibility = s_settings.Compatibility;
                LoadoutsProvider = s_settings.LoadoutsProvider;
                PortableEveInstallations = s_settings.PortableEveInstallations;
                CloudStorageServiceProvider = s_settings.CloudStorageServiceProvider;

                // Scheduler
                Scheduler.Import(s_settings.Scheduler);
            }
            finally
            {
                EveMonClient.Trace("done");

                // Notify the subscribers
                EveMonClient.OnSettingsChanged();
            }
        }

        /// <summary>
        /// Asynchronously imports the data.
        /// </summary>
        /// <returns></returns>
        public static async Task ImportDataAsync()
        {
            // Quit if the client has been shut down
            if (EveMonClient.Closed)
                return;

            IsRestoring = true;
            await TaskHelper.RunCPUBoundTaskAsync(() => ImportData());
            await SaveImmediateAsync();
            IsRestoring = false;
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        private static void ImportData()
        {
            EveMonClient.Trace("begin");

            if (s_settings == null)
                s_settings = new SerializableSettings();

            EveMonClient.ResetCollections();
            EveMonClient.Characters.Import(s_settings.Characters);
            EveMonClient.ESIKeys.Import(s_settings.ESIKeys);
            EveMonClient.Characters.ImportPlans(s_settings.Plans);
            EveMonClient.MonitoredCharacters.Import(s_settings.MonitoredCharacters);

            OnImportCompleted();

            EveMonClient.Trace("done");

            // Notify the subscribers
            EveMonClient.OnSettingsChanged();
        }

        /// <summary>
        /// Corrects the imported data and add missing stuff.
        /// </summary>
        private static void OnImportCompleted()
        {
            // Add missing notification behaviours
            foreach (NotificationCategory category in EnumExtensions.GetValues<NotificationCategory>()
                .Where(category => !Notifications.Categories.ContainsKey(category) && category.HasHeader()))
            {
                Notifications.Categories[category] = new NotificationCategorySettings();
            }

            // Add missing ESI methods update periods
            foreach (Enum method in ESIMethods.Methods.Where(method => method.GetUpdatePeriod() != null)
                .Where(method => !Updates.Periods.ContainsKey(method.ToString())))
                Updates.Periods.Add(method.ToString(), method.GetUpdatePeriod().DefaultPeriod);

            // Initialize or add missing columns
            InitializeOrAddMissingColumns();

            // Removes redundant notification behaviours
            List<KeyValuePair<NotificationCategory, NotificationCategorySettings>> notifications =
                Notifications.Categories.ToList();
            foreach (KeyValuePair<NotificationCategory, NotificationCategorySettings> notification in notifications
                .Where(notification => !notification.Key.HasHeader()))
            {
                Notifications.Categories.Remove(notification.Key);
            }

            // Removes redundant windows locations
            List<KeyValuePair<string, WindowLocationSettings>> locations = UI.WindowLocations.ToList();
            foreach (KeyValuePair<string, WindowLocationSettings> windowLocation in locations
                .Where(windowLocation => windowLocation.Key == "FeaturesWindow"))
            {
                UI.WindowLocations.Remove(windowLocation.Key);
            }

            // Removes redundant splitters
            List<KeyValuePair<string, int>> splitters = UI.Splitters.ToList();
            foreach (KeyValuePair<string, int> splitter in splitters
                .Where(splitter => splitter.Key == "EFTLoadoutImportationForm"))
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
            EveMonClient.Trace("begin");

            SerializableSettings serial = new SerializableSettings
            {
                SSOClientID = SSOClientID,
                SSOClientSecret = SSOClientSecret,
                Revision = Revision,
                Compatibility = Compatibility,
                ForkId = OurForkId,
                ForkVersion = EveMonClient.FileVersionInfo?.FileVersion ?? "5.1.0",
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
                G15 = G15,
                UI = UI
            };

            serial.Characters.AddRange(EveMonClient.Characters.Export());
            EveMonClient.Trace($"{serial.Characters.Count} characters exported");
            serial.ESIKeys.AddRange(EveMonClient.ESIKeys.Export());
            serial.Plans.AddRange(EveMonClient.Characters.ExportPlans());
            EveMonClient.Trace($"{serial.Plans.Count} plans exported");
            serial.MonitoredCharacters.AddRange(EveMonClient.MonitoredCharacters.Export());

            EveMonClient.Trace("done");
            return serial;
        }

        #endregion


        #region Initialization and loading

        /// <summary>
        /// Gets the current assembly's revision, which is also used for files versioning.
        /// </summary>
        internal static int Revision => Version.Parse(EveMonClient.FileVersionInfo.FileVersion).Revision;

        /// <summary>
        /// Initialization for the EVEMon client settings.
        /// </summary>
        /// <remarks>
        /// Will attempt to fetch and initialize settings from a storage server, if user has specified so.
        /// Otherwise attempts to initialize from a locally stored file.
        /// </remarks>
        public static void Initialize()
        {
            EveMonClient.Trace("begin");

            // Deserialize the local settings file to determine
            // which cloud storage service provider should be used
            s_settings = TryDeserializeFromFile();
            EveMonClient.Trace("TryDeserializeFromFile done");

            // Try to download the settings file from the cloud
            CloudStorageServiceAPIFile settingsFile = s_settings?.CloudStorageServiceProvider?.Provider?.DownloadSettingsFile();
            if (settingsFile != null)
            {
                EveMonClient.Trace("Cloud settings downloaded, deserializing");
                // If a settings file was downloaded try to deserialize it
                s_settings = TryDeserializeFromFileContent(settingsFile.FileContent);
            }

            // Loading settings
            // If there are none, we create them from scratch
            IsRestoring = true;
            Import();
            IsRestoring = false;

            // Migrate to JSON format if needed (gradual migration from XML)
            // This happens after Import() so we have the live objects ready
            TryMigrateToJsonAsync(s_settings).ConfigureAwait(false);

            EveMonClient.Trace("done");
        }

        /// <summary>
        /// Attempts to migrate settings from XML to the new JSON file structure.
        /// This runs in the background and doesn't block initialization.
        /// </summary>
        /// <param name="settings">The deserialized XML settings.</param>
        private static async Task TryMigrateToJsonAsync(SerializableSettings settings)
        {
            try
            {
                // Check if migration is needed
                if (!SettingsFileManager.NeedsMigration())
                {
                    if (SettingsFileManager.JsonSettingsExist())
                    {
                        EveMonClient.Trace("JSON settings already exist, no migration needed");
                    }
                    else if (!SettingsFileManager.LegacySettingsExist())
                    {
                        EveMonClient.Trace("No legacy settings to migrate");
                    }
                    return;
                }

                if (settings == null)
                {
                    EveMonClient.Trace("No settings to migrate");
                    return;
                }

                EveMonClient.Trace("Starting migration from XML to JSON format");
                await SettingsFileManager.MigrateFromXmlAsync(settings);
                EveMonClient.Trace("Migration to JSON complete");
            }
            catch (Exception ex)
            {
                // Migration failure is not critical - we still have the XML file
                EveMonClient.Trace($"Migration to JSON failed (non-critical): {ex.Message}");
            }
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
            if (string.IsNullOrWhiteSpace(fileContent))
                return null;

            EveMonClient.Trace("begin");

            // Gets the revision number of the assembly which generated this file
            int revision = Util.GetRevisionNumber(fileContent);

            // Try to load from a file (when no revision found then it's a pre 1.3.0 version file)
            // Note: revision < 0 means no revision attribute; revision >= 0 is valid (including 0)
            SerializableSettings settings = revision < 0
                ? (SerializableSettings)UIHelper.ShowNoSupportMessage()
                : Util.DeserializeXmlFromString<SerializableSettings>(fileContent,
                    SettingsTransform);

            if (settings != null)
            {
                EveMonClient.Trace("done");
                return settings;
            }

            const string Caption = "Corrupt Settings";

            DialogResult dr = MessageBox.Show($"Loading settings from {CloudStorageServiceProvider.ProviderName} failed." +
                                              $"{Environment.NewLine}Do you want to use the local settings file?",
                Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (dr != DialogResult.No)
                return TryDeserializeFromFile();

            MessageBox.Show($"A new settings file will be created.{Environment.NewLine}"
                            + @"You may wish then to restore a saved copy of the file.", Caption,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            return null;
        }

        /// <summary>
        /// Asynchronously restores the settings from the specified file.
        /// Supports both JSON (.json) and XML (.xml) backup formats.
        /// </summary>
        /// <param name="filename">The fully qualified filename of the settings file to load</param>
        /// <returns>The Settings object loaded</returns>
        public static async Task RestoreAsync(string filename)
        {
            string extension = Path.GetExtension(filename).ToLowerInvariant();

            if (extension == ".json" && SettingsFileManager.IsJsonBackupFile(filename))
            {
                // Restore from JSON backup format
                bool success = await SettingsFileManager.ImportBackupAsync(filename);
                if (!success)
                {
                    EveMonClient.Trace("Failed to import JSON backup");
                    return;
                }

                // Now reload from the XML file (JSON backup doesn't directly create SerializableSettings)
                // The JSON files are now populated, but we still need to reload the app
                // For now, prompt for restart or use existing settings
                EveMonClient.Trace("JSON backup imported - settings will be applied on next startup");

                // Clear current settings and trigger a re-import from what we have
                s_settings = new SerializableSettings();
            }
            else
            {
                // Restore from XML backup format
                s_settings = TryDeserializeFromBackupFile(filename, false);

                // Loading from file failed, we abort and keep our current settings
                if (s_settings == null)
                    return;

                // Clear JSON files - they'll be recreated from restored XML
                SettingsFileManager.ClearAllJsonFiles();

                // Immediately migrate restored settings to JSON
                await TryMigrateToJsonAsync(s_settings);
            }

            IsRestoring = true;
            Import();
            await ImportDataAsync();
            IsRestoring = false;
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

            EveMonClient.Trace("begin");

            // Check settings file length
            FileInfo settingsInfo = new FileInfo(settingsFile);
            if (settingsInfo.Length == 0)
                return TryDeserializeFromBackupFile(backupFile);

            // Read file content once - we'll use it for all checks
            string fileContent = File.ReadAllText(settingsFile);

            // Step 1: Detect migration scenario (silent - no UI yet)
            // This checks forkId and revision to determine if user is migrating from another fork
            var migration = DetectForkMigration(fileContent);

            // Step 2: Check revision compatibility
            // revision < 0 means no revision attribute found (ancient pre-1.3.0 file)
            bool revisionCompatible = migration.DetectedRevision >= 0;

            // Step 3: Handle migration scenario (peterhaneve or other fork)
            if (migration.MigrationDetected)
            {
                if (!revisionCompatible)
                {
                    // Settings format too old (ancient pre-1.3.0) - can't migrate
                    ShowMigrationMessage(migration, settingsCanBePreserved: false);
                    return null;
                }

                // Revision is compatible - try to deserialize to confirm settings can be preserved
                SerializableSettings testSettings = null;
                try
                {
                    testSettings = Util.DeserializeXmlFromString<SerializableSettings>(
                        fileContent, SettingsTransform);
                }
                catch
                {
                    // Deserialization failed
                }

                bool settingsCanBePreserved = testSettings != null;

                // Show migration message with accurate info about preservation
                ShowMigrationMessage(migration, settingsCanBePreserved);

                if (!settingsCanBePreserved)
                {
                    // Couldn't load settings - start fresh
                    return null;
                }

                // Settings loaded successfully - update file (clear ESI keys, add forkId)
                UpdateSettingsFileForMigration(fileContent, settingsFile);

                // IMPORTANT: Also clear ESI keys from the in-memory settings object
                // Otherwise they'd get written back to disk on next save
                int esiKeyCount = testSettings.ESIKeys.Count;
                testSettings.ESIKeys.Clear();
                EveMonClient.Trace($"Migration: Cleared {esiKeyCount} ESI keys from memory, preserved {testSettings.Plans.Count} plans");

                // Return the settings we already loaded
                CheckSettingsVersion(testSettings);
                FileHelper.CopyOrWarnTheUser(settingsFile, backupFile);
                EveMonClient.Trace("done (migration)");
                return testSettings;
            }

            // No migration detected - normal flow for our users

            // Check for ancient settings (pre-1.3.0)
            if (!revisionCompatible)
            {
                // Settings too old - show existing "no support" message
                UIHelper.ShowNoSupportMessage();
                return TryDeserializeFromBackupFile(backupFile);
            }

            // Add forkId if needed (for our existing users who don't have it yet)
            if (migration.NeedsForkIdAdded)
            {
                fileContent = AddForkIdToSettingsFile(fileContent, settingsFile);
            }

            // Deserialize the settings
            SerializableSettings settings = Util.DeserializeXmlFromString<SerializableSettings>(
                fileContent, SettingsTransform);

            // If the settings loaded OK, make a backup as 'last good settings' and return
            if (settings == null)
                return TryDeserializeFromBackupFile(backupFile);

            CheckSettingsVersion(settings);
            FileHelper.CopyOrWarnTheUser(settingsFile, backupFile);

            EveMonClient.Trace("done");
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

            EveMonClient.Trace("begin");

            // Check backup settings file length
            FileInfo backupInfo = new FileInfo(backupFile);
            if (backupInfo.Length == 0)
                return null;

            string settingsFile = EveMonClient.SettingsFileNameFullPath;

            const string Caption = "Corrupt Settings";
            if (recover)
            {
                // Prompts the user to use the backup
                string fileDate =
                    $"{backupInfo.LastWriteTime.ToLocalTime().ToShortDateString()} " +
                    $"at {backupInfo.LastWriteTime.ToLocalTime().ToShortTimeString()}";
                DialogResult dialogResult = MessageBox.Show(
                    $"The settings file is missing or corrupt. There is a backup available from {fileDate}.{Environment.NewLine}" +
                    @"Do you want to use the backup file?", Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show($"A new settings file will be created.{Environment.NewLine}"
                                    + @"You may wish then to restore a saved copy of the file.", Caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    // Save a copy of the corrupt file just in case
                    FileHelper.CopyOrWarnTheUser(backupFile, settingsFile + ".corrupt");

                    return null;
                }
            }

            // Get the revision number of the assembly which generated this file
            // Try to load from a file (when no revision found then it's a pre 1.3.0 version file)
            // Note: revision < 0 means no revision attribute; revision >= 0 is valid (including 0)
            SerializableSettings settings = Util.GetRevisionNumber(backupFile) < 0
                ? (SerializableSettings)UIHelper.ShowNoSupportMessage()
                : Util.DeserializeXmlFromFile<SerializableSettings>(backupFile,
                    SettingsTransform);

            // If the settings loaded OK, copy to the main settings file, then copy back to stamp date
            if (settings != null)
            {
                CheckSettingsVersion(settings);
                FileHelper.CopyOrWarnTheUser(backupFile, settingsFile);
                FileHelper.CopyOrWarnTheUser(settingsFile, backupFile);

                EveMonClient.Trace("done");
                return settings;
            }

            if (recover)
            {
                // Backup failed too, notify the user we have a problem
                MessageBox.Show($"Loading from backup failed.\nA new settings file will be created.{Environment.NewLine}"
                                + @"You may wish then to restore a saved copy of the file.",
                    Caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Save a copy of the corrupt file just in case
                FileHelper.CopyOrWarnTheUser(backupFile, settingsFile + ".corrupt");
            }
            else
            {
                // Restoring from file failed
                MessageBox.Show($"Restoring settings from {backupFile} failed, the file is corrupted.",
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
                MessageBox.Show($"The current EVEMon settings file is from a previous version.{Environment.NewLine}" +
                                @"Backup the current file before proceeding (recommended)?",
                    @"EVEMon version changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

            if (backupSettings != DialogResult.Yes)
                return;

            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Title = @"Settings file backup";
                fileDialog.Filter = @"Settings Backup Files (*.bak)|*.bak";
                fileDialog.FileName = $"EVEMon_Settings_{settings.Revision}.xml.bak";
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                if (fileDialog.ShowDialog() != DialogResult.OK)
                    return;

                FileHelper.CopyOrWarnTheUser(EveMonClient.SettingsFileNameFullPath, fileDialog.FileName);
            }
        }

        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by <see cref="XmlSerializer"/>
        /// </summary>
        private static XslCompiledTransform SettingsTransform
            => s_settingsTransform ?? (s_settingsTransform = Util.LoadXslt(Properties.Resources.SettingsXSLT));

        #endregion


        #region Save

        /// <summary>
        /// Every timer tick, checks whether we should save the settings every 10s.
        /// </summary>
        private static async Task UpdateOnOneSecondTickAsync()
        {
            // Is a save requested and is the last save older than 10s ?
            if (s_savePending && DateTime.UtcNow > s_nextSaveTime)
                await SaveImmediateAsync();
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
            if (!IsRestoring)
                s_savePending = true;
        }

        /// <summary>
        /// Saves settings immediately.
        /// </summary>
        public static async Task SaveImmediateAsync()
        {
            // Prevents the saving if we are restoring the settings at that time
            if (IsRestoring)
                return;

            // Reset flags
            s_savePending = false;
            s_nextSaveTime = DateTime.UtcNow.AddSeconds(10);

            EveMonClient.Trace("begin");

            try
            {
                // Export settings on UI thread (required for collection access)
                SerializableSettings settings = Export();
                EveMonClient.Trace("Export done");

                // Serialize to MemoryStream on background thread to avoid UI freeze
                byte[] serializedData = await Task.Run(() =>
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(SerializableSettings));
                        xs.Serialize(ms, settings);
                        return ms.ToArray();
                    }
                });
                EveMonClient.Trace($"Serialized {serializedData.Length} bytes");

                // Write to file (atomic via temp file)
                await FileHelper.OverwriteOrWarnTheUserAsync(EveMonClient.SettingsFileNameFullPath,
                    async fs =>
                    {
                        await fs.WriteAsync(serializedData, 0, serializedData.Length);
                        await fs.FlushAsync();
                        return true;
                    });
                EveMonClient.Trace("XML file written");

                // Also save to JSON format (keeps JSON files in sync with XML)
                await SettingsFileManager.SaveFromSerializableSettingsAsync(settings);
            }
            catch (Exception exception)
            {
                EveMonClient.Trace($"Error: {exception.Message}");
                ExceptionHandler.LogException(exception, true);
            }
        }

        /// <summary>
        /// Exports settings to the specified location.
        /// Supports both JSON (.json) and XML (.xml) formats based on file extension.
        /// </summary>
        /// <param name="copyFileName">The fully qualified filename of the destination file</param>
        public static async Task CopySettingsAsync(string copyFileName)
        {
            // Ensure we have the latest settings saved
            await SaveImmediateAsync();

            // Check file extension to determine format
            string extension = Path.GetExtension(copyFileName).ToLowerInvariant();
            if (extension == ".json")
            {
                // Export to JSON backup format
                SerializableSettings settings = Export();
                await SettingsFileManager.ExportBackupAsync(copyFileName, settings);
            }
            else
            {
                // Export to XML format (legacy)
                FileHelper.CopyOrWarnTheUser(EveMonClient.SettingsFileNameFullPath, copyFileName);
            }
        }

        #endregion
    }
}
