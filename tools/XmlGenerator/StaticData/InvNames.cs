using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvNames : IHasID
    {
        [XmlElement("itemID")]
        public int ID { get; set; }

        [XmlElement("itemName")]
        public string Name { get; set; }
    }
}