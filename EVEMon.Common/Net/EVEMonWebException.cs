using System;
using System.Net;
using System.Text;
using System.Xml;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Exception class for all exceptions thrown by EVEMonWebClient requests
    /// </summary>
    public class EVEMonWebException : ApplicationException
    {
        private readonly EVEMonWebExceptionStatus _status;
        private readonly string _url;

        private EVEMonWebException(EVEMonWebExceptionStatus status, string url, string message)
            : base(message)
        {
            _status = status;
            _url = url;
        }

        private EVEMonWebException(EVEMonWebExceptionStatus status, Exception ex, string url, string message)
            : base(message, ex)
        {
            _status = status;
            _url = url;
        }

        public EVEMonWebExceptionStatus Status
        {
            get { return _status; }
        }

        public string Url
        {
            get { return _url; }
        }

        public string HostName
        {
            get { return new Uri(_url).Host; }
        }

        /// <summary>
        /// Factory method to create an EVEMonWebException of type 'Exception'
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The exception that was thrown</param>
        /// <returns></returns>
        public static EVEMonWebException Exception(string url, Exception ex)
        {
            return new EVEMonWebException(EVEMonWebExceptionStatus.Exception, ex, url, "An Exception occured.");
        }

        /// <summary>
        /// Factory method to create an EVEMonWebException of type 'RedirectsExceeded'
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <returns></returns>
        public static EVEMonWebException RedirectsExceededException(string url)
        {
            return new EVEMonWebException(EVEMonWebExceptionStatus.RedirectsExceeded, url, String.Format(ExceptionMessages.RedirectsExceeded, GetHostName(url)));
        }

        /// <summary>
        /// Factory method to create an EVEMonWebException of type 'RequestsDisabled'
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <returns></returns>
        public static EVEMonWebException RequestsDisabledException(string url)
        {
            return new EVEMonWebException(EVEMonWebExceptionStatus.RequestsDisabled, url, String.Format(ExceptionMessages.RequestsDisabled, GetHostName(url)));
        }

        /// <summary>
        /// Factory method to create an EVEMonWebException resulting from a WebException.
        /// Various different EVEMonWebExceptionStatus types are applied, with appropriate messages, depending on the
        /// nature of the WebException.
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="webClientState">The EVEMonWebClientState instance of the request</param>
        /// <param name="ex">The WebException that was thrown</param>
        /// <returns></returns>
        public static EVEMonWebException WebException(string url, EVEMonWebClientState webClientState, WebException ex)
        {
            StringBuilder messageBuilder = new StringBuilder();
            EVEMonWebExceptionStatus status;
            string proxyHost;
            if (webClientState.UseCustomProxy)
                proxyHost = webClientState.Proxy.Host;
            else
                proxyHost = HttpWebRequest.DefaultWebProxy.GetProxy(new Uri(url)).Host;

            switch(ex.Status)
            {
                case WebExceptionStatus.ProtocolError:
                    HttpWebResponse response = (HttpWebResponse) ex.Response;
                    switch(response.StatusCode)
                    {
                        case HttpStatusCode.ProxyAuthenticationRequired:
                            status = EVEMonWebExceptionStatus.ProxyError;
                            if (webClientState.RequestsDisabled)
                                messageBuilder.AppendLine(
                                    String.Format(ExceptionMessages.ProxyAuthenticationFailureDisabledRequests,
                                                  proxyHost, GetHostName(url)));
                            else
                                messageBuilder.AppendLine(
                                    String.Format(ExceptionMessages.ProxyAuthenticationFailure,
                                                  proxyHost, GetHostName(url)));
                            break;
                        default:
                            status = EVEMonWebExceptionStatus.ServerError;
                            messageBuilder.AppendLine(
                                String.Format(ExceptionMessages.ServerError, GetHostName(url)));
                            messageBuilder.AppendLine(response.StatusDescription);
                            break;
                    }
                    break;
                case WebExceptionStatus.ProxyNameResolutionFailure:
                    status = EVEMonWebExceptionStatus.ProxyError;
                    messageBuilder.AppendLine(
                        String.Format(ExceptionMessages.ProxyNameResolutionFailure, proxyHost));
                    break;
                case WebExceptionStatus.RequestProhibitedByProxy:
                    status = EVEMonWebExceptionStatus.ProxyError;
                    messageBuilder.AppendLine(
                        String.Format(ExceptionMessages.RequestProhibitedByProxy, GetHostName(url), proxyHost));
                    break;
                case WebExceptionStatus.NameResolutionFailure:
                    status = EVEMonWebExceptionStatus.NameResolutionFailure;
                    messageBuilder.AppendLine(
                        String.Format(ExceptionMessages.NameResolutionFailure, proxyHost));
                    break;
                case WebExceptionStatus.ConnectFailure:
                    status = EVEMonWebExceptionStatus.ConnectFailure;
                    messageBuilder.AppendLine(String.Format(ExceptionMessages.ConnectFailure, GetHostName(url)));
                    break;
                case WebExceptionStatus.Timeout:
                    status = EVEMonWebExceptionStatus.Timeout;
                    messageBuilder.AppendLine(String.Format(ExceptionMessages.Timeout, GetHostName(url)));
                    break;
                default:
                    status = EVEMonWebExceptionStatus.WebException;
                    messageBuilder.AppendLine(
                        String.Format(ExceptionMessages.UnknownWebException, GetHostName(url), ex.Status));
                    break;
            }
            return new EVEMonWebException(status, ex, url, messageBuilder.ToString());
        }

        /// <summary>
        /// Factory method to create an EVEMonWebException for Xml download requests that fail due to an XmlException
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The XmlException that was thrown</param>
        /// <returns></returns>
        public static EVEMonWebException XmlException(string url, XmlException ex)
        {
            return new EVEMonWebException(EVEMonWebExceptionStatus.XmlException, ex, url, String.Format(ExceptionMessages.XmlException, GetHostName(url)));
        }

        /// <summary>
        /// Factory method to create an EVEMonWebException for Image download requests that fail because a valid image was not returned
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The exception that was thrown loading the image</param>
        /// <returns></returns>
        public static EVEMonWebException ImageException(string url, ArgumentException ex)
        {
            return new EVEMonWebException(EVEMonWebExceptionStatus.ImageException, ex, url, String.Format(ExceptionMessages.ImageException, GetHostName(url)));
        }

        /// <summary>
        /// Factory method to create an EVEMonWebException for File download requests that fail because the file could not be written
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The exception that was thrown creating the file</param>
        /// <returns></returns>
        public static EVEMonWebException FileError(string url, Exception ex)
        {
            return new EVEMonWebException(EVEMonWebExceptionStatus.FileError, ex, url, String.Format(ExceptionMessages.FileException, GetHostName(url)));
        }

        /// <summary>
        /// Helper method to return the host name of a url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetHostName(string url)
        {
            return new Uri(url).Host;
        }
    }
}
