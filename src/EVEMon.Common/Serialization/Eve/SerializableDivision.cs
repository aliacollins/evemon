using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public class SerializableDivision
    {
        [XmlAttribute("accountKey")]
        public int AccountKey { get; set; }

        [XmlAttribute("description")]
        public string DescriptionXml
        {
            get { return Description; }
            set { Description = value?.HtmlDecode() ?? String.Empty; }
        }

        [XmlIgnore]
        public string Description { get; set; }
    }
}