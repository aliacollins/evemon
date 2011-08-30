using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common.Controls;

namespace EVEMon.Controls
{
    /// <summary>
    /// Wrapper class for the NotifyIcon component. Implements the NotifyIcon properties and events
    /// required for EVEMon usage, and adds MouseHover and MouseLeave events not provided by
    /// the NotifyIcon class.
    /// </summary>
    public partial class TrayIcon : Component
    {
        /// <summary>
        /// Holds the current mouse position state.
        /// The initial state is <see cref="MouseStateOut"/>. See <see cref="TrayIcon.MouseState"/> for more info.
        /// </summary>
        private MouseState m_mouseState;

        private string m_iconText;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayIcon"/> class.
        /// </summary>
        public TrayIcon()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayIcon"/> class with the specfied container.
        /// </summary>
        /// <param name="container">An <see cref="System.ComponentModel.IContainer"/> that represents the container for the <see cref="TrayIcon"/> control.</param>
        public TrayIcon(IContainer container)
        {
            if (container != null)
                container.Add(this);

            InitializeComponent();

            m_mouseState = new MouseStateOut(this);
        }

        #endregion


        #region Local Properties

        /// <summary>
        /// The length of time, in milliseconds, for which the mouse must remain stationary over the control before the MouseHover event is raised.
        /// </summary>
        [Category("Behaviour"),
         Description(
             "The length of time, in milliseconds, for which the mouse must remain stationary over the control before the MouseHover event is raised"
             ),
         DefaultValue(250)]
        public int MouseHoverTime { get; set; }

        #endregion


        #region NotifyIcon properties

        /// <summary>
        /// Gets or sets the shortcut menu associated with the <see cref="TrayIcon"/>.
        /// </summary>
        /// <remarks>
        /// Exposes the value of the underlying <see cref="System.Windows.Forms.NotifyIcon.ContextMenuStrip"/> property.
        /// </remarks>
        [Category("Behaviour")]
        public ContextMenuStrip ContextMenuStrip
        {
            get { return notifyIcon.ContextMenuStrip; }
            set { notifyIcon.ContextMenuStrip = value; }
        }

        /// <summary>
        /// Gets or sets the current icon.
        /// </summary>
        /// <remarks>
        /// Exposes the value of the underlying <see cref="System.Windows.Forms.NotifyIcon.Icon"/> property.
        /// </remarks>
        [Category("Appearance"),
         Description("The icon to display in the system tray")]
        public Icon Icon
        {
            get { return notifyIcon.Icon; }
            set { notifyIcon.Icon = new Icon(value, new Size(16, 16)); }
        }

