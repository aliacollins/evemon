using System.Xml;
using System.Xml.Serialization;

namespace PatchXmlCreator
{
    public sealed class SerializableRelease
    {
        [XmlElement("date")]
        public string Date
        {
            get;
            set;
        }

        [XmlElement("version")]
        public string Version
        {
            get;
            set;
        }

        [XmlElement("url")]
        public string TopicUrl
        {
            get;
            set;
        }

        [XmlElement("autopatchurl")]
        public string Url
        {
            get;
            set;
        }

        [XmlElement("autopatchargs")]
        public string InstallerArgs
        {
            get;
            set;
        }

        [XmlElement("additionalargs")]
        public string AdditionalArgs
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
