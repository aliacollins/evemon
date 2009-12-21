using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.Serialization.API;
using EVEMon.Common;
using EVEMon.Common.Notifications;
using System.Globalization;

namespace EVEMon
{
    public partial class APIErrorWindow : EVEMonForm
    {
        private APIErrorNotification m_notification;

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
                if (value == null)
                {
                    errorLabel.Text = "No error selected.";
                    detailsTextBox.Text = "There was no associated XML document.";
                }
                else if (value.Result == null)
                {
                    errorLabel.Text = value.ToString() + "\nNo details were provided.";
                    detailsTextBox.Text = "There was no associated XML document.";
                }
                else
                {
                    errorLabel.Text = value.ToString() + "\n";
                    switch (value.Result.ErrorType)
                    {
                        case APIErrors.CCP:
                            errorLabel.Text += String.Format(CultureInfo.CurrentCulture, 
                                "CCP Error {0} : {1}", 
                                value.Result.CCPError.ErrorCode, 
                                value.Result.CCPError.ErrorMessage);
                            break;

                        case APIErrors.Http:
                            errorLabel.Text += String.Format(CultureInfo.CurrentCulture, "HTTP error: {0}", value.Result.ErrorMessage);
                            break;

                        case APIErrors.Xml:
                            errorLabel.Text += String.Format(CultureInfo.CurrentCulture, "XML error: {0}", value.Result.ErrorMessage);
                            break;

                        case APIErrors.Xslt:
                            errorLabel.Text += String.Format(CultureInfo.CurrentCulture, "XSLT error: {0}", value.Result.ErrorMessage);
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    if (value.Result.XmlDocument != null)
                    {
                        detailsTextBox.Text = Util.GetXMLStringRepresentation(value.Result.XmlDocument);
                    }
                    else
                    {
                        detailsTextBox.Text = "There was no associated XML document.";
                    }
                }

            }
        }
    }
}
