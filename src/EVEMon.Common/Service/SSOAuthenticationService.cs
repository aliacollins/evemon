using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Threading;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EVEMon.Common.Service
{
    /// <summary>
    /// Uses the built-in TcpListener class to create a server to receive responses for SSO
    /// authentication.
    /// </summary>
    public sealed class SSOAuthenticationService
    {
        /// <summary>
        /// Starts obtaining information about the character used to authenticate the specified
        /// token.
        /// </summary>
        /// <param name="token">The auth token used.</param>
        /// <param name="callback">A callback to receive the token info.</param>
        public static void BeginGetTokenInfo(string token, Action<Task<EsiAPITokenInfo>> callback)
        {
            GetTokenInfoAsync(token).ContinueWith((result) => Dispatcher.Invoke(() =>
                callback?.Invoke(result)));
        }

        /// <summary>
        /// Obtains information about the character used to authenticate the specified token.
        /// </summary>
        /// <param name="token">The auth token used.</param>
        /// <returns>The token info, or null if it cannot be determined.</returns>
        public static async Task<EsiAPITokenInfo> GetTokenInfoAsync(string token)
        {
            EsiAPITokenInfo chr = null;
            var character = await Util.DownloadJsonAsync<EsiAPITokenInfo>(new Uri(
                NetworkConstants.SSOBase + NetworkConstants.SSOCharID), token);
            if (character.Error == null)
                chr = character.Result;
            return chr;
        }

        /// <summary>
        /// The SSO client ID.
        /// </summary>
        private readonly string m_clientID;

        /// <summary>
        /// List of scopes that are to be used.
        /// </summary>
        private readonly string m_scopes;

        /// <summary>
        /// The SSO client secret.
        /// </summary>
        private readonly string m_secret;

        public SSOAuthenticationService(string clientID, string secret, string scopes)
        {
            if (string.IsNullOrEmpty(clientID))
                throw new ArgumentNullException("clientID");
            if (string.IsNullOrEmpty(scopes))
                throw new ArgumentNullException("scopes");
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");
            m_clientID = clientID;
            m_scopes = scopes;
            m_secret = secret;
        }

        /// <summary>
        /// Starts obtaining a new access token from the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="callback">A callback to receive the new token.</param>
        public void BeginGetNewToken(string refreshToken, Action<Task<AccessResponse>> callback)
        {
            GetNewTokenAsync(refreshToken).ContinueWith((result) => Dispatcher.Invoke(() =>
                callback?.Invoke(result)));
        }

        /// <summary>
        /// Starts verifying the authentication code.
        /// </summary>
        /// <param name="authCode">The code to verify.</param>
        /// <param name="callback">A callback to receive the tokens.</param>
        public void BeginVerifyAuthCode(string authCode, Action<Task<AccessResponse>> callback)
        {
            VerifyAuthCodeAsync(authCode).ContinueWith((result) => Dispatcher.Invoke(() =>
                callback?.Invoke(result)));
        }

        /// <summary>
        /// Retrieves the "basic" authentication header used when retrieving tokens.
        /// </summary>
        /// <returns>The correct Basic authentication header with client ID/secret.</returns>
        private string GetBasicAuthHeader()
        {
            return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(m_clientID +
                ":" + m_secret));
        }

        /// <summary>
        /// Obtains a new access token from the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A new auth token, or null if retrieval fails.</returns>
        public async Task<AccessResponse> GetNewTokenAsync(string refreshToken)
        {
            refreshToken.ThrowIfNull(nameof(refreshToken));
            string data = string.Format(NetworkConstants.PostDataWithRefreshToken, WebUtility.
                UrlEncode(refreshToken));
            AccessResponse token = null;

            var url = new Uri(NetworkConstants.SSOBase + NetworkConstants.SSOToken);
            var response = await Util.DownloadJsonAsync<AccessResponse>(url,
                GetBasicAuthHeader(), postData: data);
            if (response.Error == null)
                token = response.Result;
            return token;
        }

        /// <summary>
        /// Spawns a browser for the user to log in; the port is the location of the local
        /// web server which receives the response, the state is used to stop XSRF
        /// (make it random!)
        /// </summary>
        /// <param name="state">The random state parameter used to stop cross-site forgery.</param>
        /// <param name="port">The port used for the response.</param>
        public void SpawnBrowserForLogin(string state, int port)
        {
            string redirect = string.Format(NetworkConstants.SSORedirect, port);
            string url = string.Format(NetworkConstants.SSOBase + NetworkConstants.SSOLogin,
                WebUtility.UrlEncode(redirect), state, m_scopes, m_clientID);
            Util.OpenURL(new Uri(url));
        }

        /// <summary>
        /// Obtains an access and refresh token from the auth code.
        /// </summary>
        /// <param name="authCode">The authentication code from CCP.</param>
        /// <returns>An auth and refresh token, or null if it could not be retrieved.</returns>
        public async Task<AccessResponse> VerifyAuthCodeAsync(string authCode)
        {
            authCode.ThrowIfNull(nameof(authCode));
            string data = string.Format(NetworkConstants.PostDataWithAuthToken, WebUtility.
                UrlEncode(authCode));
            AccessResponse token = null;

            var url = new Uri(NetworkConstants.SSOBase + NetworkConstants.SSOToken);
            var response = await Util.DownloadJsonAsync<AccessResponse>(url,
                GetBasicAuthHeader(), postData: data);
            if (response.Error == null)
                token = response.Result;
            return token;
        }
    }

    /// <summary>
    /// Template class for verification responses from SSO authentication.
    /// </summary>
    [DataContract]
    public sealed class AccessResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }
        [DataMember(Name = "expires_in")]
        private int ExpiresIn { get; set; }

        public DateTime ExpiryUTC
        {
            get
            {
                return obtained + TimeSpan.FromSeconds(ExpiresIn);
            }
        }

        private readonly DateTime obtained;

        public AccessResponse()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            obtained = DateTime.UtcNow;
        }
    }
}
