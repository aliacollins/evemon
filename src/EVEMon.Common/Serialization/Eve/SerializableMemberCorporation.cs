using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableMemberCorporation
    {
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