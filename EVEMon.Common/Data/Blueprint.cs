using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public class Blueprint : Item
    {
        private readonly FastList<int> m_inventBlueprint;
        private readonly FastList<StaticRequiredMaterial> m_materialRequirements;

        #region Constructors

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="group">Market Group the Blueprint will be a member of.</param>
        /// <param name="src">Source serializable blueprint.</param>
        internal Blueprint(BlueprintMarketGroup group, SerializableBlueprint src)
            : base(group, src)
        {
            RunsPerCopy = src.MaxProductionLimit;
            ProducesItem = StaticItems.GetItemByID(src.ProduceItemID);
            ProductionTime = src.ProductionTime;
            ProductivityModifier = src.ProductivityModifier;
            ResearchCopyTime = src.ResearchCopyTime*2;
            ResearchMaterialTime = src.ResearchMaterialTime;
            ResearchProductivityTime = src.ResearchProductivityTime;
            ResearchTechTime = src.ResearchTechTime;
            TechLevel = src.TechLevel;
            WasteFactor = src.WasteFactor;

            // Invented blueprint
            m_inventBlueprint = new FastList<int>(src.InventionTypeID != null ? src.InventionTypeID.Length : 0);
            if (src.InventionTypeID != null)
            {
                foreach (int blueprintID in src.InventionTypeID)
                {
                    m_inventBlueprint.Add(blueprintID);
                }
            }

            // Materials prerequisites
            m_materialRequirements =
                new FastList<StaticRequiredMaterial>(src.ReqMaterial != null ? src.ReqMaterial.Length : 0);
            if (src.ReqMaterial == null)
                return;

            foreach (SerializableRequiredMaterial prereq in src.ReqMaterial)
            {
                m_materialRequirements.Add(new StaticRequiredMaterial(prereq));
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the item this blueprint produces.
        /// </summary>
        public Item ProducesItem { get; private set; }

        /// <summary>
        /// Gets the maximum items this blueprint can produce.
        /// </summary>
        public int RunsPerCopy { get; private set; }

        /// <summary>
        /// Gets the production time of the blueprint.
        /// </summary>
        public double ProductionTime { get; private set; }

        /// <summary>
        /// Gets the production modifier of the blueprint.
        /// </summary>
        public double ProductivityModifier { get; private set; }

        /// <summary>
        /// Gets the copying time of the blueprint.
        /// </summary>
        public double ResearchCopyTime { get; private set; }

        /// <summary>
        /// Gets the material efficiency research time of the blueprint.
        /// </summary>
        public double ResearchMaterialTime { get; private set; }

        /// <summary>
        /// Gets the productivity efficiency research time of the blueprint.
        /// </summary>
        public double ResearchProductivityTime { get; private set; }

        /// <summary>
        /// Gets the invention time of the blueprint.
        /// </summary>
        public double ResearchTechTime { get; private set; }

        /// <summary>
        /// Gets the tech level of the blueprint.
        /// </summary>
        public int TechLevel { get; private set; }

        /// <summary>
        /// Gets the wastage factor of the blueprint.
        /// </summary>
        public short WasteFactor { get; private set; }

        /// <summary>
        /// Gets the collection of materials this blueprint must satisfy to be build.
        /// </summary>
        public IEnumerable<StaticRequiredMaterial> MaterialRequirements
        {
            get { return m_materialRequirements; }
        }

        /// <summary>
        /// Gets the collection of blueprints this object can invent.
        /// </summary>
        public IEnumerable<Blueprint> InventsBlueprint
        {
            get { return m_inventBlueprint.Select(itemID => (StaticBlueprints.GetBlueprintByID(itemID))); }
        }

        #endregion

    }
}
