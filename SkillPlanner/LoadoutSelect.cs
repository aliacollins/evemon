using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common;
using EVEMon.Common.Net;

namespace EVEMon.SkillPlanner
{
    public partial class LoadoutSelect : Form
    {
        private Ship m_ship;
        private Plan m_plan;
        private Settings m_settings;

        private Dictionary<string,List<string>> _currentLoadoutItems;

        public LoadoutSelect()
        {
            InitializeComponent();
            m_settings = Settings.GetInstance();
        }

        public LoadoutSelect(Ship s, Plan p) : this()
        {
            SetPlan(p);
            m_ship = s;
        }

        private void LoadoutSelect_Load(object sender, EventArgs e)
        {
            LoadShip();
        }

        public void SetShip(Ship s)
        {
            m_ship = s;
            LoadShip();
        }

        public void SetPlan(Plan p)
        {
            if (m_plan != null)
            {
                // Unsubscribe to PlanChanged
                m_plan.Changed -= new EventHandler<EventArgs>(PlanChanged);

            }
            m_plan = p;
            if (m_plan != null)
            {
                // Subscribe to PlanChanged
                m_plan.Changed += new EventHandler<EventArgs>(PlanChanged);
            }
        }

        private void PlanChanged(object sender, EventArgs e)
        {
            SetPlanStatus();
        }

        private void LoadShip()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            lblShipType.Text = m_ship.Name;
            lblName.Text = "No Loadout Selected";
            lblAuthor.Text = String.Empty;
            lbDate.Text = String.Empty;
            lblTrainTime.Text = "N/A";

            EveSession.GetImageAsync(
                "http://www.eve-online.com/bitmaps/icons/itemdb/shiptypes/256_256/" +
                m_ship.Id.ToString() + ".png", true, delegate(EveSession ss, Image i)
                               {
                                   GotShipImage(i);
                               });

            lblShip.Text = "Fetching Loadouts for " + m_ship.Name;
            lvLoadouts.Items.Clear();
            try
            {
                // fetch loadouts from battleclinic
                XmlSerializer xs = new XmlSerializer(typeof(ShipLoadout));
                XmlDocument doc = CommonContext.HttpWebService.DownloadXml(
                        "http://www.battleclinic.com/eve_online/ship_loadout_feed.php?typeID=" + m_ship.Id);
                XmlElement shipNode = doc.DocumentElement.SelectSingleNode("//ship") as XmlElement;
                if (shipNode != null)
                {
                    using (XmlNodeReader xnr = new XmlNodeReader(shipNode))
                    {
                        ShipLoadout sl = (ShipLoadout)xs.Deserialize(xnr);

                        foreach (SerializableLoadout loadout in sl.Loadouts)
                        {
                            loadout.ShipObject = m_ship;
                            ListViewItem lvi = new ListViewItem(loadout.LoadoutName);
                            lvi.Text = loadout.LoadoutName;
                            lvi.SubItems.Add(loadout.Author);
                            lvi.SubItems.Add(loadout.rating.ToString());
                            lvi.SubItems.Add(loadout.SubmissionDate.ToShortDateString());
                            lvi.Tag = loadout;
                            lvLoadouts.Items.Add(lvi);
                        }
                    }
                    lblShip.Text = "Found " + lvLoadouts.Items.Count.ToString() + " Loadouts";
                }
                else
                {
                    lblShip.Text = "There are no loadouts for " + m_ship.Name + ", why not submit one to Battleclinic?";
                }
            }
            catch (Exception)
            {
                lblShip.Text = "There was a problem connecting to Battleclinic, it may be down for maintainance.";
            }
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            btnLoad.Enabled = (lvLoadouts.SelectedItems.Count != 0);
            m_columnSorter = new LoadoutListSorter(this);
            lvLoadouts.ListViewItemSorter = m_columnSorter;
            lvLoadouts.Sort();
        }

        private void GotShipImage(Image i)
        {
            pbShip.SizeMode = PictureBoxSizeMode.StretchImage;
            pbShip.Image = i;
        }

        private SerializableLoadout m_selectedLoadout = null;

        public SerializableLoadout SelectedLoadout
        {
            get { return m_selectedLoadout; }
            set { m_selectedLoadout  = value; }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            m_selectedLoadout = lvLoadouts.SelectedItems[0].Tag as SerializableLoadout;
            LoadoutLoad();
        }

