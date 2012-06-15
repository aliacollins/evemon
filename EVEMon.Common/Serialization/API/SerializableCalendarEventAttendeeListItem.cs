using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableCalendarEventAttendeeListItem
    {
        [XmlAttribute("characterID")]
        public long CharacterID { get; set; }

        [XmlAttribute("name")]
        public string CharacterName { get; set; }

        [XmlAttribute("response")]
        public string Response { get; set; }
    }
}