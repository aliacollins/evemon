using System;
using System.ComponentModel;
using EVEMon.Common.Attributes;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumeration of the corporation related API methods. Each method has an access mask.
    /// Each method should have an entry in APIMethodsEnum and
    /// an equivalent string entry in NetworkConstants indicating the default path of the method.
    /// </summary>
    [Flags]
    public enum CCPAPICorporationMethods : ulong
    {
        None = 0,

        /// <summary>
        /// The corporation issued market orders.
        /// </summary>
        [Header("Market Orders")]
        [Description("The corporation market orders of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        CorporationMarketOrders = 1 << 12,

        /// <summary>
        /// The corporation issued contracts.
        /// </summary>
        [Header("Contracts")]
        [Description("The corporation contracts of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Long)]
        CorporationContracts = 1 << 23,

        /// <summary>
        /// The corporation issued industry jobs.
        /// </summary>
        [Header("Industry Jobs")]
        [Description("The corporation industry jobs of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Long)]
        CorporationIndustryJobs = 1 << 7,

        /// <summary>
        /// A corporation's wallet balances.
        /// </summary>
        CorporationAccountBalance = 1 << 0,

        /// <summary>
        /// Asset list of a corporation.
        /// </summary>
        CorporationAssetList = 1 << 1,

        /// <summary>
        /// A list of corporation contacts.
        /// </summary>
        CorporationContactList = 1 << 4,

        /// <summary>
        /// The log of the corporation's containers.
        /// </summary>
        CorporationContainerLog = 1 << 5,

        /// <summary>
        /// A corporation sheet.
        /// </summary>
        CorporationSheet = 1 << 3,

        /// <summary>
        /// Factional warfare statistics for a corporation.
        /// </summary>
        CorporationFactionalWarfareStats = 1 << 6,

        /// <summary>
        /// The Kill log for a corporation (Kill mails).
        /// </summary>
        CorporationKillLog = 1 << 8,

        /// <summary>
        /// List of all medals created by the corporation.
        /// </summary>
        [Header("Medals")]
        [Description("The corporation medals of a character.")]
        [Update(UpdatePeriod.Hours6, UpdatePeriod.Hours6, CacheStyle.Short)]
        CorporationMedals = 1 << 13,

        /// <summary>
        /// List of medals awarded to corporation members.
        /// </summary>
        CorporationMemberMedals = 1 << 2,

        /// <summary>
        /// Member roles and titles.
        /// </summary>
        CorporationMemberSecurity = 1 << 9,

        /// <summary>
        /// Member role and title change log.
        /// </summary>
        CorporationMemberSecurityLog = 1 << 10,

        /// <summary>
        /// Limited Member information.
        /// </summary>
        CorporationMemberTrackingLimited = 1 << 11,

        /// <summary>
        /// Extensive Member information. Time of last logoff, last known location and ship.
        /// </summary>
        CorporationMemberTrackingExtended = 1 << 25,

        /// <summary>
        /// List of all outposts controlled by the corporation.
        /// </summary>
        CorporationOutpostList = 1 << 14,

        /// <summary>
        /// List of all service settings of corporate outposts.
        /// </summary>
        CorporationOutpostServiceDetail = 1 << 15,

        /// <summary>
        /// Shareholders of the corporation.
        /// </summary>
        CorporationShareholders = 1 << 16,

        /// <summary>
        /// NPC Standings towards corporation.
        /// </summary>
        CorporationStandings = 1 << 18,

        /// <summary>
        /// List of all settings of corporate starbases.
        /// </summary>
        CorporationStarbaseDetails = 1 << 17,

        /// <summary>
        /// List of all corporate starbases.
        /// </summary>
        CorporationStarbaseList = 1 << 19,

        /// <summary>
        /// Titles of corporation and the roles they grant.
        /// </summary>
        CorporationTitles = 1 << 22,

        /// <summary>
        /// Wallet journal for all corporate accounts.
        /// </summary>
        CorporationWalletJournal = 1 << 20,

        /// <summary>
        /// Market transactions of all corporate accounts.
        /// </summary>
        CorporationWalletTransactions = 1 << 21,

        /// <summary>
        /// Allows the fetching of coordinate and name data for items owned by the corporation.
        /// </summary>
        CorporationLocations = 1 << 24,

        /// <summary>
        /// List of all corporate bookmarks.
        /// </summary>
        CorporationBookmarks = 1 << 26,

        /// <summary>
        /// List of corporate contract bids.
        /// </summary>
        CorporationContractBids = 1 << 27,

        /// <summary>
        /// List of corporate contract items.
        /// </summary>
        CorporationContractItems = 1 << 28
    }
}
