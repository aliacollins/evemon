using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableMailingListsListItem
    {
        [XmlAttribute("listID")]
        public long ID { get; set; }

        [XmlAttribute("displayName")]
        public string DisplayNameXml
        {
            get { return DisplayName; }
            set { DisplayName = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlIgnore]
        public string DisplayName { get; set; }
    }
}