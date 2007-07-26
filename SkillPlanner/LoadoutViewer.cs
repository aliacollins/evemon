
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using EVEMon.Common;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;


namespace EVEMon.SkillPlanner
{
    public partial class LoadoutViewer : Form
    {
        private SerializableLoadout m_loadout;
        private Plan m_plan;
        Dictionary<int, Item> m_cachedItems = new Dictionary<int, Item>();
        private Plan m_loadoutPlan = new Plan();
        private List<Pair<string, int>> m_skillsToAdd = new List<Pair<string, int>>();

        public LoadoutViewer()
        {
            InitializeComponent();
        }

        public LoadoutViewer(SerializableLoadout loadout,Plan plan)
        {
            m_loadout = loadout;
            m_plan = plan;
            InitializeComponent();
        }


        private void LoadoutViewer_Load(object sender, EventArgs e)
        {
            lblShip.Text = m_loadout.ShipObject.Name;
            lblName.Text = m_loadout.LoadoutName;
            lblAuthor.Text  = m_loadout.Author;
            lbDate.Text = m_loadout.SubmissionDate.ToShortDateString();
            m_loadoutPlan.GrandCharacterInfo = m_plan.GrandCharacterInfo;

            EveSession.GetImageAsync(
     "http://www.eve-online.com/bitmaps/icons/itemdb/shiptypes/256_256/" +
     m_loadout.ShipObject.Id.ToString() + ".png", true, delegate(EveSession ss, Image i)
                                           {
                                               GotShipImage(i);
                                           });

            // Add ship skills to requirements
             foreach (EntityRequiredSkill irs in m_loadout.ShipObject.RequiredSkills)
             {
                 m_skillsToAdd.Add(new Pair<string, int>(irs.Name, irs.Level));
             }


            // Now get the actual loadout 
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ShipLoadout));
                XmlDocument doc = EVEMonWebRequest.LoadXml("http://www.battleclinic.com/eve_online/ship_loadout_feed.php?id=" + m_loadout.LoadoutId);
                XmlElement shipNode = doc.DocumentElement.SelectSingleNode("//ship") as XmlElement;

                using (XmlNodeReader xnr = new XmlNodeReader(shipNode))
                {
                    ShipLoadout sl = (ShipLoadout)xs.Deserialize(xnr);
                 
                    // Get the Items 
                    ItemCategory items = ItemCategory.GetRootCategory();
                    // now build the tree
                    TreeNode n = null;
                    Dictionary<string, TreeNode> nodes = new Dictionary<string, TreeNode>();
                    Item slotItem = null;
                    foreach (SerializableLoadoutSlot sls in sl.Loadouts[0].Slots)
                    {
                        if (nodes.ContainsKey(sls.SlotType))
                        {
                            n = nodes[sls.SlotType];
                        }
                        else 
                        {
                            n = new TreeNode(sls.SlotType);
                            tvLoadout.Nodes.Add(n);
                            nodes.Add(sls.SlotType, n);
                        }
                        if (m_cachedItems.ContainsKey(sls.ItemId))
                        {
                            slotItem = m_cachedItems[sls.ItemId];
                        }
                        else
                        {
                            slotItem = ItemCategory.findItem(sls.ItemId);
                            if (slotItem != null)
                            {
                                m_cachedItems.Add(sls.ItemId, slotItem);
                                foreach (EntityRequiredSkill irs in slotItem.RequiredSkills)
                                {
                                    m_skillsToAdd.Add(new Pair<string, int>(irs.Name, irs.Level));
                                }
                            }
                        }
                        if (slotItem != null)
                        {
                            TreeNode slotNode = new TreeNode();
                            slotNode.Text = slotItem.Name;
                            slotNode.Tag = slotItem;
                            n.Nodes.Add(slotNode);
                        }
                    }

                }
                tvLoadout.ExpandAll();
                m_loadoutPlan.PlanSetTo(m_skillsToAdd, m_loadout.LoadoutName, false);
                lbTrainTime.Text = Skill.TimeSpanToDescriptiveText(m_loadoutPlan.TotalTrainingTime,
                                                                   DescriptiveTextOptions.IncludeCommas |
                                                                   DescriptiveTextOptions.SpaceText);
                SetPlanStatus();
                
            }
            catch (Exception)
            {
            }


        }

        private void SetPlanStatus()
        {
            if (m_loadoutPlan.UniqueSkillCount == 0)
            {
                btnPlan.Enabled = false;
                lblPlanned.Text = "All skills already known.";
            }
            else if (m_plan.SkillsetPlanned(m_skillsToAdd))
            {
                btnPlan.Enabled = false;
                lblPlanned.Text = "All skills already known or planned.";
            }
            else
            {
                btnPlan.Enabled = true;
                lblPlanned.Text = "";
            }
        }

        private void GotShipImage(Image i)
        {
            pbShip.SizeMode = PictureBoxSizeMode.StretchImage;
            pbShip.Image = i; 
        }

        private void btnPlan_Click(object sender, EventArgs e)
        {
            m_plan.PlanSetTo(m_skillsToAdd, m_loadout.LoadoutName, true);
            SetPlanStatus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void tvLoadout_DoubleClick(object sender, EventArgs e)
        {
            if (tvLoadout.SelectedNode != null)
            {
                Item itm = tvLoadout.SelectedNode.Tag as Item;
                NewPlannerWindow w = m_plan.PlannerWindow.Target as NewPlannerWindow;
                w.ShowItemInBrowser(itm);
            }
  
        }

        private void lblForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EveSession.BrowserLinkClicked("http://eve.battleclinic.com/forum/index.php/topic," + m_loadout.Topic + ".0.html");
        }

        private TreeNode m_OldSelectNode;
        // Thankyou MSDN for the following!
        private void tvLoadout_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = tvLoadout.GetNodeAt(p);
                if (node != null && node.Tag != null)
                {

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    m_OldSelectNode = tvLoadout.SelectedNode;
                    tvLoadout.SelectedNode = node;
                    cmNode.Show(tvLoadout,p);
                    // Highlight the selected node.
       //             tvLoadout.SelectedNode = m_OldSelectNode;
                    m_OldSelectNode = null;
                }
            }
        }


 
    }
}