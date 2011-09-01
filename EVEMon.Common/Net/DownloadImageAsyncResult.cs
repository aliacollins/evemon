using System.Drawing;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class to return the result of an asynchronous image download
    /// </summary>
    public class DownloadImageAsyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadImageAsyncResult"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="error">The error.</param>
        public DownloadImageAsyncResult(Image image, HttpWebServiceException error)
        {
            Error = error;
            Result = image;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public Image Result { get; private set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public HttpWebServiceException Error { get; private set; }
    }
}