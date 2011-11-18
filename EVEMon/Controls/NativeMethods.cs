using System;
using System.Runtime.InteropServices;

namespace EVEMon.Controls
{
    public static class NativeMethods
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetTextCharacterExtra(IntPtr hdc, int nCharExtra);
    }
}
