using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableAllianceListItem
    {
        private readonly Collection<SerializableMemberCorporation> m_memberCorporations;

        public SerializableAllianceListItem()
        {
            m_memberCorporations = new Collection<SerializableMemberCorporation>();
        }

        [XmlAttribute("name")]
        public string NameXml
        {
            get { return Name; }
            set { Name = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlAttribute("shortName")]
        public string Ticker { get; set; }

        [XmlAttribute("allianceID")]
        public long ID { get; set; }

        [XmlAttribute("executorCorpID")]
        public long ExecutorCorpID { get; set; }

        [XmlAttribute("memberCount")]
        public int MemberCount { get; set; }

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

        [XmlArray("memberCorporations")]
        [XmlArrayItem("memberCorporation")]
        public Collection<SerializableMemberCorporation> MemberCorporations
        {
            get { return m_memberCorporations; }
        }

        [XmlIgnore]
        public DateTime StartDate { get; set; }

        [XmlIgnore]
        public string Name { get; set; }
    }
}