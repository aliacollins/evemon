using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

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
            set { m_metagroup = StringTable.GetSharedString(value); }
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
                string itemfile = String.Format(
                    "{1}Resources{0}eve-items2.xml.gz",
                    Path.DirectorySeparatorChar,
                    System.AppDomain.CurrentDomain.BaseDirectory);
                if (!File.Exists(itemfile))
                {
                    throw new ApplicationException(itemfile + " not found!");
                }
                using (FileStream s = new FileStream(itemfile, FileMode.Open, FileAccess.Read))
                using (GZipStream zs = new GZipStream(s, CompressionMode.Decompress))
                {
                    using (StringTable.GetInstanceScope())
                    {
                        XmlSerializer xs = new XmlSerializer(typeof (ItemCategory));
                        rootCat = xs.Deserialize(zs) as ItemCategory;
                    }
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
    }
}
