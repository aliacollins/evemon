using System;
using System.IO;
using System.Net;

namespace EVEMon.Common.Net
{
    public delegate void DownloadProgressChangedCallback(DownloadProgressChangedArgs e);

    /// <summary>
    /// HttpWebService provides all HTTP-based download services.
    /// It is intended to be used as a singleton instance via the Singleton class.
    /// </summary>
    public partial class HttpWebService
    {
        private readonly HttpWebServiceState m_state = new HttpWebServiceState();

        internal HttpWebService()
        {
            ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// State is a read-only instance of HttpWebServiceState.
        /// Changes to web client settings should be made to properties of this instance.
        /// </summary>
        public HttpWebServiceState State
        {
            get { return m_state; }
        }

        /// <summary>
        /// Validates a Url as acceptable for an HttpWebServiceRequest.
        /// </summary>
        /// <param name="url">A url <see cref="string"/> for the request. The string must specify HTTP as its scheme.</param>
        /// <param name="errorMsg">Is url is invalid, contains a descriptive message of the reason</param>
        private static bool IsValidURL(Uri url, out string errorMsg)
        {
            if (String.IsNullOrEmpty(url.AbsoluteUri))
            {
                errorMsg = "Url may not be null or an empty string.";
                return false;
            }

            if (!Uri.IsWellFormedUriString(url.AbsoluteUri, UriKind.Absolute))
            {
                errorMsg = String.Format(CultureConstants.DefaultCulture, "\"{0}\" is not a well-formed URL.", url);
                return false;
            }

            try
            {
                if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
                {
                    errorMsg = String.Format(CultureConstants.DefaultCulture, "The specified scheme ({0}) is not supported.",
                                             url.Scheme);
                    return false;
                }
            }
            catch (UriFormatException)
            {
                errorMsg = String.Format(CultureConstants.DefaultCulture, "\"{0}\" is not a valid URL for an HTTP or HTTPS request.", url);
                return false;
            }

            errorMsg = String.Empty;
            return true;
        }

        /// <summary>
        /// Cancels an asynchronous request in progress.
        /// </summary>
        public static void CancelRequest(object request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.GetType() == typeof(HttpWebServiceRequest))
                ((HttpWebServiceRequest)request).Cancelled = true;
        }

        /// <summary>
        /// Factory method to construct an EVEMonWebRequest instance.
        /// </summary>
        /// <returns></returns>
        private HttpWebServiceRequest GetRequest()
        {
            return new HttpWebServiceRequest(m_state);
        }
    }
}