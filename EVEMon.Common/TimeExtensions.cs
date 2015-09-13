using System;
using System.Globalization;
using System.Text;

namespace EVEMon.Common
{
    public static class TimeExtensions
    {

        /// <summary>
        /// Converts a Windows timestamp to <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        public static DateTime WinTimeStampToDateTime(this long timestamp)
        {
            return new DateTime(1601, 1, 1).AddTicks(timestamp);
        }

        /// <summary>
        /// Converts a DateTime to the API date/time string.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string DateTimeToTimeString(this DateTime time)
        {
            // 'time' can be any predefined or custom format
            return time.ToString("u", CultureConstants.DefaultCulture.DateTimeFormat).TrimEnd('Z');
        }

        /// <summary>
        /// Converts an API date/time string to a UTC DateTime.
        /// </summary>
        /// <param name="timeUTC"></param>
        /// <returns></returns>
        public static DateTime TimeStringToDateTime(this String timeUTC)
        {
            // timeUTC = yyyy-MM-dd HH:mm:ss
            DateTime dt;
            return DateTime.TryParse(timeUTC, CultureConstants.DefaultCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out dt)
                       ? dt
                       : default(DateTime);
        }

        /// <summary>
        /// Converts a DateTime to a dot formatted date/time string.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        /// <remarks>
        /// String Format: yyyy.MM.dd HH:mm:ss
        /// </remarks>
        public static string DateTimeToDotFormattedString(this DateTime time)
        {
            // time can be any predefined or custom format
            return time.DateTimeToTimeString().Replace("-", ".");
        }

