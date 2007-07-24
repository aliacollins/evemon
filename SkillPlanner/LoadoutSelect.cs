using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Reflection;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class LoadoutSelect : Form
    {
        private Ship m_ship;

        public LoadoutSelect()
        {
            InitializeComponent();
        }

        public LoadoutSelect(Ship s)
        {
            m_ship = s;
            InitializeComponent();
        }

        private void LoadoutSelect_Load(object sender, EventArgs e)
        {
            lblShip.Text = "Fetching Loadouts for " + m_ship.Name;
            try
            {
                // fetch loadouts from battleclinic
                XmlSerializer xs = new XmlSerializer(typeof(ShipLoadout));
                XmlDocument doc = EVEMonWebRequest.LoadXml("http://www.battleclinic.com/eve_online/ship_loadout_feed.php?typeID=" + m_ship.Id);
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
                    lblShip.Text = "Found " + lvLoadouts.Items.Count.ToString() + " Loadouts for " + m_ship.Name + " - Click column headings to sort";
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

            btnOpen.Enabled = (lvLoadouts.SelectedItems.Count != 0);
            m_columnSorter = new LoadoutListSorter(this);
            lvLoadouts.ListViewItemSorter = m_columnSorter;
            lvLoadouts.Sort();
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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lvLoadouts_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            btnOpen.Enabled = (lvLoadouts.SelectedItems.Count != 0);
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

        #region Column Sorting

        private LoadoutListSorter m_columnSorter;

        public class LoadoutListSorter : IComparer
        {
            private ListView m_loadouts;
            public LoadoutListSorter(LoadoutSelect ls)
            {
                m_loadouts = ls.lvLoadouts;
                OrderOfSort = SortOrder.Descending;
                SortColumn = 3;
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
            }
            lvLoadouts.ListViewItemSorter = m_columnSorter;
            Cursor = Cursors.WaitCursor;
            lvLoadouts.Sort();
            Cursor = Cursors.Default;
        }

    #endregion

    }

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
}