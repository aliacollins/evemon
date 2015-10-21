using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Resources.Skill_Select;

namespace EVEMon.SkillPlanner
{
    public partial class SkillSelectControl : UserControl
    {
        public event EventHandler<EventArgs> SelectedSkillChanged;

        private bool m_allExpanded;
        private Character m_character;
        private Skill m_selectedSkill;
        private Plan m_plan;
        private bool m_init;


        #region Lifecycle

        /// <summary>
        /// Constructor
        /// </summary>
        public SkillSelectControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            Disposed -= OnDisposed;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
        }

        /// <summary>
        /// On load, restore settings and update the content
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Call the base method
            base.OnLoad(e);

            // Create the attributes combinations and add them to the combo box
            // (This complex LINQ expression ensures the automatic catch of all present attributes combinations
            // and the ones that CCP may introduce in the future)
            cbFilterBy.Items.AddRange(EnumExtensions.GetValues<EveAttribute>().OrderBy(
                x => x.ToString()).SelectMany(primaryAttribute => m_character.Skills.Where(
                    x => x.PrimaryAttribute == primaryAttribute).Select(x => x.SecondaryAttribute).Distinct().OrderBy(
                        x => x.ToString()).Select(secondaryAttribute =>
                                                  String.Format(CultureConstants.InvariantCulture, "{0} - {1}",
                                                                primaryAttribute, secondaryAttribute))).ToArray<object>());

            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;

