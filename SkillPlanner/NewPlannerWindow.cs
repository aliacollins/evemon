using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class NewPlannerWindow : EVEMonForm
    {
        public NewPlannerWindow()
        {
            InitializeComponent();
        }

        public NewPlannerWindow(Settings s, Plan p)
            : this()
        {
            m_settings = s;
            m_plan = p;
            m_plan.PlannerWindow = new WeakReference<Form>(this);
            m_planKey = p.GrandCharacterInfo.Name;

            // see if this character is file based (for select plan button)
            foreach (CharFileInfo cfi in m_settings.CharFileList)
            {
                if (cfi.CharacterName.Equals(p.GrandCharacterInfo.Name))
                {
                    m_cfi = cfi;
                    m_planKey = cfi.Filename;
                }
            }

            skillBrowser.Plan = m_plan;
            // Shouldn't need this
            skillBrowser.GrandCharacterInfo = m_plan.GrandCharacterInfo;

            planEditor.Plan = m_plan;
            // Shouldn't need this
            planEditor.GrandCharacterInfo = m_plan.GrandCharacterInfo;
            planEditor.PlannerWindow = this;

            shipBrowser.Plan = m_plan;

            itemBrowser.Plan = m_plan;
            PopulateTsPlans();

            // See if this is a new plan
            if (m_plan.Entries.Count == 0)
            {
                // jump straight to the skill browser
                tabControl.SelectedTab = tpSkillBrowser;
            }
            else
            {
                // Jump to the plan queue
                tabControl.SelectedTab = tpPlanQueue;
            }

            m_settings.WorksafeChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.HighlightPlannedSkillsChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.HighlightPrerequisitesChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.HighlightConflictsChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.DimUntrainableChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);

            // Watch for changes to worksafe settings and plan changes
            m_plan.Changed += new EventHandler<EventArgs>(m_plan_Changed);

            // Force an update
            m_settings_WorksafeChanged(null, null);
            m_settings_SkillHighlightingChanged(null, null);
        }

        void tsddiTemp_MouseEnter(object sender, EventArgs e)
        {
            if (((ToolStripDropDownItem)sender).BackColor == SystemColors.Highlight)
            {
                ((ToolStripDropDownItem)sender).ForeColor = SystemColors.MenuText;
            }
        }

        void tsddiTemp_MouseLeave(object sender, EventArgs e)
        {
            if (((ToolStripDropDownItem)sender).BackColor == SystemColors.Highlight)
            {
                ((ToolStripDropDownItem)sender).ForeColor = SystemColors.HighlightText;
            }
        }

        private void m_settings_SkillHighlightingChanged(object sender, EventArgs e)
        {
            planEditor.HighlightPlannedSkills = m_settings.SkillPlannerHighlightPlannedSkills;
            planEditor.HighlightPrerequisites = m_settings.SkillPlannerHighlightPrerequisites;
            planEditor.DimUntrainable = m_settings.SkillPlannerDimUntrainable;
            planEditor.HighlightConflicts = m_settings.SkillPlannerHighlightConflicts;
        }

        public void ShowSkillInTree(Skill gs)
        {
            skillBrowser.SelectedSkill = gs;
            tabControl.SelectedTab = tpSkillBrowser;
        }

        public void ShowSkillInExplorer(Skill gs)
        {
            skillBrowser.ShowSkillInExplorer(gs);
        }

        public void ShowShipInBrowser(Ship s)
        {
            shipBrowser.SelectedShip = s;
            tabControl.SelectedTab = tpShipBrowser;
        }

        public void ShowItemInBrowser(Item i)
        {
            itemBrowser.SelectedItem = i;
            tabControl.SelectedTab = tpItemBrowser;
        }

        private void NewPlannerWindow_Shown(object sender, EventArgs e)
        {
            m_showing = true;
        }

        private bool m_showing = false;

        private void NewPlannerWindow_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            // Set the title
            this.Text = m_plan.GrandCharacterInfo.Name + " [" + m_plan.Name + "] - EVEMon Skill Planner";
            this.RememberPositionKey = "SkillPlannerWindow";

            // Show the hint tip
            TipWindow.ShowTip("planner",
                              "Welcome to the Skill Planner",
                              "Select skills to add to your plan using the list on the left. To " +
                              "view the list of skills you've added to your plan, choose " +
                              "\"View Plan\" from the dropdown in the upper left.");
            UpdateStatusBar();

            if (tabControl.SelectedIndex == 0)
            {
                // Force update of column widths in case we've just created a new plan
                // from within the planner window.
                planEditor.UpdateListColumns();
            }
        }

        private void NewPlannerWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Remove event handlers
            m_plan.Changed -= new EventHandler<EventArgs>(m_plan_Changed);
            m_settings.WorksafeChanged -= new EventHandler<EventArgs>(m_settings_WorksafeChanged);
        }

        Settings m_settings;

        private CharFileInfo m_cfi = null;
        private String m_planKey;

        public Settings Settings
        {
            get { return m_settings; }
            set
            {
                if (m_settings != null)
                {
                    m_settings.WorksafeChanged -= new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.HighlightPlannedSkillsChanged -= new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.HighlightPrerequisitesChanged -= new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.DimUntrainableChanged -= new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.HighlightConflictsChanged -= new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                }
                m_settings = value;
                if (m_settings != null)
                {
                    m_settings.WorksafeChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.HighlightPlannedSkillsChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.HighlightPrerequisitesChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.DimUntrainableChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                    m_settings.HighlightConflictsChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
                }
            }
        }

        private void m_settings_WorksafeChanged(object sender, EventArgs e)
        {
            skillBrowser.WorksafeMode = m_settings.WorksafeMode;
            planEditor.WorksafeMode = m_settings.WorksafeMode;
        }

        private Plan m_plan;
        private void m_plan_Changed(object sender, EventArgs e)
        {
            // Update the skill explorer as it might impact that window
            if (m_skillExplorer != null)
            {
                m_skillExplorer.PlanChanged();
            }
            m_settings.Save();
            UpdateStatusBar();
        }

        private bool m_suggestionTipUp = false;

        public void UpdateStatusBarSelected(String txt)
        {
            slblStatusText.Text = txt;
        }

        public void UpdateStatusBar()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateStatusBar));
                return;
            }

            TimeSpan res = m_plan.GetTotalTime(null);
            slblStatusText.Text = String.Format("{0} Skill{1} Planned ({2} Unique Skill{3}). Total training time: {4}",
                                                m_plan.Entries.Count,
                                                m_plan.Entries.Count == 1 ? "" : "s",
                                                m_plan.UniqueSkillCount,
                                                m_plan.UniqueSkillCount == 1 ? "" : "s",
                                                Skill.TimeSpanToDescriptiveText(res,
                                                                                     DescriptiveTextOptions.Default |
                                                                                     DescriptiveTextOptions.IncludeCommas |
                                                                                     DescriptiveTextOptions.SpaceText),
                                                 m_plan.TrainingCost);
            long cost = m_plan.TrainingCost;
            if (cost > 0)
            {
                slblStatusText.Text += String.Format(" Cost: {0:0,0,0} ISK", cost);
            }

            if (m_plan.HasAttributeSuggestion)
            {
                tslSuggestion.Visible = true;
                if (m_showing && !m_suggestionTipUp)
                {
                    m_suggestionTipUp = true;
                    TipWindow.ShowTip("suggestion",
                                      "Plan Suggestion",
                                      "EVEMon has analyzed your plan and has come up with a " +
                                      "suggestion of learning skills that you can add that will " +
                                      "lower the overall training time of the plan. To view this " +
                                      "suggestion and the resulting change in plan time, click the " +
                                      "\"Suggestion\" link in the planner status bar.");
                    m_suggestionTipUp = false;
                }
            }
            else
            {
                tslSuggestion.Visible = false;
            }
        }

        public SkillEnablesForm m_skillExplorer = null;

        public SkillEnablesForm SkillExplorer
        {
            get { return m_skillExplorer; }
            set { m_skillExplorer = value; }
        }

        private void ChangePlan(Plan p)
        {
            p.GrandCharacterInfo = m_plan.GrandCharacterInfo;
            p.PlannerWindow = new WeakReference<Form>(this);
            m_plan = p;

            skillBrowser.Plan = m_plan;
            // Shouldn't need this
            skillBrowser.GrandCharacterInfo = m_plan.GrandCharacterInfo;

            planEditor.Plan = m_plan;
            planEditor.PlannerWindow = this;

            shipBrowser.Plan = m_plan;

            itemBrowser.Plan = m_plan;

            // Tell ths skill explorer form we#re switching plans
            // this has to be done after we've told the skill browser about the plan!
            if (m_skillExplorer != null)
            {
                m_skillExplorer.PlanChanged();
            }

            // See if this is a new plan
            if (m_plan.Entries.Count == 0)
            {
                // jump straight to the skill browser
                tabControl.SelectedTab = tpSkillBrowser;
            }
            else
            {
                // Jump to the plan queue
                tabControl.SelectedTab = tpPlanQueue;
            }

            m_settings.WorksafeChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.HighlightPlannedSkillsChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.HighlightPrerequisitesChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.HighlightConflictsChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);
            m_settings.DimUntrainableChanged += new EventHandler<EventArgs>(m_settings_SkillHighlightingChanged);

            // Watch for changes to worksafe settings and plan changes
            m_plan.Changed += new EventHandler<EventArgs>(m_plan_Changed);

            // Force an update
            m_settings_WorksafeChanged(null, null);
            m_settings_SkillHighlightingChanged(null, null);

            this.Text = m_plan.GrandCharacterInfo.Name + " [" + m_plan.Name + "] - EVEMon Skill Planner";
            this.RememberPositionKey = "SkillPlannerWindow";
            PopulateTsPlans();
            UpdateStatusBar();
            planEditor.UpdateListColumns();
        }

        private void tsbDeletePlan_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "Are you sure you want to delete this plan?",
                "Delete Plan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes)
            {
                return;
            }
            // tell the skill planner that we're deleting the plan
            if (m_skillExplorer != null)
            {
                m_skillExplorer.Shutdown();
            }
            m_settings.RemovePlanFor(m_planKey, m_plan.Name);
        }

        private void tslSuggestion_Click(object sender, EventArgs e)
        {
            using (SuggestionWindow f = new SuggestionWindow(m_plan))
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }

            m_plan.SuppressEvents();
            try
            {
                IEnumerable<Plan.Entry> entries = m_plan.GetSuggestions();
                int i = 0;
                foreach (Plan.Entry pe in entries)
                {
                    // make learning skills top priority
                    pe.Priority = 1;
                    m_plan.Entries.Insert(i, pe);
                    i++;
                }

                //Ensures any existing learning skills in the plan get moved to
                //account for higher levels being inserted at the front...
                m_plan.CheckForMissingPrerequisites();

                // Arrange the learning skills in the plan in optimal order
                PlanSorter.SortPlan(m_plan, PlanSortType.NoChange, true, false);
            }
            finally
            {
                m_plan.ResumeEvents();
                m_plan.ResetSuggestions();
                UpdateStatusBar();
            }
        }

        private void tsddbPlans_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string s = e.ClickedItem.Text;
            Plan p = null;
            if (s != null && s != "<New Plan>" && s != m_plan.Name)
            {
                p = m_settings.GetPlanByName(m_planKey, s);
                ChangePlan(p);
            }
            else
            {
                if (s == "<New Plan>")
                {
                    bool doAgain = true;
                    while (doAgain)
                    {
                        using (NewPlanWindow npw = new NewPlanWindow())
                        {
                            DialogResult dr = npw.ShowDialog();
                            if (dr == DialogResult.Cancel)
                            {
                                return;
                            }
                            string planName = npw.Result;

                            if (p == null)
                            {
                                p = new Plan();
                            }
                            try
                            {
                                m_settings.AddPlanFor(m_planKey, p, planName);
                                p.PlannerWindow = new WeakReference<Form>(this);
                                doAgain = false;
                            }
                            catch (ApplicationException err)
                            {
                                ExceptionHandler.LogException(err, true);
                                DialogResult xdr = MessageBox.Show(err.Message, "Failed to Add Plan", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                if (xdr == DialogResult.Cancel)
                                {
                                    return;
                                }
                            }
                        }
                    }
                    ChangePlan(p);
                }
            }
        }

        #region Plan serialization
        private void tsbCopyForum_Click(object sender, EventArgs e)
        {
            PlanTextOptions pto = (PlanTextOptions)m_settings.DefaultCopyOptions.Clone();
            using (CopySaveOptionsWindow f = new CopySaveOptionsWindow(pto, m_plan, true))
            {
                if (pto.Markup == MarkupType.Undefined)
                {
                    pto.Markup = MarkupType.Forum;
                }
                f.ShowDialog();
                if (f.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                if (f.SetAsDefault)
                {
                    m_settings.DefaultCopyOptions = pto;
                    m_settings.Save();
                }
            }

            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            {
                m_plan.SaveAsText(sw, pto);
                sw.Flush();
                string s = Encoding.Default.GetString(ms.ToArray());
                Clipboard.SetText(s);
            }

            MessageBox.Show("The skill plan has been copied to the clipboard in a " +
                            "format suitable for forum posting.", "Plan Copied", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private enum PlanSaveType
        {
            None = 0,
            Emp = 1,
            Xml = 2,
            Text = 3
        }

        private enum ExportSaveType
        {
            None = 0,
            ShortXml = 1,
            LongXml = 2
        }


        private void tsbSaveAs_Click(object sender, EventArgs e)
        {
            sfdSave.Title = "Save to File";
            sfdSave.FileName = m_plan.GrandCharacterInfo.Name + " - " + m_plan.Name;
            sfdSave.Filter = "EVEMon Plan Format (*.emp)|*.emp|XML  Format (*.xml)|*.xml|Text Format (*.txt)|*.txt";
            sfdSave.FilterIndex = (int)PlanSaveType.Emp;
            DialogResult dr = sfdSave.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            string fileName = sfdSave.FileName;
            try
            {
                PlanTextOptions pto = null;
                if ((PlanSaveType)sfdSave.FilterIndex == PlanSaveType.Text)
                {
                    pto = (PlanTextOptions)m_settings.DefaultSaveOptions.Clone();
                    using (CopySaveOptionsWindow f = new CopySaveOptionsWindow(pto, m_plan, false))
                    {
                        if (pto.Markup == MarkupType.Undefined)
                        {
                            pto.Markup = MarkupType.None;
                        }
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.Cancel)
                        {
                            return;
                        }
                        if (f.SetAsDefault)
                        {
                            m_settings.DefaultSaveOptions = pto;
                            m_settings.Save();
                        }
                    }
                }

                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    switch ((PlanSaveType)sfdSave.FilterIndex)
                    {
                        case PlanSaveType.Emp:
                            using (GZipStream gzs = new GZipStream(fs, CompressionMode.Compress))
                            {
                                SerializePlanTo(gzs);
                            }
                            break;
                        case PlanSaveType.Xml:
                            SerializePlanTo(fs);
                            break;
                        case PlanSaveType.Text:
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                m_plan.SaveAsText(sw, pto);
                            }
                            break;
                        default:
                            return;
                    }
                }
            }
            catch (IOException err)
            {
                ExceptionHandler.LogException(err, true);
                MessageBox.Show("There was an error writing out the file:\n\n" + err.Message,
                                "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SerializePlanTo(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Plan));
            xs.Serialize(s, m_plan);
        }
        #endregion Plan serialization

        #region Implant Calculator
        private ImplantCalculator m_implantCalcWindow = null;

        private void tsbImplantCalculator_Click(object sender, EventArgs e)
        {
            if (m_implantCalcWindow != null)
            {
                m_implantCalcWindow.Visible = true;
                m_implantCalcWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
            }
            else
            {
                m_implantCalcWindow = new ImplantCalculator(m_plan.GrandCharacterInfo, m_plan);
                m_implantCalcWindow.Show();
                m_implantCalcWindow.Disposed += new EventHandler(m_implantCalcWindow_Disposed);
            }
            m_implantCalcWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
        }

        private void m_implantCalcWindow_Disposed(Object o, EventArgs e)
        {
            m_implantCalcWindow = null;
        }

        #endregion Implant Calculator

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            m_settings.PlannerTab = tabControl.SelectedIndex;
            if (m_implantCalcWindow != null && m_implantCalcWindow.Visible)
            {
                m_implantCalcWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
            }
            if (tabControl.SelectedIndex == 0)
            {
                // Force update of column widths in case we've just created a new plan
                // from within the planner window.
                planEditor.UpdateListColumns();
            }
        }


        private void PopulateTsPlans()
        {
            tsddbPlans.DropDownItems.Clear();
            tsddbPlans.DropDownItems.Add("<New Plan>");

            foreach (string planName in m_settings.GetPlansForCharacter(m_planKey))
            {
                try
                {
                    ToolStripDropDownItem tsddiTemp = (ToolStripDropDownItem)tsddbPlans.DropDownItems.Add(planName);
                    tsddiTemp.MouseEnter += new EventHandler(tsddiTemp_MouseEnter);
                    tsddiTemp.MouseLeave += new EventHandler(tsddiTemp_MouseLeave);
                    if (planName == m_plan.Name)
                    {
                        tsddiTemp.Font = new Font(tsddiTemp.Font, FontStyle.Bold);
                    }
                }
                catch (InvalidCastException)
                {
                    // for some reason, Visual studio cannot set the text of a TooStripDropDownItem to "-" (try it in designer view!!)
                }
            }
        }

        private void tsddbPlans_MouseDown(object sender, MouseEventArgs e)
        {
            PopulateTsPlans();
        }

        private void tsbExportToXml_Click(object sender, EventArgs e)
        {
            sfdSave.Title = "Export to XML";
            sfdSave.FileName = m_plan.GrandCharacterInfo.Name + " Planned Character Export";
            sfdSave.Filter = "XML Short Format (*.xml)|*.xml|XML Long Format (*.xml)|*.xml";
            sfdSave.FilterIndex = (int)ExportSaveType.ShortXml;
            DialogResult dr = sfdSave.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            string fileName = sfdSave.FileName;
            ExportSaveType saveType =  (ExportSaveType)sfdSave.FilterIndex;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                SerializableCharacterSheet ci = m_plan.GrandCharacterInfo.ExportSerializableCharacterSheet();
                m_plan.Merge(ci);
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    if (saveType == ExportSaveType.ShortXml)
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(SerializableCharacterSheet));
                        ser.Serialize(fs, ci);
                    }
                    else
                    {
                        SerializableCharacterInfo cfi = ci.CreateSerializableCharacterInfo();
                        XmlSerializer ser = new XmlSerializer(typeof(SerializableCharacterInfo));
                        ser.Serialize(fs, cfi);
                    }
                }
                this.Cursor = Cursors.Default;
            }
            catch (InvalidOperationException ioe)
            {
                ExceptionHandler.LogException(ioe, true);
                MessageBox.Show("There was an error writing out the file:\n\n" + ioe.Message,
                                "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException err)
            {
                ExceptionHandler.LogException(err, true);
                MessageBox.Show("There was an error writing out the file:\n\n" + err.Message,
                                "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NewPlannerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Tell the skill explorer we're closing down
            if (m_skillExplorer != null)
            {
                if (!(e.CloseReason == CloseReason.ApplicationExitCall) && // and Application.Exit() was not called
                    !(e.CloseReason == CloseReason.TaskManagerClosing) &&  // and the user isn't trying to shut the program down for some reason
                    !(e.CloseReason == CloseReason.WindowsShutDown))       // and Windows is not shutting down
                {
                    m_skillExplorer.Shutdown();
                }
            }

            // Tell the implant window we're closing
            if (m_implantCalcWindow != null)
            {
                m_implantCalcWindow.Close();
                m_implantCalcWindow = null;
            }
        }

        #region Print Plan

        WebBrowser printBrowser;
        private void tsbPrintPlan_Click(object sender, EventArgs e)
        {
            printBrowser = new WebBrowser();
            printBrowser.Width = 800;
            printBrowser.Height = 600;
            printBrowser.ScrollBarsEnabled = false;

            PlanTextOptions pto = (PlanTextOptions)m_settings.DefaultSaveOptions.Clone();
            pto.Markup = MarkupType.Html;
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            {
                m_plan.SaveAsText(sw, pto);
                sw.Flush();
                string s = Encoding.Default.GetString(ms.ToArray());
                printBrowser.DocumentText = s;
            }
            printBrowser.Update();
            printDocument1.DocumentName = this.Text;
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog(this);
            printBrowser.Dispose();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bmp = new Bitmap(printBrowser.ClientRectangle.Width, printBrowser.ClientRectangle.Height);
            printBrowser.DrawToBitmap(bmp, printBrowser.ClientRectangle);
            e.Graphics.DrawImage(bmp, 0, 0);

        }
        #endregion
    }

    public class PlannerWindowFactory : IPlannerWindowFactory
    {
        #region IPlannerWindowFactory Members
        public Form CreateWindow(Settings s, Plan p)
        {
            return new NewPlannerWindow(s, p);
        }
        #endregion
    }
}
