using System;
using System.Linq;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Medal
    {
        #region Fields

        private CCPCharacter m_ccpCharacter;

        private string m_issuer;
        private string m_corporationName;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal Medal(CCPCharacter ccpCharacter, SerializableMedalsListItem src)
        {
            m_ccpCharacter = ccpCharacter;

            ID = src.MedalID;
            Reason = src.Reason;
            Status = src.Status;
            IssuerID = src.IssuerID;
            CorporationID = src.CorporationID;
            Description = src.Description;
            Title = src.Title;
            Issued = src.Issued;
            Group = src.Group;

            m_issuer = EveIDToName.GetIDToName(src.IssuerID);
            m_corporationName = EveIDToName.GetIDToName(CorporationID);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Gets the issuer ID.
        /// </summary>
        public long IssuerID { get; private set; }

        /// <summary>
        /// Gets the corporation ID.
        /// </summary>
        public long CorporationID { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the issued.
        /// </summary>
        public DateTime Issued { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        public MedalGroup Group { get; private set; }

        /// <summary>
        /// Gets or sets the number of times this medal was awarded.
        /// </summary>
        public int TimesAwarded { get; set; }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        public string Issuer
        {
            get
            {
                return m_issuer == EVEMonConstants.UnknownText
                           ? m_issuer = EveIDToName.GetIDToName(IssuerID)
                           : m_issuer;
            }
        }

        /// <summary>
        /// Gets the corporation name.
        /// </summary>
        public string CorporationName
        {
            get
            {
                return m_corporationName == EVEMonConstants.UnknownText
                           ? m_corporationName = EveIDToName.GetIDToName(IssuerID)
                           : m_corporationName;
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Tries the assign missing title and description.
        /// </summary>
        /// <returns></returns>
        public bool TryAssignMissingTitleAndDescription()
        {
            if (!String.IsNullOrEmpty(Title) && !String.IsNullOrEmpty(Description))
                return true;

            // Find the related medal in the corporation's medals
            Medal corporationMedal = m_ccpCharacter.CorporationMedals.SingleOrDefault(corpMedal => corpMedal.ID == ID);

            if (corporationMedal == null)
                return false;

            if (String.IsNullOrEmpty(Title))
                Title = corporationMedal.Title;

            if (String.IsNullOrEmpty(Description))
                Description = corporationMedal.Description;

            return true;
        }

        #endregion
    }
}
