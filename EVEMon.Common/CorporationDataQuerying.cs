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
        internal bool CorporationMarketOrdersQueried { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [corporation industryjobs queried].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [corporation industryjobs queried]; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationIndustryJobsQueried { get; set; }

        /// <summary>
        /// Gets or sets the ended orders.
        /// </summary>
        /// <value>The ended orders.</value>
        internal List<MarketOrder> EndedOrders { get; private set; }

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
            EndedOrders = new List<MarketOrder>();

            // Exclude orders that wheren't issued by this character
            // (Delete this line upon implementing an exclusive corporation related monitor)
            IEnumerable<SerializableOrderListItem> characterOrders = result.Result.Orders.Where(
                x => x.OwnerID == m_ccpCharacter.CharacterID);

            // Import the data
            m_ccpCharacter.CharacterMarketOrders.Import(characterOrders, EndedOrders);

            // Fires the event regarding corporation market orders update
            EveMonClient.OnCorporationMarketOrdersUpdated(m_ccpCharacter);
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

            // Exclude jobs that wheren't issued by this character
            // (Delete this line upon implementing an exclusive corporation related monitor)
            IEnumerable<SerializableJobListItem> characterJobs = result.Result.Jobs.Where(
                x => x.InstallerID == m_ccpCharacter.CharacterID);

            // Import the data
            m_ccpCharacter.CorporationIndustryJobs.Import(characterJobs);

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