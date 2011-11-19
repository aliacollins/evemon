using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace EVEMon.Common.Serialization.BattleClinic
{
    public sealed class SerializableDatafile
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("md5")]
        public string MD5Sum { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("message")]
        public IXPathNavigable MessageXml
        {
            get { return new XmlDocument().CreateCDataSection(Message); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                Message = ((XmlCDataSection)value).Data;
            }
        }

        [XmlIgnore]
        public string Message { get; set; }
    }
}