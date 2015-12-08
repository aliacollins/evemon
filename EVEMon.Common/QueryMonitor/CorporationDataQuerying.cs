using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.QueryMonitor
{
    internal sealed class CorporationDataQuerying
    {
        #region Fields

        private readonly CorporationQueryMonitor<SerializableAPIMedals> m_corpMedalsMonitor;
        private readonly CorporationQueryMonitor<SerializableAPIMarketOrders> m_corpMarketOrdersMonitor;
        private readonly CorporationQueryMonitor<SerializableAPIContracts> m_corpContractsMonitor;
        private readonly CorporationQueryMonitor<SerializableAPIIndustryJobs> m_corpIndustryJobsMonitor;
        private readonly List<IQueryMonitorEx> m_corporationQueryMonitors;
        private readonly CCPCharacter m_ccpCharacter;

        #endregion


        #region Constructor

        internal CorporationDataQuerying(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
            m_corporationQueryMonitors = new List<IQueryMonitorEx>();

            // Initializes the query monitors 
            m_corpMedalsMonitor =
                new CorporationQueryMonitor<SerializableAPIMedals>(ccpCharacter,
                                                                         CCPAPICorporationMethods.CorporationMedals,
                                                                         OnMedalsUpdated) { QueryOnStartup = true };
            m_corpMarketOrdersMonitor =
                new CorporationQueryMonitor<SerializableAPIMarketOrders>(ccpCharacter,
                                                                         CCPAPICorporationMethods.CorporationMarketOrders,
                                                                         OnMarketOrdersUpdated) { QueryOnStartup = true };
            m_corpContractsMonitor =
                new CorporationQueryMonitor<SerializableAPIContracts>(ccpCharacter,
                                                                      CCPAPICorporationMethods.CorporationContracts,
                                                                      OnContractsUpdated) { QueryOnStartup = true };
            m_corpIndustryJobsMonitor =
                new CorporationQueryMonitor<SerializableAPIIndustryJobs>(ccpCharacter,
                                                                         CCPAPICorporationMethods.CorporationIndustryJobs,
                                                                         OnIndustryJobsUpdated) { QueryOnStartup = true };

            // Add the monitors in an order as they will appear in the throbber menu
            m_corporationQueryMonitors.AddRange(new IQueryMonitorEx[]
                                                    {
                                                       m_corpMedalsMonitor, m_corpMarketOrdersMonitor, m_corpContractsMonitor,
                                                        m_corpIndustryJobsMonitor
                                                    });
            m_corporationQueryMonitors.ForEach(monitor => ccpCharacter.QueryMonitors.Add(monitor));
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
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(CCPAPICorporationMethods.CorporationContracts);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIContractBids>(
                CCPAPIGenericMethods.CorporationContractBids,
                apiKey.ID,
                apiKey.VerificationCode,
                m_ccpCharacter.CharacterID,
                OnCorporationContractBidsUpdated);
        }

        /// <summary>
        /// Processes the queried character's corporation medals.
        /// </summary>
        /// <param name="result"></param>
        private void OnMedalsUpdated(CCPAPIResult<SerializableAPIMedals> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, CCPAPICorporationMethods.CorporationMedals))
                EveMonClient.Notifications.NotifyCorporationMedalsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.CorporationMedals.Import(result.Result.CorporationMedals);

            // Fires the event regarding corporation medals update
            EveMonClient.OnCorporationMedalsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's corporation market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnMarketOrdersUpdated(CCPAPIResult<SerializableAPIMarketOrders> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, CCPAPICorporationMethods.CorporationMarketOrders))
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
        private void OnContractsUpdated(CCPAPIResult<SerializableAPIContracts> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, CCPAPICorporationMethods.CorporationContracts))
                EveMonClient.Notifications.NotifyCorporationContractsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the contract bids
            QueryCorporationContractBids();

            result.Result.Contracts.ToList().ForEach(x =>
            {
                x.IssuedFor = IssuedFor.Corporation;
                x.APIMethod = CCPAPICorporationMethods.CorporationContracts;
            });

            // Import the data
            List<Contract> endedContracts = new List<Contract>();
            m_ccpCharacter.CorporationContracts.Import(result.Result.Contracts, endedContracts);

            // Fires the event regarding corporation contracts update
            EveMonClient.OnCorporationContractsUpdated(m_ccpCharacter, endedContracts);
        }

        /// <summary>
        /// Processes the queried character's corporation contract bids.
        /// </summary>
        private void OnCorporationContractBidsUpdated(CCPAPIResult<SerializableAPIContractBids> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, CCPAPIGenericMethods.CorporationContractBids))
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
        private void OnIndustryJobsUpdated(CCPAPIResult<SerializableAPIIndustryJobs> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, CCPAPICorporationMethods.CorporationIndustryJobs))
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