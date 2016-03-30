using System.Runtime.InteropServices;
using System.Security;

namespace EVEMon.WindowsApi
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/dd378422%28VS.85%29.aspx
        /// </summary>
        /// <param name="appID">appID string</param>
        [DllImport("shell32.dll")]
        internal static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string appID);
    }
}