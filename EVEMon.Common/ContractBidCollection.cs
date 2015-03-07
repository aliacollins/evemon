using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class ContractBidCollection : ReadonlyCollection<ContractBid>
    {
        private readonly CCPCharacter m_character;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal ContractBidCollection(CCPCharacter character)
        {
            m_character = character;
        }

        #endregion


        #region Importation / Exportation Methods

        /// <summary>
        /// Imports an enumeration of serializable objects.
        /// </summary>
        /// <param name="src">The source.</param>
        internal void Import(IEnumerable<SerializableContractBid> src)
        {
            Items.Clear();
            foreach (SerializableContractBid srcContractBid in src)
            {
                Items.Add(new ContractBid(srcContractBid));
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The source.</param>
        internal void Import(IEnumerable<SerializableContractBidsListItem> src)
        {
            // Add new bids to collection
            foreach (SerializableContractBidsListItem item in src)
            {
                Items.Add(new ContractBid(item));
            }

            // Copy the collection to a list so we can enumerate them
            List<ContractBid> bids = Items.ToList();

            // Remove bids for non existing contracts
            foreach (ContractBid bid in bids.Where(bid => m_character.CharacterContracts.All(x => x.ID != bid.ContractID)))
            {
                Items.Remove(bid);
            }
        }

        /// <summary>
        /// Exports the contract bids to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<SerializableContractBid> Export()
        {
            return Items.Select(contractBid => contractBid.Export());
        }

        #endregion
    }
}