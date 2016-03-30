using System;
using System.ComponentModel;
using EVEMon.Common.Attributes;
using EVEMon.Common.Enumerations.UISettings;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumeration of the character related API methods. Each method has an access mask.
    /// Each method should have an entry in APIMethodsEnum and an equivalent string entry 
    /// in NetworkConstants indicating the default path of the method.
    /// </summary>
    [Flags]
    public enum CCPAPICharacterMethods
    {
        None = 0,

        /// <summary>
        /// The account status. Used to retreive account create and expiration date.
        /// </summary>
        [Header("Account Status")]
        [Description("The status of an account.")]
        [Update(UpdatePeriod.Day, UpdatePeriod.Hours1, CacheStyle.Short)]
        [ForcedOnStartup]
        AccountStatus = 1 << 25,

        /// <summary>
        /// A character sheet (bio, skills, implants, etc).
        /// </summary>
        [Header("Character Sheet")]
        [Description("A character's sheet listing biography, skills, attributes and implants informations.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        CharacterSheet = 1 << 3,

        /// <summary>
        /// The skill queue of a character.
        /// </summary>
        [Header("Skill Queue")]
        [Description("The skill queue of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        SkillQueue = 1 << 18,

        /// <summary>
        /// A character's standings towards NPC's.
        /// </summary>
        [Header("NPC Standings")]
        [Description("The NPC standings of a character.")]
        [Update(UpdatePeriod.Hours3, UpdatePeriod.Hours3, CacheStyle.Short)]
        Standings = 1 << 19,

        /// <summary>
        /// The factional warfare stats of a character.
        /// </summary>
        [Header("Factional Warfare Stats")]
        [Description("The factional warfare stats of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        FactionalWarfareStats = 1 << 6,

        /// <summary>
        /// The assets of a character.
        /// </summary>
        [Header("Assets")]
        [Description("The assets of a character.")]
        [Update(UpdatePeriod.Hours6, UpdatePeriod.Hours6, CacheStyle.Long)]
        AssetList = 1 << 1,

        /// <summary>
        /// The personal issued market orders of a character.
        /// </summary>
        [Header("Market Orders")]
        [Description("The market orders of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        MarketOrders = 1 << 12,

        /// <summary>
        /// The personal issued contracts of a character.
        /// </summary>
        [Header("Contracts")]
        [Description("The contracts of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        Contracts = 1 << 26,

        /// <summary>
        /// The personal issued industry jobs of a character.
        /// </summary>
        [Header("Industry Jobs")]
        [Description("The industry jobs of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        IndustryJobs = 1 << 7,

        /// <summary>
        /// The research points of a character.
        /// </summary>
        [Header("Research Points")]
        [Description("The research points of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        ResearchPoints = 1 << 16,

        /// <summary>
        /// Mail messages for a character.
        /// </summary>
        [Header("EVE Mail Messages")]
        [Description("The EVE mails of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        MailMessages = 1 << 11,

        /// <summary>
        /// The EVE notifications of a character.
        /// </summary>
        [Header("EVE Notifications")]
        [Description("The EVE notifications of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        Notifications = 1 << 14,

        /// <summary>
        /// The wallet journal of a character.
        /// </summary>
        [Header("Wallet Journal")]
        [Description("The wallet journal of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        WalletJournal = 1 << 21,

        /// <summary>
        /// The wallet transactions of a character.
        /// </summary>
        [Header("Wallet Transactions")]
        [Description("The wallet transactions of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        WalletTransactions = 1 << 22,

        /// <summary>
        /// The skill in training of a character. Used to determine if an account type API key has a character in training.
        /// </summary>
        SkillInTraining = 1 << 17,

        /// <summary>
        /// A character's wallet balance.
        /// </summary>
        AccountBalance = 1 << 0,

        /// <summary>
        /// The character mailing lists. Used to convert mailing list IDs to Names.
        /// </summary>
        MailingLists = 1 << 10,

        /// <summary>
        /// The body text of an EVE mail message.
        /// </summary>
        MailBodies = 1 << 9,

        /// <summary>
        /// The body text of an EVE notification.
        /// </summary>
        NotificationTexts = 1 << 15,

        /// <summary>
        /// The character info. Used to fetch active ship, security status and last known location. 
        /// </summary>
        CharacterInfo = 1 << 23 | 1 << 24,

        /// <summary>
        /// Tha attendees to a character's calendar event.
        /// </summary>
        CalendarEventAttendees = 1 << 2,

        /// <summary>
        /// The contact list of a character.
        /// </summary>
        [Header("Contacts")]
        [Description("The contacts of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        ContactList = 1 << 4,

        /// <summary>
        /// Contact notifications for a character.
        /// </summary>
        ContactNotifications = 1 << 5,

        /// <summary>
        /// The Kill log for a character (Kill mails).
        /// </summary>
        [Header("Combat Log")]
        [Description("The combat log of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        KillLog = 1 << 8,

        /// <summary>
        /// The medals of a character.
        /// </summary>
        [Header("Medals")]
        [Description("The medals of a character.")]
        [Update(UpdatePeriod.Hours6, UpdatePeriod.Hours6, CacheStyle.Short)]
        Medals = 1 << 13,

        /// <summary>
        /// The upcoming calendar events for a character.
        /// </summary>
        //[Header("Calendar Events")]
        //[Description("The upcoming calendar events of a character.")]
        //[Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        UpcomingCalendarEvents = 1 << 20,

        /// <summary>
        /// Allows the fetching of coordinate and name data for items owned by the character.
        /// </summary>
        Locations = 1 << 27
    }
}