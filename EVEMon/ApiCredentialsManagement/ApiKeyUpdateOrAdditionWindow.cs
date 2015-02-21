using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Controls.MultiPanel;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Properties;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class ApiKeyUpdateOrAdditionWindow : EVEMonForm
    {
        private readonly bool m_updateMode;
        private APIKey m_apiKey;
        private APIKeyCreationEventArgs m_creationArgs;

        /// <summary>
        /// Constructor for new API credential.
        /// </summary>
        public ApiKeyUpdateOrAdditionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor for editing existing API credentials.
        /// </summary>
        /// <param name="apiKey"></param>
        public ApiKeyUpdateOrAdditionWindow(APIKey apiKey)
            : this()
        {
            m_apiKey = apiKey;
            m_updateMode = (m_apiKey != null);
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

            WarningLabel.Visible = m_updateMode;
            VerificationCodeTextBox.Text = (m_apiKey != null ? m_apiKey.VerificationCode : String.Empty);
            IDTextBox.Text = (m_apiKey != null ? m_apiKey.ID.ToString(CultureConstants.DefaultCulture) : String.Empty);
            IDTextBox.ReadOnly = m_updateMode;
            CharactersListView.Items.Clear();

            MultiPanel.SelectedPage = CredentialsPage;
            MultiPanel.SelectionChange += MultiPanel_SelectionChange;
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
                ButtonPrevious.Enabled = false;
                ButtonNext.Enabled = true;
                ButtonNext.Text = "&Next >";
            }
            else if (args.NewPage == WaitingPage)
            {
                ButtonPrevious.Enabled = true;
                ButtonNext.Enabled = false;
                ButtonNext.Text = "&Next >";
            }
            else
            {
                ButtonPrevious.Enabled = true;
                ButtonNext.Enabled = (ResultsMultiPanel.SelectedPage == CharactersListPage);
                ButtonNext.Text = (m_updateMode ? "&Update" : "&Import");
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
            IDTextBox.CausesValidation = VerificationCodeTextBox.CausesValidation = false;
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

            // Display a warning if the cached timer hasn't expires yet
            if (m_updateMode && m_apiKey.CachedUntil > DateTime.UtcNow)
            {
                KeyPicture.Image = Resources.KeyWrong32;
                KeyLabel.Text = "Cached timer not expired yet.";
                CharactersGroupBox.Text = "Warning report";
                CachedWarningLabel.Text = String.Format(CultureConstants.DefaultCulture, CachedWarningLabel.Text,
                                                        m_apiKey.CachedUntil.ToLocalTime());
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
                m_apiKey.TryUpdateAsync(VerificationCodeTextBox.Text, OnUpdated);
                return;
            }

            // Or creating a new one ?
            long id;
            bool apiKeyExists = false;
            if (Int64.TryParse(IDTextBox.Text, out id))
                apiKeyExists = EveMonClient.APIKeys.Any(apiKey => apiKey.ID == id &&
                                                                  apiKey.VerificationCode == VerificationCodeTextBox.Text);
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
                APIKey.TryAddOrUpdateAsync(id, VerificationCodeTextBox.Text, OnUpdated);
        }

        /// <summary>
        /// Validates the operation and closes the window.
        /// </summary>
        private void Complete()
        {
            if (m_creationArgs == null)
                return;

            m_apiKey = m_creationArgs.CreateOrUpdate();

            // Takes care of the ignore list
            foreach (ListViewItem item in CharactersListView.Items)
            {
                CharacterIdentity id = (CharacterIdentity)item.Tag;
                // If it's a newly created API key, character monitoring has been already been set
                // We only need to deal with those coming out of the ignore list
                if (item.Checked)
                {
                    if (m_apiKey.IdentityIgnoreList.Contains(id))
                    {
                        m_apiKey.IdentityIgnoreList.Remove(id);
                        id.CCPCharacter.Monitored = true;
                    }
                    continue;
                }

                // Add character in ignore list if not already
                if (!m_apiKey.IdentityIgnoreList.Contains(id))
                    m_apiKey.IdentityIgnoreList.Add(id.CCPCharacter);
            }

            // Closes the window
            Close();
        }

        /// <summary>
        /// When API credentials have been updated.
        /// </summary>
        /// <returns></returns>
        private void OnUpdated(object sender, APIKeyCreationEventArgs e)
        {
            m_creationArgs = e;

            CharactersGroupBox.Text = "Characters exposed by API key";

            // Updates the picture and label for key type
            switch (e.Type)
            {
                default:
                    KeyPicture.Image = Resources.KeyWrong32;
                    KeyLabel.Text = e.KeyTestError;
                    CharactersGroupBox.Text = "Error report";
                    ResultsMultiPanel.SelectedPage = GetErrorPage(e);
                    break;
                case APIKeyType.Account:
                    KeyPicture.Image = Resources.AccountWide32;
                    KeyLabel.Text = "This is an 'Account' wide API key.";
                    ResultsMultiPanel.SelectedPage = CharactersListPage;
                    break;
                case APIKeyType.Character:
                    KeyPicture.Image = Resources.DefaultCharacterImage32;
                    KeyLabel.Text = "This is a 'Character' restricted API key.";
                    ResultsMultiPanel.SelectedPage = CharactersListPage;
                    break;
                case APIKeyType.Corporation:
                    KeyPicture.Image = Resources.DefaultCorporationImage32;
                    KeyLabel.Text = "This is a 'Corporation' API key.";
                    ResultsMultiPanel.SelectedPage = CharactersListPage;
                    break;
            }

            // Updates the characters list
            CharactersListView.Items.Clear();
            foreach (ListViewItem item in e.Identities.Select(
                id => new ListViewItem(id.CharacterName)
                          {
                              Tag = id,
                              Checked = (m_apiKey == null || !m_apiKey.IdentityIgnoreList.Contains(id))
                          }))
            {
                CharactersListView.Items.Add(item);
            }

            // Issue a warning if the access of the API key is zero
            if (e.AccessMask == 0)
            {
                WarningLabel.Text = "Beware! This API key does not provide any data!";
                WarningLabel.Visible = true;
            }
                // Issue a warning if the access of API key is less than needed for basic features
            else if (e.Type != APIKeyType.Corporation && e.AccessMask < (int)APIMethodsExtensions.BasicCharacterFeatures)
            {
                WarningLabel.Text = "Beware! The data this API key provides does not suffice for basic features!";
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
        /// <param name="e">The <see cref="APIKeyCreationEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private MultiPanelPage GetErrorPage(APIKeyCreationEventArgs e)
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
        /// On the first page, when a textbox is changed, we ensure the previously generated <see cref="APIKeyCreationEventArgs"/> is destroyed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IDTextBox_TextChanged(object sender, EventArgs e)
        {
            m_creationArgs = null;
        }

        /// <summary>
        /// On the first page, when a textbox is changed, we ensure the previously generated <see cref="APIKeyCreationEventArgs"/> is destroyed.
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
            Util.OpenURL(
                new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVECommunityBase,
                    NetworkConstants.APICredentialsBase)));
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
            Util.OpenURL(new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVECommunityBase,
                String.Format(CultureConstants.InvariantCulture, NetworkConstants.APICredentialsUpdate, IDTextBox.Text))));
        }

        /// <summary>
        /// Handles the Validating event of the IDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void IDTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(IDTextBox.Text))
            {
                errorProvider.SetError(IDTextBox, "ID cannot be blank.");
                e.Cancel = true;
                return;
            }

            if (IDTextBox.TextLength == 1 && IDTextBox.Text == "0")
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
                if (Char.IsDigit(IDTextBox.Text[i]))
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
            errorProvider.SetError(IDTextBox, String.Empty);
        }

        /// <summary>
        /// Handles the Validating event of the VerificationCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void VerificationCodeTextBox_Validating(object sender, CancelEventArgs e)
        {
            string vCode = VerificationCodeTextBox.Text.Trim();
            if (!String.IsNullOrEmpty(vCode))
                return;

            errorProvider.SetError(VerificationCodeTextBox, "Verification Code cannot be blank.");
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the Validated event of the VerificationCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void VerificationCodeTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(VerificationCodeTextBox, String.Empty);
        }
    }
}