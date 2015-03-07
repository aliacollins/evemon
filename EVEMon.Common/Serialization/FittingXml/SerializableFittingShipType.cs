using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.FittingXml
{
    public sealed class SerializableFittingShipType
    {
        [XmlAttribute("value")]
        public string Name { get; set; }
    }
}