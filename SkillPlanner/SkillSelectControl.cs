using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;
using System.Collections;

namespace EVEMon.SkillPlanner
{
    public partial class SkillSelectControl : UserControl
    {
        public SkillSelectControl()
        {
            InitializeComponent();
        }

        private Settings m_settings;
        private CharacterInfo m_grandCharacterInfo;
        private Plan m_plan;

        public CharacterInfo GrandCharacterInfo
        {
            get { return m_grandCharacterInfo; }
            set { m_grandCharacterInfo = value; }
        }

        public Plan Plan
        {
            get { return m_plan; }
            set { m_plan = value; }
        }

        public event EventHandler<EventArgs> SelectedSkillChanged;

        private Skill m_selectedSkill;

        public Skill SelectedSkill
        {
            get { return m_selectedSkill; }
            private set
            {
                m_selectedSkill = value;
                OnSelectedSkillChanged();
            }
        }

        private void OnSelectedSkillChanged()
        {
            if (SelectedSkillChanged != null)
            {
                SelectedSkillChanged(this, new EventArgs());
            }
        }

        public ImageList GetIconSet(int index)
        {
            ImageList def = new ImageList();
            def.ColorDepth = ColorDepth.Depth32Bit;
            string groupname = null;
            if (index > 0 && index < EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties.Count)
            {
                groupname = EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties["Group" + index].DefaultValue.ToString();
            }
            if (groupname != null)
            {
                System.Resources.IResourceReader basic = new System.Resources.ResourceReader(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\icons\\Skill_Select\\Group0\\Default.resources");
                IDictionaryEnumerator basicx = basic.GetEnumerator();
                while (basicx.MoveNext())
                {
                    def.Images.Add(basicx.Key.ToString(), (System.Drawing.Icon)basicx.Value);
                }
                basic.Close();
                basic = new System.Resources.ResourceReader(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\icons\\Skill_Select\\Group" + index + "\\" + groupname + ".resources");
                basicx = basic.GetEnumerator();
                while (basicx.MoveNext())
                {
                    if (def.Images.ContainsKey(basicx.Key.ToString()))
                    {
                        def.Images.RemoveByKey(basicx.Key.ToString());
                    }
                    def.Images.Add(basicx.Key.ToString(), (System.Drawing.Icon)basicx.Value);
                }
                basic.Close();
                return def;
            }
            else
            {
                return this.ilSkillIcons;
            }
        }

        public void UseIconSet(int index)
        {
            tvItems.ImageList = GetIconSet(index);
            UpdateSkillDisplay();
        }

        private delegate bool SkillFilter(Skill gs);

        private void cbSkillFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSkillDisplay();
        }

        private void UpdateSkillFilter()
        {
            if (m_grandCharacterInfo == null || m_plan == null)
            {
                return;
            }

            SkillFilter sf;
            m_settings.SkillBrowserFilter = cbSkillFilter.SelectedIndex;

            switch (cbSkillFilter.SelectedItem.ToString())
            {
                default:
                case "All": // All Skills
                    sf = delegate
                         {
                             return true;
                         };
                    break;
                case "Known": // Known Skills
                    sf = delegate(Skill gs)
                         {
                             return gs.Known;
                         };
                    break;
                case "Not Known": // Not Known Skills
                    sf = delegate(Skill gs)
                         {
                             return !gs.Known;
                         };
                    break;

                case "Not Known - Unowned":
                    sf = delegate(Skill gs)
                         {
                             return !gs.Known && !gs.Owned;
                         };
                    break;

                case "Not Known - Owned":
                    sf = delegate(Skill gs)
                         {
                             return !gs.Known && gs.Owned;
                         };
                    break;

                case "Not Known - Trainable":
                    sf = delegate(Skill gs)
                         {
                             return !gs.Known && gs.PrerequisitesMet;
                         };
                    break;

                case "Planned": // Planned Skills
                    sf = delegate(Skill gs)
                         {
                             return m_plan.IsPlanned(gs);
                         };
                    break;
                case "Level I Ready": // Level I Ready Skills
                    sf = delegate(Skill gs)
                         {
                             return gs.Level == 0 && gs.PrerequisitesMet;
                         };
                    break;
                case "Trainable (All)": // Trainable Skills
                    sf = delegate(Skill gs)
                         {
                             return gs.PrerequisitesMet && gs.Level < 5;
                         };
                    break;

                case "Partially Trained": // partially trained skils
                    sf = delegate(Skill gs)
                         {
                             return gs.PartiallyTrained;
                         };
                    break;
                case "Not Planned": // Not Planned Skills
                    sf = delegate(Skill gs)
                         {
                             return !(m_plan.IsPlanned(gs) || gs.Level == 5);
                         };
                    break;
                case "Not Planned - Trainable": // Not Planned & Trainable Skills
                    sf = delegate(Skill gs)
                         {
                             return !m_plan.IsPlanned(gs) && gs.PrerequisitesMet && gs.Level < 5;
                         };
                    break;
            }
            int index = Settings.GetInstance().SkillIconGroup;
            if (index == 0)
            {
                index = 1;
            }
            ImageList def = new ImageList();
            def.ColorDepth = ColorDepth.Depth32Bit;
            string groupname = null;
            if (index > 0 && index < EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties.Count)
            {
                groupname = EVEMon.Resources.icons.Skill_Select.IconSettings.Default.Properties["Group" + index].DefaultValue.ToString();
            }
            if ((groupname != null && !System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\icons\Skill_Select\Group" + index + @"\" + groupname + ".resources")) ||
                !System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\icons\Skill_Select\Group0\Default.resources"))
            {
                groupname = null;
            }
            if (groupname != null)
            {
                System.Resources.IResourceReader basic = new System.Resources.ResourceReader(System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\icons\Skill_Select\Group0\Default.resources");
                IDictionaryEnumerator basicx = basic.GetEnumerator();
                while (basicx.MoveNext())
                {
                    def.Images.Add(basicx.Key.ToString(), (System.Drawing.Icon)basicx.Value);
                }
                basic.Close();
                basic = new System.Resources.ResourceReader(System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\icons\Skill_Select\Group" + index + @"\" + groupname + ".resources");
                basicx = basic.GetEnumerator();
                while (basicx.MoveNext())
                {
                    if (def.Images.ContainsKey(basicx.Key.ToString()))
                    {
                        def.Images.RemoveByKey(basicx.Key.ToString());
                    }
                    def.Images.Add(basicx.Key.ToString(), (System.Drawing.Icon)basicx.Value);
                }
                basic.Close();
            }
            else
            {
                def = this.ilSkillIcons;
            }
            tvItems.Nodes.Clear();
            tvItems.ImageList = def;
            tvItems.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            foreach (SkillGroup gsg in m_grandCharacterInfo.SkillGroups.Values)
            {
                TreeNode gtn = new TreeNode(gsg.Name, tvItems.ImageList.Images.IndexOfKey("book"), tvItems.ImageList.Images.IndexOfKey("book"));
                foreach (Skill gs in gsg)
                {
                    if (sf(gs) && (gs.Public || cbShowNonPublic.Checked))
                    {
                        TreeNode stn;
                        if (gs.Level == 0)
                        {
                            if (gs.PartiallyTrained)
                            {
                                stn = new TreeNode(gs.Name + " (" + gs.Rank + ")", tvItems.ImageList.Images.IndexOfKey("lvl0"), tvItems.ImageList.Images.IndexOfKey("lvl0"));
                            }
                            else
                            {
                                if (gs.Owned)
                                {
                                    stn = new TreeNode(gs.Name + " (" + gs.Rank + ")", tvItems.ImageList.Images.IndexOfKey("Book"), tvItems.ImageList.Images.IndexOfKey("Book"));
                                }
                                else if (gs.PrerequisitesMet) // prereqs met
                                {
                                    stn = new TreeNode(gs.Name + " (" + gs.Rank + ")", tvItems.ImageList.Images.IndexOfKey("PrereqsMet"), tvItems.ImageList.Images.IndexOfKey("PrereqsMet"));
                                }
                                else
                                {
                                    stn = new TreeNode(gs.Name + " (" + gs.Rank + ")", tvItems.ImageList.Images.IndexOfKey("PrereqsNOTMet"), tvItems.ImageList.Images.IndexOfKey("PrereqsNOTMet"));
                                }
                            }
                        }
                        else
                        {
                            stn = new TreeNode(gs.Name + " (" + gs.Rank + ")", tvItems.ImageList.Images.IndexOfKey("lvl" + gs.Level), tvItems.ImageList.Images.IndexOfKey("lvl" + gs.Level));
                        }
                        stn.Tag = gs;
                        gtn.Nodes.Add(stn);
                    }
                }
                if (gtn.Nodes.Count > 0)
                {
                    tvItems.Nodes.Add(gtn);
                }
            }
        }

        #region search

        private void lblSearchTextHint_Click(object sender, EventArgs e)
        {
            tbSearchText.Focus();
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = false;
        }

        private void tbSearch_Leave(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (m_settings.StoreBrowserFilters)
            {
                m_settings.SkillBrowserSearch = tbSearchText.Text;
            }
            SearchTextChanged();
        }

        private void SearchTextChanged()
        {
            string searchText = tbSearchText.Text.ToLower().Trim();

            if (String.IsNullOrEmpty(searchText))
            {
                if (cbSorting.SelectedIndex == 0)
                {
                    tvItems.Visible = true;
                    lbSearchList.Visible = false;
                    lbNoMatches.Visible = false;
                    lvSortedSkillList.Visible = false;
                    return;
                }
            }

            // first pass - find everything that matches the search string
            SortedList<string, Skill> filteredItems = new SortedList<string, Skill>();
            foreach (TreeNode gtn in tvItems.Nodes)
            {
                foreach (TreeNode tn in gtn.Nodes)
                {
                    if (tn.Text.ToLower().Contains(searchText) || ((Skill)tn.Tag).Description.ToLower().Contains(searchText))
                    {
                        filteredItems.Add(tn.Text, tn.Tag as Skill);
                    }
                }
            }

            SortedListSortKey sk;
            SortedListDisplayKey dk;
            MakeUniqueSortKey mu;
            string sortColName = String.Empty;

            switch (cbSorting.SelectedIndex)
            {
                default:
                case 0: // No sorting
                    lbSearchList.BeginUpdate();
                    try
                    {
                        lbSearchList.Items.Clear();
                        foreach (Skill gs in filteredItems.Values)
                        {
                            lbSearchList.Items.Add(gs);
                        }

                        lbSearchList.Location = tvItems.Location;
                        lbSearchList.Size = tvItems.Size;
                        lbSearchList.Visible = true;
                        tvItems.Visible = false;
                        lvSortedSkillList.Visible = false;
                        lbNoMatches.Visible = (filteredItems.Count == 0);
                    }
                    finally
                    {
                        lbSearchList.EndUpdate();
                    }
                    return;
                case 1: // Training time to next level
                    sortColName = "Time";
                    sk = delegate(Skill gs)
                         {
                             int curLevel = gs.Level;
                             if (curLevel == 5)
                             {
                                 return TimeSpan.MaxValue;
                             }
                             int nextLevel = curLevel + 1;
                             return gs.GetPrerequisiteTime() + gs.GetTrainingTimeToLevel(nextLevel);
                         };
                    dk = delegate(Skill gs, object v)
                         {
                             TimeSpan ts = (TimeSpan)v;
                             if (ts > TimeSpan.MaxValue - TimeSpan.FromTicks(1000))
                             {
                                 return "-";
                             }
                             else
                             {
                                 int nextLevel = gs.Level + 1;
                                 return Skill.GetRomanForInt(nextLevel) + ": " +
                                        Skill.TimeSpanToDescriptiveText(ts, DescriptiveTextOptions.Default);
                             }
                         };
                    mu = delegate(IComparable v)
                         {
                             TimeSpan ts = (TimeSpan)v;
                             if (ts > TimeSpan.MaxValue - TimeSpan.FromTicks(1000))
                             {
                                 ts -= TimeSpan.FromTicks(1);
                             }
                             else
                             {
                                 ts += TimeSpan.FromTicks(1);
                             }
                             return ts;
                         };
                    break;
                case 2: // Training time to level V
                    sortColName = "Time";
                    sk = delegate(Skill gs)
                         {
                             int curLevel = gs.Level;
                             if (curLevel == 5)
                             {
                                 return TimeSpan.MaxValue;
                             }
                             return gs.GetPrerequisiteTime() + gs.GetTrainingTimeToLevel(5);
                         };
                    dk = delegate(Skill gs, object v)
                         {
                             TimeSpan ts = (TimeSpan)v;
                             if (ts > TimeSpan.MaxValue - TimeSpan.FromTicks(1000))
                             {
                                 return "-";
                             }
                             else
                             {
                                 return Skill.GetRomanForInt(5) + ": " +
                                        Skill.TimeSpanToDescriptiveText(ts, DescriptiveTextOptions.Default);
                             }
                         };
                    mu = delegate(IComparable v)
                         {
                             TimeSpan ts = (TimeSpan)v;
                             if (ts > TimeSpan.MaxValue - TimeSpan.FromTicks(1000))
                             {
                                 ts -= TimeSpan.FromTicks(1);
                             }
                             else
                             {
                                 ts += TimeSpan.FromTicks(1);
                             }
                             return ts;
                         };
                    break;
            }

            lvSortedSkillList.BeginUpdate();
            try
            {
                SortedList<IComparable, Pair<Skill, string>> sortedItems = new SortedList<IComparable, Pair<Skill, string>>();
                foreach (Skill gs in filteredItems.Values)
                {
                    IComparable sortVal = sk(gs);
                    string dispVal = dk(gs, sortVal);
                    while (sortedItems.ContainsKey(sortVal))
                    {
                        sortVal = mu(sortVal);
                    }
                    sortedItems.Add(sortVal, new Pair<Skill, string>(gs, dispVal));
                }

                chSortKey.Text = sortColName;
                lvSortedSkillList.Items.Clear();
                foreach (Pair<Skill, string> p in sortedItems.Values)
                {
                    ListViewItem lvi = new ListViewItem(p.A.Name);
                    lvi.SubItems.Add(p.B);
                    lvi.Tag = p.A;
                    lvSortedSkillList.Items.Add(lvi);
                }

                chName.Width = -2;
                chSortKey.Width = -2;

                lvSortedSkillList.Location = tvItems.Location;
                lvSortedSkillList.Size = tvItems.Size;
                lvSortedSkillList.Visible = true;
                tvItems.Visible = false;
                lbSearchList.Visible = false;
                lbNoMatches.Visible = (sortedItems.Count == 0);
            }
            finally
            {
                lvSortedSkillList.EndUpdate();
            }
        }

        private delegate IComparable SortedListSortKey(Skill gs);

        private delegate IComparable MakeUniqueSortKey(IComparable v);

        private delegate string SortedListDisplayKey(Skill gs, object v);

        private void lbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSearchList.SelectedIndex >= 0)
            {
                this.SelectedSkill = lbSearchList.Items[lbSearchList.SelectedIndex] as Skill;
            }
            else
            {
                this.SelectedSkill = null;
            }
        }

        #endregion

        private void tvSkillList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = tvItems.SelectedNode;
            Skill gs = tn.Tag as Skill;
            this.SelectedSkill = gs;
        }

        private void SkillSelectControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                m_settings = Settings.GetInstance();
                cbSkillFilter.SelectedIndex = m_settings.SkillBrowserFilter;
                cbSorting.SelectedIndex = m_settings.SkillBrowserSort;
                cbShowNonPublic.Checked = m_settings.ShowPrivateSkills;
                if (m_settings.StoreBrowserFilters)
                {
                    tbSearchText.Text = m_settings.SkillBrowserSearch;
                }
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
            }
            catch (Exception err)
            {
                // This occurs when we're in the designer. DesignMode doesn't get set
                // when the control is a subcontrol of a user control, so we should handle
                // this here :(
                ExceptionHandler.LogException(err, true);
                return;
            }
        }

