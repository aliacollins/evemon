using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Serialization;

namespace EVEMon.Common.CloudStorageServices
{
    public abstract class CloudStorageServiceProvider
    {
        private bool m_queryPending;


        #region Event Handlers

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

        #endregion


        #region Properties

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the authentication steps.
        /// </summary>
        /// <value>
        /// The authentication steps.
        /// </value>
        public abstract AuthenticationSteps AuthSteps { get; }

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
        /// Gets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public virtual Image Logo => null;

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>
        /// The providers.
        /// </value>
        public static IEnumerable<CloudStorageServiceProvider> Providers
            => Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(CloudStorageServiceProvider).IsAssignableFrom(type) &&
                               type.GetConstructor(Type.EmptyTypes) != null)
                .Select(type => Activator.CreateInstance(type) as CloudStorageServiceProvider)
                .Where(provider => !String.IsNullOrWhiteSpace(provider.Name) && provider.Enabled)
                .OrderBy(provider => provider.Name);

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
        /// Gets the settings file name without extension.
        /// </summary>
        /// <value>
        /// The settings file name without extension.
        /// </value>
        protected static string SettingsFileNameWithoutExtension
            => Path.GetFileNameWithoutExtension(EveMonClient.SettingsFileName);

        protected static Configuration Configuration
            => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

        #endregion


        #region Abstract Methods

        /// <summary>
        /// Asynchronously checks that the provider authentication with credentials is valid.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        protected abstract Task<SerializableAPIResult<SerializableAPICredentials>>
            CheckProviderAuthWithCredentialsIsValidAsync(uint userId, string apiKey);

        /// <summary>
        /// Asynchronously requests the provider an authentication code.
        /// </summary>
        protected abstract Task<SerializableAPIResult<SerializableAPICredentials>> RequestProviderAuthCodeAsync();

        /// <summary>
        /// Asynchronously checks the provider authentication code.
        /// </summary>
        /// <param name="code">The code.</param>
        protected abstract Task<SerializableAPIResult<SerializableAPICredentials>> CheckProviderAuthCodeAsync(string code);

        /// <summary>
        /// Asynchronously checks the authentication.
        /// </summary>
        protected abstract Task<SerializableAPIResult<SerializableAPICredentials>> CheckAuthenticationAsync();

        /// <summary>
        /// Asynchronously revokes the authorization.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<SerializableAPIResult<SerializableAPICredentials>> RevokeAuthorizationAsync();

        /// <summary>
        /// Uploads the file asynchronously.
        /// </summary>
        protected abstract Task<SerializableAPIResult<CloudStorageServiceAPIFile>> UploadFileAsync();

        /// <summary>
        /// Downloads the file asynchronously.
        /// </summary>
        protected abstract Task<SerializableAPIResult<CloudStorageServiceAPIFile>> DownloadFileAsync();

        #endregion


        #region Public Methods

        /// <summary>
        /// Upgrades the settings.
        /// </summary>
        public static void UpgradeSettings()
        {
            // Quit if the settings file is of the current version
            if (Configuration.HasFile)
                return;

            // Find the parent directory of the settings file
            DirectoryInfo configFileParentDir = Directory.GetParent(Configuration.FilePath);

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
        /// Asynchronously requests an authentication code.
        /// </summary>
        public async void RequestAuthCodeAsync()
        {
            if (m_queryPending || HasCredentialsStored)
                return;

            m_queryPending = true;

            EveMonClient.Trace("Initiated");

            IsAuthenticated = false;

            SerializableAPIResult<SerializableAPICredentials> result = await RequestProviderAuthCodeAsync();

            CredentialsChecked?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));

            m_queryPending = false;

