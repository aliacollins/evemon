using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Text;

namespace EVEMon.Common
{

    /// <summary>
    /// EVEMonWebRequest should be used for any HTTP requests that EVEMon needs to make.
    /// </sumary>
    ///<remarks>
    /// The class contains some toplevel  staic methods to retrieve various types of data from a url. Genrally, only these 3 methods should
    /// be used by other classes that require data from the web.
    ///
    /// LoadXml()       gets an XmlDocument from a URL
    /// GetUrlImage()   gets an Image object from a URL
    /// GetUrlString()  gets the HTTP reponse from a URL as a String
    ///
    /// These 3 methods all use :
    ///
    /// GetUrlSteam()   The core method that reads the raw response from an HTTP request as a Stream object and is used by all the methods above.
    /// This method uses GetWebRequest.
    ///
    /// GetWebRequest() creates a new HttpWebRequest object that is configured with the EVEMon user agent string and the user's proxy setting.
    /// ANY  HttpWebRequest object required by EVEMon MUST be obtained using this method.
    ///
    /// WebRequestState is a helper class used by the methods in EVEMonWebRequest to configure the HttpWebRequest object in the GetWebRequest() method.
    ///</remarks>
    
    public static class EVEMonWebRequest
    {
        
        public static XmlDocument LoadXml(string url)
        {
            WebRequestState wrs = new WebRequestState();
            return LoadXml(url, wrs);
        }

        public static XmlDocument LoadXml(string url, WebRequestState wrs)
        {
            wrs.Accept = "text/xml,application/xml,application/xhtml+xml;q=0.8,*/*;q=0.5";
            Stream s = GetUrlStream(url, wrs);
            try
            {
                if (s != null)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(s);
                    return xdoc;
                }
                else
                {
                    throw new EVEMonNetworkException(wrs.RequestError);
                }
            }
            finally
            {
                if (s != null)
                {
                    s.Dispose();
                }
            }
        }

        public static Stream GetUrlStream(string url)
        {
            HttpWebResponse junk;
            WebRequestState wrs = new WebRequestState();
            Stream result = GetUrlStream(url, wrs, out junk);
            if (result == null)
            {
                throw new EVEMonNetworkException(wrs.RequestError);
            }
            return result;
        }

        public static Stream GetUrlStream(string url, WebRequestState wrs)
        {
            HttpWebResponse junk;
            return GetUrlStream(url, wrs, out junk);
        }

        // network logging
        public static event EventHandler<NetworkLogEventArgs> NetworkLogEvent;

        // Helper variables for the proxy misconfiguartion detetction
        private static bool _blockingRequests = false;
        private static Settings _settings;
        private static ProxySetting _lastProxySettings = null;
        private static Object _blockLock = new Object();
        
