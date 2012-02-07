using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.CustomEventArgs;
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

        private bool m_corpMarketOrdersQueried;
        private bool m_corpContractsQueried;
        private bool m_corpIndustryJobsQueried;

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
                                                                         OnCorporationMarketOrdersUpdated);
            m_corporationQueryMonitors.Add(m_corpMarketOrdersMonitor);

            m_corpContractsMonitor =
                new CorporationQueryMonitor<SerializableAPIContracts>(ccpCharacter,
                                                                         APICorporationMethods.CorporationContracts,
                                                                         OnCorporationContractsUpdated) { QueryOnStartup = true };
            m_corporationQueryMonitors.Add(m_corpContractsMonitor);

            m_corpIndustryJobsMonitor =
                new CorporationQueryMonitor<SerializableAPIIndustryJobs>(ccpCharacter,
                                                                         APICorporationMethods.CorporationIndustryJobs, OnCorporationIndustryJobsUpdated);
            m_corporationQueryMonitors.Add(m_corpIndustryJobsMonitor);

            m_corporationQueryMonitors.ForEach(monitor => ccpCharacter.QueryMonitors.Add(monitor));

            EveMonClient.CharacterListUpdated += EveMonClient_CharacterListUpdated;
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
            get
            {
                // If character can not query corporation related data
                // or corporation market orders monitor is not enabled
                // we switch the flag
                IQueryMonitor corpMarketOrdersMonitor =
                    m_ccpCharacter.QueryMonitors[APICorporationMethods.CorporationMarketOrders.ToString()];
                return m_corpMarketOrdersQueried |= corpMarketOrdersMonitor == null || !corpMarketOrdersMonitor.Enabled;
            }
            set { m_corpMarketOrdersQueried = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corporation contracts have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation contracts have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationContractsQueried
        {
            get
            {
                // If character can not query corporation related data
                // or corporation contracts monitor is not enabled
                // we switch the flag
                IQueryMonitor corpContractsMonitor =
                    m_ccpCharacter.QueryMonitors[APICorporationMethods.CorporationContracts.ToString()];
                return m_corpContractsQueried |= corpContractsMonitor == null || !corpContractsMonitor.Enabled;
            }
            set { m_corpContractsQueried = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corporation industry jobs have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation industry jobs have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationIndustryJobsQueried
        {
            get
            {
                // If character can not query corporation related data
                // or corporation industry jobs monitor is not enabled
                // we switch the flag
                IQueryMonitor corpIndustryJobsMonitor =
                    m_ccpCharacter.QueryMonitors[APICorporationMethods.CorporationIndustryJobs.ToString()];
                return m_corpIndustryJobsQueried |= corpIndustryJobsMonitor == null || !corpIndustryJobsMonitor.Enabled;
            }
            set { m_corpIndustryJobsQueried = value; }
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
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.Contracts);
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
            if (m_ccpCharacter == null)
                return;

            CorporationMarketOrdersQueried = true;

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
            if (m_ccpCharacter == null)
                return;

            CorporationContractsQueried = true;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICorporationMethods.CorporationContracts))
                EveMonClient.Notifications.NotifyCorporationContractsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the contracts bids
            QueryCorporationContractBids();

            result.Result.Contracts.ToList().ForEach(x => x.IssuedFor = IssuedFor.Corporation);

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
            if (m_ccpCharacter == null)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APIGenericMethods.CorporationContractBids))
                EveMonClient.Notifications.NotifyCorporationContractBidsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.CorporationContractBids.Import(result.Result.ContractBids);
        }

        /// <summary>
        /// Processes the queried character's corporation industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnCorporationIndustryJobsUpdated(APIResult<SerializableAPIIndustryJobs> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            CorporationIndustryJobsQueried = true;

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


        #region Event Handlers

        /// <summary>
        /// Handles the CharacterListUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.APIKeyInfoChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterListUpdated(object sender, APIKeyInfoChangedEventArgs e)
        {
            if (!m_ccpCharacter.Identity.APIKeys.Contains(e.APIKey))
                return;

            if ((e.APIKey.Type == APIKeyType.Account || e.APIKey.Type == APIKeyType.Character) &&
                m_ccpCharacter.Identity.APIKeys.All(
                    apiKey => apiKey.Type == APIKeyType.Account || apiKey.Type == APIKeyType.Character) &&
                m_corporationQueryMonitors.Exists(monitor => m_ccpCharacter.QueryMonitors.Contains(monitor)))
            {
                m_corporationQueryMonitors.ForEach(monitor => m_ccpCharacter.QueryMonitors.Remove(monitor));
                return;
            }

            if (e.APIKey.Type != APIKeyType.Corporation ||
                m_corporationQueryMonitors.Exists(monitor => m_ccpCharacter.QueryMonitors.Contains(monitor)))
                return;

            m_corporationQueryMonitors.ForEach(monitor => m_ccpCharacter.QueryMonitors.Add(monitor));
        }

        #endregion
    }
}