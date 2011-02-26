using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of the training queue. USed for settings.xml serialization and CCP querying
    /// </summary>
    public sealed class SerializableAPISkillQueue : ISynchronizableWithLocalClock
    {
        /// <summary>
        /// Defautl constructor for XML serialzation
        /// </summary>
        public SerializableAPISkillQueue()
        {
            Queue = new List<SerializableQueuedSkill>();
        }

        [XmlArray("queue")]
        [XmlArrayItem("skill")]
        public List<SerializableQueuedSkill> Queue
        {
            get;
            set;
        }

        #region ISynchronizableWithLocalClock Members
        /// <summary>
        /// Synchronizes the stored times with local clock
        /// </summary>
        /// <param name="drift"></param>
        void ISynchronizableWithLocalClock.SynchronizeWithLocalClock(TimeSpan drift)
        {
            foreach (ISynchronizableWithLocalClock synch in Queue)
            {
                synch.SynchronizeWithLocalClock(drift);
            }
        }
        #endregion
    }


    /// <summary>
    /// Represents a serializable version of a training skill. Used for settings.xml serialization and CCP querying
    /// </summary>
    public sealed class SerializableQueuedSkill : ISynchronizableWithLocalClock
    {
        [XmlAttribute("typeID")]
        public long ID
        {
            get;
            set;
        }

        [XmlAttribute("level")]
        public int Level
        {
            get;
            set;
        }

        [XmlAttribute("startSP")]
        public int StartSP
        {
            get;
            set;
        }

        [XmlAttribute("endSP")]
        public int EndSP
        {
            get;
            set;
        }

        [XmlAttribute("startTime")]
        public string CCPStartTime
        {
            get;
            set;
        }

        [XmlAttribute("endTime")]
        public string CCPEndTime
        {
            get;
            set;
        }

        [XmlIgnore]
        public DateTime StartTime
        {
            get { return CCPStartTime.CCPTimeStringToDateTime(); }
            set { CCPStartTime = value.ToCCPTimeString(); }
        }

        [XmlIgnore]
        public DateTime EndTime
        {
            get { return CCPEndTime.CCPTimeStringToDateTime(); }
            set { CCPEndTime = value.ToCCPTimeString(); }
        }

        #region ISynchronizableWithLocalClock Members
        /// <summary>
        /// Synchronizes the stored times with local clock
        /// </summary>
        /// <param name="drift"></param>
        void ISynchronizableWithLocalClock.SynchronizeWithLocalClock(TimeSpan drift)
        {
            if (!String.IsNullOrEmpty(CCPStartTime)) StartTime -= drift;
            if (!String.IsNullOrEmpty(CCPEndTime)) EndTime -= drift;
        }
        #endregion
    }

}
