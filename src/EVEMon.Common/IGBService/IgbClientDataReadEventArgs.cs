using System;
using System.Collections.Generic;

namespace EVEMon.Common.IGBService
{
    public class IgbClientDataReadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgbClientDataReadEventArgs"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="count">The count.</param>
        public IgbClientDataReadEventArgs(IEnumerable<byte> buffer, int count)
        {
            Buffer = buffer;
            Count = count;
        }

        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        /// <value>The buffer.</value>
        public IEnumerable<byte> Buffer { get; private set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; private set; }
    }
}