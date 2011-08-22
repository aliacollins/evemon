using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableEmploymentHistory
    {
        [XmlAttribute("corporationName")]
        public string CorporationName { get; set; }

        [XmlAttribute("startDate")]
        public string StartDate { get; set; }
    }
}