            if (Settings.UI.UseStoredSearchFilters)
            {
                cbShowNonPublic.Checked = Settings.UI.SkillBrowser.ShowNonPublicSkills;
                cbSkillFilter.SelectedIndex = (int)Settings.UI.SkillBrowser.Filter;
                cbFilterBy.SelectedIndex = Settings.UI.SkillBrowser.FilterByAttributesIndex;
                cbSorting.SelectedIndex = (int)Settings.UI.SkillBrowser.Sort;

                tbSearchText.Text = Settings.UI.SkillBrowser.TextSearch;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
            }
            else
            {
                cbShowNonPublic.Checked = false;
                cbSkillFilter.SelectedIndex = 0;
                cbFilterBy.SelectedIndex = 0;
                cbSorting.SelectedIndex = 0;
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


        #region Public Properties

        /// <summary>
        /// Gets or sets the plan.
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
        /// Gets the selected skill.
        /// </summary>
        [Browsable(false)]
        public Skill SelectedSkill
        {
            get { return m_selectedSkill; }
            set
            {
                if (m_selectedSkill == value)
                    return;

                m_selectedSkill = value;

                // Expands the tree view
                tvItems.SelectNodeWithTag(value);
                
                // Notify subscribers
                if (SelectedSkillChanged != null)
                    SelectedSkillChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets or sets true whether the control is hosted in the skill browser.
        /// When true, the "Show in skill browser" context menus won't be displayed.
        /// </summary>
        [Category("Behavior"),
         Description("When true, the \"Show in Skill Browser\" context menus won't be displayed."),
         DefaultValue(false)]
        public bool HostedInSkillBrowser { get; set; }

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


        #region Icon set

        /// <summary>
        /// Gets the icon set for the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ImageList GetIconSet(int index)
        {
            return GetIconSet(index, ilSkillIcons);
        }

        /// <summary>
        /// Gets the icon set for the given index, using the given list for missing icons.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="defaultList"></param>
        /// <returns></returns>
        private static ImageList GetIconSet(int index, ImageList defaultList)
        {
            string groupname = String.Empty;

            if (index > 0 && index < IconSettings.Default.Properties.Count)
            {
                SettingsProperty settingsProperty = IconSettings.Default.Properties["Group" + index];
                if (settingsProperty != null)
                    groupname = settingsProperty.DefaultValue.ToString();
            }

            if ((!String.IsNullOrEmpty(groupname) && !File.Exists(String.Format(CultureConstants.InvariantCulture,
                                                                 "{1}Resources{0}Skill_Select{0}Group{2}{0}{3}.resources",
                                                                 Path.DirectorySeparatorChar,
                                                                 AppDomain.CurrentDomain.BaseDirectory,
                                                                 index,
                                                                 groupname)) ||
                 !File.Exists(String.Format(CultureConstants.InvariantCulture,
                                            "{1}Resources{0}Skill_Select{0}Group0{0}Default.resources",
                                            Path.DirectorySeparatorChar,
                                            AppDomain.CurrentDomain.BaseDirectory))))
                groupname = String.Empty;

            return String.IsNullOrEmpty(groupname) ? defaultList : GetIconSet(index, groupname);
        }

        /// <summary>
        /// Gets the icon set for the given index, using the given list for missing icons.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="groupname">The groupname.</param>
        /// <returns></returns>
        private static ImageList GetIconSet(int index, string groupname)
        {
            ImageList imageList;
            ImageList tempImageList = null;
            try
            {
                tempImageList = new ImageList();
                IDictionaryEnumerator basicx;
                IResourceReader defaultGroupReader = null;
                tempImageList.ColorDepth = ColorDepth.Depth32Bit;

                try
                {
                    defaultGroupReader = new ResourceReader(String.Format(CultureConstants.InvariantCulture,
                                                                          "{1}Resources{0}Skill_Select{0}Group0{0}Default.resources",
                                                                          Path.DirectorySeparatorChar,
                                                                          AppDomain.CurrentDomain.BaseDirectory));

                    basicx = defaultGroupReader.GetEnumerator();

                    while (basicx.MoveNext())
                    {
                        tempImageList.Images.Add(basicx.Key.ToString(), (Icon)basicx.Value);
                    }
                }
                finally
                {
                    if (defaultGroupReader != null)
                        defaultGroupReader.Close();
                }

                IResourceReader groupReader = null;
                try
                {
                    groupReader = new ResourceReader(String.Format(CultureConstants.InvariantCulture,
                                                                   "{1}Resources{0}Skill_Select{0}Group{2}{0}{3}.resources",
                                                                   Path.DirectorySeparatorChar,
                                                                   AppDomain.CurrentDomain.BaseDirectory,
                                                                   index,
                                                                   groupname));

                    basicx = groupReader.GetEnumerator();

                    while (basicx.MoveNext())
                    {
                        if (tempImageList.Images.ContainsKey(basicx.Key.ToString()))
                            tempImageList.Images.RemoveByKey(basicx.Key.ToString());

                        tempImageList.Images.Add(basicx.Key.ToString(), (Icon)basicx.Value);
                    }
                }
                finally
                {
                    if (groupReader != null)
                        groupReader.Close();
                }

                imageList = tempImageList;
                tempImageList = null;
            }
            finally
            {
                if (tempImageList != null)
                    tempImageList.Dispose();
            }

            return imageList;
        }

        #endregion


        #region Content creation and update

        /// <summary>
        /// Updates the skills list content.
        /// </summary>
        internal void UpdateContent()
        {
            if (m_plan == null)
                return;

            IEnumerable<Skill> skills = GetFilteredData();

            tvItems.Visible = false;
            lbSearchList.Visible = false;
            lvSortedSkillList.Visible = false;
            lbNoMatches.Visible = false;

            // Nothing to display ?
            if (!skills.Any())
            {
                lbNoMatches.Visible = true;
                SelectedSkill = null;
            }
                // Is it sorted ?
            else if (cbSorting.SelectedIndex > 0)
            {
                lvSortedSkillList.Visible = true;
                UpdateListView(skills);
            }
                // Not sorted but there is a text filter
            else if (!String.IsNullOrEmpty(tbSearchText.Text))
            {
                lbSearchList.Visible = true;
                UpdateListBox(skills);
            }
                // Regular display, the tree
            else
            {
                tvItems.Visible = true;
                UpdateTree(skills);
            }
        }

        /// <summary>
        /// Gets the filtered skills.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Skill> GetFilteredData()
        {
            IEnumerable<Skill> skills = m_character.Skills;

            // Non-public skills
            if (!cbShowNonPublic.Checked)
                skills = skills.Where(x => x.IsPublic);

            // Filter
            Func<Skill, bool> predicate = GetFilter();
            skills = skills.Where(predicate);

            // Text search
            if (!String.IsNullOrEmpty(tbSearchText.Text))
            {
                string searchText = tbSearchText.Text.ToLower(CultureConstants.DefaultCulture).Trim();
                skills = skills.Where(x => (x.Name.ToLower(CultureConstants.DefaultCulture).Contains(searchText)
                                            || x.Description.ToLower(CultureConstants.DefaultCulture).Contains(searchText)));
            }

            // When sorting by "time to...", remove lv5 skills
            if (cbSorting.SelectedIndex == (int)SkillSort.TimeToLevel5 ||
                cbSorting.SelectedIndex == (int)SkillSort.TimeToNextLevel)
                skills = skills.Where(x => x.Level < 5);

            return skills;
        }

        /// <summary>
        /// Gets the skill predicate for the selected combo filter index.
        /// </summary>
        /// <returns></returns>
        private Func<Skill, bool> GetFilter()
        {
            lblFilterBy.Enabled = cbFilterBy.Enabled = (SkillFilter)cbSkillFilter.SelectedIndex == SkillFilter.ByAttributes;

            EveAttribute primary = EveAttribute.None;
            EveAttribute secondary = EveAttribute.None;
            if (cbFilterBy.Enabled)
            {
                string[] attributes = cbFilterBy.SelectedItem.ToString().Split('-');
                primary = (EveAttribute)Enum.Parse(typeof(EveAttribute), attributes.First().Trim());
                secondary = (EveAttribute)Enum.Parse(typeof(EveAttribute), attributes.Last().Trim());
            }

            switch ((SkillFilter)cbSkillFilter.SelectedIndex)
            {
                case SkillFilter.None:
                case SkillFilter.All:
                    return x => true;
                case SkillFilter.ByAttributes:
                    return x => x.PrimaryAttribute == primary && x.SecondaryAttribute == secondary;
                case SkillFilter.Known:
                    return x => x.IsKnown;
                case SkillFilter.TrailAccountFriendly:
                    return x => x.IsTrainableOnTrialAccount;
                case SkillFilter.Unknown:
                    return x => !x.IsKnown;
                case SkillFilter.UnknownAndNotOwned:
                    return x => !x.IsKnown && !x.IsOwned;
                case SkillFilter.UnknownButOwned:
                    return x => !x.IsKnown && x.IsOwned;
                case SkillFilter.UnknownButTrainable:
                    return x => !x.IsKnown && x.ArePrerequisitesMet;
                case SkillFilter.UnknownAndNotTrainable:
                    return x => !x.IsKnown && !x.ArePrerequisitesMet;
                case SkillFilter.Planned:
                    return x => m_plan.IsPlanned(x);
                case SkillFilter.Lv1Ready:
                    return x => x.Level == 0 && x.ArePrerequisitesMet && !x.IsPartiallyTrained;
                case SkillFilter.Trainable:
                    return x => x.ArePrerequisitesMet && x.Level < 5;
                case SkillFilter.PartiallyTrained:
                    return x => x.IsPartiallyTrained;
                case SkillFilter.NotPlanned:
                    return x => !(m_plan.IsPlanned(x) || x.Level == 5);
                case SkillFilter.NotPlannedButTrainable:
                    return x => !m_plan.IsPlanned(x) && x.ArePrerequisitesMet && x.Level < 5;
                case SkillFilter.NoLv5:
                    return x => x.Level < 5;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Updates the skills tree.
        /// </summary>
        private void UpdateTree(IEnumerable<Skill> skills)
        {
            // Store the selected node (if any) to restore it after the update
            int selectedItemHash = (tvItems.SelectedNodes.Count > 0
                                        ? tvItems.SelectedNodes[0].Tag.GetHashCode()
                                        : 0);

            // Update the image list choice
            int iconGroupIndex = Settings.UI.SkillBrowser.IconsGroupIndex;
            if (iconGroupIndex == 0)
                iconGroupIndex = 1;

            tvItems.ImageList = GetIconSet(iconGroupIndex);

            // Rebuild the nodes
            int numberOfItems = 0;
            tvItems.BeginUpdate();
            try
            {
                tvItems.Nodes.Clear();

                numberOfItems = AddNodes(skills, numberOfItems);

                TreeNode selectedNode = null;

                // Restore the selected node (if any)
                if (selectedItemHash > 0)
                {
                    foreach (TreeNode node in tvItems.GetAllNodes().Where(
                        node => node.Tag.GetHashCode() == selectedItemHash))
                    {
                        tvItems.SelectNodeWithTag(node.Tag);
                        selectedNode = node;
                    }
                }

                // Reset if the node doesn't exist anymore
                if (selectedNode == null)
                    tvItems.SelectNodeWithTag(null);
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
        /// Adds the nodes.
        /// </summary>
        /// <param name="skills">The skills.</param>
        /// <param name="numberOfItems">The number of items.</param>
        /// <returns></returns>
        private int AddNodes(IEnumerable<Skill> skills, int numberOfItems)
        {
            foreach (IGrouping<SkillGroup, Skill> group in skills.GroupBy(x => x.Group).OrderBy(x => x.Key.Name))
            {
                int index = (!Settings.UI.SafeForWork ? tvItems.ImageList.Images.IndexOfKey("book") : 0);

                TreeNode groupNode = new TreeNode
                                         {
                                             Text = group.Key.Name,
                                             ImageIndex = index,
                                             SelectedImageIndex = index,
                                             Tag = group.Key
                                         };

                // Add nodes for skills in this group
                foreach (Skill skill in group)
                {
                    // Choose image index
                    int imageIndex;
                    if (skill.Level != 0)
                        imageIndex = tvItems.ImageList.Images.IndexOfKey("lvl" + skill.Level);
                    else if (skill.IsKnown)
                        imageIndex = tvItems.ImageList.Images.IndexOfKey("lvl0");
                    else if (skill.IsOwned)
                        imageIndex = tvItems.ImageList.Images.IndexOfKey("book");
                    else if (skill.ArePrerequisitesMet)
                        imageIndex = tvItems.ImageList.Images.IndexOfKey("PrereqsMet");
                    else
                        imageIndex = tvItems.ImageList.Images.IndexOfKey("PrereqsNOTMet");

                    // Create node and adds it
                    TreeNode node = new TreeNode
                                        {
                                            Text = String.Format(CultureConstants.DefaultCulture, "{0} ({1})",
                                                                 skill.Name, skill.Rank),
                                            ImageIndex = imageIndex,
                                            SelectedImageIndex = imageIndex,
                                            Tag = skill
                                        };
                    groupNode.Nodes.Add(node);

                    // We color some nodes
                    if (!skill.IsPublic && Settings.UI.SkillBrowser.ShowNonPublicSkills)
                        node.ForeColor = Color.DarkRed;

                    if (skill.IsPartiallyTrained && Settings.UI.PlanWindow.HighlightPartialSkills)
                        node.ForeColor = Color.Green;

                    if (skill.IsQueued && !skill.IsTraining && Settings.UI.PlanWindow.HighlightQueuedSkills)
                        node.ForeColor = Color.RoyalBlue;

                    if (skill.IsTraining)
                    {
                        node.BackColor = Color.LightSteelBlue;
                        node.ForeColor = Color.Black;
                    }

                    numberOfItems++;
                }

                // Add group when not empty
                tvItems.Nodes.Add(groupNode);
            }
            return numberOfItems;
        }

        /// <summary>
        /// Updates the list box displayed when there is a text filter and no sort criteria.
        /// </summary>
        /// <param name="skills"></param>
        private void UpdateListBox(IEnumerable<Skill> skills)
        {
            lbSearchList.BeginUpdate();
            try
            {
                lbSearchList.Items.Clear();
                foreach (Skill skill in skills)
                {
                    lbSearchList.Items.Add(skill);
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
        /// <param name="skills"></param>
        private void UpdateListView(IEnumerable<Skill> skills)
        {
            // Store the selected node (if any) to restore it after the update
            int selectedItemHash = (tvItems.SelectedNodes.Count > 0
                                        ? tvItems.SelectedNodes[0].Tag.GetHashCode()
                                        : 0);

            // Retrieve the data to fetch into the list
            IEnumerable<string> labels = null;
            string column = GetSortedListData(ref skills, ref labels);
            if (labels == null)
                return;

            // Update the listview
            lvSortedSkillList.BeginUpdate();
            try
            {
                lvSortedSkillList.Items.Clear();

                using (IEnumerator<string> labelsEnumerator = labels.GetEnumerator())
                {
                    foreach (Skill skill in skills)
                    {
                        // Retrieves the label for the second column (sort key)
                        labelsEnumerator.MoveNext();
                        string label = labelsEnumerator.Current;

                        // Creates the item and adds it
                        ListViewItem lvi = new ListViewItem(skill.Name);
                        lvi.SubItems.Add(label);
                        lvi.Tag = skill;

                        lvSortedSkillList.Items.Add(lvi);
                    }
                }

                ListViewItem selectedItem = null;

                // Restore the selected node (if any)
                if (selectedItemHash > 0)
                {
                    foreach (ListViewItem lvItem in lvSortedSkillList.Items.Cast<ListViewItem>().Where(
                        lvItem => lvItem.Tag.GetHashCode() == selectedItemHash))
                    {
                        lvItem.Selected = true;
                        selectedItem = lvItem;
                    }
                }

                // Reset if the node doesn't exist anymore
                if (selectedItem == null)
                    tvItems.SelectNodeWithTag(null);

                // Auto adjust column widths
                chSortKey.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                chName.Width = lvSortedSkillList.ClientSize.Width - chSortKey.Width;
                chSortKey.Text = column;
            }
            finally
            {
                lvSortedSkillList.EndUpdate();
            }
        }

        /// <summary>
        /// Gets the data for the sorted list view.
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        private string GetSortedListData(ref IEnumerable<Skill> skills, ref IEnumerable<string> labels)
        {
            switch ((SkillSort)cbSorting.SelectedIndex)
            {
                    // Sort by name, default, occurs on initialization
                default:
                    return String.Empty;

                    // Time to next level
                case SkillSort.TimeToNextLevel:
                    IEnumerable<TimeSpan> times = skills.Select(
                        x => m_character.GetTrainingTimeToMultipleSkills(x.Prerequisites).Add(x.GetLeftTrainingTimeToNextLevel));

                    TimeSpan[] timesArray = times.ToArray();
                    Skill[] skillsArray = skills.ToArray();
                    Array.Sort(timesArray, skillsArray);

                    string[] labelsArray = new string[skillsArray.Length];
                    for (int i = 0; i < labelsArray.Length; i++)
                    {
                        TimeSpan time = timesArray[i];
                        if (time == TimeSpan.Zero)
                            labelsArray[i] = "-";
                        else
                        {
                            labelsArray[i] = String.Format(CultureConstants.DefaultCulture, "{0}: {1}",
                                                           Skill.GetRomanFromInt(skillsArray[i].Level + 1),
                                                           time.ToDescriptiveText(DescriptiveTextOptions.None));
                        }
                    }

                    skills = skillsArray;
                    labels = labelsArray;

                    return "Time";

                    // Time to level 5
                case SkillSort.TimeToLevel5:
                    times = skills.Select(
                        x => m_character.GetTrainingTimeToMultipleSkills(x.Prerequisites).Add(x.GetLeftTrainingTimeToLevel(5)));

                    timesArray = times.ToArray();
                    skillsArray = skills.ToArray();
                    Array.Sort(timesArray, skillsArray);

                    labelsArray = new string[skillsArray.Length];
                    for (int i = 0; i < labelsArray.Length; i++)
                    {
                        TimeSpan time = timesArray[i];
                        if (time == TimeSpan.Zero)
                            labelsArray[i] = "-";
                        else
                            labelsArray[i] = time.ToDescriptiveText(DescriptiveTextOptions.None);
                    }

                    skills = skillsArray;
                    labels = labelsArray;

                    return "Time to V";

                    // Skill rank
                case SkillSort.Rank:
                    skills = skills.ToArray().OrderBy(x => x.Rank);
                    labels = skills.Select(x => x.Rank.ToString(CultureConstants.DefaultCulture));
                    return "Rank";

                    // Skill SP/hour
                case SkillSort.SPPerHour:
                    skills = skills.ToArray().OrderBy(x => x.SkillPointsPerHour).Reverse();
                    labels = skills.Select(x => x.SkillPointsPerHour.ToString(CultureConstants.DefaultCulture));
                    return "SP/hour";
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// When the user click the text search hint, he actually wants to select the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSearchTextHint_Click(object sender, EventArgs e)
        {
            tbSearchText.Focus();
        }

        /// <summary>
        /// When the user begins to write in the text box, we don't want to bother him with the text search hint.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_Enter(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = false;
        }

        /// <summary>
        /// When the user leaves the text box, we display the text search hint if the box is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_Leave(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
        }

        /// <summary>
        /// When the search text box changes, we update the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            Settings.UI.SkillBrowser.TextSearch = tbSearchText.Text;
            
            if (m_init)
                UpdateContent();
        }

        /// <summary>
        /// When the selection of the listbox changes, updates the control's selection and fires the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedSkill = lbSearchList.SelectedIndex >= 0
                                ? lbSearchList.Items[lbSearchList.SelectedIndex] as Skill
                                : null;
        }

        /// <summary>
        /// When the selection of the tree changes, updates the control's selection and fires the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvSkillList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode treeNode = e.Node;
            if (treeNode == null)
                return;

            SelectedSkill = treeNode.Tag as Skill;
        }

        /// <summary>
        /// When the selection of the listview changes, updates the control's selection and fires the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSortedSkillList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedSkill = lvSortedSkillList.SelectedItems.Count > 0
                                ? lvSortedSkillList.SelectedItems[0].Tag as Skill
                                : null;
        }

        /// <summary>
        /// When the "show non public" checkbox is checked, we update settings and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbShowNonPublic_CheckedChanged(object sender, EventArgs e)
        {
            Settings.UI.SkillBrowser.ShowNonPublicSkills = cbShowNonPublic.Checked;
            UpdateContent();
        }

        /// <summary>
        /// When the sorting combo box changes, we update settings and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSorting_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.UI.SkillBrowser.Sort = (SkillSort)cbSorting.SelectedIndex;
            
            if (m_init)
                UpdateContent();
        }

        /// <summary>
        /// When the filter by attributes combo box changes, we update settings and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.UI.SkillBrowser.FilterByAttributesIndex = cbFilterBy.SelectedIndex;
            
            if (m_init)
                UpdateContent();
        }

        /// <summary>
        /// When the filter combo box changes, we update the settings and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSkillFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.UI.SkillBrowser.Filter = (SkillFilter)cbSkillFilter.SelectedIndex;
            
            if (m_init)
                UpdateContent();
        }

        /// <summary>
        /// When the user clicks the search text box, we select the whole text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (int)Keys.LButton)
                return;

            tbSearchText.SelectAll();
            e.Handled = true;
        }

        /// <summary>
        /// Changes the selection when you right click on a skill node.
        /// </summary>
        /// <remarks>
        /// This fixes an issue with XP (and possibly Vista) where
        /// right click does not select the list item the mouse is
        /// over. in Windows 7 (Beta) this is default behaviour and
        /// this event has no effect.
        /// </remarks>
        /// <param name="sender">is tvItems</param>
        /// <param name="e">is a member of tvItems.Items</param>
        private void tvItems_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                tvItems.SelectedNode = e.Node;
        }

        /// <summary>
        /// Changes the selection when you right click on a search.
        /// </summary>
        /// <remarks>
        /// This fixes an issue with XP (and possibly Vista) where
        /// right click does not select the list item the mouse is
        /// over. in Windows 7 (Beta) this is default behaviour and
        /// this event has no effect.
        /// </remarks>
        /// <param name="sender">is lbSearchList</param>
        /// <param name="e"></param>
        private void lbSearchList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                lbSearchList.SelectedIndex = lbSearchList.IndexFromPoint(e.Location);
        }

