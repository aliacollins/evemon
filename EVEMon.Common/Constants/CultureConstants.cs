using System.Globalization;

namespace EVEMon.Common.Constants
{
    public static class CultureConstants
    {
        public static CultureInfo DefaultCulture
        {
            get { return CultureInfo.CurrentCulture; }
        }

        public static CultureInfo InvariantCulture
        {
            get { return CultureInfo.InvariantCulture; }
        }
    }
}