using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public event EventHandler<CloudStorageServiceProviderEventArgs> CredentialsChecked;

        /// <summary>
        /// Occurs when the file gets uploaded to the cloud storage service provider.
        /// </summary>
        public event EventHandler<CloudStorageServiceProviderEventArgs> FileUploaded;

        /// <summary>
        /// Occurs when the file gets uploaded to the cloud storage service provider.
        /// </summary>
        public event EventHandler<CloudStorageServiceProviderEventArgs> FileDownloaded;

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the user is authenticated with the provider.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is authenticated with the provider; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsAuthenticated { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the provider API credentials are stored.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasCredentialsStored
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the provider supports compressed responses.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider supports compressed responses; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool SupportsDataCompression
        {
            get { return false; }
        }

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
                    .Where(provider => !String.IsNullOrWhiteSpace(provider.Name))
                    .OrderBy(provider => provider.Name);
            }
        }

        /// <summary>
        /// Gets the content of the settings file.
        /// </summary>
        /// <value>The content of the settings file.</value>
        protected static string SettingsFileContent
        {
            get
            {
                string settingsFileContent = File.ReadAllText(EveMonClient.SettingsFileNameFullPath);
                return HttpUtility.UrlEncode(settingsFileContent);
            }
        }

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

            // Find the parent directories of the settings file
            string configFileParentDir = Directory.GetParent(settings.FilePath).FullName;
            string configFileParentParentDir = Directory.GetParent(configFileParentDir).FullName;

            // Quits if the parent directory doesn't exist
            if (!Directory.Exists(configFileParentParentDir))
                return;

            // Upgrade the settings file to the current version
            CloudStorageServicesSettings.Default.Upgrade();

            // Delete all old settings files
            foreach (string directory in Directory.GetDirectories(configFileParentParentDir).Where(
                directory => directory != configFileParentDir))
            {
                // Delete the folder recursively
                Directory.Delete(directory, true);
            }
        }

        /// <summary>
        /// Checks the API credentials asynchronously.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        public abstract void CheckAPICredentialsAsync(uint userID, string apiKey);

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
        /// <param name="errorMessage">The error message.</param>
        protected virtual void OnCredentialsChecked(SerializableAPIResult<CloudStorageServiceAPICredentials> result, string errorMessage)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                if (CredentialsChecked != null)
                    CredentialsChecked(this, new CloudStorageServiceProviderEventArgs(errorMessage));

                return;
            }

            if (result.HasError)
            {
                if (CredentialsChecked != null)
                    CredentialsChecked(this, new CloudStorageServiceProviderEventArgs(result.Error.ErrorMessage));

                return;
            }

            IsAuthenticated = true;

            if (CredentialsChecked != null)
                CredentialsChecked(this, new CloudStorageServiceProviderEventArgs(String.Empty));
        }

        /// <summary>
        /// Occurs when the file got uploaded.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        protected virtual void OnFileUploaded(SerializableAPIResult<CloudStorageServiceAPIFile> result, string errorMessage)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                if (FileUploaded != null)
                    FileUploaded(this, new CloudStorageServiceProviderEventArgs(errorMessage));

                return;
            }

            if (result.HasError)
            {
                if (FileUploaded != null)
                    FileUploaded(this, new CloudStorageServiceProviderEventArgs(result.Error.ErrorMessage));

                return;
            }

            if (FileUploaded != null)
                FileUploaded(this, new CloudStorageServiceProviderEventArgs(String.Empty));
        }

        /// <summary>
        /// Occurs when the file got downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        protected virtual void OnFileDownloaded(SerializableAPIResult<CloudStorageServiceAPIFile> result, string errorMessage)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                if (FileDownloaded != null)
                    FileDownloaded(this, new CloudStorageServiceProviderEventArgs(errorMessage));

                return;
            }

            if (result.HasError)
            {
                if (FileDownloaded != null)
                    FileDownloaded(this, new CloudStorageServiceProviderEventArgs(result.Error.ErrorMessage));

                return;
            }

            if (FileDownloaded != null)
                FileDownloaded(this, new CloudStorageServiceProviderEventArgs(String.Empty));
        }

        /// <summary>
        /// Saves the queried settings file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <exception cref="System.ArgumentNullException">settingsFile</exception>
        protected static void SaveSettingsFile(CloudStorageServiceAPIFile result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

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
