using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public sealed partial class TipWindow : UserControl
    {
        private static readonly object s_lockObject = new object();
        private readonly string m_key;

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
        /// <param name="checkboxVisible">if set to <c>true</c> the checkbox is visible.</param>
        private TipWindow(string title, string tiptext, string key, bool checkboxVisible)
            : this()
        {
            Text = title;
            TipLabel.Text = tiptext;
            m_key = key;
            cbDontShowAgain.Visible = checkboxVisible;
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
            pictureBox.Image = SystemIcons.Information.ToBitmap();
        }

        /// <summary>
        /// Show a "tip of the day"-like message on the top right corner of the given window.
        /// </summary>
        /// <param name="form">The owner window.</param>
        /// <param name="key">The key used to store informations about messages the user already saw. Every messages is only displayed once.</param>
        /// <param name="title">The title of the tip window.</param>
        /// <param name="tiptext">The text of the tip window.</param>
        /// <param name="checkBoxVisible">if set to <c>true</c> the checkbox is visible.</param>
        public static void ShowTip(Form form, string key, string title, string tiptext, bool checkBoxVisible = true)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            lock (s_lockObject)
            {
                if (Settings.UI.ConfirmedTips.Contains(key))
                    return;

                // Quit if it's already shown
                if (form.Controls.OfType<TipWindow>().Any())
                    return;

                TipWindow tipWindow = new TipWindow(title, tiptext, key, checkBoxVisible);
                form.Controls.Add(tipWindow);

                // Aligns the top right corner of the tip window with the top right corner of the owner's client rectangle
                tipWindow.Location = new Point(form.ClientRectangle.Left + form.ClientSize.Width - tipWindow.Width,
                                        form.ClientRectangle.Top);
                tipWindow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                tipWindow.BringToFront();
                tipWindow.Show();

                Settings.Save();
            }
        }
    }
}