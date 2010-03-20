using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace EVEMon.Common
{
    public static class TimeExtensions
    {
        /// <summary>
        /// Returns a string representation for the time left to the given date, using the following formats : 
        /// <list type="bullet">
        /// <item>1d3h5m6s</item>
        /// <item>3h5m</item>
        /// <item>Done</item>
        /// </list>
        /// </summary>
        /// <param name="t">The time (in the future) for which you want a span.</param>
        /// <returns></returns>
        public static string ToRemainingTimeShortDescription(this DateTime t)
        {
            StringBuilder sb = new StringBuilder();
            if (t > DateTime.Now)
            {
                TimeSpan ts = t.ToUniversalTime() - DateTime.Now.ToUniversalTime();
                if (ts.Days > 0)
                {
                    sb.Append(ts.Days.ToString());
                    sb.Append("d ");
                }
                ts -= TimeSpan.FromDays(ts.Days);
                if (ts.Hours > 0)
                {
                    sb.Append(ts.Hours.ToString());
                    sb.Append("h ");
                }
                ts -= TimeSpan.FromHours(ts.Hours);
                if (ts.Minutes > 0)
                {
                    sb.Append(ts.Minutes.ToString());
                    sb.Append("m ");
                }
                ts -= TimeSpan.FromMinutes(ts.Minutes);
                if (ts.Seconds > 0)
                {
                    sb.Append(ts.Seconds.ToString());
                    sb.Append("s");
                }
                return sb.ToString();
            }
            else
            {
                return "Done";
            }
        }

        /// <summary>
        /// Returns a string representation for the time left to the given date, using the following formats : 
        /// <list type="bullet">
        /// <item>2 days, 3 hours, 1 minute, 5seconds</item>
        /// <item>3 hours, 1 minute</item>
        /// <item>Completed</item>
        /// </list>
        /// </summary>
        /// <param name="t">The time (in the future) for which you want a span.</param>
        /// <returns></returns>
        public static string ToRemainingTimeDescription(this DateTime t)
        {
            StringBuilder sb = new StringBuilder();
            if (t > DateTime.UtcNow)
            {
                TimeSpan ts = t.ToUniversalTime() - DateTime.Now.ToUniversalTime();
                if (ts.Days > 0)
                {
                    sb.Append(ts.Days.ToString());
                    sb.Append(" day");
                    if (ts.Days > 1)
                    {
                        sb.Append("s");
                    }
                }
                ts -= TimeSpan.FromDays(ts.Days);
                if (ts.Hours > 0)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ts.Hours.ToString());
                    sb.Append(" hour");
                    if (ts.Hours > 1)
                    {
                        sb.Append("s");
                    }
                }
                ts -= TimeSpan.FromHours(ts.Hours);
                if (ts.Minutes > 0)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ts.Minutes.ToString());
                    sb.Append(" minute");
                    if (ts.Minutes > 1)
                    {
                        sb.Append("s");
                    }
                }
                ts -= TimeSpan.FromMinutes(ts.Minutes);
                if (ts.Seconds > 0)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ts.Seconds.ToString());
                    sb.Append(" second");
                    if (ts.Seconds > 1)
                    {
                        sb.Append("s");
                    }
                }
                return sb.ToString();
            }
            else
            {
                return "Completed";
            }
        }

        /// <summary>
        /// Generates an absoloute string based upon the following format
        /// <list type="bullet">
        /// <item>17:27 Tomorrow</item>
        /// <item>07:43 Wednesday</item>
        /// <item>03:12 23/04/2009</item>
        /// </list>
        /// <param name="absolouteDateTime">A DateTime to get a string representation for</param>
        /// <returns>String representation of the time and relative date</returns>
        public static string ToAbsoluteDateTimeDescription(this DateTime absoluteDateTime)
        {
            string shortTime = GetShortTimeString(absoluteDateTime);

            // Yesterday (i.e. before 00:00 today)
            if (absoluteDateTime.Date == DateTime.Now.Date.AddDays(-1))
            {
                return String.Format(CultureConstants.DefaultCulture, "{0} Yesterday", shortTime);
            }

            // Today (i.e. before 00:00 tomorrow)
            if (absoluteDateTime.Date == DateTime.Now.Date)
            {
                return String.Format(CultureConstants.DefaultCulture, "{0} Today", shortTime);
            }

            // Tomorrow (i.e. after 23:59 today but before 00:00 the day after tomorrow)
            DateTime tomorrow = DateTime.Now.Date.AddDays(1);
            if (absoluteDateTime.Date == tomorrow)
            {
                return String.Format(CultureConstants.DefaultCulture, "{0} Tomorrow", shortTime);
            }

            // After tomorrow but within 7 days
            DateTime sevenDays = DateTime.Now.Date.AddDays(7);
            if (absoluteDateTime.Date > tomorrow)
            {
                string dayOfWeek = absoluteDateTime.DayOfWeek.ToString();
                if (absoluteDateTime.Date < sevenDays)
                {
                    return String.Format(CultureConstants.DefaultCulture, "{0} This {1}", shortTime, dayOfWeek);
                }
                if (absoluteDateTime.Date == sevenDays)
                {
                    return String.Format(CultureConstants.DefaultCulture, "{0} Next {1}", shortTime, dayOfWeek);
                }
            }

            // More than seven days away or more than one day ago
            string shortDate = absoluteDateTime.ToString("d", CultureConstants.DefaultCulture);
            return String.Format(CultureConstants.DefaultCulture, "{0} {1}", shortTime, shortDate);
        }

        /// <summary>
        /// Formats a DateTime object as a short string (HH:mm or hh:mm tt)
        /// </summary>
        /// <remarks>
        /// Due to a reason that escapes me, the ShortTimePattern
        /// function doesn't respect system settings, so we have to
        /// mangle LongTimePattern to track users preference.
        /// </remarks>
        /// <param name="ShortTimeString">DateTime to be formatted</param>
        /// <returns>String containing formatted text</returns>
        public static string GetShortTimeString(DateTime shortTimeString)
        {
            DateTimeFormatInfo dateTimeFormat = CultureConstants.DefaultCulture.DateTimeFormat;
            
            // Get the LongTimePattern and remove :ss and :s
            string shortTimePattern = dateTimeFormat.LongTimePattern.Replace(":ss", String.Empty);
            shortTimePattern = shortTimePattern.Replace(":s", String.Empty);

            return shortTimeString.ToString(shortTimePattern);
        }
    }
}
