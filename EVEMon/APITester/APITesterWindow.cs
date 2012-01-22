using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.APITester
{
    public partial class APITesterWindow : EVEMonForm
    {
        private Uri m_url;
        private string m_path;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="APITesterWindow"/> class.
        /// </summary>
        public APITesterWindow()
        {
            InitializeComponent();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        private Uri Url
        {
            get
            {
                // Gets the proper data
                string url = EveMonClient.APIProviders.CurrentProvider.Url.AbsoluteUri;
                m_path = EveMonClient.APIProviders.CurrentProvider.GetMethod((Enum)cbAPIMethod.SelectedItem).Path;
                if (String.IsNullOrEmpty(m_path) || String.IsNullOrEmpty(url))
                {
                    url = APIProvider.DefaultProvider.Url.AbsoluteUri;
                    m_path = APIProvider.DefaultProvider.GetMethod((Enum)cbAPIMethod.SelectedItem).Path;
                }

                // Build the uri
                Uri baseUri = new Uri(url);
                UriBuilder uriBuilder = new UriBuilder(baseUri);
                uriBuilder.Path = uriBuilder.Path.TrimEnd("/".ToCharArray()) + m_path;

                // Add the postdata to the path
                uriBuilder.Query = GetPostData();

                return uriBuilder.Uri;
            }
        }

        #endregion


        #region Main Methods

        /// <summary>
        /// Updates the API methods list.
        /// </summary>
        private void UpdateAPIMethodsList()
        {
            // List the API methods by type and name
            // Add the Server Status method on top
            List<Enum> apiMethods = new List<Enum> { APIGenericMethods.ServerStatus };

            // Add the non Account type methods
            apiMethods.AddRange(APIMethods.Methods.OfType<APIGenericMethods>().Where(
                method => !apiMethods.Contains(method) && APIMethods.NonAccountGenericMethods.Contains(method)).OrderBy(
                    method => method.ToString()).Cast<Enum>());

            // Add the Account type methods
            apiMethods.AddRange(APIMethods.Methods.OfType<APIGenericMethods>().Where(
                method => !apiMethods.Contains(method) && !APIMethods.NonAccountGenericMethods.Contains(method)).OrderBy(
                    method => method.ToString()).Cast<Enum>());

            // Add the character methods
            apiMethods.AddRange(APIMethods.Methods.OfType<APICharacterMethods>().OrderBy(
                method => method.ToString()).Cast<Enum>());

            // Add the corporation methods
            apiMethods.AddRange(APIMethods.Methods.OfType<APICorporationMethods>().OrderBy(
                method => method.ToString()).Cast<Enum>());

            cbAPIMethod.Items.Clear();
            cbAPIMethod.Items.AddRange(apiMethods.ToArray());
        }

        /// <summary>
        /// Updates the character list.
        /// </summary>
        private void UpdateCharacterList()
        {
            cbCharacter.Items.Clear();
            cbCharacter.Items.AddRange(EveMonClient.Characters.OfType<CCPCharacter>().Where(
                character => !character.Identity.APIKeys.IsEmpty()).OrderBy(
                    character => character.Name).ToArray());

            UpdateCharacterSelectionEnabling();
        }

        /// <summary>
        /// Updates the character selection enabling.
        /// </summary>
        private void UpdateCharacterSelectionEnabling()
        {
            cbCharacter.Enabled = rbInternal.Checked && cbCharacter.Items.Count > 0 && cbAPIMethod.SelectedItem != null &&
                                  !APIMethods.NonAccountGenericMethods.Contains(cbAPIMethod.SelectedItem);

            if (!cbCharacter.Enabled)
                cbCharacter.SelectedItem = null;
        }

        /// <summary>
        /// Updates the external info controls enabling.
        /// </summary>
        private void UpdateExternalInfoControlsEnabling()
        {
            tbKeyID.Enabled = tbVCode.Enabled = rbExternal.Checked && cbAPIMethod.SelectedItem != null &&
                                                !APIMethods.NonAccountGenericMethods.Contains(cbAPIMethod.SelectedItem);

            lblCharID.Visible = tbCharID.Visible = rbExternal.Checked && cbAPIMethod.SelectedItem != null &&
                                                   !(cbAPIMethod.SelectedItem is APIGenericMethods);

            if (!tbCharID.Visible)
                tbCharID.ResetText();
        }

        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            LoadDocument(new Uri("about:blank"));

            UpdateCharacterSelectionEnabling();
            UpdateExternalInfoControlsEnabling();

            lblIDOrName.Visible = tbIDOrName.Visible = cbAPIMethod.SelectedItem != null &&
                                                       (cbAPIMethod.SelectedItem.Equals(APIGenericMethods.CharacterName) ||
                                                        cbAPIMethod.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                                                        cbAPIMethod.SelectedItem.Equals(APICharacterMethods.NotificationTexts));

            if (cbAPIMethod.SelectedItem != null && lblIDOrName.Visible)
                lblIDOrName.Text = cbAPIMethod.SelectedItem.Equals(APIGenericMethods.CharacterName) ? "ID:" : "Message IDs:";
        }

        /// <summary>
        /// Checks the credentials validation.
        /// </summary>
        private void CheckCredentialsValidation()
        {
            if (!ValidateChildren())
                return;

            // When credentials are valid, load the API page
            LoadDocument(Url);
        }

        /// <summary>
        /// Gets the post data.
        /// </summary>
        /// <returns></returns>
        private String GetPostData()
        {
            if (cbAPIMethod.SelectedItem is APIGenericMethods)
                return GetPostDataForGenericAPIMethods();

            if (cbAPIMethod.SelectedItem is APICharacterMethods)
                return GetPostDataForCharacterAPIMethods();

            if (cbAPIMethod.SelectedItem is APICorporationMethods)
                return GetPostDataForCorporationAPIMethods();
            
            return String.Empty;
        }

        /// <summary>
        /// Gets the post data for generic API methods.
        /// </summary>
        /// <returns></returns>
        private String GetPostDataForGenericAPIMethods()
        {
            if (cbAPIMethod.SelectedItem.Equals(APIGenericMethods.CharacterName))
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataIDsOnly, tbIDOrName.Text);

            if (!APIMethods.NonAccountGenericMethods.Contains(cbAPIMethod.SelectedItem))
            {
                if (rbInternal.Checked)
                {
                    if (cbCharacter.SelectedItem == null)
                        return String.Empty;

                    Character character = (Character)cbCharacter.SelectedItem;
                    APIKey apiKey = character.Identity.APIKeys.FirstOrDefault(
                        key => key.Type == APIKeyType.Account || key.Type == APIKeyType.Character);

                    return apiKey == null
                               ? String.Empty
                               : String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                               apiKey.ID, apiKey.VerificationCode);
                }

                if (rbExternal.Checked)
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                         tbKeyID.Text, tbVCode.Text);
            }

            return String.Empty;
        }

        /// <summary>
        /// Gets the post data for character API methods.
        /// </summary>
        /// <returns></returns>
        private String GetPostDataForCharacterAPIMethods()
        {
            if (rbInternal.Checked)
            {
                if (cbCharacter.SelectedItem == null)
                    return String.Empty;

                Character character = (Character)cbCharacter.SelectedItem;
                APIKey apiKey = character.Identity.FindAPIKeyWithAccess((APICharacterMethods)cbAPIMethod.SelectedItem);

                if (apiKey == null)
                    return String.Empty;

                if (cbAPIMethod.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                    cbAPIMethod.SelectedItem.Equals(APICharacterMethods.NotificationTexts))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, tbIDOrName.Text);
                }

                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                     apiKey.ID, apiKey.VerificationCode, character.CharacterID);
            }

            if (rbExternal.Checked)
            {
                if (cbAPIMethod.SelectedItem.Equals(APICharacterMethods.CharacterInfo) &&
                    (tbKeyID.Text.Length == 0 || tbVCode.Text.Length == 0))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCharacterIDOnly,
                                         tbCharID.Text);
                }

                if (cbAPIMethod.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                    cbAPIMethod.SelectedItem.Equals(APICharacterMethods.NotificationTexts))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                         tbKeyID.Text, tbVCode.Text, tbCharID.Text, tbIDOrName.Text);
                }

                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                     tbKeyID.Text, tbVCode.Text, tbCharID.Text);
            }

            return String.Empty;
        }

        /// <summary>
        /// Gets the post data for corporation API methods.
        /// </summary>
        /// <returns></returns>
        private String GetPostDataForCorporationAPIMethods()
        {
            if (rbInternal.Checked)
            {
                if (cbCharacter.SelectedItem == null)
                    return String.Empty;

                Character character = (Character)cbCharacter.SelectedItem;
                APIKey apiKey = character.Identity.FindAPIKeyWithAccess((APICorporationMethods)cbAPIMethod.SelectedItem);

                return apiKey == null
                           ? String.Empty
                           : String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                           apiKey.ID, apiKey.VerificationCode, character.CharacterID);
            }

            return rbExternal.Checked
                       ? String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                       tbKeyID.Text, tbVCode.Text, tbCharID.Text)
                       : String.Empty;
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="url">The URL.</param>
        private void LoadDocument(Uri url)
        {
            m_url = url;
            webBrowser.Navigate(url);
        }

        /// <summary>
        /// Saves the document to the disk.
        /// </summary>
        private void SaveDocument()
        {
            if (webBrowser.Document == null || webBrowser.Document.Body == null)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML (*.xml)|*.xml";
                sfd.FileName = m_path.Substring(m_path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1,
                                                m_path.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) -
                                                m_path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1);

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    XmlDocument xdoc = new XmlDocument();
                    string innerText = webBrowser.Document.Body.InnerText.Trim().Replace("\n-", "\n");
                    xdoc.LoadXml(innerText);
                    string content = Util.GetXMLStringRepresentation(xdoc);

                    // Moves to the final file
                    FileHelper.OverwriteOrWarnTheUser(
                        sfd.FileName,
                        fs =>
                            {
                                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                                {
                                    writer.Write(content);
                                    writer.Flush();
                                    fs.Flush();
                                }
                                return true;
                            });
                }
                catch (IOException err)
                {
                    ExceptionHandler.LogException(err, true);
                    MessageBox.Show("There was an error writing out the file:\n\n" + err.Message,
                                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (XmlException err)
                {
                    ExceptionHandler.LogException(err, true);
                    MessageBox.Show("There was an error while converting to XML format.\r\nThe message was:\r\n" + err.Message,
                                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion


        #region Global and Local Events Handlers

        /// <summary>
        /// Handles the Load event of the APITesterWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void APITesterWindow_Load(object sender, EventArgs e)
        {           
            UpdateAPIMethodsList();
            UpdateCharacterList();

            UpdateControlsVisibility();

            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Called when disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Handles the CharacterUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterUpdated(object sender, EventArgs e)
        {
            UpdateCharacterList();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the rbInternal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void rbInternal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the rbExternal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void rbExternal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cbAPIMethod control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cbAPIMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset controls
            saveButton.Enabled = false;
            cbCharacter.SelectedItem = null;
            tbIDOrName.ResetText();

            UpdateControlsVisibility();

            if (APIMethods.NonAccountGenericMethods.Contains(cbAPIMethod.SelectedItem) && !tbIDOrName.Visible)
                LoadDocument(Url);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cbCharacter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cbCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveButton.Enabled = false;
            if (cbCharacter.SelectedItem != null && !tbIDOrName.Visible)
                LoadDocument(Url);
        }

        /// <summary>
        /// Handles the Navigating event of the webBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/> instance containing the event data.</param>
        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Prevents the browser to navigate past the shown page
            if (e.Url != m_url)
                e.Cancel = true;
        }

        /// <summary>
        /// Handles the DocumentCompleted event of the webBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserDocumentCompletedEventArgs"/> instance containing the event data.</param>
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Enable "Save" button on condition
            saveButton.Enabled = (m_url == e.Url && e.Url.AbsoluteUri != "about:blank" &&
                                 webBrowser.Document != null && webBrowser.Document.Body != null);
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the webBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
        private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Disable the reload shortcut key
            webBrowser.WebBrowserShortcutsEnabled = e.KeyData != Keys.F5;
        }

        /// <summary>
        /// Handles the Click event of the saveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        /// <summary>
        /// Handles the Validating of a text box that requires an ID.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void IDRequiredTextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox == null || !textbox.Visible)
                return;

            // Special condition in case user requests character info without API credentials
            if (cbAPIMethod.SelectedItem.Equals(APICharacterMethods.CharacterInfo) &&
                textbox.Equals(tbKeyID) && textbox.Text.Length == 0 && tbVCode.Text.Length == 0)
                return;

            if (String.IsNullOrEmpty(textbox.Text))
            {
                errorProvider.SetError(textbox, "ID cannot be blank.");
                e.Cancel = true;
                return;
            }

            if (textbox.TextLength == 1 && textbox.Text == "0")
            {
                errorProvider.SetError(textbox, "ID must not be zero.");
                e.Cancel = true;
                return;
            }

            if (textbox.Text.StartsWith("0", StringComparison.Ordinal))
            {
                errorProvider.SetError(textbox, "ID must not start with zero.");
                e.Cancel = true;
                return;
            }

            for (int i = 0; i < textbox.TextLength; i++)
            {
                if (Char.IsDigit(textbox.Text[i]))
                    continue;

                errorProvider.SetError(textbox, "ID must be numerical.");
                e.Cancel = true;
                break;
            }
        }

        /// <summary>
        /// Handles the Validating event of the tbVCode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbVCode_Validating(object sender, CancelEventArgs e)
        {
            // Special condition in case user requests character info without API credentials
            if (cbAPIMethod.SelectedItem.Equals(APICharacterMethods.CharacterInfo) &&
                tbKeyID.Text.Length == 0 && tbVCode.Text.Length == 0)
                return;

            string vCode = tbVCode.Text.Trim();
            if (vCode.Length != 0)
                return;

            errorProvider.SetError(tbVCode, "Verification Code cannot be blank.");
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the Validated event of a text box that requires an ID.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void IDRequiredTextBox_Validated(object sender, EventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox == null)
                return;

            errorProvider.SetError(textbox, String.Empty);
        }

        /// <summary>
        /// Handles the Validated event of the tbVCode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tbVCode_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbVCode, String.Empty);
        }

        /// <summary>
        /// Handles the KeyUp event of the tbIDOrName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void tbIDOrName_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                saveButton.Enabled = false;

            if (!e.KeyCode.Equals(Keys.Enter))
                return;

            if ((cbAPIMethod.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                 cbAPIMethod.SelectedItem.Equals(APICharacterMethods.NotificationTexts)) && cbCharacter.SelectedItem == null)
                return;

            LoadDocument(Url);
        }

        /// <summary>
        /// Handles the KeyUp event of the tbKeyID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void tbKeyID_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                saveButton.Enabled = false;

            if (e.KeyCode.Equals(Keys.Enter))
                CheckCredentialsValidation();
        }

        /// <summary>
        /// Handles the KeyUp event of the tbVCode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void tbVCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                saveButton.Enabled = false;

            if (e.KeyCode.Equals(Keys.Enter))
                CheckCredentialsValidation();
        }

        /// <summary>
        /// Handles the KeyUp event of the tbCharID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void tbCharID_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                saveButton.Enabled = false;

            if (e.KeyCode.Equals(Keys.Enter))
                CheckCredentialsValidation();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyPress"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyPressEventArgs"/> that contains the event data.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (e.KeyChar.Equals((char)Keys.Escape))
            {
                if (tbIDOrName.Focused)
                    tbIDOrName.ResetText();
                else if (tbKeyID.Focused)
                    tbKeyID.ResetText();
                else if (tbVCode.Focused)
                    tbVCode.ResetText();
                else if (tbCharID.Focused)
                    tbCharID.ResetText();
                else
                {
                    foreach (TextBox textBox in HeaderPanel.Controls.OfType<TextBox>())
                    {
                        textBox.CausesValidation = false;
                    }

                    Close();
                }
                e.Handled = true;
            }
            base.OnKeyPress(e);
        }

        #endregion

    }
}
