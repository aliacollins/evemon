using System;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.QueryMonitor
{
    public sealed class CorporationQueryMonitor<T> : QueryMonitor<T> where T : class
    {
        private readonly Character m_character;
        private ESIKey m_apiKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="method"></param>
        /// <param name="onUpdated"></param>
        public CorporationQueryMonitor(Character character, Enum method, Action<EsiResult<T>> onUpdated)
            : base(method, onUpdated)
        {
            m_character = character;
        }

        /// <summary>
        /// Gets the required API key information are known.
        /// </summary>
        /// <returns>False if an API key was required and not found.</returns>
        protected override bool HasAPIKey => m_character.Identity.ESIKeys.Any();

        /// <summary>
        /// Gets a value indicating whether this monitor has access to data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this monitor has access; otherwise, <c>false</c>.
        /// </value>
        public override bool HasAccess
        {
            get
            {
                m_apiKey = m_character.Identity.FindAPIKeyWithAccess((ESIAPICorporationMethods)Method);
                return m_apiKey != null;
            }
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        /// <exception cref="System.ArgumentNullException">provider</exception>
        protected override void QueryAsyncCore(APIProvider provider, APIProvider.ESIRequestCallback<T> callback)
        {
            provider.ThrowIfNull(nameof(provider));

            provider.QueryEsiAsync(Method, m_apiKey.AccessToken, m_character.CharacterID, callback);
        }
    }
}
