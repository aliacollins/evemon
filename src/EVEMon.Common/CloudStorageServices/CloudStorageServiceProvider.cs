using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.Common.CloudStorageServices
{

    public abstract class CloudStorageServiceProvider
    {
        /// <summary>
        /// Occurs when the credentials get checked with the cloud storage service provider.
        /// </summary>
        public static event EventHandler<CloudStorageServiceProviderEventArgs> CredentialsChecked;

        /// <summary>
        /// Occurs when the credentials of the cloud storage service provider get reset.
        /// </summary>
        public static event EventHandler<CloudStorageServiceProviderEventArgs> SettingsReset;

        /// <summary>
        /// Occurs when the file gets uploaded to the cloud storage service provider.
        /// </summary>
        public static event EventHandler<CloudStorageServiceProviderEventArgs> FileUploaded;

        /// <summary>
        /// Occurs when the file gets uploaded to the cloud storage service provider.
        /// </summary>
        public static event EventHandler<CloudStorageServiceProviderEventArgs> FileDownloaded;

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CloudStorageServiceProvider"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        protected abstract bool Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether the user is authenticated with the provider.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is authenticated with the provider; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public static bool IsAuthenticated { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the provider API credentials are stored.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasCredentialsStored => false;

        /// <summary>
        /// Gets a value indicating whether the provider supports compressed responses.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider supports compressed responses; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool SupportsDataCompression => false;

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected virtual ApplicationSettingsBase Settings => CloudStorageServiceSettings.Default;

        /// <summary>
        /// Gets the refferal link.
        /// </summary>
        /// <value>
        /// The refferal link.
        /// </value>
        public virtual Uri RefferalLink => null;

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>
        /// The providers.
        /// </value>
        public static IEnumerable<CloudStorageServiceProvider> Providers
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => typeof(CloudStorageServiceProvider).IsAssignableFrom(type) &&
                                   type.GetConstructor(Type.EmptyTypes) != null)
                    .Select(type => Activator.CreateInstance(type) as CloudStorageServiceProvider)
                    .Where(provider => !String.IsNullOrWhiteSpace(provider.Name) && provider.Enabled)
                    .OrderBy(provider => provider.Name);
            }
        }

        /// <summary>
        /// Gets the content of the settings file url encoded.
        /// </summary>
        /// <value>
        /// The settings file content URL encode.
        /// </value>
        protected static string SettingsFileContentUrlEncode
            => HttpUtility.UrlEncode(File.ReadAllText(EveMonClient.SettingsFileNameFullPath));

        /// <summary>
        /// Gets the content of the settings file in a byte array.
        /// </summary>
        /// <value>
        /// The settings file content byte array.
        /// </value>
        protected static byte[] SettingsFileContentByteArray
            => Encoding.UTF8.GetBytes(File.ReadAllText(EveMonClient.SettingsFileNameFullPath));

        /// <summary>
        /// Upgrades the settings.
        /// </summary>
        public static void UpgradeSettings()
        {
            // Find the settings file
            Configuration settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            // Quit if the settings file is of the current version
            if (settings.HasFile)
                return;

            // Find the parent directory of the settings file
            DirectoryInfo configFileParentDir = Directory.GetParent(settings.FilePath);

            // Find the parent directory of the settings file directory
            DirectoryInfo configFileParentParentDir = configFileParentDir.Parent;

            // Quits if there is no parent directory
            if (configFileParentParentDir == null)
                return;

            // If the parent directory doesn't exist delete all old settings folders
            if (!Directory.Exists(configFileParentParentDir.FullName))
            {
                // Delete all old settings folders
                DeleteOldSettingsFolders(configFileParentParentDir);

                return;
            }

            // Upgrade the settings file to the current version
            CloudStorageServiceSettings.Default.Upgrade();
            foreach (CloudStorageServiceProvider provider in Providers)
            {
                provider.Settings.Upgrade();
            }

            // Delete all old settings files inside the settings folder
            foreach (string directory in Directory.GetDirectories(configFileParentParentDir.FullName)
                .Where(directory => directory != configFileParentDir.FullName))
            {
                // Delete the folder recursively
                Directory.Delete(directory, true);
            }

            // Delete all old settings folders
            DeleteOldSettingsFolders(configFileParentParentDir);
        }

        /// <summary>
        /// Deletes the old settings folders.
        /// </summary>
        /// <param name="configFileParentParentDir">The configuration file parent parent dir.</param>
        private static void DeleteOldSettingsFolders(DirectoryInfo configFileParentParentDir)
        {
            if (configFileParentParentDir.Parent == null || !Directory.Exists(configFileParentParentDir.Parent.FullName))
                return;

            foreach (string directory in Directory.GetDirectories(configFileParentParentDir.Parent.FullName)
                .Where(directory => directory != configFileParentParentDir.FullName))
            {
                // Delete the folder recursively
                Directory.Delete(directory, true);
            }
        }

        /// <summary>
        /// Synchronously checks the API authentication with credentials is valid.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        public abstract void CheckAPIAuthWithCredentialsIsValid(uint userID, string apiKey);

        /// <summary>
        /// Asynchronously checks the API authentication with credentials is valid.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        public abstract void CheckAPIAuthWithCredentialsIsValidAsync(uint userID, string apiKey);

        /// <summary>
        /// Synchronously checks that API authentication is valid.
        /// </summary>
        public abstract bool CheckAPIAuthIsValid();

        /// <summary>
        /// Asynchronously checks that API authentication is valid.
        /// </summary>
        public abstract void CheckAPIAuthIsValidAsync();

        /// <summary>
        /// Asynchronously requests an authentication code.
        /// </summary>
        public abstract void RequestAuthCodeAsync();

        /// <summary>
        /// Asynchronously checks the authentication code.
        /// </summary>
        /// <param name="code">The code.</param>
        public abstract void CheckAuthCodeAsync(string code);

        /// <summary>
        /// Resets the settings.
        /// </summary>
        public abstract void ResetSettings();

        /// <summary>
        /// Uploads the settings file.
        /// </summary>
        public abstract bool UploadSettingsFile();

        /// <summary>
        /// Downloads the settings file.
        /// </summary>
        public abstract CloudStorageServiceAPIFile DownloadSettingsFile();

        /// <summary>
        /// Uploads the settings file asynchronously.
        /// </summary>
        public abstract void UploadSettingsFileAsync();

        /// <summary>
        /// Downloads the settings file asynchronously.
        /// </summary>
        public abstract void DownloadSettingsFileAsync();

        /// <summary>
        /// Occurs when the credentials get checked.
        /// </summary>
        /// <param name="result">The result.</param>
        protected virtual void OnCredentialsChecked(SerializableAPIResult<CloudStorageServiceAPICredentials> result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (result.HasError)
            {
                IsAuthenticated = false;

                CredentialsChecked?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error.ErrorMessage));

                return;
            }

            IsAuthenticated = true;

            EveMonClient.Trace($"{Name}.CheckCredentialsAsync - Completed");

            CredentialsChecked?.Invoke(this, new CloudStorageServiceProviderEventArgs(null));
        }

        /// <summary>
        /// Occurs when the settings get reset.
        /// </summary>
        protected virtual void OnSettingsReset()
        {
            EveMonClient.Trace($"{Name}.SettingsReset - Completed");

            SettingsReset?.Invoke(this, new CloudStorageServiceProviderEventArgs(null));
        }

        /// <summary>
        /// Occurs when the file got uploaded.
        /// </summary>
        /// <param name="result">The result.</param>
        protected virtual void OnFileUploaded(SerializableAPIResult<CloudStorageServiceAPIFile> result)
        {
            if (result.HasError)
            {
                FileUploaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error.ErrorMessage));

                return;
            }

            FileUploaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(null));
        }

        /// <summary>
        /// Occurs when the file got downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        protected virtual void OnFileDownloaded(SerializableAPIResult<CloudStorageServiceAPIFile> result)
        {
            if (result.HasError)
            {
                FileDownloaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error.ErrorMessage));

                return;
            }

            FileDownloaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(null));
        }

        /// <summary>
        /// Saves the queried settings file.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="System.ArgumentNullException">settingsFile</exception>
        protected static void SaveSettingsFile(CloudStorageServiceAPIFile result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = @"EVEMon Settings Backup File Save";
                saveFileDialog.DefaultExt = "bak";
                saveFileDialog.Filter = @"EVEMon Settings Backup Files (*.bak)|*.bak";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                // Prompts the user for a location
                saveFileDialog.FileName = String.Format(CultureConstants.DefaultCulture, "{0}.bak", result.FileName);
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                DialogResult dialogResult = saveFileDialog.ShowDialog();

                // Save settings file if OK
                if (dialogResult != DialogResult.OK)
                    return;

                // Save the file to destination
                File.WriteAllText(saveFileDialog.FileName, result.FileContent);
            }
        }
    }
}
