using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a serializable character defined from the API
    /// </summary>
    public sealed class SerializableCCPCharacter : SerializableSettingsCharacter
    {
        private readonly Collection<SerializableQueuedSkill> m_skillQueue;
        private readonly Collection<SerializableAPIUpdate> m_lastUpdates;
        private readonly Collection<SerializableStanding> m_standings;
        private readonly Collection<SerializableOrderBase> m_marketOrders;
        private readonly Collection<SerializableJob> m_industryJobs;

        public SerializableCCPCharacter()
        {
            m_skillQueue = new Collection<SerializableQueuedSkill>();
            m_lastUpdates = new Collection<SerializableAPIUpdate>();
            m_standings = new Collection<SerializableStanding>();
            m_marketOrders = new Collection<SerializableOrderBase>();
            m_industryJobs = new Collection<SerializableJob>();
        }

        [XmlArray("queue")]
        [XmlArrayItem("skill")]
        public Collection<SerializableQueuedSkill> SkillQueue
        {
            get { return m_skillQueue; }
        }

        [XmlArray("standings")]
        [XmlArrayItem("standing")]
        public Collection<SerializableStanding> Standings
        {
            get { return m_standings; }
        }


        [XmlArray("marketOrders")]
        [XmlArrayItem("buy", typeof(SerializableBuyOrder))]
        [XmlArrayItem("sell", typeof(SerializableSellOrder))]
        public Collection<SerializableOrderBase> MarketOrders
        {
            get { return m_marketOrders; }
        }


        [XmlArray("industryJobs")]
        [XmlArrayItem("job")]
        public Collection<SerializableJob> IndustryJobs
        {
            get { return m_industryJobs; }
        }


        [XmlElement("eveMailMessages")]
        public string EveMailMessagesIDs { get; set; }

        [XmlElement("eveNotifications")]
        public string EveNotificationsIDs { get; set; }

        [XmlArray("lastUpdates")]
        [XmlArrayItem("apiUpdate")]
        public Collection<SerializableAPIUpdate> LastUpdates
        {
            get { return m_lastUpdates; }
        }

    }
}