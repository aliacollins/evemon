using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    public class SerializableLocalhostEndPoint : SerializableEndPoint
    {
        [XmlAttribute("url")]
        public string UrlXml
        {
            get { return Url.ToString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    Url = new Uri(value);
            }
        }

        [XmlAttribute("key")]
        public string UploadKey { get; set; }

        [XmlAttribute("method")]
        public string MethodXml
        {
            get { return Method.ToString(); }
            set
            {
                if (!String.IsNullOrEmpty(value) &&
                    Enum.IsDefined(typeof(HttpMethod), value.ToTitleCase()))
                    Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), value.ToTitleCase());
            }
        }

        [XmlAttribute("compression")]
        public string CompressionXml
        {
            get { return Compression.ToString(); }
            set
            {
                if (!String.IsNullOrEmpty(value) &&
                    Enum.IsDefined(typeof(Compression), value.ToTitleCase()))
                    Compression = (Compression)Enum.Parse(typeof(Compression), value.ToTitleCase());
            }
        }

        [XmlIgnore]
        public Uri Url { get; set; }

        [XmlIgnore]
        public HttpMethod Method { get; set; }

        [XmlIgnore]
        public Compression Compression { get; set; }
    }
}