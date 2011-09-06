using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Data;

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
            Visible = false;
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
            Visible = ((m_selectedObject is EveMailMessage) && ((EveMailMessage)m_selectedObject).EVEMailBody.MessageID != 0)
                      || ((m_selectedObject is EveNotification) &&
                          ((EveNotification)m_selectedObject).EVENotificationText.NotificationID != 0);
        }

        /// <summary>
        /// Prepares the text to be shown as common HTML.
        /// </summary>
        /// <returns></returns>
        private string TidyUpHTML()
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();

            FormatLinks(replacements);

            FormatColorToRGB(replacements);

            return replacements.Aggregate(m_selectedObject.Text,
                                          (specialFormattedText, replacement) =>
                                          specialFormattedText.Replace(replacement.Key, replacement.Value));
        }

        /// <summary>
        /// Formats the links.
        /// </summary>
        /// <param name="replacements">The replacements.</param>
        private void FormatLinks(IDictionary<string, string> replacements)
        {
            // Regural expression for all HTML links
            Regex regexLinks = new Regex(@"<a\shref=""(.+?)"">(.+?)</a>", RegexOptions.IgnoreCase);

            // Regural expression for all showinfo URLs
            Regex regexShowInfo = new Regex(@"^showinfo:(\d+)//(\d+)$");

            // Regural expression for clickable/valid URLs
            Regex regexWebProtocol = new Regex(@"(?:f|ht)tps?://", RegexOptions.IgnoreCase);

            foreach (Match match in regexLinks.Matches(m_selectedObject.Text))
            {
                string matchValue = match.Groups[1].Value;
                string matchText = match.Groups[2].Value;
                string url = String.Empty;
                Match showInfoMatch = regexShowInfo.Match(matchValue);
                bool igbOnly = false;

                if (regexWebProtocol.IsMatch(matchValue))
                    url = matchValue;
                else if (showInfoMatch.Success)
                {
                    long typeID = Convert.ToInt64(showInfoMatch.Groups[1].Value);

                    if (typeID >= DBConstants.CharacterAmarrID && typeID <= DBConstants.CharacterVherokiorID)
                    {
                        url = String.Format("{0}{1}", NetworkConstants.EVEGate,
                                            String.Format(NetworkConstants.EveGateCharacterProfile,
                                                          Uri.EscapeUriString(matchText.TrimEnd("<br>".ToCharArray()))));
                    }
                    else
                    {
                        switch (typeID)
                        {
                            case DBConstants.AllianceID:
                                url = String.Format("{0}{1}", NetworkConstants.EVEGate,
                                                    String.Format(NetworkConstants.EveGateAllianceProfile,
                                                                  Uri.EscapeUriString(matchText.TrimEnd("<br>".ToCharArray()))));
                                break;
                            case DBConstants.CorporationID:
                                url = String.Format("{0}{1}", NetworkConstants.EVEGate,
                                                    String.Format(NetworkConstants.EveGateCorporationProfile,
                                                                  Uri.EscapeUriString(matchText.TrimEnd("<br>".ToCharArray()))));
                                break;
                            default:
                                igbOnly = true;
                                break;
                        }
                    }
                }
                else
                    igbOnly = true;

                if (!igbOnly)
                {
                    replacements[match.ToString()] =
                        String.Format("<a href=\"{0}\" title=\"{0}{2}Click to follow the link\">{1}</a>",
                                      url, matchText, Environment.NewLine);
                }
                else
                {
                    replacements[match.ToString()] =
                        String.Format(
                            "<span style=\"color: #000000; text-decoration: underline; cursor: pointer;\" title=\"{0}{2}Link works only in IGB\">{1}</span>",
                            matchValue, matchText, Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Formats the color to RGB.
        /// </summary>
        /// <param name="replacements">The replacements.</param>
        private void FormatColorToRGB(IDictionary<string, string> replacements)
        {
            // Regural expression for fixing text coloring
            Regex regexColor = new Regex(@"color(?:=""|:\s*)#[0-9a-f]{2}([0-9a-f]{6})(?:;|"")", RegexOptions.IgnoreCase);
            foreach (Match match in regexColor.Matches(m_selectedObject.Text))
            {
                replacements[match.ToString()] = String.Format("color=\"#{0}\"", match.Groups[1].Value);
            }
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