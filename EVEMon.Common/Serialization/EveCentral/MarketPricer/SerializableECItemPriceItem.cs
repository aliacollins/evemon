using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.EveCentral.MarketPricer
{
    public sealed class SerializableECItemPriceItem
    {
        [XmlElement("avg")]
        public double Average { get; set; }
    }
}