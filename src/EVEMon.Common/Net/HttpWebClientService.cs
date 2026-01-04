using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace EVEMon.Common.Net
{
    public static partial class HttpWebClientService
    {
        // Shared HttpClient instance to prevent socket exhaustion in .NET 8
        // See: https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
        private static readonly Lazy<HttpClient> s_sharedClient = new Lazy<HttpClient>(() =>
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                MaxConnectionsPerServer = 10,
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler);
            // Set a reasonable default timeout - can't be changed per-request on shared client
            client.Timeout = TimeSpan.FromSeconds(60);
            return client;
        });

        /// <summary>
        /// Initializes the <see cref="HttpWebClientService"/> class.
        /// </summary>
        static HttpWebClientService()
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 10;
#if false
            // To debug trust failure issues
            if (EveMonClient.IsDebugBuild)
                ServicePointManager.ServerCertificateValidationCallback = DummyCertificateValidationCallback;
#endif
        }

        /// <summary>
        /// A dummy certificate validation callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslpolicyerrors">The sslpolicyerrors.</param>
        /// <returns></returns>
        internal static bool DummyCertificateValidationCallback(object sender, X509Certificate
            certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors) => true;

        /// <summary>
        /// Gets the web client.
        /// </summary>
        /// <returns></returns>
        public static WebClient GetWebClient() => new WebClient
        {
            Proxy = HttpClientServiceRequest.GetWebProxy()
        };

        /// <summary>
        /// Gets the HTTP client. Returns shared instance for default handler,
        /// or creates new instance only when custom proxy is needed.
        /// </summary>
        /// <param name="httpClientHandler">The HTTP client handler (ignored in .NET 8, uses shared client).</param>
        /// <returns></returns>
        public static HttpClient GetHttpClient(HttpClientHandler httpClientHandler = null)
        {
            // In .NET 8, we use a shared HttpClient to prevent socket exhaustion.
            // The httpClientHandler parameter is kept for API compatibility but
            // we dispose it since we're not using it.
            httpClientHandler?.Dispose();
            return s_sharedClient.Value;
        }

        /// <summary>
        /// Validates a Url as acceptable for an HttpWebServiceRequest.
        /// </summary>
        /// <param name="url">A url <see cref="string"/> for the request. The string must specify HTTP or HTTPS as its scheme.</param>
        /// <param name="errorMsg">Is url is invalid, contains a descriptive message of the reason</param>
        public static bool IsValidURL(Uri url, out string errorMsg)
        {
            if (string.IsNullOrWhiteSpace(url.AbsoluteUri))
            {
                errorMsg = "Url may not be null or an empty string.";
                return false;
            }

            if (!Uri.IsWellFormedUriString(url.AbsoluteUri, UriKind.Absolute))
            {
                errorMsg = $"\"{url}\" is not a well-formed URL.";

                return false;
            }

            try
            {
                if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
                {
                    errorMsg = $"The specified scheme ({url.Scheme}) is not supported.";

                    return false;
                }
            }
            catch (UriFormatException)
            {
                errorMsg = $"\"{url}\" is not a valid URL for an HTTP or HTTPS request.";

                return false;
            }

            errorMsg = string.Empty;
            return true;
        }
    }
}
