using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using SortOrder = System.Windows.Forms.SortOrder;

namespace EVEMon.SkillPlanner
{
    public partial class ShipLoadoutSelectWindow : EVEMonForm
    {
        private Item m_ship;
        private Plan m_plan;
        private Character m_character;
        private ILoadoutInfo m_loadoutInfo;
        private Loadout m_selectedLoadout;
        private readonly List<StaticSkillLevel> m_prerequisites = new List<StaticSkillLevel>();
        private readonly LoadoutListSorter m_columnSorter;


        #region Initialization, loading, closing

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipLoadoutSelectWindow"/> class.
        /// </summary>
        private ShipLoadoutSelectWindow()
        {
            InitializeComponent();
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="plan"></param>
        public ShipLoadoutSelectWindow(Plan plan)
            : this()
        {
            persistentSplitContainer.RememberDistanceKey = "ShipLoadoutBrowser";

            Plan = plan;

            m_columnSorter = new LoadoutListSorter { OrderOfSort = SortOrder.Descending, SortColumn = 2 };
            lvLoadouts.ListViewItemSorter = m_columnSorter;
        }

        /// <summary>
        /// On load, download the loadouts feed for this ship.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadoutSelect_Load(object sender, EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Subscribe global events
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.LoadoutUpdated += EveMonClient_LoadoutUpdated;
            EveMonClient.LoadoutFeedUpdated += EveMonClient_LoadoutFeedUpdated;
        }

        /// <summary>
        /// When the window is closing, we clean things up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadoutSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unsubscribe global events
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.LoadoutUpdated -= EveMonClient_LoadoutUpdated;
            EveMonClient.LoadoutFeedUpdated -= EveMonClient_LoadoutFeedUpdated;
        }

        /// <summary>
        /// Gets or sets the plan.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (value == null || m_plan == value)
                    return;

