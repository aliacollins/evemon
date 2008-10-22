using System;
using System.Net;

namespace EVEMon.Common.Net
{
    public delegate void DownloadProgressChangedCallback(DownloadProgressChangedArgs e);

    /// <summary>
    /// EVEMonWebClient provides all HTTP-based download services. It is intended to be used as a singleton
    /// instance via the Singleton class.
    /// </summary>
    [Singleton]
    public partial class EVEMonWebClient
    {
        private readonly EVEMonWebClientState _state = new EVEMonWebClientState();

        public EVEMonWebClient()
        {
            ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// State is a read-only instance of EVEMonWebClientState. Changes to web client settings should be made
        /// to properties of this instance.
        /// </summary>
        public EVEMonWebClientState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Validates a Url as acceptable for an EVEMonWebRequest
        /// </summary>
        /// <param name="url">A url <see cref="string"/> for the request. The string must specify HTTP as its scheme.</param>
        /// <param name="errorMsg">Is url is invalid, contains a descriptive message of the reason</param>
        public static bool IsValidURL(string url, out string errorMsg)
        {
            if (string.IsNullOrEmpty(url))
            {
                errorMsg = "url may not be null or an empty string";
                return false;
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                errorMsg = String.Format("\"{0}\" is not a well-formed URL.", url);
                return false;
            }
            try
            {
                Uri tempUri = new Uri(url);
                if (tempUri.Scheme != Uri.UriSchemeHttp)
                {
                    errorMsg = String.Format("The specified scheme ({0}) is not supported.", tempUri.Scheme);
                    return false;
                }
            }
            catch (Exception)
            {
                errorMsg = String.Format("\"{0}\" is not a valid URL for an HTTP request.", url);
                return false;
            }
            errorMsg = "";
            return true;
        }

        /// <summary>
        /// Cancels an asynchronous request in progress
        /// </summary>
        public void CancelRequest(object request)
        {
            if (request.GetType() == typeof(EVEMonWebRequest))
            {
                ((EVEMonWebRequest) request).Cancelled = true;
            }
        }

        /// <summary>
        /// Factory method to construct an EVEMonWebRequest instance
        /// </summary>
        /// <returns></returns>
        private EVEMonWebRequest GetRequest()
        {
            return new EVEMonWebRequest(_state);
        }
    }
}