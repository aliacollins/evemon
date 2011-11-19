using System;
using System.IO;

namespace EVEMon.Common.Net
{
    public delegate void DownloadFileCompletedCallback(DownloadFileAsyncResult e);

    partial class HttpWebService
    {
        private const string FILE_ACCEPT = "*/*;q=0.5";

        /// <summary>
        /// Downloads an file from the specified url to the specified path.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public FileInfo DownloadFile(Uri url, string filePath)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpWebServiceRequest request = GetRequest();
            try
            {
                FileStream responseStream;
                try
                {
                    responseStream = GetStream(filePath);
                }
                catch (Exception ex)
                {
                    throw HttpWebServiceException.FileError(url, ex);
                }
                request.GetResponse(url, responseStream, FILE_ACCEPT);
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
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="callback">A <see cref="DownloadImageCompletedCallback"/> to be invoked when the request is completed</param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public object DownloadFileAsync(Uri url, string filePath, DownloadFileCompletedCallback callback,
                                        DownloadProgressChangedCallback progressCallback)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            FileRequestAsyncState state = new FileRequestAsyncState(filePath, callback, progressCallback,
                                                                    DownloadFileAsyncCompleted);
            HttpWebServiceRequest request = GetRequest();
            FileStream responseStream = GetStream(filePath);
            request.GetResponseAsync(url, responseStream, IMAGE_ACCEPT, null, state);
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
        /// Gets the stream.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static FileStream GetStream(string filePath)
        {
            FileStream stream;
            FileStream tempStream = null;
            try
            {
                tempStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                stream = tempStream;
                tempStream = null;
            }
            finally
            {
                if (tempStream != null)
                    tempStream.Dispose();
            }
            return stream;
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