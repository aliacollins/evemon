using EVEMon.Common.Attributes;
using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Xsl;

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

        private readonly List<ESIMethod> m_methods;
        private bool m_supportsCompressedResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal APIProvider()
        {
            m_methods = new List<ESIMethod>(ESIMethod.CreateDefaultSet());
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
                   Url = new Uri(NetworkConstants.ESIBase),
                   Name = "CCP"
               });

        /// <summary>
        /// Gets the test API provider
        /// </summary>
        public static APIProvider TestProvider
            => s_ccpTestProvider ??
               (s_ccpTestProvider = new APIProvider
               {
                   Url = new Uri(NetworkConstants.ESITestBase),
                   Name = "CCP Test API"
               });

        #endregion


        #region Helpers
        
        /// <summary>
        /// Returns the request method.
        /// </summary>
        /// <param name="requestMethod">An ESIMethodsEnum enumeration member specifying the method for which the URL is required.</param>
        private ESIMethod GetESIMethod(Enum requestMethod)
        {
            var esiMethod = m_methods.FirstOrDefault(method => method.Method.Equals(requestMethod));
            if (esiMethod == null)
                throw new InvalidOperationException("No ESI method found for " + requestMethod);
            return esiMethod;
        }

        /// <summary>
        /// Returns the full canonical ESI URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethodsEnum enumeration member specifying the method for which the URL is required.</param>
        /// <param name="getID1">The first numeric parameter for this URL, if needed.</param>
        /// <param name="getID2">The second numeric parameter for this URL, if needed.</param>
        /// <param name="getStr">The second string parameter for this URL, if needed. Overrides
        /// the numeric parameter if not null or empty.</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        private Uri GetESIUrl(Enum requestMethod, long getID1, long getID2, string getStr)
        {
            string path;
            if (string.IsNullOrEmpty(getStr))
                path = string.Format(GetESIMethod(requestMethod).Path, getID1, getID2);
            else
                path = string.Format(GetESIMethod(requestMethod).Path, getID1, getStr);
            // Build the URI
            UriBuilder uriBuilder = new UriBuilder(NetworkConstants.ESIBase);
            uriBuilder.Path = Path.Combine(uriBuilder.Path, path);
            return uriBuilder.Uri;
        }

        #endregion

        
        #region Querying
        
        /// <summary>
        /// Asynchronously queries this method with the provided ID and HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being
        /// <see cref="CCPAPIResult{T}" />).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.
        /// </param>
        /// <param name="data">The parameters to use for the request, including the token,
        /// arguments, and POST data.</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        /// <exception cref="System.ArgumentNullException">callback; The callback cannot be
        /// null.</exception>
        public void QueryEsi<T>(Enum method, ESIRequestCallback<T> callback, ESIParams
            data, object state = null) where T : class
        {
            var response = data.LastResponse;
            // Check callback not null
            callback.ThrowIfNull(nameof(callback), "The callback cannot be null.");
            Uri url = GetESIUrl(method, data.ParamOne, data.ParamTwo, data.GetData);
            Util.DownloadJsonAsync<T>(url, new RequestParams(data.PostData)
            {
                Authentication = data.Token,
                AcceptEncoded = SupportsCompressedResponse,
                ETag = response?.ETag,
                IfModifiedSince = response?.Expires
            }).ContinueWith(task =>
            {
                var esiResult = new EsiResult<T>(task.Result);
                // Sync clock on the answer if necessary and provided
                var sync = esiResult.Result as ISynchronizableWithLocalClock;
                DateTime? when = esiResult.CurrentTime;
                if (sync != null && when != null)
                    sync.SynchronizeWithLocalClock(DateTime.UtcNow - (DateTime)when);
                // Invokes the callback
                Dispatcher.Invoke(() => callback.Invoke(esiResult, state));
            });
        }
        
        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by <see cref="System.Xml.Serialization.XmlSerializer"/>
        /// </summary>
        internal static XslCompiledTransform RowsetsTransform => s_rowsetsTransform ??
            (s_rowsetsTransform = Util.LoadXslt(Properties.Resources.RowsetsXSLT));

        #endregion


        /// <summary>
        /// Returns the configuration name as a String.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

    }

    #region Helper classes

    /// <summary>
    /// Simplifies ESI request building by allowing parameters to be flexibly included.
    /// </summary>
    public struct ESIParams
    {
        /// <summary>
        /// The first parameter, usually used for the ID of the target object in public
        /// requests and for the character/corporation ID in private requests.
        /// </summary>
        public long ParamOne;
        /// <summary>
        /// The second parameter, usually used for the ID of the target object in private
        /// requests.
        /// </summary>
        public long ParamTwo;
        /// <summary>
        /// The GET data to be passed in as a string.
        /// </summary>
        public string GetData;
        /// <summary>
        /// The POST data to be passed in as a string.
        /// </summary>
        public string PostData;
        /// <summary>
        /// The last response from the server.
        /// </summary>
        public ResponseParams LastResponse;
        /// <summary>
        /// The token to use for authentication.
        /// </summary>
        public string Token;

        public ESIParams(ResponseParams lastResponse, string token = null)
        {
            ParamOne = 0L;
            ParamTwo = 0L;
            GetData = null;
            LastResponse = lastResponse ?? new ResponseParams(0);
            PostData = null;
            Token = token;
        }
    }

    #endregion

}
