using System;
using System.IO;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// EVEMonWebClient String download implementation
    /// </summary>
    public partial class EVEMonWebClient
    {
        private const string STRING_ACCEPT =
            "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Synchronously downloads a string from the specified url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public String DownloadString(string url)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);
            EVEMonWebRequest request = GetRequest();
            try
            {
                request.GetResponse(url, new MemoryStream(), STRING_ACCEPT);
                string result = String.Empty;
                if (request.ResponseStream != null)
                {
                    request.ResponseStream.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(request.ResponseStream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                return result;
            }
            finally
            {
                if (request.ResponseStream != null) request.ResponseStream.Close();
            }
        }
    }
}
