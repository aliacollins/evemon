namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class to return the result of an asynchronous string download
    /// </summary>
    public class DownloadStringAsyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadStringAsyncResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="error">The error.</param>
        public DownloadStringAsyncResult(string result, HttpWebServiceException error)
        {
            Error = error;
            Result = result;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public string Result { get; private set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public HttpWebServiceException Error { get; private set; }
    }
}