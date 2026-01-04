using System;
using EVEMon.Common.Models;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class AttributeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="remapping">The remapping.</param>
        public AttributeChangedEventArgs(RemappingResult remapping)
        {
            Remapping = remapping;
        }

        /// <summary>
        /// Gets the remapping.
        /// </summary>
        /// <value>The remapping.</value>
        public RemappingResult Remapping { get; }
    }
}
