using System;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.Common.QueryMonitor
{
    /// <summary>
    /// Represents a monitor for all queries related to characters.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public sealed class CharacterQueryMonitor<T> : CCPQueryMonitorBase<T> where T : class
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character">The character to monitor.</param>
        /// <param name="method">The method to use.</param>
        /// <param name="onSuccess">An action to call on success.</param>
        /// <param name="onFailure">The callback to use upon failure.</param>
        internal CharacterQueryMonitor(CCPCharacter character, Enum method, Action<T>
            onSuccess, NotifyErrorCallback onFailure) : base(character, method, (result) =>
            {
                // Character may have been set to not be monitored
                if (character.Monitored)
                {
                    if (character.ShouldNotifyError(result, method))
                        onFailure.Invoke(character, result);
                    if (!result.HasError && result.HasData)
                        onSuccess.Invoke(result.Result);
                }
                foreach(var monitor in character.QueryMonitors.Where(monitor => monitor.Method.
                        HasParent(method)))
                    character.QueryMonitors.Query(monitor.Method);
            })
        {
        }

        /// <summary>
        /// Retrieves the parameters required for the ESI request.
        /// </summary>
        /// <returns>The ESI request parameters.</returns>
        internal override ESIParams GetESIParams()
        {
            return new ESIParams(LastResult?.Response, m_apiKey.AccessToken)
            {
                ParamOne = m_character.CharacterID
            };
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
                m_apiKey = m_character.Identity.FindAPIKeyWithAccess((ESIAPICharacterMethods)
                    Method);
                return m_apiKey != null;
            }
        }
    }
}
