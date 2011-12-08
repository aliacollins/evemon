using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.FittingTools
{
    public sealed class SerializableFittingShipType
    {
        [XmlAttribute("value")]
        public string Name { get; set; }
    }
}