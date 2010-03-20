using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.Common
{
    public static class TimeUtil
    {
        /// <summary>
        /// Converts a UTC DateTime to the CCP API date/time string
        /// </summary>
        /// <param name="timeUTC"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToCCPTimeString(DateTime timeUTC)
        {
            // timeUTC  = yyyy-mm-dd hh:mm:ss
            string result = String.Format(CultureConstants.DefaultCulture, "{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}",
                                          timeUTC.Year,
                                          timeUTC.Month,
                                          timeUTC.Day,
                                          timeUTC.Hour,
                                          timeUTC.Minute,
                                          timeUTC.Second);
            return result;
        }

        /// <summary>
        /// Converts a CCP API date/time string to a UTC DateTime
        /// </summary>
        /// <param name="timeUTC"></param>
        /// <returns></returns>
        public static DateTime ConvertCCPTimeStringToDateTime(string timeUTC)
        {
            // timeUTC  = yyyy-mm-dd hh:mm:ss
            if (String.IsNullOrEmpty(timeUTC)) return DateTime.MinValue;

            DateTime dt = new DateTime(
                Int32.Parse(timeUTC.Substring(0, 4)),
                Int32.Parse(timeUTC.Substring(5, 2)),
                Int32.Parse(timeUTC.Substring(8, 2)),
                Int32.Parse(timeUTC.Substring(11, 2)),
                Int32.Parse(timeUTC.Substring(14, 2)),
                Int32.Parse(timeUTC.Substring(17, 2)),
                0,
                DateTimeKind.Utc);
            return dt;
        }
    }
}
