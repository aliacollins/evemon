using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    /// <summary>
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
    /// 
    /// 
    /// 
    /// </summary>
    public partial class SkillEnablesForm : Form
    {
        #region constructors
        public SkillEnablesForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">The skill we want to analyze</param>
        /// <param name="sbc">The Skill Browser Control (se we can show items in the browsers and gain access to caharcater data, and planner window)</param>
        public SkillEnablesForm(Skill s,SkillBrowser sbc) : this()
        {
            m_skill = s;
            m_skillBrowser = sbc;
            m_plannerWindow = sbc.Plan.PlannerWindow;
            m_characterInfo = m_skillBrowser.Plan.GrandCharacterInfo;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Used by the skill browser, if the form is already constructed. (the skill browser shows the form non-modally)
        /// </summary>
        /// <param name="s">The new skill we want to analyze</param>
        public void SetSkill(Skill s)
        {
            if (m_skill == s) return;
            m_skill = s;
            UpdateSkillLabel();
            if (cbHistory.Items.Contains(s.Name))
            {
                // already in list,remove it so it gets reinsetred at the top
                cbHistory.Items.RemoveAt(cbHistory.Items.IndexOf(s.Name));
            }
            cbHistory.Items.Insert(0, s.Name);

            cbHistory.Text = cbHistory.Items[0] as string;
            PopulateLists();
        }
        #endregion

        #region private methods

        #region list population

        /// <summary>
        /// Used when skill is changed, or refresh button is clicked.
        /// or we toggle between alpha and category views.
        /// Recreates the 3 trees
        /// 
        /// 
        /// Each list is constructed in a similar way.
        /// Firstly we clear the tree, then for each of the 5 levels of
        /// the base skill, we construct a list of ships/skills/items that
        /// are enabled by that level.
        /// 
        /// Next, we iterate through the 5 skill levels, adding a node that
        /// represents this level of the base skill (in blue) with a " Known" suffix if we
        /// know this level of skill. Then we analyze the list of enabled objects
        /// for this level and add nodes in either a category tree or a straight sorted list of nodes.
        /// Each item is coloured appropriately (normal if we know all the prereqs for the item,
        /// dimmed if this level enables the item or red if we need other skills to enable the item in
        /// addition to this level of skill. If it's red, we construct a tooltip to indicate the
        /// other missing skills.
        /// </summary>
        private void PopulateLists()
        {
            PopulateSkillList();
            if (IsShipSkill)
            {
                lblShips.BackColor = Color.LightCyan;
                pnlShipHeader.BackColor = Color.LightCyan;
                lblShips.Text = "Enabled Ships";
                cbShowBaseOnly.Visible = false;
                PopulateShipList();
            }
            else
            {
                lblShips.BackColor = Color.MistyRose;
                pnlShipHeader.BackColor = Color.MistyRose;
                lblShips.Text = "Enabled Items";
                cbShowBaseOnly.Visible = true;
                PopulateItemList();
            }
        }

        /// <summary>
        /// Set up the Enabled Skills tree - see PopulateLists for an overview
        /// </summary>
        private void PopulateSkillList()
        {
           this.SuspendLayout();
           tvSkills.Nodes.Clear();

            // initialise the lists of enabled skills
            SortedList<String,Skill>[] enabledSkills = new SortedList<string,Skill>[5];
            for (int i = 0; i < 5; i++)
            {
                enabledSkills[i] = new SortedList<string, Skill>();
            }

            // build the lists of skills enabled by the base skill
            foreach (SkillGroup sg in m_characterInfo.SkillGroups.Values)
            {
                foreach (Skill s in sg)
                {
                    foreach (Skill.Prereq pr in s.Prereqs)
                    {
                        if (pr.Skill.Id == m_skill.Id)
                        {
                            enabledSkills[pr.Level - 1].Add(s.Name, s);
                        }
                    }
                }
            }
            
        
           for (int i = 0; i < 5; i++)
           {
               if (enabledSkills[i].Count == 0)
               {
                   // nothing to do for this level of the skill
                   continue;
               }

               // Add a node describing the base skill & level
               TreeNode tnSkillLevel = new TreeNode(m_skill.Name + " " + roman[i]);
               if (m_skill.Level > i) tnSkillLevel.Text += " (Known)";
               tnSkillLevel.ForeColor = Color.DarkBlue;
               
               // add in the skills, either in a sorted list or a group node
               if (rbShowAlpha.Checked)
               {
                   // The easy part!
                   foreach (Skill s in enabledSkills[i].Values)
                   {
                       AddSkillNode(s,tnSkillLevel.Nodes);
                   }
               }

               else
               // Show in skill groups   
               {
                   foreach (SkillGroup sg in m_characterInfo.SkillGroups.Values)
                   {
                       TreeNode gtn = new TreeNode(sg.Name);
                       foreach (Skill s in sg)
                       {
                           if (enabledSkills[i].ContainsKey(s.Name))
                           {
                               AddSkillNode(s,gtn.Nodes);
                           }
                       }
                       if (gtn.Nodes.Count > 0)
                       {
                           tnSkillLevel.Nodes.Add(gtn);
                           gtn.Expand();
                       }
                   }
               }
               tnSkillLevel.Expand();
               tvSkills.Nodes.Add(tnSkillLevel);
           }
            if (tvSkills.Nodes.Count == 0)
                tvSkills.Nodes.Add(new TreeNode("No skills enabled by this skill"));

           this.ResumeLayout();
        }

        /// <summary>
        /// Adds a skill to the tree, and colors it appropriately, and set the tooltip
        /// Also sets the tag of the node to the Skill object
        /// </summary>
        /// <param name="s"> The skill we're adding</param>
        /// <param name="nodes">The Node we're adding to</param>
        /// <returns></returns>
        private void AddSkillNode( Skill s,TreeNodeCollection nodes)
        {
            TreeNode tnSkill = new TreeNode(s.Name);
      
            // get the skill instance from the current character
            // we need to do this as the prereqs came from the Skill.AllSkills
            // collection and will not reflect this character's position with that skill
            Skill skill = m_characterInfo.GetSkill(s.Name);
            tnSkill.Tag = skill;
            tnSkill.ToolTipText = String.Empty;

            if (!skill.Known)
            {
               // we don't know this skill, so mark it as grey
               tnSkill.ForeColor = Color.Gray;


               // check if all other prereqs are met
               bool colorRed = false;
               StringBuilder sb = new StringBuilder();

               foreach (Skill.Prereq pr in skill.Prereqs)
               {
                   // again, get the instance of this skill from the current character
                   Skill prs = m_characterInfo.GetSkill(pr.Name);
                   if (prs.Id != m_skill.Id && prs.Level < pr.Level)
                   {
                       // there's more prereqs to learn!
                       if (!colorRed) 
                       {
                           colorRed = true;
                           sb.Append("Also Need To Train:");
                       }
                       sb.Append(String.Format("\n{0} {1}", pr.Name, roman[pr.Level - 1]));
                       colorRed = true;
                   }
                }
                if (colorRed)
                {
                    tnSkill.ForeColor = Color.Red;
                    tnSkill.ToolTipText = sb.ToString();
                }
            }
            nodes.Add(tnSkill);
        }
        /// <summary>
        /// Set up the Enabled Ships tree - see PopulateLists for an overview
        /// </summary>
        private void PopulateShipList()
        {
            this.SuspendLayout();
            tvEntity.Nodes.Clear();

            // Initialize the 5 enabled ships lists
            SortedList<String, Ship>[] enabledShips = new SortedList<string, Ship>[5];
            for (int i = 0; i < 5; i++)
            {
                enabledShips[i] = new SortedList<string, Ship>();
            }

            // build lists of ships enabled by this skill
            foreach (Ship s in Ship.GetShips())
            {
                foreach (EntityRequiredSkill sr in s.RequiredSkills)
                {
                    if (sr.Name == m_skill.Name)
                    {
                        enabledShips[sr.Level - 1].Add(s.Name, s);
                    }
                }
            }


            // now process each level of the base skill and build the tree
            for (int i = 0; i < 5; i++)
            {
                if (enabledShips[i].Count == 0)
                {
                    // Nothing to add at this skill level
                    continue;
                }

                // Add a node describing the base skill & level
                TreeNode tnSkillLevel = new TreeNode(m_skill.Name + " " + roman[i]);
                if (m_skill.Level > i) tnSkillLevel.Text += " (Known)";
                tnSkillLevel.ForeColor = Color.DarkBlue;

                // add in the skills, either in a sorted list or a group node
                if (rbShowAlpha.Checked)
                {
                    // The Easy part
                    foreach (Ship s in enabledShips[i].Values)
                    {
                        AddShipNode(s,tnSkillLevel.Nodes);
                    }
                }

                else
                // Show in ship groups - modified version of shipSelect tree code  
                {

                    // Build list of types (battleships, frigs etc)
                    SortedList<string, List<Ship>> types = new SortedList<string, List<Ship>>();

                    // look at our enabled ships
                    foreach (Ship s in enabledShips[i].Values)
                    {
                        // have we already added this type?
                        if (!types.ContainsKey(s.Type))
                        {
                            // no, add a type list
                            List<Ship> nl = new List<Ship>();
                            nl.Add(s);
                            types.Add(s.Type, nl);
                        }
                        else
                        {
                            // yes. add this ship to the relevant type list
                            types[s.Type].Add(s);
                        }
                    }

                    // Now, in a similar way, work through the types and add races
                    foreach (KeyValuePair<string, List<Ship>> kvp in types)
                    {
                        TreeNode tvn = new TreeNode();
                        tvn.Text = kvp.Key;

                        SortedList<string, List<Ship>> races = new SortedList<string, List<Ship>>();
                        foreach (Ship s in kvp.Value)
                        {
                            if (!races.ContainsKey(s.Race))
                            {
                                List<Ship> nl = new List<Ship>();
                                nl.Add(s);
                                races.Add(s.Race, nl);
                            }
                            else
                            {
                                races[s.Race].Add(s);
                            }
                        }
                        // We've got the races, Add ships
                        foreach (KeyValuePair<string, List<Ship>> ssvp in races)
                        {
                            TreeNode racenode = new TreeNode();
                            racenode.Text = ssvp.Key;

                            SortedList<string, Ship> ships = new SortedList<string, Ship>();
                            foreach (Ship s in ssvp.Value)
                            {
                                ships[s.Name] = s;
                            }
                            foreach (Ship s in ships.Values)
                            {
                                AddShipNode(s,racenode.Nodes);
                            }

                            tvn.Nodes.Add(racenode);
                        }
                        tnSkillLevel.Nodes.Add(tvn);
                    }
                }
                // We're done with this skill level
                tnSkillLevel.Expand();
                tvEntity.Nodes.Add(tnSkillLevel);
            }
            if (tvEntity.Nodes.Count == 0)
                tvEntity.Nodes.Add(new TreeNode("No ships enabled by this skill"));

            this.ResumeLayout();
        }

        /// <summary>
        /// Creates a node for the ship, with tag set to the Ship object
        /// </summary>
        /// <param name="s"></param>
        /// <param name="tnc"></param>
        /// <returns></returns>
        private void AddShipNode(Ship s,TreeNodeCollection tnc)
        {
            TreeNode tnShip = new TreeNode(s.Name);
            tnShip.Tag = s;
            ColorNode(tnShip, s.RequiredSkills);
            tnc.Add(tnShip);
        }


        // Items have to be processed recursively so we need a couple of
        // private fields to help us...
        SortedList<String, Item>[] m_enabledItems = new SortedList<string, Item>[5];
        int m_enabledIndex;

        private void PopulateItemList()
        {
            this.SuspendLayout();
            tvEntity.Nodes.Clear();

            // Initialize the 5 lists of enabled items.
            for (int i=0;i<5;i++)
            {
                if (m_enabledItems[i] == null) m_enabledItems[i] = new SortedList<string, Item>();
                else m_enabledItems[i].Clear();
            }

            // build lists of items enabled by this skill
            ItemCategory rootCategory = ItemCategory.GetRootCategory();
           
            PopulateEnabledItems(rootCategory);

            // we've identified the items, now build the tree, one node for each
            // level of the base skill 

            for (int i = 0; i < 5; i++)
            {
                if (m_enabledItems[i].Count == 0)
                {
                    // no items enabled by this skill level
                    continue;
                }
                m_enabledIndex = i;

                // Create a node for this skill level
                TreeNode tnSkillLevel = new TreeNode(m_skill.Name + " " + roman[i]);
                if (m_skill.Level > i) tnSkillLevel.Text += " (Known)";
                tnSkillLevel.ForeColor = Color.DarkBlue;

                // add in the skills, either in a sorted list or a group node
                if (rbShowAlpha.Checked)
                {
                    // The easy part!
                    foreach (Item item in m_enabledItems[i].Values)
                    {
                        AddItemNode(item, tnSkillLevel.Nodes);
                    }
                }

                else
                {
                    // We're doing the category tree... More recursion!
                    rootCategory = ItemCategory.GetRootCategory();
                    TreeNode rootNode = new TreeNode(rootCategory.Name);
                    AddItemNodes(rootCategory,tnSkillLevel.Nodes);
                }

                // we're done with this skill level
                tnSkillLevel.Expand();
                tvEntity.Nodes.Add(tnSkillLevel);
            }
            if (tvEntity.Nodes.Count == 0)
                tvEntity.Nodes.Add(new TreeNode("No items enabled by this skill"));

            this.ResumeLayout();
        }

        /// <summary>
        /// Recursivly work through item categories looking for
        /// items enabled by the current skill level
        /// </summary>
        /// <param name="ic">The parnt item category</param>
        private void PopulateEnabledItems(ItemCategory ic)
        {
            foreach (Item item in ic.Items)
            {
                // Check if we're only lookin for non-variants (t1/t2)
                if (!cbShowBaseOnly.Checked || item.Metagroup == "Tech I" || item.Metagroup == "Tech II")
                {
                    // yep, now see if this skill level enables the item
                    foreach (EntityRequiredSkill sr in item.RequiredSkills)
                    {
                        if (sr.Name == m_skill.Name)
                        {
                            // yep add to the relevent enabled items list
                            m_enabledItems[sr.Level - 1].Add(item.Name, item);
                        }
                    }
                }
            }
            // and now recursivly work through sub-categories
            foreach (ItemCategory sic in ic.Subcategories)
            {
                PopulateEnabledItems(sic);
            }
        }

        /// <summary>
        /// Recursively build the enabled itesm by category tree view
        /// </summary>
        /// <param name="cat">The parent category of enabled items</param>
        /// <param name="nodeCollection">The node we're adding too</param>
        private void AddItemNodes(ItemCategory cat, TreeNodeCollection nodeCollection)
        {
            // firstly get the sub-categories in sorted order
            SortedDictionary<string, ItemCategory> sortedSubcats = new SortedDictionary<string, ItemCategory>();
            foreach (ItemCategory tcat in cat.Subcategories)
            {
                sortedSubcats.Add(tcat.Name, tcat);
            }

            // and build a node for each subcategory
            foreach (ItemCategory tcat in sortedSubcats.Values)
            {
                TreeNode tn = new TreeNode();
                tn.Text = tcat.Name;
                // Add sub-categories to this node, recursion ftw!
                AddItemNodes(tcat, tn.Nodes);
                if (tn.GetNodeCount(true) > 0)
                    // Add the sub-categories to the parent if we have any nodes
                    nodeCollection.Add(tn);
            }

            // we've done any subcategories, now the items themselves...
            SortedDictionary<string, Item> sortedItems = new SortedDictionary<string, Item>();
            foreach (Item titem in cat.Items)
            {
                // is this an enabled item?
                if (m_enabledItems[m_enabledIndex].ContainsKey(titem.Name))
                    // yes. lets add it then.
                    sortedItems.Add(titem.Name, titem);
            }
            foreach (Item titem in sortedItems.Values)
            {
                AddItemNode(titem,nodeCollection);
            }
        }

        /// <summary>
        /// Create an item node with correctky colored text, node will be tagged
        /// // with the Item object.
        /// </summary>
        /// <param name="i">The item we're adding</param>
        /// <param name="tnc">The node we're adding to</param>
        private void AddItemNode(Item i, TreeNodeCollection tnc)
        {
            TreeNode tnItem = new TreeNode(i.Name);
            tnItem.Tag = i;
            ColorNode(tnItem, i.RequiredSkills);
            tnc.Add(tnItem);
        }

        /// <summary>
        /// Utility method used by ship and item tree population.
        /// We look at the prereqs for the ship/item and determine
        /// if they're all known, or just this skill level is missing (where we
        /// show it dimmed) or there's other skills needed (shown red, with a tooltip
        /// indicating missing skills)
        /// </summary>
        /// <param name="tn">The node to be colored</param>
        /// <param name="prereqs">The list of prerequisites for this
        /// ship or item</param>
        private void ColorNode(TreeNode tn, List<EntityRequiredSkill> prereqs)
        {
            bool skillsNeeded = false;
            bool otherSkillsNeeded = false;
            Skill gs = null;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < prereqs.Count; i++)
            {
                gs = m_characterInfo.GetSkill(prereqs[i].Name);
                if (gs.Level < prereqs[i].Level)
                {
                    // we don;t know this prereq at the right level
                    skillsNeeded = true;
                    if (gs.Name != m_skill.Name)
                    {
                        // and it's not the skill we're analying, so it needs to be red
                        if (!otherSkillsNeeded) 
                        {  
                            // 1st one, so set the tooltip title
                            sb.Append("Also Need To Train:");
                            otherSkillsNeeded = true;
                        }
                        sb.Append(String.Format("\n{0} {1}",prereqs[i].Name, Skill.GetRomanForInt(prereqs[i].Level)));
                    }
                }
            }

            if (otherSkillsNeeded)
            {
                // we can't use this because of other skills, so mark it as red
                tn.ForeColor = Color.Red;
                tn.ToolTipText = sb.ToString();
            }
            else if (skillsNeeded)
            {
                // we could use this if this skill was trained at this level, so mark it as gray
                tn.ForeColor = Color.Gray;
            }
            if (!otherSkillsNeeded)  tn.ToolTipText = String.Empty;
        }

        #endregion // end of tree population stuff

        /// <summary>
        /// Sets the details for the skill we're analysing.
        /// Work out if we know the skill, if so, how many  points.
        /// If we don't know it, show if we could train it now or not
        /// and indicate if we own the book, if not, show the cost.
        /// </summary>
        private void UpdateSkillLabel()
        {
            if (this.InvokeRequired)
            {
                // called from timer thread if skill is currently training
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      UpdateSkillLabel();
                                                  }));
                return;
            }

            StringBuilder sb = new StringBuilder(m_skill.Name);
            if (m_skill.InTraining) sb.Append(" - In Training");
            sb.Append(" (");
            if (m_skill.Known)
            {
                sb.Append("Trained to level " + m_skill.Level + " with ");
                if (m_skill.CurrentSkillPoints > 0)
                    sb.Append(String.Format("{0:0,0,0} sp)", m_skill.CurrentSkillPoints));
                else
                    sb.Append(" 0 sp)");
            }
            else
            {
                sb.Append("Not Trained - prereqs ");
                if (m_skill.PrerequisitesMet) sb.Append("met, skillbook ");
                else sb.Append("not met, skillbook ");
                if (m_skill.Owned) sb.Append("is owned.)");
                else sb.Append("is not owned, book costs " + m_skill.FormattedCost + " ISK)");
            }
            lblSkill.Text = sb.ToString();
            if (m_skill.InTraining)
            {
                tmrAutoUpdate.Enabled = true;
                tmrAutoUpdate.Interval = 2;
            }
            else
                tmrAutoUpdate.Enabled = false;
        }

        /// <summary>
        /// The user has either updated current plan, or switched plans within the Planner window
        /// </summary>
        /// <param name="m_plan"></param>
        internal void PlanChanged()
        {
            SetPlanName();
            PopulateLists();
        }

        /// <summary>
        /// User is closing the planner window or has deleted this plan within the planner window so close down
        /// </summary>
        internal void Shutdown()
        {
            tmrAutoUpdate.Enabled = false;
            this.Close();
        }


        #region context menu helper methods

        /// <summary>
        ///  Helper method for the Skill context menu.
        /// Gets the Skill object for the selected node.
        /// </summary>
        /// <returns></returns>
        private Skill GetSelectedSkill()
        {
            Skill s = null;
            if (tvSkills.SelectedNode != null)
            {
                s = tvSkills.SelectedNode.Tag as Skill;
            }
            return s;
        }

        
        /// <summary>
        /// The ship and item trees share a common context menu.
        /// so this is called when the context menu is opening, to
        /// set the state of the menu items.
        /// </summary>
        /// <param name="prereqs">list of prereqs for the selected node</param>
        /// <param name="entity">is it a Ship or an Item?</param>
        private void PrepareCm(List<EntityRequiredSkill> prereqs, string entity)
        {
            tsShowObjectInBrowser.Text = "Show " + entity + " In Browser";
            tsAddObjectToPlan.Enabled = false;
            tsShowObjectPrereqs.Enabled = false;
            // Add to plan is enabled if we don't know all the prereqs 
            // and we're not already planning at least one of the unknown prereqs
            bool skillsNeeded = false;
            bool allPlanned = true;
            bool thisNeeded;
            for (int i = 0; i < prereqs.Count; i++)
            {
                // get the skill form the active character so we have the right stats
                Skill gs = m_characterInfo.GetSkill(prereqs[i].Name);
                thisNeeded = gs.Level < prereqs[i].Level;
                if (thisNeeded) skillsNeeded = true;
                if (thisNeeded && !m_skillBrowser.Plan.IsPlanned(gs, prereqs[i].Level))
                    allPlanned = false;
            }

            if (skillsNeeded)
            {
                tsShowObjectPrereqs.Enabled = true;
                if (!allPlanned)
                    tsAddObjectToPlan.Enabled = true;
            }
        }

        /// <summary>
        /// Another helper method for the shared context menu.
        /// When the menu is opening, work out which control we're on
        /// and work out if we're on a leaf node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="s"></param>
        /// <param name="item"></param>
        private void SelectedNode(Object sender, out Ship s, out Item item)
        {
            s = null;
            item = null;
            TreeNode tn = tvEntity.SelectedNode;
            if (tn != null)
            {
                // it'll be one or t'other
                s = tvEntity.SelectedNode.Tag as Ship;
                item = tvEntity.SelectedNode.Tag as Item;
            }
        }

        /// <summary>
        /// The last  helper method for the shared context menu.
        /// This one is for when a submenu is clicked - work out which control we're on
        /// and work out if we're on a leaf node

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="s"></param>
        /// <param name="item"></param>
        private void SelectedNodeTs(Object sender, out Ship s, out Item item)
        {
            s = null;
            item = null;
            ToolStripDropDownItem tsdi = sender as ToolStripDropDownItem;
            if (tsdi != null)
                // The owner will be the context menu so delegate to SelectNode()
                SelectedNode(tsdi.Owner, out s, out item);
        }

        /// <summary>
        /// Helper method for the Show Prereqs menu used by both ship and item trees.
        /// This method adds one prereq to the string that will be displayed in the
        /// dialog box.
        /// </summary>
        /// <param name="level">The level we know this skill to (0 - 5)</param>
        /// <param name="prs">The prereq</param>
        /// <param name="prNum">The position in the list of prereqs</param>
        /// <returns></returns>
        private StringBuilder FormatPrereq(int level, Skill prs, ref int prNum)
        {
            StringBuilder sb = new StringBuilder();
            if (prs.Known)
            {
                if (level > prs.Level)
                {
                    // we know this prereq, but not to a high enough level
                    prNum++;
                    sb.Append(String.Format("{0}. {1} (Known to level {2})\n", prNum, prs.Name, prs.RomanLevel));
                }
            }
            else
            {
                // we don't know this prereq at all
                prNum++;
                sb.Append(String.Format("{0}. {1}", prNum, prs.Name));
                sb.Append(" (");
                sb.Append("Prereqs ");
                
                // could we train it now?
                if (prs.PrerequisitesMet) sb.Append("met, skillbook ");
                else sb.Append("not met, skillbook ");

                // do we own  the skillbook?
                if (prs.Owned) sb.Append(" owned.)");
                else sb.Append("not owned, costs " + prs.FormattedCost + " ISK)\n");
            }
            return sb;
        }

        #endregion // context helpers

        // Set the character and plan name on the skill header group title
        private void SetPlanName()
        {
            grpPlanName.Text = m_characterInfo.Name + " - " + m_skillBrowser.Plan.Name;
        }
        #endregion /// privates

        #region Callbacks
        /// <summary>
        /// Training skill is switched or completed, could impact
        /// the enabled skills/ships/items as well as the skill header.
        /// lets not do anything too clever, it's a rare enough event that we can
        /// just redraw everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TrainingSkillChangedCallback(object sender, EventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                     TrainingSkillChangedCallback(sender, args);
                                                  }));
                return;
            }
            // stop the timer incase we've completed the base skill
            tmrAutoUpdate.Enabled = false;
            UpdateSkillLabel();
            PopulateLists();
        }
        #endregion

        #region events

        /// <summary>
        /// Loading the form - set the group box title to the character name
        /// set the skill label.
        /// default the checkboxes, and set up the 3 trees
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkillEnablesForm_Load(object sender, EventArgs e)
        {
            SetPlanName();
            m_characterInfo.TrainingSkillChanged += new EventHandler(TrainingSkillChangedCallback);
            UpdateSkillLabel();
            PopulateLists();
            cbHistory.Items.Insert(0, m_skill.Name);
            cbHistory.Text = m_skill.Name;
        }


        /// <summary>
        /// Toggling the radio buttons to swith bettwen sorted list and category views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbShowAlpha_CheckedChanged(object sender, EventArgs e)
        {
            PopulateLists();
        }

        /// <summary>
        /// Toggling the "Show base items/show variants" - just redisplay
        /// the items tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbShowBaseOnly_CheckedChanged(object sender, EventArgs e)
        {
            PopulateItemList();
        }

        /// <summary>
        /// We're closing, wave byebye to the skill browser 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkillEnablesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_plannerWindow.IsAlive)
            {
                m_skillBrowser.EnablesForm = null;
            }
            if (!(e.CloseReason == CloseReason.ApplicationExitCall) &&  // and Application.Exit() was not called
                !(e.CloseReason == CloseReason.TaskManagerClosing) &&  // and the user isn't trying to shut the program down for some reason
                !(e.CloseReason == CloseReason.WindowsShutDown))  // and Windows is not shutting down
            {
                m_characterInfo.TrainingSkillChanged -= new EventHandler(TrainingSkillChangedCallback);
            }
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
            if (tvEntity.SelectedNode != null)
            {
                Ship s = tvEntity.SelectedNode.Tag as Ship;
                Item itm = tvEntity.SelectedNode.Tag as Item;
                NewPlannerWindow w = m_skillBrowser.Plan.PlannerWindow.Target as NewPlannerWindow;
                if (s != null)
                    w.ShowShipInBrowser(s);
                else
                    w.ShowItemInBrowser(itm);
            }
        }

        /// <summary>
        /// Doubleclicks on a Ships/Item leaf node will show the ship/item in the browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_DoubleClick(object sender, EventArgs e)
        {
            if (tvEntity.SelectedNode != null)
            {
                Item i = tvEntity.SelectedNode.Tag as Item;
                if (i != null)
                {
                    NewPlannerWindow w = m_skillBrowser.Plan.PlannerWindow.Target as NewPlannerWindow;
                    if (w != null)
                        w.ShowItemInBrowser(i);
                }
            }
        }

        /// <summary>
        /// We want to go look at a skill in the history list again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string skill = cbHistory.Items[cbHistory.SelectedIndex] as string;
            SetSkill(m_characterInfo.GetSkill(skill));
        }

        /// <summary>
        /// Show user what this combobox is for
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbHistory_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "A history of the skills that you have been looking at");
            toolTip.Active = true;
        }


        /// <summary>
        /// Tick-tock - update skill header if we're training it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrAutoUpdate_Tick(object sender, EventArgs e)
        {
            if (m_skill.InTraining)
                UpdateSkillLabel();
        }

        #endregion 

        #region Context Menu

        /// <summary>
        /// We're opening up the Skills context menu...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmSkills_Opening(object sender, CancelEventArgs e)
        {
            // Are we on a skill node?
            Skill s = null;
            TreeNode tn = tvSkills.SelectedNode;
            if (tn != null)
            {
                s = tn.Tag as Skill;
            }

           
            if (s == null)
            {
                e.Cancel = true;
                return;
            }

            // yes we are, work out what context menu items are visible

            // Enabled add plan if skill is not known & not planned

            // Hide any Plan to x menu items if we know or have planned, the skill to that level
            for (int i = 1; i < 6; i++)
            {
                tsAddPlan.Enabled = false;
                tsAddPlan.DropDown.Items[i - 1].Enabled = true;
                if (s.Level >= i || m_skillBrowser.Plan.IsPlanned(s, i))
                    tsAddPlan.DropDown.Items[i - 1].Visible = false;
                else
                {
                    tsAddPlan.Enabled = true;
                    tsAddPlan.DropDown.Items[i - 1].Visible = true;
                }
            }

            // Show prereqs
            if (s.PrerequisitesMet)
            {
                tsShowPrereqs.Enabled = false;
            }
            else
            {
                tsShowPrereqs.Enabled = true;
            }
        }

        /// <summary>
        /// Skill menu - plan to level 
        /// theres a menu item for each level, each one is tagged with a 
        /// string representing level number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsAddLevel_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            string level = tsmi.Tag as string;
            Skill s = GetSelectedSkill();
            if (s != null)
            {
                m_skillBrowser.Plan.PlanTo(s, Int32.Parse(level), true);
            }

        }

        /// <summary>
        ///  Sklll context menu - set the skill we're exploring to the
        /// selected skill in the skill tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsSwitch_Click(object sender, EventArgs e)
        {
            Skill s = GetSelectedSkill();
            if (s != null)
            {
                SetSkill(s);
                PopulateLists();
            }
        }

        /// <summary>
        /// Skill menu - show skill in browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsShowInBrowser_Click(object sender, EventArgs e)
        {
            Skill s = GetSelectedSkill();
            if (s != null)
            {
                NewPlannerWindow w = m_skillBrowser.Plan.PlannerWindow.Target as NewPlannerWindow;
                if (w != null)
                    w.ShowSkillInTree(s);
            }
        }

        /// <summary>
        /// Skill Menu - Show all prereq stats in a dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsShowPrereqs_Click(object sender, EventArgs e)
        {
            Skill s = GetSelectedSkill();
            if (s != null)
            {
                StringBuilder sb = new StringBuilder();
                int prNum = 0;

                foreach (Skill.Prereq pr in s.Prereqs)
                {
                    // get the skill instance for the current character to ensure the stats are right
                    Skill prs = m_characterInfo.GetSkill(pr.Name);
                    sb.Append(FormatPrereq(pr.Level, prs, ref prNum));
                }
                // shouldn't happen!
                if (sb.Length == 0) sb.Append("This skill has no untrained prerequisites\n");
                
                MessageBox.Show(sb.ToString(), "Untrained Prerequisites for " + s.Name,
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// We showing the shared ships/items context menu
        /// Use some helper methods to determine which
        /// and prepare the context menu for display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmEntity_Opening(object sender, CancelEventArgs e)
        {

            Ship s = null;
            Item item = null;
            SelectedNode(sender, out s, out item);
            if (s != null)
            {
                PrepareCm(s.RequiredSkills, "Ship");
            }
            else if (item != null)
            {
                PrepareCm(item.RequiredSkills, "Item");
            }
            else
            {
                e.Cancel = true;
            }

        }

        /// <summary>
        /// Shared context menu - add ship/item to plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
     
        private void tsAddEntityToPlan_Click(object sender, EventArgs e)
        {
            Ship s = null;
            Item item = null;
            SelectedNode(sender, out s, out item);

            // init
            string note = String.Empty;
            List<EntityRequiredSkill> prereqs = new List<EntityRequiredSkill>();

            // Get the name and prereqs for the ship or item
            if (s != null)
            {
                prereqs = s.RequiredSkills;
                note = s.Name;
            }
            else
            {
                prereqs = item.RequiredSkills;
                note = item.Name;
            }

            // Build list of prereqs to add (the add method works out if we know 
            // the skills or not)
            List<Pair<string, int>> skillsToAdd = new List<Pair<string, int>>();
            foreach (EntityRequiredSkill srs in prereqs)
            {
                skillsToAdd.Add(new Pair<string, int>(srs.Name, srs.Level));
            }

            m_skillBrowser.Plan.PlanSetTo(skillsToAdd, note, true);
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
            Ship s = null;
            Item item = null;
            SelectedNode(sender, out s, out item);

            List<EntityRequiredSkill> prereqs = null;
            string name = String.Empty;

            if (s != null)
            {
                name = s.Name;
                prereqs = s.RequiredSkills;
            }

            if (item != null)
            {
                name = item.Name;
                prereqs = item.RequiredSkills;
            }

            // we must have a prereqs now as context menu is only enabled on a valid node
            StringBuilder sb = new StringBuilder();
            int prNum = 0;

            foreach (EntityRequiredSkill pr in prereqs)
            {
                Skill prs = m_characterInfo.GetSkill(pr.Name);
                sb.Append(FormatPrereq(pr.Level, prs, ref prNum));
            }
            // shouldn't happen!
            if (sb.Length == 0) sb.Append("This skill has no untrained prerequisites\n");
            MessageBox.Show(sb.ToString(), "Untrained Prerequisites for " + name,
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

  
        #endregion // context menu

        #region static methods
        /// <summary>
        /// Determine if this skill is a ship skill
        /// All ship enabling skills are:
        ///  - All skills in the Command Ships group
        ///  - Astrogeology (for barges)
        ///  - Jump Drive Operation (capital ships)
        /// 
        /// None of these skills enable any items.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsShipSkill
        {
            get
            {
                return m_skill.SkillGroup.ID == COMMAND_SHIP_GROUP ||
                       m_skill.Id == ASTROGEOLOGY_SKILL ||
                       m_skill.Id == JUMPDRVE_OPERATION_SKILL;
            }
        }
        #endregion

        #region member fields

        // The skill we're exploring
        private Skill m_skill;

        // The parent SkillBrowser form (and the gateway to all other objects we need
        // such as the CharacterInfo, Character's skills, and the  plannerWindow

        private WeakReference<Form> m_plannerWindow;

      
        private SkillBrowser m_skillBrowser;

        // The parent Characetr
        private CharacterInfo m_characterInfo;

        // Convert numbers to roman numerals
        private static string[] roman = { "I", "II", "III", "IV", "V" };

        private static readonly int COMMAND_SHIP_GROUP = 257;
        private static readonly int ASTROGEOLOGY_SKILL = 3410;
        private static readonly int JUMPDRVE_OPERATION_SKILL = 3456;
        #endregion

    }
}
