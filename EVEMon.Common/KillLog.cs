using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class KillLog
    {
        public event EventHandler KillLogImageUpdated;


        #region Fields

        private Image m_image;
        private readonly int m_solarSystemID;
        private readonly long m_killID;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="src">The source.</param>
        internal KillLog(Character character, SerializableKillLogListItem src)
        {
            m_killID = src.KillID;
            m_solarSystemID = src.SolarSystemID;
            KillTime = src.KillTime;
            TimeSinceKill = DateTime.UtcNow.Subtract(src.KillTime);
            MoonID = src.MoonID;
            Victim = src.Victim;
            Attackers = src.Attackers;
            Items = src.Items;

            Group = src.Victim.ID == character.CharacterID ? KillGroup.Losses : KillGroup.Kills;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the solar system.
        /// </summary>
        public SolarSystem SolarSystem
        {
            get { return StaticGeography.GetSolarSystemByID(m_solarSystemID); }
        }

        /// <summary>
        /// Gets the kill time.
        /// </summary>
        public DateTime KillTime { get; private set; }

        /// <summary>
        /// Gets the time since kill.
        /// </summary>
        public TimeSpan TimeSinceKill { get; private set; }

        /// <summary>
        /// Gets the moon ID.
        /// </summary>
        public int MoonID { get; private set; }

        /// <summary>
        /// Gets the victim.
        /// </summary>
        public SerializableKillLogVictim Victim { get; private set; }

        /// <summary>
        /// Gets the final blow attacker.
        /// </summary>
        public SerializableKillLogAttackersListItem FinalBlowAttacker
        {
            get { return Attackers.Single(x => x.FinalBlow); }
        }

        /// <summary>
        /// Gets the attackers.
        /// </summary>
        public IEnumerable<SerializableKillLogAttackersListItem> Attackers { get; private set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEnumerable<SerializableKillLogItemListItem> Items { get; private set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public KillGroup Group { get; private set; }

        /// <summary>
        /// Gets the victim image.
        /// </summary>
        public Image VictimShipImage
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
            m_image = new Bitmap(32, 32);
            ImageService.GetImageAsync(GetImageUrl(), img =>
                                                          {
                                                              if (img == null)
                                                                  return;

                                                              m_image = img;

                                                              // Notify the subscriber that we got the image
                                                              if (KillLogImageUpdated != null)
                                                                  KillLogImageUpdated(this, EventArgs.Empty);
                                                          });
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <returns></returns>
        private Uri GetImageUrl()
        {
            return new Uri(String.Format(CultureConstants.InvariantCulture,
                                         NetworkConstants.CCPIconsFromImageServer, "type", Victim.ShipTypeID,
                                         (int)EveImageSize.x32));
        }

        #endregion
    }
}