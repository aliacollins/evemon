using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.FittingXml
{
    public sealed class SerializableFittingDescription
    {
        [XmlAttribute("value")]
        public string Text { get; set; }
    }
}