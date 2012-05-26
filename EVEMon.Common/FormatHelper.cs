using System;

namespace EVEMon.Common
{
    public static class FormatHelper
    {
        /// <summary>
        /// Formats the given value into an abbreviated format string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="truncated"></param>
        /// <returns></returns>
        public static string Format(decimal value, AbbreviationFormat format, bool truncated = true)
        {
            decimal abs = Math.Abs(value);
            if (format == AbbreviationFormat.AbbreviationWords)
            {
                if (abs >= 1E9M)
                    return Format(" Billions", value / 1E9M, truncated);
                if (abs >= 1E6M)
                    return Format(" Millions", value / 1E6M, truncated);

                return abs >= 1E3M ? Format(" Thousands", value / 1E3M, truncated) : Format(String.Empty, value, truncated);
            }

            if (abs >= 1E9M)
                return Format(" B", value / 1E9M, truncated);
            if (abs >= 1E6M)
                return Format(" M", value / 1E6M, truncated);

            return abs >= 1E3M ? Format(" K", value / 1E3M, truncated) : Format(String.Empty, value, truncated);
        }

        /// <summary>
        /// Formats the given value and suffix the way we want.
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="value"></param>
        /// <param name="truncated"></param>
        /// <returns></returns>
        private static string Format(string suffix, decimal value, bool truncated)
        {
            if (!truncated)
                return value.ToString("N2", CultureConstants.DefaultCulture) + suffix;

            // Explanations : 999.99 was displayed as 1000 because only three digits were required
            // So we do the truncation at hand for the number of digits we exactly request

            decimal abs = Math.Abs(value);
            if (abs < 1.0M)
                return (((int)value * 100) / 100M).ToString("0.##", CultureConstants.DefaultCulture) + suffix;
            if (abs < 10.0M)
                return (((int)value * 1000) / 1000M).ToString("#.##", CultureConstants.DefaultCulture) + suffix;
            if (abs < 100.0M)
                return (((int)value * 1000) / 1000M).ToString("##.#", CultureConstants.DefaultCulture) + suffix;

            return (((int)value * 1000) / 1000M).ToString("###", CultureConstants.DefaultCulture) + suffix;
        }
    }
}
