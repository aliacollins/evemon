using System;
using System.Collections.Generic;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class KillLog
    {
        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal KillLog(SerializableKillLogListItem src)
        {
            KillID = src.KillID;
            SolarSystemID = src.SolarSystemID;
            KillTime = src.KillTime;
            MoonID = src.MoonID;
            Victim = src.Victim;
            Attackers = src.Attackers;
            Items = src.Items;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the kill ID.
        /// </summary>
        public long KillID { get; private set; }

        /// <summary>
        /// Gets the solar system ID.
        /// </summary>
        public int SolarSystemID { get; private set; }

        /// <summary>
        /// Gets the kill time.
        /// </summary>
        public DateTime KillTime { get; private set; }

        /// <summary>
        /// Gets the moon ID.
        /// </summary>
        public int MoonID { get; private set; }

        /// <summary>
        /// Gets the victim.
        /// </summary>
        public SerializableKillLogVictim Victim { get; private set; }

        /// <summary>
        /// Gets the attackers.
        /// </summary>
        public IEnumerable<SerializableKillLogAttackersListItem> Attackers { get; private set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEnumerable<SerializableKillLogItemListItem> Items { get; private set; }

        #endregion
    }
}