using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Medal
    {
        private readonly long m_medalID;

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


        #region Public Properties

        public string Reason { get; private set; }

        public string Status { get; private set; }

        public long IssuerID { get; private set; }

        public long CorporationID { get; private set; }

        public string Description { get; private set; }

        public string Title { get; set; }

        public DateTime Issued { get; private set; }

        public MedalGroup Group { get; set; }

        #endregion

    }
}
