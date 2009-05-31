using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace EVEMon.WindowRelocator
{
    /// <summary>
    /// Provides window-reloaction functionality through calls to User32
    /// </summary>
    public static class Relocator
    {
        private static CbtHook m_hook = null;
        private static int m_targetScreen = 0;
        private static bool m_autoselectScreen;
        private static Timer m_timer = null;
        private static object m_lockObj = new object();
        
        /// <summary>
        /// Indirectly loads EVEMon.WinHook.dll and starts timer
        /// </summary>
        /// <param name="targetScreen"></param>
        public static void Start(int targetScreen)
        {
            if (m_hook == null)
            {
                m_hook = new CbtHook();
                m_hook.WindowCreated += new EventHandler<EventArgs>(m_hook_WindowCreated);
            }
            if (m_timer == null)
            {
                m_timer = new Timer();
                m_timer.Interval = 250;
                m_timer.Tick += TimerCallbackProc;
            }
            m_targetScreen = targetScreen;
        }

        /// <summary>
        /// Disposes of the hook and stops the timer.
        /// </summary>
        public static void Stop()
        {
            m_hook.Dispose();
            m_hook = null;
            m_timer.Stop();
        }

        [DllImport("user32")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        private extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // GetClientRect: returns width and height, not right and bottom in lpRect!
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
            return new Rectangle(pt.X, pt.Y, cr.Right, cr.Bottom);
        }

        private static Point m_positionedPoint = new Point(Int32.MaxValue, Int32.MaxValue);

        /// <summary>
        /// Position the window on the targe screen
        /// </summary>
        /// <param name="hWnd"></param>
        private static void PositionWindow(IntPtr hWnd)
        {
            Rectangle ncr = GetWindowRect(hWnd);
            Rectangle cr = GetClientRectInScreenCoords(hWnd);
            int wDiff = ncr.Width - cr.Width;
            int hDiff = ncr.Height - cr.Height;

            Screen sc = null;
            if (m_targetScreen == -1)
            {
                foreach (Screen s in Screen.AllScreens)
                {
                    if (s.Bounds.Width == cr.Width && s.Bounds.Height == cr.Height)
                    {
                        sc = s;
                        break;
                    }
                }
            }
            else
            {
                if (Screen.AllScreens.Length > m_targetScreen)
                {
                    sc = Screen.AllScreens[m_targetScreen];
                }
                else
                    sc = Screen.AllScreens[0];
            }

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
        /// Event fired when window is detected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void m_hook_WindowCreated(object sender, EventArgs e)
        {
            m_failCount = 0;
            TimerCallbackProc(null, null);
        }

        private static int m_failCount = 0;

        /// <summary>
        /// Start timer
        /// </summary>
        private static void SetupTimer()
        {
            m_timer.Start();
        }

        /// <summary>
        /// Stop timer
        /// </summary>
        private static void CancelTimer()
        {
            m_timer.Stop();
            if (m_failCount <= 8)
            {
                m_failCount++;
                SetupTimer();
            }
        }

        /// <summary>
        /// Identify and move window 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private static void TimerCallbackProc(object o, EventArgs e)
        {
            lock (m_lockObj)
            {
                IntPtr fgWin = GetForegroundWindow();
                StringBuilder sb = new StringBuilder(512);
                int titleLen = GetWindowText(fgWin, sb, 512);

                int pid = 0;
                GetWindowThreadProcessId(fgWin, out pid);

                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);

                if (sb.ToString() == "EVE" && p.ProcessName == "ExeFile")
                {
                    Rectangle r = GetWindowRect(fgWin);
                    if (r.Width > 800)
                    {
                        if (r.Location != m_positionedPoint)
                        {
                            PositionWindow(fgWin);
                        }
                        m_failCount = Int32.MaxValue;
                        CancelTimer();
                    }
                    else
                    {
                        m_failCount = 0;
                        SetupTimer();
                    }
                }
                else
                {
                    CancelTimer();
                }
            }
        }
    }
}