using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a plan entry
    /// </summary>
    public sealed class SerializableInvalidPlanEntry
    {
        [XmlAttribute("skill")]
        public string SkillName { get; set; }

        [XmlAttribute("level")]
        public int PlannedLevel { get; set; }

        [XmlAttribute("acknowledged")]
        public bool Acknowledged { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SerializableInvalidPlanEntry Clone()
        {
            return (SerializableInvalidPlanEntry)MemberwiseClone();
        }
    }
}