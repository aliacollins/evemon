using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of factional warfare stats. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIFactionalWarfareStats
    {
        [XmlElement("factionID")]
        public int FactionID { get; set; }

        [XmlElement("factionName")]
        public string FactionName { get; set; }

        [XmlElement("enlisted")]
        public string EnlistedDateXml
        {
            get { return EnlistedDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    EnlistedDate = value.TimeStringToDateTime();
            }
        }

        [XmlElement("currentRank")]
        public int CurrentRank { get; set; }

        [XmlElement("highestRank")]
        public int HighestRank { get; set; }

        [XmlElement("killsYesterday")]
        public int KillsYesterday { get; set; }

        [XmlElement("killsLastWeek")]
        public int KillsLastWeek { get; set; }

        [XmlElement("killsTotal")]
        public int KillsTotal { get; set; }

        [XmlElement("victoryPointsYesterday")]
        public int VictoryPointsYesterday { get; set; }

        [XmlElement("victoryPointsLastWeek")]
        public int VictoryPointsLastWeek { get; set; }

        [XmlElement("victoryPointsTotal")]
        public int VictoryPointsTotal { get; set; }

        [XmlIgnore]
        public DateTime EnlistedDate { get; set; }
    }
}
