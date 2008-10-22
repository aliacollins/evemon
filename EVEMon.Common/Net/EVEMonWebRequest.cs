using System;
using System.IO;
using System.Net;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// The core class that retrieves data from the web via HTTP. Requests are carried out by the GetResponse methods. The content of
    /// the response is returned via the stream parameter passed to GetResponse
    /// </summary>
    internal class EVEMonWebRequest
    {
        private readonly EVEMonWebClientState _webClientState;
        private string _baseUrl;
        private string _accept;
        private Stream _responseStream;
        private HttpPostData _postData;
        private string _url;
        private int _redirectsRemaining;
        private string _referer = String.Empty;
        private WebRequestAsyncState _asyncState;
        private bool _cancelled = false;
        private readonly object _syncLock = new object();

        /// <summary>
        /// Initialises a new instance of EVEMonWebRequest to be submitted as a POST request.
        /// </summary>
        /// <param name="webClientState">An <see cref="EVEMonWebClientState"/> instance</param>
        internal EVEMonWebRequest(EVEMonWebClientState webClientState)
        {
            _webClientState = webClientState;
            _redirectsRemaining = _webClientState.MaxRedirects;
        }

        /// <summary>
        /// The <see cref="Stream"/> to which the reponse is written.
        /// </summary>
        internal Stream ResponseStream
        {
            get { return _responseStream; }
        }

        /// <summary>
        /// The original url for the request
        /// </summary>
        public string BaseUrl
        {
            get { return _baseUrl; }
        }

        /// <summary>
        /// Returns true if an asynchronous request was cancelled. When set to true, cancels the current asynchronous request.
        /// </summary>
        public bool Cancelled
        {
            get { lock(_syncLock) return _cancelled; }
            set { lock(_syncLock) _cancelled = value; }
        }

        /// <summary>
        /// Retrieve the response from the requested URL to the specified response stream as a GET request
        /// </summary>
        internal void GetResponse(string url, Stream responseStream, string accept)
        {
             GetResponse(url, responseStream, accept, null);
        }

        /// <summary>
        /// Delegate for asynchronous invocation of GetResponse
        /// </summary>
        private delegate void GetResponseDelegate(
            string url, Stream responseStream, string accept, HttpPostData postData);

        /// <summary>
        /// Retrieve the response from the reqyested URL to the specified response stream
        /// If postData is supplied, the request is submitted as a POST request, otherwise it is submitted as a GET request
        /// The download process is broken into chunks for future implementation of asynchronous requests
        /// </summary>
        internal void GetResponse(string url, Stream responseStream, string accept, HttpPostData postData)
        {
            if (_webClientState.RequestsDisabled)
                throw EVEMonWebException.RequestsDisabledException(url);
            _baseUrl = url;
            _url = url;
            _responseStream = responseStream;
            _accept = accept;
            _postData = postData;
            HttpWebResponse webResponse = null;
            Stream webResponseStream = null;
            try
            {
                webResponse = GetHttpResponse();
                webResponseStream = webResponse.GetResponseStream();
                int bytesRead;
                long totalBytesRead = 0;
                long rawBufferSize = webResponse.ContentLength / 100;
                int bufferSize = (int)(rawBufferSize > _webClientState.MaxBufferSize  ? _webClientState.MaxBufferSize : (rawBufferSize < _webClientState.MinBufferSize ? _webClientState.MinBufferSize : rawBufferSize));
                do
                {
                    byte[] buffer = new byte[bufferSize];
                    bytesRead = webResponseStream.Read(buffer, 0, bufferSize);
                    if (bytesRead > 0)
                    {
                        _responseStream.Write(buffer, 0, bytesRead);
                        if (_asyncState != null && _asyncState.ProgressCallback != null)
                        {
                            totalBytesRead += bytesRead;
                            int progressPercentage = webResponse.ContentLength == 0 ? 0 : (int)(totalBytesRead/webResponse.ContentLength);
                            _asyncState.ProgressCallback(new DownloadProgressChangedArgs(webResponse.ContentLength, totalBytesRead, progressPercentage));
                        }
                    }
                } while (bytesRead > 0 && !Cancelled);
            }
            catch (EVEMonWebException)
            {
                throw;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.ProxyAuthenticationRequired && _webClientState.DisableOnProxyAuthenticationFailure)
                {
                    _webClientState.RequestsDisabled = true;
                }
                throw EVEMonWebException.WebException(BaseUrl, _webClientState, ex);
            }
            catch (Exception ex)
            {
                throw EVEMonWebException.Exception(url, ex);
            }
            finally
            {
                if (webResponseStream != null) webResponseStream.Close();
                if (webResponse != null) webResponse.Close();
            }
        }

        /// <summary>
        /// Asynchronously retrieve the response from the requested url to the specified response stream
        /// </summary>
        public void GetResponseAsync(string url, Stream responseStream, string accept, HttpPostData postData, WebRequestAsyncState state)
        {
            _asyncState = state;
            _asyncState.Request = this;
            GetResponseDelegate caller = GetResponse;
            caller.BeginInvoke(url, responseStream, accept, postData, GetResponseAsyncCompleted, caller);
        }

        /// <summary>
        /// Callback method for asynchronous requests
        /// </summary>
        private void GetResponseAsyncCompleted(IAsyncResult ar)
        {
            GetResponseDelegate caller = (GetResponseDelegate) ar.AsyncState;
            try
            {
                caller.EndInvoke(ar);
            }
            catch(EVEMonWebException ex)
            {
                _asyncState.Error = ex;
            }
            _asyncState.Callback(_asyncState);
        }

        /// <summary>
        /// Get the HttpWebResponse for the specified URL
        /// </summary>
        private HttpWebResponse GetHttpResponse()
        {
            HttpWebRequest request = GetHttpWebRequest(_url, _referer);
            if (request.Method == "POST")
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(_postData.Content, 0, _postData.Length);
                requestStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                string target = response.GetResponseHeader("Location");
                response.Close();
                response = GetRedirectedHttpResponse(target);
            }
            return response;
        }

        /// <summary>
        /// Gets a redirected HttpWebResponse
        /// </summary>
        private HttpWebResponse GetRedirectedHttpResponse(string target)
        {
            if (_redirectsRemaining-- > 0)
            {
                Uri referer = new Uri(_url);
                _referer = referer.ToString();
                _url = new Uri(referer, target).ToString();
                return GetHttpResponse();
            }
            else
            {
                throw EVEMonWebException.RedirectsExceededException(BaseUrl);
            }
        }

        /// <summary>
        /// Constructs an HttpWebRequest for the specified url and referer
        /// </summary>
        public HttpWebRequest GetHttpWebRequest(string url, string referer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = false;
            request.Headers[HttpRequestHeader.AcceptLanguage] = "en-us,en;q=0.5";
            request.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            request.Headers[HttpRequestHeader.Pragma] = "no-cache";
            request.KeepAlive = true;
            request.UserAgent = _webClientState.UserAgent;
            request.Accept = _accept;
            if (referer != null) request.Referer = referer;
            if (_postData != null)
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = _postData.Length;
            }
            if (_webClientState.UseCustomProxy)
            {
                WebProxy proxy = new WebProxy(_webClientState.Proxy.Host, _webClientState.Proxy.Port);
                switch (_webClientState.Proxy.AuthType)
                {
                    case ProxyAuthType.None:
                        proxy.UseDefaultCredentials = false;
                        proxy.Credentials = null;
                        break;
                    case ProxyAuthType.SystemDefault:
                        proxy.UseDefaultCredentials = true;
                        break;
                    case ProxyAuthType.Specified:
                        proxy.UseDefaultCredentials = false;
                        proxy.Credentials = new NetworkCredential(_webClientState.Proxy.Username, _webClientState.Proxy.Password);
                        break;
                }
                request.Proxy = proxy;
            }
            return request;
        }
    }
}
