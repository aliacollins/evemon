using System;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    public sealed class SerializableRelease
    {
        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("md5")]
        public string MD5Sum { get; set; }

        [XmlElement("url")]
        public string TopicAddress { get; set; }

        [XmlElement("autopatchurl")]
        public string PatchAddress { get; set; }

        [XmlElement("autopatchargs")]
        public string InstallerArgs { get; set; }

        [XmlElement("additionalargs")]
        public string AdditionalArgs { get; set; }

        [XmlElement("message")]
        public XmlCDataSection MessageXml
        {
            get { return new XmlDocument().CreateCDataSection(Message); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                Message = value.Data;
            }
        }

        [XmlIgnore]
        public string Message { get; set; }
    }
}