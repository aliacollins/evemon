using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializablePlanetaryRoute
    {
        [XmlAttribute("routeID")]
        public long RouteID { get; set; }

        [XmlAttribute("sourcePinID")]
        public long SourcePinID { get; set; }

        [XmlAttribute("destinationPinID")]
        public long DestinationPinID { get; set; }

        [XmlAttribute("contentTypeID")]
        public long ContentTypeID { get; set; }

        [XmlAttribute("contentTypeName")]
        public string ContentTypeName { get; set; }

        [XmlAttribute("quantity")]
        public int Quantity { get; set; }

        [XmlAttribute("waypoint1")]
        public double Waypoint1 { get; set; }

        [XmlAttribute("waypoint2")]
        public double Waypoint2 { get; set; }

        [XmlAttribute("waypoint3")]
        public double Waypoint3 { get; set; }

        [XmlAttribute("waypoint4")]
        public double Waypoint4 { get; set; }

        [XmlAttribute("waypoint5")]
        public double Waypoint5 { get; set; }
    }
}