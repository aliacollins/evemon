using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public abstract class DataQuerying
    {
        private readonly List<SerializableOrderListItem> m_orders = new List<SerializableOrderListItem>();
        private readonly List<SerializableJobListItem> m_jobs = new List<SerializableJobListItem>();

        private Enum m_errorNotifiedMethod;

        private bool m_charOrdersUpdated;
        private bool m_corpOrdersUpdated;
        private bool m_charOrdersAdded;
        private bool m_corpOrdersAdded;

        private bool m_charJobsUpdated;
        private bool m_corpJobsUpdated;
        private bool m_charJobsAdded;
        private bool m_corpJobsAdded;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataQuerying"/> class.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        protected DataQuerying(CCPCharacter ccpCharacter)
        {
            CCPCharacter = ccpCharacter;
        }

        #endregion


        #region Protected Properties

        /// <summary>
        /// Gets or sets the CCP character.
        /// </summary>
        /// <value>The CCP character.</value>
        protected CCPCharacter CCPCharacter { get; private set; }

        #endregion


        #region Querying Helper Methods

        /// <summary>
        /// Processes the queried character's personal market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        internal void OnCharacterMarketOrdersUpdated(APIResult<SerializableAPIMarketOrders> result)
        {
            m_charOrdersUpdated = true;

            // Notify an error occurred
            if (ShouldNotifyError(result, APICharacterMethods.MarketOrders))
                EveMonClient.Notifications.NotifyCharacterMarketOrdersError(CCPCharacter, result);

            // Add orders to list
            m_charOrdersAdded = AddOrders(result, m_corpOrdersAdded, IssuedFor.Character);

            // If character can not query corporation data, we switch the corp orders updated flag
            // and proceed with the orders importation
            IQueryMonitor corporationMarketOrdersMonitor =
                CCPCharacter.QueryMonitors[APICorporationMethods.CorporationMarketOrders];
            m_corpOrdersUpdated |= corporationMarketOrdersMonitor == null || !corporationMarketOrdersMonitor.Enabled;

            // Import the data if all queried
            if (m_corpOrdersUpdated)
                Import(m_orders);
        }

        /// <summary>
        /// Processes the queried character's corporation market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        internal void OnCorporationMarketOrdersUpdated(APIResult<SerializableAPIMarketOrders> result)
        {
            m_corpOrdersUpdated = true;

            // Notify an error occurred
            if (ShouldNotifyError(result, APICorporationMethods.CorporationMarketOrders))
                EveMonClient.Notifications.NotifyCorporationMarketOrdersError(CCPCharacter, result);

            // Add orders to list
            m_corpOrdersAdded = AddOrders(result, m_charOrdersAdded, IssuedFor.Corporation);

            // Import the data if all queried
            if (m_charOrdersUpdated)
                Import(m_orders);
        }

        /// <summary>
        /// Processes the queried character's personal industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        internal void OnCharacterIndustryJobsUpdated(APIResult<SerializableAPIIndustryJobs> result)
        {
            m_charJobsUpdated = true;

            // Notify an error occurred
            if (ShouldNotifyError(result, APICharacterMethods.IndustryJobs))
                EveMonClient.Notifications.NotifyCharacterIndustryJobsError(CCPCharacter, result);

            // Add jobs to list
            m_charJobsAdded = AddJobs(result, m_corpJobsAdded, IssuedFor.Character);

            // If character can not query corporation data, we switch the corp jobs updated flag
            // and proceed with the jobs importation
            IQueryMonitor corpIndustryJobsMonitor = CCPCharacter.QueryMonitors[APICorporationMethods.CorporationIndustryJobs];
            m_corpJobsUpdated |= corpIndustryJobsMonitor == null || !corpIndustryJobsMonitor.Enabled;

            // Import the data if all queried
            if (m_corpJobsUpdated)
                Import(m_jobs);
        }

        /// <summary>
        /// Processes the queried character's corporation industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        internal void OnCorporationIndustryJobsUpdated(APIResult<SerializableAPIIndustryJobs> result)
        {
            m_corpJobsUpdated = true;

            // Notify an error occurred
            if (ShouldNotifyError(result, APICorporationMethods.CorporationIndustryJobs))
                EveMonClient.Notifications.NotifyCorporationIndustryJobsError(CCPCharacter, result);

            // Add jobs to list
            m_corpJobsAdded = AddJobs(result, m_charJobsAdded, IssuedFor.Corporation);

            // Import the data if all queried
            if (m_charJobsUpdated)
                Import(m_jobs);
        }

        #endregion


        # region Helper Methods

        /// <summary>
        /// Forces an update on the selected query monitor.
        /// </summary>
        /// <param name="queryMonitor">The query monitor.</param>
        public void ForceUpdate(IQueryMonitor queryMonitor)
        {
            IQueryMonitorEx monitor = CCPCharacter.QueryMonitors[queryMonitor.Method] as IQueryMonitorEx;
            if (monitor != null)
                monitor.ForceUpdate(false);
        }

        /// <summary>
        /// Checks whether we should notify an error.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal bool ShouldNotifyError(IAPIResult result, Enum method)
        {
            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return false;

            // We don't want to be notified about corp roles error
            if (result.CCPError != null && !result.CCPError.IsCorpRolesError)
                return false;

            // Notify an error occurred
            if (result.HasError)
            {
                if (!m_errorNotifiedMethod.Equals(APIMethodsExtensions.None))
                    return false;

                m_errorNotifiedMethod = method;
                return true;
            }

            // Removes the previous error notification
            if (m_errorNotifiedMethod == method)
            {
                EveMonClient.Notifications.InvalidateCharacterAPIError(CCPCharacter);
                m_errorNotifiedMethod = APIMethodsExtensions.None;
            }
            return false;
        }

        /// <summary>
        /// Add the queried orders to a list.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ordersAdded"></param>
        /// <param name="issuedFor"></param>
        /// <returns>True if orders get added, false otherwise</returns>
        private bool AddOrders(APIResult<SerializableAPIMarketOrders> result, bool ordersAdded, IssuedFor issuedFor)
        {
            // Add orders if there isn't an error
            if (result.HasError)
                return false;

            // Check to see if other market
            // orders have been added before
            if (!ordersAdded)
                m_orders.Clear();

            // Add orders in list
            result.Result.Orders.ForEach(order => order.IssuedFor = issuedFor);
            m_orders.AddRange(result.Result.Orders);

            return true;
        }

        /// <summary>
        /// Import the orders from both market orders querying.
        /// </summary>
        /// <param name="orders"></param>
        private void Import(IEnumerable<SerializableOrderListItem> orders)
        {
            // Exclude orders that wheren't issued by this character
            // (Delete this line upon implementing a corporation related monitor)
            IEnumerable<SerializableOrderListItem> characterOrders = orders.Where(x => x.OwnerID == CCPCharacter.CharacterID);

            List<MarketOrder> endedOrders = new List<MarketOrder>();
            CCPCharacter.MarketOrders.Import(characterOrders, endedOrders);

            // Notify for ended orders
            NotifyOnEndedOrders(endedOrders);

            // Reset flags
            m_charOrdersUpdated = false;
            m_corpOrdersUpdated = false;
            m_charOrdersAdded = false;
            m_corpOrdersAdded = false;

            // Check the character has sufficient balance
            // for its buying orders and send a notification if not
            if (!CCPCharacter.HasSufficientBalance)
            {
                EveMonClient.Notifications.NotifyInsufficientBalance(CCPCharacter);
                return;
            }

            EveMonClient.Notifications.InvalidateInsufficientBalance(CCPCharacter);
        }

        /// <summary>
        /// Notify the user which orders have ended.
        /// </summary>
        /// <param name="endedOrders"></param>
        private void NotifyOnEndedOrders(IEnumerable<MarketOrder> endedOrders)
        {
            // Sends a notification
            if (endedOrders.Count() != 0)
                EveMonClient.Notifications.NotifyMarkerOrdersEnded(CCPCharacter, endedOrders);
        }

        /// <summary>
        /// Add the queried jobs to a list.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="jobsAdded"></param>
        /// <param name="issuedFor"></param>
        /// <returns>True if jobs get added, false otherwise</returns>
        private bool AddJobs(APIResult<SerializableAPIIndustryJobs> result, bool jobsAdded, IssuedFor issuedFor)
        {
            // Add orders if there isn't an error
            if (result.HasError)
                return false;

            // Check to see if other market
            // orders have been added before
            if (!jobsAdded)
                m_jobs.Clear();

            // Add jobs in list
            result.Result.Jobs.ForEach(x => x.IssuedFor = issuedFor);
            m_jobs.AddRange(result.Result.Jobs);

            return true;
        }

        /// <summary>
        /// Import the jobs from both industry jobs querying.
        /// </summary>
        /// <param name="jobs"></param>
        private void Import(IEnumerable<SerializableJobListItem> jobs)
        {
            // Exclude jobs that wheren't issued by this character
            // (Delete this line upon implementing a corporation related monitor)
            IEnumerable<SerializableJobListItem> characterJobs = jobs.Where(x => x.InstallerID == CCPCharacter.CharacterID);

            CCPCharacter.IndustryJobs.Import(characterJobs);

            // Fires the event regarding industry jobs update
            EveMonClient.OnIndustryJobsUpdated(CCPCharacter);

            // Reset flags
            m_charJobsUpdated = false;
            m_corpJobsUpdated = false;
            m_charJobsAdded = false;
            m_corpJobsAdded = false;
        }

        #endregion

    }
}