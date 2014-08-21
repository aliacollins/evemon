using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class PlanetaryColony
    {
        private bool m_queryColoniesPending;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetaryColony"/> class.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal PlanetaryColony(CCPCharacter ccpCharacter, SerializablePlanetaryColony src)
        {
            Character = ccpCharacter;
            PlanetID = src.PlanetID;
            PlanetName = src.PlanetName;
            SolarSystemName = src.SolarSystemName;
            PlanetTypeName = src.PlanetTypeName;
            LastUpdate = src.LastUpdate;
            UpgradeLevel = src.UpgradeLevel;
            NumberOfPins = src.NumberOfPins;

            GetColonyPins();
            GetColonyRoutes();
            GetColonyLinks();
        }

        #endregion


        #region Public Properties
        /// <summary>
        /// Gets the character.
        /// </summary>
        public CCPCharacter Character { get; private set; }

        /// <summary>
        /// Gets the planet identifier.
        /// </summary>
        /// <value>
        /// The planet identifier.
        /// </value>
        public long PlanetID { get; private set; }

        /// <summary>
        /// Gets the name of the solar system.
        /// </summary>
        /// <value>
        /// The name of the solar system.
        /// </value>
        public string SolarSystemName { get; private set; }

        /// <summary>
        /// Gets the name of the planet.
        /// </summary>
        /// <value>
        /// The name of the planet.
        /// </value>
        public string PlanetName { get; private set; }

        /// <summary>
        /// Gets the name of the planet type.
        /// </summary>
        /// <value>
        /// The name of the planet type.
        /// </value>
        public string PlanetTypeName { get; private set; }

        /// <summary>
        /// Gets the last update.
        /// </summary>
        /// <value>
        /// The last update.
        /// </value>
        public DateTime LastUpdate { get; private set; }

        /// <summary>
        /// Gets the upgrade level.
        /// </summary>
        /// <value>
        /// The upgrade level.
        /// </value>
        public int UpgradeLevel { get; private set; }

        /// <summary>
        /// Gets the number of pins.
        /// </summary>
        /// <value>
        /// The number of pins.
        /// </value>
        public int NumberOfPins { get; private set; }

        #endregion


        #region Helper Methods

        private void GetColonyPins()
        {
            // Exit if we are already trying to download
            if (m_queryColoniesPending)
                return;

            m_queryColoniesPending = true;

            // Find the API key associated with planeatry pins
            APIKey apiKey = Character.Identity.FindAPIKeyWithAccess(APICharacterMethods.AssetList);

            // Quits if access denied
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIPlanetaryPins>(
                APIGenericMethods.PlanetaryPins, apiKey.ID, apiKey.VerificationCode, Character.CharacterID, PlanetID,
                OnPlanetaryPinsUpdated);
        }

        private void GetColonyLinks()
        {
        }

        private void GetColonyRoutes()
        {
        }

        /// <summary>
        /// Called when planetary pins updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnPlanetaryPinsUpdated(APIResult<SerializableAPIPlanetaryPins> result)
        {
            m_queryColoniesPending = false;

            //// Notify an error occured
            //if (Character.ShouldNotifyError(result, m_apiMethod))
            //    EveMonClient.Notifications.NotifyContractItemsError(Character, result);

            //// Quits if there is an error
            //if (result.HasError)
            //    return;

            //// Re-fetch the items if for any reason a previous attempt failed
            //if (!result.Result.ContractItems.Any())
            //{
            //    GetContractItems();
            //    return;
            //}

            //// Import the data
            //Import(result.Result.ContractItems);

            //// Fires the event regarding contract items downloaded
            //if (m_apiMethod == APIGenericMethods.ContractItems)
            //    EveMonClient.OnCharacterContractItemsDownloaded(Character);
            //else
            //    EveMonClient.OnCorporationContractItemsDownloaded(Character);
        }

        #endregion
    }
}