using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class CorporationDataQuerying
    {
        #region Fields

        private readonly CorporationQueryMonitor<SerializableAPIMarketOrders> m_corpMarketOrdersMonitor;
        private readonly CorporationQueryMonitor<SerializableAPIContracts> m_corpContractsMonitor;
        private readonly CorporationQueryMonitor<SerializableAPIIndustryJobs> m_corpIndustryJobsMonitor;
        private readonly List<IQueryMonitorEx> m_corporationQueryMonitors;
        private readonly CCPCharacter m_ccpCharacter;

        #endregion


        #region Constructor

        public CorporationDataQuerying(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
            m_corporationQueryMonitors = new List<IQueryMonitorEx>();

            // Initializes the query monitors 
            m_corpMarketOrdersMonitor =
                new CorporationQueryMonitor<SerializableAPIMarketOrders>(ccpCharacter,
                                                                         APICorporationMethods.CorporationMarketOrders,
                                                                         OnCorporationMarketOrdersUpdated) { QueryOnStartup = true };
            m_corporationQueryMonitors.Add(m_corpMarketOrdersMonitor);

            m_corpContractsMonitor =
                new CorporationQueryMonitor<SerializableAPIContracts>(ccpCharacter,
                                                                      APICorporationMethods.CorporationContracts,
                                                                      OnCorporationContractsUpdated) { QueryOnStartup = true };
            m_corporationQueryMonitors.Add(m_corpContractsMonitor);

            m_corpIndustryJobsMonitor =
                new CorporationQueryMonitor<SerializableAPIIndustryJobs>(ccpCharacter,
                                                                         APICorporationMethods.CorporationIndustryJobs,
                                                                         OnCorporationIndustryJobsUpdated);
            m_corporationQueryMonitors.Add(m_corpIndustryJobsMonitor);

            m_corporationQueryMonitors.ForEach(monitor => ccpCharacter.QueryMonitors.Add(monitor));
        }

        #endregion


        #region Dispose

        /// <summary>
        /// Called when the object gets disposed.
        /// </summary>
        internal void Dispose()
        {
            // Unsubscribe events in monitors
            foreach (IQueryMonitorEx monitor in m_corporationQueryMonitors)
            {
                monitor.Dispose();
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the corporation market orders have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation market orders have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationMarketOrdersQueried
        {
            get { return !m_corpMarketOrdersMonitor.IsUpdating; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corporation contracts have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation contracts have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationContractsQueried
        {
            get { return !m_corpContractsMonitor.IsUpdating; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corporation industry jobs have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation industry jobs have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationIndustryJobsQueried
        {
            get { return !m_corpIndustryJobsMonitor.IsUpdating; }
        }

        #endregion


        #region Querying

        /// <summary>
        /// Queries the character's contract bids.
        /// </summary>
        private void QueryCorporationContractBids()
        {
            // Quits if no network
            if (!NetworkMonitor.IsNetworkAvailable)
                return;

            // Quits if access denied
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICorporationMethods.CorporationContracts);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIContractBids>(
                APIGenericMethods.CorporationContractBids,
                apiKey.ID,
                apiKey.VerificationCode,
                m_ccpCharacter.CharacterID,
                OnCorporationContractBidsUpdated);
        }

        /// <summary>
        /// Processes the queried character's corporation market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnCorporationMarketOrdersUpdated(APIResult<SerializableAPIMarketOrders> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !EveMonClient.MonitoredCharacters.Contains(m_ccpCharacter))
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICorporationMethods.CorporationMarketOrders))
                EveMonClient.Notifications.NotifyCorporationMarketOrdersError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            result.Result.Orders.ToList().ForEach(x => x.IssuedFor = IssuedFor.Corporation);

            // Import the data
            List<MarketOrder> endedOrders = new List<MarketOrder>();
            m_ccpCharacter.CorporationMarketOrders.Import(result.Result.Orders, endedOrders);

            // Fires the event regarding corporation market orders update
            EveMonClient.OnCorporationMarketOrdersUpdated(m_ccpCharacter, endedOrders);
        }

        /// <summary>
        /// Processes the queried character's corporation contracts.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which contracts gets queried first</remarks>
        private void OnCorporationContractsUpdated(APIResult<SerializableAPIContracts> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !EveMonClient.MonitoredCharacters.Contains(m_ccpCharacter))
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICorporationMethods.CorporationContracts))
                EveMonClient.Notifications.NotifyCorporationContractsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the contract bids
            QueryCorporationContractBids();

            result.Result.Contracts.ToList().ForEach(x => x.IssuedFor = IssuedFor.Corporation);
            result.Result.Contracts.ToList().ForEach(x => x.APIMethod = APICorporationMethods.CorporationContracts);

            // Import the data
            List<Contract> endedContracts = new List<Contract>();
            m_ccpCharacter.CorporationContracts.Import(result.Result.Contracts, endedContracts);

            // Fires the event regarding corporation contracts update
            EveMonClient.OnCorporationContractsUpdated(m_ccpCharacter, endedContracts);
        }

        /// <summary>
        /// Processes the queried character's corporation contract bids.
        /// </summary>
        private void OnCorporationContractBidsUpdated(APIResult<SerializableAPIContractBids> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !EveMonClient.MonitoredCharacters.Contains(m_ccpCharacter))
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APIGenericMethods.CorporationContractBids))
                EveMonClient.Notifications.NotifyCorporationContractBidsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.CorporationContractBids.Import(result.Result.ContractBids);

            // Fires the event regarding corporation contract bids update
            EveMonClient.OnCorporationContractBidsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's corporation industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnCorporationIndustryJobsUpdated(APIResult<SerializableAPIIndustryJobs> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !EveMonClient.MonitoredCharacters.Contains(m_ccpCharacter))
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICorporationMethods.CorporationIndustryJobs))
                EveMonClient.Notifications.NotifyCorporationIndustryJobsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            result.Result.Jobs.ToList().ForEach(x => x.IssuedFor = IssuedFor.Corporation);

            // Import the data
            m_ccpCharacter.CorporationIndustryJobs.Import(result.Result.Jobs);

            // Fires the event regarding corporation industry jobs update
            EveMonClient.OnCorporationIndustryJobsUpdated(m_ccpCharacter);
        }

        #endregion
    }
}