using System;
using System.Drawing;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Standing
    {
        public event EventHandler StandingImageUpdated;


        #region Fields

        private readonly Character m_character;
        private Image m_image;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        internal Standing(Character character, SerializableStandingsListItem src)
        {
            m_character = character;

            EntityID = src.ID;
            EntityName = src.Name;
            StandingValue = src.StandingValue;
            Group = src.Group;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        public int EntityID { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string EntityName { get; private set; }

        /// <summary>
        /// Gets or sets the standing value.
        /// </summary>
        /// <value>The standing value.</value>
        public double StandingValue { get; private set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public StandingGroup Group { get; private set; }

        /// <summary>
        /// Gets or sets the entity image.
        /// </summary>
        /// <value>The entity image.</value>
        public Image EntityImage
        {
            get
            {
                if (m_image == null)
                    GetImage();

                return m_image;
            }
        }

        /// <summary>
        /// Gets the effective standing.
        /// </summary>
        /// <value>The effective standing.</value>
        public double EffectiveStanding
        {
            get
            {
                int skillLevel = (StandingValue < 0
                                      ? m_character.Skills[DBConstants.DiplomacySkillID]
                                      : m_character.Skills[DBConstants.ConnectionsSkillID]).LastConfirmedLvl;
                return StandingValue + (10 - StandingValue) * (skillLevel * 0.04);
            }
        }

        /// <summary>
        /// Gets the standing status.
        /// </summary>
        /// <value>The status.</value>
        public StandingStatus Status
        {
            get
            {
                if (EffectiveStanding <= -5.5)
                    return StandingStatus.Terrible;

                if (EffectiveStanding <= -0.5)
                    return StandingStatus.Bad;

                if (EffectiveStanding < 0.5)
                    return StandingStatus.Neutral;

                return EffectiveStanding < 5.5 ? StandingStatus.Good : StandingStatus.Excellent;
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
                                                              // Note that if the image is in cache the event doesn't get fired
                                                              // as the event object is null
                                                              if (StandingImageUpdated != null)
                                                                  StandingImageUpdated(this, EventArgs.Empty);
                                                          });
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns></returns>
        private Image GetDefaultImage()
        {
            switch (Group)
            {
                case StandingGroup.Agents:
                    return Properties.Resources.DefaultCharacterImage32;
                case StandingGroup.NPCCorporations:
                    return Properties.Resources.DefaultCorporationImage32;
                case StandingGroup.Factions:
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
            if (Group == StandingGroup.Agents)
                return new Uri(String.Format(CultureConstants.InvariantCulture,
                                             NetworkConstants.CCPPortraits, EntityID, (int)EveImageSize.x32));

            return new Uri(String.Format(CultureConstants.InvariantCulture, NetworkConstants.CCPIconsFromImageServer,
                                 (Group == StandingGroup.Factions ? "alliance" : "corporation"),
                                 EntityID, (int)EveImageSize.x32));
        }

        #endregion
    }
}