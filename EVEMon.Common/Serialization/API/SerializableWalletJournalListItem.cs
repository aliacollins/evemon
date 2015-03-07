using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableWalletJournalListItem
    {
        [XmlAttribute("refID")]
        public long ID { get; set; }

        [XmlAttribute("refTypeID")]
        public int RefTypeID { get; set; }

        [XmlAttribute("ownerID1")]
        public long OwnerID1 { get; set; }

        [XmlAttribute("ownerName1")]
        public string OwnerName1Xml
        {
            get { return OwnerName1; }
            set { OwnerName1 = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlAttribute("ownerID2")]
        public long OwnerID2 { get; set; }

        [XmlAttribute("ownerName2")]
        public string OwnerName2Xml
        {
            get { return OwnerName2; }
            set { OwnerName2 = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlAttribute("argID1")]
        public int ArgID1 { get; set; }

        [XmlAttribute("argName1")]
        public string ArgName1 { get; set; }

        [XmlAttribute("amount")]
        public decimal Amount { get; set; }

        [XmlAttribute("balance")]
        public decimal Balance { get; set; }

        [XmlAttribute("reason")]
        public string Reason { get; set; }

        [XmlAttribute("taxReceiverID")]
        public string TaxReceiverIDXml
        {
            get { return TaxReceiverID.ToString(CultureConstants.InvariantCulture); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    TaxReceiverID = Convert.ToInt64(value, CultureConstants.InvariantCulture);
            }
        }

        [XmlAttribute("taxAmount")]
        public string TaxAmountXml
        {
            get { return TaxAmount.ToString(CultureConstants.InvariantCulture); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    TaxAmount = Convert.ToDecimal(value, CultureConstants.InvariantCulture);
            }
        }

        [XmlAttribute("date")]
        public string DateXml
        {
            get { return Date.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    Date = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public string OwnerName1 { get; set; }

        [XmlIgnore]
        public string OwnerName2 { get; set; }

        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlIgnore]
        public long TaxReceiverID { get; set; }

        [XmlIgnore]
        public decimal TaxAmount { get; set; }
    }
}