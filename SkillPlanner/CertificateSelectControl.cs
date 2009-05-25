using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class CertificateSelectControl : UserControl
    {
        private readonly char[] UpperCertificatesLetters = new char[] { 'B', 'S', 'I', 'E' };
        private readonly char[] LowerCertificatesLetters = new char[] { '1', '2', '3', '4' };   // Stupid insensitive images keys' comparison, we cannot use bsie

        private delegate bool Filter(CertificateClass certClass);

        private Plan m_plan;
        private Filter m_filter;
        private Settings m_settings;
        private CharacterInfo m_character;
        private CertificateClass m_selectedCertificateClass;
        private Font m_iconsFont;
        private bool m_blockSelectionReentrancy;

        public event EventHandler<EventArgs> SelectionChanged;


        /// <summary>
        /// Constructor
        /// </summary>
        public CertificateSelectControl()
        {
            InitializeComponent();

            this.tbSearchText.KeyPress += new KeyPressEventHandler(tbSearchText_KeyPress);
            this.tbSearchText.Enter += new EventHandler(tbSearchText_Enter);
            this.tbSearchText.Leave += new EventHandler(tbSearchText_Leave);
            this.lvSortedList.SelectedIndexChanged += new EventHandler(lvSortedList_SelectedIndexChanged);
            this.tvItems.NodeMouseClick += new TreeNodeMouseClickEventHandler(tvItems_NodeMouseClick);
            this.cmListSkills.Opening += new CancelEventHandler(cmListSkills_Opening);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (this.DesignMode) return;

            try
            {
                m_iconsFont = FontHelper.GetFont("Tahoma", 8.0f, FontStyle.Bold, GraphicsUnit.Pixel);

                // Creates the nodes representing the categories
                foreach (var category in StaticCertificates.Categories)
                {
                    TreeNode node = new TreeNode(category.Name, -1, -1);
                    node.Tag = category;

                    tvItems.Nodes.Add(node);
                }

                var settings = Settings.GetInstance();
                tbSearchText.Text = settings.CertificateBrowserTextSearch;
                cbSorting.SelectedIndex = settings.CertificateBrowserSort;
                cbFilter.SelectedIndex = settings.CertificateBrowserFilter;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);

                // Load the settings (update at this point only to avoid cascaded display updates when updating the controls)
                m_settings = settings;

                // Updates the display
                UpdateDisplay();
            }
            catch (Exception err)
            {
                // This occurs when we're in the designer. DesignMode doesn't get set
                // when the control is a subcontrol of a user control, so we should handle
                // this here :(
                ExceptionHandler.LogException(err, true);
                return;
            }
        }

        /// <summary>
        /// Gets or sets the character this control is bound to
        /// </summary>
        public CharacterInfo Character
        {
            get { return m_character; }
            set 
            { 
                m_character = value;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Gets or sets the current plan
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set { m_plan = value; }
        }

        /// <summary>
        /// Gets or sets the selected certificate class
        /// </summary>
        public CertificateClass SelectedCertificateClass
        {
            get { return m_selectedCertificateClass; }
            set
            {
                if (m_selectedCertificateClass != value)
                {
                    m_selectedCertificateClass = value;

                    // Updates the selection for the three controls
                    m_blockSelectionReentrancy = true;
                    try
                    {
                        TreeViewHelper.SelectNodeWithTag(tvItems, value, true);

                        lvSortedList.SelectedItems.Clear();
                        foreach (ListViewItem item in lvSortedList.Items)
                        {
                            if (item.Tag == value) item.Selected = true;
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
        }

        #region Control events
        /// <summary>
        /// When the combo filter changes, we need to refresh the display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_filter = null;
            UpdateSettingsForFilter(cbFilter.SelectedIndex);
            UpdateDisplay();
        }

        /// <summary>
        /// When the sort filter changes, we need to refresh the display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSorting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSettingsForSort(cbSorting.SelectedIndex);
            UpdateDisplay();
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
        /// When the search text box changes, we update the settings with this new filter and we update the display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            UpdateSettingsForTextSearch(tbSearchText.Text);
            m_filter = null;
            UpdateDisplay();
        }

        /// <summary>
        /// When the "left button" key is pressed, we select the whole text (???)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x01)
            {
                tbSearchText.SelectAll();
                e.Handled = true;
            }
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
        /// When the treeview's selection is changed, we update the selected index. Also used to force node selection on a right click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            UpdateSelection(e.Node.Tag);
            if (e.Button == MouseButtons.Right)
            {
                tvItems.SelectedNode = e.Node;
            }
        }
        #endregion


        #region Display update
        /// <summary>
        /// Updates the display
        /// </summary>
        private void UpdateDisplay()
        {
            // Not ready yet ? We leave
            if (m_character == null || m_settings == null) return;

            // Choose the display mode
            UpdateDisplayMode();

            // Creates the filtered and sorted items list
            String[] sortKeys = null;
            var classes = CreateFilteredSortedItemsList(out sortKeys);

            // Fetch to the controls
            // First case : display the list view (one column for item, one for the sort keys)
            if (lvSortedList.Visible)
            {
                lvSortedList.BeginUpdate();
                try
                {
                    lvSortedList.Items.Clear();
                    for (int i = 0; i < classes.Length; i++)
                    {
                        var item = new ListViewItem(classes[i].Name);
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, sortKeys[i]));
                        lvSortedList.Items.Add(item);
                        item.Tag = classes[i];
                    }

                    lvSortedList.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
                    chName.Width = Math.Max(0, Math.Max(lvSortedList.ClientSize.Width / 2, lvSortedList.ClientSize.Width - (chSortKey.Width + 16)));
                }
                finally
                {
                    lvSortedList.EndUpdate();
                }
            }
            // Second case : display a simple list box
            else if (lbSearchList.Visible)
            {
                lbSearchList.BeginUpdate();
                try
                {
                    lbSearchList.Items.Clear();
                    for (int i = 0; i < classes.Length; i++)
                    {
                        lbSearchList.Items.Add(classes[i]);
                    }
                }
                finally
                {
                    lbSearchList.EndUpdate();
                }
            }
            // Third case : display a tree
            else
            {
                // Fill the tree
                tvItems.BeginUpdate();
                try
                {
                    foreach (TreeNode node in tvItems.Nodes)
                    {
                        var category = (CertificateCategory)node.Tag;
                        node.ImageIndex = tvItems.ImageList.Images.IndexOfKey("Certificate");
                        node.SelectedImageIndex = node.ImageIndex;
                        node.Nodes.Clear();

                        foreach(var certClass in classes)
                        {
                            if (certClass.Category == category)
                            {
                                TreeNode child = new TreeNode(certClass.Name);
                                child.ImageIndex = GetCertImageIndex(certClass);
                                child.SelectedImageIndex = child.ImageIndex;
                                child.Tag = certClass;

                                node.Nodes.Add(child);
                            }
                        }
                    }
                }
                finally
                {
                    tvItems.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Gets the image's index for the provided certificate class, lazily creating the images when they're needed
        /// </summary>
        /// <param name="certClass"></param>
        /// <returns></returns>
        private int GetCertImageIndex(CertificateClass certClass)
        {
            // Prepare datas, especially image keys like "BSIE", "BSie", "BE", etc (lower for non-granted, upper for granted, only existing certs)
            // Correction : keys are insenstive, so we use 1234 instead of lower case letters
            char[] chars = new char[4];
            bool[] granted = new bool[4];
            List<Certificate> certs = new List<Certificate>(certClass.Certificates);

            int index = 0;
            int totalGranted = 0;
            foreach (var cert in certs) 
            {
                bool isGranted = (m_character.GetCertificateStatus(cert) == CertificateStatus.Granted);
                if (isGranted)
                {
                    totalGranted++;
                    granted[index] = true;
                    chars[index] = UpperCertificatesLetters[(int)cert.Grade];   // Gets "B" for granted basic
                }
                else
                {
                    chars[index] = LowerCertificatesLetters[(int)cert.Grade];  // Gets "b" for non-granted basic
                }

                index++;
            }

            // Special cases where we return immediately
            if (totalGranted == certs.Count) return tvItems.ImageList.Images.IndexOfKey("AllGranted");

            // Create key and retrieves its index, then returns if it already exists
            string key = new string(chars);
            index = tvItems.ImageList.Images.IndexOfKey(key);
            if (index != -1) return index;

            // Create the image if it does not exist yet
            const int ImageSize = 24;
            const int MaxLetterWidth = 6;
            Bitmap bmp = new Bitmap(ImageSize, ImageSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bmp))
            {
                string[] letters = new string[4];
                float[] xPositions = new float[4];
                float x = 0.0f, height = 0.0f;

                // Scroll through letters and measure them
                for(int i=0; i<certs.Count;i++)
                {
                    letters[i] = UpperCertificatesLetters[(int)certs[i].Grade].ToString();
                    var size = g.MeasureString(letters[i], m_iconsFont, MaxLetterWidth, StringFormat.GenericTypographic);
                    height = Math.Max(height, size.Height);
                    xPositions[i] = x;
                    x += (size.Width + 1.0f);
                }

                // Y offset
                float y = Math.Max(0.0f, (ImageSize - (float)height) * 0.5f);

                // Draw the letters
                g.Clear(Color.White);
                using(var grantedBrush = new SolidBrush(Color.Blue))
                {
                    using (var nonGrantedBrush = new SolidBrush(Color.Gray))
                    {
                        for(int i=0; i<certs.Count; i++)
                        {
                            // Special color for granted, gray for the other ones
                            bool isGranted = granted[i];
                            var brush = (isGranted ? grantedBrush : nonGrantedBrush);
                            g.DrawString(letters[i], m_iconsFont, brush, xPositions[i], y);
                        }
                    }
                }
            }

            // Insert image and return its index
            tvItems.ImageList.Images.Add(key, bmp);
            return tvItems.ImageList.Images.IndexOfKey(key);
        }

        /// <summary>
        /// Creates the filtered and sorted items list
        /// </summary>
        /// <param name="sortKeys"></param>
        /// <returns></returns>
        private CertificateClass[] CreateFilteredSortedItemsList(out string[] sortKeys)
        {
            // Updates the filter if out of date
            if (m_filter == null) UpdateFilter();


            // Creates the filtered items array
            List<CertificateClass> classesList = new List<CertificateClass>();
            foreach (var certClass in StaticCertificates.Classes)
            {
                if (m_filter(certClass)) classesList.Add(certClass);
            }

            var classes = classesList.ToArray();
            sortKeys = new string[classes.Length];



            // Sort the items array
            switch (cbSorting.SelectedIndex)
            {
                // Sort by name, default
                case -1:
                case 0:
                    break;

                // Sort by time to next grade
                case 1:
                    // Gather sort keys
                    TimeSpan[] times = new TimeSpan[classes.Length];
                    for (int i = 0; i < classes.Length; i++)
                    {
                        var nextUntrained = classes[i].GetLowestGradeUntrainedCertificate(m_character);
                        times[i] = (nextUntrained == null ? TimeSpan.Zero : nextUntrained.ComputeTrainingTime(m_character));
                    }

                    // Sort
                    Array.Sort(times, classes);

                    // Create sort keys representation
                    for (int i = 0; i < times.Length; i++)
                    {
                        sortKeys[i] = Skill.TimeSpanToDescriptiveText(times[i], DescriptiveTextOptions.IncludeCommas);
                    }
                    break;

                // Sort by time to last grade
                case 2:
                    // Gather sort keys
                    times = new TimeSpan[classes.Length];
                    for (int i = 0; i < classes.Length; i++)
                    {
                        var lastGrade = classes[i].HighestGradeCertificate;
                        var status = m_character.GetCertificateStatus(lastGrade);
                        if (status == CertificateStatus.Granted || status == CertificateStatus.Claimable) times[i] = TimeSpan.Zero;
                        else times[i] = lastGrade.ComputeTrainingTime(m_character);
                    }

                    // Sort
                    Array.Sort(times, classes);

                    // Create sort keys representation
                    for (int i = 0; i < times.Length; i++)
                    {
                        sortKeys[i] = Skill.TimeSpanToDescriptiveText(times[i], DescriptiveTextOptions.IncludeCommas);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            return classes;
        }

        /// <summary>
        /// Updates the items' filter 
        /// </summary>
        private void UpdateFilter()
        {
            // Update the base filter from the combo box
            Filter baseFilter = null;
            string sortKey = (cbFilter.SelectedItem == null ? "All" : cbFilter.SelectedItem.ToString());
            switch (sortKey)
            {
                case "All": // All certificates
                    baseFilter = (x) => true;
                    break;

                case "Hide elite": // Hide completed certificates
                    baseFilter = (x) => !x.HasBeenCompletedBy(m_character);
                    break;

                case "Next grade is trainable": // Can be trained further
                    baseFilter = (x) => x.IsFurtherTrainableBy(m_character);
                    break;

                case "Next grade is untrainable": // Cannot be trained further
                    baseFilter = (x) => !x.IsFurtherTrainableBy(m_character);
                    break;

                default:
                    throw new NotImplementedException();
            }
            
            // Combines the previous fitler with the text one
            if (String.IsNullOrEmpty(tbSearchText.Text))
            {
                m_filter = baseFilter;
            }
            else
            {
                var text = tbSearchText.Text.ToLower();
                m_filter = (x) => baseFilter(x) && x.Name.ToLower().Contains(text);
            }
        }

        /// <summary>
        /// Sets up the visibility of the different controls used to present tthe results (listbox, listview or treeview)
        /// </summary>
        private void UpdateDisplayMode()
        {
            if (cbSorting.SelectedIndex > 0)
            {
                if (!lvSortedList.Visible) UpdateSelection(null);

                lvSortedList.Visible = true;
                lbSearchList.Visible = false;
                tvItems.Visible = false;
            }
            else if (!String.IsNullOrEmpty(tbSearchText.Text))
            {
                if (!lbSearchList.Visible) UpdateSelection(null);

                lvSortedList.Visible = false;
                lbSearchList.Visible = true;
                tvItems.Visible = false;
            }
            else
            {
                if (!tvItems.Visible) UpdateSelection(null);

                lvSortedList.Visible = false;
                lbSearchList.Visible = false;
                tvItems.Visible = true;
            }
        }
        #endregion


        /// <summary>
        /// Called whenever the selection changes, fires the approriate event
        /// </summary>
        private void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Updates the selection by pickking the good control (the one visible)
        /// </summary>
        private void UpdateSelectionFromControls()
        {
            if (lvSortedList.Visible)
            {
                if (lvSortedList.SelectedItems.Count == 0) UpdateSelection(null);
                else UpdateSelection(lvSortedList.SelectedItems[0].Tag);
            }
            else if (lbSearchList.Visible)
            {
                if (lbSearchList.SelectedItems.Count == 0) UpdateSelection(null);
                else UpdateSelection(lbSearchList.SelectedItems[0]);
            }
            else
            {
                if (tvItems.SelectedNode == null) UpdateSelection(null);
                else UpdateSelection(tvItems.SelectedNode.Tag);
            }
        }

        /// <summary>
        /// Updates the selection with the provided item
        /// </summary>
        /// <param name="selection"></param>
        private void UpdateSelection(object selection)
        {
            if (!m_blockSelectionReentrancy)
            {
                this.SelectedCertificateClass = selection as CertificateClass;
            }
        }

        private void UpdateSettingsForTextSearch(string textSearch)
        {
            if (m_settings != null && m_settings.StoreBrowserFilters)
            {
                m_settings.CertificateBrowserTextSearch = textSearch;
            }
        }

        private void UpdateSettingsForFilter(int filterIndex)
        {
            if (m_settings != null && m_settings.StoreBrowserFilters)
            {
                m_settings.CertificateBrowserFilter = filterIndex;
            }
        }

        private void UpdateSettingsForSort(int sortIndex)
        {
            if (m_settings != null && m_settings.StoreBrowserFilters)
            {
                m_settings.CertificateBrowserSort = sortIndex;
            }
        }

        #region Context menus
        void cmListSkills_Opening(object sender, CancelEventArgs e)
        {
            var certClass = this.SelectedCertificateClass;
            if (certClass == null)
            {
                cmiLvPlanTo.Enabled = false;
                cmiLvPlanTo.Text = "Plan to...";
            }
            else
            {
                cmiLvPlanTo.Enabled = true;
                cmiLvPlanTo.Text = "Plan \"" + certClass.Name + "\" to...";
                SetAdditionMenuStatus(tsmLevel1, certClass, CertificateGrade.Basic);
                SetAdditionMenuStatus(tsmLevel2, certClass, CertificateGrade.Standard);
                SetAdditionMenuStatus(tsmLevel3, certClass, CertificateGrade.Improved);
                SetAdditionMenuStatus(tsmLevel4, certClass, CertificateGrade.Elite);
            }

            this.tsSeparator.Visible = this.tvItems.Visible;
            this.tsmExpandAll.Visible = this.tvItems.Visible;
            this.tsmCollapseAll.Visible = this.tvItems.Visible;
        }

        private void SetAdditionMenuStatus(ToolStripMenuItem menu, CertificateClass certClass, CertificateGrade grade)
        {
            var cert = certClass[grade];
            if (cert == null)
            {
                menu.Visible = false;
            }
            else
            {
                menu.Visible = true;
                var status = m_character.GetCertificateStatus(cert);
                menu.Enabled = !m_plan.WillGrantEligibilityFor(cert);
            }

        }

        private void tsmLevel1_Click(object sender, EventArgs e)
        {
            m_plan.PlanTo(this.SelectedCertificateClass[CertificateGrade.Basic], false);
        }

        private void tsmLevel2_Click(object sender, EventArgs e)
        {
            m_plan.PlanTo(this.SelectedCertificateClass[CertificateGrade.Standard], false);
        }

        private void tsmLevel3_Click(object sender, EventArgs e)
        {
            m_plan.PlanTo(this.SelectedCertificateClass[CertificateGrade.Improved], false);
        }

        private void tsmLevel4_Click(object sender, EventArgs e)
        {
            m_plan.PlanTo(this.SelectedCertificateClass[CertificateGrade.Elite], false);
        }

        private void tsmExpandAll_Click(object sender, EventArgs e)
        {
            this.tvItems.ExpandAll();
        }

        private void tsmCollapseAll_Click(object sender, EventArgs e)
        {
            this.tvItems.CollapseAll();
        }
        #endregion
    }
}
