using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.QueryMonitor
{
    internal sealed class CorporationDataQuerying
    {
        #region Fields

        private readonly CorporationQueryMonitor<EsiAPIMedals> m_corpMedalsMonitor;
        private readonly CorporationQueryMonitor<EsiAPIMarketOrders> m_corpMarketOrdersMonitor;
        private readonly CorporationQueryMonitor<EsiAPIContracts> m_corpContractsMonitor;
        private readonly CorporationQueryMonitor<EsiAPIIndustryJobs> m_corpIndustryJobsMonitor;
        private readonly List<IQueryMonitorEx> m_corporationQueryMonitors;
        private readonly CCPCharacter m_ccpCharacter;

        #endregion


        #region Constructor

        internal CorporationDataQuerying(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
            m_corporationQueryMonitors = new List<IQueryMonitorEx>();

            // Initializes the query monitors 
            m_corpMedalsMonitor = new CorporationQueryMonitor<EsiAPIMedals>(ccpCharacter,
                ESIAPICorporationMethods.CorporationMedals, OnMedalsUpdated) { QueryOnStartup = true };
            m_corpMarketOrdersMonitor = new CorporationQueryMonitor<EsiAPIMarketOrders>(ccpCharacter,
                ESIAPICorporationMethods.CorporationMarketOrders, OnMarketOrdersUpdated) { QueryOnStartup = true };
            m_corpContractsMonitor = new CorporationQueryMonitor<EsiAPIContracts>(ccpCharacter,
                ESIAPICorporationMethods.CorporationContracts, OnContractsUpdated) { QueryOnStartup = true };
            m_corpIndustryJobsMonitor = new CorporationQueryMonitor<EsiAPIIndustryJobs>(ccpCharacter,
                ESIAPICorporationMethods.CorporationIndustryJobs, OnIndustryJobsUpdated) { QueryOnStartup = true };

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
        internal bool CorporationMarketOrdersQueried => !m_corpMarketOrdersMonitor.IsUpdating;

        /// <summary>
        /// Gets or sets a value indicating whether the corporation contracts have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation contracts have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationContractsQueried => !m_corpContractsMonitor.IsUpdating;

        /// <summary>
        /// Gets or sets a value indicating whether the corporation industry jobs have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation industry jobs have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationIndustryJobsQueried => !m_corpIndustryJobsMonitor.IsUpdating;

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
            ESIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(ESIAPICorporationMethods.CorporationContracts);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<EsiAPIContractBids>(
                ESIAPICorporationMethods.CorporationContractBids, apiKey.AccessToken,
                m_ccpCharacter.CharacterID, OnCorporationContractBidsUpdated);
        }

        /// <summary>
        /// Processes the queried character's corporation medals.
        /// </summary>
        /// <param name="result"></param>
        private void OnMedalsUpdated(EsiResult<EsiAPIMedals> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICorporationMethods.CorporationMedals))
                EveMonClient.Notifications.NotifyCorporationMedalsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.CorporationMedals.Import(result.Result.ToXMLItem().CorporationMedals);

            // Fires the event regarding corporation medals update
            EveMonClient.OnCorporationMedalsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's corporation market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnMarketOrdersUpdated(EsiResult<EsiAPIMarketOrders> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICorporationMethods.CorporationMarketOrders))
                EveMonClient.Notifications.NotifyCorporationMarketOrdersError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            var orders = result.Result.ToXMLItem(m_ccpCharacter.CorporationID).Orders;
            foreach (var order in orders)
                order.IssuedFor = IssuedFor.Corporation;

            // Import the data
            List<MarketOrder> endedOrders = new List<MarketOrder>();
            m_ccpCharacter.CorporationMarketOrders.Import(orders, endedOrders);

            // Fires the event regarding corporation market orders update
            EveMonClient.OnCorporationMarketOrdersUpdated(m_ccpCharacter, endedOrders);
        }

        /// <summary>
        /// Processes the queried character's corporation contracts.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which contracts gets queried first</remarks>
        private void OnContractsUpdated(EsiResult<EsiAPIContracts> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICorporationMethods.CorporationContracts))
                EveMonClient.Notifications.NotifyCorporationContractsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the contract bids
            QueryCorporationContractBids();

            var contracts = result.Result.ToXMLItem().Contracts;
            foreach (var contract in contracts)
            {
                contract.APIMethod = ESIAPICorporationMethods.CorporationContracts;
                contract.IssuedFor = IssuedFor.Corporation;
            }

            // Import the data
            List<Contract> endedContracts = new List<Contract>();
            m_ccpCharacter.CorporationContracts.Import(contracts, endedContracts);

            // Fires the event regarding corporation contracts update
            EveMonClient.OnCorporationContractsUpdated(m_ccpCharacter, endedContracts);
        }

        /// <summary>
        /// Processes the queried character's corporation contract bids.
        /// </summary>
        private void OnCorporationContractBidsUpdated(EsiResult<EsiAPIContractBids> result, object forContract)
        {
            long contractID = (forContract as long?) ?? 0L;

            // Character may have been deleted or set to not be monitored since we queried
            if (contractID == 0L || m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICorporationMethods.CorporationContractBids))
                EveMonClient.Notifications.NotifyCorporationContractBidsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.CorporationContractBids.Import(result.Result.ToXMLItem(contractID).ContractBids);

            // Fires the event regarding corporation contract bids update
            EveMonClient.OnCorporationContractBidsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's corporation industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnIndustryJobsUpdated(EsiResult<EsiAPIIndustryJobs> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICorporationMethods.CorporationIndustryJobs))
                EveMonClient.Notifications.NotifyCorporationIndustryJobsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            var jobs = result.Result.ToXMLItem().Jobs;
            foreach (var job in jobs)
                job.IssuedFor = IssuedFor.Corporation;

            // Import the data
            m_ccpCharacter.CorporationIndustryJobs.Import(jobs);

            // Fires the event regarding corporation industry jobs update
            EveMonClient.OnCorporationIndustryJobsUpdated(m_ccpCharacter);
        }

        #endregion
    }
}
