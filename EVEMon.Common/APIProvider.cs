using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;

using EVEMon.Common.Net;
using EVEMon.Common.Serialization;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.API;
using System.Threading;

namespace EVEMon.Common
{
    /// <summary>
    /// A delegate for query callbacks.
    /// </summary>
    /// <param name="document"></param>
    public delegate void QueryCallback<T>(APIResult<T> result);


    /// <summary>
    /// Serializable class abstracting an API queries provider and its configuration.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class APIProvider
    {
        private static APIProvider s_ccpProvider;
        private static XslCompiledTransform m_rowsetsTransform;

        private List<APIMethod> m_methods;
        private string m_url;
        private string m_name;

        /// <summary>
        /// Default constructor 
        /// </summary>
        internal APIProvider()
        {
            m_methods = new List<APIMethod>(APIMethod.CreateDefaultSet());
            m_url = "http://your-custom-API-provider.com";
            m_name = "your provider's name";
        }

        #region Configuration
        /// <summary>
        /// Returns the name of this APIConfiguration.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Returns the server host for this APIConfiguration.
        /// </summary>
        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }

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
            get
            {
                return (this == DefaultProvider);
            }
        }

        /// <summary>
        /// Returns the request method
        /// </summary>
        /// <param name="requestMethod">An APIMethods enumeration member specfying the method for which the URL is required.</param>
        public APIMethod GetMethod(APIMethods requestMethod)
        {
            foreach (APIMethod method in m_methods)
            {
                if (method.Method == requestMethod)
                    return method;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the full canonical URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethods enumeration member specfying the method for which the URL is required.</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        public string GetMethodUrl(APIMethods requestMethod)
        {
            // Gets the proper data
            var url = m_url;
            var path = GetMethod(requestMethod).Path;
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(url))
            {
                url = s_ccpProvider.Url;
                path = s_ccpProvider.GetMethod(requestMethod).Path;
            }

            // Build the uri
            var baseUri = new Uri(url);
            var uriBuilder = new UriBuilder(baseUri);
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
                if (s_ccpProvider == null)
                {
                    s_ccpProvider = new APIProvider();
                    s_ccpProvider.Url = NetworkConstants.APIBase;
                    s_ccpProvider.Name = "CCP";
                }

                return s_ccpProvider;
            }
        }
        #endregion


        #region Queries
        /// <summary>
        /// Query the account balance for a character. Requires full api key. Used to test whether a key is a full or limited one.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="charID"></param>
        /// <returns></returns>
        public APIResult<SerializableAccountBalanceList> QueryCharacterAccountBalance(long userID, string apiKey, long charID)
        {
            HttpPostData postData = new HttpPostData("userID=" + userID + "&apiKey=" + apiKey + "&characterID=" + charID.ToString());
            return QueryMethod<SerializableAccountBalanceList>(APIMethods.CharacterAccountBalance, postData, RowsetsTransform);
        }

        /// <summary>
        /// Query the characters skill in training.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="charID"></param>
        /// <returns></returns>
        public APIResult<SerializableSkillInTraining> QuerySkillInTraining(long userID, string apiKey, long charID)
        {
            HttpPostData postData = new HttpPostData("userID=" + userID + "&apiKey=" + apiKey + "&characterID=" + charID.ToString());
            return QueryMethod<SerializableSkillInTraining>(APIMethods.CharacterSkillInTraining, postData, RowsetsTransform);
        }

        /// <summary>
        /// Query the characters list for the provided account.
        /// </summary>
        /// <param name="userID">The account's ID</param>
        /// <param name="apiKey">The account's API key</param>
        public APIResult<SerializableCharacterList> QueryCharactersList(long userID, string apiKey)
        {
            HttpPostData postData = new HttpPostData("userID=" + userID.ToString() + "&apiKey=" + apiKey);
            return QueryMethod<SerializableCharacterList>(APIMethods.CharacterList, postData, RowsetsTransform);
        }

        /// <summary>
        /// Query the conquerable station list.
        /// </summary>
        public APIResult<SerializableConquerableStationList> QueryConquerableStationList()
        {
            return QueryMethod<SerializableConquerableStationList>(APIMethods.ConquerableStationList, null, RowsetsTransform);
        }

        /// <summary>
        /// Query a method without arguments.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="callback">The callback to invoke once the query has been completed. It will be done on the data actor (see <see cref="DataObject.CommonActor"/>).</param>
        public void QueryMethodAsync<T>(APIMethods method, QueryCallback<T> callback)
        {
            QueryMethodAsync<T>(method, null, RowsetsTransform, callback);
        }

        /// <summary>
        /// Query a method with the provided arguments for an account.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="callback">The callback to invoke once the query has been completed. It will be done on the data actor (see <see cref="DataObject.CommonActor"/>).</param>
        public void QueryMethodAsync<T>(APIMethods method, long userID, string apiKey, QueryCallback<T> callback)
        {
            HttpPostData postData = new HttpPostData("userID=" + userID.ToString() + "&apiKey=" + apiKey);
            QueryMethodAsync<T>(method, postData, RowsetsTransform, callback);
        }

        /// <summary>
        /// Query a method with the provided arguments for a character.
        /// </summary>
        /// <typeparam name="T">The type of the deserialization object.</typeparam>
        /// <param name="method"></param>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="charID"></param>
        /// <param name="callback">The callback to invoke once the query has been completed. It will be done on the data actor (see <see cref="DataObject.CommonActor"/>).</param>
        public void QueryMethodAsync<T>(APIMethods method, long userID, string apiKey, long charID, QueryCallback<T> callback)
        {
            HttpPostData postData = new HttpPostData("userID=" + userID + "&apiKey=" + apiKey + "&characterID=" + charID.ToString());
            QueryMethodAsync<T>(method, postData, RowsetsTransform, callback);
        }
        #endregion


        #region Querying helpers
        /// <summary>
        /// Query this method with the provided HTTP POST data
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
            var result = Util.DownloadAPIResult<T>(url, postData, transform);

            // On failure with a custom method, fallback to CCP
            if (ShouldRetryWithCCP(result))
                return s_ccpProvider.QueryMethod<T>(method, postData, transform);

            // If the result is a character sheet, we store the result
            if (method == APIMethods.CharacterSheet && !result.HasError)
            {
                SerializableAPICharacter sheet = (SerializableAPICharacter)(Object)result.Result;
                LocalXmlCache.Save(sheet.Name, result.XmlDocument);
            }

            // If the result is a conquerable station list, we store the result
            if (method == APIMethods.ConquerableStationList && !result.HasError)
                LocalXmlCache.Save(method.ToString(), result.XmlDocument);

            // Returns
            return result;
        }

        /// <summary>
        /// Asynchrnoneously queries this method with the provided HTTP POST data
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being <see cref="APIResult{T}"/>).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="postData">The http POST data</param>
        /// <param name="callback">The callback to invoke once the query has been completed. It will be done on the data actor (see <see cref="DataObject.CommonActor"/>).</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        private void QueryMethodAsync<T>(APIMethods method, HttpPostData postData, XslCompiledTransform transform, QueryCallback<T> callback)
        {
            // Check callback not null
            if (callback == null)
                throw new ArgumentNullException("The callback cannot be null.", "callback");

            // Lazy download
            string url = GetMethodUrl(method);
            Util.DownloadAPIResultAsync<T>(url, postData, transform, (result) =>
            {
                // On failure with a custom method, fallback to CCP
                if (ShouldRetryWithCCP(result))
                    result = s_ccpProvider.QueryMethod<T>(method, postData, transform);

                // If the result is a character sheet, we store the result
                if (method == APIMethods.CharacterSheet && !result.HasError)
                {
                    SerializableAPICharacter sheet = (SerializableAPICharacter)(Object)result.Result;
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
            return (s_ccpProvider != this && result.HasError && result.ErrorType != APIErrors.CCP);
        }

        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by <see cref="XMLDeserializer"/>
        /// </summary>
        internal static XslCompiledTransform RowsetsTransform
        {
            get
            {
                if (m_rowsetsTransform == null)
                    m_rowsetsTransform = Util.LoadXSLT(EVEMon.Common.Properties.Resources.RowsetsXSLT);

                return m_rowsetsTransform;
            }
        }
        #endregion

        /// <summary>
        /// Returns the configuration name as a String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }
    }
}
