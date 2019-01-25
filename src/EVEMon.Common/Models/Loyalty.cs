using System;
using System.Drawing;
using System.Threading.Tasks;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    public sealed class Loyalty
    {
        #region Fields

        private readonly Character m_character;

        private string m_corporationName;
        private Image m_image;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        internal Loyalty(Character character, EsiLoyaltyListItem src)
        {
            m_character = character;

            LoyaltyPoints = src.LoyaltyPoints;
            CorpId = src.CorpID;
            m_corporationName = EveIDToName.GetIDToName(src.CorpID);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the corporation.
        /// </summary>
        /// <value>The name of the corporation.</value>
        public string CorporationName => m_corporationName.IsEmptyOrUnknown() ?
            (m_corporationName = EveIDToName.GetIDToName(CorpId)) : m_corporationName;

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
