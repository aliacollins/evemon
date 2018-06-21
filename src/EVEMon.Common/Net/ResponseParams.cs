using System;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Groups together response parameters from HTTP requests. Included are the status code,
    /// ETag (if present), expiration date (if present), and encoding.
    /// </summary>
    public sealed class ResponseParams
    {
        /// <summary>
        /// The error count reported from ESI to avoid running into backoff.
        /// </summary>
        public int ErrorCount { get; set; }
        /// <summary>
        /// The E-Tag received from the server. Null if no e-tag was sent.
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// The time when this data expires. Null if no expiry was sent.
        /// </summary>
        public DateTimeOffset? Expires { get; set; }
        /// <summary>
        /// The response code from the server.
        /// </summary>
        public int ResponseCode { get; }

        public ResponseParams(int responseCode)
        {
            ErrorCount = 0;
            ETag = null;
            Expires = null;
            ResponseCode = responseCode;
        }
        public override string ToString()
        {
            return string.Format("ResponseParams[code={0:D},expires={1}]", ResponseCode,
                Expires);
        }
    }
}
