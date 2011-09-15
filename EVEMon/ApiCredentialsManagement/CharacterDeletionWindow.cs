using System;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class CharacterDeletionWindow : EVEMonForm
    {
        private readonly APIKey m_apiKey;
        private readonly Character m_character;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public CharacterDeletionWindow(Character character)
        {
            InitializeComponent();
            m_character = character;
            m_apiKey = character.Identity.APIKey;

            // Replaces end of text with character's name
            string newText = characterToRemoveLabel.Text.Replace("a character", m_character.Name);
            characterToRemoveLabel.Text = newText;

            // Checks whether there will be no characters left after this deletion and hide/display the relevant labels.
            int charactersLeft = EveMonClient.Characters.Count(x => x.Identity.APIKey == m_apiKey);
            bool noCharactersLeft = (m_apiKey != null && m_character is CCPCharacter && charactersLeft == 1);
            noCharactersCheckBox.Visible = noCharactersLeft;
            noCharactersLabel.Visible = noCharactersLeft;
        }

        /// <summary>
        /// Delete button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Either delete this character only or the whole account.
            if (noCharactersCheckBox.Checked)
            {
                EveMonClient.Characters.Remove(m_character);
                EveMonClient.APIKeys.Remove(m_apiKey);
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