using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class DgmTraits : IHasID
    {
        [XmlElement("traitID")]
        public int ID { get; set; }

        [XmlElement("bonusText")]
        public string BonusText { get; set; }

        [XmlElement("unitID")]
        public short? UnitID { get; set; }

    }
}
