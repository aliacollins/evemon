using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace EVEMon
{
    /// <summary>
    /// Provides window-reloaction functionality through calls to User32
    /// </summary>
    public static class Relocator
    {
        private delegate bool WindowFoundHandler(int hwnd, int lParam);

        private static readonly int m_pid = Application.ProductName.GetHashCode();
        private static readonly List<IntPtr> m_foundWindows = new List<IntPtr>();

        #region DLL Imports
        [DllImport("user32.Dll")]
        private static extern IntPtr EnumWindows(WindowFoundHandler x, int y);

        [DllImport("User32.Dll")]
        private static extern void GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        private extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32")]
        private static extern int GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32")]
        private static extern int ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }
        #endregion

        #region Dimensions
        /// <summary>
        /// Get the dimensions of the window specified by hWnd
        /// </summary>
        /// <param name="hWnd">A valid window</param>
        /// <returns>new Rectangle(Left, Top, Width, Height)</returns>
        private static Rectangle GetWindowRect(IntPtr hWnd)
        {
            RECT r;
            GetWindowRect(hWnd, out r);
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        /// <summary>
        /// Get the screen coordinates relative to the window
        /// </summary>
        /// <param name="hWnd">A valid window</param>
        /// <returns>new Rectangle(Left, Top, Right, Bottom) relative to the screen</returns>
        private static Rectangle GetClientRectInScreenCoords(IntPtr hWnd)
        {
            RECT cr;
            GetClientRect(hWnd, out cr);
            POINT pt = new POINT();
            pt.X = 0;
            pt.Y = 0;
            ClientToScreen(hWnd, ref pt);
            return new Rectangle(pt.X, pt.Y, cr.Right - cr.Left, cr.Bottom - cr.Top);
        }

        private static Point m_positionedPoint = new Point(Int32.MaxValue, Int32.MaxValue);
        #endregion

        #region Private functions
        /// <summary>
        /// Callback function for finding all open EVE instance windows
        /// </summary>
        /// <param name="hwnd">the window information to be tested - automatically passed by EnumWindows</param>
        private static bool EnumWindowCallBack(int hwnd, int lParam)
        {
            IntPtr windowHandle = (IntPtr)hwnd;
            StringBuilder sbText = new StringBuilder(512);
            StringBuilder sbClass = new StringBuilder(512);
            GetWindowText(windowHandle, sbText, 512);
            GetClassName(windowHandle, sbClass, 512);

            if (sbText.ToString().StartsWith("EVE", StringComparison.CurrentCultureIgnoreCase) && sbClass.ToString() == "triuiScreen")
            {
                int pid = 0;
                GetWindowThreadProcessId(windowHandle, out pid);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);
                if (p.ProcessName == "ExeFile")
                {
                    m_foundWindows.Add(windowHandle);
                }
            }
            
            return true;
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Identifies all open EVE instances
        /// </summary>
        /// <returns>IntPtr array of EVE instances</returns>
        public static IEnumerable<IntPtr> FindEveWindows()
        {
            lock (m_foundWindows)
            {
                m_foundWindows.Clear();
                EnumWindows(new WindowFoundHandler(Relocator.EnumWindowCallBack), m_pid);
                return m_foundWindows;
            }
        }

        /// <summary>
        /// Position the window on the targe screen
        /// </summary>
        /// <param name="hWnd">EVE instance to be moved</param>
        /// <param name="targetScreen">Screen to be moved to</param>
        public static void Relocate(IntPtr hWnd, int targetScreen)
        {
            Rectangle ncr = GetWindowRect(hWnd);
            Rectangle cr = GetClientRectInScreenCoords(hWnd);
            int wDiff = ncr.Width - cr.Width;
            int hDiff = ncr.Height - cr.Height;

            Screen sc = null;

            if (Screen.AllScreens.Length > targetScreen)
            {
                sc = Screen.AllScreens[targetScreen];
            }
            else
                sc = Screen.AllScreens[0];

            if (sc == null)
                return;

            // Don't resize if the eve window if it's size is not that of the screen
            if (sc.Bounds.Width != cr.Width || sc.Bounds.Height != cr.Height)
                return;


            m_positionedPoint = new Point(sc.Bounds.X - (cr.Left - ncr.Left),
                                          sc.Bounds.Y - (cr.Top - ncr.Top));

            MoveWindow(hWnd, sc.Bounds.X - (cr.Left - ncr.Left),
                       sc.Bounds.Y - (cr.Top - ncr.Top),
                       sc.Bounds.Width + wDiff,
                       sc.Bounds.Height + hDiff, true);
        }

        /// <summary>
        /// Get's the title bar text and resolution of the specified window
        /// </summary>
        /// <param name="hWnd">Passed EVE client instance</param>
        /// <returns>String containing the title bar text and resolution</returns>
        public static string GetWindowDescription(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(512);

            GetWindowText(hWnd, sb, 512);
            sb.Append(" - ");

            Rectangle cr = GetClientRectInScreenCoords(hWnd);

            if ((cr.Height == 0) && (cr.Width == 0))
            {
                sb.Append("Minimized");
            }
            else
            {
                sb.AppendFormat("{0}x{1}", cr.Width, cr.Height);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the resolution of the specified window
        /// </summary>
        /// <param name="hWnd">EVE client instance</param>
        /// <returns>Rectangle containing the resolution of the window</returns>
        public static Rectangle GetWindowDimensions(IntPtr hWnd)
        {
            return GetClientRectInScreenCoords(hWnd);
        }

        /// <summary>
        /// Returns the number and resolution of the passed screen number
        /// </summary>
        /// <param name="screen">Integer of the screen to be identified</param>
        /// <returns>Screen z - WidthxHeight</returns>
        public static string GetScreenDescription(int screen)
        {
            string data = "";
            Screen sc = Screen.AllScreens[screen];

            data += "Screen " + screen + " - " + sc.Bounds.Width + "x" + sc.Bounds.Height;
            
            return data;
        }


        #endregion
    }
}