using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common.SettingsObjects;
using System.ComponentModel;

namespace EVEMon.Common.Controls
{
    /// <summary>
    /// This base class provides the common icon shared by all of our forms, along with an optional position storing service.
    /// </summary>
    public partial class EVEMonForm : Form
    {
        delegate void OnLayoutCallback(LayoutEventArgs levent);

        private bool m_loaded;
        private string m_rememberPositionKey = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public EVEMonForm()
        {
            InitializeComponent();
            this.Font = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Regular);
        }

        /// <summary>
        /// Gets or sets a key used to store and restore the position and size of the window. When null or empty, the position won't be persisted.
        /// </summary>
        [Category("Behavior")]
        [Description("A key used to store and restore the position and size of the window. When null or empty, the position won't be persisted.")]
        public string RememberPositionKey
        {
            get { return m_rememberPositionKey; }
            set { m_rememberPositionKey = value; }
        }

        /// <summary>
        /// On load, restores the window rect from the settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RestoreLocation();
            m_loaded = true;
        }

        /// <summary>
        /// On layout, center to screen when requested.
        /// </summary>
        /// <param name="levent"></param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            // Ensure it is called on the correct thread
            if (this.InvokeRequired)
            {
                this.Invoke((OnLayoutCallback)OnLayout, levent);
                return;
            }

            // Center to screen when required
            base.OnLayout(levent);
            if (this.AutoSize && this.StartPosition == FormStartPosition.CenterScreen)
            {
                this.CenterToScreen();
            }
        }

        /// <summary>
        /// On closing, stores the window rect.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveLocation();
            base.OnFormClosed(e);
        }

        /// <summary>
        /// When the size changes, stores the window rect.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            SaveLocation();
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// When the location changes, stores the window rect.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLocationChanged(EventArgs e)
        {
            SaveLocation();
            base.OnLocationChanged(e);
        }

        /// <summary>
        /// Saves the window rect to the settings when the key is not null or empty.
        /// </summary>
        private void SaveLocation()
        {
            if (!m_loaded) return;
            if (String.IsNullOrEmpty(m_rememberPositionKey)) return;

            Rectangle r = new Rectangle(this.Location, this.Size);
            if (this.WindowState == FormWindowState.Normal && VerifyValidWindowLocation(r) == r)
            {
                Settings.UI.WindowLocations[m_rememberPositionKey] = 
                    (SerializableRectangle)new Rectangle(this.Location, this.Size);
            }
        }

        /// <summary>
        /// Restores the window rect from the settings when the key is not null or empty.
        /// </summary>
        private void RestoreLocation()
        {
            if (String.IsNullOrEmpty(m_rememberPositionKey)) return;

            if (Settings.UI.WindowLocations.ContainsKey(m_rememberPositionKey))
            {
                Rectangle r = (Rectangle)Settings.UI.WindowLocations[m_rememberPositionKey];
                r = VerifyValidWindowLocation(r);
                this.SetBounds(r.Left, r.Top, r.Width, r.Height);
            }
        }

        /// <summary>
        /// Verify the window location is validation and trims it when necessary.
        /// </summary>
        /// <param name="inRect">The proposed rectangle.</param>
        /// <returns>The corrected rectangle.</returns>
        private Rectangle VerifyValidWindowLocation(Rectangle inRect)
        {
            Point p = inRect.Location;
            Size s = inRect.Size;
            s.Width = Math.Max(s.Width, this.MinimumSize.Width);
            s.Height = Math.Max(s.Height, this.MinimumSize.Height);

            foreach (Screen ts in Screen.AllScreens)
            {
                if (ts.WorkingArea.Contains(inRect))
                {
                    return inRect;
                }
                if (ts.WorkingArea.Contains(p))
                {
                    p.X = Math.Min(p.X, ts.WorkingArea.Right - s.Width);
                    p.Y = Math.Min(p.Y, ts.WorkingArea.Bottom - s.Height);
                    return new Rectangle(p, s);
                }
            }

            p.X = Screen.PrimaryScreen.WorkingArea.X + 5;
            p.Y = Screen.PrimaryScreen.WorkingArea.Y + 5;
            return new Rectangle(p, s);
        }
    }
}
