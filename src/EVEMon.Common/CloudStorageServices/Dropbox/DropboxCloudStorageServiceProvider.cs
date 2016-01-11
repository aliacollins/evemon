using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dropbox.Api;
using Dropbox.Api.Babel;
using Dropbox.Api.Files;
using Dropbox.Api.Users;
using EVEMon.Common.Constants;
using EVEMon.Common.Net;

namespace EVEMon.Common.CloudStorageServices.Dropbox
{
    public sealed class DropboxCloudStorageServiceProvider : CloudStorageServiceProvider
    {
        private static bool s_queryPending;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxCloudStorageServiceProvider"/> class.
        /// </summary>
        public DropboxCloudStorageServiceProvider()
        {
            InitializeCertPinning();
        }


        #region Properties

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Dropbox";

        /// <summary>
        /// Gets a value indicating whether this <see cref="CloudStorageServiceProvider" /> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        protected override bool Enabled => EveMonClient.IsDebugBuild;

        /// <summary>
        /// Gets a value indicating whether the provider API credentials are stored.
        /// </summary>
        /// <value>
        /// <c>true</c> if the provider API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public override bool HasCredentialsStored
            => !String.IsNullOrEmpty(DropboxCloudStorageServiceSettings.Default.DropboxAccessToken);

        /// <summary>
        /// Gets a value indicating whether the provider API credentials are stored.
        /// </summary>
        /// <value>
        /// <c>true</c> if the provider API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        protected override bool SupportsDataCompression => true;

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected override ApplicationSettingsBase Settings => DropboxCloudStorageServiceSettings.Default;
        
        /// <summary>
        /// Gets the refferal link.
        /// </summary>
        /// <value>
        /// The refferal link.
        /// </value>
        public override Uri RefferalLink => new Uri("https://www.dropbox.com/referrals/NTEyOTk1Njg4MDk?src=app9-891961");

        #endregion


        #region Implementation Methods

        /// <summary>
        /// Synchronously checks the API authentication with credentials is valid.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void CheckAPIAuthWithCredentialsIsValid(uint userID, string apiKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously checks the API authentication with credentials is valid.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void CheckAPIAuthWithCredentialsIsValidAsync(uint userID, string apiKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Synchronously checks that API authentication is valid.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool CheckAPIAuthIsValid() =>
            !Task.Run(async () => await CheckAccessTokenAsync()).Result.HasError;

        /// <summary>
        /// Asynchronously checks that API authentication is valid.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override async void CheckAPIAuthIsValidAsync()
        {
            if (s_queryPending)
                return;

            s_queryPending = true;
            IsAuthenticated = false;
            
            SerializableAPIResult<CloudStorageServiceAPICredentials> result = await CheckAccessTokenAsync();
            OnCredentialsChecked(result);
            s_queryPending = false;
        }

        /// <summary>
        /// Asynchronously requests an authentication code.
        /// </summary>
        public override void RequestAuthCodeAsync()
        {
            if (s_queryPending || HasCredentialsStored)
                return;

            s_queryPending = true;
            IsAuthenticated = false;

            string oauth2State = Guid.NewGuid().ToString("N");
            Uri authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code,
                DropboxCloudStorageServiceSettings.Default.DropboxAPIKey, String.Empty, oauth2State);

            Util.OpenURL(authorizeUri);

            s_queryPending = false;

            EveMonClient.Trace($"{Name}.CheckCredentialsAsync - Initiated");
        }

        /// <summary>
        /// Asynchronously checks the authentication code.
        /// </summary>
        /// <param name="code">The code.</param>
        public override async void CheckAuthCodeAsync(string code)
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            try
            {
                OAuth2Response response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code,
                    DropboxCloudStorageServiceSettings.Default.DropboxAPIKey,
                    DropboxCloudStorageServiceSettings.Default.DropBoxAppSecret);

