namespace EVEMon.Common
{
    /// <summary>
    /// Represents a monitor for all queries related to characters and their corporations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CharacterQueryMonitor<T> : QueryMonitor<T>
    {
        private readonly Character m_character;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="method"></param>
        internal CharacterQueryMonitor(Character character, APIMethods method)
            : base(method)
        {
            m_character = character;
        }

        /// <summary>
        /// Gets the required API key information are known.
        /// </summary>
        /// <returns>False if an API key was required and not found.</returns>
        protected override bool HasAPIKey
        {
            get { return m_character.Identity.APIKey != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this monitor has access to data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this monitor has access; otherwise, <c>false</c>.
        /// </value>
        public override bool HasAccess
        {
            get { return (int)Method == (m_character.Identity.APIKey.AccessMask & (int)Method); }
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        protected override void QueryAsyncCore(APIProvider provider, QueryCallback<T> callback)
        {
            APIKey apiKey = m_character.Identity.APIKey;
            provider.QueryMethodAsync(Method, apiKey.ID, apiKey.VerificationCode, m_character.CharacterID, callback);
        }
    }
}