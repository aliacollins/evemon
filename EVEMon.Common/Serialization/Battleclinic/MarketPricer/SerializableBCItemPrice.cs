using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic.MarketPricer
{
    public class SerializableBCItemPrice
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlElement("value")]
        public double Price { get; set; }
    }
}