            var actionText = result.HasError ? "Failed" : "Completed";
            EveMonClient.Trace(actionText);
        }

        /// <summary>
        /// Asynchronously checks the authentication code.
        /// </summary>
        /// <param name="code">The code.</param>
        public async void CheckAuthCodeAsync(string code)
        {
            if (m_queryPending && AuthSteps == AuthenticationSteps.Two)
                return;

            m_queryPending = true;

            EveMonClient.Trace("Initiated");

            SerializableAPIResult<SerializableAPICredentials> result = await CheckProviderAuthCodeAsync(code);
            
            if (!result.HasError)
                Settings.Save();

            IsAuthenticated = !result.HasError && HasCredentialsStored;

            CredentialsChecked?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));

            m_queryPending = false;

            var actionText = result.HasError ? "Failed" : "Completed";
            EveMonClient.Trace($"CloudStorageServiceProvider.CheckAuthCodeAsync - {actionText}", printMethod: false);
        }

        /// <summary>
        /// Synchronously checks the API authentication with credentials is valid.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        public bool CheckAPIAuthWithCredentialsIsValid(uint userID, string apiKey)
            => !Task.Run(async () => await CheckProviderAuthWithCredentialsIsValidAsync(userID, apiKey)).Result.HasError;

        /// <summary>
        /// Asynchronously checks the API authentication with credentials is valid.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        public async void CheckAPIAuthWithCredentialsIsValidAsync(uint userID, string apiKey)
        {
            if (m_queryPending)
                return;

            m_queryPending = true;

            EveMonClient.Trace("Initiated");

            IsAuthenticated = false;

            SerializableAPIResult<SerializableAPICredentials> result =
                await CheckProviderAuthWithCredentialsIsValidAsync(userID, apiKey);

            IsAuthenticated = !result.HasError;
            CredentialsChecked?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));
            m_queryPending = false;

            var actionText = result.HasError ? "Failed" : "Completed";
            EveMonClient.Trace(actionText);
        }

        /// <summary>
        /// Synchronously checks that API authentication is valid.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CheckAPIAuthIsValid()
            => !Task.Run(async () => await CheckAuthenticationAsync()).Result.HasError;

        /// <summary>
        /// Asynchronously checks that API authentication is valid.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public async void CheckAPIAuthIsValidAsync()
        {
            if (m_queryPending)
                return;

            m_queryPending = true;

            EveMonClient.Trace("Initiated");

            IsAuthenticated = false;

            SerializableAPIResult<SerializableAPICredentials> result = await CheckAuthenticationAsync();

            IsAuthenticated = !result.HasError && HasCredentialsStored;

            CredentialsChecked?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));

            m_queryPending = false;

            var actionText = result.HasError ? "Failed" : "Completed";
            EveMonClient.Trace(actionText);
        }

        /// <summary>
        /// Asynchronously resets the settings.
        /// </summary>
        public async void ResetSettingsAsync()
        {
            EveMonClient.Trace("Initiated");

            SerializableAPIResult<SerializableAPICredentials> result = await RevokeAuthorizationAsync();

            if (!result.HasError)
                Settings.Reset();

            SettingsReset?.Invoke(this, new CloudStorageServiceProviderEventArgs(null));

            var actionText = result.HasError ? "Failed" : "Completed";
            EveMonClient.Trace(actionText);
        }

        /// <summary>
        /// Uploads the settings file.
        /// </summary>
        /// <returns></returns>
        public bool UploadSettingsFile()
        {
            if (!CloudStorageServiceSettings.Default.UploadAlways || !HasCredentialsStored)
                return true;

            // Quit if user is not authenticated
            if (!IsAuthenticated && !CheckAPIAuthIsValid())
            {
                MessageBox.Show($"The {Name} API credentials could not be authenticated.", $"{Name} API Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            EveMonClient.Trace("Initiated");

            // Ask for user action if uploading fails
            while (true)
            {
                SerializableAPIResult<CloudStorageServiceAPIFile> result = Task.Run(async () => await UploadFileAsync()).Result;
                FileUploaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));

                if (!result.HasError)
                {
                    EveMonClient.Trace("Completed");
                    return true;
                }

                DialogResult dialogResult = MessageBox.Show(result.Error?.ErrorMessage, $"{Name} API Error",
                    MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                switch (dialogResult)
                {
                    case DialogResult.Abort:
                        EveMonClient.Trace("Failed and Aborted");
                        return false;
                    case DialogResult.Retry:
                        continue;
                }

                EveMonClient.Trace("Failed and Ignored");
                return true;
            }
        }

        /// <summary>
        /// Downloads the settings file.
        /// </summary>
        /// <returns></returns>
        public CloudStorageServiceAPIFile DownloadSettingsFile()
        {
            if (!CloudStorageServiceSettings.Default.DownloadAlways || !HasCredentialsStored)
                return null;

            if (!IsAuthenticated && !CheckAPIAuthIsValid())
            {
                MessageBox.Show($"The {Name} API credentials could not be authenticated.",
                    $"{Name} API Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }

            EveMonClient.Trace("Initiated");

            SerializableAPIResult<CloudStorageServiceAPIFile> result = Task.Run(async () => await DownloadFileAsync()).Result;
            FileDownloaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));

            if (CloudStorageServiceSettings.Default.UseImmediately)
            {
                if (result.HasError)
                {
                    MessageBox.Show(String.Format(CultureConstants.DefaultCulture,
                        $"File could not be downloaded.\n\nThe error was:\n{result.Error?.ErrorMessage}"),
                        $"{Name} API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    EveMonClient.Trace("Completed");
                    return result.Result;
                }
            }
            else
            {
                var resultText = result.HasError ? "Failed" : "Completed";
                EveMonClient.Trace(resultText);

                if (!result.HasError)
                    SaveSettingsFile(result.Result);
            }

            return null;
        }

        /// <summary>
        /// Uploads the settings file asynchronously.
        /// </summary>
        public async void UploadSettingsFileAsync()
        {
            if (m_queryPending)
                return;

            m_queryPending = true;

            EveMonClient.Trace("Initiated");

            SerializableAPIResult<CloudStorageServiceAPIFile> result = await UploadFileAsync();
            FileUploaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));
            m_queryPending = false;

            string resultText = result.HasError ? "Failed" : "Completed";
            EveMonClient.Trace(resultText);
        }

        /// <summary>
        /// Downloads the settings file asynchronously.
        /// </summary>
        public async void DownloadSettingsFileAsync()
        {
            if (m_queryPending)
                return;

            m_queryPending = true;

            EveMonClient.Trace("Initiated");

            SerializableAPIResult<CloudStorageServiceAPIFile> result = await DownloadFileAsync();
            FileDownloaded?.Invoke(this, new CloudStorageServiceProviderEventArgs(result.Error?.ErrorMessage));
            m_queryPending = false;

            string resultText = result.HasError ? "Failed" : "Completed";
            EveMonClient.Trace(resultText);

            if (!result.HasError)
                SaveSettingsFile(result.Result);
        }

        /// <summary>
        /// Cancels the pending queries.
        /// </summary>
        public void CancelPendingQueries()
        {
            m_queryPending = false;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Deletes the old settings folders.
        /// </summary>
        /// <param name="configFileParentParentDir">The configuration file parent parent dir.</param>
        private static void DeleteOldSettingsFolders(DirectoryInfo configFileParentParentDir)
        {
            if (configFileParentParentDir.Parent == null || !Directory.Exists(configFileParentParentDir.Parent.FullName))
                return;

            foreach (string directory in Directory.GetDirectories(configFileParentParentDir.Parent.FullName)
                .Where(directory => directory != configFileParentParentDir.FullName &&
                                    directory.StartsWith(EveMonClient.FileVersionInfo.ProductName,
                                        StringComparison.OrdinalIgnoreCase)))
            {
                // Delete the folder recursively
                Directory.Delete(directory, true);
            }
        }

        /// <summary>
        /// Saves the queried settings file.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="System.ArgumentNullException">settingsFile</exception>
        private static void SaveSettingsFile(CloudStorageServiceAPIFile result)
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

        /// <summary>
        /// Asynchronously gets a mapped API file.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        protected static Task<SerializableAPIResult<CloudStorageServiceAPIFile>> GetMappedAPIFileAsync(
            SerializableAPIResult<CloudStorageServiceAPIFile> result, Stream response)
        {
            if (response == null)
                return null;

            return Task.Run(() =>
            {
                string content;
                using (StreamReader reader = new StreamReader(Util.ZlibUncompress(response)))
                    content = reader.ReadToEnd();

                if (String.IsNullOrWhiteSpace(content))
                {
                    result.Error = new SerializableAPIError
                    {
                        ErrorMessage = @"The settings file was not in a correct format."
                    };
                    return result;
                }

                result.Result = new CloudStorageServiceAPIFile
                {
                    FileName = $"{SettingsFileNameWithoutExtension}.xml",
                    FileContent = content
                };
                return result;
            });
        }

        #endregion
    }
}
