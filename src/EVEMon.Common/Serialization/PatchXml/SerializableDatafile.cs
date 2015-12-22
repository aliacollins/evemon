using System;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.PatchXml
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
        public string Address { get; set; }

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