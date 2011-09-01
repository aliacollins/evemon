using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public sealed partial class TipWindow : UserControl
    {
        private readonly string m_key;
        private static readonly object s_lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="TipWindow"/> class.
        /// </summary>
        private TipWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TipWindow"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="tiptext">The tiptext.</param>
        /// <param name="key">The key.</param>
        private TipWindow(string title, string tiptext, string key)
            : this()
        {
            Text = title;
            label1.Text = tiptext;
            m_key = key;
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cbDontShowAgain.Checked)
                Settings.UI.ConfirmedTips.Add(m_key);

            Parent.Controls.Remove(this);
        }

        /// <summary>
        /// Handles the Load event of the TipWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TipWindow_Load(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.DrawIcon(SystemIcons.Information, 0, 0);
            }
            pictureBox1.Image = b;
        }

        /// <summary>
        /// Show a "tip of the day"-like message on the top right corner of the given window.
        /// </summary>
        /// <param name="form">The owner window.</param>
        /// <param name="key">The key used to store informations about messages the user already saw. Every messages is only displayed once.</param>
        /// <param name="title">The title of the tip window.</param>
        /// <param name="tiptext">The text of the tip window.</param>
        public static void ShowTip(Form form, string key, string title, string tiptext)
        {
            lock (s_lockObject)
            {
                if (Settings.UI.ConfirmedTips.Contains(key))
                    return;

                TipWindow tw = new TipWindow(title, tiptext, key);
                form.Controls.Add(tw);

                // Aligns the top right corner of the tip window with the top right corner of the owner's client rectangle.
                tw.Location = new Point(form.ClientRectangle.Left + form.ClientSize.Width - tw.Width,
                                        form.ClientRectangle.Top);
                tw.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                tw.BringToFront();
                tw.Show();

                Settings.Save();
            }
        }
    }
}