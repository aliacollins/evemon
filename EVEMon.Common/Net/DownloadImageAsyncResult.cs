using System.Drawing;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class to return the result of an asynchronous image download
    /// </summary>
    public class DownloadImageAsyncResult
    {
        private readonly Image _result;
        private readonly EVEMonWebException _error;

        public DownloadImageAsyncResult(Image image, EVEMonWebException error)
        {
            _error = error;
            _result = image;
        }

        public Image Result
        {
            get { return _result; }
        }

        public EVEMonWebException Error
        {
            get { return _error; }
        }

    }
}
