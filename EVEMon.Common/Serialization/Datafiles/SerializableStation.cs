using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents an eve station.
    /// </summary>
    public sealed class SerializableStation
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("corporationID")]
        public int CorporationID
        {
            get;
            set;
        }

        [XmlAttribute("corporationName")]
        public string CorporationName
        {
            get;
            set;
        }

        [XmlElement("reprocessingEfficiency")]
        public float ReprocessingEfficiency
        {
            get;
            set;
        }

        [XmlElement("reprocessingStationsTake")]
        public float ReprocessingStationsTake
        {
            get;
            set;
        }

        [XmlArray("agents")]
        [XmlArrayItem("agent")]
        public SerializableAgent[] Agents
        {
            get;
            set;
        }
    }
}
