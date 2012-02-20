using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableContractBid
    {
        [XmlAttribute("bidID")]
        public long BidID { get; set; }

        [XmlAttribute("contractID")]
        public long ContractID { get; set; }

        [XmlAttribute("Bidder")]
        public string Bidder { get; set; }

        [XmlAttribute("amount")]
        public decimal Amount { get; set; }

        [XmlAttribute("bidDate")]
        public DateTime BidDate { get; set; }
    }
}