        private void lvLoadouts_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            btnLoad.Enabled = (lvLoadouts.SelectedItems.Count != 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_selectedLoadout = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void lvLoadouts_DoubleClick(object sender, EventArgs e)
        {
            if (lvLoadouts.SelectedItems.Count == 1)
            {
                btnOpen_Click(this, e);
            }

        }

        #region Loadout Viewer

        private List<Pair<string, int>> m_skillsToAdd = new List<Pair<string, int>>();
        private Plan m_loadoutPlan = new Plan();
        Dictionary<int, Item> m_cachedItems = new Dictionary<int, Item>();

        private void LoadoutLoad()
        {
            _currentLoadoutItems = new Dictionary<String, List<String>>();

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor; 

            m_skillsToAdd = new List<Pair<string, int>>();
            m_loadoutPlan = new Plan();
            m_loadoutPlan.GrandCharacterInfo = m_plan.GrandCharacterInfo;
            m_cachedItems = new Dictionary<int, Item>();

            // Set the headings
            lblName.Text = m_selectedLoadout.LoadoutName;
            lblAuthor.Text = m_selectedLoadout.Author;
            lbDate.Text = m_selectedLoadout.SubmissionDate.ToShortDateString();
            tvLoadout.Nodes.Clear();

            // Add ship skills to requirements
            foreach (EntityRequiredSkill irs in m_selectedLoadout.ShipObject.RequiredSkills)
            {
                m_skillsToAdd.Add(new Pair<string, int>(irs.Name, irs.Level));
            }

            // Now get the actual loadout 
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ShipLoadout));
                XmlDocument doc = CommonContext.HttpWebService.DownloadXml(
                        "http://www.battleclinic.com/eve_online/ship_loadout_feed.php?id=" + m_selectedLoadout.LoadoutId);
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

