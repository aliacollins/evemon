using System;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EmploymentRecord
    {
        private readonly Character m_character;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        public EmploymentRecord(Character character, SerializableEmploymentHistoryListItem src)
        {
            m_character = character;

            CorporationName = GetIDToName(src.CorporationID);
            StartDate = src.StartDate;
        }

        /// <summary>
        /// Constructor from the settings.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        public EmploymentRecord(Character character, SerializableEmploymentHistory src)
        {
            m_character = character;

            CorporationName = src.CorporationName;
            StartDate = src.StartDate.TimeStringToDateTime();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the corporation.
        /// </summary>
        /// <value>The name of the corporation.</value>
        public string CorporationName { get; private set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; private set; }

        #endregion


        #region Export Method

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        public SerializableEmploymentHistory Export()
        {
            SerializableEmploymentHistory serial = new SerializableEmploymentHistory
                                                       {
                                                           CorporationName = CorporationName,
                                                           StartDate = StartDate.DateTimeToTimeString()
                                                       };
            return serial;
        }

        #endregion


        #region Helper Method

        /// <summary>
        /// Gets the Corporation name from the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private static string GetIDToName(long id)
        {
            NPCCorporation corporation = StaticGeography.GetCorporationByID(id);
            string corporationName = corporation != null ? corporation.Name : String.Empty;

            // If it's a player's corporation, query the API
            return String.IsNullOrEmpty(corporationName) ? EveIDToName.GetIDToName(id) : corporationName;
        }

        #endregion
    }
}