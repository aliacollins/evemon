namespace EVEMon.Common.Net
{
    internal delegate void WebRequestAsyncCallback(WebRequestAsyncState state);

    /// <summary>
    /// Abstract base class for asynchronous request state classes
    /// </summary>
    internal abstract class WebRequestAsyncState
    {
        private EVEMonWebRequest _request;
        private EVEMonWebException _error;
        private readonly WebRequestAsyncCallback _callback;
        private readonly DownloadProgressChangedCallback _progressCallback;

        public WebRequestAsyncState(WebRequestAsyncCallback callback)
            :this(callback, null)
        {
        }

        public WebRequestAsyncState(WebRequestAsyncCallback callback, DownloadProgressChangedCallback progressCallback)
        {
            _callback = callback;
            _progressCallback = progressCallback;
        }

        public EVEMonWebRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }

        public EVEMonWebException Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public WebRequestAsyncCallback Callback
        {
            get { return _callback; }
        }

        public DownloadProgressChangedCallback ProgressCallback
        {
            get { return _progressCallback; }
        }
    }
}