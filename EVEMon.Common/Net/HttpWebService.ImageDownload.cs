using System;
using System.Drawing;
using System.IO;

namespace EVEMon.Common.Net
{
    public delegate void DownloadImageCompletedCallback(DownloadImageAsyncResult e, object userState);

    /// <summary>
    /// HttpWebService Image download implementation.
    /// </summary>
    partial class HttpWebService
    {
        private const string IMAGE_ACCEPT = "image/png,*/*;q=0.5";

        /// <summary>
        /// Downloads an image from the specified url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Image DownloadImage(Uri url)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpWebServiceRequest request = GetRequest();
            try
            {
                request.GetResponse(url, new MemoryStream(), IMAGE_ACCEPT);
                return GetImage(request);
            }
            finally
            {
                if (request.ResponseStream != null)
                    request.ResponseStream.Close();
            }
        }

        /// <summary>
        /// Asynchronously downloads an image from the specified url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback">A <see cref="DownloadImageCompletedCallback"/> to be invoked when the request is completed</param>
        /// <param name="userState">A state object to be returned to the callback</param>
        /// <returns></returns>
        public void DownloadImageAsync(Uri url, DownloadImageCompletedCallback callback, object userState)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            ImageRequestAsyncState state = new ImageRequestAsyncState(callback, DownloadImageAsyncCompleted, userState);
            HttpWebServiceRequest request = GetRequest();
            request.GetResponseAsync(url, new MemoryStream(), IMAGE_ACCEPT, null, state);
        }

        /// <summary>
        /// Callback method for asynchronous requests
        /// </summary>
        private static void DownloadImageAsyncCompleted(WebRequestAsyncState state)
        {
            ImageRequestAsyncState requestState = (ImageRequestAsyncState)state;
            Image imageResult = null;
            try
            {
                if (!requestState.Request.Cancelled && requestState.Error == null)
                    imageResult = GetImage(requestState.Request);
            }
            catch (HttpWebServiceException ex)
            {
                requestState.Error = ex;
            }

            if (requestState.Request.ResponseStream != null)
                requestState.Request.ResponseStream.Close();

            requestState.DownloadImageCompleted(new DownloadImageAsyncResult(imageResult, requestState.Error),
                                                requestState.UserState);
        }

        /// <summary>
        /// Helper method to return an Image from the completed request.
        /// </summary>
        private static Image GetImage(HttpWebServiceRequest request)
        {
            if (request.ResponseStream == null)
                return null;

            request.ResponseStream.Seek(0, SeekOrigin.Begin);
            try
            {
                return Image.FromStream(request.ResponseStream, true);
            }
            catch (ArgumentException ex)
            {
                throw HttpWebServiceException.ImageException(request.BaseUrl, ex);
            }
        }

        /// <summary>
        /// Helper class to retain the original callback and return data for asynchronous requests
        /// </summary>
        private class ImageRequestAsyncState : WebRequestAsyncState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ImageRequestAsyncState"/> class.
            /// </summary>
            /// <param name="callback">The callback.</param>
            /// <param name="webRequestCallback">The web request callback.</param>
            /// <param name="userState">State of the user.</param>
            public ImageRequestAsyncState(DownloadImageCompletedCallback callback, WebRequestAsyncCallback webRequestCallback,
                                          object userState)
                : base(webRequestCallback)
            {
                DownloadImageCompleted = callback;
                UserState = userState;
            }

            /// <summary>
            /// Gets the download image completed.
            /// </summary>
            /// <value>The download image completed.</value>
            public DownloadImageCompletedCallback DownloadImageCompleted { get; private set; }

            /// <summary>
            /// Gets the state of the user.
            /// </summary>
            /// <value>The state of the user.</value>
            public object UserState { get; private set; }
        }
    }
}