using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class CorporationDataQuerying : DataQuerying
    {
        private readonly CorporationQueryMonitor<SerializableAPIMarketOrders> m_corpMarketOrdersMonitor;
        private readonly CorporationQueryMonitor<SerializableAPIIndustryJobs> m_corpIndustryJobsMonitor;

        private readonly List<IQueryMonitorEx> m_corporationQueryMonitors;

        public CorporationDataQuerying(CCPCharacter ccpCharacter)
            : base(ccpCharacter)
        {
            m_corporationQueryMonitors = new List<IQueryMonitorEx>();

            // Initializes the query monitors 
            m_corpMarketOrdersMonitor =
                new CorporationQueryMonitor<SerializableAPIMarketOrders>(ccpCharacter,
                                                                         APICorporationMethods.CorporationMarketOrders);
            m_corpMarketOrdersMonitor.Updated += OnCorporationMarketOrdersUpdated;
            m_corporationQueryMonitors.Add(m_corpMarketOrdersMonitor);

            m_corpIndustryJobsMonitor =
                new CorporationQueryMonitor<SerializableAPIIndustryJobs>(ccpCharacter,
                                                                         APICorporationMethods.CorporationIndustryJobs);
            m_corpIndustryJobsMonitor.Updated += OnCorporationIndustryJobsUpdated;
            m_corporationQueryMonitors.Add(m_corpIndustryJobsMonitor);

            m_corporationQueryMonitors.ForEach(monitor => ccpCharacter.QueryMonitors.Add(monitor));

            EveMonClient.CharacterListUpdated += EveMonClient_CharacterListUpdated;
        }


        #region Event Handlers

        /// <summary>
        /// Handles the CharacterListUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.APIKeyInfoChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterListUpdated(object sender, APIKeyInfoChangedEventArgs e)
        {
            if (!CCPCharacter.Identity.APIKeys.Contains(e.APIKey))
                return;

            if ((e.APIKey.Type == APIKeyType.Account || e.APIKey.Type == APIKeyType.Character) &&
                CCPCharacter.Identity.APIKeys.All(
                    apiKey => apiKey.Type == APIKeyType.Account || apiKey.Type == APIKeyType.Character) &&
                m_corporationQueryMonitors.Exists(monitor => CCPCharacter.QueryMonitors.Contains(monitor)))
            {
                m_corporationQueryMonitors.ForEach(monitor => CCPCharacter.QueryMonitors.Remove(monitor));
                return;
            }

            if (e.APIKey.Type != APIKeyType.Corporation ||
                m_corporationQueryMonitors.Exists(monitor => CCPCharacter.QueryMonitors.Contains(monitor)))
                return;

            m_corporationQueryMonitors.ForEach(monitor => CCPCharacter.QueryMonitors.Add(monitor));
        }

        #endregion
    }
}