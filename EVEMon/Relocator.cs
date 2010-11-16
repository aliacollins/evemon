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

        private static bool s_initilized;
        private static readonly int s_pid = Application.ProductName.GetHashCode();
        private static readonly List<IntPtr> s_foundWindows = new List<IntPtr>();
        private static int s_autoRelocateDefaultMonitor;
        private static int s_counter;
        private static bool s_autoRelocation;
        private static bool s_dialogActive;

        public static void Initialize()
        {
            if (s_initilized)
                return;

            EveClient.TimerTick += EveClient_TimerTick;
            EveClient.SettingsChanged += EveClient_SettingsChanged;
            EveClient_SettingsChanged(null, EventArgs.Empty);

            s_initilized = true;
        }

        #region Dimensions
        /// <summary>
        /// Get the dimensions of the window specified by hWnd.
        /// </summary>
        /// <param name="hWnd">A valid window</param>
        /// <returns>new Rectangle(Left, Top, Width, Height)</returns>
        private static Rectangle GetWindowRect(this IntPtr hWnd)
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
        internal static Rectangle GetClientRectInScreenCoords(this IntPtr hWnd)
        {
            NativeRelocatorMethods.RECT cr;
            NativeRelocatorMethods.GetClientRect(hWnd, out cr);
            var pt = new NativeRelocatorMethods.POINT
            {
                X = 0,
                Y = 0
            };
            NativeRelocatorMethods.ClientToScreen(hWnd, ref pt);
            return new Rectangle(pt.X, pt.Y, cr.Right - cr.Left, cr.Bottom - cr.Top);
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Callback function for finding all open EVE instance windows.
        /// </summary>
        /// <param name="hwnd">the window information to be tested - automatically passed by EnumWindows</param>
        /// <param name="lParam"></param>
        private static bool EnumWindowCallBack(int hwnd, int lParam)
        {
            var windowHandle = (IntPtr)hwnd;
            var sbText = new StringBuilder(512);
            var sbClass = new StringBuilder(512);
            NativeRelocatorMethods.GetWindowText(windowHandle, sbText, 512);
            NativeRelocatorMethods.GetClassName(windowHandle, sbClass, 512);

            if (sbText.ToString().StartsWith("EVE", StringComparison.CurrentCultureIgnoreCase) && sbClass.ToString() == "triuiScreen")
            {
                int pid;
                NativeRelocatorMethods.GetWindowThreadProcessId(windowHandle, out pid);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);
                if (p.ProcessName == "ExeFile")
                {
                    s_foundWindows.Add(windowHandle);
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
            lock (s_foundWindows)
            {
                s_foundWindows.Clear();
                NativeRelocatorMethods.EnumWindows(EnumWindowCallBack, s_pid);
                return s_foundWindows;
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

            // Turn off dialog frame, border and sizebox atttribute
            int newStyle = oldStyle & ~(NativeRelocatorMethods.WS_DLGFRAME | NativeRelocatorMethods.WS_BORDER | NativeRelocatorMethods.WS_SIZEBOX);

            NativeRelocatorMethods.SetWindowLong(hWnd, NativeRelocatorMethods.GWL_STYLE, newStyle);

            NativeRelocatorMethods.MoveWindow(hWnd, sc.Bounds.X, sc.Bounds.Y, cr.Width, cr.Height, true);
        }

        /// <summary>
        /// Get's the title bar text and resolution of the specified window.
        /// </summary>
        /// <param name="hWnd">Passed EVE client instance</param>
        /// <returns>String containing the title bar text and resolution</returns>
        public static string GetWindowDescription(this IntPtr hWnd)
        {
            var sb = new StringBuilder(512);

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
        /// Returns the number and resolution of the passed screen number.
        /// </summary>
        /// <param name="screen">Integer of the screen to be identified</param>
        /// <returns>Screen z - WidthxHeight</returns>
        public static string GetScreenDescription(this int screen)
        {
            Screen sc = Screen.AllScreens[screen];
            return String.Format(CultureConstants.DefaultCulture, "Screen {0} - {1}x{2}", screen + 1, sc.Bounds.Width, sc.Bounds.Height);
        }

        /// <summary>
        /// Determines whether the specified EVE client instance is relocated.
        /// </summary>
        /// <param name="hWnd">The EVE client instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified EVE client instance is relocated; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRelocated(this IntPtr hWnd)
        {
            Rectangle ncr = hWnd.GetWindowRect();
            Rectangle cr = hWnd.GetClientRectInScreenCoords();
            int wDiff = ncr.Width - cr.Width;
            int hDiff = ncr.Height - cr.Height;

            return (wDiff == 0 && hDiff == 0);
        }

        /// <summary>
        /// Determines whether the specified EVE client instance is minimized.
        /// </summary>
        /// <param name="hWnd">The EVE client instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified EVE client instance is minimized; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMinimized(this IntPtr hWnd)
        {
            Rectangle cr = hWnd.GetClientRectInScreenCoords();
            return (cr.Height == 0 && cr.Width == 0);
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
                return s_autoRelocation = Settings.UI.MainWindow.EnableAutomaticRelocation;
            }
        }

        /// <summary>
        /// Gets the autorelocate default monitor.
        /// </summary>  
        public static int AutoRelocateDefaultMonitor
        {
            get
            {
                return s_autoRelocateDefaultMonitor = Settings.UI.MainWindow.AutoRelocateDefaultMonitor;
            }
        }

        /// <summary>
        /// Perfoms the AutoRelocation.
        /// </summary>
        private static void AutoRelocate()
        {
            int screenCount = Screen.AllScreens.Length;

            foreach (IntPtr eveInstance in FindEveWindows())
            {
                // Skip if null ptr
                if (eveInstance == IntPtr.Zero)
                    continue;

                // We skip if the client is minimized or already relocated
                if (eveInstance.IsMinimized() || eveInstance.IsRelocated())
                    continue;

                int sameResScr = screenCount.GetSameResScreens();

                // More than one monitor with same resolution ?
                if (sameResScr > 1)
                {
                    RelocateOnSameResScreens(eveInstance, screenCount, sameResScr);

                    // We have changed the enumeration
                    // so we need to exit
                    return;
                }
                // Different monitor resolutions ?
                // Relocate to the monitor that fits the client
                else
                {
                    RelocateOnDiffResScreens(eveInstance, screenCount);
                }
            }
        }

        /// <summary>
        /// Relocates the client on same resolution screens.
        /// </summary>
        /// <param name="screenCount">The screen count.</param>
        /// <param name="eveInstance">The eve instance.</param>
        /// <param name="sameResScr">The same res SCR.</param>
        private static void RelocateOnSameResScreens(IntPtr eveInstance, int screenCount, int sameResScr)
        {
            if (!ClientFitsToScreen(eveInstance, screenCount))
                return;

            // Has the user set a default monitor to relocate ?
            if (AutoRelocateDefaultMonitor != 0 && AutoRelocateDefaultMonitor <= screenCount)
            {
                Relocate(eveInstance, s_autoRelocateDefaultMonitor - 1);
                return;
            }

            // Show the dialog window
            if (!s_dialogActive)
            {
                s_dialogActive = true;
                ShowDialog(eveInstance, sameResScr);
                s_dialogActive = false;
            }
        }

        /// <summary>
        /// Relocates the client on different resolution screens.
        /// </summary>
        /// <param name="screenCount">The screen count.</param>
        /// <param name="eveInstance">The eve instance.</param>
        private static void RelocateOnDiffResScreens(IntPtr eveInstance, int screenCount)
        {
            for (int screen = 0; screen < screenCount; screen++)
            {
                // We skip if the client width doesn't match the screen width
                if (eveInstance.GetClientRectInScreenCoords().Width != Screen.AllScreens[screen].Bounds.Width)
                    continue;

                Relocate(eveInstance, screen);
            }
        }

        /// <summary>
        /// Gets the number of same resolution screens.
        /// </summary>
        /// <param name="screenCount">The screen count.</param>
        /// <returns></returns>
        private static int GetSameResScreens(this int screenCount)
        {
            int sameResScr = 0;

            // Check if monitors with same resolution are present
            for (int screen = 0; screen < screenCount; screen++)
            {
                var nextScreen = Math.Min(screen + 1, screenCount - 1);
                if (Screen.AllScreens[screen].Bounds.Size == Screen.AllScreens[nextScreen].Bounds.Size)
                    sameResScr += 1;
            }
            return sameResScr;
        }

        /// <summary>
        /// Checks if the client fits to the screen.
        /// </summary>
        /// <param name="screenCount">The screen count.</param>
        /// <param name="eveInstance">The eve instance.</param>
        /// <returns></returns>
        private static bool ClientFitsToScreen(IntPtr eveInstance, int screenCount)
        {
            // Ensure that the client is the same size as the monitor's resolution
            for (int screen = 0; screen < screenCount; screen++)
            {
                if (eveInstance.GetClientRectInScreenCoords().Width != Screen.AllScreens[screen].Bounds.Width)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Shows a dialog to the user requesting
        /// to which monitor to relocate the client.
        /// </summary>
        private static void ShowDialog(IntPtr eveInstance, int sameResScr)
        {
            float dpi;
            
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

                    var button = new Button();
                    button.AutoSize = true;
                    button.Text = @"Monitor " + (scr + 1);
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
                var checkbox = new CheckBox();
                checkbox.AutoSize = true;
                checkbox.TextAlign = ContentAlignment.MiddleLeft;
                checkbox.Text = @"Set my choice as the default monitor.";
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
        private static void EveClient_TimerTick(object sender, EventArgs e)
        {
            var interval = TimeSpan.FromSeconds(Settings.UI.MainWindow.AutomaticRelocationInterval);
            var checkInterval = (int)interval.TotalSeconds;

            if (AutoRelocationEnabled)
            {
                while (s_counter == checkInterval)
                {
                    s_counter = 0;
                    AutoRelocate();
                }
                s_counter++;
            }
        }

        #endregion

        #region Reaction to settings change

        /// <summary>
        /// Occurs when the settings have changed.
        /// </summary>
        private static void EveClient_SettingsChanged(object sender, EventArgs e)
        {
            string text = String.Format("AutoRelocation.{0}abled", AutoRelocationEnabled ? "En" : "Dis");

            EveClient.Trace(text);
        }

        #endregion
    }
}