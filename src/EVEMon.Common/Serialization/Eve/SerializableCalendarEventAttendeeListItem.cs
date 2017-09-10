using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableCalendarEventAttendeeListItem
    {
        [XmlAttribute("characterID")]
        public long CharacterID { get; set; }

        [XmlAttribute("name")]
        public string CharacterNameXml
        {
            get { return CharacterName; }
            set { CharacterName = value?.HtmlDecode() ?? String.Empty; }
        }

        [XmlAttribute("response")]
        public string ResponseXml
        {
            get { return Response; }
            set { Response = value?.HtmlDecode() ?? String.Empty; }
        }

        [XmlIgnore]
        public string CharacterName { get; set; }

        [XmlIgnore]
        public string Response { get; set; }
    }
}