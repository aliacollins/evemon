using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a base for character serialization in the settings
    /// </summary>
    public class SerializableSettingsCharacter : SerializableCharacterBase
    {
        public SerializableSettingsCharacter()
        {
            this.ImplantSets = new SerializableImplantSetCollection();
        }

        [XmlAttribute("guid")]
        public Guid Guid
        {
            get;
            set;
        }

        [XmlElement("implants")]
        public SerializableImplantSetCollection ImplantSets
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a serializable character defined from an uri file
    /// </summary>
    public sealed class SerializableUriCharacter : SerializableSettingsCharacter
    {
        [XmlElement("uri")]
        public string Uri
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a serializable character defined from the API
    /// </summary>
    public sealed class SerializableCCPCharacter : SerializableSettingsCharacter
    {
        public SerializableCCPCharacter()
        {
            this.SkillQueue = new List<SerializableQueuedSkill>();
            this.LastUpdates = new List<SerializableAPIUpdate>();
            this.MarketOrders = new List<SerializableOrderBase>();
            this.IndustryJobs = new List<SerializableJob>();
        }

        [XmlArray("queue")]
        [XmlArrayItem("skill")]
        public List<SerializableQueuedSkill> SkillQueue
        {
            get;
            set;
        }

        [XmlArray("marketOrders")]
        [XmlArrayItem("buy", typeof(SerializableBuyOrder))]
        [XmlArrayItem("sell", typeof(SerializableSellOrder))]
        public List<SerializableOrderBase> MarketOrders
        {
            get;
            set;
        }

        [XmlArray("industryJobs")]
        [XmlArrayItem("job")]
        public List<SerializableJob> IndustryJobs
        {
            get;
            set;
        }

        [XmlArray("lastUpdates")]
        [XmlArrayItem("apiUpdate")]
        public List<SerializableAPIUpdate> LastUpdates
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Represents an API method and the last time we updated it from CCP.
    /// </summary>
    public sealed class SerializableAPIUpdate
    {
        [XmlAttribute("method")]
        public APIMethods Method
        {
            get;
            set;
        }

        [XmlAttribute("time")]
        public DateTime Time
        {
            get;
            set;
        }

        public SerializableAPIUpdate Clone()
        {
            return (SerializableAPIUpdate)MemberwiseClone();
        }

    }
}
