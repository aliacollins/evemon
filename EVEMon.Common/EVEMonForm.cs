using System;
using System.Drawing;
using System.Windows.Forms;

namespace EVEMon.Common
{
    public partial class EVEMonForm : Form
    {
        public EVEMonForm()
        {
            InitializeComponent();
            this.Font = FontHelper.GetFont("Tahoma", 8.25F, FontStyle.Regular);
        }

        private string m_rememberPositionKey = null;

        public string RememberPositionKey
        {
            get { return m_rememberPositionKey; }
            set { m_rememberPositionKey = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!String.IsNullOrEmpty(m_rememberPositionKey))
            {
                Settings s = Settings.GetInstance();
                if (s.SavedWindowLocations.ContainsKey(m_rememberPositionKey))
                {
                    Rectangle r = s.SavedWindowLocations[m_rememberPositionKey];
                    r = VerifyValidWindowLocation(r);
                    this.SetBounds(r.Left, r.Top, r.Width, r.Height);
                }
            }
        }

        delegate void OnLayoutCallback(LayoutEventArgs levent);

        protected override void OnLayout(LayoutEventArgs levent)
        {
            // If the calling thread is not the creating thread then need to be thread safe
            if (this.InvokeRequired)
            {
                OnLayoutCallback d = new OnLayoutCallback(OnLayout);
                this.Invoke(d, levent);
            }
            else
            {
                base.OnLayout(levent);

                if (this.AutoSize && this.StartPosition == FormStartPosition.CenterScreen)
                {
                    this.CenterToScreen();
                }
            }

        }

        //protected override void OnShown(EventArgs e)
        //{
        //    base.OnShown(e);

        //    if (this.AutoSize && this.StartPosition == FormStartPosition.CenterScreen)
        //    {
        //        this.CenterToScreen();
        //    }
        //}

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (!String.IsNullOrEmpty(m_rememberPositionKey))
            {
                Settings s = Settings.GetInstance();
                Rectangle r = new Rectangle(this.Location, this.Size);
                if (this.WindowState == FormWindowState.Normal && VerifyValidWindowLocation(r) == r)
                {
                    s.SavedWindowLocations[m_rememberPositionKey] =
                        new Rectangle(this.Location, this.Size);
                }
            }

            base.OnFormClosed(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!String.IsNullOrEmpty(m_rememberPositionKey))
            {
                Settings s = Settings.GetInstance();
                Rectangle r = new Rectangle(this.Location, this.Size);
                if (this.WindowState == FormWindowState.Normal && VerifyValidWindowLocation(r) == r)
                {
                    s.SavedWindowLocations[m_rememberPositionKey] =
                        new Rectangle(this.Location, this.Size);
                }
            }
            
            base.OnSizeChanged(e);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            if (!String.IsNullOrEmpty(m_rememberPositionKey))
            {
                Settings s = Settings.GetInstance();
                Rectangle r = new Rectangle(this.Location, this.Size);
                if (this.WindowState == FormWindowState.Normal && VerifyValidWindowLocation(r) == r)
                {
                    s.SavedWindowLocations[m_rememberPositionKey] =
                        new Rectangle(this.Location, this.Size);
                }
            }

            base.OnLocationChanged(e);
        }

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
