using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace EVEMon.Controls
{
    public static class NativeMethods
    {
        private const int SW_SHOWNOACTIVATE = 4;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SRCCOPY = 0x00CC0020;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
           int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int SetTextCharacterExtra(IntPtr hdc, int nCharExtra);

        /// <summary>
        /// Show the given form on topmost without activating it.
        /// </summary>
        /// <param name="frm"></param>
        public static void ShowInactiveTopmost(this Form frm)
        {
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST, frm.Left, frm.Top, frm.Width, frm.Height, SWP_NOACTIVATE);
        }

        /// <summary>
        /// Wrapper around BitBlt
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
        public static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);


        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public const string TaskbarClass = "Shell_TrayWnd";

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public static APPBARDATA Create()
            {
                APPBARDATA appBarData = new APPBARDATA();
                appBarData.cbSize = Marshal.SizeOf(typeof(APPBARDATA));
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
    }
}
