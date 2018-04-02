using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Controls.MultiPanel;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Models;
using EVEMon.Common.Properties;
using EVEMon.Common.Service;
using System.IO;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class EsiKeyUpdateOrAdditionWindow : EVEMonForm
    {
        private readonly SSOAuthenticationService m_authService;
        private readonly bool m_updateMode;
        private ESIKey m_esiKey;
        private ESIKeyCreationEventArgs m_creationArgs;
        private readonly SSOWebServer m_server;
        private readonly string m_state;

        /// <summary>
        /// Constructor for new ESI credential.
        /// </summary>
        public EsiKeyUpdateOrAdditionWindow()
        {
            string id = Settings.SSOClientID, secret = Settings.SSOClientSecret;
            InitializeComponent();
            m_server = new SSOWebServer();
            m_state = DateTime.UtcNow.ToFileTime().ToString();
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(secret))
                m_authService = null;
            else
                m_authService = new SSOAuthenticationService(id, secret,
                    NetworkConstants.SSOScopes);
        }

        /// <summary>
        /// Constructor for editing existing ESI credentials.
        /// </summary>
        /// <param name="esiKey"></param>
        public EsiKeyUpdateOrAdditionWindow(ESIKey esiKey)
            : this()
        {
            m_esiKey = esiKey;
            m_updateMode = m_esiKey != null;
        }

        /// <summary>
        /// Starts the SSO server.
        /// </summary>
        private void StartServer()
        {
            try
            {
                m_server.Start();
            }
            catch (IOException)
            {
                MessageBox.Show(string.Format(@"Failed to start SSO server. Check your " +
                    "firewall settings (using port {0:D}) and ensure that only one " +
                    "instance of EVEMon is active when adding ESI keys.", SSOWebServer.PORT),
                    @"Cannot start authentication", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Update the controls visibility depending on whether we are in update or creation mode.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            if (m_authService == null)
            {
                MessageBox.Show(@"Please set the ESI Client ID and Client Secret in Settings before adding ESI keys.",
                    @"Client Secret not set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }

            WarningLabel.Visible = m_updateMode;
            AccessTokenTextBox.Text = m_esiKey?.AccessToken ?? string.Empty;
            IDTextBox.Text = m_esiKey?.ID.ToString(CultureConstants.DefaultCulture) ?? string.Empty;
            IDTextBox.ReadOnly = m_updateMode;
            CharactersListView.Items.Clear();

            MultiPanel.SelectedPage = CredentialsPage;
            MultiPanel.SelectionChange += MultiPanel_SelectionChange;
            StartServer();
        }

        /// <summary>
        /// When we switch panels, we update the "next", "previous" and "cancel" buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MultiPanel_SelectionChange(object sender, MultiPanelSelectionChangeEventArgs args)
        {
            if (args.NewPage == CredentialsPage)
            {
                StartServer();
                ButtonPrevious.Visible = false;
                ButtonPrevious.Enabled = false;
                ButtonNext.Enabled = true;
                ButtonNext.Text = "&Next >";
            }
            else if (args.NewPage == WaitingPage)
            {
                m_server.Stop();
                ButtonPrevious.Visible = true;
                ButtonPrevious.Enabled = true;
                ButtonNext.Enabled = false;
                ButtonNext.Text = "&Next >";
            }
            else
            {
                ButtonPrevious.Visible = true;
                ButtonPrevious.Enabled = true;
                ButtonNext.Enabled = ResultsMultiPanel.SelectedPage == CharactersListPage;
                ButtonNext.Text = m_updateMode ? "&Update" : "&Import";
                ButtonNext.Focus();
            }
        }

        /// <summary>
        /// Previous.
        /// When the previous button is clicked, we select the first page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPrevious_Click(object sender, EventArgs e)
        {
            MultiPanel.SelectedPage = CredentialsPage;
        }

        /// <summary>
        /// Cancel.
        /// Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            m_creationArgs = null;
            IDTextBox.CausesValidation = AccessTokenTextBox.CausesValidation = false;
            Close();
        }

        /// <summary>
        /// Next / Import / Update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonNext_Click(object sender, EventArgs e)
        {
            // Are at the last page ?
            if (MultiPanel.SelectedPage == ResultPage)
            {
                Complete();
                return;
            }

            // We are at the first page
            if (!ValidateChildren())
                return;

            // If the args have not been invalidated since the last time...
            if (m_creationArgs != null)
            {
                MultiPanel.SelectedPage = ResultPage;
                return;
            }

            // Display a warning if the cached timer hasn't expired yet
            if (m_updateMode && m_esiKey.CachedUntil > DateTime.UtcNow)
            {
                KeyPicture.Image = Resources.KeyWrong32;
                KeyLabel.Text = @"Cached timer not expired yet.";
                CharactersGroupBox.Text = @"Warning report";
                CachedWarningLabel.Text = String.Format(CultureConstants.DefaultCulture, CachedWarningLabel.Text,
                                                        m_esiKey.CachedUntil.ToLocalTime());
                ResultsMultiPanel.SelectedPage = CachedWarningPage;
                Throbber.State = ThrobberState.Stopped;
                MultiPanel.SelectedPage = ResultPage;
                return;
            }

            MultiPanel.SelectedPage = WaitingPage;
            Throbber.State = ThrobberState.Rotating;

            // Are we updating existing API key ?
            if (m_updateMode)
            {
                m_esiKey.TryUpdateAsync(AccessTokenTextBox.Text, OnUpdated);
                return;
            }

            // Or creating a new one ?
            long id;
            bool apiKeyExists = false;
            if (long.TryParse(IDTextBox.Text, out id))
                apiKeyExists = EveMonClient.ESIKeys.Any(esiKey => esiKey.ID == id &&
                                                                  esiKey.AccessToken == AccessTokenTextBox.Text);
            // Does this API key already exists in our list ?
            if (apiKeyExists)
            {
                KeyPicture.Image = Resources.KeyWrong32;
                KeyLabel.Text = "API key already in list.";
                CharactersGroupBox.Text = "Error report";
                ResultsMultiPanel.SelectedPage = APIKeyExistsErrorPage;
                Throbber.State = ThrobberState.Stopped;
                MultiPanel.SelectedPage = ResultPage;
            }
            else
                ESIKey.TryAddOrUpdateAsync(id, AccessTokenTextBox.Text, OnUpdated);
        }

        /// <summary>
        /// Validates the operation and closes the window.
        /// </summary>
        private void Complete()
        {
            if (m_creationArgs == null)
                return;

            m_esiKey = m_creationArgs.CreateOrUpdate();

            // Takes care of the ignore list
            foreach (ListViewItem item in CharactersListView.Items)
            {
                CharacterIdentity id = (CharacterIdentity)item.Tag;
                // If it's a newly created API key, character monitoring has been already been set
                // We only need to deal with those coming out of the ignore list
                if (item.Checked)
                {
                    if (m_esiKey.IdentityIgnoreList.Contains(id))
                    {
                        m_esiKey.IdentityIgnoreList.Remove(id);
                        id.CCPCharacter.Monitored = true;
                    }
                    continue;
                }

                // Add character in ignore list if not already
                if (!m_esiKey.IdentityIgnoreList.Contains(id))
                    m_esiKey.IdentityIgnoreList.Add(id.CCPCharacter);
            }

            // Closes the window
            Close();
        }

        /// <summary>
        /// When ESI credentials have been updated.
        /// </summary>
        /// <returns></returns>
        private void OnUpdated(object sender, ESIKeyCreationEventArgs e)
        {
            m_creationArgs = e;

            CharactersGroupBox.Text = "Characters exposed by ESI key";

            // Updates the picture and label for key type
            switch (e.Type)
            {
                default:
                    KeyPicture.Image = Resources.KeyWrong32;
                    KeyLabel.Text = e.KeyTestError;
                    CharactersGroupBox.Text = "Error report";
                    ResultsMultiPanel.SelectedPage = GetErrorPage(e);
                    break;
                case CCPAPIKeyType.Account:
                    KeyPicture.Image = Resources.AccountWide32;
                    KeyLabel.Text = "This is an 'Account' wide ESI key.";
                    ResultsMultiPanel.SelectedPage = CharactersListPage;
                    break;
                case CCPAPIKeyType.Character:
                    KeyPicture.Image = Resources.DefaultCharacterImage32;
                    KeyLabel.Text = "This is a 'Character' restricted ESI key.";
                    ResultsMultiPanel.SelectedPage = CharactersListPage;
                    break;
                case CCPAPIKeyType.Corporation:
                    KeyPicture.Image = Resources.DefaultCorporationImage32;
                    KeyLabel.Text = "This is a 'Corporation' ESI key.";
                    ResultsMultiPanel.SelectedPage = CharactersListPage;
                    break;
            }

            // Updates the characters list
            CharactersListView.Items.Clear();
            foreach (ListViewItem item in e.Identities.Select(
                id => new ListViewItem(id.CharacterName)
                          {
                              Tag = id,
                              Checked = m_esiKey == null || !m_esiKey.IdentityIgnoreList.Contains(id)
                          }))
            {
                CharactersListView.Items.Add(item);
            }

            // Issue a warning if the access of the ESI key is zero
            if (e.AccessMask == 0)
            {
                WarningLabel.Text = "Beware! This ESI key does not provide any data!";
                WarningLabel.Visible = true;
            }
            // Issue a warning if the access of ESI key is less than needed for basic features
            else if (e.Type != CCPAPIKeyType.Corporation && e.AccessMask < (long)CCPAPIMethodsEnum.BasicCharacterFeatures)
            {
                WarningLabel.Text = "Beware! The data this ESI key provides does not suffice for basic features!";
                WarningLabel.Visible = true;
            }
            else
            {
                WarningLabel.Visible = m_updateMode;
            }

            // Selects the last page
            Throbber.State = ThrobberState.Stopped;
            MultiPanel.SelectedPage = ResultPage;
        }

        /// <summary>
        /// Gets the error page.
        /// </summary>
        /// <param name="e">The <see cref="ESIKeyCreationEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private MultiPanelPage GetErrorPage(ESIKeyCreationEventArgs e)
        {
            if (e.CCPError.IsAuthenticationFailure)
                return AuthenticationErrorPage;

            if (e.CCPError.IsLoginDeniedByAccountStatus)
                return LoginDeniedErrorPage;

            if (e.CCPError.IsAPIKeyExpired)
                return APIKeyExpiredErrorPage;

            GeneralErrorLabel.Text = String.Format(CultureConstants.DefaultCulture, GeneralErrorLabel.Text, e.KeyTestError);
            return GeneralErrorPage;
        }

        /// <summary>
        /// On the first page, when a textbox is changed, we ensure the previously generated <see cref="ESIKeyCreationEventArgs"/> is destroyed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IDTextBox_TextChanged(object sender, EventArgs e)
        {
            m_creationArgs = null;
        }

        /// <summary>
        /// On the first page, when a textbox is changed, we ensure the previously generated <see cref="ESIKeyCreationEventArgs"/> is destroyed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerificationCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            m_creationArgs = null;
        }

        /// <summary>
        /// Handles the LinkClicked event of the FeaturesLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void FeaturesLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (EVEMonFeaturesWindow window = new EVEMonFeaturesWindow())
            {
                window.ShowDialog(this);
            }
        }

        /// <summary>
        /// Handles the LinkClicked event of the ApiCredentialsLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void ApiCredentialsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(new Uri($"{NetworkConstants.EVECommunityBase}{NetworkConstants.APICredentialsBase}"));
        }

        /// <summary>
        /// Handles the LinkClicked event of the LoginDeniedLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void LoginDeniedLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(new Uri(NetworkConstants.CCPAccountManage));
        }

        /// <summary>
        /// Handles the LinkClicked event of the APIKeyExpiredLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void APIKeyExpiredLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(new Uri(NetworkConstants.EVECommunityBase + string.Format(
                NetworkConstants.APICredentialsUpdate, IDTextBox.Text)));
        }

        /// <summary>
        /// Handles the Validating event of the IDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void IDTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(IDTextBox.Text))
            {
                errorProvider.SetError(IDTextBox, "ID cannot be blank.");
                e.Cancel = true;
                return;
            }

            if (IDTextBox.TextLength == 1 && IDTextBox.Text == @"0")
            {
                errorProvider.SetError(IDTextBox, "ID must not be zero.");
                e.Cancel = true;
                return;
            }

            if (IDTextBox.Text.StartsWith("0", StringComparison.CurrentCulture))
            {
                errorProvider.SetError(IDTextBox, "ID must not start with zero.");
                e.Cancel = true;
                return;
            }

            for (int i = 0; i < IDTextBox.TextLength; i++)
            {
                if (char.IsDigit(IDTextBox.Text[i]))
                    continue;

                errorProvider.SetError(IDTextBox, "ID must be numerical.");
                e.Cancel = true;
                break;
            }
        }

        /// <summary>
        /// Handles the Validated event of the IDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void IDTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(IDTextBox, string.Empty);
        }

        /// <summary>
        /// Handles the Validating event of the VerificationCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void VerificationCodeTextBox_Validating(object sender, CancelEventArgs e)
        {
            string vCode = AccessTokenTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(vCode))
                return;

            errorProvider.SetError(AccessTokenTextBox, "Verification Code cannot be blank.");
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the Validated event of the VerificationCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void VerificationCodeTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(AccessTokenTextBox, string.Empty);
        }

        /// <summary>
        /// Starts a browser with the ESI login page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonESILogin_Click(object sender, EventArgs e)
        {
            m_authService?.SpawnBrowserForLogin(m_state, SSOWebServer.PORT);
        }
    }
}
