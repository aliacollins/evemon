using System;
using System.IO;
using System.Xml;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// EVEMonWebClient Xml download implementation
    /// </summary>
    partial class EVEMonWebClient
    {
        private const string XML_ACCEPT =
            "text/xml,application/xml,application/xhtml+xml;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Downloads an Xml document from the specified url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public XmlDocument DownloadXml(string url)
        {
            return DownloadXml(url, null);
        }

        /// <summary>
        /// Downloads an Xml document from the specified url using the specified POST data
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public XmlDocument DownloadXml(string url, HttpPostData postData)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);
            EVEMonWebRequest request = GetRequest();
            try
            {
                request.GetResponse(url, new MemoryStream(), XML_ACCEPT, postData);
                XmlDocument result = new XmlDocument();
                if (request.ResponseStream != null)
                {
                    request.ResponseStream.Seek(0, SeekOrigin.Begin);
                    result.Load(request.ResponseStream);
                }
                return result;
            }
            catch (XmlException ex)
            {
                throw EVEMonWebException.XmlException(url, ex);
            }
            finally
            {
                if (request.ResponseStream != null) request.ResponseStream.Close();
            }
        }
    }
}
