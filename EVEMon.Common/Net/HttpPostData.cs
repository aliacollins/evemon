using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<byte> m_content;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPostData"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="dataCompression">The compression.</param>
        public HttpPostData(string data, DataCompression dataCompression = DataCompression.None)
        {
            m_data = data;

            byte[] encoded = Encoding.UTF8.GetBytes(data);
            switch (dataCompression)
            {
                case DataCompression.Gzip:
                    m_content = Util.GZipCompress(encoded);
                    break;
                case DataCompression.Deflate:
                    m_content = Util.DeflateCompress(encoded);
                    break;
                case DataCompression.None:
                    m_content = encoded;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the content's bytes.
        /// </summary>
        public IEnumerable<byte> Content
        {
            get { return m_content; }
        }

        /// <summary>
        /// Gets the number of bytes of the content.
        /// </summary>
        public int Length
        {
            get { return Content.Count(); }
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