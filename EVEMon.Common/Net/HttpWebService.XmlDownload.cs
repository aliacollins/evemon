using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Net
{
    public delegate void DownloadXmlCompletedCallback(DownloadXmlAsyncResult e, object userState);

    /// <summary>
    /// HttpWebService Xml download implementation.
    /// </summary>
    static partial class HttpWebService
    {
        private const string XMLAccept =
            "text/xml,application/xml,application/xhtml+xml;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Downloads an Xml document from the specified url using the specified POST data.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        /// <returns></returns>
        public static IXPathNavigable DownloadXml(Uri url, HttpMethod method = HttpMethod.Get, bool acceptEncoded = false,
                                                  string postdata = null, DataCompression dataCompression = DataCompression.None)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, dataCompression);
            HttpWebServiceRequest request = GetRequest();
            try
            {
                MemoryStream responseStream = Util.GetMemoryStream();
                request.GetResponse(url, method, postData, dataCompression, responseStream, acceptEncoded, XMLAccept);
                return GetXmlDocument(request);
            }
            finally
            {
                if (request.ResponseStream != null)
                    request.ResponseStream.Close();
            }
        }

        /// <summary>
        /// Asynchronously downloads an xml file from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="callback">A <see cref="DownloadXmlCompletedCallback"/> to be invoked when the request is completed</param>
        /// <param name="userState">A state object to be returned to the callback</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        public static void DownloadXmlAsync(Uri url, DownloadXmlCompletedCallback callback, object userState,
                                            HttpMethod method = HttpMethod.Get, bool acceptEncoded = false, string postdata = null,
                                            DataCompression dataCompression = DataCompression.None)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            XmlRequestAsyncState state = new XmlRequestAsyncState(callback, DownloadXmlAsyncCompleted, userState);
            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, dataCompression);
            HttpWebServiceRequest request = GetRequest();
            MemoryStream responseStream = Util.GetMemoryStream();
            request.GetResponseAsync(url, method, postData, dataCompression, responseStream, acceptEncoded, XMLAccept, state);
        }

        /// <summary>
        /// Callback method for asynchronous requests.
        /// </summary>
        private static void DownloadXmlAsyncCompleted(WebRequestAsyncState state)
        {
            XmlRequestAsyncState requestState = (XmlRequestAsyncState)state;
            XmlDocument xdocResult = new XmlDocument();
            try
            {
                if (!requestState.Request.Cancelled && requestState.Error == null && requestState.Request.ResponseStream != null)
                    xdocResult = (XmlDocument)GetXmlDocument(requestState.Request);
            }
            catch (HttpWebServiceException ex)
            {
                requestState.Error = ex;
            }

            if (requestState.Request.ResponseStream != null)
                requestState.Request.ResponseStream.Close();

            requestState.DownloadXmlCompleted(new DownloadXmlAsyncResult(xdocResult, requestState.Error), requestState.UserState);
        }

        /// <summary>
        /// Helper method to return an Xml document from the completed request.
        /// </summary>
        private static IXPathNavigable GetXmlDocument(HttpWebServiceRequest request)
        {
            if (request.ResponseStream == null)
                return null;

            request.ResponseStream.Seek(0, SeekOrigin.Begin);
            try
            {
                XmlDocument result = new XmlDocument();
                result.Load(Util.ZlibUncompress(request.ResponseStream));
                return result;
            }
            catch (XmlException ex)
            {
                throw HttpWebServiceException.XmlException(request.BaseUrl, ex);
            }
        }

        /// <summary>
        /// Helper class to retain the original callback and return data for asynchronous requests.
        /// </summary>
        private class XmlRequestAsyncState : WebRequestAsyncState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="XmlRequestAsyncState"/> class.
            /// </summary>
            /// <param name="callback">The callback.</param>
            /// <param name="webRequestCallback">The web request callback.</param>
            /// <param name="userState">State of the user.</param>
            public XmlRequestAsyncState(DownloadXmlCompletedCallback callback, WebRequestAsyncCallback webRequestCallback,
                                        object userState)
                : base(webRequestCallback)
            {
                DownloadXmlCompleted = callback;
                UserState = userState;
            }

            /// <summary>
            /// Gets the download XML completed.
            /// </summary>
            /// <value>The download XML completed.</value>
            public DownloadXmlCompletedCallback DownloadXmlCompleted { get; private set; }

            /// <summary>
            /// Gets the state of the user.
            /// </summary>
            /// <value>The state of the user.</value>
            public object UserState { get; private set; }
        }
    }
}