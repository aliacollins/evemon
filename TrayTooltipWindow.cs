using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class TrayTooltipWindow : Form
    {
        public TrayTooltipWindow()
        {
            InitializeComponent();
            m_initialized = true;
        }

        private bool m_initialized = false;
        private Size m_size = new Size(50, 50);

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if (m_initialized)
                {
                    CalculateSize();
                    PositionWindow();
                }
            }
        }

        private void CalculateSize()
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                m_size = TextRenderer.MeasureText(g, this.Text, this.Font, new Size(0, 0), TextFormatFlags.NoClipping | TextFormatFlags.NoPadding);
            }
            m_size = new Size(m_size.Width + 6, m_size.Height + 4);
            this.ClientSize = m_size;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.FillRectangle(SystemBrushes.Info, e.ClipRectangle);
            g.DrawRectangle(SystemPens.InfoText, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, new Point(3, 2), SystemColors.InfoText,
                                    Color.Transparent, TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);
        }

        internal class NativeMethods
        {
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
        }

        private LowLevelMouseHook m_hook;

        private void TrayTooltipWindow_Shown(object sender, EventArgs e)
        {
            m_hook = new LowLevelMouseHook();
            m_hook.MouseMove += new EventHandler<EventArgs>(m_hook_MouseMove);
            m_hook.Start();
            CalculateSize();
            PositionWindow();
        }

        private void PositionWindow()
        {
            Point mp = MousePosition;
            NativeMethods.APPBARDATA appBarData = NativeMethods.APPBARDATA.Create();
            NativeMethods.SHAppBarMessage(NativeMethods.ABM_GETTASKBARPOS, ref appBarData);
            NativeMethods.RECT taskBarLocation = appBarData.rc;

            Point winPoint = mp;
            Screen curScreen = Screen.FromPoint(mp);
            bool slideLeftRight = true;
            switch (appBarData.uEdge)
            {
                default:
                case NativeMethods.ABE_BOTTOM:
                    winPoint = new Point(mp.X, taskBarLocation.Top - this.Height);
                    slideLeftRight = true;
                    break;
                case NativeMethods.ABE_TOP:
                    winPoint = new Point(mp.X, taskBarLocation.Bottom);
                    slideLeftRight = true;
                    break;
                case NativeMethods.ABE_LEFT:
                    winPoint = new Point(taskBarLocation.Right, mp.Y);
                    slideLeftRight = false;
                    break;
                case NativeMethods.ABE_RIGHT:
                    winPoint = new Point(taskBarLocation.Left - this.Width, mp.Y);
                    slideLeftRight = false;
                    break;
            }
            if (slideLeftRight)
            {
                if (winPoint.X + this.Width > curScreen.Bounds.Right)
                {
                    winPoint = new Point(curScreen.Bounds.Right - this.Width - 1, winPoint.Y);
                }
                if (winPoint.X < curScreen.Bounds.Left)
                {
                    winPoint = new Point(curScreen.Bounds.Left + 2, winPoint.Y);
                }
            }
            else
            {
                if (winPoint.Y + this.Height > curScreen.Bounds.Bottom)
                {
                    winPoint = new Point(winPoint.X, curScreen.Bounds.Bottom - this.Height - 1);
                }
                if (winPoint.Y < curScreen.Bounds.Top)
                {
                    winPoint = new Point(winPoint.X, curScreen.Bounds.Top + 2);
                }
            }
            this.Location = winPoint;
        }

        private int m_moveCount = 0;

        private void m_hook_MouseMove(object sender, EventArgs e)
        {
            m_moveCount++;
            if (m_moveCount == 2)
            {
                this.Close();
            }
        }

        public void RefreshAlive()
        {
            m_moveCount = 0;
        }

        private void TrayTooltipWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_hook.Dispose();
            m_hook = null;
        }
    }

    public class LowLevelMouseHook : IDisposable
    {
        public delegate int LowLevelMouseDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        public const int WH_MOUSE_LL = 14;

        public const int WM_MOUSEMOVE = 0x0200;

        private int hHook = 0;

        [DllImport("user32", SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, LowLevelMouseDelegate lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32")]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        ~LowLevelMouseHook()
        {
            Dispose();
        }

        public event EventHandler<EventArgs> MouseMove;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private LowLevelMouseDelegate m_delegate;

        public void Start()
        {
            //IntPtr hinst = Marshal.GetHINSTANCE(this.GetType().Module);
            //IntPtr hinst = Marshal.GetHINSTANCE(System.Diagnostics.Process.GetCurrentProcess().MainModule.M);
            IntPtr hinst = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
            m_delegate = new LowLevelMouseDelegate(OnMouseProc);

            hHook = SetWindowsHookEx(WH_MOUSE_LL,
                                     m_delegate,
                //Marshal.GetHINSTANCE(this.GetType().Module),
                                     hinst,
                                     0);
            if (hHook == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public void Dispose()
        {
            if (hHook == 0)
            {
                return;
            }
            UnhookWindowsHookEx(hHook);
            m_delegate = null;
            hHook = 0;
            GC.SuppressFinalize(this);
        }

        private int OnMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode <= 0)
                {
                    switch ((int)wParam)
                    {
                        case WM_MOUSEMOVE:
                            OnMouseMove();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private void OnMouseMove()
        {
            if (MouseMove != null)
            {
                MouseMove(this, new EventArgs());
            }
        }
    }
}