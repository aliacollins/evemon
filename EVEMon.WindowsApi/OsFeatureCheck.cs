using System;

namespace EVEMon.WindowsApi
{
    /// <summary>
    /// Allows for quick checks for support of various opperating system features
    /// </summary>
    public static class OsFeatureCheck
    {
        /// <summary>
        /// Checks to see if the current opperating system is Windows
        /// </summary>
        public static bool IsWindows
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32Windows)
                    return true;

                if (Environment.OSVersion.Platform == PlatformID.Win32S)
                    return true;

                return Environment.OSVersion.Platform == PlatformID.Win32NT;
            }
        }

        /// <summary>
        /// Checks to see if the current opperating system is Windows NT
        /// </summary>
        public static bool IsWindowsNT
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT;
            }
        }

        /// <summary>
        /// Checks to see if the Windows 7 Taskbar is supported in the
        /// current operating system.
        /// </summary>
        public static bool TaskbarSupported
        {
            get
            {
                if (!IsWindowsNT)
                    return false;

                Version winVer = Environment.OSVersion.Version;

                if (winVer.Major < 6)
                    return false;

                return winVer.Major != 6 || winVer.Minor >= 1;
            }
        }
    }
}
