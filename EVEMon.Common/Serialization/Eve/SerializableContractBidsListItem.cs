using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableContractBidsListItem
    {
        [XmlAttribute("bidID")]
        public long ID { get; set; }

        [XmlAttribute("contractID")]
        public long ContractID { get; set; }

        [XmlAttribute("bidderID")]
        public long BidderID { get; set; }

        [XmlAttribute("dateBid")]
        public string DateBidXml
        {
            get { return DateBid.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    DateBid = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("amount")]
        public decimal Amount { get; set; }

        [XmlIgnore]
        public DateTime DateBid { get; set; }
    }
}