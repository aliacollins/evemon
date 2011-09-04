using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class ReadingPane : UserControl
    {
        private IEveMessage m_selectedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadingPane"/> class.
        /// </summary>
        public ReadingPane()
        {
            InitializeComponent();

            lblMessageHeader.Font = FontFactory.GetDefaultFont(10F, FontStyle.Bold);
            lblSender.Font = FontFactory.GetDefaultFont(10F);
            flPanelHeader.ForeColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        internal IEveMessage SelectedObject
        {
            get { return m_selectedObject; }
            set
            {
                m_selectedObject = value;
                UpdatePane();
            }
        }

        /// <summary>
        /// Hides the reading pane.
        /// </summary>
        internal void HidePane()
        {
            flPanelHeader.Visible = false;
            wbMailBody.Visible = false;
        }

        /// <summary>
        /// Updates the reading pane.
        /// </summary>
        private void UpdatePane()
        {
            // Update the text on the header labels
            lblMessageHeader.Text = m_selectedObject.Title;
            lblSender.Text = m_selectedObject.Sender;
            lblSendDate.Text = String.Format(CultureConstants.DefaultCulture, "Sent: {0:ddd} {0:G}",
                                             m_selectedObject.SentDate.ToLocalTime());
            lblRecipient.Text = String.Format(CultureConstants.DefaultCulture, "To: {0}",
                                              string.Join(", ", m_selectedObject.Recipient));

            // Parce the mail body text to the web browser
            // so for the text to be formatted accordingly
            wbMailBody.DocumentText = TidyUpHTML();

            // We need to wait for the Document to be loaded
            do
            {
                Application.DoEvents();
            } while (wbMailBody.IsBusy);

            // Show the controls
            flPanelHeader.Visible = true;
            wbMailBody.Visible = true;
        }

        /// <summary>
        /// Prepares the text to be shown as common HTML.
        /// </summary>
        /// <returns></returns>
        private string TidyUpHTML()
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();

            Regex regex = new Regex(@"<a\shref=""(.+?)"">(.+?)</a>", RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(m_selectedObject.Text))
            {
                string matchValue = match.Groups[1].Value;
                string matchText = match.Groups[2].Value;
                string url = String.Empty;

                if (matchValue.StartsWith("http://") || matchValue.StartsWith("https://"))
                    url = matchValue;

                if (matchValue.StartsWith("showinfo:"))
                {
                    url = String.Format("{0}{1}", NetworkConstants.EVEGate,
                                        String.Format(NetworkConstants.EveGateCharacterProfile,
                                                      Uri.EscapeUriString(matchText.TrimEnd("<br>".ToCharArray()))));
                }

                replacements[match.ToString()] = String.Format("<a href=\"{0}\" title=\"{0}{1}Click to follow link\">{2}</a>",
                                                               url, Environment.NewLine, matchText);
            }

            Regex regexColor = new Regex(@"color(?:=""|:\s*)#[0-9a-f]{2}([0-9a-f]{6})(?:;|"")", RegexOptions.IgnoreCase);
            foreach (Match match in regexColor.Matches(m_selectedObject.Text))
            {
                replacements[match.ToString()] = String.Format("color=\"#{0}\"", match.Groups[1].Value);
            }

            return replacements.Aggregate(m_selectedObject.Text,
                                   (specialFormattedText, replacement) => specialFormattedText.Replace(replacement.Key, replacement.Value));
        }

        /// <summary>
        /// Every time the mail header panel gets painted we add a line at the bottom.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void flPanelHeader_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the height of the panel
            flPanelHeader.Height = lblMessageHeader.Height + lblSender.Height + lblSendDate.Height + lblRecipient.Height + 10;

            // Draw a line at the bottom of the panel
            using (Graphics g = flPanelHeader.CreateGraphics())
            {
                Pen blackPen = new Pen(Color.Black);
                g.DrawLine(blackPen, 5, flPanelHeader.Height - 1, flPanelHeader.Width - 5, flPanelHeader.Height - 1);
            }
        }

        /// <summary>
        /// Handles the Navigating event of the wbMailBody control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/> instance containing the event data.</param>
        private void wbMailBody_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // We assure that the internal browser will initialize and
            // any other attempt to navigate to a non valid link will fail
            if (e.Url.ToString() == "about:blank" && wbMailBody.DocumentText != m_selectedObject.Text)
                return;

            // If the link complies with HTTP or HTTPS, open the link on the system's default browser
            if (e.Url.ToString().StartsWith("http://") || e.Url.ToString().StartsWith("https://"))
                Util.OpenURL(e.Url.ToString());

            // Prevents the browser to navigate past the shown page
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the wbMailBody control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
        private void wbMailBody_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Disables the reload shortcut key
            wbMailBody.WebBrowserShortcutsEnabled = e.KeyData != Keys.F5;
        }
    }
}