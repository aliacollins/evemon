using System;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumerations to support APIMethodsEnum.
    /// </summary>
    [Flags]
    public enum CCPAPIMethodsEnum
    {
        None = 0,

        /// <summary>
        /// The basic character features of APIMethodsEnum.
        /// </summary>
        BasicCharacterFeatures = CCPAPICharacterMethods.CharacterSheet | CCPAPICharacterMethods.CharacterInfo |
                                 CCPAPICharacterMethods.SkillQueue | CCPAPICharacterMethods.SkillInTraining,

        /// <summary>
        /// The advanced character features of APIMethodsEnum.
        /// </summary>
        AdvancedCharacterFeatures = CCPAPICharacterMethods.AccountStatus | CCPAPICharacterMethods.AssetList |
                                    /*APICharacterMethods.CalendarEventAttendees |*/ CCPAPICharacterMethods.ContactList |
                                    CCPAPICharacterMethods.Contracts | CCPAPICharacterMethods.FactionalWarfareStats |
                                    CCPAPICharacterMethods.FactionalWarfareStats | CCPAPICharacterMethods.IndustryJobs |
                                    CCPAPICharacterMethods.KillLog | CCPAPICharacterMethods.MailMessages |
                                    CCPAPICharacterMethods.MailBodies | CCPAPICharacterMethods.MailingLists |
                                    CCPAPICharacterMethods.MarketOrders | CCPAPICharacterMethods.Medals |
                                    CCPAPICharacterMethods.Notifications | CCPAPICharacterMethods.NotificationTexts |
                                    CCPAPICharacterMethods.ResearchPoints | CCPAPICharacterMethods.Standings |
                                    CCPAPICharacterMethods.WalletJournal | CCPAPICharacterMethods.WalletTransactions /*|
            APICharacterMethods.UpcomingCalendarEvents*/
        ,

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