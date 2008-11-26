using System.Reflection;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Conainer class for HttpWebService settings and state
    /// </summary>
    public class HttpWebServiceState
    {
        private const int MAX_BUFFER_SIZE = 8192;
        private const int MIN_BUFFER_SIZE = 1024;
        private const int MAX_REDIRECTS = 5;
        private readonly object _syncLock = new object();
        private readonly string _userAgent = "EVEMon/" + Assembly.GetExecutingAssembly().GetName().Version;
        private bool _requestsDisabled = false;
        private bool _useCustomProxy = false;
        private bool _disableOnProxyAuthenticationFailure = true;
        private ProxySetting _proxy = new ProxySetting();

        internal HttpWebServiceState()
        {
        }

        /// <summary>
        /// The maximum size of a download section
        /// </summary>
        public int MaxBufferSize
        {
            get { return MAX_BUFFER_SIZE; }
        }

        /// <summary>
        /// The minimum size if a download section
        /// </summary>
        public int MinBufferSize
        {
            get { return MIN_BUFFER_SIZE; }
        }

        /// <summary>
        /// The user agent string for requests
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
        }

        /// <summary>
        /// The maximum redirects allowed for a request
        /// </summary>
        public int MaxRedirects
        {
            get { return MAX_REDIRECTS; }
        }

        /// <summary>
        /// Flag to indicate where requests are temporarily disabled
        /// </summary>
        public bool RequestsDisabled
        {
            get { lock(_syncLock) {return _requestsDisabled;} }
            set { lock(_syncLock) {_requestsDisabled = value;} }
        }

        /// <summary>
        /// When true, further requests will be temporarily disabled if proxy authentication fails
        /// </summary>
        public bool DisableOnProxyAuthenticationFailure
        {
            get { lock(_syncLock) {return _disableOnProxyAuthenticationFailure;} }
            set
            {
                lock(_syncLock)
                {
                    if (value == false && _requestsDisabled)
                        _requestsDisabled = false;
                    _disableOnProxyAuthenticationFailure = value;
                }
            }
        }

        /// <summary>
        /// When true, custom proxy settings will be used, otherwise default proxy settings will be used.
        /// If the setting is reset and requests are currently disabled they will be enabled again
        /// </summary>
        public bool UseCustomProxy
        {
            get { lock(_syncLock) {return _useCustomProxy;} }
            set
            {
                 lock(_syncLock)
                 {
                     if (_useCustomProxy && value == false && _requestsDisabled) _requestsDisabled = false;
                     _useCustomProxy = value;
                 }
            }
        }

        /// <summary>
        /// A ProxySetting instance for the custom proxy to be used
        /// </summary>
        public ProxySetting Proxy
        {
            get { lock(_syncLock) {return _proxy;} }
            set
            {
                 lock(_syncLock)
                 {
                     if (ProxyChanged(value) && _requestsDisabled) _requestsDisabled = false;
                     _proxy = value;
                 }
             }
        }

        /// <summary>
        /// Helper method to determine if proxy settings have been changed
        /// </summary>
        /// <param name="proxySetting"></param>
        /// <returns></returns>
        private bool ProxyChanged(ProxySetting proxySetting)
        {
            return (proxySetting.Host != _proxy.Host)
                   || (proxySetting.Port != _proxy.Port)
                   || (proxySetting.AuthType != _proxy.AuthType)
                   || (proxySetting.Username != _proxy.Username)
                   || (proxySetting.Password != _proxy.Password);
        }
    }
}
