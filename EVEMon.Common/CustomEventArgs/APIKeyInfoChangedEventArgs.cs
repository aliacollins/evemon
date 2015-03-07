using System;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class APIKeyInfoChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiKey"></param>
        public APIKeyInfoChangedEventArgs(APIKey apiKey)
        {
            APIKey = apiKey;
        }

        /// <summary>
        /// Gets or sets the API key related to this event.
        /// </summary>
        public APIKey APIKey { get; private set; }
    }
}
