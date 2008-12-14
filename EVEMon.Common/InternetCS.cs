using System;
using System.Net;

namespace EVEMon.Common
{
    /// <summary>
    /// Class to determine if we have an internet connection
    /// We use a different URL to eve-online in case eve-online is down
    /// The server needs to be a big public server that is up 24/7/365 such as google.com
    /// </summary>
    public class InternetCS
    {
        /// <summary>
        /// Performs a HTTP request to a url (default is http://google.com)
        /// </summary>
        /// <returns>true if internet connection is working properly</returns>
        public static bool IsConnectedToInternet(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine("Connection to google.com failed: " + e.Message);
                }
            }
            return false;
        }
    }
}