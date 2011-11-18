using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public static class NativeMethods
    {
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SW_SHOWNOACTIVATE = 0x0004;
        public const int HWND_TOPMOST = -1;
        public const uint SWP_NOACTIVATE = 0x0010;

        private const uint SRCCOPY = 0x00CC0020;

        [DllImport("psapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EmptyWorkingSet(IntPtr proc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr handle, uint messg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter, Int32 x, Int32 y,
                                               Int32 cx, Int32 cy, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
                                          int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, uint dwRop);

        /// <summary>
        /// Show the given form on topmost without activating it.
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="flags"></param>
        public static void ShowInactiveTopmost(this Control frm, uint flags = 0)
        {
            if (frm == null)
                throw new ArgumentNullException("frm");

            // We store the 'left' and 'top' position because for some reason
            // on first execution of 'ShowWindow' the form position gets reset
            int left = frm.Left;
            int top = frm.Top;

            SetWindowPos(frm.Handle, HWND_TOPMOST, left, top, frm.Width, frm.Height, SWP_NOACTIVATE | flags);
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
        }

        /// <summary>
        /// Wrapper around BitBlt.
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="destClip">Clipping rectangle on dest</param>
        /// <param name="src"></param>
        /// <param name="bltFrom">Upper-left point on src to blt from</param>
        /// <returns></returns>
        public static void CopyGraphics(Graphics dest, Rectangle destClip, Graphics src, Point bltFrom)
        {
            if (dest == null)
                throw new ArgumentNullException("dest");

            if (src == null)
                throw new ArgumentNullException("src");

            BitBlt(dest.GetHdc(), destClip.Left, destClip.Top, destClip.Width, destClip.Height,
                   src.GetHdc(), bltFrom.X, bltFrom.Y, SRCCOPY);
        }


        #region Tray Icon

        // All definitions taken from http://pinvoke.net
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SHAppBarMessage(uint dwMessage, ref AppBarData pData);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public const string TaskbarClass = "Shell_TrayWnd";

        [StructLayout(LayoutKind.Sequential)]
        internal struct AppBarData
        {
            private int cbSize;
            private readonly IntPtr hWnd;
            private readonly uint uCallbackMessage;
            private readonly uint uEdge;
            private readonly RECT m_rect;
            private readonly int lParam;

            public uint UEdge
            {
                get { return uEdge; }
            }

            public RECT Rect
            {
                get { return m_rect; }
            }

            public static AppBarData Create()
            {
                AppBarData appBarData = new AppBarData { cbSize = Marshal.SizeOf(typeof(AppBarData)) };
                return appBarData;
            }
        }

        public const int ABM_QUERYPOS = 0x00000002,
                         ABM_GETTASKBARPOS = 5;

        public const int ABE_LEFT = 0;
        public const int ABE_TOP = 1;
        public const int ABE_RIGHT = 2;
        public const int ABE_BOTTOM = 3;

        [Serializable, StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            private RECT(int left, int top, int right, int bottom)
                : this()
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int Left { get; private set; }

            public int Top { get; private set; }

            public int Right { get; private set; }

            public int Bottom { get; private set; }


            public int Height
            {
                get { return Bottom - Top + 1; }
            }

            public int Width
            {
                get { return Right - Left + 1; }
            }

            public Size Size
            {
                get { return new Size(Width, Height); }
            }

            public Point Location
            {
                get { return new Point(Left, Top); }
            }

            // Handy method for converting to a System.Drawing.Rectangle
            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }

            public static RECT FromRectangle(Rectangle rectangle)
            {
                return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode()
            {
                return Left ^ ((Top << 13) | (Top >> 0x13))
                       ^ ((Width << 0x1a) | (Width >> 6))
                       ^ ((Height << 7) | (Height >> 0x19));
            }


            #region Operator overloads

            public static implicit operator Rectangle(RECT rect)
            {
                return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            public static implicit operator RECT(Rectangle rect)
            {
                return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            #endregion
        }

        #endregion


        #region Custom Message Box

        public const int SC_CLOSE = 0xF060;
        public const int MF_BYCOMMAND = 0x0;
        public const int MF_GRAYED = 0x1;
        public const int MF_ENABLED = 0x0;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        #endregion
    }
}