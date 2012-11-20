using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// A delegate for query callbacks.
    /// </summary>
    /// <param name="result"></param>
    public delegate void QueryCallback<T>(APIResult<T> result);

    /// <summary>
    /// Serializable class abstracting an API queries provider and its configuration.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class APIProvider
    {
        private static APIProvider s_ccpProvider;
        private static APIProvider s_ccpTestProvider;
        private static XslCompiledTransform s_rowsetsTransform;

        private readonly List<APIMethod> m_methods;
        private bool m_supportsCompressedResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal APIProvider()
        {
            m_methods = new List<APIMethod>(APIMethod.CreateDefaultSet());
            Url = new Uri("http://your-custom-API-provider.com");
            Name = "your provider's name";
        }


        #region Properties

        /// <summary>
        /// Returns the name of this APIConfiguration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the server host for this APIConfiguration.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Returns a list of APIMethods supported by this APIConfiguration.
        /// </summary>
        public IEnumerable<APIMethod> Methods
        {
            get { return m_methods; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the provider supports compressed responses.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider supports compressed responses; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsCompressedResponse
        {
            get { return IsDefault || this == TestProvider || m_supportsCompressedResponse; }
            set { m_supportsCompressedResponse = value; }
        }

        /// <summary>
        /// Returns true if this APIConfiguration represents the default API service.
        /// </summary>
        public bool IsDefault
        {
            get { return (this == DefaultProvider); }
        }

        /// <summary>
        /// Gets the default API provider
        /// </summary>
        public static APIProvider DefaultProvider
        {
            get { return s_ccpProvider ?? (s_ccpProvider = new APIProvider { Url = new Uri(NetworkConstants.APIBase), Name = "CCP" }); }
        }

        /// <summary>
        /// Gets the test API provider
        /// </summary>
        public static APIProvider TestProvider
        {
            get
            {
                return s_ccpTestProvider ??
                       (s_ccpTestProvider = new APIProvider { Url = new Uri(NetworkConstants.APITestBase), Name = "CCP Test API" });
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Returns the request method.
        /// </summary>
        /// <param name="requestMethod">An APIMethods enumeration member specifying the method for which the URL is required.</param>
        public APIMethod GetMethod(Enum requestMethod)
        {
            foreach (APIMethod method in m_methods.Where(method => method.Method.Equals(requestMethod)))
            {
                return method;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the full canonical URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethods enumeration member specifying the method for which the URL is required.</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        public Uri GetMethodUrl(Enum requestMethod)
        {
            // Gets the proper data
            Uri url = Url;
            string path = GetMethod(requestMethod).Path;
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(url.AbsoluteUri))
            {
                url = s_ccpProvider.Url;
                path = s_ccpProvider.GetMethod(requestMethod).Path;
            }

            // Build the uri
            Uri baseUri = url;
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = uriBuilder.Path.TrimEnd("/".ToCharArray()) + path;
            return uriBuilder.Uri;
        }

        #endregion


        #region Queries



        /// <summary>
        /// Query a method without arguments.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryMethodAsync<T>(Enum method, QueryCallback<T> callback)
        {
            QueryMethodAsync(method, callback, null, RowsetsTransform);
        }

        /// <summary>
        /// Query a method with the provided arguments for an API key.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="keyId">The API key's ID</param>
        /// <param name="verificationCode">The API key's verification code</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryMethodAsync<T>(Enum method, long keyId, string verificationCode, QueryCallback<T> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                            keyId, verificationCode);
            QueryMethodAsync(method, callback, postData, RowsetsTransform);
        }

        /// <summary>
        /// Query a method with the provided arguments for a character.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="keyId">The API key's ID</param>
        /// <param name="verificationCode">The API key's verification code</param>
        /// <param name="id">The character or corporation ID.</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryMethodAsync<T>(Enum method, long keyId, string verificationCode, long id, QueryCallback<T> callback)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            string postData = GetPostDataString(method, keyId, verificationCode, id);

            QueryMethodAsync(method, callback, postData, RowsetsTransform);
        }

        private static string GetPostDataString(Enum method, long id, string verificationCode, long characterID)
        {
            if (method.Equals(APICharacterMethods.CharacterInfo) && id == 0 && string.IsNullOrEmpty(verificationCode))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCharacterIDOnly, characterID);
            }

            if (method.Equals(APICorporationMethods.CorporationSheet) && id == 0 && string.IsNullOrEmpty(verificationCode))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCorporationIDOnly, characterID);
            }

            if (method.Equals(APICharacterMethods.WalletJournal) || method.Equals(APICharacterMethods.WalletTransactions))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndRowCount,
                                     id, verificationCode, characterID, 2560);
            }

            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                 id, verificationCode, characterID);
        }

        /// <summary>
        /// Query a method with the provided arguments for a character messages and contracts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="keyId">The API key's ID</param>
        /// <param name="verificationCode">The API key's verification code</param>
        /// <param name="id">The character ID.</param>
        /// <param name="messageID">The message ID.</param>
        /// <param name="callback">The callback.</param>
        public void QueryMethodAsync<T>(Enum method, long keyId, string verificationCode, long id, long messageID,
                                        QueryCallback<T> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture, GetPostDataFormat(method),
                                            keyId, verificationCode, id, messageID);
            QueryMethodAsync(method, callback, postData, RowsetsTransform);
        }

        /// <summary>
        /// Query a method with the provided arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="ids">The ids.</param>
        /// <param name="callback">The callback.</param>
        public void QueryMethodAsync<T>(Enum method, string ids, QueryCallback<T> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataIDsOnly,
                                            ids);
            QueryMethodAsync(method, callback, postData, RowsetsTransform);
        }

        #endregion


        #region Querying helpers

        /// <summary>
        /// Query this method with the provided HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="APIResult&lt;T&gt;"/>).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        /// <returns>The deserialized object</returns>
        private APIResult<T> QueryMethod<T>(Enum method, string postData, XslCompiledTransform transform)
        {
            // Download
            Uri url = GetMethodUrl(method);
            APIResult<T> result = Util.DownloadAPIResult<T>(url, SupportsCompressedResponse, postData, transform);

            // On failure with a custom method, fallback to CCP
            return ShouldRetryWithCCP(result) ? s_ccpProvider.QueryMethod<T>(method, postData, transform) : result;
        }

        /// <summary>
        /// Asynchrnoneously queries this method with the provided HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="APIResult&lt;T&gt;"/>).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        private void QueryMethodAsync<T>(Enum method, QueryCallback<T> callback, string postData, XslCompiledTransform transform)
        {
            // Check callback not null
            if (callback == null)
                throw new ArgumentNullException("callback", "The callback cannot be null.");

            // Lazy download
            Uri url = GetMethodUrl(method);
            Util.DownloadAPIResultAsync<T>(
                url,
                result =>
                    {
                        // On failure with a custom provider, fallback to CCP
                        if (ShouldRetryWithCCP(result))
                        {
                            APIProvider ccpProvider = EveMonClient.APIProviders.CurrentProvider.Url.Host != TestProvider.Url.Host
                                                          ? s_ccpProvider
                                                          : s_ccpTestProvider;
                            ccpProvider.QueryMethodAsync(method, callback, postData, transform);
                            return;
                        }

                        // Invokes the callback
                        callback(result);
                    },
                    SupportsCompressedResponse, postData, transform);
        }

        /// <summary>
        /// Checks whether the query must be retrieved with CCP as the default provider.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ShouldRetryWithCCP(IAPIResult result)
        {
            return (s_ccpProvider != this && s_ccpTestProvider != this && result.HasError &&
                    result.ErrorType != APIError.CCP);
        }

        /// <summary>
        /// Gets the post data format.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        private static string GetPostDataFormat(Enum method)
        {
            return (method.GetType() == typeof(APICharacterMethods))
                       ? NetworkConstants.PostDataWithCharIDAndIDS
                       : NetworkConstants.PostDataWithCharIDAndContractID;
        }

        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by <see cref="XmlSerializer"/>
        /// </summary>
        internal static XslCompiledTransform RowsetsTransform
        {
            get { return s_rowsetsTransform ?? (s_rowsetsTransform = Util.LoadXslt(Properties.Resources.RowsetsXSLT)); }
        }

        #endregion


        /// <summary>
        /// Returns the configuration name as a String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}