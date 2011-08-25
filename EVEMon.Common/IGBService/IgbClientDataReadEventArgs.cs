using System;

namespace EVEMon.Common.IgbService
{
    public class IgbClientDataReadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgbClientDataReadEventArgs"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public IgbClientDataReadEventArgs(byte[] buffer, int offset, int count)
        {
            Buffer = buffer;
            Offset = offset;
            Count = count;
        }

        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        /// <value>The buffer.</value>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; private set; }
    }
}