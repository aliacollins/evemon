using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public class EveFactionWar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveFactionWar"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        public EveFactionWar(SerializableEveFactionWarsListItem src)
        {
            FactionID = src.FactionID;
            FactionName = src.FactionName;
            AgainstID = src.AgainstID;
            AgainstName = src.AgainstName;
        }


        #region Public Properties

        /// <summary>
        /// Gets or sets the faction ID.
        /// </summary>
        public int FactionID { get; set; }

        /// <summary>
        /// Gets or sets the name of the faction.
        /// </summary>
        public string FactionName { get; set; }

        /// <summary>
        /// Gets or sets the against ID.
        /// </summary>
        public int AgainstID { get; set; }

        /// <summary>
        /// Gets or sets the name of the against.
        /// </summary>
        public string AgainstName { get; set; }

        #endregion
   
    }
}