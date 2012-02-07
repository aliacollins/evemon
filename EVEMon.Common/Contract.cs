using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class Contract
    {
        private readonly List<ContractItem> m_contractItems = new List<ContractItem>();
        private ContractState m_state;

        private bool m_queryPending;

        /// <summary>
        /// The maximum number of days after contract expires. Beyond this limit, we do not import contracts anymore.
        /// </summary>
        public const int MaxEndedDays = 7;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal Contract(CCPCharacter ccpCharacter, SerializableContractListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            Character = ccpCharacter;
            ID = src.ContractID;
            PopulateContractInfo(src);
            m_state = GetState(src);
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal Contract(CCPCharacter ccpCharacter, SerializableContract src)
        {
            Character = ccpCharacter;
            ID = src.ContractID;
            m_state = src.ContractState;
        }

        #endregion


        #region Properties

        public CCPCharacter Character { get; private set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; private set; }

        /// <summary>
        /// Gets or sets the start station.
        /// </summary>
        /// <value>The start station.</value>
        public Station StartStation { get; private set; }

        /// <summary>
        /// Gets or sets the end station.
        /// </summary>
        /// <value>The end station.</value>
        public Station EndStation { get; private set; }

        /// <summary>
        /// Gets or sets the type of the contract.
        /// </summary>
        /// <value>The type of the contract.</value>
        public ContractType ContractType { get; private set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public CCPContractStatus Status { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The title.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [for corp].
        /// </summary>
        /// <value><c>true</c> if [for corp]; otherwise, <c>false</c>.</value>
        public IssuedFor IssuedFor { get; private set; }

        /// <summary>
        /// Gets or sets the availability.
        /// </summary>
        /// <value>The availability.</value>
        public ContractAvailability Availability { get; private set; }

        /// <summary>
        /// Gets or sets the issued date.
        /// </summary>
        /// <value>The issued date.</value>
        public DateTime Issued { get; private set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>The expiration date.</value>
        public DateTime Expiration { get; private set; }

        /// <summary>
        /// Gets or sets the accepted date.
        /// </summary>
        /// <value>The accepted date.</value>
        public DateTime Accepted { get; private set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; private set; }

        /// <summary>
        /// Gets or sets the days to complete a courier contract.
        /// </summary>
        /// <value>The days to complete.</value>
        public int DaysToComplete { get; private set; }

        /// <summary>
        /// Gets or sets the completed date.
        /// </summary>
        /// <value>The completed date.</value>
        public DateTime Completed { get; private set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>The price.</value>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets or sets the reward.
        /// </summary>
        /// <value>The reward.</value>
        public decimal Reward { get; private set; }

        /// <summary>
        /// Gets or sets the collateral.
        /// </summary>
        /// <value>The collateral.</value>
        public decimal Collateral { get; private set; }

        /// <summary>
        /// Gets or sets the buyout.
        /// </summary>
        /// <value>The buyout.</value>
        public decimal Buyout { get; private set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>The volume.</value>
        public double Volume { get; private set; }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; private set; }

        /// <summary>
        /// Gets or sets the assignee.
        /// </summary>
        /// <value>The assignee.</value>
        public string Assignee { get; private set; }

        /// <summary>
        /// Gets or sets the acceptor.
        /// </summary>
        /// <value>The acceptor.</value>
        public string Acceptor { get; private set; }

        /// <summary>
        /// Gets the contract items.
        /// </summary>
        /// <value>The contract items.</value>
        public IEnumerable<ContractItem> ContractItems
        {
            get { return m_contractItems.Where(x => x.Item != null); }
        }

        /// <summary>
        /// Gets the contract text.
        /// </summary>
        /// <value>The contract text.</value>
        /// <remarks>Keep this order of checking.</remarks>
        public string ContractText
        {
            get
            {
                if (ContractType == ContractType.Courier)
                    return String.Format(CultureConstants.DefaultCulture, "{0} >> {1} ({2} m³)",
                                         StartStation.SolarSystem.Name, EndStation.SolarSystem.Name, Volume);

                if (!m_contractItems.Any())
                    return "Unknown";

                if (IsTrading)
                    return "[Want To Trade]";

                if (IsMultipleItems)
                    return "[Multiple Items]";

                if (IsBuyOnly)
                    return Reward == 0 ? "[Want A Gift]" : "[Want To Buy]";

                return String.Format(CultureConstants.DefaultCulture, "{0}{1}", m_contractItems.First().Item.Name,
                                     m_contractItems.First().Quantity > 1
                                         ? String.Format(CultureConstants.DefaultCulture, " x {0}",
                                                         m_contractItems.First().Quantity)
                                         : String.Empty);
            }
        }

        /// <summary>
        /// When true, the contract will be deleted unless it was found on the API feed.
        /// </summary>
        internal bool MarkedForDeletion { get; set; }

        /// <summary>
        /// Checks whether the given API object matches with this contract.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private bool MatchesWith(SerializableContractListItem src)
        {
            return src.ContractID == ID;
        }

        /// <summary>
        /// Gets the contract state.
        /// </summary>
        public ContractState State
        {
            get
            {
                if (IsExpired && (m_state == ContractState.Created || m_state == ContractState.Assigned))
                    return ContractState.Expired;

                return m_state;
            }
        }

        /// <summary>
        /// Gets true if the contract is of "buy only" type.
        /// </summary>
        public bool IsBuyOnly
        {
            get { return m_contractItems.All(x => !x.Included); }
        }

        /// <summary>
        /// Gets true if the contract is of "multiple items" type.
        /// </summary>
        public bool IsMultipleItems
        {
            get { return m_contractItems.Count > 1 && m_contractItems.All(x => x.Included); }
        }

        /// <summary>
        /// Gets true if the contract is of "trading" type.
        /// </summary>
        public bool IsTrading
        {
            get { return m_contractItems.Count > 1 && m_contractItems.Any(x => x.Included) && m_contractItems.Any(x => !x.Included); }
        }

        /// <summary>
        /// Gets true if the contract is not finished, canceled, expired, etc.
        /// </summary>
        public bool IsAvailable
        {
            get { return (m_state == ContractState.Created || m_state == ContractState.Assigned) && !IsExpired; }
        }

        /// <summary>
        /// Gets true if the contract is expired or rejected but not yet deleted (needs attention).
        /// </summary>
        public bool NeedsAttention
        {
            get { return m_state == ContractState.Expired || m_state == ContractState.Rejected; }
        }

        /// <summary>
        /// Gets true if contract naturally expired because of its duration.
        /// </summary>
        public bool IsExpired
        {
            get { return Expiration < DateTime.UtcNow; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a notification for this contract have been send.
        /// </summary>
        /// <value><c>true</c> if a notification for this contract have been send; otherwise, <c>false</c>.</value>
        public bool NotificationSend { get; set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the state of a contract.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private ContractState GetState(SerializableContractListItem src)
        {
            CCPContractStatus status = Enum.IsDefined(typeof(CCPContractStatus), src.Status)
                                           ? (CCPContractStatus)Enum.Parse(typeof(CCPContractStatus), src.Status)
                                           : CCPContractStatus.None;

            if (status == CCPContractStatus.Outstanding && IsExpired)
                return ContractState.Expired;

            switch (status)
            {
                case CCPContractStatus.Canceled:
                case CCPContractStatus.Failed:
                    return ContractState.Canceled;
                case CCPContractStatus.Outstanding:
                case CCPContractStatus.InProgress:
                    return (src.IssuerID != Character.CharacterID) ? ContractState.Assigned : ContractState.Created;
                case CCPContractStatus.Completed:
                case CCPContractStatus.CompletedByContractor:
                case CCPContractStatus.CompletedByIssuer:
                    return ContractState.Finished;
                case CCPContractStatus.Rejected:
                    return ContractState.Rejected;
                case CCPContractStatus.Deleted:
                    return ContractState.Deleted;
                default:
                    return ContractState.Unknown;
            }
        }

        #endregion


        #region Importation, Exportation

        /// <summary>
        /// Try to update this contract with a serialization object from the API.
        /// </summary>
        /// <param name="src">The serializable source.</param>
        /// <param name="endedContracts">The ended contracts.</param>
        /// <returns>True if import sucessful otherwise, false.</returns>
        internal bool TryImport(SerializableContractListItem src, List<Contract> endedContracts)
        {
            // Note that, before a match is found, all contracts have been marked for deletion : m_markedForDeletion == true

            // Checks whether ID is the same
            if (!MatchesWith(src))
                return false;

            // Prevent deletion
            MarkedForDeletion = false;

            // Contract is from a serialized object, so populate the missing info
            if (String.IsNullOrEmpty(Issuer))
                PopulateContractInfo(src);

            // Update state
            ContractState state = GetState(src);
            if (state != m_state || NeedsAttention)
            {
                m_state = state;

                // Should we notify it to the user ?
                if (NeedsAttention || state == ContractState.Finished)
                    endedContracts.Add(this);
            }

            return true;
        }

        /// <summary>
        /// Populates the serialization object contract with the info from the API.
        /// </summary>
        /// <param name="src">The source.</param>
        private void PopulateContractInfo(SerializableContractListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            StartStation = Station.GetByID(src.StartStationID);
            EndStation = Station.GetByID(src.EndStationID);
            Description = String.IsNullOrWhiteSpace(src.Title) ? "(None)" : src.Title;
            IssuedFor = src.ForCorp ? IssuedFor.Corporation : IssuedFor.Character;
            Issued = src.DateIssued;
            Expiration = src.DateExpired;
            Accepted = src.DateAccepted;
            Duration = (int)src.DateExpired.Subtract(src.DateIssued).TotalDays;
            DaysToComplete = src.NumDays;
            Completed = src.DateCompleted;
            Price = src.Price;
            Reward = src.Reward;
            Collateral = src.Collateral;
            Buyout = src.Buyout;
            Volume = Math.Round(src.Volume);

            Availability = Enum.IsDefined(typeof(ContractAvailability), src.Availability)
                               ? (ContractAvailability)Enum.Parse(typeof(ContractAvailability), src.Availability)
                               : ContractAvailability.None;

            ContractType = Enum.IsDefined(typeof(ContractType), src.Type)
                               ? (ContractType)Enum.Parse(typeof(ContractType), src.Type)
                               : ContractType.None;

            Status = Enum.IsDefined(typeof(CCPContractStatus), src.Status)
                         ? (CCPContractStatus)Enum.Parse(typeof(CCPContractStatus), src.Status)
                         : CCPContractStatus.None;

            Issuer = src.ForCorp
                         ? Character.Corporation.Name
                         : src.IssuerID == Character.CharacterID
                               ? Character.Name
                               : EveIDToName.GetIDToName(src.IssuerID);

            Assignee = src.AssigneeID == Character.CharacterID
                           ? Character.Name
                           : src.AssigneeID == Character.CorporationID
                                 ? Character.Corporation.Name
                                 : EveIDToName.GetIDToName(src.AssigneeID);

            Acceptor = src.AcceptorID == Character.CharacterID
                           ? Character.Name
                           : src.AcceptorID == Character.CorporationID
                                 ? Character.Corporation.Name
                                 : EveIDToName.GetIDToName(src.AcceptorID);

            GetContractItems();       
        }

        /// <summary>
        /// Imports the contract items to a list.
        /// </summary>
        /// <param name="src">The source.</param>
        private void Import(IEnumerable<SerializableContractItemsListItem> src)
        {
            foreach (SerializableContractItemsListItem item in src)
            {
                m_contractItems.Add(new ContractItem(item));
            }
        }

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableContract Export()
        {
            return new SerializableContract
                       {
                           ContractID = ID,
                           ContractState = m_state
                       };
        }

        #endregion


        #region Querying Methods

        /// <summary>
        /// Gets the contract items.
        /// </summary>
        private void GetContractItems()
        {
            // Exit if we are already trying to download
            if (m_queryPending)
                return;

            m_queryPending = true;

            // Quits if access denied
            APIKey apiKey = Character.Identity.FindAPIKeyWithAccess(APICharacterMethods.Contracts);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIContractItems>(
                APIGenericMethods.ContractItems,
                apiKey.ID,
                apiKey.VerificationCode,
                Character.CharacterID,
                ID,
                OnContractItemsDownloaded);
        }

        /// <summary>
        /// Called when contract items downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnContractItemsDownloaded(APIResult<SerializableAPIContractItems> result)
        {
            m_queryPending = false;

            // Notify an error occured
            if (Character.ShouldNotifyError(result, APIGenericMethods.ContractItems))
                EveMonClient.Notifications.NotifyContractItemsError(Character, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Quit if for any reason there is no data
            if (!result.Result.ContractItems.Any())
                return;

            // Import the data
            Import(result.Result.ContractItems);

            EveMonClient.OnCharacterContractItemsDownloaded(Character);
        }

        #endregion

    }
}
