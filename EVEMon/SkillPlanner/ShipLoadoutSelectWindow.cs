using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.BattleClinic;
using SortOrder = System.Windows.Forms.SortOrder;

namespace EVEMon.SkillPlanner
{
    public partial class ShipLoadoutSelectWindow : EVEMonForm
    {

        // TODO : This class needs totally re-writing to split the data components from the UI components.

        private Item m_ship;
        private Plan m_plan;
        private readonly Character m_character;
        private SerializableLoadout m_selectedLoadout;
        private readonly List<StaticSkillLevel> m_prerequisites = new List<StaticSkillLevel>();
        private readonly LoadoutListSorter m_columnSorter;
        private static readonly Dictionary<string, string> s_typeMap = new Dictionary<string, string>();


        #region Initialization, loading, closing

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipLoadoutSelectWindow"/> class.
        /// </summary>
        private ShipLoadoutSelectWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="plan"></param>
        public ShipLoadoutSelectWindow(Item ship, Plan plan)
            : this()
        {
            persistentSplitContainer1.RememberDistanceKey = "ShipLoadoutBrowser";

            m_character = (Character)plan.Character;
            m_plan = plan;
            m_ship = ship;

            m_columnSorter = new LoadoutListSorter { OrderOfSort = SortOrder.Descending, SortColumn = 2 };
            lvLoadouts.ListViewItemSorter = m_columnSorter;
        }

        /// <summary>
        /// On load, download the loadouts feed for this ship
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadoutSelect_Load(object sender, EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted())
                return;

            s_typeMap["high"] = "High Slots";
            s_typeMap["med"] = "Medium Slots";
            s_typeMap["lo"] = "Low Slots";
            s_typeMap["rig"] = "Rig Slots";
            s_typeMap["drone"] = "Drones";
            s_typeMap["ammo"] = "Ammo";
            s_typeMap["subSystem"] = "SubSystem";

            // Subscribe global events
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;

            QueryLoadoutsFeed();
        }

