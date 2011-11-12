using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class ApiKeysManagementWindow : EVEMonForm
    {
        private int m_refreshingCharactersCounter;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiKeysManagementWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On loading, intialize the controls and subscribe events.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            apiKeysListBox.Font = FontFactory.GetFont("Tahoma", 9.75f);
            charactersListView.Font = FontFactory.GetFont("Tahoma", 9.75f);
            ListViewHelper.EnableDoubleBuffer(charactersListView);

            EveMonClient.APIKeyCollectionChanged += EveMonClient_APIKeyCollectionChanged;
            EveMonClient.APIKeyInfoUpdated += EveMonClient_APIKeyInfoUpdated;
            EveMonClient.CharacterCollectionChanged += EveMonClient_CharacterCollectionChanged;
            EveMonClient.MonitoredCharacterCollectionChanged += EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.AccountStatusUpdated += EveMonClient_AccountStatusUpdated;

            UpdateAPIKeysList();
            UpdateCharactersList();
            AdjustLastColumn();

            // Selects the second page if no API key known so far
            if (EveMonClient.Characters.Count == 0)
                tabControl.SelectedIndex = 1;
        }

        /// <summary>
        /// On closing, unsubscribe events.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            EveMonClient.APIKeyCollectionChanged -= EveMonClient_APIKeyCollectionChanged;
            EveMonClient.APIKeyInfoUpdated -= EveMonClient_APIKeyInfoUpdated;
            EveMonClient.CharacterCollectionChanged -= EveMonClient_CharacterCollectionChanged;
            EveMonClient.MonitoredCharacterCollectionChanged -= EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.AccountStatusUpdated -= EveMonClient_AccountStatusUpdated;
            base.OnClosing(e);
        }

        /// <summary>
        /// When the size changes, we adjust the characters' columns.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (charactersListView != null)
                AdjustLastColumn();

            base.OnSizeChanged(e);
        }


        #region Global Events Handlers

        /// <summary>
        /// When the API key collection changes, we update the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_APIKeyCollectionChanged(object sender, EventArgs e)
        {
            UpdateAPIKeysList();
        }

        /// <summary>
        /// When the API key info updates, we update the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_APIKeyInfoUpdated(object sender, EventArgs e)
        {
            if (!Visible)
                return;

            apiKeysListBox.Invalidate();
        }

        /// <summary>
        /// When the characters collection changed, we update the characters list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterCollectionChanged(object sender, EventArgs e)
        {
            UpdateCharactersList();
        }

        /// <summary>
        /// When the monitored characters collection changed, we update the characters list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_MonitoredCharacterCollectionChanged(object sender, EventArgs e)
        {
            UpdateCharactersList();
        }

        /// <summary>
        /// When the character changes, the displayed names changes too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            UpdateCharactersList();
        }

        /// <summary>
        /// When the account status updates, we update the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_AccountStatusUpdated(object sender, EventArgs e)
        {
            if (!Visible)
                return;

            apiKeysListBox.Invalidate();
        }

        #endregion


        #region API keys management

        /// <summary>
        /// Updates the API keys list.
        /// </summary>
        private void UpdateAPIKeysList()
        {
            if (!Visible)
                return;

            apiKeysListBox.APIKeys = EveMonClient.APIKeys;
            apiKeysMultiPanel.SelectedPage = (EveMonClient.APIKeys.IsEmpty() ? noAPIKeysPage : apiKeysListPage);
        }

        /// <summary>
        /// Handles the MouseClick event of the apiKeysListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void apiKeysListBox_MouseClick(object sender, MouseEventArgs e)
        {
            bool itemClicked = false;

            // Search for the clicked item
            for (int index = 0; index < apiKeysListBox.APIKeys.Count(); index++)
            {
                Rectangle rect = apiKeysListBox.GetItemRectangle(index);

                // Did click occured generally on the item ?
                if (!rect.Contains(e.Location))
                    continue;

                itemClicked = true;

                int yOffset = (rect.Height - ApiKeysListBox.CheckBoxSize.Height) / 2;
                Rectangle cbRect = new Rectangle(rect.Left + apiKeysListBox.Margin.Left, rect.Top + yOffset,
                                                 ApiKeysListBox.CheckBoxSize.Width, ApiKeysListBox.CheckBoxSize.Height);
                cbRect.Inflate(2, 2);

                // Did click occured on the checkbox ?
                if (e.Button == MouseButtons.Middle || !cbRect.Contains(e.Location))
                    continue;

                APIKey apiKey = apiKeysListBox.APIKeys.ElementAt(index);
                apiKey.Monitored = !apiKey.Monitored;
                apiKeysListBox.Invalidate();
            }

            if (!itemClicked)
                apiKeysListBox.SelectedIndex = -1;
        }

        /// <summary>
        /// When the selection changes, we update the controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void apiKeysListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteAPIKeyMenu.Enabled = (apiKeysListBox.SelectedIndex != -1);
            editAPIKeyMenu.Enabled = (apiKeysListBox.SelectedIndex != -1);
        }

        /// <summary>
        /// On double click, forces the edition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void apiKeysListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Search for the double-clicked item
            int index = 0;
            foreach (APIKey apiKey in apiKeysListBox.APIKeys)
            {
                Rectangle rect = apiKeysListBox.GetItemRectangle(index);
                index++;

                if (!rect.Contains(e.Location))
                    continue;

                // Open the edition window
                using (ApiKeyUpdateOrAdditionWindow window = new ApiKeyUpdateOrAdditionWindow(apiKey))
                {
                    window.ShowDialog(this);
                    return;
                }
            }
        }

        /// <summary>
        /// API key toolbar > Edit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editAPIKeyMenu_Click(object sender, EventArgs e)
        {
            APIKey apiKey = apiKeysListBox.APIKeys.ElementAt(apiKeysListBox.SelectedIndex);
            using (ApiKeyUpdateOrAdditionWindow window = new ApiKeyUpdateOrAdditionWindow(apiKey))
            {
                window.ShowDialog(this);
            }
        }

        /// <summary>
        /// API key toolbar > Add.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addAPIKeyMenu_Click(object sender, EventArgs e)
        {
            using (ApiKeyUpdateOrAdditionWindow window = new ApiKeyUpdateOrAdditionWindow())
            {
                window.ShowDialog(this);
            }
        }

        /// <summary>
        /// Accounts toolbar > Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteAPIKeyMenu_Click(object sender, EventArgs e)
        {
            APIKey apiKey = apiKeysListBox.APIKeys.ElementAt(apiKeysListBox.SelectedIndex);
            using (ApiKeyDeletionWindow window = new ApiKeyDeletionWindow(apiKey))
            {
                window.ShowDialog(this);
            }
            deleteAPIKeyMenu.Enabled = (apiKeysListBox.SelectedIndex != -1);
            editAPIKeyMenu.Enabled = (apiKeysListBox.SelectedIndex != -1);
        }

        /// <summary>
        /// Handles the KeyDown event of the apiKeysListBox control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void apiKeysListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteAPIKeyMenu_Click(sender, e);
        }

        #endregion


        #region Characters management

        /// <summary>
        /// Updates the characters list.
        /// </summary>
        private void UpdateCharactersList()
        {
            if (!Visible)
                return;

            // Begin the update
            m_refreshingCharactersCounter++;

            // Update the list view item
            UpdateCharactersListContent();

            // Invalidates the API keys list
            apiKeysListBox.Invalidate();

            // Make a help message appears when no characters exist
            charactersMultiPanel.SelectedPage = EveMonClient.Characters.Count == 0 ? noCharactersPage : charactersListPage;

            // End of the update
            m_refreshingCharactersCounter--;
        }

        /// <summary>
        /// Recreate the items in the characters listview
        /// </summary>
        private void UpdateCharactersListContent()
        {
            charactersListView.BeginUpdate();
            try
            {
                // Retrieve current selection and grouping option
                List<Character> oldSelection =
                    new List<Character>(charactersListView.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as Character));
                charactersListView.Groups.Clear();
                charactersListView.Items.Clear();

                // Grouping (no API key, API key #1, API key #2, character files, character urls)
                bool isGrouping = groupingMenu.Checked;
                ListViewGroup noAPIKeyGroup = new ListViewGroup("No API key");
                ListViewGroup fileGroup = new ListViewGroup("Character files");
                ListViewGroup urlGroup = new ListViewGroup("Character urls");
                Dictionary<APIKey, ListViewGroup> apiKeyGroups = new Dictionary<APIKey, ListViewGroup>();

                if (isGrouping)
                {
                    bool hasNoAPIKey = false;
                    bool hasFileChars = false;
                    bool hasUrlChars = false;

                    // Scroll through listview items to gather the groups
                    foreach (Character character in EveMonClient.Characters)
                    {
                        UriCharacter uriCharacter = character as UriCharacter;

                        // Uri character ?
                        if (uriCharacter != null)
                        {
                            if (uriCharacter.Uri.IsFile)
                                hasFileChars = true;
                            else
                                hasUrlChars = true;
                        }
                            // CCP character ?
                        else
                        {
                            if (character.Identity.APIKeys.IsEmpty())
                                hasNoAPIKey = true;
                            else
                            {
                                foreach (APIKey apiKey in character.Identity.APIKeys.Where(
                                    apiKey => !apiKeyGroups.ContainsKey(apiKey)))
                                {
                                    apiKeyGroups.Add(apiKey,
                                                     new ListViewGroup(String.Format(CultureConstants.DefaultCulture,
                                                                                     "Key ID #{0}", apiKey.ID)));
                                }
                            }
                        }
                    }

                    // Add the groups
                    if (hasNoAPIKey)
                        charactersListView.Groups.Add(noAPIKeyGroup);

                    foreach (ListViewGroup group in apiKeyGroups.Values)
                    {
                        charactersListView.Groups.Add(group);
                    }

                    if (hasFileChars)
                        charactersListView.Groups.Add(fileGroup);

                    if (hasUrlChars)
                        charactersListView.Groups.Add(urlGroup);
                }

                // Add items
                foreach (Character character in EveMonClient.Characters.OrderBy(x => x.Name))
                {
                    ListViewItem item = new ListViewItem { Checked = character.Monitored, Tag = character };

                    // Retrieve the texts for the different columns
                    IEnumerable<APIKey> apiKeys = character.Identity.APIKeys.OrderBy(apiKey => apiKey.ID);
                    string apiKeyIDText = (apiKeys.IsEmpty()
                                               ? String.Empty
                                               : String.Join(", ", apiKeys.Select(apiKey => apiKey.ID)));
                    string typeText = "CCP";
                    string uriText = "-";

                    UriCharacter uriCharacter = character as UriCharacter;
                    if (uriCharacter != null)
                    {
                        typeText = (uriCharacter.Uri.IsFile ? "File" : "Url");
                        uriText = uriCharacter.Uri.ToString();

                        if (isGrouping)
                            item.Group = (uriCharacter.Uri.IsFile ? fileGroup : urlGroup);
                    }
                        // Grouping CCP characters
                    else if (isGrouping)
                    {
                        if (apiKeys.IsEmpty())
                            item.Group = noAPIKeyGroup;
                        else
                        {
                            APIKey accountTypeAPIKey = apiKeys.FirstOrDefault(apiKey => apiKey.Type == APIKeyType.Account);
                            item.Group = accountTypeAPIKey != null
                                             ? apiKeyGroups[accountTypeAPIKey]
                                             : apiKeyGroups[apiKeys.First()];
                        }
                    }

                    // Add the item and its subitems
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, typeText));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, character.CharacterID.ToString(CultureConstants.DefaultCulture)));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, character.Name));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, apiKeyIDText));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, uriText));

                    charactersListView.Items.Add(item);
                    if (oldSelection.Contains(character))
                        item.Selected = true;
                }

                // Auto-resize the columns
                charactersListView.Columns.Cast<ColumnHeader>().Where(
                    column => column.Index != charactersListView.Columns.Count - 1).ToList().ForEach(column => column.Width = -2);

                AdjustLastColumn();
            }
            finally
            {
                charactersListView.EndUpdate();
            }

            // Forces a refresh of the enabled/disabled items
            UpdateControlsUsability();
        }

        /// <summary>
        /// Adjusts the last column width.
        /// </summary>
        private void AdjustLastColumn()
        {
            ColumnHeader lastColumn = charactersListView.Columns[charactersListView.Columns.Count - 1];
            int pad = Size.Width - charactersListView.Size.Width;
            int width = (charactersListView.Columns.Cast<ColumnHeader>().Where(column => column.Index != lastColumn.Index).Select(
                column => column.Width)).Sum();

            int lastColumnMaxWidth = charactersListView.Columns[lastColumn.Index].ListView.Items.Cast<ListViewItem>().Select(
                item => TextRenderer.MeasureText(item.SubItems[lastColumn.Index].Text, Font).Width).Concat(
                    new[] { TextRenderer.MeasureText(charactersListView.Columns[lastColumn.Index].Text, Font).Width }).
                                         Concat(new[] { charactersListView.ClientSize.Width - width - pad }).Max() + pad;

            lastColumn.Width = lastColumnMaxWidth;
        }

        /// <summary>
        /// Updates the controls usability.
        /// </summary>
        private void UpdateControlsUsability()
        {
            // "Edit uri" enabled when an uri char is selected
            editUriMenu.Enabled = (charactersListView.SelectedItems.Count != 0) &&
                                  ((charactersListView.SelectedItems[0].Tag as UriCharacter) != null);

            // Delete char enabled if one character selected
            deleteCharacterMenu.Enabled = (charactersListView.SelectedItems.Count != 0);
        }

        /// <summary>
        /// We monitor/unmonitor characters as they are checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void charactersListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (m_refreshingCharactersCounter != 0)
                return;

            Character character = (Character)e.Item.Tag;
            character.Monitored = e.Item.Checked;
        }

        /// <summary>
        /// Handle the "delete" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void charactersListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteCharacterMenu_Click(sender, e);
        }

        /// <summary>
        /// On double click, we edit if this is an uri character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void charactersListView_DoubleClick(object sender, EventArgs e)
        {
            editUriButton_Click(sender, e);
        }

        /// <summary>
        /// When the index changes, we enable or disable the toolbar buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void charactersListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsUsability();
        }

        /// <summary>
        /// Characters toolbar > Import...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importCharacterMenu_Click(object sender, EventArgs e)
        {
            using (CharacterImportationWindow form = new CharacterImportationWindow())
            {
                form.ShowDialog(this);
            }
        }

        /// <summary>
        /// Characters toolbar > Delete...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteCharacterMenu_Click(object sender, EventArgs e)
        {
            // Retrieve the selected URI character
            if (charactersListView.SelectedItems.Count == 0)
                return;

            ListViewItem item = charactersListView.SelectedItems[0];
            Character character = item.Tag as Character;

            // Opens the character deletion
            using (CharacterDeletionWindow window = new CharacterDeletionWindow(character))
            {
                window.ShowDialog(this);
            }
        }

        /// <summary>
        /// Characters toolbar > Edit Uri...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editUriButton_Click(object sender, EventArgs e)
        {
            // Retrieve the selected URI character
            if (charactersListView.SelectedItems.Count == 0)
                return;

            ListViewItem item = charactersListView.SelectedItems[0];
            UriCharacter uriCharacter = item.Tag as UriCharacter;

            // Returns if the selected item is not an Uri character
            if (uriCharacter == null)
                return;

            // Opens the importation form
            using (CharacterImportationWindow form = new CharacterImportationWindow(uriCharacter))
            {
                form.ShowDialog(this);
            }
        }

        /// <summary>
        /// Characters toolbar > Group items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupingMenu_Click(object sender, EventArgs e)
        {
            m_refreshingCharactersCounter++;
            UpdateCharactersListContent();
            m_refreshingCharactersCounter--;
        }

        /// <summary>
        /// Close on "close" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

    }
}