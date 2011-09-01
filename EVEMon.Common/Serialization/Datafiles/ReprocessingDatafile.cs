using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    [XmlRoot("reprocessingDatafile")]
    public sealed class ReprocessingDatafile
    {
        [XmlElement("item")]
        public SerializableItemMaterials[] Items { get; set; }
    }
}