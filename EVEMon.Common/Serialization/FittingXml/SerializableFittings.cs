using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.FittingXml
{
    [XmlRoot("fittings")]
    public sealed class SerializableFittings
    {
        [XmlElement("fitting")]
        public SerializableFitting Fitting { get; set; }
    }
}
