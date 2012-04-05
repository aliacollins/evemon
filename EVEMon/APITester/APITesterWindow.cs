using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.ApiTester
{
    public partial class ApiTesterWindow : EVEMonForm
    {
        private Uri m_url;
        private const string NoAPIKeyWithAccess = "No API key with access to API call found";

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiTesterWindow"/> class.
        /// </summary>
        public ApiTesterWindow()
        {
            InitializeComponent();

            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;
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
                string postData = GetPostData();
                string uriString = EveMonClient.APIProviders.CurrentProvider
                    .GetMethodUrl((Enum)APIMethodComboBox.SelectedItem).AbsoluteUri;
                string errorText = postData == NoAPIKeyWithAccess ? NoAPIKeyWithAccess : String.Empty;

                ErrorProvider.SetError(APIUrlLabel, errorText);

                if (!String.IsNullOrWhiteSpace(postData) && postData != NoAPIKeyWithAccess)
                    uriString += String.Format(CultureConstants.InvariantCulture, "?{0}", postData);

                return new Uri(uriString);
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
                method => !apiMethods.Contains(method) &&
                          APIMethods.NonAccountGenericMethods.Contains(method)).Cast<Enum>().OrderBy(method => method.ToString()));

            // Add the Account type methods
            apiMethods.AddRange(APIMethods.Methods.OfType<APIGenericMethods>().Where(
                method => !apiMethods.Contains(method) && !APIMethods.NonAccountGenericMethods.Contains(method) &&
                          !APIMethods.AllSupplementalMethods.Contains(method)).Cast<Enum>().OrderBy(method => method.ToString()));

            // Add the character methods
            apiMethods.AddRange(
                APIMethods.Methods.OfType<APICharacterMethods>().Cast<Enum>().Concat(
                APIMethods.CharacterSupplementalMethods).OrderBy(method => method.ToString()));

            // Add the corporation methods
            apiMethods.AddRange(
                APIMethods.Methods.OfType<APICorporationMethods>().Cast<Enum>().Concat(
                APIMethods.CorporationSupplementalMethods).OrderBy(method => method.ToString()));

            APIMethodComboBox.Items.Clear();
            APIMethodComboBox.Items.AddRange(apiMethods.ToArray<Object>());
        }

        /// <summary>
        /// Updates the character list.
        /// </summary>
        private void UpdateCharacterList()
        {
            CharacterComboBox.Items.Clear();
            CharacterComboBox.Items.AddRange(EveMonClient.Characters.OfType<CCPCharacter>().Where(
                character => character.Identity.APIKeys.Any()).OrderBy(character => character.Name).ToArray<Object>());

            UpdateCharacterSelectionEnabling();
        }

        /// <summary>
        /// Updates the character selection enabling.
        /// </summary>
        private void UpdateCharacterSelectionEnabling()
        {
            CharacterComboBox.Enabled = InternalInfoRadioButton.Checked && CharacterComboBox.Items.Count > 0 && APIMethodComboBox.SelectedItem != null &&
                                  !APIMethods.NonAccountGenericMethods.Contains(APIMethodComboBox.SelectedItem);

            if (!CharacterComboBox.Enabled)
                CharacterComboBox.SelectedItem = null;
        }

        /// <summary>
        /// Updates the external info controls enabling.
        /// </summary>
        private void UpdateExternalInfoControlsEnabling()
        {
            KeyIDTextBox.Enabled = VCodeTextBox.Enabled = ExternalInfoRadioButton.Checked && APIMethodComboBox.SelectedItem != null &&
                                                !APIMethods.NonAccountGenericMethods.Contains(APIMethodComboBox.SelectedItem);

            CharIDLabel.Visible = CharIDTextBox.Visible = ExternalInfoRadioButton.Checked && APIMethodComboBox.SelectedItem != null &&
                                                   (APIMethodComboBox.SelectedItem is APICharacterMethods ||
                                                    APIMethods.CharacterSupplementalMethods.Contains(APIMethodComboBox.SelectedItem) ||
                                                    APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationLocations));

            if (!CharIDTextBox.Visible)
                CharIDTextBox.ResetText();
        }

        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            LoadDocument(new Uri("about:blank"));

            UpdateCharacterSelectionEnabling();
            UpdateExternalInfoControlsEnabling();

            if (APIMethodComboBox.SelectedItem == null)
            {
                IDOrNameLabel.Visible = IDOrNameTextBox.Visible = false;
                return;
            }

            IDOrNameLabel.Visible =
                IDOrNameTextBox.Visible = APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterID) ||
                                     APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterName) ||
                                     APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.TypeName) ||
                                     APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.ContractItems) ||
                                     APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CorporationContractItems) ||
                                     APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CalendarEventAttendees) ||
                                     APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.Locations) ||
                                     APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                                     APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.NotificationTexts) ||
                                     APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationLocations) ||
                                     APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationStarbaseDetails);

            if (!IDOrNameLabel.Visible)
                return;

            IDOrNameLabel.Text = APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterName)
                                   ? "IDs:"
                                   : APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterID)
                                         ? "Names:"
                                         : APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.TypeName)
                                               ? "Type IDs:"
                                               : APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.ContractItems) ||
                                                 APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CorporationContractItems)
                                                     ? "Contract ID:"
                                                     : APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CalendarEventAttendees)
                                                           ? "Event IDs:"
                                                           : APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                                                             APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.NotificationTexts)
                                                                 ? "Message IDs:"
                                                                 : APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.Locations) ||
                                                                   APIMethodComboBox.SelectedItem.Equals(
                                                                       APICorporationMethods.CorporationLocations)
                                                                       ? "Item IDs:"
                                                                       : APIMethodComboBox.SelectedItem.Equals(
                                                                           APICorporationMethods.CorporationStarbaseDetails)
                                                                             ? "Starbase ID:"
                                                                             : String.Empty;
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
        private string GetPostData()
        {
            if (APIMethodComboBox.SelectedItem is APIGenericMethods)
                return GetPostDataForGenericAPIMethods();

            if (APIMethodComboBox.SelectedItem is APICharacterMethods)
                return GetCharacterAPIMethodsPostData();

            if (APIMethodComboBox.SelectedItem is APICorporationMethods)
                return GetCorporationAPIMethodsPostData();
            
            return String.Empty;
        }

        /// <summary>
        /// Gets the post data for generic API methods.
        /// </summary>
        /// <returns></returns>
        private string GetPostDataForGenericAPIMethods()
        {
            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterName) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.TypeName))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataIDsOnly, IDOrNameTextBox.Text);
            }

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterID))
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataNamesOnly, IDOrNameTextBox.Text);

            if (APIMethods.AllSupplementalMethods.Contains(APIMethodComboBox.SelectedItem))
                return SupplementalAPIMethodsPostData();

            if (!APIMethods.NonAccountGenericMethods.Contains(APIMethodComboBox.SelectedItem))
            {
                if (InternalInfoRadioButton.Checked)
                {
                    if (CharacterComboBox.SelectedItem == null)
                        return String.Empty;

                    Character character = (Character)CharacterComboBox.SelectedItem;
                    APIKey apiKey = character.Identity.APIKeys.FirstOrDefault(key => key.IsCharacterOrAccountType);

                    return apiKey == null
                               ? String.Empty
                               : String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                               apiKey.ID, apiKey.VerificationCode);
                }

                if (ExternalInfoRadioButton.Checked)
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                         KeyIDTextBox.Text, VCodeTextBox.Text);
            }

            return String.Empty;
        }

        /// <summary>
        /// Gets the post data for the supplemental API methods.
        /// </summary>
        /// <returns></returns>
        private string SupplementalAPIMethodsPostData()
        {
            if (InternalInfoRadioButton.Checked)
            {
                if (CharacterComboBox.SelectedItem == null)
                    return String.Empty;

                Character character = (Character)CharacterComboBox.SelectedItem;
                APIKey apiKey = null;

                if (APIMethodComboBox.SelectedItem.ToString().StartsWith("CorporationContract", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(APICorporationMethods.CorporationContracts);

                if (APIMethodComboBox.SelectedItem.ToString().StartsWith("Contract", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(APICharacterMethods.Contracts);

                if (apiKey == null)
                    return NoAPIKeyWithAccess;

                if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.ContractItems) ||
                    APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CorporationContractItems))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndContractID,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameTextBox.Text);
                }

                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                     apiKey.ID, apiKey.VerificationCode, character.CharacterID);
            }

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.ContractItems) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CorporationContractItems))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndContractID,
                                     KeyIDTextBox.Text, VCodeTextBox.Text, CharIDTextBox.Text, IDOrNameTextBox.Text);
            }

            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                 KeyIDTextBox.Text, VCodeTextBox.Text, CharIDTextBox.Text);
        }

        /// <summary>
        /// Gets the post data for character API methods.
        /// </summary>
        /// <returns></returns>
        private String GetCharacterAPIMethodsPostData()
        {
            if (InternalInfoRadioButton.Checked)
            {
                if (CharacterComboBox.SelectedItem == null)
                    return String.Empty;

                Character character = (Character)CharacterComboBox.SelectedItem;
                APIKey apiKey = character.Identity.FindAPIKeyWithAccess((APICharacterMethods)APIMethodComboBox.SelectedItem);

                if (apiKey == null)
                    return NoAPIKeyWithAccess;

                if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CalendarEventAttendees) ||
                    APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.Locations) ||
                    APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                    APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.NotificationTexts))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameTextBox.Text);
                }

                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                     apiKey.ID, apiKey.VerificationCode, character.CharacterID);
            }

            if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CharacterInfo) &&
                (KeyIDTextBox.Text.Length == 0 || VCodeTextBox.Text.Length == 0))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCharacterIDOnly,
                                     CharIDTextBox.Text);
            }

            if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.Locations) ||
                APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.NotificationTexts))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                     KeyIDTextBox.Text, VCodeTextBox.Text, CharIDTextBox.Text, IDOrNameTextBox.Text);
            }

            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                 KeyIDTextBox.Text, VCodeTextBox.Text, CharIDTextBox.Text);
        }

        /// <summary>
        /// Gets the post data for corporation API methods.
        /// </summary>
        /// <returns></returns>
        private String GetCorporationAPIMethodsPostData()
        {
            if (InternalInfoRadioButton.Checked)
            {
                if (CharacterComboBox.SelectedItem == null)
                    return String.Empty;

                Character character = (Character)CharacterComboBox.SelectedItem;
                APIKey apiKey = character.Identity.FindAPIKeyWithAccess((APICorporationMethods)APIMethodComboBox.SelectedItem);

                if (apiKey == null)
                    return NoAPIKeyWithAccess;

                if (APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationLocations))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameTextBox.Text);
                }

                if (APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationMemberTrackingExtended))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithExtendedParameter,
                                         apiKey.ID, apiKey.VerificationCode);
                }

                if (APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationStarbaseDetails))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithItemID,
                                         apiKey.ID, apiKey.VerificationCode, IDOrNameTextBox.Text);
                }

                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                     apiKey.ID, apiKey.VerificationCode);
            }

            if (APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationLocations))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                     KeyIDTextBox.Text, VCodeTextBox.Text, CharIDTextBox, IDOrNameTextBox.Text);
            }

            if (APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationMemberTrackingExtended))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithExtendedParameter,
                                     KeyIDTextBox.Text, VCodeTextBox.Text);
            }

            if (APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationStarbaseDetails))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithItemID,
                                     KeyIDTextBox.Text, VCodeTextBox.Text, IDOrNameTextBox.Text);
            }

            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                 KeyIDTextBox.Text, VCodeTextBox.Text);
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="url">The URL.</param>
        private void LoadDocument(Uri url)
        {
            APIUrlLabel.Text = url.AbsoluteUri != "about:blank"
                                 ? String.Format(CultureConstants.InvariantCulture, "URL: {0}", url.AbsoluteUri)
                                 : String.Empty;

            m_url = url;
            WebBrowser.Navigate(url);
        }

        /// <summary>
        /// Saves the document to the disk.
        /// </summary>
        private void SaveDocument()
        {
            if (WebBrowser.Document == null || WebBrowser.Document.Body == null)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string path = Url.AbsolutePath;

                sfd.Filter = "XML (*.xml)|*.xml";
                sfd.FileName = path.Substring(path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1,
                                                path.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) -
                                                path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1);

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    XmlDocument xdoc = new XmlDocument();
                    string innerText = WebBrowser.Document.Body.InnerText.Trim().Replace("\n-", "\n");
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
            APIUrlLabel.Text = String.Empty;

            UpdateAPIMethodsList();
            UpdateCharacterList();

            UpdateControlsVisibility();

            EveMonClient.APIKeyInfoUpdated += EveMonClient_APIKeyInfoUpdated;
            EveMonClient.CharacterCollectionChanged += EveMonClient_CharacterCollectionChanged;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Called when disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.APIKeyInfoUpdated -= EveMonClient_APIKeyInfoUpdated;
            EveMonClient.CharacterCollectionChanged -= EveMonClient_CharacterCollectionChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Handles the APIKeyInfoUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_APIKeyInfoUpdated(object sender, EventArgs e)
        {
            UpdateCharacterList();
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Handles the CharacterCollectionChanged event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterCollectionChanged(object sender, EventArgs e)
        {
            UpdateCharacterList();
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the InternalInfoRadioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InternalInfoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the ExternalInfoRadioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExternalInfoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the APIMethodComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void APIMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset controls
            SaveButton.Enabled = false;
            CharacterComboBox.SelectedItem = null;
            IDOrNameTextBox.ResetText();

            UpdateControlsVisibility();

            if (APIMethods.NonAccountGenericMethods.Contains(APIMethodComboBox.SelectedItem) && !IDOrNameTextBox.Visible)
                LoadDocument(Url);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the CharacterComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CharacterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveButton.Enabled = false;
            if (CharacterComboBox.SelectedItem != null && !IDOrNameTextBox.Visible)
                LoadDocument(Url);
        }

        /// <summary>
        /// Handles the Navigating event of the WebBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/> instance containing the event data.</param>
        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Prevents the browser to navigate past the shown page
            if (e.Url != m_url)
                e.Cancel = true;
        }

        /// <summary>
        /// Handles the DocumentCompleted event of the WebBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserDocumentCompletedEventArgs"/> instance containing the event data.</param>
        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Enable "Save" button on condition
            SaveButton.Enabled = (m_url == e.Url && e.Url.AbsoluteUri != "about:blank" &&
                                 WebBrowser.Document != null && WebBrowser.Document.Body != null);
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the WebBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
        private void WebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Disable the reload shortcut key
            WebBrowser.WebBrowserShortcutsEnabled = e.KeyData != Keys.F5;
        }

        /// <summary>
        /// Handles the Click event of the SaveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        /// <summary>
        /// Handles the Validating of a text box that requires an ID.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void RequiredIDTextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox == null || !textbox.Visible || !textbox.Enabled)
                return;

            // Special condition in case user requests character info without API credentials
            if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CharacterInfo) &&
                textbox.Equals(KeyIDTextBox) && textbox.Text.Length == 0 && VCodeTextBox.Text.Length == 0)
                return;

            if (String.IsNullOrEmpty(textbox.Text))
            {
                ErrorProvider.SetError(textbox, "ID cannot be blank.");
                e.Cancel = true;
                return;
            }

            if (textbox.TextLength == 1 && textbox.Text == "0")
            {
                ErrorProvider.SetError(textbox, "ID must not be zero.");
                e.Cancel = true;
                return;
            }

            if (textbox.Text.StartsWith("0", StringComparison.Ordinal))
            {
                ErrorProvider.SetError(textbox, "ID must not start with zero.");
                e.Cancel = true;
                return;
            }

            for (int i = 0; i < textbox.TextLength; i++)
            {
                if (Char.IsDigit(textbox.Text[i]))
                    continue;

                ErrorProvider.SetError(textbox, "ID must be numerical.");
                e.Cancel = true;
                break;
            }
        }

        /// <summary>
        /// Handles the Validating event of the VCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void VCodeTextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox == null || !textbox.Enabled)
                return;

            // Special condition in case user requests character info without API credentials
            if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CharacterInfo) &&
                KeyIDTextBox.Text.Length == 0 && VCodeTextBox.Text.Length == 0)
                return;

            string vCode = VCodeTextBox.Text.Trim();
            if (vCode.Length != 0)
                return;

            ErrorProvider.SetError(VCodeTextBox, "Verification Code cannot be blank.");
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the Validated event of the CharIDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CharIDTextBox_Validated(object sender, EventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox == null)
                return;

            ErrorProvider.SetError(textbox, String.Empty);
        }

        /// <summary>
        /// Handles the Validated event of the VCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void VCodeTextBox_Validated(object sender, EventArgs e)
        {
            ErrorProvider.SetError(VCodeTextBox, String.Empty);
        }

        /// <summary>
        /// Handles the KeyUp event of the IDOrNameTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void IDOrNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                SaveButton.Enabled = false;

            if (!e.KeyCode.Equals(Keys.Enter))
                return;

            if ((APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                 APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.NotificationTexts)) && CharacterComboBox.SelectedItem == null)
                return;

            LoadDocument(Url);
        }

        /// <summary>
        /// Handles the KeyUp event of the KeyIDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void KeyIDTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                SaveButton.Enabled = false;

            if (e.KeyCode.Equals(Keys.Enter))
                CheckCredentialsValidation();
        }

        /// <summary>
        /// Handles the KeyUp event of the VCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void VCodeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                SaveButton.Enabled = false;

            if (e.KeyCode.Equals(Keys.Enter))
                CheckCredentialsValidation();
        }

        /// <summary>
        /// Handles the KeyUp event of the CharIDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void CharIDTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab))
                SaveButton.Enabled = false;

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
                if (IDOrNameTextBox.Focused)
                    IDOrNameTextBox.ResetText();
                else if (KeyIDTextBox.Focused)
                    KeyIDTextBox.ResetText();
                else if (VCodeTextBox.Focused)
                    VCodeTextBox.ResetText();
                else if (CharIDTextBox.Focused)
                    CharIDTextBox.ResetText();
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
