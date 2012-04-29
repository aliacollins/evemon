using System;
using System.IO;

namespace EVEMon.Common.Net
{
    public delegate void DownloadStringCompletedCallback(DownloadStringAsyncResult e, object userState);

    /// <summary>
    /// HttpWebService String download implementation.
    /// </summary>
    public partial class HttpWebService
    {
        private const string StringAccept =
            "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Synchronously downloads a string from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="gzipCompressed">if set to <c>true</c> use gzip compressed request.</param>
        /// <returns></returns>
        public String DownloadString(Uri url, string postdata = null, bool gzipCompressed = false)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, gzipCompressed);
            HttpWebServiceRequest request = GetRequest();
            try
            {
                MemoryStream responseStream = Util.GetMemoryStream();
                request.GetResponse(url, postData, gzipCompressed, responseStream, StringAccept);
                string result = String.Empty;
                if (request.ResponseStream != null)
                {
                    request.ResponseStream.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(request.ResponseStream))
                    {
                        result = reader.ReadToEnd();
                    }
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
        /// Asynchronously downloads a string from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="callback">A <see cref="DownloadXmlCompletedCallback"/> to be invoked when the request is completed</param>
        /// <param name="userState">A state object to be returned to the callback</param>
        /// <param name="postdata">The postdata.</param>
        /// <param name="gzipCompressed">if set to <c>true</c> use gzip compressed request.</param>
        public void DownloadStringAsync(Uri url, DownloadStringCompletedCallback callback, object userState,
                                        string postdata = null, bool gzipCompressed = false)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            StringRequestAsyncState state = new StringRequestAsyncState(callback, DownloadStringAsyncCompleted, userState);
            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, gzipCompressed);
            HttpWebServiceRequest request = GetRequest();
            MemoryStream responseStream = Util.GetMemoryStream();
            request.GetResponseAsync(url, postData, gzipCompressed, responseStream, StringAccept, state);
        }

        /// <summary>
        /// Callback method for asynchronous requests.
        /// </summary>
        private static void DownloadStringAsyncCompleted(WebRequestAsyncState state)
        {
            StringRequestAsyncState requestState = (StringRequestAsyncState)state;
            string result = String.Empty;
            if (!requestState.Request.Cancelled && requestState.Error == null && requestState.Request.ResponseStream != null)
            {
                requestState.Request.ResponseStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(requestState.Request.ResponseStream))
                {
                    result = reader.ReadToEnd();
                }
            }

            if (requestState.Request.ResponseStream != null)
                requestState.Request.ResponseStream.Close();

            requestState.DownloadStringCompleted(new DownloadStringAsyncResult(result, requestState.Error), requestState.UserState);
        }

        /// <summary>
        /// Helper class to retain the original callback and return data for asynchronous requests.
        /// </summary>
        private class StringRequestAsyncState : WebRequestAsyncState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StringRequestAsyncState"/> class.
            /// </summary>
            /// <param name="callback">The callback.</param>
            /// <param name="webRequestCallback">The web request callback.</param>
            /// <param name="userState">State of the user.</param>
            public StringRequestAsyncState(DownloadStringCompletedCallback callback, WebRequestAsyncCallback webRequestCallback,
                                           object userState)
                : base(webRequestCallback)
            {
                DownloadStringCompleted = callback;
                UserState = userState;
            }

            /// <summary>
            /// Gets the download string completed.
            /// </summary>
            /// <value>The download string completed.</value>
            public DownloadStringCompletedCallback DownloadStringCompleted { get; private set; }

            /// <summary>
            /// Gets the state of the user.
            /// </summary>
            /// <value>The state of the user.</value>
            public object UserState { get; private set; }
        }
    }
}