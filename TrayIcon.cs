using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace EVEMon
{
    /// <summary>
    /// Wrapper class for the NotifyIcon component. Implements the NotifyIcon properties and events
    /// required for EveMon usage, and adds MouseHover and MouseLeave events not provided by
    /// the NotifyIcon class
    /// </summary>
    public partial class TrayIcon : Component
    {
        /// <summary>
        /// Holds the current mouse position state. The initial state is <see cref="TrayIcon.MouseOut"/> See <see cref="TrayIcon.MouseState"/> for more info.
        /// </summary>
        private MouseState mouseState;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EVEMon.TrayIcon"/> class.
        /// </summary>
        public TrayIcon() : this(null) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="EVEMon.TrayIcon"/> class with the specfied container.
        /// </summary>
        /// <param name="container">An <see cref="System.ComponentModel.IContainer"/> that represents the container for the <see cref="EVEMon.TrayIcon"/> control.</param>
        public TrayIcon(IContainer container)
        {
            if (container != null)
                container.Add(this);
            InitializeComponent();
            this.mouseState = new MouseStateOut(this);
        }
        #endregion

        #region Local Properties
        private int mouseHoverTime = 250;
        /// <summary>
        /// The length of time, in milliseconds, for which the mouse must remain stationary over the control before the MouseHover event is raised.
        /// </summary>
        [Category("Behaviour"),
            Description("The length of time, in milliseconds, for which the mouse must remain stationary over the control before the MouseHover event is raised"),
            DefaultValue(250)]
        public int MouseHoverTime
        {
            get { return mouseHoverTime; }
            set { mouseHoverTime = value; }
        }
        #endregion

        #region NotifyIcon properties
        /// <summary>
        /// Gets or sets the shortcut menu associated with the <see cref="EVEMon.TrayIcon"/>.
        /// </summary>
        /// <remarks>
        /// Exposes the value of the underlying <see cref="System.Windows.Forms.NotifyIcon.ContextMenuStrip"/> property.
        /// </remarks>
        [Category ("Behaviour")]
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
        [Category ("Appearance"), Description("The icon to display in the system tray")]
        public Icon Icon
        {
            get { return notifyIcon.Icon; }
            set { notifyIcon.Icon = value; }
        }

        /// <summary>
        /// Gets or sets the ToolTip text displayed when the mouse pointer rests on a notification area icon.
        /// </summary>
        /// <remarks>
        /// Exposes the value of the underlying <see cref="System.Windows.Forms.NotifyIcon.Text"/> property.
        /// </remarks>
        [Category("Appearance"), Description("The text that will be displayed when the mouse hovers over the icon")]
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
        [Category("Behaviour"), Description("Determines whether the control is visible or hidden"), DefaultValue(false)]
        public bool Visible
        {
            get { return notifyIcon.Visible; }
            set { notifyIcon.Visible = value; }
        }
        #endregion

        #region NotifyIcon Event Handler Methods
        /// <summary>
        /// Propagates the NotifyIcon Click event to our subscribers
        /// </summary>
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when the user clicks the icon in the notification area
        /// </summary>
        [Category("Action"), Description("Occurs when the icon is clicked")]
        public event EventHandler Click;

        /// <summary>
        /// Raised when the mouse pointer remains stationery over the icon in the notification area for the length of time specified by <see cref="EVEMon.TrayIcon.MouseHoverTime"/>
        /// </summary>
        [Category("Mouse"), Description("Occurs when the mouse remains stationary inside the control for an amount of time")]
        public event EventHandler MouseHover;

        /// <summary>
        /// Raised when the mouse pointer moves away from the icon in the notification area after it has been hovering over the icon
        /// </summary>
        [Category("Mouse"), Description("Occurs when the mouse leaves the visible part of the control")]
        public event EventHandler MouseLeave;
        #endregion

        #region Event methods
        /// <summary>
        /// Helper method to fire events in a thread safe manner.
        /// </summary>
        /// <remarks>
        /// Checks whether subscribers implement <see cref="System.ComponentModel.ISyncronizeInvoke"/> to ensure we raise the
        /// event on the correct thread.
        /// </remarks>
        /// <param name="mainHandler">The <see cref="System.EventHandler"/> for the event to be raised.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> to be passed with the event invocation.</param>
        private void FireEvent(EventHandler mainHandler, EventArgs e)
        {
            // Make sure we have some subscribers
            if (mainHandler != null)
            {
                // Get each subscriber in turn
                foreach (EventHandler handler in mainHandler.GetInvocationList())
                {
                    // Get the object containing the subscribing method
                    // If the target doesn't implement ISyncronizeInvoke, this will be null
                    ISynchronizeInvoke sync = handler.Target as ISynchronizeInvoke;

                    // Check if our target requires an Invoke due to running on a different thread
                    if (sync != null && sync.InvokeRequired)
                    {
                        // Yes it does, so invoke the handler using the target's Invoke method
                        sync.Invoke(handler, new object[] { this, e });
                    }
                    else
                    {
                        // No it doesn't, so invoke the handler directly
                        handler(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the Click event
        /// </summary>
        protected virtual void OnClick(EventArgs e)
        {
            FireEvent(this.Click, e);
        }

        /// <summary>
        /// Raises the MouseHover event
        /// </summary>
        protected virtual void OnMouseHover(EventArgs e)
        {
            FireEvent(this.MouseHover, e);
        }

        /// <summary>
        /// Raises the MouseLeave event
        /// </summary>
        protected virtual void OnMouseLeave(EventArgs e)
        {
            FireEvent(this.MouseLeave, e);
        }
        #endregion

        #region State Management
        /// <summary>
        /// Abstract base class for monitoring mouse state through the derived concrete classes
        /// </summary>
        /// <remarks>
        /// Provides methods for monitoring mouse position and changing state
        /// </remarks>
        private abstract class MouseState
        {
            /// <summary>
            /// A <see cref="System.Drawing.Point"/> holding the last known mouse position
            /// </summary>
            protected Point mousePosition;

            /// <summary>
            /// The <see cref="EVEMon.TrayIcon"/> whose MouseState we are managing
            /// </summary>
            protected TrayIcon trayIcon;

            /// <summary>
            /// Specifies a mouse state
            /// </summary>
            protected enum States { MouseOut, MouseOver, MouseHovering };

            /// <summary>
            /// Thread syncronisation lock
            /// </summary>
            protected Object syncLock;

            /// <summary>
            /// Initialises a new instance of the <see cref="EVEMon.MouseState"/> class with the given trayIcon and mousePosition
            /// </summary>
            /// <param name="trayIcon">The <see cref="EVEMon.TrayIcon"/> whose state is being managed.</param>
            /// <param name="mousePosition">A <see cref="System.Drawing.Point"/> representing the last known mouse location.</param>
            public MouseState(TrayIcon trayIcon, Point mousePosition)
            {
                this.trayIcon = trayIcon;
                this.mousePosition = mousePosition;
                syncLock = new Object();
                // Add event handler for mouse movement
                this.trayIcon.notifyIcon.MouseMove += new MouseEventHandler(notifyIcon_MouseMove);
            }

            /// <summary>
            /// Event handler to track the position of the mouse over the notification area icon.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void notifyIcon_MouseMove(object sender, MouseEventArgs e)
            {
                lock (syncLock)
                {
                    this.mousePosition = Control.MousePosition;
                }
                OnMouseMove();
            }

            /// <summary>
            /// Virtual stub overridden by derived classes to capture mouse movement
            /// </summary>
            protected virtual void OnMouseMove() { }

            /// <summary>
            /// Changes the state of the parent <see cref="EVEMon.TrayIcon"/>
            /// </summary>
            /// <param name="state">A <see cref="EVEMon.TrayIcon.MouseState.States"/> indicating the state to change to.</param>
            protected void ChangeState(States state)
            {
                // Unsubscribe this MouseState from the notify icon MouseMove event
                this.trayIcon.notifyIcon.MouseMove -= new MouseEventHandler(notifyIcon_MouseMove);
                // Change the parent TrayIcon's state
                switch (state)
                {
                    case States.MouseOut:
                        this.trayIcon.mouseState = new MouseStateOut(this.trayIcon);
                        break;
                    case States.MouseOver:
                        this.trayIcon.mouseState = new MouseStateOver(this.trayIcon, this.mousePosition);
                        break;
                    case States.MouseHovering:
                        this.trayIcon.mouseState = new MouseStateHovering(this.trayIcon, this.mousePosition);
                        break;
                }
            }

        }

        /// <summary>
        /// The initial state for mouse tracking
        /// </summary>
        /// <remarks>
        /// State is changed to <see cref="EVEMon.TrayIcon.MouseStateOver"/> when the mouses moves over the icon
        /// </remarks>
        private class MouseStateOut : MouseState
        {
            
            /// <summary>
            /// Initializes a new instance of the <see cref="EVEMon.TrayIcon.MouseStateOut"/> class for a given trayIcon.
            /// </summary>
            /// <param name="trayIcon">A <see cref="EVEMon.TrayIcon"/> whose state we are managing.</param>
            public MouseStateOut(TrayIcon trayIcon) : base(trayIcon, new Point(0,0))
            {
            }
            
            /// <summary>
            /// Overrides the base OnMouseMove method to change state to MouseOver when we capture a MouseMove event
            /// fro the parent TrayIcon's underlying NotifyIcon.
            /// </summary>
            protected override void OnMouseMove()
            {
                ChangeState(States.MouseOver);
            }

        }

        /// <summary>
        /// Mouse tracking state where the mouse has moved over the tray icon
        /// but we haven't established a hover state. To move to MouseStateHovering
        /// the mouse must remain stationary for the length of time specified
        /// by TrayIcon.MouseHoverTime
        /// </summary>
        private class MouseStateOver : MouseState
        {
            /// <summary>
            /// A <see cref="System.Threading.Timer"/> used to monitor mouse hover
            /// </summary>
            private System.Threading.Timer timer;

            /// <summary>
            /// Initialises a new instance of the <see cref="EVEMon.MouseState"/> class with the given trayIcon and mousePosition
            /// </summary>
            /// <param name="trayIcon">The <see cref="EVEMon.TrayIcon"/> whose state is being managed.</param>
            /// <param name="mousePosition">A <see cref="System.Drawing.Point"/> representing the last known mouse location.</param>
            public MouseStateOver(TrayIcon trayIcon, Point mousePosition)
                : base(trayIcon, mousePosition)
            {
                // Start the hover timer
                this.timer = new System.Threading.Timer(new System.Threading.TimerCallback(HoverTimeout), null, this.trayIcon.MouseHoverTime, System.Threading.Timeout.Infinite);
            }

            /// <summary>
            /// Overrides the base OnMouseMove method to reset the hover timer if the mouse moves while over the notification area icon.
            /// </summary>
            protected override void OnMouseMove()
            {
                try
                {
                    // Mouse has moved, so reset the hover timer
                    this.timer.Change(this.trayIcon.MouseHoverTime, System.Threading.Timeout.Infinite);
                }
                catch (ObjectDisposedException)
                {
                    // Swallow any dispoed exceptions
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
                // Check if the mouse is still in the same place
                // Since we update mousePosition and reset the timer on MouseMove events, if it has moved
                // when HoverTimeout is called it means its no longer over the icon
                Point mousePosition;
                lock (syncLock)
                {
                    mousePosition = this.mousePosition;
                }
                if (Control.MousePosition == mousePosition)
                {
                    ChangeState(States.MouseHovering);
                }
                else
                {
                    ChangeState(States.MouseOut);
                }
                // Dispose of the timer since we're done with it
                this.timer.Dispose();
            }
        }

        /// <summary>
        /// The hover state reached when the mouse has been stationary over the notification icon
        /// for at least the length of time specified by <see cref="EVEMon.TrayIcon.MouseHoverTime"/>.
        /// Fires the parent TrayIcon's MouseHover event on entry.
        /// </summary>
        /// <remarks>
        /// The mouse position is monitored every 100ms. If the mouse position changes but does not match
        /// the position from the last MouseMove event, we assume the mouse has moved away and fire
        /// the paretn TrayIcon's MouseLeave event.
        /// </remarks>
        private class MouseStateHovering : MouseState
        {
            private System.Threading.Timer timer;

            /// <summary>
            /// Initialises a new instance of the <see cref="EVEMon.MouseState"/> class with the given trayIcon and mousePosition
            /// </summary>
            /// <param name="trayIcon">The <see cref="EVEMon.TrayIcon"/> whose state is being managed.</param>
            /// <param name="mousePosition">A <see cref="System.Drawing.Point"/> representing the last known mouse location.</param>
            public MouseStateHovering(TrayIcon trayicon, Point mousePosition)
                : base(trayicon, mousePosition)
            {
                // Fire the MouseHover event
                trayIcon.OnMouseHover(new EventArgs());
                // Start the timer to monitor mouse position
                this.timer = new System.Threading.Timer(new System.Threading.TimerCallback(MouseMonitor), null, 100, System.Threading.Timeout.Infinite);
            }

            /// <summary>
            /// A <see cref="System.Threading.TimerCallback"/> method invoked to monitor mouse position.
            /// </summary>
            /// <remarks>
            /// Called every 100ms so long as the mouse does not move. If the mouse moves, fire the parent TrayIcon's MouseLeave event
            /// and changes state to MouseOut
            /// </remarks>
            /// <param name="state"></param>
            private void MouseMonitor(object state)
            {
                Point mousePosition;
                lock (syncLock)
                {
                    mousePosition = this.mousePosition;
                }
                if (Control.MousePosition == mousePosition)
                {
                    // Mouse hasn't moved so check back in 100ms
                    this.timer.Change(100, System.Threading.Timeout.Infinite);
                }
                else
                {
                    // Mouse has moved, and since we're tracking it over the icon
                    // this means its moved away
                    // Fire the MouseLeave event
                    this.trayIcon.OnMouseLeave(new EventArgs());
                    // Change to MouseOut state
                    ChangeState(States.MouseOut);
                    // Dispose of the timer since we're done with it
                    this.timer.Dispose();
                }
            }
        }
        #endregion

        #region Static Popup management methods
        /// <summary>
        /// Set the form location
        /// </summary>
        /// <remarks>
        /// This is derived by working out where the taskbar is, and setting the position accordingly.
        /// </remarks>
        public static void SetToolTipLocation(Form tooltipForm)
        {
            int xPos = 0;
            int yPos = 0;
            switch (TaskBarEdge)
            {
                case TaskBarEdges.Bottom:
                    xPos = ScreenMouseLocation.X < (TaskBarScreen.Bounds.Width - tooltipForm.Width) ? ScreenMouseLocation.X : TaskBarScreen.Bounds.Width - tooltipForm.Width;
                    yPos = TaskBarScreen.WorkingArea.Height - tooltipForm.Height;
                    break;
                case TaskBarEdges.Top:
                    xPos = ScreenMouseLocation.X < (TaskBarScreen.Bounds.Width - tooltipForm.Width) ? ScreenMouseLocation.X : TaskBarScreen.Bounds.Width - tooltipForm.Width;
                    yPos = TaskBarScreen.Bounds.Height - TaskBarScreen.WorkingArea.Height;
                    break;
                case TaskBarEdges.Left:
                    xPos = TaskBarScreen.Bounds.Width - TaskBarScreen.WorkingArea.Width;
                    yPos = ScreenMouseLocation.Y < (TaskBarScreen.Bounds.Height - tooltipForm.Height) ? ScreenMouseLocation.Y : TaskBarScreen.Bounds.Height - tooltipForm.Height;
                    break;
                case TaskBarEdges.Right:
                    xPos = TaskBarScreen.WorkingArea.Width - tooltipForm.Width;
                    yPos = ScreenMouseLocation.Y < (TaskBarScreen.Bounds.Height - tooltipForm.Height) ? ScreenMouseLocation.Y : TaskBarScreen.Bounds.Height - tooltipForm.Height;
                    break;
            }
            tooltipForm.Location = new Point(TaskBarScreen.Bounds.X + xPos, TaskBarScreen.Bounds.Y + yPos);
        }

        /// <summary>
        /// Returns the location of the mouse relative to the task bar screen origin
        /// </summary>
        private static Point ScreenMouseLocation
        {
            get
            {
                Point mousePosition = Control.MousePosition;
                return new Point(mousePosition.X - TaskBarScreen.Bounds.X, mousePosition.Y - TaskBarScreen.Bounds.Y);
            }
        }

        /// <summary>
        /// Returns the screen where the taskbar is displayed
        /// </summary>
        /// <remarks>
        /// This is derived from the current mouse location, since the mouse must be over the system tray
        /// </remarks>
        private static Screen TaskBarScreen
        {
            get
            {
                return Screen.FromPoint(Control.MousePosition);
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
        private static TaskBarEdges TaskBarEdge
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
