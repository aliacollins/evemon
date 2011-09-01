using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a blueprint in our datafile
    /// </summary>
    public sealed class SerializableBlueprint
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("metaGroup")]
        public ItemMetaGroup MetaGroup { get; set; }

        [XmlAttribute("productTypeID")]
        public short ProduceItemID { get; set; }

        [XmlAttribute("productionTime")]
        public int ProductionTime { get; set; }

        [XmlAttribute("techLevel")]
        public short TechLevel { get; set; }

        [XmlAttribute("researchProductivityTime")]
        public int ResearchProductivityTime { get; set; }

        [XmlAttribute("researchMaterialTime")]
        public int ResearchMaterialTime { get; set; }

        [XmlAttribute("researchCopyTime")]
        public int ResearchCopyTime { get; set; }

        [XmlAttribute("researchTechTime")]
        public int ResearchTechTime { get; set; }

        [XmlAttribute("productivityModifier")]
        public int ProductivityModifier { get; set; }

        [XmlAttribute("wasteFactor")]
        public short WasteFactor { get; set; }

        [XmlAttribute("maxProductionLimit")]
        public int MaxProductionLimit { get; set; }

        [XmlElement("inventTypeID")]
        public int[] InventionTypeID { get; set; }

        [XmlElement("s")]
        public SerializablePrereqSkill[] PrereqSkill { get; set; }

        [XmlElement("m")]
        public SerializableRequiredMaterial[] ReqMaterial { get; set; }
    }
}