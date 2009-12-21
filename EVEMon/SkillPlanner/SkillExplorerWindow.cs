using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Skill Explorer Window - allows easy exploration of skills.
    /// </summary>
    /// <remarks>
    /// This class is the Skill Explorer - or "What items/ships other skills does this skill enable for me" form.
    /// For a given skill, we show 2 tree controls (with splitter bars seperating them), one for skills, one for either ships or items, depending on the skill we#re exploring.
    /// (There is no skill that enables both a ship and an item)
    /// Each tree shows a node for each level of the chosen skill with child nodes for all items that this level
    /// is a direct prerequisite for. If we can use the item, it is shown in a normal font. If this level of skill would 
    /// enable use of the item, we show it dimmed. If this level is one of a number of missing skills for the item, then
    /// we show the item in red (with a tooltip indicating the other missing skills).
    /// The user can chose to view the trees in alphabetic order, or by the categories (alpha is default).
    /// For items, you can chose to show just the base T1/T2 items, or all variants of an item (default is base items)
    /// 
    /// There's a dropdown listbox that keeps track of what skills you've been exploring in this session.
    /// 
    /// Each node has a context menu that enables you to view the item in the relevant browser, add missing skills to 
    /// your plan, show list of all prerequisites (including costs if unowned), and for the skill tree, use the selected skill as a new base skill.
    /// 
    /// Doubleclicking a ship/item leaf node will show the selected item in the browser.
    /// Doubleclicking a skill in the tree will set that skill as the base skill we're exploring
    /// </remarks>
    public partial class SkillExplorerWindow : EVEMonForm
    {
        private PlanWindow m_planWindow;
        private Character m_character;
        private Skill m_skill;
        private bool m_hasItems = true;
        private bool m_hasShips = true;
        
        /// <summary>
        /// Constructor for designer.
        /// </summary>
        public SkillExplorerWindow()
        {
            InitializeComponent();
            this.splitContainer1.RememberDistanceKey = "SkillExplorer";

            toolTip.SetToolTip(cbHistory, "A history of the skills that you have been looking at");
        }

        /// <summary>
        /// Constructor for use in code
        /// </summary>
        /// <param name="skill">The skill we want to analyze</param>
        /// <param name="planWindow">The plan window</param>
        public SkillExplorerWindow(Skill skill, PlanWindow planWindow) 
            : this()
        {
            m_planWindow = planWindow;
            this.Skill = skill;

            EveClient.PlanChanged += new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            EveClient.CharacterChanged += new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
        }

        /// <summary>
        /// Unsubscribe events on closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            EveClient.CharacterChanged -= new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.PlanChanged -= new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            base.OnClosing(e);
        }

        /// <summary>
        /// Gets or sets the represented skill.
        /// </summary>
        public Skill Skill
        {
            get { return m_skill; }
            set
            {
                if (m_skill == value) return;
                m_character = value.Character;
                m_skill = value;

                // If already in history combo, remove it to reinsert it at the top
                if (cbHistory.Items.Contains(m_skill.Name))
                {
                    cbHistory.Items.RemoveAt(cbHistory.Items.IndexOf(m_skill.Name));
                }
                cbHistory.Items.Insert(0, m_skill.Name);
                cbHistory.SelectedIndex = 0;

                UpdateContent();
            }
        }

        #region Content creation and update
        /// <summary>
        /// Update all the content
        /// </summary>
        private void UpdateContent()
        {
            UpdatePlanName();
            UpdateHeader();
            UpdateTrees();
        }

        /// <summary>
        /// Update the the character and plan name on the skill header group title
        /// </summary>
         private void UpdatePlanName()
        {
            grpPlanName.Text = m_character.Name + " - " + m_planWindow.Plan.Name;
        }

        /// <summary>
        /// Sets the details for the skill we're analysing.
        /// Work out if we know the skill, if so, how many  points.
        /// If we don't know it, show if we could train it now or not
        /// and indicate if we own the book, if not, show the cost.
        /// </summary>
        private void UpdateHeader()
        {
            StringBuilder sb = new StringBuilder(m_skill.Name);
            if (m_skill.IsTraining) sb.Append(" - In Training");

            sb.Append(" (");
            if (m_skill.IsKnown)
            {
                sb.Append("Trained to level " + m_skill.Level + " with ");
                if (m_skill.SkillPoints > 0) sb.Append(String.Format("{0:0,0,0} sp)", m_skill.SkillPoints));
                else sb.Append(" 0 sp)");
            }
            else
            {
                sb.Append("Not Trained - prereqs ");

                if (m_skill.ArePrerequisitesMet) sb.Append("met, skillbook ");
                else sb.Append("not met, skillbook ");

                if (m_skill.IsOwned) sb.Append("is owned.)");
                else sb.Append("is not owned, book costs " + m_skill.FormattedCost + " ISK)");
            }

            lblSkill.Text = sb.ToString();
            tmrAutoUpdate.Enabled = m_skill.IsTraining;
        }

        /// <summary>
        /// Recreate the trees and their headers.
        /// 
        /// Items, ships and skills may be presented alphabetically (skill level > enabled skill) or grouped (skill level > enabled skill group > enabled skill)
        /// 
        /// Each item is coloured appropriately : normal if we know all the prereqs for the item,
        /// dimmed if this level enables the item or red if we need other skills to enable the item in
        /// addition to this level of skill.
        /// </summary>
        private void UpdateTrees()
        {
            UpdateItemsTree();
            UpdateSkillsTree();
            UpdateTreesHeaders();
        }

        /// <summary>
        /// Update the trees' headers : "items" or "ships"
        /// </summary>
        private void UpdateTreesHeaders()
        {
            if (m_hasItems && m_hasShips)
            {
                lblShips.BackColor = Color.LightBlue;
                pnlShipHeader.BackColor = Color.LightBlue;
                lblShips.Text = "Enabled Ships and Items";
                cbShowBaseOnly.Visible = true;
            }
            else if (m_hasShips)
            {
                lblShips.BackColor = Color.LightCyan;
                pnlShipHeader.BackColor = Color.LightCyan;
                lblShips.Text = "Enabled Ships";
                cbShowBaseOnly.Visible = false;
            }
            else
            {
                lblShips.BackColor = Color.MistyRose;
                pnlShipHeader.BackColor = Color.MistyRose;
                lblShips.Text = "Enabled Items";
                cbShowBaseOnly.Visible = true;
            }
        }

        /// <summary>
        /// Set up the enabled Skills tree
        /// </summary>
        private void UpdateSkillsTree()
        {

            tvSkills.BeginUpdate();
            try
            {
                tvSkills.Nodes.Clear();
                if (m_skill == null) return;

                for (int i = 1; i <= 5; i++)
                {
                    var skillLevel = new SkillLevel(m_skill, i);

                    // Gets the enabled skills and check it's not empty
                    var enabledSkills = m_skill.Character.Skills.Where(x => x.Prerequisites.Any(y => y.Skill == m_skill && y.Level == i) && x.IsPublic).ToArray();
                    if (enabledSkills.IsEmpty()) continue;

                    // Add a node for this skill level
                    TreeNode levelNode = new TreeNode(skillLevel.ToString());
                    if (m_skill.Level >= i) levelNode.Text += " (Trained)";
                    levelNode.ForeColor = Color.DarkBlue;

                    // Is it a plain alphabetical presentation ?
                    if (rbShowAlpha.Checked)
                    {
                        foreach (Skill skill in enabledSkills.OrderBy(x => x.Name))
                        {
                            levelNode.Nodes.Add(CreateNode(skill, skill.Prerequisites));
                        }
                    }
                    // Or do we need to group skills by their groups ?
                    else
                    {
                        foreach (var group in enabledSkills.GroupBy(x => x.Group).ToArray().OrderBy(x => x.Key.Name))
                        {
                            TreeNode groupNode = new TreeNode(group.Key.Name);
                            foreach (Skill skill in group.ToArray().OrderBy(x => x.Name))
                            {
                                groupNode.Nodes.Add(CreateNode(skill, skill.Prerequisites));
                            }
                            levelNode.Nodes.Add(groupNode);
                        }
                    }

                    // Add node
                    levelNode.Expand();
                    tvSkills.Nodes.Add(levelNode);
                }


                // No enabled skill found for any level ?
                if (tvSkills.Nodes.Count == 0)
                {
                    tvSkills.Nodes.Add(new TreeNode("No skills enabled by this skill"));
                }

            }
            finally
            {
                tvSkills.EndUpdate();
            }
        }

        /// <summary>
        /// Set up the items/ships tree
        /// </summary>
        private void UpdateItemsTree()
        {
            m_hasShips = false;
            m_hasItems = false;

            tvEntity.BeginUpdate();
            try
            {
                tvEntity.Nodes.Clear();
                if (m_skill == null) return;

                List<Item> items = new List<Item>(StaticItems.AllItems.
                    Where(x => x.MarketGroup.ParentGroup.ID != 150). // exclude skills
                    Where(x => x.Prerequisites.Any(y => y.Skill == m_skill.StaticData)).
                    Where(x => !cbShowBaseOnly.Checked || x.MetaGroup == ItemMetaGroup.T1 || x.MetaGroup == ItemMetaGroup.T2));

                // Scroll through levels
                for (int i = 1; i <= 5; i++)
                {
                    var skillLevel = new SkillLevel(m_skill, i);

                    // Gets the enabled objects and check it's not empty
                    var enabledObjects = items.Where(x => x.Prerequisites.Any(y => y.Skill == m_skill.StaticData && y.Level == i));
                    if (enabledObjects.IsEmpty()) continue;

                    // Add a node for this skill level
                    TreeNode levelNode = new TreeNode(skillLevel.ToString());
                    if (m_skill.Level >= i) levelNode.Text += " (Trained)";
                    levelNode.ForeColor = Color.DarkBlue;

                    // Is it a plain alphabetical presentation ?
                    if (rbShowAlpha.Checked)
                    {
                        foreach (var ship in enabledObjects.Where(x => x is Ship).ToArray().OrderBy(x => x.Name))
                        {
                            levelNode.Nodes.Add(CreateNode(ship, ship.Prerequisites.ToCharacter(m_character)));
                            m_hasShips = true;
                        }
                        foreach (var item in enabledObjects.Where(x => !(x is Ship)).ToArray().OrderBy(x => x.Name))
                        {
                            levelNode.Nodes.Add(CreateNode(item, item.Prerequisites.ToCharacter(m_character)));
                            m_hasItems = true;
                        }
                    }
                    // Or do we need to group items by their groups ?
                    else
                    {
                        // Add ships
                        var shipsToAdd = enabledObjects.Where(x => x is Ship).GroupBy(x => x.MarketGroup.ParentGroup).ToArray();
                        foreach (var shipGroup in shipsToAdd.OrderBy(x => x.Key.Name))
                        {
                            TreeNode groupNode = new TreeNode(shipGroup.Key.Name);
                            foreach (var ship in shipGroup.ToArray().OrderBy(x => x.Name))
                            {
                                groupNode.Nodes.Add(CreateNode(ship, ship.Prerequisites.ToCharacter(m_skill.Character)));
                            }
                            levelNode.Nodes.Add(groupNode);
                            m_hasShips = true;
                        }

                        // Add items recursively
                        foreach (var marketGroup in StaticItems.MarketGroups)
                        {
                            foreach (var node in CreateMarketGroupsNode(marketGroup, enabledObjects))
                            {
                                levelNode.Nodes.Add(node);
                                m_hasItems = true;
                            }
                        }
                    }

                    // Add node
                    levelNode.Expand();
                    tvEntity.Nodes.Add(levelNode);
                }


                // No enabled skill found for any level ?
                if (tvEntity.Nodes.Count == 0)
                {
                    tvEntity.Nodes.Add(new TreeNode("No ships or items enabled by this skill"));
                }

            }
            finally
            {
                tvEntity.EndUpdate();
            }
        }

        /// <summary>
        /// Recursively creates tree nodes for the children market groups of the given group.
        /// The added items will be the ones which require the current skill (<see cref="m_skill"/>) at the specified level.
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <param name="levelNode"></param>
        /// <param name="level"></param>
        private IEnumerable<TreeNode> CreateMarketGroupsNode(MarketGroup marketGroup, IEnumerable<Item> items)
        {
            // Add categories
            foreach (var category in marketGroup.SubGroups)
            {
                var children = CreateMarketGroupsNode(category, items);
                if (children.IsEmpty()) continue;

                TreeNode node = new TreeNode(category.Name);
                node.Nodes.AddRange(children.ToArray());
                yield return node;
            }

            // Add items
            foreach (var item in items.Where(x => x.MarketGroup == marketGroup))
            {
                yield return CreateNode(item, item.Prerequisites.ToCharacter(m_character));
            }
        }

        /// <summary>
        /// Adds a skill to the tree, and colors it appropriately, and set the tooltip
        /// Also sets the tag of the node to the Skill object
        /// </summary>
        /// <param name="name"> The skill we're adding</param>
        /// <param name="prerequisites">The Node we're adding to</param>
        /// <returns></returns>
        private TreeNode CreateNode(Object obj, IEnumerable<SkillLevel> prerequisites)
        {
            TreeNode skillNode = new TreeNode(obj.ToString());
            skillNode.ToolTipText = String.Empty;
            skillNode.Tag = obj;

            // When all prereqs satisifed, keep the default color
            if (prerequisites.All(x => x.IsKnown)) return skillNode;


            // Are all other prerequisites known ?
            if (prerequisites.All(x => x.IsKnown || x.Skill == m_skill))
            {
                skillNode.ForeColor = Color.Gray;
                return skillNode;
            }

            // Then we need to list the other prerequisites
            StringBuilder sb = new StringBuilder("Also Need To Train:");
            foreach(var prereq in prerequisites.Where(x => x.Skill != m_skill && !x.IsKnown))
            {
                sb.AppendLine().Append(prereq.ToString());
            }

            skillNode.ToolTipText = sb.ToString();
            skillNode.ForeColor = Color.Red;
            return skillNode;
        }
        #endregion


        #region Global events and auto-update
        /// <summary>
        /// Occurs whenever the plan changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            UpdatePlanName();
        }

        /// <summary>
        /// occurs whenever the character is updated from CCP, skills are estimed to have completed, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_CharacterChanged(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character) return;
            UpdateContent();
        }

        /// <summary>
        /// Update skill header every 30s if we're training it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrAutoUpdate_Tick(object sender, EventArgs e)
        {
            UpdateHeader();
        }
        #endregion


        #region Skills context Menu
        /// <summary>
        /// When the user clicks the node, we select it and checks whether we must display the context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tvSkills_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.tvSkills.SelectedNode = e.Node;
            if (e.Button != MouseButtons.Right) return;

            // Do not display menu for non-skill nodes
            var skill = e.Node.Tag as Skill;
            if (skill == null) return;

            // Updates selection
            tvSkills.SelectedNode = e.Node;

            // Update the "plan to X" menus
            tsAddPlan.Enabled = false;
            if (skill.Level < 5) tsAddPlan.Enabled = true;
            for (int i = 1; i <= 5; i++)
            {
                PlanHelper.UpdatesRegularPlanToMenu(tsAddPlan.DropDownItems[i - 1], m_planWindow.Plan, skill, i);
            }

            // Update the "show prerequisites" menu
            if (skill.ArePrerequisitesMet)
            {
                tsShowPrereqs.Enabled = false;
            }
            else
            {
                tsShowPrereqs.Enabled = true;
            }

            // Show menu
            cmSkills.Show(tvSkills, e.Location);
        }

        /// <summary>
        /// Skill context menu > Plan to level 
        /// theres a menu item for each level, each one is tagged with a 
        /// string representing level number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsAddLevel_Click(object sender, EventArgs e)
        {
            var operation = ((ToolStripMenuItem)sender).Tag as IPlanOperation;
            PlanHelper.PerformSilently(operation);
        }

        /// <summary>
        /// Sklll context menu > Show me what this skill unlocks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsSwitch_Click(object sender, EventArgs e)
        {
            Skill skill = (Skill)tvSkills.SelectedNode.Tag;
            this.Skill = skill;
        }

        /// <summary>
        /// Sklll context menu > Show skill in browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsShowInBrowser_Click(object sender, EventArgs e)
        {
            Skill skill = (Skill)tvSkills.SelectedNode.Tag;
            m_planWindow.ShowSkillInBrowser(skill);
        }

        /// <summary>
        /// Skill Menu - Show all prereq stats in a dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsShowPrereqs_Click(object sender, EventArgs e)
        {
            Skill skill = (Skill)tvSkills.SelectedNode.Tag;

            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var prereq in skill.Prerequisites)
            {
                FormatPrerequisite(sb, prereq, ref index);
            }

            MessageBox.Show(sb.ToString(), "Untrained Prerequisites for " + skill.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Helper method for the Show Prereqs menu used by both ship and item trees.
        /// This method adds one prereq to the string that will be displayed in the
        /// dialog box.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="prereq">The prereq</param>
        /// <param name="prNum">The position in the list of prereqs</param>
        /// <returns></returns>
        private void FormatPrerequisite(StringBuilder sb, SkillLevel prereq, ref int index)
        {
            if (prereq.Skill.IsKnown)
            {
                // we know this prereq, but not to a high enough level
                if (!prereq.IsKnown)
                {
                    index++;
                    //sb.AppendLine(String.Format("{0}. {1} (Known to level {2})\n", index, prereq.Skill.Name, prereq.Skill.RomanLevel));
                    sb.AppendLine(String.Format("{0}. {1} {2}\n", index, prereq.Skill.Name,
                        (prereq.Skill.Level > 1 ? String.Format("(Trained to level {0})", prereq.Skill.RomanLevel) : "(Not yet trained)")));
                }
                return;
            }

            // We don't know this prereq at all
            index++;
            sb.Append(String.Format("{0}. {1}", index, prereq.Skill.Name));
            sb.Append(" (");
            sb.Append("Prereqs ");

            // Could we train it now?
            if (prereq.Skill.Prerequisites.AreTrained()) sb.Append("met, skillbook ");
            else sb.Append("not met, skillbook ");

            // Do we own  the skillbook?
            if (prereq.Skill.IsOwned) sb.AppendLine(" owned.)");
            else sb.AppendLine("not owned,\n costs " + prereq.Skill.FormattedCost + " ISK)\n");
        }
        #endregion


        #region Items and ships context menu
        /// <summary>
        /// When the user clicks the node, we select it and checks whether we must display the context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tvEntity_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvEntity.SelectedNode = e.Node;
            if (e.Button != MouseButtons.Right) return;

            // Display menu only for items or ships nodes (not market groups and level nodes)
            var entity = e.Node.Tag as Item;
            if (entity == null) return;

            // Updates selection
            tvEntity.SelectedNode = e.Node;

            // "Add to plan" is enabled if we don't know all the prereqs 
            // and we're not already planning at least one of the unknown prereqs
            bool canPlan = entity.Prerequisites.ToCharacter(m_character).Any(x => !x.IsKnown && !m_planWindow.Plan.IsPlanned(x.Skill, x.Level));
            tsShowObjectPrereqs.Enabled = canPlan;
            tsAddObjectToPlan.Enabled = canPlan;

            // Other menus
            tsShowObjectInBrowser.Text = "Show " + entity.Name + " In Browser";

            // Show menu
            cmEntity.Show(tvEntity, e.Location);
        }

        /// <summary>
        /// Shared context menu - add ship/item to plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsAddEntityToPlan_Click(object sender, EventArgs e)
        {
            var entity = (Item)tvEntity.SelectedNode.Tag;
            var operation = m_planWindow.Plan.TryAddSet(entity.Prerequisites, entity.Name);
            PlanHelper.Perform(operation);
        }

        /// <summary>
        /// Shared Ship/Item Show Prereqs menu
        /// Builds a nicely formatted list of prereqs for the ship/item and shows
        /// them in a dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsShowShipPrereqs_Click(object sender, EventArgs e)
        {
            var entity = (Item)tvEntity.SelectedNode.Tag;

            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var prereq in entity.Prerequisites.ToCharacter(m_character))
            {
                FormatPrerequisite(sb, prereq, ref index);
            }

            MessageBox.Show(sb.ToString(), "Untrained Prerequisites for " + entity.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        #endregion


        #region Controls' events
        /// <summary>
        /// Toggling the radio buttons to switch bettwen sorted list and category views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbShowAlpha_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTrees();
        }

        /// <summary>
        /// Toggling the "Show base items/show variants" - just redisplay
        /// the items tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbShowBaseOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateItemsTree();
        }

        /// <summary>
        /// We're done!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Doubleclicks on a ship/item leaf node will show the ship/item in the browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvEntity_DoubleClick(object sender, EventArgs e)
        {
            if (tvEntity.SelectedNode == null) return;

            Item ship = tvEntity.SelectedNode.Tag as Ship;
            Item item = tvEntity.SelectedNode.Tag as Item;

            if (ship != null) m_planWindow.ShowShipInBrowser(ship);
            else if (item != null) m_planWindow.ShowItemInBrowser(item);
        }

        /// <summary>
        /// Sklll context menu > Show me what this skill unlocks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvSkills_DoubleClick(object sender, EventArgs e)
        {
            Skill skill = (Skill)tvSkills.SelectedNode.Tag;
            this.Skill = skill;
        }

        /// <summary>
        /// We want to go look at a skill in the history list again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string skillName = cbHistory.Items[cbHistory.SelectedIndex] as string;
            this.Skill = m_character.Skills[skillName];
        }
        #endregion 

    }
}
