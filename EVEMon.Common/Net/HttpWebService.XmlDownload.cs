using System;
using System.IO;
using System.Xml;

namespace EVEMon.Common.Net
{
    public delegate void DownloadXmlCompletedCallback(DownloadXmlAsyncResult e, object userState);

    /// <summary>
    /// HttpWebService Xml download implementation.
    /// </summary>
    partial class HttpWebService
    {
        private const string XML_ACCEPT =
            "text/xml,application/xml,application/xhtml+xml;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Downloads an Xml document from the specified url using the specified POST data.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public XmlDocument DownloadXml(string url, HttpPostData postData = null)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpWebServiceRequest request = GetRequest();
            try
            {
                request.GetResponse(url, new MemoryStream(), XML_ACCEPT, postData);
                XmlDocument result = new XmlDocument();
                if (request.ResponseStream != null)
                {
                    request.ResponseStream.Seek(0, SeekOrigin.Begin);
                    result.Load(request.ResponseStream);
                }
                return result;
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
        /// <param name="postData">The post data.</param>
        /// <param name="callback">A <see cref="DownloadXmlCompletedCallback"/> to be invoked when the request is completed</param>
        /// <param name="userState">A state object to be returned to the callback</param>
        /// <returns></returns>
        public void DownloadXmlAsync(string url, HttpPostData postData, DownloadXmlCompletedCallback callback, object userState)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            XmlRequestAsyncState state = new XmlRequestAsyncState(callback, DownloadXmlAsyncCompleted, userState);
            HttpWebServiceRequest request = GetRequest();
            request.GetResponseAsync(url, new MemoryStream(), XML_ACCEPT, postData, state);
        }

        /// <summary>
        /// Callback method for asynchronous requests.
        /// </summary>
        private void DownloadXmlAsyncCompleted(WebRequestAsyncState state)
        {
            XmlRequestAsyncState requestState = (XmlRequestAsyncState)state;
            XmlDocument xdocResult = new XmlDocument();
            if (!requestState.Request.Cancelled && requestState.Error == null && requestState.Request.ResponseStream != null)
            {
                try
                {
                    requestState.Request.ResponseStream.Seek(0, SeekOrigin.Begin);
                    xdocResult.Load(requestState.Request.ResponseStream);
                }
                catch (XmlException ex)
                {
                    requestState.Error = HttpWebServiceException.XmlException(requestState.Request.BaseUrl, ex);
                }
            }

            if (requestState.Request.ResponseStream != null)
                requestState.Request.ResponseStream.Close();
            requestState.DownloadXmlCompleted(new DownloadXmlAsyncResult(xdocResult, requestState.Error), requestState.UserState);
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