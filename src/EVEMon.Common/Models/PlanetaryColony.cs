using System;
using System.Collections.Generic;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Models
{
    public sealed class PlanetaryColony : IComparable, IComparable<PlanetaryColony>
    {
        private readonly List<PlanetaryPin> m_planetaryPins = new List<PlanetaryPin>();
        private readonly List<PlanetaryRoute> m_planetaryRoutes = new List<PlanetaryRoute>();
        private readonly List<PlanetaryLink> m_planetaryLinks = new List<PlanetaryLink>();

        private bool m_queryPinsPending;
        private bool m_queryRoutesPending;
        private bool m_queryLinksPending;


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
            SolarSystem = StaticGeography.GetSolarSystemByID(src.SolarSystemID);
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
        public CCPCharacter Character { get; }

        /// <summary>
        /// Gets the planet identifier.
        /// </summary>
        /// <value>
        /// The planet identifier.
        /// </value>
        public long PlanetID { get; }

        /// <summary>
        /// Gets the name of the planet.
        /// </summary>
        /// <value>
        /// The name of the planet.
        /// </value>
        public string PlanetName { get; }

        /// <summary>
        /// Gets the name of the planet type.
        /// </summary>
        /// <value>
        /// The name of the planet type.
        /// </value>
        public string PlanetTypeName { get; }

        /// <summary>
        /// Gets the solar system where this job is located.
        /// </summary>
        /// <value>
        /// The solar system.
        /// </value>
        public SolarSystem SolarSystem { get; }

        /// <summary>
        /// Gets the installation full celestrial path.
        /// </summary>
        /// <value>
        /// The full location.
        /// </value>
        public string FullLocation => String.Format(CultureConstants.DefaultCulture, "{0} > {1}", SolarSystem.FullLocation, PlanetName);

        /// <summary>
        /// Gets the last update.
        /// </summary>
        /// <value>
        /// The last update.
        /// </value>
        public DateTime LastUpdate { get; }

        /// <summary>
        /// Gets the upgrade level.
        /// </summary>
        /// <value>
        /// The upgrade level.
        /// </value>
        public int UpgradeLevel { get; }

        /// <summary>
        /// Gets the number of pins.
        /// </summary>
        /// <value>
        /// The number of pins.
        /// </value>
        public int NumberOfPins { get; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>
        /// The pins.
        /// </value>
        public IEnumerable<PlanetaryPin> Pins => m_planetaryPins;

        /// <summary>
        /// Gets the routes.
        /// </summary>
        /// <value>
        /// The routes.
        /// </value>
        public IEnumerable<PlanetaryRoute> Routes => m_planetaryRoutes;

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public IEnumerable<PlanetaryLink> Links => m_planetaryLinks;

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the colony pins.
        /// </summary>
        private void GetColonyPins()
        {
            // Exit if we are already trying to download
            if (m_queryPinsPending)
                return;

            m_queryPinsPending = true;

            // Find the API key associated with planeatry pins
            APIKey apiKey = Character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.AssetList);

            // Quits if access denied
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIPlanetaryPins>(
                CCPAPIGenericMethods.PlanetaryPins, apiKey.ID, apiKey.VerificationCode, Character.CharacterID, PlanetID,
                OnPlanetaryPinsUpdated);
        }

        /// <summary>
        /// Gets the colony routes.
        /// </summary>
        private void GetColonyRoutes()
        {
            // Exit if we are already trying to download
            if (m_queryRoutesPending)
                return;

            m_queryRoutesPending = true;

            // Find the API key associated with planeatry pins
            APIKey apiKey = Character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.AssetList);

            // Quits if access denied
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIPlanetaryRoutes>(
                CCPAPIGenericMethods.PlanetaryRoutes, apiKey.ID, apiKey.VerificationCode, Character.CharacterID, PlanetID,
                OnPlanetaryRoutesUpdated);
        }

        /// <summary>
        /// Gets the colony links.
        /// </summary>
        private void GetColonyLinks()
        {
            // Exit if we are already trying to download
            if (m_queryLinksPending)
                return;

            m_queryLinksPending = true;

            // Find the API key associated with planeatry pins
            APIKey apiKey = Character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.AssetList);

            // Quits if access denied
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIPlanetaryLinks>(
                CCPAPIGenericMethods.PlanetaryLinks, apiKey.ID, apiKey.VerificationCode, Character.CharacterID, PlanetID,
                OnPlanetaryLinksUpdated);
        }

        /// <summary>
        /// Called when planetary pins updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnPlanetaryPinsUpdated(CCPAPIResult<SerializableAPIPlanetaryPins> result)
        {
            m_queryPinsPending = false;

            // Notify an error occured
            if (Character.ShouldNotifyError(result, CCPAPIGenericMethods.PlanetaryPins))
                EveMonClient.Notifications.NotifyCharacterPlanetaryPinsError(Character, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            Import(result.Result.Pins);

            // Fires the event regarding planetary pins updated
            EveMonClient.OnCharacterPlanetaryPinsUpdated(Character);
        }

        /// <summary>
        /// Called when planetary routes updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnPlanetaryRoutesUpdated(CCPAPIResult<SerializableAPIPlanetaryRoutes> result)
        {
            m_queryRoutesPending = false;

            // Notify an error occured
            if (Character.ShouldNotifyError(result, CCPAPIGenericMethods.PlanetaryRoutes))
                EveMonClient.Notifications.NotifyCharacterPlanetaryRoutesError(Character, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            Import(result.Result.Routes);

            // Fires the event regarding planetary routes updated
            EveMonClient.OnCharacterPlanetaryRoutesUpdated(Character);
        }

        /// <summary>
        /// Called when planetary links updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnPlanetaryLinksUpdated(CCPAPIResult<SerializableAPIPlanetaryLinks> result)
        {
            m_queryPinsPending = false;

            // Notify an error occured
            if (Character.ShouldNotifyError(result, CCPAPIGenericMethods.PlanetaryLinks))
                EveMonClient.Notifications.NotifyCharacterPlanetaryLinksError(Character, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            Import(result.Result.Links);

            // Fires the event regarding planetary links updated
            EveMonClient.OnCharacterPlanetaryLinksUpdated(Character);
        }

        #endregion


        #region Importation

        /// <summary>
        /// Imports the planeatry pins to a list.
        /// </summary>
        /// <param name="src">The source.</param>
        private void Import(IEnumerable<SerializablePlanetaryPin> src)
        {
            foreach (SerializablePlanetaryPin item in src)
            {
                m_planetaryPins.Add(new PlanetaryPin(this, item));
            }
        }

        /// <summary>
        /// Imports the planeatry routes to a list.
        /// </summary>
        /// <param name="src">The source.</param>
        private void Import(IEnumerable<SerializablePlanetaryRoute> src)
        {
            foreach (SerializablePlanetaryRoute item in src)
            {
                m_planetaryRoutes.Add(new PlanetaryRoute(this, item));
            }
        }

        /// <summary>
        /// Imports the planeatry links to a list.
        /// </summary>
        /// <param name="src">The source.</param>
        private void Import(IEnumerable<SerializablePlanetaryLink> src)
        {
            foreach (SerializablePlanetaryLink item in src)
            {
                m_planetaryLinks.Add(new PlanetaryLink(this, item));
            }
        }

        #endregion


        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal to <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />.
        /// </returns>
        public int CompareTo(object obj)
        {
            return CompareTo((PlanetaryColony)obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(PlanetaryColony other)
        {
            return this == other ? 1 : -1;
        }
    }
}