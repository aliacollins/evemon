using System.Xml;
using System.Xml.Serialization;

namespace PatchXmlCreator
{
    public sealed class SerializableDatafile
    {
        [XmlElement("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("date")]
        public string Date
        {
            get;
            set;
        }

        [XmlElement("md5")]
        public string MD5Sum
        {
            get;
            set;
        }

        [XmlElement("url")]
        public string Url
        {
            get;
            set;
        }

        [XmlElement("message")]
        public XmlCDataSection Message
        {
            get;
            set;
        }
    }
}
