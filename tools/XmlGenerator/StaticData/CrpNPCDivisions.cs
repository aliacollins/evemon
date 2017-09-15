using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class CrpNPCDivisions : IHasID
    {
        [XmlElement("divisionID")]
        public int ID { get; set; }

        [XmlElement("divisionName")]
        public string DivisionName { get; set; }
    }
}