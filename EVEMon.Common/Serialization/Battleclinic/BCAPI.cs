using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace EVEMon.Common.Serialization.BattleClinic
{
    /// <summary>
    /// A delegate for BattleClinic query callbacks.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="errorMessage"></param>
    public delegate void QueryCallback<T>(BCAPIResult<T> result, string errorMessage);

    public static class BCAPI
    {
        private static readonly List<BCAPIMethod> s_methods = new List<BCAPIMethod>(BCAPIMethod.CreateDefaultSet());
        private static string s_apiKey;
        private static bool s_queryPending;


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the BattleClinic API credentials are authenticated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the BattleClinic API credentials are authenticated; otherwise, <c>false</c>.
        /// </value>
        public static bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the BattleClinic API credentials are stored.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the BattleClinic API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public static bool HasCredentialsStored
        {
            get
            {
                return BCAPISettings.Default.BCUserID != 0
                       && !String.IsNullOrEmpty(BCAPISettings.Default.BCAPIKey);
            }
        }

        /// <summary>
        /// Gets the content of the settings file.
        /// </summary>
        /// <value>The content of the settings file.</value>
        private static string SettingsFileContent
        {
            get
            {
                string settingsFileContent = File.ReadAllText(EveMonClient.SettingsFileNameFullPath);
                return HttpUtility.UrlEncode(settingsFileContent);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the provider supports compressed responses.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider supports compressed responses; otherwise, <c>false</c>.
        /// </value>
        private static bool SupportsCompressedResponse
        {
            get { return true; }
        }

        #endregion


        #region Helper Methods

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
            BCAPISettings.Default.Upgrade();

            // Delete all old settings files
            foreach (string directory in Directory.GetDirectories(configFileParentParentDir).Where(
                directory => directory != configFileParentDir))
            {
                // Delete the folder recursively
                Directory.Delete(directory, true);
            }
        }

        /// <summary>
        /// Returns the request method.
        /// </summary>
        /// <param name="requestMethod">A BCAPIMethods enumeration member specfying the method for which the URL is required.</param>
        private static BCAPIMethod GetMethod(BCAPIMethods requestMethod)
        {
            foreach (BCAPIMethod method in s_methods.Where(method => method.Method.Equals(requestMethod)))
            {
                return method;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the full canonical URL for the specified BCAPIMethod as constructed from the Server and BCAPIMethod properties.
        /// </summary>
        /// <param name="requestMethod">A BCAPIMethods enumeration member specfying the method for which the URL is required.</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        private static Uri GetMethodUrl(BCAPIMethods requestMethod)
        {
            // Build the uri
            Uri baseUri = new Uri(NetworkConstants.BCAPIBase);
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = String.Format(CultureConstants.InvariantCulture, "{0}{1}",
                                            uriBuilder.Path.TrimEnd("/".ToCharArray()), GetMethod(requestMethod).Path);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Checks the BattleClinic API credentials.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="apiKey">The API key.</param>
        public static void CheckAPICredentials(uint userID, string apiKey)
        {
            if (s_queryPending)
                return;

            s_queryPending = true;
            IsAuthenticated = false;

            s_apiKey = apiKey;
            CheckAPICredentialsAsync(userID, apiKey, OnCredentialsQueried);
        }

        /// <summary>
        /// Uploads the settings file.
        /// </summary>
        public static bool UploadSettingsFile()
        {
            if (!BCAPISettings.Default.UploadAlways || !HasCredentialsStored)
                return true;

            // Quit if user is not authenticated
            if (!IsAuthenticated && !CheckAPICredentials())
            {
                MessageBox.Show("The BattleClinic API credentials could not be authenticated.", "BattleClinic API Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            EveMonClient.Trace("BCAPI.UploadSettingsFile - Initiated");

            // Ask for user action if uploading fails
            while (true)
            {
                BCAPIResult<SerializableBCAPIFiles> result = FileSave();

                if (!result.HasError)
                {
                    EveMonClient.Trace("BCAPI.UploadSettingsFile - Completed");
                    return true;
                }

                DialogResult dialogResult = MessageBox.Show(result.Error.ErrorMessage, "BattleClinic API Error",
                                                            MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.Abort)
                {
                    EveMonClient.Trace("BCAPI.UploadSettingsFile - Failed and Aborted");
                    return false;
                }

                if (dialogResult == DialogResult.Retry)
                    continue;

                EveMonClient.Trace("BCAPI.UploadSettingsFile - Failed and Ignored");
                return true;
            }
        }

        /// <summary>
        /// Downloads the settings file.
        /// </summary>
        public static SerializableFilesListItem DownloadSettingsFile()
        {
            if (!BCAPISettings.Default.DownloadAlways || !HasCredentialsStored)
                return null;

            if (!IsAuthenticated && !CheckAPICredentials())
            {
                MessageBox.Show("The BattleClinic API credentials could not be authenticated.",
                                "BattleClinic API Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            EveMonClient.Trace("BCAPI.DownloadSettingsFile - Initiated");

            SerializableFilesListItem settingsFile = null;
            if (BCAPISettings.Default.UseImmediately)
            {
                BCAPIResult<SerializableBCAPIFiles> result = FileGetByName();
                if (result.HasError)
                {
                    MessageBox.Show(String.Format(CultureConstants.DefaultCulture,
                                                  "File could not be downloaded.\n\nThe error was:\n{0}",
                                                  result.Error.ErrorMessage),
                                    "BattleClinic API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    settingsFile = result.Result.Files.First();
                    EveMonClient.Trace("BCAPI.DownloadSettingsFile - Completed");
                }
            }
            else
                FileGetByNameAsync(OnFileGetByName);

            return settingsFile;
        }

        #endregion


        #region Queries

        /// <summary>
        /// Checks the BattleClinic API credentials.
        /// </summary>
        /// <returns><c>true</c> if credentials are authenticated; otherwise <c>false</c></returns>
        public static bool CheckAPICredentials()
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                                            "userID={0}&apiKey={1}&applicationKey={2}",
                                            BCAPISettings.Default.BCUserID,
                                            BCAPISettings.Default.BCAPIKey,
                                            BCAPISettings.Default.BCApplicationKey);

            BCAPIResult<SerializableBCAPICredentials> result =
                QueryMethod<SerializableBCAPICredentials>(BCAPIMethods.CheckCredentials, postData);
            IsAuthenticated = !result.HasError;
            return IsAuthenticated;
        }

        /// <summary>
        /// Saves a file content to the BattleClinic server.
        /// </summary>
        /// <returns><c>true</c> if file saving succeded; otherwise <c>false</c></returns>
        public static BCAPIResult<SerializableBCAPIFiles> FileSave()
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                                            "userID={0}&apiKey={1}&applicationKey={2}&id=0&name={3}&content={4}",
                                            BCAPISettings.Default.BCUserID, BCAPISettings.Default.BCAPIKey,
                                            BCAPISettings.Default.BCApplicationKey,
                                            EveMonClient.SettingsFileName, SettingsFileContent);

            return QueryMethod<SerializableBCAPIFiles>(BCAPIMethods.FileSave, postData, DataCompression.Gzip);
        }

        /// <summary>
        /// Gets the file content by the file name.
        /// </summary>
        public static BCAPIResult<SerializableBCAPIFiles> FileGetByName()
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                                            "userID={0}&apiKey={1}&applicationKey={2}&fileName={3}",
                                            BCAPISettings.Default.BCUserID, BCAPISettings.Default.BCAPIKey,
                                            BCAPISettings.Default.BCApplicationKey,
                                            EveMonClient.SettingsFileName);

            return QueryMethod<SerializableBCAPIFiles>(BCAPIMethods.FileGetByName, postData);
        }

        /// <summary>
        /// Asyncronously checks the BattleClinic API credentials.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="callback">The callback.</param>
        public static void CheckAPICredentialsAsync(uint userID, string apiKey,
                                                    QueryCallback<SerializableBCAPICredentials> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                                            "userID={0}&apiKey={1}&applicationKey={2}",
                                            userID, apiKey, BCAPISettings.Default.BCApplicationKey);

            QueryMethodAsync(BCAPIMethods.CheckCredentials, callback, postData);
        }

        /// <summary>
        /// Asyncronously saves a file content to the BattleClinic server.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public static void FileSaveAsync(QueryCallback<SerializableBCAPIFiles> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                                            "userID={0}&apiKey={1}&applicationKey={2}&id=0&name={3}&content={4}",
                                            BCAPISettings.Default.BCUserID, BCAPISettings.Default.BCAPIKey,
                                            BCAPISettings.Default.BCApplicationKey,
                                            EveMonClient.SettingsFileName, SettingsFileContent);

            QueryMethodAsync(BCAPIMethods.FileSave, callback, postData, DataCompression.Gzip);
        }

        /// <summary>
        /// Asyncronously gets the file content by the file name.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public static void FileGetByNameAsync(QueryCallback<SerializableBCAPIFiles> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                                            "userID={0}&apiKey={1}&applicationKey={2}&fileName={3}",
                                            BCAPISettings.Default.BCUserID, BCAPISettings.Default.BCAPIKey,
                                            BCAPISettings.Default.BCApplicationKey,
                                            EveMonClient.SettingsFileName);

            QueryMethodAsync(BCAPIMethods.FileGetByName, callback, postData);
        }

        #endregion


        #region Querying helpers

        /// <summary>
        /// Query this method with the provided HTTP POST data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="dataCompression">The data compression.</param>
        /// <returns></returns>
        private static BCAPIResult<T> QueryMethod<T>(BCAPIMethods method, string postData = null,
                                                     DataCompression dataCompression = DataCompression.None)
        {
            Uri url = GetMethodUrl(method);
            var result = Util.DownloadBCAPIResult<T>(url, SupportsCompressedResponse, postData, dataCompression);
            return result.HasError && dataCompression != DataCompression.None
                       ? Util.DownloadBCAPIResult<T>(url, SupportsCompressedResponse, postData)
                       : result;
        }

        /// <summary>
        /// Asynchrnoneously queries this method with the provided HTTP POST data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="dataCompression">The data compression.</param>
        private static void QueryMethodAsync<T>(BCAPIMethods method, QueryCallback<T> callback, string postData = null,
                                                DataCompression dataCompression = DataCompression.None)
        {
            // Check callback not null
            if (callback == null)
                throw new ArgumentNullException("callback", "The callback cannot be null.");

            Uri url = GetMethodUrl(method);
            Util.DownloadBCAPIResultAsync<T>(
                url,
                (result, errorMessage) =>
                    {
                        if ((!String.IsNullOrEmpty(errorMessage) || result.HasError) && dataCompression != DataCompression.None)
                        {
                            QueryMethodAsync(method, callback, postData);
                            return;
                        }
                        callback(result, errorMessage);
                    }, SupportsCompressedResponse, postData, dataCompression);
        }

        /// <summary>
        /// Occurs when the BattleClinic credentials get queried.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private static void OnCredentialsQueried(BCAPIResult<SerializableBCAPICredentials> result, string errorMessage)
        {
            s_queryPending = false;

            if (!String.IsNullOrEmpty(errorMessage))
            {
                EveMonClient.OnBCAPICredentialsUpdated(errorMessage);
                return;
            }

            if (result.HasError)
            {
                EveMonClient.OnBCAPICredentialsUpdated(result.Error.ErrorMessage);
                return;
            }

            IsAuthenticated = true;

            BCAPISettings.Default.BCUserID = Convert.ToUInt32(result.Result.UserID);
            BCAPISettings.Default.BCAPIKey = s_apiKey;
            BCAPISettings.Default.Save();

            EveMonClient.OnBCAPICredentialsUpdated(String.Empty);
        }

        /// <summary>
        /// Occurs when the FileGetByName get queried.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private static void OnFileGetByName(BCAPIResult<SerializableBCAPIFiles> result, string errorMessage)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (result.HasError)
            {
                MessageBox.Show(result.Error.ErrorMessage, "BattleClinic API Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            EveMonClient.Trace("BCAPI.DownloadSettingsFile - Completed");

            SaveSettingsFile(result.Result.Files.First());
        }

        /// <summary>
        /// Saves the queried settings file.
        /// </summary>
        /// <param name="settingsFile">The settings file.</param>
        public static void SaveSettingsFile(SerializableFilesListItem settingsFile)
        {
            if (settingsFile == null)
                throw new ArgumentNullException("settingsFile");

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "EVEMon Settings Backup File Save";
                saveFileDialog.DefaultExt = "bak";
                saveFileDialog.Filter = "EVEMon Settings Backup Files (*.bak)|*.bak";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                // Prompts the user for a location
                saveFileDialog.FileName = String.Format(CultureConstants.DefaultCulture, "{0}.bak", settingsFile.FileName);
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                DialogResult result = saveFileDialog.ShowDialog();

                // Save settings file if OK
                if (result != DialogResult.OK)
                    return;

                // Save the file to destination
                File.WriteAllText(saveFileDialog.FileName, settingsFile.FileContent);
            }
        }

        #endregion
    }
}