using System;
using System.Drawing;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Contact
    {
        public event EventHandler ContactImageUpdated;


        #region Fields

        private readonly long m_contactID;
        private Image m_image;
        private ContactType m_contactType;
        private bool m_contactTypeChanged;

        #endregion


        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal Contact(SerializableContactListItem src)
        {
            m_contactID = src.ContactID;
            Name = src.ContactName;
            IsInWatchlist = src.InWatchlist;
            Standing = src.Standing;
            Group = (src.Group == ContactGroup.Contact && StaticGeography.AllAgents.Any(x => x.ID == m_contactID))
                        ? ContactGroup.Agent
                        : src.Group;

            GetContactType();
        }

        #region Public Properties

        /// <summary>
        /// Gets the name of the contact.
        /// </summary>
        /// <value>
        /// The name of the contact.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the contact is in the watchlist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the contact is in the watchlist; otherwise, <c>false</c>.
        /// </value>
        public bool IsInWatchlist { get; private set; }

        /// <summary>
        /// Gets the standing.
        /// </summary>
        public double Standing { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        public ContactGroup Group { get; private set; }


        /// <summary>
        /// Gets the entity image.
        /// </summary>
        public Image EntityImage
        {
            get
            {
                // When the contact type changed update the image
                if (m_contactTypeChanged)
                {
                    m_image = null;
                    
                    // Reset flag
                    m_contactTypeChanged = false;
                }

                if (m_image == null)
                    GetImage();
                
                return m_image;
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the type of the contact.
        /// </summary>
        private void GetContactType()
        {
            // Quit here if it's an EVE agent
            if (Group == ContactGroup.Agent)
                return;

            // Assign the contact type if it's an EVE Faction
            if(DBConstants.FactionIDs.Contains((int)m_contactID))
            {
                m_contactType = ContactType.Alliance;
                return;
            }

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICharacterInfo>(
                APICharacterMethods.CharacterInfo, 0, null, m_contactID, OnCharacterInfoQueried);
        }

        /// <summary>
        /// Called when the character info got queried.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterInfoQueried(APIResult<SerializableAPICharacterInfo> result)
        {
            if (result.EVEDatabaseError)
                return;

            if (!result.HasError)
                return;

            if (result.CCPError != null && result.CCPError.IsCharacterInfoFailure)
            {
                EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICorporationSheet>(
                    APICorporationMethods.CorporationSheet, 0, null, m_contactID, OnCorporationSheetQuery);
            }
        }

        /// <summary>
        /// Called when the corporation sheet got queried.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCorporationSheetQuery(APIResult<SerializableAPICorporationSheet> result)
        {
            if (result.EVEDatabaseError)
                return;

            // Set flag for contact type change
            m_contactTypeChanged = true;

            if (result.HasError)
            {
                if (result.CCPError != null && result.CCPError.IsCorporationInfoFailure)
                    m_contactType = ContactType.Alliance;

                return;
            }

            m_contactType = ContactType.Corporation;
        }

        /// <summary>
        /// Gets the entity image.
        /// </summary>
        private void GetImage()
        {
            m_image = GetDefaultImage();
            ImageService.GetImageAsync(GetImageUrl(), img =>
                                                          {
                                                              if (img == null)
                                                                  return;

                                                              m_image = img;

                                                              // Notify the subscriber that we got the image
                                                              if (ContactImageUpdated != null)
                                                                  ContactImageUpdated(this, EventArgs.Empty);
                                                          });
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns></returns>
        private Image GetDefaultImage()
        {
            switch (m_contactType)
            {
                case ContactType.Character:
                    return Properties.Resources.DefaultCharacterImage32;
                case ContactType.Corporation:
                    return Properties.Resources.DefaultCorporationImage32;
                case ContactType.Alliance:
                    return Properties.Resources.DefaultAllianceImage32;
            }
            return new Bitmap(32, 32);
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <returns></returns>
        private Uri GetImageUrl()
        {
            if (m_contactType == ContactType.Character)
                return new Uri(String.Format(CultureConstants.InvariantCulture,
                                             NetworkConstants.CCPPortraits, m_contactID, (int)EveImageSize.x32));

            return new Uri(String.Format(CultureConstants.InvariantCulture, NetworkConstants.CCPIconsFromImageServer,
                                         (m_contactType == ContactType.Alliance ? "alliance" : "corporation"),
                                         m_contactID, (int)EveImageSize.x32));
        }

        #endregion
    }
}