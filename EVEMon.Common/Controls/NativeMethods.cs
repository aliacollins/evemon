using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public static class NativeMethods
    {
        public const int HWND_TOPMOST = -1;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SW_SHOWNOACTIVATE = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;

        private const uint SRCCOPY = 0x00CC0020;

        [DllImport("psapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EmptyWorkingSet(IntPtr proc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr handle, Int32 messg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter, Int32 x, Int32 y,
                                                Int32 cx, Int32 cy, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
                                          int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, uint dwRop);

        /// <summary>
        /// Show the given form on topmost without activating it.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="uFlags"></param>
        public static void ShowInactiveTopmost(this Control form, uint uFlags = 0)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            // We store the 'left' and 'top' position because for some reason
            // on first execution of 'ShowWindow' the form position gets reset
            int left = form.Left;
            int top = form.Top;

            SetWindowPos(form.Handle, HWND_TOPMOST, left, top, form.Width, form.Height, SWP_NOACTIVATE | uFlags);
            ShowWindow(form.Handle, SW_SHOWNOACTIVATE);
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


        #region Graphic Text Character Spacing

        /// <summary>
        /// Sets the text character spacing.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="spacing">The spacing.</param>
        public static void SetTextCharacterSpacing(Graphics g, int spacing)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            IntPtr hdc = g.GetHdc();
            SetTextCharacterExtra(hdc, spacing);
            g.ReleaseHdc();
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern int SetTextCharacterExtra(IntPtr hdc, int nCharExtra);

        #endregion


        #region ListView ScrollBar positioning

        /// <summary>
        /// Gets the sroll bar position of the list view.
        /// </summary>
        /// <param name="control">The list view.</param>
        /// <returns>The scroll bar position.</returns>
        public static int GetVerticalScrollBarPosition(this ListView control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            Scrollinfo currentInfo = new Scrollinfo();
            currentInfo.cbSize = Marshal.SizeOf(currentInfo);
            currentInfo.fMask = (int)ScrollInfoMask.SIF_ALL;

            GetScrollInfo(control.Handle, (int)ScrollBarDirection.SB_VERT, ref currentInfo);
            return currentInfo.nPos;
        }

        /// <summary>
        /// Sets the scroll bar position of the list view.
        /// </summary>
        /// <param name="control">The list view.</param>
        /// <param name="position">The scroll bar position.</param>
        public static void SetVerticalScrollBarPosition(this ListView control, int position)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            SendMessage(new HandleRef(null, control.Handle), (uint)ListViewMessages.LVM_SCROLL, IntPtr.Zero, (IntPtr)position);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Scrollinfo
        {
            public int cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        [Flags]
        private enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS
        }

        private enum ListViewMessages
        {
            LVM_FIRST = 0x1000,
            LVM_SCROLL = (LVM_FIRST + 20)
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollInfo(IntPtr hwnd, int fnBar, ref Scrollinfo lpsi);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(HandleRef hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        #endregion


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