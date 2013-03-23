using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableOutpost
    {
        [XmlAttribute("stationID")]
        public int StationID { get; set; }

        [XmlAttribute("stationName")]
        public string StationNameXml
        {
            get { return StationName; }
            set { StationName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlAttribute("stationTypeID")]
        public int StationTypeID { get; set; }

        [XmlAttribute("solarSystemID")]
        public int SolarSystemID { get; set; }

        [XmlAttribute("corporationID")]
        public int CorporationID { get; set; }

        [XmlAttribute("corporationName")]
        public string CorporationNameXml
        {
            get { return CorporationName; }
            set { CorporationName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlIgnore]
        public string StationName { get; set; }

        [XmlIgnore]
        public string CorporationName { get; set; }
    }
}