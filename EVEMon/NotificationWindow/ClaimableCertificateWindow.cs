using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Notifications;

namespace EVEMon.NotificationWindow
{
    public partial class ClaimableCertificateWindow : EVEMonForm
    {
        private ClaimableCertificateNotificationEventArgs m_notification;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimableCertificateWindow"/> class.
        /// </summary>
        public ClaimableCertificateWindow()
        {
            InitializeComponent();
            RememberPositionKey = "ClaimableCertificateWindow";
        }

        /// <summary>
        /// Gets or sets the list of completed skills.
        /// </summary> 
        internal ClaimableCertificateNotificationEventArgs Notification
        {
            get { return m_notification; }
            set
            {
                m_notification = value;
                StringBuilder text = new StringBuilder();
                foreach (Certificate skill in m_notification.Certificates)
                {
                    text.AppendLine(skill.ToString());
                }

                Size textSize = TextRenderer.MeasureText(text.ToString(), Font);
                MinimumSize = new Size(280, textSize.Height + 30);

                detailsTextBox.Text = text.ToString();
            }
        }
    }
}