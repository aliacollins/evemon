using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.IO;

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
                pbItemIcon.Size = new Size(0, 0);
                lblItemCategory.Location = new Point(3,lblItemCategory.Location.Y);
                lblItemName.Location = new Point(3, lblItemName.Location.Y);
            }
 
        }

        private bool m_showImages;
        private bool m_allSkillsKnown;
        private bool m_skillsUnplanned;

        private Plan m_plan = null;

        public Plan Plan
        {
            get { return m_plan; }
            set 
            { 
                m_plan = value;
                itemSelectControl1.Plan = value;
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
                if ( c == panel1 || c == lblHelp)
                    c.Visible = (item == null);
                else
                    c.Visible = (item != null);
            }
            if (item != null)
            {
                if (m_showImages)
                {
                    Bitmap b = new Bitmap(pbItemIcon.ClientSize.Width, pbItemIcon.ClientSize.Height);
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.FillRectangle(Brushes.Black, new Rectangle(0, 0, b.Width, b.Height));
                    }
                    pbItemIcon.Image = b;
                }
                StringBuilder sb = new StringBuilder();
                ItemCategory cat = item.ParentCategory;
                while (cat != null)
                {
                    sb.Insert(0, cat.Name);
                    cat = cat.ParentCategory;
                    if (cat != null)
                    {
                        sb.Insert(0, " > ");
                    }
                }
                lblItemCategory.Text = sb.ToString();
                lblItemName.Text = item.Name;
                lblItemDescription.Text = item.Description;

                m_allSkillsKnown = true;
                m_skillsUnplanned = false;

                SetSkillLabel(0, lblItemSkillA, item);
                SetSkillLabel(1, lblItemSkillB, item);
                SetSkillLabel(2, lblItemSkillC, item);

                if (!m_allSkillsKnown)
                {
                    List<Pair<Skill, int>> reqSkills = new List<Pair<Skill, int>>();
                    foreach (EntityRequiredSkill irs in item.RequiredSkills)
                    {
                        Pair<Skill, int> p = new Pair<Skill, int>();
                        p.A = m_plan.GrandCharacterInfo.GetSkill(irs.Name);
                        p.B = irs.Level;
                        reqSkills.Add(p);
                    }
                    TimeSpan trainTime = m_plan.GrandCharacterInfo.GetTrainingTimeToMultipleSkills(reqSkills);
                    lblItemTimeRequired.Text = "Training Time: " +
                                               Skill.TimeSpanToDescriptiveText(trainTime,
                                                                                    DescriptiveTextOptions.IncludeCommas |
                                                                                    DescriptiveTextOptions.SpaceText);
                }
                else 
                {
                    lblItemTimeRequired.Text = String.Empty;
                    btnItemSkillsAdd.Enabled = false;
                }
                if (m_skillsUnplanned)
                {
                    btnItemSkillsAdd.Enabled = true;
                }
                else
                {
                    btnItemSkillsAdd.Enabled = false;
                }

                if (!String.IsNullOrEmpty(item.Icon) && m_showImages)
                {
                    bool pic_got = false;
                    System.Resources.IResourceReader basic;
                    if (item.ParentCategory != null && item.ParentCategory.Name != "Drone Upgrades" && item.ParentCategory.ParentCategory != null && ((item.ParentCategory.ParentCategory.Name == "Drones") || (item.ParentCategory.ParentCategory.ParentCategory != null && item.ParentCategory.ParentCategory.ParentCategory.Name == "Drones")))
                    {
                        string drones_resouces = String.Format(
                                "{1}Resources{0}Optional{0}Drones64_64.resources",
                                Path.DirectorySeparatorChar,
                                System.AppDomain.CurrentDomain.BaseDirectory);
                        if (System.IO.File.Exists(drones_resouces))
                        {
                            basic = new System.Resources.ResourceReader(drones_resouces);
                            System.Collections.IDictionaryEnumerator basicx = basic.GetEnumerator();
                            while (basicx.MoveNext())
                            {
                                if (basicx.Key.ToString() == "_" + item.Id)
                                {
                                    pbItemIcon.Image = (System.Drawing.Image)basicx.Value;
                                    pic_got = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        string items_resouces = String.Format(
                                "{1}Resources{0}Optional{0}Items64_64.resources",
                                Path.DirectorySeparatorChar,
                                System.AppDomain.CurrentDomain.BaseDirectory);
                        if (System.IO.File.Exists(items_resouces))
                        {
                            basic = new System.Resources.ResourceReader(items_resouces);
                            System.Collections.IDictionaryEnumerator basicx = basic.GetEnumerator();
                            while (basicx.MoveNext())
                            {
                                if (basicx.Key.ToString() == item.Icon.ToString().Substring(item.Icon.ToString().LastIndexOf('/') + 1, item.Icon.ToString().Substring(item.Icon.ToString().LastIndexOf('/') + 1).Length - 4))
                                {
                                    pbItemIcon.Image = (System.Drawing.Image)basicx.Value;
                                    pic_got = true;
                                }
                            }
                        }
                    }
                    if (!pic_got)
                    {
                        EveSession.GetImageAsync("http://www.eve-online.com" + item.Icon, true,
                                                 delegate(EveSession sess, Image img)
                                                 {
                                                     if (
                                                         itemSelectControl1.SelectedObject == item)
                                                     {
                                                         GotItemImage(item.Id, img);
                                                     }
                                                 });
                    }
                }

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

        private void SetSkillLabel(int skillNum, LinkLabel lblSkill, Item i)
        {
            if (i.RequiredSkills.Count <= skillNum)
            {
                lblSkill.Text = String.Empty;
                lblSkill.Tag = null;
                return;
            }
            


            EntityRequiredSkill rs = i.RequiredSkills[skillNum];
            Skill gs = m_plan.GrandCharacterInfo.GetSkill(rs.Name);
            StringBuilder sb = new StringBuilder();
            lblSkill.Tag = gs;
            sb.Append(rs.Name);
            sb.Append(' ');
            sb.Append(Skill.GetRomanForInt(rs.Level));
            if (gs.Level >= rs.Level)
            {
                sb.Append(" (Known)");
            }
            else
            {
                if (m_plan.IsPlanned(gs, rs.Level))
                {
                    sb.Append(" (Planned)");
                    m_allSkillsKnown = false;
                }
                else
                {
                    m_allSkillsKnown = false;
                    m_skillsUnplanned = true;
                }
            }
            lblSkill.Text = sb.ToString();
        }


        private void GotItemImage(int itemId, Image i)
        {
            if (i == null)
            {
                return;
            }
            if (itemSelectControl1.SelectedObject == null)
            {
                return;
            }
            if (itemId != itemSelectControl1.SelectedObject.Id)
            {
                return;
            }
            pbItemIcon.Image = i;
        }

        private void ItemBrowserControl_Load(object sender, EventArgs e)
        {
            itemSelectControl1_SelectedItemChanged(null, null);
        }

        private void btnItemSkillsAdd_Click(object sender, EventArgs e)
        {
            Item i = itemSelectControl1.SelectedObject as Item;
            if (i == null)
            {
                return;
            }

            string m_note = i.Name;
            List<Pair<string, int>> skillsToAdd = new List<Pair<string, int>>();
            foreach (EntityRequiredSkill irs in i.RequiredSkills)
            {
                skillsToAdd.Add(new Pair<string, int>(irs.Name, irs.Level));
            }
            m_plan.PlanSetTo(skillsToAdd, m_note, true);
            itemSelectControl1_SelectedItemChanged(new Object(), new EventArgs());
        }


        private void llblItemSkillA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lbl = sender as LinkLabel;
            if (lbl.Tag == null) return;
            NewPlannerWindow pw = m_plan.PlannerWindow.Target as NewPlannerWindow;
            pw.ShowSkillInTree(lbl.Tag as Skill);
        }

        private void lblItemSkill_MouseHover(object sender, EventArgs e)
        {
            LinkLabel lbl = sender as LinkLabel;
            if (lbl.Tag == null) return;
            Skill s = lbl.Tag as Skill;
            StringBuilder prereqs = new StringBuilder(lbl.Text + "\n");
            prereqs.Append(buildPrereqs(s));
            ttItem.SetToolTip(lbl, prereqs.ToString());
            ttItem.IsBalloon = false;
            ttItem.Active = true;
        }

        private StringBuilder buildPrereqs(Skill s)
        {
            StringBuilder msg = new StringBuilder();
            foreach (Skill.Prereq p in s.Prereqs)
            {
                msg.Append(p.Name + " " + Skill.GetRomanForInt(p.Level));
                Skill gs = m_plan.GrandCharacterInfo.GetSkill(p.Name);
                if (gs.Level >= p.Level)
                {
                    msg.Append(" (Known)");
                }
                else if (Plan.IsPlanned(gs, p.Level))
                {
                    msg.Append(" (Planned)");
                }
                msg.Append("\n");
                msg.Append(buildPrereqs(gs));
            }
            return msg;
        }

    }
}
