using System;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumerations to support APIMethodsEnum.
    /// </summary>
    [Flags]
    public enum CCPAPIMethodsEnum : ulong
    {
        None = 0,

        /// <summary>
        /// The basic character features of APIMethodsEnum.
        /// </summary>
        BasicCharacterFeatures = ESIAPICharacterMethods.CharacterSheet | ESIAPICharacterMethods.CharacterInfo |
                                 ESIAPICharacterMethods.SkillQueue,

        /// <summary>
        /// The advanced character features of APIMethodsEnum.
        /// </summary>
        AdvancedCharacterFeatures = ESIAPICharacterMethods.CalendarEventAttendees | ESIAPICharacterMethods.AssetList |
                                    ESIAPICharacterMethods.ContactList | ESIAPICharacterMethods.UpcomingCalendarEvents |
                                    ESIAPICharacterMethods.Contracts | ESIAPICharacterMethods.FactionalWarfareStats |
                                    ESIAPICharacterMethods.FactionalWarfareStats | ESIAPICharacterMethods.IndustryJobs |
                                    ESIAPICharacterMethods.KillLog | ESIAPICharacterMethods.MailMessages |
                                    ESIAPICharacterMethods.MailBodies | ESIAPICharacterMethods.MailingLists |
                                    ESIAPICharacterMethods.MarketOrders | ESIAPICharacterMethods.Medals |
                                    ESIAPICharacterMethods.Notifications | ESIAPICharacterMethods.NotificationTexts |
                                    ESIAPICharacterMethods.ResearchPoints | ESIAPICharacterMethods.Standings |
                                    ESIAPICharacterMethods.WalletJournal | ESIAPICharacterMethods.WalletTransactions,

        /// <summary>
        /// The advanced corporation features of APIMethodsEnum.
        /// </summary>
        AdvancedCorporationFeatures = CCPAPICorporationMethods.CorporationContracts | CCPAPICorporationMethods.CorporationMedals |
                                      CCPAPICorporationMethods.CorporationMarketOrders |
                                      CCPAPICorporationMethods.CorporationIndustryJobs,

        /// <summary>
        /// All character features of APIMethodsEnum
        /// </summary>
        AllCharacterFeatures = BasicCharacterFeatures | AdvancedCharacterFeatures,
    }
}
