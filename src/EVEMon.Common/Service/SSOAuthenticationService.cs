using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
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
        // The SSO client ID
        private readonly string clientID;
        // List of scopes that are to be used
        private readonly string scopes;
        // The SSO client secret
        private readonly string secret;

        public SSOAuthenticationService(string clientID, string secret, string scopes)
        {
            if (string.IsNullOrEmpty(clientID))
                throw new ArgumentNullException("clientID");
            if (string.IsNullOrEmpty(scopes))
                throw new ArgumentNullException("scopes");
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");
            this.clientID = clientID;
            this.scopes = scopes;
            this.secret = secret;
        }
        // Obtains the character authenticated on the specified token
        public static async Task<long> GetCharacterUsed(string token)
        {
            long id = 0L;
            var character = await Util.DownloadJsonAsync<ESICharacter>(new Uri(
                NetworkConstants.SSOBase + NetworkConstants.SSOCharID), token);
            if (character.Error == null)
                id = character.Result.CharacterID;
            return id;
        }
        // Spawns a browser for the user to log in; the port is the location of the local
        // web server which receives the response, the state is used to stop XSRF
        // (make it random!)
        public void SpawnBrowserForLogin(string state, int port)
        {
            string redirect = WebUtility.UrlEncode(string.Format(NetworkConstants.SSORedirect,
                port));
            Util.OpenURL(new Uri(string.Format(NetworkConstants.SSOBase +
                NetworkConstants.SSOLogin, redirect, state, scopes, clientID)));
        }
        // Obtains an access token from the auth code
        public async Task<string> VerifyAuthCode(string authCode)
        {
            string data = string.Format(NetworkConstants.PostDataWithToken, WebUtility.
                UrlEncode(authCode));
            // Build basic auth header
            string key = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientID +
                ":" + secret)), token = null;
            var response = await Util.DownloadJsonAsync<AccessResponse>(new Uri(
                NetworkConstants.SSOBase + NetworkConstants.SSOToken), key, postData: data);
            if (response.Error == null)
                token = response.Result.AccessToken ?? string.Empty;
            return token;
        }

        /// <summary>
        /// Template class for verification responses from SSO authentication.
        /// </summary>
        [DataContract]
        private sealed class AccessResponse
        {
            [DataMember(Name = "access_token")]
            public string AccessToken { get; set; }
            [DataMember(Name = "refresh_token")]
            public string RefreshToken { get; set; }

            public AccessResponse()
            {
                AccessToken = string.Empty;
                RefreshToken = string.Empty;
            }
        }

        /// <summary>
		/// Template class for an ESI character response.
		/// </summary>
		[DataContract]
        private sealed class ESICharacter : IComparable<ESICharacter>
        {
            // Character ID #
            [DataMember(Name = "CharacterID")]
            public long CharacterID { get; set; }
            // Character name
            [DataMember(Name = "CharacterName")]
            public string CharacterName { get; set; }
            // Date the token expires (JSON format)
            [DataMember(Name = "ExpiresOn")]
            public string ExpiresOnString
            {
                get
                {
                    // Report as "2016-05-31T04:02:16Z"
                    return expires.DateTimeToTimeString();
                }
                set
                {
                    // Parse from "2016-05-31T04:02:16Z"
                    if (!string.IsNullOrEmpty(value))
                        expires = value.TimeStringToDateTime();
                }
            }
            // Scopes authorized
            [DataMember(Name = "Scopes")]
            private string Scopes { get; set; }

            // When this token expires
            private DateTime expires;

            public ESICharacter()
            {
                expires = DateTime.UtcNow;
            }
            public int CompareTo(ESICharacter other)
            {
                if (other == null)
                    throw new ArgumentNullException("other");
                return CharacterID.CompareTo(other.CharacterID);
            }
            public override bool Equals(object obj)
            {
                var other = obj as ESICharacter;
                return other != null && CompareTo(other) == 0;
            }
            public override int GetHashCode()
            {
                return CharacterID.GetHashCode();
            }
            public override string ToString()
            {
                return CharacterName;
            }
        }
    }
}
