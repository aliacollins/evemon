using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializablePlanetaryLink
    {
        [XmlAttribute("sourcePinID")]
        public long SourcePinID { get; set; }

        [XmlAttribute("destinationPinID")]
        public long DestinationPinID { get; set; }

        [XmlAttribute("linkLevel")]
        public short LinkLevel { get; set; }
    }
}