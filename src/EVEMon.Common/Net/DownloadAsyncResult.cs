namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class to return the result of an asynchronous string download
    /// </summary>
    public class DownloadAsyncResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadAsyncResult{T}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="error">The error.</param>
        public DownloadAsyncResult(T result, HttpWebClientServiceException error)
        {
            Error = error;
            Result = result;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public T Result { get; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public HttpWebClientServiceException Error { get; }
    }
}