using System;
using System.IO;

namespace EVEMon.Common.Net
{
    public delegate void DownloadFileCompletedCallback(DownloadFileAsyncResult e);

    partial class HttpWebService
    {
        private const string FileAccept = "*/*;q=0.5";

        /// <summary>
        /// Downloads an file from the specified url to the specified path.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="postdata">The post data.</param>
        /// <param name="compressed">if set to <c>true</c> use compressed request.</param>
        /// <returns></returns>
        public FileInfo DownloadFile(Uri url, string filePath, string postdata = null, bool compressed = false)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, compressed);
            HttpWebServiceRequest request = GetRequest();
            try
            {
                FileStream responseStream;
                try
                {
                    responseStream = Util.GetFileStream(filePath, FileMode.Create, FileAccess.Write);
                }
                catch (Exception ex)
                {
                    throw HttpWebServiceException.FileError(url, ex);
                }
                request.GetResponse(url, postData, compressed, responseStream, FileAccept);
                return new FileInfo(filePath);
            }
            finally
            {
                if (request.ResponseStream != null)
                    request.ResponseStream.Close();
            }
        }

        /// <summary>
        /// Asynchronously downloads file from the specified url to the specified path.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="callback">A <see cref="DownloadImageCompletedCallback"/> to be invoked when the request is completed</param>
        /// <param name="progressCallback">The progress callback.</param>
        /// <param name="postdata">The postdata.</param>
        /// <param name="compressed">if set to <c>true</c> use compressed request.</param>
        /// <returns></returns>
        public object DownloadFileAsync(Uri url, string filePath, DownloadFileCompletedCallback callback,
                                        DownloadProgressChangedCallback progressCallback, string postdata = null, bool compressed = false)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            FileRequestAsyncState state = new FileRequestAsyncState(filePath, callback, progressCallback,
                                                                    DownloadFileAsyncCompleted);
            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, compressed);
            HttpWebServiceRequest request = GetRequest();
            FileStream responseStream = Util.GetFileStream(filePath, FileMode.Create, FileAccess.Write);
            request.GetResponseAsync(url, postData, compressed, responseStream, FileAccept, state);
            return request;
        }

        /// <summary>
        /// Callback method for asynchronous requests.
        /// </summary>
        private static void DownloadFileAsyncCompleted(WebRequestAsyncState state)
        {
            FileRequestAsyncState requestState = (FileRequestAsyncState)state;
            FileInfo fileResult = null;
            if (!requestState.Request.Cancelled && requestState.Error == null)
            {
                try
                {
                    fileResult = new FileInfo(requestState.FilePath);
                }
                catch (UnauthorizedAccessException ex)
                {
                    requestState.Error = HttpWebServiceException.FileError(requestState.Request.BaseUrl, ex);
                }
            }

            if (requestState.Request.ResponseStream != null)
                requestState.Request.ResponseStream.Close();

            requestState.DownloadFileCompleted(new DownloadFileAsyncResult(fileResult, requestState.Error,
                                                                           requestState.Request.Cancelled));
        }

        /// <summary>
        /// Helper class to retain the original callback and return data for asynchronous requests.
        /// </summary>
        private class FileRequestAsyncState : WebRequestAsyncState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FileRequestAsyncState"/> class.
            /// </summary>
            /// <param name="filepath">The filepath.</param>
            /// <param name="callback">The callback.</param>
            /// <param name="progressCallback">The progress callback.</param>
            /// <param name="webRequestCallback">The web request callback.</param>
            public FileRequestAsyncState(string filepath, DownloadFileCompletedCallback callback,
                                         DownloadProgressChangedCallback progressCallback,
                                         WebRequestAsyncCallback webRequestCallback)
                : base(webRequestCallback, progressCallback)
            {
                FilePath = filepath;
                DownloadFileCompleted = callback;
            }

            /// <summary>
            /// Gets or sets the download file completed.
            /// </summary>
            /// <value>The download file completed.</value>
            public DownloadFileCompletedCallback DownloadFileCompleted { get; private set; }

            /// <summary>
            /// Gets or sets the file path.
            /// </summary>
            /// <value>The file path.</value>
            public string FilePath { get; private set; }
        }
    }
}