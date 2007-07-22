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

        public LoadoutViewer()
        {
            InitializeComponent();
        }

        public LoadoutViewer(SerializableLoadout loadout)
        {
            m_loadout = loadout;
            InitializeComponent();
        }


        private void LoadoutViewer_Load(object sender, EventArgs e)
        {
            lblShip.Text = m_loadout.ShipClass;
            lblName.Text = m_loadout.LoadoutName;
            lblAuthor.Text  = m_loadout.Author;
            EveSession.GetImageAsync(
     "http://www.eve-online.com/bitmaps/icons/itemdb/shiptypes/256_256/" +
     m_loadout.TypeID.ToString() + ".png", true, delegate(EveSession ss, Image i)
                                           {
                                               GotShipImage(i);
                                           });


            // Now get the actual loadout 
            // TODO - refactor this to another class
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
                    Dictionary<int,Item> cachedItems = new Dictionary<int,Item>();
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
                        if (cachedItems.ContainsKey(sls.ItemId))
                        {
                            slotItem = cachedItems[sls.ItemId];
                        }
                        else
                        {
                             slotItem = ItemCategory.findItem(sls.ItemId);
                             cachedItems.Add(sls.ItemId,slotItem);
                        }
                        TreeNode slotNode = new TreeNode();
                        slotNode.Text = slotItem.Name;
                        slotNode.Tag = slotItem;
                        n.Nodes.Add(slotNode);
                    }

                }
                tvLoadout.ExpandAll();

            }
            catch (Exception)
            {
            }


        }

        private void GotShipImage(Image i)
        {
            pbShip.SizeMode = PictureBoxSizeMode.StretchImage;
            pbShip.Image = i; 
        }

        private void btnPlan_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}