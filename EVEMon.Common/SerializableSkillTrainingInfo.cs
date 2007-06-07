using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot("skillTraining")]
    public class SerializableSkillTrainingInfo : ICloneable
    {
        private int m_characterId;

        /// <summary>
        /// This value will always be the value of the characterId of the character your querying unless something went seriously pear shaped
        /// </summary>
        [XmlAttribute("characterID")]
        public int CharacterId
        {
            get { return m_characterId; }
            set { m_characterId = value; }
        }

        private string m_error = string.Empty;

        /// <summary>
        /// If this is non 0 length or not null then an error has occurred
        /// </summary>
        /// <value>"characterID does not belong to you."</value> will be the only value in the entire class other than "characterID"
        /// <value>"You are trying too fast."</value> will be one of three values in the class, the others being "characterID" and "tryAgainIn"
		/// <value>""</value> or <value>null</value> when the operation was successful (will be one of 5 values - "characterID", "TryAgainIn", "currentTimeTQOffset" and "currentTime"
        /// This is the variable that needs checking to see if the thing has worked.
        [XmlElement("error")]
        public string Error
        {
            get { return m_error; }
            set { m_error = value; }
        }

        private int m_timer;
        
        /// <summary>
        /// this has a value of <value>900</value> when it's been successful - this value may change at little or no notice.
        /// </summary>
        [XmlElement("tryAgainIn")]
        public int TimerToNextUpdate
        {
            get { return m_timer; }
            set { m_timer = value; }
        }
        
        private int m_offset;

        // This is actually unrequired for anything useful.
        [XmlElement("currentTimeTQOffset")]
        
        public int Offset
        {
            get { return m_offset; }
            set { m_offset = value; }
        }

        private DateTime m_curTime = DateTime.MinValue;

        [XmlElement("currentTime")]
        public string CurrentTime
        {
            get { return ConvertDateTimeToTimeString(m_curTime); }
            set { m_curTime = ConvertTimeStringToDateTime(value); }
        }

        [XmlIgnore]
        public DateTime GetDateTimeAtUpdate
        {
            get { return m_curTime; }
        }
        /* This does work, now. The missing piece to the puzzle was [XmlText]
         * Currently though, Garthagk has yet to revert back to the format that uses this :(
        private CT m_curTime = new CT();

        [XmlElement("currentTime", typeof(SerializableSkillTrainingInfo.CT))]
        public CT CurrentTime
        {
            get { return m_curTime; }
            set { m_curTime = value; }
        }

        [XmlRoot("currentTime")]
        public class CT
        {
            private int m_offset;

            [XmlAttribute("offset")]
            public int Offset
            {
                get { return m_offset; }
                set { m_offset = value; }
            }

            private DateTime m_curTime = DateTime.MinValue;

            [XmlText]
            public string CurrentTime
            {
                get { return ConvertDateTimeToTimeString(m_curTime); }
                set { m_curTime = ConvertTimeStringToDateTime(value); }
            }

            [XmlIgnore]
            public DateTime GetDateTimeAtUpdate
            {
                get { return m_curTime; }
            }
        }
        */
        private DateTime m_endTime = DateTime.MinValue;

        [XmlElement("trainingEndTime")]
        public string TrainingEndTimeString
        {
            get { return ConvertDateTimeToTimeString(m_endTime); }
            set { m_endTime = ConvertTimeStringToDateTime(value); }
        }

        [XmlIgnore]
        public DateTime getTrainingEndTime
        {
            get { return m_endTime; }
        }

        private DateTime m_startTime = DateTime.MinValue;

        [XmlElement("trainingStartTime")]
        public string TrainingStartTimeString
        {
            get { return ConvertDateTimeToTimeString(m_startTime); }
            set { m_startTime = ConvertTimeStringToDateTime(value); }
        }

        [XmlIgnore]
        public DateTime getTrainingStartTime
        {
            get { return m_startTime; }
        }

        private int m_typeID;

        [XmlElement("trainingTypeID")]
        public int TrainingSkillWithTypeID
        {
            get { return m_typeID; }
            set { m_typeID = value; }
        }

        private int m_startSP;

        [XmlElement("trainingStartSP")]
        public int TrainingSkillStartSP
        {
            get { return m_startSP; }
            set { m_startSP = value; }
        }

        private int m_destSP;

        [XmlElement("trainingDestinationSP")]
        public int TrainingSkillDestinationSP
        {
            get { return m_destSP; }
            set { m_destSP = value; }
        }

        private int m_toLevel;

        [XmlElement("trainingToLevel")]
        public int TrainingSkillToLevel
        {
            get { return m_toLevel; }
            set { m_toLevel = value; }
        }

        public static string ConvertDateTimeToTimeString(DateTime timeUTC)
        {
            // timeUTC  = yyyy-mm-dd hh:mm:ss
            string result = string.Format("{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}",
                        timeUTC.Year,
                        timeUTC.Month,
                        timeUTC.Day,
                        timeUTC.Hour,
                        timeUTC.Minute,
                        timeUTC.Second);
            return result;
        }

        public static DateTime ConvertTimeStringToDateTime(string timeUTC)
        {
            // timeUTC  = yyyy-mm-dd hh:mm:ss
            if (timeUTC == null || timeUTC == "")
                return DateTime.MinValue;
            DateTime dt = new DateTime(
                            Int32.Parse(timeUTC.Substring(0, 4)),
                            Int32.Parse(timeUTC.Substring(5, 2)),
                            Int32.Parse(timeUTC.Substring(8, 2)),
                            Int32.Parse(timeUTC.Substring(11, 2)),
                            Int32.Parse(timeUTC.Substring(14, 2)),
                            Int32.Parse(timeUTC.Substring(17, 2)),
                            0,
                            DateTimeKind.Utc);
            return dt;
        }

        [XmlIgnore]
        public int EstimatedCurrentPoints
        {
            get
            {
                TimeSpan trainingTime = m_endTime - m_startTime;
                double spPerMinute = (m_destSP - m_startSP) / trainingTime.TotalMinutes;
                TimeSpan timeSoFar = DateTime.Now - ((DateTime)m_startTime.Subtract(TimeSpan.FromMilliseconds(m_TQOffset))).ToLocalTime();
                return (m_startSP + (int)(timeSoFar.TotalMinutes * spPerMinute));
            }
        }

        [XmlIgnore]
        public int EstimatedPointsAtUpdate
        {
            get
            {
                TimeSpan trainingTime = m_endTime - m_startTime;
                double spPerMinute = (m_destSP - m_startSP) / trainingTime.TotalMinutes;
                TimeSpan timeSoFar = m_curTime - m_startTime;
                return (m_startSP + (int)(timeSoFar.TotalMinutes * spPerMinute));
            }
        }

        public int EstimatedPointsAtTime(DateTime checkTime)
        {
            TimeSpan trainingTime = m_endTime - m_startTime;
            double spPerMinute = (m_destSP - m_startSP) / trainingTime.TotalMinutes;
            TimeSpan timeSoFar = checkTime - m_startTime.ToLocalTime();
            return (m_startSP + (int)(timeSoFar.TotalMinutes * spPerMinute));
        }

        private double m_TQOffset = 0.0;

        [XmlElement("TQOffset")]
        public double TQOffset
        {
            get { return m_TQOffset; }
            set { m_TQOffset = value; }
        }

        private bool m_PreWarningGiven = false;

        [XmlElement("PreWarningGive")]
        public bool PreWarningGiven
        {
            get { return m_PreWarningGiven; }
            set { m_PreWarningGiven = value; }
        }

        private bool m_AlertRaisedAlready = false;

        [XmlElement("AlertRaisedAlready")]
        public bool AlertRaisedAlready
        {
            get { return m_AlertRaisedAlready; }
            set { m_AlertRaisedAlready = value; }
        }

		[XmlIgnore]
		public bool isSkillInTraining
		{
			get
			{
				return ((Error == "" || Error == null) && m_startTime != DateTime.MinValue);
			}
		}

        public object Clone()
        {
            SerializableSkillTrainingInfo ssti = new SerializableSkillTrainingInfo();
            ssti.m_characterId = this.m_characterId;
            ssti.m_error = (string)this.m_error.Clone();
            ssti.m_timer = this.m_timer;
            ssti.m_offset = this.m_offset;
            ssti.m_curTime = this.m_curTime;
            ssti.m_endTime = this.m_endTime;
            ssti.m_startTime = this.m_startTime;
            ssti.m_typeID = this.m_typeID;
            ssti.m_startSP = this.m_startSP;
            ssti.m_destSP = this.m_destSP;
            ssti.m_toLevel = this.m_toLevel;
            ssti.m_TQOffset = this.m_TQOffset;
            ssti.PreWarningGiven = this.PreWarningGiven;
            ssti.m_AlertRaisedAlready = this.m_AlertRaisedAlready;
            return ssti;
        }

        public void CopyFrom(SerializableSkillTrainingInfo ssti)
        {
            this.m_characterId = ssti.m_characterId;
            this.m_error = (string)ssti.m_error.Clone();
            this.m_timer = ssti.m_timer;
            this.m_offset = ssti.m_offset;
            this.m_curTime = ssti.m_curTime;
            this.m_endTime = ssti.m_endTime;
            this.m_startTime = ssti.m_startTime;
            this.m_typeID = ssti.m_typeID;
            this.m_startSP = ssti.m_startSP;
            this.m_destSP = ssti.m_destSP;
            this.m_toLevel = ssti.m_toLevel;
            this.m_TQOffset = ssti.m_TQOffset;
            this.PreWarningGiven = ssti.PreWarningGiven;
            this.m_AlertRaisedAlready = ssti.m_AlertRaisedAlready;
        }
    }
}
