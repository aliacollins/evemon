using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAllianceListItem
    {
        private readonly Collection<SerializableMemberCorporation> m_memberCorporations;

        public SerializableAllianceListItem()
        {
            m_memberCorporations = new Collection<SerializableMemberCorporation>();
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

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

        [XmlAttribute("memberCorporations")]
        [XmlArrayItem("memberCorporation")]
        public Collection<SerializableMemberCorporation> MemberCorporations
        {
            get { return m_memberCorporations; }
        }

        [XmlIgnore]
        public DateTime StartDate { get; set; }
    }
}