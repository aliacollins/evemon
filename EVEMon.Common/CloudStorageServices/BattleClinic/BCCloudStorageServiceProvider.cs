using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.BattleClinic.CloudStorage;

namespace EVEMon.Common.CloudStorageServices.BattleClinic
{
    /// <summary>
    /// A delegate for query callbacks.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="errorMessage"></param>
    public delegate void QueryCallback<T>(BCAPIResult<T> result, string errorMessage);

    public sealed class BCCloudStorageServiceProvider : CloudStorageServiceProvider
    {
        private static readonly List<BCAPIMethod> s_methods = new List<BCAPIMethod>(BCAPIMethod.CreateDefaultSet());
        private static string s_apiKey;
        private static bool s_queryPending;


        #region Properties

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get { return "BattleClinic"; }
        }

        /// <summary>
        /// Gets a value indicating whether the BattleClinic API credentials are stored.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the BattleClinic API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public override bool HasCredentialsStored
        {
            get
            {
                return CloudStorageServicesSettings.Default.BCUserID != 0
                       && !String.IsNullOrEmpty(CloudStorageServicesSettings.Default.BCAPIKey);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the provider supports compressed responses.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider supports compressed responses; otherwise, <c>false</c>.
        /// </value>
        protected override bool SupportsDataCompression
        {
            get { return true; }
        }

        #endregion


        #region Implementation Methods

        /// <summary>
        /// Checks the BattleClinic API credentials.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="apiKey">The API key.</param>
        public override void CheckAPICredentialsAsync(uint userID, string apiKey)
        {
            if (s_queryPending)
                return;

            s_queryPending = true;
            IsAuthenticated = false;

            s_apiKey = apiKey;

            EveMonClient.Trace("{0}.CheckAPICredentialsAsync - Initiated", Name);

            CheckAPICredentialsAsync(userID, apiKey, OnCredentialsQueried);
        }

        /// <summary>
        /// Uploads the settings file.
        /// </summary>
        public override bool UploadSettingsFile()
        {
            if (!CloudStorageServicesSettings.Default.UploadAlways || !HasCredentialsStored)
                return true;

            // Quit if user is not authenticated
            if (!IsAuthenticated && !CheckAPICredentials())
            {
                MessageBox.Show(@"The BattleClinic API credentials could not be authenticated.", @"BattleClinic API Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            EveMonClient.Trace("{0}.UploadSettingsFile - Initiated", Name);

            // Ask for user action if uploading fails
            while (true)
            {
                BCAPIResult<SerializableBCAPIFiles> result = FileSave();

                if (!result.HasError)
                {
                    EveMonClient.Trace("{0}.UploadSettingsFile - Completed", Name);
                    return true;
                }

                DialogResult dialogResult = MessageBox.Show(result.Error.ErrorMessage, @"BattleClinic API Error",
                    MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.Abort)
                {
                    EveMonClient.Trace("BCAPI.UploadSettingsFile - Failed and Aborted");
                    return false;
                }

                if (dialogResult == DialogResult.Retry)
                    continue;

                EveMonClient.Trace("{0}.UploadSettingsFile - Failed and Ignored", Name);
                return true;
            }
        }

        /// <summary>
        /// Downloads the settings file.
        /// </summary>
        public override object DownloadSettingsFile()
        {
            if (!CloudStorageServicesSettings.Default.DownloadAlways || !HasCredentialsStored)
                return null;

            if (!IsAuthenticated && !CheckAPICredentials())
            {
                MessageBox.Show(@"The BattleClinic API credentials could not be authenticated.",
                    @"BattleClinic API Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }

            EveMonClient.Trace("{0}.DownloadSettingsFile - Initiated", Name);

            SerializableFilesListItem settingsFile = null;
            if (CloudStorageServicesSettings.Default.UseImmediately)
            {
                BCAPIResult<SerializableBCAPIFiles> result = FileGetByName();
                if (result.HasError)
                {
                    MessageBox.Show(String.Format(CultureConstants.DefaultCulture,
                        "File could not be downloaded.\n\nThe error was:\n{0}",
                        result.Error.ErrorMessage),
                        @"BattleClinic API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    settingsFile = result.Result.Files.First();
                    EveMonClient.Trace("{0}.DownloadSettingsFile - Completed", Name);
                }
            }
            else
                DownloadSettingsFileAsync(OnDownloadSettingsFile);

            return settingsFile;
        }

        /// <summary>
        /// Uploads the settings file asynchronously.
        /// </summary>
        public override void UploadSettingsFileAsync()
        {
            UploadSettingsFileAsync(OnUploadSettingsFileAsync);
        }

        /// <summary>
        /// Downloads the settings file asynchronously.
        /// </summary>
        public override void DownloadSettingsFileAsync()
        {
            DownloadSettingsFileAsync(OnDownloadSettingsFileAsync);
        }

        /// <summary>
        /// Uploads the settings file asynchronously.
        /// </summary>
        /// <param name="callback">The callback.</param>
        private void UploadSettingsFileAsync(QueryCallback<SerializableBCAPIFiles> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                "userID={0}&apiKey={1}&applicationKey={2}&id=0&name={3}&content={4}",
                CloudStorageServicesSettings.Default.BCUserID, CloudStorageServicesSettings.Default.BCAPIKey,
                CloudStorageServicesSettings.Default.BCApplicationKey,
                EveMonClient.SettingsFileName, SettingsFileContent);

            QueryMethodAsync(BCAPIMethods.FileSave, callback, postData, DataCompression.Gzip);
        }

        /// <summary>
        /// Downloads the settings file asynchronous.
        /// </summary>
        /// <param name="callback">The callback.</param>
        private void DownloadSettingsFileAsync(QueryCallback<SerializableBCAPIFiles> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                "userID={0}&apiKey={1}&applicationKey={2}&fileName={3}",
                CloudStorageServicesSettings.Default.BCUserID, CloudStorageServicesSettings.Default.BCAPIKey,
                CloudStorageServicesSettings.Default.BCApplicationKey,
                EveMonClient.SettingsFileName);

            QueryMethodAsync(BCAPIMethods.FileGetByName, callback, postData, DataCompression.Gzip);
        }

        #endregion


        #region Helper Methods

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
            Uri baseUri = new Uri(NetworkConstants.BattleClinicAPIBase);
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = String.Format(CultureConstants.InvariantCulture, "{0}{1}",
                uriBuilder.Path.TrimEnd(Path.AltDirectorySeparatorChar), GetMethod(requestMethod).Path);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the mapped result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private static SerializableAPIResult<CloudStorageServiceAPICredentials> GetMappedAPICredentials(
            BCAPIResult<SerializableBCAPICredentials> result)
        {
            if (result == null)
                return null;

            var mappedResult = new SerializableAPIResult<CloudStorageServiceAPICredentials>
            {
                CacheExpires = result.CacheExpires,
                Error = result.Error != null
                    ? new CloudStorageServiceAPIError
                    {
                        ErrorCode = result.Error.ErrorCode,
                        ErrorMessage = result.Error.ErrorMessage
                    }
                    : null,
                Result = result.Result != null
                    ? new CloudStorageServiceAPICredentials { UserID = result.Result.UserID }
                    : null,
            };

            return mappedResult;
        }

        /// <summary>
        /// Gets the mapped API file.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private static SerializableAPIResult<CloudStorageServiceAPIFile> GetMappedAPIFile(
            BCAPIResult<SerializableBCAPIFiles> result)
        {
            if (result == null)
                return null;

            var mappedResult = new SerializableAPIResult<CloudStorageServiceAPIFile>
            {
                CacheExpires = result.CacheExpires,
                Error = result.Error != null
                    ? new CloudStorageServiceAPIError
                    {
                        ErrorCode = result.Error?.ErrorCode,
                        ErrorMessage = result.Error?.ErrorMessage
                    }
                    : null,
                Result = result.Result != null
                    ? new CloudStorageServiceAPIFile
                    {
                        FileName = result.Result.Files?.FirstOrDefault()?.FileName,
                        FileContent = result.Result.Files?.FirstOrDefault()?.FileContent
                    }
                    : null
            };

            return mappedResult;
        }

        #endregion


        #region Queries

        /// <summary>
        /// Checks the BattleClinic API credentials.
        /// </summary>
        /// <returns><c>true</c> if credentials are authenticated; otherwise <c>false</c></returns>
        private bool CheckAPICredentials()
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                "userID={0}&apiKey={1}&applicationKey={2}",
                CloudStorageServicesSettings.Default.BCUserID,
                CloudStorageServicesSettings.Default.BCAPIKey,
                CloudStorageServicesSettings.Default.BCApplicationKey);

            BCAPIResult<SerializableBCAPICredentials> result =
                QueryMethod<SerializableBCAPICredentials>(BCAPIMethods.CheckCredentials, postData);

            IsAuthenticated = !result.HasError;

            return IsAuthenticated;
        }

        /// <summary>
        /// Saves a file content to the BattleClinic server.
        /// </summary>
        /// <returns><c>true</c> if file saving succeded; otherwise <c>false</c></returns>
        private BCAPIResult<SerializableBCAPIFiles> FileSave()
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                "userID={0}&apiKey={1}&applicationKey={2}&id=0&name={3}&content={4}",
                CloudStorageServicesSettings.Default.BCUserID, CloudStorageServicesSettings.Default.BCAPIKey,
                CloudStorageServicesSettings.Default.BCApplicationKey,
                EveMonClient.SettingsFileName, SettingsFileContent);

            return QueryMethod<SerializableBCAPIFiles>(BCAPIMethods.FileSave, postData, DataCompression.Gzip);
        }

