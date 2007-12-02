using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using EVEMon.Common;
using System.Text.RegularExpressions;

namespace EVEMon
{
    /// <summary>
    /// Popup form displayed when the user hovers over the tray icon
    /// </summary>
    /// <remarks>
    /// Displays all monitored characters, grouped by account, using EVEMon.TrayStatusPopUpChar
    /// for character display details<br/>
    /// Also displays Tranquility status.<br/>
    /// Displayed items are governed by user settings (see EVEMon.SettingsForm).<br/>
    /// Popup location is determined using mouse location, screen and screen bounds (see SetPosition()).<br/>
    /// The form is persisted using a series of display states and a timer:
    /// Opening - the mouse entered the tray icon window, but the window has not yet been displayed. If the
    /// mouse remains over the tray icon for 1 sec, the form is shown and the status changed to visible.
    /// Visible - the form is currently visible. Mouse position is checked every 0.5 sec. Once the mouse moves away
    /// from the tray icon, status is changed to Closing.
    /// Closing - the form is currently visible. If the mouse re-enters the tray icon, status reverts to Visible.
    /// Otherwise the form is closed.
    /// When the popup terminates, a PopupClosed event is published to notify MainWindow that it can clean up
    /// any references.
    /// </remarks>
    public partial class TrayStatusPopUp : Form
    {
        #region Fields
        private Point m_mouseLocation;
        private enum DisplayStatus {Opening, Visible, Closing, Closed};
        private DisplayStatus m_displayStatus;
        private Object mutexLock = new Object();
        private List<CharacterMonitor> m_characters;
        private NotifyIcon m_trayIcon;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public TrayStatusPopUp()
            : this(null, null)
        {
        }

        /// <summary>
        /// Main constructor 
        /// </summary>
        /// <param name="trayIcon">The NotifyIcon for the tray icon. Used to detect mouse events for popup persistence.</param>
        /// <param name="characters">The list of characters to display in the popup.</param>
        public TrayStatusPopUp(NotifyIcon trayIcon, List<CharacterMonitor> characters)
        {
            // Store the current mouse location as a base reference
            m_mouseLocation = Control.MousePosition;
            // Initialisation
            m_characters = characters;
            m_trayIcon = trayIcon;
            InitializeComponent();
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Adds the character panes to the form, gets the TQ status message and sets the popup position
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Get settings
            Settings s = Settings.GetInstance();
            // Organise the character list by account
            SortedList<int, List<CharacterMonitor>> m_accounts = new SortedList<int, List<CharacterMonitor>>();
            if (m_characters != null)
            {
                foreach (CharacterMonitor cm in m_characters)
                {
                    if (!m_accounts.ContainsKey(cm.GrandCharacterInfo.UserId))
                    {
                        m_accounts.Add(cm.GrandCharacterInfo.UserId, new List<CharacterMonitor>());
                    }
                    m_accounts[cm.GrandCharacterInfo.UserId].Add(cm);
                }
            }
            // Now add the characters to the popup, grouped by account
            foreach (int account in m_accounts.Keys)
            {
                // Add a flowlayout to hold the character panels
                FlowLayoutPanel accountPanel = new FlowLayoutPanel();
                accountPanel.AutoSize = true;
                accountPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                accountPanel.FlowDirection = FlowDirection.TopDown;
                accountPanel.BorderStyle = BorderStyle.FixedSingle;
                // Now add the characters
                foreach (CharacterMonitor cm in m_accounts[account])
                {
                    accountPanel.Controls.Add(new TrayStatusPopUpChar(cm));
                }
                // Add the groupbox to the main panel
                pnlCharInfo.Controls.Add(accountPanel);
            }
            // Fix the panel widths to the largest.
            // We let the framework determine the appropriate widths, then fix them so that
            // updates to training time remaining don't cause the form to resize.
            int pnlWidth = 0;
            foreach (Control panel in pnlCharInfo.Controls)
            {
                if (panel is FlowLayoutPanel && panel.Width > pnlWidth)
                {
                    pnlWidth = panel.Width;
                }
            }
            foreach (Control panel in pnlCharInfo.Controls)
            {
                if (panel is FlowLayoutPanel)
                {
                    FlowLayoutPanel flowPanel = panel as FlowLayoutPanel;
                    int pnlHeight = flowPanel.Height;
                    flowPanel.AutoSize = false;
                    flowPanel.Width = pnlWidth;
                    flowPanel.Height = pnlHeight;
                }
            }
            // Set the server status message, trimming the leading slashes
            if (s.TrayPopupShowTQStatus)
            {
                lblTQStatus.Text = Regex.Replace(EveServer.GetInstance().StatusText, @"^// ", String.Empty, RegexOptions.Singleline);
            }
            else
            {
                lblTQStatus.Hide();
            }
            // Position Popup
            SetPosition();
        }