                            if (!_currentLoadoutItems.ContainsKey(sls.SlotType))
                            {
                                _currentLoadoutItems.Add(sls.SlotType, new List<String>());
                            }
                            _currentLoadoutItems[sls.SlotType].Add(slotItem.Name);
                        }
                    }

                }
                tvLoadout.ExpandAll();
                m_loadoutPlan.PlanSetTo(m_skillsToAdd, m_selectedLoadout.LoadoutName, false);
                lblTrainTime.Text = Skill.TimeSpanToDescriptiveText(m_loadoutPlan.TotalTrainingTime,
                                                                   DescriptiveTextOptions.IncludeCommas |
                                                                   DescriptiveTextOptions.SpaceText);
                SetPlanStatus();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default; 

            }
            catch (Exception)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default; 
            }


        }

        private void SetPlanStatus()
        {
            if (m_loadoutPlan.UniqueSkillCount == 0)
            {
                btnPlan.Enabled = false;
                lblPlanned.Visible = true;
                lblPlanned.Text = "All skills already known.";
                lblTrainTime.Visible = false;
            }
            else if (m_plan.SkillsetPlanned(m_skillsToAdd))
            {
                btnPlan.Enabled = false;
                lblPlanned.Visible = true;
                lblPlanned.Text = "All skills already known or planned.";
                lblTrainTime.Visible = false;
            }
            else
            {
                btnPlan.Enabled = true;
                lblPlanned.Text = "";
                lblPlanned.Visible = false;
                lblTrainTime.Visible = true;
            }
        }

        #endregion

        #region Column Sorting

        private LoadoutListSorter m_columnSorter;

        public class LoadoutListSorter : IComparer
        {

            private Settings m_settings;

            private ListView m_loadouts;
            public LoadoutListSorter(LoadoutSelect ls)
            {
                m_settings = Settings.GetInstance();
                m_loadouts = ls.lvLoadouts;
                SortColumn = m_settings.LoadoutSortColumn;
                if (m_settings.LoadoutSortAscending)
                {
                    OrderOfSort = SortOrder.Ascending;
                }
                else
                {
                    OrderOfSort = SortOrder.Descending;
                }
            }

            private int m_sortColumn;
            public int SortColumn
            {
                get { return m_sortColumn; }
                set { m_sortColumn = value; }
            }

            public int Compare(object x, object y)
            {
                int compareResult = 0;
                ListViewItem a = (ListViewItem)x;
                ListViewItem b = (ListViewItem)y;

                if (m_sortOrder == SortOrder.Descending)
                {
                    ListViewItem tmp = b;
                    b = a;
                    a = tmp;
                }

                SerializableLoadout sla = a.Tag as SerializableLoadout;
                SerializableLoadout slb = b.Tag as SerializableLoadout;

                switch (m_sortColumn)
                {
                    case 0: // sort by name
                        compareResult = String.Compare(a.Text, b.Text);
                        break;
                    case 1: // Author
                        compareResult = String.Compare(a.SubItems[1].Text, b.SubItems[1].Text);
                        break;
                    case 2:  // Rating
                        if (sla.rating < slb.rating) compareResult = -1;
                        else if (sla.rating > slb.rating) compareResult = 1;
                        else compareResult = 0;
                        break;
                    case 3:  // Date
                        compareResult = sla.SubmissionDate.CompareTo(slb.SubmissionDate);
                        break;
                }

                return compareResult;
            }


            private SortOrder m_sortOrder = SortOrder.None;

            public SortOrder OrderOfSort
            {
                get { return m_sortOrder; }
                set { m_sortOrder = value; }
            }
        }

        private void lvLoadouts_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == m_columnSorter.SortColumn)
            {
                // already sorting on this column so swap sort order
                m_settings.LoadoutSortAscending = !m_settings.LoadoutSortAscending;
                if (m_columnSorter.OrderOfSort == SortOrder.Ascending)
                {
                    m_columnSorter.OrderOfSort = SortOrder.Descending;
                }
                else
                {
                    m_columnSorter.OrderOfSort = SortOrder.Ascending;
                }
                
            }
            else
            {
                m_columnSorter.SortColumn = e.Column;
                m_columnSorter.OrderOfSort = SortOrder.Ascending;
                m_settings.LoadoutSortAscending = true;
                m_settings.LoadoutSortColumn = e.Column;
            }
            lvLoadouts.ListViewItemSorter = m_columnSorter;
            Cursor = Cursors.WaitCursor;
            lvLoadouts.Sort();
            Cursor = Cursors.Default;
        }

        private void lblForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (m_selectedLoadout != null)
            {
                EveSession.BrowserLinkClicked("http://eve.battleclinic.com/forum/index.php/topic," + m_selectedLoadout.Topic + ".0.html");
            }
            else
            {
                MessageBox.Show("Please select a loadout to discuss.", "No Loadout Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void btnPlan_Click(object sender, EventArgs e)
        {
            m_plan.PlanSetTo(m_skillsToAdd, m_selectedLoadout.LoadoutName, true);
            SetPlanStatus();
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
                    cmNode.Show(tvLoadout, p);
                    // Highlight the selected node.
                    //             tvLoadout.SelectedNode = m_OldSelectNode;
                    m_OldSelectNode = null;
                }
            }
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

    #endregion

        private void LoadoutSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unsubscribe to events
            if (m_plan != null) { m_plan.Changed -= new EventHandler<EventArgs>(PlanChanged); }
            NewPlannerWindow w = m_plan.PlannerWindow.Target as NewPlannerWindow;
            try
            {
                w.LoadoutForm = null;
            }
            catch { }
        }

        private void miExportToEFT_Click(object sender, EventArgs e)
        {
            foreach(EntityProperty prop in m_ship.Properties)
            {
                if (prop.Name.Contains("High Slots"))
                {
                    int highSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", ""));
                    while (_currentLoadoutItems.ContainsKey("high") && _currentLoadoutItems["high"].Count < highSlots)
                    {
                        _currentLoadoutItems["high"].Add("[empty high slot]");
                    }
                }
                else if (prop.Name.Contains("Med Slots"))
                {
                    int medSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", ""));
                    while (_currentLoadoutItems.ContainsKey("med") && _currentLoadoutItems["med"].Count < medSlots)
                    {
                        _currentLoadoutItems["med"].Add("[empty med slot]");
                    }
                }
                else if (prop.Name.Contains("Low Slots"))
                {
                    int lowSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", ""));
                    while (_currentLoadoutItems.ContainsKey("lo") && _currentLoadoutItems["lo"].Count < lowSlots)
                    {
                        _currentLoadoutItems["lo"].Add("[empty low slot]");
                    }
                }
                else if (prop.Name.Contains("Rig Slots"))
                {
                    int rigsSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", ""));
                    while (_currentLoadoutItems.ContainsKey("rig") && _currentLoadoutItems["rig"].Count < rigsSlots)
                    {
                        _currentLoadoutItems["rig"].Add("[empty rig slot]");
                    }
                }
            }

            StringBuilder exportText = new StringBuilder();
            exportText.AppendLine("["+m_ship.Name+", EVEMON "+lblName.Text+"]" );

            if (_currentLoadoutItems.ContainsKey("lo"))
            {
                exportText.AppendLine(String.Join(Environment.NewLine, _currentLoadoutItems["lo"].ToArray()));    
            }
            if (_currentLoadoutItems.ContainsKey("med"))
            {
                exportText.AppendLine(String.Join(Environment.NewLine, _currentLoadoutItems["med"].ToArray()));
            }
            if (_currentLoadoutItems.ContainsKey("high"))
            {
                exportText.AppendLine(String.Join(Environment.NewLine, _currentLoadoutItems["high"].ToArray()));
            }
            if (_currentLoadoutItems.ContainsKey("rig"))
            {
                exportText.AppendLine(String.Join(Environment.NewLine, _currentLoadoutItems["rig"].ToArray()));
            }
            if (_currentLoadoutItems.ContainsKey("drone"))
            {
                foreach (String s in _currentLoadoutItems["drone"])
                {
                    exportText.AppendLine(s + " x1");
                }
            }
            Clipboard.Clear();
            Clipboard.SetText(exportText.ToString());
        }
    }

    #region XML Loadout
    [XmlRoot("ship")]
    public class ShipLoadout
    {
        private List<SerializableLoadout> m_loadouts = new List<SerializableLoadout>();
        [XmlElement("loadout")]
        public List<SerializableLoadout> Loadouts
        {
            get { return m_loadouts; }
            set { m_loadouts = value; }
        }

    }

    [XmlRoot("loadout")]
    public class SerializableLoadout
    {
        private string m_loadoutName;
        [XmlAttribute("name")]
        public string LoadoutName
        {
            get { return m_loadoutName; }
            set { m_loadoutName = value; }
        }

        private string  m_author;
        [XmlAttribute("Author")]
        public string  Author
        {
            get { return m_author; }
            set { m_author = value; }
        }

        private double m_rating;

        [XmlAttribute("rating")]
        public double rating
        {
            get { return m_rating; }
            set { m_rating = value; }
        }

        private string m_loadoutID;
                        
        [XmlAttribute("loadoutID")]
        public string LoadoutId
        {
            get { return m_loadoutID; }
            set { m_loadoutID = value; }
        }

        private string m_submissionDateString = null;
        private DateTime m_submissionDate;

        [XmlAttribute("date")]
        public string SubmissionDateString
        {
            get { return m_submissionDateString; }
            set 
            {
                m_submissionDateString = value;
               m_submissionDate=DateTime.Parse(value);
            }
        }

        private int m_topic;

        [XmlAttribute("topic")]
        public int Topic
        {
            get { return m_topic; }
            set { m_topic = value; }
        }
	

        [XmlIgnore]
        public DateTime SubmissionDate
        {
            get { return m_submissionDate; }
            set { m_submissionDate = value; }
        }

        private List<SerializableLoadoutSlot> m_slots = new List<SerializableLoadoutSlot>();
        [XmlElement("slot")]
        public List<SerializableLoadoutSlot> Slots
        {
            get { return m_slots; }
            set { m_slots = value; }
        }


        private Ship m_ship;
        [XmlIgnore]
        public Ship ShipObject
        {
            get { return m_ship; }
            set { m_ship = value; }
        }


    }

    [XmlRoot("slot")]
    public class SerializableLoadoutSlot
    {
        private string m_type;
        [XmlAttribute("type")]
        public string SlotType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private string m_position;
        [XmlAttribute("position")]
        public string SlotPosition
        {
            get { return m_position; }
            set { m_position = value; }
        }

        private int m_itemId;
        [XmlText]
        public int ItemId
        {
            get { return m_itemId; }
            set { m_itemId = value; }
        }

    }
    #endregion
}
