using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot("eveapi")]
    public class SerializableSkillTrainingInfo : ICloneable
    {

        #region public static
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

        #endregion

        #region CCP Xml Elements

        /// <summary>
        /// If this is non 0 length or not null then an error has occurred and will this will be the only value in the entire class other than "current time
        /// </summary>
        /// <value>"Invalid characterID."</value>
        /// <value>"Authentication Failure."</value>
        /// <value>"Cached API key authentication failure"</value>
        /// <value>"Character does not belong to account"</value>
        /// <value>""</value> or <value>null</value> when the operation was successful (will be one of 5 values - "characterID", "TryAgainIn", "currentTimeTQOffset" and "currentTime"
        private CCPApiError m_ApiError = new CCPApiError();

        [XmlElement("error")]
        public CCPApiError APIError
        {
            get { return m_ApiError; }
            set { m_ApiError = value; }
        }

        private DateTime m_curTime = DateTime.MinValue;

        [XmlElement("currentTime")]
        public string CurrentTime
        {
            get { return ConvertDateTimeToTimeString(m_curTime); }
            set 
            { 
                m_curTime = ConvertTimeStringToDateTime(value);
            }
        }

        private SerializableSkillTrainingInfo.ApiResults m_results = new SerializableSkillTrainingInfo.ApiResults();
        [XmlElement("result")]

        public SerializableSkillTrainingInfo.ApiResults TrainingResult
        {
            get { return m_results; }
            set { m_results = value; }
        }


        [XmlElement("cachedUntil")]
        public string CachedUntilTime
        {
            get { return ConvertDateTimeToTimeString(m_cachedUntilTime); }
            set { m_cachedUntilTime = ConvertTimeStringToDateTime(value); }
        }
        #endregion

        #region EVEMon added elements

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

        #endregion

        #region non-serialised public fields

        [XmlIgnore]
        public int TrainingSkillWithTypeID
        {
            get { return m_results.TrainingSkillWithTypeID; }
            set { m_results.TrainingSkillWithTypeID = value; }
        }

        [XmlIgnore]

        public int TrainingSkillStartSP
        {
            get { return m_results.TrainingSkillStartSP; }
            set { m_results.TrainingSkillStartSP = value; }
        }

        [XmlIgnore]
        public int TrainingSkillDestinationSP
        {
            get { return m_results.TrainingSkillDestinationSP; }
            set { m_results.TrainingSkillDestinationSP = value; }
        }

        [XmlIgnore]
        public int TrainingSkillToLevel
        {
            get { return m_results.TrainingSkillToLevel; }
            set { m_results.TrainingSkillToLevel = value; }
        }

        [XmlIgnore]
        public DateTime GetDateTimeAtUpdate
        {
            get { return m_curTime; }
        }

        [XmlIgnore]
        public DateTime getTrainingStartTime
        {
            get { return m_results.TrainingStartTime; }
        }

        
        [XmlIgnore]
        public DateTime getTrainingEndTime
        {
            get { return m_results.TrainingEndTime.ToUniversalTime(); }
        }

        [XmlIgnore]
        public TimeSpan TrainingTime
        {
            get { return m_results.TrainingEndTime - m_results.TrainingStartTime; }
        }

        [XmlIgnore]
        public double SpPerMinute
        {

            get { return (m_results.TrainingSkillDestinationSP - m_results.TrainingSkillStartSP) / TrainingTime.TotalMinutes; }
        }

        [XmlIgnore]
        public int EstimatedCurrentPoints
        {
            get
            {
                TimeSpan timeSoFar = DateTime.Now - m_results.TrainingStartTime.ToLocalTime();
                int points = m_results.TrainingSkillStartSP + (int)(timeSoFar.TotalMinutes * SpPerMinute);
                return (points < m_results.TrainingSkillDestinationSP) ? points : m_results.TrainingSkillDestinationSP;
            }
        }

        [XmlIgnore]
        public int EstimatedPointsAtUpdate
        {
            get
            {
                TimeSpan timeSoFar = m_curTime - m_results.TrainingStartTime;
                return (m_results.TrainingSkillStartSP + (int)(timeSoFar.TotalMinutes * SpPerMinute));
            }
        }

        [XmlIgnore]
        /// Gets the number of seconds until the next update is available
        public int TimerToNextUpdate
        {
            get
            {
                if (APIError.ErrorCode == 0)
                {
                    TimeSpan t = m_cachedUntilTime - m_curTime;
                    return Convert.ToInt32(t.TotalSeconds);
                }
                else return 0;
            }
        }

        [XmlIgnore]
        public bool isSkillInTraining
        {
            // Be careful with this - skill finish time could be in the past!
            get
            {
                if (APIError.ErrorCode != 0) return false;
                return (m_results.SkillInTraining == 1);
            }
        }


        #endregion

        #region public methods

        /// <summary>
        /// Fixup the currentTime and cachedUntil time to match the user's clock.
        /// This should ONLY be called when the xml is first recieved from CCP
        /// </summary>
        /// <param name="millisecondsDrift"></param>
        public void FixServerTimes(double millisecondsDrift)
        {
            TimeSpan drift = new TimeSpan(0,0,0,0,Convert.ToInt32(millisecondsDrift));
            m_curTime.Subtract(drift);
            // and do the same for TQ
            m_results.FixTQTimes(millisecondsDrift,m_curTime);
        }

        public int EstimatedPointsAtTime(DateTime checkTime)
        {
            TimeSpan timeSoFar = checkTime - m_results.TrainingStartTime.ToLocalTime();
            return (m_results.TrainingSkillStartSP + (int)(timeSoFar.TotalMinutes * SpPerMinute));
        }

        private DateTime m_cachedUntilTime = DateTime.MinValue;

        public object Clone()
        {
            SerializableSkillTrainingInfo ssti = new SerializableSkillTrainingInfo();
            ssti.APIError.ErrorCode = this.APIError.ErrorCode;
            ssti.APIError.ErrorMessage  = (string)this.APIError.ErrorMessage.Clone();
            ssti.m_curTime = this.m_curTime;
            ssti.m_results = (SerializableSkillTrainingInfo.ApiResults)m_results.Clone();
            ssti.m_cachedUntilTime = this.m_cachedUntilTime;
            ssti.PreWarningGiven = this.PreWarningGiven;
            ssti.m_AlertRaisedAlready = this.m_AlertRaisedAlready;
            return ssti;
        }

        public void CopyFrom(SerializableSkillTrainingInfo ssti)
        {
            this.APIError.ErrorCode = ssti.APIError.ErrorCode;
            this.APIError.ErrorMessage = (string)ssti.APIError.ErrorMessage.Clone();
            this.m_curTime = ssti.m_curTime;
            this.m_results = (SerializableSkillTrainingInfo.ApiResults)ssti.m_results.Clone();
            this.PreWarningGiven = ssti.PreWarningGiven;
            this.m_AlertRaisedAlready = ssti.m_AlertRaisedAlready;
        }
        #endregion

        #region Results class
        [XmlRoot("result")]
        public class ApiResults : ICloneable
        {
            private DateTime m_curTQTime = DateTime.MinValue;

            [XmlElement("currentTQTime")]
            public string CurrentTQTime
            {
                get { return ConvertDateTimeToTimeString(m_curTQTime); }
                set
                {
                    m_curTQTime = ConvertTimeStringToDateTime(value);
                }
            }

            /// <summary>
            /// Bring the times into the current PC's clock timeframe
            /// Should ONLY be called at the time we get XML from CCP
            /// </summary>
            /// <param name="offset">Difference between CCP "Current time" and PC clock at the time the xml was received</param>
            /// <param name="currentTime">CCP Webserver time at the time the xml was received</param>
            public void FixTQTimes(double millisecondsDrift, DateTime currentTime)
            {
                TimeSpan TQdrift = currentTime - m_curTQTime;
                TimeSpan drift = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(millisecondsDrift));
                if (m_startTime != DateTime.MinValue)
                {
                    m_startTime.Subtract(drift);
                    m_startTime.Add(TQdrift);
                }
                if (m_endTime != DateTime.MinValue)
                {
                    m_endTime.Subtract(drift);
                    m_endTime.Add(TQdrift);
                }
            }

            private  DateTime m_endTime = DateTime.MinValue;

            [XmlElement("trainingEndTime")]
            public string TrainingEndTimeString
            {
                get { return SerializableSkillTrainingInfo.ConvertDateTimeToTimeString(m_endTime); }
                set { m_endTime = SerializableSkillTrainingInfo.ConvertTimeStringToDateTime(value); }
            }

            [XmlIgnore]
            public DateTime TrainingEndTime
            {
                get { return m_endTime; }
            }

            private DateTime m_startTime = DateTime.MinValue;

            [XmlElement("trainingStartTime")]
            public string TrainingStartTimeString
            {
                get { return SerializableSkillTrainingInfo.ConvertDateTimeToTimeString(m_startTime); }
                set { m_startTime = SerializableSkillTrainingInfo.ConvertTimeStringToDateTime(value); }
            }

            [XmlIgnore]
            public DateTime TrainingStartTime
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
            private int m_inTraining;

            [XmlElement("skillInTraining")]
            public int SkillInTraining
            {
                get { return m_inTraining; }
                set { m_inTraining = value; }
            }


            public Object Clone()
            {
                SerializableSkillTrainingInfo.ApiResults ssti = new SerializableSkillTrainingInfo.ApiResults();
                ssti.m_typeID = this.m_typeID;
                ssti.m_endTime = this.m_endTime;
                ssti.m_startTime = this.m_startTime;

                ssti.m_startSP = this.m_startSP;
                ssti.m_destSP = this.m_destSP;
                ssti.m_toLevel = this.m_toLevel;
                ssti.m_inTraining = this.m_inTraining;
                return ssti;

            }
            public void CopyFrom(SerializableSkillTrainingInfo.ApiResults ssti)
            {
                this.m_startTime = ssti.m_startTime;
                this.m_endTime = ssti.m_endTime;
                this.m_typeID = ssti.m_typeID;
                this.m_startSP = ssti.m_startSP;
                this.m_destSP = ssti.m_destSP;
                this.m_toLevel = ssti.m_toLevel;
                this.m_inTraining =ssti.m_inTraining;
            }
        }

        #endregion

    }
}
