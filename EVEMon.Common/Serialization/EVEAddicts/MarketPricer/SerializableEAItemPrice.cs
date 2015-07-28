using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.EveAddicts.MarketPricer
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
            get
            {
                if (Math.Abs(BuyPrice) <= Double.Epsilon)
                    return SellPrice;

                if (Math.Abs(SellPrice) <= Double.Epsilon)
                    return BuyPrice;
               
                return (BuyPrice + SellPrice) / 2;
            }
        }
    }
}
