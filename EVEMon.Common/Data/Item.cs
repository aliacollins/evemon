using System;
using System.Collections.Generic;
using System.Text;

using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Base class for ships, items, implants.
    /// </summary>
    public class Item
    {
        protected readonly FastList<StaticSkillLevel> m_prerequisites;
        protected readonly FastList<Material> m_reprocessing;

        #region Constructors

        /// <summary>
        /// Base constructor for default items.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        internal Item(int id, string name)
        {
            ID = id;
            Name = name;
            Description = "No description.";

            m_reprocessing = new FastList<Material>(0);
            m_prerequisites = new FastList<StaticSkillLevel>(0);
        }

        /// <summary>
        /// Base constructor for blueprints.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        internal Item(BlueprintMarketGroup group, SerializableBlueprint src)
            : this(src.ID, src.Name)
        {
            Icon = src.Icon;
            MetaGroup = src.MetaGroup;
            MarketGroup = group;
            Family = ItemFamily.Bpo;

            m_prerequisites = new FastList<StaticSkillLevel>(src.PrereqSkill != null ? src.PrereqSkill.Length : 0);
            if (src.PrereqSkill == null)
                return;

            foreach (SerializablePrereqSkill prereq in src.PrereqSkill)
            {
                m_prerequisites.Add(new StaticSkillLevel(prereq.ID, prereq.Level, prereq.Activity));
            }
        }

        /// <summary>
        /// Base deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        internal Item(MarketGroup group, SerializableItem src)
            : this(src.ID, src.Name)
        {
            MarketGroup = group;
            Icon = src.Icon;
            Race = src.Race;
            FittingSlot = src.Slot;
            Family = src.Family;
            Description = src.Description;

            MetaLevel = src.MetaLevel;
            MetaGroup = src.MetaGroup;

            Properties = new EvePropertyCollection(src.Properties);
            m_reprocessing = new FastList<Material>(0);

            // Skills prerequisites
            m_prerequisites = new FastList<StaticSkillLevel>(src.Prereqs != null ? src.Prereqs.Length : 0);
            if (src.Prereqs == null)
                return;

            foreach (SerializablePrerequisiteSkill prereq in src.Prereqs)
            {
                m_prerequisites.Add(new StaticSkillLevel(prereq.ID, prereq.Level));
            }
        }

        #endregion


        #region Internal Initilizators

        /// <summary>
        /// Initializes the reprocessing informations.
        /// </summary>
        /// <param name="srcMaterials"></param>
        internal void InitializeReprocessing(SerializableMaterialQuantity[] srcMaterials)
        {
            foreach (SerializableMaterialQuantity src in srcMaterials)
            {
                m_reprocessing.Add(new Material(src));
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets this object's ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets this object's icon.
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// Gets this object's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the item's family.
        /// </summary>
        public ItemFamily Family { get; private set; }

        /// <summary>
        /// Gets the race this object is bound to.
        /// </summary>
        public Race Race { get; private set; }

        /// <summary>
        /// Gets this object's description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the metalevel this item belong to.
        /// </summary>
        public int MetaLevel { get; private set; }

        /// <summary>
        /// Gets the metagroup this item belong to.
        /// </summary>
        public ItemMetaGroup MetaGroup { get; private set; }

        /// <summary>
        /// Gets the market group this item belong to.
        /// </summary>
        public MarketGroup MarketGroup { get; private set; }

        /// <summary>
        /// Gets the slot this items fit to.
        /// </summary>
        public ItemSlot FittingSlot { get; private set; }

        /// <summary>
        /// Gets the collection of properties of this object.
        /// </summary>
        public EvePropertyCollection Properties { get; private set; }

        /// <summary>
        /// Gets the collection of skills this object must satisfy to be used.
        /// </summary>
        public IEnumerable<StaticSkillLevel> Prerequisites
        {
            get { return m_prerequisites; }
        }

        /// <summary>
        /// Gets the skill used to reprocess those items.
        /// </summary>
        public StaticSkill ReprocessingSkill
        {
            get
            {
                Nullable<EvePropertyValue> property = Properties[StaticProperties.GetPropertyById(DBConstants.ReprocessingSkillPropertyID)];

                // Returns scrap metal processing by default
                if (property == null)
                    return StaticSkills.GetSkillById(DBConstants.ScrapMetalProcessingSkillID);

                // Returns the reprocessing skill specified by the property
                int id = property.Value.IValue;
                return StaticSkills.GetSkillById(id);
            }
        }

        /// <summary>
        /// Gets the reprocessing materials and their base quantities.
        /// </summary>
        public IEnumerable<Material> ReprocessingMaterials
        {
            get
            {
                StaticItems.EnsureReprocessingInitialized();

                foreach (var material in m_reprocessing)
                {
                    yield return material;
                }

                // Debug : 10 trits and 20 pyerite.
                yield return new Material(new SerializableMaterialQuantity { ID = 34, Quantity = 10 });
                yield return new Material(new SerializableMaterialQuantity { ID = 35, Quantity = 20 });
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        ///   Evaluates whether this item can be activated if the given CPU
        ///   and/or PowerGrid resources are available. Either argument for this
        ///   method can be null, which is interpreted as "no upper limit".
        ///   If the CPU and/or Grid requirements for this item are unknown,
        ///   the method returns false.
        /// </summary>
        /// <param name="cpuAvailable">The upper limit for CPU usage, or null for unlimited.</param>
        /// <param name="gridAvailable">The upper limit for Grid usage or null for unlimited.</param>
        /// <returns>true if both the CPU and powergrid requirements of this
        /// item lie between 0.0d and the given bounds.</returns>
        public bool CanActivate(double? cpuAvailable, double? gridAvailable)
        {
            // There are no limits, so anything fits
            if (cpuAvailable == null && gridAvailable == null)
                return true;

            if (FittingSlot != ItemSlot.Empty)
            {
                // If we have a slot index, we're a fittable item
                // Now see if we can find our usage numbers
                String cpuUsage = FindProperty(EveProperty.CPU, null);
                String gridUsage = FindProperty(EveProperty.Powergrid, null);

                double? cpuRequired = TryParseNullable(TryStripTail(cpuUsage, " tf"));
                double? gridRequired = TryParseNullable(TryStripTail(gridUsage, " MW"));

                if (cpuRequired != null || gridRequired != null)
                {
                    //We have information about this item, see if it fits
                    bool fits = true;
                    if (cpuAvailable != null)
                        fits &= cpuRequired <= cpuAvailable;

                    if (gridAvailable != null)
                        fits &= gridRequired <= gridAvailable;

                    return fits;
                }
            }

            // We lack information about this item, or this item isn't fittable
            // Return false as specced in the method docs
            return false;
        }

        /// <summary>
        /// Gets the category path of this item.
        /// </summary>
        /// <returns></returns>
        public string GetCategoryPath()
        {
            StringBuilder sb = new StringBuilder();
            MarketGroup cat = MarketGroup;

            while (cat != null)
            {
                sb.Insert(0, cat.Name);
                cat = cat.ParentGroup;
                if (cat != null)
                    sb.Insert(0, " > ");
            }
            return sb.ToString();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Tries to strip the given tail from the end of some string.
        /// </summary>
        /// <param name="stripMe">The string to evaluate</param>
        /// <param name="tail">The &quot;tail&quot; to try and remove</param>
        /// <returns>null if stripMe is null, stripMe if tail is null or stripMe doesn't
        /// end in tail, stripMe-with-tail-removed otherwise.</returns>
        private String TryStripTail(String stripMe, String tail)
        {
            if (stripMe == null)
                return null;

            if (tail == null)
                return stripMe;

            if (stripMe.EndsWith(tail))
                return stripMe.Remove(stripMe.Length - tail.Length);

            return stripMe;
        }

        /// <summary>
        /// Try to parse a String as a double. Returns null for any kind of
        /// invalid input.
        /// </summary>
        /// <param name="parseMe">The string to try and parse.</param>
        /// <returns>The string as double, or null if failed to parse.</returns>
        private double? TryParseNullable(String parseMe)
        {
            double? result = null;
            double tempValue;
            if (Double.TryParse(parseMe, out tempValue))
                result = tempValue;

            return result;
        }

        /// <summary>
        /// Searches _properties for a property with the given property name and
        /// returns its value. If the property isn't found, it returns the given
        /// default value. If the property occurs more than once, only the first
        /// occurance is considered.
        /// </summary>
        /// <param name="property">The property name to look for.</param>
        /// <param name="defaultValue">The value to return if the property isn't found.</param>
        /// <returns>Either the value of the named property, or the given default value.</returns>
        private String FindProperty(EveProperty property, String defaultValue)
        {
            String result = defaultValue;
            foreach (EvePropertyValue prop in Properties)
            {
                if (prop.Property != property)
                    continue;

                result = prop.Value;
                break;
            }
            return result;
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>Name of the Item</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

    }
}
