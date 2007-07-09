using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml.Serialization;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class PlanSelectWindow : EVEMonForm
    {
        public PlanSelectWindow()
        {
            InitializeComponent();
        }

        public PlanSelectWindow(Settings s, CharacterInfo gci)
            : this()
        {
            m_settings = s;
            m_characterInfo = gci;
            m_charKey = m_characterInfo.Name;
        }

        private Settings m_settings;
        private CharacterInfo m_characterInfo;
        private string m_charKey;
        private bool m_planOrderChanged = false;

        public string CharKey
        {
            get { return m_charKey; }
            set { m_charKey = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (m_planOrderChanged)
            {
                SavePlanOrder();
            }
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SavePlanOrder()
        {
            List<string> newOrder = new List<string>();
            for (int i = 0; i < lbPlanList.Items.Count; i++)
            {
                newOrder.Add((string)lbPlanList.Items[i].Text);
            }
            m_settings.RearrangePlansFor(m_charKey, newOrder);

        }

        private void PlanSelectWindow_Load(object sender, EventArgs e)
        {
            PopulatePlanList(true);
            m_columnSorter = new PlanListSorter(this,m_characterInfo);
            lbPlanList.ListViewItemSorter = null;
        }

        private Dictionary<Plan, TimeSpan> planCache = null;

        private void PopulatePlanList(bool calculateTime)
        {
            lbPlanList.BeginUpdate();
            try
            {
                lbPlanList.Items.Clear();

                if (planCache == null || calculateTime)
                {
                    calculateTime = true;
                    planCache = new Dictionary<Plan, TimeSpan>();
                }
                foreach (string PlanName in m_settings.GetPlansForCharacter(m_charKey))
                {
                    Plan tmpPlan = m_settings.GetPlanByName(m_charKey, m_characterInfo, PlanName);
                    TimeSpan tsPlan = FindPlanTimeSpan(tmpPlan, calculateTime);
                    ListViewItem lvi = new ListViewItem(PlanName);
                    lvi.Text = PlanName;
                    lvi.SubItems.Add(Skill.TimeSpanToDescriptiveText(tsPlan,
                                                                            DescriptiveTextOptions.FullText |
                                                                            DescriptiveTextOptions.IncludeCommas |
                                                                            DescriptiveTextOptions.SpaceText));
                    lvi.SubItems.Add(tmpPlan.UniqueSkillCount.ToString());
                    lbPlanList.Items.Add(lvi);
                }
            }
            finally
            {
                btnOpen.Enabled = (lbPlanList.SelectedItems.Count > 0);
                lbPlanList.EndUpdate();
            }
        }

        private TimeSpan FindPlanTimeSpan(Plan plan, bool ignoreCache)
        {
            if (planCache == null)
            {
                ignoreCache = true;
                planCache = new Dictionary<Plan, TimeSpan>();
            }

            if (plan.GrandCharacterInfo == null)
            {
                plan.GrandCharacterInfo = m_characterInfo;
            }
            TimeSpan tsPlan;
            if (!ignoreCache && planCache.ContainsKey(plan))
            {
                tsPlan = planCache[plan];
            }
            else
            {
                tsPlan = plan.GetTotalTime(null);
                planCache.Add(plan, tsPlan);
            }
            return tsPlan;
        }

        private void lbPlanList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we have 1 and only 1 plan selected we can move it
            if (lbPlanList.SelectedItems.Count == 1)
            {
                int idx = lbPlanList.SelectedIndices[0];
                tsbMoveUp.Enabled = (idx > 0);
                tsbMoveDown.Enabled = (idx < lbPlanList.Items.Count - 1);
            }
            else
            {
                tsbMoveUp.Enabled = false;
                tsbMoveDown.Enabled = false;
            }

            // The Open button in enabled if there is at least 1 selected item
            // and the Open button is called "Merge" if more than 1 plan is selected.
            btnOpen.Enabled = (lbPlanList.SelectedItems.Count > 0);
            btnOpen.Text = (lbPlanList.SelectedItems.Count > 1 ? "Merge" : "Open");
        }


        private Plan m_result;

        public Plan ResultPlan
        {
            get { return m_result; }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (lbPlanList.SelectedItems.Count > 1)
            {
                m_result = new Plan();

                foreach (ListViewItem plan in lbPlanList.SelectedItems)
                {
                    string s = (string)plan.Text;
                    Plan p = m_settings.GetPlanByName(m_charKey, m_characterInfo, s);
                    foreach (Plan.Entry entry in p.Entries)
                    {
                        Plan.Entry entryInMergedPlan = m_result.GetEntry(entry.SkillName, entry.Level);
                        if (m_result.GetEntry(entry.SkillName, entry.Level) == null)
                        {
                            Plan.Entry entryClone = (Plan.Entry)entry.Clone();
                            entryClone.PlanGroups.Add(p.Name);
                            m_result.Entries.Add(entryClone);
                        }
                        else
                        {
                            entryInMergedPlan.PlanGroups.Add(p.Name);
                            foreach (string subPlanName in entry.PlanGroups)
                            {
                                if (!entryInMergedPlan.PlanGroups.Contains(subPlanName))
                                {
                                    entryInMergedPlan.PlanGroups.Add(subPlanName);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                string s = (string)lbPlanList.SelectedItems[0].Text;
                m_result = m_settings.GetPlanByName(m_charKey, m_characterInfo, s);
            }
            if (m_planOrderChanged)
            {
                SavePlanOrder();
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lbPlanList_DoubleClick(object sender, EventArgs e)
        {
            if (lbPlanList.SelectedItems.Count > 0)
            {
                btnOpen_Click(this, new EventArgs());
            }
        }

        private void LoadPlan()
        {
            DialogResult dr = ofdOpenDialog.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            try
            {
                Plan loadedPlan = null;
                using (Stream s = new FileStream(ofdOpenDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Plan));
                    try
                    {
                        loadedPlan = (Plan)xs.Deserialize(s);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex, true);

                        s.Seek(0, SeekOrigin.Begin);
                        using (GZipStream gzs = new GZipStream(s, CompressionMode.Decompress))
                        {
                            try
                            {
                                loadedPlan = (Plan)xs.Deserialize(gzs);
                            }
                            catch (Exception e)
                            {
                                ExceptionHandler.LogException(e, true);
                                throw new ApplicationException("Could not determine input file format.");
                            }
                        }
                    }
                }

                using (NewPlanWindow npw = new NewPlanWindow())
                {
                    string oldPlanName = "";
                    string newPlanName = "";
                    while (newPlanName == "")
                    {
                        npw.Text = "Load Plan";
                        string fileName = Path.GetFileNameWithoutExtension(ofdOpenDialog.FileName);
                        if(fileName.StartsWith(m_characterInfo.Name + " - "))
                            fileName = fileName.Substring((m_characterInfo.Name + " - ").Length);
                        npw.PlanName = fileName;
                        DialogResult xdr = npw.ShowDialog();
                        if (xdr == DialogResult.Cancel)
                        {
                            return;
                        }
                        string PlanName = npw.Result;

                        Plan oldPlan = m_settings.GetPlanByName(m_charKey, m_characterInfo,PlanName);
                        if (oldPlan == null)
                        {
                            // No plan of the same name, so no replacement necessary
                            oldPlanName = "";
                            newPlanName = PlanName;
                        }
                        else
                        {
                            // Should we try replacing the original plan?
                            string message = "Plan with name '" + PlanName + "' already exists. Replace plan '" +
                                             PlanName + "'?";
                            string caption = "Replace Plan '" + PlanName + "'";
                            DialogResult result;

                            // Display the MessageBox.
                            result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                // Rename the original plan for removal once we've loaded the new one
                                oldPlanName = PlanName + "_BACKUP";
                                m_settings.RenamePlanFor(m_charKey, PlanName, oldPlanName);
                                newPlanName = PlanName;
                            }
                            else
                            {
                                // User does not want to replace original plan, so get them to choose a new name
                                oldPlanName = "";
                                newPlanName = "";
                            }
                        }
                    }

                    // Now we have a valid name for the new plan, and potentially an old plan to be removed.
                    try
                    {
                        m_settings.AddPlanFor(m_charKey, loadedPlan, newPlanName);

                    }
                    catch (ApplicationException err)
                    {
                        ExceptionHandler.LogException(err, true);
                        // Rename the old plan to it's original name
                        if (oldPlanName != "")
                        {
                            m_settings.RenamePlanFor(m_charKey, oldPlanName, newPlanName);
                        }

                        MessageBox.Show("Could not add the plan:\n" + err.Message,
                                        "Could Not Add Plan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // We have successfully loaded the plan so remove the old one for good if needed
                    if (oldPlanName != "")
                    {
                        m_settings.RemovePlanFor(m_charKey, oldPlanName);
                    }
                }
            }
            catch (Exception err)
            {
                ExceptionHandler.LogException(err, true);
                MessageBox.Show("There was an error loading the saved plan:\n" + err.Message,
                                "Could Not Load Plan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PopulatePlanList(true);
        }


        private void miLoadPlanFromFile_Click(object sender, EventArgs e)
        {
            LoadPlan();
        }

        private void miLoadPlanFromCharacter_Click(object sender, EventArgs e)
        {
            using (CrossPlanSelect cps = new CrossPlanSelect(m_characterInfo.Name))
            {
                DialogResult dr = cps.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    String planName = cps.SelectedPlan;
                    String charKey = cps.SelectedCharKey;
                    String charName = cps.SelectedCharName;
                    String newPlanName;
                    using (NewPlanWindow f = new NewPlanWindow())
                    {
                        f.Text = "Save Plan As";
                        f.PlanName = charName + "-" + planName;
                        dr = f.ShowDialog();
                        if (dr == DialogResult.Cancel)
                        {
                            return;
                        }
                        newPlanName = f.Result;
                    }
                    // So we have the character and plan, and the new plan name...
                    // Get the plan from the other character, and add it here.
                    try
                    {
                        Plan otherPlan = m_settings.GetPlanByName(charKey, m_characterInfo, planName);
                        Plan newPlan = otherPlan.CopyTo(m_characterInfo);
                        newPlan.Name = newPlanName;
                        m_settings.AddPlanFor(m_charKey, newPlan, newPlanName);
                    }
                    catch (ApplicationException err)
                    {
                        // More than likely because the plan exits already...
                        ExceptionHandler.LogException(err, true);
                        MessageBox.Show("Could not add the plan:\n" + err.Message,
                                        "Could Not Add Plan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            // This line is magical!!
            // By telling PopulatePlanList to recalculate plan times, it kicks off a check of the plan
            // which results in removing any duplicate skills and addition of prerequisites
            // - so we end up with the correct time and # of skills. Just Magic!!
            PopulatePlanList(true);
        }

        private void miRename_Click(object sender, EventArgs e)
        {
            using (NewPlanWindow f = new NewPlanWindow())
            {
                string oldName = (string)lbPlanList.SelectedItems[0].Text;
                f.Text = "Rename Plan";
                f.PlanName = oldName;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                string newName = f.Result;
                if (oldName == newName)
                {
                    return;
                }
                if (m_settings.GetPlanByName(m_charKey, m_characterInfo, newName) != null)
                {
                    MessageBox.Show("A plan with that name already exists.",
                                    "Duplicate Plan Name", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                m_settings.RenamePlanFor(m_charKey,
                                         oldName, f.Result);
                lbPlanList.SelectedItems[0].Text = newName;
            }
        }

        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            m_planOrderChanged = true;
            int idx = lbPlanList.SelectedIndices[0];
            // deselect
            lbPlanList.SelectedItems[0].Selected = false;

            // move
            ListViewItem lvi = lbPlanList.Items[idx];
            lbPlanList.Items.RemoveAt(idx);
            lbPlanList.Items.Insert(idx - 1, lvi);

            // reselect
            lbPlanList.Items[idx - 1].Selected = true;
        }

        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            m_planOrderChanged = true;
            int idx = lbPlanList.SelectedIndices[0];
            // deselect
            lbPlanList.SelectedItems[0].Selected = false;

            // move
            ListViewItem lvi = lbPlanList.Items[idx];
            lbPlanList.Items.RemoveAt(idx);
            lbPlanList.Items.Insert(idx + 1, lvi);

            // reselect
            lbPlanList.Items[idx + 1].Selected = true;
        }

        private void miDelete_Click(object sender, EventArgs e)
        {
            string planName;
            string title = "Delete Plan";
            if (lbPlanList.SelectedItems.Count > 1)
            {
                planName = "the selected plans";
                title += "s";
            }
            else
            {
                planName = "\"" + lbPlanList.SelectedItems[0].Text + "\"";
            }

            DialogResult dr = MessageBox.Show("Are you sure you want to delete " + planName +
                                              "?", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes)
            {
                return;
            }
            foreach (ListViewItem lvi in lbPlanList.SelectedItems)
            {
                planName = lvi.Text;
                m_settings.RemovePlanFor(m_charKey, planName);
                lbPlanList.Items.RemoveAt(lbPlanList.Items.IndexOf(lvi));
            }
            lbPlanList.SelectedItems.Clear();
        }

        #region Dragging
        private void lbPlanList_ListViewItemsDragging(object sender, ListViewDragEventArgs e)
        {
            m_planOrderChanged = true;
        }
        #endregion

        #region Column Sorting

        private PlanListSorter m_columnSorter;

        public class PlanListSorter : IComparer
        {

            public PlanListSorter(PlanSelectWindow psw,CharacterInfo ci)
            {
                OrderOfSort = SortOrder.None;
                SortColumn = 0;
                m_planSelectWindow = psw;
                m_characterInfo = ci;
            }

            private PlanSelectWindow m_planSelectWindow;
            private int m_sortColumn;
            private CharacterInfo m_characterInfo;

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

                switch (m_sortColumn)
                {
                    case 0: // sort by name
                        compareResult = String.Compare(a.Text, b.Text);
                        break;
                    case 1: // Time

                        Settings s = Settings.GetInstance();
                        Plan p = s.GetPlanByName(m_planSelectWindow.m_charKey, m_characterInfo, a.Text);
                        TimeSpan t1 = m_planSelectWindow.FindPlanTimeSpan(p, false);
                        p = s.GetPlanByName(m_planSelectWindow.m_charKey,m_characterInfo, b.Text);
                        TimeSpan t2 = m_planSelectWindow.FindPlanTimeSpan(p, false);
                        compareResult = TimeSpan.Compare(t1, t2);
                        if (compareResult == 0)
                            compareResult = String.Compare(a.Text, b.Text);
                        break;
                    case 2:  //# skills
                        int s1 = Int32.Parse(a.SubItems[2].Text);
                        int s2 = Int32.Parse(b.SubItems[2].Text);
                        compareResult = s1 - s2;
                        if (compareResult == 0)
                            compareResult = String.Compare(a.Text, b.Text);
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

        private void lbPlanList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            m_planOrderChanged = true;

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
            lbPlanList.ListViewItemSorter = m_columnSorter;
            Cursor = Cursors.WaitCursor;
            lbPlanList.Sort();
            Cursor = Cursors.Default;
            // Now Disable the sort so users can still drag items manually
            lbPlanList.ListViewItemSorter = null;

        }

        #endregion

        #region Context Menu

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lbPlanList.SelectedItems.Count > 1)
            {
                cmiDelete.Enabled = true;
                cmiRename.Enabled = false;
                cmiOpen.Text = "Merge";
            }
            else if (lbPlanList.SelectedItems.Count == 1)
            {
                cmiDelete.Enabled = true;
                cmiRename.Enabled = true;
                cmiOpen.Enabled = true;
                cmiOpen.Text = "Open";
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion

        private void miNewPlan_Click(object sender, EventArgs e)
        {
            m_result = null;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void mFile_DropDownOpening(object sender, EventArgs e)
        {
            // See if we have multiple characters to determine if load from character is enabled
            miLoadPlanFromCharacter.Enabled = (m_settings.CharacterList.Count + m_settings.CharFileList.Count) > 1;
        }

        private void mEdit_DropDownOpening(object sender, EventArgs e)
        {
            miRename.Enabled = lbPlanList.SelectedItems.Count == 1;
            miDelete.Enabled = lbPlanList.SelectedItems.Count >= 1;
        }

    }
}
