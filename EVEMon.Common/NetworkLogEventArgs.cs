using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    public class NetworkLogEventArgs : EventArgs
    {
        internal NetworkLogEventArgs()
        {
        }

        private NetworkLogEventType m_type;

        public NetworkLogEventType NetworkLogEventType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private string m_url;

        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }

        private string m_referer;

        public string Referer
        {
            get { return m_referer; }
            set { m_referer = value; }
        }

        private CookieCollection m_cookies;

        [XmlIgnore]
        public CookieCollection Cookies
        {
            get { return m_cookies; }
            set { m_cookies = value; }
        }

        public List<string> CookieList
        {
            get
            {
                if (m_cookies == null)
                {
                    return null;
                }
                List<string> cook = new List<string>();
                foreach (Cookie c in m_cookies)
                {
                    cook.Add(c.ToString());
                }
                return cook;
            }
        }

        private Exception m_exception;

        [XmlIgnore]
        public Exception Exception
        {
            get { return m_exception; }
            set { m_exception = value; }
        }

        public string ExceptionText
        {
            get
            {
                if (m_exception != null)
                {
                    return m_exception.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        private string m_redirectTo;

        public string RedirectTo
        {
            get { return m_redirectTo; }
            set { m_redirectTo = value; }
        }

        private HttpStatusCode m_statusCode = HttpStatusCode.OK;

        public HttpStatusCode StatusCode
        {
            get { return m_statusCode; }
            set { m_statusCode = value; }
        }

        private string m_data;

        public string Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
    }
}