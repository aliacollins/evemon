using System;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class LoadoutFeedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError
        {
            get { return Error != null;}
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets the loadout feed.
        /// </summary>
        /// <value>
        /// The feed.
        /// </value>
        public object LoadoutFeed { get; set; }
    }
}
