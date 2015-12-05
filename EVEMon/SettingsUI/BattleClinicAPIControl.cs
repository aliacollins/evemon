using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CloudStorageServices;
using EVEMon.Common.CloudStorageServices.BattleClinic;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls.MultiPanel;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Factories;

namespace EVEMon.SettingsUI
{
    public partial class BattleClinicAPIControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BattleClinicAPIControl"/> class.
        /// </summary>
        public BattleClinicAPIControl()
        {
            InitializeComponent();
            apiResponseLabel.Font = FontFactory.GetFont("Tahoma", FontStyle.Bold);
            apiResponseLabel.Text = String.Empty;
            throbber.Visible = false;
            throbber.BringToFront();
        }

        #endregion


        #region Local Events

        /// <summary>
        /// Occurs when the control loads.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BattleClinicAPIControl_Load(object sender, EventArgs e)
        {
            Font = FontFactory.GetFont("Tahoma");

            Settings.CloudStorageServiceProvider.Provider.CredentialsChecked += CloudStorageServiceProvider_CheckCredentials;
            Disposed += OnDisposed;

            if (!Settings.CloudStorageServiceProvider.Provider.HasCredentialsStored)
                return;

            bcUserIDTextBox.Text = CloudStorageServicesSettings.Default.BCUserID.ToString(CultureConstants.DefaultCulture);
            bcAPIKeyTextBox.Text = CloudStorageServicesSettings.Default.BCAPIKey;

            CheckAPICredentials();
        }

        /// <summary>
        /// Occurs when the control gets disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            Settings.CloudStorageServiceProvider.Provider.CredentialsChecked -= CloudStorageServiceProvider_CheckCredentials;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Opens a web page to the users BattleClinic API credentials.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void bcAPILinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(
                new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BattleClinicAPIBase,
                    NetworkConstants.BattleClinicAPIAccountCredentials)));
        }

        /// <summary>
        /// Resets the BattleClinic API credentials.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void resetButton_Click(object sender, EventArgs e)
        {
            apiResponseLabel.ForeColor = SystemColors.ControlText;
            apiResponseLabel.Text = String.Empty;

            bcUserIDTextBox.ResetText();
            bcAPIKeyTextBox.ResetText();

            CloudStorageServicesSettings.Default.BCUserID = 0;
            CloudStorageServicesSettings.Default.BCAPIKey = String.Empty;
            CloudStorageServicesSettings.Default.Save();
        }

        /// <summary>
        /// Applies the BattleClinic API credentials.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void applyButton_Click(object sender, EventArgs e)
        {
            apiResponseLabel.ForeColor = SystemColors.ControlText;
            apiResponseLabel.Text = String.Empty;

            if (!ValidateChildren())
                return;

            CheckAPICredentials();
        }

        /// <summary>
        /// Checks the validity of the BattleClinic UserID.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void bcUserIDTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(bcUserIDTextBox.Text))
            {
                errorProvider.SetError(bcUserIDTextBox, "UserID cannot be blank.");
                e.Cancel = true;
                return;
            }

            if (bcUserIDTextBox.TextLength == 1 && bcUserIDTextBox.Text == @"0")
            {
                errorProvider.SetError(bcUserIDTextBox, "UserID must not be zero.");
                e.Cancel = true;
                return;
            }

            if (bcUserIDTextBox.Text.StartsWith("0", StringComparison.CurrentCulture))
            {
                errorProvider.SetError(bcUserIDTextBox, "UserID must not start with zero.");
                e.Cancel = true;
                return;
            }

            for (int i = 0; i < bcUserIDTextBox.TextLength; i++)
            {
                if (Char.IsDigit(bcUserIDTextBox.Text[i]))
                    continue;

                errorProvider.SetError(bcUserIDTextBox, "UserID must be numerical.");
                e.Cancel = true;
                break;
            }
        }

        /// <summary>
        /// Occurs when the BattleClinic UserID gets validated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bcUserIDTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(bcUserIDTextBox, String.Empty);
        }

        /// <summary>
        /// Checks the validity of the BattleClinic APIKey.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void bcAPIKeyTextBox_Validating(object sender, CancelEventArgs e)
        {
            string apiKey = bcAPIKeyTextBox.Text.Trim();
            if (!String.IsNullOrEmpty(apiKey))
                return;

            errorProvider.SetError(bcAPIKeyTextBox, "APIKey cannot be blank.");
            e.Cancel = true;
        }

        /// <summary>
        /// Occurs when the BattleClinic APIKey gets validated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bcAPIKeyTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(bcAPIKeyTextBox, String.Empty);
        }

        #endregion


        #region Global Events

        /// <summary>
        /// When BattleClinic API credentials get checked, informs the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CloudStorageServiceProvider_CheckCredentials(object sender, CloudStorageServiceProviderEventArgs e)
        {
            throbber.State = ThrobberState.Stopped;
            throbber.Visible = false;

            if (e.HasError)
            {
                apiResponseLabel.ForeColor = Color.Red;
                apiResponseLabel.Text = e.ErrorMessage;
                return;
            }

            apiResponseLabel.ForeColor = Color.Green;
            apiResponseLabel.Text = @"Authenticated.";
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Checks the BattleClinic API credentials.
        /// </summary>
        private void CheckAPICredentials()
        {
            if (!(Settings.CloudStorageServiceProvider.Provider is BCCloudStorageServiceProvider))
                return;

            throbber.Visible = true;
            throbber.State = ThrobberState.Rotating;

            uint bcUserID = Convert.ToUInt32(bcUserIDTextBox.Text, CultureConstants.DefaultCulture);
            string bcAPIKey = bcAPIKeyTextBox.Text;

            Settings.CloudStorageServiceProvider.Provider.CheckAPICredentialsAsync(bcUserID, bcAPIKey);
        }

        #endregion
    }
}