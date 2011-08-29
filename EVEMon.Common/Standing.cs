using System;
using System.Drawing;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

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
            Group = src.GroupType;
        }

        /// <summary>
        /// Constructor from the settings.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        internal Standing(Character character, SerializableStanding src)
        {
            m_character = character;

            EntityID = src.EntityID;
            EntityName = src.EntityName;
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
        public string Group { get; private set; }

        /// <summary>
        /// Gets or sets the entity image.
        /// </summary>
        /// <value>The entity image.</value>
        public Image EntityImage
        {
            get
            {
                if (m_image == null)
                {
                    m_image = GetDefaultImage();
                    GetImage();
                }
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
                int skillLevel = (StandingValue < 0 ?
                    m_character.Skills[DBConstants.DiplomacySkillID] :
                    m_character.Skills[DBConstants.ConnectionsSkillID])
                    .LastConfirmedLvl;
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

        private Image GetDefaultImage()
        {
            switch (Group)
            {
                case "Agents":
                    return Properties.Resources.DefaultCharacterImage32;
                case "NPC Corporations":
                    return Properties.Resources.DefaultCorporationImage32;
                case "Factions":
                    return Properties.Resources.DefaultAllianceImage32;
            }
            return new Bitmap(32, 32);
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <returns></returns>
        private string GetImageUrl()
        {
                if (Group == "Agents")
                    return String.Format(NetworkConstants.CCPPortraits, EntityID, (int)EveImageSize.x32);

                return String.Format(NetworkConstants.CCPIconsFromImageServer,
                    (Group == "Factions" ? "alliance" : "corporation"),
                    EntityID, (int)EveImageSize.x32);
        }

        /// <summary>
        /// Gets the entity image.
        /// </summary>
        private void GetImage()
        {
            ImageService.GetImageAsync(GetImageUrl(), true, img =>
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

        #endregion


        #region Export Method

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        internal SerializableStanding Export()
        {
            SerializableStanding serial = new SerializableStanding
                                              {
                                                  EntityID = EntityID,
                                                  EntityName = EntityName,
                                                  StandingValue = StandingValue,
                                                  Group = Group
                                              };


            return serial;
        }

        #endregion
    }
}
