using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    public sealed class SerializablePropertyCategory
    {
        [XmlElement("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description
        {
            get;
            set;
        }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public SerializableProperty[] Properties
        {
            get;
            set;
        }
    }
}
