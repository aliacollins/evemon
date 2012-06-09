using System.IO;
using System.Windows.Forms;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Conainer class for HttpWebService settings and state
    /// </summary>
    public static class HttpWebServiceState
    {
        private static readonly object s_syncLock = new object();
        private static ProxySettings s_proxy = new ProxySettings();

        /// <summary>
        /// The maximum size of a download section.
        /// </summary>
        public static int MaxBufferSize
        {
            get { return 8192; }
        }

        /// <summary>
        /// The minimum size if a download section.
        /// </summary>
        public static int MinBufferSize
        {
            get { return 1024; }
        }

        /// <summary>
        /// The user agent string for requests.
        /// </summary>
        public static string UserAgent
        {
            get { return Application.ProductName + Path.AltDirectorySeparatorChar + Application.ProductVersion; }
        }

        /// <summary>
        /// The maximum redirects allowed for a request.
        /// </summary>
        public static int MaxRedirects
        {
            get { return 5; }
        }

        /// <summary>
        /// A ProxySetting instance for the custom proxy to be used.
        /// </summary>
        public static ProxySettings Proxy
        {
            get
            {
                lock (s_syncLock)
                {
                    return s_proxy;
                }
            }
            set
            {
                lock (s_syncLock)
                {
                    s_proxy = value;
                }
            }
        }
    }
}