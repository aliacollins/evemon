using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIAccountStatus
    {
        public SerializableAPIAccountStatus()
        {
        }

        [XmlElement("userID")]
        public long ID
        {
            get;
            set;
        }

        [XmlElement("paidUntil")]
        public string paidUntilXml
        {
            get { return PaidUntil.ToCCPTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    PaidUntil = value.CCPTimeStringToDateTime();
            }
        }

        [XmlElement("createDate")]
        public string createDateXml
        {
            get { return CreateDate.ToCCPTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    CreateDate = value.CCPTimeStringToDateTime();
            }
        }

        /// <summary>
        /// The date and time the account expires.
        /// </summary>
        [XmlIgnore]
        public DateTime PaidUntil
        {
            get;
            set;
        }

        /// <summary>
        /// The date and time the account was created.
        /// </summary>
        [XmlIgnore]
        public DateTime CreateDate
        {
            get;
            set;
        }

    }
}
