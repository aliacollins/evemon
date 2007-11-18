using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.IO;
using System.Text.RegularExpressions;

namespace EVEMon.SkillPlanner
{
    public partial class ItemBrowserControl : EveObjectBrowserSimple
    {
        public ItemBrowserControl()
        {
            InitializeComponent();
            this.scObjectBrowser.RememberDistanceKey = "ItemBrowser";
            this.ObjectSelectControl = this.itemSelectControl;
        }

        protected override void DisplayItemDetails(EveObject eveitem)
        {
            base.DisplayItemDetails(eveitem);
            Item item = (Item)eveitem;
            if (item != null)
            {
                lvItemProperties.BeginUpdate();
                try
                {
                    // remove excess columns that might have been added by 'compare with' earlier
                    while (lvItemProperties.Columns.Count > 2)
                        lvItemProperties.Columns.RemoveAt(2);
                    // (re)construct item properties list
                    lvItemProperties.Items.Clear();

                    ListViewItem listItem = new ListViewItem(new string[] { "Class", item.Metagroup });
                    listItem.Name = "Class";
                    lvItemProperties.Items.Add(listItem);

                    foreach (EntityProperty prop in item.Properties)
                    {
                        listItem = new ListViewItem(new string[] { prop.Name, prop.Value });
                        listItem.Name = prop.Name;
                        lvItemProperties.Items.Add(listItem);
                    }


                    // Add compare with columns (if additional selections)
                    for (int i_item = 0; i_item < itemSelectControl.SelectedObjects.Count; i_item++)
                    {
                        Item selectedItem = itemSelectControl.SelectedObjects[i_item] as Item;
                        // Skip if it's the base item or not an item
                        if (selectedItem == itemSelectControl.SelectedObject || selectedItem == null)
                            continue;

                        // add new column header and values
                        lvItemProperties.Columns.Add(selectedItem.Name);
                        foreach (EntityProperty prop in selectedItem.Properties)
                        {
                            ListViewItem propItem = null;
                            // Try and find a matching listview item. Note we require a case sensitive search.
                            foreach (ListViewItem existingItem in lvItemProperties.Items)
                            {
                                if (String.Compare(existingItem.Name, prop.Name, false) == 0)
                                {
                                    propItem = existingItem;
                                    break;
                                }
                            }
                            if (propItem == null)
                            {
                                // new property
                                int skipColumns = lvItemProperties.Columns.Count - 2;
                                propItem = lvItemProperties.Items.Add(prop.Name);
                                propItem.Name = prop.Name;
                                while (skipColumns-- > 0)
                                    propItem.SubItems.Add("");
                            }
                            propItem.SubItems.Add(prop.Value);
                        }
                        // mark items with changed value in blue
                        foreach (ListViewItem li in lvItemProperties.Items)
                        {
                            for (int i = 2; i < li.SubItems.Count; i++)
                            {
                                if (li.SubItems[i - 1].Text.CompareTo(li.SubItems[i].Text) != 0)
                                {
                                    li.BackColor = Color.LightBlue;
                                    break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    lvItemProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    lvItemProperties.EndUpdate();
                }
            }

        }

    }
}
