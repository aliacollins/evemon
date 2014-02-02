using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class CrtCertificates : IHasID
    {
        [XmlElement("certificateID")]
        public int ID { get; set; }

        [XmlElement("categoryID")]
        public int CategoryID { get; set; }

        [XmlElement("classID")]
        public int ClassID { get; set; }

        [XmlElement("grade")]
        public int Grade { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }
    }
}