        /// <summary>
        /// When the user begins dragging a skill from the treeview, we start a drag'n drop operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = tvItems.SelectedNode;
            if (node == null)
                return;

            if (node.Nodes.Count != 0)
                return;

            Skill skill = node.Tag as Skill;
            if (skill == null || m_plan.GetPlannedLevel(skill) == 5 || skill.Level == 5)
                return;

            DoDragDrop(node, DragDropEffects.Move);
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


        #region Context menu

        /// <summary>
        /// When the tree's context menu opens, we update the submenus' statuses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmSkills_Opening(object sender, CancelEventArgs e)
        {
            Skill skill = null;
            TreeNode node = tvItems.SelectedNode;
            if (node != null)
                skill = tvItems.SelectedNode.Tag as Skill;

            if (SelectedSkill == null && skill != null)
                node = null;

            tsSeparatorBrowser.Visible = (node != null);

            // "Show in skill browser/explorer"
            showInSkillExplorerMenu.Visible = (SelectedSkill != null);
            showInSkillBrowserMenu.Visible = (SelectedSkill != null && !HostedInSkillBrowser);

            // "Collapse" and "Expand" menus
            cmiCollapseSelected.Visible = (SelectedSkill == null && node != null && node.IsExpanded);
            cmiExpandSelected.Visible = (SelectedSkill == null && node != null && !node.IsExpanded);

            cmiExpandSelected.Text = (SelectedSkill == null && node != null &&
                                      !node.IsExpanded
                                          ? String.Format(CultureConstants.DefaultCulture, "Expand \"{0}\"", node.Text)
                                          : String.Empty);
            cmiCollapseSelected.Text = (SelectedSkill == null && node != null &&
                                        node.IsExpanded
                                            ? String.Format(CultureConstants.DefaultCulture, "Collapse \"{0}\"", node.Text)
                                            : String.Empty);

            // "Expand All" and "Collapse All" menus
            cmiCollapseAll.Enabled = cmiCollapseAll.Visible = m_allExpanded;
            cmiExpandAll.Enabled = cmiExpandAll.Visible = !cmiCollapseAll.Enabled;

            // "Plan to N" menus
            cmiPlanTo.Enabled = (SelectedSkill != null && SelectedSkill.Level < 5);
            if (SelectedSkill == null)
                return;

            for (int i = 0; i <= 5; i++)
            {
                PlanHelper.UpdatesRegularPlanToMenu(cmiPlanTo.DropDownItems[i], m_plan, SelectedSkill, i);
            }
        }

