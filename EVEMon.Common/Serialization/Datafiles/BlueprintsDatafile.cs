using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents our blueprints datafile.
    /// </summary>
    [XmlRoot("blueprints")]
    public sealed class BlueprintsDatafile
    {
        [XmlElement("group")]
        public SerializableBlueprintMarketGroup[] MarketGroups { get; set; }
    }
}