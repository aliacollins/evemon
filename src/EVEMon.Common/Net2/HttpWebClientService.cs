using System;
using System.Net;
using System.Net.Http;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Net2
{
    public static partial class HttpWebClientService
    {
        /// <summary>
        /// Initializes the <see cref="HttpWebClientService"/> class.
        /// </summary>
        static HttpWebClientService()
        {
            ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// Gets the web client.
        /// </summary>
        /// <returns></returns>
        public static WebClient GetWebClient() => new WebClient();

        /// <summary>
        /// Gets the HTTP client.
        /// </summary>
        /// <param name="httpClientHandler">The HTTP client handler.</param>
        /// <returns></returns>
        public static HttpClient GetHttpClient(HttpClientHandler httpClientHandler = null) =>
            httpClientHandler == null
                ? new HttpClient()
                : new HttpClient(httpClientHandler);

        /// <summary>
        /// Validates a Url as acceptable for an HttpWebServiceRequest.
        /// </summary>
        /// <param name="url">A url <see cref="string"/> for the request. The string must specify HTTP or HTTPS as its scheme.</param>
        /// <param name="errorMsg">Is url is invalid, contains a descriptive message of the reason</param>
        public static bool IsValidURL(Uri url, out string errorMsg)
        {
            if (String.IsNullOrWhiteSpace(url.AbsoluteUri))
            {
                errorMsg = "Url may not be null or an empty string.";
                return false;
            }

            if (!Uri.IsWellFormedUriString(url.AbsoluteUri, UriKind.Absolute))
            {
                errorMsg = String.Format(CultureConstants.DefaultCulture,
                    "\"{0}\" is not a well-formed URL.",
                    url);

                return false;
            }

            try
            {
                if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
                {
                    errorMsg = String.Format(CultureConstants.DefaultCulture,
                        "The specified scheme ({0}) is not supported.",
                        url.Scheme);

                    return false;
                }
            }
            catch (UriFormatException)
            {
                errorMsg = String.Format(CultureConstants.DefaultCulture,
                    "\"{0}\" is not a valid URL for an HTTP or HTTPS request.",
                    url);

                return false;
            }

            errorMsg = String.Empty;
            return true;
        }
    }
}