        /// <summary>
        /// Query the loadouts feed for the current ship.
        /// </summary>
        private void QueryLoadoutsFeed()
        {
            // Wait cursor until we retrieved the loadout.
            Cursor.Current = Cursors.WaitCursor;

            //Download the eve image
            eveImage.EveItem = m_ship;

            // Download the loadouts feed
            string url = String.Format(CultureConstants.DefaultCulture, NetworkConstants.BattleclinicLoadoutsFeed,
                                       m_ship.ID.ToString());
            Util.DownloadXMLAsync<SerializableLoadoutFeed>(url, null, OnLoadoutFeedDownloaded);

            // Set labels while the user wait
            lblShipName.Text = m_ship.Name;
            lblLoadoutName.Text = "No Loadout Selected";
            lblAuthor.Text = String.Empty;
            lblSubmitDate.Text = String.Empty;
            lblTrainTime.Text = "N/A";
            lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture, "Fetching loadouts for {0}", m_ship.Name);
            btnPlan.Enabled = false;
        }

        /// <summary>
        /// When the window is closing, we clean things up and notify the parent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadoutSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unsubscribe global events
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
        }

        /// <summary>
        /// Gets or sets the plan.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan == value)
                    return;

                m_plan = value;
                UpdatePlanningControls();
            }
        }

        /// <summary>
        /// Gets or sets the ship.
        /// </summary>
        public Item Ship
        {
            get { return m_ship; }
            set
            {
                if (m_ship == value)
                    return;

                m_ship = value;
                QueryLoadoutsFeed();
            }
        }

        #endregion


        #region Downloads

        /// <summary>
        /// Occurs when we downloaded a loadouts feed from BattleClinic
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private void OnLoadoutFeedDownloaded(SerializableLoadoutFeed feed, string errorMessage)
        {
            if (IsDisposed)
                return;

            // Restore the default cursor instead of the waiting one
            Cursor.Current = Cursors.Default;
            m_selectedLoadout = null;
            btnPlan.Enabled = false;

            // Was there an error ?
            if (!String.IsNullOrEmpty(errorMessage))
            {
                lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture,
                                                 "There was a problem connecting to BattleClinic, it may be down for maintainance.\r\n{0}",
                                                 errorMessage);
                return;
            }

            // Are there no feeds ?
            if (feed.Race == null || feed.Race.Loadouts.Length == 0)
            {
                lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture,
                                                 "There are no loadouts for {0}, why not submit one to BattleClinic?", m_ship.Name);
                return;
            }

            // Add the listview items for every loadout
            lvLoadouts.Items.Clear();
            foreach (SerializableLoadout loadout in feed.Race.Loadouts)
            {
                ListViewItem lvi = new ListViewItem(loadout.LoadoutName) { Text = loadout.LoadoutName };
                lvi.SubItems.Add(loadout.Author);
                lvi.SubItems.Add(loadout.Rating.ToString());
                lvi.SubItems.Add(loadout.SubmissionDate.ToString());
                lvi.Tag = loadout;
                lvLoadouts.Items.Add(lvi);
            }

            // Update the header
            lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture, "Found {0} loadouts", lvLoadouts.Items.Count);

            // Update the listview's comparer and sort
            lvLoadouts.Sort();
        }

        /// <summary>
        /// Downloads the given loadout
        /// </summary>
        /// <param name="loadout"></param>
        private void DownloadLoadout(SerializableLoadout loadout)
        {
            // See the cursor to wait
            Cursor.Current = Cursors.WaitCursor;

            // Retrieve the selected loadout
            m_selectedLoadout = loadout;

            // Set the headings
            lblLoadoutName.Text = m_selectedLoadout.LoadoutName;
            lblAuthor.Text = m_selectedLoadout.Author;
            lblSubmitDate.Text = m_selectedLoadout.SubmissionDate.ToString();

            // Download the loadout details
            string url = String.Format(CultureConstants.DefaultCulture, NetworkConstants.BattleclinicLoadoutDetails,
                                       m_selectedLoadout.LoadoutId);
            Util.DownloadXMLAsync<SerializableLoadoutFeed>(url, null, OnLoadoutDownloaded);
        }

        /// <summary>
        /// Occurs when we downloaded a loadout from BattleClinic
        /// </summary>
        /// <param name="loadoutFeed"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private void OnLoadoutDownloaded(SerializableLoadoutFeed loadoutFeed, string errorMessage)
        {
            if (IsDisposed)
                return;

            // Reset the controls
            btnPlan.Enabled = false;
            m_prerequisites.Clear();
            tvLoadout.Nodes.Clear();
            Cursor.Current = Cursors.Default;

            // Was there an error ?
            if (!String.IsNullOrEmpty(errorMessage) || loadoutFeed.Race.Loadouts.Length == 0)
            {
                lblTrainTime.Text = String.Format(CultureConstants.DefaultCulture, "Couldn't download that loadout.\r\n{0}",
                                                  errorMessage);
                lblTrainTime.Visible = true;
                return;
            }

            SerializableLoadout loadout = loadoutFeed.Race.Loadouts[0];

            // Fill the items tree
            IEnumerable<IGrouping<string, SerializableLoadoutSlot>> slotTypes = loadout.Slots.GroupBy(x => x.SlotType);
            foreach (var slotType in slotTypes)
            {
                TreeNode typeNode = new TreeNode(s_typeMap[slotType.Key]);

                foreach (var slot in slotType)
                {
                    Item item = StaticItems.GetItemByID(slot.ItemID);
                    if (item == null)
                        continue;

                    TreeNode slotNode = new TreeNode { Text = item.Name, Tag = item };
                    typeNode.Nodes.Add(slotNode);

                    m_prerequisites.AddRange(item.Prerequisites);
                }

                tvLoadout.Nodes.Add(typeNode);
            }

            // Compute the training time
            UpdatePlanningControls();
            tvLoadout.ExpandAll();
        }

        /// <summary>
        /// Update the plan status : training time, skills already trained, etc
        /// </summary>
        private void UpdatePlanningControls()
        {
            // Are all the prerequisites trained ?
            if (m_prerequisites.All(x => m_character.GetSkillLevel(x.Skill) >= x.Level))
            {
                btnPlan.Enabled = false;
                lblPlanned.Visible = true;
                lblPlanned.Text = "All skills already trained.";
                lblTrainTime.Visible = false;
                return;
            }

            // Are all the prerequisites planned ?
            if (m_plan.AreSkillsPlanned(m_prerequisites.Where(x => m_character.Skills[x.Skill].Level < x.Level)))
            {
                btnPlan.Enabled = false;
                lblPlanned.Visible = true;
                lblPlanned.Text = "All skills already trained or planned.";
                lblTrainTime.Visible = false;
                return;
            }

            // Compute the training time
            CharacterScratchpad scratchpad = new CharacterScratchpad(m_character);
            foreach (var entry in m_plan)
            {
                scratchpad.Train(entry);
            }

            TimeSpan startTime = scratchpad.TrainingTime;
            foreach (StaticSkillLevel prereq in m_prerequisites)
            {
                scratchpad.Train(prereq);
            }

            TimeSpan trainingTime = scratchpad.TrainingTime.Subtract(startTime);

            // update the labels
            btnPlan.Enabled = true;
            lblPlanned.Text = String.Empty;
            lblPlanned.Visible = false;
            lblTrainTime.Visible = true;
            lblTrainTime.Text = trainingTime.ToDescriptiveText(
                DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.SpaceText);
        }

        #endregion


        #region Global events

        /// <summary>
        /// Occurs when the plan changed. We update the status of the training time and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            if (e.Plan == m_plan)
                UpdatePlanningControls();
        }

        #endregion


        #region Controls' events handlers

        /// <summary>
        /// When the user double-click a loadout, we download it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvLoadouts_Click(object sender, EventArgs e)
        {
            if (lvLoadouts.SelectedItems.Count == 0)
                return;

            SerializableLoadout loadout = lvLoadouts.SelectedItems[0].Tag as SerializableLoadout;
            DownloadLoadout(loadout);
        }

        /// <summary>
        /// When the user clicks cancel, we quit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_selectedLoadout = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// When the user clicks one of the loadouts list' column, we update the sort.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvLoadouts_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Is the column we're already sorting by ? Then swap sort order
            if (e.Column == m_columnSorter.SortColumn)
            {
                m_columnSorter.OrderOfSort = (m_columnSorter.OrderOfSort == SortOrder.Ascending
                                                  ? SortOrder.Descending
                                                  : SortOrder.Ascending);
            }
                // Then the user wants to sort by a different column
            else
            {
                m_columnSorter.SortColumn = e.Column;
                m_columnSorter.OrderOfSort = SortOrder.Ascending;
            }

            // Sort
            lvLoadouts.ListViewItemSorter = m_columnSorter;
            lvLoadouts.Sort();
        }

        /// <summary>
        /// When the user clicks the "discuss this loadout", we open the browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (m_selectedLoadout != null)
            {
                Util.OpenURL(String.Format(CultureConstants.DefaultCulture, NetworkConstants.BattleclinicLoadoutTopic,
                                           m_selectedLoadout.Topic.ToString()));
            }
            else
            {
                MessageBox.Show("Please select a loadout to discuss.", "No Loadout Selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// When the user clicks the "plan" button, we add the prerqs to the plan and refresh the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlan_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryAddSet(m_prerequisites, m_selectedLoadout.LoadoutName);
            PlanHelper.Perform(operation);
            UpdatePlanningControls();
        }

        /// <summary>
        /// When the user right-click an item, we display a context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvLoadout_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button != MouseButtons.Right)
                return;

            // Point where the mouse is clicked.
            Point p = new Point(e.X, e.Y);

            // Get the node that the user has clicked.
            TreeNode node = tvLoadout.GetNodeAt(p);
            if (node == null || node.Tag == null)
                return;

            // Select the node the user has clicked.
            // The node appears selected until the menu is displayed on the screen.
            tvLoadout.SelectedNode = node;
            cmNode.Show(tvLoadout, p);
        }

        /// <summary>
        /// When the user double-click an item or uses the "Show
        /// in item browser" context menu item, we open the items
        /// browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvLoadout_DoubleClick(object sender, EventArgs e)
        {
            // user double clicked an area that isn't a node
            if (tvLoadout.SelectedNode == null)
                return;

            Item item = tvLoadout.SelectedNode.Tag as Item;

            // if the loadout node isn't tagged or we couldn't cast it
            // to an Item return
            if (item == null)
                return;

            PlanWindow window = WindowsFactory<PlanWindow>.ShowByTag(m_plan);

            window.ShowItemInBrowser(item);
        }

        /// <summary>
        /// Export to EFT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miExportToEFT_Click(object sender, EventArgs e)
        {
            ExportToEFT();
        }

        #endregion


        #region EFT Export Function

        private void ExportToEFT()
        {
            Dictionary<string, List<string>> items = GetItemsBySlots();
            ExtractProperties(items);
            string exportText = FormatForEFT(items);

            // Copy to clipboard
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(exportText);
            }
            catch (ExternalException ex)
            {
                // there is a bug that results in an exception being
                // thrown when the clipboard is in use by another process.
                ExceptionHandler.LogException(ex, true);
            }
        }

        private Dictionary<String, List<string>> GetItemsBySlots()
        {
            // Groups items by slots
            Dictionary<String, List<string>> items = new Dictionary<string, List<string>>();
            foreach (TreeNode typeNode in tvLoadout.Nodes)
            {
                items[typeNode.Text] = new List<string>();
                foreach (TreeNode itemNode in typeNode.Nodes)
                {
                    items[typeNode.Text].Add(itemNode.Text);
                }
            }
            return items;
        }

        private string FormatForEFT(Dictionary<String, List<string>> items)
        {
            // Build the output format for EFT
            StringBuilder exportText = new StringBuilder();
            exportText.AppendFormat(CultureConstants.DefaultCulture, "[{0}, EVEMon {1}]", m_ship.Name, lblLoadoutName.Text);
            exportText.AppendLine();

            if (items.ContainsKey(s_typeMap["lo"]))
                exportText.AppendLine(String.Join(Environment.NewLine, items[s_typeMap["lo"]].ToArray()));
            
            if (items.ContainsKey(s_typeMap["med"]))
                exportText.AppendLine(String.Join(Environment.NewLine, items[s_typeMap["med"]].ToArray()));

            if (items.ContainsKey(s_typeMap["high"]))
                exportText.AppendLine(String.Join(Environment.NewLine, items[s_typeMap["high"]].ToArray()));

            if (items.ContainsKey(s_typeMap["rig"]))
                exportText.AppendLine(String.Join(Environment.NewLine, items[s_typeMap["rig"]].ToArray()));

            if (items.ContainsKey(s_typeMap["subSystem"]))
                exportText.AppendLine(String.Join(Environment.NewLine, items[s_typeMap["subSystem"]].ToArray()));

            if (items.ContainsKey(s_typeMap["drone"]))
            {
                foreach (String s in items[s_typeMap["drone"]])
                {
                    exportText.AppendFormat(CultureConstants.DefaultCulture, "{0} x1", s);
                    exportText.AppendLine();
                }
            }
            return exportText.ToString();
        }

        /// <summary>
        /// Extracts the properties.
        /// </summary>
        /// <param name="items">The items.</param>
        private void ExtractProperties(Dictionary<String, List<string>> items)
        {
            // Add "empty slot" mentions for every slot type
            foreach (EvePropertyValue prop in m_ship.Properties.Where(prop => prop.Property != null))
            {
                if (prop.Property.Name.Contains("High Slots"))
                {
                    int highSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty));
                    while (items.ContainsKey("high") && items["high"].Count < highSlots)
                    {
                        items["high"].Add("[empty high slot]");
                    }
                }
                else if (prop.Property.Name.Contains("Med Slots"))
                {
                    int medSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty));
                    while (items.ContainsKey("med") && items["med"].Count < medSlots)
                    {
                        items["med"].Add("[empty med slot]");
                    }
                }
                else if (prop.Property.Name.Contains("Low Slots"))
                {
                    int lowSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty));
                    while (items.ContainsKey("lo") && items["lo"].Count < lowSlots)
                    {
                        items["lo"].Add("[empty low slot]");
                    }
                }
                else if (prop.Property.Name.Contains("Rig Slots"))
                {
                    int rigsSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty));
                    while (items.ContainsKey("rig") && items["rig"].Count < rigsSlots)
                    {
                        items["rig"].Add("[empty rig slot]");
                    }
                }
                else if (prop.Property.Name.Contains("Sub System Slots"))
                {
                    int subSysSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty));
                    while (items.ContainsKey("subSystem") && items["subSystem"].Count < subSysSlots)
                    {
                        items["subSystem"].Add(String.Empty);
                    }
                }
            }
        }

        #endregion


        #region LoadoutListSorter

        private class LoadoutListSorter : IComparer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LoadoutListSorter"/> class.
            /// </summary>
            public LoadoutListSorter()
            {
                OrderOfSort = SortOrder.Ascending;
            }

            /// <summary>
            /// Gets or sets the order of sort.
            /// </summary>
            /// <value>The order of sort.</value>
            public SortOrder OrderOfSort { get; set; }

            /// <summary>
            /// Gets or sets the sort column.
            /// </summary>
            /// <value>The sort column.</value>
            public int SortColumn { get; set; }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>,
            /// as shown in the following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
            /// Zero <paramref name="x"/> equals <paramref name="y"/>. Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <exception cref="T:System.ArgumentException">Neither <paramref name="x"/> nor <paramref name="y"/>
            /// implements the <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and <paramref name="y"/>
            /// are of different types and neither one can handle comparisons with the other. </exception>
            public int Compare(object x, object y)
            {
                int compareResult = 0;
                ListViewItem a = (ListViewItem)x;
                ListViewItem b = (ListViewItem)y;

                if (OrderOfSort == SortOrder.Descending)
                {
                    ListViewItem tmp = b;
                    b = a;
                    a = tmp;
                }

                SerializableLoadout sla = a.Tag as SerializableLoadout;
                SerializableLoadout slb = b.Tag as SerializableLoadout;

                switch (SortColumn)
                {
                    case 0: // sort by name
                        compareResult = String.Compare(a.Text, b.Text);
                        break;
                    case 1: // Author
                        compareResult = String.Compare(a.SubItems[1].Text, b.SubItems[1].Text);
                        break;
                    case 2: // Rating
                        if (slb != null && (sla != null && sla.Rating < slb.Rating))
                        {
                            compareResult = -1;
                        }
                        else if (slb != null && (sla != null && sla.Rating > slb.Rating))
                        {
                            compareResult = 1;
                        }
                        break;
                    case 3: // Date
                        if (sla != null && slb != null)
                                compareResult = sla.SubmissionDate.CompareTo(slb.SubmissionDate);
                        break;
                }

                return compareResult;
            }
        }

        #endregion

    }
}
