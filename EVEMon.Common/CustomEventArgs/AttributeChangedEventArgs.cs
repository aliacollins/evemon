using System;

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
        /// Gets or sets the remapping.
        /// </summary>
        /// <value>The remapping.</value>
        public RemappingResult Remapping { get; private set; }
    }
}
