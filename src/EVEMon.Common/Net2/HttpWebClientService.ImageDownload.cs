using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Net;

namespace EVEMon.Common.Net2
{
    static partial class HttpWebClientService
    {
        private const string ImageAccept = "image/*";

        /// <summary>
        /// Synchronously downloads an image from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        public static DownloadImageAsyncResult DownloadImage(Uri url, System.Net.Http.HttpMethod method = null,
            bool acceptEncoded = false, string postdata = null, DataCompression dataCompression = DataCompression.None)
            => Task.Run(
                async () => await DownloadImageAsync(url, method, acceptEncoded, postdata, dataCompression))
                .Result;

        /// <summary>
        /// Asynchronously downloads an image from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        public static async Task<DownloadImageAsyncResult> DownloadImageAsync(Uri url, System.Net.Http.HttpMethod method = null,
            bool acceptEncoded = false, string postdata = null, DataCompression dataCompression = DataCompression.None)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, dataCompression);
            HttpClientServiceRequest request = new HttpClientServiceRequest();
            HttpResponseMessage response =
                await request.SendAsync(url, method, postData, dataCompression, acceptEncoded, ImageAccept);
            Stream stream = await response.Content.ReadAsStreamAsync();
            return GetImage(request, stream);
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="stream">The stream.</param>
        private static DownloadImageAsyncResult GetImage(HttpClientServiceRequest request, Stream stream)
        {
            Image image = null;
            HttpWebServiceException error = null;
            try
            {
                image = Image.FromStream(Util.ZlibUncompress(stream), true);
            }
            catch (ArgumentException ex)
            {
                error = HttpWebServiceException.ImageException(request.BaseUrl, ex);
            }

            return new DownloadImageAsyncResult(image, error);
        }
    }
}
