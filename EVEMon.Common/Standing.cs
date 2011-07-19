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

        private Character m_character;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal Standing(Character character, SerializableStandingsListItem src)
        {
            m_character = character;

            EntityID = src.ID;
            EntityName = src.Name;
            StandingValue = src.StandingValue;
            Group = src.GroupType;
            EntityImage = new Bitmap(32, 32);
            GetImage();
        }

        /// <summary>
        /// Constructor from the settings.
        /// </summary>
        /// <param name="src"></param>
        internal Standing(Character character, SerializableStanding src)
        {
            m_character = character;

            EntityID = src.EntityID;
            EntityName = src.EntityName;
            StandingValue = src.StandingValue;
            Group = src.Group;
            EntityImage = new Bitmap(32, 32);
            GetImage();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        public int EntityID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the standing value.
        /// </summary>
        /// <value>The standing value.</value>
        public double StandingValue { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public Image EntityImage { get; set; }

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

                if (EffectiveStanding < 5.5)
                    return StandingStatus.Good;

                return StandingStatus.Excellent;
            }
        }

        #endregion


        #region Private Properties

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="standing">The standing.</param>
        /// <returns></returns>
        private string ImageUrl
        {
            get
            {
                if (Group == "Agents")
                    return String.Format(NetworkConstants.CCPPortraits, EntityID, (int)EveImageSize.x32);

                return String.Format(NetworkConstants.CCPIconsFromImageServer,
                    (Group == "Factions" ? "alliance" : "corporation"),
                    EntityID, (int)EveImageSize.x32);
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the entity image.
        /// </summary>
        /// <returns></returns>
        private void GetImage()
        {
            ImageService.GetImageAsync(ImageUrl, true, (img) =>
            {
                if (img != null)
                {
                    EntityImage = img;

                    // Notify the subscriber that we got the image
                    // Note that if the image is in cache the event doesn't get fired
                    // as the event object is null
                    if (StandingImageUpdated != null)
                        StandingImageUpdated(this, EventArgs.Empty);
                }
            });
        }

        #endregion


        #region Export Method

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        internal SerializableStanding Export()
        {
            SerializableStanding serial = new SerializableStanding();

            serial.EntityID = EntityID;
            serial.EntityName = EntityName;
            serial.StandingValue = StandingValue;
            serial.Group = Group;

            return serial;
        }

        #endregion
    }
}
