using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents an eve constellation.
    /// </summary>
    public sealed class SerializableConstellation
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

        [XmlElement("systems")]
        public SerializableSolarSystem[] Systems
        {
            get;
            set;
        }
    }
}
