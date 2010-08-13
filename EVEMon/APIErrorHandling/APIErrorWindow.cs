using System;
using System.ComponentModel;
using EVEMon.Common.Controls;
using EVEMon.Common.Serialization.API;
using EVEMon.Common;
using EVEMon.Common.Notifications;
using System.Windows.Forms;
using EVEMon.Common.Net;
using EVEMon.APIErrorHandling;

namespace EVEMon.ApiErrorHandling
{
    public partial class APIErrorWindow : EVEMonForm
    {
        private APIErrorNotification m_notification;
        private UserControl m_troubleshooter;

        /// <summary>
        /// Constructor
        /// </summary>
        public APIErrorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the notification for this error.
        /// </summary>
        [Browsable(false)]
        public APIErrorNotification Notification
        {
            get { return m_notification; }
            set
            {
                m_notification = value;
                errorLabel.Text = GetErrorLabelText(value);
                detailsTextBox.Text = GetXmlData(value.Result);

                DisplayTroubleshooter(value.Result.Exception);
            }
        }

        /// <summary>
        /// Displays the troubleshooter.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private void DisplayTroubleshooter(Exception exception)
        {
            TroubleshooterPanel.Visible = false;

            m_troubleshooter = GetTroubleshooter(exception);

            if (m_troubleshooter == null)
                return;

            TroubleshooterPanel.Visible = true;
            TroubleshooterPanel.Controls.Add(m_troubleshooter);
            m_troubleshooter.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Gets the troubleshooter.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A troubleshooter for the error message.</returns>
        private static UserControl GetTroubleshooter(Exception exception)
        {
            if (exception == null)
                return null;

            var httpException = exception as HttpWebServiceException;

            if (httpException == null)
                return null;

            if (httpException.Status == HttpWebServiceExceptionStatus.Timeout)
                return new HttpTimeoutTroubleshooter();

            return null;
        }

        /// <summary>
        /// Gets the error label text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>String representing the error.</returns>
        private static string GetErrorLabelText(APIErrorNotification value)
        {
            if (value == null)
                return "No error selected.";

            if (value.Result == null)
                return String.Format("{0}{1}No details were provided.", value, Environment.NewLine);

            return String.Format("{0}{1}{2}", value, Environment.NewLine, GetErrorLabelTextDetail(value.Result));
        }

        /// <summary>
        /// Gets the error label text detail.
        /// </summary>
        /// <param name="value">The value.</param>
        private static string GetErrorLabelTextDetail(IAPIResult result)
        {
            switch (result.ErrorType)
            {
                case APIErrors.None:
                    return "No error specified";

                case APIErrors.CCP:
                    return String.Format(CultureConstants.DefaultCulture,
                        "CCP Error {0} : {1}",
                        result.CCPError.ErrorCode,
                        result.CCPError.ErrorMessage);

                case APIErrors.Http:
                    return String.Format(CultureConstants.DefaultCulture,
                        "HTTP error: {0}", result.ErrorMessage);

                case APIErrors.Xml:
                    return String.Format(CultureConstants.DefaultCulture,
                        "XML error: {0}", result.ErrorMessage);

                case APIErrors.Xslt:
                    return String.Format(CultureConstants.DefaultCulture,
                        "XSLT error: {0}", result.ErrorMessage);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the XML data from the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private static string GetXmlData(IAPIResult result)
        {
            if (result == null || result.XmlDocument == null)
                return "There was no associated XML document.";

            return Util.GetXMLStringRepresentation(result.XmlDocument);
        }

        /// <summary>
        /// On closing, disposes of the troubleshooter.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (m_troubleshooter != null)
            {
                m_troubleshooter.Dispose();
                m_troubleshooter = null;
            }            

            base.OnFormClosed(e);
        }
    }
}
