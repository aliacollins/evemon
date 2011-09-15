
using System.Linq;

namespace EVEMon.Common
{
    public sealed class APIKeyQueryMonitor<T> : QueryMonitor<T>
    {
        private readonly APIKey m_apiKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="method"></param>
        internal APIKeyQueryMonitor(APIKey apiKey, APIMethods method)
            : base(method)
        {
            m_apiKey = apiKey;
        }

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
                if (!EnumExtensions.GetBitValues<APIMethods>().Contains(Method))
                    return true;

                return (int)Method == (m_apiKey.AccessMask & (int)Method);
            }
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        protected override void QueryAsyncCore(APIProvider provider, QueryCallback<T> callback)
        {
            provider.QueryMethodAsync(Method, m_apiKey.ID, m_apiKey.VerificationCode, callback);
        }
    }
}