                m_plan = value;
                m_character = (Character)m_plan.Character;
                Tag = value;
                Text = String.Format(CultureConstants.DefaultCulture, "{0} [{1}] - {2} Loadout Selection",
                                     value.Character, value.Name, Settings.LoadoutsProvider.Provider?.Name);

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
        /// Query the loadouts feed for the current ship.
        /// </summary>
        private void QueryLoadoutsFeed()
        {
            if (Settings.LoadoutsProvider.Provider == null)
                return;

            // Wait cursor until we retrieved the loadout
            Cursor.Current = Cursors.WaitCursor;
            throbberLoadouts.State = ThrobberState.Rotating;
            persistentSplitContainer.Visible = false;

            // We clear previous data
            lvLoadouts.Items.Clear();
            tvLoadout.Nodes.Clear();
            m_prerequisites.Clear();

            // Download the eve image
            eveImage.EveItem = m_ship;

            // Download the loadouts feed
            Settings.LoadoutsProvider.Provider.GetLoadoutsFeedAsync(m_ship);

            // Set labels while the user wait
            lblShipName.Text = m_ship.Name;
            lblLoadoutName.Text = @"No Loadout Selected";
            lblAuthor.Text = String.Empty;
            lblSubmitDate.Text = String.Empty;
            lblPlanned.Text = String.Empty;
            lblPlanned.Visible = false;
            lblTrainTime.Text = @"N/A";
            lblTrainTime.Visible = true;
            lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture, "Fetching loadouts for {0}", m_ship.Name);
            btnPlan.Enabled = false;
        }

        /// <summary>
        /// Updates the loadout feed information.
        /// </summary>
        /// <param name="e">The <see cref="LoadoutFeedEventArgs"/> instance containing the event data.</param>
        private void UpdateLoadoutFeedInfo(LoadoutFeedEventArgs e)
        {
            if (IsDisposed)
                return;

            // Restore the default cursor instead of the waiting one
            Cursor.Current = Cursors.Default;
            btnPlan.Enabled = false;

            if (Settings.LoadoutsProvider.Provider == null)
                return;

            // Was there an error ?
            if (e.HasError)
            {
                throbberLoadouts.State = ThrobberState.Strobing;
                lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture,
                    "There was a problem connecting to {0}, it may be down for maintainance.{1}{2}",
                    Settings.LoadoutsProvider.Provider.Name, Environment.NewLine, e.Error.Message);

                return;
            }

            m_loadoutInfo = Settings.LoadoutsProvider.Provider.DeserializeLoadoutsFeed(m_ship, e.LoadoutFeed);

            // Are there no feeds ?
            if (!m_loadoutInfo.Loadouts.Any())
            {
                throbberLoadouts.State = ThrobberState.Strobing;
                lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture,
                    "There are no loadouts for {0}, why not submit one to {1}?", m_ship.Name, Settings.LoadoutsProvider.Provider.Name);
                return;
            }

            // Add the listview items for every loadout
            foreach (Loadout loadout in m_loadoutInfo.Loadouts)
            {
                ListViewItem lvi = new ListViewItem(loadout.Name) { Text = loadout.Name, Tag = loadout };
                lvi.SubItems.Add(loadout.Author);
                lvi.SubItems.Add(loadout.Rating.ToString(CultureConstants.DefaultCulture));
                lvi.SubItems.Add(loadout.SubmissionDate.ToString("G"));
                lvLoadouts.Items.Add(lvi);
            }

            // Update the header
            lblLoadouts.Text = String.Format(CultureConstants.DefaultCulture, "Found {0} loadouts", lvLoadouts.Items.Count);

            // Update the listview's comparer and sort
            lvLoadouts.Sort();
            UpdateSortVisualFeedback();

            // Adjust the size of the columns
            AdjustColumns();

            throbberLoadouts.State = ThrobberState.Stopped;
            persistentSplitContainer.Visible = lvLoadouts.Items.Count > 0;
        }

        /// <summary>
        /// Downloads the given loadout.
        /// </summary>
        /// <param name="loadout"></param>
        private void DownloadLoadout(Loadout loadout)
        {
            // Prevent downloading the same loadout
            if (m_selectedLoadout == loadout)
                return;

            if (Settings.LoadoutsProvider.Provider == null)
                return;

            // Reset controls and set the cursor to wait
            btnPlan.Enabled = false;
            lblTrainTime.Visible = false;
            Cursor.Current = Cursors.WaitCursor;
            throbberFitting.State = ThrobberState.Rotating;
            throbberFitting.BringToFront();
            tvLoadout.Nodes.Clear();

            // Store the selected loadout
            m_selectedLoadout = loadout;

            // Set the headings
            lblLoadoutName.Text = m_selectedLoadout.Name;
            lblAuthor.Text = m_selectedLoadout.Author;
            lblSubmitDate.Text = m_selectedLoadout.SubmissionDate.ToString("G");

            // Download the loadout details
            Settings.LoadoutsProvider.Provider.GetLoadoutByIDAsync(m_selectedLoadout.ID);
        }

        /// <summary>
        /// Updates the loadout information.
        /// </summary>
        /// <param name="e">The <see cref="LoadoutEventArgs"/> instance containing the event data.</param>
        private void UpdateLoadoutInfo(LoadoutEventArgs e)
        {
            if (IsDisposed)
                return;

            // Reset the controls
            m_prerequisites.Clear();
            tvLoadout.Nodes.Clear();
            Cursor.Current = Cursors.Default;

            if (Settings.LoadoutsProvider.Provider == null)
                return;

            // Was there an error ?
            if (e.HasError)
            {
                throbberFitting.State = ThrobberState.Strobing;
                lblTrainTime.Text = String.Format(CultureConstants.DefaultCulture, "Couldn't download that loadout.{0}{1}",
                                                  Environment.NewLine, e.Error.Message);
                lblTrainTime.Visible = true;
                return;
            }

            Settings.LoadoutsProvider.Provider.DeserializeLoadout(m_selectedLoadout, e.Loadout);

            // Fill the items tree
            BuildTreeNodes(m_selectedLoadout.Items);

            // Compute the training time
            UpdatePlanningControls();

            throbberFitting.State = ThrobberState.Stopped;
            throbberFitting.SendToBack();
        }

        /// <summary>
        /// Builds the tree nodes.
        /// </summary>
        /// <param name="items">The items.</param>
        private void BuildTreeNodes(IEnumerable<Item> items)
        {
            foreach (IGrouping<string, Item> slotItems in items.GroupBy(LoadoutHelper.GetSlotByItem))
            {
                TreeNode typeNode = new TreeNode(slotItems.Key);

                foreach (Item item in slotItems)
                {
                    TreeNode slotNode = new TreeNode { Text = item.Name, Tag = item };
                    typeNode.Nodes.Add(slotNode);

                    m_prerequisites.AddRange(item.Prerequisites);
                }

                tvLoadout.Nodes.Add(typeNode);
            }

            // Order the nodes
            TreeNode[] orderNodes = tvLoadout.Nodes.Cast<TreeNode>().OrderBy(
                node => LoadoutHelper.OrderedSlotNames.IndexOf(String.Intern(node.Text))).ToArray();

            tvLoadout.BeginUpdate();
            try
            {
                tvLoadout.Nodes.Clear();
                tvLoadout.Nodes.AddRange(orderNodes);
                tvLoadout.ExpandAll();

                IList<TreeNode> tvNodes = tvLoadout.Nodes.Cast<TreeNode>().ToList();

                if (tvNodes.Any())
                    tvNodes.First().EnsureVisible();
            }
            finally
            {
                tvLoadout.EndUpdate();
            }
        }

        /// <summary>
        /// Update the plan status : training time, skills already trained, etc
        /// </summary>
        private void UpdatePlanningControls()
        {
            if (!m_prerequisites.Any())
                return;

            // Are all the prerequisites trained ?
            if (m_prerequisites.All(x => m_character.GetSkillLevel(x.Skill) >= x.Level))
            {
                btnPlan.Enabled = false;
                lblPlanned.Visible = true;
                lblPlanned.Text = @"All skills already trained.";
                lblTrainTime.Visible = false;
                return;
            }

            // Are all the prerequisites planned ?
            if (m_plan.AreSkillsPlanned(m_prerequisites.Where(x => m_character.Skills[x.Skill.ID].Level < x.Level)))
            {
                btnPlan.Enabled = false;
                lblPlanned.Visible = true;
                lblPlanned.Text = @"All skills already trained or planned.";
                lblTrainTime.Visible = false;
                return;
            }

            // Compute the training time
            CharacterScratchpad scratchpad = new CharacterScratchpad(m_character);
            foreach (PlanEntry entry in m_plan)
            {
                scratchpad.Train(entry);
            }

            TimeSpan startTime = scratchpad.TrainingTime;
            foreach (StaticSkillLevel prereq in m_prerequisites)
            {
                scratchpad.Train(prereq);
            }

            TimeSpan trainingTime = scratchpad.TrainingTime.Subtract(startTime);

            // Update the labels
            lblTrainTime.Text = trainingTime.ToDescriptiveText(
                DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.SpaceText);
            lblTrainTime.Visible = true;
            lblPlanned.Text = String.Empty;
            lblPlanned.Visible = false;
            btnPlan.Enabled = true;
        }

        /// <summary>
        /// Updates the sort visual feedback.
        /// </summary>
        private void UpdateSortVisualFeedback()
        {
            foreach (ColumnHeader columnHeader in lvLoadouts.Columns.Cast<ColumnHeader>())
            {
                if (m_columnSorter.SortColumn == columnHeader.Index)
                    columnHeader.ImageIndex = (m_columnSorter.OrderOfSort == SortOrder.Ascending ? 0 : 1);
                else
                    columnHeader.ImageIndex = 2;
            }
        }

        /// <summary>
        /// Adjusts the columns.
        /// </summary>
        private void AdjustColumns()
        {
            foreach (ColumnHeader column in lvLoadouts.Columns)
            {
                column.Width = -2;

                // Due to .NET design we need to prevent the last colummn to resize to the right end

                // Return if it's not the last column and not set to auto-resize
                if (column.Index != lvLoadouts.Columns.Count - 1)
                    continue;

                const int Pad = 4;

                // Calculate column header text width with padding
                int columnHeaderWidth = TextRenderer.MeasureText(column.Text, Font).Width + Pad * 2;

                // If there is an image assigned to the header, add its width with padding
                if (lvLoadouts.SmallImageList != null && column.ImageIndex > -1)
                    columnHeaderWidth += lvLoadouts.SmallImageList.ImageSize.Width + Pad;

                // Calculate the width of the header and the items of the column
                int columnMaxWidth = column.ListView.Items.Cast<ListViewItem>().Select(
                    item => TextRenderer.MeasureText(item.SubItems[column.Index].Text, Font).Width).Concat(
                        new[] { columnHeaderWidth }).Max() + Pad + 1;

                // Assign the width found
                column.Width = columnMaxWidth;
            }
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

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Occurs when the loadout feed from the provider updated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void EveMonClient_LoadoutFeedUpdated(object sender, LoadoutFeedEventArgs e)
        {
            UpdateLoadoutFeedInfo(e);
        }

        /// <summary>
        /// Occurs when the loadout from the provider updated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LoadoutEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_LoadoutUpdated(object sender, LoadoutEventArgs e)
        {
            UpdateLoadoutInfo(e);
        }

        #endregion


        #region Controls' events handlers

        /// <summary>
        /// When the user click a loadout, we download it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvLoadouts_Click(object sender, EventArgs e)
        {
            if (lvLoadouts.SelectedItems.Count == 0)
                return;

            Loadout loadout = (Loadout)lvLoadouts.SelectedItems[0].Tag;
            DownloadLoadout(loadout);
        }

        /// <summary>
        /// When the user clicks cancel, we quit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
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
            UpdateSortVisualFeedback();
        }

        /// <summary>
        /// When the user clicks the "discuss this loadout", we open the browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonForumTopic_Click(object sender, EventArgs e)
        {
            if (m_selectedLoadout != null)
            {
                Util.OpenURL(m_selectedLoadout.TopicUrl);
                return;
            }

            MessageBox.Show(@"Please select a loadout to discuss.", @"No Loadout Selected", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// When the user clicks the "plan" button, we add the prerqs to the plan and refresh the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlan_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryAddSet(m_prerequisites, m_selectedLoadout.Name);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.Perform(new PlanToOperationForm(operation), window);
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
            Item item = tvLoadout.SelectedNode?.Tag as Item;

            // if the loadout node isn't tagged or we couldn't cast it
            // to an Item return
            if (item == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(m_plan);
            window.ShowItemInBrowser(item);
        }

        /// <summary>
        /// Export to Clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miExportToClipboard_Click(object sender, EventArgs e)
        {
            LoadoutExporter.ExportToClipboard(m_loadoutInfo, m_selectedLoadout);
        }

        #endregion


        #region Helper methods


        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            eveImage.Visible = !Settings.UI.SafeForWork;
            if (!Settings.UI.SafeForWork && m_ship != null)
                eveImage.EveItem = m_ship;
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

                Loadout loadoutA = a.Tag as Loadout;
                Loadout loadoutB = b.Tag as Loadout;

                switch (SortColumn)
                {
                    case 0: // sort by name
                        compareResult = String.Compare(a.Text, b.Text, StringComparison.CurrentCulture);
                        break;
                    case 1: // Author
                        compareResult = String.Compare(a.SubItems[1].Text, b.SubItems[1].Text, StringComparison.CurrentCulture);
                        break;
                    case 2: // Rating
                        if (loadoutB != null && (loadoutA != null && loadoutA.Rating < loadoutB.Rating))
                            compareResult = -1;
                        else if (loadoutB != null && (loadoutA != null && loadoutA.Rating > loadoutB.Rating))
                            compareResult = 1;
                        break;
                    case 3: // Date
                        if (loadoutA != null && loadoutB != null)
                            compareResult = loadoutA.SubmissionDate.CompareTo(loadoutB.SubmissionDate);
                        break;
                }

                return compareResult;
            }
        }

        #endregion
    }
}