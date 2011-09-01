using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a blueprint group in our datafile.
    /// </summary>
    public sealed class SerializableBlueprintMarketGroup
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("blueprint")]
        public SerializableBlueprint[] Blueprints { get; set; }

        [XmlArray("subGroups")]
        [XmlArrayItem("subGroup")]
        public SerializableBlueprintMarketGroup[] SubGroups { get; set; }
    }
}