using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    public sealed class SerializableLoadout
    {
        public SerializableLoadout()
        {
            Slots = new List<SerializableLoadoutSlot>();
        }

        private string m_loadoutName;

        [XmlAttribute("name")]
        public string LoadoutName
        {
            get { return HttpUtility.HtmlDecode(m_loadoutName); }
            set { m_loadoutName = value; }
        }

        [XmlAttribute("Author")]
        public string Author { get; set; }

        [XmlAttribute("rating")]
        public double Rating { get; set; }

        [XmlAttribute("loadoutID")]
        public string LoadoutId { get; set; }

        [XmlAttribute("date")]
        public string SubmissionDateString { get; set; }

        [XmlIgnore]
        public DateTime SubmissionDate
        {
            get
            {
                DateTime parsedDate;
                return DateTime.TryParse(SubmissionDateString, out parsedDate) ? parsedDate : DateTime.MinValue;
            }
        }

        [XmlAttribute("topic")]
        public int Topic { get; set; }

        [XmlElement("slot")]
        public List<SerializableLoadoutSlot> Slots { get; set; }
    }
}