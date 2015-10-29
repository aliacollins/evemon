using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    public partial class CertificateSelectControl : UserControl
    {
        // Blank image list for 'Safe for work' setting
        private readonly ImageList m_emptyImageList = new ImageList();

        private Plan m_plan;
        private Character m_character;
        private CertificateClass m_selectedCertificateClass;
        private Font m_iconsFont;
        private bool m_blockSelectionReentrancy;
        private bool m_allExpanded;
        private bool m_init;

        public event EventHandler<EventArgs> SelectionChanged;


        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public CertificateSelectControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the current plan.
        /// </summary>
        [Browsable(false)]
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (value == null || m_plan == value)
                    return;

                m_plan = value;
                m_character = (Character)m_plan.Character;
            }
        }

        /// <summary>
        /// Gets or sets the selected certificate class.
        /// </summary>
        [Browsable(false)]
        public CertificateClass SelectedCertificateClass
        {
            get { return m_selectedCertificateClass; }
            set
            {
                if (m_selectedCertificateClass == value)
                    return;

                m_selectedCertificateClass = value;

                // Updates the selection for the three controls
                m_blockSelectionReentrancy = true;
                try
                {
                    tvItems.SelectNodeWithTag(value);

                    lvSortedList.SelectedItems.Clear();
                    foreach (ListViewItem item in lvSortedList.Items.Cast<ListViewItem>().Where(item => item.Tag == value))
                    {
                        item.Selected = true;
                    }

                    lbSearchList.SelectedItem = value;
                }
                finally
                {
                    m_blockSelectionReentrancy = false;
                }

                // Fires event for subscribers
                OnSelectionChanged();
            }
        }

        #endregion


        #region Events

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;

            tbSearchText.KeyPress -= tbSearchText_KeyPress;
            tbSearchText.Enter -= tbSearchText_Enter;
            tbSearchText.Leave -= tbSearchText_Leave;
            lvSortedList.SelectedIndexChanged -= lvSortedList_SelectedIndexChanged;
            tvItems.NodeMouseClick -= tvItems_NodeMouseClick;
            tvItems.AfterSelect -= tvItems_AfterSelect;
            cmListCerts.Opening -= cmListCerts_Opening;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// On load, read settings and update the content.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted())
                return;

            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;

            tbSearchText.KeyPress += tbSearchText_KeyPress;
            tbSearchText.Enter += tbSearchText_Enter;
            tbSearchText.Leave += tbSearchText_Leave;
            lvSortedList.SelectedIndexChanged += lvSortedList_SelectedIndexChanged;
            tvItems.NodeMouseClick += tvItems_NodeMouseClick;
            tvItems.AfterSelect += tvItems_AfterSelect;
            cmListCerts.Opening += cmListCerts_Opening;
            Disposed += OnDisposed;

            m_emptyImageList.ImageSize = new Size(24, 24);
            m_emptyImageList.Images.Add(new Bitmap(24, 24));

            m_iconsFont = FontFactory.GetFont("Tahoma", 8.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            // Read the settings
            if (Settings.UI.UseStoredSearchFilters)
            {
                cbSorting.SelectedIndex = (int)Settings.UI.CertificateBrowser.Sort;
                cbFilter.SelectedIndex = (int)Settings.UI.CertificateBrowser.Filter;

                tbSearchText.Text = Settings.UI.CertificateBrowser.TextSearch;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
            }
            else
            {
                cbSorting.SelectedIndex = 0;
                cbFilter.SelectedIndex = 0;
            }

            m_init = true;

            // Updates the controls
            UpdateControlVisibility();
        }

        /// <summary>
        /// When the settings are changed, update the display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            pbSearchImage.Visible = !Settings.UI.SafeForWork;
            UpdateContent();
        }

        #endregion


        #region Control Events

        /// <summary>
        /// When the combo filter changes, we need to refresh the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSettingsForFilter(cbFilter.SelectedIndex);
            
            if (m_init)
                UpdateContent();
        }

        /// <summary>
        /// When the sort filter changes, we need to refresh the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSorting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSettingsForSort(cbSorting.SelectedIndex);
            
            if (m_init)
                UpdateContent();
        }

        /// <summary>
        /// When the "Search Text" label changes, we focus the textbox behind.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSearchTextHint_Click(object sender, EventArgs e)
        {
            tbSearchText.Focus();
        }

        /// <summary>
        /// When the user enters the search textbox, we hide the "search text" hint...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_Enter(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = false;
        }

        /// <summary>
        /// When the user leaves the search textbox, we display the "search text" hint...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_Leave(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
        }

        /// <summary>
        /// When the search text box changes, we update the settings with this new filter and we update the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            UpdateSettingsForTextSearch(tbSearchText.Text);

            if (!tbSearchText.Focused)
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);

            if (m_init)
                UpdateContent();
        }

        /// <summary>
        /// When the "left button" key is pressed, we select the whole text. (???)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 0x01)
                return;

            tbSearchText.SelectAll();
            e.Handled = true;
        }

        /// <summary>
        /// When the results listbox's selection is changed, we update the selected index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectionFromControls();
        }

        /// <summary>
        /// When the sorted listview' selection is changed, we update the selected index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSortedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectionFromControls();
        }

        /// <summary>
        /// Forces the selection update when a node is right-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                tvItems.SelectedNode = e.Node;
        }

        /// <summary>
        /// When the treeview's selection is changed, we update the selected index.
        /// Also used to force node selection on a right click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateSelection(e.Node.Tag);
        }

        /// <summary>
        /// Handles the MouseUp event of the pbSearchTextDel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void pbSearchTextDel_MouseUp(object sender, MouseEventArgs e)
        {
            tbSearchText.Clear();
        }

        #endregion


        #region Display update

        /// <summary>
        /// Updates the display.
        /// </summary>
        private void UpdateContent()
        {
            // Not ready yet ? We leave
            if (m_character == null || m_iconsFont == null)
                return;

            IList<CertificateClass> classes = GetFilteredData().ToList();

            lbSearchList.Items.Clear();

            tvItems.Visible = false;
            lbSearchList.Visible = false;
            lvSortedList.Visible = false;
            lbNoMatches.Visible = false;

            // Nothing to display ?
            if (!classes.Any())
            {
                lbNoMatches.Visible = true;
                UpdateSelection(null);
            }
                // Is it sorted ?
            else if (cbSorting.SelectedIndex != 0)
            {
                lvSortedList.Visible = true;
                UpdateListView(classes);
            }
                // Not sorted but there is a text filter
            else if (!String.IsNullOrEmpty(tbSearchText.Text))
            {
                lbSearchList.Visible = true;
                UpdateListBox(classes);
            }
                // Regular display, the tree 
            else
            {
                tvItems.Visible = true;
                UpdateTree(classes);
            }
        }

        /// <summary>
        /// Updates the certificates tree.
        /// </summary>
        /// <param name="classes"></param>
        private void UpdateTree(IList<CertificateClass> classes)
        {
            // Store the selected node (if any) to restore it after the update
            int selectedItemHash = (tvItems.SelectedNodes.Count > 0
                ? tvItems.SelectedNodes[0].Tag.GetHashCode()
                : 0);

            TreeNode selectedNode = null;

            // Fill the tree
            int numberOfItems = 0;
            tvItems.BeginUpdate();
            try
            {
                tvItems.ImageList = Settings.UI.SafeForWork ? m_emptyImageList : ilCertIcons;

                // Clear the existing nodes
                tvItems.Nodes.Clear();

                // Creates the nodes representing the categories
                foreach (CertificateGroup category in m_character.CertificateCategories)
                {
                    int imageIndex = tvItems.ImageList.Images.IndexOfKey("Certificate");

                    TreeNode node = new TreeNode
                    {
                        Text = category.Name,
                        ImageIndex = imageIndex,
                        SelectedImageIndex = imageIndex,
                        Tag = category
                    };

                    foreach (TreeNode childNode in classes.Where(x => x.Category == category).Select(
                        certClass => new
                        {
                            certClass,
                            index = GetCertImageIndex(certClass.Certificate)
                        }).Select(childNode => new TreeNode
                        {
                            Text = childNode.certClass.Name,
                            ImageIndex = childNode.index,
                            SelectedImageIndex = childNode.index,
                            Tag = childNode.certClass
                        }))
                    {
                        numberOfItems++;
                        node.Nodes.Add(childNode);
                    }

                    tvItems.Nodes.Add(node);
                }

                // Restore the selected node (if any)
                if (selectedItemHash > 0)
                {
                    foreach (TreeNode node in tvItems.GetAllNodes().Where(node => node.Tag.GetHashCode() == selectedItemHash))
                    {
                        tvItems.SelectNodeWithTag(node.Tag);
                        selectedNode = node;
                    }
                }

                // Reset if the node doesn't exist anymore
                if (selectedNode == null)
                    UpdateSelection(null);
            }
            finally
            {
                tvItems.EndUpdate();
                m_allExpanded = false;

                // If the filtered set is small enough to fit all nodes on screen, call expandAll()
                if (numberOfItems < (tvItems.DisplayRectangle.Height / tvItems.ItemHeight))
                {
                    tvItems.ExpandAll();
                    m_allExpanded = true;
                }
            }
        }

        /// <summary>
        /// Updates the list box displayed when there is a text filter and no sort criteria.
        /// </summary>
        /// <param name="classes"></param>
        private void UpdateListBox(IEnumerable<CertificateClass> classes)
        {
            lbSearchList.BeginUpdate();
            try
            {
                lbSearchList.Items.Clear();
                SelectedCertificateClass = null;
                foreach (CertificateClass certClass in classes)
                {
                    lbSearchList.Items.Add(certClass);
                }
            }
            finally
            {
                lbSearchList.EndUpdate();
            }
        }

        /// <summary>
        /// Updates the listview displayed when there is a sort criteria.
        /// </summary>
        /// <param name="classes"></param>
        private void UpdateListView(IList<CertificateClass> classes)
        {
            // Store the selected node (if any) to restore it after the update
            int selectedItemHash = (tvItems.SelectedNodes.Count > 0
                                        ? tvItems.SelectedNodes[0].Tag.GetHashCode()
                                        : 0);

            // Retrieve the data to fetch into the list
            IEnumerable<string> labels = null;
            string column = GetSortedListData(ref classes, ref labels);
            if (labels == null)
                return;

            lvSortedList.BeginUpdate();
            try
            {
                lvSortedList.Items.Clear();

                using (IEnumerator<string> labelsEnumerator = labels.GetEnumerator())
                {
                    foreach (CertificateClass certClass in classes)
                    {
                        // Retrieves the label for the second column (sort key)
                        labelsEnumerator.MoveNext();
                        string label = labelsEnumerator.Current;

                        // Add the item
                        ListViewItem item = new ListViewItem(certClass.Name);
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, label));
                        lvSortedList.Items.Add(item);
                        item.Tag = certClass;
                    }
                }

                ListViewItem selectedItem = null;

                // Restore the selected node (if any)
                if (selectedItemHash > 0)
                {
                    foreach (ListViewItem lvItem in lvSortedList.Items.Cast<ListViewItem>().Where(
                        lvItem => lvItem.Tag.GetHashCode() == selectedItemHash))
                    {
                        lvItem.Selected = true;
                        selectedItem = lvItem;
                    }
                }

                // Reset if the node doesn't exist anymore
                if (selectedItem == null)
                    UpdateSelection(null);

                // Auto adjust column widths
                chSortKey.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                chName.Width = Math.Max(0, Math.Max(lvSortedList.ClientSize.Width / 2,
                                                    lvSortedList.ClientSize.Width - (chSortKey.Width + 16)));
                chSortKey.Text = column;
            }
            finally
            {
                lvSortedList.EndUpdate();
            }
        }

        /// <summary>
        /// Gets the filtered list of data.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CertificateClass> GetFilteredData()
        {
            IEnumerable<CertificateClass> classes = m_character.CertificateClasses;

            // Apply the selected filter 
            if (cbFilter.SelectedIndex != 0)
            {
                Func<CertificateClass, bool> predicate = GetFilter();
                classes = classes.Where(predicate);
            }

            // Text search
            if (!String.IsNullOrEmpty(tbSearchText.Text))
            {
                string searchText = tbSearchText.Text.ToLower(CultureConstants.DefaultCulture).Trim();
                classes = classes.Where(x => x.Name.ToLower(CultureConstants.DefaultCulture).Contains(searchText));
            }

            // When sorting by "time to...", filter completed items
            if (cbSorting.SelectedIndex == (int)CertificateSort.TimeToMaxLevel ||
                cbSorting.SelectedIndex == (int)CertificateSort.TimeToNextLevel)
                classes = classes.Where(x => !x.IsCompleted);

            return classes;
        }

        /// <summary>
        /// Gets the items' filter.
        /// </summary>
        private Func<CertificateClass, bool> GetFilter()
        {
            if (cbFilter.SelectedIndex == -1)
                return x => true;

            // Update the base filter from the combo box
            switch ((CertificateFilter)cbFilter.SelectedIndex)
            {
                case CertificateFilter.All:
                    return x => true;
                case CertificateFilter.HideMaxLevel:
                    return x => !x.IsCompleted;
                case CertificateFilter.NextLevelTrainable:
                    return x => x.IsFurtherTrainable;
                case CertificateFilter.NextLevelUntrainable:
                    return x => !x.IsFurtherTrainable & !x.IsCompleted;
                case CertificateFilter.Completed:
                    return x => x.IsCompleted;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the data for the sorted list view.
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        private string GetSortedListData(ref IList<CertificateClass> classes, ref IEnumerable<string> labels)
        {
            IEnumerable<TimeSpan> times;
            string columnHeader;

            switch ((CertificateSort)cbSorting.SelectedIndex)
            {
                    // Sort by name, default, occurs on initialization
                case CertificateSort.Name:
                    return String.Empty;
                    // Sort by time to next level
                case CertificateSort.TimeToNextLevel:
                    {
                        times = classes.Select(GetTimeToNextLevel);
                        columnHeader = "Time";
                        break;
                    }
                    // Sort by time to max level
                case CertificateSort.TimeToMaxLevel:
                    {
                        times = classes.Select(GetTimeToMaxLevel);
                        columnHeader = "Time to max level";
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            CertificateClass[] classesArray = classes.ToArray();
            TimeSpan[] timesArray = times.ToArray();
            Array.Sort(timesArray, classesArray);
            classes = classesArray;
            labels = timesArray.Select(x => x.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas));
            return columnHeader;
        }

        /// <summary>
        /// Gets the time to next grade.
        /// </summary>
        /// <param name="certificateClass">The certificate class.</param>
        /// <returns></returns>
        private static TimeSpan GetTimeToNextLevel(CertificateClass certificateClass)
        {
            CertificateLevel lowestTrinedLevel = certificateClass.Certificate.LowestUntrainedLevel;
            return lowestTrinedLevel == null
                ? TimeSpan.Zero
                : lowestTrinedLevel.GetTrainingTime;
        }

        /// <summary>
        /// Gets the time to elite grade.
        /// </summary>
        /// <param name="certificateClass">The certificate class.</param>
        /// <returns></returns>
        private static TimeSpan GetTimeToMaxLevel(CertificateClass certificateClass)
        {
            CertificateLevel levelFive = certificateClass.Certificate.GetCertificateLevel(5);
            return levelFive.IsTrained
                ? TimeSpan.Zero
                : levelFive.GetTrainingTime;
        }

        /// <summary>
        /// Gets the image's index for the provided certificate class,
        /// lazily creating the images when they're needed.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <returns></returns>
        private int GetCertImageIndex(Certificate certificate)
        {
            if (!Settings.UI.SafeForWork)
            {
                return certificate.HighestTrainedLevel == null
                    ? 0
                    : (int)certificate.HighestTrainedLevel.Level;
            }

            // Prepares data
            char[] chars = new char[5];
            bool[] trained = new bool[5];

            foreach (int level in certificate.AllLevel
                .Select(certLevel =>
                    new
                    {
                        certLevel,
                        level = (int)certLevel.Level
                    })
                .Where(cert => cert.certLevel.IsTrained)
                .Select(cert => cert.level))
            {
                trained[level - 1] = true;
                chars[level - 1] = Convert.ToChar(level);
            }

            // Create key and retrieves its index, then returns if it already exists
            string key = string.Concat(chars);
            int index = tvItems.ImageList.Images.IndexOfKey(key);
            if (index != -1)
                return index;

            // Create the image if it does not exist yet
            const int ImageSize = 32;
            const int MaxLetterWidth = 6;

            Bitmap bmp;
            using(Bitmap tempBitmap = new Bitmap(ImageSize, ImageSize, PixelFormat.Format32bppArgb))
            {
                bmp = (Bitmap)tempBitmap.Clone();
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                string[] letters = new string[5];
                float[] xPositions = new float[5];
                float x = 0.0f,
                    height = 0.0f;

                int i = 0;
                // Scroll through letters and measure them
                foreach (var certLevel in certificate.AllLevel)
                {
                    letters[i] = ((int)certLevel.Level).ToString();
                    SizeF size = g.MeasureString(letters[i], m_iconsFont, MaxLetterWidth, StringFormat.GenericTypographic);
                    height = Math.Max(height, size.Height);
                    xPositions[i] = x;
                    x += (size.Width + 1.0f);
                    i++;
                }

                // Y offset
                float y = Math.Max(0.0f, (ImageSize - height) * 0.5f);

                // Draw the letters
                g.Clear(Color.White);
                using (SolidBrush grantedBrush = new SolidBrush(Color.Blue))
                using (SolidBrush nonGrantedBrush = new SolidBrush(Color.Gray))
                {
                    for (int j = 0; j < certificate.AllLevel.Count(); j++)
                    {
                        // Special color for trained, gray for the other ones
                        bool isTrained = trained[j];
                        SolidBrush brush = isTrained ? grantedBrush : nonGrantedBrush;
                        g.DrawString(letters[j], m_iconsFont, brush, xPositions[j], y);
                    }
                }
            }
            // Insert image and return its index
            tvItems.ImageList.Images.Add(key, bmp);
            return tvItems.ImageList.Images.IndexOfKey(key);
        }

        #endregion


        #region Selection Helper Methods

        /// <summary>
        /// Called whenever the selection changes,
        /// fires the approriate event.
        /// </summary>
        private void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// Updates the selection by pickking the good control (the one visible).
        /// </summary>
        private void UpdateSelectionFromControls()
        {
            if (lvSortedList.Visible)
            {
                UpdateSelection(lvSortedList.SelectedItems.Count == 0 ? null : lvSortedList.SelectedItems[0].Tag);
                return;
            }

            if (lbSearchList.Visible)
            {
                UpdateSelection(lbSearchList.SelectedItems.Count == 0 ? null : lbSearchList.SelectedItems[0]);
                return;
            }

            UpdateSelection(tvItems.SelectedNode == null ? null : tvItems.SelectedNode.Tag);
        }

        /// <summary>
        /// Updates the selection with the provided item.
        /// </summary>
        /// <param name="selection"></param>
        private void UpdateSelection(object selection)
        {
            if (!m_blockSelectionReentrancy)
                SelectedCertificateClass = selection as CertificateClass;
        }

        /// <summary>
        /// Updates the settings for the search text.
        /// </summary>
        /// <param name="textSearch"></param>
        private static void UpdateSettingsForTextSearch(string textSearch)
        {
            Settings.UI.CertificateBrowser.TextSearch = textSearch;
        }

        /// <summary>
        /// Updates the settings for the filter.
        /// </summary>
        /// <param name="filterIndex"></param>
        private static void UpdateSettingsForFilter(int filterIndex)
        {
            Settings.UI.CertificateBrowser.Filter = (CertificateFilter)filterIndex;
        }

        /// <summary>
        /// Updates the settings for the sort.
        /// </summary>
        /// <param name="sortIndex"></param>
        private static void UpdateSettingsForSort(int sortIndex)
        {
            Settings.UI.CertificateBrowser.Sort = (CertificateSort)sortIndex;
        }

        #endregion


        #region Context menus

        /// <summary>
        /// Context > Plan To > Level N
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void planToLevelMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem levelItem = (ToolStripMenuItem)sender;
            IPlanOperation operation = levelItem.Tag as IPlanOperation;
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);
        }

        /// <summary>
        /// When the tree's context menu opens,
        /// we update the submenus' statuses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmListCerts_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = tvItems.SelectedNode;
            CertificateClass certClass = SelectedCertificateClass;

            if (certClass == null || m_plan.WillGrantEligibilityFor(certClass.Certificate.GetCertificateLevel(5)))
            {
                cmiLvPlanTo.Enabled = false;
                cmiLvPlanTo.Text = @"Plan to...";
            }
            else
            {
                cmiLvPlanTo.Enabled = true;
                cmiLvPlanTo.Text = String.Format(CultureConstants.DefaultCulture, "Plan \"{0}\" to...", certClass.Name);

                // "Plan to N" menus
                for (int i = 1; i <= 5; i++)
                {
                    SetAdditionMenuStatus(cmiLvPlanTo.DropDownItems[i - 1], certClass.Certificate.GetCertificateLevel(i));
                }
            }

            tsSeparatorPlanTo.Visible = (certClass == null && node != null && lbSearchList.Items.Count == 0);

            // "Expand" and "Collapse" selected menu
            tsmExpandSelected.Visible = (certClass == null && node != null && lbSearchList.Items.Count == 0 && !node.IsExpanded);
            tsmCollapseSelected.Visible = (certClass == null && node != null && lbSearchList.Items.Count == 0 && node.IsExpanded);

            tsmExpandSelected.Text = (certClass == null && node != null &&
                                      !node.IsExpanded
                                          ? String.Format(CultureConstants.DefaultCulture, "Expand \"{0}\"", node.Text)
                                          : String.Empty);
            tsmCollapseSelected.Text = (certClass == null && node != null &&
                                        node.IsExpanded
                                            ? String.Format(CultureConstants.DefaultCulture, "Collapse \"{0}\"", node.Text)
                                            : String.Empty);

            tspSeparatorExpandCollapse.Visible = lbSearchList.Items.Count == 0;

            // "Expand All" and "Collapse All" menu
            tsmCollapseAll.Enabled = tsmCollapseAll.Visible = m_allExpanded && lbSearchList.Items.Count == 0;
            tsmExpandAll.Enabled = tsmExpandAll.Visible = !tsmCollapseAll.Enabled && lbSearchList.Items.Count == 0;
        }

        /// <summary>
        /// Sets the visible status of the context menu submenu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="certLevel">The cert level.</param>
        private void SetAdditionMenuStatus(ToolStripItem menu, CertificateLevel certLevel)
        {
            menu.Visible = certLevel != null;

            if (certLevel == null)
                return;

            menu.Enabled = !m_plan.WillGrantEligibilityFor(certLevel);

            if (menu.Enabled)
                menu.Tag = m_plan.TryPlanTo(certLevel);
        }

        /// <summary>
        /// Treeview's context menu > Expand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmExpandSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Expand();
        }

        /// <summary>
        /// Treeview's context menu > Collapse 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmCollapseSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Collapse();
        }

        /// <summary>
        /// Treeview's context menu > Expand All
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmExpandAll_Click(object sender, EventArgs e)
        {
            tvItems.ExpandAll();
            m_allExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse All
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmCollapseAll_Click(object sender, EventArgs e)
        {
            tvItems.CollapseAll();
            m_allExpanded = false;
        }

        #endregion
    }
}