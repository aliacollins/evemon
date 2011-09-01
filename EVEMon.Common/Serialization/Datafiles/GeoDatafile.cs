using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents out eve geography datafile.
    /// </summary>
    [XmlRoot("geographyDatafile")]
    public sealed class GeoDatafile
    {
        [XmlArray("regions")]
        [XmlArrayItem("region")]
        public SerializableRegion[] Regions { get; set; }

        [XmlArray("jumps")]
        [XmlArrayItem("jump")]
        public SerializableJump[] Jumps { get; set; }
    }
}