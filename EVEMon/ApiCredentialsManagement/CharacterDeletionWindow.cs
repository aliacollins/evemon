using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class CharacterDeletionWindow : EVEMonForm
    {
        private readonly Character m_character;
        private List<APIKey> m_apiKeys;

        /// <summary>
        /// Constructor.
        /// </summary>
        private CharacterDeletionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public CharacterDeletionWindow(Character character)
            : this()
        {
            m_character = character;
        }

        /// <summary>
        /// Occurs when the control loads.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
                return;

            apiKeyslistView.Items.Clear();

            // Replaces end of text with character's name
            characterToRemoveLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                                        characterToRemoveLabel.Text, m_character.Name);

            // Find the API keys bind only to this character
            m_apiKeys = EveMonClient.APIKeys.Select(
                apiKey => new { apiKey, identities = apiKey.CharacterIdentities }).Where(
                    apiKey => apiKey.identities.Count() == 1 && apiKey.identities.Contains(m_character.Identity)).Select(
                        apiKey => apiKey.apiKey).ToList();

            apiKeyslistView.Items.AddRange(m_apiKeys.Select(
                apiKey => new ListViewItem(apiKey.ID.ToString(CultureConstants.DefaultCulture))).ToArray());

            // Checks whether there will be no characters left after this deletion and hide/display the relevant labels
            bool noCharactersLeft = (!m_apiKeys.IsEmpty() && m_character is CCPCharacter);
            deleteAPIKeyCheckBox.Text = String.Format(CultureConstants.DefaultCulture, deleteAPIKeyCheckBox.Text,
                                                      m_apiKeys.Count > 1 ? "s" : String.Empty);
            noCharactersLabel.Text = String.Format(CultureConstants.DefaultCulture, noCharactersLabel.Text,
                                                   m_apiKeys.Count > 1 ? "s" : String.Empty);

            deleteAPIKeyCheckBox.Visible = noCharactersLeft;
            noCharactersLabel.Visible = noCharactersLeft;

            // Resize window if there is no API key to remove
            if (!noCharactersLeft)
                Size = new Size(Size.Width, Size.Height - (apiKeyslistView.Height / 2));

        }

        /// <summary>
        /// Delete button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Either delete this character only or the whole API key
            if (deleteAPIKeyCheckBox.Checked)
            {
                // Note: Keep this order of removal
                m_apiKeys.ForEach(apiKey => EveMonClient.APIKeys.Remove(apiKey));
                EveMonClient.Characters.Remove(m_character);
            }
            else
                EveMonClient.Characters.Remove(m_character);

            Close();
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}