using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Medal
    {
        private readonly long m_medalID;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal Medal(SerializableMedalsListItem src)
        {
            m_medalID = src.MedalID;
            Reason = src.Reason;
            Status = src.Status;
            IssuerID = src.IssuerID;
            CorporationID = src.CorporationID;
            Description = src.Description;
            Title = src.Title;
            Issued = src.Issued;
            Group = src.Group;
        }

        #endregion


        #region Public Properties

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

        #endregion
    }
}
