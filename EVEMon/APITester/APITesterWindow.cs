using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.XPath;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.API;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models;
using EVEMon.Common.Net;
using APIMethods = EVEMon.Common.Models.Extended.APIMethods;

namespace EVEMon.ApiTester
{
    public partial class ApiTesterWindow : EVEMonForm
    {
        private Uri m_url;
        private Uri m_defaultUri;
        private IXPathNavigable m_result;


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


        #region Main Methods

        /// <summary>
        /// Updates the API methods list.
        /// </summary>
        private void UpdateAPIMethodsList()
        {
            APIMethodComboBox.Items.Clear();
            APIMethodComboBox.Items.AddRange(ApiTesterUIHelper.GetApiMethods.ToArray<Object>());
        }

        /// <summary>
        /// Updates the character list.
        /// </summary>
        private void UpdateCharacterList()
        {
            CharacterComboBox.Items.Clear();
            CharacterComboBox.Items.AddRange(ApiTesterUIHelper.GetCharacters.ToArray<Object>());
        }

        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            UpdateControlContext();
            LoadDocument(m_defaultUri);
            UpdateInternalInfoControlsEnabling();
            UpdateExternalInfoControlsEnabling();
            UpdateIDOrNameLabelVisibility();
        }

        /// <summary>
        /// Updates the internal info controls enabling.
        /// </summary>
        private void UpdateInternalInfoControlsEnabling()
        {
            CharacterComboBox.Enabled = InternalInfoRadioButton.Checked && CharacterComboBox.Items.Count > 0 &&
                                        APIMethodComboBox.SelectedItem != null &&
                                        !APIMethods.NonAccountGenericMethods.Contains(APIMethodComboBox.SelectedItem);

            if (!CharacterComboBox.Enabled)
                CharacterComboBox.SelectedItem = null;
        }

