using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.Accounting
{
    public partial class AccountDeletionWindow : EVEMonForm
    {
        private readonly Account m_account;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account"></param>
        public AccountDeletionWindow(Account account)
        {
            InitializeComponent();
            m_account = account;

            // Add characters
            charactersListView.Items.Clear();
            foreach (CharacterIdentity id in account.CharacterIdentities)
            {
                // Skip if there is no CCP character for this identity.
                CCPCharacter ccpCharacter = id.CCPCharacter;
                if (ccpCharacter == null)
                    continue;

                // Add an item for this character
                ListViewItem item = new ListViewItem(ccpCharacter.Name);
                item.Tag = ccpCharacter;
                item.Checked = true;
                charactersListView.Items.Add(item);
            }
        }

        /// <summary>
        /// "Delete" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void validationButton_Click(object sender, EventArgs e)
        {
            // Remove the account
            EveMonClient.Accounts.Remove(m_account);

            // Remove the characters
            foreach (var ccpCharacter in charactersListView.Items.Cast<ListViewItem>().Where(
                item => item.Checked).Select(item => item.Tag as CCPCharacter))
            {
                EveMonClient.Characters.Remove(ccpCharacter);
            }

            // Closes the window
            Close();
        }
    }
}
