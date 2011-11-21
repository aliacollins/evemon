using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a blueprint in our datafile
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableBlueprint
    {
        private readonly Collection<int> m_inventTypeID;
        private readonly Collection<SerializablePrereqSkill> m_prereqSkills;
        private readonly Collection<SerializableRequiredMaterial> m_requiredMaterials;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableBlueprint"/> class.
        /// </summary>
        public SerializableBlueprint()
        {
            m_inventTypeID = new Collection<int>();
            m_prereqSkills = new Collection<SerializablePrereqSkill>();
            m_requiredMaterials = new Collection<SerializableRequiredMaterial>();
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        [XmlAttribute("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the meta group.
        /// </summary>
        /// <value>The meta group.</value>
        [XmlAttribute("metaGroup")]
        public ItemMetaGroup MetaGroup { get; set; }

        /// <summary>
        /// Gets or sets the produce item ID.
        /// </summary>
        /// <value>The produce item ID.</value>
        [XmlAttribute("productTypeID")]
        public short ProduceItemID { get; set; }

        /// <summary>
        /// Gets or sets the production time.
        /// </summary>
        /// <value>The production time.</value>
        [XmlAttribute("productionTime")]
        public int ProductionTime { get; set; }

        /// <summary>
        /// Gets or sets the tech level.
        /// </summary>
        /// <value>The tech level.</value>
        [XmlAttribute("techLevel")]
        public short TechLevel { get; set; }

        /// <summary>
        /// Gets or sets the research productivity time.
        /// </summary>
        /// <value>The research productivity time.</value>
        [XmlAttribute("researchProductivityTime")]
        public int ResearchProductivityTime { get; set; }

        /// <summary>
        /// Gets or sets the research material time.
        /// </summary>
        /// <value>The research material time.</value>
        [XmlAttribute("researchMaterialTime")]
        public int ResearchMaterialTime { get; set; }

        /// <summary>
        /// Gets or sets the research copy time.
        /// </summary>
        /// <value>The research copy time.</value>
        [XmlAttribute("researchCopyTime")]
        public int ResearchCopyTime { get; set; }

        /// <summary>
        /// Gets or sets the research tech time.
        /// </summary>
        /// <value>The research tech time.</value>
        [XmlAttribute("researchTechTime")]
        public int ResearchTechTime { get; set; }

        /// <summary>
        /// Gets or sets the productivity modifier.
        /// </summary>
        /// <value>The productivity modifier.</value>
        [XmlAttribute("productivityModifier")]
        public int ProductivityModifier { get; set; }

        /// <summary>
        /// Gets or sets the waste factor.
        /// </summary>
        /// <value>The waste factor.</value>
        [XmlAttribute("wasteFactor")]
        public short WasteFactor { get; set; }

        /// <summary>
        /// Gets or sets the max production limit.
        /// </summary>
        /// <value>The max production limit.</value>
        [XmlAttribute("maxProductionLimit")]
        public int MaxProductionLimit { get; set; }

        /// <summary>
        /// Gets or sets the invention type ID.
        /// </summary>
        /// <value>The invention type ID.</value>
        [XmlElement("inventTypeID")]
        public Collection<int> InventionTypeID
        {
            get { return m_inventTypeID; }
        }

        /// <summary>
        /// Gets the prereq skill.
        /// </summary>
        /// <value>The prereq skill.</value>
        [XmlElement("s")]
        public Collection<SerializablePrereqSkill> PrereqSkill
        {
            get { return m_prereqSkills; }
        }

        /// <summary>
        /// Gets the req material.
        /// </summary>
        /// <value>The req material.</value>
        [XmlElement("m")]
        public Collection<SerializableRequiredMaterial> ReqMaterial
        {
            get { return m_requiredMaterials; }
        }

        /// <summary>
        /// Adds the specified invent type ID.
        /// </summary>
        /// <param name="inventTypeId">The invent type ID.</param>
        public void AddRange(IEnumerable<int> inventTypeId)
        {
            m_inventTypeID.Clear();
            inventTypeId.ToList().ForEach(inventType => m_inventTypeID.Add(inventType));
        }

        /// <summary>
        /// Adds the specified prereq skills.
        /// </summary>
        /// <param name="prereqSkills">The prereq skills.</param>
        public void AddRange(IEnumerable<SerializablePrereqSkill> prereqSkills)
        {
            m_prereqSkills.Clear();
            prereqSkills.ToList().ForEach(prereqSkill => m_prereqSkills.Add(prereqSkill));
        }

        /// <summary>
        /// Adds the specified required materials.
        /// </summary>
        /// <param name="requiredMaterials">The required materials.</param>
        public void AddRange(IEnumerable<SerializableRequiredMaterial> requiredMaterials)
        {
            m_requiredMaterials.Clear();
            requiredMaterials.ToList().ForEach(requiredMaterial => m_requiredMaterials.Add(requiredMaterial));
        }
    }
}