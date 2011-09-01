using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents an items category (standard item categories, not market groups) from our datafile.
    /// </summary>
    public sealed class SerializableMarketGroup
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public SerializableItem[] Items { get; set; }

        [XmlArray("marketGroups")]
        [XmlArrayItem("marketGroup")]
        public SerializableMarketGroup[] SubGroups { get; set; }
    }
}