                DropboxCloudStorageServiceSettings.Default.DropboxAccessToken = response.AccessToken;
                Settings.Save();
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }
            finally
            {
                s_queryPending = false;

                OnCredentialsChecked(result);
            }
        }

        /// <summary>
        /// Resets the settings.
        /// </summary>
        public override void ResetSettings()
        {
            Settings.Reset();
            OnSettingsReset();
        }

        /// <summary>
        /// Uploads the settings file.
        /// </summary>
        /// <returns></returns>
        public override bool UploadSettingsFile()
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

            EveMonClient.Trace($"{Name}.UploadSettingsFile - Initiated");

            // Ask for user action if uploading fails
            while (true)
            {
                SerializableAPIResult<CloudStorageServiceAPIFile> result = UploadFile();

                if (!result.HasError)
                {
                    EveMonClient.Trace($"{Name}.UploadSettingsFile - Completed");
                    return true;
                }

                DialogResult dialogResult = MessageBox.Show(result.Error.ErrorMessage, $"{Name} API Error",
                    MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                switch (dialogResult)
                {
                    case DialogResult.Abort:
                        EveMonClient.Trace($"{Name}.UploadSettingsFile - Failed and Aborted");
                        return false;
                    case DialogResult.Retry:
                        continue;
                }

                EveMonClient.Trace($"{Name}.UploadSettingsFile - Failed and Ignored", Name);
                return true;
            }
        }

        /// <summary>
        /// Downloads the settings file.
        /// </summary>
        /// <returns></returns>
        public override CloudStorageServiceAPIFile DownloadSettingsFile()
        {
            if (!CloudStorageServiceSettings.Default.DownloadAlways || !HasCredentialsStored)
                return null;

            if (!IsAuthenticated && !CheckAPIAuthIsValid())
            {
                MessageBox.Show($"The {Name} API credentials could not be authenticated.",
                    $"{Name} API Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }

            EveMonClient.Trace($"{Name}.DownloadSettingsFile - Initiated");

            SerializableAPIResult<CloudStorageServiceAPIFile> result = DownloadFile();
            OnFileDownloaded(result);

            if (CloudStorageServiceSettings.Default.UseImmediately)
            {
                if (result.HasError)
                {
                    MessageBox.Show(String.Format(CultureConstants.DefaultCulture,
                        $"File could not be downloaded.\n\nThe error was:\n{result.Error.ErrorMessage}"),
                        $"{Name} API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    EveMonClient.Trace($"{Name}.DownloadSettingsFile - Completed");
                    return result.Result;
                }
            }
            else
            {
                var actionText = result.HasError ? "Failed" : "Completed";
                EveMonClient.Trace($"{Name}.DownloadSettingsFile - {actionText}");

                if (!result.HasError)
                    SaveSettingsFile(result.Result);
            }

            return null;
        }

        /// <summary>
        /// Uploads the settings file asynchronously.
        /// </summary>
        public override async void UploadSettingsFileAsync()
        {
            if (s_queryPending)
                return;

            s_queryPending = true;

            SerializableAPIResult<CloudStorageServiceAPIFile> result = await UploadFileAsync();
            OnFileUploaded(result);

            s_queryPending = false;
        }

        /// <summary>
        /// Downloads the settings file asynchronously.
        /// </summary>
        public override async void DownloadSettingsFileAsync()
        {
            if (s_queryPending)
                return;

            s_queryPending = true;

            SerializableAPIResult<CloudStorageServiceAPIFile> result = await DownloadFileAsync();
            OnFileDownloaded(result);

            s_queryPending = false;

            if (result.HasError)
                return;

            SaveSettingsFile(result.Result);
        }

        #endregion


        #region Queries

        /// <summary>
        /// Asynchronously checks the access token.
        /// </summary>
        private async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> CheckAccessTokenAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            try
            {
                using (DropboxClient client = GetClient())
                {
                    await client.Users.GetCurrentAccountAsync();
                    IsAuthenticated = true;
                    return result;
                }
            }
            catch (ApiException<GetAccountError> exc)
            {
                IsAuthenticated = false;
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }
            catch (AuthException exc)
            {
                Dictionary<string, object> json = Util.DeserializeJsonToObject(exc.Message);
                string errorMessage = json[".tag"] as string ?? exc.Message;
                IsAuthenticated = false;
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = errorMessage };
            }
            catch (Exception exc)
            {
                IsAuthenticated = false;
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <returns></returns>
        private static SerializableAPIResult<CloudStorageServiceAPIFile> UploadFile() =>
            Task.Run(async () => await UploadFileAsync()).Result;

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <returns></returns>
        private SerializableAPIResult<CloudStorageServiceAPIFile> DownloadFile() =>
            Task.Run(async () => await DownloadFileAsync()).Result;

        /// <summary>
        /// Uploads the file asynchronously.
        /// </summary>
        private static async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> UploadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                byte[] content = Util.GZipCompress(SettingsFileContentByteArray).ToArray();
                CommitInfo commitInfo = new CommitInfo($"/{Path.GetFileNameWithoutExtension(EveMonClient.SettingsFileName)}",
                    WriteMode.Overwrite.Instance);

                using (DropboxClient client = GetClient())
                using (MemoryStream stream = Util.GetMemoryStream(content))
                {
                    await client.Files.UploadAsync(commitInfo, stream);
                    return result;
                }
            }
            catch (ApiException<UploadError> ex)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = ex.Message };
            }
            catch (Exception ex)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = ex.Message };
            }

            return result;
        }

        /// <summary>
        /// Downloads the file asynchronously.
        /// </summary>
        private async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> DownloadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                DownloadArg arg = new DownloadArg($"/{Path.GetFileNameWithoutExtension(EveMonClient.SettingsFileName)}");
                using (DropboxClient client = GetClient())
                {
                    IDownloadResponse<FileMetadata> response = await client.Files.DownloadAsync(arg);
                    result = await GetMappedAPIFile(response);
                }
            }
            catch (ApiException<DownloadError> ex)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = ex.Message };
            }
            catch (Exception ex)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = ex.Message };
            }

            return result;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Initializes the cert pinning.
        /// </summary>
        private static void InitializeCertPinning()
        {
            if (EveMonClient.IsDebugBuild)
            {
                ServicePointManager.ServerCertificateValidationCallback = HttpWebClientService.DummyCertificateValidationCallback;
                return;
            }

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                X509ChainElement root = chain.ChainElements[chain.ChainElements.Count - 1];
                string publicKey = root.Certificate.GetPublicKeyString();

                return DropboxCertHelper.IsKnownRootCertPublicKey(publicKey);
            };
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
        private static DropboxClient GetClient()
            => new DropboxClient(DropboxCloudStorageServiceSettings.Default.DropboxAccessToken,
                userAgent: HttpWebClientServiceState.UserAgent);


        /// <summary>
        /// Gets the mapped API file.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> GetMappedAPIFile(
            IDownloadResponse<FileMetadata> response)
        {
            if (response == null)
                return null;

            return await response.GetContentAsStreamAsync().ContinueWith(task =>
            {
                SerializableAPIResult<CloudStorageServiceAPIFile> result =
                    new SerializableAPIResult<CloudStorageServiceAPIFile>();

                string content;
                using (StreamReader reader = new StreamReader(Util.ZlibUncompress(task.Result)))
                    content = reader.ReadToEnd();

                if (String.IsNullOrWhiteSpace(content))
                {
                    result.Error = new CloudStorageServiceAPIError
                    {
                        ErrorMessage = @"The settings file was not in a correct format."
                    };
                    return result;
                }

                result.Result = response.Response != null
                    ? new CloudStorageServiceAPIFile
                    {
                        FileName = $"{response.Response.Name}.xml",
                        FileContent = content
                    }
                    : null;
                return result;
            });
        }

        #endregion
    }
}
