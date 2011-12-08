using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.FittingTools
{
    public sealed class SerializableFittingDescription
    {
        [XmlAttribute("value")]
        public string Text { get; set; }
    }
}