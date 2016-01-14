using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Helpers;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using File = Google.Apis.Drive.v3.Data.File;

namespace EVEMon.Common.CloudStorageServices.GoogleDrive
{
    public sealed class GoogleDriveCloudStorageServiceProvider : CloudStorageServiceProvider
    {
        private static UserCredential s_credential;
        private static string s_fileId;

        private const string Spaces = "appDataFolder";
        private const string ContentType = "application/xml";


        #region Properties

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Google Drive";

        /// <summary>
        /// Gets the authentication steps.
        /// </summary>
        /// <value>
        /// The authentication steps.
        /// </value>
        public override AuthenticationSteps AuthenticationSteps => AuthenticationSteps.One;

        /// <summary>
        /// Gets a value indicating whether this <see cref="CloudStorageServiceProvider" /> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        protected override bool Enabled => EveMonClient.IsDebugBuild || EveMonClient.IsSnapshotBuild;

        /// <summary>
        /// Gets a value indicating whether the provider API credentials are stored.
        /// </summary>
        /// <value>
        /// <c>true</c> if the provider API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public override bool HasCredentialsStored
            => !String.IsNullOrEmpty(GoogleDriveCloudStorageServiceSettings.Default.AccessToken);

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected override ApplicationSettingsBase Settings => GoogleDriveCloudStorageServiceSettings.Default;

        /// <summary>
        /// Gets the refferal link.
        /// </summary>
        /// <value>
        /// The refferal link.
        /// </value>
        public override Uri RefferalLink => new Uri("https://accounts.google.com/SignUp");

        #endregion


        #region Implementation Methods

        /// <summary>
        /// Checks the provider authentication with credentials is valid asynchronous.
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
        protected override async void RequestProviderAuthCodeAsync()
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = Util.Decrypt(GoogleDriveCloudStorageServiceSettings.Default.AppKey,
                    CultureConstants.InvariantCulture.NativeName),
                ClientSecret = Util.Decrypt(GoogleDriveCloudStorageServiceSettings.Default.AppSecret,
                    CultureConstants.InvariantCulture.NativeName)
            };

            s_credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets,
                new[] { DriveService.Scope.DriveAppdata }, "user", CancellationToken.None);

            CheckAuthCodeAsync(String.Empty);
        }

        /// <summary>
        /// Asynchronously checks the provider authentication code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> CheckProviderAuthCodeAsync(
            string code)
            => await Task.Run(() =>
            {
                GoogleDriveCloudStorageServiceSettings.Default.AccessToken = s_credential.Token.AccessToken;
                return new SerializableAPIResult<CloudStorageServiceAPICredentials>();
            });

        /// <summary>
        /// Asynchronously checks the access token.
        /// </summary>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> CheckAccessTokenAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            try
            {
                if (!HasCredentialsStored)
                    return result;

                if (s_credential == null)
                    RequestAuthCodeAsync();

                using (DriveService client = GetClient())
                {
                    AboutResource.GetRequest request = client.About.Get();
                    request.Fields = "user";

                    await request.ExecuteAsync();
                }
            }
            catch (GoogleApiException exc)
            {
                string errorMessage =
                    exc.Message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Skip(1)
                        .FirstOrDefault() ?? exc.Message;
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = errorMessage };

                if (s_credential == null && HasCredentialsStored)
                    ResetSettingsAsync();
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        /// <summary>
        /// Asynchronously revokes the access token.
        /// </summary>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPICredentials>> RevokeAccessTokenAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPICredentials> result =
                new SerializableAPIResult<CloudStorageServiceAPICredentials>();

            try
            {
                Task<bool> revokeTokenAsync = s_credential?.RevokeTokenAsync(CancellationToken.None);
                bool success = revokeTokenAsync != null && await revokeTokenAsync;

                if (revokeTokenAsync != null && !success)
                {
                    result.Error = new CloudStorageServiceAPIError { ErrorMessage = "Unable to revoke token" };
                }
            }
            catch (TokenResponseException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return result;
            }
            catch (Exception exc)
            {
                result.Error = new CloudStorageServiceAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        /// <summary>
        /// Uploads the file asynchronously.
        /// </summary>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> UploadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                s_fileId = s_fileId ?? await GetFileId();

                byte[] content = Util.GZipCompress(SettingsFileContentByteArray).ToArray();

                using (DriveService client = GetClient())
                using (Stream stream = Util.GetMemoryStream(content))
                {
                    ResumableUpload<File, File> request;
                    File fileMetadata = new File { Name = SettingsFileNameWithoutExtension };
                    if (String.IsNullOrWhiteSpace(s_fileId))
                    {
                        //Upload
                        fileMetadata.Parents = new List<string> { Spaces };
                        request = client.Files.Create(fileMetadata, stream, ContentType);
                        ((FilesResource.CreateMediaUpload)request).Fields = "id, name";
                    }
                    else
                    {
                        //Update
                        request = client.Files.Update(fileMetadata, s_fileId, stream, ContentType);
                        ((FilesResource.UpdateMediaUpload)request).AddParents = Spaces;
                        ((FilesResource.UpdateMediaUpload)request).Fields = "id, name";
                    }

                    // Do the actual upload
                    var response = await request.UploadAsync();
                    s_fileId = request.ResponseBody?.Id;

                    // Chceck response for exception
                    if (response.Exception != null)
                    {
                        result.Error = new CloudStorageServiceAPIError { ErrorMessage = response.Exception.Message };
                    }
                }
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
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> DownloadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                s_fileId = s_fileId ?? await GetFileId();

                if (String.IsNullOrWhiteSpace(s_fileId))
                    throw new FileNotFoundException();

                using (DriveService client = GetClient())
                using (Stream stream = new MemoryStream())
                {
                    FilesResource.GetRequest request = client.Files.Get(s_fileId);
                    request.Fields = "id, name";

                    IDownloadProgress response = await request.DownloadAsync(stream);

                    if (response.Exception == null)
                        return await GetMappedAPIFile(result, stream);

                    result.Error = new CloudStorageServiceAPIError { ErrorMessage = response.Exception.Message };
                }
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
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
        private static DriveService GetClient()
        {
            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = s_credential,
                ApplicationName = EveMonClient.FileVersionInfo.ProductName,
            };

            return new DriveService(initializer);
        }

        /// <summary>
        /// Gets the file identifier.
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetFileId()
        {
            FileList result;
            using (DriveService client = GetClient())
            {
                FilesResource.ListRequest listRequest = client.Files.List();
                listRequest.Spaces = Spaces;
                listRequest.Fields = "files(id, name)";

                result = await listRequest.ExecuteAsync();
            }

            return result.Files.SingleOrDefault(file => file.Name == SettingsFileNameWithoutExtension)?.Id;
        }

        /// <summary>
        /// Gets the mapped API file.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private static async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> GetMappedAPIFile(
            SerializableAPIResult<CloudStorageServiceAPIFile> result, Stream response)
        {
            if (response == null)
                return null;

            return await Task.Run(() =>
            {
                string content;
                using (StreamReader reader = new StreamReader(Util.ZlibUncompress(response)))
                    content = reader.ReadToEnd();

                if (String.IsNullOrWhiteSpace(content))
                {
                    result.Error = new CloudStorageServiceAPIError
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
