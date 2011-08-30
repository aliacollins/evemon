using System;
using System.Drawing;
using System.Windows.Forms;

using CommonProperties = EVEMon.Common.Properties;

namespace EVEMon.Controls
{
    public enum ThrobberState
    {
        Stopped,
        Rotating,
        Strobing
    }

    /// <summary>
    /// The little "flower" displayed on the top right of the characters monitors.
    /// </summary>
    public sealed class Throbber : PictureBox
    {
        // Static members
        private static int s_runners;
        private static Timer s_timer;
        private static Image s_strobeFrame;
        private static Image[] s_movingFrames;

        // Instance members
        private ThrobberState m_state = ThrobberState.Stopped;
        private bool m_running;
        private int m_ticks;

        /// <summary>
        /// Constructor
        /// </summary>
        public Throbber()
        {
            // Initializes the common images
            if (s_strobeFrame == null)
                InitImages();

            // Initializes the common timer
            if (s_timer == null)
                s_timer = new Timer { Interval = 100 };

            // Always subscribed to the timer (ridiculous CPU overhead, less work for the GC with no subscriptions/unsubscriptions, cleaner code)
            s_timer.Tick += TimerTick;

            // Forces the control to be 24*24
            MinimumSize = new Size(24, 24);
            MaximumSize = new Size(24, 24);
        }

        /// <summary>
        /// Gets or sets the throbber's state
        /// </summary>
        public ThrobberState State
        {
            get { return m_state; }
            set
            {
                // Is the state unchanged ? 
                if (value == m_state)
                    return;
                m_state = value;

                // Leave it stopped if not visible
                if (!Visible)
                    return;

                // Start or stop
                switch (m_state)
                {
                    case ThrobberState.Rotating:
                    case ThrobberState.Strobing:
                        Start();
                        Refresh();
                        break;

                    default:
                        Stop();
                        Invalidate();
                        break;
                }
            }
        }

        /// <summary>
        /// Start animating this throbber
        /// </summary>
        private void Start()
        {
            // Always refresh the ticks since we're changing the state or becoming visible
            m_ticks = 0;

            // Is it already running ?
            if (m_running)
                return;
            m_running = true;

            // Start
            s_runners++;
            if (s_runners == 1)
                s_timer.Start();
        }

        /// <summary>
        /// Stop animating this throbber
        /// </summary>
        private void Stop()
        {
            // Is it already stopped ?
            if (!m_running)
                return;
            m_running = false;

            // Stop
            s_runners--;
            if (s_runners == 0)
                s_timer.Stop();
        }

        /// <summary>
        /// occurs 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object sender, EventArgs e)
        {
            // Invalidates the control
            Refresh();
            m_ticks++;
        }

        /// <summary>
        /// Handles the painting
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Image frame = s_strobeFrame;

            // Select the frame to display
            switch (m_state)
            {
                case ThrobberState.Rotating:
                    frame = s_movingFrames[m_ticks % s_movingFrames.Length];
                    break;

                case ThrobberState.Strobing:
                    if ((m_ticks % 10) >= 5)
                        return;
                    break;
            }

            // Draw the selected image
            pe.Graphics.DrawImage(frame, 0, 0);

            // Calling the base method
            base.OnPaint(pe);
        }

        /// <summary>
        /// Any time the visibility change, we may start or stop the timer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            // When not visible, stop
            if (!Visible)
            {
                Stop();
                return;
            }

            // When animated and visible, restart
            if (m_state != ThrobberState.Stopped)
            {
                Start();
            }
        }

        /// <summary>
        /// Initialize the images shared by those controls.
        /// </summary>
        private static void InitImages()
        {
            const int ImageWidth = 24;
            const int ImageHeight = 24;
            Image b = CommonProperties.Resources.Throbber;

            //Make the stopped Image
            s_strobeFrame = new Bitmap(ImageWidth, ImageHeight);
            using (Graphics g = Graphics.FromImage(s_strobeFrame))
            {
                g.DrawImage(b, new Rectangle(0, 0, ImageWidth, ImageHeight), new Rectangle(0, 0, ImageWidth, ImageHeight), GraphicsUnit.Pixel);
            }

            //Make the moving Images
            s_movingFrames = new Image[8];
            for (int i = 1; i < 9; i++)
            {
                Bitmap ib = new Bitmap(ImageWidth, ImageHeight);
                using (Graphics g = Graphics.FromImage(ib))
                {
                    g.DrawImage(b, new Rectangle(0, 0, ImageWidth, ImageHeight), new Rectangle(i * ImageWidth, 0, ImageWidth, ImageHeight),
                                GraphicsUnit.Pixel);
                }
                s_movingFrames[i - 1] = ib;
            }
        }
    }
}
