using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
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
        public XmlCDataSection MessageXml
        {
            get;
            set;
        }

        [XmlIgnore]
        public string Message
        {
            get { return MessageXml.InnerText; }
            set
            {
                XmlDocument xmlDoc = new XmlDocument();
                MessageXml = xmlDoc.CreateCDataSection(value);
            }
        }
    }
}
