using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents an eve agent of a station.
    /// </summary>
    public sealed class SerializableAgent
    {
        [XmlAttribute("agentID")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("agentName")]
        public string Name
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

        [XmlAttribute("quality")]
        public int Quality
        {
            get;
            set;
        }
    }
}
