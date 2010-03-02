using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Collections;
using System.Text;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Base class for ships, items, implants.
    /// </summary>
    public class Item
    {
        protected readonly int m_id;
        protected readonly int m_metaLevel;

        protected readonly string m_name;
        protected readonly string m_icon;
        protected readonly string m_description;

        protected readonly Race m_race;
        protected readonly ItemSlot m_slot;
        protected readonly ItemFamily m_family;
        protected readonly MarketGroup m_marketGroup;
        protected readonly ItemMetaGroup m_metaGroup;
        protected readonly PropertiesCollection m_properties;
        protected readonly FastList<StaticSkillLevel> m_prerequisites;
        protected readonly FastList<Material> m_reprocessing;

        /// <summary>
        /// Base constructor for default items
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        internal Item(int id, string name)
        {
            m_id = id;
            m_name = name;
            m_description = "No description.";

            m_reprocessing = new FastList<Material>(0);
            m_prerequisites = new FastList<StaticSkillLevel>(0);
        }


        /// <summary>
        /// Base deserialization constructor
        /// </summary>
        /// <param name="src"></param>
        internal Item(MarketGroup group, SerializableItem src)
        {
            m_marketGroup = group;

            m_id = src.ID;
            m_name = src.Name;
            m_icon = src.Icon;
            m_race = src.Race;
            m_slot = src.Slot;
            m_family = src.Family;
            m_description = src.Description;

            m_metaLevel = src.MetaLevel;
            m_metaGroup = src.MetaGroup;

            m_reprocessing = new FastList<Material>(0);
            m_properties = new PropertiesCollection(src.Properties);

            // Skills prerequisites
            m_prerequisites = new FastList<StaticSkillLevel>((src.Prereqs != null ? src.Prereqs.Length : 0));
            if (src.Prereqs == null) return;

            foreach (var prereq in src.Prereqs)
            {
                m_prerequisites.Add(new StaticSkillLevel(prereq.ID, prereq.Level));
            }
        }

        /// <summary>
        /// Initializes the reprocessing informations.
        /// </summary>
        /// <param name="srcMaterials"></param>
        internal void InitializeReprocessing(SerializableMaterialQuantity[] srcMaterials)
        {
            foreach (var src in srcMaterials)
            {
                m_reprocessing.Add(new Material(src));
            }
        }

        /// <summary>
        /// Gets this object's ID
        /// </summary>
        public int ID
        {
            get { return m_id; }
        }

        /// <summary>
        /// Gets this object's icon
        /// </summary>
        public string Icon
        {
            get { return m_icon; }
        }

        /// <summary>
        /// Gets this object's name
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the item's family.
        /// </summary>
        public ItemFamily Family
        {
            get { return m_family; }
        }

        /// <summary>
        /// Gets the race this object is bound to.
        /// </summary>
        public Race Race
        {
            get { return m_race; }
        }

        /// <summary>
        /// Gets this object's description
        /// </summary>
        public string Description
        {
            get { return m_description; }
        }

        /// <summary>
        /// Gets the metagroup this item belong to
        /// </summary>
        public ItemMetaGroup MetaGroup
        {
            get { return m_metaGroup; }
        }

        /// <summary>
        /// Gets the market group this item belong to
        /// </summary>
        public MarketGroup MarketGroup
        {
            get { return m_marketGroup; }
        }

        /// <summary>
        /// Gets the slot this items fit to.
        /// </summary>
        public ItemSlot FittingSlot
        {
            get { return m_slot; }
        }

        /// <summary>
        /// Gets the collection of skills this object must satisfy to be used.
        /// </summary>
        public IEnumerable<StaticSkillLevel> Prerequisites
        {
            get { return m_prerequisites; }
        }

        /// <summary>
        /// Gets the collection of properties of this object
        /// </summary>
        public PropertiesCollection Properties
        {
            get { return m_properties; }
        }

        /// <summary>
        /// Gets the skill used to reprocess those items.
        /// </summary>
        public StaticSkill ReprocessingSkill
        {
            get
            {
                var property = m_properties[StaticProperties.GetPropertyById(DBConstants.ReprocessingSkillPropertyID)];

                // Returns scrap metal processing by default.
                if (property == null) return StaticSkills.GetSkillById(DBConstants.ScrapMetalProcessingSkillID);

                // Returns the reprocessing skill specified by the property.
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
            if (cpuAvailable == null && gridAvailable == null)
            {
                //Shortcut. There are no limits, so anything fits.
                return true;
            }

            if (this.FittingSlot != ItemSlot.Empty)
            {
                // If we have a slot index, we're a fittable item. Now see if we can find
                // our usage numbers.

                String cpuUsage = FindProperty(EveProperty.CPU, null);
                String gridUsage = FindProperty(EveProperty.Powergrid, null);

                double? cpuRequired = TryParseNullable(TryStripTail(cpuUsage, " tf"));
                double? gridRequired = TryParseNullable(TryStripTail(gridUsage, " MW"));

                if (cpuRequired != null || gridRequired != null)
                {
                    //We have information about this item, see if it fits
                    bool fits = true;
                    if (cpuAvailable != null)
                    {
                        fits &= cpuRequired <= cpuAvailable;
                    }
                    if (gridAvailable != null)
                    {
                        fits &= gridRequired <= gridAvailable;
                    }
                    return fits;
                }
            }
            //We lack information about this item, or this item isn't fittable. 
            //Return false as specced in the method docs.
            return false;
        }

        /// <summary>
        /// Tries to strip the given tail from the end of some string.
        /// </summary>
        /// <param name="stripMe">The string to evaluate</param>
        /// <param name="tail">The &quot;tail&quot; to try and remove</param>
        /// <returns>null if stripMe is null, stripMe if tail is null or stripMe doesn't
        /// end in tail, stripMe-with-tail-removed otherwise.</returns>
        private static String TryStripTail(String stripMe, String tail)
        {
            if (stripMe == null) return null;
            if (tail == null) return stripMe;
            if (stripMe.EndsWith(tail)) return stripMe.Remove(stripMe.Length - tail.Length);
            return stripMe;
        }

        /// <summary>
        /// Try to parse a String as a double. Returns null for any kind of
        /// invalid input.
        /// </summary>
        /// <param name="parseMe">The string to try and parse.</param>
        /// <returns>The string as double, or null if failed to parse.</returns>
        private static double? TryParseNullable(String parseMe)
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
            foreach (EvePropertyValue prop in m_properties)
            {
                if (prop.Property != property) continue;

                result = prop.Value;
                break;
            }
            return result;
        }

        /// <summary>
        /// Gets the category path of this item
        /// </summary>
        /// <returns></returns>
        public string GetCategoryPath()
        {
            StringBuilder sb = new StringBuilder();
            MarketGroup cat = m_marketGroup;

            while (cat != null)
            {
                sb.Insert(0, cat.Name);
                cat = cat.ParentGroup;
                if (cat != null)
                {
                    sb.Insert(0, " > ");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }
    }



    #region ItemCollection
    /// <summary>
    /// Represents a read-only collection of items
    /// </summary>
    public sealed class ItemCollection : ReadonlyCollection<Item>
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="src"></param>
        internal ItemCollection(MarketGroup group, SerializableItem[] src)
            : base(src == null ? 0 : src.Length)
        {
            if (src == null) return;

            foreach (var item in src)
            {
                switch (item.Family)
                {
                    default:
                        m_items.Add(new Item(group, item));
                        break;
                    case ItemFamily.Implant:
                        m_items.Add(new Implant(group, item));
                        break;
                    case ItemFamily.Ship:
                        m_items.Add(new Ship(group, item));
                        break;
                }
            }
        }
    }
    #endregion
}
