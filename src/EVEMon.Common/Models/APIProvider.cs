using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Xsl;
using EVEMon.Common.Attributes;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Threading;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{

    /// <summary>
    /// Serializable class abstracting an API queries provider and its configuration.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class APIProvider
    {
        // Called when an ESI request completes
        public delegate void ESIRequestCallback<T>(EsiResult<T> result, object state);

        private static APIProvider s_ccpProvider;
        private static APIProvider s_ccpTestProvider;
        private static XslCompiledTransform s_rowsetsTransform;

        private readonly List<APIMethod> m_methods;
        private readonly List<ESIMethod> m_methodsNew;
        private bool m_supportsCompressedResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal APIProvider()
        {
            m_methods = new List<APIMethod>(APIMethod.CreateDefaultSet());
            m_methodsNew = new List<ESIMethod>(ESIMethod.CreateDefaultSet());
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
        /// Returns a list of APIMethodsEnum supported by this APIConfiguration.
        /// </summary>
        public IEnumerable<APIMethod> Methods => m_methods;

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
        public bool IsDefault => this == DefaultProvider;

        /// <summary>
        /// Gets the default API provider
        /// </summary>
        public static APIProvider DefaultProvider
            => s_ccpProvider ??
               (s_ccpProvider = new APIProvider
               {
                   Url = new Uri(NetworkConstants.APIBase),
                   Name = "CCP"
               });

        /// <summary>
        /// Gets the test API provider
        /// </summary>
        public static APIProvider TestProvider
            => s_ccpTestProvider ??
               (s_ccpTestProvider = new APIProvider
               {
                   Url = new Uri(NetworkConstants.APITestBase),
                   Name = "CCP Test API"
               });

        #endregion


        #region Helpers

        /// <summary>
        /// Returns the request method.
        /// </summary>
        /// <param name="requestMethod">An APIMethodsEnum enumeration member specifying the method for which the URL is required.</param>
        public APIMethod GetMethod(Enum requestMethod)
        {
            var apiMethod = m_methods.FirstOrDefault(method => method.Method.Equals(requestMethod));
            if (apiMethod == null)
                throw new InvalidOperationException("No API method found for " + requestMethod);
            return apiMethod;
        }

        /// <summary>
        /// Returns the request method.
        /// </summary>
        /// <param name="requestMethod">An ESIMethodsEnum enumeration member specifying the method for which the URL is required.</param>
        private ESIMethod GetEsiMethod(Enum requestMethod)
        {
            var esiMethod = m_methodsNew.FirstOrDefault(method => method.Method.Equals(requestMethod));
            if (esiMethod == null)
                throw new InvalidOperationException("No ESI method found for " + requestMethod);
            return esiMethod;
        }

        /// <summary>
        /// Returns the full canonical ESI URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethodsEnum enumeration member specifying the method for which the URL is required.</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        private Uri GetESIUrl(Enum requestMethod)
        {
            string path = GetEsiMethod(requestMethod).Path;

            // Build the uri
            Uri baseUri = new Uri(NetworkConstants.ESIBase);
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = Path.Combine(uriBuilder.Path, path);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Returns the full canonical ESI URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethodsEnum enumeration member specifying the method for which the URL is required.</param>
        /// <param name="param">The parameter for this URL</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        private Uri GetESIUrl(Enum requestMethod, long param)
        {
            string path = string.Format(GetEsiMethod(requestMethod).Path, param);

            // Build the uri
            Uri baseUri = new Uri(NetworkConstants.ESIBase);
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = Path.Combine(uriBuilder.Path, path);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Returns the full canonical URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethodsEnum enumeration member specifying the method for which the URL is required.</param>
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
            uriBuilder.Path = Path.Combine(uriBuilder.Path, path);
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
        public void QueryMethodAsync<T>(Enum method, Action<CCPAPIResult<T>> callback)
        {
            QueryMethodAsync(method, callback, null, RowsetsTransform);
        }

        /// <summary>
        /// Query a public ESI method without arguments.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        public void QueryEsiAsync<T>(Enum method, ESIRequestCallback<T> callback, object state = null)
            where T : class
        {
            QueryEsiAsync(method, callback, 0L, null, state);
        }

        /// <summary>
        /// Query a public ESI method with an ID argument.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="id">The ID to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        public void QueryEsiAsync<T>(Enum method, long id, ESIRequestCallback<T> callback, object state = null)
            where T : class
        {
            QueryEsiAsync(method, callback, id, null, state);
        }

        /// <summary>
        /// Query a public ESI method with POST arguments.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryEsiAsync<T>(Enum method, string postData, ESIRequestCallback<T> callback, object state = null)
            where T : class
        {
            QueryEsiAsync(method, callback, 0L, postData, state);
        }

        /// <summary>
        /// Query a public ESI method with an ID argument.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="id">The ID to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        public void QueryEsiAsync<T>(Enum method, string token, long id, ESIRequestCallback<T> callback, object state = null)
            where T : class
        {
            QueryEsiAsync(method, callback, id, null, state);
        }

        /// <summary>
        /// Query a method with the provided arguments for an API key.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="keyId">The API key's ID</param>
        /// <param name="verificationCode">The API key's verification code</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryMethodAsync<T>(Enum method, long keyId, string verificationCode, Action<CCPAPIResult<T>> callback)
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
        /// <exception cref="System.ArgumentNullException">method</exception>
        public void QueryMethodAsync<T>(Enum method, long keyId, string verificationCode, long id,
            Action<CCPAPIResult<T>> callback)
        {
            method.ThrowIfNull(nameof(method));

            string postData = GetPostDataString(method, keyId, verificationCode, id);

            QueryMethodAsync(method, callback, postData, RowsetsTransform);
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
            Action<CCPAPIResult<T>> callback)
        {
            string postData = String.Format(CultureConstants.InvariantCulture, GetPostDataFormat(method),
                keyId, verificationCode, id, messageID);
            QueryMethodAsync(method, callback, postData, RowsetsTransform);
        }
        
        #endregion


        #region Querying helpers

        /// <summary>
        /// Asynchronously queries this method with the provided ID and HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="CCPAPIResult{T}" />).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="param">The parameter to use, can be anything if method does not requre a parameter</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        /// <exception cref="System.ArgumentNullException">callback; The callback cannot be null.</exception>
        private void QueryEsiAsync<T>(Enum method, ESIRequestCallback<T> callback, long param,
            string postData, object state) where T : class
        {
            // Check callback not null
            callback.ThrowIfNull(nameof(callback), "The callback cannot be null.");

            // Lazy download
            Uri url = GetESIUrl(method, param);

            Util.DownloadEsiResultAsync<T>(url, null, SupportsCompressedResponse, postData)
                .ContinueWith(task =>
                {
                    // Invokes the callback
                    Dispatcher.Invoke(() => callback.Invoke(task.Result, state));
                });
        }

        /// <summary>
        /// Asynchronously queries this method with the provided ID and HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="CCPAPIResult{T}" />).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="param">The parameter to use, can be anything if method does not requre a parameter</param>
        /// <param name="token">The ESI token</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        /// <exception cref="System.ArgumentNullException">callback; The callback cannot be null.</exception>
        private void QueryEsiAsync<T>(Enum method, ESIRequestCallback<T> callback, long param,
            string token, string postData, object state) where T : class
        {
            // Check callback not null
            callback.ThrowIfNull(nameof(callback), "The callback cannot be null.");

            // Lazy download
            Uri url = GetESIUrl(method, param);

            Util.DownloadEsiResultAsync<T>(url, token, SupportsCompressedResponse, postData)
                .ContinueWith(task =>
                {
                    // Invokes the callback
                    Dispatcher.Invoke(() => callback.Invoke(task.Result, state));
                });
        }

        /// <summary>
        /// Asynchrnoneously queries this method with the provided HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="CCPAPIResult{T}" />).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        /// <exception cref="System.ArgumentNullException">callback; The callback cannot be null.</exception>
        private void QueryMethodAsync<T>(Enum method, Action<CCPAPIResult<T>> callback, string postData,
            XslCompiledTransform transform)
        {
            // Check callback not null
            callback.ThrowIfNull(nameof(callback), "The callback cannot be null.");

            // Lazy download
            Uri url = GetMethodUrl(method);

            Util.DownloadAPIResultAsync<T>(url, SupportsCompressedResponse, postData, transform)
                .ContinueWith(task =>
                {
                    // Invokes the callback
                    Dispatcher.Invoke(() => callback.Invoke(task.Result));
                });
        }
        
        /// <summary>
        /// Gets the post data string.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="keyId">The key identifier.</param>
        /// <param name="verificationCode">The verification code.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private static string GetPostDataString(Enum method, long keyId, string verificationCode, long id)
        {
            if (method.Equals(CCPAPICharacterMethods.CharacterInfo) && keyId == 0 && string.IsNullOrEmpty(verificationCode))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCharacterIDOnly, id);
            }

            if (method.Equals(CCPAPICorporationMethods.CorporationSheet) && keyId == 0 && string.IsNullOrEmpty(verificationCode))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCorporationIDOnly, id);
            }

            if (method.Equals(CCPAPICharacterMethods.WalletJournal) || method.Equals(CCPAPICharacterMethods.WalletTransactions))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndRowCount,
                    keyId, verificationCode, id, 2560);
            }

            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                keyId, verificationCode, id);
        }

        /// <summary>
        /// Gets the post data format.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        private static string GetPostDataFormat(Enum method)
        {
            if (method.GetType() == typeof(CCPAPIGenericMethods))
            {
                if (method.ToString().Contains("Contract"))
                    return NetworkConstants.PostDataWithCharIDAndContractID;

                if (method.ToString().Contains("Planetary"))
                    return NetworkConstants.PostDataWithCharIDAndPlanetID;
            }

            return NetworkConstants.PostDataWithCharIDAndIDS;
        }

        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by <see cref="System.Xml.Serialization.XmlSerializer"/>
        /// </summary>
        internal static XslCompiledTransform RowsetsTransform
            => s_rowsetsTransform ?? (s_rowsetsTransform = Util.LoadXslt(Properties.Resources.RowsetsXSLT));

        #endregion


        /// <summary>
        /// Returns the configuration name as a String.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;
    }
}
