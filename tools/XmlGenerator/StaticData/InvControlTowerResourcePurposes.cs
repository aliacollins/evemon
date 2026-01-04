using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvControlTowerResourcePurposes : IHasID
    {
        [XmlElement("purpose")]
        public int ID { get; set; }

        [XmlElement("purposeText")]
        public string PurposeName { get; set; }
    }
}