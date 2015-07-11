using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.EVEAddicts.MarketPricer
{
    public sealed class SerializableEAItemPrice
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlElement("buy_price")]
        public double BuyPrice { get; set; }

        [XmlElement("sell_price")]
        public double SellPrice { get; set; }

        [XmlIgnore]
        public double Price
        {
            get { return (BuyPrice + SellPrice) / 2; }
        }
    }
}
