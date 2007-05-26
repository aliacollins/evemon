using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;

namespace EVEMon
{
    public partial class Throbber : PictureBox
    {
        public enum ThrobberState
        {
            Stopped,
            Rotating,
            Strobing
        }

        private static Image _strobeFrame = null;
        private static Image[] _movingFrames = null;
        private Image _currentFrame = null;
        private Thread _animatorThread = null;
        private ThrobberState _state = ThrobberState.Stopped;

        private delegate void AnimatorDelegate();

        public ThrobberState State
        {
            get { return _state; }
            set
            {
                if (value == _state)
                    return;

                _state = value;

                if (value == ThrobberState.Stopped)
                {
                    //Stop any running animation thread
                    if (_animatorThread != null)
                    {
                        _animatorThread.Abort();
                        _animatorThread = null;
                    }

                    //Trigger a redraw
                    if (this.IsHandleCreated)
                    {
                        this.Invoke(new AnimatorDelegate(this.Refresh));
                    }
                }
                else if (_animatorThread == null)
                {
                    //Start a new animator thread if necessary
                    _animatorThread = new Thread(new ThreadStart(Animate));
                    _animatorThread.IsBackground = true;
                    _animatorThread.Start();
                }
            }
        }

        public Throbber()
        {
            InitializeComponent();

            if (_strobeFrame == null)
            {
                InitImages();
            }

            this.MinimumSize = new Size(24, 24);
            this.MaximumSize = new Size(24, 24);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_state != ThrobberState.Stopped)
            {
                if (_currentFrame != null) //Draw whatever the animator thread chooses
                {
                    pe.Graphics.DrawImage(_currentFrame, 0, 0);
                }
                //If _currentFrame is null, we're in the blank part of the strobe. Draw nothing, appear transparent
            }
            else
            {
                pe.Graphics.DrawImage(_strobeFrame, 0, 0);
            }

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void Animate()
        {
            int counter = 0;

            while (!this.IsDisposed)
            {
                if (_state == ThrobberState.Strobing)
                {
                    //Switch the strobe frame
                    if (_currentFrame == null)
                    {
                        _currentFrame = _strobeFrame;
                    }
                    else
                    {
                        _currentFrame = null;
                    }

                    //Redraw and sleep
                    if (this.IsHandleCreated)
                    {
                        this.Invoke(new AnimatorDelegate(this.Refresh));
                    }
                    Thread.Sleep(500);
                }
                else if (_state == ThrobberState.Rotating)
                {
                    //Advance to the next frame
                    counter = ++counter % _movingFrames.Length;
                    _currentFrame = _movingFrames[counter];

                    //Redraw and sleep
                    if (this.IsHandleCreated)
                    {
                        this.Invoke(new AnimatorDelegate(this.Refresh));
                    }
                    Thread.Sleep(100);
                }
            }
        }

        private void InitImages()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream s = asm.GetManifestResourceStream("EVEMon.throbber.png"))
            using (Image b = Image.FromStream(s, true, true))
            {
                int width = 24;
                int height = 24;

                //Make the stopped Image
                _strobeFrame = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(_strobeFrame))
                {
                    g.DrawImage(b, new Rectangle(0, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                }

                //Make the moving Images
                _movingFrames = new Image[8];
                for (int i = 1; i < 9; i++)
                {
                    Bitmap ib = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(ib))
                    {
                        g.DrawImage(b, new Rectangle(0, 0, width, height), new Rectangle(i * width, 0, width, height), GraphicsUnit.Pixel);
                    }
                    _movingFrames[i - 1] = ib;
                }
            }
        }
    }
}
