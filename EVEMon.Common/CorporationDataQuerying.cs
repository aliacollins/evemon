using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class CorporationDataQuerying
    {
        #region Fields

        private readonly CorporationQueryMonitor<SerializableAPIMarketOrders> m_corpMarketOrdersMonitor;
        private readonly CorporationQueryMonitor<SerializableAPIIndustryJobs> m_corpIndustryJobsMonitor;
        private readonly List<IQueryMonitorEx> m_corporationQueryMonitors;
        private readonly CCPCharacter m_ccpCharacter;

        private bool m_corpMarketOrdersQueried;
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


        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [corporation market orders queried].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [corporation market orders queried]; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationMarketOrdersQueried
        {
            get
            {
                // If character can not query corporation related data
                // or corporation market orders monitor is not enabled
                // we switch the flag
                IQueryMonitor corpMarketOrdersMonitor =
                    m_ccpCharacter.QueryMonitors[APICorporationMethods.CorporationMarketOrders];
                return m_corpMarketOrdersQueried |= corpMarketOrdersMonitor == null || !corpMarketOrdersMonitor.Enabled;
            }
            set { m_corpMarketOrdersQueried = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [corporation industry jobs queried].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [corporation industry jobs queried]; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationIndustryJobsQueried
        {
            get
            {
                // If character can not query corporation related data
                // or corporation industry jobs monitor is not enabled
                // we switch the flag
                IQueryMonitor corpIndustryJobsMonitor =
                    m_ccpCharacter.QueryMonitors[APICorporationMethods.CorporationIndustryJobs];
                return m_corpIndustryJobsQueried |= corpIndustryJobsMonitor == null || !corpIndustryJobsMonitor.Enabled;
            }
            set { m_corpIndustryJobsQueried = value; }
        }

        #endregion


        #region Querying


        /// <summary>
        /// Processes the queried character's corporation market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnCorporationMarketOrdersUpdated(APIResult<SerializableAPIMarketOrders> result)
        {
            CorporationMarketOrdersQueried = true;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICorporationMethods.CorporationMarketOrders))
                EveMonClient.Notifications.NotifyCorporationMarketOrdersError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            result.Result.Orders.ForEach(x => x.IssuedFor = IssuedFor.Corporation);

            // Import the data
            List<MarketOrder> endedOrders = new List<MarketOrder>();
            m_ccpCharacter.CharacterMarketOrders.Import(result.Result.Orders, endedOrders);

            // Fires the event regarding corporation market orders update
            EveMonClient.OnCorporationMarketOrdersUpdated(m_ccpCharacter, endedOrders);
        }


        /// <summary>
        /// Processes the queried character's corporation industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnCorporationIndustryJobsUpdated(APIResult<SerializableAPIIndustryJobs> result)
        {
            CorporationIndustryJobsQueried = true;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICorporationMethods.CorporationIndustryJobs))
                EveMonClient.Notifications.NotifyCorporationIndustryJobsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            result.Result.Jobs.ForEach(x => x.IssuedFor = IssuedFor.Corporation);

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