using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class WalletJournalCollection : ReadonlyCollection<WalletJournal>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal WalletJournalCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable wallet journal from the API.</param>
        internal void Import(IEnumerable<SerializableWalletJournalListItem> src)
        {
            Items.Clear();

            // Import the wallet journal from the API
            foreach (SerializableWalletJournalListItem srcWalletJournal in src)
            {
                Items.Add(new WalletJournal(srcWalletJournal));
            }
        }
    }
}