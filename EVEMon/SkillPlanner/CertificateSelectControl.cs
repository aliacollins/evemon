using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Imaging;

using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.SkillPlanner
{
    public partial class CertificateSelectControl : UserControl
    {
        private readonly char[] m_upperCertificatesLetters = new[] { 'B', 'S', 'I', 'E' };

        private readonly char[] m_lowerCertificatesLetters = new[] { '1', '2', '3', '4' };
                                // Stupid insensitive images keys' comparison, we cannot use bsie

        private Plan m_plan;
        private Character m_character;
        private CertificateClass m_selectedCertificateClass;
        private Font m_iconsFont;
        private bool m_blockSelectionReentrancy;
        private bool m_allExpanded;

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
            cmListSkills.Opening -= cmListSkills_Opening;
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
            cmListSkills.Opening += cmListSkills_Opening;
            Disposed += OnDisposed;

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

            // Updates the controls
            EveMonClient_SettingsChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// When the settings are changed, update the display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            pbSearchImage.Visible = !Settings.UI.SafeForWork;
            UpdateContent();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the current plan.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan == value)
                    return;

                m_plan = value;
                m_character = (Character)m_plan.Character;
            }
        }

        /// <summary>
        /// Gets or sets the selected certificate class.
        /// </summary>
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


        #region Control events

        /// <summary>
        /// When the combo filter changes, we need to refresh the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSettingsForFilter(cbFilter.SelectedIndex);
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

            IEnumerable<CertificateClass> classes = GetFilteredData();

            tvItems.Visible = false;
            lbSearchList.Visible = false;
            lvSortedList.Visible = false;
            lbNoMatches.Visible = false;

            // Nothing to display ?
            if (classes.IsEmpty())
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
        private void UpdateTree(IEnumerable<CertificateClass> classes)
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
                // Blank image list for 'Safe for work' setting
                ImageList newImageList = new ImageList { ImageSize = new Size(24, 24) };
                newImageList.Images.Add(new Bitmap(24, 24));

                tvItems.ImageList = (Settings.UI.SafeForWork ? newImageList : ilCertIcons);

                // Clear the existing nodes
                tvItems.Nodes.Clear();

                // Creates the nodes representing the categories
                foreach (CertificateCategory category in m_character.CertificateCategories)
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
                                             index = GetCertImageIndex(certClass)
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
        private void UpdateListView(IEnumerable<CertificateClass> classes)
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
                chName.Width = Math.Max(0,
                                        Math.Max(lvSortedList.ClientSize.Width / 2,
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
            if (cbSorting.SelectedIndex == (int)CertificateSort.TimeToEliteGrade ||
                cbSorting.SelectedIndex == (int)CertificateSort.TimeToNextGrade)
                classes = classes.Where(x => !x.IsCompleted);

            return classes;
        }

        /// <summary>
        /// Gets the items' filter.
        /// </summary>
        private Func<CertificateClass, bool> GetFilter()
        {
            // Update the base filter from the combo box
            switch ((CertificateFilter)cbFilter.SelectedIndex)
            {
                case CertificateFilter.All:
                    return x => true;
                case CertificateFilter.HideElite:
                    return x => !x.IsCompleted;
                case CertificateFilter.NextGradeTrainable:
                    return x => x.IsFurtherTrainable;
                case CertificateFilter.NextGradeUntrainable:
                    return x => !x.IsFurtherTrainable & !x.IsCompleted;
                case CertificateFilter.Claimable:
                    return x => x.Any(y => y.CanBeClaimed);
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
        private string GetSortedListData(ref IEnumerable<CertificateClass> classes, ref IEnumerable<string> labels)
        {
            switch ((CertificateSort)cbSorting.SelectedIndex)
            {
                    // Sort by name, default, occurs on initialization
                case CertificateSort.Name:
                    return String.Empty;
                    // Sort by time to next grade
                case CertificateSort.TimeToNextGrade:
                    IEnumerable<TimeSpan> times = classes.Select(x =>
                                                                     {
                                                                         Certificate nextUntrained = x.LowestUntrainedGrade;
                                                                         return (nextUntrained == null
                                                                                     ? TimeSpan.Zero
                                                                                     : nextUntrained.GetTrainingTime());
                                                                     });

                    CertificateClass[] classesArray = classes.ToArray();
                    TimeSpan[] timesArray = times.ToArray();
                    Array.Sort(timesArray, classesArray);
                    classes = classesArray;
                    labels = timesArray.Select(x => x.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas));
                    return "Time";
                    // Sort by time to elite (or highest) grade
                case CertificateSort.TimeToEliteGrade:
                    times = classes.Select(x =>
                                               {
                                                   Certificate lastGrade = x.HighestGradeCertificate;
                                                   CertificateStatus status = lastGrade.Status;

                                                   if (status == CertificateStatus.Granted ||
                                                       status == CertificateStatus.Claimable)
                                                       return TimeSpan.Zero;

                                                   return lastGrade.GetTrainingTime();
                                               });
                    classesArray = classes.ToArray();
                    timesArray = times.ToArray();
                    Array.Sort(timesArray, classesArray);
                    classes = classesArray;
                    labels = timesArray.Select(x => x.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas));
                    return "Time to Elite";
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the image's index for the provided certificate class,
        /// lazily creating the images when they're needed.
        /// </summary>
        /// <param name="certClass"></param>
        /// <returns></returns>
        private int GetCertImageIndex(IEnumerable<Certificate> certClass)
        {
            // Prepare datas, especially image keys like "BSIE", "BSie", "BE", etc (lower for non-granted, upper for granted, only existing certs)
            // Correction : keys are insenstive, so we use 1234 instead of lower case letters
            char[] chars = new char[4];
            bool[] granted = new bool[4];
            List<Certificate> certs = new List<Certificate>(certClass);

            int index = 0;
            int totalGranted = 0;
            foreach (Certificate cert in certs)
            {
                bool isGranted = (cert.Status == CertificateStatus.Granted);
                if (isGranted)
                {
                    totalGranted++;
                    granted[index] = true;
                    chars[index] = m_upperCertificatesLetters[(int)cert.Grade]; // Gets "B" for granted basic
                }
                else
                {
                    chars[index] = m_lowerCertificatesLetters[(int)cert.Grade]; // Gets "b" for non-granted basic
                }

                index++;
            }

            // Special cases where we return immediately
            if (totalGranted == certs.Count)
                return tvItems.ImageList.Images.IndexOfKey("AllGranted");

            // Create key and retrieves its index, then returns if it already exists
            string key = new string(chars);
            index = tvItems.ImageList.Images.IndexOfKey(key);
            if (index != -1)
                return index;

            // Create the image if it does not exist yet
            const int ImageSize = 24;
            const int MaxLetterWidth = 6;
            Bitmap bmp = new Bitmap(ImageSize, ImageSize, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                string[] letters = new string[4];
                float[] xPositions = new float[4];
                float x = 0.0f,
                      height = 0.0f;

                // Scroll through letters and measure them
                for (int i = 0; i < certs.Count; i++)
                {
                    letters[i] = m_upperCertificatesLetters[(int)certs[i].Grade].ToString();
                    SizeF size = g.MeasureString(letters[i], m_iconsFont, MaxLetterWidth, StringFormat.GenericTypographic);
                    height = Math.Max(height, size.Height);
                    xPositions[i] = x;
                    x += (size.Width + 1.0f);
                }

                // Y offset
                float y = Math.Max(0.0f, (ImageSize - height) * 0.5f);

                // Draw the letters
                g.Clear(Color.White);
                using (SolidBrush grantedBrush = new SolidBrush(Color.Blue))
                {
                    using (SolidBrush nonGrantedBrush = new SolidBrush(Color.Gray))
                    {
                        for (int i = 0; i < certs.Count; i++)
                        {
                            // Special color for granted, gray for the other ones
                            bool isGranted = granted[i];
                            SolidBrush brush = (isGranted ? grantedBrush : nonGrantedBrush);
                            g.DrawString(letters[i], m_iconsFont, brush, xPositions[i], y);
                        }
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
        /// When the tree's context menu opens,
        /// we update the submenus' statuses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmListSkills_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = tvItems.SelectedNode;
            CertificateClass certClass = SelectedCertificateClass;
            if (certClass == null || m_plan.WillGrantEligibilityFor(certClass.HighestGradeCertificate))
            {
                cmiLvPlanTo.Enabled = false;
                cmiLvPlanTo.Text = "Plan to...";
            }
            else
            {
                cmiLvPlanTo.Enabled = true;
                cmiLvPlanTo.Text = String.Format(CultureConstants.DefaultCulture, "Plan \"{0}\" to...", certClass.Name);
                SetAdditionMenuStatus(tsmLevel1, certClass[CertificateGrade.Basic]);
                SetAdditionMenuStatus(tsmLevel2, certClass[CertificateGrade.Standard]);
                SetAdditionMenuStatus(tsmLevel3, certClass[CertificateGrade.Improved]);
                SetAdditionMenuStatus(tsmLevel4, certClass[CertificateGrade.Elite]);
            }

            tsSeparatorPlanTo.Visible = (certClass == null && node != null);

            // "Expand" and "Collapse" selected menu
            tsmExpandSelected.Visible = (certClass == null && node != null && !node.IsExpanded);
            tsmCollapseSelected.Visible = (certClass == null && node != null && node.IsExpanded);

            tsmExpandSelected.Text = (certClass == null && node != null &&
                                      !node.IsExpanded
                                          ? String.Format("Expand \"{0}\"", node.Text)
                                          : String.Empty);
            tsmCollapseSelected.Text = (certClass == null && node != null &&
                                        node.IsExpanded
                                            ? String.Format("Collapse \"{0}\"", node.Text)
                                            : String.Empty);

            // "Expand All" and "Collapse All" menu
            tsmCollapseAll.Enabled = tsmCollapseAll.Visible = m_allExpanded;
            tsmExpandAll.Enabled = tsmExpandAll.Visible = !tsmCollapseAll.Enabled;
        }

        /// <summary>
        /// Sets the visible status of the context menu submenu.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="cert"></param>
        private void SetAdditionMenuStatus(ToolStripMenuItem menu, Certificate cert)
        {
            if (cert == null)
            {
                menu.Visible = false;
                return;
            }

            menu.Visible = true;
            menu.Enabled = !m_plan.WillGrantEligibilityFor(cert);
        }

        /// <summary>
        /// Context menu > Plan to > Basic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmLevel1_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(SelectedCertificateClass[CertificateGrade.Basic]);
            PlanHelper.SelectPerform(operation);
        }

        /// <summary>
        /// Context menu > Plan to > Standard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmLevel2_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(SelectedCertificateClass[CertificateGrade.Standard]);
            PlanHelper.SelectPerform(operation);
        }

        /// <summary>
        /// Context menu > Plan to > Improved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmLevel3_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(SelectedCertificateClass[CertificateGrade.Improved]);
            PlanHelper.SelectPerform(operation);
        }

        /// <summary>
        /// Context menu > Plan to > Elite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmLevel4_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(SelectedCertificateClass[CertificateGrade.Elite]);
            PlanHelper.SelectPerform(operation);
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