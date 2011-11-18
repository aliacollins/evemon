using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Exception class for all exceptions thrown by HttpWebService requests.
    /// </summary>
    [Serializable]
    public sealed class HttpWebServiceException : Exception
    {
        public HttpWebServiceException()
        {
        }

        public HttpWebServiceException(string message)
            : base(message)
        {
        }

        public HttpWebServiceException(string message, Exception ex)
            : base(message, ex)
        {
        }

        private HttpWebServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebServiceException"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        private HttpWebServiceException(HttpWebServiceExceptionStatus status, Uri url, string message)
            : base(message)
        {
            Status = status;
            Url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebServiceException"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        private HttpWebServiceException(HttpWebServiceExceptionStatus status, Exception ex, Uri url, string message)
            : base(message, ex)
        {
            Status = status;
            Url = url;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public HttpWebServiceExceptionStatus Status { get; private set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public Uri Url { get; private set; }

        /// <summary>
        /// Gets the name of the host.
        /// </summary>
        /// <value>The name of the host.</value>
        public string HostName
        {
            get { return Url.Host; }
        }

        /// <summary>
        /// Factory method to create an HttpWebServiceException of type 'Exception'
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The exception that was thrown</param>
        /// <returns></returns>
        public static HttpWebServiceException Exception(Uri url, Exception ex)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new HttpWebServiceException(HttpWebServiceExceptionStatus.Exception, ex, url, "An Exception occurred.");
        }

        /// <summary>
        /// Factory method to create an HttpWebServiceException of type 'RedirectsExceeded'
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <returns></returns>
        public static HttpWebServiceException RedirectsExceededException(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new HttpWebServiceException(HttpWebServiceExceptionStatus.RedirectsExceeded, url,
                                               String.Format(CultureConstants.DefaultCulture,
                                                             ExceptionMessages.RedirectsExceeded, url.Host));
        }

        /// <summary>
        /// Factory method to create an HttpWebServiceException of type 'RequestsDisabled'
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <returns></returns>
        public static HttpWebServiceException RequestsDisabledException(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new HttpWebServiceException(HttpWebServiceExceptionStatus.RequestsDisabled, url,
                                               String.Format(CultureConstants.DefaultCulture,
                                                             ExceptionMessages.RequestsDisabled, url.Host));
        }

        /// <summary>
        /// Factory method to create an HttpWebServiceException resulting from a WebException.
        /// Various different HttpWebServiceExceptionStatus types are applied, with appropriate messages, depending on the
        /// nature of the WebException.
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="webServiceState">The EVEMonWebClientState instance of the request</param>
        /// <param name="ex">The WebException that was thrown</param>
        /// <returns></returns>
        public static HttpWebServiceException WebException(Uri url, HttpWebServiceState webServiceState, WebException ex)
        {
            if (webServiceState == null)
                throw new ArgumentNullException("webServiceState");

            string proxyHost = webServiceState.Proxy.Enabled
                                   ? webServiceState.Proxy.Host
                                   : WebRequest.DefaultWebProxy.GetProxy(url).Host;

            HttpWebServiceExceptionStatus status;

            string msg = ParseWebException(ex, url, proxyHost, out status);
            return new HttpWebServiceException(status, ex, url, msg);
        }

        /// <summary>
        /// Parses a web exception to get an error message and a <see cref="HttpWebServiceExceptionStatus"/> status code.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="url"></param>
        /// <param name="proxyHost"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private static string ParseWebException(WebException ex, Uri url, string proxyHost,
                                                out HttpWebServiceExceptionStatus status)
        {
            StringBuilder messageBuilder = new StringBuilder();
            switch (ex.Status)
            {
                case WebExceptionStatus.ProtocolError:
                    HttpWebResponse response = (HttpWebResponse)ex.Response;
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.ProxyAuthenticationRequired:
                            status = HttpWebServiceExceptionStatus.ProxyError;
                            messageBuilder.AppendFormat(
                                ExceptionMessages.ProxyAuthenticationFailure, proxyHost, url.Host);
                            break;

                        default:
                            status = HttpWebServiceExceptionStatus.ServerError;
                            messageBuilder.AppendFormat(ExceptionMessages.ServerError, url.Host);
                            messageBuilder.AppendLine(response.StatusDescription);
                            break;
                    }
                    break;
                case WebExceptionStatus.ProxyNameResolutionFailure:
                    status = HttpWebServiceExceptionStatus.ProxyError;
                    messageBuilder.AppendFormat(
                        ExceptionMessages.ProxyNameResolutionFailure, proxyHost);
                    break;
                case WebExceptionStatus.RequestProhibitedByProxy:
                    status = HttpWebServiceExceptionStatus.ProxyError;
                    messageBuilder.AppendFormat(ExceptionMessages.RequestProhibitedByProxy, url.Host, proxyHost);
                    break;
                case WebExceptionStatus.NameResolutionFailure:
                    status = HttpWebServiceExceptionStatus.NameResolutionFailure;
                    messageBuilder.AppendFormat(ExceptionMessages.NameResolutionFailure,
                                                proxyHost);
                    break;
                case WebExceptionStatus.ConnectFailure:
                    status = HttpWebServiceExceptionStatus.ConnectFailure;
                    messageBuilder.AppendFormat(ExceptionMessages.ConnectFailure, url.Host);
                    break;
                case WebExceptionStatus.Timeout:
                    status = HttpWebServiceExceptionStatus.Timeout;
                    messageBuilder.AppendFormat(ExceptionMessages.Timeout, url.Host);
                    break;
                default:
                    status = HttpWebServiceExceptionStatus.WebException;
                    messageBuilder.AppendFormat(ExceptionMessages.UnknownWebException, url.Host, ex.Status);
                    break;
            }

            return messageBuilder.ToString();
        }

        /// <summary>
        /// Factory method to create an HttpWebServiceException for Xml download requests that fail due to an XmlException
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The XmlException that was thrown</param>
        /// <returns></returns>
        public static HttpWebServiceException XmlException(Uri url, Exception ex)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new HttpWebServiceException(HttpWebServiceExceptionStatus.XmlException, ex, url,
                                               String.Format(CultureConstants.DefaultCulture,
                                                             ExceptionMessages.XmlException, url.Host));
        }

        /// <summary>
        /// Factory method to create an HttpWebServiceException for Image download requests that fail because a valid image was not returned
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The exception that was thrown loading the image</param>
        /// <returns></returns>
        public static HttpWebServiceException ImageException(Uri url, Exception ex)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new HttpWebServiceException(HttpWebServiceExceptionStatus.ImageException, ex, url,
                                               String.Format(CultureConstants.DefaultCulture,
                                                             ExceptionMessages.ImageException, url.Host));
        }

        /// <summary>
        /// Factory method to create an HttpWebServiceException for File download requests that fail because the file could not be written
        /// </summary>
        /// <param name="url">The url of the request that failed</param>
        /// <param name="ex">The exception that was thrown creating the file</param>
        /// <returns></returns>
        public static HttpWebServiceException FileError(Uri url, Exception ex)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new HttpWebServiceException(HttpWebServiceExceptionStatus.FileError, ex, url,
                                               String.Format(CultureConstants.DefaultCulture,
                                                             ExceptionMessages.FileException, url.Host));
        }
    }
}