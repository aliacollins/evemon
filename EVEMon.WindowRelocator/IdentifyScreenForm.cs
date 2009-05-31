using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.WindowRelocator
{
    public partial class IdentifyScreenForm : Form
    {
        public IdentifyScreenForm()
        {
            InitializeComponent();
        }

        private int m_number = 0;

        public int Number
        {
            get { return m_number; }
            set { m_number = value; }
        }

        private static IdentifyScreenForm[] m_displayedForms = null;

        public static void Display()
        {
            m_displayedForms = new IdentifyScreenForm[Screen.AllScreens.Length];

            int screenNumber = 1;
            foreach (Screen thisScreen in Screen.AllScreens)
            {
                IdentifyScreenForm f = new IdentifyScreenForm();
                f.Number = screenNumber;
                f.Show();

                m_displayedForms[screenNumber - 1] = f;
                screenNumber++;
            }
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 Thread.Sleep(TimeSpan.FromSeconds(5));
                                                 ClearDisplay();
                                             });
        }

        private static void ClearDisplay()
        {
            if (m_displayedForms == null)
            {
                return;
            }
            if (m_displayedForms[0].InvokeRequired)
            {
                m_displayedForms[0].Invoke(new MethodInvoker(ClearDisplay));
                return;
            }
            foreach (IdentifyScreenForm f in m_displayedForms)
            {
                f.Close();
            }
            m_displayedForms = null;
        }

        private const int STROKE_SIZE = 4;

        private void IdentifyScreenForm_Load(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(pbScreenNumber.ClientSize.Width,
                                  pbScreenNumber.ClientSize.Height);
            using (Graphics g = Graphics.FromImage(b))
            using (Font tf = FontHelper.GetFont("Tahoma", b.Height - STROKE_SIZE, GraphicsUnit.Pixel))
            using (GraphicsPath p = new GraphicsPath())
            using (Pen sp = new Pen(Color.Black, Convert.ToSingle(STROKE_SIZE)))
            {
                g.FillRectangle(Brushes.Magenta, new Rectangle(new Point(0, 0), b.Size));

                p.AddString(m_number.ToString(), new FontFamily("Tahoma"), (int) FontStyle.Bold,
                            tf.SizeInPoints, new Point(0, STROKE_SIZE/2), StringFormat.GenericTypographic);
                RectangleF bounds = p.GetBounds();
                p.Reset();
                p.AddString(m_number.ToString(), new FontFamily("Tahoma"), (int) FontStyle.Bold,
                            tf.SizeInPoints, new Point(Convert.ToInt32((b.Width/2) - (bounds.Width/2)), STROKE_SIZE/2),
                            StringFormat.GenericTypographic);

                g.FillPath(Brushes.White, p);
                g.DrawPath(sp, p);
            }

            pbScreenNumber.Image = b;

            Screen ts = Screen.AllScreens[m_number - 1];
            this.Location = new Point(
                (ts.Bounds.Width/2) - (this.Width/2) + ts.Bounds.X,
                (ts.Bounds.Height/2) - (this.Height/2) + ts.Bounds.Y);
        }
    }
}