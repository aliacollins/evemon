using System.Xml.XPath;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class to return the result of an asynchronous xml download
    /// </summary>
    public class DownloadXmlAsyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadXmlAsyncResult"/> class.
        /// </summary>
        /// <param name="xdoc">The xdoc.</param>
        /// <param name="error">The error.</param>
        public DownloadXmlAsyncResult(IXPathNavigable xdoc, HttpWebServiceException error)
        {
            Error = error;
            Result = xdoc;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public IXPathNavigable Result { get; private set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public HttpWebServiceException Error { get; private set; }
    }
}