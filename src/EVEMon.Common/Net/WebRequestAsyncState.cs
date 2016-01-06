namespace EVEMon.Common.Net
{
    internal delegate void WebRequestAsyncCallback(WebRequestAsyncState state);

    /// <summary>
    /// Abstract base class for asynchronous request state classes
    /// </summary>
    internal abstract class WebRequestAsyncState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestAsyncState"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        ///// <param name="progressCallback">The progress callback.</param>
        protected WebRequestAsyncState(WebRequestAsyncCallback callback/*, DownloadProgressChangedCallback progressCallback = null*/)
        {
            Callback = callback;
            //ProgressCallback = progressCallback;
        }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public HttpWebServiceRequest Request { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public HttpWebServiceException Error { get; set; }

        /// <summary>
        /// Gets or sets the callback.
        /// </summary>
        /// <value>The callback.</value>
        public WebRequestAsyncCallback Callback { get; private set; }

        ///// <summary>
        ///// Gets or sets the progress callback.
        ///// </summary>
        ///// <value>The progress callback.</value>
        //public DownloadProgressChangedCallback ProgressCallback { get; private set; }
    }
}