        /// <summary>
        /// When the list's context menu opens, we update the menus status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmListSkills_Opening(object sender, CancelEventArgs e)
        {
            if (SelectedSkill == null)
            {
                e.Cancel = true;
                return;
            }

            // "Plan to N" menus
            for (int i = 0; i <= 5; i++)
            {
                PlanHelper.UpdatesRegularPlanToMenu(cmiLvPlanTo.DropDownItems[i], m_plan, SelectedSkill, i);
            }

            // "Show in skill browser"
            showInSkillBrowserListMenu.Visible = !HostedInSkillBrowser;
        }

        /// <summary>
        /// Listview, listbox, and tree view contexts menu > Plan to > Plan to N.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Treeview's context menu > Collapse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Collapse();
        }

        /// <summary>
        /// Treeview's context menu > Expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Expand();
        }

        /// <summary>
        /// Treeview's context menu > Expand all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandAll_Click(object sender, EventArgs e)
        {
            tvItems.ExpandAll();
            m_allExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseAll_Click(object sender, EventArgs e)
        {
            tvItems.CollapseAll();
            m_allExpanded = false;
        }

        /// <summary>
        /// Context menu > Show In Skill Browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillBrowserMenu_Click(object sender, EventArgs e)
        {
            // Retrieve the owner window
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (planWindow == null || planWindow.IsDisposed)
                return;

            // Open the skill explorer
            planWindow.ShowSkillInBrowser(SelectedSkill);
        }

        /// <summary>
        /// Context menu > Show In Skill Explorer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillExplorerMenu_Click(object sender, EventArgs e)
        {
            // Retrieve the owner window
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (planWindow == null || planWindow.IsDisposed)
                return;

            // Open the skill explorer
            planWindow.ShowSkillInExplorer(SelectedSkill);
        }

        #endregion
    }
}