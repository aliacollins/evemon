using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableContractListItem
    {
        [XmlAttribute("contractID")]
        public long ContractID { get; set; }

        [XmlAttribute("issuerID")]
        public long IssuerID { get; set; }

        [XmlAttribute("issuerCorpID")]
        public long IssuerCorpID { get; set; }

        [XmlAttribute("assigneeID")]
        public long AssigneeID { get; set; }

        [XmlAttribute("acceptorID")]
        public long AcceptorID { get; set; }

        [XmlAttribute("startStationID")]
        public int StartStationID { get; set; }

        [XmlAttribute("endStationID")]
        public int EndStationID { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }
   
        [XmlAttribute("forCorp")]
        public bool ForCorp { get; set; }

        [XmlAttribute("availability")]
        public string Availability { get; set; }

        [XmlAttribute("dateIssued")]
        public string DateIssuedXml
        {
            get { return DateIssued.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    DateIssued = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("dateExpired")]
        public string DateExpiredXml
        {
            get { return DateExpired.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    DateExpired = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("dateAccepted")]
        public string DateAcceptedXml
        {
            get { return DateAccepted.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    DateAccepted = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("numDays")]
        public int NumDays { get; set; }

        [XmlAttribute("dateCompleted")]
        public string DateCompletedXml
        {
            get { return DateCompleted.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    DateCompleted = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [XmlAttribute("reward")]
        public decimal Reward { get; set; }

        [XmlAttribute("collateral")]
        public decimal Collateral { get; set; }

        [XmlAttribute("buyout")]
        public decimal Buyout { get; set; }

        [XmlAttribute("volume")]
        public double Volume { get; set; }

        [XmlIgnore]
        public DateTime DateIssued { get; set; }

        [XmlIgnore]
        public DateTime DateExpired { get; set; }

        [XmlIgnore]
        public DateTime DateAccepted { get; set; }

        [XmlIgnore]
        public DateTime DateCompleted { get; set; }

        /// <summary>
        /// Which this contract was issued for.
        /// </summary>
        [XmlIgnore]
        public IssuedFor IssuedFor { get; set; }
    }
}