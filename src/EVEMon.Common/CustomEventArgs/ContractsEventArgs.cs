using System;
using System.Collections.Generic;
using EVEMon.Common.Models;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class ContractsEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="endedContracts">The ended contracts.</param>
        public ContractsEventArgs(Character character, IEnumerable<Contract> endedContracts)
        {
            Character = character;
            EndedContracts = endedContracts;
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <value>The character.</value>
        public Character Character { get; }

        /// <summary>
        /// Gets the ended contracts.
        /// </summary>
        /// <value>The ended contracts.</value>
        public IEnumerable<Contract> EndedContracts { get; }
    }
}