        private void cbShowNonPublic_CheckedChanged(object sender, EventArgs e)
        {
            m_settings.ShowPrivateSkills = cbShowNonPublic.Checked;
            UpdateSkillDisplay();
        }

        private void cbSorting_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_settings.SkillBrowserSort = cbSorting.SelectedIndex;
            UpdateSkillDisplay();
        }

        private void UpdateSkillDisplay()
        {
            UpdateSkillFilter();
            SearchTextChanged();
        }

        private void lvSortedSkillList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSortedSkillList.SelectedItems.Count == 0)
            {
                this.SelectedSkill = null;
            }
            else
            {
                ListViewItem lvi = lvSortedSkillList.SelectedItems[0];
                this.SelectedSkill = lvi.Tag as Skill;
            }
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x01)
            {
                tbSearchText.SelectAll();
                e.Handled = true;
            }
        }

        private void cmiExpandAll_Click(object sender, EventArgs e)
        {
            tvItems.ExpandAll();
        }

        private void cmiCollapseAll_Click(object sender, EventArgs e)
        {
            tvItems.CollapseAll();
        }

        private void tvItems_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeViewHitTestInfo tvHit = tvItems.HitTest(e.Location);
                if (tvHit.Location == TreeViewHitTestLocations.Label)
                {
                    tvItems.SelectedNode = e.Node;
                }
            }
        }

        private void cmiCollapseSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Collapse();
        }

        private void cmiExpandSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.ExpandAll();
        }

        private void cmSkills_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cmiCollapseSelected.Enabled = cmiExpandSelected.Enabled = (tvItems.SelectedNode.GetNodeCount(true) > 0);
            cmiPlanTo.Enabled = !cmiCollapseSelected.Enabled;
            if (tvItems.SelectedNode.GetNodeCount(true) == 0)
            {
                for (int i = 1; i <= cmiPlanTo.DropDownItems.Count; i++)
                {
                    cmiPlanTo.DropDownItems[String.Format("level{0}ToolStripMenuItem", i)].Enabled = m_plan.PlannedLevel(this.SelectedSkill) != i && this.SelectedSkill.Level < i;
                }
            }
            string aString;
            if (cmiCollapseSelected.Enabled)
            {
                aString = tvItems.SelectedNode.Text;
            }
            else
            {
                aString = "Selected";
            }

            cmiExpandSelected.Text = "Expand " + aString;
            cmiCollapseSelected.Text = "Collapse " + aString;
        }

        private void levelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem levelItem = (ToolStripMenuItem)sender;
            int level = Convert.ToInt32(levelItem.Text.Remove(0, 7));
            m_plan.PlanTo(this.SelectedSkill, level);
        }

        private void tvItems_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (((TreeNode)e.Item).Nodes.Count == 0)
            {
                Skill tmp = (Skill)((TreeNode)e.Item).Tag;
                if (m_plan.PlannedLevel(tmp) == 5 || tmp.Level == 5) {
                    return;
                }
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

    }
}
