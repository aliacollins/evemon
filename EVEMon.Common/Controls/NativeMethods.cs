using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public static class NativeMethods
    {
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int HWND_TOPMOST = -1;
        public const uint SWP_NOACTIVATE = 0x0010;

        private const uint SRCCOPY = 0x00CC0020;

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter, Int32 x, Int32 y,
                                               Int32 cx, Int32 cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int SetTextCharacterExtra(IntPtr hdc, int nCharExtra);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
                                          int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, uint dwRop);

        /// <summary>
        /// Show the given form on topmost without activating it.
        /// </summary>
        /// <param name="frm"></param>
        public static void ShowInactiveTopmost(this Form frm)
        {
            // We store the 'left' and 'top' position because for some reason
            // on first execution of 'ShowWindow' the form position gets reset
            int left = frm.Left;
            int top = frm.Top;

            SetWindowPos(frm.Handle, HWND_TOPMOST, left, top, frm.Width, frm.Height, SWP_NOACTIVATE);
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
        public static bool CopyGraphics(Graphics dest, Rectangle destClip, Graphics src, Point bltFrom)
        {
            return BitBlt(dest.GetHdc(), destClip.Left, destClip.Top, destClip.Width, destClip.Height,
                          src.GetHdc(), bltFrom.X, bltFrom.Y, SRCCOPY);
        }


        #region Tray Icon

        // All definitions taken from http://pinvoke.net
        [DllImport("shell32.dll")]
        public static extern IntPtr SHAppBarMessage(uint dwMessage, ref AppBarData pData);


        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public const string TaskbarClass = "Shell_TrayWnd";

        [StructLayout(LayoutKind.Sequential)]
        public struct AppBarData
        {
            public static AppBarData Create()
            {
                AppBarData appBarData = new AppBarData();
                appBarData.cbSize = Marshal.SizeOf(typeof (AppBarData));
                return appBarData;
            }

            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        public const int ABM_QUERYPOS = 0x00000002,
                         ABM_GETTASKBARPOS = 5;

        public const int ABE_LEFT = 0;
        public const int ABE_TOP = 1;
        public const int ABE_RIGHT = 2;
        public const int ABE_BOTTOM = 3;


        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left_, int top_, int right_, int bottom_)
            {
                Left = left_;
                Top = top_;
                Right = right_;
                Bottom = bottom_;
            }

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
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        
        #endregion
    }
}
