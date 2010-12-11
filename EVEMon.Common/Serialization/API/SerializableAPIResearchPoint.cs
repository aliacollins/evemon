using System;
using System.Xml.Serialization;
using System.Collections.Generic;

using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIResearchList
    {
        public SerializableAPIResearchList()
        {
            this.ResearchPoints = new List<SerializableAPIResearchPoint>();
        }

        [XmlArray("research")]
        [XmlArrayItem("researc")]
        public List<SerializableAPIResearchPoint> ResearchPoints
        {
            get;
            set;
        }
    }

    public sealed class SerializableAPIResearchPoint
    {
        [XmlAttribute("agentID")]
        public int AgentID
        {
            get;
            set;
        }

        [XmlAttribute("skillTypeID")]
        public int SkillID
        {
            get;
            set;
        }

        [XmlAttribute("researchStartDate")]
        public string ResearchStartDateXml
        {
            get { return ResearchStartDate.ToCCPTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    ResearchStartDate = value.CCPTimeStringToDateTime();
            }
        }

        [XmlAttribute("pointsPerDay")]
        public double PointsPerDay
        {
            get;
            set;
        }

        [XmlAttribute("remainderPoints")]
        public float RemainderPoints
        {
            get;
            set;
        }

        /// <summary>
        /// The time this research was started.
        /// </summary>
        [XmlIgnore]
        public DateTime ResearchStartDate
        {
            get;
            set;
        }
    }
}
