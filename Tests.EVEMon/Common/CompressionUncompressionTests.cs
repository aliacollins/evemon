using System.IO;
using System.Linq;
using System.Text;
using EVEMon.Common;
using NUnit.Framework;

namespace Tests.EVEMon.Common
{
    [TestFixture]
    public static class CompressionUncompressionTests
    {
        private static readonly byte[] s_data = Encoding.UTF8.GetBytes("Data to compressed and uncompressed.");


        #region Tests

        /// <summary>
        /// Compress and uncompress using zlib gzip.
        /// </summary>
        [Test]
        public static void GzipCompressAndUncompress()
        {
            var compressed = Util.GZipCompress(s_data);
            var uncompressed = Util.GZipUncompress(compressed.ToArray());

            Assert.AreEqual(uncompressed, s_data);
        }

        /// <summary>
        /// Compress and uncompress using zlib deflate-inflate.
        /// </summary>
        [Test]
        public static void DeflateCompressAndInflateUncompress()
        {
            var compressed = Util.DeflateCompress(s_data);
            var uncompressed = Util.InflateUncompress(compressed.ToArray());

            Assert.AreEqual(uncompressed, s_data);
        }

        /// <summary>
        /// Compress using zlib gzip and uncompress using zlib uncompress automation.
        /// </summary>
        [Test]
        public static void GzipCompressAndZlibUncompress()
        {
            var compressed = Util.GZipCompress(s_data);
            var uncompressed = Util.ZlibUncompress(compressed.ToArray());

            Assert.AreEqual(uncompressed, s_data);
        }

        /// <summary>
        /// Compress using zlib deflate and uncompress using zlib uncompress automation.
        /// </summary>
        [Test]
        public static void DeflateCompressAndZlibUncompress()
        {
            var compressed = Util.DeflateCompress(s_data);
            var uncompressed = Util.ZlibUncompress(compressed.ToArray());

            Assert.AreEqual(uncompressed, s_data);
        }

        /// <summary>
        /// Compress using zlib gzip and uncompress using zlib uncompress automation with stream.
        /// </summary>
        [Test]
        public static void GzipCompressAndZlibUncompressUsingStream()
        {
            var compressed = (byte[])Util.GZipCompress(s_data);
            var uncompressed = (MemoryStream)Util.ZlibUncompress(new MemoryStream(compressed));

            Assert.AreEqual(uncompressed.ToArray(), s_data);
        }

        /// <summary>
        /// Compress using zlib deflate and uncompress using zlib uncompress automation with stream.
        /// </summary>
        [Test]
        public static void DeflateCompressAndZlibUncompressUsingStream()
        {
            var compressed = (byte[])Util.DeflateCompress(s_data);
            var uncompressed = (MemoryStream)Util.ZlibUncompress(new MemoryStream(compressed));

            Assert.AreEqual(uncompressed.ToArray(), s_data);
        }

        #endregion
    }
}
