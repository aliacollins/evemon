using System;
using EVEMon.Common.Models;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class APIKeyInfoChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiKey"></param>
        public APIKeyInfoChangedEventArgs(ESIKey apiKey)
        {
            APIKey = apiKey;
        }

        /// <summary>
        /// Gets the API key related to this event.
        /// </summary>
        public ESIKey APIKey { get; }
    }
}
