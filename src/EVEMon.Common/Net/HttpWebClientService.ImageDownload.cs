using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Net
{
    static partial class HttpWebClientService
    {
        private const string ImageAccept = "image/*,*/*;q=0.5";

        /// <summary>
        /// Synchronously downloads an image from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        public static DownloadAsyncResult<Image> DownloadImage(Uri url, System.Net.Http.HttpMethod method = null,
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
        public static async Task<DownloadAsyncResult<Image>> DownloadImageAsync(Uri url, System.Net.Http.HttpMethod method = null,
            bool acceptEncoded = false, string postdata = null, DataCompression dataCompression = DataCompression.None)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, dataCompression);
            HttpClientServiceRequest request = new HttpClientServiceRequest();
            try
            {
                HttpResponseMessage response =
                    await request.SendAsync(url, method, postData, dataCompression, acceptEncoded, ImageAccept);
                Stream stream = await response.Content.ReadAsStreamAsync();
                return GetImage(request, stream);
            }
            catch (HttpWebClientServiceException ex)
            {
                return new DownloadAsyncResult<Image>(null, ex);
            }
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="stream">The stream.</param>
        private static DownloadAsyncResult<Image> GetImage(HttpClientServiceRequest request, Stream stream)
        {
            Image image = null;
            HttpWebClientServiceException error = null;

            if (stream == null)
            {
                error = HttpWebClientServiceException.Exception(request.BaseUrl, new ArgumentNullException("stream"));
                return new DownloadAsyncResult<Image>(null, error);
            }

            try
            {
                image = Image.FromStream(Util.ZlibUncompress(stream), true);
            }
            catch (ArgumentException ex)
            {
                error = HttpWebClientServiceException.ImageException(request.BaseUrl, ex);
            }

            return new DownloadAsyncResult<Image>(image, error);
        }
    }
}
