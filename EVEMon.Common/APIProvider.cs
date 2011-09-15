using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using EVEMon.Common.Attributes;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
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

        /// <summary>
        /// Default constructor 
        /// </summary>
        internal APIProvider()
        {
            m_methods = new List<APIMethod>(APIMethod.CreateDefaultSet());
            Url = "http://your-custom-API-provider.com";
            Name = "your provider's name";
        }


        #region Configuration

        /// <summary>
        /// Returns the name of this APIConfiguration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the server host for this APIConfiguration.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Returns a list of APIMethods supported by this APIConfiguration.
        /// </summary>
        public IEnumerable<APIMethod> Methods
        {
            get { return m_methods.AsReadOnly(); }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Returns true if this APIConfiguration represents the default API service.
        /// </summary>
        public bool IsDefault
        {
            get { return (this == DefaultProvider); }
        }

        /// <summary>
        /// Returns the request method.
        /// </summary>
        /// <param name="requestMethod">An APIMethods enumeration member specfying the method for which the URL is required.</param>
        public APIMethod GetMethod(APIMethods requestMethod)
        {
            foreach (APIMethod method in m_methods.Where(method => method.Method == requestMethod))
            {
                return method;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the full canonical URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethods enumeration member specfying the method for which the URL is required.</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        private string GetMethodUrl(APIMethods requestMethod)
        {
            // Gets the proper data
            string url = Url;
            string path = GetMethod(requestMethod).Path;
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(url))
            {
                url = s_ccpProvider.Url;
                path = s_ccpProvider.GetMethod(requestMethod).Path;
            }

            // Build the uri
            Uri baseUri = new Uri(url);
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = uriBuilder.Path.TrimEnd("/".ToCharArray()) + path;
            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// Gets the default API provider
        /// </summary>
        public static APIProvider DefaultProvider
        {
            get
            {
                if (s_ccpProvider != null)
                    return s_ccpProvider;

                s_ccpProvider = new APIProvider { Url = NetworkConstants.APIBase, Name = "CCP" };

                return s_ccpProvider;
            }
        }

        /// <summary>
        /// Gets the test API provider
        /// </summary>
        public static APIProvider TestProvider
        {
            get
            {
                if (s_ccpTestProvider != null)
                    return s_ccpTestProvider;

                s_ccpTestProvider = new APIProvider { Url = NetworkConstants.APITestBase, Name = "CCP Test API" };

                return s_ccpTestProvider;
            }
        }

        #endregion


        #region Queries

        /// <summary>
        /// Query the info for an API key.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verificationCode"></param>
        /// <returns></returns>
        public APIResult<SerializableAPIKeyInfo> QueryAPIKeyInfo(long id, string verificationCode)
        {
            HttpPostData postData = new HttpPostData(String.Format(NetworkConstants.PostDataBase, id, verificationCode));
            return QueryMethod<SerializableAPIKeyInfo>(APIMethods.APIKeyInfo, postData, RowsetsTransform);
        }

        /// <summary>
        /// Query the conquerable station list.
        /// </summary>
        public APIResult<SerializableAPIConquerableStationList> QueryConquerableStationList()
        {
            return QueryMethod<SerializableAPIConquerableStationList>(APIMethods.ConquerableStationList, null,
                                                                      RowsetsTransform);
        }

        /// <summary>
        /// Query the character name from the provided list of IDs.
        /// </summary>
        /// <param name="ids">The Ids.</param>
        /// <returns></returns>
        public APIResult<SerializableAPICharacterName> QueryCharacterName(string ids)
        {
            HttpPostData postData = new HttpPostData(String.Format(NetworkConstants.PostDataIDsOnly, ids));
            return QueryMethod<SerializableAPICharacterName>(APIMethods.CharacterName, postData, RowsetsTransform);
        }

        /// <summary>
        /// Query a method without arguments.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryMethodAsync<T>(APIMethods method, QueryCallback<T> callback)
        {
            QueryMethodAsync(method, null, RowsetsTransform, callback);
        }

        /// <summary>
        /// Query a method with the provided arguments for an account.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryMethodAsync<T>(APIMethods method, long userID, string apiKey, QueryCallback<T> callback)
        {
            HttpPostData postData = new HttpPostData(String.Format(NetworkConstants.PostDataBase, userID, apiKey));
            QueryMethodAsync(method, postData, RowsetsTransform, callback);
        }

        /// <summary>
        /// Query a method with the provided arguments for a character.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="charID"></param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        public void QueryMethodAsync<T>(APIMethods method, long userID, string apiKey, long charID,
                                        QueryCallback<T> callback)
        {
            HttpPostData postData = new HttpPostData(String.Format(
                NetworkConstants.PostDataWithCharID, userID, apiKey, charID));
            QueryMethodAsync(method, postData, RowsetsTransform, callback);
        }

        /// <summary>
        /// Query a method with the provided arguments for a character messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="userID">The account's ID</param>
        /// <param name="apiKey">The account's API key</param>
        /// <param name="characterID">The character ID.</param>
        /// <param name="messageID">The message ID.</param>
        /// <param name="callback">The callback.</param>
        public void QueryMethodAsync<T>(APIMethods method, long userID, string apiKey, long characterID, long messageID,
                                        QueryCallback<T> callback)
        {
            HttpPostData postData = new HttpPostData(
                String.Format(NetworkConstants.PostDataWithCharIDAndIDS, userID, apiKey, characterID, messageID));
            QueryMethodAsync(method, postData, RowsetsTransform, callback);
        }

        #endregion


        #region Querying helpers

        /// <summary>
        /// Query this method with the provided HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="APIResult{T}"/>).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        /// <returns>The deserialized object</returns>
        private APIResult<T> QueryMethod<T>(APIMethods method, HttpPostData postData, XslCompiledTransform transform)
        {
            // Download
            string url = GetMethodUrl(method);
            APIResult<T> result = Util.DownloadAPIResult<T>(url, postData, transform);

            // On failure with a custom method, fallback to CCP
            if (ShouldRetryWithCCP(result))
                return s_ccpProvider.QueryMethod<T>(method, postData, transform);

            // If the result is a character sheet, we store the result
            if (method == APIMethods.CharacterSheet && !result.HasError)
            {
                SerializableAPICharacterSheet sheet = (SerializableAPICharacterSheet)(Object)result.Result;
                LocalXmlCache.Save(sheet.Name, result.XmlDocument);
            }

            // If the result is a conquerable station list, we store the result
            if (method == APIMethods.ConquerableStationList && !result.HasError)
                LocalXmlCache.Save(method.ToString(), result.XmlDocument);

            // Returns
            return result;
        }

        /// <summary>
        /// Asynchrnoneously queries this method with the provided HTTP POST data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="APIResult{T}"/>).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="callback">The callback to invoke once the query has been completed.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        private void QueryMethodAsync<T>(APIMethods method, HttpPostData postData, XslCompiledTransform transform,
                                         QueryCallback<T> callback)
        {
            // Check callback not null
            if (callback == null)
                throw new ArgumentNullException("callback", "The callback cannot be null.");

            // Lazy download
            string url = GetMethodUrl(method);
            Util.DownloadAPIResultAsync<T>(
                url, postData, transform,
                result =>
                    {
                        // On failure with a custom method, fallback to CCP
                        if (ShouldRetryWithCCP(result))
                            result = s_ccpProvider.QueryMethod<T>(method, postData, transform);

                        // If the result is a character sheet, we store the result
                        if (method == APIMethods.CharacterSheet && !result.HasError)
                        {
                            SerializableAPICharacterSheet sheet = (SerializableAPICharacterSheet)(Object)result.Result;
                            LocalXmlCache.Save(sheet.Name, result.XmlDocument);
                        }

                        // If the result is a conquerable station list, we store the result
                        if (method == APIMethods.ConquerableStationList && !result.HasError)
                            LocalXmlCache.Save(method.ToString(), result.XmlDocument);

                        // Invokes the callback
                        callback(result);
                    });
        }

        /// <summary>
        /// Checks whether the query must be retrieved with CCP as the default provider.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ShouldRetryWithCCP(IAPIResult result)
        {
            return (s_ccpProvider != this && s_ccpTestProvider != this && result.HasError &&
                    result.ErrorType != APIEnumerations.APIErrors.CCP);
        }

        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by <see cref="XmlSerializer"/>
        /// </summary>
        internal static XslCompiledTransform RowsetsTransform
        {
            get { return s_rowsetsTransform ?? (s_rowsetsTransform = Util.LoadXSLT(Properties.Resources.RowsetsXSLT)); }
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