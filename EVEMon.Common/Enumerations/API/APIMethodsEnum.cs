using System;

namespace EVEMon.Common.Enumerations.API
{
    /// <summary>
    /// Enumerations to support APIMethodsEnum.
    /// </summary>
    [Flags]
    public enum APIMethodsEnum
    {
        None = 0,

        /// <summary>
        /// The basic character features of APIMethodsEnum.
        /// </summary>
        BasicCharacterFeatures = APICharacterMethods.CharacterSheet | APICharacterMethods.CharacterInfo |
                                 APICharacterMethods.SkillQueue | APICharacterMethods.SkillInTraining,

        /// <summary>
        /// The advanced character features of APIMethodsEnum.
        /// </summary>
        AdvancedCharacterFeatures = APICharacterMethods.AccountStatus | APICharacterMethods.AssetList |
                                    /*APICharacterMethods.CalendarEventAttendees |*/ APICharacterMethods.ContactList |
                                    APICharacterMethods.Contracts | APICharacterMethods.FactionalWarfareStats |
                                    APICharacterMethods.FactionalWarfareStats | APICharacterMethods.IndustryJobs |
                                    APICharacterMethods.KillLog | APICharacterMethods.MailMessages |
                                    APICharacterMethods.MailBodies | APICharacterMethods.MailingLists |
                                    APICharacterMethods.MarketOrders | APICharacterMethods.Medals |
                                    APICharacterMethods.Notifications | APICharacterMethods.NotificationTexts |
                                    APICharacterMethods.ResearchPoints | APICharacterMethods.Standings |
                                    APICharacterMethods.WalletJournal | APICharacterMethods.WalletTransactions /*|
            APICharacterMethods.UpcomingCalendarEvents*/
        ,

        /// <summary>
        /// The advanced corporation features of APIMethodsEnum.
        /// </summary>
        AdvancedCorporationFeatures = APICorporationMethods.CorporationContracts | APICorporationMethods.CorporationMedals |
                                      APICorporationMethods.CorporationMarketOrders |
                                      APICorporationMethods.CorporationIndustryJobs,

        /// <summary>
        /// All character features of APIMethodsEnum
        /// </summary>
        AllCharacterFeatures = BasicCharacterFeatures | AdvancedCharacterFeatures,
    }
}