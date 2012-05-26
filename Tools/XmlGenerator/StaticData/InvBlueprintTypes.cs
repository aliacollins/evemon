using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvBlueprintTypes : IHasID
    {
        [XmlElement("blueprintTypeID")]
        public int ID { get; set; }

        [XmlElement("parentBlueprintTypeID")]
        public int? ParentID { get; set; }

        [XmlElement("productTypeID")]
        public int ProductTypeID { get; set; }

        [XmlElement("productionTime")]
        public int ProductionTime { get; set; }

        [XmlElement("techLevel")]
        public short TechLevel { get; set; }

        [XmlElement("researchProductivityTime")]
        public int ResearchProductivityTime { get; set; }

        [XmlElement("researchMaterialTime")]
        public int ResearchMaterialTime { get; set; }

        [XmlElement("researchCopyTime")]
        public int ResearchCopyTime { get; set; }

        [XmlElement("researchTechTime")]
        public int ResearchTechTime { get; set; }

        [XmlElement("productivityModifier")]
        public int ProductivityModifier { get; set; }

        [XmlElement("wasteFactor")]
        public short WasteFactor { get; set; }

        [XmlElement("maxProductionLimit")]
        public int MaxProductionLimit { get; set; }
    }
}