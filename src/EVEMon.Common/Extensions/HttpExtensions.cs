using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;

namespace EVEMon.Common.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Retrieves the number of ESI errors remaining, or null if this header is not
        /// included.
        /// </summary>
        /// <param name="headers">The response headers.</param>
        public static int? ErrorCount(this HttpResponseHeaders headers)
        {
            IEnumerable<string> values;
            int? errorsLeft = null;
            // If values are available, try to parse as integer, use the last one
            if (headers.TryGetValues("X-Esi-Error-Limit-Remain", out values))
                foreach (string value in values)
                {
                    int errors;
                    if (!string.IsNullOrEmpty(value) && int.TryParse(value.Trim(), out errors))
                        errorsLeft = errors;
                }
            return errorsLeft;
        }
    }
}
