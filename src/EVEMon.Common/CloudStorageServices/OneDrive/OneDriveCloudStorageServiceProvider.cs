using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using Microsoft.OneDrive.Sdk;
using Microsoft.OneDrive.Sdk.WindowsForms;

namespace EVEMon.Common.CloudStorageServices.OneDrive
{
    public sealed class OneDriveCloudStorageServiceProvider : CloudStorageServiceProvider
    {
        private const string RedirectUri = "https://login.live.com/oauth20_desktop.srf";

        private static readonly string[] s_scopes = { "wl.offline_access", "onedrive.appfolder" };

        private string m_fileId;


        #region Properties

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "OneDrive";

        /// <summary>
        /// Gets the authentication steps.
        /// </summary>
        /// <value>
        /// The authentication steps.
        /// </value>
        public override AuthenticationSteps AuthSteps => AuthenticationSteps.One;

        /// <summary>
        /// Gets a value indicating whether this <see cref="CloudStorageServiceProvider" /> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        protected override bool Enabled => true;

        /// <summary>
        /// Gets a value indicating whether the provider API credentials are stored.
        /// </summary>
        /// <value>
        /// <c>true</c> if the provider API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public override bool HasCredentialsStored
            => !String.IsNullOrEmpty(OneDriveCloudStorageServiceSettings.Default.RefreshToken);

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected override ApplicationSettingsBase Settings => OneDriveCloudStorageServiceSettings.Default;

        #endregion


        #region Implementation Methods

        /// <summary>
        /// Asynchronously checks that the provider authentication with credentials is valid.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override Task<SerializableAPIResult<CloudStorageServiceAPICredentials>>
            CheckProviderAuthWithCredentialsIsValidAsync(uint userId, string apiKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously requests the provider an authentication code.
        /// </summary>
        /// <returns></returns>
        protected override Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> RequestProviderAuthCodeAsync()
        {
            CheckAuthCodeAsync(String.Empty);

            return Task.FromResult(new SerializableAPIResult<CloudStorageServiceAPICredentials>());
        }

        /// <summary>
        /// Asynchronously checks the provider authentication code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> CheckProviderAuthCodeAsync(
            string code)
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            try
            {
                // Checks that the client is authenticated
                // Settings save is done on caller method
                return await CheckAuthAsync();
            }
            catch (OneDriveException exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        /// <summary>
        /// Asynchronously checks the authentication.
        /// </summary>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> CheckAuthenticationAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            try
            {
                if (!HasCredentialsStored)
                    return result;

                // Checks that the client is authenticated
                // Settings save is done on called method
                return await CheckAuthAsync(saveOnChangeCheck: true);
            }
            catch (OneDriveException exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        /// <summary>
        /// Asynchronously revokes the authorization.
        /// </summary>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> RevokeAuthorizationAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            try
            {
                using (OneDriveClient client = (OneDriveClient)await GetClient())
                {
                    bool cansignout = client.AuthenticationProvider.CurrentAccountSession.CanSignOut;

                    if (cansignout)
                        await client.AuthenticationProvider.SignOutAsync();
                    else
                        result.Error = new CloudStorageServiceAPIError { ErrorMessage = "Unable to revoke authorization" };
                }
            }
            catch (OneDriveException exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        protected override async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> UploadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                m_fileId = m_fileId ?? await GetFileId();

                byte[] content = Util.GZipCompress(SettingsFileContentByteArray).ToArray();

                using (OneDriveClient client = (OneDriveClient)await GetClient())
                using (Stream stream = Util.GetMemoryStream(content))
                {
                    Item response = await (String.IsNullOrWhiteSpace(m_fileId)
                        ? client.Drive.Special.AppRoot
                            .ItemWithPath(Uri.EscapeUriString(SettingsFileNameWithoutExtension))
                        : client.Drive.Items[m_fileId])
                        .Content.Request().PutAsync<Item>(stream);

                    m_fileId = response?.Id;
                }
            }
            catch (OneDriveException exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        protected override async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> DownloadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                m_fileId = m_fileId ?? await GetFileId();

                if (String.IsNullOrWhiteSpace(m_fileId))
                    throw new FileNotFoundException();

                using (OneDriveClient client = (OneDriveClient)await GetClient())
                {
                    Stream stream = await client.Drive.Items[m_fileId].Content.Request().GetAsync();
                    return await GetMappedAPIFile(result, stream);
                }
            }
            catch (OneDriveException exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Asynchronously checks the authentication.
        /// </summary>
        /// <param name="saveOnChangeCheck">if set to <c>true</c> save the settings on change check.</param>
        /// <returns></returns>
        private async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> CheckAuthAsync(bool saveOnChangeCheck = false)
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            using (OneDriveClient client = (OneDriveClient)await GetClient())
            {
                if (!client.IsAuthenticated)
                {
                    result.Error = new CloudStorageServiceAPIError { ErrorMessage = "The client could not be authenticated" };
                    return result;
                }

                AuthenticationProvider authenticationProvider =
                    (MicrosoftAccountAuthenticationProvider)client.AuthenticationProvider;

                if (authenticationProvider.CurrentAccountSession.RefreshToken ==
                    OneDriveCloudStorageServiceSettings.Default.RefreshToken)
                {
                    return result;
                }

                OneDriveCloudStorageServiceSettings.Default.Credentials =
                    Encoding.Default.GetString(authenticationProvider.ServiceInfo.CredentialCache.GetCacheBlob());
                OneDriveCloudStorageServiceSettings.Default.UserId = authenticationProvider.CurrentAccountSession.UserId;
                OneDriveCloudStorageServiceSettings.Default.RefreshToken =
                    authenticationProvider.CurrentAccountSession.RefreshToken;

                if (saveOnChangeCheck)
                    Settings.Save();
            }

            return result;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
        private static async Task<IOneDriveClient> GetClient()
        {
            CredentialCache credentialCache = new CredentialCache();
            string credentials = OneDriveCloudStorageServiceSettings.Default.Credentials;

            if (!String.IsNullOrWhiteSpace(credentials))
                credentialCache.InitializeCacheFromBlob(Encoding.Default.GetBytes(credentials));

            IServiceInfoProvider serviceInfoProvider = new ServiceInfoProvider(new FormsWebAuthenticationUi())
            {
                UserSignInName = OneDriveCloudStorageServiceSettings.Default.UserId
            };

            return await OneDriveClient.GetAuthenticatedMicrosoftAccountClient(
                Util.Decrypt(OneDriveCloudStorageServiceSettings.Default.AppKey, CultureConstants.InvariantCulture.NativeName),
                RedirectUri, s_scopes, serviceInfoProvider, credentialCache);
        }

        /// <summary>
        /// Gets the file identifier.
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetFileId()
        {
            try
            {
                using (OneDriveClient client = (OneDriveClient)await GetClient())
                {
                    IItemRequest request = client.Drive.Special.AppRoot
                        .ItemWithPath($"/{Uri.EscapeUriString(SettingsFileNameWithoutExtension)}")
                        .Request();
                    Item response = await request.GetAsync();
                    return response.Id;
                }
            }
            catch (OneDriveException exc)
            {
                if (exc.IsMatch(OneDriveErrorCode.ItemNotFound.ToString()))
                    return String.Empty;

                throw;
            }
        }

        #endregion

    }
}
