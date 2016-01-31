using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Net
{
    static partial class HttpWebClientService
    {
        private const string StringAccept = "text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Synchronously downloads a string from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        /// <returns></returns>
        public static String DownloadString(Uri url, HttpMethod method = null, bool acceptEncoded = false,
            string postdata = null, DataCompression dataCompression = DataCompression.None)
            => DownloadStringAsync(url, method, acceptEncoded, postdata, dataCompression).Result.Result;


        /// <summary>
        /// Asynchronously downloads a string from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        public static async Task<DownloadAsyncResult<String>> DownloadStringAsync(Uri url, HttpMethod method = null, bool acceptEncoded = false,
            string postdata = null, DataCompression dataCompression = DataCompression.None)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = String.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, dataCompression);
            HttpClientServiceRequest request = new HttpClientServiceRequest();
            try
            {
                HttpResponseMessage response =
                    await request.SendAsync(url, method, postData, dataCompression, acceptEncoded, StringAccept).ConfigureAwait(false);

                using (response)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    return GetString(request, stream);
                }
            }
            catch (HttpWebClientServiceException ex)
            {
                return new DownloadAsyncResult<String>(String.Empty, ex);
            }
        }

        /// <summary>
        /// Helper method to return a string from the completed request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private static DownloadAsyncResult<String> GetString(HttpClientServiceRequest request, Stream stream)
        {
            String text = String.Empty;
            HttpWebClientServiceException error = null;

            if (stream == null)
            {
                error = HttpWebClientServiceException.Exception(request.BaseUrl, new ArgumentNullException("stream"));
                return new DownloadAsyncResult<String>(text, error);
            }

            try
            {
                using (StreamReader reader = new StreamReader(Util.ZlibUncompress(stream)))
                    text = reader.ReadToEnd();
            }
            catch (ArgumentException ex)
            {
                error = HttpWebClientServiceException.Exception(request.BaseUrl, ex);
            }

            return new DownloadAsyncResult<String>(text, error);
        }
    }
}