        /// <summary>
        /// The fundamental method for recieving HTTP data. ALL HTTP web requests that EVEMon makes
        /// must come through this method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="wrs"></param>
        /// <param name="resp"></param>
        /// <returns></returns>
        public static Stream GetUrlStream(string url, WebRequestState wrs, out HttpWebResponse resp)
        {
            // have we detected a potential proxy misconfiguration?
            lock (_blockLock)
            {
                if (_blockingRequests) 
                {
                    // yes, user has requested a block - see if they've changed proxy credentials
                    if (_settings.HttpProxy.Username == _lastProxySettings.Username && _settings.HttpProxy.Password == _lastProxySettings.Password)
                    {
                        resp = null;
                        return null;
                    }
                    
                    // credentials have changed, let's try again
                     _lastProxySettings = _settings.HttpProxy;
                     _blockingRequests = false;
                }
            }

            NetworkLogEventArgs args = new NetworkLogEventArgs();
             
            if (NetworkLogEvent != null)
            {
                args.NetworkLogEventType = NetworkLogEventType.BeginGetUrl;
                args.Url = url;
                args.Referer = wrs.Referer;
            }


            while (--wrs.RedirectsRemain > 0)
            {
                if (NetworkLogEvent != null)
                {
                    NetworkLogEvent(null, args);
                }

#if DEBUG
                Console.WriteLine("Debug:  Redirects Remaining: {0}  url {1}", wrs.RedirectsRemain, url);
#endif
                HttpWebRequest req = GetWebRequest(url, wrs);
                try
                {
                    if (req.Method == "POST")
                    {
                        Stream reqStream = req.GetRequestStream();
                        reqStream.Write(wrs.PostData, 0, wrs.PostData.Length);
                        reqStream.Close();
                    }
                    resp = (HttpWebResponse) req.GetResponse();
                }
                catch (WebException ex)
                {
                    // The request failed. If this has failed because of a custom proxy misconfiguration,
                    // then let's give the user the opportunity to block requests so they don't potentially lock
                    // their whole internet account! (see trac ticket 503)
                    ExceptionHandler.LogException(ex, true);
                    wrs.RequestError = RequestError.WebException;
                    wrs.WebException = ex;
                    resp = null;

                    if (NetworkLogEvent != null)
                    {
                        args.NetworkLogEventType = NetworkLogEventType.GotUrlFailure;
                        args.StatusCode = resp.StatusCode;
                        args.RedirectTo = null;
                        args.Exception= ex;
                        NetworkLogEvent(null, args);
                    }

                    lock (_blockLock)
                    {
                        // check the flag again in case another thread has set the flag
                        if (!_blockingRequests)
                        {
                            // check if we are using proxy authentication - user might have changed his proxy password
                            // and not updated EVEMon
                            if (_settings == null) _settings = Settings.GetInstance();
                            if (_settings.UseCustomProxySettings && _settings.HttpProxy.Username != String.Empty)
                            {
                                // User has a custom authenticating proxy  - check the response status to see if it
                                // could be a result of an incorrect proxy setting
                                if (ex.Status == WebExceptionStatus.ProtocolError ||
                                    ex.Status == WebExceptionStatus.RequestProhibitedByProxy ||
                                    ex.Status == WebExceptionStatus.UnknownError)
                                {
                                    // Houston, we have a problem! Strip off parameters so we don't reveal passwords etc.
                                    string host = url;
                                    int paramPos = url.IndexOf("?");
                                    if (paramPos > 0)
                                        host = url.Substring(0, paramPos);

                                    System.Windows.Forms.DialogResult answer = System.Windows.Forms.MessageBox.Show("EVEMon made a request to " + host + " which failed with the message:\n\n" +
                                                                        ex.Message + ".\n\nThis may indicate that your proxy settings are incorrect.\n\n" +
                                                                        "It is recommended that you block further requests until you have checked your proxy authentication setings in EVEMon.\n" +
                                                                        "If you do not do this, you could lock your internet account!\n\n" +
                                                                        "Do you wish to temporarily block further traffic until you restart EVEMon or change your authentication settings?",
                                                                        "Possible proxy misconfiguration", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning);
                                    if (answer == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        _blockingRequests = true;
                                        _lastProxySettings = _settings.HttpProxy;
                                    }
                                }
                            }
                        }
                    }
                    return null;
                }
                if (resp.StatusCode == HttpStatusCode.Redirect && wrs.AllowRedirects)
                {

                    string loc = resp.GetResponseHeader("Location");
                    Uri x = new Uri(url);
                    Uri newUri = new Uri(x, loc);

                    wrs.Referer = url;
                    url = newUri.ToString();
                    resp.Close();
                    if (NetworkLogEvent != null)
                    {
                        args.NetworkLogEventType = NetworkLogEventType.ParsedRedirect;
                        args.StatusCode = resp.StatusCode;
                        args.RedirectTo = url;
                        NetworkLogEvent(null, args);
                        args.NetworkLogEventType = NetworkLogEventType.Redirected;
                    }

                    continue; // Continue the loop to try and load the redirected content
                }

                if (NetworkLogEvent != null)
                {
                    args.NetworkLogEventType = NetworkLogEventType.GotUrlSuccess;
                    args.StatusCode = resp.StatusCode;
                    args.RedirectTo = null;
                    NetworkLogEvent(null, args);
                }

                AutoShrink.Dirty();
                return resp.GetResponseStream();

            }
            //did not get an answer before running out of redirects.
            wrs.RedirectsRemain = 0;
            wrs.RequestError = RequestError.MaxRedirectsExceeded;
            resp = null;

            AutoShrink.Dirty();
            return null;
        }

        public static Image GetUrlImage(string url)
        {
            WebRequestState wrs = new WebRequestState();
            wrs.Accept = "image/*;q=0.8,*/*;q=0.5";
            Image result = GetUrlImage(url, wrs);
            if (result == null)
            {
                throw new EVEMonNetworkException(wrs.RequestError);
            }
            return result;
        }

        public static Image GetUrlImage(string url, WebRequestState wrs)
        {
            Stream s = GetUrlStream(url, wrs);
            try
            {
                if (s == null)
                {
                    return null;
                }
                return Image.FromStream(s, true, true);
            }
            catch (WebException ex)
            {
                ExceptionHandler.LogException(ex, true);
                wrs.RequestError = RequestError.WebException;
                wrs.WebException = ex;
                return null;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogRethrowException(ex);
                if (s != null)
                {
                    s.Dispose();
                }
                throw;
            }
        }

        public static string GetUrlString(string url)
        {
            WebRequestState wrs = new WebRequestState();
            string result = GetUrlString(url, wrs);
            if (result == null)
            {
                throw new EVEMonNetworkException(wrs.RequestError);
            }
            return result;
        }

        public static string GetUrlString(string url, WebRequestState wrs)
        {
            HttpWebResponse junk;
            return GetUrlString(url, wrs, out junk);
        }

