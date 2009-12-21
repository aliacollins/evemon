using System.Text;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class for data to be submitted to a url as a POST request
    /// </summary>
    public class HttpPostData
    {
        private readonly byte[] _content;

        public HttpPostData(string data)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            _content = encoding.GetBytes(data);
        }

        public byte[] Content
        {
            get { return _content; }
        }

        public int Length
        {
            get { return _content.Length; }
        }
    }
}
