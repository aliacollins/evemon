using System;
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
        private string m_shipName;
        private int m_shipID;

        public LoadoutSelect()
        {
            InitializeComponent();
        }

        public LoadoutSelect(string shipName, int shipID)
        {
            m_shipName = shipName;
            m_shipID = shipID;
            InitializeComponent();
        }

        private void LoadoutSelect_Load(object sender, EventArgs e)
        {
            lblShip.Text = "Fetching Loadouts for " + m_shipName;
            try
            {
                // fetch loadouts from battleclinic
                XmlSerializer xs = new XmlSerializer(typeof(ShipLoadout));
                XmlDocument doc = EVEMonWebRequest.LoadXml("http://www.battleclinic.com/eve_online/ship_loadout_feed.php?typeID=" + m_shipID);
                XmlElement shipNode = doc.DocumentElement.SelectSingleNode("//ship") as XmlElement;
                if (shipNode != null)
                {
                    using (XmlNodeReader xnr = new XmlNodeReader(shipNode))
                    {
                        ShipLoadout sl = (ShipLoadout)xs.Deserialize(xnr);

                        foreach (SerializableLoadout loadout in sl.Loadouts)
                        {
                            loadout.TypeID = m_shipID;
                            loadout.ShipClass = m_shipName;
                            ListViewItem lvi = new ListViewItem(loadout.LoadoutName);
                            lvi.Text = loadout.LoadoutName;
                            lvi.SubItems.Add(loadout.Author);
                            lvi.SubItems.Add(loadout.rating.ToString());
                            lvi.Tag = loadout;
                            lvLoadouts.Items.Add(lvi);
                        }
                    }
                    lblShip.Text = "Found " + lvLoadouts.Items.Count.ToString() + " Loadouts for " + m_shipName;
                }
                else
                {
                    lblShip.Text = "There are no loadouts for " + m_shipName + ", why not submit one to Battleclinic?";
                }
            }
            catch (Exception ex)
            {
                lblShip.Text = "There was a problem connecting to Battleclinic, it may be down for maintainance.";
            }

            btnOpen.Enabled = (lvLoadouts.SelectedItems.Count != 0);
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

        private List<SerializableLoadoutSlot> m_slots = new List<SerializableLoadoutSlot>();
        [XmlElement("slot")]
        public List<SerializableLoadoutSlot> Slots
        {
            get { return m_slots; }
            set { m_slots = value; }
        }

        private string m_shipClass;
        
        [XmlIgnore]
        public string ShipClass
        {
            get { return m_shipClass; }
            set { m_shipClass = value; }
        }

        private int m_typeID;

        [XmlIgnore]
        public int TypeID
        {
            get { return m_typeID; }
            set { m_typeID = value; }
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