        public static string GetUrlString(string url, WebRequestState wrs, out HttpWebResponse resp)
        {
            Stream s = GetUrlStream(url, wrs, out resp);
            try
            {
                if (s == null)
                {
                    return null;
                }
                using (StreamReader sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                ExceptionHandler.LogException(ex, true);
                if (s != null)
                {
                    s.Dispose();
                }
                wrs.RequestError = RequestError.WebException;
                wrs.WebException = ex;
                resp = null;
                return null;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogRethrowException(e);
                if (s != null)
                {
                    s.Dispose();
                }
                throw;
            }
        }

        public static HttpWebRequest GetWebRequest(string url)
        {
            return GetWebRequest(url, new WebRequestState());
        }

        public static HttpWebRequest GetWebRequest(string url, WebRequestState wrs)
        {
            Settings s = Settings.GetInstance();

            HttpWebRequest wr = WebRequest.Create(url) as HttpWebRequest;
            wr.AllowAutoRedirect = false;
            if (wrs.Referer != String.Empty)
                wr.Referer = wrs.Referer;
            wr.UserAgent = wrs.UserAgent;
            wr.CookieContainer = wrs.CookieContainer;
            wr.Accept = wrs.Accept;
            wr.Headers[HttpRequestHeader.AcceptLanguage] = "en-us,en;q=0.5";
            wr.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            wr.Headers[HttpRequestHeader.Pragma] = "no-cache";
            wr.KeepAlive = true;
            if (wrs.Method == "POST")
            {
                wr.Method = wrs.Method;
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.ContentLength = wrs.PostData.Length;


            }
            if (s.UseCustomProxySettings)
            {
                WebProxy prox = new WebProxy(s.HttpProxy.Host, s.HttpProxy.Port);
                switch (s.HttpProxy.AuthType)
                {
                    case ProxyAuthType.None:
                        prox.UseDefaultCredentials = false;
                        prox.Credentials = null;
                        break;
                    case ProxyAuthType.SystemDefault:
                        prox.UseDefaultCredentials = true;
                        break;
                    case ProxyAuthType.Specified:
                        prox.UseDefaultCredentials = false;
                        prox.Credentials = new NetworkCredential(s.HttpProxy.Username, s.HttpProxy.Password);
                        break;
                }
                wr.Proxy = prox;
            }

            return wr;
        }
    }

    public class WebRequestState
    {
        static WebRequestState()
        {
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            sm_appVersion = "EVEMon/" + currentVersion.ToString();
            sm_userAgent = USER_AGENT_BASE + " " + sm_appVersion;
        }

        public WebRequestState()
        {
            m_userAgent = sm_userAgent;
        }

        private static string sm_appVersion;
        private static string sm_userAgent;

        private const string USER_AGENT_BASE = "";
            //"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.0.1) Gecko/20060111 Firefox/1.5.0.1";

        private string m_userAgent;
        private string m_referer = string.Empty;

        public string Referer
        {
            get { return m_referer; }
            set { m_referer = value; }
        }

        public string UserAgent
        {
            get { return m_userAgent; }
            set { m_userAgent = value; }
        }

        private CookieContainer m_cookieContainer = null;

        public CookieContainer CookieContainer
        {
            get { return m_cookieContainer; }
            set { m_cookieContainer = value; }
        }

        private string m_accept =
            "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";

        public string Accept
        {
            get { return m_accept; }
            set { m_accept = value; }
        }

        private string m_method = "GET";
        private string m_contentType=String.Empty;
        private byte[] m_data;
        
        public string Method
        {
            get { return m_method; }
        }

        public int ContentLength
        {
            get { return m_data.Length; }
        }

        public byte[] PostData
        {
            get { return m_data; }
        }

        public void SetPost(string postData)
        {
            m_method = "POST";
            ASCIIEncoding encoding = new ASCIIEncoding();
            m_data = encoding.GetBytes(postData);
        }

        private EventHandler<NetworkLogEventArgs> m_logDelegate;

        public EventHandler<NetworkLogEventArgs> LogDelegate
        {
            get { return m_logDelegate; }
            set { m_logDelegate = value; }
        }

        private bool m_allowRedirects = true;

        public bool AllowRedirects
        {
            get { return m_allowRedirects; }
            set { m_allowRedirects = value; }
        }

        private const int MAX_REDIRECTS = 6;

        private int m_redirectsRemain = MAX_REDIRECTS;

        public int RedirectsRemain
        {
            get { return m_redirectsRemain; }
            set { m_redirectsRemain = value; }
        }

        private RequestError m_requestError;

        public RequestError RequestError
        {
            get { return m_requestError; }
            set { m_requestError = value; }
        }

        private WebException m_webException;

        public WebException WebException
        {
            get { return m_webException; }
            set { m_webException = value; }
        }
    }

    public enum RequestError
    {
        None,
        MaxRedirectsExceeded,
        WebException
    }

    public class EVEMonNetworkException : ApplicationException
    {
        private RequestError m_requestError;

        public RequestError RequestError
        {
            get { return m_requestError; }
            set { m_requestError = value; }
        }

        private WebException m_webException;

        public WebException WebException
        {
            get { return m_webException; }
            set { m_webException = value; }
        }

        public EVEMonNetworkException(RequestError re)
            : base(re.ToString())
        {
            m_requestError = re;
        }

        public EVEMonNetworkException(RequestError re, WebException exc)
            : base(exc.ToString())
        {
            m_requestError = re;
            m_webException = exc;
        }
    }
}
