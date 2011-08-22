using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableEmploymentHistoryListItem
    {
        [XmlAttribute("recordID")]
        public long RecordID { get; set; }

        [XmlAttribute("corporationID")]
        public long CorporationID { get; set; }

        [XmlAttribute("startDate")]
        public string StartDateXml
        {
            get { return StartDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    StartDate = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime StartDate { get; set; }
    }
}
