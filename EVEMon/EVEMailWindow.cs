using System;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon
{
    public partial class EVEMailWindow : EVEMonForm
    {
        private EVEMailMessage m_message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EVEMailWindow"/> class.
        /// </summary>
        internal EVEMailWindow()
        {
            InitializeComponent();

            RememberPositionKey = "EVEMailWindow";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EVEMailWindow"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public EVEMailWindow(EVEMailMessage message)
            : this()
        {
            MailMessage = message;
        }

        /// <summary>
        /// Gets or sets the mail message.
        /// </summary>
        /// <value>The mail message.</value>
        internal EVEMailMessage MailMessage
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
