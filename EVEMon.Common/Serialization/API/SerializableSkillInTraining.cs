using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// This class represents the skill in training of a character.
    /// </summary>
    public sealed class SerializableSkillInTraining : ISynchronizableWithLocalClock
    {
        /// <summary>
        /// Default constructor for XML serializer
        /// </summary>
        public SerializableSkillInTraining()
        {
        }

        [XmlElement("currentTQTime")]
        public string CurrentTQTime
        {
            get;
            set;
        }

        [XmlElement("trainingEndTime")]
        public string TrainingEndTime
        {
            get;
            set;
        }

        [XmlElement("trainingStartTime")]
        public string TrainingStartTime
        {
            get;
            set;
        }

        [XmlElement("trainingTypeID")]
        public short TrainingTypeID
        {
            get;
            set;
        }

        [XmlElement("trainingStartSP")]
        public int TrainingStartSP
        {
            get;
            set;
        }

        [XmlElement("trainingDestinationSP")]
        public int TrainingDestinationSP
        {
            get;
            set;
        }

        [XmlElement("trainingToLevel")]
        public byte TrainingToLevel
        {
            get;
            set;
        }

        [XmlElement("skillInTraining")]
        public byte SkillInTraining
        {
            get;
            set;
        }

        [XmlIgnore]
        public DateTime CurrentServerTime
        {
            get { return CurrentTQTime.CCPTimeStringToDateTime(); }
            set { CurrentTQTime = value.ToCCPTimeString(); }
        }

        [XmlIgnore]
        public DateTime StartTime
        {
            get { return TrainingStartTime.CCPTimeStringToDateTime(); }
            set { TrainingStartTime = value.ToCCPTimeString(); }
        }

        [XmlIgnore]
        public DateTime EndTime
        {
            get { return TrainingEndTime.CCPTimeStringToDateTime(); }
            set { TrainingEndTime = value.ToCCPTimeString(); }
        }

        #region ISynchronizableWithLocalClock Members

        /// <summary>
        /// Synchronizes the stored times with local clock
        /// </summary>
        /// <param name="drift"></param>
        void ISynchronizableWithLocalClock.SynchronizeWithLocalClock(TimeSpan drift)
        {
            if (!String.IsNullOrEmpty(TrainingStartTime)) StartTime -= drift;
            if (!String.IsNullOrEmpty(TrainingEndTime)) EndTime -= drift;
        }

        #endregion
    }
}
