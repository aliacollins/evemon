using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public abstract class DataQuerying
    {
        #region Fields

        private readonly List<SerializableOrderListItem> m_orders = new List<SerializableOrderListItem>();
        private readonly List<SerializableJobListItem> m_jobs = new List<SerializableJobListItem>();

        private Enum m_errorNotifiedMethod;

        #endregion


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
        protected bool AddOrders(APIResult<SerializableAPIMarketOrders> result, bool ordersAdded, IssuedFor issuedFor)
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
        protected void ImportOrders()
        {
            // Exclude orders that wheren't issued by this character
            // (Delete this line upon implementing a corporation related monitor)
            IEnumerable<SerializableOrderListItem> characterOrders = m_orders.Where(x => x.OwnerID == CCPCharacter.CharacterID);

            List<MarketOrder> endedOrders = new List<MarketOrder>();
            CCPCharacter.MarketOrders.Import(characterOrders, endedOrders);

            // Notify for ended orders
            if (endedOrders.Count() != 0)
                EveMonClient.Notifications.NotifyMarkerOrdersEnded(CCPCharacter, endedOrders);

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
        /// Add the queried jobs to a list.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="jobsAdded"></param>
        /// <param name="issuedFor"></param>
        /// <returns>True if jobs get added, false otherwise</returns>
        protected bool AddJobs(APIResult<SerializableAPIIndustryJobs> result, bool jobsAdded, IssuedFor issuedFor)
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
        protected void ImportJobs()
        {
            // Exclude jobs that wheren't issued by this character
            // (Delete this line upon implementing a corporation related monitor)
            IEnumerable<SerializableJobListItem> characterJobs = m_jobs.Where(x => x.InstallerID == CCPCharacter.CharacterID);

            CCPCharacter.IndustryJobs.Import(characterJobs);

            // Fires the event regarding industry jobs update
            EveMonClient.OnIndustryJobsUpdated(CCPCharacter);
        }

        #endregion

    }
}