        /// <summary>
        /// Gets or sets the ToolTip text displayed when the mouse pointer rests on a notification area icon.
        /// </summary>
        /// <remarks>
        /// Exposes the value of the underlying <see cref="System.Windows.Forms.NotifyIcon.Text"/> property.
        /// </remarks>
        [Category("Appearance"),
         Description("The text that will be displayed when the mouse hovers over the icon")]
        public string Text
        {
            get { return notifyIcon.Text; }
            set { notifyIcon.Text = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the icon is visible in the notification area of the taskbar.
        /// </summary>
        /// <remarks>
        /// Exposes the value of the underlying <see cref="System.Windows.Forms.NotifyIcon.Visible"/> property.
        /// </remarks>
        [Category("Behaviour"),
         Description("Determines whether the control is visible or hidden"),
         DefaultValue(false)]
        public bool Visible
        {
            get { return notifyIcon.Visible; }
            set { notifyIcon.Visible = value; }
        }

        #endregion


        #region NotifyIcon Event Handler Methods

        /// <summary>
        /// Propagates the NotifyIcon Click event to our subscribers.
        /// </summary>
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        #endregion


        #region Events

        /// <summary>
        /// Raised when the user clicks the icon in the notification area.
        /// </summary>
        [Category("Action"),
         Description("Occurs when the icon is clicked")]
        public event EventHandler Click;

        /// <summary>
        /// Raised when the mouse pointer remains stationery over the icon in the notification area.
        /// for the length of time specified by <see cref="TrayIcon.MouseHoverTime"/>
        /// </summary>
        [Category("Mouse"),
         Description("Occurs when the mouse remains stationary inside the control for an amount of time")]
        public event EventHandler MouseHover;

        /// <summary>
        /// Raised when the mouse pointer moves away from the icon in the notification area after it has been hovering over the icon.
        /// </summary>
        [Category("Mouse"),
         Description("Occurs when the mouse leaves the visible part of the control")]
        public event EventHandler MouseLeave;

        #endregion


        #region Event methods

        /// <summary>
        /// Helper method to fire events in a thread safe manner.
        /// </summary>
        /// <remarks>
        /// Checks whether subscribers implement <see cref="System.ComponentModel.ISynchronizeInvoke"/> to ensure we raise the
        /// event on the correct thread.
        /// </remarks>
        /// <param name="mainHandler">The <see cref="System.EventHandler"/> for the event to be raised.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> to be passed with the event invocation.</param>
        private void FireEvent(EventHandler mainHandler, EventArgs e)
        {
            // Make sure we have some subscribers
            if (mainHandler == null)
                return;

            // Get each subscriber in turn
            foreach (EventHandler handler in mainHandler.GetInvocationList())
            {
                // Get the object containing the subscribing method
                // If the target doesn't implement ISyncronizeInvoke, this will be null
                ISynchronizeInvoke sync = handler.Target as ISynchronizeInvoke;

                // Check if our target requires an Invoke
                if (sync != null && sync.InvokeRequired)
                {
                    // Yes it does, so invoke the handler using the target's BeginInvoke method, but wait for it to finish
                    // This is preferable to using Invoke so that if an exception is thrown its presented
                    // in the context of the handler, not the current thread
                    IAsyncResult result = sync.BeginInvoke(handler, new object[] {this, e});
                    sync.EndInvoke(result);
                    return;
                }

                // No it doesn't, so invoke the handler directly
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the Click event.
        /// </summary>
        private void OnClick(EventArgs e)
        {
            FireEvent(Click, e);
        }

        /// <summary>
        /// Raises the MouseHover event.
        /// </summary>
        private void OnMouseHover(EventArgs e)
        {
            FireEvent(MouseHover, e);
        }

        /// <summary>
        /// Raises the MouseLeave event.
        /// </summary>
        private void OnMouseLeave(EventArgs e)
        {
            FireEvent(MouseLeave, e);
        }

        #endregion


        #region Static Popup management methods

        /// <summary>
        /// Sets the tool tip location.
        /// </summary>
        /// <param name="tooltipForm">The tooltip form.</param>
        public static void SetToolTipLocation(Form tooltipForm)
        {
            Point mp = Control.MousePosition;
            NativeMethods.AppBarData appBarData = NativeMethods.AppBarData.Create();
            NativeMethods.SHAppBarMessage(NativeMethods.ABM_GETTASKBARPOS, ref appBarData);
            NativeMethods.RECT taskBarLocation = appBarData.rc;

            Screen curScreen = Screen.FromPoint(mp);
            Point winPoint;
            bool slideLeftRight;
            switch (appBarData.uEdge)
            {
                default:
                    winPoint = mp;
                    slideLeftRight = true;
                    break;
                case NativeMethods.ABE_BOTTOM:
                    winPoint = new Point(mp.X, taskBarLocation.Top - tooltipForm.Height);
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
                    winPoint = new Point(taskBarLocation.Left - tooltipForm.Width, mp.Y);
                    slideLeftRight = false;
                    break;
            }

            if (slideLeftRight)
            {
                if (winPoint.X + tooltipForm.Width > curScreen.Bounds.Right)
                    winPoint = new Point(curScreen.Bounds.Right - tooltipForm.Width - 1, winPoint.Y);

                if (winPoint.X < curScreen.Bounds.Left)
                    winPoint = new Point(curScreen.Bounds.Left + 2, winPoint.Y);
            }
            else
            {
                if (winPoint.Y + tooltipForm.Height > curScreen.Bounds.Bottom)
                    winPoint = new Point(winPoint.X, curScreen.Bounds.Bottom - tooltipForm.Height - 1);

                if (winPoint.Y < curScreen.Bounds.Top)
                    winPoint = new Point(winPoint.X, curScreen.Bounds.Top + 2);
            }

            tooltipForm.Location = winPoint;
        }

        #endregion


        #region State Management

        #region Private Abstract Class "MouseState"

        /// <summary>
        /// Abstract base class for monitoring mouse state through the derived concrete classes
        /// </summary>
        /// <remarks>
        /// Provides methods for monitoring mouse position and changing state
        /// </remarks>
        private abstract class MouseState
        {
            /// <summary>
            /// Thread syncronisation lock. Used extensively to enusre that mouseMove event handlers
            /// and thread timer callbacks always have a consistent object state.
            /// </summary>
            protected readonly Object SyncLock = new Object();

            /// <summary>
            /// Flag to determine if mouse tracking enabled
            /// </summary>
            private bool m_mouseTrackingEnabled;

            /// <summary>
            /// Specifies a mouse state
            /// </summary>
            protected enum States
            {
                MouseOut,
                MouseOver,
                MouseHovering
            };


            /// <summary>
            /// Initialises a new instance of the <see cref="MouseState"/> class with the given trayIcon and mousePosition.
            /// </summary>
            /// <param name="trayIcon">The <see cref="Controls.TrayIcon"/> whose state is being managed.</param>
            /// <param name="mousePosition">A <see cref="System.Drawing.Point"/> representing the last known mouse location.</param>
            protected MouseState(TrayIcon trayIcon, Point mousePosition)
            {
                TrayIcon = trayIcon;
                MousePosition = mousePosition;
            }

            /// <summary>
            /// A <see cref="System.Drawing.Point"/> holding the last known mouse position
            /// </summary>
            protected Point MousePosition { get; private set; }

            /// <summary>
            /// The <see cref="Controls.TrayIcon"/> whose MouseState we are managing
            /// </summary>
            protected TrayIcon TrayIcon { get; private set; }

            /// <summary>
            /// Enables the mouse tracking.
            /// </summary>
            protected void EnableMouseTracking()
            {
                lock (SyncLock)
                {
                    // Add event handler for mouse movement
                    TrayIcon.notifyIcon.MouseMove += notifyIcon_MouseMove;
                    m_mouseTrackingEnabled = true;
                }
            }

            /// <summary>
            /// Disables the mouse tracking.
            /// </summary>
            protected void DisableMouseTracking()
            {
                lock (SyncLock)
                {
                    if (!m_mouseTrackingEnabled)
                        return;

                    // Unsubscribe this MouseState from the notify icon MouseMove event
                    TrayIcon.notifyIcon.MouseMove -= notifyIcon_MouseMove;
                    m_mouseTrackingEnabled = false;
                }
            }

            /// <summary>
            /// Event handler to track the position of the mouse over the notification area icon.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void notifyIcon_MouseMove(object sender, MouseEventArgs e)
            {
                // Lock syncLock to ensure that further events block until
                // we've handled this one
                lock (SyncLock)
                {
                    // Only cascade the event if mouse tracking still active
                    if (!m_mouseTrackingEnabled)
                        return;

                    MousePosition = Control.MousePosition;
                    OnMouseMove();
                }
            }

            /// <summary>
            /// Virtual stub overridden by derived classes to capture mouse movement.
            /// </summary>
            protected virtual void OnMouseMove()
            {
            }

            /// <summary>
            /// Changes the state of the parent <see cref="Controls.TrayIcon"/>.
            /// </summary>
            /// <param name="state">A <see cref="Controls.TrayIcon.MouseState.States"/> indicating the state to change to.</param>
            protected void ChangeState(States state)
            {
                // Change the parent TrayIcon's state
                switch (state)
                {
                    case States.MouseOut:
                        TrayIcon.m_mouseState = new MouseStateOut(TrayIcon);
                        break;
                    case States.MouseOver:
                        TrayIcon.m_mouseState = new MouseStateOver(TrayIcon, MousePosition);
                        break;
                    case States.MouseHovering:
                        TrayIcon.m_mouseState = new MouseStateHovering(TrayIcon, MousePosition);
                        break;
                }
            }
        }

        #endregion


        #region Private Class "MouseStateOut"

        /// <summary>
        /// The initial state for mouse tracking.
        /// </summary>
        /// <remarks>
        /// State is changed to <see cref="TrayIcon.MouseStateOver"/> when the mouses moves over the icon.
        /// </remarks>
        private class MouseStateOut : MouseState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TrayIcon.MouseStateOut"/> class for a given trayIcon.
            /// </summary>
            /// <param name="trayIcon">A <see cref="TrayIcon"/> whose state we are managing.</param>
            public MouseStateOut(TrayIcon trayIcon)
                : base(trayIcon, new Point(0, 0))
            {
                EnableMouseTracking();
            }

            /// <summary>
            /// Overrides the base OnMouseMove method to change state to MouseOver when we capture a MouseMove event
            /// from the parent TrayIcon's underlying NotifyIcon.
            /// </summary>
            protected override void OnMouseMove()
            {
                DisableMouseTracking();
                ChangeState(States.MouseOver);
            }

        }

        #endregion


        #region Private Class "MaouseStateOver"

        /// <summary>
        /// Mouse tracking state where the mouse has moved over the tray icon
        /// but we haven't established a hover state. To move to MouseStateHovering
        /// the mouse must remain stationary for the length of time specified
        /// by TrayIcon.MouseHoverTime.
        /// </summary>
        private class MouseStateOver : MouseState
        {
            /// <summary>
            /// A <see cref="System.Threading.Timer"/> used to monitor mouse hover.
            /// </summary>
            private System.Threading.Timer m_timer;

            /// <summary>
            /// Initialises a new instance of the <see cref="MouseState"/> class with the given trayIcon and mousePosition.
            /// </summary>
            /// <param name="trayIcon">The <see cref="TrayIcon"/> whose state is being managed.</param>
            /// <param name="mousePosition">A <see cref="System.Drawing.Point"/> representing the last known mouse location.</param>
            public MouseStateOver(TrayIcon trayIcon, Point mousePosition)
                : base(trayIcon, mousePosition)
            {
                // Store the existing icon text, then reset it
                trayIcon.m_iconText = trayIcon.notifyIcon.Text;
                trayIcon.notifyIcon.Text = String.Empty;

                // Start the timer and enable mouse tracking
                // Lock the syncLock since we don't know the timeout value and need to ensure
                // initialisation completes before the timeout occurs
                lock (SyncLock)
                {
                    // Start the hover timer
                    m_timer = new System.Threading.Timer(HoverTimeout, null, TrayIcon.MouseHoverTime,
                                                         System.Threading.Timeout.Infinite);

                    // Start tracking the mouse
                    EnableMouseTracking();
                }
            }

            /// <summary>
            /// Overrides the base OnMouseMove method to reset the hover timer if the mouse moves while over the notification area icon.
            /// </summary>
            protected override void OnMouseMove()
            {
                try
                {
                    // Mouse has moved, so reset the hover timer
                    m_timer.Change(TrayIcon.MouseHoverTime, System.Threading.Timeout.Infinite);
                }
                catch (ObjectDisposedException)
                {
                    // Swallow any disposed exceptions
                    // Can only occur if timings cause a MouseMove after we've disposed of the timer
                    // Shouldn't happen, but catch it just in case
                }
            }

            /// <summary>
            /// A <see cref="System.Threading.TimerCallback"/> method invoked when the hover timer expires.
            /// </summary>
            /// <remarks>
            /// If the mouse position is unchanged from the last captured mouse position we change to MouseHovering state
            /// otherwise we change to MouseOut.
            /// </remarks>
            /// <param name="state"></param>
            private void HoverTimeout(object state)
            {
                lock (SyncLock)
                {
                    // We may have multiple callbacks pending because the threads in the threadpool were busy waiting for our requests to CCP
                    // As a result, they're going to execute one after the other one, raising ObjectDisposedException when trying to stops the timer
                    if (m_timer == null)
                        return;

                    // Stops and disposes the timer.
                    try
                    {
                        // Stop the timer in case its been restarted by a MouseMove
                        // in the event we were blocked
                        m_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                    finally
                    {
                        // Dispose of the timer since we're done with it
                        m_timer.Dispose();
                        m_timer = null;
                    }

                    // Mouse tracking no longer required
                    DisableMouseTracking();

                    // Check if the mouse is still in the same place
                    // Since we update mousePosition and reset the timer on MouseMove events, if it has moved
                    // when HoverTimeout is called it means its no longer over the icon
                    ChangeState(Control.MousePosition == MousePosition ? States.MouseHovering : States.MouseOut);
                }
            }
        }

        #endregion


        #region Private Class "MouseStateHovering"

        /// <summary>
        /// The hover state reached when the mouse has been stationary over the notification icon
        /// for at least the length of time specified by <see cref="TrayIcon.MouseHoverTime"/>.
        /// Fires the parent TrayIcon's MouseHover event on entry.
        /// </summary>
        /// <remarks>
        /// The mouse position is monitored every 100ms. If the mouse position changes but does not match
        /// the position from the last MouseMove event, we assume the mouse has moved away and fire
        /// the paretn TrayIcon's MouseLeave event.
        /// </remarks>
        private class MouseStateHovering : MouseState
        {
            private readonly System.Threading.Timer m_timer;

            /// <summary>
            /// Initialises a new instance of the <see cref="MouseState"/> class with the given trayIcon and mousePosition.
            /// </summary>
            /// <param name="trayicon">The <see cref="TrayIcon"/> whose state is being managed.</param>
            /// <param name="mousePosition">A <see cref="System.Drawing.Point"/> representing the last known mouse location.</param>
            public MouseStateHovering(TrayIcon trayicon, Point mousePosition)
                : base(trayicon, mousePosition)
            {
                // Fire the MouseHover event
                TrayIcon.OnMouseHover(new EventArgs());

                // Lock the syncLock to make sure the timer is initialised before mouse events are handled
                lock (SyncLock)
                {
                    // Start tracking the mouse
                    EnableMouseTracking();

                    // Start the timer to monitor mouse position
                    m_timer = new System.Threading.Timer(MouseMonitor, null, 100, System.Threading.Timeout.Infinite);
                }
            }

            /// <summary>
            /// A <see cref="System.Threading.TimerCallback"/> method invoked to monitor mouse position.
            /// </summary>
            /// <remarks>
            /// Called every 100ms so long as the mouse does not move.
            /// If the mouse moves, fire the parent TrayIcon's MouseLeave event and changes state to MouseOut.
            /// </remarks>
            /// <param name="state"></param>
            private void MouseMonitor(object state)
            {
                lock (SyncLock)
                {
                    if (Control.MousePosition == MousePosition)
                    {
                        // Mouse hasn't moved so check back in 100ms
                        m_timer.Change(100, System.Threading.Timeout.Infinite);
                        return;
                    }

                    // Mouse has moved, and since we're tracking it over the icon
                    // this means its moved away
                    // Dispose of the timer since we're done with it
                    m_timer.Dispose();

                    // Switch of mouse tracking
                    DisableMouseTracking();

                    // Restore the default icon text
                    TrayIcon.notifyIcon.Text = TrayIcon.m_iconText;

                    // Fire the MouseLeave event
                    TrayIcon.OnMouseLeave(new EventArgs());

                    // Change to MouseOut state
                    ChangeState(States.MouseOut);
                }
            }
        }

        #endregion

        #endregion

    }
}
