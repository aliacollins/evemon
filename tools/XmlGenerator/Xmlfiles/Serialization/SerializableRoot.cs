using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Xmlfiles.Serialization
{
    public class SerializableRoot<T>
    {
        [XmlElement("rowset")]
        public SerialiazableRowset<T> Rowset { get; set; }
    }
}