        /// <summary>
        /// Returns a string representation for the time left to the given date, using the following formats : 
        /// <list type="bullet">
        /// <item>1d3h5m6s</item>
        /// <item>3h5m</item>
        /// <item>Done</item>
        /// </list>
        /// </summary>
        /// <param name="t">The time (in the future) for which you want a span.</param>
        /// <param name="dateTimeKind">The kind of the dateTime (UTC or Local) being converted.</param>
        /// <remarks>DateTimeKind.Unspecified will be treated as UTC</remarks>
        /// <returns></returns>
        public static string ToRemainingTimeShortDescription(this DateTime t, DateTimeKind dateTimeKind)
        {
            DateTime now = (dateTimeKind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow);
            StringBuilder sb = new StringBuilder();
            if (t <= now)
                return "Done";
            
            // Fixing the small chance that the method could cross over the
            // second boundary, and have an inconsistent result.
            double factor = Math.Pow(10, 7);
            long roundedTicks = (long)Math.Round(t.Subtract(now).Ticks / factor) * (int)factor;
            TimeSpan ts = new TimeSpan(roundedTicks);

            if (ts.Days > 0)
            {
                sb.Append(ts.Days.ToString(CultureConstants.DefaultCulture));
                sb.Append("d ");
            }
            ts -= TimeSpan.FromDays(ts.Days);
            if (ts.Hours > 0)
            {
                sb.Append(ts.Hours.ToString(CultureConstants.DefaultCulture));
                sb.Append("h ");
            }
            ts -= TimeSpan.FromHours(ts.Hours);
            if (ts.Minutes > 0)
            {
                sb.Append(ts.Minutes.ToString(CultureConstants.DefaultCulture));
                sb.Append("m ");
            }
            ts -= TimeSpan.FromMinutes(ts.Minutes);
            if (ts.Seconds > 0)
            {
                sb.Append(ts.Seconds.ToString(CultureConstants.DefaultCulture));
                sb.Append("s");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string representation for the time left to the given date, using the following formats : 
        /// <list type="bullet">
        /// <item>1D 13:00:05</item>
        /// <item>13:00:05</item>
        /// <item>00:13:05</item>
        /// </list>
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="dateTimeKind">Kind of the date time.</param>
        /// <returns></returns>
        public static string ToRemainingTimeDigitalDescription(this DateTime t, DateTimeKind dateTimeKind)
        {
            DateTime now = (dateTimeKind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow);

            // Small chance that the function could cross over the
            // second boundry, and have an inconsistent result.
            StringBuilder sb = new StringBuilder();
            if (t <= now)
                return String.Empty;

            double factor = Math.Pow(10, 7);
            long roundedTicks = (long)Math.Round(t.Subtract(now).Ticks / factor) * (int)factor;
            TimeSpan ts = new TimeSpan(roundedTicks);

            if (ts.Days > 0)
            {
                sb.Append(ts.Days.ToString(CultureConstants.DefaultCulture));
                sb.Append("D ");
            }

            ts -= TimeSpan.FromDays(ts.Days);
            sb.Append(ts.Hours.ToString("0#"));
            sb.Append(":");

            ts -= TimeSpan.FromHours(ts.Hours);
            sb.Append(ts.Minutes.ToString("0#"));
            sb.Append(":");

            ts -= TimeSpan.FromMinutes(ts.Minutes);
            sb.Append(ts.Seconds.ToString("0#"));

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string representation for the time left to the given date, using the following formats :
        /// <list type="bullet">
        /// 		<item>2 days, 3 hours, 1 minute, 5seconds</item>
        /// 		<item>3 hours, 1 minute</item>
        /// 		<item>Completed</item>
        /// 	</list>
        /// </summary>
        /// <param name="t">The time (in the future) for which you want a span.</param>
        /// <param name="dateTimeKind">The kind of the dateTime (UTC or Local) being converted.</param>
        /// <remarks>DateTimeKind.Unspecified will be treated as UTC</remarks>
        /// <returns></returns>
        public static string ToRemainingTimeDescription(this DateTime t, DateTimeKind dateTimeKind)
        {
            DateTime now = (dateTimeKind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow);

            StringBuilder sb = new StringBuilder();
            if (t <= now)
                return "Completed";

            double factor = Math.Pow(10, 7);
            long roundedTicks = (long)Math.Round(t.Subtract(now).Ticks / factor) * (int)factor;
            TimeSpan ts = new TimeSpan(roundedTicks);

            if (ts.Days > 0)
            {
                sb.Append(ts.Days.ToString(CultureConstants.DefaultCulture));
                sb.Append(" day");
                if (ts.Days > 1)
                    sb.Append("s");
            }
            ts -= TimeSpan.FromDays(ts.Days);
            if (ts.Hours > 0)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                sb.Append(ts.Hours.ToString(CultureConstants.DefaultCulture));
                sb.Append(" hour");
                if (ts.Hours > 1)
                    sb.Append("s");
            }
            ts -= TimeSpan.FromHours(ts.Hours);
            if (ts.Minutes > 0)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                sb.Append(ts.Minutes.ToString(CultureConstants.DefaultCulture));
                sb.Append(" minute");
                if (ts.Minutes > 1)
                    sb.Append("s");
            }
            ts -= TimeSpan.FromMinutes(ts.Minutes);
            if (ts.Seconds > 0)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(ts.Seconds.ToString(CultureConstants.DefaultCulture));
                sb.Append(" second");
                if (ts.Seconds > 1)
                    sb.Append("s");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates an absolute string based upon the following format :
        /// <list type="bullet">
        /// 		<item>17:27 Tomorrow</item>
        /// 		<item>07:43 Wednesday</item>
        /// 		<item>03:12 23/04/2009</item>
        /// 	</list>
        /// </summary>
        /// <param name="absoluteDateTime">The absolute date time.</param>
        /// <param name="dateTimeKind">The kind of the dateTime (UTC or Local) being converted.</param>
        /// <remarks>DateTimeKind.Unspecified will be treated as UTC</remarks>
        /// <returns>String representation of the time and relative date.</returns>
        public static string ToAbsoluteDateTimeDescription(this DateTime absoluteDateTime, DateTimeKind dateTimeKind)
        {
            DateTime now = (dateTimeKind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow);
            string shortTime = absoluteDateTime.ToShortTimeString();

            // Yesterday (i.e. before 00:00 today)
            if (absoluteDateTime.Date == now.Date.AddDays(-1))
                return String.Format(CultureConstants.DefaultCulture, "{0} Yesterday", shortTime);

            // Today (i.e. before 00:00 tomorrow)
            if (absoluteDateTime.Date == now.Date)
                return String.Format(CultureConstants.DefaultCulture, "{0} Today", shortTime);

            // Tomorrow (i.e. after 23:59 today but before 00:00 the day after tomorrow)
            DateTime tomorrow = now.Date.AddDays(1);
            if (absoluteDateTime.Date == tomorrow)
                return String.Format(CultureConstants.DefaultCulture, "{0} Tomorrow", shortTime);

            // After tomorrow but within 7 days
            DateTime sevenDays = now.Date.AddDays(7);
            if (absoluteDateTime.Date > tomorrow)
            {
                string dayOfWeek = absoluteDateTime.DayOfWeek.ToString();
                if (absoluteDateTime.Date < sevenDays)
                    return String.Format(CultureConstants.DefaultCulture, "{0} This {1}", shortTime, dayOfWeek);

                if (absoluteDateTime.Date == sevenDays)
                    return String.Format(CultureConstants.DefaultCulture, "{0} Next {1}", shortTime, dayOfWeek);
            }

            // More than seven days away or more than one day ago
            string shortDate = absoluteDateTime.ToString("d", CultureConstants.DefaultCulture);
            return String.Format(CultureConstants.DefaultCulture, "{0} {1}", shortTime, shortDate);
        }

        /// <summary>
        /// Convert a timespan into English text.
        /// </summary>
        /// <param name="ts">The timespan.</param>
        /// <param name="dto">Formatting options.</param>
        /// <param name="includeSeconds"></param>
        /// <returns>Timespan formatted as English text.</returns>
        public static string ToDescriptiveText(this TimeSpan ts, DescriptiveTextOptions dto, bool includeSeconds = true)
        {
            StringBuilder sb = new StringBuilder();

            BuildDescriptiveFragment(sb, ts.Days, dto, "days");
            BuildDescriptiveFragment(sb, ts.Hours, dto, "hours");
            BuildDescriptiveFragment(sb, ts.Minutes, dto, "minutes");

            if (includeSeconds)
                BuildDescriptiveFragment(sb, ts.Seconds, dto, "seconds");

            if (sb.Length == 0)
                sb.Append("(none)");

            return sb.ToString();
        }

        /// <summary>
        /// Builds the string representation for this string, just for one part of the time
        /// (may be the days, the hours, the mins, etc).
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="p"></param>
        /// <param name="dto"></param>
        /// <param name="dstr"></param>
        private static void BuildDescriptiveFragment(StringBuilder sb, int p, DescriptiveTextOptions dto, string dstr)
        {
            if (((dto & DescriptiveTextOptions.IncludeZeroes) == 0) && p == 0)
                return;

            if ((dto & DescriptiveTextOptions.IncludeCommas) != 0)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
            }

            if ((dto & DescriptiveTextOptions.SpaceBetween) != 0)
            {
                if (sb.Length > 0)
                    sb.Append(' ');
            }

            sb.Append(p.ToString(CultureConstants.DefaultCulture));

            if ((dto & DescriptiveTextOptions.SpaceText) != 0)
                sb.Append(' ');

            if ((dto & DescriptiveTextOptions.FirstLetterUppercase) != 0)
                dstr = char.ToUpper(dstr[0], CultureConstants.DefaultCulture) + dstr.Substring(1);

            if ((dto & DescriptiveTextOptions.UppercaseText) != 0)
                dstr = dstr.ToUpper(CultureConstants.DefaultCulture);

            if ((dto & DescriptiveTextOptions.FullText) != 0)
            {
                if (p == 1)
                    dstr = dstr.Substring(0, dstr.Length - 1);

                sb.Append(dstr);
            }
            else
                sb.Append(dstr[0]);
        }
    }
}