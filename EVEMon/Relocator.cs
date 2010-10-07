using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon
{
    /// <summary>
    /// Provides window-relocation functionality through calls to User32.
    /// </summary>
    public static class Relocator
    {
        public delegate bool WindowFoundHandler(int hwnd, int lParam);

        private static bool m_initilized;
        private static readonly int m_pid = Application.ProductName.GetHashCode();
        private static readonly List<IntPtr> m_foundWindows = new List<IntPtr>();
        private static int m_autoRelocateDefaultMonitor;
        private static int counter;
        private static bool m_autoRelocation;
        private static bool dialogActive;

        public static void Initialize()
        {
            if (m_initilized)
                return;

            EveClient.TimerTick += EveClient_TimerTick;
            EveClient.SettingsChanged += EveClient_SettingsChanged;
            EveClient_SettingsChanged(null, EventArgs.Empty);

            m_initilized = true;
        }

        #region Dimensions
        /// <summary>
        /// Get the dimensions of the window specified by hWnd.
        /// </summary>
        /// <param name="hWnd">A valid window</param>
        /// <returns>new Rectangle(Left, Top, Width, Height)</returns>
        private static Rectangle GetWindowRect(IntPtr hWnd)
        {
            NativeRelocatorMethods.RECT r;
            NativeRelocatorMethods.GetWindowRect(hWnd, out r);
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        /// <summary>
        /// Get the screen coordinates relative to the window.
        /// </summary>
        /// <param name="hWnd">A valid window</param>
        /// <returns>new Rectangle(Left, Top, Right, Bottom) relative to the screen</returns>
        private static Rectangle GetClientRectInScreenCoords(IntPtr hWnd)
        {
            NativeRelocatorMethods.RECT cr;
            NativeRelocatorMethods.GetClientRect(hWnd, out cr);
            NativeRelocatorMethods.POINT pt = new NativeRelocatorMethods.POINT();
            pt.X = 0;
            pt.Y = 0;
            NativeRelocatorMethods.ClientToScreen(hWnd, ref pt);
            return new Rectangle(pt.X, pt.Y, cr.Right - cr.Left, cr.Bottom - cr.Top);
        }

        #endregion

        #region Private functions
        /// <summary>
        /// Callback function for finding all open EVE instance windows.
        /// </summary>
        /// <param name="hwnd">the window information to be tested - automatically passed by EnumWindows</param>
        private static bool EnumWindowCallBack(int hwnd, int lParam)
        {
            IntPtr windowHandle = (IntPtr)hwnd;
            StringBuilder sbText = new StringBuilder(512);
            StringBuilder sbClass = new StringBuilder(512);
            NativeRelocatorMethods.GetWindowText(windowHandle, sbText, 512);
            NativeRelocatorMethods.GetClassName(windowHandle, sbClass, 512);

            if (sbText.ToString().StartsWith("EVE", StringComparison.CurrentCultureIgnoreCase) && sbClass.ToString() == "triuiScreen")
            {
                int pid = 0;
                NativeRelocatorMethods.GetWindowThreadProcessId(windowHandle, out pid);
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
        /// Identifies all open EVE instances.
        /// </summary>
        /// <returns>IntPtr array of EVE instances</returns>
        public static IEnumerable<IntPtr> FindEveWindows()
        {
            lock (m_foundWindows)
            {
                m_foundWindows.Clear();
                NativeRelocatorMethods.EnumWindows(EnumWindowCallBack, m_pid);
                return m_foundWindows;
            }
        }

        /// <summary>
        /// Position the window on the target screen.
        /// </summary>
        /// <param name="hWnd">EVE instance to be moved</param>
        /// <param name="targetScreen">Screen to be moved to</param>
        public static void Relocate(IntPtr hWnd, int targetScreen)
        {
            Rectangle cr = GetClientRectInScreenCoords(hWnd);

            Screen sc = Screen.AllScreens[targetScreen];

            // Null guard? Could in any case sc be null?
            if (sc == null)
                return;

            // Grab the current window style
            int oldStyle = NativeRelocatorMethods.GetWindowLong(hWnd, NativeRelocatorMethods.GWL_STYLE);

            // Turn off dialog frame and border
            int newStyle = oldStyle & ~(NativeRelocatorMethods.WS_DLGFRAME | NativeRelocatorMethods.WS_BORDER);
            NativeRelocatorMethods.SetWindowLong(hWnd, NativeRelocatorMethods.GWL_STYLE, newStyle);

            NativeRelocatorMethods.MoveWindow(hWnd, sc.Bounds.X,
                       sc.Bounds.Y,
                       cr.Width,
                       cr.Height, true);
        }

        /// <summary>
        /// Get's the title bar text and resolution of the specified window.
        /// </summary>
        /// <param name="hWnd">Passed EVE client instance</param>
        /// <returns>String containing the title bar text and resolution</returns>
        public static string GetWindowDescription(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(512);

            NativeRelocatorMethods.GetWindowText(hWnd, sb, 512);
            sb.Append(" - ");

            Rectangle cr = GetClientRectInScreenCoords(hWnd);

            if ((cr.Height == 0) && (cr.Width == 0))
            {
                sb.Append("Minimized");
            }
            else
            {
                sb.AppendFormat(CultureConstants.DefaultCulture, "{0}x{1}", cr.Width, cr.Height);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the resolution of the specified window.
        /// </summary>
        /// <param name="hWnd">EVE client instance</param>
        /// <returns>Rectangle containing the resolution of the window</returns>
        public static Rectangle GetWindowDimensions(IntPtr hWnd)
        {
            return GetClientRectInScreenCoords(hWnd);
        }

        /// <summary>
        /// Returns the number and resolution of the passed screen number.
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
        /// Gets the state of autorelocation checkbox.
        /// </summary>  
        public static bool AutoRelocationEnabled
        {
            get
            {
                return m_autoRelocation = Settings.UI.MainWindow.EnableAutomaticRelocation;
            }
        }

        /// <summary>
        /// Gets the autorelocate default monitor.
        /// </summary>  
        public static int AutoRelocateDefaultMonitor
        {
            get
            {
                return m_autoRelocateDefaultMonitor = Settings.UI.MainWindow.AutoRelocateDefaultMonitor;
            }
        }

        /// <summary>
        /// Perfoms the AutoRelocation.
        /// </summary>
        private static void AutoRelocate()
        {
            // TODO: Refactor this method into smaller more managable chunks
            // Breaks []wiki:CodingGuidlines]]:
            //  - Small methods (no more than 15 lines) is the ideal.

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

                    // Has the user set a default monitor to relocate ?
                    if (AutoRelocateDefaultMonitor != 0 && AutoRelocateDefaultMonitor <= screenCount)
                    {
                        Relocate(eveInstance, m_autoRelocateDefaultMonitor - 1);
                        return;
                    }

                    // Show the dialog window
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
        /// to which monitor to relocate the client.
        /// </summary>
        private static void ShowDialog(IntPtr eveInstance, int sameResScr)
        {
            float dpi = 0;
            // We create a dialog for the user
            using (var dialog = new EVEMonForm())
            {
                // Calculate the dpi used for better large font support
                using (Graphics graphics = dialog.CreateGraphics())
                {
                    dpi = graphics.DpiX;
                }
                float scale = dpi / 96; 

                int width = 0;
                int height = 0;
                int vpad = (int)(30 * scale);

                // We add a panel to the form
                Panel panel = new Panel();
                panel.Dock = DockStyle.Fill;
                dialog.Controls.Add(panel);

                // Add label
                Label label = new Label();
                label.AutoSize = true;
                label.Text = String.Format(CultureConstants.DefaultCulture, "EVEMon detected that you have more than one{0}monitor with the same resolution."+
                    "\r\rChoose to which monitor to relocate the EVE client.",
                    (sameResScr < 4 ? "\r" : " "));
                panel.Controls.Add(label);
                width += label.Width;
                height += (label.Height + vpad);

                // Add buttons
                int buttonHeight = 0;
                int selectedScreen = 0;
                var buttons = new List<Button>();

                for (int scr = 0; scr <= sameResScr - 1; scr++)
                {
                    int screen = scr;
                    var sectionWidth = (width / sameResScr);
                    var startPoint = sectionWidth * scr;

                    Button button = new Button();
                    button.AutoSize = true;
                    button.Text = "Monitor " + (scr + 1);
                    button.Location = new Point(startPoint + ((sectionWidth - button.Width) / 2), height);
                    buttonHeight = button.Height;
                    buttons.Add(button);
                    panel.Controls.Add(button);

                    // Handles the button press
                    button.Click += (sender, args) =>
                    {
                        selectedScreen = screen;
                        Relocate(eveInstance, screen);
                        dialog.Close();
                    };
                }
                height += (buttonHeight + (vpad / 2));

                // Add checkbox
                CheckBox checkbox = new CheckBox();
                checkbox.AutoSize = true;
                checkbox.TextAlign = ContentAlignment.MiddleLeft;
                checkbox.Text = "Set my choice as the default monitor.";
                checkbox.Location = new Point(10, height);
                panel.Controls.Add(checkbox);
                height += checkbox.Height + (vpad / 2);

                // Sets form properties
                dialog.StartPosition = FormStartPosition.CenterScreen;
                dialog.TopMost = true;

                // Sets the size of the dialog
                // and prevents manual resizing
                int dialogWidth = label.Width + vpad;
                int dialogHeight = height + vpad;

                dialog.Size = new Size(dialogWidth, dialogHeight);
                dialog.MaximumSize = dialog.Size;
                dialog.MinimumSize = dialog.Size;

                // Centers the label to the panel
                label.Location = new Point((panel.Width - label.Width) / 2, 10);

                // Centers the buttons to the panel
                foreach (var button in buttons)
                {
                    button.Location = new Point(label.Location.X + button.Location.X, button.Location.Y);
                }

                // We show the dialog
                dialog.ShowDialog();

                // Sets the autorelocate default monitor
                if (checkbox.Checked)
                {
                    Settings.UI.MainWindow.AutoRelocateDefaultMonitor = selectedScreen + 1;
                    Settings.SaveImmediate();
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Checks whether an AutoRelocation should occur.
        /// </summary>
        /// <param name="interval">The length of time to wait between checks.</param>
        private static void EveClient_TimerTick(object sender, EventArgs e)
        {
            var interval = TimeSpan.FromSeconds(Settings.UI.MainWindow.AutomaticRelocationInterval);
            int m_checkInterval = (int)interval.TotalSeconds;

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
            if (AutoRelocationEnabled)
            {
                EveClient.Trace("AutoRelocation.Enabled");
            }
            else
            {
                EveClient.Trace("AutoRelocation.Disabled");
            }
        }

        #endregion
    }
}