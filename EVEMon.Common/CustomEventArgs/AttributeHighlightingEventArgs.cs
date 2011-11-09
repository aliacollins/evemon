using System;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class AttributeHighlightingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeHighlightingEventArgs"/> class.
        /// </summary>
        /// <param name="highlightValue">The highlight value.</param>
        public AttributeHighlightingEventArgs(int highlightValue)
        {
            Value = highlightValue;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value { get; private set; }
    }
}