        /// <summary>
        /// Updates the external info controls enabling.
        /// </summary>
        private void UpdateExternalInfoControlsEnabling()
        {
            KeyIDTextBox.Enabled =
                VCodeTextBox.Enabled = ExternalInfoRadioButton.Checked && APIMethodComboBox.SelectedItem != null &&
                                       !APIMethods.NonAccountGenericMethods.Contains(APIMethodComboBox.SelectedItem);

            CharIDLabel.Visible =
                CharIDTextBox.Visible = ExternalInfoRadioButton.Checked && APIMethodComboBox.SelectedItem != null &&
                                        (APIMethodComboBox.SelectedItem is APICharacterMethods ||
                                         APIMethods.CharacterSupplementalMethods.Contains(APIMethodComboBox.SelectedItem) ||
                                         APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationSheet) ||
                                         APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationLocations));

            CharIDLabel.Text = APIMethodComboBox.SelectedItem != null &&
                               APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationSheet)
                                   ? "Corporation ID:"
                                   : "Character ID:";

            if (!CharIDTextBox.Visible)
                CharIDTextBox.ResetText();
        }

        /// <summary>
        /// Updates the control context.
        /// </summary>
        private void UpdateControlContext()
        {
            ApiTesterUIHelper.UseInternalInfo = InternalInfoRadioButton.Checked;
            ApiTesterUIHelper.UseExternalInfo = ExternalInfoRadioButton.Checked;
            ApiTesterUIHelper.SelectedItem = APIMethodComboBox.SelectedItem;
            ApiTesterUIHelper.SelectedCharacter = CharacterComboBox.SelectedItem;
            ApiTesterUIHelper.KeyID = KeyIDTextBox.Text;
            ApiTesterUIHelper.VCode = VCodeTextBox.Text;
            ApiTesterUIHelper.CharID = CharIDTextBox.Text;
            ApiTesterUIHelper.IDOrNameText = IDOrNameTextBox.Text;
        }

        /// <summary>
        /// Updates the ID or Name label visibility.
        /// </summary>
        private void UpdateIDOrNameLabelVisibility()
        {
            IDOrNameLabel.Visible = IDOrNameTextBox.Visible = ApiTesterUIHelper.HasIDOrName;

            if (!IDOrNameLabel.Visible || APIMethodComboBox.SelectedItem == null)
                return;

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterName) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterAffiliation))
                IDOrNameLabel.Text = "IDs:";

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterID) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.OwnerID))
                IDOrNameLabel.Text = "Names:";

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.TypeName))
                IDOrNameLabel.Text = "Type IDs:";

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.ContractItems) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CorporationContractItems))
                IDOrNameLabel.Text = "Contract ID:";

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.PlanetaryPins) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.PlanetaryRoutes) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.PlanetaryLinks))
                IDOrNameLabel.Text = "Planet ID:";

            if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CalendarEventAttendees))
                IDOrNameLabel.Text = "Event IDs:";

            if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.MailBodies) ||
                APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.NotificationTexts))
                IDOrNameLabel.Text = "Message IDs:";

            if (APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.Locations) ||
                APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationLocations))
                IDOrNameLabel.Text = "Item IDs:";

            if (APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationStarbaseDetails))
                IDOrNameLabel.Text = "Starbase ID:";
        }

        /// <summary>
        /// Checks the credentials validation.
        /// </summary>
        private void CheckCredentialsValidation()
        {
            if (!ValidateChildren())
                return;

            // When credentials are valid, load the API page
            UpdateControlContext();
            LoadDocument(ApiTesterUIHelper.Url);
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="url">The URL.</param>
        private void LoadDocument(Uri url)
        {
            string errorText = url != m_defaultUri ? ApiTesterUIHelper.ErrorText : String.Empty;

            ErrorProvider.SetError(UrlLabel, errorText);

            UrlLabel.Text = url != m_defaultUri
                                ? String.Format(CultureConstants.InvariantCulture, "URL: {0}", url.AbsoluteUri)
                                : String.Empty;

            m_url = url;

            // Get the raw xml document to use when saving
            if (m_url != m_defaultUri)
            {
                APIProvider provider = EveMonClient.APIProviders.CurrentProvider;
                Uri nUrl = provider.GetMethodUrl((Enum)ApiTesterUIHelper.SelectedItem);
                string postData = m_url.Query.Replace("?", String.Empty);
                try
                {
                    m_result = HttpWebService.DownloadXml(nUrl, HttpMethod.Post, provider.SupportsCompressedResponse, postData);
                }
                catch (HttpWebServiceException ex)
                {
                    ExceptionHandler.LogException(ex, false);
                }
            }

            // Show the xml document using the webbrowser control
            WebBrowser.Navigate(url);
        }

        #endregion


        #region Global Events Handlers

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

        #endregion


        #region Local Events Handlers

        /// <summary>
        /// Handles the Load event of the ApiTesterWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ApiTesterWindow_Load(object sender, EventArgs e)
        {
            m_defaultUri = new Uri("about:blank");
            UrlLabel.Text = String.Empty;

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
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = false;
            base.OnClosing(e);
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

            if (!APIMethods.NonAccountGenericMethods.Contains(APIMethodComboBox.SelectedItem) || IDOrNameTextBox.Visible)
                return;

            LoadDocument(ApiTesterUIHelper.Url);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the CharacterComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CharacterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveButton.Enabled = false;

            if (CharacterComboBox.SelectedItem == null || IDOrNameTextBox.Visible)
                return;

            UpdateControlContext();
            LoadDocument(ApiTesterUIHelper.Url);
        }

        /// <summary>
        /// Handles the Navigating event of the WebBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/> instance containing the event data.</param>
        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Prevents the browser to navigate past the shown page
            e.Cancel = e.Url != m_url;
        }

        /// <summary>
        /// Handles the DocumentCompleted event of the WebBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserDocumentCompletedEventArgs"/> instance containing the event data.</param>
        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Enable "Save" button on condition
            SaveButton.Enabled = (m_url == e.Url && e.Url != m_defaultUri &&
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
            if (m_result == null)
                return;

            string filename = Path.GetFileNameWithoutExtension(WebBrowser.Url.AbsoluteUri);
            ApiTesterUIHelper.SaveDocument(filename, m_result);
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

            // Special condition in case user requests character info or corporation sheet without API credentials
            if ((APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CharacterInfo) ||
                 APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationSheet)) &&
                textbox.Equals(KeyIDTextBox) && textbox.Text.Length == 0 && VCodeTextBox.Text.Length == 0)
                return;

            string[] ids = textbox.Text.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            if (String.IsNullOrEmpty(textbox.Text))
            {
                string errorText = String.Format(CultureConstants.DefaultCulture, "{0} can not be blank.",
                                                 APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterID) ||
                                                 APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.OwnerID)
                                                     ? "Names"
                                                     : "IDs");
                ErrorProvider.SetError(textbox, errorText);
                e.Cancel = true;
                return;
            }

            if (APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.CharacterID) ||
                APIMethodComboBox.SelectedItem.Equals(APIGenericMethods.OwnerID))
                return;

            if (ids.Any(id => id == "0"))
            {
                ErrorProvider.SetError(textbox, "ID must not be zero.");
                e.Cancel = true;
                return;
            }

            if (ids.Any(id => id.StartsWith("0", StringComparison.Ordinal)))
            {
                ErrorProvider.SetError(textbox, "ID must not start with zero.");
                e.Cancel = true;
                return;
            }

            if (ids.All(id => id.ToCharArray().All(Char.IsDigit)))
                return;

            ErrorProvider.SetError(textbox, "ID must be numerical.");
            e.Cancel = true;
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
            if ((APIMethodComboBox.SelectedItem.Equals(APICharacterMethods.CharacterInfo) ||
                 APIMethodComboBox.SelectedItem.Equals(APICorporationMethods.CorporationSheet)) &&
                KeyIDTextBox.Text.Length == 0 && VCodeTextBox.Text.Length == 0)
                return;

            string vCodeText = VCodeTextBox.Text.Trim();
            if (vCodeText.Length != 0)
                return;

            ErrorProvider.SetError(VCodeTextBox, "Verification Code cannot be blank.");
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the Validated event of a text box that requires an ID.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RequiredIDTextBox_Validated(object sender, EventArgs e)
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
            UpdateOnKeyUp(e);
        }

        /// <summary>
        /// Handles the KeyUp event of the KeyIDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void KeyIDTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateOnKeyUp(e);
        }

        /// <summary>
        /// Handles the KeyUp event of the VCodeTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void VCodeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateOnKeyUp(e);
        }

        /// <summary>
        /// Handles the KeyUp event of the CharIDTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void CharIDTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateOnKeyUp(e);
        }

        /// <summary>
        /// Updates controls on key up.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void UpdateOnKeyUp(KeyEventArgs e)
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
