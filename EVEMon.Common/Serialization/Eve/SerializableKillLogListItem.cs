using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableKillLogListItem
    {
        private readonly Collection<SerializableKillLogAttackersListItem> m_attackers;
        private readonly Collection<SerializableKillLogItemListItem> m_items;

        public SerializableKillLogListItem()
        {
            m_attackers = new Collection<SerializableKillLogAttackersListItem>();
            m_items = new Collection<SerializableKillLogItemListItem>();
        }

        [XmlAttribute("killID")]
        public long KillID { get; set; }

        [XmlAttribute("solarSystemID")]
        public int SolarSystemID { get; set; }

        [XmlAttribute("killTime")]
        public string KillTimeXml
        {
            get { return KillTime.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    KillTime = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("moonID")]
        public int MoonID { get; set; }

        [XmlElement("victim")]
        public SerializableKillLogVictim Victim { get; set; }

        [XmlArray("attackers")]
        [XmlArrayItem("attacker")]
        public Collection<SerializableKillLogAttackersListItem> Attackers
        {
            get { return m_attackers; }
        }

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public Collection<SerializableKillLogItemListItem> Items
        {
            get { return m_items; }
        }

        [XmlIgnore]
        public DateTime KillTime { get; set; }
    }
}