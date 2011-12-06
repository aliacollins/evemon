using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.FittingTools
{
    [XmlRoot("fittings")]
    public sealed class SerializableFittings
    {
        [XmlElement("fitting")]
        public SerializableFitting Fitting { get; set; }
    }
}
