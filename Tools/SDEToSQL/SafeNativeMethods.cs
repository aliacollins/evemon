using System.Runtime.InteropServices;

namespace EVEMon.SDEToSQL
{
    internal static class SafeNativeMethods
    {
        internal delegate bool EventHandler(CtrlType ctrlType);

        [DllImport("Kernel32")]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool SetConsoleCtrlHandler(EventHandler handler, [MarshalAs(UnmanagedType.U1)] bool add);
    }
}
