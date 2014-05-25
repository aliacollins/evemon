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
        private readonly ContactType m_contactType;
        private Image m_image;

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
            Group = (src.Group == ContactGroup.Personal && StaticGeography.AllAgents.Any(x => x.ID == m_contactID))
                ? ContactGroup.Agent
                : src.Group;

            m_contactType = src.ContactTypeID == DBConstants.CorporationID
                ? m_contactType = ContactType.Corporation
                : src.ContactTypeID == DBConstants.AllianceID
                    ? m_contactType = ContactType.Alliance
                    : ContactType.Character;
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
                if (m_image == null)
                    GetImage();
                
                return m_image;
            }
        }

        #endregion


        #region Helper Methods

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
                return
                    new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageBase,
                        String.Format(CultureConstants.InvariantCulture,
                            NetworkConstants.CCPPortraits, m_contactID, (int)EveImageSize.x32)));

            return
                new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageBase,
                    String.Format(CultureConstants.InvariantCulture, NetworkConstants.CCPIconsFromImageServer,
                        (m_contactType == ContactType.Alliance ? "alliance" : "corporation"),
                        m_contactID, (int)EveImageSize.x32)));
        }

        #endregion
    }
}