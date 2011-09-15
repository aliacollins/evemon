using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class ApiKeyDeletionWindow : EVEMonForm
    {
        private readonly APIKey m_apiKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiKey"></param>
        public ApiKeyDeletionWindow(APIKey apiKey)
        {
            InitializeComponent();
            m_apiKey = apiKey;

            // Add characters
            charactersListView.Items.Clear();
            foreach (ListViewItem item in apiKey.CharacterIdentities.Select(
                id => id.CCPCharacter).Where(ccpCharacter => ccpCharacter != null).Select(
                    ccpCharacter => new ListViewItem(ccpCharacter.Name) { Tag = ccpCharacter, Checked = true }))
            {
                charactersListView.Items.Add(item);
            }
        }

        /// <summary>
        /// "Delete" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Remove the API Key
            EveMonClient.APIKeys.Remove(m_apiKey);

            // Remove the characters
            foreach (CCPCharacter ccpCharacter in charactersListView.Items.Cast<ListViewItem>().Where(
                item => item.Checked).Select(item => item.Tag as CCPCharacter))
            {
                EveMonClient.Characters.Remove(ccpCharacter);
            }

            // Closes the window
            Close();
        }
    }
}