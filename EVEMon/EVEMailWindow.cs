using System;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon
{
    public partial class EveMailWindow : EVEMonForm
    {
        private EveMailMessage m_message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailWindow"/> class.
        /// </summary>
        internal EveMailWindow()
        {
            InitializeComponent();

            RememberPositionKey = "EVEMailWindow";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailWindow"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public EveMailWindow(EveMailMessage message)
            : this()
        {
            MailMessage = message;
        }

        /// <summary>
        /// Gets or sets the mail message.
        /// </summary>
        /// <value>The mail message.</value>
        internal EveMailMessage MailMessage
        {
            get { return m_message; }
            set
            {
                if (m_message == value)
                    return;

                m_message = value;
                Tag = value;

                eveMailReadingPane.SelectedObject = m_message;
                UpdateWindowHeaderText();
            }
        }

        /// <summary>
        /// Updates the window header text.
        /// </summary>
        private void UpdateWindowHeaderText()
        {
            Text = String.Format("{0} - EVE Mail Message", m_message.Title);
        }
    }
}
