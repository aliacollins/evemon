using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer=System.Threading.Timer;

namespace EVEMon.WindowRelocator
{
    public static class Relocator
    {
        private static CbtHook m_hook = null;
        private static int m_targetScreen = 0;

        public static void Start(int targetScreen)
        {
            if (m_hook == null)
            {
                m_hook = new CbtHook();
                m_hook.WindowCreated += new EventHandler<EventArgs>(m_hook_WindowCreated);
            }
            m_targetScreen = targetScreen;
        }

        public static void Stop()
        {
            m_hook.Dispose();
            m_hook = null;
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

        private static Rectangle GetWindowRect(IntPtr hWnd)
        {
            RECT r;
            GetWindowRect(hWnd, out r);
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

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

        private static void PositionWindow(IntPtr hWnd)
        {
            // TODO: get screen number from settings!!
            Screen sc = Screen.AllScreens[0];
            if (Screen.AllScreens.Length > m_targetScreen)
            {
                sc = Screen.AllScreens[m_targetScreen];
            }

            Rectangle ncr = GetWindowRect(hWnd);
            Rectangle cr = GetClientRectInScreenCoords(hWnd);
            int wDiff = ncr.Width - cr.Width;
            int hDiff = ncr.Height - cr.Height;

            m_positionedPoint = new Point(sc.Bounds.X - (cr.Left - ncr.Left),
                                          sc.Bounds.Y - (cr.Top - ncr.Top));

            MoveWindow(hWnd, sc.Bounds.X - (cr.Left - ncr.Left),
                       sc.Bounds.Y - (cr.Top - ncr.Top),
                       sc.Bounds.Width + wDiff,
                       sc.Bounds.Height + hDiff, true);
        }

        private static Timer m_timer = null;
        private static object m_lockObj = new object();

        private static void m_hook_WindowCreated(object sender, EventArgs e)
        {
            m_failCount = 0;
            TimerCallbackProc(null);
        }

        private static int m_failCount = 0;

        private static void SetupTimer()
        {
            if (m_timer == null)
            {
                m_timer = new Timer(new TimerCallback(TimerCallbackProc));
            }
            m_timer.Change(250, Timeout.Infinite);
        }

        private static void CancelTimer()
        {
            if (m_failCount > 8)
            {
                if (m_timer != null)
                {
                    m_timer.Dispose();
                    m_timer = null;
                }
            }
            else
            {
                m_failCount++;
                SetupTimer();
            }
        }

        private static void TimerCallbackProc(object o)
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