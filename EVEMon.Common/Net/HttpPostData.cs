using System.Text;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class for data to be submitted to a url as a POST request
    /// </summary>
    public sealed class HttpPostData
    {
        private readonly string m_data;

        public HttpPostData(string data)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Content = encoding.GetBytes(data);
            m_data = data;
        }

        /// <summary>
        /// Gets the content's bytes
        /// </summary>
        public byte[] Content { get; private set; }

        /// <summary>
        /// Gets the number of bytes of the content
        /// </summary>
        public int Length
        {
            get { return Content.Length; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the post data.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the post data.
        /// </returns>
        public override string ToString()
        {
            return m_data;
        }
    }
}