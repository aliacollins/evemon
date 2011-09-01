using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveMailingListsCollection : ReadonlyCollection<EveMailingList>
    {
        private readonly Character m_character;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        public EveMailingListsCollection(Character character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal void Import(IEnumerable<SerializableMailingListsListItem> src)
        {
            Items.Clear();

            // Import the mail messages from the API
            foreach (SerializableMailingListsListItem srcEVEMailingList in src)
            {
                Items.Add(new EveMailingList(srcEVEMailingList));
            }
        }
    }
}