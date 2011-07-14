using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.Notifications;

namespace EVEMon
{
    public partial class ClaimableCertificateWindow : EVEMonForm
    {
        private ClaimableCertificateNotification m_notification;

        /// <summary>
        /// Constructor
        /// </summary>
        public ClaimableCertificateWindow()
        {
            InitializeComponent();
            RememberPositionKey = "ClaimableCertificateWindow";
        }

        /// <summary>
        /// Gets or sets the list of completed skills.
        /// </summary> 
        [Browsable(false)]
        public ClaimableCertificateNotification Notification
        {
            get { return m_notification; }
            set
            {
                m_notification = value;
                StringBuilder text = new StringBuilder();
                foreach (var skill in m_notification.Certificates)
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
