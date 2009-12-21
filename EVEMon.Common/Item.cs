using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Text;

namespace EVEMon.Common
{
    public class Item : EveObject
    {
        private ItemCategory m_category;

        [XmlIgnore]
        public ItemCategory ParentCategory
        {
            get { return m_category; }
            set { m_category = value; }
        }


        private string m_metagroup = String.Empty;

        [XmlAttribute]
        public string Metagroup
        {
            get { return m_metagroup; }
            set { m_metagroup = String.Intern(value); }
        }

        private int m_slotIndex = -1;
        public int SlotIndex
        {
            get
            {
                if (m_slotIndex != -1)
                    return m_slotIndex;
                foreach (EntityProperty prop in _properties)
                {
                    if (prop.Name != "Slot type")
                        continue;
                    switch (prop.Value)
                    {
                        default:
                            m_slotIndex = 0; // FIXME: debug warning?
                            break;
                        case "High slot":
                            m_slotIndex = 1;
                            break;
                        case "Mid slot":
                            m_slotIndex = 2;
                            break;
                        case "Low slot":
                            m_slotIndex = 3;
                            break;
                    }
                    return m_slotIndex;
                }
                m_slotIndex = 0; // No slot
                return m_slotIndex;
            }
        }

        private int m_techLevel = -1;
        public int TechLevel
        {
            get
            {
                if (m_techLevel == -1)
                {
                    foreach (EntityProperty prop in _properties)
                    {
                        if (prop.Name != "Tech Level") continue;

                        switch (prop.Value)
                        {
                            case "1":
                                m_techLevel = 1;
                                break;
                            case "2":
                                m_techLevel = 2;
                                break;
                            case "3":
                                m_techLevel = 3;
                                break;
                            default:
                                m_techLevel = 0; // FIXME: debug warning?
                                break;
                        }

                        return m_techLevel;
                    }
                    m_techLevel = 0; // No tech level
                }
                return m_techLevel;

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
        public bool canActivate(double? cpuAvailable, double? gridAvailable)
        {
            if (cpuAvailable == null && gridAvailable == null)
            {
                //Shortcut. There are no limits, so anything fits.
                return true;
            }
            if (this.SlotIndex > 0)
            {
                //If we have a slot index, we're a fittable item. Now see if we can find
                //our usage numbers.

                String cpuUsage = findProperty("CPU usage", null);
                String gridUsage = findProperty("powergrid usage", null);

                double? cpuRequired = tryParseNullable(tryStripTail(cpuUsage, " tf"));
                double? gridRequired = tryParseNullable(tryStripTail(gridUsage, " MW"));

                if (cpuRequired != null || gridRequired != null)
                {
                    //We have information about this item, see if it fits
                    bool fits = true;
                    if (cpuAvailable != null)
                    {
                        fits = fits && cpuRequired <= cpuAvailable;
                    }
                    if (gridAvailable != null)
                    {
                        fits = fits && gridRequired <= gridAvailable;
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
        private static String tryStripTail(String stripMe, String tail)
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
        private static double? tryParseNullable(String parseMe)
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
        /// <param name="propertyName">The property name to look for.</param>
        /// <param name="defaultValue">The value to return if the property isn't found.</param>
        /// <returns>Either the value of the named property, or the given default value.</returns>
        private String findProperty(String propertyName, String defaultValue)
        {
            String result = defaultValue;
            foreach (EntityProperty prop in _properties)
            {
                if (prop.Name != propertyName)
                    continue;
                result = prop.Value;
                break;
            }
            return result;
        }

        public override string GetCategoryPath()
        {
            StringBuilder sb = new StringBuilder();
            ItemCategory cat = this.ParentCategory;
            while (cat.Name != "Ship Items")
            {
                sb.Insert(0, cat.Name);
                cat = cat.ParentCategory;
                if (cat.Name != "Ship Items")
                {
                    sb.Insert(0, " > ");
                }
            }
            return sb.ToString();
        }
    }

    public class ItemCategory
    {
        private static WeakReference<ItemCategory> m_rootCategory;

        public static ItemCategory GetRootCategory()
        {
            ItemCategory rootCat = null;
            if (m_rootCategory != null)
            {
                rootCat = m_rootCategory.Target;
            }
            if (rootCat == null)
            {
                string itemfile = Settings.FindDatafile("eve-items2.xml.gz");
                if (!File.Exists(itemfile))
                {
                    throw new ApplicationException(itemfile + " not found!");
                }
                using (FileStream s = new FileStream(itemfile, FileMode.Open, FileAccess.Read))
                using (GZipStream zs = new GZipStream(s, CompressionMode.Decompress))
                {
                    XmlSerializer xs = new XmlSerializer(typeof (ItemCategory));
                    rootCat = xs.Deserialize(zs) as ItemCategory;
                }

                m_rootCategory = new WeakReference<ItemCategory>(rootCat);
            }
            return rootCat;
        }

        public ItemCategory()
        {
            m_subCategories = new MonitoredList<ItemCategory>();
            m_subCategories.Changed += new EventHandler<ChangedEventArgs<ItemCategory>>(m_subCategories_Changed);

            m_items = new MonitoredList<Item>();
            m_items.Changed += new EventHandler<ChangedEventArgs<Item>>(m_items_Changed);
        }

        private void m_items_Changed(object sender, ChangedEventArgs<Item> e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Added:
                    e.Item.ParentCategory = this;
                    break;
                case ChangeType.Removed:
                    e.Item.ParentCategory = null;
                    break;
            }
        }

        private void m_subCategories_Changed(object sender, ChangedEventArgs<ItemCategory> e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Added:
                    e.Item.ParentCategory = this;
                    break;
                case ChangeType.Removed:
                    e.Item.ParentCategory = null;
                    break;
            }
        }

        private ItemCategory m_parentCategory;

        [XmlIgnore]
        public ItemCategory ParentCategory
        {
            get { return m_parentCategory; }
            set { m_parentCategory = value; }
        }

        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private MonitoredList<ItemCategory> m_subCategories;

        public MonitoredList<ItemCategory> Subcategories
        {
            get { return m_subCategories; }
        }

        private MonitoredList<Item> m_items;

        public MonitoredList<Item> Items
        {
            get { return m_items; }
        }

        /// <summary>
        /// Recursively searches the root category and all underlying categories for the first item with a 
        /// name that exactly matches the given itemName.
        /// </summary>
        /// <param name="itemName">The name of the item to find.</param>
        /// <returns>The first item which name matches itemName, Null if no such item is found.</returns>
        public static Item findItem(string itemName)
        {
            return searchCategory(ItemCategory.GetRootCategory(), itemName);
        }

        /// <summary>
        /// Recursively searches the root category and all underlying categories for the first item with an 
        /// Id matching the given itemId.
        /// </summary>
        /// <param name="itemId">The id of the item to find.</param>
        /// <returns>The first item which id matches itemId, Null if no such item is found.</returns>
        public static Item findItem(int itemId)
        {
            ItemCategory ic = ItemCategory.GetRootCategory();
            return searchCategory(ic, itemId);
        }

        /// <summary>
        /// Recursively searches the ItemCategory ic and all underlying categories for the first item with a
        /// name that exactly matches the given itemName.
        /// </summary>
        /// <param name="ic">The ItemCategory to search in.</param>
        /// <param name="itemName">The name of the item to find.</param>
        /// <returns>The first item which name matches itemName, Null if no such item is found.</returns>
        private static Item searchCategory(ItemCategory ic, string itemName)
        {
            Item item = null;
            foreach (Item anItem in ic.Items)
            {
                if (anItem.Name == itemName)
                {
                    item = anItem;
                    break;
                }
            }
           if (item != null) return item;

            foreach(ItemCategory sc in ic.Subcategories)
            {
                item = searchCategory(sc, itemName);
                if (item != null) return item;
            }

            return null;
        }

        /// <summary>
        /// Recursively searches the ItemCategory ic and all underlying categories for the first item with an
        /// id matching the given itemId.
        /// </summary>
        /// <param name="ic">The ItemCategory to search in.</param>
        /// <param name="itemId">The item id to search for.</param>
        /// <returns>The first item with id matching itemId, Null if no such item is found.</returns>
        private static Item searchCategory(ItemCategory ic, int itemId)
        {
            foreach (Item i in ic.Items)
            {
                if (i.Id == itemId) return i;
            }

            foreach (ItemCategory subCat in ic.Subcategories)
            {
                Item si = searchCategory(subCat, itemId);
                if (si != null) return si;
            }

            return null;
        }

    }
}
