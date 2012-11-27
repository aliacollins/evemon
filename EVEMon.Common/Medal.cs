using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Medal
    {
        #region Fields

        private string m_issuer;
        private string m_corporationName;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal Medal(SerializableMedalsListItem src)
        {
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
        public string Description { get; set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; set; }

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
                return m_issuer == "Unknown"
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
                return m_corporationName == "Unknown"
                           ? m_corporationName = EveIDToName.GetIDToName(IssuerID)
                           : m_corporationName;
            }
        }

        #endregion
    }
}
