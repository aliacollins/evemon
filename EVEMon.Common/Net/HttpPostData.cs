using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class for data to be submitted to a url as a POST request.
    /// </summary>
    public sealed class HttpPostData
    {
        private readonly string m_data;

        public HttpPostData(string data)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Content = encoding.GetBytes(data).ToList().AsReadOnly();
            m_data = data;
        }

        /// <summary>
        /// Gets the content's bytes.
        /// </summary>
        public ReadOnlyCollection<byte> Content { get; private set; }

        /// <summary>
        /// Gets the number of bytes of the content.
        /// </summary>
        public int Length
        {
            get { return Content.Count; }
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