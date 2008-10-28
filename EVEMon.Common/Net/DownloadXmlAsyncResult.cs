using System.Xml;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class to return the result of an asynchronous xml download
    /// </summary>
    public class DownloadXmlAsyncResult
    {
        private readonly XmlDocument _result;
        private readonly EVEMonWebException _error;

        public DownloadXmlAsyncResult(XmlDocument xdoc, EVEMonWebException error)
        {
            _error = error;
            _result = xdoc;
        }

        public XmlDocument Result
        {
            get { return _result; }
        }

        public EVEMonWebException Error
        {
            get { return _error; }
        }

    }
}
