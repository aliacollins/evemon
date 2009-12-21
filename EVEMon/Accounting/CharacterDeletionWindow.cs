using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common;

namespace EVEMon.Accounting
{
    public partial class CharacterDeletionWindow : EVEMonForm
    {
        private readonly Account m_account;
        private readonly Character m_character;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public CharacterDeletionWindow(Character character)
        {
            InitializeComponent();
            m_character = character;
            m_account = character.Identity.Account;

            // Checks whether there will be no characters left after this deletion and hide/display the relevant labels.
            int charactersLeft = EveClient.Characters.Count(x => x.Identity.Account == m_account);
            bool noCharactersLeft = (m_account != null && m_character is CCPCharacter && charactersLeft == 1);
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
                EveClient.Accounts.Remove(m_account, true);
            }
            else
            {
                EveClient.Characters.Remove(m_character);
            }

            this.Close();
        }
    }
}
