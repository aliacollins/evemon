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
    public partial class ItemBrowserControl : EveObjectBrowserControl
    {
        public ItemBrowserControl()
        {
            InitializeComponent();
            this.splitContainer1.RememberDistanceKey = "ItemBrowser";
            m_showImages = !Settings.GetInstance().WorksafeMode;
            if (this.DesignMode)
            {
                return;
            }

            if (!m_showImages)
            {
                eveImage.ImageSize = EveImage.EveImageSize._0_0;
                lblItemCategory.Location = new Point(3,lblItemCategory.Location.Y);
                lblItemName.Location = new Point(3, lblItemName.Location.Y);
            }
 
        }

        private bool m_showImages;

        private Plan m_plan = null;

        public Plan Plan
        {
            get { return m_plan; }
            set 
            { 
                m_plan = value;
                itemSelectControl1.Plan = value;
                requiredSkillsControl.Plan = value;
            }
        }

        public Item SelectedItem
        {
            set
            {
                itemSelectControl1.SelectedObject  = value;
                itemSelectControl1_SelectedItemChanged(this, null);
            }
        }

        private void itemSelectControl1_SelectedItemChanged(object sender, EventArgs e)
        {
            Item item = itemSelectControl1.SelectedObject  as Item;
            foreach (Control c in splitContainer1.Panel2.Controls)
            {
                if ( c == pnlHeader || c == lblHelp)
                    c.Visible = (item == null);
                else
                    c.Visible = (item != null);
            }


            if (item != null)
            {
                // Update required Skills
                requiredSkillsControl.EveItem = item as EveObject;
                // Update Eveimage
                eveImage.EveItem = item;
                StringBuilder sb = new StringBuilder();
                ItemCategory cat = item.ParentCategory;
                while (cat.Name != "Ship Items")
                {
                    sb.Insert(0, cat.Name);
                    cat = cat.ParentCategory;
                    if (cat.Name != "Ship Items")
                    {
                        sb.Insert(0, " > ");
                    }
                }
                lblItemCategory.Text = sb.ToString();
                lblItemName.Text = item.Name;
                tbDescription.Text = Regex.Replace(item.Description, "\n", Environment.NewLine, RegexOptions.Singleline);


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
                    for (int i_item = 0; i_item < itemSelectControl1.SelectedObjects.Count; i_item++)
                    {
                        Item selectedItem = itemSelectControl1.SelectedObjects[i_item] as Item;
                        // Skip if it's the base item or not an item
                        if (selectedItem == itemSelectControl1.SelectedObject || selectedItem == null)
                            continue;

                        // add new column header and values
                        lvItemProperties.Columns.Add(selectedItem.Name);
                        foreach (EntityProperty prop in selectedItem.Properties)
                        {
                            ListViewItem[] items = lvItemProperties.Items.Find(prop.Name, false);
                            if (items.Length != 0)
                            {
                                // existing property
                                ListViewItem oldItem = items[0];
                                oldItem.SubItems.Add(prop.Value);
                            }
                            else
                            {
                                // new property
                                int skipColumns = lvItemProperties.Columns.Count - 2;
                                ListViewItem newItem = lvItemProperties.Items.Add(prop.Name);
                                newItem.Name = prop.Name;
                                while (skipColumns-- > 0)
                                    newItem.SubItems.Add("");
                                newItem.SubItems.Add(prop.Value);
                            }
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

        private void ItemBrowserControl_Load(object sender, EventArgs e)
        {
            itemSelectControl1_SelectedItemChanged(null, null);
        }

        private void ItemBrowserControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                requiredSkillsControl.UpdateDisplay();
            }
        }

    }
}
