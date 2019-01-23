using System;
using System.Threading.Tasks;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{
    public sealed class Loyalty
    {
        #region Fields

        private readonly Character m_character;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        internal Loyalty(Character character, EsiLoyaltyListItem src)
        {
            m_character = character;

            LoyaltyPoints = src.LoyaltyPoints;
            CorpId = src.CorpID;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the loyalty point value.
        /// </summary>
        /// <value>The loyalty point value.</value>
        public int LoyaltyPoints { get; }

        /// <summary>
        /// Gets or sets the corp ID.
        /// </summary>
        /// <value>The corp ID.</value>
        public int CorpId { get; }

        #endregion


        #region Helper Methods



        #endregion
    }
}
