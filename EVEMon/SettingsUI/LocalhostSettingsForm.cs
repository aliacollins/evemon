using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.SettingsUI
{
    public sealed partial class LocalhostSettingsForm : EVEMonForm
    {
        private readonly IEnumerable<SerializableLocalhostEndPoint> m_localhosts;
        private readonly SerializableLocalhostEndPoint m_newLocalhost;


        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="LocalhostSettingsForm"/> class from being created.
        /// </summary>
        private LocalhostSettingsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalhostSettingsForm"/> class.
        /// </summary>
        /// <param name="localhosts">The localhosts.</param>
        /// <param name="newLocalhost">The new localhost.</param>
        internal LocalhostSettingsForm(IEnumerable<SerializableLocalhostEndPoint> localhosts,
                                       SerializableLocalhostEndPoint newLocalhost)
            : this()
        {
            m_localhosts = localhosts;
            m_newLocalhost = newLocalhost;

            LocalhostNameTextBox.Text = newLocalhost.Name;

            if (newLocalhost.Url == null)
                return;

            PortNumericUpDown.Value = newLocalhost.Url.Port;
            PathTextBox.Text = newLocalhost.Url.AbsolutePath;

            if (PathTextBox.Text.StartsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
                PathTextBox.Text = PathTextBox.Text.Remove(0, 1);
        }


        #endregion


        #region LocalEvent Handlers

        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
                return;

            if (PathTextBox.Text.StartsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
                PathTextBox.Text = PathTextBox.Text.Remove(0, 1);
            m_newLocalhost.Name = LocalhostNameTextBox.Text;
            m_newLocalhost.Url = new Uri(String.Format(CultureConstants.InvariantCulture, "http://127.0.0.1:{0}/{1}",
                                                       PortNumericUpDown.Value, PathTextBox.Text));

            DialogResult = DialogResult.OK;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Displays a validation error notification for the specified control using the specified message.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="errorMessage"></param>
        private void ShowValidationError(Control control, string errorMessage)
        {
            errorProvider.SetError(control, errorMessage);
        }

        /// <summary>
        /// Clears a validation error notification on the specified control.
        /// </summary>
        /// <param name="control"></param>
        private void ClearValidationError(Control control)
        {
            errorProvider.SetError(control, String.Empty);
        }

        #endregion


        #region Controls Validation Methods

        /// <summary>
        /// Handles the Validating event of the LocalhostNameTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void LocalhostNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            string localhostName = LocalhostNameTextBox.Text.Trim();

            // Checks it is not a empty name
            if (String.IsNullOrEmpty(localhostName))
            {
                ShowValidationError(LocalhostNameTextBox, "Localhost name cannot be blank.");
                e.Cancel = true;
                return;
            }

            // Check the name does not already exist
            e.Cancel = m_localhosts.Aggregate(false, (current, localhost) =>
                                                     current | (localhostName == localhost.Name && localhost != m_newLocalhost));

            if (e.Cancel)
            {
                ShowValidationError(LocalhostNameTextBox, String.Format(CultureConstants.DefaultCulture,
                                                                        "There is already a localhost named \"{0}\".",
                                                                        localhostName));
            }
        }

        /// <summary>
        /// Handles the Validated event of the LocalhostNameTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LocalhostNameTextBox_Validated(object sender, EventArgs e)
        {
            ClearValidationError(LocalhostNameTextBox);
        }

        /// <summary>
        /// Handles the Validating event of the PortNumericUpDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void PortNumericUpDown_Validating(object sender, CancelEventArgs e)
        {
            // Check the port is valid
            e.Cancel = (PortNumericUpDown.Value < IPEndPoint.MinPort) || (PortNumericUpDown.Value > IPEndPoint.MaxPort);

            if (e.Cancel)
            {
                ShowValidationError(PortNumericUpDown,
                                    String.Format(CultureConstants.DefaultCulture, "Port value must be between {0} and {1}.",
                                                  IPEndPoint.MinPort, IPEndPoint.MaxPort));
                return;
            }

            // Check the port does not already exist
            e.Cancel = m_localhosts.Aggregate(false, (current, localhost) =>
                                                     current |
                                                     (PortNumericUpDown.Value == localhost.Url.Port &&
                                                      localhost != m_newLocalhost));

            if (e.Cancel)
            {
                ShowValidationError(PortNumericUpDown, String.Format(CultureConstants.DefaultCulture,
                                                                     "There is already a localhost named \"{0}\" assigned to this port.",
                                                                     LocalhostNameTextBox.Text.Trim()));
            }
        }

        /// <summary>
        /// Handles the Validated event of the PortNumericUpDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PortNumericUpDown_Validated(object sender, EventArgs e)
        {
            ClearValidationError(PortNumericUpDown);
        }

        #endregion
    }
}
