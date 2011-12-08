using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml.Xsl;
using EVEMon.Common.Attributes;
using EVEMon.Common.Notifications;
using EVEMon.Common.Scheduling;
using EVEMon.Common.Serialization.Importation;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;
using System.Collections.ObjectModel;

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
            Notifications = new NotificationSettings();

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
        public static CompatibilityMode Compatibility { get; set; }

        /// <summary>
        /// Gets the settings for updates.
        /// </summary>
        public static UpdateSettings Updates { get; set; }

        /// <summary>
        /// Gets the settings for UI (look'n feel)
        /// </summary>
        public static UISettings UI { get; set; }

        /// <summary>
        /// Gets the settings for the G15 keyboard.
        /// </summary>
        public static G15Settings G15 { get; set; }

        /// <summary>
        /// Gets the settings for the notifications (alerts).
        /// </summary>
        public static NotificationSettings Notifications { get; set; }

        /// <summary>
        /// Gets the settings for the network.
        /// </summary>
        public static IGBSettings IGB { get; set; }

        /// <summary>
        /// Gets the calendar settings.
        /// </summary>
        public static CalendarSettings Calendar { get; set; }

        /// <summary>
        /// Gets or sets the exportation settings.
        /// </summary>
        public static ExportationSettings Exportation { get; set; }

        /// <summary>
        /// Gets or sets the custom proxy settings.
        /// </summary>
        public static ProxySettings Proxy { get; set; }

        #endregion


        #region Serialization - Core - Methods to update to add a property

        /// <summary>
        /// Creates new empty Settings file, overwriting the existing file
        /// </summary>
        public static void Reset()
        {
            // Append new properties here
            Import(new SerializableSettings(), false);

            // Notifies the client and save
            SaveImmediate();
        }

        /// <summary>
        /// Updates from the provided serialization object
        /// </summary>
        /// <param name="serial">The serializable version of the new settings. May be null (acts as a reset)</param>
        /// <param name="preferencesOnly">When true, only the user preferences will be reimported, not plans, characters, accounts and such.</param>
        public static void Import(SerializableSettings serial, bool preferencesOnly)
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

                // Import the characters, API keys and plans
                if (!preferencesOnly)
                {
                    // The above check prevents the settings form to trigger a 
                    // characters updates since the last queried infos would be lost
                    EveMonClient.Characters.Import(serial.Characters);
                    EveMonClient.Characters.ImportPlans(serial.Plans);
                    EveMonClient.MonitoredCharacters.Import(serial.MonitoredCharacters);
                    EveMonClient.APIKeys.Import(serial.APIKeys);
                }

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
                Notifications = serial.Notifications;
                Exportation = serial.Exportation;
                Calendar = serial.Calendar;

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
                    category => !Notifications.Categories.ContainsKey(category)))
            {
                Notifications.Categories[category] = new NotificationCategorySettings();
            }

            // Add missing API methods update periods
            foreach (Enum method in APIMethods.Methods.Where(method => method.GetUpdatePeriod() != null).Where(
                method => !Updates.Periods.ContainsKey(method.ToString())))
            {
                Updates.Periods[method.ToString()] = method.GetUpdatePeriod().DefaultPeriod;

                // Bind the APIKeyInfo and CharacterList update period
                if ((APIGenericMethods)method == APIGenericMethods.APIKeyInfo &&
                    Updates.Periods[APIGenericMethods.CharacterList.ToString()] != Updates.Periods[method.ToString()])
                    Updates.Periods[method.ToString()] = Updates.Periods[APIGenericMethods.CharacterList.ToString()];
            }

            // Initialize or add missing columns
            InitializeOrAddMissingColumns();

            // Removes reduntant windows locations
            List<KeyValuePair<string, WindowLocationSettings>> locations = new List<KeyValuePair<string, WindowLocationSettings>>();
            locations.AddRange(UI.WindowLocations);
            foreach (KeyValuePair<string, WindowLocationSettings> windowLocation in locations.Where(
                windowLocation => windowLocation.Key == "FeaturesWindow"))
            {
                UI.WindowLocations.Remove(windowLocation.Key);
            }
        }

        /// <summary>
        /// Initializes or adds missing columns.
        /// </summary>
        private static void InitializeOrAddMissingColumns()
        {
            // Initializes the plan columns or adds missing ones
            UI.PlanWindow.Columns.AddRange(UI.PlanWindow.DefaultColumns);

            // Initializes the market order columns or adds missing ones
            UI.MainWindow.MarketOrders.Columns.AddRange(UI.MainWindow.MarketOrders.DefaultColumns);

            // Initializes the industry jobs columns or adds missing ones
            UI.MainWindow.IndustryJobs.Columns.AddRange(UI.MainWindow.IndustryJobs.DefaultColumns);

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
            get { return Assembly.GetExecutingAssembly().GetName().Version.Revision; }
        }

        /// <summary>
        /// Initialization for the EVEMon client. Will automatically load the settings file.
        /// </summary>
        /// <exception cref="InvalidOperationException">The instance has been initialized already</exception>
        public static void InitializeFromFile()
        {
            // Creates the settings from the file
            SerializableSettings settings = TryDeserializeSettings();

            // Loading from file failed, we create settings from scratch
            if (settings == null)
                Reset();
            else
                Import(settings, false);
        }

        /// <summary>
        /// Loads a settings file from a specified filepath and sets m_instance.
        /// </summary>
        /// <param name="filename">The fully qualified filename of the settings file to load</param>
        /// <returns>The Settings object loaded</returns>
        public static void Restore(string filename)
        {
            // Try deserialize
            string settingsFile = EveMonClient.SettingsFileNameFullPath;
            SerializableSettings settings = TryDeserializeBackup(filename, settingsFile, false);

            // Loading from file failed, we abort and keep our current settings
            if (settings == null)
            {
                MessageBox.Show(String.Format(CultureConstants.DefaultCulture,
                                              "Cannot restore the settings from {0}, the file is corrupted.", filename),
                                "Bad settings file.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Updates and save
            Import(settings, false);
        }

        /// <summary>
        /// loads the settings file, or the backup, prompting the user for errors.
        /// </summary>
        /// <returns><c>Null</c> if we have been unable to load anything from files, the generated settings otherwise</returns>
        private static SerializableSettings TryDeserializeSettings()
        {
            string settingsFile = EveMonClient.SettingsFileNameFullPath;
            string backupFile = settingsFile + ".bak";

            // Check that a settings file or backup exists
            if (File.Exists(settingsFile))
            {
                EveMonClient.Trace("Settings.TryDeserializeSettings - begin");

                // Check settings file length
                FileInfo settingsInfo = new FileInfo(settingsFile);
                if (settingsInfo.Length > 0)
                {
                    // Gets the revision number of the assembly which generated this file
                    int revision = Util.GetRevisionNumber(settingsFile);

                    // Try to load from a file (when no revision found then it's a pre 1.3.0 version file)
                    SerializableSettings settings = revision == 0
                                                        ? DeserializeOldFormat(settingsFile)
                                                        : Util.DeserializeXMLFromFile<SerializableSettings>(settingsFile, SettingsTransform);

                    // If the settings loaded OK, make a backup as 'last good settings' and return
                    if (settings != null)
                    {
                        CheckSettingsVersion(settings);
                        FileHelper.OverwriteOrWarnTheUser(settingsFile, backupFile);
                        EveMonClient.Trace("Settings.TryDeserializeSettings - done");
                        return settings;
                    }
                }
            }

            // Try to recover from the backup
            return TryDeserializeBackup(backupFile, settingsFile, true);
        }

        /// <summary>
        /// Try to deserialize from the backup file.
        /// </summary>
        /// <param name="backupFile"></param>
        /// <param name="settingsFile"></param>
        /// <param name="recover"></param>
        /// <returns></returns>
        private static SerializableSettings TryDeserializeBackup(string backupFile, string settingsFile, bool recover)
        {
            // Load failed, so check for backup
            if (File.Exists(backupFile))
            {
                EveMonClient.Trace("Settings.TryDeserializeBackup - begin");

                FileInfo backupInfo = new FileInfo(backupFile);
                if (backupInfo.Length > 0)
                {
                    if (recover)
                    {
                        // Prompts the user to use the backup
                        String fileDate = String.Format(CultureConstants.DefaultCulture, "{0} at {1}",
                                                        backupInfo.LastWriteTime.ToLocalTime().ToShortDateString(),
                                                        backupInfo.LastWriteTime.ToLocalTime().ToCustomShortTimeString());
                        DialogResult dr = MessageBox.Show(String.Format(CultureConstants.DefaultCulture,
                                                                        "Your settings file is missing or corrupt. There is a backup available from {0}. Do you want to use the backup file?",
                                                                        fileDate),
                                                          "Corrupt Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                        if (dr == DialogResult.No)
                        {
                            MessageBox.Show(
                                "Your settings file is corrupt, and no backup is available. A new settings file will be created."
                                + " You may wish to close down EVEMon and restore a saved copy of your file.", "Corrupt Settings",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return null;
                        }
                    }
                    // Gets the revision number of the assembly which generated this file
                    int revision = Util.GetRevisionNumber(backupFile);

                    // Try to load from a file (when no revison found then it's a pre 1.3.0 version file)
                    SerializableSettings settings = revision == 0
                                                        ? DeserializeOldFormat(backupFile)
                                                        : Util.DeserializeXMLFromFile<SerializableSettings>(backupFile, SettingsTransform);

                    // If the settings loaded OK, copy to the main settings file, then copy back to stamp date
                    if (settings != null)
                    {
                        CheckSettingsVersion(settings);
                        FileHelper.OverwriteOrWarnTheUser(backupFile, settingsFile);
                        FileHelper.OverwriteOrWarnTheUser(settingsFile, backupFile);
                        EveMonClient.Trace("Settings.TryDeserializeBackup - done");
                        return settings;
                    }

                    // Backup failed too, notify the user we have a problem
                    MessageBox.Show("Load from backup failed. A new settings file will be created."
                                    + " You may wish to close down EVEMon and restore a saved copy of your file.",
                                    "Corrupt Settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Deserializes a settings file from an old format.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static SerializableSettings DeserializeOldFormat(string filename)
        {
            OldSettings oldSerial = Util.DeserializeXMLFromFile<OldSettings>(filename,
                                                                     Util.LoadXSLT(Properties.Resources.SettingsAndPlanImport));

            if (oldSerial == null)
                return null;

            SerializableSettings serial = new SerializableSettings();

            // Characters
            foreach (SerializableCCPCharacter character in oldSerial.Characters.Select(
                oldCharacter => new SerializableCCPCharacter
                                    {
                                        ID = oldCharacter.ID,
                                        Name = oldCharacter.Name,
                                        Guid = Guid.NewGuid()
                                    }))
            {
                serial.MonitoredCharacters.Add(new MonitoredCharacterSettings { CharacterGuid = character.Guid });
                serial.Characters.Add(character);
            }

            // Plans
            foreach (OldSettingsPlan oldPlan in oldSerial.Plans)
            {
                // Look for the owner by his name
                SerializableSettingsCharacter owner = serial.Characters.SingleOrDefault(x => x.Name == oldPlan.Owner);
                if (owner == null)
                    continue;

                // Imports the plan
                SerializablePlan plan = new SerializablePlan
                                            {
                                                Owner = owner.Guid,
                                                Name = oldPlan.Name,
                                                Description = String.Empty,
                                            };
                plan.Entries.AddRange(oldPlan.Entries);
                serial.Plans.Add(plan);
            }

            return serial;
        }

        /// <summary>
        /// Compare the settings version with this version and, when different, update and prompt the user for a backup.
        /// </summary>
        /// <param name="settings"></param>
        private static void CheckSettingsVersion(SerializableSettings settings)
        {
            if (EveMonClient.IsDebugBuild)
                return;

            int revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            if (revision == settings.Revision)
                return;

            DialogResult backupSettings =
                MessageBox.Show("The current EVEMon settings file is from a previous version of EVEMon.\n" +
                                "Backup the current file before proceeding (recommended)?",
                                "EVEMon version changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1);

            if (backupSettings != DialogResult.Yes)
                return;

            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Title = "Settings file backup";
                fileDialog.Filter = "Settings Backup Files (*.bak)|*.bak";
                fileDialog.FileName = String.Format(CultureConstants.DefaultCulture, "EVEMon_Settings_{0}.xml.bak", settings.Revision);
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
            get { return s_settingsTransform ?? (s_settingsTransform = Util.LoadXSLT(Properties.Resources.SettingsXSLT)); }
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
            XmlSerializer xs = new XmlSerializer(typeof(SerializableSettings));

            // Save in settings file
            FileHelper.OverwriteOrWarnTheUser(EveMonClient.SettingsFileNameFullPath,
                                              fs =>
                                                  {
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