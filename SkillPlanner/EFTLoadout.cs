using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Presents the user with an interface in which an EFT exported
    /// ship loadout can be pasted. When given a meaningful ship
    /// loadout is presented the form then parses it into a) a data 
    /// structure that can be passed to the provided and b) a TreeView
    /// which can be used to browse the items in the loadout and to
    /// check if the loadout has been correctly parsed.
    /// </summary>
    public partial class EFTLoadout : Form
    {
        /// <summary>
        /// Holds the skills extracted from the parsed loadout.
        /// </summary>
        private List<Pair<string, int>> m_skillsToAdd = new List<Pair<string, int>>();

        /// <summary>
        /// A temporary Plan used internally to calculate the remaining
        /// training time without actually modifying the users'
        /// selected plan.
        /// </summary>
        private Plan m_loadoutPlan = new Plan();

        /// <summary>
        /// The Plan to which the extracted skills will be added. The
        /// Plan is also monitored for changes. If the Plan changes the
        /// remaining training time is updated accordingly.
        /// </summary>
        private Plan m_plan;

        /// <summary>
        /// Gets or sets the Plan to which the extracted skills of the
        /// loadout should be added.
        /// </summary>
        public Plan Plan
        {
            get
            {
                return m_plan;
            }
            set
            {
                if (m_plan != null)
                {
                    m_plan.Changed -= PlanChanged;
                }
                m_plan = value;
                if (m_plan != null)
                {
                    m_plan.Changed += PlanChanged;
                }
            }
        }

        /// <summary>
        /// Default constructor for the EFTLoadout form, it initializes
        /// the form and sets the contained controls to their default
        /// values.
        /// </summary>
        public EFTLoadout()
        {
            InitializeComponent();
            SetPlanStatus();
        }

        /// <summary>
        /// Updates the form state after changes to the Plan have been
        /// detected.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void PlanChanged(object sender, EventArgs e)
        {
            SetPlanStatus();
        }

        /// <summary>
        /// Parses the content of LoadoutTextBox into usable data structures.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tbEFTLoadout_TextChanged(object sender, EventArgs e)
        {
            m_skillsToAdd = new List<Pair<string, int>>();
            m_loadoutPlan = new Plan
                               {
                                   GrandCharacterInfo = Plan.GrandCharacterInfo
                               };

            ResultsTreeView.Nodes.Clear();

            for (int i = 0; i < PasteTextBox.Lines.Length; i++)
            {
                string line = PasteTextBox.Lines[i];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (i==0)
                {
                    if (!line.StartsWith("[") || !line.Contains(","))
                    {
                        ResultsTreeView.Nodes.Add("Cannot determine ship type");
                        ResultsTreeView.Enabled = false;
                        return;
                    }

                    int m_lineLength = line.Length;
                    int m_commaIndex = line.IndexOf(',');
                    string shipTypeName = line.Substring(1, m_commaIndex - 1);
                    m_loadoutPlan.Name = line.Substring(m_commaIndex + 1, (m_lineLength - m_commaIndex - 2));

                    Ship ship = Array.Find(Ship.GetShips(), delegate(Ship s) { return s.Name == shipTypeName; });

                    if (ship != null)
                    {
                        foreach (EntityRequiredSkill requiredSkill in ship.RequiredSkills)
                        {
                            m_skillsToAdd.Add(new Pair<string, int>(requiredSkill.Name, requiredSkill.Level));
                        }
                    }
                    else
                    {
                        ResultsTreeView.Nodes.Add("Cannot determine ship type");
                        ResultsTreeView.Enabled = false;
                        return;
                    }

                    continue;
                }
                
                AddItem(line);
            }

            ResultsTreeView.ExpandAll();
            m_loadoutPlan.PlanSetTo(m_skillsToAdd, m_loadoutPlan.Name, false);
            TrainTimeLabel.Text = Skill.TimeSpanToDescriptiveText(m_loadoutPlan.TotalTrainingTime,
                                                                   DescriptiveTextOptions.IncludeCommas |
                                                                   DescriptiveTextOptions.SpaceText);
            SetPlanStatus();
            ResultsTreeView.Enabled = true;
            Cursor.Current = Cursors.Default; 
        }

        /// <summary>
        /// Parses one line of loadout text and adds the required
        /// skills for the items to the _SkillsToAdd list.
        /// </summary>
        /// <remarks>
        /// parsed items are also added to the TreeView.
        /// </remarks>
        /// <param name="line">Line of text to be parsed.</param>
        private void AddItem(string line)
        {
            string itemName = line.Contains(",")?line.Substring(0, line.LastIndexOf(',')):line;
            string chargeName = line.Contains(",")?line.Substring(line.LastIndexOf(',') + 2):null;

            Item item = ItemCategory.findItem(itemName);
            Item charge = !string.IsNullOrEmpty(chargeName)?ItemCategory.findItem(chargeName):null;
            
            if (item != null)
            {
                TreeNode node = null;
                if (item.TechLevel == 3)
                {
                    node = !ResultsTreeView.Nodes.ContainsKey("Subsystem") ? ResultsTreeView.Nodes.Add("Subsystem", "Subsystem") : ResultsTreeView.Nodes["Subsystem"];
                }
                else
                {
                    switch (item.SlotIndex)
                    {
                        case 1:
                            // High
                            node = !ResultsTreeView.Nodes.ContainsKey("High") ? ResultsTreeView.Nodes.Add("High", "High") : ResultsTreeView.Nodes["High"];
                            break;
                        case 2:
                            // Med
                            node = !ResultsTreeView.Nodes.ContainsKey("Med") ? ResultsTreeView.Nodes.Add("Med", "Med") : ResultsTreeView.Nodes["Med"];
                            break;
                        case 3:
                            // Low
                            node = !ResultsTreeView.Nodes.ContainsKey("Low") ? ResultsTreeView.Nodes.Add("Low", "Low") : ResultsTreeView.Nodes["Low"];
                            break;
                        case 0:
                            //Rig
                            node = !ResultsTreeView.Nodes.ContainsKey("Rigs") ? ResultsTreeView.Nodes.Add("Rigs", "Rigs") : ResultsTreeView.Nodes["Rigs"];
                            break;
                    }
                }
                
                foreach (EntityRequiredSkill requiredSkill in item.RequiredSkills)
                {
                    m_skillsToAdd.Add(new Pair<string, int>(requiredSkill.Name,requiredSkill.Level));
                }
                TreeNode slotNode = new TreeNode();
                slotNode.Text = item.Name;
                slotNode.Tag = item;
                node.Nodes.Add(slotNode);
            }
            else
            {
                // Might be drone
                itemName = line.Contains(" x") ? line.Substring(0, line.LastIndexOf(" x")) : line;
                item = ItemCategory.findItem(itemName);
                if (item != null)
                {
                    foreach (EntityRequiredSkill requiredSkill in item.RequiredSkills)
                    {
                        m_skillsToAdd.Add(new Pair<string, int>(requiredSkill.Name, requiredSkill.Level));
                    }
                    TreeNode node = !ResultsTreeView.Nodes.ContainsKey("Drones") ? ResultsTreeView.Nodes.Add("Drones", "Drones") : ResultsTreeView.Nodes["Drones"];
                    TreeNode slotNode = new TreeNode();
                    slotNode.Text = item.Name;
                    slotNode.Tag = item;
                    node.Nodes.Add(slotNode);
                }
            }
            if (charge != null)
            {
                TreeNode node = !ResultsTreeView.Nodes.ContainsKey("Ammunition") ? ResultsTreeView.Nodes.Add("Ammunition", "Ammunition") : ResultsTreeView.Nodes["Ammunition"];
                foreach (EntityRequiredSkill requiredSkill in charge.RequiredSkills)
                {
                    m_skillsToAdd.Add(new Pair<string, int>(requiredSkill.Name,requiredSkill.Level));
                }
                TreeNode ammoNode = new TreeNode();
                ammoNode.Text = charge.Name;
                ammoNode.Tag = charge;
                node.Nodes.Add(ammoNode);
            }

            
        }

        /// <summary>
        /// Sets the DialogResult to Cancel and closes the form.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Updates the form controls to reflect the status of the Plan specified by the Plan property.
        /// </summary>
        private void SetPlanStatus()
        {
            if (m_loadoutPlan.UniqueSkillCount == 0)
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = "All skills already known.";
                TrainTimeLabel.Visible = false;
            }
            else if (Plan.SkillsetPlanned(m_skillsToAdd))
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = "All skills already known or planned.";
                TrainTimeLabel.Visible = false;
            }
            else
            {
                AddToPlanButton.Enabled = true;
                PlanedLabel.Text = "";
                PlanedLabel.Visible = false;
                TrainTimeLabel.Visible = true;
            }
        }

        /// <summary>
        /// Adds the required skills to the Plan specified by the Plan property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlan_Click(object sender, EventArgs e)
        {
            Plan.PlanSetTo(m_skillsToAdd, m_loadoutPlan.Name, true);
            SetPlanStatus();
        }

        /// <summary>
        /// Browses the form that opened this instance of EFTLoadout to
        /// the item that was double clicked in the TreeView.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tvLoadout_DoubleClick(object sender, EventArgs e)
        {
            if (ResultsTreeView.SelectedNode != null)
            {
                Item itm = ResultsTreeView.SelectedNode.Tag as Item;
                NewPlannerWindow opener = m_plan.PlannerWindow.Target as NewPlannerWindow;
                opener.ShowItemInBrowser(itm);
            }
        }

        /// <summary>
        /// Pops up the context menu for the TreeView.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tvLoadout_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = ResultsTreeView.GetNodeAt(p);
                if (node != null && node.Tag != null)
                {
                    // Select the node the user has clicked.
                    ResultsTreeView.SelectedNode = node;
                    RightClickContextMenuStrip.Show(ResultsTreeView, p);
                }
            }
        }
    }
}