using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EVEMon
{
    /// <summary>
    /// Provides window-reloaction functionality through calls to User32
    /// </summary>
    public static class NativeRelocatorMethods
    {

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public const int GWL_STYLE = -16;
        public const int WS_DLGFRAME = 0x00400000;
        public const int WS_BORDER = 0x00800000;
        public const int WS_SIZEBOX = 0x00040000;

        [DllImport("user32.Dll")]
        public static extern IntPtr EnumWindows(Relocator.WindowFoundHandler x, int y);

        [DllImport("User32.Dll")]
        public static extern void GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32")]
        public static extern int GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32")]
        public static extern int ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    }
}