        /// <summary>
        /// Gets the file content by the file name.
        /// </summary>
        private BCAPIResult<SerializableBCAPIFiles> FileGetByName()
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                "userID={0}&apiKey={1}&applicationKey={2}&fileName={3}",
                CloudStorageServicesSettings.Default.BCUserID, CloudStorageServicesSettings.Default.BCAPIKey,
                CloudStorageServicesSettings.Default.BCApplicationKey,
                EveMonClient.SettingsFileName);

            return QueryMethod<SerializableBCAPIFiles>(BCAPIMethods.FileGetByName, postData);
        }

        /// <summary>
        /// Asyncronously checks the BattleClinic API credentials.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="callback">The callback.</param>
        private void CheckAPICredentialsAsync(uint userID, string apiKey, QueryCallback<SerializableBCAPICredentials> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture,
                "userID={0}&apiKey={1}&applicationKey={2}",
                userID, apiKey, CloudStorageServicesSettings.Default.BCApplicationKey);

            QueryMethodAsync(BCAPIMethods.CheckCredentials, callback, postData);
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
        private BCAPIResult<T> QueryMethod<T>(BCAPIMethods method, string postData = null,
            DataCompression dataCompression = DataCompression.None)
        {
            Uri url = GetMethodUrl(method);
            var result = Util.DownloadBCAPIResult<T>(url, SupportsDataCompression, postData, dataCompression);
            return result.HasError && dataCompression != DataCompression.None
                ? Util.DownloadBCAPIResult<T>(url, SupportsDataCompression, postData)
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
        private void QueryMethodAsync<T>(BCAPIMethods method, QueryCallback<T> callback, string postData = null,
            DataCompression dataCompression = DataCompression.None)
        {
            // Check callback not null
            if (callback == null)
                throw new ArgumentNullException("callback", @"The callback cannot be null.");

            Uri url = GetMethodUrl(method);
            Util.DownloadBCAPIResultAsync<T>(
                url,
                (result, errorMessage) =>
                {
                    // In case something went wrong with the compression request
                    // retry without compression
                    if ((!String.IsNullOrEmpty(errorMessage) || result.HasError) && dataCompression != DataCompression.None)
                    {
                        QueryMethodAsync(method, callback, postData);
                        return;
                    }
                    callback(result, errorMessage);
                }, SupportsDataCompression, postData, dataCompression);
        }

        /// <summary>
        /// Occurs when the BattleClinic credentials get queried.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private void OnCredentialsQueried(BCAPIResult<SerializableBCAPICredentials> result, string errorMessage)
        {
            s_queryPending = false;

            if (!String.IsNullOrEmpty(errorMessage) || result.HasError)
            {
                OnCredentialsChecked(GetMappedAPICredentials(result), errorMessage);
                return;
            }

            EveMonClient.Trace("{0}.CheckAPICredentialsAsync - Completed", Name);

            SerializableAPIResult<CloudStorageServiceAPICredentials> mappedAPIResult = GetMappedAPICredentials(result);
            CloudStorageServicesSettings.Default.BCUserID = Convert.ToUInt32(mappedAPIResult.Result.UserID);
            CloudStorageServicesSettings.Default.BCAPIKey = s_apiKey;
            CloudStorageServicesSettings.Default.Save();

            OnCredentialsChecked(mappedAPIResult, errorMessage);
        }

        /// <summary>
        /// Occurs when upload settings file completes.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private void OnUploadSettingsFileAsync(BCAPIResult<SerializableBCAPIFiles> result, string errorMessage)
        {
            OnFileUploaded(GetMappedAPIFile(result), errorMessage);
        }

        /// <summary>
        /// Occurs when download settings file completes.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private void OnDownloadSettingsFileAsync(BCAPIResult<SerializableBCAPIFiles> result, string errorMessage)
        {
            if (!String.IsNullOrEmpty(errorMessage) || result.HasError)
            {
                OnFileDownloaded(GetMappedAPIFile(result), errorMessage);
                return;
            }

            SerializableAPIResult<CloudStorageServiceAPIFile> mappedAPIFile = GetMappedAPIFile(result);

            SaveSettingsFile(mappedAPIFile.Result);

            OnFileDownloaded(mappedAPIFile, errorMessage);
        }

        /// <summary>
        /// Occurs when download settings file completes.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private void OnDownloadSettingsFile(BCAPIResult<SerializableBCAPIFiles> result, string errorMessage)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, @"Network Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (result.HasError)
            {
                MessageBox.Show(result.Error.ErrorMessage, @"BattleClinic API Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            EveMonClient.Trace("{0}.DownloadSettingsFileAsync - Completed", Name);

            SaveSettingsFile(GetMappedAPIFile(result).Result);
        }

        #endregion

    }
}
