using EVEMon.Common.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace EVEMon.Common.Net
{
    static partial class HttpWebClientService
    {
        private const string StreamAccept = "application/json;q=0.9,text/plain;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Passed to DownloadStreamAsync to process the stream into actual data.
        /// </summary>
        /// <typeparam name="T">The type to decode.</typeparam>
        /// <param name="stream">The input stream to read.</param>
        /// <param name="responseCode">The HTTP response code returned by the server.</param>
        /// <returns>The decoded value, or default(T) if none could be parsed.</returns>
        public delegate T ParseDataDelegate<T>(Stream stream, int responseCode) where T : class;

        /// <summary>
        /// Asynchronously downloads an object (streaming) from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postData">The post data. If null, GET will be used.</param>
        /// <param name="parser">The function which will parse the stream.</param>
        /// <param name="token">The ESI token, or null if none is used.</param>
        public static async Task<DownloadResult<T>> DownloadStreamAsync<T>(Uri url,
            ParseDataDelegate<T> parser, bool acceptEncoded = false,
            HttpPostData postData = null, string token = null) where T : class
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpClientServiceRequest request = new HttpClientServiceRequest();
            request.AuthToken = token;
            request.AcceptEncoded = acceptEncoded;
            request.DataCompression = postData.Compression;
            int code = 0;
            try
            {
                var response = await request.SendAsync(url, (postData == null) ?
                    HttpMethod.Get : HttpMethod.Post, postData, StreamAccept).
                    ConfigureAwait(false);
                code = (int)response.StatusCode;

                using (response)
                {
                    var stream = await response.Content.ReadAsStreamAsync().
                        ConfigureAwait(false);
                    return GetResult(url, stream, parser, code);
                }
            }
            catch (HttpWebClientServiceException ex)
            {
                return new DownloadResult<T>(default(T), ex, code);
            }
        }

        /// <summary>
        /// Helper method to return an object from the completed request stream.
        /// </summary>
        /// <param name="requestBaseUrl">The request base URL.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="parser">The function which will parse the stream.</param>
        /// <param name="responseCode">The response HTTP code from the server.</param>
        /// <returns>The parsed object.</returns>
        private static DownloadResult<T> GetResult<T>(Uri requestBaseUrl, Stream stream,
            ParseDataDelegate<T> parser, int responseCode) where T : class
        {
            T result = default(T);
            HttpWebClientServiceException error = null;

            if (stream == null)
            {
                error = HttpWebClientServiceException.Exception(requestBaseUrl,
                    new ArgumentNullException(nameof(stream)));
                return new DownloadResult<T>(result, error, responseCode);
            }

            try
            {
                result = parser.Invoke(Util.ZlibUncompress(stream), responseCode);
            }
            catch (HttpWebClientServiceException)
            {
                // Pass this one on
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                // Wrap other exceptions in a safe exception
                error = HttpWebClientServiceException.Exception(requestBaseUrl, ex);
            }

            return new DownloadResult<T>(result, error, responseCode);
        }
    }
}
