using System;
using EVEMon.Common;
using EVEMon.Common.Constants;

namespace EVEMon.MarketUnifiedUploader
{
    internal static class Extensions
    {
        /// <summary>
        /// Converts a DateTime format to an ISO 8601 compliant date time utc string.
        /// </summary>
        /// <param name="dateTimeUTC">The date time.</param>
        /// <returns></returns>
        internal static string ToIsoDateTimeUTCString(this DateTime dateTimeUTC)
        {
            return String.Format(CultureConstants.InvariantCulture, "{0}+00:00",
                                 dateTimeUTC.ToString("s", CultureConstants.InvariantCulture));
        }

        /// <summary>
        /// Converts a DateTime format to a universal date time string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        internal static string ToUniversalDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("u", CultureConstants.InvariantCulture).Replace("Z", String.Empty);
        }
    }
}