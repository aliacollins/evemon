using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableCalendarEventAttendeeListItem
    {
        [XmlAttribute("characterID")]
        public long CharacterID { get; set; }

        [XmlAttribute("name")]
        public string CharacterNameXml
        {
            get { return CharacterName; }
            set { CharacterName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlAttribute("response")]
        public string ResponseXml
        {
            get { return Response; }
            set { Response = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlIgnore]
        public string CharacterName { get; set; }

        [XmlIgnore]
        public string Response { get; set; }
    }
}