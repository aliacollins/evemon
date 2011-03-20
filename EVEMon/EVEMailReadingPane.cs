using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class EVEMailReadingPane : UserControl
    {
        private EVEMailMessage m_selectedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="EVEMailReadingPane"/> class.
        /// </summary>
        public EVEMailReadingPane()
        {
            InitializeComponent();

            DoubleBuffered = true;

            lblMailHeader.Font = FontFactory.GetDefaultFont(10F, FontStyle.Bold);
            lblSender.Font = FontFactory.GetDefaultFont(10F);
            flPanelMailHeader.ForeColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        internal EVEMailMessage SelectedObject
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
            flPanelMailHeader.Visible = false;
            wbMailBody.Visible = false;
        }

        /// <summary>
        /// Updates the reading pane.
        /// </summary>
        private void UpdatePane()
        {
            // Update the text on the header labels
            lblMailHeader.Text = m_selectedObject.Title;
            lblSender.Text = m_selectedObject.Sender;
            lblSendDate.Text = String.Format(CultureConstants.DefaultCulture, "Sent: {0:ddd} {0:G}", m_selectedObject.SentDate.ToLocalTime());
            lblRecipient.Text = String.Format(CultureConstants.DefaultCulture, "To: {0}", string.Join(", ", m_selectedObject.ToCharacterIDs));

            // Allows the text in the webbrowser to be displayed
            wbMailBody.AllowNavigation = true;

            // Parce the mail body text to the web browser
            // so for the text to be formatted accordingly
            wbMailBody.DocumentText = m_selectedObject.EVEMailBody.BodyText;

            // We need to wait for the Document to be loaded
            do
            {
                Application.DoEvents();
            } while (wbMailBody.ReadyState != WebBrowserReadyState.Complete);

            // Show the controls
            flPanelMailHeader.Visible = true;
            wbMailBody.Visible = true;       
        }

        /// <summary>
        /// Every time the mail header panel gets painted we add a line at the bottom.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void flPanelMailHeader_Paint(object sender, PaintEventArgs e)
        {
            // Draw a line at the bottom of the panel
            using (Graphics g = flPanelMailHeader.CreateGraphics())
            {
                Pen blackPen = new Pen(Color.Black);
                g.DrawLine(blackPen, 5, 69, flPanelMailHeader.Width - 5, 69);
            }
        }

        /// <summary>
        /// Handles the Navigating event of the wbMailBody control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/> instance containing the event data.</param>
        private void wbMailBody_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Prevents the browser to navigate past the home page
            wbMailBody.AllowNavigation = false;
        }
    }
}
