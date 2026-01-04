using System;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Conainer class for HttpWebService settings and state
    /// </summary>
    public static class HttpWebClientServiceState
    {
        private static readonly object s_syncLock = new object();
        private static ProxySettings s_proxy = new ProxySettings();

        /// <summary>
        /// The maximum size of a download section.
        /// </summary>
        public static int MaxBufferSize => 8192;

        /// <summary>
        /// The minimum size if a download section.
        /// </summary>
        public static int MinBufferSize => 1024;

        /// <summary>
        /// The maintainer contact email for ESI User-Agent header.
        /// Per ESI best practices, this should be set to allow CCP to contact developers.
        /// </summary>
        public static string MaintainerEmail { get; set; } = "evemon-maintainer@example.com";

        /// <summary>
        /// The source code URL for ESI User-Agent header.
        /// </summary>
        public static string SourceUrl { get; set; } = "https://github.com/peterhaneve/evern";

        /// <summary>
        /// The user agent string for requests.
        /// Follows ESI best practices: https://developers.eveonline.com/docs/best-practices/
        /// Format: AppName/Version (email; +sourceUrl) (OS; arch)
        /// </summary>
        public static string UserAgent
        {
            get
            {
                var architecture = Environment.Is64BitOperatingSystem
                    ? "x64"
                    : "x86";
                var productName = EveMonClient.FileVersionInfo.ProductName;
                var version = EveMonClient.FileVersionInfo.FileVersion;

                // Build user agent per ESI best practices
                // Format: AppName/Version (contact info) (OS info)
                return $"{productName}/{version} ({MaintainerEmail}; +{SourceUrl}) ({Environment.OSVersion.VersionString}; {architecture})";
            }
        }

        /// <summary>
        /// The maximum redirects allowed for a request.
        /// </summary>
        public static int MaxRedirects => 5;

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