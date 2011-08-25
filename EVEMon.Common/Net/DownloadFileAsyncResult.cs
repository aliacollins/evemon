using System.IO;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class to return the result of an asynchronous file download
    /// </summary>
    public class DownloadFileAsyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileAsyncResult"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="error">The error.</param>
        /// <param name="cancelled">if set to <c>true</c> [cancelled].</param>
        public DownloadFileAsyncResult(FileInfo file, HttpWebServiceException error, bool cancelled)
        {
            Error = error;
            Cancelled = cancelled;
            Result = file;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public FileInfo Result { get; private set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public HttpWebServiceException Error { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DownloadFileAsyncResult"/> is cancelled.
        /// </summary>
        /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
        public bool Cancelled { get; private set; }
    }
}