        /// <summary>
        /// Draws the rounded rectangle border and background
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw the border and background
            DrawBorder(e);
        }

        /// <summary>
        /// Replaces the default Show() behaviour.
        /// </summary>
        /// <remarks>
        /// Displaying the form is delayed until the mouse has hovered over the tray icon
        /// for 1 sec. This stops unwanted popups as the mouse moves acrros the tray icon to
        /// access something else.
        /// </remarks>
        new public void Show()
        {
            // Add an event handler so we can track whether the mouse is still over the tray icon
            m_trayIcon.MouseMove += new MouseEventHandler(trayIcon_MouseMove);
            // Initialise the popup status
            m_displayStatus = DisplayStatus.Opening;
            // Start timer. Popup will display after 1 sec provided mouse has not moved away
            displayTimer.Interval = 1000;
            displayTimer.Start();
        }

        /// <summary>
        /// Overrides the default Close() behaviour.
        /// </summary>
        /// <remarks>
        /// Cleans up the timer and event subscriptions, and publishes the PopUpClosed event.
        /// Required since FormClosed is not fired unless the form has been shown.
        /// </remarks>
        new public void Close()
        {
            // Stop the timer
            displayTimer.Stop();
            // Unsubscribe to tray icon events
            m_trayIcon.MouseMove -= new MouseEventHandler(trayIcon_MouseMove);
            // Call default behaviour in case its needed
            base.Close();
            // Now fire the proper closed event
            OnPopUpClosed();
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        #endregion

        #region Event Handler Methods
        /// <summary>
        /// Responds to mouse movement over the tray icon.
        /// </summary>
        /// <remarks>
        /// Updates the mouseLocation field with the current mouse location. Required to test if
        /// the mouse has moved away from the tray icon.<br/>
        /// If the form status is Closing, then change it to Visible to keep it alive.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trayIcon_MouseMove(object sender, MouseEventArgs e)
        {
            // In case we're called from a different thread
            lock (mutexLock)
            {
                // Update the mouse location since we're still over the tray icon
                m_mouseLocation = Control.MousePosition;
                // If we were closing, then reset to Visible
                if (m_displayStatus == DisplayStatus.Closing)
                {
                    m_displayStatus = DisplayStatus.Visible;
                }
            }
        }

        /// <summary>
        /// Controls display of the popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayTimer_Tick(object sender, EventArgs e)
        {
            if (m_displayStatus == DisplayStatus.Opening)
            {
                // Form is not displayed yet, so check if mouse has moved
                if (Control.MousePosition == m_mouseLocation)
                {
                    // No it hasn't so we can display the form
                    base.Show();
                    m_displayStatus = DisplayStatus.Visible;
                    // Set the timer interval to 500ms to track status
                    // This should allow time for any MouseMove events due to the mouse moving
                    // but staying over the notify icon
                    displayTimer.Interval = 500;
                }
                else
                {
                    // Mouse has moved, so assume its moved away and change status to Closing
                    m_displayStatus = DisplayStatus.Closing;
                }
            }
            else if (m_displayStatus == DisplayStatus.Visible)
            {
                if (Control.MousePosition != m_mouseLocation)
                {
                    // Mouse has moved, so assume its moved away and change status to closing
                    m_displayStatus = DisplayStatus.Closing;
                }
            }
            else if (m_displayStatus == DisplayStatus.Closing)
            {
                // The mouse has moved away from the tray icon for more than 0.5 sec, so terminate popup
                Close();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Issued when the popup is terminated
        /// </summary>
        public event EventHandler PopUpClosed;

        /// <summary>
        /// Raises the PopUpClosed event
        /// </summary>
        private void OnPopUpClosed()
        {
            if (PopUpClosed != null)
            {
                PopUpClosed(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Draws the rounded rectangle border
        /// </summary>
        /// <param name="e"></param>
        private void DrawBorder(PaintEventArgs e)
        {
            // Create graphics object to work with
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Define the size of the rectangle used for each of the 4 corner arcs.
            int radius = 5;
            Size cornerSize = new Size(radius * 2, radius * 2);
            // Create 4 rectangles for the arcs to fit in. tl=topleft, br=bottomright
            Rectangle tl = new Rectangle(0, 0, cornerSize.Width, cornerSize.Height);
            Rectangle tr = new Rectangle(e.ClipRectangle.Width - cornerSize.Width, 0, cornerSize.Width, cornerSize.Height);
            Rectangle bl = new Rectangle(0, e.ClipRectangle.Height - cornerSize.Height, cornerSize.Width, cornerSize.Height);
            Rectangle br = new Rectangle(e.ClipRectangle.Width - cornerSize.Width, e.ClipRectangle.Height - cornerSize.Height, cornerSize.Width, cornerSize.Height);
            // Construct a GraphicsPath for the outline
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(tl, 180, 90);
            path.AddArc(tr, 270, 90);
            path.AddArc(br, 0, 90);
            path.AddArc(bl, 90, 90);
            path.CloseFigure();
            // Draw a filled object
            Brush fillBrush = SystemBrushes.Control;
            g.FillPath(fillBrush, path);
            // Draw a border
            Pen borderPen = SystemPens.WindowFrame;
            g.DrawPath(borderPen, path);
        }

        /// <summary>
        /// Set the form location
        /// </summary>
        /// <remarks>
        /// This is derived by working out where the taskbar is, and setting the position accordingly.
        /// </remarks>
        private void SetPosition()
        {
            int xPos = 0;
            int yPos = 0;
            switch (TaskBarEdge)
            {
                case TaskBarEdges.Bottom:
                    xPos = ScreenMouseLocation.X < (TaskBarScreen.Bounds.Width - this.Width) ? ScreenMouseLocation.X : TaskBarScreen.Bounds.Width - this.Width;
                    yPos = TaskBarScreen.WorkingArea.Height - this.Height;
                    break;
                case TaskBarEdges.Top:
                    xPos = ScreenMouseLocation.X < (TaskBarScreen.Bounds.Width - this.Width) ? ScreenMouseLocation.X : TaskBarScreen.Bounds.Width - this.Width;
                    yPos = TaskBarScreen.Bounds.Height - TaskBarScreen.WorkingArea.Height;
                    break;
                case TaskBarEdges.Left:
                    xPos = TaskBarScreen.Bounds.Width - TaskBarScreen.WorkingArea.Width;
                    yPos = ScreenMouseLocation.Y < (TaskBarScreen.Bounds.Height - this.Height) ? ScreenMouseLocation.Y : TaskBarScreen.Bounds.Height - this.Height;
                    break;
                case TaskBarEdges.Right:
                    xPos = TaskBarScreen.WorkingArea.Width - this.Width;
                    yPos = ScreenMouseLocation.Y < (TaskBarScreen.Bounds.Height - this.Height) ? ScreenMouseLocation.Y : TaskBarScreen.Bounds.Height - this.Height;
                    break;
            }
            this.Location = new Point(TaskBarScreen.Bounds.X + xPos, TaskBarScreen.Bounds.Y + yPos);
        }

        /// <summary>
        /// Returns the location of the mouse relative to the task bar screen origin
        /// </summary>
        private Point ScreenMouseLocation
        {
            get
            {
                return new Point(m_mouseLocation.X - TaskBarScreen.Bounds.X, m_mouseLocation.Y - TaskBarScreen.Bounds.Y);
            }
        }

        /// <summary>
        /// Returns the screen where the taskbar is displayed
        /// </summary>
        /// <remarks>
        /// This is derived from the current mouse location, since the mouse must be over the system tray
        /// </remarks>
        private Screen TaskBarScreen
        {
            get
            {
                return Screen.FromPoint(m_mouseLocation);
            }
        }

        private enum TaskBarEdges { Top, Bottom, Left, Right }

        /// <summary>
        /// Derive the screen edge where the taskbar is located
        /// </summary>
        /// <remarks>
        /// This makes two assumptions:
        /// 1. We use the difference between Screen.WorkingArea and Screen.Bounds to establish
        /// whether the taskbar is Left/Right or Top/Bottom
        /// 2. We then use the mouse location to determine which edge it is closest to since
        /// the mouse must be over the system tray
        /// </remarks>
        private TaskBarEdges TaskBarEdge
        {
            get
            {
                if (TaskBarScreen.WorkingArea.Height < TaskBarScreen.Bounds.Height)
                {
                    // TaskBar is either top or bottom
                    int yPosBottom = TaskBarScreen.Bounds.Height - ScreenMouseLocation.Y;
                    // If the mouse location is closer to the bottom than the top, the taskbar must be at the bottom
                    if (yPosBottom < ScreenMouseLocation.Y)
                    {
                        return TaskBarEdges.Bottom;
                    }
                    else
                    {
                        return TaskBarEdges.Top;
                    }
                }
                else
                {
                    // TaskBar must be left or right
                    int xPosRight = TaskBarScreen.Bounds.Width - ScreenMouseLocation.X;
                    // If the mouse location is closer to the right than then left, the taskbar must be at the right
                    if (xPosRight < ScreenMouseLocation.X)
                    {
                        return TaskBarEdges.Right;
                    }
                    else
                    {
                        return TaskBarEdges.Left;
                    }
                }
            }
        }

        #endregion

    }
}
