using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableWalletTransactionsListItem
    {
        [XmlAttribute("transactionID")]
        public long ID { get; set; }

        [XmlAttribute("journalTransactionID")]
        public long JournalTransactionID { get; set; }

        [XmlAttribute("quantity")]
        public long Quantity { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("typeName")]
        public string TypeName { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [XmlAttribute("clientID")]
        public long ClientID { get; set; }

        [XmlAttribute("clientName")]
        public string ClientName { get; set; }

        [XmlAttribute("stationID")]
        public int StationID { get; set; }

        [XmlAttribute("stationName")]
        public string StationName { get; set; }

        [XmlAttribute("transactionType")]
        public string TransactionType { get; set; }

        [XmlAttribute("transactionFor")]
        public string TransactionFor { get; set; }

        [XmlAttribute("transactionDateTime")]
        public string TransactionDateXml
        {
            get { return TransactionDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    TransactionDate = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime TransactionDate { get; set; }
    }
}