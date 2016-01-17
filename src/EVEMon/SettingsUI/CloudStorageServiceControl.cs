using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CloudStorageServices;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Factories;

namespace EVEMon.SettingsUI
{
    public partial class CloudStorageServiceControl : UserControl
    {
        private bool m_authCodeRequested;
        private static bool s_forceAuthOnLoad;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudStorageServiceControl"/> class.
        /// </summary>
        public CloudStorageServiceControl()
        {
            InitializeComponent();
            apiResponseLabel.Font = FontFactory.GetFont("Tahoma", FontStyle.Bold);
            apiResponseLabel.ResetText();

            throbber.Visible = false;
            throbber.BringToFront();

            btnRequestApply.Enabled = false;
            lblAuthCode.Enabled = false;
            txtBoxAuthCode.Enabled = false;
            btnReset.Enabled = false;

            s_forceAuthOnLoad = true;
        }

        #region Properties

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        [Browsable(false)]
        private static CloudStorageServiceProvider Provider => Settings.CloudStorageServiceProvider.Provider;

        #endregion


        #region Local Events

        /// <summary>
        /// Occurs when the control loads.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CloudStorageServiceControl_Load(object sender, EventArgs e)
        {
            Font = FontFactory.GetFont("Tahoma");
            Disposed += OnDisposed;

            CloudStorageServiceProvider.CredentialsChecked += CloudStorageServiceProvider_CheckCredentials;
            CloudStorageServiceProvider.SettingsReset += CloudStorageServiceProvider_SettingsReset;
        }

        /// <summary>
        /// Occurs when the control gets disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            CloudStorageServiceProvider.CredentialsChecked -= CloudStorageServiceProvider_CheckCredentials;
            CloudStorageServiceProvider.SettingsReset -= CloudStorageServiceProvider_SettingsReset;

            Disposed -= OnDisposed;
        }

        /// <summary>
        /// When the control becomes visible again, we update the content.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!Visible)
                return;

            CheckAPIAuthIsValid(s_forceAuthOnLoad);

            s_forceAuthOnLoad = false;
        }

        /// <summary>
        /// Handles the Click event of the btnReset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetTextAndColor();

            txtBoxAuthCode.ResetText();

            throbber.State = ThrobberState.Rotating;
            throbber.Visible = true;

            Provider?.ResetSettingsAsync();
        }

        /// <summary>
        /// Request or Applies the authentication code.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnRequestApply_Click(object sender, EventArgs e)
        {
            ResetTextAndColor();

            if (Provider == null)
                return;

            if (!m_authCodeRequested && !Provider.HasCredentialsStored)
            {
                Provider.RequestAuthCodeAsync();

                if (Provider.AuthSteps == AuthenticationSteps.One)
                {
                    btnRequestApply.Enabled = false;
                    return;
                }

                btnRequestApply.Text = @"Apply Auth Code";
                lblAuthCode.Enabled = txtBoxAuthCode.Enabled = btnRequestApply.Enabled =
                    !Provider.HasCredentialsStored;

                m_authCodeRequested = true;

                return;
            }

            if (String.IsNullOrWhiteSpace(txtBoxAuthCode.Text))
                return;

            throbber.State = ThrobberState.Rotating;
            throbber.Visible = true;

            Provider.CheckAuthCodeAsync(txtBoxAuthCode.Text);
        }

        /// <summary>
        /// Occures when clicking on the getAnAccountLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void getAnAccountLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Provider?.RefferalLink != null)
                Util.OpenURL(Provider?.RefferalLink);
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Occurs when provider's API credentials get checked, informing the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CloudStorageServiceProviderEventArgs"/> instance containing the event data.</param>
        private void CloudStorageServiceProvider_CheckCredentials(object sender, CloudStorageServiceProviderEventArgs e)
        {
            throbber.State = ThrobberState.Stopped;
            throbber.Visible = false;

            apiResponseLabel.ForeColor = e.HasError ? Color.Red : Color.Green;
            apiResponseLabel.Text = e.HasError
                ? e.ErrorMessage
                : CloudStorageServiceProvider.IsAuthenticated
                    ? @"Authenticated"
                    : String.Empty;

            if (!e.HasError && (Provider.AuthSteps != AuthenticationSteps.One) && m_authCodeRequested &&
                CloudStorageServiceProvider.IsAuthenticated)
            {
                m_authCodeRequested = false;
            }

            UpdateControlsVisibility();
        }

        /// <summary>
        /// Occurs when provider's API credentials get reset.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CloudStorageServiceProviderEventArgs"/> instance containing the event data.</param>
        private void CloudStorageServiceProvider_SettingsReset(object sender, CloudStorageServiceProviderEventArgs e)
        {
            throbber.State = ThrobberState.Stopped;
            throbber.Visible = false;

            UpdateControlsVisibility();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Checks the API authentication is valid.
        /// </summary>
        /// <param name="forceRecheck">if set to <c>true</c> forces an authentication recheck.</param>
        internal void CheckAPIAuthIsValid(bool forceRecheck = false)
        {
            m_authCodeRequested = false;

            UpdateControlsVisibility();

            if (Provider == null || (!forceRecheck && !Provider.HasCredentialsStored))
                return;

            if (!forceRecheck && CloudStorageServiceProvider.IsAuthenticated)
            {
                apiResponseLabel.ForeColor = Color.Green;
                apiResponseLabel.Text = @"Authenticated";
                return;
            }

            ResetTextAndColor();
            txtBoxAuthCode.ResetText();

            throbber.State = ThrobberState.Rotating;
            throbber.Visible = true;

            if (forceRecheck)
                Provider.CancelPendingQueries();

            Provider?.CheckAPIAuthIsValidAsync();
        }

        /// <summary>
        /// Resets the color of the text and.
        /// </summary>
        private void ResetTextAndColor()
        {
            apiResponseLabel.ResetForeColor();
            apiResponseLabel.ResetText();
        }

        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            createAccountLinkLabel.Visible = Provider?.RefferalLink != null;

            if (Provider == null || m_authCodeRequested)
                return;

            if (!Provider.HasCredentialsStored)
            {
                btnRequestApply.Text = @"Request Authentication";
                btnRequestApply.Enabled = !Provider.HasCredentialsStored;
                lblAuthCode.Enabled = txtBoxAuthCode.Enabled = btnReset.Enabled =
                    Provider.HasCredentialsStored;

                return;
            }

            btnRequestApply.Text = @"Request Authentication";
            lblAuthCode.Enabled = txtBoxAuthCode.Enabled = btnRequestApply.Enabled =
                !Provider.HasCredentialsStored;
            btnReset.Enabled = Provider.HasCredentialsStored;
        }

        #endregion
    }
}
