using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon
{
    /// <summary>
    /// Provides window-reloaction functionality through calls to User32
    /// </summary>
    public static class Relocator
    {
        public delegate bool WindowFoundHandler(int hwnd, int lParam);

        private static bool m_initilized = false;
        private static readonly int m_pid = Application.ProductName.GetHashCode();
        private static readonly List<IntPtr> m_foundWindows = new List<IntPtr>();
        private static bool m_autoRelocation;
        private static int counter = 0;
        private static bool dialogActive;

        public static void Initialize()
        {
            if (m_initilized)
                return;
            
            EveClient.TimerTick += new EventHandler(EveClient_TimerTick);
            EveClient.SettingsChanged += new EventHandler(EveClient_SettingsChanged);
            EveClient_SettingsChanged(null, EventArgs.Empty);

            m_initilized = true;
        }

        #region Dimensions
        /// <summary>
        /// Get the dimensions of the window specified by hWnd
        /// </summary>
        /// <param name="hWnd">A valid window</param>
        /// <returns>new Rectangle(Left, Top, Width, Height)</returns>
        private static Rectangle GetWindowRect(IntPtr hWnd)
        {
            NativeMethods.RECT r;
            NativeMethods.GetWindowRect(hWnd, out r);
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        /// <summary>
        /// Get the screen coordinates relative to the window
        /// </summary>
        /// <param name="hWnd">A valid window</param>
        /// <returns>new Rectangle(Left, Top, Right, Bottom) relative to the screen</returns>
        private static Rectangle GetClientRectInScreenCoords(IntPtr hWnd)
        {
            NativeMethods.RECT cr;
            NativeMethods.GetClientRect(hWnd, out cr);
            NativeMethods.POINT pt = new NativeMethods.POINT();
            pt.X = 0;
            pt.Y = 0;
            NativeMethods.ClientToScreen(hWnd, ref pt);
            return new Rectangle(pt.X, pt.Y, cr.Right - cr.Left, cr.Bottom - cr.Top);
        }

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
            NativeMethods.GetWindowText(windowHandle, sbText, 512);
            NativeMethods.GetClassName(windowHandle, sbClass, 512);

            if (sbText.ToString().StartsWith("EVE", StringComparison.CurrentCultureIgnoreCase) && sbClass.ToString() == "triuiScreen")
            {
                int pid = 0;
                NativeMethods.GetWindowThreadProcessId(windowHandle, out pid);
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
                NativeMethods.EnumWindows(new WindowFoundHandler(EnumWindowCallBack), m_pid);
                return m_foundWindows;
            }
        }

        /// <summary>
        /// Position the window on the target screen
        /// </summary>
        /// <param name="hWnd">EVE instance to be moved</param>
        /// <param name="targetScreen">Screen to be moved to</param>
        public static void Relocate(IntPtr hWnd, int targetScreen)
        {
            Rectangle ncr = GetWindowRect(hWnd);
            Rectangle cr = GetClientRectInScreenCoords(hWnd);
            int wDiff = ncr.Width - cr.Width;
            int hDiff = ncr.Height - cr.Height;

            Screen sc = Screen.AllScreens[targetScreen];

            // Null guard? Could in any case sc be null?
            if (sc == null)
                return;

            // Grab the current window style
            long oldStyle = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);

            // Turn off dialog frame and border
            long newStyle = oldStyle & ~(NativeMethods.WS_DLGFRAME | NativeMethods.WS_BORDER);
            NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_STYLE, newStyle);

            NativeMethods.MoveWindow(hWnd, sc.Bounds.X,
                       sc.Bounds.Y,
                       cr.Width,
                       cr.Height, true);
        }

        /// <summary>
        /// Get's the title bar text and resolution of the specified window
        /// </summary>
        /// <param name="hWnd">Passed EVE client instance</param>
        /// <returns>String containing the title bar text and resolution</returns>
        public static string GetWindowDescription(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(512);

            NativeMethods.GetWindowText(hWnd, sb, 512);
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
            Screen sc = Screen.AllScreens[screen];
            return String.Format(CultureConstants.DefaultCulture, "Screen {0} - {1}x{2}", screen + 1, sc.Bounds.Width, sc.Bounds.Height);
        }

        #endregion

        #region Automatic Relocation
        /// <summary>
        /// Gets the state of autorelocation checkbox
        /// </summary>  
        public static bool AutoRelocationEnabled
        {
            get
            {
                return m_autoRelocation = Settings.UI.MainWindow.EnableAutomaticRelocation;
            }
        }
        
        /// <summary>
        /// Perfoms the AutoRelocation
        /// </summary>
        private static void AutoRelocate()
        {
            int screenCount = Screen.AllScreens.Length;
            int sameResScr = 0;

            foreach (IntPtr eveInstance in FindEveWindows())
            {
                // Skip if null ptr
                if (eveInstance == IntPtr.Zero)
                    continue;

                Rectangle ncr = GetWindowRect(eveInstance);
                Rectangle cr = GetClientRectInScreenCoords(eveInstance);
                int wDiff = ncr.Width - cr.Width;
                int hDiff = ncr.Height - cr.Height;

                // We skip if the client is minimized
                if ((cr.Height == 0) && (cr.Width == 0))
                    continue;

                // We skip if the client is already relocated
                if (wDiff == 0 && hDiff == 0)
                    continue;

                // Check if monitors with same resolution are present
                for (int screen = 0; screen < screenCount; screen++)
                {
                    var nextScreen = Math.Min(screen + 1, screenCount - 1);
                    if (Screen.AllScreens[screen].Bounds.Size == Screen.AllScreens[nextScreen].Bounds.Size)
                    {
                        sameResScr += 1;
                    }
                }

                // More than one monitor with same resolution ?
                if (sameResScr > 1)
                {
                    // Ensure that the client is the same size as the monitor's resolution
                    for (int screen = 0; screen < screenCount; screen++)
                    {
                        if (cr.Width != Screen.AllScreens[screen].Bounds.Width)
                            return;
                    }

                    if (!dialogActive)
                    {
                        dialogActive = true;
                        ShowDialog(eveInstance, sameResScr);
                        dialogActive = false;

                        // We have changed the enumeration
                        // so we need to exit
                        return;
                    }
                }
                // Different monitor resolutions ?
                // Relocate to the monitor that fits the client
                else
                {
                    for (int screen = 0; screen < screenCount; screen++)
                    {
                        if (cr.Width != Screen.AllScreens[screen].Bounds.Width)
                            continue;

                        Relocate(eveInstance, screen);
                    }
                }
            }
        }

        /// <summary>
        /// Shows a dialog to the user requesting
        /// to which monitor to relocate the client
        /// </summary>
        private static void ShowDialog(IntPtr eveInstance, int sameResScr)
        {
            var buttonWidth = 0;
            var buttonHeight = 0;
            int hPad = (sameResScr > 2 ? 15 : 60);
            int pad = 30;
            
            // We create a dialog for the user
            using (var dialog = new EVEMonForm())
            {
                // We add a panel to the form
                Panel panel = new Panel();
                panel.Dock = DockStyle.Fill;
                dialog.Controls.Add(panel);

                // Add label
                Label label = new Label();
                label.AutoSize = true;
                label.Text = "EVEMon detected that you have more than one\rmonitor with the same resolution.\r\rChoose to which monitor to relocate the EVE client.";
                panel.Controls.Add(label);

                // Add buttons
                for (int scr = 0; scr <= sameResScr - 1; scr++)
                {
                    int screen = scr;
                    int spacing = (scr == sameResScr ? 10 : 5);

                    Button button = new Button();
                    button.Text = "Monitor " + (scr + 1);
                    button.Location = new Point(hPad + buttonWidth, label.Height + pad);
                    buttonWidth += button.Width + spacing;
                    buttonHeight = button.Height;
                    panel.Controls.Add(button);

                    // Handles the button press
                    button.Click += (sender, args) =>
                    {
                        Relocate(eveInstance, screen);
                        dialog.Close();
                    };
                }

                // Sets form properties
                dialog.StartPosition = FormStartPosition.CenterScreen;
                dialog.TopMost = true;

                // Sets the size of the dialog
                // and prevents manual resizing
                int dialogWidth = Math.Max((label.Width + pad > buttonWidth + pad ? label.Width + pad : buttonWidth + pad), 220);
                int dialogHeight = label.Height * 2 + buttonHeight * 2;

                dialog.Size = new Size(dialogWidth, dialogHeight);
                dialog.MaximumSize = dialog.Size;
                dialog.MinimumSize = dialog.Size;

                // Centers the label to the panel
                label.Location = new Point((panel.Width - label.Width) / 2, 10);

                // We show the dialog
                dialog.ShowDialog();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Checks whether an AutoRelocation should occur
        /// </summary>
        /// <param name="interval">The length of time to wait between checks.</param>
        private static void EveClient_TimerTick(object sender, EventArgs e)
        {
            var interval = TimeSpan.FromSeconds(Settings.UI.MainWindow.AutomaticRelocationInterval);
            int m_checkInterval = (int)interval.Seconds;

            if (AutoRelocationEnabled)
            {
                while (counter == m_checkInterval)
                {
                    counter = 0;
                    AutoRelocate();
                }
                counter++;
            }
        }

        #endregion

        #region Reaction to settings change

        /// <summary>
        /// Occurs when the settings have changed.
        /// </summary>
        private static void EveClient_SettingsChanged(object sender, EventArgs e)
        {
            if (m_autoRelocation) EveClient.Trace("AutoRelocation.Enabled");
            else EveClient.Trace("AutoRelocation.Disabled");
        }

        #endregion
    }

    #region Native Stuff
    /// <summary>
    /// Provides window-reloaction functionality through calls to User32
    /// </summary>
    public static class NativeMethods
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
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern long SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32")]
        public static extern int ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    }
    #endregion

}